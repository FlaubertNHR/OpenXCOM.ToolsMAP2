using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

using XCom;
using XCom.Interfaces;


namespace McdView
{
	/// <summary>
	/// The panel that displays the entire MCD recordset with each record's
	/// Sprite1 sprite.
	/// </summary>
	internal sealed class TilesetPanel
		:
			Panel
	{
		#region Fields
		private readonly McdviewF _f;
		private readonly HScrollBar Scroller = new HScrollBar();

		private int TableWidth;
		private const int _largeChange = XCImage.SpriteWidth32 + 1;

		private readonly Pen   _penControl   = new Pen(SystemColors.Control, 1);
		private readonly Brush _brushControl = new SolidBrush(SystemColors.Control);

		private bool _bypassScrollZero;
		#endregion Fields


		#region Properties
		private Tilepart[] _parts;
		internal Tilepart[] Parts
		{
			private get { return _parts; }
			set // IMPORTANT: Set 'Parts' via McdviewF only.
			{
				_parts = value;

				TableWidth = _parts.Length * (XCImage.SpriteWidth32 + 1) - 1;

				OnResize(null);

				if (!_bypassScrollZero)
					Scroller.Value = 0;
				else
					_bypassScrollZero = false;

				Invalidate();
			}
		}

		private ContextMenu Context
		{ get; set; }
		#endregion Properties


		#region cTor
		internal TilesetPanel(McdviewF f)
		{
			_f = f;

			SetStyle(ControlStyles.OptimizedDoubleBuffer
				   | ControlStyles.AllPaintingInWmPaint
				   | ControlStyles.UserPaint
				   | ControlStyles.ResizeRedraw
				   | ControlStyles.Selectable, true);

			Anchor = (AnchorStyles)(AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			BackColor = SystemColors.Desktop;
			Margin = new Padding(0);
			Name = "pnl_Collection";
			TabIndex = 0;
			TabStop = true;

			Location = new Point(5, 15);

			Scroller.Dock = DockStyle.Bottom;
			Scroller.LargeChange = _largeChange;
			Scroller.ValueChanged += OnValueChanged_Scroll;
			Controls.Add(Scroller);

			Height = y3_sprite + XCImage.SpriteHeight40 + Scroller.Height;

			CreateContext();
		}

		/// <summary>
		/// Builds and assigns an RMB context-menu.
		/// </summary>
		private void CreateContext()
		{
			var itAdd         = new MenuItem("add",          OnAddClick);
			var itAddRange    = new MenuItem("add range",    OnAddRangeClick);
			var itDelete      = new MenuItem("delete",       OnDeleteClick);
			var itDeleteRange = new MenuItem("delete range", OnDeleteRangeClick);

			var itSep0        = new MenuItem("-");

			var itCut         = new MenuItem("cut",          OnCutClick);
			var itCopy        = new MenuItem("copy",         OnCopyClick);
			var itPaste       = new MenuItem("paste",        OnPasteClick);

			var itSep1        = new MenuItem("-");

			var itFile        = new MenuItem("file",         OnFileClick);

			var itSep2        = new MenuItem("-");

			var itLeft        = new MenuItem("left",         OnLeftClick);
			var itRight       = new MenuItem("right",        OnRightClick);

			var itSep3        = new MenuItem("-");

			var itDeselect    = new MenuItem("deselect",     OnDeselectClick);

			Context = new ContextMenu();
			Context.MenuItems.AddRange(new []
										{
											itAdd,			//  0
											itAddRange,		//  1
											itDelete,		//  2
											itDeleteRange,	//  3
											itSep0,			//  4
											itCut,			//  5
											itCopy,			//  6
											itPaste,		//  7
											itSep1,			//  8
											itFile,			//  9
											itSep2,			// 10
											itLeft,			// 11
											itRight,		// 12
											itSep3,			// 13
											itDeselect		// 14
										});
			ContextMenu = Context;

			Context.Popup += OnPopup_Context;
		}
		#endregion cTor


		#region Events
		private void OnPopup_Context(object sender, EventArgs e)
		{
			// add #0 - after selid even if selid=-1
			Context.MenuItems[0].Enabled = (Parts != null);
			// add range #1 - after selid even if selid=-1
			Context.MenuItems[1].Enabled = (Parts != null && false);
			// delete #2 - selid if selid>-1 & selid<Parts.Length
			Context.MenuItems[2].Enabled = (_f.SelId != -1); //Parts != null &&
			// delete range #3 - selid if selid>-1 & selid<Parts.Length
			Context.MenuItems[3].Enabled = (Parts != null && _f.SelId != -1 && false);

			// cut #5 - selid if selid>-1 & selid<Parts.Length
			Context.MenuItems[5].Enabled = (Parts != null && _f.SelId != -1);
			// copy #6 - selid if selid>-1 & selid<Parts.Length
			Context.MenuItems[6].Enabled = (Parts != null && _f.SelId != -1);
			// paste #7 - selid if selid>-1 & selid<Parts.Length
			Context.MenuItems[7].Enabled = (Parts != null && _f.SelId != -1);

			// file #9 - inject after selid even if seld=-1
			Context.MenuItems[9].Enabled = (Parts != null);

			// left #11 - swap selid w/ pre if selid not 0
			Context.MenuItems[11].Enabled = (Parts != null && _f.SelId > 0);
			// right #12 - swap selid w/ post if selid not Parts.Length-1
			Context.MenuItems[12].Enabled = (Parts != null && _f.SelId != -1 && _f.SelId != Parts.Length - 1);

			// deselect #14 - if selid>-1 & selid<Parts.Length
			Context.MenuItems[14].Enabled = (_f.SelId != -1); //Parts != null &&
		}

		private void OnAddClick(object sender, EventArgs e)
		{
			_f.Changed = true;

			if (_f.Parts == null)
				_f.Parts = new Tilepart[0];

			var array = new Tilepart[Parts.Length + 1];

			int id = _f.SelId + 1;
			for (int i = 0; i != id; ++i)
				array[i] = Parts[i];

			McdRecord record = McdRecordFactory.CreateRecord();

			array[id] = new Tilepart(
								id,
								_f.Spriteset,
								record);

			array[id].Dead      =
			array[id].Alternate = null;

			Tilepart part;
			for (int i = id + 1; i != array.Length; ++i)
			{
				part = Parts[i - 1];
				part.TerId += 1; // not used in McdView but keep things consistent ....
				array[i] = part;
			}

			_bypassScrollZero = true;
			_f.Parts = array; // assign back to 'Parts' via McdviewF

			_f.SelId = id;
		}

		private void OnAddRangeClick(object sender, EventArgs e)
		{
		}

		private void OnDeleteClick(object sender, EventArgs e)
		{
			_f.Changed = true;

			var array = new Tilepart[Parts.Length - 1];

			int id = _f.SelId;
			for (int i = 0; i != id; ++i)
				array[i] = Parts[i];

			Tilepart part;
			for (int i = id; i != array.Length; ++i)
			{
				part = Parts[i + 1];
				part.TerId -= 1;
				array[i] = part;
			}

			_f.SelId = -1;

			_bypassScrollZero = true;
			_f.Parts = array;
		}

		private void OnDeleteRangeClick(object sender, EventArgs e)
		{
		}

		private void OnCutClick(object sender, EventArgs e)
		{
		}

		private void OnCopyClick(object sender, EventArgs e)
		{
		}

		private void OnPasteClick(object sender, EventArgs e)
		{
		}

		private void OnFileClick(object sender, EventArgs e)
		{
		}

		private void OnLeftClick(object sender, EventArgs e)
		{
		}

		private void OnRightClick(object sender, EventArgs e)
		{
		}

		private void OnDeselectClick(object sender, EventArgs e)
		{
			_f.SelId = -1;
		}


		private void OnValueChanged_Scroll(object sender, EventArgs e)
		{
			Invalidate();
		}
		#endregion Events


		#region Events (override)
		Graphics _graphics;
		ImageAttributes _attri;

		// constants for vertical align ->
		const int y1_sprite = 0;
		const int y1_fill   = XCImage.SpriteHeight40;
		const int y1_fill_h = 18;
		const int y2_sprite = y1_fill + y1_fill_h;
		const int y2_line   = y2_sprite + XCImage.SpriteHeight40 + 1;
		const int y3_sprite = y2_line;

		protected override void OnPaint(PaintEventArgs e)
		{
			if (Parts != null && Parts.Length != 0)
			{
				_graphics = e.Graphics;
				_graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

				_attri = new ImageAttributes();
				if (_f._spriteShadeEnabled)
					_attri.SetGamma(_f.SpriteShadeFloat, ColorAdjustType.Bitmap);

				Bitmap sprite;

				Rectangle rect;

				int offset = -Scroller.Value;

				int i;
				for (i = 0; i != Parts.Length; ++i)
				{
					if (i != 0)
						_graphics.DrawLine( // draw vertical line before each sprite except the first sprite
										_penControl,
										i * XCImage.SpriteWidth32 + i + offset, 0,
										i * XCImage.SpriteWidth32 + i + offset, Height);

					if (_f.Spriteset != null
						&& Parts[i] != null // not sure why Tilepart entries go null but aren't null but they do
						&& Parts[i].Sprites != null
						&& Parts[i][0] != null
						&& (sprite = Parts[i][0].Sprite) != null)
					{
						DrawSprite(
								sprite,
								i * XCImage.SpriteWidth32 + i + offset,
								y1_sprite - Parts[i].Record.TileOffset);
					}
				}

				_graphics.FillRectangle(
									_brushControl,
									0,     y1_fill,
									Width, y1_fill_h);

				if (_f.SelId != -1)
					_graphics.FillRectangle(
										McdviewF.BrushHilight,
										_f.SelId * (XCImage.SpriteWidth32 + 1) + offset,
										y1_fill,
										XCImage.SpriteWidth32,
										y1_fill_h);

				for (i = 0; i != Parts.Length; ++i)
				{
					rect = new Rectangle(
									i * XCImage.SpriteWidth32 + i + offset,
									y1_fill,
									XCImage.SpriteWidth32,
									y1_fill_h);

					TextRenderer.DrawText(
										_graphics,
										i.ToString(),
										Font,
										rect,
										SystemColors.ControlText,
										McdviewF.FLAGS);
				}

				if (_f.Spriteset != null)
				{
					for (i = 0; i != Parts.Length; ++i) // dead part ->
					{
						if (Parts[i] != null
							&& Parts[i].Dead != null
							&& Parts[i].Dead.Sprites != null
							&& Parts[i].Dead[0] != null
							&& (sprite = Parts[i].Dead[0].Sprite) != null)
						{
							DrawSprite(
									sprite,
									i * XCImage.SpriteWidth32 + i + offset,
									y2_sprite - Parts[i].Dead.Record.TileOffset);
						}
					}
				}

				_graphics.DrawLine(
								_penControl,
								0,     y2_line,
								Width, y2_line);

				if (_f.Spriteset != null)
				{
					for (i = 0; i != Parts.Length; ++i) // alternate part ->
					{
						if (Parts[i] != null
							&& Parts[i].Alternate != null
							&& Parts[i].Alternate.Sprites != null
							&& Parts[i].Alternate[0] != null
							&& (sprite = Parts[i].Alternate[0].Sprite) != null)
						{
							DrawSprite(
									sprite,
									i * XCImage.SpriteWidth32 + i + offset,
									y3_sprite - Parts[i].Alternate.Record.TileOffset);
						}
					}
				}
			}
		}

		/// <summary>
		/// Helper for OnPaint().
		/// </summary>
		/// <param name="sprite"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		private void DrawSprite(
				Image sprite,
				int x,
				int y)
		{
			_graphics.DrawImage(
							sprite,
							new Rectangle(
										x, y,
										XCImage.SpriteWidth32,
										XCImage.SpriteHeight40),
							0, 0, XCImage.SpriteWidth32, XCImage.SpriteHeight40,
							GraphicsUnit.Pixel,
							_attri);
		}


		/// <summary>
		/// Handles client resizing. Sets the scrollbar's Enabled and Maximum
		/// values.
		/// @note Holy f*ck I hate .NET scrollbars.
		/// </summary>
		/// <param name="eventargs"></param>
		protected override void OnResize(EventArgs eventargs)
		{
			if (eventargs != null) // ie. is *not* Parts load
				base.OnResize(eventargs);

			int range = 0;
			if (Parts != null && Parts.Length != 0)
			{
				range = TableWidth + (_largeChange - 1) - Width;
				if (range < _largeChange)
					range = 0;
			}

			Scroller.Maximum =  range;
			Scroller.Enabled = (range != 0);

			if (Scroller.Enabled
				&& TableWidth < Width + Scroller.Value)
			{
				Scroller.Value = TableWidth - Width;
			}
			ScrollTile();
		}


		/// <summary>
		/// @note If user has the openfile dialog open and double-clicks to open
		/// a file that happens to be over the panel a mouse-up event fires. So
		/// use MouseDown here.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			Select();

			if (e.Button == MouseButtons.Left)
			{
				if (Parts != null && Parts.Length != 0
					&& e.Y < Height - Scroller.Height)
				{
					int id = (e.X + Scroller.Value) / (XCImage.SpriteWidth32 + 1);
					if (id >= Parts.Length)
						id = -1;

					_f.SelId = id;
				}
			}
			// else if (e.Button == MouseButtons.Right) // This is handled auto by the panel's ContextMenu var.
		}

