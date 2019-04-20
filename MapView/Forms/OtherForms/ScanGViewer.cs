using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

using XCom;
using XCom.Interfaces.Base;


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
		internal MapFileBase _base;

		private int[,]  _icons;
		private Palette _pal;

		private int  Level;
		private bool SingleLevel;
		#endregion


		#region Properties (override)
		/// <summary>
		/// This works great. Absolutely kills flicker on redraws.
		/// </summary>
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.ExStyle |= 0x02000000;
				return cp;
			}
		}
		#endregion


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		internal ScanGViewer(MapFileBase @base)
		{
			InitializeComponent();

			SetStyle(ControlStyles.OptimizedDoubleBuffer	// perhaps this should be set on the Panel
				   | ControlStyles.AllPaintingInWmPaint		// but it doesn't like that
				   | ControlStyles.UserPaint
				   | ControlStyles.ResizeRedraw, true);


			_base = @base;
			Level = _base.Level;
			Text = GetTitle();

			if (_base.Descriptor.Pal == Palette.TftdBattle)
			{
				_icons = ResourceInfo.ScanGtftd;
				_pal   = Palette.TftdBattle;
			}
			else
			{
				_icons = ResourceInfo.ScanGufo;
				_pal   = Palette.UfoBattle;
			}

			ClientSize = new Size(
								_base.MapSize.Cols * 16 + 2,
								_base.MapSize.Rows * 16 + 2);
		}
		#endregion


		#region Events
		private void OnFormClosed(object sender, FormClosedEventArgs e)
		{
			XCMainWindow.that.UncheckScanG();
			XCMainWindow.ScanG = null;

			Dispose();
		}

		/// <summary>
		/// Closes the screen on an Escape keydown event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
				Close();
		}

		/// <summary>
		/// Paint handler for the panel.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnPaint(object sender, PaintEventArgs e)
		{
			if (_icons == null || _pal == null)
				return;

			var graphics = e.Graphics;

			var overlay = XCMainWindow.that.MainViewUnderlay.MainViewOverlay;

			var spriteAttributes = new ImageAttributes();
			if (overlay._spriteShadeEnabled)
				spriteAttributes.SetGamma(overlay.SpriteShadeLocal, ColorAdjustType.Bitmap);


			var pic = new Bitmap(
							_base.MapSize.Cols * 16,
							_base.MapSize.Rows * 16,
							PixelFormat.Format8bppIndexed);

			var data = pic.LockBits(
								new Rectangle(0, 0, pic.Width, pic.Height),
								ImageLockMode.WriteOnly,
								PixelFormat.Format8bppIndexed);
			var start = data.Scan0;

			unsafe
			{
				var pos = (byte*)start.ToPointer();

				for (uint row = 0; row != pic.Height; ++row)
				for (uint col = 0; col != pic.Width;  ++col)
				{
					byte* pixel = pos + col + row * data.Stride;
					*pixel = 15; // fill w/ a dark color.
				}


				byte* ptrPixel, ptr;

				XCMapTile tile;
				int iconid, palid, j;


				int zStart;
				if (SingleLevel) zStart = Level;
				else             zStart = _base.MapSize.Levs - 1;

				int iconsTotal = _icons.Length / 16;

				for (int z = zStart; z >= Level; --z)
				for (int y = 0; y != _base.MapSize.Rows; ++y)
				for (int x = 0; x != _base.MapSize.Cols; ++x)
				{
					ptrPixel = pos + (x * 16) + (y * 16 * data.Stride);

					tile = _base[y,x,z] as XCMapTile;

					if (tile.Floor != null
						&& (iconid = tile.Floor.Record.ScanG) < iconsTotal)
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
							if (palid != Palette.TransparentId)
							{
								ptr = ptrPixel + (i % 16) + (i / 16 * data.Stride);
								*ptr = (byte)palid;
							}
						}
					}

					if (tile.West != null
						&& (iconid = tile.West.Record.ScanG) < iconsTotal)
					{
						for (int i = 0; i != 256; ++i)
						{
							j = ((i / 64) * 4) + ((i % 16) / 4);

							palid = _icons[iconid, j];
							if (palid != Palette.TransparentId)
							{
								ptr = ptrPixel + (i % 16) + (i / 16 * data.Stride);
								*ptr = (byte)palid;
							}
						}
					}

					if (tile.North != null
						&& (iconid = tile.North.Record.ScanG) < iconsTotal)
					{
						for (int i = 0; i != 256; ++i)
						{
							j = ((i / 64) * 4) + ((i % 16) / 4);

							palid = _icons[iconid, j];
							if (palid != Palette.TransparentId)
							{
								ptr = ptrPixel + (i % 16) + (i / 16 * data.Stride);
								*ptr = (byte)palid;
							}
						}
					}

					if (tile.Content != null
						&& (iconid = tile.Content.Record.ScanG) < iconsTotal)
					{
						for (int i = 0; i != 256; ++i)
						{
							j = ((i / 64) * 4) + ((i % 16) / 4);

							palid = _icons[iconid, j];
							if (palid != Palette.TransparentId)
							{
								ptr = ptrPixel + (i % 16) + (i / 16 * data.Stride);
								*ptr = (byte)palid;
							}
						}
					}
				}
			}
			pic.UnlockBits(data);

			pic.Palette = _pal.ColorTable;

			graphics.DrawImage(
							pic,
							new Rectangle(1, 1, pic.Width, pic.Height),
							0, 0, pic.Width, pic.Height,
							GraphicsUnit.Pixel,
							spriteAttributes);
		}
		#endregion


		#region Events (override)
		protected override void OnMouseWheel(MouseEventArgs e)
		{
			int level = Level;

			if (e.Delta < 0 && Level != 0)
			{
				--Level;
			}
			else if (e.Delta > 0 && Level != _base.MapSize.Levs - 1)
			{
				++Level;
			}

			if (level != Level)
			{
				Text = GetTitle();
				pnl_ScanG.Invalidate();
			}
		}

		/// <summary>
		/// MouseDoubleClick handler for the panel. LMB toggles between
		/// single-level view and multilevel view. RMB reloads ScanG.Dat file.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMouseDoubleClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				SingleLevel = !SingleLevel;
				Text = GetTitle();
				pnl_ScanG.Invalidate();
			}
			else if (e.Button == MouseButtons.Right)
			{
				string result, title;
				MessageBoxIcon icon;

				var shared = SharedSpace.Instance;

				if (_base.Descriptor.Pal == Palette.TftdBattle)
				{
					if (ResourceInfo.LoadScanGtftd(shared.GetShare(SharedSpace.ResourceDirectoryTftd)))
					{
						_icons = ResourceInfo.ScanGtftd;

						result = "SCANG.DAT has reloaded.";
						title  = "Info";
						icon   = MessageBoxIcon.Information;
					}
					else
					{
						result = "SCANG.DAT failed to reload.";
						title  = "Error";
						icon   = MessageBoxIcon.Error;
					}
				}
				else if (ResourceInfo.LoadScanGufo(shared.GetShare(SharedSpace.ResourceDirectoryUfo)))
				{
					_icons = ResourceInfo.ScanGufo;

					result = "SCANG.DAT has reloaded.";
					title  = "Info";
					icon   = MessageBoxIcon.Information;
				}
				else
				{
					result = "SCANG.DAT failed to reload.";
					title  = "Error";
					icon   = MessageBoxIcon.Error;
				}
				ShowReloadResult(result, title, icon);
				// NOTE: invalidate/refresh is not needed apparently.
			}
		}

		private void ShowReloadResult(
				string result,
				string title,
				MessageBoxIcon icon)
		{
			MessageBox.Show(
						result,
						title,
						MessageBoxButtons.OK,
						icon,
						MessageBoxDefaultButton.Button1,
						0);
		}
		#endregion


		#region Methods
		private string GetTitle()
		{
			return " ScanG - "
				 + "L " + (_base.MapSize.Levs - Level)
				 + (SingleLevel ? " - 1" : String.Empty);
		}

		internal void LoadMapfile(MapFileBase @base)
		{
			_base = @base;
			ClientSize = new Size(
								_base.MapSize.Cols * 16 + 2,
								_base.MapSize.Rows * 16 + 2);
			Level = _base.Level;
			Text = GetTitle();

			Invalidate();
		}

		internal void InvalidatePanel()
		{
			pnl_ScanG.Invalidate();
		}
		#endregion


		#region Windows Form Designer generated code
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
				components.Dispose();

			base.Dispose(disposing);
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.pnl_ScanG = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// pnl_ScanG
			// 
			this.pnl_ScanG.BackColor = System.Drawing.SystemColors.Desktop;
			this.pnl_ScanG.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnl_ScanG.Location = new System.Drawing.Point(0, 0);
			this.pnl_ScanG.Margin = new System.Windows.Forms.Padding(0);
			this.pnl_ScanG.Name = "pnl_ScanG";
			this.pnl_ScanG.Size = new System.Drawing.Size(292, 274);
			this.pnl_ScanG.TabIndex = 0;
			this.pnl_ScanG.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPaint);
			this.pnl_ScanG.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.OnMouseDoubleClick);
			// 
			// ScanGViewer
			// 
			this.ClientSize = new System.Drawing.Size(292, 274);
			this.Controls.Add(this.pnl_ScanG);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.Name = "ScanGViewer";
			this.ShowIcon = false;
			this.Text = " ScanG";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnFormClosed);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
			this.ResumeLayout(false);

		}
		#endregion

		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Panel pnl_ScanG;
	}
}
