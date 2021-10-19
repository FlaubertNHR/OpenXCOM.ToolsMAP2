using System;
using System.Drawing;
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
		/// <remarks>w/ Thanks to Natxo
		/// https://stackoverflow.com/questions/2086213/how-can-i-catch-both-single-click-and-double-click-events-on-wpf-frameworkelement/2087517#2087517</remarks>
		private static readonly System.Timers.Timer _t1 = new System.Timers.Timer(SystemInformation.DoubleClickTime);

		private static int  _t1clicks;
		private static bool _t1subscribed;
		#endregion Fields (static)


		// TODO: Figure out what else can be made static here


		#region Fields
		private MapFile _file;

		/// <summary>
		/// For use by keyboard-input.
		/// </summary>
		private PartType _keyslot = PartType.Invalid;
		#endregion Fields


		#region Properties
		private PartType _slot = PartType.Floor;
		internal PartType SelectedQuadrant
		{
			get { return _slot; }
			set
			{
				_slot = value; Refresh();
			}
		}

		internal MapTile Tile
		{ private get; set; }

		internal MapLocation SelectedLocation
		{ private get; set; }
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor.
		/// TODO: This should be a static class.
		/// </summary>
		/// <remarks>There are 2 QuadrantControls - one in TopView and another
		/// in TopRouteView(Top).</remarks>
		internal QuadrantControl()
		{
			Name   = "QuadrantControl";
			Height = 70;
			Dock   = DockStyle.Bottom;

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
		/// <param name="slot"></param>
		internal void doMouseDown(MouseEventArgs e, PartType slot)
		{
			if (slot != PartType.Invalid)
				_keyslot = slot;
			else
				_keyslot = SelectedQuadrant;

			OnMouseDown(e);
		}

		/// <summary>
		/// Handles mousedown events on this <c>QuadrantControl</c>.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			ObserverManager.TopView     .Control   .TopControl.Select();
			ObserverManager.TopRouteView.ControlTop.TopControl.Select();

			bool isKeyInput = _keyslot !=  PartType.Invalid
						   && _keyslot != (PartType)QuadrantDrawService.QuadrantPart;

			if (!isKeyInput)
			{
				int x = (e.X - QuadrantDrawService.StartX);
				if (x > -1 && x % QuadrantDrawService.Quadwidth < Spriteset.SpriteWidth32) // ignore spaces between sprites
					_keyslot = (PartType)(x / QuadrantDrawService.Quadwidth);
			}

			bool isPartSlot = false;

			PartType part = PartType.Invalid;
			switch (_keyslot)
			{
				case PartType.Floor:   part = PartType.Floor;   break; // TODO: refactor
				case PartType.West:    part = PartType.West;    break;
				case PartType.North:   part = PartType.North;   break;
				case PartType.Content: part = PartType.Content; break;

				case (PartType)QuadrantDrawService.QuadrantPart:
					isPartSlot = true;
					if (QuadrantDrawService.SelectedTilepart != null)
						part = QuadrantDrawService.SelectedTilepart.Record.PartType;
					break;
			}

			if (part != PartType.Invalid)
			{
				ObserverManager.TopView     .Control   .QuadrantControl.SelectedQuadrant = part;
				ObserverManager.TopRouteView.ControlTop.QuadrantControl.SelectedQuadrant = part;

				if (!isPartSlot)
					Clicker(e.Button, e.Clicks, isKeyInput);
			}
			_keyslot = PartType.Invalid;
		}

		/// <summary>
		/// Handles the details of LMB and RMB wrt the <c>QuadrantControl</c>.
		/// </summary>
		/// <param name="button"></param>
		/// <param name="clicks"></param>
		/// <param name="isKeyInput">true if invoked by keyboard-input</param>
		/// <remarks>TODO: GENERAL - Bypass operations (and the MapChanged flag)
		/// if user does an operation that results in an identical state.</remarks>
		internal void Clicker(MouseButtons button, int clicks, bool isKeyInput = false)
		{
			if (Tile != null)
			{
				switch (button)
				{
					case MouseButtons.Left: // NOTE: clicks=1 is handled by caller.
						if (clicks == 2)
							ObserverManager.TileView.Control.SelectedTilepart = Tile[SelectedQuadrant];
						break;

					case MouseButtons.Right:
						if (MainViewOverlay.that.FirstClick) // do not set a part in a quad unless a tile is selected.
						{
							if (isKeyInput || !TopView.Optionables.EnableRightClickWaitTimer)
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
		}


		/// <summary>
		/// Overrides
		/// <c><see cref="DSShared.Controls.DoubleBufferedControl">DoubleBufferedControl.OnPaintControl()</see></c>
		/// - ie, <c><see cref="DSShared.Controls.DoubleBufferedControl">DoubleBufferedControl.OnPaint()</see></c>.
		/// </summary>
		/// <param name="graphics"></param>
		/// <remarks>Calls the draw-function in
		/// <c><see cref="QuadrantDrawService"/></c>.</remarks>
		protected override void OnPaintControl(Graphics graphics)
		{
			QuadrantDrawService.SetGraphics(graphics);
			QuadrantDrawService.Draw(Tile, SelectedQuadrant);

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
	}
}
