using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;


namespace DSShared.Controls
{
	/// <summary>
	/// Based on code by NJF. But that was 1500 LoC.
	/// </summary>
	public sealed class CollapsibleSplitter
		:
			Splitter
	{
		#region IDisposable
		private bool _disposed;
		protected override void Dispose(bool disposing)
		{
			Logfile.Log("CollapsibleSplitter.Dispose(" + disposing + ")");
			if (!_disposed && disposing)
			{
				_disposed = true;

				_pathTrisLeft .Dispose();
				_pathTrisRight.Dispose();
			}
			base.Dispose(disposing);
		}
		#endregion IDisposable


		#region Fields (static)
		private const int WidthTri  = 3;
		private const int HeightTri = 6;
		private const int DotPeriod = 3;
		private const int OffsetY   = 3;
		private const int Offset    = 5 + OffsetY + HeightTri;
		#endregion Fields (static)


		#region Fields
		private Control _control;

		private bool _collapsed;
		private bool _over;

		private readonly GraphicsPath _pathTrisLeft  = new GraphicsPath();
		private readonly GraphicsPath _pathTrisRight = new GraphicsPath();
		#endregion Fields


		#region Properties
		private Rectangle Clickable
		{ get; set; }
		#endregion Properties


		int x1, x2;

		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		public CollapsibleSplitter()
		{
			Width = 7;
			DoubleBuffered = true;

			x1 = (Width - WidthTri) / 2;
			x2 = x1 + WidthTri;
		}
		#endregion cTor


		#region Methods
		/// <summary>
		/// Assigns what <c>Control</c> to collapse.
		/// </summary>
		/// <param name="control"></param>
		public void SetControl(Control control)
		{
			_control = control;
		}

		/// <summary>
		/// Defines the center third of the splitter as a clickable area that
		/// collapses the <c><see cref="CompositedTreeView"/></c>. Also sets the
		/// graphics-paths for the small triangles.
		/// </summary>
		public void SetClickableRectangle()
		{
			int height = Height / 3;
			Clickable = new Rectangle(
								0, (Height - height) / 2 - 10,
								Width, height);

			_pathTrisLeft .Reset();
			_pathTrisRight.Reset();

			int y1 = Clickable.Y + OffsetY;
			int y2 = y1 + HeightTri / 2;
			int y7 = Clickable.Y + Clickable.Height;
			int y3 = y7 - OffsetY;
			int y4 = y3 - HeightTri;
			int y5 = y1 + HeightTri;
			int y6 = y7 - (OffsetY + HeightTri / 2);

			_pathTrisLeft.AddLine(x2,y1, x1,y2);
			_pathTrisLeft.AddLine(x1,y2, x2,y5);
			_pathTrisLeft.CloseFigure();

			_pathTrisLeft.AddLine(x2,y3, x2,y4);
			_pathTrisLeft.AddLine(x2,y4, x1,y6);
			_pathTrisLeft.CloseFigure();

			_pathTrisRight.AddLine(x1,y1, x2,y2);
			_pathTrisRight.AddLine(x2,y2, x1,y5);
			_pathTrisRight.CloseFigure();

			_pathTrisRight.AddLine(x1,y3, x1,y4);
			_pathTrisRight.AddLine(x1,y4, x2,y6);
			_pathTrisRight.CloseFigure();
		}


		/// <summary>
		/// Toggles visibility of this <c>CollapsibleSplitter</c>.
		/// </summary>
		private void ToggleSplitter()
		{
			_control.Visible = !(_collapsed = !_collapsed);
		}
		#endregion Methods


		#region Events (override)
		/// <summary>
		/// Overrides the resize event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnResize(EventArgs e)
		{
			SetClickableRectangle();
			Invalidate();
		}


		/// <summary>
		/// Overrides the click event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnClick(EventArgs e)
		{
			if (_over)
				ToggleSplitter();
		}

		/// <summary>
		/// Overrides the mousedown event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (!_over && !_collapsed)
				base.OnMouseDown(e);
		}

		/// <summary>
		/// Overrides the mousemove event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (   e.X >= Clickable.X && e.X <= Clickable.X + Clickable.Width
				&& e.Y >= Clickable.Y && e.Y <= Clickable.Y + Clickable.Height)
			{
				if (!_over)
				{
					_over = true;
					Invalidate();

					if (!_collapsed)
						Cursor = Cursors.PanWest;
					else
						Cursor = Cursors.PanEast;
				}
			}
			else
			{
				if (_over)
				{
					_over = false;
					Invalidate();
				}

				if (!_collapsed)
					Cursor = Cursors.VSplit;
				else
					Cursor = Cursors.Default;
			}
			base.OnMouseMove(e);
		}

		/// <summary>
		/// Overrides the mouseleave event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseLeave(EventArgs e)
		{
			_over = false;
			Invalidate();
		}


		/// <summary>
		/// Overrides the paint event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			var graphics = e.Graphics;

			if (_over)
				graphics.FillRectangle(Brushes.PaleTurquoise, Clickable);

			graphics.DrawLine( // draw the top & bottom lines for the clickable area ->
							SystemPens.ControlDark,
							Clickable.X,                   Clickable.Y,
							Clickable.X + Clickable.Width, Clickable.Y);
			graphics.DrawLine(
							SystemPens.ControlDark,
							Clickable.X,                   Clickable.Y + Clickable.Height,
							Clickable.X + Clickable.Width, Clickable.Y + Clickable.Height);

			GraphicsPath tris; // draw the arrows for the clickable area ->
			if (!_collapsed)
				tris = _pathTrisLeft;
			else
				tris = _pathTrisRight;

			graphics.FillPath(SystemBrushes.ControlDark, tris);


			int x = Width / 2 - 1; // draw the dots for the clickable area ->
			int y = Clickable.Y + Offset;

			while (y < Clickable.Y + Clickable.Height - Offset)
			{
				graphics.DrawLine(
								SystemPens.ControlDark,
								x,     y,
								x + 2, y + 2);
				y += DotPeriod;
			}

			ControlPaint.DrawBorder3D(graphics, ClientRectangle, Border3DStyle.Flat);
		}
		#endregion Events (override)
	}
}
