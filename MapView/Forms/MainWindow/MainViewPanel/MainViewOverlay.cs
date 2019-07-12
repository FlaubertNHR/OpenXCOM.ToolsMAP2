//#define LOCKBITS // toggle this to change OnPaint routine in standard build.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

using MapView.Forms.MainWindow;
using MapView.Forms.MapObservers.TopViews;

using XCom;
using XCom.Interfaces;
using XCom.Interfaces.Base;


namespace MapView
{
	internal sealed class MainViewOverlay
		:
			Panel // god I hate these double-panels!!!! cf. MainViewUnderlay
	{
		#region Delegates
		internal delegate void MouseDragEvent();
		#endregion Delegates


		#region Events
		internal event MouseDragEvent MouseDrag;
		#endregion Events


		#region Fields (static)
		internal const int HalfWidthConst  = 16;
		internal const int HalfHeightConst =  8;
		#endregion Fields (static)


		#region Fields
		/// <summary>
		/// Suppresses display of the targeter sprite when the panel loses
		/// focus or the mouse-cursor leaves the clientarea. This gets overruled
		/// by '_targeterForced'.
		/// </summary>
		private bool _targeterSuppressed;

		/// <summary>
		/// Forces display of the targeter sprite at the DragEnd position when
		/// tiles are selected by keyboard. This overrules '_targeterSuppressed'.
		/// </summary>
		internal bool _targeterForced;

		private int _col; // these are used to print the clicked location
		private int _row;
		private int _lev;

		private int _colOver; // these are used to track the mouseover location
		private int _rowOver;
		#endregion Fields


		#region Properties (static)
		internal static MainViewOverlay that
		{ get; private set; }
		#endregion Properties (static)


		#region Properties
		/// <summary>
		/// MapBase is set only by MainViewUnderlay.MapBase{set}.
		/// </summary>
		internal MapFileBase MapBase
		{ get; set; }

		private Point _origin = new Point(0,0);
		internal Point Origin
		{
			get { return _origin; }
			set { _origin = value; }
		}

		internal int HalfWidth
		{ private get; set; }

		internal int HalfHeight
		{ private get; set; }


		private bool _firstClick;
		/// <summary>
		/// A flag that indicates that the user has selected a tile(s).
		/// @note The operation of the flag relies on the fact that once a
		/// tile(s) has been selected on a Map there will always be a tile(s)
		/// selected until either (a) the Map is resized or (b) user loads a
		/// different Map.
		/// </summary>
		internal bool FirstClick
		{
			get { return _firstClick; }
			set
			{
				if (!(_firstClick = value))
				{
					_dragBeg = new Point(-1,-1);
					_dragEnd = new Point(-1,-1);
				}

				ViewerFormsManager.ToolFactory.SetEditButtonsEnabled( _firstClick);
				ViewerFormsManager.ToolFactory.SetPasteButtonsEnabled(_firstClick && _copied != null);
			}
		}

		/// <summary>
		/// List of SolidBrushes used to draw sprites from XCImage.Bindata (in
		/// Mono). Can be either UfoBattle palette brushes or TftdBattle
		/// palette brushes.
		/// </summary>
		internal List<Brush> SpriteBrushes
		{ private get; set; }
		#endregion Properties


		#region Fields (graphics)
		private GraphicsPath _layerFill = new GraphicsPath();

		private Graphics _graphics;
		private ImageAttributes _spriteAttributes = new ImageAttributes();

		private Brush _brushLayer;

		private int _anistep;
		private int _cols, _rows;
		#endregion Fields (graphics)


		#region Properties (options)
		private Color _colorLayer = Color.MediumVioletRed;							// initial color for the grid-layer Option
		public Color GridLayerColor													// <- public for Reflection.
		{
			get { return _colorLayer; }
			set
			{
				_colorLayer = value;
				_brushLayer = new SolidBrush(Color.FromArgb(GridLayerOpacity, _colorLayer));
				Refresh();
			}
		}

		private int _opacity = 180;													// initial opacity for the grid-layer Option
		public int GridLayerOpacity													// <- public for Reflection.
		{
			get { return _opacity; }
			set
			{
				_opacity = value.Clamp(0, 255);
				_brushLayer = new SolidBrush(Color.FromArgb(_opacity, ((SolidBrush)_brushLayer).Color));
				Refresh();
			}
		}

		private Pen _penGrid = new Pen(Color.Black, 1);								// initial pen for grid-lines Option
		public Color GridLineColor													// <- public for Reflection.
		{
			get { return _penGrid.Color; }
			set
			{
				_penGrid.Color = value;
				Refresh();
			}
		}
		public int GridLineWidth													// <- public for Reflection.
		{
			get { return (int)_penGrid.Width; }
			set
			{
				_penGrid.Width = value;
				Refresh();
			}
		}

		private Pen _penGrid10 = new Pen(Color.Black, 2);							// initial pen for x10 grid-lines Option
		public Color Grid10LineColor												// <- public for Reflection.
		{
			get { return _penGrid10.Color; }
			set
			{
				_penGrid10.Color = value;
				Refresh();
			}
		}
		public int Grid10LineWidth													// <- public for Reflection.
		{
			get { return (int)_penGrid10.Width; }
			set
			{
				_penGrid10.Width = value;
				Refresh();
			}
		}

		private bool _showGrid = true;												// initial val for show-grid Option
		public bool ShowGrid														// <- public for Reflection.
		{
			get { return _showGrid; }
			set
			{
				_showGrid = value;
				Refresh();
			}
		}

		private Pen _penSelect = new Pen(Color.Tomato, 2);							// initial pen for selection-border Option
		public Color SelectionLineColor												// <- public for Reflection.
		{
			get { return _penSelect.Color; }
			set
			{
				_penSelect.Color = value;
				Refresh();
			}
		}
		public int SelectionLineWidth												// <- public for Reflection.
		{
			get { return (int)_penSelect.Width; }
			set
			{
				_penSelect.Width = value;
				Refresh();
			}
		}

		private bool _graySelection = true;											// initial val for gray-selection Option
		// NOTE: Remove suppression for Release cfg. .. not workie.
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
		"CA1811:AvoidUncalledPrivateCode",
		Justification = "Because the setter is called dynamically w/ Reflection" +
		"or other: not only is it used it needs to be public.")]
		public bool GraySelection													// <- public for Reflection.
		{
			get { return _graySelection; }
			set
			{
				_graySelection = value;
				Refresh();
			}
		}

#if !LOCKBITS
		internal bool _spriteShadeEnabled = true; // was private, see ScanGViewer and TilePanel and QuadrantPanel
