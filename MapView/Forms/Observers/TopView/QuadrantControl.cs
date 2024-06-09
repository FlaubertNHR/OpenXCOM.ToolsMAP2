using System;
using System.Timers;
using System.Windows.Forms;

using DSShared.Controls;

using MapView.Forms.MainView;

using XCom;


namespace MapView.Forms.Observers
{
	/// <summary>
	/// The bottom region of <c><see cref="TopView"/></c>.
	/// </summary>
	/// <remarks>These are not actually "quadrants"; they are tilepart types.
	/// But that's the way this trolls.</remarks>
	internal sealed class QuadrantControl
		:
			DoubleBufferedControl
	{
		public static void DisposeControl()
		{
			//DSShared.Logfile.Log("QuadrantControl.DisposeControl() static");
			_t1.Dispose();
		}


		#region Fields (static)
		/// <summary>
		/// A timer that delays processing clicks until the user's double-click
		/// duration has elapsed. That is, don't do 1-click RMB processing if
		/// 2-clicks are inc.
		/// </summary>
		/// <remarks>w/ Thanks to Natxo<br/>
		/// https://stackoverflow.com/questions/2086213/how-can-i-catch-both-single-click-and-double-click-events-on-wpf-frameworkelement/2087517#2087517</remarks>
		private static readonly System.Timers.Timer _t1 = new System.Timers.Timer(SystemInformation.DoubleClickTime);

		private static int  _t1clicks;
		private static bool _t1subscribed;

		/// <summary>
		/// This is set to a quadrant-slot by keyboard-input.
		/// </summary>
		private static PartType _quadrant = PartType.Invalid;
		#endregion Fields (static)


		#region Fields
		// TODO: Figure out what else can be made static here

		private MapFile _file;
		#endregion Fields


		#region Properties (static)
		private static PartType _selectedquadrant = PartType.Floor;
		internal static PartType SelectedQuadrant
		{
			get { return _selectedquadrant; }
			set
			{
				_selectedquadrant = value;
				ObserverManager.RefreshQuadrantControls();
			}
		}
		#endregion Properties (static)


		#region Properties
		// TODO: Figure out what else can be made static here

		internal MapTile Tile
		{ private get; set; }

		internal MapLocation SelectedLocation
		{ private get; set; }
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <remarks>There are 2 QuadrantControls - one in TopView and another
		/// in TopRouteView(Top).</remarks>
		internal QuadrantControl()
		{
			Name   = "QuadrantControl";
			Height = 70;
			Dock   = DockStyle.Bottom;

			SetStyle(ControlStyles.SupportsTransparentBackColor, true);

			MainViewUnderlay.PhaseEvent += OnPhaseEvent;

			if (!_t1subscribed) // only once (for both QuadrantControls)
			{
				_t1subscribed = true;
				_t1.Elapsed += OnClicksElapsed;
			}
		}
		#endregion cTor


		#region Events (override)
		/// <summary>
		/// Wrapper for <c><see cref="OnMouseDown()">OnMouseDown()</see></c> for
		/// use by keyboard-input only.
		/// </summary>
		/// <param name="e"></param>
		/// <param name="quadrant"></param>
		internal void doMouseDown(MouseEventArgs e, PartType quadrant)
		{
			//DSShared.Logfile.Log("QuadrantControl.doMouseDown() quadrant= " + quadrant);

			if (quadrant != PartType.Invalid) _quadrant = quadrant;
			else                              _quadrant = SelectedQuadrant;

			OnMouseDown(e);
		}

		/// <summary>
		/// Handles <c>MouseDown</c> events on this <c>QuadrantControl</c>.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			//DSShared.Logfile.Log("QuadrantControl.OnMouseDown()");
			//DSShared.Logfile.Log(". _quadrant= " + _quadrant);

			ObserverManager.TopView     .Control   .TopControl.Select();
			ObserverManager.TopRouteView.ControlTop.TopControl.Select();

			bool askey = isQuadrantLegit(true);
			//DSShared.Logfile.Log(". askey= " + askey);

			if (!askey)
			{
				int x = e.X - QuadrantDrawService.StartX;
				if (x > -1 && x % QuadrantDrawService.Quadwidth < Spriteset.SpriteWidth32) // ignore spaces between sprites
				{
					// WARNING: '_quadrant' could be set to a nonstandard 'PartType' here
					_quadrant = (PartType)(x / QuadrantDrawService.Quadwidth);
					//DSShared.Logfile.Log(". . _quadrant= " + _quadrant);
				}
			}

			if (isQuadrantLegit())
			{
				PartType quadrant;

				if (_quadrant != (PartType)QuadrantDrawService.Quad_PART)
					quadrant = _quadrant;
				else if (QuadrantDrawService.SelectedTilepart != null)
					quadrant = QuadrantDrawService.SelectedTilepart.Record.PartType;
				else
					quadrant = PartType.Invalid;

				//DSShared.Logfile.Log(". quadrant= " + quadrant);
				if (quadrant != PartType.Invalid)
				{
					SelectedQuadrant = quadrant;

					if (_quadrant != (PartType)QuadrantDrawService.Quad_PART)
						Clicker(e.Button, e.Clicks, askey);
				}
			}
			_quadrant = PartType.Invalid;
		}

