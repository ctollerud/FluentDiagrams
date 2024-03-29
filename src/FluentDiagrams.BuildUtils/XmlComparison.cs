﻿using FluentAssertions;
using FluentAssertions.Execution;
using LinqGarden;
using LinqGarden.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace FluentDiagrams.BuildUtils
{
	public class XmlComparisonStructure
	{
		public XmlComparisonStructure( XElement element )
		{
			Name = element.Name.ToString();
			InnerElements = element.Elements().Select( x => new XmlComparisonStructure( x ) ).ToList();
			InnerText =
				element.NoneIfNull()
				.Where( element => element.Elements().Any() == false )
				.Select( element => element.Value );

			Attributes =
				( from attribute in element.Attributes()
				  let name = attribute.Name.ToString()
				  orderby name
				  select (name, attribute.Value) )
				.ToList();
		}

		public string Name { get; }

		public List<XmlComparisonStructure> InnerElements { get; }

		public Maybe<string> InnerText { get; }

		public List<(string Name, string Value)> Attributes { get; }
	}

	public static class XmlComparison
	{
		public static Fallible<string, Unit> CompareElements( XElement expected, XElement actual ) =>
			Function.From( () => CompareElementsUnsafely( expected, actual ) )
			.CatchAsFailure<AssertionFailedException>()
			.SelectFailure( exc => exc.Message )
			.Invoke();

		/// <summary>
		/// Compare the two elements using FluentAssertion's functionality.  Throws an exception if 
		/// they are deemed to not be structurally equivalent.
		/// </summary>
		/// <param name="expected"></param>
		/// <param name="actual"></param>
		private static void CompareElementsUnsafely( XElement expected, XElement actual )
		{
			Console.Out.WriteLine( "Performing Comparison:" );
			new XmlComparisonStructure( actual ).Should().BeEquivalentTo( new XmlComparisonStructure( expected ) );
		}
	}
}
