using FluentDiagrams.Core;
using FluentDiagrams.Core.Filters;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;
using FluentDiagrams.Svg.Internal;
using LinqGarden;
using LinqGarden.Functions;

namespace FluentDiagrams.Svg
{
	public static class SvgFilterFactory
	{
		private class FilterChainState
		{
			public int LastGeneratedId { get; }

			/// <summary>
			/// TODO: replace with an "Either?"
			/// </summary>
			public int NextInputId { get; }

			private FilterChainState( int lastGeneratedId, int inputId )
			{
				LastGeneratedId = lastGeneratedId;
				NextInputId = inputId;
			}

			/// <summary>
			/// Create a state with initial input set to SourceGraphic
			/// </summary>
			/// <returns></returns>
			public static FilterChainState Initial() =>
				new FilterChainState( 0, 0 );

			public static State<FilterChainState, int> RegisterNewId { get; } =
				from state in State.Get<FilterChainState>()
				let newId = state.LastGeneratedId + 1
				from _ in State.Put( new FilterChainState( newId, newId ) )
				select newId;

			public static State<FilterChainState, Unit> SetInputId( int id ) =>
				from state in State.Get<FilterChainState>()
				from unit in State.Put( new FilterChainState( state.LastGeneratedId, inputId: id ) )
				select unit;

			public static State<FilterChainState, Unit> SetInputToSource( SourceType sourceType ) =>
				SetInputId( (int)sourceType );

			public static State<FilterChainState, int> GetInputId =>
				from chainState in State.Get<FilterChainState>()
				select chainState.NextInputId;

		}

		private static readonly State<FilterChainState, XAttribute> InputAttributeBuilder =
			from inputId in FilterChainState.GetInputId
			select new XAttribute( "in", BuildFilterIdString( inputId ) );

		private static readonly State<FilterChainState, XAttribute> Input2AttributeBuilder =
			from inputId in FilterChainState.GetInputId
			select new XAttribute( "in2", BuildFilterIdString( inputId ) );

		private static readonly State<FilterChainState, XAttribute> OutputAttributeBuilder =
			from id in FilterChainState.RegisterNewId
			select new XAttribute( "result", BuildFilterIdString( id ) );

		private static string BuildFilterIdString( int filterId ) =>
			filterId <= 0
			? ( (SourceType)filterId ).ToString()
			: $"filterElement{filterId}";


		internal static XElementBuilder BuildFilter( IFilterComponent component, CoordinatesConverter converter )
		{
			var firstAttempt = BuildFilterComponent( component, converter )
					.Func( FilterChainState.Initial() );
			var firstAttemptItems = firstAttempt.Value.ToList();

			List<XElement> filterChainList;

			if( firstAttemptItems.Any() == true )
			{
				filterChainList = firstAttemptItems;
			}
			else
			{
				var filterComponent = Filters.Noop;
				filterChainList = BuildFilterComponent( filterComponent, converter )
					.Func( firstAttempt.State ).Value.ToList();
			}
			return
			XElementBuilder.WithName( "filter" )
				.Add( filterChainList );
		}

		private static State<FilterChainState, IEnumerable<XElement>> BuildFilterComponent(
			IFilterComponent component,
			CoordinatesConverter converter )
		{
			if( component is ColorMatrix colorMatrix )
			{
				return BuildColorMatrix( colorMatrix ).Select( x => new[] { x }.AsEnumerable() );
			}
			if( component is GaussianBlurFilter gaussFilter )
			{
				return BuildGaussianBlur( gaussFilter, converter )
					.Select( x => new[] { x }.AsEnumerable() );
			}
			if( component is DropShadowFilter dropShadowFilter )
			{
				return BuildDropShadowFilter( dropShadowFilter, converter ).Select( x => new[] { x }.AsEnumerable() );
			}
			if( component is FilterChain chain )
			{
				return BuildFilterChain( chain, converter );
			}
			if( component is DisplacementFilter displacementFilter )
			{
				return BuildDisplacementFilter( displacementFilter, converter );
			}
			if( component is TurbulenceGenerator turbulenceGenerator )
			{
				return BuildTurbulenceGenerator( turbulenceGenerator ).Select( x => new[] { x }.AsEnumerable() );
			}
			if( component is FloodGenerator floodGenerator )
			{
				return BuildFloodGenerator( floodGenerator.Color ).Select( x => new[] { x.Build() }.AsEnumerable() );
			}
			if( component is BlendFilter blendFilter )
			{
				return BuildBlendFilter( blendFilter, converter );
			}
			if( component is FilterSource filterSource )
			{
				return
					from state in FilterChainState.SetInputToSource( filterSource.SourceType )
					select Enumerable.Empty<XElement>();
			}
			if( component is MergeFilter mergeFilter )
			{
				return BuildMergeFilter( mergeFilter, converter );
			}
			if( component is Morphology morphology )
			{
				return BuildMorphologyFilter( morphology, converter );
			}
			if( component is OffsetFilter offsetFilter )
			{
				return BuildOffsetFilter( offsetFilter, converter );
			}
			if( component is CompositeFilter compositeFilter )
			{
				return BuildCompositeFilter( compositeFilter, converter );
			}
			if( component is DiffuseLightingGenerator diffuseLightingGenerator )
			{
				return BuildDiffuseLightingGenerator( diffuseLightingGenerator, converter );
			}
			if( component is SpecularLightingGenerator specularLightingGenerator )
			{
				return BuildSpecularLightingGenerator( specularLightingGenerator, converter );
			}

			throw new FilterComponentNotSupportedException( component );
		}

