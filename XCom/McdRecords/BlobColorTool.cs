using System;
using System.Drawing;


namespace XCom
{
	/// <summary>
	/// A <c>BlobColorTool</c> is used for drawing blobs in <c>TopView</c> and
	/// <c>RouteView</c> as well as Top/RouteView preview-blobs in McdView.
	/// </summary>
	/// <remarks>This object is disposable but eff their <c>IDisposable</c>
	/// crap.</remarks>
	public sealed class BlobColorTool
	{
		/// <summary>
		/// Disposes pens and brushes owned by this object.
		/// </summary>
		public void Dispose()
		{
			//DSShared.Logfile.Log("BlobColorTool.Dispose() " + Label);
			Pen         .Dispose();
			PenLight    .Dispose();
			PenLightPrep.Dispose();
			Brush       .Dispose();
			BrushLight  .Dispose();

			if (PenDoor != null) PenDoor.Dispose(); // is instantiated by McdView only
		}


		#region Fields (static)
		/// <summary>
		/// Alpha-transparency for the light pen and brush.
		/// </summary>
		public const int Alfalfa = 100;

		/// <summary>
		/// A brush for drawing the underlay of translucent brush-blobs.
		/// </summary>
		/// <seealso cref="BrushLight"><c>BrushLight</c></seealso>
		/// <remarks>Do not dispose this brush.</remarks>
		public static Brush BrushLightPrep = Brushes.White;
		#endregion Fields (static)


		#region Properties
		/// <summary>
		/// A pen for drawing walls.
		/// </summary>
		public Pen Pen
		{ get; private set; }

		/// <summary>
		/// A translucent pen for drawing non-solid walls (eg, windows and
		/// fences).
		/// </summary>
		public Pen PenLight
		{ get; private set; }

		/// <summary>
		/// A brush for drawing the underlay of translucent pen-blobs aka
		/// <c><see cref="PenLight"/></c>.
		/// </summary>
		public Pen PenLightPrep
		{ get; private set; }


		/// <summary>
		/// A pen for drawing a door indicator in walls.
		/// </summary>
		/// <remarks>Used only by McdView.</remarks>
		public Pen PenDoor
		{ get; private set; }


		/// <summary>
		/// A brush for drawing content objects.
		/// </summary>
		public Brush Brush
		{ get; private set; }

		/// <summary>
		/// A translucent brush for drawing floor objects.
		/// </summary>
		public Brush BrushLight
		{ get; private set; }
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor[0]. Instantiates a colortool from a <c>Pen</c> object.
		/// </summary>
		/// <param name="pen"></param>
		/// <remarks>Used only by MapView.</remarks>
		public BlobColorTool(Pen pen)
		{
			Color colorLight = Color.FromArgb(Alfalfa, pen.Color);

			Pen          = new Pen(pen.Color,   pen.Width);
			PenLight     = new Pen(colorLight,  pen.Width);
			PenLightPrep = new Pen(Color.White, pen.Width);

//			PenDoor = null;

			Brush        = new SolidBrush(pen.Color);
			BrushLight   = new SolidBrush(colorLight);
		}

		/// <summary>
		/// cTor[1]. Instantiates a colortool from a <c>Brush</c> object.
		/// </summary>
		/// <param name="brush"></param>
		/// <param name="width"></param>
		/// <remarks>Used only by MapView.</remarks>
		public BlobColorTool(SolidBrush brush, float width)
		{
			Color colorLight = Color.FromArgb(Alfalfa, brush.Color);

			Pen          = new Pen(brush.Color, width);
			PenLight     = new Pen(colorLight,  width);
			PenLightPrep = new Pen(Color.White, width);

//			PenDoor = null;

			Brush        = new SolidBrush(brush.Color);
			BrushLight   = new SolidBrush(colorLight);
		}

		/// <summary>
		/// cTor[2]. Instantiates a colortool from a <c>Color</c> object.
		/// </summary>
		/// <param name="color"></param>
		/// <remarks>Used only by McdView.</remarks>
		public BlobColorTool(Color color)
		{
			Color light = Color.FromArgb(Alfalfa, color);

			Pen          = new Pen(color,       7.0f);
			PenLight     = new Pen(light,       7.0f);
			PenLightPrep = new Pen(Color.White, 7.0f);

			PenDoor      = new Pen(color,       2.0f);

			Brush        = new SolidBrush(color);
			BrushLight   = new SolidBrush(light);
		}
		#endregion cTor
	}
}
