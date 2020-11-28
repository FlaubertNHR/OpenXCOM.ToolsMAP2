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
	/// Note that the draw-scale is 4x.
	/// </summary>
	internal sealed class ScanGViewer
		:
			Form
	{
		#region Fields
		private MapFile _file;

		private int[,]  _icons;
		private Palette _pal;

		private int  Level;
		private bool SingleLevel;
		#endregion Fields


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		internal ScanGViewer(MapFile file)
		{
			InitializeComponent();

			Level = (_file = file).Level;
			Text = GetTitle();

			SetResources();

			if (!RegistryInfo.RegisterProperties(this))	// NOTE: Respect only left and top props;
			{											// let ClientSize deter width and height.
				Left = 200;
				Top  = 100;
			}

			ClientSize = new Size(
								_file.MapSize.Cols * 16 + 2,
								_file.MapSize.Rows * 16 + 2);
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
		/// Overrides the FormClosed eventhandler.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnFormClosed(FormClosedEventArgs e)
		{
			RegistryInfo.UpdateRegistry(this);

			MenuManager.DecheckScanG();
			MainViewF.ScanG = null;
		}

		/// <summary>
		/// Closes the screen on [Esc] keyup event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnKeyUp(KeyEventArgs e)
		{
			if (e.KeyData == Keys.Escape)
				Close();
		}

		/// <summary>
		/// Overrides the MouseWheel eventhandler.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseWheel(MouseEventArgs e)
		{
			int level = Level;

			if (e.Delta < 0 && Level != 0)
			{
				--Level;
			}
			else if (e.Delta > 0 && Level != _file.MapSize.Levs - 1)
			{
				++Level;
			}

			if (level != Level)
			{
				Text = GetTitle();
				pnl_ScanG.Invalidate();
			}
		}
		#endregion Events (override)


		#region Events (panel)
		/// <summary>
		/// MouseDoubleClick handler for the panel. LMB toggles between
		/// single-level view and multilevel view. RMB reloads ScanG.Dat file.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void panel_OnMouseDoubleClick(object sender, MouseEventArgs e)
		{
			switch (e.Button)
			{
				case MouseButtons.Left:
					SingleLevel = !SingleLevel;
					Text = GetTitle();
					pnl_ScanG.Invalidate();
					break;

				case MouseButtons.Right:
				{
					string result, title;
					MessageBoxIcon icon;

					if (_file.Descriptor.GroupType == GameType.Tftd)
					{
						if (ResourceInfo.LoadScanGtftd(SharedSpace.GetShareString(SharedSpace.ResourceDirectoryTftd)))
						{
							_icons = ResourceInfo.ScanGtftd;

							result = "SCANG.DAT reloaded.";
							title  = " Info";
							icon   = MessageBoxIcon.None;
						}
						else
						{
							_icons = null;

							result = "SCANG.DAT failed to reload. Take the red pill.";
							title  = " Error";
							icon   = MessageBoxIcon.Error;
						}
					}
					else if (ResourceInfo.LoadScanGufo(SharedSpace.GetShareString(SharedSpace.ResourceDirectoryUfo)))
					{
						_icons = ResourceInfo.ScanGufo;

						result = "SCANG.DAT reloaded.";
						title  = " Info";
						icon   = MessageBoxIcon.None;
					}
					else
					{
						_icons = null;

						result = "SCANG.DAT failed to reload. Take the red pill.";
						title  = " Error";
						icon   = MessageBoxIcon.Error;
					}
					ShowReloadResult(result, title, icon);

					// NOTE: Invalidate/refresh is not needed apparently.
					break;
				}
			}
		}

		/// <summary>
		/// Shows a messagebox with the reload-result.
		/// </summary>
		/// <param name="result"></param>
		/// <param name="title"></param>
		/// <param name="icon"></param>
		private void ShowReloadResult(
				string result,
				string title,
				MessageBoxIcon icon)
		{
			MessageBox.Show(
						this,
						result,
						title,
						MessageBoxButtons.OK,
						icon,
						MessageBoxDefaultButton.Button1,
						0);
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
										_file.MapSize.Cols * 16,
										_file.MapSize.Rows * 16,
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


						int zStart;
						if (SingleLevel) zStart = Level;
						else             zStart = _file.MapSize.Levs - 1;

						int iconsTotal = _icons.Length / ScanGicon.Length_ScanG;

						for (int z = zStart; z >= Level; --z)
						for (int y = 0; y != _file.MapSize.Rows; ++y)
						for (int x = 0; x != _file.MapSize.Cols; ++x)
						{
							ptrPixel = pos + (x * 16) + (y * 16 * data.Stride);

							tile = _file[x,y,z];

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
									if (palid != Palette.TranId)
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
									if (palid != Palette.TranId)
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
									if (palid != Palette.TranId)
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
									if (palid != Palette.TranId)
									{
										ptr = ptrPixel + (i % 16) + (i / 16 * data.Stride);
										*ptr = (byte)palid;
									}
								}
							}
						}
					}
					icon.UnlockBits(data);

					icon.Palette = _pal.ColorTable;

					var spriteAttributes = new ImageAttributes();
					if (MainViewF.Optionables.SpriteShadeEnabled)
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


		#region Methods
		/// <summary>
		/// Gets a title-string.
		/// </summary>
		/// <returns></returns>
		private string GetTitle()
		{
			return "ScanG - "
				 + "L" + (_file.MapSize.Levs - Level)
				 + (SingleLevel ? " - 1 layer" : String.Empty);
		}

		/// <summary>
		/// Invalidates the panel.
		/// </summary>
		internal void InvalidatePanel()
		{
			pnl_ScanG.Invalidate();
		}

		/// <summary>
		/// Loads a Mapfile.
		/// </summary>
		/// <param name="base"></param>
		internal void LoadMapfile(MapFile file)
		{
			Level = (_file = file).Level;
			Text = GetTitle();

			SetResources();

			ClientSize = new Size(
								_file.MapSize.Cols * 16 + 2,
								_file.MapSize.Rows * 16 + 2);
			Refresh(); // req'd.
		}

		/// <summary>
		/// Sets the iconset and palette.
		/// </summary>
		private void SetResources()
		{
			if (_file.Descriptor.GroupType == GameType.Tftd)
			{
				_icons = ResourceInfo.ScanGtftd;
				_pal   = Palette.TftdBattle;
			}
			else
			{
				_icons = ResourceInfo.ScanGufo;
				_pal   = Palette.UfoBattle;
			}
		}
		#endregion Methods


		#region Designer
		private System.ComponentModel.Container components = null;

		private BufferedPanel pnl_ScanG;


		/// <summary>
		/// Cleans up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
				components.Dispose();

			base.Dispose(disposing);
		}


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
			this.pnl_ScanG.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.panel_OnMouseDoubleClick);
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
