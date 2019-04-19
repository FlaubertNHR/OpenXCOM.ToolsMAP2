using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using MapView.Forms.MainWindow;
using MapView.Forms.MapObservers.TopViews;
using MapView.Forms.McdInfo;
using MapView.OptionsServices;

using McdView;

using PckView;

using XCom;
using XCom.Interfaces.Base;

namespace MapView.Forms.MapObservers.TileViews
{
	internal sealed partial class TileView
		:
			MapObserverControl0
	{
		#region Events
		internal event TileSelectedEventHandler TileSelectedEvent_Observer0;

		/// <summary>
		/// Fires if a save was done in PckView (via TileView).
		/// </summary>
		internal event MethodInvoker PckSavedEvent;
		#endregion


		#region Fields
		private ShowHideManager _showHideManager;

		private TilePanel _allTiles;
		private TilePanel[] _panels;

		internal McdInfoF _mcdInfoForm;

		private Hashtable _brushesSpecial = new Hashtable();
		#endregion


		#region Properties
		public override MapFileBase MapBase
		{
			set
			{
				base.MapBase = value;
				TileParts = (value != null) ? value.Parts
											: null;
			}
		}

		private IList<TilepartBase> TileParts
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
		internal TilepartBase SelectedTilepart
		{
			get { return _panels[tcTileTypes.SelectedIndex].PartSelected; }
			set
			{
				_allTiles.PartSelected = value;
				tcTileTypes.SelectedIndex = 0;

				Refresh();
			}
		}
		#endregion



		#region cTor
		/// <summary>
		/// cTor. Instantiates the TileView viewer and its pages/panels.
		/// </summary>
		internal TileView()
		{
			InitializeComponent();

			tcTileTypes.MouseWheel           += OnMouseWheelTabs;
			tcTileTypes.SelectedIndexChanged += OnSelectedIndexChanged;

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
		#endregion


		private void AddPanel(TilePanel panel, Control page)
		{
			panel.TileSelectedEvent += OnTileSelected;
			page.Controls.Add(panel);
		}


		#region Events (override)
		/// <summary>
		/// Bypasses level-change in MapObserverControl0 and scrolls through the
		/// tabpages instead.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseWheel(MouseEventArgs e)
		{
//			base.OnMouseWheel(e);
		}
		#endregion Events (override)


		#region Events
		private void OnMouseWheelTabs(object sender, MouseEventArgs e)
		{
			LogFile.WriteLine("OnMouseWheelTabs() delta= " + e.Delta);
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
		/// Fires when a tab is clicked.
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
			{
				panel_.SetTickerSubscription(panel_ == panel);
			}

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

			if (_mcdInfoForm != null)
				_mcdInfoForm.UpdateData(record, id, label);
		}

		/// <summary>
		/// Fires when a tilepart is selected. Passes an event to
		/// 'TileSelectedEvent_Observer0'.
		/// </summary>
		/// <param name="part"></param>
		private void OnTileSelected(TilepartBase part)
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

			if (_mcdInfoForm != null)
				_mcdInfoForm.UpdateData(record, id, label);

			SelectQuadrant(part);
		}

		/// <summary>
		/// Changes the currently selected quadrant in the QuadrantPanel when
		/// a tilepart is selected in TileView.
		/// That is, fires
		///   TopView.Control.SelectQuadrant()
		/// and
		///   TopRouteView.ControlTop.SelectQuadrant()
		/// through 'TileSelectedEvent_Observer0'.
		/// </summary>
		/// <param name="part"></param>
		private void SelectQuadrant(TilepartBase part)
		{
			if (TileSelectedEvent_Observer0 != null)
				TileSelectedEvent_Observer0(part);
		}
		#endregion


		/// <summary>
		/// Sets the ShowHideManager.
		/// </summary>
		/// <param name="showHideManager"></param>
		internal void SetShowHideManager(ShowHideManager showHideManager)
		{
			_showHideManager = showHideManager;
		}

		/// <summary>
		/// These are default colors for the SpecialType of a tilepart.
		/// TileView will load these colors when the app loads, then any colors
		/// of SpecialType that were customized will be set and accessed by
		/// TilePanel and/or the Help|Colors screen later.
		/// </summary>
		internal static readonly Color[] TileColors =
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
		protected internal override void LoadControl0Options()
		{
			string desc = String.Empty;

			foreach (string type in Enum.GetNames(typeof(SpecialType)))
			{
				int i = (int)Enum.Parse(typeof(SpecialType), type);
				_brushesSpecial[type] = new SolidBrush(TileColors[i]);

				switch (i)
				{
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
								type,
								((SolidBrush)_brushesSpecial[type]).Color,
								desc,					// appears as a tip at the bottom of the Options screen.
								"TileBackgroundColors",	// this identifies what Option category the setting appears under.
								OnSpecialPropertyColorChanged);
			}
			TilePanel.SetSpecialPropertyBrushes(_brushesSpecial);

			VolutarSettingService.LoadOptions(Options);
		}

		/// <summary>
		/// Loads a different brush/color for a SpecialType into an already
		/// existing key.
		/// </summary>
		/// <param name="key">a string representing the SpecialType</param>
		/// <param name="val">the brush to insert</param>
		private void OnSpecialPropertyColorChanged(string key, object val)
		{
			((SolidBrush)_brushesSpecial[key]).Color = (Color)val;
			Refresh();
		}

		/// <summary>
		/// Gets the brushes/colors for all SpecialTypes.
		/// Used by the Help|Colors screen.
		/// </summary>
		/// <returns>a hashtable of the brushes</returns>
		internal Hashtable GetSpecialPropertyBrushes()
		{
			return _brushesSpecial;
		}


		private Form _foptions;
		private bool _closing; // wtf is this for like really

		/// <summary>
		/// Handles a click on the Options button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void OnOptionsClick(object sender, EventArgs e)
		{
			var tsb = sender as ToolStripButton;
			if (tsb.Checked = !tsb.Checked)
			{
				_foptions = new OptionsForm("TileViewOptions", Options);
				_foptions.Text = "TileView Options";

				_foptions.Show();

				_foptions.FormClosing += (sender1, e1) =>
				{
					if (!_closing)
						OnOptionsClick(sender, e);

					_closing = false;
				};
			}
			else
			{
				_closing = true;
				_foptions.Close();
			}
		}

		/// <summary>
		/// Gets the Options button on the toolstrip.
		/// </summary>
		/// <returns>the Options button</returns>
		internal ToolStripButton GetOptionsButton()
		{
			return tsb_Options;
		}


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

				if (_mcdInfoForm == null)
				{
					_mcdInfoForm = new McdInfoF();
					_mcdInfoForm.FormClosing += OnMcdInfoFormClosing;

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

					_mcdInfoForm.UpdateData(record, id, label);
				}
				_mcdInfoForm.Show();
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

			if (e != null)			// if (e==null) the form is hiding due to a menu-click, or a double-click on a tile
				e.Cancel = true;	// if (e!=null) the form really was closed, so cancel that.
									// NOTE: wtf - is way too complicated for what it is
			_mcdInfoForm.Hide();
		}

