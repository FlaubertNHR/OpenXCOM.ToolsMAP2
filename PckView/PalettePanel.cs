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
		/// The PalettePanel cannot be resized, but OnResize will fire when its
		/// form loads etc.
		/// </summary>
		/// <param name="eventargs"></param>
		protected override void OnResize(EventArgs eventargs)
		{
			SwatchWidth  = Width  / Sqrt;
			SwatchHeight = Height / Sqrt;

			_pathTran_hori.Reset();
			_pathTran_vert.Reset();

			int x0 = SwatchWidth  / 3;
			int x1 = SwatchWidth  * 2 / 3;
			int y0 = SwatchHeight / 3;
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
		/// Handles the mousedown event.
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
		/// Draws the palette viewer.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{
			var graphics = e.Graphics;
			PckViewF f = _fpalette._feditor._f;

			if (f.SpriteShade >= PckViewF.SPRITESHADE_ON)
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
		/// Handles the PaletteChanged event.
		/// </summary>
		private void OnPaletteChanged()
		{
			UpdatePalette();
		}
		#endregion Events


		#region Methods
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
		/// Handler for PaletteChanged. Prints color-info to the statusbar and
		/// invalidates this panel.
		/// </summary>
		private void UpdatePalette()
		{
			_fpalette.PrintPaletteColor(Palid);
			if (Visible) Invalidate();
		}

		/// <summary>
		/// Disposes graphics paths and unsubscribes from static event.
		/// </summary>
		/// <remarks>I have no idea if this is really necessary despite hundreds
		/// of hours reading about Dispose() et al. This class is a visual
		/// control so it gets disposed when its parent closes, but do its
		/// private fields get disposed reliably ... the designer doesn't appear
		/// to care re. Font eg.</remarks>
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
