using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using DSShared;
using DSShared.Controls;

using XCom;


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
		private readonly SpriteEditorF _feditor;
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

				string title = "Sprite Editor";
				if (_sprite != null)
					title += " - id " + _sprite.Id;
				_feditor.Text = title;

				ByteTableManager.ReloadTable(_sprite, _feditor._f.SetType);

				Refresh();
			}
		}

		private int _palid = -1;
		private int Palid
		{
			get { return _palid; }
			set
			{
				_feditor.PrintPixelColor(GetColorInfo(_palid = value));
			}
		}

		private Pen _penGrid;
		internal Pen PenGrid
		{
			set
			{
				_penGrid = value;
				Invalidate();
			}
		}

		private int _scale = 10;
		internal int ScaleFactor
		{
			set
			{
				_scale = value;
				Invalidate();
			}
		}
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		internal SpritePanel(SpriteEditorF f)
		{
			Dock = DockStyle.Fill;

			_feditor = f;

			PckViewF.PaletteChanged += OnPaletteChanged;
		}
		#endregion cTor


		#region Events (override)
		/// <summary>
		/// Clears <c><see cref="Palid"/></c> when the cursor leaves this panel.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseLeave(EventArgs e)
		{
			Palid = -1;
		}

		/// <summary>
		/// Handles a <c>MouseDown</c> event on this <c>SpritePanel</c>.
		/// </summary>
		/// <param name="e"></param>
		/// <remarks><c><see cref="EditMode.Enabled">EditMode.Enabled</see></c>:
		/// changes a clicked pixel's palette-id (color) to whatever the current
		/// <c><see cref="PalettePanel.Palid">PalettePanel.Palid</see></c>.
		/// 
		/// <c><see cref="EditMode.Locked">EditMode.Locked</see></c>: changes
		/// the <c><see cref="PalettePanel.Palid">PalettePanel.Palid</see></c>
		/// to whatever a clicked pixel's palette-id (color) is.</remarks>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);

			if (e.Button == MouseButtons.Left
				&& Sprite != null
				&& e.X > 0 && e.X < _feditor._f.SpriteWidth  * _scale
				&& e.Y > 0 && e.Y < _feditor._f.SpriteHeight * _scale)
			{
				int pixelX = e.X / _scale;
				int pixelY = e.Y / _scale;

				byte[] bindata = Sprite.GetBindata();

				int binid = pixelY * (bindata.Length / _feditor._f.SpriteHeight) + pixelX;

				if (binid > -1 && binid < bindata.Length) // safety.
				{
					switch (SpriteEditorF.Mode)
					{
						case EditMode.Enabled: // paint ->
							if (_feditor._f.SetType != Spriteset.SsType.LoFT)
							{
								int palid = _feditor._fpalette.PalPanel.Palid;
								if (palid > -1)
								{
									if (palid < PckSprite.MarkerRle
										|| _feditor._f.TilePanel.Spriteset.TabwordLength == SpritesetManager.TAB_WORD_LENGTH_0)
									{
										if (palid != (int)bindata[binid])
										{
											bindata[binid] = (byte)palid;
											Bitmap sprite = SpriteService.CreateSprite(
																					_feditor._f.SpriteWidth,
																					_feditor._f.SpriteHeight,
																					bindata,
																					Sprite.Pal.Table); //_feditor._f.Pal.Table
											Sprite.Dispose();
											Sprite.Sprite = sprite;

											Invalidate();
											_feditor._f.TilePanel.Invalidate();

											_feditor._f.Changed = true;

											ByteTableManager.ReloadTable(Sprite, _feditor._f.SetType);
										}
									}
									else
									{
										switch (palid)
										{
											case PckSprite.MarkerRle: // #254
											case PckSprite.MarkerEos: // #255
												using (var f = new Infobox(
																		"Error",
																		Infobox.SplitString("The colortable values #254 and #255"
																				+ " are reserved as special markers in a .PCK file."),
																		"#254 - RLE" + Environment.NewLine
																	  + "#255 - End-of-Sprite",
																		InfoboxType.Error))
												{
													f.ShowDialog(this);
												}
												break;
										}
									}
								}
							}
							else // is LoFT
							{
								if (bindata[binid] != Palette.LoFTclear)
									bindata[binid]  = Palette.LoFTclear;
								else
									bindata[binid]  = Palette.LoFTSolid;

								Bitmap sprite = SpriteService.CreateSprite(
																		_feditor._f.SpriteWidth,
																		_feditor._f.SpriteHeight,
																		bindata,
																		Sprite.Pal.Table); //Palette.Binary.Table
								Sprite.Dispose();
								Sprite.Sprite = sprite;

								Invalidate();
								_feditor._f.TilePanel.Invalidate();

								_feditor._f.Changed = true;

								ByteTableManager.ReloadTable(Sprite, _feditor._f.SetType);
							}
							break;

						case EditMode.Locked: // eye-dropper ->
							_feditor._fpalette.PalPanel.SelectPalid(bindata[binid]);
							break;
					}
				}
			}
		}

		/// <summary>
		/// Displays the color of any mouseovered pixel.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (Sprite != null)
			{
				if (   e.X > 0 && e.X < _feditor._f.SpriteWidth  * _scale
					&& e.Y > 0 && e.Y < _feditor._f.SpriteHeight * _scale)
				{
					int pixelX = e.X / _scale;
					int pixelY = e.Y / _scale;

					byte[] bindata = Sprite.GetBindata();

					int binid = pixelY * (bindata.Length / _feditor._f.SpriteHeight) + pixelX;

					if (binid > -1 && binid < bindata.Length) // safety.
					{
						int palid = bindata[binid];
						if (palid != Palid)
							Palid = palid;

						return;
					}
				}
				Palid = -1;
			}
		}


		/// <summary>
		/// Handles the Paint event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{
			var graphics = e.Graphics;
			graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

			if (Sprite != null)
			{
				byte[] bindata = Sprite.GetBindata();

				if (_feditor._f.SpriteShade >= PckViewF.SPRITESHADE_ON
					&& _feditor._f.SetType != Spriteset.SsType.LoFT)
				{
					for (int y = 0; y != _feditor._f.SpriteHeight; ++y)
					for (int x = 0; x != _feditor._f.SpriteWidth;  ++x)
					{
						int palid = bindata[y * _feditor._f.SpriteWidth + x];
						using (var brush = new SolidBrush(Shade(Sprite.Pal.Table.Entries[palid])))
						{
							graphics.FillRectangle(
												brush,
												x * _scale,
												y * _scale,
													_scale,
													_scale);
						}
					}
				}
				else
				{
					for (int y = 0; y != _feditor._f.SpriteHeight; ++y)
					for (int x = 0; x != _feditor._f.SpriteWidth;  ++x)
					{
						int palid = bindata[y * _feditor._f.SpriteWidth + x];
						using (var brush = new SolidBrush(Sprite.Pal.Table.Entries[palid]))
						{
							graphics.FillRectangle(
												brush,
												x * _scale,
												y * _scale,
													_scale,
													_scale);
						}
					}
				}
			}


			if (_penGrid != null && _scale > 2)
			{
				for (int x = 0; x != _feditor._f.SpriteWidth; ++x) // vertical lines
					graphics.DrawLine(
									_penGrid,
									x * _scale + Pad,
									0,
									x * _scale + Pad,
									_feditor._f.SpriteHeight * _scale);

				for (int y = 0; y != _feditor._f.SpriteHeight; ++y) // horizontal lines
					graphics.DrawLine(
									_penGrid,
									0,
									y * _scale + Pad,
									_feditor._f.SpriteWidth * _scale,
									y * _scale + Pad);
			}


			var p0 = new Point( // draw a 1px border around the image ->
							0,
							1);
			var p1 = new Point(
							_feditor._f.SpriteWidth  * _scale + Pad,
							1);
			var p2 = new Point(
							_feditor._f.SpriteWidth  * _scale + Pad,
							_feditor._f.SpriteHeight * _scale + Pad);
			var p3 = new Point(
							1,
							_feditor._f.SpriteHeight * _scale + Pad);
			var p4 = new Point(
							1,
							1);

			using (var path = new GraphicsPath())
			{
				path.AddLine(p0, p1);
				path.AddLine(p1, p2);
				path.AddLine(p2, p3);
				path.AddLine(p3, p4);

				graphics.DrawPath(Pens.Black, path);
			}
		}
		#endregion Events (override)


		#region Events
		/// <summary>
		/// Handler for
		/// <c><see cref="PckViewF.PaletteChanged">PckViewF.PaletteChanged</see></c>.
		/// Invalidates this panel.
		/// </summary>
		private void OnPaletteChanged()
		{
			if (Visible) Invalidate();
		}
		#endregion Events


		#region Methods (static)
		/// <summary>
		/// Adjusts the gamma-value of each pixel in
		/// <c><see cref="OnPaint()">OnPaint()</see></c>.
		/// Also called by <c><see cref="PalettePanel"/>.OnPaint()</c>.
		/// </summary>
		/// <param name="color"></param>
		/// <returns></returns>
		internal static Color Shade(Color color)
		{
			double red   = (double)color.R / 255;
			double green = (double)color.G / 255;
			double blue  = (double)color.B / 255;

			double factor = (double)PckViewF.SpriteShadeFloat + 1.65;	// <- is arbitrary - it would help to know the actual
																		//    algorithm used by ImageAttributes.SetGamma()
			return Color.FromArgb(
							color.A,
							(int)(Math.Pow(red,   1 / factor) * 255),
							(int)(Math.Pow(green, 1 / factor) * 255),
							(int)(Math.Pow(blue,  1 / factor) * 255));
		}
		#endregion Methods (static)


		#region Methods
		/// <summary>
		/// Gets a string of information that describes either a pixel's or a
		/// palette-swatch's color.
		/// </summary>
		/// <param name="palid">a palette-id to get info about</param>
		/// <returns>string of color-info</returns>
		/// <remarks>Palette ids #254 and #255 are invalid (as colors) in PCK
		/// files because the RLE-compression algorithm uses them as markers
		/// with different meanings.</remarks>
		internal string GetColorInfo(int palid)
		{
			if (palid != -1)
			{
				string text = String.Format(
										"id:{0} (0x{0:X2})",
										palid);

				Color color = _feditor._f.GetCurrentPalette()[palid];
				text += " [a:" + color.A + "]"
					  +  " r:" + color.R
					  +  " g:" + color.G
					  +  " b:" + color.B;

				switch (palid)
				{
					case Palette.Tid:
						text += " - transparent";
						break;

					case PckSprite.MarkerRle:
					case PckSprite.MarkerEos:
						if (_feditor._f.TilePanel.Spriteset.TabwordLength != SpritesetManager.TAB_WORD_LENGTH_0)
						{
							text += " - INVALID";
						}
						break;
				}
				return text;
			}
			return String.Empty;
		}

		/// <summary>
		/// fing jackasses.
		/// </summary>
		internal void Destroy()
		{
			PckViewF.PaletteChanged -= OnPaletteChanged;

//			base.Dispose(); // <- I *still* don't know if that is a Good Thing or not.
		}
		#endregion Methods
	}
}