		/// <summary>
		/// Handles the details of LMB and RMB wrt the <c>QuadrantControl</c>.
		/// </summary>
		/// <param name="button"></param>
		/// <param name="clicks"></param>
		/// <param name="askey"><c>true</c> if invoked by keyboard-input</param>
		/// <remarks>TODO: GENERAL - Bypass operations (and the MapChanged flag)
		/// if user does an operation that results in an identical state.</remarks>
		internal void Clicker(MouseButtons button, int clicks, bool askey = false)
		{
			//DSShared.Logfile.Log("QuadrantControl.Clicker() button= " + button + " clicks= " + clicks + " askey= " + askey);

			if (Tile != null)
			{
				//DSShared.Logfile.Log(". Tile VALID");

				switch (button)
				{
					case MouseButtons.Left: // NOTE: clicks=1 is handled by caller.
						if (clicks == 2)
							ObserverManager.TileView.Control.SelectedTilepart = Tile[SelectedQuadrant];
						break;

					case MouseButtons.Right:
						if (MainViewOverlay.that.FirstClick) // do not set a part in a quadrant unless a tile is selected.
						{
							if (askey || !TopView.Optionables.EnableRightClickWaitTimer)
							{
								_t1clicks = clicks;
								OnClicksElapsed(null,null);
							}
							else
							{
								_t1.Stop();
								++_t1clicks;
								_t1.Start();
							}
						}
						break;
				}
			}
			else if (button == MouseButtons.Left && clicks == 2) // no Tile selected - null the CurrentPart
			{
				//DSShared.Logfile.Log(". Tile NOT Valid");
				ObserverManager.TileView.Control.SelectedTilepart = null;
			}
		}


		/// <summary>
		/// Overrides the <c>Paint</c> event.
		/// </summary>
		/// <param name="e"></param>
		/// <remarks>Passes all paint-duties to
		/// <c><see cref="QuadrantDrawService"/></c> - because why not.</remarks>
		protected override void OnPaint(PaintEventArgs e)
		{
			if (MainViewF.Dontdrawyougits) return;

			QuadrantDrawService.SetGraphics(e.Graphics);
			QuadrantDrawService.Paint(Tile);

			if (SelectedLocation != null)
				QuadrantDrawService.PrintSelectedLocation(SelectedLocation, Width);
		}
		#endregion Events (override)


		#region Events
		/// <summary>
		/// Handler for <c><see cref="MapFile"/>.LocationSelected</c>.
		/// </summary>
		/// <param name="args"></param>
		private void OnLocationSelectedObserver(LocationSelectedArgs args)
		{
			Tile             = args.Tile;
			SelectedLocation = args.Location;

			Refresh();
		}

		/// <summary>
		/// Handler for <c><see cref="MapFile"/>.LevelSelected</c>.
		/// </summary>
		/// <param name="args"></param>
		private void OnLevelSelectedObserver(LevelSelectedArgs args)
		{
			if (SelectedLocation != null)
			{
				Tile = _file.GetTile(SelectedLocation.Col,
									 SelectedLocation.Row);
				SelectedLocation.Lev = args.Level;
			}
			Refresh();
		}


		/// <summary>
		/// Clever handling of RMB double-click event ...
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		/// <remarks>WARNING: The interaction between this <c>QuadrantControl</c>,
		/// its respective <c><see cref="TopControl"/></c>, and the
		/// <c><see cref="TilePanel"/></c> in <c><see cref="TileView"/></c> is a
		/// little bit fragile.</remarks>
		private void OnClicksElapsed(object source, ElapsedEventArgs e)
		{
			_t1.Stop();

			switch (_t1clicks)
			{
				case 1:
					MainViewOverlay.that.FillSelectedQuadrants();
					break;

				case 2:
					MainViewOverlay.that.ClearSelectedQuadrants();
					break;
			}
			_t1clicks = 0;
		}

		/// <summary>
		/// Invalidates this <c>QuadrantControl</c> if tileparts are being
		/// animated.
		/// </summary>
		private void OnPhaseEvent()
		{
			Invalidate();
		}
		#endregion Events


		#region Methods
		/// <summary>
		/// Sets <c><see cref="_file"/></c>.
		/// </summary>
		/// <param name="file">a <c><see cref="MapFile"/></c></param>
		/// <remarks>I don't believe it is necessary to unsubscribe the handlers
		/// here from events in the old <c>MapFile</c>. The old <c>MapFile</c>
		/// held the references and it goes poof, which ought release these
		/// handlers and this <c>QuadrantControl</c> from any further
		/// obligations.</remarks>
		internal void SetMapfile(MapFile file)
		{
			if (_file != null)
			{
				_file.LocationSelected -= OnLocationSelectedObserver;
				_file.LevelSelected    -= OnLevelSelectedObserver;
			}

			if ((_file = file) != null)
			{
				_file.LocationSelected += OnLocationSelectedObserver;
				_file.LevelSelected    += OnLevelSelectedObserver;
			}
		}
		#endregion Methods


		#region Methods (static)
		/// <summary>
		/// Coordinates the <c>BackColor</c> between TopView and
		/// TopRouteView(Top).
		/// </summary>
		internal static void SetBackcolorCoordinator()
		{
			ObserverManager.TopView     .Control   .QuadrantControl.BackColor =
			ObserverManager.TopRouteView.ControlTop.QuadrantControl.BackColor = TopView.Optionables.QuadrantBackcolor;
		}

		/// <summary>
		/// Checks if mouse or key op hits a legit quadrant-slot.
		/// </summary>
		/// <param name="excludePart"><c>true</c> to exclude the current-part
		/// pseudo-slot</param>
		/// <returns><c>true</c> if <c><see cref="_quadrant"/></c> is a legal
		/// <c><see cref="PartType"/></c></returns>
		/// <remarks>Helper for
		/// <c><see cref="OnMouseDown()">OnMouseDown().</see></c></remarks>
		private static bool isQuadrantLegit(bool excludePart = false)
		{
			return _quadrant == PartType.Floor
				|| _quadrant == PartType.West
				|| _quadrant == PartType.North
				|| _quadrant == PartType.Content
				|| (!excludePart && _quadrant == (PartType)QuadrantDrawService.Quad_PART);
		}
		#endregion Methods (static)
	}
}
