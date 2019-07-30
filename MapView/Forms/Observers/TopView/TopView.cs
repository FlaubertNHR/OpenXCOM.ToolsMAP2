using System;
using System.Drawing;
using System.Windows.Forms;

using DSShared;

using MapView.Forms.MainView;
using MapView.Forms.Observers;

using XCom;


namespace MapView.Forms.Observers.TopViews
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

			var visQuads = tsddbVisibleQuads.DropDown.Items;

			Floor   = new ToolStripMenuItem(QuadrantDrawService.Floor);
			West    = new ToolStripMenuItem(QuadrantDrawService.West);
			North   = new ToolStripMenuItem(QuadrantDrawService.North);
			Content = new ToolStripMenuItem(QuadrantDrawService.Content);

			visQuads.Add(Floor);
			visQuads.Add(West);
			visQuads.Add(North);
			visQuads.Add(Content);

			Floor  .ShortcutKeys = Keys.F1;
			West   .ShortcutKeys = Keys.F2;
			North  .ShortcutKeys = Keys.F3;
			Content.ShortcutKeys = Keys.F4;

			Floor  .Checked =
			West   .Checked =
			North  .Checked =
			Content.Checked = true;

			VisibleQuadrants = FLOOR | WEST | NORTH | CONTENT;

			foreach (ToolStripMenuItem it in visQuads)
				it.Click += OnQuadrantVisibilityClick;

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
						if (!XCMainWindow.Quit)
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
