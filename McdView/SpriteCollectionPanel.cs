using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using XCom;
using XCom.Interfaces;


namespace McdView
{
	/// <summary>
	/// The panel that displays the entire MCD recordset with each record's
	/// Sprite1 sprite.
	/// </summary>
	internal sealed class SpriteCollectionPanel
		:
			Panel
	{
		#region Fields
		private readonly McdviewF _f;
		private readonly HScrollBar Scroller = new HScrollBar();

		private readonly Pen _penControl     = new Pen(SystemColors.Control, 1);
		private readonly Brush _brushControl = new SolidBrush(SystemColors.Control);

		private const TextFormatFlags flags = TextFormatFlags.HorizontalCenter
											| TextFormatFlags.VerticalCenter
											| TextFormatFlags.NoPadding;
		#endregion Fields


		#region Properties
		private Tilepart[] _records;
		internal Tilepart[] Records
		{
			private get { return _records; }
			set
			{
				_records = value;
				Invalidate();
			}
		}
		#endregion Properties


		#region cTor
		internal SpriteCollectionPanel(McdviewF f)
		{
			_f = f;

			SetStyle(ControlStyles.OptimizedDoubleBuffer
				   | ControlStyles.AllPaintingInWmPaint
				   | ControlStyles.UserPaint
				   | ControlStyles.ResizeRedraw, true);

			Anchor = (AnchorStyles)(AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			BackColor = SystemColors.Desktop;
			Margin = new Padding(0);
			Name = "pnl_Collection";
			TabIndex = 0;

			Location = new Point(5, 15);

			Scroller.Dock = DockStyle.Bottom;
			Controls.Add(Scroller);

			Height = y3_sprite + XCImage.SpriteHeight40 + Scroller.Height;
		}
		#endregion cTor


		#region Events (override)
		// constants for vertical align ->
		const int y1_sprite     = 0;
		const int y1_filltop    = XCImage.SpriteHeight40;
		const int y1_fillheight = 18;
		const int y2_sprite     = y1_filltop + y1_fillheight;
		const int y2_line       = y2_sprite + XCImage.SpriteHeight40 + 1;
		const int y3_sprite     = y2_line;

		protected override void OnPaint(PaintEventArgs e)
		{
			if (Records != null && Records.Length != 0)
			{
				var graphics = e.Graphics;
				graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

				Bitmap sprite;

				Rectangle rect;

				int i;
				for (i = 0; i != Records.Length; ++i)
				{
					graphics.DrawLine( // vertical lines
									_penControl,
									i * XCImage.SpriteWidth32 + i, 0,
									i * XCImage.SpriteWidth32 + i, Height);

					if ((sprite = Records[i][0].Sprite) != null)
						graphics.DrawImage(
										sprite,
										i * XCImage.SpriteWidth32 + i, y1_sprite);
				}
				graphics.DrawLine( // last vertical line
								_penControl,
								i * XCImage.SpriteWidth32 + i, 0,
								i * XCImage.SpriteWidth32 + i, Height);


				graphics.FillRectangle(
									_brushControl,
									0,     y1_filltop,
									Width, y1_fillheight);
				for (i = 0; i != Records.Length; ++i)
				{
					rect = new Rectangle(
									i * XCImage.SpriteWidth32 + i,
									y1_filltop,
									XCImage.SpriteWidth32,
									y1_fillheight);
					TextRenderer.DrawText(
									graphics,
									i.ToString(),
									Font,
									rect,
									SystemColors.ControlText,
									flags);
				}

				// dead part
				for (i = 0; i != Records.Length; ++i)
				{
					if (Records[i].Dead != null
						&& (sprite = Records[i].Dead[0].Sprite) != null)
					{
						graphics.DrawImage(
										sprite,
										i * XCImage.SpriteWidth32 + i, y2_sprite);
					}
				}

				graphics.DrawLine(
								_penControl,
								0,     y2_line,
								Width, y2_line);

				// alternate part
				for (i = 0; i != Records.Length; ++i)
				{
					if (Records[i].Alternate != null
					    && (sprite = Records[i].Alternate[0].Sprite) != null)
					{
						graphics.DrawImage(
										sprite,
										i * XCImage.SpriteWidth32 + i, y3_sprite);
					}
				}
			}
		}
		#endregion Events (override)
	}
}
