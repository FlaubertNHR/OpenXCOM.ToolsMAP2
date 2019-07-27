using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;


namespace PckView
{
	internal delegate void PaletteIdChangedEventHandler(int selectedId);


	internal sealed class PalettePanel
		:
			Panel
	{
		#region Events
		internal event PaletteIdChangedEventHandler PaletteIdChangedEvent;
		#endregion Events


		#region Fields (static)
		internal const int SwatchesPerSide = 16; // 16 swatches across the panel.
		#endregion Fields (static)


		#region Fields
		private int _swatchWidth;
		private int _swatchHeight;
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


		#region Properties (static)
		internal static PalettePanel that
		{ get; private set; }
		#endregion Properties (static)


		#region Properties
		private int _palid = -1;
		internal int Palid
		{
			get { return _palid; }
			set { _palid = value; }
		}

		private int _clickX = -1;
		private int ClickX
		{
			get { return _clickX; }
			set { _clickX = value; }
		}
		private int _clickY = -1;
		private int ClickY
		{
			get { return _clickY; }
			set { _clickY = value; }
		}
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		internal PalettePanel()
		{
			that = this;
			PckViewForm.PaletteChanged += OnPaletteChanged;
		}
		#endregion cTor


		#region Events (override)
		protected override void OnResize(EventArgs eventargs)
		{
			_swatchWidth  = Width  / SwatchesPerSide;
			_swatchHeight = Height / SwatchesPerSide;

			if (Palid != -1)
			{
				ClickX = Palid % SwatchesPerSide * _swatchWidth  + 1;
				ClickY = Palid / SwatchesPerSide * _swatchHeight + 1;
			}
			Refresh();
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);

			int swatchX = e.X / _swatchWidth;
			int swatchY = e.Y / _swatchHeight;

			ClickX = swatchX * _swatchWidth  + 1;
			ClickY = swatchY * _swatchHeight + 1;

			Palid = swatchY * SwatchesPerSide + swatchX;

			PrintStatusPaletteId();
		}

		/// <summary>
		/// Draws the palette viewer.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{
//			base.OnPaint(e);

			if (!DesignMode) // otherwise PaletteForm has probls drawing a PalettePanel in the designer.
			{
				var graphics = e.Graphics;
				graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

				for (int
						i = 0,
							y = 0;
						i != SwatchesPerSide;
						++i,
							y += _swatchHeight)
				{
					for (int
							j = 0,
								x = 0;
							j != SwatchesPerSide;
							++j,
								x += _swatchWidth)
					{
						graphics.FillRectangle(
											new SolidBrush(PckViewForm.Pal[i * SwatchesPerSide + j]),
											x,
											y,
											_swatchWidth,
											_swatchHeight);
					}
				}

				if (Palid != -1)
				{
					graphics.DrawRectangle(
										Pens.Red,
										ClickX,
										ClickY,
										_swatchWidth  - 1,
										_swatchHeight - 1);
					graphics.FillRectangle( // -> fill the darn hole that .NET leaves in the top-left corner.
										Brushes.Red,
										ClickX - 1,
										ClickY - 1,
										1,
										1);
				}
			}
		}
		#endregion Events (override)


		#region Events
		private void OnPaletteChanged()
		{
			if (Palid > -1 && Palid < 256
				&& PaletteIdChangedEvent != null)
			{
				PaletteIdChangedEvent(Palid);
			}
			Refresh();
		}
		#endregion Events


		#region Methods
		/// <summary>
		/// Forces selection of a specific palette-id.
		/// </summary>
		/// <param name="palid">the palette id</param>
		internal void SelectPaletteId(int palid)
		{
			Palid = palid;

			ClickX = palid % SwatchesPerSide * _swatchWidth  + 1;
			ClickY = palid / SwatchesPerSide * _swatchHeight + 1;

			PrintStatusPaletteId();
		}

		internal void PrintStatusPaletteId()
		{
			if (Palid > -1 && Palid < 256
				&& PaletteIdChangedEvent != null)
			{
				PaletteIdChangedEvent(Palid);
				Refresh();
			}
		}
		#endregion Methods
	}
}
