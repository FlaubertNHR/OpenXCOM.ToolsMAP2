using System;
using System.Drawing;


namespace MapView.Forms.MapObservers.TopViews
{
	/// <summary>
	/// A ColorTool is used for drawing blobs on TopView and RouteView.
	/// </summary>
	internal sealed class ColorTool
		:
			IDisposable
	{
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
		#endregion Properties


		#region cTors
		/// <summary>
		/// cTor[1].
		/// </summary>
		/// <param name="pen"></param>
		internal ColorTool(Pen pen)
		{
			var colorLight = Color.FromArgb(80, pen.Color);

			_pen        = pen;
			_penLight   = new Pen(colorLight, pen.Width);

			_brush      = new SolidBrush(pen.Color);
			_brushLight = new SolidBrush(colorLight);
		}

		/// <summary>
		/// cTor[2].
		/// </summary>
		/// <param name="brush"></param>
		/// <param name="width"></param>
		internal ColorTool(SolidBrush brush, float width)
		{
			var colorLight = Color.FromArgb(50, brush.Color);

			_pen        = new Pen(brush.Color, width);
			_penLight   = new Pen(colorLight, width);

			_brush      = brush;
			_brushLight = new SolidBrush(colorLight);
		}
		#endregion cTors


		/// <summary>
		/// This isn't really necessary since the Pens and Brushes last the
		/// lifetime of the app. But FxCop gets antsy ....
		/// NOTE: Dispose() is never called. cf DrawBlobService. cf QuadrantPanelDrawService.
		/// WARNING: This is NOT a robust implementation perhaps. But it
		/// satisifes the core of the matter and could likely be used for
		/// further development if that's ever required.
		/// </summary>
		public void Dispose()
		{
			if (_pen        != null) _pen       .Dispose();
			if (_penLight   != null) _penLight  .Dispose();
			if (_brush      != null) _brush     .Dispose();
			if (_brushLight != null) _brushLight.Dispose();

			GC.SuppressFinalize(this);
		}
	}
}
