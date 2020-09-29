using System;
using System.Collections.Generic;
using System.Text;

namespace FluentDiagrams.Core.Filters
{
	public enum SourceType
	{
		SourceGraphic = 0,
		SourceAlpha = -1,


		/// <summary>
		/// !!!!!! Not yet supported !!!!!!
		/// </summary>
		BackgroundImage = -2,

		/// <summary>
		/// ????? May or may not yet be supported ??????
		/// </summary>
		BackgroundAlpha = -3,
		FillPaint = -4,
		StrokePaint = -5
	}
}