		/// <summary>
		/// Scrolls the table by the mousewheel.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseWheel(MouseEventArgs e)
		{
			if (Scroller.Enabled)
			{
				if (e.Delta > 0)
				{
					if (Scroller.Value - Scroller.LargeChange < 0)
						Scroller.Value = 0;
					else
						Scroller.Value -= Scroller.LargeChange;
				}
				else if (e.Delta < 0)
				{
					if (Scroller.Value + Scroller.LargeChange + (Scroller.LargeChange - 1) > Scroller.Maximum)
						Scroller.Value = Scroller.Maximum - (Scroller.LargeChange - 1);
					else
						Scroller.Value += Scroller.LargeChange;
				}
			}
		}

		/// <summary>
		/// This is required in order to accept arrow-keyboard-input via
		/// McdviewF.OnKeyDown().
		/// </summary>
		/// <param name="keyData"></param>
		/// <returns></returns>
		protected override bool IsInputKey(Keys keyData)
		{
			switch (keyData)
			{
				case Keys.Left:
				case Keys.Up:
				case Keys.Right:
				case Keys.Down:
					return true;
			}
			return base.IsInputKey(keyData);
		}
		#endregion Events (override)


		#region Methods
		/// <summary>
		/// Scrolls the panel to ensure that the currently selected tile is
		/// fully displayed.
		/// </summary>
		internal void ScrollTile()
		{
			if (Scroller.Enabled && _f.SelId != -1)
			{
				int x = _f.SelId * (XCImage.SpriteWidth32 + 1);
				if (x < Scroller.Value)
				{
					Scroller.Value = x;
				}
				else if ((x += XCImage.SpriteWidth32) > Width + Scroller.Value)
				{
					Scroller.Value = x - Width;
				}
			}
		}

