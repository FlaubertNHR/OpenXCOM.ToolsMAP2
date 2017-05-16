using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using XCom;


namespace PckView
{
	internal delegate void PaletteIndexChangedEventHandler(int selectedId);


	internal sealed class PalettePanel
		:
			Panel
	{
//		private SolidBrush _brush = new SolidBrush(Color.FromArgb(204, 204, 255));

		private Palette _palette;

		private const int Marginalia = 0; // well, you know ...

		private int _width  = 15;
		private int _height = 10;

		private int _id;
		private int _clickX;
		private int _clickY;

		internal const int Across = 16;

		internal event PaletteIndexChangedEventHandler PaletteIndexChangedEvent;


		internal PalettePanel()
		{
			SetStyle(ControlStyles.OptimizedDoubleBuffer
				   | ControlStyles.AllPaintingInWmPaint
				   | ControlStyles.UserPaint
				   | ControlStyles.ResizeRedraw, true);

			_palette = null;

			MouseDown += OnMouseDown;

			_clickX = -100;
			_clickY = -100;

			_id = -1;
		}


		protected override void OnResize(EventArgs eventargs)
		{
			_width  = (Width  / Across) - Marginalia * 2;
			_height = (Height / Across) - Marginalia * 2;

			_clickX = (_id % Across) * (_width  + Marginalia * 2);
			_clickY = (_id / Across) * (_height + Marginalia * 2);

			Refresh();
		}

		private void OnMouseDown(object sender, MouseEventArgs e)
		{
			_clickX = (e.X / (_width  + Marginalia * 2)) * (_width  + Marginalia * 2);
			_clickY = (e.Y / (_height + Marginalia * 2)) * (_height + Marginalia * 2);

			_id = (e.X / (_width + Marginalia * 2)) + (e.Y / (_height + Marginalia * 2)) * Across;

			if (PaletteIndexChangedEvent != null && _id < 256)
			{
				PaletteIndexChangedEvent(_id);
				Refresh();
			}
		}

		[DefaultValue(null)]
		[Browsable(false)]
		internal Palette Palette
		{
			get { return _palette; }
			set
			{
				_palette = value;
				Refresh();
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			if (_palette != null)
			{
				var graphics = e.Graphics;
				graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

				for (int
						i = 0,
							y = Marginalia;
						i < Across;
						++i,
							y += _height + Marginalia * 2)
					for (int
							j = 0,
								x = Marginalia;
							j < Across;
							++j,
								x += _width + Marginalia * 2)
					{
						graphics.FillRectangle(new SolidBrush(
															_palette[i * Across + j]),
															x, y,
															_width, _height);
					}

				graphics.DrawRectangle(
									Pens.Red,
									_clickX, _clickY,
									_width + Marginalia * 2 - 1, _height + Marginalia * 2 - 1);
			}
		}
	}
}