		private static State<FilterChainState, IEnumerable<XElement>> BuildSpecularLightingGenerator( SpecularLightingGenerator specularLightingGenerator, CoordinatesConverter converter ) =>
			XElementBuilder.WithName( "feSpecularLighting" )
			.Add(
				new XAttribute( "surfaceScale", specularLightingGenerator.SurfaceScale ),
				new XAttribute( "specularConstant", specularLightingGenerator.SpecularConstant ),
				new XAttribute( "specularExponent", specularLightingGenerator.SpecularExponent ),
				new XAttribute( "lighting-color", SvgColorFactory.SvgColor( specularLightingGenerator.LightColor ) ) )
			.Add( BuildLightComponent( specularLightingGenerator.Light, converter ) )
			.Pipe( BuildOutputOnlyFilter )
			.Select( x => new[] { x.Build() }.AsEnumerable() );


		private static XElement BuildLightComponent( IFilterLight filterLight, CoordinatesConverter coordinatesConverter )
		{
			if( filterLight is Spotlight spotlight )
			{
				SvgCoordinate3d startPosition = spotlight.LightSource.Pipe( coordinatesConverter.ToSvgCoord );
				SvgCoordinate3d endPosition = spotlight.LightDestination.Pipe( coordinatesConverter.ToSvgCoord );

				return
				XElementBuilder.WithName( "feSpotLight" )
					.Add(
					new XAttribute( "x", startPosition.X ),
					new XAttribute( "y", startPosition.Y ),
					new XAttribute( "z", startPosition.Z ),
					new XAttribute( "pointsAtX", endPosition.X ),
					new XAttribute( "pointsAtY", endPosition.Y ),
					new XAttribute( "pointsAtZ", endPosition.Z ),
					new XAttribute( "specularExponent", spotlight.SpecularExponent ),
					new XAttribute( "limitingConeAngle", spotlight.ConeAngle.Degrees )
					)
					.Build();

			}
			if( filterLight is DistantLight distantLight )
			{
				return XElementBuilder.WithName( "feDistantLight" )
					.Add(
						new XAttribute( "azimuth", distantLight.Azimuth.Degrees ),
						new XAttribute( "elevation", distantLight.Elevation.Degrees ) )
					.Build();

			}
			if( filterLight is PointLight pointLight )
			{
				var coordinate = coordinatesConverter.ToSvgCoord( pointLight.Coordinate );
				return XElementBuilder.WithName( "fePointLight" ).Add(
					new XAttribute( "x", coordinate.X ),
					new XAttribute( "y", coordinate.Y ),
					new XAttribute( "z", coordinate.Z ) )
					.Build();
			}

			throw new Exception( $"unable to generate a light source from type '{filterLight}'" );
		}

		private static State<FilterChainState, IEnumerable<XElement>> BuildDiffuseLightingGenerator(
			DiffuseLightingGenerator diffuseLightingGenerator,
			CoordinatesConverter converter )
		{
			XElement lightComponent = diffuseLightingGenerator.Light.Pipe( x => BuildLightComponent( x, converter ) );

			var elementCore =
				XElementBuilder.WithName( "feDiffuseLighting" )
				.Add(
					new XAttribute( "lighting-color", SvgColorFactory.SvgColor( diffuseLightingGenerator.LightColor ) ),
					new XAttribute( "surfaceScale", diffuseLightingGenerator.SurfaceScale ),
					new XAttribute( "diffuseConstant", diffuseLightingGenerator.DiffuseConstant ) )
				.Add( lightComponent );

			return BuildIOFilter( elementCore ).Select( x => x.Build() ).Select( x => new[] { x }.AsEnumerable() );
		}

