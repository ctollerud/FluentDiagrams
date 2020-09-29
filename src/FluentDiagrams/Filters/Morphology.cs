using System;
using System.Collections.Generic;
using System.Text;

namespace FluentDiagrams.Core.Filters
{
	public class Morphology : IFilterComponent
	{
		public decimal RadiusY { get; }
		public decimal RadiusX { get; }
		public Type MorphologyType { get; }

		public enum Type
		{
			Dilate,
			Erode
		}


		public Morphology( Type morphologyType, decimal radiusX, decimal radiusY )
		{
			RadiusY = radiusY;
			RadiusX = radiusX;
			MorphologyType = morphologyType;
		}
	}
}