#endif
		// NOTE: Options don't like floats afaict, hence this workaround w/
		// 'SpriteShade' and 'SpriteShadeLocal' ->
		private int _spriteShade;													// 0 = initial val for sprite shade Option
		public int SpriteShade														// <- public for Reflection.
		{
			get { return _spriteShade; }
			set
			{
				_spriteShade = value;

				if (_spriteShade > 9 && _spriteShade < 101)
				{
#if !LOCKBITS
					_spriteShadeEnabled = true;
#endif
					SpriteShadeLocal = _spriteShade * 0.03f;
				}
#if !LOCKBITS
				else
					_spriteShadeEnabled = false;
#endif
				Refresh();

				// refresh ScanGViewer panel and TilePanel and QuadrantPanel
				if (XCMainWindow.ScanG != null)
					XCMainWindow.ScanG.InvalidatePanel();

				ViewerFormsManager.TileView    .Control                 .Refresh();
				ViewerFormsManager.TopView     .Control   .QuadrantPanel.Refresh();
				ViewerFormsManager.TopRouteView.ControlTop.QuadrantPanel.Refresh();
			}
		}
		private float _spriteShadeLocal = 1.0f;										// initial val for local sprite shade
		internal float SpriteShadeLocal // was private, see ScanGViewer
		{
			get { return _spriteShadeLocal; }
			set { _spriteShadeLocal = value; }
		}

		// NOTE: Options don't like enums afaict, hence this workaround w/
		// 'Interpolation' and 'InterpolationLocal' ->
		private int _interpolation;													// 0 = initial val for interpolation Option
		public int Interpolation													// <- public for Reflection.
		{
			get { return _interpolation; }
			set
			{
				_interpolation = value.Clamp(0, 7);
				InterpolationLocal = (InterpolationMode)_interpolation;
				Refresh();
			}
		}
		private InterpolationMode _interpolationLocal = InterpolationMode.Default;	// initial val for local interpolation
		private InterpolationMode InterpolationLocal
		{
			get { return _interpolationLocal; }
			set { _interpolationLocal = value; }
		}
		#endregion Properties (options)


		#region cTor
		internal MainViewOverlay()
		{
			that =
			XCMainWindow.that.MainViewOverlay = this;

			SetStyle(ControlStyles.OptimizedDoubleBuffer
				   | ControlStyles.AllPaintingInWmPaint
				   | ControlStyles.UserPaint
				   | ControlStyles.ResizeRedraw
				   | ControlStyles.Selectable, true);
			TabStop = true;
			TabIndex = 4;

			_brushLayer = new SolidBrush(Color.FromArgb(GridLayerOpacity, GridLayerColor));

//			var t1 = new Timer();
//			t1.Interval = 250;
//			t1.Enabled = true;
//			t1.Tick += t1_Tick;

			GotFocus  += OnFocusGained;
			LostFocus += OnFocusLost;
		}
		#endregion cTor


/*		/// <summary>
		/// Hides the cuboid-targeter when the mouse leaves this control unless
		/// the targeter was enabled by a keyboard tiles-selection.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void t1_Tick(object sender, EventArgs e)
		{
			if (!_targeterForced
				&& !ClientRectangle.Contains(PointToClient(Cursor.Position)))
			{
				Invalidate();
			}
		} */

		private void OnFocusGained(object sender, EventArgs e)
		{
			if (MapBase != null)
			{
				var pt = PointToClient(Cursor.Position);
					pt = GetTileLocation(pt.X, pt.Y);
				_colOver = pt.X;
				_rowOver = pt.Y;

				Invalidate();
			}
		}

		private void OnFocusLost(object sender, EventArgs e)
		{
			if (MapBase != null)
			{
				_targeterForced = false;
				Invalidate();
			}
		}


		#region Events and Methods for the edit-functions
		// The following functs are for subscription to toolstrip Editor buttons.
		internal void OnCut(object sender, EventArgs e)
		{
			Copy();
			ClearSelection();
		}
		internal void OnCopy(object sender, EventArgs e)
		{
			Copy();
		}
		internal void OnPaste(object sender, EventArgs e)
		{
			Paste();
		}
		internal void OnDelete(object sender, EventArgs e)
		{
			ClearSelection();
		}
		internal void OnFill(object sender, EventArgs e)
		{
			FillSelectedTiles();
		}


		/// <summary>
		/// Handles keyboard-input for editing and saving the Mapfile.
		/// @note Navigation keys are handled by 'KeyPreview' at the form level.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			Edit(e);
