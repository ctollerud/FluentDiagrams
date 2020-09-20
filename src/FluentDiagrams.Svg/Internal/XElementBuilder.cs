using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace FluentDiagrams.Svg.Internal
{
	public class XElementBuilder
	{
		public ImmutableDictionary<string, XAttribute> Attributes { get; }
		public ImmutableList<XElement> Elements { get; }

		public XName Name { get; }

		private XElementBuilder( XName elementName, ImmutableDictionary<string, XAttribute> attributes, ImmutableList<XElement> elements )
		{
			Name = elementName;
			Attributes = attributes;
			Elements = elements.ToImmutableList();
		}

		public static XElementBuilder WithName( XName elementName ) =>
			new XElementBuilder( elementName, ImmutableDictionary<string, XAttribute>.Empty, ImmutableList<XElement>.Empty );


		public XElementBuilder Add( params XAttribute[] attributes ) =>
			Add( attributes.AsEnumerable() );

		public XElementBuilder Add( IEnumerable<XAttribute> attributes ) =>
			new XElementBuilder(
				Name,
				Attributes
					.RemoveRange( attributes.Select( x => x.Name.ToString() ) )
					.AddRange( attributes.ToDictionary( x => x.Name.ToString(), x => x ) ),
					Elements );

		public XElementBuilder Add( params XElement[] elements ) =>
			Add( elements.AsEnumerable() );

		public XElementBuilder Add( IEnumerable<XElement> elements ) =>
			new XElementBuilder( Name, Attributes, Elements.AddRange( elements ) );

		public XElement Build() =>
			new XElement( Name, Attributes.Select( pair => pair.Value ), Elements );

		public XElementBuilder AddOrModifyAttribute(
			XName attributeName,
			Func<string> genNewValue,
			Func<string, string> modifyExistingValue
			)
		{
			if( Attributes.TryGetValue( attributeName.ToString(), out XAttribute value ) )
			{
				return this.Add( new XAttribute( attributeName, modifyExistingValue( value.Value ) ) );
			}

			return this.Add( new XAttribute( attributeName, genNewValue() ) );
		}



	}
}
