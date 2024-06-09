//#define LOCKBITS	// toggle this to change OnPaint routine in standard build.
					// Must be set in MainViewOptionables as well. Purely experimental.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
#if LOCKBITS
using System.Drawing.Imaging;
#endif
using System.Linq;
using System.Text;
using System.Windows.Forms;

using DSShared;
using DSShared.Controls;

using MapView.Forms.Observers;

using XCom;


namespace MapView.Forms.MainView
{
	internal sealed class MainViewOverlay
		:
			BufferedPanel // god I hate these double-panels!!!! cf. MainViewUnderlay
	{
		internal void DisposeOverlay()
		{
			//Logfile.Log("MainViewOverlay.DisposeOverlay()");
			LocationFont .Dispose();
			LocationBrush.Dispose();
			_layerFill   .Dispose();
//			_t1          .Dispose();
		}


		#region Delegates
		internal delegate void MouseDragEvent();
		#endregion Delegates


		#region Events
		internal event MouseDragEvent MouseDrag;
		#endregion Events


		#region Fields (static)
		internal const int HalfWidthConst  = 16;
		internal const int HalfHeightConst =  8;

		private static readonly Font LocationFont = new Font("Verdana", 7F, FontStyle.Bold);
		internal static readonly SolidBrush LocationBrush = new SolidBrush(MainViewOptionables.def_PanelForecolor);
		#endregion Fields (static)


		#region Fields
		private MapFile _file;

		/// <summary>
		/// Suppresses display of the targeter sprite when the panel loses
		/// focus or the mouse-cursor leaves the clientarea. This gets overruled
		/// by <c><see cref="_targeterForced"/></c>.
		/// </summary>
		private bool _targeterSuppressed;

		/// <summary>
		/// Forces display of the targeter sprite at the DragEnd position when
		/// tiles are selected by keyboard. This takes priority over
		/// <c><see cref="_targeterSuppressed"/></c>.
		/// </summary>
		private bool _targeterForced;
		internal void ReleaseTargeter()
		{
			_targeterForced = false;
		}

		/// <summary>
		/// Tracks the mouseover location col.
		/// </summary>
		private int _col = -1;

		/// <summary>
		/// Tracks the mouseover location row.
		/// </summary>
		private int _row = -1;


//		private Timer _t1 = new Timer();
		private int _phase;
		#endregion Fields


		#region Properties (static)
		internal static MainViewOverlay that
		{ get; private set; }
		#endregion Properties (static)


		#region Properties
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
		/// </summary>
		/// <remarks>The operation of the flag relies on the fact that once a
		/// tile(s) has been selected on a Map there will always be a tile(s)
		/// selected until either (a) the Map is resized or (b) user loads a
		/// different Map or (c) the Map/terrains are reloaded.</remarks>
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

				ObserverManager.ToolFactory.EnableEditors(_firstClick);
				ObserverManager.ToolFactory.EnablePasters(_firstClick && _copy.terrains != null);
			}
		}

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

		private int _cols, _rows;
		#endregion Fields (graphics)


		#region Structs
		/// <summary>
		/// This is MUTATABLE muahahahahahahaa
		/// </summary>
		private struct CopiedTerrainsStruct
		{
/*			/// <summary>
			/// The <c><see cref="Descriptor"/></c> of the
			/// <c><see cref="MapFile"/></c> from which tiles are copied.
			/// </summary>
			/// <remarks>This pointer can cause a <c>Descriptor</c> to remain in
			/// memory after user unloads a <c>MapFile</c>.</remarks>
			internal Descriptor descriptor; */

			/// <summary>
			/// <c>true</c> if <c><see cref="_file"/></c> hasn't changed since
			/// the last Copy operation.
			/// </summary>
			/// <remarks>Set <c>true</c> when a Copy is performed. Set
			/// <c>false</c> in
			/// <c><see cref="SetMapFile()">SetMapFile()</see></c>.
			/// 
			/// 
			/// Do not use a pointer to <c><see cref="Descriptor"/></c> since
			/// that would prevent <c>GC</c> from releasing memory.</remarks>
			internal bool isDescriptor;

			/// <summary>
			/// A <c>List</c> of the terrain-paths in order.
			/// </summary>
			internal IList<string> terrains;

			/// <summary>
			/// The first D is the cols-dimension and the second D stores the
			/// part-ids of four <c><see cref="Tilepart">Tileparts</see></c> for
			/// each row in the col.
			/// </summary>
			/// <remarks>A part-id of <c>-1</c> denotes that a <c>Tilepart</c>
			/// is <c>null</c>.</remarks>
			internal int[,] partids;
		}
		private CopiedTerrainsStruct _copy;
		#endregion Structs


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		internal MainViewOverlay()
		{
			that = this;

			SetStyle(ControlStyles.Selectable, true);
			TabStop = true;
			TabIndex = 4; // TODO: Check that.

			GotFocus  += OnFocusGained;
			LostFocus += OnFocusLost;

//			_t1.Interval = Globals.PERIOD;
//			_t1.Enabled = true;
//			_t1.Tick += t1_Tick;
		}
		#endregion cTor


		#region Methods
		/// <summary>
		/// Sets <c><see cref="_file"/></c> and subscribes to events.
		/// </summary>
		/// <param name="file">a <c><see cref="MapFile"/></c></param>
		/// <remarks>I don't believe it is necessary to unsubscribe the handlers
		/// here from events in the old <c>MapFile</c>. The old <c>MapFile</c>
		/// held the references and it goes poof, which ought release these
		/// handlers and this <c>MainViewOverlay</c> from any further
		/// obligations.</remarks>
		internal void SetMapFile(MapFile file)
		{
			if (_file != null)
			{
				_file.LocationSelected -= OnLocationSelectedMain;
				_file.LevelSelected    -= OnLevelSelectedMain;
			}

			if ((_file = file) != null)
			{
				_file.LocationSelected += OnLocationSelectedMain;
				_file.LevelSelected    += OnLevelSelectedMain;

				_copy.isDescriptor = false;
			}
		}
		#endregion Methods


		#region Events and Methods for targeter-suppression
		/// <summary>
		/// Handles the <c>FocusGained</c> event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnFocusGained(object sender, EventArgs e)
		{
			if (_file != null)
			{
				ResetMouseoverTracker();
				Invalidate();
			}
		}

		/// <summary>
		/// Handles the <c>FocusLost</c> event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnFocusLost(object sender, EventArgs e)
		{
			if (_file != null)
			{
				_targeterForced = false;
				Invalidate();
			}
		}