//			base.OnKeyDown(e);
		}

		/// <summary>
		/// Performs edit-functions by keyboard or saves the Mapfile.
		/// </summary>
		/// <param name="e"></param>
		internal void Edit(KeyEventArgs e)
		{
			if (e.Control)
			{
				switch (e.KeyCode)
				{
					case Keys.S:
						XCMainWindow.that.OnSaveMapClick(null, EventArgs.Empty);
						break;

					case Keys.X:
						Copy();
						ClearSelection();
						break;

					case Keys.C:
						Copy();
						break;

					case Keys.V:
						Paste();
						break;
				}
			}
			else
			{
				switch (e.KeyCode)
				{
					case Keys.Delete:
						ClearSelection();
						break;

					case Keys.Escape:
						if (MapBase != null)
						{
							_targeterForced = false;

							var pt = PointToClient(Cursor.Position);
								pt = GetTileLocation(pt.X, pt.Y);
							_colOver = pt.X;
							_rowOver = pt.Y;

							_keyDeltaX =
							_keyDeltaY = 0;

							ProcessSelection(DragBeg, DragBeg);
						}
						break;

					case Keys.F:
						FillSelectedTiles();
						break;
				}
			}
		}

		/// <summary>
		/// Clears all tileparts from any currently selected tiles.
		/// </summary>
		internal void ClearSelection()
		{
			if (MapBase != null && FirstClick)
			{
				XCMainWindow.that.MapChanged = true;

				MapTile tile;

				int visible = ViewerFormsManager.TopView.Control.VisibleParts;

				var a = GetDragBeg_abs();
				var b = GetDragEnd_abs();

				for (int col = a.X; col <= b.X; ++col)
				for (int row = a.Y; row <= b.Y; ++row)
				{
					tile = MapBase[row, col] as MapTile;

					if ((visible & TopView.FLOOR)   != 0) tile.Floor   = null;
					if ((visible & TopView.WEST)    != 0) tile.West    = null;
					if ((visible & TopView.NORTH)   != 0) tile.North   = null;
					if ((visible & TopView.CONTENT) != 0) tile.Content = null;

					tile.Vacancy();
				}

				MapBase.CalculateOccultations();

				RefreshViewers();
			}
		}


		private Dictionary<int, Tuple<string,string>> _copiedTerrains;
		private MapTileBase[,] _copied;

		/// <summary>
		/// Copies any selected tiles to an internal buffer.
		/// </summary>
		internal void Copy()
		{
			if (MapBase != null && FirstClick)
			{
				ViewerFormsManager.ToolFactory.SetPasteButtonsEnabled();

				_copiedTerrains = MapBase.Descriptor.Terrains;

				var a = GetDragBeg_abs();
				var b = GetDragEnd_abs();

				_copied = new MapTileBase[b.Y - a.Y + 1,
										  b.X - a.X + 1];

				MapTile tile;

				for (int col = a.X; col <= b.X; ++col)
				for (int row = a.Y; row <= b.Y; ++row)
				{
					tile = MapBase[row, col] as MapTile;
					_copied[row - a.Y,
							col - a.X] = new MapTile(
												tile.Floor,
												tile.West,
												tile.North,
												tile.Content);
				}
			}
		}

		/// <summary>
		/// Pastes any copied tiles to the currently selected location.
		/// @note The terrainset of the current tileset needs to be identical to
		/// the terrainset of the tileset from which parts were copied (or
		/// nearly so).
		/// </summary>
		internal void Paste()
		{
			if (MapBase != null && FirstClick && _copied != null)
			{
				if (AllowPaste(_copiedTerrains, MapBase.Descriptor.Terrains))
				{
					XCMainWindow.that.MapChanged = true;

					MapTile tile, copy;

					int visible = ViewerFormsManager.TopView.Control.VisibleParts;

					for (int
							row = DragBeg.Y;
							row != MapBase.MapSize.Rows && (row - DragBeg.Y) < _copied.GetLength(0);
							++row)
					{
						for (int
								col = DragBeg.X;
								col != MapBase.MapSize.Cols && (col - DragBeg.X) < _copied.GetLength(1);
								++col)
						{
							if ((tile = MapBase[row, col] as MapTile) != null
								&& (copy = _copied[row - DragBeg.Y,
												   col - DragBeg.X] as MapTile) != null)
							{
								if ((visible & TopView.FLOOR)   != 0) tile.Floor   = copy.Floor;
								if ((visible & TopView.WEST)    != 0) tile.West    = copy.West;
								if ((visible & TopView.NORTH)   != 0) tile.North   = copy.North;
								if ((visible & TopView.CONTENT) != 0) tile.Content = copy.Content;

								tile.Vacancy();
							}
						}
					}

					MapBase.CalculateOccultations();

					RefreshViewers();
				}
				else
				{
					string info = "copied:";
					foreach (var key in _copiedTerrains)
					{
						info += Environment.NewLine + key.Value.Item1 + " - " // TODO: Align w/ tabs.
							  + GetBasepathDescript(key.Value.Item2);
					}

					info += Environment.NewLine + Environment.NewLine
						  + "currently allocated:";
					foreach (var key in MapBase.Descriptor.Terrains)
					{
						info += Environment.NewLine + key.Value.Item1 + " - " // TODO: Align w/ tabs.
							  + GetBasepathDescript(key.Value.Item2);
					}

					using (var f = new Infobox(
											" Allocated terrains differ",
											"The list of terrains that were copied are too different"
												+ " from the terrains in the currently loaded Map.",
											info))
					{
						f.SetLabelColor(Color.Firebrick);
						f.ShowDialog(this);
					}
				}
			}
		}

		/// <summary>
		/// Checks if two terrain-definitions are or are nearly identical.
		/// @note It's okay if 'dst' has more terrains allocated than 'src'.
		/// </summary>
		/// <param name="src">source terrain definition</param>
		/// <param name="dst">destination terrain definition</param>
		/// <returns></returns>
		private bool AllowPaste(
				IDictionary<int, Tuple<string,string>> src,
				IDictionary<int, Tuple<string,string>> dst)
		{
			if (src.Keys.Count > dst.Keys.Count)
				return false;

			for (int i = 0; i != src.Keys.Count; ++i)
			{
				if (   src[i].Item1 != dst[i].Item1
					|| src[i].Item2 != dst[i].Item2) // TODO: Compare Item2 by expanding it.
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Gets a string descripting a specified basepath.
		/// </summary>
		/// <param name="basepath"></param>
		/// <returns></returns>
		private string GetBasepathDescript(string basepath)
		{
			if (String.IsNullOrEmpty(basepath))
				return "config basepath";

			if (basepath == GlobalsXC.BASEPATH)
				return "tileset basepath";

			return basepath;
		}

		/// <summary>
		/// Fills the correct quadrant of the currently selected tiles with the
		/// currently selected tilepart from TileView.
		/// </summary>
		internal void FillSelectedTiles()
		{
			if (MapBase != null && FirstClick)
			{
				XCMainWindow.that.MapChanged = true;

				var a = GetDragBeg_abs();
				var b = GetDragEnd_abs();

				var quad = ViewerFormsManager.TopView .Control.QuadrantPanel.SelectedQuadrant;
				var part = ViewerFormsManager.TileView.Control.SelectedTilepart;

				MapTile tile;
				for (int col = a.X; col <= b.X; ++col)
				for (int row = a.Y; row <= b.Y; ++row)
				{
					tile = ((MapTile)MapBase[row, col]);
					tile[quad] = part;
					tile.Vacancy();
				}

				MapBase.CalculateOccultations();

				RefreshViewers();
			}
		}

		/// <summary>
		/// Causes this panel to redraw along with the TopView, RouteView, and
		/// TopRouteView forms - also invalidates the ScanG panel.
		/// </summary>
		private void RefreshViewers()
		{
			Invalidate();

			ViewerFormsManager.TopView     .Control     .Refresh();
			ViewerFormsManager.RouteView   .Control     .Refresh();
			ViewerFormsManager.TopRouteView.ControlTop  .Refresh();
			ViewerFormsManager.TopRouteView.ControlRoute.Refresh();

			if (XCMainWindow.ScanG != null)
				XCMainWindow.ScanG.InvalidatePanel();	// incl/ ProcessTileSelection() for selection rectangle
		}												// not used by ScanG view at present
		#endregion Events and Methods for the edit-functions


		#region Keyboard navigation
		/// <summary>
		/// Keyboard navigation called by XCMainWindow (form-level) key events
		/// OnKeyDown() and ProcessCmdKey().
		/// </summary>
		/// <param name="keyData"></param>
		/// <param name="isTop">true if TopView is the active viewer</param>
		internal void Navigate(Keys keyData, bool isTop = false)
		{
			if (MapBase != null)
			{
				if (!FirstClick)
				{
					_keyDeltaX =
					_keyDeltaY = 0;

					MapBase.Location = new MapLocation(0,0, MapBase.Level); // fire SelectLocation

					var loc = new Point(0,0);
					ProcessSelection(loc,loc);
				}
				else if (!keyData.HasFlag(Keys.Shift))
				{
					var loc = new Point(0,0);
					int vert = 0;
					switch (keyData)
					{
						case Keys.Up:    loc.X = -1; loc.Y = -1; break;
						case Keys.Right: loc.X = +1; loc.Y = -1; break;
						case Keys.Down:  loc.X = +1; loc.Y = +1; break;
						case Keys.Left:  loc.X = -1; loc.Y = +1; break;

						case Keys.PageUp:   loc.Y = -1; break;
						case Keys.PageDown: loc.X = +1; break;
						case Keys.End:      loc.Y = +1; break;
						case Keys.Home:     loc.X = -1; break;

//						case Keys.Delete: // oops Delete is delete tile - try [Shift+Insert]
						case Keys.Add:      vert = +1; break;
//						case Keys.Insert:
						case Keys.Subtract: vert = -1; break;
					}

					if (loc.X != 0 || loc.Y != 0)
					{
						int r = MapBase.Location.Row + loc.Y;
						int c = MapBase.Location.Col + loc.X;
						if (   r > -1 && r < MapBase.MapSize.Rows // safety.
							&& c > -1 && c < MapBase.MapSize.Cols)
						{
							_keyDeltaX =
							_keyDeltaY = 0;

							MapBase.Location = new MapLocation(r,c, MapBase.Level); // fire SelectLocation

							loc.X = _colOver = c;
							loc.Y = _rowOver = r;
							ProcessSelection(loc,loc);
						}
					}
					else if (vert != 0)
					{
						int level = MapBase.Location.Lev + vert;
						if (level > -1 && level < MapBase.MapSize.Levs) // safety.
						{
							OnMouseWheel(new MouseEventArgs(
														MouseButtons.None,
														isTop ? TARGETER_KEY_TOP : TARGETER_KEY_MAIN, // WARNING: this is a trick (sic)
														0,0, vert));
						}
					}
				}
				else // [Shift] = drag select ->
				{
					var loc = new Point(0,0);
					switch (keyData)
					{
						case Keys.Shift | Keys.Up:    loc.X = -1; loc.Y = -1; break;
						case Keys.Shift | Keys.Right: loc.X = +1; loc.Y = -1; break;
						case Keys.Shift | Keys.Down:  loc.X = +1; loc.Y = +1; break;
						case Keys.Shift | Keys.Left:  loc.X = -1; loc.Y = +1; break;

						case Keys.Shift | Keys.PageUp:   loc.Y = -1; break;
						case Keys.Shift | Keys.PageDown: loc.X = +1; break;
						case Keys.Shift | Keys.End:      loc.Y = +1; break;
						case Keys.Shift | Keys.Home:     loc.X = -1; break;
					}

					if (loc.X != 0 || loc.Y != 0)
					{
						int r = MapBase.Location.Row + loc.Y;
						int c = MapBase.Location.Col + loc.X;
						if (   r > -1 && r < MapBase.MapSize.Rows
							&& c > -1 && c < MapBase.MapSize.Cols)
						{
							_targeterForced = !isTop;

							int pos = DragBeg.X + _keyDeltaX + loc.X;
							if (pos > -1 && pos < MapBase.MapSize.Cols)
								_keyDeltaX += loc.X;

							pos = DragBeg.Y + _keyDeltaY + loc.Y;
							if (pos > -1 && pos < MapBase.MapSize.Rows)
								_keyDeltaY += loc.Y;

							loc.X = _colOver = MapBase.Location.Col + _keyDeltaX;
							loc.Y = _rowOver = MapBase.Location.Row + _keyDeltaY;
							ProcessSelection(DragBeg, loc);
						}
					}
				}
			}

			if (!isTop)		// force redraw on every step when MainView is the active viewer
				Refresh();	// else the selector-sprite stops then jumps to the end on key up.
		}

		internal int _keyDeltaX;
		internal int _keyDeltaY;
		#endregion Keyboard navigation


		#region Mouse & drag-points
		private const int TARGETER_MOUSE    = 0; // a regular mousewheel event
		private const int TARGETER_KEY_MAIN = 1; // keyboard navigation in MainView
		private const int TARGETER_KEY_TOP  = 2; // keyboard navigation in TopView

		/// <summary>
		/// Scrolls the z-axis for MainView (and TopView by keyboard).
		/// @note 'e.Clicks' denotes where the call is coming from and how it's
		/// to be handled. It is not mouseclicks.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseWheel(MouseEventArgs e)
		{
			base.OnMouseWheel(e);

			if (MapBase != null)
			{
				if      (e.Delta < 0) MapBase.LevelUp();
				else if (e.Delta > 0) MapBase.LevelDown();

				if (e.Clicks == TARGETER_MOUSE)
				{
					_targeterForced = false;

					var loc = GetTileLocation(e.X, e.Y);
					_colOver = loc.X;
					_rowOver = loc.Y;
				}
				else // ie. is keyboard navigation
				{
					_targeterForced = (e.Clicks == TARGETER_KEY_MAIN);

					_colOver = DragEnd.X;
					_rowOver = DragEnd.Y;
				}

				ViewerFormsManager.ToolFactory.SetLevelButtonsEnabled(MapBase.Level, MapBase.MapSize.Levs);
			}
		}


		private bool _isMouseDragL;
		private bool _isMouseDragR;
		private Point _preloc;

		/// <summary>
		/// Selects a tile and/or starts a drag-select procedure.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			Select();

			if (MapBase != null)
			{
				switch (e.Button)
				{
					case MouseButtons.Left:
					{
						var loc = GetTileLocation(e.X, e.Y);
						if (   loc.X > -1 && loc.X < MapBase.MapSize.Cols
							&& loc.Y > -1 && loc.Y < MapBase.MapSize.Rows)
						{
							_keyDeltaX =
							_keyDeltaY = 0;

							_colOver = loc.X; // stop the targeter from persisting at its
							_rowOver = loc.Y; // previous location when the form is activated.

							MapBase.Location = new MapLocation( // fire SelectLocation
															loc.Y, loc.X,
															MapBase.Level);
							_isMouseDragL = true;
							ProcessSelection(loc,loc);
						}
						break;
					}

					case MouseButtons.Right:
						Cursor.Current = Cursors.SizeAll;
						_isMouseDragR = true;
						_preloc = e.Location;
						break;
				}
			}
		}

		/// <summary>
		/// uh.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseUp(MouseEventArgs e)
		{
			Cursor.Current = Cursors.Default;
			_isMouseDragL =
			_isMouseDragR = false;
		}

		private int _x = -1;	// these keep track of whether the mouse-cursor
		private int _y = -1;	// actually moves or if .NET is simply firing
								// arbitrary MouseMove events.
		/// <summary>
		/// Updates the drag-selection process.
		/// @note The MouseMove event appears to fire multiple times when the
		/// form is activated but there is no actual mouse-movement; so good
		/// luck with that. Workaround: '_x' and '_y'.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (MapBase != null)
			{
				if (e.Button == MouseButtons.Right)
				{
					if (_isMouseDragR)
					{
						Point delta = _preloc - (Size)e.Location;

						MainViewUnderlay.that.ScrollHori(delta.X);
						MainViewUnderlay.that.ScrollVert(delta.Y);
					}
				}
				else if (e.X != _x || e.Y != _y)
				{
					_x = e.X; _y = e.Y;

					var loc = GetTileLocation(e.X, e.Y);

					_targeterForced = false;
					_colOver = loc.X;
					_rowOver = loc.Y;

					if (_isMouseDragL
						&& (_colOver != DragEnd.X || _rowOver != DragEnd.Y))
					{
						_keyDeltaX = _colOver - DragBeg.X;	// NOTE: These are in case a mousedrag-selection protocol stops
						_keyDeltaY = _rowOver - DragBeg.Y;	// but the selection protocol is then continued using the keyboard.
															// TODO: Implement [Ctrl+LMB] to instantly select an area based
						ProcessSelection(DragBeg, loc);		// on the currently selected tile ofc.
					}
					else
						Invalidate();
				}
			}
		}

		/// <summary>
		/// Sets drag-start and drag-end and fires a MouseDrag (path
		/// selected lozenge).
		/// @note Fires OnMouseDown and OnMouseMove in Main,Top,Route viewers.
		/// </summary>
		/// <param name="beg"></param>
		/// <param name="end"></param>
		internal void ProcessSelection(Point beg, Point end)
		{
			if (DragBeg != beg || DragEnd != end)
			{
				DragBeg = beg; // these ensure that the start and end points stay
				DragEnd = end; // within the bounds of the currently loaded map.
	
				if (MouseDrag != null) // path the selected-lozenge
					MouseDrag();

				RefreshViewers();

				// update SelectionSize on statusbar
				var a = GetDragBeg_abs();
				var b = GetDragEnd_abs();

				XCMainWindow.that.sb_PrintSelectionSize(
													b.X - a.X + 1,
													b.Y - a.Y + 1);
			}
		}

		/// <summary>
		/// Gets the drag-begin point as a lesser value than the drag-end point.
		/// See 'DragBeg' for bounds.
		/// </summary>
		/// <returns>the lesser of two evils</returns>
		internal Point GetDragBeg_abs()
		{
			return new Point(
						Math.Min(DragBeg.X, DragEnd.X),
						Math.Min(DragBeg.Y, DragEnd.Y));
		}

		/// <summary>
		/// Gets the drag-end point as a greater value than the drag-start point.
		/// See 'DragEnd' for bounds.
		/// </summary>
		/// <returns>the greater of two evils</returns>
		internal Point GetDragEnd_abs()
		{
			return new Point(
						Math.Max(DragBeg.X, DragEnd.X),
						Math.Max(DragBeg.Y, DragEnd.Y));
		}


		private Point _dragBeg = new Point(-1,-1);
		private Point _dragEnd = new Point(-1,-1);

		/// <summary>
		/// Gets/Sets the drag-begin point.
		/// </summary>
		internal Point DragBeg
		{
			get { return _dragBeg; }
			private set
			{
				_dragBeg = value;

				if      (_dragBeg.Y < 0)                     _dragBeg.Y = 0;
				else if (_dragBeg.Y >= MapBase.MapSize.Rows) _dragBeg.Y = MapBase.MapSize.Rows - 1;

				if      (_dragBeg.X < 0)                     _dragBeg.X = 0;
				else if (_dragBeg.X >= MapBase.MapSize.Cols) _dragBeg.X = MapBase.MapSize.Cols - 1;
			}
		}

		/// <summary>
		/// Gets/Sets the drag-end point.
		/// </summary>
		internal Point DragEnd
		{
			get { return _dragEnd; }
			private set
			{
				_dragEnd = value;

				if      (_dragEnd.Y < 0)                     _dragEnd.Y = 0;
				else if (_dragEnd.Y >= MapBase.MapSize.Rows) _dragEnd.Y = MapBase.MapSize.Rows - 1;

				if      (_dragEnd.X < 0)                     _dragEnd.X = 0;
				else if (_dragEnd.X >= MapBase.MapSize.Cols) _dragEnd.X = MapBase.MapSize.Cols - 1;
			}
		}
		#endregion Mouse & drag-points


		#region Events
		/// <summary>
		/// Fires when a location is selected in MainView.
		/// </summary>
		/// <param name="args"></param>
		internal void OnSelectLocationMain(SelectLocationEventArgs args)
		{
			//LogFile.WriteLine("MainViewOverlay.OnSelectLocationMain");

			FirstClick = true;

			_col = args.Location.Col;
			_row = args.Location.Row;
			_lev = args.Location.Lev;

			XCMainWindow.that.sb_PrintPosition(_col, _row, _lev);
		}

		/// <summary>
		/// Fires when the map level changes in MainView.
		/// </summary>
		/// <param name="args"></param>
		internal void OnSelectLevelMain(SelectLevelEventArgs args)
		{
			//LogFile.WriteLine("MainViewOverlay.OnSelectLevelMain");

			_lev = args.Level;

			XCMainWindow.that.sb_PrintPosition(_col, _row, _lev);
			Invalidate();
		}
		#endregion Events


		#region Draw
