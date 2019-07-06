using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using DSShared;
using DSShared.Windows;

using MapView.Forms.MainWindow;
using MapView.Forms.MapObservers.TopViews;
using MapView.Forms.McdInfo;
using MapView.Volutar;

using McdView;

using PckView;

using XCom;
using XCom.Interfaces.Base;


namespace MapView.Forms.MapObservers.TileViews
{
	internal sealed partial class TileView
		:
			MapObserverControl // UserControl, IMapObserver
	{
		#region Events
		internal event TileSelectedEvent TileSelected_SelectQuadrant;

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
		public override MapFileBase MapBase
		{
			set
			{
				if ((base.MapBase = value) != null)
				{
					TileParts = value.Parts;
				}
				else
					TileParts = null;
			}
		}

		private IList<Tilepart> TileParts
		{
			set
			{
				for (int id = 0; id != _panels.Length; ++id)
					_panels[id].SetTiles(value);

				tsslTotal.Text = "Total " + value.Count;
				if (value.Count > MapFileBase.MaxTerrainId)
					tsslTotal.ForeColor = Color.MediumVioletRed;
				else
					tsslTotal.ForeColor = SystemColors.ControlText;

				OnResize(null);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal Tilepart SelectedTilepart
		{
			get { return _panels[tcTileTypes.SelectedIndex].PartSelected; }
			set
			{
				_allTiles.PartSelected = value;
				tcTileTypes.SelectedIndex = 0;

				Refresh();
			}
		}

		internal McdInfoF McdInfobox
		{ get; set; }
		#endregion Properties



		#region cTor
		/// <summary>
		/// cTor. Instantiates the TileView viewer and its pages/panels.
		/// </summary>
		internal TileView()
		{
			InitializeComponent();

			tcTileTypes.MouseWheel           += OnMouseWheel_Tabs;
			tcTileTypes.SelectedIndexChanged += OnSelectedIndexChanged;

			TilePanel.Chaparone = this;

			_allTiles      = new TilePanel(PartType.All);
			var floors     = new TilePanel(PartType.Floor);
			var westwalls  = new TilePanel(PartType.West);
			var northwalls = new TilePanel(PartType.North);
			var content    = new TilePanel(PartType.Content);

			tpFloors    .Text = QuadrantPanelDrawService.Floor;
			tpWestwalls .Text = QuadrantPanelDrawService.West;
			tpNorthwalls.Text = QuadrantPanelDrawService.North;
			tpContents  .Text = QuadrantPanelDrawService.Content;

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
		}

		private void AddPanel(TilePanel panel, Control page)
		{
			panel.TileSelected += OnTileSelected;
			page.Controls.Add(panel);
		}
		#endregion cTor


		#region Events (override)
		/// <summary>
		/// Bypasses level-change in MapObserverControl and scrolls through the
		/// tabpages instead.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseWheel(MouseEventArgs e)
		{
//			base.OnMouseWheel(e);
		}
		#endregion Events (override)


		#region Events
		private void OnMouseWheel_Tabs(object sender, MouseEventArgs e)
		{
			int dir = 0;
			if (e.Delta < 0)
				dir = +1;
			else if (e.Delta > 0)
				dir = -1;

			if (dir != 0)
			{
				int page = tcTileTypes.SelectedIndex + dir;
				if (page > -1 && page < tcTileTypes.TabCount)
				{
					tcTileTypes.SelectedIndex = page;
				}
			}
			//_panels[tcTileTypes.SelectedIndex] as TilePanel;
		}

		/// <summary>
		/// Triggers when a tab-index changes.
		/// Focuses the selected page/panel, updates the quadrant and MCD-info
		/// if applicable. And subscribes/unsubscribes panels to the static
		/// ticker's eventhandler.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnSelectedIndexChanged(object sender, EventArgs e)
		{
			var panel = GetSelectedPanel();
			foreach (var panel_ in _panels)
				panel_.SetTickerSubscription(panel_ == panel);


//			panel.Focus();

			McdRecord record;
			int id;
			string label;

			var f = FindForm();
			if (SelectedTilepart != null)
			{
				ViewerFormsManager.TopView     .Control   .SelectQuadrant(SelectedTilepart.Record.PartType);
				ViewerFormsManager.TopRouteView.ControlTop.SelectQuadrant(SelectedTilepart.Record.PartType);

				f.Text = BuildTitleString(SelectedTilepart.SetId, SelectedTilepart.TerId);
				record = SelectedTilepart.Record;
				id = SelectedTilepart.TerId;
				label = GetTerrainLabel();
			}
			else
			{
				f.Text = "TileView";
				record = null;
				id = -1;
				label = String.Empty;
			}

			if (McdInfobox != null)
				McdInfobox.UpdateData(record, id, label);
		}

		/// <summary>
		/// Triggers on the 'TileSelected' event. Further triggers the
		/// 'TileSelected_SelectQuadrant' event.
		/// </summary>
		/// <param name="part"></param>
		private void OnTileSelected(Tilepart part)
		{
			var f = FindForm();

			McdRecord record;
			int id;
			string label;

			if (part != null)
			{
				f.Text = BuildTitleString(part.SetId, part.TerId);
				record = part.Record;
				id = part.TerId;
				label = GetTerrainLabel();
			}
			else
			{
				f.Text = "TileView";
				record = null;
				id = -1;
				label = String.Empty;
			}

			if (McdInfobox != null)
				McdInfobox.UpdateData(record, id, label);

			if (TileSelected_SelectQuadrant != null)
				TileSelected_SelectQuadrant(part);
		}
		#endregion Events


		#region Options
		/// <summary>
		/// These are default colors for the SpecialProperty of a tilepart.
		/// TileView will load these colors when the app loads, then any colors
		/// of SpecialType that were customized will be set and accessed by
		/// TilePanel and/or the Help|Colors screen later.
		/// </summary>
		internal static readonly Color[] SpecialColors =
		{
									//      UFO:			TFTD:
			Color.NavajoWhite,		//  0 - Standard
			Color.Lavender,			//  1 - EntryPoint
			Color.IndianRed,		//  2 - PowerSource		IonBeamAccel
			Color.MediumTurquoise,	//  3 - Navigation
			Color.Khaki,			//  4 - Construction
			Color.MistyRose,		//  5 - Food			Cryo
			Color.Aquamarine,		//  6 - Reproduction	Clon
			Color.DeepSkyBlue,		//  7 - Entertainment	LearnArrays
			Color.Thistle,			//  8 - Surgery			Implant
			Color.YellowGreen,		//  9 - ExaminationRoom
			Color.MediumOrchid,		// 10 - Alloys			Plastics
			Color.LightSteelBlue,	// 11 - Habitat			Re-anim
			Color.Cyan,				// 12 - Destroyed
			Color.BurlyWood,		// 13 - ExitPoint
			Color.LightCoral		// 14 - MustDestroy
		};

		/// <summary>
		/// Loads default options for TileView screen.
		/// </summary>
		protected internal override void LoadControlOptions()
		{
			string desc;

			int i = -1;
			foreach (string key in Enum.GetNames(typeof(SpecialType)))
			{
//				int i = (int)Enum.Parse(typeof(SpecialType), key);
				TilePanel.SpecialBrushes[key] = new SolidBrush(SpecialColors[++i]);

				switch (i)
				{
					default:
					case  0: desc = "Color of Standard parts";                     break;
					case  1: desc = "Color of Entry Point parts";                  break;
					case  2: desc = "Color of UFO Power Source parts"            + Environment.NewLine
								  + "Color of TFTD Ion-beam Accelerators parts";   break;
					case  3: desc = "Color of UFO Navigation parts"              + Environment.NewLine
								  + "Color of TFTD Magnetic Navigation parts";     break;
					case  4: desc = "Color of UFO Construction parts"            + Environment.NewLine
								  + "Color of TFTD Alien Sub Construction parts";  break;
					case  5: desc = "Color of UFO Alien Food parts"              + Environment.NewLine
								  + "Color of TFTD Alien Cryogenics parts";        break;
					case  6: desc = "Color of UFO Alien Reproduction parts"      + Environment.NewLine
								  + "Color of TFTD Alien Cloning parts";           break;
					case  7: desc = "Color of UFO Alien Entertainment parts"     + Environment.NewLine
								  + "Color of TFTD Alien Learning Arrays parts";   break;
					case  8: desc = "Color of UFO Alien Surgery parts"           + Environment.NewLine
								  + "Color of TFTD Alien Implanter parts";         break;
					case  9: desc = "Color of Examination Room parts";             break;
					case 10: desc = "Color of UFO Alien Alloys parts"            + Environment.NewLine
								  + "Color of TFTD Aqua Plastics parts";           break;
					case 11: desc = "Color of UFO Alien Habitat parts"           + Environment.NewLine
								  + "Color of TFTD Alien Re-animation Zone parts"; break;
					case 12: desc = "Color of Destroyed Alloys/Plastics parts";    break;
					case 13: desc = "Color of Exit Point parts";                   break;
					case 14: desc = "Color of Must Destroy parts"                + Environment.NewLine
								  + "eg. UFO Alien Brain parts"                  + Environment.NewLine
								  + "eg. TFTD T'leth Power Cylinders parts";       break;
				}

				// NOTE: The colors of these brushes get overwritten by the
				// Option settings somewhere/how between here and their actual
				// use in TilePanel.OnPaint(). That is, this only sets default
				// colors and might not even be very useful other than as
				// perhaps for placeholder-key(s) for the actual values that
				// are later retrieved from Options ....
				//
				// See OnSpecialPropertyColorChanged() below_
				Options.AddOption(
								key,
								TilePanel.SpecialBrushes[key].Color,
								desc,						// descriptive hint at the bottom of the Options screen.
								"SpecialPropertyColors",	// Option category.
								OnSpecialPropertyColorChanged);
			}

			VolutarService.LoadOptions(Options);
		}

		/// <summary>
		/// Sets a different color for a SpecialBrush.
		/// </summary>
		/// <param name="key">a string representing the SpecialBrush</param>
		/// <param name="val">its color</param>
		private void OnSpecialPropertyColorChanged(string key, object val)
		{
			TilePanel.SpecialBrushes[key].Color = (Color)val;
			Invalidate();
		}


		private static Form _foptions;	// is static for no special reason

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
					_foptions = new OptionsForm("TileViewOptions", Options);
					_foptions.Text = " TileView Options";

					OptionsManager.Screens.Add(_foptions);

					_foptions.FormClosing += (sender1, e1) =>
					{
						if (!XCMainWindow.Quit)
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
		/// Opens the MCD-info screen.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void OnMcdInfoClick(object sender, EventArgs e)
		{
			if (!GetSelectedPanel().ContextMenu.MenuItems[3].Checked)
			{
				foreach (var panel in _panels)
				{
					panel.ContextMenu.MenuItems[3].Checked = true;
				}

				if (McdInfobox == null)
				{
					McdInfobox = new McdInfoF();
					McdInfobox.FormClosing += OnMcdInfoFormClosing;

					var f = FindForm();

					McdRecord record;
					int id;
					string label;

					var part = SelectedTilepart;
					if (part != null)
					{
						f.Text = BuildTitleString(part.SetId, part.TerId);
						record = part.Record;
						id = part.TerId;
						label = GetTerrainLabel();
					}
					else
					{
						f.Text = "TileView";
						record = null;
						id = -1;
						label = String.Empty;
					}

					McdInfobox.UpdateData(record, id, label);
				}
				McdInfobox.Show();
			}
			else
				OnMcdInfoFormClosing(null, null);
		}

		/// <summary>
		/// Hides the MCD-info screen.
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
			if ((MapBase as MapFile) != null)
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
		internal void OnPckEditClick(object sender, EventArgs e)
		{
			if (SelectedTilepart != null)
			{
				var terrain = ((MapFile)MapBase).GetTerrain(SelectedTilepart);

				string terr = terrain.Item1;
				string path = terrain.Item2;

				path = MapBase.Descriptor.GetTerrainDirectory(path);

				string pfePck = Path.Combine(path, terr + GlobalsXC.PckExt);
				string pfeTab = Path.Combine(path, terr + GlobalsXC.TabExt);

				if (!File.Exists(pfePck))
				{
					using (var f = new Infobox(" File not found", "File does not exist.", pfePck))
						f.ShowDialog(this);
				}
				else if (!File.Exists(pfeTab))
				{
					using (var f = new Infobox(" File not found", "File does not exist.", pfeTab))
						f.ShowDialog(this);
				}
				else
				{
					using (var fPckView = new PckViewForm(true))
					{
						fPckView.LoadSpriteset(pfePck);
						fPckView.SetPalette(MapBase.Descriptor.Pal.Label);
						fPckView.SetSelectedId(SelectedTilepart[0].Id);

						ShowHideManager.HideViewers();
						fPckView.ShowDialog(ViewerFormsManager.TileView); // <- Pause UI until PckView is closed.
						ShowHideManager.RestoreViewers();


						if (fPckView.FireMvReload					// the Descriptor needs to reload
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
		internal void OnMcdEditClick(object sender, EventArgs e)
		{
			if (SelectedTilepart != null)
			{
				var terrain = ((MapFile)MapBase).GetTerrain(SelectedTilepart);

				string terr = terrain.Item1;
				string path = terrain.Item2;

				path = MapBase.Descriptor.GetTerrainDirectory(path);

				string pfeMcd = Path.Combine(path, terr + GlobalsXC.McdExt);

				if (!File.Exists(pfeMcd))
				{
					using (var f = new Infobox(" File not found", "File does not exist.", pfeMcd))
						f.ShowDialog(this);
				}
				else
				{
					using (var fMcdView = new McdviewF(true))
					{
						Palette.UfoBattle .SetTransparent(false); // NOTE: McdView wants non-transparent palettes.
						Palette.TftdBattle.SetTransparent(false);

						fMcdView.LoadRecords(
										pfeMcd,
										MapBase.Descriptor.Pal.Label,
										SelectedTilepart.TerId);

						ShowHideManager.HideViewers();
						fMcdView.ShowDialog(ViewerFormsManager.TileView); // <- Pause UI until McdView is closed.
						ShowHideManager.RestoreViewers();

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
		DialogResult CheckReload()
		{
			string notice = "The Map needs to reload to show any"
						  + " changes that were made to the terrainset.";

			string changed = String.Empty;
			if (MapBase.MapChanged)
				changed = "Map";

			if (MapBase.RoutesChanged)
			{
				if (!String.IsNullOrEmpty(changed))
					changed += " and its ";

				changed += "Routes";
			}

			if (!String.IsNullOrEmpty(changed))
			{
				notice += Environment.NewLine + Environment.NewLine
						+ "You will be asked to save the current"
						+ " changes to the " + changed + ".";
			}

			return MessageBox.Show(
								this,
								notice,
								" Reload Map",
								MessageBoxButtons.OKCancel,
								MessageBoxIcon.Information,
								MessageBoxDefaultButton.Button1,
								0);
		}

		/// <summary>
		/// An errorbox telling the user that the operation they are attempting
		/// is invalid because they haven't selected a tilepart.
		/// </summary>
		void error_SelectTile()
		{
			MessageBox.Show(
						this,
						"Select a Tile.",
						" Error",
						MessageBoxButtons.OK,
						MessageBoxIcon.Asterisk,
						MessageBoxDefaultButton.Button1,
						0);
		}
		#endregion Events (menu)


		#region Methods
		/// <summary>
		/// Builds and returns a string that's appropriate for a currently
		/// selected tilepart.
		/// </summary>
		/// <param name="setId">the ID in total-terrains</param>
		/// <param name="terId">the ID in a terrain</param>
		/// <returns></returns>
		private string BuildTitleString(int setId, int terId)
		{
			return String.Format(
							System.Globalization.CultureInfo.CurrentCulture,
							"TileView - {2}  terId {1}  setId {0}",
							setId,
							terId,
							GetTerrainLabel());
		}

		/// <summary>
		/// Prints info for a mouseovered tilepart.
		/// </summary>
		/// <param name="part"></param>
		internal void StatbarOverInfo(Tilepart part)
		{
			string info = String.Empty;

			if (part != null)
			{
				string label = ((MapFile)MapBase).GetTerrainLabel(part);
				info = String.Format(
								System.Globalization.CultureInfo.CurrentCulture,
								"{2}  terId {1}  setId {0}",
								part.SetId,
								part.TerId,
								label);
			}
			tsslOver.Text = "Over " + info;
		}

		/// <summary>
		/// Gets the label of the terrain of the currently selected tilepart.
		/// </summary>
		/// <returns></returns>
		internal string GetTerrainLabel()
		{
			return (SelectedTilepart != null) ? ((MapFile)MapBase).GetTerrainLabel(SelectedTilepart)
											  : "ERROR";
		}

		/// <summary>
		/// Gets the panel of the currently displayed tabpage.
		/// </summary>
		/// <returns></returns>
		internal TilePanel GetSelectedPanel()
		{
			return _panels[tcTileTypes.SelectedIndex] as TilePanel;
		}
		#endregion Methods
	}


	internal class TileTabControl
		:
			TabControl
	{
		#region Events (override)
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.ExStyle |= 0x02000000; // enable 'WS_EX_COMPOSITED'
				return cp;
			}
		}
		#endregion Events (override)
	}
}
