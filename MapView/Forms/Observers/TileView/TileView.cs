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
		#region Events
		/// <summary>
		/// Fires if a save was done in PckView or McdView (via TileView).
		/// </summary>
		internal event MethodInvoker ReloadDescriptor;
		#endregion Events


		#region Fields
		private TilePanel _allTiles;
		private TilePanel[] _panels;
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
			get { return GetVisiblePanel().SelectedTilepart; }
			set
			{
				_allTiles.SelectedTilepart = value;
				tcTileTypes.SelectedIndex = 0;

				Refresh(); // req'd.
			}
		}

		internal McdInfoF McdInfobox
		{ get; set; }
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
		/// cTor. Instantiates the TileView viewer and its pages/panels.
		/// </summary>
		internal TileView()
		{
			Optionables = new TileViewOptionables(this);

			InitializeComponent();
			var tpTileTypes = new TabPageBorder(tcTileTypes);

//			tcTileTypes.MouseWheel           += tabs_OnMouseWheel;
			tcTileTypes.SelectedIndexChanged += tabs_OnSelectedIndexChanged;

			TilePanel.Chaparone = this;

			_allTiles      = new TilePanel(PartType.Invalid);
			var floors     = new TilePanel(PartType.Floor);
			var westwalls  = new TilePanel(PartType.West);
			var northwalls = new TilePanel(PartType.North);
			var content    = new TilePanel(PartType.Content);

			tpFloors    .Text = QuadrantDrawService.Floor;
			tpWestwalls .Text = QuadrantDrawService.West;
			tpNorthwalls.Text = QuadrantDrawService.North;
			tpContents  .Text = QuadrantDrawService.Content;

			_panels = new[]
			{
				_allTiles,
				floors,
				westwalls,
				northwalls,
				content
			};

			AddPanel(_allTiles,  tpAll);
			AddPanel(floors,     tpFloors);
			AddPanel(westwalls,  tpWestwalls);
			AddPanel(northwalls, tpNorthwalls);
			AddPanel(content,    tpContents);

			_allTiles.SetTickerSubscription(true);

			ssStatus.Renderer = new CustomToolStripRenderer();
		}

		/// <summary>
		/// Adds a panel to a specified page and subscribes to the
		/// TilepartSelected event.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="page"></param>
		private void AddPanel(TilePanel panel, Control page)
		{
			panel.TilepartSelected += panel_OnTilepartSelected;
			page.Controls.Add(panel);
		}
		#endregion cTor


		#region Events (override)
		/// <summary>
		/// Bypasses level-change in <see cref="ObserverControl"/> to prevent
		/// awkward level changes.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseWheel(MouseEventArgs e)
		{
//			base.OnMouseWheel(e);
		}
		#endregion Events (override)


		#region Events
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
				int page = tcTileTypes.SelectedIndex + dir;
				if (page > -1 && page < tcTileTypes.TabCount)
				{
					tcTileTypes.SelectedIndex = page;
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
			var current = GetVisiblePanel();
			foreach (var panel in _panels)
				panel.SetTickerSubscription(panel == current);

			panel_OnTilepartSelected(SelectedTilepart);
		}

		/// <summary>
		/// Triggers on the 'TilepartSelected' event. Further triggers the
		/// 'TilepartSelected_SelectQuadrant' event.
		/// </summary>
		/// <param name="part"></param>
		private void panel_OnTilepartSelected(Tilepart part)
		{
			SetTitleText(part);

			if (McdInfobox != null)
			{
				McdRecord record;
				int id;
				string label;

				if (part != null)
				{
					record = part.Record;
					id = part.TerId;
					label = GetTerrainLabel();
				}
				else
				{
					record = null;
					id = -1;
					label = String.Empty;
				}

				McdInfobox.UpdateData(record, id, label);
			}

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
		/// form. Instantiates an 'OptionsForm' if one doesn't exist for this
		/// viewer. Also subscribes to a form-closing handler that will hide the
		/// form unless MainView is closing.
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
											OptionsForm.OptionableType.TileView);
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
			if (!GetVisiblePanel().ContextMenu.MenuItems[3].Checked)
			{
				foreach (var panel in _panels)
				{
					panel.ContextMenu.MenuItems[3].Checked = true;
				}

				if (McdInfobox == null)
				{
					McdInfobox = new McdInfoF();
					McdInfobox.FormClosing += OnMcdInfoFormClosing;

					McdRecord record;
					int id;
					string label;

					Tilepart part = SelectedTilepart;
					if (part != null)
					{
						record = part.Record;
						id = part.TerId;
						label = GetTerrainLabel();
					}
					else
					{
						record = null;
						id = -1;
						label = String.Empty;
					}

					McdInfobox.UpdateData(record, id, label);
				}
				McdInfobox.Show();

				if (McdInfobox.WindowState == FormWindowState.Minimized)
					McdInfobox.WindowState  = FormWindowState.Normal;
			}
			else
				OnMcdInfoFormClosing(null, null);
		}

		/// <summary>
		/// Hides the <see cref="McdInfoF"/> dialog.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMcdInfoFormClosing(object sender, CancelEventArgs e)
		{
			foreach (var panel in _panels)
			{
				panel.ContextMenu.MenuItems[3].Checked = false;
			}

			if (e != null)			// if (e==null) the form is hiding due to a menu-click, or a double-click on a part
				e.Cancel = true;	// if (e!=null) the form really was closed, so cancel that.
									// NOTE: wtf - is way too complicated for what it is
			McdInfobox.Hide();
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
											Infobox.BoxType.Error))
					{
						f.ShowDialog(this);
					}
				}
				else
				{
					using (var fPckView = new PckViewF(true, GetSpriteshade()))
					{
						fPckView.SetType = PckViewF.Type.Pck;
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
											Infobox.BoxType.Error))
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
		/// Gets the current sprite-shade in <see cref="MainViewF.Optionables"/>.
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
				if (info != String.Empty) info += " and its ";
				info += "Routes";
			}

			if (info != String.Empty)
			{
				head += " You will be asked to save changes to the " + info + ".";
			}

			using (var f = new Infobox(
									"Reload tileset",
									Infobox.SplitString(head),
									null,
									Infobox.BoxType.Warn,
									Infobox.Buttons.CancelOkay))
			{
				return f.ShowDialog(this);
			}
		}

		/// <summary>
		/// An <see cref="Infobox"/> telling the user that the operation they
		/// are attempting is invalid because they haven't selected a tilepart.
		/// </summary>
		private void error_SelectTile()
		{
			using (var f = new Infobox(
									"Error",
									"Select a Tile.",
									null,
									Infobox.BoxType.Error))
			{
				f.ShowDialog(this);
			}
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
		/// Gets the panel of the currently displayed tabpage.
		/// </summary>
		/// <returns></returns>
		internal TilePanel GetVisiblePanel()
		{
			return _panels[tcTileTypes.SelectedIndex];
		}
		#endregion Methods
	}
}
