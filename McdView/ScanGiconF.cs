using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

using XCom;


namespace McdView
{
	internal sealed class ScanGiconF
		:
			Form
	{
		#region Fields (constant)
		private const int COLS          = 32;
		private const int ICON_WIDTH    = 16;
		private const int ICON_HEIGHT   = 16;
		private const int VERT_TEXT_PAD = 16;
		private const int HORI_PAD      =  1;
		#endregion Fields (constant)


		#region Fields
		private readonly McdviewF _f;

		private int IconId;
		private ColorPalette Pal;
		#endregion Fields


		#region cTor
		/// <summary>
		/// 
		/// </summary>
		/// <param name="f"></param>
		/// <param name="iconId"></param>
		/// <param name="pal"></param>
		internal ScanGiconF(
				McdviewF f,
				int iconId,
				ColorPalette pal)
		{
			InitializeComponent();

			SetStyle(ControlStyles.OptimizedDoubleBuffer
				   | ControlStyles.AllPaintingInWmPaint
				   | ControlStyles.UserPaint
				   | ControlStyles.ResizeRedraw, true);

			_f = f;
			IconId = iconId;
			Pal = pal;

			Text = " SCANG.DAT"; // TODO: + "ufo"/"tftd"

			int icons = _f.ScanG.Length / 16;

			int w;
			if (icons < COLS)
			{
				w = icons;
				if (w < 16) w = 16;
			}
			else
				w = COLS;

			w = (w * ICON_WIDTH) + (w * HORI_PAD) - 1;

			int h = (icons + COLS - 1) / COLS * (ICON_HEIGHT + VERT_TEXT_PAD);

			ClientSize = new Size(w, h);
		}
		#endregion cTor


		#region Events (override)
		protected override void OnPaint(PaintEventArgs e)
		{
			var graphics = e.Graphics;
			graphics.PixelOffsetMode   = PixelOffsetMode.Half;
			graphics.InterpolationMode = InterpolationMode.NearestNeighbor;

			var attri = new ImageAttributes();
			if (_f._spriteShadeEnabled)
				attri.SetGamma(_f.SpriteShadeFloat, ColorAdjustType.Bitmap);

			Rectangle rect;

			int x, y;
			for (int i = 0; i != _f.ScanG.Length / 16; ++i)
			{
				x = (i % COLS) * (ICON_WIDTH  + HORI_PAD);
				y = (i / COLS) * (ICON_HEIGHT + VERT_TEXT_PAD);

				var icon = new Bitmap(
									4,4,
									PixelFormat.Format8bppIndexed);

				var data = icon.LockBits(
									new Rectangle(0,0, icon.Width, icon.Height),
									ImageLockMode.WriteOnly,
									PixelFormat.Format8bppIndexed);
				var start = data.Scan0;

				unsafe
				{
					var pos = (byte*)start.ToPointer();

					int palid;
					for (uint row = 0; row != icon.Height; ++row)
					for (uint col = 0; col != icon.Width;  ++col)
					{
						byte* pixel = pos + col + row * data.Stride;

						palid = _f.ScanG[i, row * 4 + col];
						*pixel = (byte)palid;
					}
				}
				icon.UnlockBits(data);

				ColorPalette pal = Pal; // palettes get copied not referenced ->
				pal.Entries[Palette.TransparentId] = Color.Transparent;
				icon.Palette = pal;

				graphics.DrawImage(
								icon,
								new Rectangle(
											x, y,
											ICON_WIDTH, ICON_HEIGHT),
								0,0, icon.Width, icon.Height,
								GraphicsUnit.Pixel,
								attri);

				rect = new Rectangle(
								x,
								y + ICON_HEIGHT,
								ICON_WIDTH,
								VERT_TEXT_PAD);

				if (i == IconId)
					graphics.FillRectangle(
										McdviewF.BrushHilight,
										rect);

				TextRenderer.DrawText(
									graphics,
									i.ToString(),
									Font,
									rect,
									SystemColors.ControlText,
									McdviewF.FLAGS);
			}
		}


		protected override void OnMouseUp(MouseEventArgs e)
		{
			if (   e.X > -1 && e.X < ClientSize.Width
				&& e.Y > -1 && e.Y < ClientSize.Height)
			{
				int id = e.Y / (ICON_HEIGHT + VERT_TEXT_PAD) * COLS
					   + e.X / (ICON_WIDTH  + HORI_PAD);

				if (id < _f.ScanG.Length / 16)
				{
					if (id < 35) id = 35;

					_f.SetIcon(id);
					Close();
				}
			}
		}



		/// <summary>
		/// Closes the screen on an Escape keydown event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
				Close();
		}
		#endregion Events (override)


		#region Designer
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
				components.Dispose();

			base.Dispose(disposing);
		}

		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The
		/// Forms designer might not be able to load this method if it was
		/// changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.SuspendLayout();
			// 
			// ScanGiconF
			// 
			this.ClientSize = new System.Drawing.Size(494, 276);
			this.Font = new System.Drawing.Font("Verdana", 5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ScanGiconF";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "iconset";
			this.ResumeLayout(false);

		}
		#endregion Designer
	}
}