		private static State<FilterChainState, IEnumerable<XElement>> BuildCompositeFilter( CompositeFilter compositeFilter, CoordinatesConverter converter )
		{
			var elementCore =
				XElementBuilder.WithName( "feComposite" )
				.Add(
					new XAttribute( "operator", compositeFilter.Operation.ToString().ToLower() ),
					new XAttribute( "k1", compositeFilter.K1 ),
					new XAttribute( "k2", compositeFilter.K2 ),
					new XAttribute( "k3", compositeFilter.K3 ),
					new XAttribute( "k4", compositeFilter.K4 )
					);
			return BuildTwoInputFilter( elementCore, compositeFilter.SecondInput, converter );
		}

		private static State<FilterChainState, IEnumerable<XElement>> BuildOffsetFilter( OffsetFilter offsetFilter, CoordinatesConverter converter )
		{
			var svgVector = converter.ToSvgVector( offsetFilter.Vector );

			var core =
				XElementBuilder.WithName( "feOffset" )
				.Add(
					new XAttribute( "dx", svgVector.Dx ),
					new XAttribute( "dy", svgVector.Dy )
					);

			return BuildIOFilter( core ).Select( x => x.Build() ).Select( x => new[] { x }.AsEnumerable() );
		}

		private static State<FilterChainState, IEnumerable<XElement>> BuildMorphologyFilter( Morphology morphology, CoordinatesConverter converter )
		{
			var svgRadiusX = converter.ScaleDistance( morphology.RadiusX );
			var svgRadiusY = converter.ScaleDistance( morphology.RadiusY );

			var core =
				XElementBuilder.WithName( "feMorphology" )
				.Add(
					new XAttribute( "radius", $"{svgRadiusX} {svgRadiusY}" ),
					new XAttribute( "operator", morphology.MorphologyType.ToString() )
					);

			return BuildIOFilter( core ).Select( x => x.Build() ).Select( x => new[] { x }.AsEnumerable() );
		}

		private static State<FilterChainState, IEnumerable<XElement>> BuildMergeFilter( MergeFilter mergeFilter, CoordinatesConverter converter )
		{
			var states =
				mergeFilter.FilterComponents.Select( component => BuildInputAndMergeNode( component, converter ) );

			var concatenated = State.Concat( states );
			State<FilterChainState, (ImmutableList<XElement> FilterElements, ImmutableList<XElement> MergeNodes)> aggregatedState =
				from items in concatenated
				select
					items
					.Aggregate(
						(FilterElements: ImmutableList<XElement>.Empty,
						 MergeNodes: ImmutableList<XElement>.Empty),
						( acc, next ) =>
							(acc.FilterElements.AddRange( next.FilterInputs ),
							 acc.MergeNodes.Add( next.MergeNode )) );

			State<FilterChainState, IEnumerable<XElement>> finishedElements =
				from items in aggregatedState
				from mergeElement in BuildMergeElement( items.MergeNodes )
				select items.FilterElements.Concat( new[] { mergeElement } );

			return finishedElements;

		}

		private static State<FilterChainState, XElement> BuildMergeElement( IEnumerable<XElement> mergeNodes ) =>
			XElementBuilder.WithName( "feMerge" ).Add( mergeNodes )
			.Pipe( BuildOutputOnlyFilter )
			.Select( x => x.Build() );

		private static State<FilterChainState, (IEnumerable<XElement> FilterInputs, XElement MergeNode)> BuildInputAndMergeNode( IFilterComponent component, CoordinatesConverter converter ) =>
			from startingId in FilterChainState.GetInputId
			from filterComponent in BuildFilterComponent( component, converter )
			from endingId in FilterChainState.GetInputId
			from _ in FilterChainState.SetInputId( startingId )
			let mergeNode =
				XElementBuilder.WithName( "feMergeNode" ).Add( new XAttribute( "in", BuildFilterIdString( endingId ) ) ).Build()
			select (filterComponent, mergeNode);

		private static State<FilterChainState, IEnumerable<XElement>> BuildBlendFilter( BlendFilter blendFilter, CoordinatesConverter converter )
		{
			var builder =
				XElementBuilder.WithName( "feBlend" )
				.Add(
					new XAttribute( "mode", blendFilter.BlendModeSvgName ) );

			return BuildTwoInputFilter( builder, blendFilter.Source2, converter );
		}

		private static State<FilterChainState, XElement> BuildTurbulenceGenerator( TurbulenceGenerator turbulenceGenerator )
		{
			//TODO: extract logic out for build purely "generative" filters.
			var builder =
				XElementBuilder.WithName( "feTurbulence" )
				.Add(
					new XAttribute( "baseFrequency", $"{turbulenceGenerator.FrequencyX} {turbulenceGenerator.FrequencyY}" ),
					new XAttribute( "numOctaves", turbulenceGenerator.NumberOfOctaves ),
					new XAttribute( "seed", turbulenceGenerator.Seed ),
					new XAttribute( "stitchTiles", turbulenceGenerator.StitchTiles ? "stitch" : "noStitch" ) );

			return
				from outputAttribute in OutputAttributeBuilder
				select builder.Add( outputAttribute ).Build();

		}

