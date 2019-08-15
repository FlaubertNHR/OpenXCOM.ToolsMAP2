using System;
using System.Drawing;


namespace MapView.Forms.Observers
{
	/// <summary>
	/// A ColorTool is used for drawing blobs on TopView and RouteView.
	/// </summary>
	internal sealed class ColorTool
		:
			IDisposable
	{
		#region Fields (static)
		private const int ALFALFA = 90;
		#endregion Fields (static)


		#region Properties
		private readonly Pen _pen;
		/// <summary>
		/// A pen for drawing walls.
		/// </summary>
		internal Pen Pen
		{
			get { return _pen; }
		}

		private readonly Pen _penLight;
		/// <summary>
		/// A translucent pen for drawing non-solid walls (eg, windows and
		/// fences).
		/// </summary>
		internal Pen PenLight
		{
			get { return _penLight; }
		}

		private readonly Pen _penLightPrep;
		internal Pen PenLightPrep
		{
			get { return _penLightPrep; }
		}


		private readonly SolidBrush _brush;
		/// <summary>
		/// A brush for drawing content objects.
		/// </summary>
		internal Brush Brush
		{
			get { return _brush; }
		}

		private readonly SolidBrush _brushLight;
		/// <summary>
		/// A translucent brush for drawing floor objects.
		/// </summary>
		internal Brush BrushLight
		{
			get { return _brushLight; }
		}

		private readonly Brush _brushLightPrep;
		internal Brush BrushLightPrep
		{
			get { return _brushLightPrep; }
		}
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor[0]. Instantiates a colortool from a Pen object.
		/// </summary>
		/// <param name="pen"></param>
		internal ColorTool(Pen pen)
		{
			var colorLight = Color.FromArgb(ALFALFA, pen.Color);

			_pen          = new Pen(pen.Color,        pen.Width);
			_penLight     = new Pen(colorLight,       pen.Width);
			_penLightPrep = new Pen(Color.GhostWhite, pen.Width);

			_brush          = new SolidBrush(pen.Color);
			_brushLight     = new SolidBrush(colorLight);
			_brushLightPrep = new SolidBrush(Color.GhostWhite);
		}

		/// <summary>
		/// cTor[1]. Instantiates a colortool from a Brush object.
		/// </summary>
		/// <param name="brush"></param>
		/// <param name="width"></param>
		internal ColorTool(SolidBrush brush, float width)
		{
			var colorLight = Color.FromArgb(ALFALFA, brush.Color);

			_pen          = new Pen(brush.Color,      width);
			_penLight     = new Pen(colorLight,       width);
			_penLightPrep = new Pen(Color.GhostWhite, width);

			_brush          = new SolidBrush(brush.Color);
			_brushLight     = new SolidBrush(colorLight);
			_brushLightPrep = new SolidBrush(Color.GhostWhite);
		}
		#endregion cTor


		/// <summary>
		/// @note cf DrawBlobService. cf QuadrantDrawService.
		/// WARNING: This is NOT a robust implementation perhaps. But it
		/// satisifes the core of the matter and could likely be used for
		/// further development if that's ever required.
		/// </summary>
		public void Dispose()
		{
			_pen           .Dispose();
			_penLight      .Dispose();
			_penLightPrep  .Dispose();
			_brush         .Dispose();
			_brushLight    .Dispose();
			_brushLightPrep.Dispose();

			GC.SuppressFinalize(this);
		}
	}
}
