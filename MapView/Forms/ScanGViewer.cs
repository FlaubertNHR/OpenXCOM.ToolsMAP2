using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

using DSShared;
using DSShared.Controls;

using MapView.Forms.MainView;

using XCom;


namespace MapView
{
	/// <summary>
	/// A simplistic presentation of the XCOM MiniMap.
	/// A ScanG entry is 4x4 pixels and fills a single tile of the Map. The
	/// floor-part is drawn first, followed by the westwall-part, the northwall-
	/// part, and finally the content-part. The MCD-record of each tilepart
	/// references a ScanG entry using this formula:
	///     offset = [21] * 256 + [20] + 35
	/// </summary>
	/// <remarks>The draw-scale is 4x.</remarks>
	internal sealed class ScanGViewer
		:
			Form
	{
		// TODO: Consider inheriting from IObserver.

		private enum Layer
		{ @default, single, locked }


		#region Fields (static)
		private static Layer Aspect = Layer.@default;
		#endregion Fields (static)


		#region Fields
		private MapFile _file;

		private int[,]  _icons;
		private Palette _pal;
		#endregion Fields


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		internal ScanGViewer(MapFile file)
		{
			InitializeComponent();

			if (!RegistryInfo.RegisterProperties(this))	// NOTE: Respect only left and top props;
			{											// let ClientSize deter width and height.
				Left = 200;
				Top  = 100;
			}

			LoadMapfile(file);
		}
		#endregion cTor


		#region Events (override)
		/// <summary>
		/// Overrides the Activated eventhandler.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnActivated(EventArgs e)
		{
			ShowHideManager._zOrder.Remove(this);
			ShowHideManager._zOrder.Add(this);
		}

		/// <summary>
		/// Overrides the FormClosing eventhandler.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			if (!RegistryInfo.FastClose(e.CloseReason))
			{
				RegistryInfo.UpdateRegistry(this);

				_file.LevelSelected -= OnLevelSelected;

				MenuManager.DecheckScanG();
				MainViewF.ScanG = null;
			}
			base.OnFormClosing(e);
		}