		/// <summary>
		/// Opens MCDEdit.exe or any program or file specified in Settings.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnVolutarMcdEditorClick(object sender, EventArgs e)
		{
			if ((MapBase as MapFileChild) != null)
			{
				var service = new VolutarSettingService(Options);
				var pfeService = service.FullPath;	// this will invoke a box for the user to input the
													// executable's path if it doesn't exist in Options.
				if (File.Exists(pfeService))
				{
					// change to MCDEdit dir so that accessing MCDEdit.txt doesn't cause probls.
					string dirService = Path.GetDirectoryName(pfeService);
					Directory.SetCurrentDirectory(dirService);

					Process.Start(new ProcessStartInfo(pfeService));

					// change back to app dir
					Directory.SetCurrentDirectory(SharedSpace.Instance.GetShare(SharedSpace.ApplicationDirectory));
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
				var terrain = ((MapFileChild)MapBase).GetTerrain(SelectedTilepart);

				string terr = terrain.Item1;
				string path = terrain.Item2;

				path = MapBase.Descriptor.GetTerrainDirectory(path);

				string pfePck = Path.Combine(path, terr + GlobalsXC.PckExt);
				string pfeTab = Path.Combine(path, terr + GlobalsXC.TabExt);

				if (!File.Exists(pfePck))
				{
					using (var f = new Infobox(" File not found", "File does not exist.", pfePck))
						f.ShowDialog();
				}
				else if (!File.Exists(pfeTab))
				{
					using (var f = new Infobox(" File not found", "File does not exist.", pfeTab))
						f.ShowDialog();
				}
				else
				{
					using (var fPckView = new PckViewForm())
					{
						fPckView.LoadSpriteset(pfePck);
						fPckView.SetPalette(MapBase.Descriptor.Pal.Label);
						fPckView.SetSelectedId(SelectedTilepart[0].Id);

						_showHideManager.HideViewers();
						fPckView.ShowDialog(FindForm()); // <- Pause until PckView is closed.
						_showHideManager.RestoreViewers();

						if (fPckView.SpritesChanged) // (re)load the selected Map.
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

							if (MessageBox.Show(
											this,
											notice,
											" Reload Map",
											MessageBoxButtons.OKCancel,
											MessageBoxIcon.Information,
											MessageBoxDefaultButton.Button1,
											0) == DialogResult.OK)
							{
								TriggerPckSaved();
							}
						}
					}
				}
			}
			else
				MessageBox.Show(
							this,
							"Select a Tile.",
							" Error",
							MessageBoxButtons.OK,
							MessageBoxIcon.Asterisk,
							MessageBoxDefaultButton.Button1,
							0);
		}

