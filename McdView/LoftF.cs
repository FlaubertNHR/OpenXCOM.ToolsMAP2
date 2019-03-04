﻿using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;


namespace McdView
{
	internal sealed class LoftF
		:
			Form
	{
		#region Fields (constant)
		private const int COLS          = 32;
		private const int LOFT_WIDTH    = 16;
		private const int LOFT_HEIGHT   = 16;
		private const int VERT_TEXT_PAD = 16;
		private const int HORI_PAD      =  1;
		#endregion Fields (constant)


		#region Fields
		private readonly McdviewF _f;

		private int LoftId;
		#endregion Fields


		#region cTor
		/// <summary>
		/// 
		/// </summary>
		/// <param name="f"></param>
		/// <param name="loftId"></param>
		internal LoftF(
				McdviewF f,
				int loftId)
		{
			InitializeComponent();

			SetStyle(ControlStyles.OptimizedDoubleBuffer
				   | ControlStyles.AllPaintingInWmPaint
				   | ControlStyles.UserPaint
				   | ControlStyles.ResizeRedraw, true);

			_f = f;
			LoftId = loftId;

			Text = " LOFTEMPS.DAT"; // TODO: + "ufo"/"tftd"

			int lofts = _f.LoFT.Length / 256;

			int w;
			if (lofts < COLS)
			{
				w = lofts;
				if (w < 16) w = 16;
			}
			else
				w = COLS;

			w = (w * LOFT_WIDTH) + (w * HORI_PAD) - 1;

			int h = (lofts + COLS - 1) / COLS * (LOFT_HEIGHT + VERT_TEXT_PAD);

			ClientSize = new Size(w, h);
		}
		#endregion cTor


		#region Events (override)
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			var graphics = e.Graphics;
			graphics.PixelOffsetMode   = PixelOffsetMode.Half;
			graphics.InterpolationMode = InterpolationMode.NearestNeighbor;

			Rectangle rect;

			int x, y;
			for (int i = 0; i != _f.LoFT.Length / 256; ++i)
			{
				var loft = new Bitmap(
									16,16,
									PixelFormat.Format8bppIndexed);	// Format1bppIndexed <- uses only 1 BIT per pixel
																	// - causes probs setting the pixels below.
				var data = loft.LockBits(
									new Rectangle(0,0, loft.Width, loft.Height),
									ImageLockMode.WriteOnly,
									PixelFormat.Format8bppIndexed); // Format1bppIndexed
				var start = data.Scan0;

				unsafe
				{
					var pos = (byte*)start.ToPointer();

					byte palid;
					for (int row = 0; row != loft.Height; ++row)
					for (int col = 0; col != loft.Width;  ++col)
					{
						byte* pixel = pos + (row * data.Stride) + col;

						palid = Convert.ToByte(_f.LoFT[(i * 256) + (row * loft.Width) + col]);
						*pixel = palid;
					}
				}
				loft.UnlockBits(data);

				ColorPalette pal = loft.Palette;
				pal.Entries[0] = SystemColors.ControlDarkDark;
				pal.Entries[1] = SystemColors.ControlLightLight;
				loft.Palette = pal;

				x = (i % COLS) * (LOFT_WIDTH  + HORI_PAD);
				y = (i / COLS) * (LOFT_HEIGHT + VERT_TEXT_PAD);

				graphics.DrawImage(
								loft,
								new Rectangle(
											x, y,
											LOFT_WIDTH, LOFT_HEIGHT));

				rect = new Rectangle(
								x,
								y + LOFT_HEIGHT,
								LOFT_WIDTH,
								VERT_TEXT_PAD);

				if (i == LoftId)
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
				int id = e.Y / (LOFT_HEIGHT + VERT_TEXT_PAD) * COLS
					   + e.X / (LOFT_WIDTH  + HORI_PAD);

				if (id < _f.LoFT.Length / 256)
				{
					_f.SetLoft(id);
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
			// LoftF
			// 
			this.ClientSize = new System.Drawing.Size(494, 276);
			this.Font = new System.Drawing.Font("Verdana", 5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "LoftF";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "loftset";
			this.ResumeLayout(false);

		}
		#endregion Designer
	}
}