		private static State<FilterChainState, XElementBuilder> BuildFloodGenerator( Color color ) =>
			XElementBuilder.WithName( "feFlood" )
			.Add(
				new XAttribute( "flood-color", SvgColorFactory.SvgColor( color ) ),
				new XAttribute( "flood-opacity", SvgColorFactory.SvgOpacity( color ) )
				)
			.Pipe( BuildOutputOnlyFilter );

		private static State<FilterChainState, IEnumerable<XElement>> BuildDisplacementFilter( DisplacementFilter displacementFilter, CoordinatesConverter converter )
		{

			var elementCore =
			XElementBuilder.WithName( "feDisplacementMap" )
				.Add(
					new XAttribute( "scale", converter.ScaleDistance( displacementFilter.Scale ) ),
					new XAttribute( "xChannelSelector", "R" ),
					new XAttribute( "yChannelSelector", "B" )
					);
			return BuildTwoInputFilter( elementCore, displacementFilter.DisplacementProvider, converter );


		}

		private static State<FilterChainState, IEnumerable<XElement>> BuildFilterChain( FilterChain chain, CoordinatesConverter converter )
		{
			IEnumerable<State<FilterChainState, IEnumerable<XElement>>> states = chain.Components.Select( x => BuildFilterComponent( x, converter ) );

			State<FilterChainState, ICollection<IEnumerable<XElement>>> concatenatedStates = State.Concat<FilterChainState, IEnumerable<XElement>>( states );

			return concatenatedStates.Select( x => x.SelectMany( y => y ) );
		}

		private static State<FilterChainState, XElement> BuildGaussianBlur( GaussianBlurFilter filter, CoordinatesConverter converter )
		{
			var basicElement =
				XElementBuilder
				.WithName( "feGaussianBlur" )
				.Add(
					new XAttribute(
						"stdDeviation",
						$"{ converter.ScaleDistance( filter.StandardDeviationX )} { converter.ScaleDistance( filter.StandardDeviationY )}" ) );

			return basicElement.Pipe( BuildIOFilter ).Select( x => x.Build() );
		}

		private static State<FilterChainState, XElement> BuildDropShadowFilter( DropShadowFilter filter, CoordinatesConverter converter )
		{
			var svgVector = filter.ShadowVector.Pipe( converter.ToSvgVector );

			var element =
				XElementBuilder.WithName( "feDropShadow" ).Add(
					new XAttribute( "dx", svgVector.Dx ),
					new XAttribute( "dy", svgVector.Dy ),
					new XAttribute( "stdDeviation", converter.ScaleDistance( filter.StdDeviation ) ) );

			return element.Pipe( BuildIOFilter ).Select( x => x.Build() );
		}

		private static State<FilterChainState, XElement> BuildColorMatrix( ColorMatrix matrix )
		{
			var element =
				XElementBuilder
				.WithName( "feColorMatrix" )
				.Add(
					new XAttribute( "type", "matrix" ),
					new XAttribute(
						"values",
						matrix.GetMatrixValues().Select( x => x.ToString() ).Pipe( x => string.Join( " ", x ) ) ) );
			return element.Pipe( BuildIOFilter ).Select( x => x.Build() );
		}

		private static State<FilterChainState, XElementBuilder> BuildOutputOnlyFilter( XElementBuilder elementCore ) =>
			from outputAttribute in OutputAttributeBuilder
			select elementCore.Add( outputAttribute );

		private static State<FilterChainState, IEnumerable<XElement>> BuildTwoInputFilter( XElementBuilder elementCore, IFilterComponent input2, CoordinatesConverter converter ) =>
				from startingState in State.Get<FilterChainState>()
				from input2ProviderElements in BuildFilterComponent( input2, converter )
				from input2Attribute in Input2AttributeBuilder
				from postDisplacementState in State.Get<FilterChainState>()
				from resetStartingState in FilterChainState.SetInputId( startingState.NextInputId )
				let displacementBuilder = elementCore
					.Add( input2Attribute )
				from finalElement in BuildIOFilter( displacementBuilder )
				select input2ProviderElements.Concat( new[] { finalElement.Build() } );

		/// <summary>
		/// Build a filter that has an input and an output
		/// </summary>
		private static State<FilterChainState, XElementBuilder> BuildIOFilter( XElementBuilder elementCore ) =>
			from inputAttribute in InputAttributeBuilder
			from elementWithOutput in BuildOutputOnlyFilter( elementCore )
			select elementWithOutput.Add( inputAttribute );
	}
}