#if !LOCKBITS
		int _halfwidth2, _halfheight5; // standard draw only.

		/// <summary>
		/// Dimension (scaled both x and y) of a drawn sprite.
		/// </summary>
		private int _d; // Mono draw only.
#else
		Bitmap _b;
#endif

		/// <summary>
		/// Draws the Map in MainView.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{
//			base.OnPaint(e);

			if (MapBase != null)
			{
				_targeterSuppressed = !_targeterForced
								   && (!Focused || !ClientRectangle.Contains(PointToClient(Cursor.Position)));

				_graphics = e.Graphics;
				_graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
#if !LOCKBITS
				if (!XCMainWindow.UseMonoDraw)
				{
					_graphics.InterpolationMode = InterpolationLocal;

					if (_spriteShadeEnabled)
						_spriteAttributes.SetGamma(SpriteShadeLocal, ColorAdjustType.Bitmap);
				}
#endif

				// Image Processing using C# - https://www.codeproject.com/Articles/33838/Image-Processing-using-C
				// ColorMatrix Guide - https://docs.rainmeter.net/tips/colormatrix-guide/

				ControlPaint.DrawBorder3D(_graphics, ClientRectangle, Border3DStyle.Etched);


				_anistep = MainViewUnderlay.AniStep;

				_cols = MapBase.MapSize.Cols;
				_rows = MapBase.MapSize.Rows;

#if !LOCKBITS
				if (XCMainWindow.UseMonoDraw)
				{
					_d = (int)(Globals.Scale - 0.1) + 1; // NOTE: Globals.ScaleMinimum is 0.25; don't let it drop to negative value.
					DrawPicasso();
				}
				else
				{
					_halfwidth2  = HalfWidth  * 2;
					_halfheight5 = HalfHeight * 5;
					DrawRembrandt();
				}

#else
				_b = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
				BuildPanelImage();
//				_graphics.DrawImage(_b, 0, 0, _b.Width, _b.Height);
				_graphics.DrawImageUnscaled(_b, Point.Empty);	// uh does not draw the image unscaled. it
#endif															// still uses the DPI in the Graphics object ...
			}
		}

