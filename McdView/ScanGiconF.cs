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
		#region Fields (static)
		private const int COLS             = 16;
		private const int ROWS_VISIBLE_Max = 16;
		private const int ICON_WIDTH       = 32;
		private const int ICON_HEIGHT      = 32;
		private const int VERT_TEXT_PAD    = 16;
		private const int HORI_PAD         =  1;

		internal static Point Loc = new Point(-1,-1);
		#endregion Fields (static)


		#region Fields
		private readonly McdviewF _f;

		private readonly int IconId;
		private readonly ColorPalette Pal;

		private readonly VScrollBar Scroller = new VScrollBar();
		private readonly int MaxScrollVal;
		private readonly int TotalHeight;
		private int _scrolloffset;
		#endregion Fields


		#region Properties (override)
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.ExStyle |= 0x02000000; // enable 'WS_EX_COMPOSITED'
				return cp;
			}
		}
		#endregion Properties (override)


		#region cTor
		/// <summary>
		/// Creates a ScanG icon viewer/chooser.
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

			_f = f;
			IconId = iconId;
			Pal = pal;

			Text = " SCANG.DAT"; // TODO: + "ufo"/"tftd"

			Scroller.Dock = DockStyle.Right;
			Scroller.Scroll += OnScroll;
			Controls.Add(Scroller);

			int iconCount = _f.ScanG.Length / 16;

			int w;
			if (iconCount < COLS)
			{
				w = iconCount;
				if (w < 16) w = 16;
			}
			else
				w = COLS;

			w = (w * ICON_WIDTH) + (w * HORI_PAD) - 1 + Scroller.Width;

			int h = (iconCount + COLS - 1) / COLS * (ICON_HEIGHT + VERT_TEXT_PAD);

			TotalHeight = h;

			if (h > (ICON_HEIGHT + VERT_TEXT_PAD) * ROWS_VISIBLE_Max)
				h = (ICON_HEIGHT + VERT_TEXT_PAD) * ROWS_VISIBLE_Max;

			ClientSize = new Size(w, h);

			MaxScrollVal = Scroller.Maximum - (Scroller.LargeChange - 1);
			ScrollIcon();

			blink();
		}
		#endregion cTor


		#region Methods
		private void ScrollIcon()
		{
			int r = IconId / COLS;
			if (r > ROWS_VISIBLE_Max - 1)
			{
				r -= ROWS_VISIBLE_Max - 1;
				r *= ICON_HEIGHT + VERT_TEXT_PAD;
				int h = TotalHeight - ClientSize.Height;
				r = (r * MaxScrollVal + (h - 1)) / h;

				Scroller.Value = r;
			}
		}

		private int _id;
		/// <summary>
		/// Blinks the current iconId text-bg.
		/// </summary>
		private async void blink()
		{
			_id = IconId;

			int tick = Int32.MinValue;
			while (++tick != Int32.MaxValue)
			{
				await System.Threading.Tasks.Task.Delay(McdviewF.PERIOD);

				if (tick % 2 != 0)
					_id = -1;
				else
					_id = IconId;

				Invalidate();
			}
		}
		#endregion Methods


		#region Events (override)
		protected override void OnPaint(PaintEventArgs e)
		{
			var graphics = e.Graphics;
			graphics.PixelOffsetMode   = PixelOffsetMode.Half;
			graphics.InterpolationMode = InterpolationMode.NearestNeighbor;

			var attri = new ImageAttributes();
			if (_f._spriteShadeEnabled)
				attri.SetGamma(_f.SpriteShadeFloat, ColorAdjustType.Bitmap);

			_scrolloffset = (-Scroller.Value * (TotalHeight - ClientSize.Height)) / MaxScrollVal;

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

				// - if assigning a Bitmap's palette it gets cloned
				// - if assigning a standalone palette it gets referenced
				// so yes this higgledy-piggeldy is necessary.
				// The point is to draw icons that are unavailable to terrain-
				// parts with a non-transparent background.
				icon.Palette = Pal;
				ColorPalette pal = icon.Palette; // <- clone Palette
				if (i > 35)
					pal.Entries[Palette.TranId] = Color.Transparent;
				icon.Palette = pal;

				graphics.DrawImage(
								icon,
								new Rectangle(
											x, y + _scrolloffset,
											ICON_WIDTH, ICON_HEIGHT),
								0,0, icon.Width, icon.Height,
								GraphicsUnit.Pixel,
								attri);

				rect = new Rectangle(
								x,
								y + ICON_HEIGHT + _scrolloffset,
								ICON_WIDTH,
								VERT_TEXT_PAD);

				if (i == _id)
					graphics.FillRectangle(
										Colors.BrushHilight,
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

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			if (e.Delta > 0)
			{
				int val0 = Scroller.Value;
				int val = Scroller.Value - Scroller.LargeChange;
				if (val < 0)
					val = 0;
				Scroller.Value = val;

				if (Scroller.Value != val0)
					Invalidate();
			}
			else if (e.Delta < 0)
			{
				int val0 = Scroller.Value;
				int val = Scroller.Value + Scroller.LargeChange;
				if (val > MaxScrollVal)
					val = MaxScrollVal;
				Scroller.Value = val;

				if (Scroller.Value != val0)
					Invalidate();
			}
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			if (   e.X > -1 && e.X < ClientSize.Width
				&& e.Y > -1 && e.Y < ClientSize.Height)
			{
				int id = (e.Y - _scrolloffset) / (ICON_HEIGHT + VERT_TEXT_PAD) * COLS
					   +  e.X / (ICON_WIDTH  + HORI_PAD);

				if (id < _f.ScanG.Length / 16)
				{
					if (id < 35) id = 35;

					_f.SetIcon(id);
					Close();
				}
			}
		}

		/// <summary>
		/// Closes the screen on an Escape or Enter keydown event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Escape:
				case Keys.Enter:
					Close();
					break;
			}
		}

		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			Loc = new Point(Location.X, Location.Y);
			base.OnFormClosing(e);
		}

		protected override void OnLoad(EventArgs e)
		{
			if (Loc.X == -1)
			{
				Location = new Point(_f.Location.X + 170, _f.Location.Y + 30);
			}
			else
				Location = new Point(Loc.X, Loc.Y);

			base.OnLoad(e);
		}
		#endregion Events (override)


		#region Events
		private void OnScroll(object sender, EventArgs e)
		{
			Invalidate();
		}
		#endregion Events


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
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
