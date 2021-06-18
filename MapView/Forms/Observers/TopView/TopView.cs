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
			ObserverControl // UserControl, IObserver
	{
		/// <summary>
		/// Disposes <see cref="TopControl"/>.
		/// </summary>
		/// <remarks>Do NOT use <c>public void Dispose()</c> or else you'll have
		/// one Fuck of a time trying to trace usage. Use <c>public void Dispose()</c>
		/// only for the Designer code w/ <c>components</c>. Thank yourself for
		/// heeding this piece of ornery advice later.</remarks>
		internal void DisposeObserver()
		{
			LogFile.WriteLine("TopView.DisposeObserver()");
			TopControl.DisposeControl();
		}


		#region Fields (static)
		internal const int Vis_FLOOR   = 0x1; // flags for parttype visibility ->
		internal const int Vis_WEST    = 0x2;
		internal const int Vis_NORTH   = 0x4;
		internal const int Vis_CONTENT = 0x8;

		/// <summary>
		/// The TestPartslots dialog - a nonmodal <see cref="Infobox"/>.
		/// </summary>
		/// <remarks>Be careful with this pointer because closing the dialog in
		/// the dialog itself does *not* null this pointer. So check for both
		/// !null and !IsDisposed if necessary.</remarks>
		internal static Infobox _fpartslots;
		#endregion Fields (static)


		#region Properties (static)
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
		internal static TopViewOptionables Optionables
		{ get; set; }
		#endregion Properties (static)


		#region Properties
		internal TopControl TopControl
		{ get; private set; }

		internal QuadrantControl QuadrantControl
		{ get; private set; }


		internal ToolStripMenuItem Floor
		{ get; private set; }

		internal ToolStripMenuItem North
		{ get; private set; }

		internal ToolStripMenuItem West
		{ get; private set; }

		internal ToolStripMenuItem Content
		{ get; private set; }

		/// <summary>
		/// A bit-cache denoting which parttypes are currently visible or not.
		/// </summary>
		internal int VisibleQuadrants
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
			Optionables = new TopViewOptionables(this);

			InitializeComponent();

			SuspendLayout();

			// Mono prefers the toolstrip added here in the cTor instead of the
			// designer ... uh, apparently
			tscPanel.LeftToolStripPanel.Controls.Add(tsTools);

			CreateQuadrantPanel();

			TopControl = new TopControl(this);
			TopControl.Dock = DockStyle.Fill;

			pnlMain.Controls.Add(TopControl);
			pnlMain.Resize += (sender, e) => TopControl.ResizeObserver(
																	pnlMain.Width,
																	pnlMain.Height);

			Floor   = new ToolStripMenuItem(QuadrantDrawService.Floor,   null, OnQuadrantVisibilityClick, Keys.F1);
			West    = new ToolStripMenuItem(QuadrantDrawService.West,    null, OnQuadrantVisibilityClick, Keys.F2);
			North   = new ToolStripMenuItem(QuadrantDrawService.North,   null, OnQuadrantVisibilityClick, Keys.F3);
			Content = new ToolStripMenuItem(QuadrantDrawService.Content, null, OnQuadrantVisibilityClick, Keys.F4);

			ToolStripItemCollection visQuads = tsddbVisibleQuads.DropDown.Items;

			visQuads.Add(Floor);
			visQuads.Add(West);
			visQuads.Add(North);
			visQuads.Add(Content);

			Floor  .Checked =
			West   .Checked =
			North  .Checked =
			Content.Checked = true;

			VisibleQuadrants = Vis_FLOOR | Vis_WEST | Vis_NORTH | Vis_CONTENT;

			ObserverControls.Add("TopControl",      TopControl);
			ObserverControls.Add("QuadrantControl", QuadrantControl);

			ResumeLayout();
		}

		/// <summary>
		/// Instantiates and initializes the <see cref="QuadrantControl"/>.
		/// </summary>
		private void CreateQuadrantPanel()
		{
			QuadrantControl = new QuadrantControl();
			Controls.Add(QuadrantControl);
		}
		#endregion cTor


		#region Events (override) inherited from IObserver/ObserverControl
		/// <summary>
		/// Inherited from <see cref="IObserver"/> through <see cref="ObserverControl"/>.
		/// </summary>
		/// <param name="args"></param>
		public override void OnLevelSelectedObserver(LevelSelectedArgs args)
		{
			//LogFile.WriteLine("TopView.OnLevelSelectedObserver() " + Tag);
			Refresh(); // req'd.
		}
		#endregion Events (override) inherited from IObserver/ObserverControl


		#region Events
		/// <summary>
		/// Handles a click on any of the quadrant-visibility menuitems.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void OnQuadrantVisibilityClick(object sender, EventArgs e)
		{
			var it = sender as ToolStripMenuItem;
			if (it == Floor)
			{
				if (ObserverManager.TopView     .Control   .Floor.Checked =
					ObserverManager.TopRouteView.ControlTop.Floor.Checked = !it.Checked)
				{
					VisibleQuadrants |= Vis_FLOOR;
				}
				else
					VisibleQuadrants &= ~Vis_FLOOR;

				MapFile.CalculateOccultations(!it.Checked);
			}
			else if (it == West)
			{
				if (ObserverManager.TopView     .Control   .West.Checked =
					ObserverManager.TopRouteView.ControlTop.West.Checked = !it.Checked)
				{
					VisibleQuadrants |= Vis_WEST;
				}
				else
					VisibleQuadrants &= ~Vis_WEST;
			}
			else if (it == North)
			{
				if (ObserverManager.TopView     .Control   .North.Checked =
					ObserverManager.TopRouteView.ControlTop.North.Checked = !it.Checked)
				{
					VisibleQuadrants |= Vis_NORTH;
				}
				else
					VisibleQuadrants &= ~Vis_NORTH;
			}
			else //if (it == Content)
			{
				if (ObserverManager.TopView     .Control   .Content.Checked =
					ObserverManager.TopRouteView.ControlTop.Content.Checked = !it.Checked)
				{
					VisibleQuadrants |= Vis_CONTENT;
				}
				else
					VisibleQuadrants &= ~Vis_CONTENT;
			}

			MainViewOverlay.that.SetQuadrantVisibilities(
													(VisibleQuadrants & Vis_FLOOR)   != 0,
													(VisibleQuadrants & Vis_WEST)    != 0,
													(VisibleQuadrants & Vis_NORTH)   != 0,
													(VisibleQuadrants & Vis_CONTENT) != 0);
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

			for (int l = 0; l != MapFile.Levs; ++l)
			for (int r = 0; r != MapFile.Rows; ++r)
			for (int c = 0; c != MapFile.Cols; ++c)
			{
				tile = MapFile.GetTile(c,r,l);
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
			lev = MapFile.Levs - lev; // invert.

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
		/// <summary>
		/// Loads default options for TopView in TopRouteView(Top) screens.
		/// </summary>
		internal protected override void LoadControlDefaultOptions()
		{
			//LogFile.WriteLine("TopView.LoadControlDefaultOptions()");
			Optionables.LoadDefaults(Options);
		}


		internal static Form _foptions;	// is static so that it will be used by
										// both TopView and TopRouteView(Top)
		/// <summary>
		/// Handles a click on the Options button to show or hide an Options-
		/// form. Instantiates an 'OptionsForm' if one doesn't exist for this
		/// viewer. Also subscribes to a form-closing handler that will hide the
		/// form unless MainView is closing.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void OnOptionsClick(object sender, EventArgs e)
		{
			var tsb = sender as ToolStripButton;
			if (!tsb.Checked)
			{
				setOptionsChecked(true);

				if (_foptions == null)
				{
					_foptions = new OptionsForm(
											Optionables,
											Options,
											OptionableType.TopView);
					_foptions.Text = "TopView Options";

					OptionsManager.Views.Add(_foptions);

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