#if !LOCKBITS
		/// <summary>
		/// Draws the panel using the standard algorithm.
		/// @note This is nearly identical to DrawPicasso; they are separated
		/// only because they'd cause multiple calls to DrawTile() conditioned
		/// on the setting of 'UseMonoDraw' inside the lev/row/col loops.
		/// </summary>
		private void DrawRembrandt()
		{
			var rect = new Rectangle(-1,-1, 0,0); // This is different between REMBRANDT and PICASSO ->
			if (FirstClick)
			{
				var a = GetDragBeg_abs();
				var b = GetDragEnd_abs();

				rect.X = a.X;
				rect.Y = a.Y;
				rect.Width  = b.X - a.X + 1;
				rect.Height = b.Y - a.Y + 1;
			}


			MapTileBase tile;
			bool cuboid;

			int heightfactor = HalfHeight * 3;
			for (int
				lev  = MapBase.MapSize.Levs - 1;
				lev >= MapBase.Level;
				--lev)
			{
				if (_showGrid && lev == MapBase.Level)
					DrawGrid();

				for (int
						row = 0,
							startX = Origin.X,
							startY = Origin.Y + (lev * heightfactor);
						row != _rows;
						++row,
							startX -= HalfWidth,
							startY += HalfHeight)
				{
					for (int
							col = 0,
								x = startX,
								y = startY;
							col != _cols;
							++col,
								x += HalfWidth,
								y += HalfHeight)
					{
						if (cuboid = (col == DragBeg.X && row == DragBeg.Y))
						{
							CuboidSprite.DrawCuboid_Rembrandt(
														_graphics,
														x,y,
														HalfWidth,
														HalfHeight,
														false,
														lev == MapBase.Level);
						}

						if (!(tile = MapBase[row, col, lev]).Occulted
							|| lev == MapBase.Level)
						{
							// This is different between REMBRANDT and PICASSO ->
							DrawTile(
									tile as MapTile,
									x,y,
									_graySelection
										&& lev == MapBase.Level
										&& rect.Contains(col, row));
						}

						if (cuboid)
						{
							CuboidSprite.DrawCuboid_Rembrandt(
														_graphics,
														x,y,
														HalfWidth,
														HalfHeight,
														true,
														lev == MapBase.Level);
						}
					}
				}
			}

			if (rect.Width > 1 || rect.Height > 1) // This is different between REMBRANDT and PICASSO ->
			{
				DrawSelectionBorder(rect);
			}

			if (!_targeterSuppressed // draw Targeter after selection-border ->
				&& _colOver > -1 && _colOver < MapBase.MapSize.Cols
				&& _rowOver > -1 && _rowOver < MapBase.MapSize.Rows)
			{
				CuboidSprite.DrawTargeter_Rembrandt(
											_graphics,
											_colOver * HalfWidth  + Origin.X - (_rowOver * HalfWidth),
											_colOver * HalfHeight + Origin.Y + (_rowOver * HalfHeight) + (MapBase.Level * heightfactor),
											HalfWidth,
											HalfHeight);
			}
		}

		/// <summary>
		/// Draws the panel using the Mono algorithm.
		/// @note This is nearly identical to DrawRembrandt; they are separated
		/// only because they'd cause multiple calls to DrawTile() conditioned
		/// on the setting of 'UseMonoDraw' inside the lev/row/col loops.
		/// </summary>
		private void DrawPicasso()
		{
			MapTileBase tile;
			bool cuboid;

			int heightfactor = HalfHeight * 3;
			for (int
				lev  = MapBase.MapSize.Levs - 1;
				lev >= MapBase.Level;
				--lev)
			{
				if (_showGrid && lev == MapBase.Level)
					DrawGrid();

				for (int
						row = 0,
							startX = Origin.X,
							startY = Origin.Y + (lev * heightfactor);
						row != _rows;
						++row,
							startX -= HalfWidth,
							startY += HalfHeight)
				{
					for (int
							col = 0,
								x = startX,
								y = startY;
							col != _cols;
							++col,
								x += HalfWidth,
								y += HalfHeight)
					{
						if (cuboid = (col == DragBeg.X && row == DragBeg.Y))
						{
							CuboidSprite.DrawCuboid_Picasso(
														_graphics,
														x,y,
														false,
														lev == MapBase.Level);
						}

						if (!(tile = MapBase[row, col, lev]).Occulted
							|| lev == MapBase.Level)
						{
							// This is different between REMBRANDT and PICASSO ->
							DrawTile(tile as MapTile, x,y);
						}

						if (cuboid)
						{
							CuboidSprite.DrawCuboid_Picasso(
														_graphics,
														x,y,
														true,
														lev == MapBase.Level);
						}
					}
				}
			}

			if (FirstClick) // This is different between REMBRANDT and PICASSO ->
			{
				var a = GetDragBeg_abs();
				var b = GetDragEnd_abs();

				int width  = b.X - a.X + 1;
				int height = b.Y - a.Y + 1;

				if (width > 1 || height > 1)
				{
					var rect = new Rectangle(
										a.X, a.Y,
										width, height);
					DrawSelectionBorder(rect);
				}
			}

			if (!_targeterSuppressed // draw Targeter after selection-border ->
				&& _colOver > -1 && _colOver < MapBase.MapSize.Cols
				&& _rowOver > -1 && _rowOver < MapBase.MapSize.Rows)
			{
				CuboidSprite.DrawTargeter_Picasso(
											_graphics,
											_colOver * HalfWidth  + Origin.X - (_rowOver * HalfWidth),
											_colOver * HalfHeight + Origin.Y + (_rowOver * HalfHeight) + (MapBase.Level * heightfactor));
			}
		}