//		/// <summary>
//		/// Hides the cuboid-targeter when the cursor leaves the center-panel
//		/// unless the targeter was enabled by a keyboard tile-selection.
//		/// </summary>
//		/// <param name="sender"></param>
//		/// <param name="e"></param>
//		private void t1_Tick(object sender, EventArgs e)
//		{
//			// TODO: this does not appear to be useful ...
//			// Reason. The targeter does not get drawn unless the panel is focused.
//			if (Focused && _file != null && !_targeterForced && CursorOutsideClient())
//				Invalidate();
//		}

		/// <summary>
		/// Checks if the cursor is *not* inside the ToolStripContainer's
		/// center-panel and hence if the targeter ought be suppressed.
		/// </summary>
		/// <returns></returns>
		/// <remarks>This funct ignores the <c><see cref="_targeterForced"/></c>
		/// var so that needs to be checked before call.</remarks>
		private static bool CursorOutsideClient()
		{
			MainViewUnderlay underlay = MainViewUnderlay.that;
			ToolStripContentPanel centerpanel = MainViewF.that.tscPanel.ContentPanel;

			Rectangle allowablearea = centerpanel.ClientRectangle;
			allowablearea.Width  -= underlay.GetVertbarWidth();
			allowablearea.Height -= underlay.GetHoribarHeight();

			return !allowablearea.Contains(centerpanel.PointToClient(Control.MousePosition));
		}
		#endregion Events and Methods for targeter-suppression


		#region Events and Methods for the edit-functions
		/// <summary>
		/// For subscription to toolstrip Editor button. Handles the
		/// <c>Click</c> event on the Cut button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void OnCut(object sender, EventArgs e)
		{
			Copy();
			ClearSelection();
		}

		/// <summary>
		/// For subscription to toolstrip Editor button. Handles the
		/// <c>Click</c> event on the Copy button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void OnCopy(object sender, EventArgs e)
		{
			Copy();
		}

		/// <summary>
		/// For subscription to toolstrip Editor button. Handles the
		/// <c>Click</c> event on the Paste button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void OnPaste(object sender, EventArgs e)
		{
			Paste();
		}

		/// <summary>
		/// For subscription to toolstrip Editor button. Handles the
		/// <c>Click</c> event on the Delete button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void OnDelete(object sender, EventArgs e)
		{
			ClearSelection();
		}

		/// <summary>
		/// For subscription to toolstrip Editor button. Handles the
		/// <c>Click</c> event on the Fill button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void OnFill(object sender, EventArgs e)
		{
			FillSelectedQuadrants();
		}


		/// <summary>
		/// Handles keyboard-input for editing and saving the
		/// <c><see cref="MapFile"/></c>.
		/// </summary>
		/// <param name="e"></param>
		/// <remarks>Navigation keys etc. are handled by <c>KeyPreview</c> in
		/// <c><see cref="MainViewF"/></c>.</remarks>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			//Logfile.Log("MainViewOverlay.OnKeyDown()");
			Edit(e);
		}

		/// <summary>
		/// Performs edit-functions by keyboard or saves the
		/// <c><see cref="MapFile"/></c>.
		/// </summary>
		/// <param name="e"></param>
		internal void Edit(KeyEventArgs e)
		{
			switch (e.KeyData)
			{
				case Keys.Escape:
					if (_file != null)
					{
						_targeterForced = false;

						ResetMouseoverTracker();

						_keyDeltaX =
						_keyDeltaY = 0;

						ProcessSelection(DragBeg, DragBeg);
					}
					break;

				case Keys.F:
					FillSelectedQuadrants();
					break;

				case Keys.Delete:
					ClearSelection();
					break;

				case Keys.Control | Keys.S:
					MainViewF.that.OnSaveMapClick(null, EventArgs.Empty);
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
		/// Resets the values of <c><see cref="_col"/></c> and
		/// <c><see cref="_row"/></c> on <c>[Esc]</c> and
		/// <c><see cref="OnFocusGained()">OnFocusGained()</see></c>.
		/// </summary>
		private void ResetMouseoverTracker()
		{
			Point loc = PointToClient(Control.MousePosition);
				  loc = GetTileLocation(loc.X, loc.Y);

			_col = loc.X;
			_row = loc.Y;
		}

		/// <summary>
		/// Clears all tileparts from any currently selected tiles.
		/// </summary>
		/// <remarks><c>ClearSelection()</c> respects quadrant disability unlike
		/// <c><see cref="ClearSelectedQuadrants()">ClearSelectedQuadrants()</see></c>.</remarks>
		private void ClearSelection()
		{
			if (_file != null && FirstClick)
			{
				MainViewF.that.MapChanged = true;

				MapTile tile;

				Point a = GetDragBeg_abs();
				Point b = GetDragEnd_abs();

				for (int col = a.X; col <= b.X; ++col)
				for (int row = a.Y; row <= b.Y; ++row)
				{
					tile = _file.GetTile(col, row);

					if (!_disFloor)   tile.Floor   = null;
					if (!_disWest)    tile.West    = null;
					if (!_disNorth)   tile.North   = null;
					if (!_disContent) tile.Content = null;

					tile.Vacancy();
				}

				_file.CalculateOccultations();

				InvalidateObservers();
			}
		}

		/// <summary>
		/// Updates data in <c><see cref="_copy"/></c>.
		/// </summary>
		/// <remarks>Disrespects quadrant visibility.</remarks>
		private void Copy()
		{
			if (_file != null && FirstClick)
			{
				ObserverManager.ToolFactory.EnablePasters();

				_copy.isDescriptor = true;

				Descriptor descriptor = _file.Descriptor;

				var terrains = new List<string>();
				for (int i = 0; i != descriptor.Terrains.Count; ++i)
				{
					terrains.Add(descriptor.GetTerrainPathfile(i));
				}
				_copy.terrains = terrains;

				Point a = GetDragBeg_abs();
				Point b = GetDragEnd_abs();

				var partids = new int[b.X - a.X + 1,		// for each element of the cols-dimension
									 (b.Y - a.Y + 1) * 4];	// each element of the rows-dimension has 4 part-ids

				MapTile tile;

				int id;
				for (int col = a.X;        col <= b.X; ++col)
				for (int row = a.Y, r = 0; row <= b.Y; ++row)
				{
					tile = _file.GetTile(col, row);

					if (tile.Floor != null) id = tile.Floor.SetId;
					else                    id = -1;

					partids[col - a.X,
							row - a.Y + r] = id;

					if (tile.West != null) id = tile.West.SetId;
					else                   id = -1;

					partids[col - a.X,
							row - a.Y + ++r] = id;

					if (tile.North != null) id = tile.North.SetId;
					else                    id = -1;

					partids[col - a.X,
							row - a.Y + ++r] = id;

					if (tile.Content != null) id = tile.Content.SetId;
					else                      id = -1;

					partids[col - a.X,
							row - a.Y + ++r] = id;
				}
				_copy.partids = partids;
			}
		}

		/// <summary>
		/// Pastes the
		/// <c><see cref="CopiedTerrainsStruct.partids">CopiedTerrainsStruct.partids</see></c>
		/// 2d-array of part-ids to the currently selected location.
		/// </summary>
		/// <remarks>The terrainset of the current tileset needs to be identical
		/// to the terrainset of the tileset from which parts were copied (or
		/// nearly so).
		/// 
		/// 
		/// <c>Paste()</c> respects quadrant disability unlike
		/// <c><see cref="FillSelectedQuadrants()">FillSelectedQuadrants()</see></c>.</remarks>
		private void Paste()
		{
			if (_file != null && FirstClick && _copy.terrains != null)
			{
				if (AreTerrainsetsCompatible())
				{
					MainViewF.that.MapChanged = true;

					MapTile tile;

					int[,] partids = _copy.partids;

					for (int
							col = DragBeg.X, c = 0;
							col != _file.Cols && col - DragBeg.X < partids.GetLength(0);		// for each element of the cols-dimension
							++col, ++c)
					for (int
							row = DragBeg.Y, r = 0;
							row != _file.Rows && row - DragBeg.Y < partids.GetLength(1) / 4;	// each element of the rows-dimension has 4 tile-ids
							++row, ++r)
					{
						tile = _file.GetTile(col, row);

						if (!_disFloor)
						{
							if (partids[c,r] != -1) tile.Floor = _file.Parts[partids[c,r]];
							else                    tile.Floor = null;
						}

						++r;
						if (!_disWest)
						{
							if (partids[c,r] != -1) tile.West = _file.Parts[partids[c,r]];
							else                    tile.West = null;
						}

						++r;
						if (!_disNorth)
						{
							if (partids[c,r] != -1) tile.North = _file.Parts[partids[c,r]];
							else                    tile.North = null;
						}

						++r;
						if (!_disContent)
						{
							if (partids[c,r] != -1) tile.Content = _file.Parts[partids[c,r]];
							else                    tile.Content = null;
						}

						tile.Vacancy();
					}

					_file.CalculateOccultations();

					InvalidateObservers();
				}
				else
				{
					var sb = new StringBuilder();

					sb.Append("copied:");
					foreach (var terrain in _copy.terrains)
					{
						sb.AppendLine();
						sb.Append(terrain);
					}

					sb.AppendLine();
					sb.AppendLine();

					sb.Append("currently allocated:");
					for (int i = 0; i != _file.Descriptor.Terrains.Count; ++i)
					{
						sb.AppendLine();
						sb.Append(_file.Descriptor.GetTerrainPathfile(i));
					}

					using (var f = new Infobox(
											"Allocated terrains differ",
											Infobox.SplitString("The list of terrains that were copied are too"
													+ " different from the terrains in the currently loaded Map."),
											sb.ToString(),
											InfoboxType.Error))
					{
						f.ShowDialog(this);
					}
				}
			}
		}

		/// <summary>
		/// Compares the terrainset of the currently loaded
		/// <c><see cref="MapFile">MapFile's</see></c>
		/// <c><see cref="Descriptor"/></c> with the terrainset defined in
		/// <c><see cref="_copy"/></c>. Checks if two terrain definitions are or
		/// are nearly identical.
		/// </summary>
		/// <returns><c>true</c> if the two terrain definitions are close enough
		/// to allow a Paste operation without causing complete and utter mayhem</returns>
		private bool AreTerrainsetsCompatible()
		{
			if (!_copy.isDescriptor)
			{
				Descriptor descriptor = _file.Descriptor;

				if (descriptor.Terrains.Count < _copy.terrains.Count)
					return false;

				for (int i = 0; i != _copy.terrains.Count; ++i)
				{
					if (descriptor.GetTerrainPathfile(i) != _copy.terrains[i])
						return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Fills the selected quadrant of the currently selected tile(s) with
		/// the currently selected tilepart from <c><see cref="TileView"/></c>.
		/// </summary>
		/// <remarks><c>FillSelectedQuadrants()</c> ignores quadrant disability
		/// unlike <c><see cref="Paste()">Paste()</see></c>.</remarks>
		internal void FillSelectedQuadrants()
		{
			if (_file != null && FirstClick)
			{
				Tilepart part = ObserverManager.TileView.Control.SelectedTilepart;
				if (part == null
					|| part.SetId <= MapFile.MaxTerrainId)
				{
					MainViewF.that.MapChanged = true;

					Point a = GetDragBeg_abs();
					Point b = GetDragEnd_abs();

					PartType quadrant = QuadrantControl.SelectedQuadrant;

					MapTile tile;
					for (int col = a.X; col <= b.X; ++col)
					for (int row = a.Y; row <= b.Y; ++row)
					{
						(tile = _file.GetTile(col, row))[quadrant] = part;
						tile.Vacancy();
					}

					_file.CalculateOccultations();

					InvalidateObservers();
				}
				else
				{
					using (var f = new Infobox(
											"Error",
											Infobox.SplitString("Cannot place a tilepart that has setId greater than "
													+ MapFile.MaxTerrainId + ". The value cannot be written to"
													+ " a Mapfile due to the 1-byte restriction on Tilepart ids."),
											null,
											InfoboxType.Error))
					{
						f.ShowDialog(this);
					}
				}
			}
		}

		/// <summary>
		/// Clears the selected quadrant of the currently selected tile(s).
		/// </summary>
		/// <remarks><c>ClearSelectedQuadrants()</c> ignores quadrant disability
		/// unlike <c><see cref="ClearSelection()">ClearSelection()</see></c>.</remarks>
		internal void ClearSelectedQuadrants()
		{
			MainViewF.that.MapChanged = true;

			Point a = GetDragBeg_abs();
			Point b = GetDragEnd_abs();

			PartType quadrant = QuadrantControl.SelectedQuadrant;

			MapTile tile;
			for (int col = a.X; col <= b.X; ++col)
			for (int row = a.Y; row <= b.Y; ++row)
			{
				tile = _file.GetTile(col, row);
				tile[quadrant] = null;
				tile.Vacancy();
			}

			_file.CalculateOccultations();

			InvalidateObservers();
		}


		/// <summary>
		/// Replaces <c><see cref="Tilepart">Tileparts</see></c> throughout the
		/// currently loaded <c><see cref="MapFile"/></c>.
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
			MainViewF.that.MapChanged = true;

			MapTile tile;
			Tilepart part;
			int id;

			int records = Math.Min(_file.Parts.Count, MapFile.MaxMcdRecords);	// NOTE: Also checked in the TilepartSubstitution
																				// dialog else the Accept button does not enable.
			for (int lev = 0; lev != _file.Levs; ++lev)
			for (int row = 0; row != _file.Rows; ++row)
			for (int col = 0; col != _file.Cols; ++col)
			{
				tile = _file.GetTile(col, row, lev);

				if ((part = tile.Floor) != null
					&& (id = part.SetId) >= src0 && id <= src1)
				{
					if (dst != Int32.MaxValue)
					{
						if (dst < records) // safety. i hope
							tile.Floor = _file.Parts[dst];
					}
					else if (shift != Int32.MaxValue)
					{
						if ((id += shift) > -1 && id < records) // safety. i hope
							tile.Floor = _file.Parts[id];
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
							tile.West = _file.Parts[dst];
					}
					else if (shift != Int32.MaxValue)
					{
						if ((id += shift) > -1 && id < records) // safety. i hope
							tile.West = _file.Parts[id];
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
							tile.North = _file.Parts[dst];
					}
					else if (shift != Int32.MaxValue)
					{
						if ((id += shift) > -1 && id < records) // safety. i hope
							tile.North = _file.Parts[id];
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
							tile.Content = _file.Parts[dst];
					}
					else if (shift != Int32.MaxValue)
					{
						if ((id += shift) > -1 && id < records) // safety. i hope
							tile.Content = _file.Parts[id];
					}
					else
						tile.Content = null;
				}
			}

			_file.CalculateOccultations();

			InvalidateObservers();
		}


		/// <summary>
		/// Causes this panel to redraw along with the TopView, RouteView, and
		/// TopRouteView forms - also invalidates the ScanG panel.
		/// </summary>
		/// <param name="refresh"><c>true</c> to refresh MainView; <c>false</c>
		/// to invalidate</param>
		private void InvalidateObservers(bool refresh = false)
		{
			if (refresh)
				Refresh(); // fast update for drag-select
			else
				Invalidate();

			ObserverManager.InvalidateTopControls();
			ObserverManager.InvalidateQuadrantControls();
			RouteView.InvalidatePanels();

			if (MainViewF.ScanG != null)
				MainViewF.ScanG.InvalidatePanel();	// incl/ ProcessTileSelection() for selection rectangle
		}											// not used by ScanG view at present
		#endregion Events and Methods for the edit-functions


		#region Keyboard navigation
		/// <summary>
		/// Keyboard navigation called by <c><see cref="MainViewF"/></c>
		/// key-events <c>OnKeyDown()</c> and <c>ProcessCmdKey()</c>.
		/// </summary>
		/// <param name="keyData"></param>
		/// <param name="top"><c>true</c> if <c>TopView</c> is the active viewer</param>
		internal void Navigate(Keys keyData, bool top = false)
		{
			if (_file != null && (keyData & (Keys.Control | Keys.Alt)) == Keys.None)
			{
				if (!FirstClick) // allow Shift
				{
					_keyDeltaX =
					_keyDeltaY = 0;

					_file.Location = new MapLocation(0,0, _file.Level); // fire LocationSelected

					var loc = new Point(0,0);
					ProcessSelection(loc,loc);
				}
				else if ((keyData & Keys.Shift) == Keys.None)
				{
					var loc = new Point(0,0);
					int vert = MapFile.LEVEL_no;

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

						case Keys.Add:      vert = MapFile.LEVEL_Dn; break;
						case Keys.Subtract: vert = MapFile.LEVEL_Up; break;
					}

					if (loc.X != 0 || loc.Y != 0)
					{
						int c = _file.Location.Col + loc.X;
						if (c > -1 && c < _file.Cols)
						{
							int r = _file.Location.Row + loc.Y;
							if (r > -1 && r < _file.Rows)
							{
								RouteView.DeselectNodeStatic(true);

								_keyDeltaX =
								_keyDeltaY = 0;

								_file.Location = new MapLocation(c,r, _file.Level); // fire LocationSelected

								loc.X = _col = c;
								loc.Y = _row = r;
								ProcessSelection(loc,loc);
							}
						}
					}
					else if (vert != MapFile.LEVEL_no)
					{
						int level = _file.Location.Lev + vert;
						if (level > -1 && level < _file.Levs) // safety.
						{
							OnMouseWheel(new MouseEventArgs(
														MouseButtons.None,
														top ? TARGETER_KEY_TOP : TARGETER_KEY_MAIN, // WARNING: this is a trick (sic)
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
						RouteView.DeselectNodeStatic(true);

						_targeterForced = !top;

						int pos = DragBeg.X + _keyDeltaX + loc.X;
						if (pos > -1 && pos < _file.Cols)
							_keyDeltaX += loc.X;

						pos = DragBeg.Y + _keyDeltaY + loc.Y;
						if (pos > -1 && pos < _file.Rows)
							_keyDeltaY += loc.Y;

						loc.X = _col = _file.Location.Col + _keyDeltaX;
						loc.Y = _row = _file.Location.Row + _keyDeltaY;
						ProcessSelection(DragBeg, loc);
					}
				}

				if (!top)		// force redraw on every step when MainView is the active viewer
					Refresh();	// else the selector-sprite stops then jumps to the end on key up.
			}
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
		/// </summary>
		/// <param name="e"></param>
		/// <remarks><c>e.Clicks</c> denotes where the call is coming from and
		/// how it's to be handled. It is not mouseclicks.</remarks>
		protected override void OnMouseWheel(MouseEventArgs e)
		{
			base.OnMouseWheel(e);

			if (_file != null)
			{
				int delta;
				if (MainViewF.Optionables.InvertMousewheel)
					delta = -e.Delta;
				else
					delta =  e.Delta;

				int dir = MapFile.LEVEL_no;
				if      (delta < 0) dir = MapFile.LEVEL_Up;
				else if (delta > 0) dir = MapFile.LEVEL_Dn;
				_file.ChangeLevel(dir);

				if (e.Clicks == TARGETER_MOUSE)
				{
					_targeterForced = false;

					Point loc = GetTileLocation(e.X, e.Y);
					_col = loc.X;
					_row = loc.Y;
				}
				else // ie. is keyboard navigation
				{
					_targeterForced = (e.Clicks == TARGETER_KEY_MAIN);

					_col = DragEnd.X;
					_row = DragEnd.Y;
				}

				ObserverManager.ToolFactory.EnableLevelers(_file.Level, _file.Levs);
			}
		}


		private bool _isMouseDragL; // is a drag-selection
		private bool _isMouseDragR; // scrolls the Map when zoomed-in
		private Point _preloc;

		/// <summary>
		/// Selects a tile and/or starts a drag-select procedure.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (_file != null)
			{
				Select();

				switch (e.Button)
				{
					case MouseButtons.Left:
					{
						if (   _col > -1 && _col < _file.Cols
							&& _row > -1 && _row < _file.Rows)
						{
							RouteView.DeselectNodeStatic(true);

							_keyDeltaX =
							_keyDeltaY = 0;

							var loc = new Point(_col, _row);

							_file.Location = new MapLocation( // fire LocationSelected
														loc.X, loc.Y,
														_file.Level);
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
			else
				MainViewF.that.MapTree.Select();
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

		/// <summary>
		/// Updates the drag-selection process.
		/// </summary>
		/// <param name="e"></param>
		/// <remarks>The MouseMove event appears to fire multiple times when the
		/// form is activated but there is no actual mouse-movement; so good
		/// luck with that. Workaround: '_x' and '_y'.</remarks>
		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (_file != null)
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
				else
				{
					Point loc = GetTileLocation(e.X, e.Y);
					if (loc.X != _col || loc.Y != _row)
					{
						_col = loc.X;
						_row = loc.Y;

						_targeterForced = false;

						if (_isMouseDragL
							&& (_col != DragEnd.X || _row != DragEnd.Y))
						{
							_keyDeltaX = _col - DragBeg.X;	// NOTE: These are in case a mousedrag-selection protocol stops
							_keyDeltaY = _row - DragBeg.Y;	// but the selection protocol is then continued using the keyboard.
															// TODO: Implement [Ctrl+LMB] to instantly select an area based
							ProcessSelection(DragBeg, loc);	// on the currently selected tile ofc.
						}
						else
							Invalidate();
					}
				}
			}
		}

		/// <summary>
		/// Sets drag-start and drag-end and fires MouseDrag (path selected
		/// lozenge).
		/// </summary>
		/// <param name="beg"></param>
		/// <param name="end"></param>
		/// <remarks>Fires OnMouseDown and OnMouseMove in Main,Top,Route
		/// viewers.</remarks>
		internal void ProcessSelection(Point beg, Point end)
		{
			if (DragBeg != beg || DragEnd != end)
			{
				DragBeg = beg; // these ensure that the start and end points stay
				DragEnd = end; // within the bounds of the currently loaded map.
	
				MouseDrag(); // path the selected-lozenge

				InvalidateObservers(true);

				Point a = GetDragBeg_abs(); // update SelectionSize on statusbar ->
				Point b = GetDragEnd_abs();
				MainViewF.that.sb_PrintSelectionSize(
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

				if      (_dragBeg.Y < 0)           _dragBeg.Y = 0;
				else if (_dragBeg.Y >= _file.Rows) _dragBeg.Y = _file.Rows - 1;

				if      (_dragBeg.X < 0)           _dragBeg.X = 0;
				else if (_dragBeg.X >= _file.Cols) _dragBeg.X = _file.Cols - 1;
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

				if      (_dragEnd.Y < 0)           _dragEnd.Y = 0;
				else if (_dragEnd.Y >= _file.Rows) _dragEnd.Y = _file.Rows - 1;

				if      (_dragEnd.X < 0)           _dragEnd.X = 0;
				else if (_dragEnd.X >= _file.Cols) _dragEnd.X = _file.Cols - 1;
			}
		}
		#endregion Mouse & drag-points


		#region Events
		/// <summary>
		/// Fires when a location is selected in MainView.
		/// </summary>
		/// <param name="args"></param>
		internal void OnLocationSelectedMain(LocationSelectedArgs args)
		{
			//Logfile.Log("MainViewOverlay.OnLocationSelectedMain");

			FirstClick = true;
			MainViewF.that.sb_PrintPosition();
		}

		/// <summary>
		/// Fires when the Maplevel changes in MainView.
		/// </summary>
		/// <param name="args"></param>
		internal void OnLevelSelectedMain(LevelSelectedArgs args)
		{
			//Logfile.Log("MainViewOverlay.OnLevelSelectedMain");

			MainViewF.that.sb_PrintPosition();
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
			//Logfile.Log("");
			//Logfile.Log("OnPaint()");
			//Logfile.Log(Environment.StackTrace);
//			base.OnPaint(e);

			if (MainViewF.Dontdrawyougits) return;


			if (_file != null)
			{
				_targeterSuppressed = !_targeterForced && (!Focused || CursorOutsideClient());

				CuboidSprite.SetGraphics(_graphics = e.Graphics);
				_graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
#if !LOCKBITS
				if (!MainViewF.Optionables.UseMono)
				{
					_graphics.InterpolationMode = MainViewF.Optionables.InterpolationE;
				}
#endif

				// Image Processing using C# - https://www.codeproject.com/Articles/33838/Image-Processing-using-C
				// ColorMatrix Guide - https://docs.rainmeter.net/tips/colormatrix-guide/


				_cols = _file.Cols;
				_rows = _file.Rows;

				_phase = MainViewUnderlay.Phase;

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
				using (_b = new Bitmap(Width, Height, PixelFormat.Format32bppArgb))
				{
					BuildPanelImage();
//					_graphics.DrawImage(_b, 0,0, _b.Width, _b.Height);
					_graphics.DrawImageUnscaled(_b, Point.Empty);	// uh does not draw the image unscaled. it
				}													// still uses the DPI in the Graphics object ...
#endif
				if (Focused // else the loc-string can stick when the cursor leaves this panel
					&& Globals.Scale > 0.55 // else the text starts to hit the sprites
					&& _col > -1 && _col < _cols
					&& _row > -1 && _row < _rows)
				{
					PrintSelectorLocation();
				}

				ControlPaint.DrawBorder3D(_graphics, ClientRectangle, Border3DStyle.Etched);
			}
		}


		/// <summary>
		/// Prints the selector's current tile location.
		/// </summary>
		private void PrintSelectorLocation()
		{
			string loc = Globals.GetLocationString(
												_col,
												_row,
												_file.Level,
												_file.Levs);

			int x = Width - TextRenderer.MeasureText(loc, LocationFont).Width;
			int y = Height - 20;
			_graphics.DrawString(
							loc,
							LocationFont,
							LocationBrush,
							x,y);
		}

#if !LOCKBITS
		/// <summary>
		/// Draws the panel using the standard algorithm.
		/// </summary>
		/// <remarks>This is nearly identical to DrawPicasso; they are separated
		/// only because they'd cause multiple calls to DrawTile() conditioned
		/// on the setting of 'UseMono' inside the lev/row/col loops.</remarks>
		private void DrawRembrandt()
		{
			var rect = new Rectangle(-1,-1, 0,0); // This is different between REMBRANDT and PICASSO ->
			if (FirstClick)
			{
				Point a = GetDragBeg_abs();
				Point b = GetDragEnd_abs();

				rect.X = a.X;
				rect.Y = a.Y;
				rect.Width  = b.X - a.X + 1;
				rect.Height = b.Y - a.Y + 1;
			}


			MapTile tile;
			bool cuboid;

			bool isLevel;

			int heightfactor = HalfHeight * 3;
			int offsetVert;

			for (int
				lev  = _file.Levs - 1;
				lev >= _file.Level;
				--lev)
			{
				isLevel = (lev == _file.Level);

				if (isLevel && MainViewF.Optionables.GridVisible)
					DrawGrid();

				offsetVert = lev * heightfactor;

				for (int
						row = 0,
							startX = Origin.X,
							startY = Origin.Y + offsetVert;
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
						var rect0 = new Rectangle(
												x,y,
												_halfwidth2, _halfheight5);

						if (cuboid = (col == DragBeg.X && row == DragBeg.Y))
						{
							CuboidSprite.DrawCuboid_Rembrandt(
//														x,y,
//														HalfWidth,
//														HalfHeight,
														false,
														isLevel,
														rect0);
						}

						if (!(tile = _file.GetTile(col, row, lev)).Vacant
							&& (isLevel || !tile.Occulted))
						{
							// This is different between REMBRANDT and PICASSO ->
							DrawTile(
									tile,
									x,y,
									isLevel
										&& MainViewF.Optionables.SelectedTileToner != 0
										&& rect.Contains(col, row));
						}

						if (isLevel && !_targeterSuppressed
							&& col == _col && row == _row)
						{
							CuboidSprite.DrawTargeter_Rembrandt(
//														x,y,
//														HalfWidth,
//														HalfHeight,
														rect0);
						}

						if (cuboid)
						{
							CuboidSprite.DrawCuboid_Rembrandt(
//														x,y,
//														HalfWidth,
//														HalfHeight,
														true,
														isLevel,
														rect0);
						}
					}
				}
			}

			if ((FirstClick && MainViewF.Optionables.OneTileDraw) // This is different between REMBRANDT and PICASSO ->
				|| rect.Width > 1 || rect.Height > 1)
			{
				DrawSelectionBorder(rect);
			}

/*			if (!_targeterSuppressed // draw Targeter after selection-border ->
				&& _col > -1 && _col < _file.MapSize.Cols
				&& _row > -1 && _row < _file.MapSize.Rows)
			{
				CuboidSprite.DrawTargeter_Rembrandt(
											_graphics,
											_col * HalfWidth  + Origin.X - (_row * HalfWidth),
											_col * HalfHeight + Origin.Y + (_row * HalfHeight) + (_file.Level * heightfactor),
											HalfWidth,
											HalfHeight);
			} */
		}

		/// <summary>
		/// Draws the panel using the Mono algorithm.
		/// </summary>
		/// <remarks>This is nearly identical to DrawRembrandt; they are separated
		/// only because they'd cause multiple calls to DrawTile() conditioned
		/// on the setting of 'UseMono' inside the lev/row/col loops.</remarks>
		private void DrawPicasso()
		{
			MapTile tile;
			bool isLocCuboid;

			bool isLevel;

			int heightfactor = HalfHeight * 3;
			int offsetVert;

			for (int
				lev  = _file.Levs - 1;
				lev >= _file.Level;
				--lev)
			{
				isLevel = (lev == _file.Level);

				if (isLevel && MainViewF.Optionables.GridVisible)
					DrawGrid();

				offsetVert = (lev * heightfactor);

				for (int
						row = 0,
							startX = Origin.X,
							startY = Origin.Y + offsetVert;
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
						if (isLocCuboid = (col == DragBeg.X && row == DragBeg.Y))
						{
							CuboidSprite.DrawCuboid_Picasso(
														x,y,
														false,
														isLevel);
						}

						if (!(tile = _file.GetTile(col, row, lev)).Vacant
							&& (isLevel || !tile.Occulted))
						{
							// This is different between REMBRANDT and PICASSO ->
							DrawTile(tile, x,y);
						}

						if (isLevel && !_targeterSuppressed
							&& col == _col
							&& row == _row)
						{
							CuboidSprite.DrawTargeter_Picasso(
														x,y);
						}

						if (isLocCuboid)
						{
							CuboidSprite.DrawCuboid_Picasso(
														x,y,
														true,
														isLevel);
						}
					}
				}
			}

			if (FirstClick) // This is different between REMBRANDT and PICASSO ->
			{
				Point a = GetDragBeg_abs();
				Point b = GetDragEnd_abs();

				int width  = b.X - a.X + 1;
				int height = b.Y - a.Y + 1;

				if (MainViewF.Optionables.OneTileDraw
					|| width > 1 || height > 1)
				{
					var rect = new Rectangle(
										a.X, a.Y,
										width, height);
					DrawSelectionBorder(rect);
				}
			}

/*			if (!_targeterSuppressed // draw Targeter after selection-border ->
				&& _col > -1 && _col < _file.MapSize.Cols
				&& _row > -1 && _row < _file.MapSize.Rows)
			{
				CuboidSprite.DrawTargeter_Picasso(
											_graphics,
											_col * HalfWidth  + Origin.X - (_row * HalfWidth),
											_col * HalfHeight + Origin.Y + (_row * HalfHeight) + (_file.Level * heightfactor));
			} */
		}

#else
		BitmapData _data; IntPtr _scan0;
		private void BuildPanelImage()
		{
			Graphics graphics = Graphics.FromImage(_b);
			graphics.Clear(Color.Transparent);

			_data = _b.LockBits(
							new Rectangle(0,0, _b.Width, _b.Height),
							ImageLockMode.WriteOnly,
							PixelFormat.Format32bppArgb);
			_scan0 = _data.Scan0;


			MapTile tile;

//			bool isTargeted = Focused
//						   && !_suppressTargeter
//						   && ClientRectangle.Contains(PointToClient(Control.MousePosition));

			for (int
				lev = _file.Levs - 1;
				lev >= _file.Level && lev != -1;
				--lev)
			{
//				if (_showGrid && lev == _file.Level)
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
//											lev == _file.Level);
//						}

						tile = _file.GetTile(row, col, lev);
						if (lev == _file.Level || !tile.Occulted)
						{
							DrawTile(tile, x, y);
						}

//						if (isClicked)
//						{
//							Cuboid.DrawCuboid(
//											graphics,
//											x,y,
//											HalfWidth,
//											HalfHeight,
//											true,
//											lev == _file.Level);
//						}
//						else if (isTargeted
//							&& col == _col
//							&& row == _row
//							&& lev == _file.Level)
//						{
//							Cuboid.DrawTargeter(
//											graphics,
//											x,y,
//											HalfWidth,
//											HalfHeight);
//						}
					}
				}
			}
			_b.UnlockBits(_data);

			if (FirstClick)
			{
				Point beg = GetDragBeg_abs();
				Point end = GetDragEnd_abs();

				int width  = end.X - beg.X + 1;
				int height = end.Y - beg.Y + 1;

				if (    width > 2 || height > 2
					|| (width > 1 && height > 1))
				{
					var dragrect = new Rectangle(
											beg.X, beg.Y,
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
			int y = Origin.Y + HalfHeight * (_file.Level + 1) * 3;

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
			int y = Origin.Y + HalfHeight * (_file.Level + 1) * 3;

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
				if (i % 10 != 0) pen = PenGrid;
				else             pen = PenGrid10;

				graphics.DrawLine(
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

				graphics.DrawLine(
								pen,
								x + HalfWidth  * i,
								y + HalfHeight * i,
								x - x1 + HalfWidth  * i,
								y + y1 + HalfHeight * i);
			}
		}
#endif

		bool _disFloor;
		bool _disWest;
		bool _disNorth;
		bool _disContent;

		/// <summary>
		/// Sets the floor-disabled flag.
		/// </summary>
		/// <param name="disabled"><c>true</c> if quadrant is disabled</param>
		internal void SetFloorDisabled(bool disabled)
		{
			_disFloor = disabled;
		}

		/// <summary>
		/// Sets the westwall-disabled flag.
		/// </summary>
		/// <param name="disabled"><c>true</c> if quadrant is disabled</param>
		internal void SetWestDisabled(bool disabled)
		{
			_disWest = disabled;
		}

		/// <summary>
		/// Sets the northwall-disabled flag.
		/// </summary>
		/// <param name="disabled"><c>true</c> if quadrant is disabled</param>
		internal void SetNorthDisabled(bool disabled)
		{
			_disNorth = disabled;
		}

		/// <summary>
		/// Sets the content-disabled flag.
		/// </summary>
		/// <param name="disabled"><c>true</c> if quadrant is disabled</param>
		internal void SetContentDisabled(bool disabled)
		{
			_disContent = disabled;
		}

		/// <summary>
		/// Draws the <c><see cref="Tilepart">Tileparts</see></c> in a specified
		/// <c><see cref="MapTile"/></c> if
		/// <c><see cref="MainViewOptionables.UseMono">MainViewOptionables.UseMono</see></c>
		/// or <c>#LOCKBITS</c>.
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

			if (!_disFloor && (part = tile.Floor) != null)
			{
				DrawSprite(
						part[_phase].GetBindata(),
						x, y - part.Record.SpriteOffset * HalfHeight / HalfHeightConst);
			}

			if (!_disWest && (part = tile.West) != null)
			{
				DrawSprite(
						part[_phase].GetBindata(),
						x, y - part.Record.SpriteOffset * HalfHeight / HalfHeightConst);
			}

			if (!_disNorth && (part = tile.North) != null)
			{
				DrawSprite(
						part[_phase].GetBindata(),
						x, y - part.Record.SpriteOffset * HalfHeight / HalfHeightConst);
			}

			if (!_disContent && (part = tile.Content) != null)
			{
				DrawSprite(
						part[_phase].GetBindata(),
						x, y - part.Record.SpriteOffset * HalfHeight / HalfHeightConst);
			}
		}

#if !LOCKBITS
		/// <summary>
		/// Draws the <c><see cref="Tilepart">Tileparts</see></c> in a specified
		/// <c><see cref="MapTile"/></c> if not
		/// <c><see cref="MainViewOptionables.UseMono">MainViewOptionables.UseMono</see></c>.
		/// </summary>
		/// <param name="tile"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="toned"><c>true</c> to draw the monotone version of the
		/// tile-sprites</param>
		private void DrawTile(
				MapTile tile,
				int x, int y,
				bool toned)
		{
			Tilepart part;
			var rect = new Rectangle(
								 x,0,
								_halfwidth2, _halfheight5);

			if (toned)
			{
				if (!_disFloor && (part = tile.Floor) != null)
				{
					rect.Y = y - part.Record.SpriteOffset * HalfHeight / HalfHeightConst;
					DrawSprite((part[_phase] as PckSprite).SpriteToned, rect);
				}

				if (!_disWest && (part = tile.West) != null)
				{
					rect.Y = y - part.Record.SpriteOffset * HalfHeight / HalfHeightConst;
					DrawSprite((part[_phase] as PckSprite).SpriteToned, rect);
				}

				if (!_disNorth && (part = tile.North) != null)
				{
					rect.Y = y - part.Record.SpriteOffset * HalfHeight / HalfHeightConst;
					DrawSprite((part[_phase] as PckSprite).SpriteToned, rect);
				}

				if (!_disContent && (part = tile.Content) != null)
				{
					rect.Y = y - part.Record.SpriteOffset * HalfHeight / HalfHeightConst;
					DrawSprite((part[_phase] as PckSprite).SpriteToned, rect);
				}
			}
			else
			{
				if (!_disFloor && (part = tile.Floor) != null)
				{
					rect.Y = y - part.Record.SpriteOffset * HalfHeight / HalfHeightConst;
					DrawSprite(part[_phase].Sprite, rect);
				}

				if (!_disWest && (part = tile.West) != null)
				{
					rect.Y = y - part.Record.SpriteOffset * HalfHeight / HalfHeightConst;
					DrawSprite(part[_phase].Sprite, rect);
				}

				if (!_disNorth && (part = tile.North) != null)
				{
					rect.Y = y - part.Record.SpriteOffset * HalfHeight / HalfHeightConst;
					DrawSprite(part[_phase].Sprite, rect);
				}

				if (!_disContent && (part = tile.Content) != null)
				{
					rect.Y = y - part.Record.SpriteOffset * HalfHeight / HalfHeightConst;
					DrawSprite(part[_phase].Sprite, rect);
				}
			}
		}

		/// <summary>
		/// Draws a tilepart's sprite w/ <c>FillRectangle()</c>.
		/// </summary>
		/// <param name="bindata">binary data of XCImage (list of palette-ids)</param>
		/// <param name="x">x-pixel start</param>
		/// <param name="y">y-pixel start</param>
		private void DrawSprite(IList<byte> bindata, int x, int y)
		{
			int palid;

			int i = -1, w,h;
			for (h = 0; h != Spriteset.SpriteHeight40; ++h)
			for (w = 0; w != Spriteset.SpriteWidth32;  ++w)
			{
				palid = bindata[++i];
				if (palid != Palette.Tid) // <- this is the fix for Mono.
				{
					_graphics.FillRectangle(
										Palette.MonoBrushes[palid],
										x + (int)(w * Globals.Scale),
										y + (int)(h * Globals.Scale),
										_d, _d);
				}
			}
		}

		/// <summary>
		/// Draws a tilepart's sprite w/ <c>DrawImage()</c>.
		/// </summary>
		/// <param name="sprite"></param>
		/// <param name="rect">destination rectangle</param>
		private void DrawSprite(Image sprite, Rectangle rect)
		{
//			if (sprite != null) {
			_graphics.DrawImage(
							sprite,
							rect,
							0,0, Spriteset.SpriteWidth32, Spriteset.SpriteHeight40,
							GraphicsUnit.Pixel,
							Globals.Ia);
//			}
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
//								new Rectangle(0,0, _b.Width, _b.Height),
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
				for (y = 0; y != Spriteset.SpriteHeight40; ++y)
				for (x = 0; x != Spriteset.SpriteWidth32;  ++x)
				{
					palid = bindata[++i];

					if (palid != Palette.Tid)
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
		/// Draws a colored lozenge around selected Tiles.
		/// </summary>
		/// <param name="dragrect"></param>
		private void DrawSelectionBorder(Rectangle dragrect)
		{
			Point t = GetClientCoordinates(new Point(dragrect.Left,  dragrect.Top));
			Point r = GetClientCoordinates(new Point(dragrect.Right, dragrect.Top));
			Point b = GetClientCoordinates(new Point(dragrect.Right, dragrect.Bottom));
			Point l = GetClientCoordinates(new Point(dragrect.Left,  dragrect.Bottom));

			t.X += HalfWidth;
			r.X += HalfWidth;
			b.X += HalfWidth;
			l.X += HalfWidth;

			if (MainViewF.Optionables.LayerSelectionBorder < 2) // draw at grid level ->
			{
				_graphics.DrawLine(PenSelect, t,r);
				_graphics.DrawLine(PenSelect, r,b);
				_graphics.DrawLine(PenSelect, b,l);
				_graphics.DrawLine(PenSelect, l,t);
			}

			if (MainViewF.Optionables.LayerSelectionBorder > 0) // draw at level above ->
			{
				int offsetVert = HalfHeight * 3;

				t.Y -= offsetVert;
				r.Y -= offsetVert;
				b.Y -= offsetVert;
				l.Y -= offsetVert;

				_graphics.DrawLine(PenSelect, t,r);
				_graphics.DrawLine(PenSelect, r,b);
				_graphics.DrawLine(PenSelect, b,l);
				_graphics.DrawLine(PenSelect, l,t);
			}
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

			graphics.DrawLine(PenSelect, t, r);
			graphics.DrawLine(PenSelect, r, b);
			graphics.DrawLine(PenSelect, b, l);
			graphics.DrawLine(PenSelect, l, t);

			// TODO: Respect MainViewF.Optionables.LayerSelectionBorder
			// see !LOCKBITS DrawSelectionBorder()
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
			int verticalOffset = HalfHeight * (_file.Level + 1) * 3;
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

			double verticalOffset = (_file.Level + 1) * 3;

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
