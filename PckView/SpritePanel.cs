using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using DSShared.Controls;

using XCom;
using XCom.Interfaces;


namespace PckView
{
	internal sealed class SpritePanel
		:
			BufferedPanel
	{
		#region Fields (static)
		/// <summary>
		/// For adding a 1px margin to the left and top inside edges of the
		/// client area.
		/// </summary>
		internal const int Pad = 1;
		#endregion Fields (static)


		#region Fields
		private SpriteEditorF _feditor;

		private Pen _penGrid;
		private readonly Pen _gridBlack = new Pen(Color.FromArgb(50,    0,   0,   0)); // black w/ 50  alpha
		private readonly Pen _gridWhite = new Pen(Color.FromArgb(180, 255, 255, 255)); // white w/ 180 alpha
		#endregion Fields


		#region Properties
		private XCImage _sprite;
		internal XCImage Sprite
		{
			get { return _sprite; }
			set
			{
				_sprite = value;

				Palid = -1;

				string caption = "Sprite Editor";
				if (_sprite != null)
					caption += " - id " + _sprite.Id;
				_feditor.Text = caption;

				SpriteBytesManager.ReloadBytesTable(_sprite); // this will clear the show-bytes box if null.

				Refresh();
			}
		}

		private int _palid = -1;
		private int Palid
		{
			get { return _palid; }
			set
			{
				if ((_palid = value) != -1)
				{
					_feditor.PrintColorInfo(GetColorInfo(_palid));
				}
				else
					_feditor.ClearColorInfo();
			}
		}

		private bool _grid;
		internal bool Grid
		{
			set
			{
				_grid = value;
				Refresh();
			}
		}

		private int _scale = 10;
		internal int ScaleFactor
		{
			set
			{
				_scale = value;
				Refresh();
			}
		}
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		internal SpritePanel(SpriteEditorF f)
		{
			_feditor = f;

			_penGrid = _gridBlack;

			PckViewForm.PaletteChanged += OnPaletteChanged;
		}
		#endregion cTor


		#region Events (override)
		protected override void OnMouseLeave(EventArgs e)
		{
//			base.OnMouseLeave(e);

			Palid = -1;
		}

		/// <summary>
		/// Handles a mousedown event on the editor-panel.
		/// EditMode.Enabled: changes a clicked pixel's palette-id (color) to
		/// whatever the current 'PaletteId' is in PalettePanel.
		/// EditMode.Locked: changes the 'PaletteId' in the PalettePanel to
		/// whatever a clicked pixel's palette-id (color) is.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);

			if (Sprite != null
				&& e.X > 0 && e.X < XCImage.SpriteWidth  * _scale
				&& e.Y > 0 && e.Y < XCImage.SpriteHeight * _scale)
			{
				int pixelX = e.X / _scale;
				int pixelY = e.Y / _scale;

				int bindataId = pixelY * (Sprite.Bindata.Length / XCImage.SpriteHeight) + pixelX;

				if (bindataId > -1 && bindataId < Sprite.Bindata.Length) // safety.
				{
					switch (SpriteEditorF.Mode)
					{
						case SpriteEditorF.EditMode.Enabled: // paint ->
						{
							int palid = _feditor._fpalette._pnlPalette.Palid;
							if (palid > -1
								&& (palid < PckImage.MarkerRle
									|| _feditor._f.TilePanel.Spriteset.TabwordLength == ResourceInfo.TAB_WORD_LENGTH_0))
							{
								if (palid != (int)Sprite.Bindata[bindataId])
								{
									Sprite.Bindata[bindataId] = (byte)palid;
									Sprite.Sprite = BitmapService.CreateColorized(
																				XCImage.SpriteWidth,
																				XCImage.SpriteHeight,
																				Sprite.Bindata,
																				PckViewForm.Pal.ColorTable);
									Invalidate();
									_feditor._f.TilePanel.Invalidate();

									_feditor._f.Changed = true;
								}
							}
							else
							{
								switch (palid)
								{
									case PckImage.MarkerRle: // #254
									case PckImage.MarkerEos: // #255
										MessageBox.Show(
													this,
													"The colortable values #254 and #255 are reserved"
														+ " as special markers in a .PCK file."
														+ Environment.NewLine + Environment.NewLine
														+ "#254 is used for RLE encoding"
														+ Environment.NewLine
														+ "#255 is the End-of-Sprite marker",
													" Error",
													MessageBoxButtons.OK,
													MessageBoxIcon.Error,
													MessageBoxDefaultButton.Button1,
													0);
										break;
								}
							}
							break;
						}

						case SpriteEditorF.EditMode.Locked: // eye-dropper ->
							_feditor._fpalette._pnlPalette.SelectPaletteId((int)Sprite.Bindata[bindataId]);
							break;
					}
				}
			}
		}