#else
		BitmapData _data; IntPtr _scan0;
		private void BuildPanelImage()
		{
			Graphics graphics = Graphics.FromImage(_b);
			graphics.Clear(Color.Transparent);

			_data = _b.LockBits(
							new Rectangle(0, 0, _b.Width, _b.Height),
							ImageLockMode.WriteOnly,
							PixelFormat.Format32bppArgb);
			_scan0 = _data.Scan0;


			MapTileBase tile;

//			bool isTargeted = Focused
//						   && !_suppressTargeter
//						   && ClientRectangle.Contains(PointToClient(Cursor.Position));

			for (int
				lev = MapBase.MapSize.Levs - 1;
				lev >= MapBase.Level && lev != -1;
				--lev)
			{
//				if (_showGrid && lev == MapBase.Level)
//					DrawGrid(graphics);

				for (int
						row = 0,
							startY = Origin.Y + (HalfHeight * lev * 3),
							startX = Origin.X;
						row != _rows;
						++row,
							startY += HalfHeight,
							startX -= HalfWidth)
				{
					for (int
							col = 0,
								x = startX,
								y = startY;
							col != _cols;
							++col,
								x += HalfWidth,
								y += HalfHeight)
					{
//						bool isClicked = FirstClick
//									  && (   (col == DragStart.X && row == DragStart.Y)
//										  || (col == DragEnd.X   && row == DragEnd.Y));

//						if (isClicked)
//						{
//							Cuboid.DrawCuboid(
//											graphics,
//											x, y,
//											HalfWidth,
//											HalfHeight,
//											false,
//											lev == MapBase.Level);
//						}

						tile = MapBase[row, col, lev];
						if (lev == MapBase.Level || !tile.Occulted)
						{
							DrawTile(
									tile as MapTile,
									x, y);
						}

//						if (isClicked)
//						{
//							Cuboid.DrawCuboid(
//											graphics,
//											x, y,
//											HalfWidth,
//											HalfHeight,
//											true,
//											lev == MapBase.Level);
//						}
//						else if (isTargeted
//							&& col == _colOver
//							&& row == _rowOver
//							&& lev == MapBase.Level)
//						{
//							Cuboid.DrawTargeter(
//											graphics,
//											x, y,
//											HalfWidth,
//											HalfHeight);
//						}
					}
				}
			}
			_b.UnlockBits(_data);

			if (FirstClick)
			{
				var start = GetAbsoluteDragStart();
				var end   = GetAbsoluteDragEnd();

				int width  = end.X - start.X + 1;
				int height = end.Y - start.Y + 1;

				if (    width > 2 || height > 2
					|| (width > 1 && height > 1))
				{
					var dragrect = new Rectangle(
											start.X, start.Y,
											width, height);
					DrawSelectionBorder(dragrect, graphics);
				}
			}

			graphics.Dispose();
		}
#endif