		/// <summary>
		/// Overrides the KeyDown eventhandler.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			switch (e.KeyData)
			{
				case Keys.Escape:
				case Keys.Control | Keys.G:
					Close();
					break;

				case Keys.Enter:
					ReloadScanGfile();
					break;

				case Keys.Subtract:
					--_file.Level;
					break;

				case Keys.Add:
					++_file.Level;
					break;

				case Keys.L:
					CycleLayers();
					break;
			}
		}

		/// <summary>
		/// Overrides the MouseWheel eventhandler.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseWheel(MouseEventArgs e)
		{
			if (e.Delta != 0)
			{
				int delta;
				if (MainViewF.Optionables.InvertMousewheel)
					delta = -e.Delta;
				else
					delta =  e.Delta;

				if      (delta >  1) ++_file.Level;
				else if (delta < -1) --_file.Level;
			}
		}
		#endregion Events (override)


		#region Events (panel)
		/// <summary>
		/// MouseClick handler for the panel. RMB cycles among aspects.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void panel_OnMouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
				CycleLayers();
		}

		/// <summary>
		/// Paint handler for the panel.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void panel_OnPaint(object sender, PaintEventArgs e)
		{
			if (_icons != null && _pal != null)
			{
				using (var icon = new Bitmap(
										_file.Cols * 16,
										_file.Rows * 16,
										PixelFormat.Format8bppIndexed))
				{
					var data = icon.LockBits(
										new Rectangle(0,0, icon.Width, icon.Height),
										ImageLockMode.WriteOnly,
										PixelFormat.Format8bppIndexed);
					var start = data.Scan0;

					unsafe
					{
						var pos = (byte*)start.ToPointer();

						for (uint row = 0; row != icon.Height; ++row)
						for (uint col = 0; col != icon.Width;  ++col)
						{
							byte* pixel = pos + col + row * data.Stride;
							*pixel = 15; // fill w/ a dark color.
						}


						byte* ptrPixel, ptr;

						MapTile tile;
						int iconid, palid, j;


						int zStrt, zStop;
						switch (Aspect)
						{
							default: //case Layer.@default
								zStrt = _file.Levs - 1;
								zStop = _file.Level;
								break;

							case Layer.single:
								zStrt =
								zStop = _file.Level;
								break;

							case Layer.locked:
								zStrt = _file.Levs - 1;
								zStop = 0;
								break;
						}

						int iconsTotal = _icons.Length / ScanGicon.Length_ScanG;

						for (int z = zStrt; z >= zStop; --z)
						for (int y = 0; y != _file.Rows; ++y)
						for (int x = 0; x != _file.Cols; ++x)
						{
							ptrPixel = pos + (x * 16) + (y * 16 * data.Stride);

							tile = _file.GetTile(x,y,z);

							if (tile.Floor != null
								&& (iconid = tile.Floor.Record.ScanG) < iconsTotal
								&& iconid > ScanGicon.UNITICON_Max)
							{
								for (int i = 0; i != 256; ++i)
								{
									//   0..  3  #0    4..  7  #1    8.. 11   #2   12.. 15   #3
									//  16.. 19  #0   20.. 23  #1   24.. 27   #2   28.. 31   #3
									//  32.. 35  #0   36.. 39  #1   40.. 43   #2   44.. 47   #3
									//  48.. 51  #0   52.. 55  #1   56.. 59   #2   60.. 63   #3

									//  64.. 67  #4   68.. 71  #5   72.. 75   #6   76.. 79   #7
									//  80.. 83  #4   84.. 87  #5   88.. 91   #6   92.. 95   #7
									//  96.. 99  #4  100..103  #5  104..107   #6  108..111   #7
									// 112..115  #4  116..119  #5  120..123   #6  124..127   #7

									// 128..131  #8  132..135  #9  136..139  #10  140..143  #11
									//
									// etc ......255 #15
									j = ((i / 64) * 4) + ((i % 16) / 4);

									palid = _icons[iconid, j];
									if (palid != Palette.Tid)
									{
										ptr = ptrPixel + (i % 16) + (i / 16 * data.Stride);
										*ptr = (byte)palid;
									}
								}
							}

							if (tile.West != null
								&& (iconid = tile.West.Record.ScanG) < iconsTotal
								&& iconid > ScanGicon.UNITICON_Max)
							{
								for (int i = 0; i != 256; ++i)
								{
									j = ((i / 64) * 4) + ((i % 16) / 4);

									palid = _icons[iconid, j];
									if (palid != Palette.Tid)
									{
										ptr = ptrPixel + (i % 16) + (i / 16 * data.Stride);
										*ptr = (byte)palid;
									}
								}
							}

							if (tile.North != null
								&& (iconid = tile.North.Record.ScanG) < iconsTotal
								&& iconid > ScanGicon.UNITICON_Max)
							{
								for (int i = 0; i != 256; ++i)
								{
									j = ((i / 64) * 4) + ((i % 16) / 4);

									palid = _icons[iconid, j];
									if (palid != Palette.Tid)
									{
										ptr = ptrPixel + (i % 16) + (i / 16 * data.Stride);
										*ptr = (byte)palid;
									}
								}
							}

							if (tile.Content != null
								&& (iconid = tile.Content.Record.ScanG) < iconsTotal
								&& iconid > ScanGicon.UNITICON_Max)
							{
								for (int i = 0; i != 256; ++i)
								{
									j = ((i / 64) * 4) + ((i % 16) / 4);

									palid = _icons[iconid, j];
									if (palid != Palette.Tid)
									{
										ptr = ptrPixel + (i % 16) + (i / 16 * data.Stride);
										*ptr = (byte)palid;
									}
								}
							}
						}
					}
					icon.UnlockBits(data);

					icon.Palette = _pal.Table;

					var spriteAttributes = new ImageAttributes();
					if (MainViewF.Optionables.SpriteShadeEnabled) // TODO: how does UseMono cope w/ this
						spriteAttributes.SetGamma(MainViewF.Optionables.SpriteShadeFloat, ColorAdjustType.Bitmap);

					e.Graphics.DrawImage(
										icon,
										new Rectangle(1,1, icon.Width, icon.Height),
										0,0, icon.Width, icon.Height,
										GraphicsUnit.Pixel,
										spriteAttributes);
				}
			}
		}
		#endregion Events (panel)


		#region Events
		/// <summary>
		/// Fires when the Maplevel changes.
		/// </summary>
		/// <param name="args"></param>
		internal void OnLevelSelected(LevelSelectedArgs args)
		{
			SetTitle();
			pnl_ScanG.Refresh(); // fast Refresh for key-repeats
		}
		#endregion Events


		#region Methods
		/// <summary>
		/// Loads a Mapfile.
		/// </summary>
		/// <param name="file"></param>
		internal void LoadMapfile(MapFile file)
		{
			if (_file != null)
				_file.LevelSelected -= OnLevelSelected;

			(_file = file).LevelSelected += OnLevelSelected;

			SetTitle();

			SetResources();

			ClientSize = new Size(
								_file.Cols * 16 + 2,
								_file.Rows * 16 + 2);

			Refresh(); // req'd. if loading a Mapfile iff this viewer is already instantiated.
		}

		/// <summary>
		/// Sets the iconset and palette.
		/// </summary>
		private void SetResources()
		{
			if (_file.Descriptor.GroupType == GameType.Tftd)
			{
				_icons = SpritesetManager.GetScanGtftd();
				_pal   = Palette.TftdBattle;
			}
			else
			{
				_icons = SpritesetManager.GetScanGufo();
				_pal   = Palette.UfoBattle;
			}
		}

		/// <summary>
		/// Sets the title on the titlebar.
		/// </summary>
		internal void SetTitle()
		{
			int level = _file.Levs - _file.Level;
			if (!MainViewF.Optionables.Base1_z) --level;

			Text = "ScanG - "
				 + "L" + level + " - "
				 + Enum.GetName(typeof(Layer), Aspect);
		}

		/// <summary>
		/// Cycles between default-layer, single-layer, multi-layer views.
		/// </summary>
		private void CycleLayers()
		{
			switch (Aspect)
			{
				case Layer.@default: Aspect = Layer.single;   break;
				case Layer.single:   Aspect = Layer.locked;   break;
				case Layer.locked:   Aspect = Layer.@default; break;
			}
			SetTitle();
			pnl_ScanG.Invalidate();
		}

		/// <summary>
		/// Invalidates the panel.
		/// </summary>
		internal void InvalidatePanel()
		{
			pnl_ScanG.Invalidate();
		}

		/// <summary>
		/// Reloads the ScanG.Dat file.
		/// </summary>
		private void ReloadScanGfile()
		{
			string title, head;
			Infobox.BoxType boxType;

			if (_file.Descriptor.GroupType == GameType.Tftd)
			{
				if (SpritesetManager.LoadScanGtftd(SharedSpace.GetShareString(SharedSpace.ResourceDirectoryTftd)))
				{
					_icons = SpritesetManager.GetScanGtftd();

					title   = "Info";
					head    = "SCANG.DAT reloaded.";
					boxType = Infobox.BoxType.Info;
				}
				else
				{
					_icons = null;

					title   = "Error";
					head    = "SCANG.DAT failed to reload. Take the red pill.";
					boxType = Infobox.BoxType.Error;
				}
			}
			else if (SpritesetManager.LoadScanGufo(SharedSpace.GetShareString(SharedSpace.ResourceDirectoryUfo)))
			{
				_icons = SpritesetManager.GetScanGufo();

				title   = "Info";
				head    = "SCANG.DAT reloaded.";
				boxType = Infobox.BoxType.Info;
			}
			else
			{
				_icons = null;

				title   = "Error";
				head    = "SCANG.DAT failed to reload. Take the red pill.";
				boxType = Infobox.BoxType.Error;
			}

			using (var f = new Infobox(
									title,
									head,
									null,
									boxType))
			{
				f.ShowDialog(this);
			}
			// NOTE: Invalidate/refresh is not needed apparently.
		}
		#endregion Methods


		#region Designer
		private BufferedPanel pnl_ScanG;

		/// <summary>
		/// Required method for Designer support - do not modify the contents of
		/// this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.pnl_ScanG = new DSShared.Controls.BufferedPanel();
			this.SuspendLayout();
			// 
			// pnl_ScanG
			// 
			this.pnl_ScanG.BackColor = System.Drawing.SystemColors.Desktop;
			this.pnl_ScanG.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnl_ScanG.Location = new System.Drawing.Point(0, 0);
			this.pnl_ScanG.Margin = new System.Windows.Forms.Padding(0);
			this.pnl_ScanG.Name = "pnl_ScanG";
			this.pnl_ScanG.Size = new System.Drawing.Size(294, 276);
			this.pnl_ScanG.TabIndex = 0;
			this.pnl_ScanG.Paint += new System.Windows.Forms.PaintEventHandler(this.panel_OnPaint);
			this.pnl_ScanG.MouseClick += new System.Windows.Forms.MouseEventHandler(this.panel_OnMouseClick);
			// 
			// ScanGViewer
			// 
			this.ClientSize = new System.Drawing.Size(294, 276);
			this.Controls.Add(this.pnl_ScanG);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ScanGViewer";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "ScanG";
			this.ResumeLayout(false);

		}
		#endregion Designer
	}
}
