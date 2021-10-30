using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using DSShared.Controls;


namespace PckView
{
	internal sealed class PalettePanel
		:
			BufferedPanel
	{
		#region Fields (static)
		/// <summary>
		/// 16 swatches hori/vert in the panel.
		/// </summary>
		internal const int Sqrt = 16;
		#endregion Fields (static)


		#region Fields
		private readonly PaletteF _fpalette;

		private int _x = -1;
		private int _y = -1;

		private readonly GraphicsPath _pathTran_hori = new GraphicsPath();
		private readonly GraphicsPath _pathTran_vert = new GraphicsPath();
		#endregion Fields


		#region Properties
		private int SwatchWidth
		{ get; set; }

		private int SwatchHeight
		{ get; set; }

		private int _palid = -1;
		internal int Palid
		{
			get { return _palid; }
			private set { _palid = value; }
		}
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		internal PalettePanel(PaletteF f)
		{
			_fpalette = f;

			Dock = DockStyle.Fill;
			PckViewF.PaletteChanged += OnPaletteChanged;
		}
		#endregion cTor


		#region Events (override)
		/// <summary>
		/// Thus <c>PalettePanel</c> cannot be resized, but the <c>Resize</c>
		/// <c>event</c> will fire when its parent <c>Form</c> loads etc.
		/// </summary>
		/// <param name="eventargs"></param>
		protected override void OnResize(EventArgs eventargs)
		{
			SwatchWidth  = Width  / Sqrt;
			SwatchHeight = Height / Sqrt;

			_pathTran_hori.Reset();
			_pathTran_vert.Reset();

			int x0 = SwatchWidth      / 3;
			int x1 = SwatchWidth  * 2 / 3;
			int y0 = SwatchHeight     / 3;
			int y1 = SwatchHeight * 2 / 3;

			_pathTran_hori.AddLine(x0,y1, x0,y0);
			_pathTran_hori.AddLine(x0,y0, x1,y0);
			_pathTran_vert.AddLine(x1,y0, x1,y1);
			_pathTran_vert.AddLine(x1,y1, x0,y1);

			if (Palid != -1)
			{
				_x = Palid % Sqrt * SwatchWidth  + 1;
				_y = Palid / Sqrt * SwatchHeight + 1;
			}
			Refresh();
		}

		/// <summary>
		/// Overrides the <c>MouseDown</c> handler.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);

			if (e.Button == MouseButtons.Left)
			{
				int swatchX = e.X / SwatchWidth;
				int swatchY = e.Y / SwatchHeight;

				_x = swatchX * SwatchWidth  + 1;
				_y = swatchY * SwatchHeight + 1;

				Palid = swatchY * Sqrt + swatchX;

				UpdatePalette();
			}
		}

		/// <summary>
		/// Overrides the <c>MouseWheel</c> handler. Scrolls the selected
		/// <c><see cref="Palid"/></c>.
		/// </summary>
		/// <param name="e"></param>
		/// <remarks>This <c>PalettePanel</c> cannot have focus because it is a
		/// <c>Panel</c> which would require <c>SetControlStyle()</c> to make it
		/// selectable - which could create issues in Win10 - but by calling
		/// <c>Application.AddMessageFilter()</c> in the app-constructor the
		/// panel can be forced to take a mousewheel-message anyway.</remarks>
		protected override void OnMouseWheel(MouseEventArgs e)
		{
			if (Palid == -1)
			{
				SelectPalid((byte)0);
			}
			else if (e.Delta > 0)
			{
				if (Palid != Byte.MinValue)
					SelectPalid((byte)(Palid - 1));
			}
			else if (e.Delta < 0)
			{
				if (Palid != Byte.MaxValue)
					SelectPalid((byte)(Palid + 1));
			}
//			base.OnMouseWheel(e);
		}

		/// <summary>
		/// Overrides the <c>Paint</c> handler.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics graphics = e.Graphics;
			PckViewF f = _fpalette._feditor._f;

