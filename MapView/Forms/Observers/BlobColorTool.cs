using System;
using System.Drawing;


namespace MapView.Forms.Observers
{
	/// <summary>
	/// A BlobColorTool is used for drawing blobs in <see cref="TopView"/> and
	/// <see cref="RouteView"/>.
	/// </summary>
	internal sealed class BlobColorTool
		:
			IDisposable
	{
		#region Fields (static)
		private const int ALFALFA = 100;

		internal static Brush BrushLightPrep = Brushes.White;
		#endregion Fields (static)


		#region Properties
		/// <summary>
		/// A pen for drawing walls.
		/// </summary>
		internal Pen Pen
		{ get; set; }

		/// <summary>
		/// A translucent pen for drawing non-solid walls (eg, windows and
		/// fences).
		/// </summary>
		internal Pen PenLight
		{ get; set; }

		internal Pen PenLightPrep
		{ get; set; }


		/// <summary>
		/// A brush for drawing content objects.
		/// </summary>
		internal Brush Brush
		{ get; set; }

		/// <summary>
		/// A translucent brush for drawing floor objects.
		/// </summary>
		internal Brush BrushLight
		{ get; set; }
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor[0]. Instantiates a colortool from a Pen object.
		/// </summary>
		/// <param name="pen"></param>
		internal BlobColorTool(Pen pen)
		{
			var colorLight = Color.FromArgb(ALFALFA, pen.Color);

			Pen          = new Pen(pen.Color,   pen.Width);
			PenLight     = new Pen(colorLight,  pen.Width);
			PenLightPrep = new Pen(Color.White, pen.Width);

			Brush          = new SolidBrush(pen.Color);
			BrushLight     = new SolidBrush(colorLight);
		}

		/// <summary>
		/// cTor[1]. Instantiates a colortool from a Brush object.
		/// </summary>
		/// <param name="brush"></param>
		/// <param name="width"></param>
		internal BlobColorTool(SolidBrush brush, float width)
		{
			var colorLight = Color.FromArgb(ALFALFA, brush.Color);

			Pen          = new Pen(brush.Color, width);
			PenLight     = new Pen(colorLight,  width);
			PenLightPrep = new Pen(Color.White, width);

			Brush          = new SolidBrush(brush.Color);
			BrushLight     = new SolidBrush(colorLight);
		}
		#endregion cTor


		/// <summary>
		/// Disposes pens and brushes owned by this object.
		/// </summary>
		public void Dispose()
		{
			Pen         .Dispose();
			PenLight    .Dispose();
			PenLightPrep.Dispose();
			Brush       .Dispose();
			BrushLight  .Dispose();
		}
	}
}