		/// <summary>
		/// Takes keyboard-input from the Form's KeyDown event to scroll this
		/// panel.
		/// </summary>
		/// <param name="e"></param>
		internal void KeyTile(KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Left:
				case Keys.Up:
				case Keys.Back:
					if (_f.SelId != 0)
						_f.SelId -= 1;
					break;

				case Keys.Right:
				case Keys.Down:
				case Keys.Space:
					if (_f.SelId != Parts.Length - 1)
						_f.SelId += 1;
					break;

				case Keys.PageUp:
				{
					int d = Width / (XCImage.SpriteWidth32 + 1);
					if (_f.SelId - d < 0)
						_f.SelId = 0;
					else
						_f.SelId -= d;

					break;
				}

				case Keys.PageDown:
				{
					int d = Width / (XCImage.SpriteWidth32 + 1);
					if (_f.SelId + d > Parts.Length - 1)
						_f.SelId = Parts.Length - 1;
					else
						_f.SelId += d;

					break;
				}

				case Keys.Home:
					_f.SelId = 0;
					break;

				case Keys.End:
					_f.SelId = Parts.Length - 1;
					break;
			}
		}

/*		/// <summary>
		/// Gets the loc of the currently selected tile relative to the table.
		/// </summary>
		/// <returns></returns>
		private int GetTileLeft()
		{
			return _f.SelId * (XCImage.SpriteWidth32 + 1);
		}
		/// <summary>
		/// Gets the loc+width of the currently selected tile relative to the table.
		/// </summary>
		/// <returns></returns>
		private int GetTileRight()
		{
			return _f.SelId * (XCImage.SpriteWidth32 + 1) + XCImage.SpriteWidth32;
		} */
		#endregion Methods
	}
}
