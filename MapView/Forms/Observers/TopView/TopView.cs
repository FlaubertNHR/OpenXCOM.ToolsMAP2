using System;
using System.Collections.Generic;
using System.Windows.Forms;

using DSShared;

using MapView.Forms.MainView;

using XCom;


namespace MapView.Forms.Observers
{
	internal sealed partial class TopView
		:
			UserControl
	{
		#region Fields (static)
		/// <summary>
		/// The TestPartslots dialog - a nonmodal <see cref="Infobox"/>.
		/// </summary>
		/// <remarks>Be careful with this pointer because closing the dialog in
		/// the dialog itself does *not* null this pointer. So check for both
		/// !null and !IsDisposed if necessary.</remarks>
		internal static Infobox _fpartslots;
		#endregion Fields (static)


		#region Fields
		private MapFile _file;
		#endregion Fields


		#region Properties (static)
		internal static Options Options = new Options();

		/// <summary>
		/// A class-object that holds TopView's optionable Properties.
		/// @note C# doesn't allow inheritance of multiple class-objects, which
		/// would have been a way to separate the optionable properties from all
		/// the other properties that are not optionable; they need to be
		/// separate or else all Properties would show up in the Options form's
		/// PropertyGrid. An alternative would have been to give all those other
		/// properties the Browsable(false) attribute but I didn't want to
		/// clutter up the code and also because the Browsable(false) attribute
		/// is used to hide Properties from the designer also - but whether or
		/// not they are accessible in the designer is an entirely different
		/// consideration than whether or not they are Optionable Properties. So
		/// I created an independent class just to hold and handle TopView's
		/// Optionable Properties ... and wired it up. It's a tedious shitfest
		/// but better than the arcane MapViewI system or screwing around with
		/// equally arcane TypeDescriptors. Both of which had been implemented
		/// but then rejected.
		/// </summary>
		internal static TopViewOptionables Optionables = new TopViewOptionables();
		#endregion Properties (static)


		#region Properties
		internal TopControl TopControl
		{ get; private set; }

		internal QuadrantControl QuadrantControl
		{ get; private set; }


		internal ToolStripMenuItem it_Floor
		{ get; private set; }

		internal ToolStripMenuItem it_North
		{ get; private set; }

		internal ToolStripMenuItem it_West
		{ get; private set; }

		internal ToolStripMenuItem it_Content
		{ get; private set; }
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor. Instantiates the TopView viewer and its components/controls.
		/// </summary>
		/// <remarks><see cref="RouteViewForm"/> and <see cref="TopRouteViewForm"/>
		/// will each invoke and maintain their own instantiations.</remarks>
		internal TopView()
		{
			InitializeComponent();

			SuspendLayout();

			// Mono prefers the toolstrip added here in the cTor instead of the
			// designer ... uh, apparently
			tscPanel.LeftToolStripPanel.Controls.Add(tsTools);

			QuadrantControl = new QuadrantControl();
			Controls.Add(QuadrantControl);

			TopControl = new TopControl(this);
			TopControl.Dock = DockStyle.Fill;

			pnlMain.Controls.Add(TopControl);
			pnlMain.Resize += (sender, e) => TopControl.ResizeObserver(
																	pnlMain.Width,
																	pnlMain.Height);

			it_Floor   = new ToolStripMenuItem(QuadrantDrawService.Floor,   null, OnQuadrantDisabilityClick, Keys.F1);
			it_West    = new ToolStripMenuItem(QuadrantDrawService.West,    null, OnQuadrantDisabilityClick, Keys.F2);
			it_North   = new ToolStripMenuItem(QuadrantDrawService.North,   null, OnQuadrantDisabilityClick, Keys.F3);
			it_Content = new ToolStripMenuItem(QuadrantDrawService.Content, null, OnQuadrantDisabilityClick, Keys.F4);

			tsddbDisabledQuads.DropDown.Items.AddRange(new []
			{
				it_Floor,
				it_West,
				it_North,
				it_Content
			});

			ResumeLayout();
		}
		#endregion cTor


		#region Events (override)
		/// <summary>
		/// Scrolls the z-axis.
		/// </summary>
		/// <param name="e"></param>
		/// <remarks>Duplicated in <c><see cref="RouteView"/></c>.</remarks>
		protected override void OnMouseWheel(MouseEventArgs e)
		{
			base.OnMouseWheel(e);

			int delta;
			if (MainViewF.Optionables.InvertMousewheel)
				delta = -e.Delta;
			else
				delta =  e.Delta;

			int dir = MapFile.LEVEL_no;
			if      (delta < 0) dir = MapFile.LEVEL_Up;
			else if (delta > 0) dir = MapFile.LEVEL_Dn;
			_file.ChangeLevel(dir);

			ObserverManager.ToolFactory.EnableLevelers(_file.Level, _file.Levs);
		}
		#endregion Events (override)


		#region Events
		/// <summary>
		/// Handler for <c><see cref="MapFile"/>.LevelSelected</c>.
		/// </summary>
		/// <param name="args"></param>
		private void OnLevelSelectedObserver(LevelSelectedArgs args)
		{
			//Logfile.Log("TopView.OnLevelSelectedObserver() " + Tag);
			Refresh(); // req'd.
		}


		/// <summary>
		/// Handles a click on any of the quadrant-visibility menuitems.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks>Mapview2 uses quadrant-visibility in 2 ways.
		/// <list type="bullet">
		/// <item>by setting/checking the <c>Checked</c> state of the respective
		/// it</item>
		/// <item>by setting/checking a respective <c>bool</c> in
		/// <c><see cref="MainViewOverlay"/></c></item>
		/// </list></remarks>
		internal void OnQuadrantDisabilityClick(object sender, EventArgs e)
		{
			var it = sender as ToolStripMenuItem;
			if (it == it_Floor)
			{
				ObserverManager.TopView     .Control   .it_Floor.Checked =
				ObserverManager.TopRouteView.ControlTop.it_Floor.Checked = !it.Checked;

				MainViewOverlay.that.SetFloorDisabled(it.Checked);
				_file.CalculateOccultations(it.Checked);
			}
			else if (it == it_West)
			{
				ObserverManager.TopView     .Control   .it_West.Checked =
				ObserverManager.TopRouteView.ControlTop.it_West.Checked = !it.Checked;

				MainViewOverlay.that.SetWestDisabled(it.Checked);
			}
			else if (it == it_North)
			{
				ObserverManager.TopView     .Control   .it_North.Checked =
				ObserverManager.TopRouteView.ControlTop.it_North.Checked = !it.Checked;

				MainViewOverlay.that.SetNorthDisabled(it.Checked);
			}
			else // it_Content
			{
				ObserverManager.TopView     .Control   .it_Content.Checked =
				ObserverManager.TopRouteView.ControlTop.it_Content.Checked = !it.Checked;

				MainViewOverlay.that.SetContentDisabled(it.Checked);
			}

			MainViewOverlay.that.Invalidate();

			ObserverManager.InvalidateTopControls();
			ObserverManager.InvalidateQuadrantControls();
		}


		/// <summary>
		/// Handles a click on the TestPartslots menuitem.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnTestPartslotsClick(object sender, EventArgs e)
		{
			if (_fpartslots != null && !_fpartslots.IsDisposed)
			{
				_fpartslots.Close(); // TODO: Update the data if/when parts change.
				_fpartslots = null;
			}

			var lines = new List<string>();

			MapTile tile;
			Tilepart part;
			McdRecord record;

			for (int l = 0; l != _file.Levs; ++l)
			for (int r = 0; r != _file.Rows; ++r)
			for (int c = 0; c != _file.Cols; ++c)
			{
				tile = _file.GetTile(c,r,l);
				if (!tile.Vacant)
				{
					if ((part = tile.Floor) != null
						&& (record = part.Record).PartType != PartType.Floor)
					{
						lines.Add(GetParttestString(
												c,r,l,
												PartType.Floor,
												record.PartType,
												part.SetId));
					}

					if ((part = tile.West) != null
						&& (record = part.Record).PartType != PartType.West)
					{
						lines.Add(GetParttestString(
												c,r,l,
												PartType.West,
												record.PartType,
												part.SetId));
					}

					if ((part = tile.North) != null
						&& (record = part.Record).PartType != PartType.North)
					{
						lines.Add(GetParttestString(
												c,r,l,
												PartType.North,
												record.PartType,
												part.SetId));
					}

					if ((part = tile.Content) != null
						&& (record = part.Record).PartType != PartType.Content)
					{
						lines.Add(GetParttestString(
												c,r,l,
												PartType.Content,
												record.PartType,
												part.SetId));
					}
				}
			}


			const string title = "Partslots test";

			if (lines.Count != 0)
			{
				string copyable = "  c   r   L - slot     record   id" + Environment.NewLine;
				foreach (var line in lines)
					copyable += Environment.NewLine + line;

				_fpartslots = new Infobox( // not Modal.
									title,
									Infobox.SplitString("The following tileslots are occupied by incorrect"
											+ " PartTypes. This could result in broken battlescape behavior."),
									copyable,
									InfoboxType.Warn);
				_fpartslots.Show();
			}
			else
			{
				using (var f = new Infobox( // is Modal.
										title,
										"All assigned parts are in their correct slots."))
				{
					f.ShowDialog();
				}
			}
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
		/// handlers and this <c>TopView</c> from any further obligations.</remarks>
		internal void SetMapfile(MapFile file)
		{
			if (_file != null)
				_file.LevelSelected -= OnLevelSelectedObserver;

			if ((_file = file) != null)
				_file.LevelSelected += OnLevelSelectedObserver;
		}

		/// <summary>
		/// Adds the tool-objects in the toolstrip.
		/// </summary>
		internal void AddToolstripControls()
		{
			ObserverManager.ToolFactory.AddEditorTools(tsTools, true);
		}

		/// <summary>
		/// Formats a string of x/y/z + parttype for the TestPartslots dialog.
		/// </summary>
		/// <param name="col"></param>
		/// <param name="row"></param>
		/// <param name="lev"></param>
		/// <param name="quadrant"></param>
		/// <param name="parttype"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		private string GetParttestString(
				int col,
				int row,
				int lev,
				PartType quadrant,
				PartType parttype,
				int id)
		{
			lev = _file.Levs - lev; // invert.

			if (MainViewF.Optionables.Base1_xy) { ++col; ++row; }
			if (!MainViewF.Optionables.Base1_z) { --lev; }

			string c = col.ToString().PadLeft(3);
			string r = row.ToString().PadLeft(3);
			string l = lev.ToString().PadLeft(3);

			string q = Enum.GetName(typeof(PartType), quadrant).PadRight(9);
			string p = Enum.GetName(typeof(PartType), parttype).PadRight(9);

			return c + " " + r + " " + l + " - " + q + p + id;
		}
		#endregion Methods


		#region Options
		internal static OptionsF _foptions;	// is static so that it will be used by
											// both TopView and TopRouteView(Top)
		/// <summary>
		/// Handles a click on the Options button to show or hide an
		/// <c><see cref="OptionsF"/></c>. Instantiates an <c>OptionsF</c> if
		/// one doesn't exist for this viewer. Also subscribes to a
		/// <c>FormClosing</c> handler that will hide the <c>Form</c> unless
		/// MapView is quitting.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void OnOptionsClick(object sender, EventArgs e)
		{
			var tsb = sender as ToolStripButton;
			if (tsb.Checked && _foptions.WindowState == FormWindowState.Minimized)
			{
				_foptions.WindowState = FormWindowState.Normal;
			}
			else if (!tsb.Checked)
			{
				setOptionsChecked(true);

				if (_foptions == null)
				{
					_foptions = new OptionsF(
										Optionables,
										Options,
										OptionableType.TopView);
					_foptions.Text = "TopView Options";

//					if (MainViewF.Optionables.OptionsOnTop)
//						_foptions.Owner = ObserverManager.TopView;

					OptionsManager.Options.Add(_foptions);

					_foptions.FormClosing += (sender1, e1) =>
					{
						if (!MainViewF.Quit)
						{
							setOptionsChecked(false);

							e1.Cancel = true;
							_foptions.Hide();
						}
						else
							RegistryInfo.UpdateRegistry(_foptions);
					};
				}

				_foptions.Show();

				if (_foptions.WindowState == FormWindowState.Minimized)
					_foptions.WindowState  = FormWindowState.Normal;
			}
			else
				_foptions.Close();
		}

		/// <summary>
		/// Checks or unchecks the Options button.
		/// </summary>
		/// <param name="checked"></param>
		private static void setOptionsChecked(bool @checked)
		{
			ObserverManager.TopView     .Control   .tsb_Options.Checked =
			ObserverManager.TopRouteView.ControlTop.tsb_Options.Checked = @checked;
		}

		/// <summary>
		/// Gets the Options button on the toolstrip.
		/// </summary>
		/// <returns>either the button in TopView or TopRouteView(Top)
		/// - doesn't matter as long as they are kept synched</returns>
		internal ToolStripButton GetOptionsButton()
		{
			return tsb_Options;
		}
		#endregion Options
	}
}