			if (f.Shader == PckViewF.ShaderOn)
			{
				for (int i = 0, y = 0; i != Sqrt; ++i, y += SwatchHeight)
				for (int j = 0, x = 0; j != Sqrt; ++j, x += SwatchWidth)
				{
					using (var brush = new SolidBrush(SpritePanel.Shade(f.Pal[i * Sqrt + j])))
					{
						graphics.FillRectangle(
											brush,
											x,y,
											SwatchWidth, SwatchHeight);
					}
				}
			}
			else
			{
				for (int i = 0, y = 0; i != Sqrt; ++i, y += SwatchHeight)
				for (int j = 0, x = 0; j != Sqrt; ++j, x += SwatchWidth)
				{
					using (var brush = new SolidBrush(f.Pal[i * Sqrt + j]))
					{
						graphics.FillRectangle(
											brush,
											x,y,
											SwatchWidth, SwatchHeight);
					}
				}
			}


			graphics.DrawPath(Pens.Black, _pathTran_hori); // draw an indicator in the transparent-id swatch ->
			graphics.DrawPath(Pens.White, _pathTran_vert);

			if (Palid != -1) // highlight the selected swatch ->
			{
				graphics.DrawLine(
							Pens.White,
							_x - 1,               _y - 1,
							_x + SwatchWidth - 2, _y - 1);
				graphics.DrawLine(
							Pens.White,
							_x - 1,               _y - 1,
							_x - 1,               _y + SwatchHeight - 2);
				graphics.DrawLine(
							Pens.Black,
							_x + SwatchWidth - 2, _y - 1,
							_x + SwatchWidth - 2, _y + SwatchHeight - 2);
				graphics.DrawLine(
							Pens.Black,
							_x - 1,               _y + SwatchHeight - 2,
							_x + SwatchWidth - 2, _y + SwatchHeight - 2);
			}
		}
		#endregion Events (override)


		#region Events
		/// <summary>
		/// Handles the
		/// <c><see cref="PckViewF.PaletteChanged">PckViewF.PaletteChanged</see></c>
		/// event.
		/// </summary>
		private void OnPaletteChanged()
		{
			UpdatePalette();
		}
		#endregion Events


		#region Methods
		/// <summary>
		/// Navigates this <c>PalettePanel</c> by keyboard.
		/// </summary>
		/// <param name="keys"></param>
		/// <remarks>Called by <c><see cref="PaletteF"/>.OnKeyDown()</c>.</remarks>
		internal void Navigate(Keys keys)
		{
			if (Palid == -1)
			{
				SelectPalid((byte)0);
			}
			else
			{
				switch (keys)
				{
					case Keys.Up:
						if (Palid >= Sqrt)
							SelectPalid((byte)(Palid - Sqrt));
						break;

					case Keys.Down:
						if (Palid <= Byte.MaxValue - Sqrt)
							SelectPalid((byte)(Palid + Sqrt));
						break;

					case Keys.Left:
						if (Palid % Sqrt > 0)
							SelectPalid((byte)(Palid - 1));
						break;

					case Keys.Right:
						if (Palid % Sqrt < Sqrt - 1)
							SelectPalid((byte)(Palid + 1));
						break;
				}
			}
		}

		/// <summary>
		/// Forces selection of a specific palette-id.
		/// </summary>
		/// <param name="palid">the palette id</param>
		internal void SelectPalid(byte palid)
		{
			Palid = (int)palid;

			_x = Palid % Sqrt * SwatchWidth  + 1;
			_y = Palid / Sqrt * SwatchHeight + 1;

			UpdatePalette();
		}

		/// <summary>
		/// Prints color-info to the statusbar and invalidates this
		/// <c>PalettePanel</c>.
		/// </summary>
		private void UpdatePalette()
		{
			_fpalette.PrintPaletteColor(Palid);
			if (Visible) Invalidate();
		}

		/// <summary>
		/// Disposes graphics paths and unsubscribes from
		/// <c><see cref="PckViewF.PaletteChanged">PckViewF.PaletteChanged</see></c>.
		/// </summary>
		/// <remarks>I have no idea if this is really necessary despite hundreds
		/// of hours reading about <c>Dispose()</c> et al. This class is a
		/// visual control so it gets disposed when its parent closes, but do
		/// its private fields get disposed reliably ... the designer doesn't
		/// appear to care re. <c>Font</c> eg.</remarks>
		internal void Destroy()
		{
			PckViewF.PaletteChanged -= OnPaletteChanged;

			_pathTran_hori.Dispose();
			_pathTran_vert.Dispose();

//			base.Dispose(); // <- I *still* don't know if that is a Good Thing or not.
		}
		#endregion Methods
	}
}
