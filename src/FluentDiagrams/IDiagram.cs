using FluentDiagrams.Primitives;
using LinqGarden;
using System;

namespace FluentDiagrams
{
	public interface IDiagram
	{
		Maybe<BoundingBox> Bounds { get; }
	}
}
