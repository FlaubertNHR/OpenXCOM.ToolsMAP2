//#define LOCKBITS	// toggle this to change OnPaint routine in standard build.
					// Must be set in MainViewOptionables as well. Purely experimental.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;

using DSShared;
using DSShared.Controls;

using MapView.Forms.Observers;

using XCom;
using XCom.Base;



namespace MapView.Forms.MainView
{
	internal sealed class MainViewOverlay
		:
			BufferedPanel // god I hate these double-panels!!!! cf. MainViewUnderlay
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

		/// <summary>
		/// Tracks the mouseover location col.
		/// </summary>
		private int _overCol;

		/// <summary>
		/// Tracks the mouseover location row.
		/// </summary>
		private int _overRow;
		#endregion Fields


		#region Properties (static)
		internal static MainViewOverlay that
		{ get; private set; }
		#endregion Properties (static)


		#region Properties
		private MainViewF MainView;

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
		/// different Map or (c) the Map/terrains are reloaded.
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

				ObserverManager.ToolFactory.SetEditButtonsEnabled( _firstClick);
				ObserverManager.ToolFactory.SetPasteButtonsEnabled(_firstClick && _copied != null);
			}
		}

		/// <summary>
		/// List of SolidBrushes used to draw sprites from XCImage.Bindata (in
		/// Mono). Can be either UfoBattle palette brushes or TftdBattle
		/// palette brushes.
		/// </summary>
		internal List<Brush> SpriteBrushes
		{ private get; set; }

		internal SolidBrush BrushLayer
		{ get; set; }

		internal Pen PenGrid
		{ get; set; }

		internal Pen PenGrid10
		{ get; set; }

		internal Pen PenSelect
		{ get; set; }
		#endregion Properties


		#region Fields (graphics)
		private GraphicsPath _layerFill = new GraphicsPath();

		private Graphics _graphics;
		private ImageAttributes _spriteAttributes = new ImageAttributes();

		private int _anistep;
		private int _cols, _rows;
		#endregion Fields (graphics)


		#region cTor
		internal MainViewOverlay(MainViewF main)
		{
			MainView = main;

			that = MainView.MainViewOverlay = this;

			SetStyle(ControlStyles.Selectable, true);
			TabStop = true;
			TabIndex = 4; // TODO: Check that.

			GotFocus  += OnFocusGained;
			LostFocus += OnFocusLost;

			var t1 = new Timer();
			t1.Interval = Globals.PERIOD;
			t1.Enabled = true;
			t1.Tick += t1_Tick;
		}
		#endregion cTor


		#region Events and Methods for targeter-suppression
		private void OnFocusGained(object sender, EventArgs e)
		{
			if (MapBase != null)
			{
				ResetOverValues();
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

		/// <summary>
		/// Hides the cuboid-targeter when the mouse leaves the center-panel
		/// unless the targeter was enabled by a keyboard tile-selection.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void t1_Tick(object sender, EventArgs e)
		{
			if (Focused && MapBase != null && !_targeterForced && SuppressTargeter())
				Invalidate();
		}

		/// <summary>
		/// Checks if the cursor is *not* inside the ToolStripContainer's
		/// center-panel and hence if the targeter ought be suppressed.
		/// @note This funct ignores the '_targeterForced' var so that needs to
		/// be checked before call.
		/// </summary>
		/// <returns></returns>
		private bool SuppressTargeter()
		{
			MainViewUnderlay underlay = MainViewUnderlay.that;
			var centerpanel = MainViewF.that.tscPanel.ContentPanel;

			var allowablearea = centerpanel.ClientRectangle;
			if (underlay.IsVertbarVisible)
				allowablearea.Width -= underlay.WidthVertbar;

			if (underlay.IsHoribarVisible)
				allowablearea.Height -= underlay.HeightHoribar;

			return !allowablearea.Contains(centerpanel.PointToClient(Control.MousePosition));
		}
		#endregion Events and Methods for targeter-suppression


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
			FillSelectedQuads();
		}


		/// <summary>
		/// Handles keyboard-input for editing and saving the Mapfile.
		/// @note Navigation keys are handled by 'KeyPreview' at the form level.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			Edit(e);
		}

		/// <summary>
		/// Performs edit-functions by keyboard or saves the Mapfile.
		/// </summary>
		/// <param name="e"></param>
		internal void Edit(KeyEventArgs e)
		{
			switch (e.KeyData)
			{
				case Keys.Escape:
					if (MapBase != null)
					{
						_targeterForced = false;

						ResetOverValues();

						_keyDeltaX =
						_keyDeltaY = 0;

						ProcessSelection(DragBeg, DragBeg);
					}
					break;

				case Keys.F:
					FillSelectedQuads();
					break;

				case Keys.Delete:
					ClearSelection();
					break;

				case Keys.Control | Keys.S:
					MainView.OnSaveMapClick(null, EventArgs.Empty);
					break;

				case Keys.Control | Keys.X:
					Copy();
					ClearSelection();
					break;

				case Keys.Control | Keys.C:
					Copy();
					break;

				case Keys.Control | Keys.V:
					Paste();
					break;
			}
		}

		/// <summary>
		/// Resets the values of '_overCol' and '_overRow'.
		/// </summary>
		private void ResetOverValues()
		{
			var pt = PointToClient(Control.MousePosition);
				pt = GetTileLocation(pt.X, pt.Y);
			_overCol = pt.X;
			_overRow = pt.Y;
		}

		/// <summary>
		/// Clears all tileparts from any currently selected tiles.
		/// </summary>
		private void ClearSelection()
		{
			if (MapBase != null && FirstClick)
			{
				MainView.MapChanged = true;

				MapTile tile;

				int visible = ObserverManager.TopView.Control.VisibleQuadrants;

				var a = GetDragBeg_abs();
				var b = GetDragEnd_abs();

				for (int col = a.X; col <= b.X; ++col)
				for (int row = a.Y; row <= b.Y; ++row)
				{
					tile = MapBase[col, row];

					if ((visible & TopView.FLOOR)   != 0) tile.Floor   = null;
					if ((visible & TopView.WEST)    != 0) tile.West    = null;
					if ((visible & TopView.NORTH)   != 0) tile.North   = null;
					if ((visible & TopView.CONTENT) != 0) tile.Content = null;

					tile.Vacancy();
				}

				MapBase.CalculateOccultations();

				InvalidateObservers();
			}
		}


		private Dictionary<int, Tuple<string,string>> _copiedTerrains;
		private MapTile[,] _copied;

		/// <summary>
		/// Copies any selected tiles to an internal buffer.
		/// </summary>
		private void Copy()
		{
			if (MapBase != null && FirstClick)
			{
				ObserverManager.ToolFactory.SetPasteButtonsEnabled();

				_copiedTerrains = MapBase.Descriptor.Terrains;

				var a = GetDragBeg_abs();
				var b = GetDragEnd_abs();

				_copied = new MapTile[b.X - a.X + 1,
									  b.Y - a.Y + 1];

				MapTile tile;

				for (int col = a.X; col <= b.X; ++col)
				for (int row = a.Y; row <= b.Y; ++row)
				{
					tile = MapBase[col, row];
					_copied[col - a.X,
							row - a.Y] = new MapTile(
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
		private void Paste()
		{
			if (MapBase != null && FirstClick && _copied != null)
			{
				if (AllowPaste(_copiedTerrains, MapBase.Descriptor.Terrains))
				{
					MainView.MapChanged = true;

					MapTile tile, copy;

					int visible = ObserverManager.TopView.Control.VisibleQuadrants;

					for (int
							row = DragBeg.Y;
							row != MapBase.MapSize.Rows && (row - DragBeg.Y) < _copied.GetLength(1);
							++row)
					{
						for (int
								col = DragBeg.X;
								col != MapBase.MapSize.Cols && (col - DragBeg.X) < _copied.GetLength(0);
								++col)
						{
							if ((tile = MapBase[col, row]) != null
								&& (copy = _copied[col - DragBeg.X,
												   row - DragBeg.Y]) != null)
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

					InvalidateObservers();
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
											"Allocated terrains differ",
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
		/// Fills the selected quadrant of the currently selected tile(s) with
		/// the currently selected tilepart from TileView.
		/// @note Unlike Paste() this ignores quadtype visibility.
		/// </summary>
		internal void FillSelectedQuads()
		{
			if (MapBase != null && FirstClick)
			{
				MainView.MapChanged = true;

				var a = GetDragBeg_abs();
				var b = GetDragEnd_abs();

				var quad = ObserverManager.TopView .Control.QuadrantPanel.SelectedQuadrant;
				var part = ObserverManager.TileView.Control.SelectedTilepart;

				MapTile tile;
				for (int col = a.X; col <= b.X; ++col)
				for (int row = a.Y; row <= b.Y; ++row)
				{
					tile = MapBase[col, row];
					tile[quad] = part;
					tile.Vacancy();
				}

				MapBase.CalculateOccultations();

				InvalidateObservers();
			}
		}

		/// <summary>
		/// Clears the selected quadrant of the currently selected tile(s).
		/// @note Unlike ClearSelection() this ignores quadtype visibility.
		/// </summary>
		internal void ClearSelectedQuads()
		{
			MainView.MapChanged = true;

			var a = GetDragBeg_abs();
			var b = GetDragEnd_abs();

			var quad = ObserverManager.TopView .Control.QuadrantPanel.SelectedQuadrant;

			MapTile tile;
			for (int col = a.X; col <= b.X; ++col)
			for (int row = a.Y; row <= b.Y; ++row)
			{
				tile = MapBase[col, row];
				tile[quad] = null;
				tile.Vacancy();
			}

			MapBase.CalculateOccultations();

			InvalidateObservers();
		}

		/// <summary>
		/// Replaces tileparts throughout the currently loaded Mapfile.
		/// </summary>
		/// <param name="src0">start setId of parts to replace</param>
		/// <param name="src1">stop  setId of parts to replace</param>
		/// <param name="dst">setId of part to replace with</param>
		/// <param name="shift">shift parts' current id +/-</param>
		internal void SubstituteTileparts(
				int src0,
				int src1,
				int dst,
				int shift)
		{
			MainView.MapChanged = true;

			MapTile tile;
			Tilepart part;
			int id;

			int records = MapBase.Parts.Count;
			if (records > MapFileBase.MaxTerrainId)
				records = MapFileBase.MaxTerrainId;

			MapSize size = MapBase.MapSize;

			for (int lev = 0; lev != size.Levs; ++lev)
			for (int row = 0; row != size.Rows; ++row)
			for (int col = 0; col != size.Cols; ++col)
			{
				tile = MapBase[col, row, lev];

				if ((part = tile.Floor) != null
					&& (id = part.SetId) >= src0 && id <= src1)
				{
					if (dst != Int32.MaxValue)
					{
						if (dst < records) // safety. i hope
							tile.Floor = MapBase.Parts[dst];
					}
					else if (shift != Int32.MaxValue)
					{
						if ((id += shift) > -1 && id < records) // safety. i hope
							tile.Floor = MapBase.Parts[id];
					}
					else
						tile.Floor = null;
				}

				if ((part = tile.West) != null
					&& (id = part.SetId) >= src0 && id <= src1)
				{
					if (dst != Int32.MaxValue)
					{
						if (dst < records) // safety. i hope
							tile.West = MapBase.Parts[dst];
					}
					else if (shift != Int32.MaxValue)
					{
						if ((id += shift) > -1 && id < records) // safety. i hope
							tile.West = MapBase.Parts[id];
					}
					else
						tile.West = null;
				}

				if ((part = tile.North) != null
					&& (id = part.SetId) >= src0 && id <= src1)
				{
					if (dst != Int32.MaxValue)
					{
						if (dst < records) // safety. i hope
							tile.North = MapBase.Parts[dst];
					}
					else if (shift != Int32.MaxValue)
					{
						if ((id += shift) > -1 && id < records) // safety. i hope
							tile.North = MapBase.Parts[id];
					}
					else
						tile.North = null;
				}

				if ((part = tile.Content) != null
					&& (id = part.SetId) >= src0 && id <= src1)
				{
					if (dst != Int32.MaxValue)
					{
						if (dst < records) // safety. i hope
							tile.Content = MapBase.Parts[dst];
					}
					else if (shift != Int32.MaxValue)
					{
						if ((id += shift) > -1 && id < records) // safety. i hope
							tile.Content = MapBase.Parts[id];
					}
					else
						tile.Content = null;
				}
			}

			MapBase.CalculateOccultations();

			InvalidateObservers();
		}

		/// <summary>
		/// Causes this panel to redraw along with the TopView, RouteView, and
		/// TopRouteView forms - also invalidates the ScanG panel.
		/// </summary>
		/// <param name="refresh">true to refresh MainView; false to invalidate</param>
		private void InvalidateObservers(bool refresh = false)
		{
			if (refresh)
				Refresh(); // fast update for drag-select
			else
				Invalidate();

			ObserverManager.TopView     .Control     .TopPanel     .Invalidate();
			ObserverManager.TopRouteView.ControlTop  .TopPanel     .Invalidate();
			ObserverManager.TopView     .Control     .QuadrantPanel.Invalidate();
			ObserverManager.TopRouteView.ControlTop  .QuadrantPanel.Invalidate();
			ObserverManager.RouteView   .Control     .RoutePanel   .Invalidate();
			ObserverManager.TopRouteView.ControlRoute.RoutePanel   .Invalidate();

			if (MainViewF.ScanG != null)
				MainViewF.ScanG.InvalidatePanel();	// incl/ ProcessTileSelection() for selection rectangle
		}											// not used by ScanG view at present
		#endregion Events and Methods for the edit-functions


		#region Keyboard navigation
		/// <summary>
		/// Keyboard navigation called by MainViewF (form-level) key events
		/// OnKeyDown() and ProcessCmdKey().
		/// </summary>
		/// <param name="keyData"></param>
		/// <param name="isTop">true if TopView is the active viewer</param>
		internal void Navigate(Keys keyData, bool isTop = false)
		{
			if (MapBase != null && (keyData & (Keys.Control | Keys.Alt)) == Keys.None)
			{
				if (!FirstClick) // allow Shift
				{
					_keyDeltaX =
					_keyDeltaY = 0;

					MapBase.Location = new MapLocation(0,0, MapBase.Level); // fire SelectLocation

					var loc = new Point(0,0);
					ProcessSelection(loc,loc);
				}
				else if ((keyData & Keys.Shift) == Keys.None)
				{
					var loc = new Point(0,0);
					int vert = MapFileBase.LEVEL_no;

					switch (keyData)
					{
						case Keys.Up:       loc.X = -1; loc.Y = -1; break;
						case Keys.Right:    loc.X = +1; loc.Y = -1; break;
						case Keys.Down:     loc.X = +1; loc.Y = +1; break;
						case Keys.Left:     loc.X = -1; loc.Y = +1; break;

						case Keys.PageUp:   loc.Y = -1; break;
						case Keys.PageDown: loc.X = +1; break;
						case Keys.End:      loc.Y = +1; break;
						case Keys.Home:     loc.X = -1; break;

						case Keys.Add:      vert = MapFileBase.LEVEL_Dn; break;
						case Keys.Subtract: vert = MapFileBase.LEVEL_Up; break;
					}

					if (loc.X != 0 || loc.Y != 0)
					{
						int c = MapBase.Location.Col + loc.X;
						if (c > -1 && c < MapBase.MapSize.Cols)
						{
							int r = MapBase.Location.Row + loc.Y;
							if (r > -1 && r < MapBase.MapSize.Rows)
							{
								ObserverManager.RouteView   .Control     .DeselectNode(false);
								ObserverManager.TopRouteView.ControlRoute.DeselectNode(false);

								_keyDeltaX =
								_keyDeltaY = 0;

								MapBase.Location = new MapLocation(c,r, MapBase.Level); // fire SelectLocation

								loc.X = _overCol = c;
								loc.Y = _overRow = r;
								ProcessSelection(loc,loc);
							}
						}
					}
					else if (vert != MapFileBase.LEVEL_no)
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
				else // Shift = drag select ->
				{
					var loc = new Point(0,0);

					switch (keyData)
					{
						case Keys.Shift | Keys.Up:       loc.X = -1; loc.Y = -1; break;
						case Keys.Shift | Keys.Right:    loc.X = +1; loc.Y = -1; break;
						case Keys.Shift | Keys.Down:     loc.X = +1; loc.Y = +1; break;
						case Keys.Shift | Keys.Left:     loc.X = -1; loc.Y = +1; break;

						case Keys.Shift | Keys.PageUp:   loc.Y = -1; break;
						case Keys.Shift | Keys.PageDown: loc.X = +1; break;
						case Keys.Shift | Keys.End:      loc.Y = +1; break;
						case Keys.Shift | Keys.Home:     loc.X = -1; break;
					}

					if (loc.X != 0 || loc.Y != 0)
					{
						ObserverManager.RouteView   .Control     .DeselectNode(false);
						ObserverManager.TopRouteView.ControlRoute.DeselectNode(false);

						_targeterForced = !isTop;

						int pos = DragBeg.X + _keyDeltaX + loc.X;
						if (pos > -1 && pos < MapBase.MapSize.Cols)
							_keyDeltaX += loc.X;

						pos = DragBeg.Y + _keyDeltaY + loc.Y;
						if (pos > -1 && pos < MapBase.MapSize.Rows)
							_keyDeltaY += loc.Y;

						loc.X = _overCol = MapBase.Location.Col + _keyDeltaX;
						loc.Y = _overRow = MapBase.Location.Row + _keyDeltaY;
						ProcessSelection(DragBeg, loc);
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
				int dir = MapFileBase.LEVEL_no;
				if      (e.Delta < 0) dir = MapFileBase.LEVEL_Up;
				else if (e.Delta > 0) dir = MapFileBase.LEVEL_Dn;
				MapBase.ChangeLevel(dir);

				if (e.Clicks == TARGETER_MOUSE)
				{
					_targeterForced = false;

					var loc = GetTileLocation(e.X, e.Y);
					_overCol = loc.X;
					_overRow = loc.Y;
				}
				else // ie. is keyboard navigation
				{
					_targeterForced = (e.Clicks == TARGETER_KEY_MAIN);

					_overCol = DragEnd.X;
					_overRow = DragEnd.Y;
				}

				ObserverManager.ToolFactory.SetLevelButtonsEnabled(MapBase.Level, MapBase.MapSize.Levs);
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
							ObserverManager.RouteView   .Control     .DeselectNode(false);
							ObserverManager.TopRouteView.ControlRoute.DeselectNode(false);

							_keyDeltaX =
							_keyDeltaY = 0;

							_overCol = loc.X; // stop the targeter from persisting at its
							_overRow = loc.Y; // previous location when the form is activated.

							MapBase.Location = new MapLocation( // fire SelectLocation
															loc.X, loc.Y,
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
					_overCol = loc.X;
					_overRow = loc.Y;

					if (_isMouseDragL
						&& (_overCol != DragEnd.X || _overRow != DragEnd.Y))
					{
						_keyDeltaX = _overCol - DragBeg.X;	// NOTE: These are in case a mousedrag-selection protocol stops
						_keyDeltaY = _overRow - DragBeg.Y;	// but the selection protocol is then continued using the keyboard.
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
	
				MouseDrag(); // path the selected-lozenge

				InvalidateObservers(true);

				var a = GetDragBeg_abs(); // update SelectionSize on statusbar ->
				var b = GetDragEnd_abs();
				MainView.sb_PrintSelectionSize(
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
			MainView.sb_PrintPosition();
		}

		/// <summary>
		/// Fires when the map level changes in MainView.
		/// </summary>
		/// <param name="args"></param>
		internal void OnSelectLevelMain(SelectLevelEventArgs args)
		{
			//LogFile.WriteLine("MainViewOverlay.OnSelectLevelMain");

			MainView.sb_PrintPosition();
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
			//LogFile.WriteLine("");
			//LogFile.WriteLine("OnPaint()");
			//LogFile.WriteLine(Environment.StackTrace);
//			base.OnPaint(e);

			if (MapBase != null)
			{
				_targeterSuppressed = !_targeterForced && (!Focused || SuppressTargeter());

				_graphics = e.Graphics;
				_graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
#if !LOCKBITS
				if (!MainViewF.Optionables.UseMono)
				{
					_graphics.InterpolationMode = MainViewF.Optionables.InterpolationE;

					if (MainViewF.Optionables.SpriteShadeEnabled)
						_spriteAttributes.SetGamma(MainViewF.Optionables.SpriteShadeFloat, ColorAdjustType.Bitmap);
				}
#endif

				// Image Processing using C# - https://www.codeproject.com/Articles/33838/Image-Processing-using-C
				// ColorMatrix Guide - https://docs.rainmeter.net/tips/colormatrix-guide/

				ControlPaint.DrawBorder3D(_graphics, ClientRectangle, Border3DStyle.Etched);


				_anistep = MainViewUnderlay.AniStep;

				_cols = MapBase.MapSize.Cols;
				_rows = MapBase.MapSize.Rows;

#if !LOCKBITS
				if (MainViewF.Optionables.UseMono)
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
		/// on the setting of 'UseMono' inside the lev/row/col loops.
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


			MapTile tile;
			bool cuboid;

			int heightfactor = HalfHeight * 3;
			for (int
				lev  = MapBase.MapSize.Levs - 1;
				lev >= MapBase.Level;
				--lev)
			{
				if (MainViewF.Optionables.GridVisible && lev == MapBase.Level)
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
														x, y,
														HalfWidth,
														HalfHeight,
														false,
														lev == MapBase.Level);
						}

						if (!(tile = MapBase[col, row, lev]).Vacant
							&& (!tile.Occulted
								|| lev == MapBase.Level))
						{
							// This is different between REMBRANDT and PICASSO ->
							DrawTile(
									tile,
									x, y,
									MainViewF.Optionables.SelectedTileToner != 0
										&& lev == MapBase.Level
										&& rect.Contains(col, row));
						}

						if (!_targeterSuppressed
							&& col == _overCol
							&& row == _overRow
							&& lev == MapBase.Level)
						{
							CuboidSprite.DrawTargeter_Rembrandt(
														_graphics,
														x, y,
														HalfWidth,
														HalfHeight);
						}

						if (cuboid)
						{
							CuboidSprite.DrawCuboid_Rembrandt(
														_graphics,
														x, y,
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

/*			if (!_targeterSuppressed // draw Targeter after selection-border ->
				&& _overCol > -1 && _overCol < MapBase.MapSize.Cols
				&& _overRow > -1 && _overRow < MapBase.MapSize.Rows)
			{
				CuboidSprite.DrawTargeter_Rembrandt(
											_graphics,
											_overCol * HalfWidth  + Origin.X - (_overRow * HalfWidth),
											_overCol * HalfHeight + Origin.Y + (_overRow * HalfHeight) + (MapBase.Level * heightfactor),
											HalfWidth,
											HalfHeight);
			} */
		}

		/// <summary>
		/// Draws the panel using the Mono algorithm.
		/// @note This is nearly identical to DrawRembrandt; they are separated
		/// only because they'd cause multiple calls to DrawTile() conditioned
		/// on the setting of 'UseMono' inside the lev/row/col loops.
		/// </summary>
		private void DrawPicasso()
		{
			MapTile tile;
			bool cuboid;

			int heightfactor = HalfHeight * 3;
			for (int
				lev  = MapBase.MapSize.Levs - 1;
				lev >= MapBase.Level;
				--lev)
			{
				if (MainViewF.Optionables.GridVisible && lev == MapBase.Level)
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
														x, y,
														false,
														lev == MapBase.Level);
						}

						if (!(tile = MapBase[col, row, lev]).Vacant
							&& (!tile.Occulted || lev == MapBase.Level))
						{
							// This is different between REMBRANDT and PICASSO ->
							DrawTile(tile, x, y);
						}

						if (!_targeterSuppressed
							&& col == _overCol
							&& row == _overRow
							&& lev == MapBase.Level)
						{
							CuboidSprite.DrawTargeter_Picasso(
														_graphics,
														x, y);
						}

						if (cuboid)
						{
							CuboidSprite.DrawCuboid_Picasso(
														_graphics,
														x, y,
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

/*			if (!_targeterSuppressed // draw Targeter after selection-border ->
				&& _overCol > -1 && _overCol < MapBase.MapSize.Cols
				&& _overRow > -1 && _overRow < MapBase.MapSize.Rows)
			{
				CuboidSprite.DrawTargeter_Picasso(
											_graphics,
											_overCol * HalfWidth  + Origin.X - (_overRow * HalfWidth),
											_overCol * HalfHeight + Origin.Y + (_overRow * HalfHeight) + (MapBase.Level * heightfactor));
			} */
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


			MapTile tile;

//			bool isTargeted = Focused
//						   && !_suppressTargeter
//						   && ClientRectangle.Contains(PointToClient(Control.MousePosition));

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
							DrawTile(tile, x, y);
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
//							&& col == _overCol
//							&& row == _overRow
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

			_graphics.FillPath(BrushLayer, _layerFill); // the grid-sheet

			// draw the grid-lines ->
			Pen pen;
			for (int i = 0; i <= _rows; ++i)
			{
				if (i % 10 != 0) pen = PenGrid;
				else             pen = PenGrid10;

				_graphics.DrawLine(
								pen,
								x - HalfWidth  * i,
								y + HalfHeight * i,
								x + (_cols - i) * HalfWidth,
								y + (_cols + i) * HalfHeight);
			}

			for (int i = 0; i <= _cols; ++i)
			{
				if (i % 10 != 0) pen = PenGrid;
				else             pen = PenGrid10;

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

			graphics.FillPath(BrushLayer, _layerFill); // the grid-sheet

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

			var topView = ObserverManager.TopView.Control;
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
		/// <param name="toned">true to draw the monotone version of the
		/// tile-sprites</param>
		private void DrawTile(
				MapTile tile,
				int x, int y,
				bool toned)
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

			var topView = ObserverManager.TopView.Control;
			if (topView.Floor.Checked
				&& (part = tile.Floor) != null)
			{
				var sprite = toned ? (part[_anistep] as PckImage).SpriteT
								   :  part[_anistep].Sprite;
				rect.Y -= part.Record.TileOffset * HalfHeight / HalfHeightConst;
				DrawSprite(sprite, rect);
			}

			if (topView.West.Checked
				&& (part = tile.West) != null)
			{
				var sprite = toned ? (part[_anistep] as PckImage).SpriteT
								   :  part[_anistep].Sprite;
				rect.Y -= part.Record.TileOffset * HalfHeight / HalfHeightConst;
				DrawSprite(sprite, rect);
			}

			if (topView.North.Checked
				&& (part = tile.North) != null)
			{
				var sprite = toned ? (part[_anistep] as PckImage).SpriteT
								   :  part[_anistep].Sprite;
				rect.Y -= part.Record.TileOffset * HalfHeight / HalfHeightConst;
				DrawSprite(sprite, rect);
			}

			if (topView.Content.Checked
				&& (part = tile.Content) != null)
			{
				var sprite = toned ? (part[_anistep] as PckImage).SpriteT
								   :  part[_anistep].Sprite;
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
			if (MainViewF.Optionables.SpriteShadeEnabled)
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

				uint x, y, offset;
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

			_graphics.DrawLine(PenSelect, t, r);
			_graphics.DrawLine(PenSelect, r, b);
			_graphics.DrawLine(PenSelect, b, l);
			_graphics.DrawLine(PenSelect, l, t);
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
