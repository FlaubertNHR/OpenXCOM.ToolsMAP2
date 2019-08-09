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
			MapObserverControl // UserControl, IMapObserver
	{
		#region Fields (static)
		internal const int FLOOR   = 1; // flags for parttype visibility ->
		internal const int WEST    = 2;
		internal const int NORTH   = 4;
		internal const int CONTENT = 8;

		private const int DIGITS = 3;
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
		internal TopPanel TopPanel
		{ get; private set; }

		internal QuadrantPanel QuadrantPanel
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
		/// IMPORTANT: TopViewForm and TopRouteViewForm will each invoke and
		/// maintain their own instantiations.
		/// </summary>
		internal TopView()
		{
			Optionables = new TopViewOptionables(this);

			InitializeComponent();
			InitializeQuadrantPanel();

			SuspendLayout();

			QuadrantPanel.SelectedQuadrant = QuadrantType.Floor;


			TopPanel = new TopPanel(this);
			TopPanel.Dock = DockStyle.Fill;

			pnlMain.Controls.Add(TopPanel);
			pnlMain.Resize += (sender, e) => TopPanel.ResizeObserver(
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

			ObserverPanels.Add("TopPanel",      TopPanel);
			ObserverPanels.Add("QuadrantPanel", QuadrantPanel);

			ResumeLayout();
		}

		/// <summary>
		/// Instantiates and initializes the QuadrantPanel.
		/// </summary>
		private void InitializeQuadrantPanel()
		{
			QuadrantPanel = new QuadrantPanel();

			QuadrantPanel.Name     = "QuadrantPanel";
			QuadrantPanel.Location = new Point(0, 410);
			QuadrantPanel.Size     = new Size(640, 70);
			QuadrantPanel.Dock     = DockStyle.Bottom;
			QuadrantPanel.TabIndex = 2;
			QuadrantPanel.TabStop  = false;

			Controls.Add(QuadrantPanel);
		}
		#endregion cTor


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

				MapBase.CalculateOccultations(!it.Checked);
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

			MainViewOverlay.that.Invalidate();

			ObserverManager.TopView     .Control   .TopPanel     .Invalidate();
			ObserverManager.TopRouteView.ControlTop.TopPanel     .Invalidate();
			ObserverManager.TopView     .Control   .QuadrantPanel.Invalidate();
			ObserverManager.TopRouteView.ControlTop.QuadrantPanel.Invalidate();
		}


		/// <summary>
		/// Handles a click on the TestPartslots menuitem.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnTestPartslotsClick(object sender, EventArgs e)
		{
			var list = new List<string>();

			MapTile tile;

			for (int l = 0; l != MapBase.MapSize.Levs; ++l)
			for (int r = 0; r != MapBase.MapSize.Rows; ++r)
			for (int c = 0; c != MapBase.MapSize.Cols; ++c)
			{
				tile = MapBase[r,c,l];
				if (!tile.Vacant)
				{
					if (tile.Floor != null && (QuadrantType)tile.Floor.Record.PartType != QuadrantType.Floor)
						list.Add(FormatTilequad(c,r,l,QuadrantType.Floor));

					if (tile.West != null && (QuadrantType)tile.West.Record.PartType != QuadrantType.West)
						list.Add(FormatTilequad(c,r,l,QuadrantType.West));

					if (tile.North != null && (QuadrantType)tile.North.Record.PartType != QuadrantType.North)
						list.Add(FormatTilequad(c,r,l,QuadrantType.North));

					if (tile.Content != null && (QuadrantType)tile.Content.Record.PartType != QuadrantType.Content)
						list.Add(FormatTilequad(c,r,l,QuadrantType.Content));
				}
			}


			const string title = "Partslots test";

			if (list.Count != 0)
			{
				string copyable = "  c   r   L - slot" + Environment.NewLine;
				foreach (var line in list)
					copyable += Environment.NewLine + line;

				if (_finfobox != null && !_finfobox.IsDisposed) // close Infobox because it's easier than updating its controls.
					_finfobox.Close();

				_finfobox = new Infobox( // not Modal.
									title,
									"The following tileslots are occupied by incorrect PartTypes."
										+ " This could result in broken battlescape behavior.",
									copyable);
				_finfobox.Show();
			}
			else
			{
				using (var f = new Infobox(
										title,
										"All assigned parts are in their correct slots."))
					f.ShowDialog();
			}
		}

		/// <summary>
		/// The TestPartslots dialog.
		/// @note Be careful with this pointer because closing the dialog in the
		/// dialog itself does *not* null this pointer. So check for both !null
		/// and !IsDisposed if necessary.
		/// </summary>
		internal static Infobox _finfobox;
		#endregion Events


		#region Methods
		/// <summary>
		/// Creates the tool-objects in the toolstrip.
		/// </summary>
		/// <param name="tools"></param>
		internal void InitializeToolstrip(ToolstripFactory tools)
		{
			tools.CreateToolstripEditorObjects(tsTools, true);
		}

		/// <summary>
		/// Selects a quadrant in the QuadrantPanel given a selected tiletype.
		/// </summary>
		/// <param name="parttype"></param>
		internal void SelectQuadrant(PartType parttype)
		{
			switch (parttype)
			{
				case PartType.Floor:   QuadrantPanel.SelectedQuadrant = QuadrantType.Floor;   break;
				case PartType.West:    QuadrantPanel.SelectedQuadrant = QuadrantType.West;    break;
				case PartType.North:   QuadrantPanel.SelectedQuadrant = QuadrantType.North;   break;
				case PartType.Content: QuadrantPanel.SelectedQuadrant = QuadrantType.Content; break;
			}
		}

		/// <summary>
		/// Formats a string of x/y/z + quadtype for the TestPartslots dialog.
		/// </summary>
		/// <param name="c"></param>
		/// <param name="r"></param>
		/// <param name="l"></param>
		/// <param name="quad"></param>
		/// <returns></returns>
		private string FormatTilequad(int c, int r, int l, QuadrantType quad)
		{
			c += 1;							// 1-based count
			r += 1;							// 1-based count
			l = MapBase.MapSize.Levs - l;	// invert.

			string c1 = c.ToString().PadLeft(DIGITS);
			string r1 = r.ToString().PadLeft(DIGITS);
			string l1 = l.ToString().PadLeft(DIGITS);

			string quad1 = Enum.GetName(typeof(QuadrantType), quad);

			return c1 + " " + r1 + " " + l1 + " - " + quad1;
		}
		#endregion Methods


		#region Options
		/// <summary>
		/// Loads default options for TopView in TopRouteView screens.
		/// </summary>
		protected internal override void LoadControlDefaultOptions()
		{
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

		private void setOptionsChecked(bool @checked)
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
