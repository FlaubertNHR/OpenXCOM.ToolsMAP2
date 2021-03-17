using System;
using System.Drawing;
using System.Timers;
using System.Windows.Forms;

using MapView.Forms.MainView;

using XCom;


namespace MapView.Forms.Observers
{
	/// <summary>
	/// The bottom region of <see cref="TopView"/>.
	/// </summary>
	/// <remarks>These are not actually "quadrants"; they are tile-part types.
	/// But that's the way this trolls.</remarks>
	internal sealed class QuadrantControl
		:
			ObserverControl_Top // DoubleBufferedControl, IObserver
	{
		#region Fields (static)
		/// <summary>
		/// A timer that delays processing clicks until the user's double-click
		/// duration has elapsed. That is, don't do 1-click RMB processing if
		/// 2-clicks are inc.
		/// w/ Thanks to Natxo
		/// https://stackoverflow.com/questions/2086213/how-can-i-catch-both-single-click-and-double-click-events-on-wpf-frameworkelement/2087517#2087517
		/// </summary>
		private static readonly System.Timers.Timer _t1 = new System.Timers.Timer(SystemInformation.DoubleClickTime);

		private static int _t1Clicks;
		private static bool _t1subscribed;
		#endregion Fields (static)


		// TODO: Figure out what else can be made static here


		#region Fields
		/// <summary>
		/// For use by keyboard-input.
		/// </summary>
		private PartType _keyslot = PartType.Invalid;
		#endregion Fields


		#region Properties
		private PartType _slot;
		internal PartType SelectedQuadrant
		{
			get { return _slot; }
			set { _slot = value; Refresh(); }
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
		/// <remarks>There are 2 controls - one in TopView and another in
		/// TopRouteView(Top).</remarks>
		internal QuadrantControl()
		{
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
		/// Inherited from IObserver through ObserverControl_Top.
		/// </summary>
		/// <param name="args"></param>
		public override void OnLocationSelectedObserver(LocationSelectedEventArgs args)
		{
			Tile             = args.Tile;
			SelectedLocation = args.Location;

			Refresh();
		}

		/// <summary>
		/// Inherited from IObserver through ObserverControl_Top.
		/// </summary>
		/// <param name="args"></param>
		public override void OnLevelSelectedObserver(LevelSelectedEventArgs args)
		{
			if (SelectedLocation != null)
			{
				Tile = MapFile[SelectedLocation.Col, SelectedLocation.Row];
				SelectedLocation.Lev = args.Level;
			}
			Refresh();
		}


		/// <summary>
		/// Wrapper for OnMouseDown() for use by keyboard-input only.
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
		///  Handles mousedown events on this QuadrantControl.
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
				if (x > -1 && x % QuadrantDrawService.Quadwidth < XCImage.SpriteWidth32) // ignore spaces between sprites
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
					if (QuadrantDrawService.CurrentTilepart != null)
						part = QuadrantDrawService.CurrentTilepart.Record.PartType;
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
		/// Handles the details of LMB and RMB wrt the QuadrantControls.
		/// TODO: GENERAL - Bypass operations (and the MapChanged flag)
		///       if user does an operation that results in identical state.
		/// </summary>
		/// <param name="button"></param>
		/// <param name="clicks"></param>
		/// <param name="isKeyInput">true if invoked by keyboard-input</param>
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
								_t1Clicks = clicks;
								OnClicksElapsed(null,null);
							}
							else
							{
								_t1.Stop();
								++_t1Clicks;
								_t1.Start();
							}
						}
						break;
				}
			}
		}


		/// <summary>
		/// Overrides DoubleBufferedControl.RenderGraphics() - ie, OnPaint().
		/// @note Calls the draw-function in QuadrantDrawService.
		/// </summary>
		/// <param name="graphics"></param>
		protected override void RenderGraphics(Graphics graphics)
		{
			QuadrantDrawService.SetGraphics(graphics);
			QuadrantDrawService.Draw(Tile, SelectedQuadrant);

			if (SelectedLocation != null)
				QuadrantDrawService.PrintSelectedLocation(SelectedLocation, Width);
		}
		#endregion Events (override)


		#region Events
		/// <summary>
		/// Clever handling of RMB double-click event ...
		/// WARNING: The interaction between this QuadrantControl, its respective
		/// TopControl, and the TilePanel in TileView is a little bit fragile.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		private void OnClicksElapsed(object source, ElapsedEventArgs e)
		{
			//DSShared.LogFile.WriteLine("QuadrantControl.OnClicksElapsed() _t1Clicks= " + _t1Clicks);

			_t1.Stop();

			switch (_t1Clicks)
			{
				case 1:
					MainViewOverlay.that.FillSelectedQuads();
					break;

				case 2:
					MainViewOverlay.that.ClearSelectedQuads();
					break;
			}
			_t1Clicks = 0;
		}

		/// <summary>
		/// Invalidates this QuadrantControl if tileparts are being animated.
		/// </summary>
		private void OnPhaseEvent()
		{
			Invalidate();
		}
		#endregion Events
	}
}