#if !LOCKBITS
		/// <summary>
		/// Draws the grid-lines and the grid-sheet.
		/// </summary>
		private void DrawGrid()
		{
			int x = Origin.X + HalfWidth;
			int y = Origin.Y + HalfHeight * (MapBase.Level + 1) * 3;

			int x1 = _rows * HalfWidth;
			int y1 = _rows * HalfHeight;

			var pt0 = new Point(x, y);
			var pt1 = new Point(
							x + _cols * HalfWidth,
							y + _cols * HalfHeight);
			var pt2 = new Point(
							x + (_cols - _rows) * HalfWidth,
							y + (_rows + _cols) * HalfHeight);
			var pt3 = new Point(x - x1, y + y1);

			_layerFill.Reset();
			_layerFill.AddLine(pt0, pt1);
			_layerFill.AddLine(pt1, pt2);
			_layerFill.AddLine(pt2, pt3);
			_layerFill.CloseFigure();

			_graphics.FillPath(_brushLayer, _layerFill); // the grid-sheet

			// draw the grid-lines ->
			Pen pen;
			for (int i = 0; i <= _rows; ++i)
			{
				if (i % 10 == 0) pen = _penGrid10;
				else             pen = _penGrid;

				_graphics.DrawLine(
								pen,
								x - HalfWidth  * i,
								y + HalfHeight * i,
								x + (_cols - i) * HalfWidth,
								y + (_cols + i) * HalfHeight);
			}

			for (int i = 0; i <= _cols; ++i)
			{
				if (i % 10 == 0) pen = _penGrid10;
				else             pen = _penGrid;

				_graphics.DrawLine(
								pen,
								x + HalfWidth  * i,
								y + HalfHeight * i,
								x - x1 + HalfWidth  * i,
								y + y1 + HalfHeight * i);
			}
		}
#else
		/// <summary>
		/// Draws the grid-lines and the grid-sheet.
		/// </summary>
		/// <param name="graphics"></param>
		private void DrawGrid(Graphics graphics)
		{
			int x = Origin.X + HalfWidth;
			int y = Origin.Y + HalfHeight * (MapBase.Level + 1) * 3;

			int x1 = _rows * HalfWidth;
			int y1 = _rows * HalfHeight;

			var pt0 = new Point(x, y);
			var pt1 = new Point(
							x + _cols * HalfWidth,
							y + _cols * HalfHeight);
			var pt2 = new Point(
							x + (_cols - _rows) * HalfWidth,
							y + (_rows + _cols) * HalfHeight);
			var pt3 = new Point(x - x1, y + y1);

			_layerFill.Reset();
			_layerFill.AddLine(pt0, pt1);
			_layerFill.AddLine(pt1, pt2);
			_layerFill.AddLine(pt2, pt3);
			_layerFill.CloseFigure();

			graphics.FillPath(_brushLayer, _layerFill); // the grid-sheet

			// draw the grid-lines ->
			Pen pen;
			for (int i = 0; i <= _rows; ++i)
			{
				if (i % 10 == 0) pen = _penGrid10;
				else             pen = _penGrid;

				graphics.DrawLine(
								_penGrid,
								x - HalfWidth  * i,
								y + HalfHeight * i,
								x + (_cols - i) * HalfWidth,
								y + (_cols + i) * HalfHeight);
			}

			for (int i = 0; i <= _cols; ++i)
			{
				if (i % 10 == 0) pen = _penGrid10;
				else             pen = _penGrid;

				graphics.DrawLine(
								_penGrid,
								x + HalfWidth  * i,
								y + HalfHeight * i,
								x - x1 + HalfWidth  * i,
								y + y1 + HalfHeight * i);
			}
		}
#endif

		/// <summary>
		/// Draws the tileparts in the Tile if 'UseMono' or LOCKBITS.
		/// </summary>
		/// <param name="tile"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		private void DrawTile(
				MapTile tile,
				int x, int y)
		{
			// NOTE: The width and height args are based on a sprite that's 32x40.
			// Going back to a universal sprite-size would do this:
			//   (int)(sprite.Width  * Globals.Scale)
			//   (int)(sprite.Height * Globals.Scale)
			// with its attendent consequences.

			Tilepart part;

			var topView = ViewerFormsManager.TopView.Control;
			if (topView.Floor.Checked
				&& (part = tile.Floor) != null)
			{
				DrawSprite(
						part[_anistep].Bindata,
						x, y - part.Record.TileOffset * HalfHeight / HalfHeightConst);
			}

			if (topView.West.Checked
				&& (part = tile.West) != null)
			{
				DrawSprite(
						part[_anistep].Bindata,
						x, y - part.Record.TileOffset * HalfHeight / HalfHeightConst);
			}

			if (topView.North.Checked
				&& (part = tile.North) != null)
			{
				DrawSprite(
						part[_anistep].Bindata,
						x, y - part.Record.TileOffset * HalfHeight / HalfHeightConst);
			}

			if (topView.Content.Checked
				&& (part = tile.Content) != null)
			{
				DrawSprite(
						part[_anistep].Bindata,
						x, y - part.Record.TileOffset * HalfHeight / HalfHeightConst);
			}
		}

#if !LOCKBITS
		/// <summary>
		/// Draws the tileparts in the Tile if not 'UseMono'.
		/// </summary>
		/// <param name="tile"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="gray">true to draw the grayscale version of any tile-sprites</param>
		private void DrawTile(
				MapTile tile,
				int x, int y,
				bool gray)
		{
			// NOTE: The width and height args are based on a sprite that's 32x40.
			// Going back to a universal sprite-size would do this:
			//   (int)(sprite.Width  * Globals.Scale)
			//   (int)(sprite.Height * Globals.Scale)
			// with its attendent consequences.

			Tilepart part;
			var rect = new Rectangle(
								x, y,
								_halfwidth2, _halfheight5);

			var topView = ViewerFormsManager.TopView.Control;
			if (topView.Floor.Checked
				&& (part = tile.Floor) != null)
			{
				var sprite = (gray) ? part[_anistep].SpriteGr
									: part[_anistep].Sprite;
				rect.Y -= part.Record.TileOffset * HalfHeight / HalfHeightConst;
				DrawSprite(sprite, rect);
			}

			if (topView.West.Checked
				&& (part = tile.West) != null)
			{
				var sprite = (gray) ? part[_anistep].SpriteGr
									: part[_anistep].Sprite;
				rect.Y -= part.Record.TileOffset * HalfHeight / HalfHeightConst;
				DrawSprite(sprite, rect);
			}

			if (topView.North.Checked
				&& (part = tile.North) != null)
			{
				var sprite = (gray) ? part[_anistep].SpriteGr
									: part[_anistep].Sprite;
				rect.Y -= part.Record.TileOffset * HalfHeight / HalfHeightConst;
				DrawSprite(sprite, rect);
			}

			if (topView.Content.Checked
				&& (part = tile.Content) != null)
			{
				var sprite = (gray) ? part[_anistep].SpriteGr
									: part[_anistep].Sprite;
				rect.Y -= part.Record.TileOffset * HalfHeight / HalfHeightConst;
				DrawSprite(sprite, rect);
			}
		}

		/// <summary>
		/// Draws a tilepart's sprite w/ FillRectangle().
		/// </summary>
		/// <param name="bindata">binary data of XCImage (list of palette-ids)</param>
		/// <param name="x">x-pixel start</param>
		/// <param name="y">y-pixel start</param>
		private void DrawSprite(IList<byte> bindata, int x, int y)
		{
			int palid;

			int i = -1, w,h;
			for (h = 0; h != XCImage.SpriteHeight40; ++h)
			for (w = 0; w != XCImage.SpriteWidth32;  ++w)
			{
				palid = bindata[++i];
				if (palid != Palette.TranId) // <- this is the fix for Mono.
				{
					_graphics.FillRectangle(
										SpriteBrushes[palid],
										x + (int)(w * Globals.Scale),
										y + (int)(h * Globals.Scale),
										_d, _d);
				}
			}
		}

		/// <summary>
		/// Draws a tilepart's sprite w/ DrawImage().
		/// </summary>
		/// <param name="sprite"></param>
		/// <param name="rect">destination rectangle</param>
		private void DrawSprite(Image sprite, Rectangle rect)
		{
			if (_spriteShadeEnabled)
				_graphics.DrawImage(
								sprite,
								rect,
								0, 0, XCImage.SpriteWidth32, XCImage.SpriteHeight40,
								GraphicsUnit.Pixel,
								_spriteAttributes);
			else
				_graphics.DrawImage(sprite, rect);
		}
