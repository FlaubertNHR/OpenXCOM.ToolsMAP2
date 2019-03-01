using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

using XCom.Interfaces;


namespace McdView
{
	internal sealed class SpritesetF
		:
			Form
	{
		#region Fields (constant)
		const int COLS = 16;
		const int VERT_TEXT_PAD = 16;
		#endregion Fields (constant)


		#region Fields
		private readonly McdviewF _f;

		private int Pos;
		private int SpriteId;
		#endregion Fields


		#region cTor
		/// <summary>
		/// 
		/// </summary>
		/// <param name="f"></param>
		/// <param name="pos"></param>
		/// <param name="spriteId"></param>
		internal SpritesetF(
				McdviewF f,
				int pos,
				int spriteId)
		{
			InitializeComponent();

			SetStyle(ControlStyles.OptimizedDoubleBuffer
				   | ControlStyles.AllPaintingInWmPaint
				   | ControlStyles.UserPaint
				   | ControlStyles.ResizeRedraw, true);

			_f = f;
			Pos = pos;
			SpriteId = spriteId;

			Text = " " + _f.Label + " - phase " + (Pos + 1);

			int w;
			if (_f.Spriteset.Count < 16)
			{
				w = _f.Spriteset.Count;
				if (w < 8) w = 8;
			}
			else
				w = 16;

			w *= XCImage.SpriteWidth32;

			int h = (_f.Spriteset.Count + COLS - 1) / COLS * (XCImage.SpriteHeight40 + VERT_TEXT_PAD);

			ClientSize = new Size(w, h);
		}
		#endregion cTor


		#region Events (override)
		protected override void OnPaint(PaintEventArgs e)
		{
			//base.OnPaint(e);

			var graphics = e.Graphics;
			graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

			var attri = new ImageAttributes();
			if (_f._spriteShadeEnabled)
				attri.SetGamma(_f.SpriteShadeFloat, ColorAdjustType.Bitmap);

			Rectangle rect;

			int x, y;
			for (int i = 0; i != _f.Spriteset.Count; ++i)
			{
				x = (i % COLS) *  XCImage.SpriteWidth32;
				y = (i / COLS) * (XCImage.SpriteHeight40 + VERT_TEXT_PAD);

				graphics.DrawImage(
								_f.Spriteset[i].Sprite,
								new Rectangle(
											x,
											y,
											XCImage.SpriteWidth32,
											XCImage.SpriteHeight40),
								0, 0, XCImage.SpriteWidth32, XCImage.SpriteHeight40,
								GraphicsUnit.Pixel,
								attri);

				rect = new Rectangle(
								x,
								y + XCImage.SpriteHeight40,
								XCImage.SpriteWidth32,
								VERT_TEXT_PAD);

				if (i == SpriteId)
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
		#endregion Events (override)


		#region Events
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
		#endregion Events


		#region Designer
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be
		/// disposed; otherwise, false.</param>
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
			// SpritesetF
			// 
			this.ClientSize = new System.Drawing.Size(494, 276);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SpritesetF";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "spriteset";
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
			this.ResumeLayout(false);

		}
		#endregion Designer
	}
}
