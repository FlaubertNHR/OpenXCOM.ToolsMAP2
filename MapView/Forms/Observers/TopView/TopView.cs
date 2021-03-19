using System;
using System.Collections.Generic;
using System.Drawing;
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
		#region Fields (static)
		internal const int FLOOR   = 0x1; // flags for parttype visibility ->
		internal const int WEST    = 0x2;
		internal const int NORTH   = 0x4;
		internal const int CONTENT = 0x8;

		/// <summary>
		/// The TestPartslots dialog.
		/// </summary>
		/// <remarks>Be careful with this pointer because closing the dialog in
		/// the dialog itself does *not* null this pointer. So check for both
		/// !null and !IsDisposed if necessary.</remarks>
		internal static Infobox _finfobox;
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

			// Mono prefers the toolstrip added here in the cTor instead of the
			// designer ... uh, apparently
			tscPanel.LeftToolStripPanel.Controls.Add(tsTools);

			CreateQuadrantPanel();

			SuspendLayout();

			QuadrantControl.SelectedQuadrant = PartType.Floor;


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

			var visQuads = tsddbVisibleQuads.DropDown.Items;

			visQuads.Add(Floor);
			visQuads.Add(West);
			visQuads.Add(North);
			visQuads.Add(Content);

			Floor  .Checked =
			West   .Checked =
			North  .Checked =
			Content.Checked = true;

			VisibleQuadrants = FLOOR | WEST | NORTH | CONTENT;

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

			QuadrantControl.Name     = "QuadrantControl";
			QuadrantControl.Location = new Point(0, 410);
			QuadrantControl.Size     = new Size(640, 70);
			QuadrantControl.Dock     = DockStyle.Bottom;
			QuadrantControl.TabIndex = 2;
			QuadrantControl.TabStop  = false;

			Controls.Add(QuadrantControl);
		}
		#endregion cTor


		#region Events (override) inherited from IObserver/ObserverControl
		/// <summary>
		/// Inherited from <see cref="IObserver"/> through <see cref="ObserverControl"/>.
		/// </summary>
		/// <param name="args"></param>
		public override void OnLevelSelectedObserver(LevelSelectedEventArgs args)
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
					VisibleQuadrants |= FLOOR;
				}
				else
					VisibleQuadrants &= ~FLOOR;

				MapFile.CalculateOccultations(!it.Checked);
			}
			else if (it == West)
			{
				if (ObserverManager.TopView     .Control   .West.Checked =
					ObserverManager.TopRouteView.ControlTop.West.Checked = !it.Checked)
				{
					VisibleQuadrants |= WEST;
				}
				else
					VisibleQuadrants &= ~WEST;
			}
			else if (it == North)
			{
				if (ObserverManager.TopView     .Control   .North.Checked =
					ObserverManager.TopRouteView.ControlTop.North.Checked = !it.Checked)
				{
					VisibleQuadrants |= NORTH;
				}
				else
					VisibleQuadrants &= ~NORTH;
			}
			else //if (it == Content)
			{
				if (ObserverManager.TopView     .Control   .Content.Checked =
					ObserverManager.TopRouteView.ControlTop.Content.Checked = !it.Checked)
				{
					VisibleQuadrants |= CONTENT;
				}
				else
					VisibleQuadrants &= ~CONTENT;
			}

			MainViewOverlay.that.SetQuadrantVisibilities(
													(VisibleQuadrants & FLOOR)   != 0,
													(VisibleQuadrants & WEST)    != 0,
													(VisibleQuadrants & NORTH)   != 0,
													(VisibleQuadrants & CONTENT) != 0);
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
			if (_finfobox != null && !_finfobox.IsDisposed)
			{
				_finfobox.Close(); // TODO: Close the dialog if the Mapfile changes/reloads/etc
				_finfobox = null;
			}

			var lines = new List<string>();

			MapTile tile;
			Tilepart part;
			McdRecord record;

			for (int l = 0; l != MapFile.MapSize.Levs; ++l)
			for (int r = 0; r != MapFile.MapSize.Rows; ++r)
			for (int c = 0; c != MapFile.MapSize.Cols; ++c)
			{
				tile = MapFile[c,r,l];
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

				_finfobox = new Infobox( // not Modal.
									title,
									Infobox.SplitString("The following tileslots are occupied by incorrect"
											+ " PartTypes. This could result in broken battlescape behavior."),
									copyable,
									Infobox.BoxType.Warn);
				_finfobox.Show();
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
		/// Creates the tool-objects in the toolstrip.
		/// </summary>
		internal void CreateToolstripControls()
		{
			ObserverManager.ToolFactory.CreateEditorTools(tsTools, true);
		}

		/// <summary>
		/// Formats a string of x/y/z + quadtype for the TestPartslots dialog.
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
			lev = MapFile.MapSize.Levs - lev; // invert.

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
											OptionsForm.OptionableType.TopView);
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
