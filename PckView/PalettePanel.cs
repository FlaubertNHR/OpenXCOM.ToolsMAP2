using System;
using System.Drawing;
using System.Windows.Forms;

using DSShared.Controls;


namespace PckView
{
	internal sealed class PalettePanel
		:
			BufferedPanel
	{
		#region Fields (static)
		internal const int SwatchesPerSide = 16; // 16 swatches across the panel.
		#endregion Fields (static)


		#region Fields
		private readonly PaletteForm _fpalette;

		private int _swatchWidth;
		private int _swatchHeight;

		private int _x = -1;
		private int _y = -1;
		#endregion Fields


		#region Properties
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
		internal PalettePanel(PaletteForm f)
		{
			_fpalette = f;

			Dock = DockStyle.Fill;
			PckViewForm.PaletteChanged += OnPaletteChanged;
		}
		#endregion cTor


		#region Events (override)
		/// <summary>
		/// @note The PalettePanel cannot be resized, but OnResize will fire
		/// when its form loads etc.
		/// </summary>
		/// <param name="eventargs"></param>
		protected override void OnResize(EventArgs eventargs)
		{
			_swatchWidth  = Width  / SwatchesPerSide;
			_swatchHeight = Height / SwatchesPerSide;

			if (Palid != -1)
			{
				_x = Palid % SwatchesPerSide * _swatchWidth  + 1;
				_y = Palid / SwatchesPerSide * _swatchHeight + 1;
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

			int swatchX = e.X / _swatchWidth;
			int swatchY = e.Y / _swatchHeight;

			_x = swatchX * _swatchWidth  + 1;
			_y = swatchY * _swatchHeight + 1;

			Palid = swatchY * SwatchesPerSide + swatchX;

			UpdatePalette();
		}

		/// <summary>
		/// Draws the palette viewer.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{
//			base.OnPaint(e);

			var graphics = e.Graphics;

			if (_fpalette._feditor._f.SpriteShade > -1)
			{
				for (int
						i = 0,
							y = 0;
						i != SwatchesPerSide;
						++i,
							y += _swatchHeight)
				{
					for (int
							j = 0,
								x = 0;
							j != SwatchesPerSide;
							++j,
								x += _swatchWidth)
					{
						using (var brush = new SolidBrush(SpritePanel.AdjustColor(PckViewForm.Pal[j + SwatchesPerSide * i])))
						{
							graphics.FillRectangle(
												brush,
												x, y,
												_swatchWidth, _swatchHeight);
						}
					}
				}
			}
			else
			{
				for (int
						i = 0,
							y = 0;
						i != SwatchesPerSide;
						++i,
							y += _swatchHeight)
				{
					for (int
							j = 0,
								x = 0;
							j != SwatchesPerSide;
							++j,
								x += _swatchWidth)
					{
						using (var brush = new SolidBrush(PckViewForm.Pal[j + SwatchesPerSide * i]))
						{
							graphics.FillRectangle(
												brush,
												x, y,
												_swatchWidth, _swatchHeight);
						}
					}
				}
			}

			if (Palid != -1)
			{
				graphics.DrawRectangle(
									Pens.Red,
									_x           - 1, _y            - 1,
									_swatchWidth - 1, _swatchHeight - 1);
			}
		}
		#endregion Events (override)


		#region Events
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
		internal void SelectPaletteId(int palid)
		{
			Palid = palid;

			_x = palid % SwatchesPerSide * _swatchWidth  + 1;
			_y = palid / SwatchesPerSide * _swatchHeight + 1;

			UpdatePalette();
		}

		internal void UpdatePalette()
		{
			_fpalette.PrintPaletteId(Palid);
			Invalidate();
		}
		#endregion Methods
	}
}