		/// <summary>
		/// Displays the color of any mouseovered paletteId.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseMove(MouseEventArgs e)
		{
//			base.OnMouseMove(e);

			if (Sprite != null)
			{
				if (   e.X > 0 && e.X < XCImage.SpriteWidth  * _scale
					&& e.Y > 0 && e.Y < XCImage.SpriteHeight * _scale)
				{
					int pixelX = e.X / _scale;
					int pixelY = e.Y / _scale;

					int bindataId = pixelY * (Sprite.Bindata.Length / XCImage.SpriteHeight) + pixelX;

					if (bindataId > -1 && bindataId < Sprite.Bindata.Length) // safety.
					{
						int palid = Sprite.Bindata[bindataId];
						if (palid != Palid)
							Palid = palid;
					}
					else
						Palid = -1;
				}
				else
					Palid = -1;
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
//			base.OnPaint(e);

			var graphics = e.Graphics;
			graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

			if (Sprite != null)
			{
				for (int y = 0; y != XCImage.SpriteHeight; ++y)
				for (int x = 0; x != XCImage.SpriteWidth;  ++x)
					graphics.FillRectangle(
										new SolidBrush(Sprite.Sprite.GetPixel(x, y)),
										x * _scale,
										y * _scale,
											_scale,
											_scale);
			}


			if (_grid && _scale != 1)
			{
				for (int x = 0; x != XCImage.SpriteWidth; ++x) // vertical lines
					graphics.DrawLine(
									_penGrid,
									x * _scale + Pad,
									0,
									x * _scale + Pad,
									XCImage.SpriteHeight * _scale);

				for (int y = 0; y != XCImage.SpriteHeight; ++y) // horizontal lines
					graphics.DrawLine(
									_penGrid,
									0,
									y * _scale + Pad,
									XCImage.SpriteWidth * _scale,
									y * _scale + Pad);
			}


//			var p0 = new Point(0,     1); // draw a 1px border around the panel ->
//			var p1 = new Point(Width, 1);
//			var p2 = new Point(Width, Height);
//			var p3 = new Point(1,     Height);
//			var p4 = new Point(1,     1);

			var p0 = new Point( // draw a 1px border around the image ->
							0,
							1);
			var p1 = new Point(
							XCImage.SpriteWidth  * _scale + Pad,
							1);
			var p2 = new Point(
							XCImage.SpriteWidth  * _scale + Pad,
							XCImage.SpriteHeight * _scale + Pad);
			var p3 = new Point(
							1,
							XCImage.SpriteHeight * _scale + Pad);
			var p4 = new Point(
							1,
							1);

			var path = new GraphicsPath();

			path.AddLine(p0, p1);
			path.AddLine(p1, p2);
			path.AddLine(p2, p3);
			path.AddLine(p3, p4);

			graphics.DrawPath(Pens.Black, path);
		}
		#endregion Events (override)


		#region Events
		private void OnPaletteChanged()
		{
			Refresh();
		}
		#endregion Events


		#region Methods
		/// <summary>
		/// Gets a string of information that describes either a pixel's or a
		/// palette-swatch's color.
		/// @note Palette ids #254 and #255 are invalid (as colors) in PCK files
		/// because the RLE-compression algorithm uses them as markers with
		/// different meanings.
		/// </summary>
		/// <param name="palid">a palette-id to get info about</param>
		/// <returns>string of color-info</returns>
		internal string GetColorInfo(int palid)
		{
			if (palid != -1)
			{
				string text = String.Format(
										System.Globalization.CultureInfo.CurrentCulture,
										"id:{0} (0x{0:X2})",
										palid);

				var color = PckViewForm.Pal[palid];
				text += String.Format(
									System.Globalization.CultureInfo.CurrentCulture,
									" r:{0} g:{1} b:{2} a:{3}",
									color.R,
									color.G,
									color.B,
									color.A);

				switch (palid)
				{
					case Palette.TranId: // #0
						text += " [transparent]";
						break;

					case PckImage.MarkerRle: // #254
					case PckImage.MarkerEos: // #255
						if (_feditor._f.TilePanel.Spriteset.TabwordLength != ResourceInfo.TAB_WORD_LENGTH_0)
						{
							text += " [invalid]";
						}
						break;
				}
				return text;
			}
			return String.Empty;
		}

		internal void InvertGridColor(bool invert)
		{
			_penGrid = (invert) ? _gridWhite
								: _gridBlack;
			Refresh();
		}
		#endregion Methods
	}
}