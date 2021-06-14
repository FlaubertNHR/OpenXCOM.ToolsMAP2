using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using DSShared;
using DSShared.Controls;

using MapView;
using MapView.Forms.MainView;
using MapView.Volutar;

using McdView;

using PckView;

using XCom;


namespace MapView.Forms.Observers
{
	internal sealed partial class TileView
		:
			ObserverControl // UserControl, IObserver
	{
		internal void DisposeObserver()
		{
			LogFile.WriteLine("TileView.DisposeObserver()");
			if (ContextMenu != null)
			{
				DisposeContext(); // paranoia ... this is .net disposal stuff after all.

				ContextMenu.Dispose();
				ContextMenu = null;
			}

			if (McdInfo != null)
			{
				McdInfo.Dispose();
				McdInfo = null;
			}

			if (_t1 != null)
			{
				_t1.Tick -= t1_OnTick; // not a problem if already unsubscribed.
				_t1.Dispose();
				_t1 = null;
			}

			// TODO: _foptions - look closer

			TilePanel.DisposePanel();
		}

		/// <summary>
		/// Disposes the context paranoicly.
		/// </summary>
		private void DisposeContext()
		{
			ContextMenu.MenuItems[3].Click -= OnMcdInfoClick; // CONTEXT_MI_MCDINFO
			ContextMenu.MenuItems[3].Dispose();

			ContextMenu.MenuItems[2].Dispose();

			ContextMenu.MenuItems[1].Click -= OnMcdViewClick;
			ContextMenu.MenuItems[1].Dispose();

			ContextMenu.MenuItems[0].Click -= OnPckViewClick;
			ContextMenu.MenuItems[0].Dispose();
		}


		#region Events
		/// <summary>
		/// Fires if a save was done in PckView or McdView (via TileView).
		/// </summary>
		internal event MethodInvoker ReloadDescriptor;
		#endregion Events


		#region Fields (static)
		private const int CONTEXT_MI_MCDINFO = 3;
		#endregion Fields (static)


		#region Fields
		private TilePanel _allTiles;
		private TilePanel[] _panels;

		private Timer _t1 = new Timer();
		#endregion Fields


		#region Properties
		/// <summary>
		/// Inherited from <see cref="IObserver"/> through <see cref="ObserverControl"/>.
		/// </summary>
		[Browsable(false)]
		public override MapFile MapFile
		{
			set
			{
				IList<Tilepart> parts;
				if ((base.MapFile = value) != null)
					parts = value.Parts;
				else
					parts = null;

				SetTileParts(parts);
			}
		}

		/// <summary>
		/// Gets the selected-tilepart.
		/// Sets the selected-tilepart when a valid QuadrantControl quad is
		/// double-clicked.
		/// </summary>
		/// <remarks>TileView switches to the ALL tabpage and selects the
		/// appropriate tilepart, w/ TilePanel.SelectedTilepart, when a quad is
		/// selected in the QuadrantControl. The TilepartSelected event then
		/// fires, and then the TilepartSelected_SelectQuadrant event fires.
		/// Thought you'd like to know how good the spaghetti tastes.</remarks>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)] // w.t.f.
		internal Tilepart SelectedTilepart
		{
			get { return GetSelectedPanel().SelectedTilepart; }
			set
			{
				_allTiles.SelectedTilepart = value;
				tcPartTypes.SelectedIndex = 0;

				Refresh(); // req'd.
			}
		}

		internal McdInfoF McdInfo
		{ get; private set; }
		#endregion Properties


		#region Properties (static)
		/// <summary>
		/// A class-object that holds TileView's optionable Properties.
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
		/// I created an independent class just to hold and handle TileView's
		/// Optionable Properties ... and wired it up. It's a tedious shitfest
		/// but better than the arcane MapViewI system or screwing around with
		/// equally arcane TypeDescriptors. Both of which had been implemented
		/// but then rejected.
		/// </summary>
		internal static TileViewOptionables Optionables
		{ get; private set; }
		#endregion Properties (static)


		#region cTor
		/// <summary>
		/// cTor. Instantiates the TileView control and its pages/panels.
		/// </summary>
		internal TileView()
		{
			Optionables = new TileViewOptionables(this);

			InitializeComponent();
			var tpBorder = new TabPageBorder(tcPartTypes);
			tpBorder.TabPageBorder_init();

			Dock = DockStyle.Fill;

//			tcPartTypes.MouseWheel           += tabs_OnMouseWheel;
			tcPartTypes.SelectedIndexChanged += tabs_OnSelectedIndexChanged;

			TilePanel.TileView = this;

			tpFloors    .Text = QuadrantDrawService.Floor;
			tpWestwalls .Text = QuadrantDrawService.West;
			tpNorthwalls.Text = QuadrantDrawService.North;
			tpContents  .Text = QuadrantDrawService.Content;

			_allTiles      = new TilePanel(PartType.Invalid);
			var floors     = new TilePanel(PartType.Floor);
			var westwalls  = new TilePanel(PartType.West);
			var northwalls = new TilePanel(PartType.North);
			var content    = new TilePanel(PartType.Content);

			_panels = new[]
			{
				_allTiles,
				floors,
				westwalls,
				northwalls,
				content
			};

			AddPanel(tpAll,       _allTiles);
			AddPanel(tpFloors,     floors);
			AddPanel(tpWestwalls,  westwalls);
			AddPanel(tpNorthwalls, northwalls);
			AddPanel(tpContents,   content);

			ssStatus.Renderer = new CustomToolStripRenderer();

			CreateContext();

			_t1.Tick += t1_OnTick;			// Because the mouse OnLeave event doesn't
			_t1.Interval = Globals.PERIOD;	// fire when the mouse moves out of the
			_t1.Enabled = true;				// panel directly from a tilepart's icon.
		}

		/// <summary>
		/// Adds a panel to a specified tabpage and subscribes the panel's
		/// <see cref="TilePanel.TilepartSelected">TilePanel.TilepartSelected</see>
		/// event to <see cref="panel_OnTilepartSelected"/>.
		/// </summary>
		/// <param name="page"></param>
		/// <param name="panel"></param>
		private void AddPanel(Control page, TilePanel panel)
		{
			panel.TilepartSelected += panel_OnTilepartSelected; // 'this' won't be released until all panels are disposed.
			page.Controls.Add(panel);
		}


		/// <summary>
		/// Builds the ContextMenu.
		/// </summary>
		private void CreateContext()
		{
			ContextMenu = new ContextMenu();

			ContextMenu.MenuItems.Add(new MenuItem("open in PckView", OnPckViewClick, Shortcut.F9));	// 0
			ContextMenu.MenuItems.Add(new MenuItem("open in McdView", OnMcdViewClick, Shortcut.F10));	// 1
			ContextMenu.MenuItems.Add(new MenuItem("-"));												// 2
			ContextMenu.MenuItems.Add(new MenuItem("view MCD record", OnMcdInfoClick));					// 3 //null, EventArgs.Empty
		}
		#endregion cTor


		#region Events (override)
		/// <summary>
		/// Bypasses level-change in <see cref="ObserverControl"/> to prevent
		/// awkward level changes.
		/// </summary>
		/// <param name="e"></param>
		/// <remarks>redesign wanted.</remarks>
		protected override void OnMouseWheel(MouseEventArgs e)
		{
//			base.OnMouseWheel(e);
		}
		#endregion Events (override)


		#region Events
		/// <summary>
		/// Clears OverInfo on the statusbar when the cursor is not in a panel.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void t1_OnTick(object sender, EventArgs e)
		{
			if (tsslOver.Text.Length != 0)
				GetSelectedPanel().ElvisHasLeft();
		}

/*		/// <summary>
		/// Is supposed to flip through the tabs on mousewheel events when the
		/// tabcontrol is focused, but focus switches to the page's panel ...
		/// not sure why though.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tabs_OnMouseWheel(object sender, MouseEventArgs e)
		{
			int dir = 0;
			if      (e.Delta < 0) dir = +1;
			else if (e.Delta > 0) dir = -1;

			if (dir != 0)
			{
				int page = tcPartTypes.SelectedIndex + dir;
				if (page > -1 && page < tcPartTypes.TabCount)
				{
					tcPartTypes.SelectedIndex = page;
				}
			}
		} */

		/// <summary>
		/// Triggers when a tab-index changes. Updates the titlebar-text,
		/// quadrant, and MCD-info (if applicable). Also subscribes/unsubscribes
		/// panels to the static ticker's event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tabs_OnSelectedIndexChanged(object sender, EventArgs e)
		{
			panel_OnTilepartSelected(SelectedTilepart);
		}

		/// <summary>
		/// Handles the panels' <see cref="TilePanel.TilepartSelected">TilePanel.TilepartSelected</see>
		/// event. Further triggers the 'TilepartSelected_SelectQuadrant' event.
		/// </summary>
		/// <param name="part"></param>
		private void panel_OnTilepartSelected(Tilepart part)
		{
			SetTitleText(part);

			if (McdInfo != null)
				McdInfo.UpdateData();

			if (part != null)
			{
				ObserverManager.TopView     .Control   .QuadrantControl.SelectedQuadrant =
				ObserverManager.TopRouteView.ControlTop.QuadrantControl.SelectedQuadrant = part.Record.PartType;
			}

			QuadrantDrawService.CurrentTilepart = part;
			ObserverManager.InvalidateQuadrantControls();
		}
		#endregion Events


		#region Options
		/// <summary>
		/// Loads default options for TileView.
		/// </summary>
		internal protected override void LoadControlDefaultOptions()
		{
			//LogFile.WriteLine("TileView.LoadControlDefaultOptions()");
			Optionables.LoadDefaults(Options);
		}


		internal static Form _foptions; // is static for no special reason

		/// <summary>
		/// Handles a click on the Options button to show or hide an Options-
		/// form. Instantiates an <see cref="OptionsForm"/> if one doesn't exist
		/// for this viewer. Also subscribes to a form-closing handler that will
		/// hide the form unless MapView is quitting.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void OnOptionsClick(object sender, EventArgs e)
		{
			var tsb = sender as ToolStripButton;
			if (tsb.Checked = !tsb.Checked)
			{
				if (_foptions == null)
				{
					_foptions = new OptionsForm(
											Optionables,
											Options,
											OptionableType.TileView);
					_foptions.Text = "TileView Options";

					OptionsManager.Views.Add(_foptions);

					_foptions.FormClosing += (sender1, e1) =>
					{
						if (!MainViewF.Quit)
						{
							tsb.Checked = false;

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
		/// Gets the Options button on the toolstrip.
		/// </summary>
		/// <returns>the Options button</returns>
		internal ToolStripButton GetOptionsButton()
		{
			return tsb_Options;
		}
		#endregion Options


		#region Events (menu)
		/// <summary>
		/// Opens the <see cref="McdInfoF"/> dialog.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void OnMcdInfoClick(object sender, EventArgs e)
		{
			MenuItem it = ContextMenu.MenuItems[CONTEXT_MI_MCDINFO];
			if (it.Checked = !it.Checked)
			{
				if (McdInfo == null)
					McdInfo = new McdInfoF(this);

				McdInfo.Show(); // no owner.

				if (McdInfo.WindowState == FormWindowState.Minimized)
					McdInfo.WindowState  = FormWindowState.Normal;
			}
			else
				McdInfo.Hide();
		}

		/// <summary>
		/// Opens MCDEdit.exe or any program or file specified in Settings.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnVolutarMcdEditorClick(object sender, EventArgs e)
		{
			if (MapFile != null)
			{
				var service = new VolutarService(Options);
				var pfe = service.FullPath;	// this will invoke a box for the user to input the
											// executable's path if it doesn't exist in Options.
				if (File.Exists(pfe))
				{
					// change to MCDEdit dir so that accessing MCDEdit.txt doesn't cause probls.
					Directory.SetCurrentDirectory(Path.GetDirectoryName(pfe));

					Process.Start(new ProcessStartInfo(pfe));

					// change back to app dir
					Directory.SetCurrentDirectory(SharedSpace.GetShareString(SharedSpace.ApplicationDirectory));
				}
			}
		}

		/// <summary>
		/// Opens PckView with the spriteset of the currently selected tilepart
		/// loaded.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void OnPckViewClick(object sender, EventArgs e)
		{
			if (SelectedTilepart != null)
			{
				Tuple<string,string> terrain = MapFile.GetTerrain(SelectedTilepart);

				string terr = terrain.Item1;
				string path = terrain.Item2;

				path = MapFile.Descriptor.GetTerrainDirectory(path);

				string pfePck = Path.Combine(path, terr + GlobalsXC.PckExt);
				string pfeTab = Path.Combine(path, terr + GlobalsXC.TabExt);

				int lines = 0;
				string copyable = null;
				if (!File.Exists(pfePck))
				{
					++lines;
					copyable = pfePck;
				}

				if (!File.Exists(pfeTab))
				{
					++lines;
					if (copyable != null)
					{
						copyable += Environment.NewLine;
						copyable += pfeTab;
					}
					else
						copyable = pfeTab;
				}

				if (lines != 0)
				{
					string title, head;
					if (lines == 1)
					{
						title = "File not found";
						head  = "File does not exist.";
					}
					else
					{
						title = "Files not found";
						head  = "Files do not exist.";
					}

					using (var f = new Infobox(
											title,
											head,
											copyable,
											InfoboxType.Error))
					{
						f.ShowDialog(this);
					}
				}
				else
				{
					using (var fPckView = new PckViewF(true, GetSpriteshade()))
					{
						fPckView.SetSpritesetType(PckView.SpritesetType.Pck);
						fPckView.LoadSpriteset(pfePck);
						fPckView.SetPalette(MapFile.Descriptor.Pal);
						fPckView.SetSelected(SelectedTilepart[0].Id);

						ShowHideManager.HideViewers();
						fPckView.ShowDialog(ObserverManager.TileView); // <- Pause UI until PckView is closed.
						ShowHideManager.ShowViewers();

						Palette.UfoBattle .SetTransparent(true); // ensure that transparency is turned on after returning
						Palette.TftdBattle.SetTransparent(true); // from PckView

						XCImage.SpriteWidth  = XCImage.SpriteWidth32;  // ensure that sprite width and height return
						XCImage.SpriteHeight = XCImage.SpriteHeight40; // to MapView defaults


						if (fPckView.RequestReload					// the Descriptor needs to reload
							&& CheckReload() == DialogResult.OK)	// so ask user if he/she wants to save the current Map+Routes (if changed)
						{
							if (ReloadDescriptor != null)
								ReloadDescriptor();
						}
					}
				}
			}
			else
				error_SelectTile();
		}

		/// <summary>
		/// Opens McdView with the recordset of the currently selected tilepart
		/// loaded.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void OnMcdViewClick(object sender, EventArgs e)
		{
			if (SelectedTilepart != null)
			{
				Tuple<string,string> terrain = MapFile.GetTerrain(SelectedTilepart);

				string terr = terrain.Item1;
				string path = terrain.Item2;

				path = MapFile.Descriptor.GetTerrainDirectory(path);

				string pfeMcd = Path.Combine(path, terr + GlobalsXC.McdExt);

				if (!File.Exists(pfeMcd))
				{
					using (var f = new Infobox(
											"File not found",
											"File does not exist.",
											pfeMcd,
											InfoboxType.Error))
					{
						f.ShowDialog(this);
					}
				}
				else
				{
					using (var fMcdView = new McdviewF(true, GetSpriteshade()))
					{
						Palette.UfoBattle .SetTransparent(false); // NOTE: McdView wants non-transparent palettes.
						Palette.TftdBattle.SetTransparent(false);

						fMcdView.LoadRecords(
										pfeMcd,
										MapFile.Descriptor.Pal,
										SelectedTilepart.TerId);

						ShowHideManager.HideViewers();
						fMcdView.ShowDialog(ObserverManager.TileView); // <- Pause UI until McdView is closed.
						ShowHideManager.ShowViewers();

						Palette.UfoBattle .SetTransparent(true);
						Palette.TftdBattle.SetTransparent(true);


						if (fMcdView.FireMvReload					// the Descriptor needs to reload
							&& CheckReload() == DialogResult.OK)	// so ask user if he/she wants to save the current Map+Routes (if changed)
						{
							if (ReloadDescriptor != null)
								ReloadDescriptor();
						}
					}
				}
			}
			else
				error_SelectTile();
		}

		/// <summary>
		/// Gets the current sprite-shade in
		/// <see cref="MainViewF.Optionables">MainViewF.Optionables</see>.
		/// </summary>
		/// <returns>sprite-shade or -1 if disabled</returns>
		private static int GetSpriteshade()
		{
			if (MainViewF.Optionables.SpriteShadeEnabled)
				return MainViewF.Optionables.SpriteShade;

			return -1;
		}

		/// <summary>
		/// Warns user that they are about to be asked to save the Map+Routes
		/// after any changes were saved in McdView/PckView. The warning makes
		/// things a bit less jarring by stating the reason why.
		/// </summary>
		/// <returns></returns>
		private DialogResult CheckReload()
		{
			string head = "The tileset needs to reload to show any changes"
						+ " that may have been made to its terrainset.";

			string info = String.Empty;
			if (MapFile.MapChanged)
				info = "Map";

			if (MapFile.RoutesChanged)
			{
				if (info.Length != 0) info += " and its ";
				info += "Routes";
			}

			if (info.Length != 0)
			{
				head += " You will be asked to save changes to the " + info + ".";
			}

			using (var f = new Infobox(
									"Reload tileset",
									Infobox.SplitString(head),
									null,
									InfoboxType.Warn,
									InfoboxButtons.CancelOkay))
			{
				return f.ShowDialog(this);
			}
		}

		/// <summary>
		/// Invokes an <see cref="Infobox"/> telling the user that the operation
		/// they are attempting is invalid because they haven't selected a
		/// tilepart.
		/// </summary>
		private void error_SelectTile()
		{
			using (var f = new Infobox(
									"Error",
									"Select a Tilepart.",
									null,
									InfoboxType.Error))
			{
				f.ShowDialog(this);
			}
		}


		/// <summary>
		/// Displays MainView's Colorhelp dialog.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnColorhelpClick(object sender, EventArgs e)
		{
			MainViewF.that.OnColorsClick(null, EventArgs.Empty);
		}

		/// <summary>
		/// Checks/Unchecks the colorhelp button.
		/// </summary>
		/// <param name="checked"></param>
		internal void CheckColorhelp(bool @checked)
		{
			tsb_Colorhelp.Checked = @checked;
		}
		#endregion Events (menu)


		#region Methods
		/// <summary>
		/// Fills the <see cref="TileView"/> pages with tileparts.
		/// </summary>
		/// <param name="parts"></param>
		private void SetTileParts(IList<Tilepart> parts)
		{
			for (int id = 0; id != _panels.Length; ++id)
				_panels[id].SetTiles(parts);

			tsslTotal.Text = "Total " + parts.Count;

			if (parts.Count > MapFile.MaxTerrainId)
				tsslTotal.ForeColor = Color.MediumVioletRed;
			else
				tsslTotal.ForeColor = SystemColors.ControlText;

			OnResize(null);
		}

		/// <summary>
		/// Sets the title-text to a string that's appropriate for the currently
		/// selected tilepart.
		/// </summary>
		/// <param name="part">can be null</param>
		private void SetTitleText(Tilepart part)
		{
			string title = "TileView";
			if (part != null)
			{
				title += " - " + GetTerrainLabel()
					   + "  terId " + part.TerId
					   + "  setId " + part.SetId;
			}
			ObserverManager.TileView.Text = title;
		}

		/// <summary>
		/// Prints info for a mouseovered tilepart.
		/// </summary>
		/// <param name="part"></param>
		internal void PrintOverInfo(Tilepart part)
		{
			string info = String.Empty;

			if (part != null)
			{
				info = MapFile.GetTerrainLabel(part)
					 + "  terId " + part.TerId
					 + "  setId " + part.SetId;
			}
			tsslOver.Text = info;
		}

		/// <summary>
		/// Gets the label of the terrain of the currently selected tilepart.
		/// </summary>
		/// <returns></returns>
		internal string GetTerrainLabel()
		{
			if (SelectedTilepart != null)
				return MapFile.GetTerrainLabel(SelectedTilepart);

			return "ERROR";
		}

		/// <summary>
		/// Gets the panel of the currently selected tabpage.
		/// </summary>
		/// <returns></returns>
		internal TilePanel GetSelectedPanel()
		{
			return _panels[tcPartTypes.SelectedIndex];
		}
		#endregion Methods
	}
}