#else
		/// <summary>
		/// Draws a tilepart's sprite w/ LockBits().
		/// </summary>
		/// <param name="bindata">binary data of XCImage (list of palette-ids)</param>
		/// <param name="x0">x-pixel start</param>
		/// <param name="y0">y-pixel start</param>
		private void DrawSprite(IList<byte> bindata, int x0, int y0)
		{
//			var data = _b.LockBits(
//								new Rectangle(0, 0, _b.Width, _b.Height),
//								ImageLockMode.WriteOnly,
//								PixelFormat.Format32bppArgb);
//			var scan0 = data.Scan0;

			unsafe
			{
//				byte* dstPos;
//				if (dstLocked.Stride > 0)
//					dstPos = (byte*)dstStart.ToPointer();
//				else
//					dstPos = (byte*)dstStart.ToPointer() + dstLocked.Stride * (_b.Height - 1);
//				uint dstStride = (uint)Math.Abs(dstLocked.Stride);

				var start = (byte*)_scan0.ToPointer();

				uint stride = (uint)_data.Stride;

				byte palid;
				int i = -1;

				byte* pos;

//x + (int)(w * Globals.Scale),
//y + (int)(h * Globals.Scale),
//_d = (int)(Globals.Scale - 0.1) + 1; // NOTE: Globals.ScaleMinimum is 0.25; don't let it drop to negative value.

				uint x,y,offset;
				for (y = 0; y != XCImage.SpriteHeight40; ++y)
				for (x = 0; x != XCImage.SpriteWidth;    ++x)
				{
					palid = bindata[++i];

					if (palid != Palette.TranId)
					{
						pos = start
							+ (((uint)y0 + y) * stride)
							+ (((uint)x0 + x) * 4);
						for (offset = 0; offset != 4; ++offset) // 4 bytes in dest-pixel.
						{
							switch (offset)
							{
								case 0: pos[offset] = Palette.UfoBattle[palid].B; break;
								case 1: pos[offset] = Palette.UfoBattle[palid].G; break;
								case 2: pos[offset] = Palette.UfoBattle[palid].R; break;
								case 3: pos[offset] = 255;                        break;
							}
						}
					}
				}
			}
//			_b.UnlockBits(data);
		}
#endif

#if !LOCKBITS
		/// <summary>
		/// Draws a colored lozenge around any selected Tiles.
		/// </summary>
		/// <param name="dragrect"></param>
		private void DrawSelectionBorder(Rectangle dragrect)
		{
			var t = GetClientCoordinates(new Point(dragrect.Left,  dragrect.Top));
			var r = GetClientCoordinates(new Point(dragrect.Right, dragrect.Top));
			var b = GetClientCoordinates(new Point(dragrect.Right, dragrect.Bottom));
			var l = GetClientCoordinates(new Point(dragrect.Left,  dragrect.Bottom));

			t.X += HalfWidth;
			r.X += HalfWidth;
			b.X += HalfWidth;
			l.X += HalfWidth;

			_graphics.DrawLine(_penSelect, t, r);
			_graphics.DrawLine(_penSelect, r, b);
			_graphics.DrawLine(_penSelect, b, l);
			_graphics.DrawLine(_penSelect, l, t);
		}
#else
		/// <summary>
		/// Draws a colored lozenge around any selected Tiles.
		/// </summary>
		/// <param name="dragrect"></param>
		/// <param name="graphics"></param>
		private void DrawSelectionBorder(Rectangle dragrect, Graphics graphics)
		{
			var t = GetClientCoordinates(new Point(dragrect.Left,  dragrect.Top));
			var r = GetClientCoordinates(new Point(dragrect.Right, dragrect.Top));
			var b = GetClientCoordinates(new Point(dragrect.Right, dragrect.Bottom));
			var l = GetClientCoordinates(new Point(dragrect.Left,  dragrect.Bottom));

			t.X += HalfWidth;
			r.X += HalfWidth;
			b.X += HalfWidth;
			l.X += HalfWidth;

			graphics.DrawLine(_penSelect, t, r);
			graphics.DrawLine(_penSelect, r, b);
			graphics.DrawLine(_penSelect, b, l);
			graphics.DrawLine(_penSelect, l, t);
		}
#endif
		#endregion Draw


		#region Coordinate conversion
		/// <summary>
		/// Converts a point from tile-location to client-coordinates.
		/// </summary>
		/// <param name="point">the x/y-position of a tile</param>
		/// <returns></returns>
		private Point GetClientCoordinates(Point point)
		{
			int verticalOffset = HalfHeight * (MapBase.Level + 1) * 3;
			return new Point(
							Origin.X + (point.X - point.Y) * HalfWidth,
							Origin.Y + (point.X + point.Y) * HalfHeight + verticalOffset);
		}

		/// <summary>
		/// Converts a position from client-coordinates to tile-location.
		/// </summary>
		/// <param name="x">the x-position of the mouse cursor in pixels wrt/ client-coords</param>
		/// <param name="y">the y-position of the mouse cursor in pixels wrt/ client-coords</param>
		/// <returns></returns>
		private Point GetTileLocation(int x, int y)
		{
			x -= Origin.X;
			y -= Origin.Y;

			double halfWidth  = (double)HalfWidth;
			double halfHeight = (double)HalfHeight;

			double verticalOffset = (MapBase.Level + 1) * 3;

			double xd = Math.Floor(x - halfWidth);						// x=0 is the axis from the top to the bottom of the map-lozenge.
			double yd = Math.Floor(y - halfHeight * verticalOffset);	// y=0 is measured from the top of the map-lozenge downward.

			double x1 = xd / (halfWidth  * 2)
					  + yd / (halfHeight * 2);
			double y1 = (yd * 2 - xd) / (halfWidth * 2);

			return new Point(
						(int)Math.Floor(x1),
						(int)Math.Floor(y1));
		}
		#endregion Coordinate conversion
	}
}
