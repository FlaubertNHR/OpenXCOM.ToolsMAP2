using System;
using System.Drawing;


namespace MapView.Forms.Observers
{
	/// <summary>
	/// A <c>BlobColorTool</c> is used for drawing blobs in
	/// <c><see cref="TopView"/></c> and <c><see cref="RouteView"/></c>.
	/// </summary>
	/// <remarks>This object is disposable but eff their <c>IDisposable</c> crap.</remarks>
	internal sealed class BlobColorTool
	{
		/// <summary>
		/// Disposes pens and brushes owned by this object.
		/// </summary>
		internal void Dispose()
		{
			//DSShared.Logfile.Log("BlobColorTool.Dispose() " + Label);
			Pen         .Dispose();
			PenLight    .Dispose();
			PenLightPrep.Dispose();
			Brush       .Dispose();
			BrushLight  .Dispose();
		}


		#region Fields (static)
		internal const int ALFALFA = 100;

		internal static Brush BrushLightPrep = Brushes.White;
		#endregion Fields (static)


		#region Properties
//		/// <summary>
//		/// A human-readable label for this <c>BlobColorTool</c>.
//		/// </summary>
//		internal string Label
//		{ get; private set; }

		/// <summary>
		/// A pen for drawing walls.
		/// </summary>
		internal Pen Pen
		{ get; private set; }

		/// <summary>
		/// A translucent pen for drawing non-solid walls (eg, windows and
		/// fences).
		/// </summary>
		internal Pen PenLight
		{ get; private set; }

		internal Pen PenLightPrep
		{ get; private set; }


		/// <summary>
		/// A brush for drawing content objects.
		/// </summary>
		internal Brush Brush
		{ get; private set; }

		/// <summary>
		/// A translucent brush for drawing floor objects.
		/// </summary>
		internal Brush BrushLight
		{ get; private set; }
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor[0]. Instantiates a colortool from a <c>Pen</c> object.
		/// </summary>
		/// <param name="pen"></param>
//		/// <param name="label"></param>
		internal BlobColorTool(Pen pen)
//			string label
		{
//			Label = label;

			Color colorLight = Color.FromArgb(ALFALFA, pen.Color);

			Pen          = new Pen(pen.Color,   pen.Width);
			PenLight     = new Pen(colorLight,  pen.Width);
			PenLightPrep = new Pen(Color.White, pen.Width);

			Brush        = new SolidBrush(pen.Color);
			BrushLight   = new SolidBrush(colorLight);
		}

		/// <summary>
		/// cTor[1]. Instantiates a colortool from a <c>Brush</c> object.
		/// </summary>
		/// <param name="brush"></param>
		/// <param name="width"></param>
//		/// <param name="label"></param>
		internal BlobColorTool(SolidBrush brush, float width)
//			string label
		{
//			Label = label;

			Color colorLight = Color.FromArgb(ALFALFA, brush.Color);

			Pen          = new Pen(brush.Color, width);
			PenLight     = new Pen(colorLight,  width);
			PenLightPrep = new Pen(Color.White, width);

			Brush        = new SolidBrush(brush.Color);
			BrushLight   = new SolidBrush(colorLight);
		}
		#endregion cTor
	}
}
