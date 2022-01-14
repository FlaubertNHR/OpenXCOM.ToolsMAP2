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
using MapView.ExternalProcess;

using McdView;

using PckView;

using XCom;


namespace MapView.Forms.Observers
{
	internal sealed partial class TileView
		:
			UserControl
	{
		internal void DisposeObserver()
		{
			//Logfile.Log("TileView.DisposeObserver()");
			if (ContextMenuStrip != null)
			{
				DisposeContext(); // paranoia ... this is .net disposal stuff after all.

				ContextMenuStrip.Dispose();
				ContextMenuStrip = null;
			}

			if (McdInfo != null)
			{
				McdInfo.Close(); // needs to update registry
				McdInfo = null;
			}

			if (_t1 != null)
			{
				_t1.Tick -= t1_OnTick; // not a problem if already unsubscribed.
				_t1.Dispose();
				_t1 = null;
			}

			// TODO: _foptions - look closer

			TilePanel.DisposePanels();
		}

		/// <summary>
		/// Disposes the context paranoicly.
		/// </summary>
		private void DisposeContext()
		{
			ContextMenuStrip.Items[3].Click -= OnMcdInfoClick; // CONTEXT_MI_MCDINFO
			ContextMenuStrip.Items[3].Dispose();

			ContextMenuStrip.Items[2].Dispose();

			ContextMenuStrip.Items[1].Click -= OnMcdViewClick;
			ContextMenuStrip.Items[1].Dispose();

			ContextMenuStrip.Items[0].Click -= OnPckViewClick;
			ContextMenuStrip.Items[0].Dispose();
		}


		#region Events
		/// <summary>
		/// Fires if a save was done in PckView or McdView after invoked by
		/// <c>TileView</c>.
		/// </summary>
		internal event MethodInvoker ReloadDescriptor;
		#endregion Events


		#region Fields (static)
		private const string TITLE = "TileView";

		private const int CONTEXT_MI_MCDINFO = 3;
		#endregion Fields (static)


		#region Fields
		private MapFile _file;

		private TilePanel _allTiles;
		private TilePanel[] _panels;

		private Timer _t1 = new Timer();
		#endregion Fields


		#region Properties
		/// <summary>
		/// Gets the current <c>SelectedTilepart</c>.
		/// 
		/// Sets the <c>SelectedTilepart</c> when a valid
		/// <c><see cref="QuadrantControl"/></c> quad is selected.
		/// </summary>
		/// <remarks>The setter is used only by
		/// <c><see cref="QuadrantControl.Clicker()">QuadrantControl.Clicker()</see></c>.
		/// <c>TileView</c> switches to the ALL tabpage and selects the
		/// appropriate <c><see cref="Tilepart"/></c> per
		/// <c><see cref="TilePanel.SelectedTilepart">TilePanel.SelectedTilepart</see></c>
		/// when a quad is selected in the <c>QuadrantControl</c>.</remarks>
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
		internal static Options Options = new Options();

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
		internal static TileViewOptionables Optionables = new TileViewOptionables();
		#endregion Properties (static)


		#region cTor
		/// <summary>
		/// cTor. Instantiates this <c>TileView</c> control and its pages of
		/// <c><see cref="TilePanel">TilePanels</see></c>.
		/// </summary>
		internal TileView()
		{
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
			var contents   = new TilePanel(PartType.Content);

			tpAll       .Controls.Add(_allTiles);
			tpFloors    .Controls.Add(floors);
			tpWestwalls .Controls.Add(westwalls);
			tpNorthwalls.Controls.Add(northwalls);
			tpContents  .Controls.Add(contents);

			_panels = new[]
			{
				_allTiles,
				floors,
				westwalls,
				northwalls,
				contents
			};

//			TilePanel.TextWidth = TextRenderer.MeasureText(TilePanel.Door, TilePanel.Font).Width;						// =30
			TilePanel.TextWidth = (int)_allTiles.CreateGraphics().MeasureString(TilePanel.Door, _allTiles.Font).Width;	// =24

			ssStatus.Renderer = new CustomToolStripRenderer();

			CreateContext();

			_t1.Tick += t1_OnTick;			// Because the mouse OnLeave event doesn't
			_t1.Interval = Globals.PERIOD;	// fire when the mouse moves out of a
			_t1.Enabled = true;				// panel directly from a tilepart's icon.
		}

		/// <summary>
		/// Builds the <c>ContextMenuStrip</c>.
		/// </summary>
		private void CreateContext()
		{
			ContextMenuStrip = new ContextMenuStrip();

			var it = new ToolStripMenuItem();			// 0
			it.Text = "open in PckView";
			it.ShortcutKeys = Keys.F9;
			it.Click += OnPckViewClick;
			ContextMenuStrip.Items.Add(it);

			it = new ToolStripMenuItem();				// 1
			it.Text = "open in McdView";
			it.ShortcutKeys = Keys.F10;
			it.Click += OnMcdViewClick;
			ContextMenuStrip.Items.Add(it);

			var separator = new ToolStripSeparator();	// 2
			ContextMenuStrip.Items.Add(separator);

			it = new ToolStripMenuItem();				// 3 - CONTEXT_MI_MCDINFO
			it.Text = "MCD record";
			it.Click += OnMcdInfoClick; //null, EventArgs.Empty
			ContextMenuStrip.Items.Add(it);

			ContextMenuStrip.Opening += context_OnOpening;
		}
		#endregion cTor


		#region Events
		/// <summary>
		/// Cancels opening the <c>ContextMenuStrip</c> if the cursor is not
		/// inside the bounds of a <c><see cref="TilePanel"/></c>.
		/// </summary>
		/// <param name="sender"><c>ContextMenuStrip</c></param>
		/// <param name="e"></param>
		private void context_OnOpening(object sender, CancelEventArgs e)
		{
			if (!_allTiles.ClientRectangle.Contains(_allTiles.PointToClient(Cursor.Position)))
				e.Cancel = true;
		}

		/// <summary>
		/// Clears OverInfo on the statusbar when the cursor is not in a panel.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void t1_OnTick(object sender, EventArgs e)
		{
			if (tsslOver.Text.Length != 0)
				GetSelectedPanel().ElvisHasLeftThePanel();
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
		/// quadrant, and <c><see cref="McdInfo"/></c> (if applicable).
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tabs_OnSelectedIndexChanged(object sender, EventArgs e)
		{
			SelectTilepart(SelectedTilepart);
		}

		/// <summary>
		/// Updates stuff after user selects a <c><see cref="Tilepart"/></c>.
		/// </summary>
		internal void SelectTilepart(Tilepart part)
		{
			SetTitleText(part);

			if (McdInfo != null)
				McdInfo.UpdateData();

			if (part != null)
			{
				ObserverManager.TopView     .Control   .QuadrantControl.SelectedQuadrant =
				ObserverManager.TopRouteView.ControlTop.QuadrantControl.SelectedQuadrant = part.Record.PartType;
			}

			QuadrantDrawService.SelectedTilepart = part;
			ObserverManager.InvalidateQuadrantControls();
		}
		#endregion Events


		#region Options
		internal static OptionsF _foptions; // is static for no special reason

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
			if (tsb.Checked = !tsb.Checked)
			{
				if (_foptions == null)
				{
					_foptions = new OptionsF(
										Optionables,
										Options,
										OptionableType.TileView);
					_foptions.Text = "TileView Options";

					OptionsManager.Options.Add(_foptions);

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
		/// Opens or hides the <c><see cref="McdInfoF"/></c> dialog.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void OnMcdInfoClick(object sender, EventArgs e)
		{
			var it = ContextMenuStrip.Items[CONTEXT_MI_MCDINFO] as ToolStripMenuItem;
			if (it.Checked = !it.Checked)
			{
				if (McdInfo == null)
					McdInfo = new McdInfoF();

				McdInfo.Show(); // no owner.

				if (McdInfo.WindowState == FormWindowState.Minimized)
					McdInfo.WindowState  = FormWindowState.Normal;
			}
			else
				McdInfo.Hide();
		}

		/// <summary>
		/// Opens any application - or file if its extension is associated in
		/// the Windows Registry.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnExternalProcessClick(object sender, EventArgs e)
		{
			string pfe = new ExternalProcessService(Options).GetFullpath();
			if (File.Exists(pfe))
			{
				// change to pfe-dir so that accessing MCDEdit.txt (eg) doesn't cause probls.
				Directory.SetCurrentDirectory(Path.GetDirectoryName(pfe));

				Process.Start(new ProcessStartInfo(pfe));

				// change back to app-dir
				Directory.SetCurrentDirectory(SharedSpace.GetShareString(SharedSpace.ApplicationDirectory));
			}
		}

		/// <summary>
		/// Invokes <c><see cref="PckViewF"/>.PckViewF()</c> with the
		/// <c><see cref="Spriteset"/></c> of the currently selected
		/// <c><see cref="Tilepart"/></c> loaded.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnPckViewClick(object sender, EventArgs e)
		{
			if (SelectedTilepart != null)
			{
				Tuple<string,string> terrain = _file.GetTerrain(SelectedTilepart);

				string terr = terrain.Item1;
				string path = terrain.Item2;

				path = _file.Descriptor.GetTerrainDirectory(path);

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
					using (var fPckView = new PckViewF(true, MainViewF.Optionables.SpriteShade))
					{
						fPckView.SetSpritesetType(SpritesetType.Pck);
						fPckView.LoadSpriteset(pfePck);
						fPckView.SetPalette(_file.Descriptor.Pal);
						fPckView.SetSelected(SelectedTilepart[0].Id);

						ShowHideManager.HideViewers();
						fPckView.ShowDialog(ObserverManager.TileView); // <- Pause UI until PckView is closed.
						ShowHideManager.ShowViewers();

						Palette.UfoBattle .SetTransparent(true); // ensure that transparency is turned on after returning
						Palette.TftdBattle.SetTransparent(true); // from PckView


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
		/// Invokes <c><see cref="McdviewF"/>.McdviewF()</c> with the recordset
		/// of the currently selected <c><see cref="Tilepart"/></c> loaded.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMcdViewClick(object sender, EventArgs e)
		{
			if (SelectedTilepart != null)
			{
				Tuple<string,string> terrain = _file.GetTerrain(SelectedTilepart);

				string terr = terrain.Item1;
				string path = terrain.Item2;

				path = _file.Descriptor.GetTerrainDirectory(path);

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
					using (var fMcdView = new McdviewF(true, MainViewF.Optionables.SpriteShade))
					{
						Palette.UfoBattle .SetTransparent(false); // NOTE: McdView wants non-transparent palettes.
						Palette.TftdBattle.SetTransparent(false);

						fMcdView.LoadRecords(
										pfeMcd,
										_file.Descriptor.Pal,
										SelectedTilepart.Id);

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
			if (_file.MapChanged)
				info = "Map";

			if (_file.RoutesChanged)
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
		/// Invokes an <c><see cref="Infobox"/></c> telling the user that the
		/// operation they are attempting is invalid because they haven't
		/// selected a <c><see cref="Tilepart"/></c>.
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
		/// Displays the <c><see cref="ColorHelp"/></c> dialog in
		/// <c><see cref="MainViewF"/></c>.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnColorhelpClick(object sender, EventArgs e)
		{
			MainViewF.that.OnColorsClick(null, EventArgs.Empty);
		}

		/// <summary>
		/// Checks/Unchecks the
		/// <c><see cref="tsb_Colorhelp">colorhelp button</see></c>.
		/// </summary>
		/// <param name="checked"></param>
		internal void CheckColorhelp(bool @checked)
		{
			tsb_Colorhelp.Checked = @checked;
		}
		#endregion Events (menu)


		#region Methods
		/// <summary>
		/// Sets <c><see cref="_file"/></c> and populates
		/// <c>TileView's</c> <c><see cref="TilePanel">TilePanels</see></c> with
		/// <c><see cref="Tilepart">Tileparts</see></c>.
		/// </summary>
		/// <param name="file">a <c><see cref="MapFile"/></c></param>
		internal void SetMapfile(MapFile file)
		{
			IList<Tilepart> parts = (_file = file).Parts;

			for (int id = 0; id != _panels.Length; ++id)
				_panels[id].PopulatePanel(parts);

			tsslTotal.Text = "Total " + parts.Count;

			if (parts.Count > MapFile.MaxTerrainId)
				tsslTotal.ForeColor = Color.MediumVioletRed;
			else
				tsslTotal.ForeColor = SystemColors.ControlText;

			OnResize(null);
		}


		/// <summary>
		/// Sets the title-text to a string that's appropriate for the currently
		/// selected <c><see cref="Tilepart"/></c>.
		/// </summary>
		/// <param name="part">can be null</param>
		private void SetTitleText(Tilepart part)
		{
			string title = TITLE;
			if (part != null)
			{
				title += " - " + GetTerrainLabel()
					   + "  terId " + part.Id
					   + "  setId " + part.SetId;
			}
			ObserverManager.TileView.Text = title;
		}

		/// <summary>
		/// Prints info for a mouseovered <c><see cref="Tilepart"/></c>.
		/// </summary>
		/// <param name="part"></param>
		internal void PrintOverInfo(Tilepart part)
		{
			string info = String.Empty;

			if (part != null)
			{
				info = _file.GetTerrainLabel(part)
					 + "  terId " + part.Id
					 + "  setId " + part.SetId;
			}
			tsslOver.Text = info;
		}

		/// <summary>
		/// Gets the label of the terrain of the
		/// <c><see cref="SelectedTilepart"/></c>.
		/// </summary>
		/// <returns></returns>
		internal string GetTerrainLabel()
		{
			if (SelectedTilepart != null)
				return _file.GetTerrainLabel(SelectedTilepart);

			return "ERROR";
		}

		/// <summary>
		/// Gets the <c><see cref="TilePanel"/></c> of the currently selected
		/// tabpage.
		/// </summary>
		/// <returns></returns>
		internal TilePanel GetSelectedPanel()
		{
			return _panels[tcPartTypes.SelectedIndex];
		}
		#endregion Methods
	}
}
