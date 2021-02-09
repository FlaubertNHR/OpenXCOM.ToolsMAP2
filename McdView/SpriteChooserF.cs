using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using DSShared;

using XCom;


namespace McdView
{
	internal sealed class SpriteChooserF
		:
			Form
	{
		#region Fields (static)
		private const int COLS          = 16;
		private const int VERT_TEXT_PAD = 16;

		internal static Point Loc = new Point(-1,-1);
		#endregion Fields (static)


		#region Fields
		private readonly McdviewF _f;

		private readonly int  Id;
		private          int _id;

		private readonly int _phase;
		#endregion Fields


		#region cTor
		/// <summary>
		/// 
		/// </summary>
		/// <param name="f"></param>
		/// <param name="phase"></param>
		/// <param name="spriteId"></param>
		internal SpriteChooserF(
				McdviewF f,
				int phase,
				int spriteId)
		{
			InitializeComponent();

			_f = f;
			_phase = phase;
			Id = spriteId;

			Text = _f.Label + GlobalsXC.PckExt + " - phase " + (_phase + 1);

			int w;
			if (_f.Spriteset.Count < COLS)
			{
				w = _f.Spriteset.Count;
				if (w < 8) w = 8;
			}
			else
				w = COLS;

			w *= XCImage.SpriteWidth32;

			int h = (_f.Spriteset.Count + COLS - 1) / COLS * (XCImage.SpriteHeight40 + VERT_TEXT_PAD);

			ClientSize = new Size(w, h);

			blink();
		}
		#endregion cTor


		#region Methods
		/// <summary>
		/// Blinks the current spriteId text-bg.
		/// </summary>
		private async void blink() // yes i know - this goes FOREVER!!
		{
			_id = Id;

			uint tick = UInt32.MinValue;
			while (++tick != UInt32.MaxValue)
			{
				await System.Threading.Tasks.Task.Delay(McdviewF.PERIOD);

				if (tick % 2 != 0)
					_id = -1;
				else
					_id = Id;

				Invalidate();
			}
			tick = UInt32.MinValue;
			blink();
		}
		#endregion Methods


		#region Events (override)
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{
			var graphics = e.Graphics;
			graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

			Rectangle rect;

			int x, y;
			for (int i = 0; i != _f.Spriteset.Count; ++i)
			{
				x = (i % COLS) *  XCImage.SpriteWidth32;
				y = (i / COLS) * (XCImage.SpriteHeight40 + VERT_TEXT_PAD);

				graphics.DrawImage(
								_f.Spriteset[i].Sprite,
								new Rectangle(
											x, y,
											XCImage.SpriteWidth32,
											XCImage.SpriteHeight40),
								0, 0, XCImage.SpriteWidth32, XCImage.SpriteHeight40,
								GraphicsUnit.Pixel,
								_f.Ia);

				rect = new Rectangle(
								x,
								y + XCImage.SpriteHeight40,
								XCImage.SpriteWidth32,
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


		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseUp(MouseEventArgs e)
		{
			if (   e.X > -1 && e.X < ClientSize.Width // NOTE: Bypass event if cursor moves off the clientarea before released.
				&& e.Y > -1 && e.Y < ClientSize.Height)
			{
				int id = e.Y / (XCImage.SpriteHeight40 + VERT_TEXT_PAD) * COLS
					   + e.X /  XCImage.SpriteWidth32;

				if (id < _f.Spriteset.Count)
				{
					switch (e.Button)
					{
						case MouseButtons.Left:
							_f.SetSprite(_phase, id);
							break;

						case MouseButtons.Right:
							if (MessageBox.Show(
											this,
											"Set all sprite phases to #" + id,
											" Set all sprite phases",
											MessageBoxButtons.YesNo,
											MessageBoxIcon.Question,
											MessageBoxDefaultButton.Button1,
											0) == DialogResult.No)
							{
								return; // do nothing if No.
							}
							_f.SetAllSprites(id.ToString());
							break;
					}
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
			switch (e.KeyData)
			{
				case Keys.Escape:
				case Keys.Enter:
					Close();
					break;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			if (!RegistryInfo.FastClose(e.CloseReason))
				Loc = new Point(Location.X, Location.Y);

			base.OnFormClosing(e);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLoad(EventArgs e)
		{
			if (Loc.X == -1)
			{
				Location = new Point(_f.Location.X + 25, _f.Location.Y + 345);
			}
			else
				Location = new Point(Loc.X, Loc.Y);

			base.OnLoad(e);
		}
		#endregion Events (override)



		#region Designer
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
			// SpriteChooserF
			// 
			this.ClientSize = new System.Drawing.Size(494, 276);
			this.DoubleBuffered = true;
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SpriteChooserF";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.ResumeLayout(false);

		}
		#endregion Designer
	}
}
