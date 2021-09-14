using LinqGarden;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace FluentDiagrams.Svg.Internal
{
	using DefinitionCollection = ImmutableDictionary<object, (string Id, int Order, XElement DefBody)>;

	internal class SvgDrawState
	{
		public SvgDrawState( CoordinatesConverter converter ) :
			this( converter, DefinitionCollection.Empty )
		{ }

		private SvgDrawState( CoordinatesConverter converter, DefinitionCollection definitions )
		{
			Converter = converter;
			m_Definitions = definitions;
		}

		public CoordinatesConverter Converter { get; }

		private readonly ImmutableDictionary<object, (string Id, int Order, XElement DefBody)> m_Definitions;

		public IEnumerable<XElement> BuildDeclarations()
		{
			yield return
				new XElement( "defs",
					m_Definitions.Values
					.OrderBy( x => x.Order )
					.Select( x => x.DefBody ) );
		}

		public State<SvgDrawState, string> IncludeDefinitionInternal( object defined, State<SvgDrawState, XElementBuilder> definitionFactory )
		{
			if( m_Definitions.TryGetValue( defined, out var values ) )
			{
				return State.Return<SvgDrawState, string>( values.Id );
			}
			else
			{
				return
					from element in definitionFactory
					from state in State.Get<SvgDrawState>()
					let definitions = state.m_Definitions
					let orderingIndex = definitions.Count
					let cssId = $"{defined.GetType().Name}_{ orderingIndex}"
					let elementWithId = element.Add( new XAttribute( "id", cssId ) )
					let newCollection = definitions.Add( defined, (cssId, definitions.Count, elementWithId.Build()) )
					from addNewState in State.Put<SvgDrawState>( new SvgDrawState( Converter, newCollection ) )
					select cssId;
			}
		}

		public static State<SvgDrawState, string> IncludeDefinition( object key, State<SvgDrawState, XElementBuilder> definitionRenderer ) =>
			from drawState in State.Get<SvgDrawState>()
			from definitionKey in drawState.IncludeDefinitionInternal( key, definitionRenderer )
			select definitionKey;
	}
}