		/// <summary>
		/// Raised when a save is done in PckView.
		/// </summary>
		private void TriggerPckSaved()
		{
			if (PckSavedEvent != null)
				PckSavedEvent();
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
				var terrain = ((MapFileChild)MapBase).GetTerrain(SelectedTilepart);

				string terr = terrain.Item1;
				string path = terrain.Item2;

				path = MapBase.Descriptor.GetTerrainDirectory(path);

				string pfeMcd = Path.Combine(path, terr + GlobalsXC.McdExt);

				if (!File.Exists(pfeMcd))
				{
					using (var f = new Infobox(" File not found", "File does not exist.", pfeMcd))
						f.ShowDialog();
				}
				else
				{
					using (var fMcdView = new McdviewF())
					{
						Palette.UfoBattle .SetTransparent(false);
						Palette.TftdBattle.SetTransparent(false);
						fMcdView.LoadRecords(
										pfeMcd,
										MapBase.Descriptor.Pal.Label,
										SelectedTilepart.TerId);

						_showHideManager.HideViewers();
						fMcdView.ShowDialog(FindForm()); // <- Pause until McdView is closed.
						_showHideManager.RestoreViewers();
						Palette.UfoBattle .SetTransparent(true);
						Palette.TftdBattle.SetTransparent(true);


						if (fMcdView.RecordsChanged) // (re)load the selected Map.
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

							if (MessageBox.Show(
											this,
											notice,
											" Reload Map",
											MessageBoxButtons.OKCancel,
											MessageBoxIcon.Information,
											MessageBoxDefaultButton.Button1,
											0) == DialogResult.OK)
							{
								TriggerPckSaved();
							}
						}
					}
				}
			}
			else
				MessageBox.Show(
							this,
							"Select a Tile.",
							" Error",
							MessageBoxButtons.OK,
							MessageBoxIcon.Asterisk,
							MessageBoxDefaultButton.Button1,
							0);
		}


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
		internal void StatusbarOverInfo(TilepartBase part)
		{
			string info = String.Empty;

			if (part != null)
			{
				string label = ((MapFileChild)MapBase).GetTerrainLabel(part);
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
			return (SelectedTilepart != null) ? ((MapFileChild)MapBase).GetTerrainLabel(SelectedTilepart)
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
		#endregion
	}
}
