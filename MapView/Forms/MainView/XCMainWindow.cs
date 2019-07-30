using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
#if !__MonoCS__
using System.Runtime.InteropServices;
#endif
using System.Windows.Forms;

using DSShared;
using DSShared.Controls;

using MapView.Forms.MainView;
using MapView.Forms.Observers.RouteViews;
using MapView.Forms.Observers.TopViews;

using XCom;
using XCom.Interfaces;
using XCom.Interfaces.Base;
using XCom.Resources.Map.RouteData;

using YamlDotNet.RepresentationModel; // read values (deserialization)


namespace MapView
{
	/// <summary>
	/// Instantiates a MainView screen as the basis for all user-interaction.
	/// </summary>
	internal sealed partial class XCMainWindow
		:
			Form
	{
		#region Events
		internal event DontBeepEventHandler DontBeepEvent;
		#endregion


		#region Fields (static)
		private const string TITLE = "Map Editor ||";

		private const double ScaleDelta = 0.125;
		#endregion Fields (static)


		#region Fields
		private CompositedTreeView MapTree;

		internal Options Options;
		#endregion Fields


		#region Properties (static)
		internal static XCMainWindow that
		{ get; private set; }

		/// <summary>
		/// A class-object that holds MainView's optionable Properties.
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
		/// I created an independent class just to hold and handle MainView's
		/// Optionable Properties ... and wired it up. It's a tedious shitfest
		/// but better than the arcane MapViewI system or screwing around with
		/// equally arcane TypeDescriptors. Both of which had been implemented
		/// but then rejected.
		/// </summary>
		internal static MainViewOptionables Optionables
		{ get; private set; }

		/// <summary>
		/// Allows MainView and all its subsidiary forms to close, otherwise
		/// usually only hide. This boolean gets flagged true only by MainView's
		/// FormClosing event-handler. But it shall be checked by the
		/// FormClosing event-handlers of any subsidiary form that should only
		/// hide unless MainView itself is closing and therefore flagged to
		/// Quit. If the flag is still false, 'e.Cancel' any subsidiary form
		/// from closing in its FormClosing event-handler.
		/// </summary>
		internal static bool Quit
		{ get; private set; }

		internal static ScanGViewer ScanG
		{ get; set; }
		#endregion Properties (static)


		#region Properties
		private MainViewUnderlay MainViewUnderlay
		{ get; set; }

		internal MainViewOverlay MainViewOverlay
		{ private get; set; }

		private bool _treeChanged;
		/// <summary>
		/// Gets/Sets the MaptreeChanged flag.
		/// </summary>
		internal bool MaptreeChanged
		{
			get { return _treeChanged; }
			set
			{
				if (_treeChanged = value)
				{
					if (!Text.EndsWith("*", StringComparison.OrdinalIgnoreCase))
						Text += "*";
				}
				else if (Text.EndsWith("*", StringComparison.OrdinalIgnoreCase))
					Text = Text.Substring(0, Text.Length - 1);
			}
		}

		/// <summary>
		/// Sets the MapChanged flag. This is only an intermediary that adds an
		/// asterisk to the file-label in MainView's statusbar; the real
		/// MapChanged flag is stored in XCom..MapFileBase. reasons.
		/// </summary>
		internal bool MapChanged
		{
			set
			{
				string text = tsslMapLabel.Text;
				if (MainViewUnderlay.MapBase.MapChanged = value) // shuffle the value down to MapFileBase.MapChanged ...
				{
					if (!text.EndsWith("*", StringComparison.OrdinalIgnoreCase))
						text += "*";
				}
				else if (text.EndsWith("*", StringComparison.OrdinalIgnoreCase))
					text = text.Substring(0, text.Length - 1);

				tsslMapLabel.Text = text;
			}
		}

		/// <summary>
		/// The currently searched and found and highlighted Treenode on the
		/// MapTree.
		/// </summary>
		private TreeNode Searched
		{ get; set; }
		#endregion Properties


		#region cTor
		/// <summary>
		/// This is where the user-app end of things *really* starts.
		/// </summary>
		internal XCMainWindow()
		{
			string dirAppL = Path.GetDirectoryName(Application.ExecutablePath);
			string dirSetT = Path.Combine(dirAppL, PathInfo.SettingsDirectory);
#if DEBUG
			LogFile.SetLogFilePath(dirAppL); // creates a logfile/ wipes the old one.
			DSLogFile.CreateLogFile();
#endif

			LogFile.WriteLine("Starting MAIN MapView window ...");

			SharedSpace.SetShare(SharedSpace.ApplicationDirectory, dirAppL); // why. -> use Application.ExecutablePath
			SharedSpace.SetShare(SharedSpace.SettingsDirectory,    dirSetT);

			LogFile.WriteLine("App paths cached.");


			var pathOptions   = new PathInfo(dirSetT, PathInfo.ConfigOptions);
			var pathResources = new PathInfo(dirSetT, PathInfo.ConfigResources);
			var pathTilesets  = new PathInfo(dirSetT, PathInfo.ConfigTilesets);
			var pathViewers   = new PathInfo(dirSetT, PathInfo.ConfigViewers);

			SharedSpace.SetShare(PathInfo.ShareOptions,   pathOptions);
			SharedSpace.SetShare(PathInfo.ShareResources, pathResources);
			SharedSpace.SetShare(PathInfo.ShareTilesets,  pathTilesets);
			SharedSpace.SetShare(PathInfo.ShareViewers,   pathViewers);

			LogFile.WriteLine("PathInfo cached.");


			// Check if MapTilesets.yml and MapResources.yml exist yet, show the
			// Configuration window if not.
			// NOTE: MapResources.yml and MapTilesets.yml are created by ConfigurationForm.
			if (!pathResources.FileExists() || !pathTilesets.FileExists())
			{
				LogFile.WriteLine("Resources or Tilesets file does not exist: run configurator.");

				using (var f = new ConfigurationForm())
					f.ShowDialog(this);
			}
			else
				LogFile.WriteLine("Resources and Tilesets files exist.");


			// Exit app if either MapResources.yml or MapTilesets.yml doesn't exist
			if (!pathResources.FileExists() || !pathTilesets.FileExists()) // safety. The Configurator shall demand that both these files get created.
			{
				LogFile.WriteLine("Resources or Tilesets file does not exist: quit MapView.");
				Environment.Exit(0);
			}


			// Check if MapViewers.yml exists yet, if not create it
			if (!pathViewers.FileExists())
			{
				CreateViewersFile();
				LogFile.WriteLine("Viewers file created.");
			}
			else
				LogFile.WriteLine("Viewers file exists.");



			InitializeComponent();
			LogFile.WriteLine("MainView initialized.");


			var splitter = new CollapsibleSplitter();
			Controls.Add(splitter);

			MapTree = new CompositedTreeView();

			MapTree.Name          = "MapTree";
			MapTree.Dock          = DockStyle.Left;
			MapTree.DrawMode      = TreeViewDrawMode.OwnerDrawText;
			MapTree.ForeColor     = SystemColors.ControlText;
			MapTree.BackColor     = SystemColors.Control;
			MapTree.Indent        = 15;
			MapTree.Location      = new Point(0, 0);
			MapTree.Size          = new Size(240, 454);
			MapTree.Margin        = new Padding(0);
			MapTree.TabIndex      = 0;
			MapTree.HideSelection = false;

			MapTree.DrawNode       += tv_DrawNode;
			MapTree.GotFocus       += OnMapTreeFocusChanged;
			MapTree.LostFocus      += OnMapTreeFocusChanged;
			MapTree.MouseDown      += OnMapTreeMouseDown;
			MapTree.BeforeSelect   += OnMapTreeBeforeSelect;
			MapTree.AfterSelect    += OnMapTreeAfterSelected;
			MapTree.NodeMouseClick += OnMapTreeNodeClick;
//			MapTree.NodeMouseClick += (sender, args) => MapTree.SelectedNode = args.Node;

			Controls.Add(MapTree);
			splitter.Collapsible = MapTree;


			// WORKAROUND: The size of the form in the designer keeps increasing
			// (for whatever reason) based on the
			// 'SystemInformation.CaptionButtonSize.Height' value (the titlebar
			// height - note that 'SystemInformation.CaptionHeight' seems to be
			// 1 pixel larger, which (for whatever reason) is not the
			// 'SystemInformation.BorderSize.Height'). To prevent all that, in
			// the designer cap the form's Size by setting its MaximumSize to
			// the Size - but now run this code that sets the MaximumSize to
			// "0,0" (unlimited, as wanted). And, as a safety, do the same thing
			// with MinimumSize ... if desired.
			//
			// - observed & tested in SharpDevelop 5.1
			//
			// NOTE: This code (the constructor of a Form) shouldn't run when
			// opening the designer; it appears to run only when actually
			// running the application:
			// NOTE: Mono users have to restart the app for the window to be
			// resizable.

			MaximumSize =
			MinimumSize = new Size(0,0); // fu.net


			that = this;

			MainViewUnderlay = new MainViewUnderlay(this);
			LogFile.WriteLine("MainView panels instantiated.");

			Optionables = new MainViewOptionables(MainViewOverlay);
			LogFile.WriteLine("MainView optionables initialized.");

			RegistryInfo.InitializeRegistry(dirAppL);
			LogFile.WriteLine("Registry initialized.");
			RegistryInfo.RegisterProperties(this);
			LogFile.WriteLine("MainView registered.");

			Options.InitializeConverters();
			LogFile.WriteLine("OptionsConverters initialized.");
			Option.InitializeParsers();
			LogFile.WriteLine("OptionParsers initialized.");

			Options = new Options();
			OptionsManager.setOptionsType(RegistryInfo.MainView, Options);

			LoadDefaultOptions();									// TODO: check if this should go after the managers load
			LogFile.WriteLine("MainView Default Options loaded.");	// since managers might be re-instantiating needlessly
																	// when OnOptionsClick() runs ....


			Palette.UfoBattle .SetTransparent(true);
			Palette.TftdBattle.SetTransparent(true);
			Palette.UfoBattle .Grayscale.SetTransparent(true);
			Palette.TftdBattle.Grayscale.SetTransparent(true);
			LogFile.WriteLine("Palette transparencies set.");

			Globals.LoadExtraSprites();	// sprites for TileView's eraser and QuadrantPanel's blank quads.
										// NOTE: transparency of the 'UfoBattle' palette must be set first.


			QuadrantDrawService.Punkstrings();
			LogFile.WriteLine("Quadrant strings punked.");


			ObserverManager.Initialize(); // adds each subsidiary viewer's options and Options-type etc.
			LogFile.WriteLine("ObserverManager initialized.");

			ObserverManager.TileView.Control.ReloadDescriptor += OnReloadDescriptor;


			MapTree.TreeViewNodeSorter = StringComparer.OrdinalIgnoreCase;

			tscPanel.ContentPanel.Controls.Add(MainViewUnderlay);

			tsTools.SuspendLayout();
			ObserverManager.ToolFactory.CreateToolstripSearchObjects(tsTools);
			ObserverManager.ToolFactory.CreateToolstripScaleObjects(tsTools);
			ObserverManager.ToolFactory.CreateToolstripEditorObjects(tsTools);
			tsTools.ResumeLayout();
			LogFile.WriteLine("MainView toolstrip created.");

			MenuManager.Initialize(menuViewers);
			MenuManager.PopulateMenu();
			LogFile.WriteLine("Viewers menu populated.");


			// Read MapResources.yml to get the resources dir (for both UFO and TFTD).
			// NOTE: MapResources.yml is created by ConfigurationForm
			using (var sr = new StreamReader(File.OpenRead(pathResources.Fullpath)))
			{
				var str = new YamlStream();
				str.Load(sr);

				string key = null;
				string val = null;

				var nodeRoot = str.Documents[0].RootNode as YamlMappingNode;
				foreach (var node in nodeRoot.Children)
				{
					switch (node.Key.ToString())
					{
						case "ufo":
							key = SharedSpace.ResourceDirectoryUfo;
							break;
						case "tftd":
							key = SharedSpace.ResourceDirectoryTftd;
							break;
					}

					val = node.Value.ToString();
					val = (!val.Equals(PathInfo.NotConfigured)) ? val : null;

					SharedSpace.SetShare(key, val);
				}
			}


			// Setup an XCOM cursor-sprite.
			// NOTE: This is the only stock XCOM resource that is required for
			// MapView to start. See ConfigurationForm ...
			// TODO: give user the option to choose which cursor-spriteset to use.
			var cuboid = ResourceInfo.LoadSpriteset(
												SharedSpace.CursorFilePrefix,
												SharedSpace.GetShareString(SharedSpace.ResourceDirectoryUfo),
												ResourceInfo.TAB_WORD_LENGTH_2,
												Palette.UfoBattle);
			if (cuboid != null)
			{
				CuboidSprite.Cursorset = cuboid;
				LogFile.WriteLine("UFO Cursor loaded.");
			}
			else
				LogFile.WriteLine("UFO Cursor not found.");

			// NOTE: The TFTD cursorsprite takes precedence over the UFO cursorsprite.
			cuboid = ResourceInfo.LoadSpriteset(
											SharedSpace.CursorFilePrefix,
											SharedSpace.GetShareString(SharedSpace.ResourceDirectoryTftd),
											ResourceInfo.TAB_WORD_LENGTH_4,
											Palette.TftdBattle);
			if (cuboid != null)
			{
				CuboidSprite.Cursorset = cuboid;
				LogFile.WriteLine("TFTD Cursor loaded.");
			}
			else
				LogFile.WriteLine("TFTD Cursor not found.");


			if (ResourceInfo.LoadScanGufo(SharedSpace.GetShareString(SharedSpace.ResourceDirectoryUfo)))
				LogFile.WriteLine("ScanG UFO loaded.");
			else
				LogFile.WriteLine("ScanG UFO not found.");

			if (ResourceInfo.LoadScanGtftd(SharedSpace.GetShareString(SharedSpace.ResourceDirectoryTftd)))
				LogFile.WriteLine("ScanG TFTD loaded.");
			else
				LogFile.WriteLine("ScanG TFTD not found.");


			ResourceInfo.InitializeResources(pathTilesets); // load resources from YAML.
			LogFile.WriteLine("ResourceInfo initialized.");


			if (pathOptions.FileExists()) // load user-options before MenuManager.StartSecondaryStage()
			{
				OptionsManager.LoadUserOptions(pathOptions.Fullpath);
				LogFile.WriteLine("User options loaded.");
			}
			else
				LogFile.WriteLine("User options NOT loaded - no options file to load.");


			CreateTree();
			LogFile.WriteLine("Tilesets created and loaded to tree panel.");


			splitter.SetClickableRectangle();
			ShiftSplitter();


			DontBeepEvent += FireContext;

			var r = new CustomToolStripRenderer();
			ssMain.Renderer = r;


			LogFile.WriteLine("About to show MainView ..." + Environment.NewLine);
		}
		#endregion cTor


		#region Methods (static)
		/// <summary>
		/// Transposes all the default viewer positions and sizes from the
		/// embedded MapViewers manifest to "/settings/MapViewers.yml".
		/// Based on 'ConfigurationForm'.
		/// </summary>
		private static void CreateViewersFile()
		{
			var info = SharedSpace.GetShareObject(PathInfo.ShareViewers) as PathInfo;
			info.CreateDirectory();

			string pfe = info.Fullpath;

			using (var sr = new StreamReader(Assembly.GetExecutingAssembly()
													 .GetManifestResourceStream("MapView._Embedded.MapViewers.yml")))
			using (var fs = new FileStream(pfe, FileMode.Create))
			using (var sw = new StreamWriter(fs))
			{
				while (sr.Peek() != -1)
					sw.WriteLine(sr.ReadLine());
			}
		}
		#endregion Methods (static)


		#region Create tree
		/// <summary>
		/// Creates the Map-tree on the left side of MainView.
		/// </summary>
		private void CreateTree()
		{
			//LogFile.WriteLine("");
			//LogFile.WriteLine("XCMainWindow.CreateTree");

			MapTree.BeginUpdate();
			MapTree.Nodes.Clear();

			var groups = ResourceInfo.TileGroupManager.TileGroups;
			//LogFile.WriteLine(". groups= " + groups);

			SortableTreeNode nodeGroup;
			TileGroupBase tileGroup;

			SortableTreeNode nodeCategory;

			SortableTreeNode nodeTileset;
			Dictionary<string, Descriptor> descriptors;


			foreach (string keyGroup in groups.Keys)
			{
				//LogFile.WriteLine(". . keyGroup= " + keyGroup);

				tileGroup = groups[keyGroup];

				nodeGroup = new SortableTreeNode(tileGroup.Label);
				nodeGroup.Tag = tileGroup;
				MapTree.Nodes.Add(nodeGroup);

				foreach (string keyCategory in tileGroup.Categories.Keys)
				{
					//LogFile.WriteLine(". . . keyCategory= " + keyCategory);

					nodeCategory = new SortableTreeNode(keyCategory);
					descriptors = tileGroup.Categories[keyCategory];
					nodeCategory.Tag = descriptors;
					nodeGroup.Nodes.Add(nodeCategory);

					foreach (string keyTileset in descriptors.Keys)
					{
						//LogFile.WriteLine(". . . . keyTileset= " + keyTileset);

						nodeTileset = new SortableTreeNode(keyTileset);
						nodeTileset.Tag = descriptors[keyTileset];
						nodeCategory.Nodes.Add(nodeTileset);
					}
				}
			}
			MapTree.EndUpdate();
		}

		/// <summary>
		/// A functor that sorts tree-nodes.
		/// </summary>
		private sealed class SortableTreeNode
			:
				TreeNode,
				IComparable
		{
			public SortableTreeNode(string text)
				:
					base(text)
			{}

			public int CompareTo(object other)
			{
				var node = other as SortableTreeNode;
				return (node != null) ? String.CompareOrdinal(Text, node.Text)
									  : -1;
			}
		}

		/// <summary>
		/// Shifts the splitter between the MapTree and the MapPanel to ensure
		/// that the longest tree-node's Text gets fully displayed.
		/// </summary>
		private void ShiftSplitter()
		{
			int width = MapTree.Width, widthTest;

			foreach (TreeNode node0 in MapTree.Nodes)
			{
				widthTest = TextRenderer.MeasureText(node0.Text, MapTree.Font).Width + 18;
				if (widthTest > width)
					width = widthTest;

				foreach (TreeNode node1 in node0.Nodes)
				{
					widthTest = TextRenderer.MeasureText(node1.Text, MapTree.Font).Width + 36;
					if (widthTest > width)
						width = widthTest;

					foreach (TreeNode node2 in node1.Nodes)
					{
						widthTest = TextRenderer.MeasureText(node2.Text, MapTree.Font).Width + 54;
						if (widthTest > width)
							width = widthTest;
					}
				}
			}
			MapTree.Width = width;
		}
		#endregion Create tree


		#region Options
		/// <summary>
		/// Loads user-settings into MainView's Options screen.
		/// </summary>
		private void LoadDefaultOptions()
		{
			// kL_note: This is for retrieving MainView's location and size from
			// the Windows Registry:
//			using (var keySoftware = Registry.CurrentUser.CreateSubKey(DSShared.Windows.RegistryInfo.SoftwareRegistry))
//			using (var keyMapView  = keySoftware.CreateSubKey(DSShared.Windows.RegistryInfo.MapViewRegistry))
//			using (var keyMainView = keyMapView.CreateSubKey("MainView"))
//			{
//				Left   = (int)keyMainView.GetValue("Left",   Left);
//				Top    = (int)keyMainView.GetValue("Top",    Top);
//				Width  = (int)keyMainView.GetValue("Width",  Width);
//				Height = (int)keyMainView.GetValue("Height", Height);
//				keyMainView.Close();
//				keyMapView.Close();
//				keySoftware.Close();
//			}

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
			var it = sender as MenuItem;
			if (it.Checked = !it.Checked)
			{
				if (_foptions == null)
				{
					_foptions = new OptionsForm(
											Optionables,
											Options,
											OptionsForm.OptionableType.MainView);
					_foptions.Text = "MainView Options";

					OptionsManager.Views.Add(_foptions);

					_foptions.FormClosing += (sender1, e1) =>
					{
						if (!XCMainWindow.Quit)
						{
							it.Checked = false;

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
		#endregion Options


		#region Events (override)
		/// <summary>
		/// This has nothing to do with the Registry anymore, but it saves
		/// MainView's Options as well as its screen-location and -size to YAML
		/// when the app closes.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			Quit = true;
			e.Cancel = false;

			if (SaveAlertMap() == DialogResult.Cancel) // NOTE: do not short-circuit these ->
			{
				Quit = false;
				e.Cancel = true;
			}

			if (SaveAlertRoutes() == DialogResult.Cancel)
			{
				Quit = false;
				e.Cancel = true;
			}

			if (SaveAlertMaptree() == DialogResult.Cancel)
			{
				Quit = false;
				e.Cancel = true;
			}

			if (Quit)
			{
				OptionsManager.SaveOptions(); // save MV_OptionsFile // TODO: do SaveOptions() every time an Options form closes.

				ObserverManager.CloseViewers();
				OptionsManager .CloseOptions();

				if (ScanG    != null) ScanG   .Close();
				if (_fcolors != null) _fcolors.Close();
				if (_fabout  != null) _fabout .Close();
				if (_finfo   != null) _finfo  .Close();

				if (ObserverManager.TileView.Control.McdInfobox != null)
					ObserverManager.TileView.Control.McdInfobox.Close();

				RegistryInfo.UpdateRegistry(this);
				RegistryInfo.FinalizeRegistry();

				// kL_note: This is for storing MainView's location and size in
				// the Windows Registry:
//				if (PathsEditor.SaveRegistry)
//				{
//					using (var keySoftware = Registry.CurrentUser.CreateSubKey(DSShared.Windows.RegistryInfo.SoftwareRegistry))
//					using (var keyMapView = keySoftware.CreateSubKey(DSShared.Windows.RegistryInfo.MapViewRegistry))
//					using (var keyMainView = keyMapView.CreateSubKey("MainView"))
//					{
//						_mainViewsManager.CloseAllViewers();
//						WindowState = FormWindowState.Normal;
//						keyMainView.SetValue("Left",   Left);
//						keyMainView.SetValue("Top",    Top);
//						keyMainView.SetValue("Width",  Width);
//						keyMainView.SetValue("Height", Height - SystemInformation.CaptionButtonSize.Height); ps. not
//						keyMainView.Close();
//						keyMapView.Close();
//						keySoftware.Close();
//					}
//				}
			}

			base.OnFormClosing(e);
		}


		private static bool Inited; // on 1st Activated keep the tree focused, on 2+ focus the panel
		internal static bool BypassActivatedEvent;

		/// <summary>
		/// Overrides the Activated event. Brings any other open viewers to the
		/// top of the desktop, along with this. And focuses the panel.
		/// IMPORTANT: trying to bring this form to the top after the other
		/// forms apparently fails in Windows 10 - which makes it impossible for
		/// MainView to gain focus when clicked (if there are other viewers
		/// open). Hence MainView's option "AllowBringToFront" is FALSE by
		/// default.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnActivated(EventArgs e)
		{
			ShowHideManager._zOrder.Remove(this);
			ShowHideManager._zOrder.Add(this);

			if (XCMainWindow.Optionables.BringAllToFront)
			{
				if (!BypassActivatedEvent)			// don't let 'TopMost_set' (etc) fire the OnActivated event.
				{
					BypassActivatedEvent = true;	// don't let the loop over the viewers re-trigger this activated event.
													// NOTE: 'TopMost_set' won't, but other calls like BringToFront() or Select() can/will.

					var zOrder = ShowHideManager.getZorderList();
					foreach (var f in zOrder)
					{
						f.TopMost = true;
						f.TopMost = false;
					}

					BypassActivatedEvent = false;
				}
			}

			if (Inited)
				MainViewOverlay.Focus();
			else
				Inited = true;

//			base.OnActivated(e);
		}

		/// <summary>
		/// Overrides the Deactivated event. Allows the targeter to go away.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnDeactivate(EventArgs e)
		{
			MainViewOverlay._targeterForced = false;
			Invalidate();

//			base.OnDeactivate(e);
		}


		/// <summary>
		/// Stops keys that shall be used for navigating the tiles from doing
		/// anything stupid. Does other stupid stuff instead.
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="keyData"></param>
		/// <returns></returns>
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			//LogFile.WriteLine("ProcessCmdKey() " + keyData);

			bool invalidate  = true;
			bool focussearch = false;
			switch (keyData)
			{
				case Keys.Tab:
					focussearch = MainViewOverlay.Focused;
					break;

				case Keys.Shift | Keys.Tab:
					focussearch = MapTree.Focused;
					break;

				case Keys.Shift | Keys.F3:
					focussearch = true;
					break;

				default:
					invalidate = false;
					break;
			}

			if (invalidate)
			{
				MainViewOverlay.Invalidate();

				if (focussearch)
				{
					ObserverManager.ToolFactory.FocusSearch();
					return true;
				}
			}


			if (MainViewOverlay.Focused)
			{
				switch (keyData)
				{
					case Keys.Left:
					case Keys.Right:
					case Keys.Up:
					case Keys.Down:
					case Keys.Shift | Keys.Left:
					case Keys.Shift | Keys.Right:
					case Keys.Shift | Keys.Up:
					case Keys.Shift | Keys.Down:
						MainViewOverlay.Navigate(keyData);
						return true;
				}
			}
			else
			{
				switch (keyData)
				{
					case Keys.F3:		// panel must *not* have focus (F3 also toggles doors)
						Search(ObserverManager.ToolFactory.GetSearchText());
						return true;

					case Keys.Escape:	// panel must *not* have focus (Escape also cancels multi-tile selection)
						MainViewOverlay.Focus();
						MainViewOverlay.Invalidate();
						return true;
				}
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}

		/// <summary>
		/// Handles key-down event at the Form level.
		/// Requires 'KeyPreview' true.
		/// @note This differs from the Viewers-menu item-click/key in that
		/// [Ctrl] is used to focus a subsidiary viewer instead of doing
		/// show/hide.
		/// Cf OnKeyDown() in TileViewForm, TopViewForm, RouteViewForm, and
		/// TopRouteViewForm -> ViewerKeyDown().
		/// @note Edit/Save keys are handled by 'MainViewOverlay.OnKeyDown()'.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			//LogFile.WriteLine("OnKeyDown() " + e.KeyCode);

			string key; object val = null;

			switch (e.KeyCode)
			{
				case Keys.Enter: // do this here to get rid of the beep.
					if (MapTree.Focused && _selected != null)
					{
						e.SuppressKeyPress = true;
						_dontbeep1 = !e.Shift;
						BeginInvoke(DontBeepEvent);
					}
					break;

				case Keys.F2:
					key = MainViewOptionables.str_AnimateSprites;
					val = !XCMainWindow.Optionables.AnimateSprites;
					Options[key].Value = val;
					Optionables.OnSpriteStateChanged(key,val);
					break;

				case Keys.F3:
					key = MainViewOptionables.str_OpenDoors;
					val = !XCMainWindow.Optionables.OpenDoors;
					Options[key].Value = val;
					Optionables.OnSpriteStateChanged(key,val);
					break;

				case Keys.F4:
					key = MainViewOptionables.str_GridVisible;
					val = !XCMainWindow.Optionables.GridVisible;
					Options[key].Value = val;
					Optionables.OnOptionChanged(key,val);
					break;

				default:
					if (e.Control)
					{
						if (menuViewers.Enabled)
						{
							ToolStripMenuItem part = null;
							int it = -1;

							switch (e.KeyCode)
							{
								// toggle TopView tilepart visibilities ->
								case Keys.F1: part = ObserverManager.TopView.Control.Floor;   break;
								case Keys.F2: part = ObserverManager.TopView.Control.West;    break;
								case Keys.F3: part = ObserverManager.TopView.Control.North;   break;
								case Keys.F4: part = ObserverManager.TopView.Control.Content; break;

								// show/hide viewer ->
								case Keys.F5: it = 0; break;
								case Keys.F6: it = 2; break;
								case Keys.F7: it = 3; break;
								case Keys.F8: it = 4; break;
							}

							if (it != -1)
							{
								e.SuppressKeyPress = true;
								MenuManager.OnMenuItemClick(
														menuViewers.MenuItems[it],
														EventArgs.Empty);
							}
							else if (part != null)
							{
								e.SuppressKeyPress = true;
								ObserverManager.TopView.Control.OnQuadrantVisibilityClick(part, EventArgs.Empty);
							}
						}
					}
					else if (MainViewOverlay.Focused)
					{
						switch (e.KeyCode)
						{
							case Keys.Add:
							case Keys.Subtract:
							case Keys.PageDown:
							case Keys.PageUp:
							case Keys.Home:
							case Keys.End:
								e.SuppressKeyPress = true;
								MainViewOverlay.Navigate(e.KeyData);
								break;
						}
					}
					break;
			}

			if (val != null && _foptions != null && _foptions.Visible)
				(_foptions as OptionsForm).propertyGrid.Refresh();

			base.OnKeyDown(e);
		}
		#endregion Events (override)


		#region Events
		private void tv_DrawNode(object sender, DrawTreeNodeEventArgs e)
		{
			if (e.Node != null)
			{
				var graphics = e.Graphics;

				Brush brush;
				Pen pen;

				if ((e.State & TreeNodeStates.Focused) == TreeNodeStates.Focused) // WARNING: May require 'HideSelection' false.
				{
					brush = Brushes.BurlyWood;
					pen   = Pens.SlateBlue;
				}
				else if ((e.State & TreeNodeStates.Selected) == TreeNodeStates.Selected) // WARNING: Requires 'HideSelection' false.
				{
					brush = Brushes.Wheat;
					pen   = Pens.SlateBlue;
				}
				else if (e.Node == Searched)
				{
					pen = Pens.SlateBlue;

					if (MapTree.Focused)
						brush = Brushes.SkyBlue;
					else
						brush = Brushes.PowderBlue;
				}
				else
				{
//					e.DrawDefault = true;
					brush = SystemBrushes.Control;
					pen   = SystemPens.Control;
				}

				Rectangle rect = e.Bounds;
				rect.Width += 4;							// conceal .NET glitch.
				graphics.FillRectangle(brush, rect);
				rect.Height -= 1;							// keep border inside bounds
				graphics.DrawRectangle(pen, rect);

				rect = e.Bounds;
				rect.X += 2;								// re-align text due to .NET glitch.
				TextRenderer.DrawText(
									graphics,
									e.Node.Text,
									e.Node.TreeView.Font,	//e.Node.NodeFont ?? e.Node.TreeView.Font
									rect,
									SystemColors.ControlText);
			}
		}


		private void OnSaveAllClick(object sender, EventArgs e)
		{
			if (MainViewUnderlay.MapBase != null)
			{
				MainViewUnderlay.MapBase.SaveMap();
				MainViewUnderlay.MapBase.SaveRoutes();

				MapChanged =
				ObserverManager.RouteView   .Control     .RoutesChanged =
				ObserverManager.TopRouteView.ControlRoute.RoutesChanged = false;
			}
			MaptreeChanged = !ResourceInfo.TileGroupManager.SaveTileGroups();
		}

		internal void OnSaveMapClick(object sender, EventArgs e)
		{
			if (MainViewUnderlay.MapBase != null)
			{
				MainViewUnderlay.MapBase.SaveMap();
				MapChanged = false;
			}
		}

		internal void OnSaveRoutesClick(object sender, EventArgs e)
		{
			if (MainViewUnderlay.MapBase != null)
			{
				MainViewUnderlay.MapBase.SaveRoutes();

				ObserverManager.RouteView   .Control     .RoutesChanged =
				ObserverManager.TopRouteView.ControlRoute.RoutesChanged = false;
			}
		}

		private void OnSaveAsClick(object sender, EventArgs e)
		{
			if (   MainViewUnderlay.MapBase != null				// safety. Not sure if a 'MapBase' could be
				&& MainViewUnderlay.MapBase.Descriptor != null)	// instantiated without a 'Descriptor'.
			{
				var sfd = new SaveFileDialog();

				sfd.FileName = MainViewUnderlay.MapBase.Descriptor.Label + GlobalsXC.MapExt;
				sfd.Filter = "Map files (*.MAP)|*.MAP|All files (*.*)|*.*";
				sfd.Title = "Save Map and subordinate Route file as ...";
				sfd.InitialDirectory = Path.Combine(MainViewUnderlay.MapBase.Descriptor.Basepath, GlobalsXC.MapsDir);

				if (sfd.ShowDialog() == DialogResult.OK)
				{
					string dir = Path.GetDirectoryName(sfd.FileName); // 'FileName' is fullpath.
					//LogFile.WriteLine("dir= " + dir);
					// NOTE: GetDirectoryName() will return a string ending with a
					// path-separator if it's the root dir, and without one if it's
					// not. But Path.Combine() doesn't figure out the difference.

					//LogFile.WriteLine("pathroot= " + Path.GetPathRoot(dir));
					if (dir != Path.GetPathRoot(dir))
					{
						string basepath = dir.Substring(0, dir.LastIndexOf(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal));
						//LogFile.WriteLine("basepath= " + basepath);
						if (basepath.IndexOf(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal) == -1)	// check if root dir, again
							basepath += Path.DirectorySeparatorChar.ToString();											// account for awkward path at the root dir.
																														// NOTE: But that's probly not valid for
																														// things like mounted or network drives.
						string dirMaps   = Path.Combine(basepath, GlobalsXC.MapsDir);
						string dirRoutes = Path.Combine(basepath, GlobalsXC.RoutesDir);
						//LogFile.WriteLine("dirMaps= " + dirMaps);
						//LogFile.WriteLine("dirRoutes= " + dirRoutes);

						string file = Path.GetFileNameWithoutExtension(sfd.FileName);
						string pfMaps   = Path.Combine(dirMaps, file);
						string pfRoutes = Path.Combine(dirRoutes, file);
						//LogFile.WriteLine("pfMaps= " + pfMaps);
						//LogFile.WriteLine("pfRoutes= " + pfRoutes);

						MainViewUnderlay.MapBase.SaveMap(pfMaps);
						MainViewUnderlay.MapBase.SaveRoutes(pfRoutes);
					}
					else
						MessageBox.Show(
									this,
									"Saving to a root folder is not allowed. raesons.",
									" Error",
									MessageBoxButtons.OK,
									MessageBoxIcon.Error,
									MessageBoxDefaultButton.Button1,
									0);
				}
			}
		}

		private void OnSaveMaptreeClick(object sender, EventArgs e)
		{
			MaptreeChanged = !ResourceInfo.TileGroupManager.SaveTileGroups();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnReloadClick(object sender, EventArgs e)
		{
			OnReloadDescriptor();
		}

		private void OnScreenshotClick(object sender, EventArgs e)
		{
			MapFileBase @base = MainViewUnderlay.MapBase;
			if (@base != null)
			{
				sfdSaveDialog.FileName = @base.Descriptor.Label;
				if (sfdSaveDialog.ShowDialog() == DialogResult.OK)
					@base.Screenshot(sfdSaveDialog.FileName);
			}
		}

		/// <summary>
		/// Closes Everything.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnQuitClick(object sender, EventArgs e)
		{
			Close();
		}


		/// <summary>
		/// Opens a dialog that allows user to resize the current Mapfile.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMapResizeClick(object sender, EventArgs e)
		{
			MapFileBase @base = MainViewUnderlay.MapBase;
			if (@base != null)
			{
				using (var f = new MapResizeInputBox(@base))
				{
					if (f.ShowDialog(this) == DialogResult.OK)
					{
						int changes = @base.MapResize(
													f.Rows,
													f.Cols,
													f.Levs,
													f.zType);

						if (!@base.MapChanged && ((changes & MapFileBase.CHANGED_MAP) != 0))
							MapChanged = true;

						if (!@base.RoutesChanged && (changes & MapFileBase.CHANGED_NOD) != 0)
						{
							ObserverManager.RouteView   .Control     .RoutesChanged =
							ObserverManager.TopRouteView.ControlRoute.RoutesChanged = true;
						}

						MainViewUnderlay.ForceResize();

						MainViewOverlay.FirstClick = false;

						ObserverManager.RouteView   .Control     .ClearSelectedInfo();
						ObserverManager.TopRouteView.ControlRoute.ClearSelectedInfo();

						ObserverManager.ToolFactory.SetLevelButtonsEnabled(@base.Level, @base.MapSize.Levs);

						tsslDimensions   .Text = @base.MapSize.ToString();
						tsslPosition     .Text =
						tsslSelectionSize.Text = String.Empty;

						ObserverManager.SetObservers(@base);

						ObserverManager.TopView     .Control   .TopPanel.ClearSelectorLozenge();
						ObserverManager.TopRouteView.ControlTop.TopPanel.ClearSelectorLozenge();

						if (ScanG != null) // update ScanG viewer if open
							ScanG.LoadMapfile(@base);

						ResetQuadrantPanel();
					}
				}
			}
		}

		/// <summary>
		/// Opens the Configuration Editor.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnConfiguratorClick(object sender, EventArgs e)
		{
			string changed = null;

			if (MainViewUnderlay.MapBase != null)
			{
				if (MainViewUnderlay.MapBase.MapChanged)
					changed = "Map";

				if (MainViewUnderlay.MapBase.RoutesChanged)
				{
					if (!String.IsNullOrEmpty(changed))
						changed += " and ";

					changed += "Routes";
				}
			}

			if (MaptreeChanged)
			{
				if (!String.IsNullOrEmpty(changed))
					changed += " and ";

				changed += "Maptree";
			}

			if (!String.IsNullOrEmpty(changed))
			{
				switch (MessageBox.Show(
									this,
									"Accepting the Configuration Editor can restart MapView."
										+ " The current " + changed + " should be saved or else"
										+ " any changes will be lost. How do you wish to proceed?"
										+ Environment.NewLine + Environment.NewLine
										+ "Abort\treturn to state"
										+ Environment.NewLine
										+ "Retry\tsave changes and open the Configurator"
										+ Environment.NewLine
										+ "Ignore\trisk losing changes and open the Configurator",
									" Changes detected",
									MessageBoxButtons.AbortRetryIgnore,
									MessageBoxIcon.Asterisk,
									MessageBoxDefaultButton.Button1,
									0))
				{
					case DialogResult.Abort:
						break;

					case DialogResult.Retry:
						if (MainViewUnderlay.MapBase != null)
						{
							if (MainViewUnderlay.MapBase.MapChanged)
							{
								MainViewUnderlay.MapBase.SaveMap();
								MapChanged = false;
							}

							if (MainViewUnderlay.MapBase.RoutesChanged)
							{
								MainViewUnderlay.MapBase.SaveRoutes();

								ObserverManager.RouteView   .Control     .RoutesChanged =
								ObserverManager.TopRouteView.ControlRoute.RoutesChanged = false;
							}
						}

						if (MaptreeChanged)
						{
//							MaptreeChanged = !ResourceInfo.TileGroupInfo.SaveTileGroups(); // <- that could cause endless recursion.
							ResourceInfo.TileGroupManager.SaveTileGroups();
							MaptreeChanged = false;
						}

						OnConfiguratorClick(null, EventArgs.Empty); // recurse.
						break;

					case DialogResult.Ignore:	// TODO: A bypass-variable should be implemented to deal
						MapChanged =			// with Changes so that the real Changed vals don't get wiped.
						ObserverManager.RouteView   .Control     .RoutesChanged =
						ObserverManager.TopRouteView.ControlRoute.RoutesChanged =
						MaptreeChanged = false;

						OnConfiguratorClick(null, EventArgs.Empty); // recurse.
						break;
				}
			}
			else
			{
				using (var f = new ConfigurationForm(true))
				{
					if (f.ShowDialog(this) == DialogResult.OK)
					{
						Application.Restart();
						Environment.Exit(0);
					}
				}
			}
		}


		/// <summary>
		/// Opens the CHM helpfile.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnHelpClick(object sender, EventArgs e)
		{
			string help = Path.GetDirectoryName(Application.ExecutablePath);
				   help = Path.Combine(help, "MapView.chm");
			Help.ShowHelp(XCMainWindow.that, "file://" + help);
		}

		internal ColorHelp _fcolors;
		/// <summary>
		/// Opens the ColorsHelp dialog.
		/// @note This handler is not a toggle. The dialog will be focused if
		/// already open.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnColorsClick(object sender, EventArgs e)
		{
			if (!miColors.Checked)
			{
				miColors.Checked = true;

				_fcolors = new ColorHelp(this);
				_fcolors.Show(); // no owner.
			}
			else
				_fcolors.BringToFront();
		}

		/// <summary>
		/// Dechecks the Colors item when the Colors dialog closes.
		/// </summary>
		internal void DecheckColors()
		{
			miColors.Checked = false;
		}

		internal About _fabout;
		/// <summary>
		/// Opens the About dialog
		/// @note This handler is not a toggle. The dialog will be focused if
		/// already open.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnAboutClick(object sender, EventArgs e)
		{
			if (!miAbout.Checked)
			{
				miAbout.Checked = true;

				_fabout = new About();
				_fabout.Show(); // no owner.
			}
			else
				_fabout.BringToFront();
		}

		/// <summary>
		/// Dechecks the About item when the About dialog closes.
		/// </summary>
		internal void DecheckAbout()
		{
			miAbout.Checked = false;
		}

		internal MapInfoDialog _finfo;
		/// <summary>
		/// Opens the MapInfo dialog.
		/// @note This handler is a toggle. The dialog will be closed if it's
		/// open.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMapInfoClick(object sender, EventArgs e)
		{
			if (!miMapInfo.Checked)
			{
				if (MainViewUnderlay.MapBase != null) // safety.
				{
					miMapInfo.Checked = true;
					_finfo = new MapInfoDialog(this, MainViewUnderlay.MapBase as MapFile);
					_finfo.Show(); // no owner.
					_finfo.Analyze();
				}
			}
			else
				_finfo.Close(); // TODO: Close MapInfo when loading a different Mapfile. (etc)
		}

		/// <summary>
		/// Dechecks the MapInfo item when the MapInfo dialog closes.
		/// </summary>
		internal void DecheckMapInfo()
		{
			miMapInfo.Checked = false;
		}


		internal void OnScaleInClick(object sender, EventArgs e)
		{
			if (Globals.Scale < Globals.ScaleMaximum)
			{
				Globals.Scale += Math.Min(
										Globals.ScaleMaximum - Globals.Scale,
										ScaleDelta);
				Scale();
			}
		}

		internal void OnScaleOutClick(object sender, EventArgs e)
		{
			if (Globals.Scale > Globals.ScaleMinimum)
			{
				Globals.Scale -= Math.Min(
										Globals.Scale - Globals.ScaleMinimum,
										ScaleDelta);
				Scale();
			}
		}

		private void Scale()
		{
			ObserverManager.ToolFactory.DisableScaleChecked();
			Globals.AutoScale = false;

			MainViewUnderlay.SetOverlaySize();
			MainViewUnderlay.UpdateScrollers();

			Invalidate();
		}

		internal void OnScaleClick(object sender, EventArgs e)
		{
			Globals.AutoScale = ObserverManager.ToolFactory.ToggleScaleChecked();
			if (Globals.AutoScale)
			{
				MainViewUnderlay.SetScale();
				MainViewUnderlay.SetOverlaySize();
			}
			MainViewUnderlay.UpdateScrollers();
		}


		/// <summary>
		/// Searches the MapTree for a given string.
		/// </summary>
		/// <param name="text"></param>
		internal void Search(string text)
		{
			if (!String.IsNullOrEmpty(text))
			{
				TreeNode
					start0,
					start = null;

				if (Searched != null)
					start0 = Searched;
				else
					start0 = MapTree.SelectedNode;

				if (start0 != null)
				{
					start = start0;

					if (start.Nodes.Count != 0)
					{
						start = start.Nodes[0];
					}
					else if (start.NextNode != null)
					{
						start = start.NextNode;
					}
					else if (start.Parent != null)
					{
						if (start.Parent.NextNode != null)
						{
							start = start.Parent.NextNode;
						}
						else if (start.Parent.Parent != null
							&& start.Parent.Parent.NextNode != null)
						{
							start = start.Parent.Parent.NextNode;
						}
						else
							start = MapTree.Nodes[0];
					}
					else
						start = MapTree.Nodes[0];

					if (start != null) // jic.
					{
						var node = SearchTreeview(
												text.ToLower(),
												MapTree.Nodes,
												start,
												start0);
						if (node != null)
						{
							Searched = node;
							Searched.EnsureVisible();
							MapTree.Invalidate();
						}

						_active   =
						_hardstop = false;
					}
				}
			}
		}

		bool _active, _hardstop;

		/// <summary>
		/// Searches through the MapTree given a node to start at.
		/// </summary>
		/// <param name="text">the text to search for (lowercase)</param>
		/// <param name="nodes">the collection of nodes to search through</param>
		/// <param name="start">the node to start at</param>
		/// <param name="start0">the node to stop at</param>
		/// <returns>a found node or null</returns>
		private TreeNode SearchTreeview(
				string text,
				TreeNodeCollection nodes,
				TreeNode start,
				TreeNode start0)
		{
			if (!_hardstop)
			{
				TreeNode child;

				foreach (TreeNode node in nodes)
				{
					if (!_active)
					{
						_active = (node == start);
					}
					else if (node == start0)
					{
						_hardstop = true;	// <- whatever you were doing in the way of recursions stop it.
						return null;		// -> not found after wrapping. NOTE: Does not highlight the
					}						// current node even if that node has the searched for text.

					if (_active && node.Text.ToLower().Contains(text))
					{
						return node;
					}

					if (node.Nodes.Count != 0)
					{
						if ((child = SearchTreeview(text, node.Nodes, start, start0)) != null)
						{
							return child;
						}
					}
					else if (                             node              .NextNode == null	// if no more nodes at the current level
						&& (node.Parent        == null || node.Parent       .NextNode == null)	// and no parent OR parent is last node at its level
						&& (node.Parent.Parent == null || node.Parent.Parent.NextNode == null))	// and no parent-of-parent OR parent-of-parent is last node at its level
					{
						return SearchTreeview(text, MapTree.Nodes, MapTree.Nodes[0], start0);
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Clears the searched/found/highlighted Treenode.
		/// </summary>
		internal void ClearSearched()
		{
			Searched = null;
			MapTree.Invalidate();
		}

		// debug versions of functions above^
//		int _recurse = -1;
/*		internal void Search(string text)
		{
			if (!String.IsNullOrEmpty(text))
			{
				TreeNode start0, start;

				if (Searched != null)
					start0 = Searched;
				else
					start0 = MapTree.SelectedNode;

				if (start0 != null)
				{
					start = start0;

					LogFile.WriteLine("");
					LogFile.WriteLine("start= " + start.Text);

					if (start.Nodes.Count != 0)
					{
						start = start.Nodes[0];
						LogFile.WriteLine(". start.Nodes.Count != 0 start= " + start.Text);
					}
					else if (start.NextNode != null)
					{
						start = start.NextNode;
						LogFile.WriteLine(". start.NextNode != null start= " + start.Text);
					}
					else if (start.Parent != null)
					{
						LogFile.WriteLine(". start.Parent != null");

						if (start.Parent.NextNode != null)
						{
							start = start.Parent.NextNode;
							LogFile.WriteLine(". . start.Parent.NextNode != null start= " + start.Text);
						}
						else if (start.Parent.Parent != null
							&& start.Parent.Parent.NextNode != null)
						{
							start = start.Parent.Parent.NextNode;
							LogFile.WriteLine(". . start.Parent.Parent != null && start.Parent.Parent.NextNode != null start= " + start.Text);
						}
						else // wrap
						{
//							LogFile.WriteLine(". . . start.Parent.Parent == null RETURN");
//							return;
							start = MapTree.Nodes[0];
							LogFile.WriteLine(". . . start.Parent.Parent == null start= " + start.Text);
						}
					}
					else // wrap
					{
//						LogFile.WriteLine(". start.NextNode == null && start.Parent == null RETURN");
//						return;
						start = MapTree.Nodes[0];
						LogFile.WriteLine(". start.NextNode == null && start.Parent == null start= " + start.Text);
					}

					if (start != null) // jic.
					{
						var node = SearchTreeview(text.ToLower(), MapTree.Nodes, start, start0);
						if (node != null)
						{
							Searched = node;
							Searched.EnsureVisible();
						}
						_active   =
						_hardstop = false;
					}
				}
			}
		} */
/*		private TreeNode SearchTreeview(
				string text,
				TreeNodeCollection nodes,
				TreeNode start,
				TreeNode start0)
		{
			if (!_hardstop)
			{
				int recurse = ++_recurse; // debug.

				TreeNode child;

				foreach (TreeNode node in nodes)
				{
					if (!_active)
					{
						_active = (node == start);
						if (_active) LogFile.WriteLine(recurse + " set Active");
					}
					else if (node == start0)
					{
						LogFile.WriteLine(recurse + " node == start0 ret NULL");
						_hardstop = true;	// <- whatever you were doing in the way of recursions stop it.
						return null;		// not found after wrapping. NOTE: Does not highlight the current node even if that node has the searched for text.
					}

					if (_active && node.Text.ToLower().Contains(text))
					{
						LogFile.WriteLine(recurse + " get " + node.Text);
						return node;
					}

					if (node.Nodes.Count != 0)
					{
						if ((child = SearchTreeview(text, node.Nodes, start, start0)) != null)
						{
							LogFile.WriteLine(recurse + " get child " + child.Text);
							return child;
						}
					}
					else if (node.NextNode == null												// if no more nodes at the current level
						&& (node.Parent == null        || node.Parent.NextNode == null)			// and no parent OR parent is last node at its level
						&& (node.Parent.Parent == null || node.Parent.Parent.NextNode == null))	// and no parent-of-parent OR parent-of-parent is last node at its level
					{
						LogFile.WriteLine(recurse + " search from Nodes[0]");
						return SearchTreeview(text, MapTree.Nodes, MapTree.Nodes[0], start0);
					}
				}
			}

			LogFile.WriteLine("ret NULL");
			return null;
		} */


		private bool _dontbeep1; // aka. "just because you have billions of dollars don't mean you can beep"

		/// <summary>
		/// Opens the Maptree's contextmenu on keydown event [Enter] via a
		/// circuitous pattern delegate called DontBeep. Does what it says here.
		/// Or if [Shift+Enter] then try to reload the Mapfile.
		/// </summary>
		private void FireContext()
		{
			if (_dontbeep1)
			{
				var nodebounds = _selected.Bounds;
				var args = new MouseEventArgs(
											MouseButtons.Right,
											1,
											nodebounds.X + 15, nodebounds.Y + 5,
											0);
				OnMapTreeMouseDown(null, args);
			}
			else
			{
				var args = new TreeNodeMouseClickEventArgs(
														_selected,
														MouseButtons.None,
														0, 0,0);
				OnMapTreeNodeClick(null, args);
			}
		}

		/// <summary>
		/// Opens a context-menu on RMB-click.
		/// NOTE: A MouseDown event occurs *before* the treeview's BeforeSelect
		/// and AfterSelected events occur ....
		/// NOTE: A MouseClick event occurs *after* the treeview's BeforeSelect
		/// and AfterSelected events occur. So the selected Map will change
		/// *before* a context-menu is shown, which is good.
		/// NOTE: A MouseClick event won't work if the tree is blank. So use MouseDown.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMapTreeMouseDown(object sender, MouseEventArgs e)
		{
			//LogFile.WriteLine("XCMainWindow.OnMapTreeMouseDown");
			//if (MapTree.SelectedNode != null) LogFile.WriteLine(". selected= " + MapTree.SelectedNode.Text);

			switch (e.Button)
			{
				case MouseButtons.Right:
					if (MainViewUnderlay.MapBase == null					// prevents a bunch of problems, like looping dialogs when
						|| (   !MainViewUnderlay.MapBase.MapChanged			// returning from the Tileset Editor and the Maptree-node
							&& !MainViewUnderlay.MapBase.RoutesChanged))	// gets re-selected, causing this class-object to react as
					{														// if a different Map is going to load ... cf, LoadSelectedDescriptor()
						cmMapTreeMenu.MenuItems.Clear();

						cmMapTreeMenu.MenuItems.Add("Add Group ...", new EventHandler(OnAddGroupClick));

						if (MapTree.SelectedNode != null)
						{
							switch (MapTree.SelectedNode.Level)
							{
								case 0: // group-node.
									cmMapTreeMenu.MenuItems.Add("-");
									cmMapTreeMenu.MenuItems.Add("Edit Group ...", new EventHandler(OnEditGroupClick));
									cmMapTreeMenu.MenuItems.Add("Delete Group",   new EventHandler(OnDeleteGroupClick));
									cmMapTreeMenu.MenuItems.Add("-");
									cmMapTreeMenu.MenuItems.Add("Add Category ...", new EventHandler(OnAddCategoryClick));
									break;

								case 1: // category-node.
									cmMapTreeMenu.MenuItems.Add("-");
									cmMapTreeMenu.MenuItems.Add("Edit Category ...", new EventHandler(OnEditCategoryClick));
									cmMapTreeMenu.MenuItems.Add("Delete Category",   new EventHandler(OnDeleteCategoryClick));
									cmMapTreeMenu.MenuItems.Add("-");
									cmMapTreeMenu.MenuItems.Add("Add Tileset ...", new EventHandler(OnAddTilesetClick));
									break;

								case 2: // tileset-node.
									cmMapTreeMenu.MenuItems.Add("-");
									cmMapTreeMenu.MenuItems.Add("Edit Tileset ...", new EventHandler(OnEditTilesetClick));
									cmMapTreeMenu.MenuItems.Add("Delete Tileset",   new EventHandler(OnDeleteTilesetClick));
									break;
							}
						}

						cmMapTreeMenu.Show(MapTree, e.Location);
					}
					else
					{
						switch (MessageBox.Show(
											this,
											"Modifying the Maptree can cause the Tilesets to reload."
												+ " The current Map and/or its Routes should be saved or else"
												+ " any changes would be lost. How do you wish to proceed?"
												+ Environment.NewLine + Environment.NewLine
												+ "Abort\treturn to state"
												+ Environment.NewLine
												+ "Retry\tsave changes and show the Maptree-menu"
												+ Environment.NewLine
												+ "Ignore\trisk losing changes and show the Maptree-menu",
											" Changes detected",
											MessageBoxButtons.AbortRetryIgnore,
											MessageBoxIcon.Asterisk,
											MessageBoxDefaultButton.Button1,
											0))
						{
							case DialogResult.Abort:
								break;

							case DialogResult.Retry:
								if (MainViewUnderlay.MapBase.MapChanged)
								{
									MainViewUnderlay.MapBase.SaveMap();
									MapChanged = false;
								}

								if (MainViewUnderlay.MapBase.RoutesChanged)
								{
									MainViewUnderlay.MapBase.SaveRoutes();

									ObserverManager.RouteView   .Control     .RoutesChanged =
									ObserverManager.TopRouteView.ControlRoute.RoutesChanged = false;
								}

								OnMapTreeMouseDown(null, e); // recurse.
								break;

							case DialogResult.Ignore:
								MapChanged =
								ObserverManager.RouteView   .Control     .RoutesChanged =
								ObserverManager.TopRouteView.ControlRoute.RoutesChanged = false;

								OnMapTreeMouseDown(null, e); // recurse.
								break;
						}
					}
					break;
			}
		}

		/// <summary>
		/// Adds a group to the map-tree.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnAddGroupClick(object sender, EventArgs e)
		{
			//LogFile.WriteLine("XCMainWindow.OnAddGroupClick");

			using (var f = new MapTreeInputBox(
											"Enter the label for a new Map group."
												+ " It needs to start with UFO or TFTD (case insensitive) since"
												+ " the prefix will set the default path and palette of its tilesets.",
											"Note that groups that do not contain tilesets will not be saved.",
											MapTreeInputBox.BoxType.AddGroup,
											String.Empty))
			{
				if (f.ShowDialog(this) == DialogResult.OK)
				{
					MaptreeChanged = true;

					ResourceInfo.TileGroupManager.AddTileGroup(f.Label);

					CreateTree();
					SelectGroupNode(f.Label);
				}
			}
		}

		/// <summary>
		/// Edits the label of a group on the map-tree.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnEditGroupClick(object sender, EventArgs e)
		{
			//LogFile.WriteLine("XCMainWindow.OnEditGroupClick");

			using (var f = new MapTreeInputBox(
											"Enter a new label for the Map group."
												+ " It needs to start with UFO or TFTD (case insensitive) since"
												+ " the prefix will set the default path and palette of its tilesets.",
											"Note that groups that do not contain tilesets will not be saved.",
											MapTreeInputBox.BoxType.EditGroup,
											String.Empty))
			{
				string labelGroup = MapTree.SelectedNode.Text;

				f.Label = labelGroup;
				if (f.ShowDialog(this) == DialogResult.OK)
				{
					MaptreeChanged = true;

					ResourceInfo.TileGroupManager.EditTileGroup(
															f.Label,
															labelGroup);
					CreateTree();
					SelectGroupNode(f.Label);
				}
			}
		}

		/// <summary>
		/// Deletes a group from the map-tree.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnDeleteGroupClick(object sender, EventArgs e)
		{
			//LogFile.WriteLine("XCMainWindow.OnDeleteGroupClick");

			// TODO: Make a custom box for delete Group/Category/Tileset.

			string labelGroup = MapTree.SelectedNode.Text;

			string notice = String.Format(
										CultureInfo.CurrentCulture,
										"Are you sure you want to remove this Map group?"
											+ " This will also remove all its categories and tilesets,"
											+ " but files on disk are unaffected.{0}{0}group\t{1}",
										Environment.NewLine,
										labelGroup);
			if (MessageBox.Show(
							this,
							notice,
							" Warning",
							MessageBoxButtons.OKCancel,
							MessageBoxIcon.Warning,
							MessageBoxDefaultButton.Button1,
							0) == DialogResult.OK)
			{
				MaptreeChanged = true;

				ResourceInfo.TileGroupManager.DeleteTileGroup(labelGroup);

				CreateTree();
				SelectGroupNodeTop();
			}
		}

		/// <summary>
		/// Adds a category to a group on the map-tree.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnAddCategoryClick(object sender, EventArgs e)
		{
			//LogFile.WriteLine("XCMainWindow.OnAddCategoryClick");

			string labelGroup = MapTree.SelectedNode.Text;

			using (var f = new MapTreeInputBox(
											"Enter the label for a new Map category.",
											"Note that categories that do not contain tilesets will not be saved.",
											MapTreeInputBox.BoxType.AddCategory,
											labelGroup))
			{
				if (f.ShowDialog(this) == DialogResult.OK)
				{
					MaptreeChanged = true;

					var @group = ResourceInfo.TileGroupManager.TileGroups[labelGroup];
					@group.AddCategory(f.Label);

					CreateTree();
					SelectCategoryNode(f.Label, @group.Label);
				}
			}
		}

		/// <summary>
		/// Edits the label of a category on the map-tree.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnEditCategoryClick(object sender, EventArgs e)
		{
			//LogFile.WriteLine("XCMainWindow.OnEditCategoryClick");

			string labelGroup = MapTree.SelectedNode.Parent.Text;

			using (var f = new MapTreeInputBox(
											"Enter a new label for the Map category.",
											"Note that categories that do not contain tilesets will not be saved.",
											MapTreeInputBox.BoxType.EditCategory,
											labelGroup))
			{
				string labelCategory = MapTree.SelectedNode.Text;

				f.Label = labelCategory;
				if (f.ShowDialog(this) == DialogResult.OK)
				{
					MaptreeChanged = true;

					var @group = ResourceInfo.TileGroupManager.TileGroups[labelGroup];
					@group.EditCategory(f.Label, labelCategory);

					CreateTree();
					SelectCategoryNode(f.Label, @group.Label);
				}
			}
		}

		/// <summary>
		/// Deletes a category from the map-tree.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnDeleteCategoryClick(object sender, EventArgs e)
		{
			//LogFile.WriteLine("XCMainWindow.OnDeleteCategoryClick");

			// TODO: Make a custom box for delete Group/Category/Tileset.

			string labelGroup    = MapTree.SelectedNode.Parent.Text;
			string labelCategory = MapTree.SelectedNode.Text;

			string notice = String.Format(
										CultureInfo.CurrentCulture,
										"Are you sure you want to remove this Map category?"
											+ " This will also remove all its tilesets, but"
											+ " files on disk are unaffected.{0}{0}"
											+ "group\t{1}{0}"
											+ "category\t{2}",
										Environment.NewLine,
										labelGroup, labelCategory);
			if (MessageBox.Show(
							this,
							notice,
							" Warning",
							MessageBoxButtons.OKCancel,
							MessageBoxIcon.Warning,
							MessageBoxDefaultButton.Button1,
							0) == DialogResult.OK)
			{
				MaptreeChanged = true;

				var @group = ResourceInfo.TileGroupManager.TileGroups[labelGroup];
				@group.DeleteCategory(labelCategory);

				CreateTree();
				SelectCategoryNodeTop(labelGroup);
			}
		}

		/// <summary>
		/// Adds a tileset and its characteristics to the map-tree.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnAddTilesetClick(object sender, EventArgs e)
		{
			//LogFile.WriteLine("XCMainWindow.OnAddTilesetClick");

			string labelGroup = MapTree.SelectedNode.Parent.Text;
			if (isGrouptypeConfigured(labelGroup))
			{
				string labelCategory = MapTree.SelectedNode.Text;
				string labelTileset  = String.Empty;

				using (var f = new TilesetEditor(
											TilesetEditor.BoxType.AddTileset,
											labelGroup,
											labelCategory,
											labelTileset))
				{
					if (f.ShowDialog(this) == DialogResult.OK)
					{
						//LogFile.WriteLine(". f.Tileset= " + f.Tileset);

						MaptreeChanged = true;

						CreateTree();
						SelectTilesetNode(f.Tileset, labelCategory, labelGroup);
					}
				}
			}
		}

		/// <summary>
		/// Edits the characteristics of a tileset on the map-tree.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnEditTilesetClick(object sender, EventArgs e)
		{
			//LogFile.WriteLine("XCMainWindow.OnEditTilesetClick");

			string labelGroup = MapTree.SelectedNode.Parent.Parent.Text;
			if (isGrouptypeConfigured(labelGroup))
			{
				string labelCategory = MapTree.SelectedNode.Parent.Text;
				string labelTileset  = MapTree.SelectedNode.Text;

				using (var f = new TilesetEditor(
											TilesetEditor.BoxType.EditTileset,
											labelGroup,
											labelCategory,
											labelTileset))
				{
					if (f.ShowDialog(this) == DialogResult.OK)
					{
						//LogFile.WriteLine(". f.Tileset= " + f.Tileset);

						MaptreeChanged = true;

						CreateTree();
						SelectTilesetNode(f.Tileset, labelCategory, labelGroup);
					}
				}
			}
		}

		/// <summary>
		/// Checks that the group-type is configured so that the TilesetEditor
		/// doesn't explode. Shows an error if not configured.
		/// </summary>
		/// <param name="labelGroup">the label of the group</param>
		/// <returns>true if okay to proceed</returns>
		private bool isGrouptypeConfigured(string labelGroup)
		{
			var TileGroup = ResourceInfo.TileGroupManager.TileGroups[labelGroup] as TileGroup;

			string key = null;
			switch (TileGroup.GroupType)
			{
				case GameType.Ufo:  key = SharedSpace.ResourceDirectoryUfo;  break;
				case GameType.Tftd: key = SharedSpace.ResourceDirectoryTftd; break;
			}

			if (SharedSpace.GetShareString(key) == null)
			{
				switch (TileGroup.GroupType)
				{
					case GameType.Ufo:  key = "UFO";  break;
					case GameType.Tftd: key = "TFTD"; break;
				}

				MessageBox.Show(
							this,
							key + " is not configured.",
							" Error",
							MessageBoxButtons.OK,
							MessageBoxIcon.Error,
							MessageBoxDefaultButton.Button1,
							0);
				return false;
			}
			return true;
		}

		/// <summary>
		/// Deletes a tileset from the map-tree.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnDeleteTilesetClick(object sender, EventArgs e)
		{
			//LogFile.WriteLine("XCMainWindow.OnDeleteTilesetClick");

			// TODO: Make a custom box for delete Group/Category/Tileset.

			string labelGroup    = MapTree.SelectedNode.Parent.Parent.Text;
			string labelCategory = MapTree.SelectedNode.Parent.Text;
			string labelTileset  = MapTree.SelectedNode.Text;

			string notice = String.Format(
										CultureInfo.CurrentCulture,
										"Are you sure you want to remove this Map tileset?"
											+ " Files on disk are unaffected.{0}{0}"
											+ "group\t{1}{0}"
											+ "category\t{2}{0}"
											+ "tileset\t{3}",
										Environment.NewLine,
										labelGroup, labelCategory, labelTileset);
			if (MessageBox.Show(
							this,
							notice,
							" Warning",
							MessageBoxButtons.OKCancel,
							MessageBoxIcon.Warning,
							MessageBoxDefaultButton.Button1,
							0) == DialogResult.OK)
			{
				MaptreeChanged = true;

				var @group = ResourceInfo.TileGroupManager.TileGroups[labelGroup];
				@group.DeleteTileset(labelTileset, labelCategory);

				CreateTree();
				SelectTilesetNodeTop(labelCategory);
			}
		}


		// TODO: consolidate the select node functions into a single function.

		/// <summary>
		/// Selects the top treenode in the Maps tree if one exists.
		/// </summary>
		private void SelectGroupNodeTop()
		{
			//LogFile.WriteLine("");
			//LogFile.WriteLine("SelectGroupNodeTop");

			if (MapTree.Nodes.Count != 0)
				MapTree.SelectedNode = MapTree.Nodes[0];
		}

		/// <summary>
		/// Selects the top category treenode in the Maps tree if one exists
		/// under a given group treenode.
		/// NOTE: Assumes that the parent-group node is valid.
		/// </summary>
		/// <param name="labelGroup"></param>
		private void SelectCategoryNodeTop(string labelGroup)
		{
			//LogFile.WriteLine("");
			//LogFile.WriteLine("SelectCategoryNodeTop");

			foreach (TreeNode nodeGroup in MapTree.Nodes)
			{
				if (nodeGroup.Text == labelGroup)
				{
					var groupCollection = nodeGroup.Nodes;
					MapTree.SelectedNode = (groupCollection.Count != 0) ? groupCollection[0]
																		: nodeGroup;
				}
			}
		}

		/// <summary>
		/// Selects the top tileset treenode in the Maps tree if one exists
		/// under a given category treenode.
		/// NOTE: Assumes that the parent-parent-group and parent-category nodes
		/// are valid.
		/// </summary>
		/// <param name="labelCategory"></param>
		private void SelectTilesetNodeTop(string labelCategory)
		{
			//LogFile.WriteLine("");
			//LogFile.WriteLine("SelectTilesetNodeTop");

			foreach (TreeNode nodeGroup in MapTree.Nodes)
			{
				var groupCollection = nodeGroup.Nodes;
				foreach (TreeNode nodeCategory in groupCollection)
				{
					if (nodeCategory.Text == labelCategory)
					{
						var categoryCollection = nodeCategory.Nodes;
						MapTree.SelectedNode = (categoryCollection.Count != 0) ? categoryCollection[0]
																			   : nodeCategory;
					}
				}
			}
		}

		/// <summary>
		/// Selects a treenode in the Maps tree given a group-label.
		/// </summary>
		/// <param name="labelGroup"></param>
		private void SelectGroupNode(string labelGroup)
		{
			//LogFile.WriteLine("");
			//LogFile.WriteLine("SelectGroupNode");

			foreach (TreeNode nodeGroup in MapTree.Nodes)
			{
				if (nodeGroup.Text == labelGroup)
				{
					MapTree.SelectedNode = nodeGroup;
					nodeGroup.Expand();
					break;
				}
			}
		}

		/// <summary>
		/// Selects a treenode in the Maps tree given a category-label.
		/// </summary>
		/// <param name="labelCategory"></param>
		/// <param name="labelGroup"></param>
		private void SelectCategoryNode(string labelCategory, string labelGroup)
		{
			//LogFile.WriteLine("");
			//LogFile.WriteLine("SelectCategoryNode");

			bool found = false;

			foreach (TreeNode nodeGroup in MapTree.Nodes)
			{
				if (found) break;

				if (nodeGroup.Text == labelGroup)
				{
					var groupCollection = nodeGroup.Nodes;
					foreach (TreeNode nodeCategory in groupCollection)
					{
						if (nodeCategory.Text == labelCategory)
						{
							found = true;

							MapTree.SelectedNode = nodeCategory;
							nodeCategory.Expand();
							break;
						}
					}
				}
			}
		}

		/// <summary>
		/// Selects a treenode in the Maps tree given a tileset-label and
		/// Category.
		/// </summary>
		/// <param name="labelTileset"></param>
		/// <param name="labelCategory"></param>
		/// <param name="labelGroup"></param>
		private void SelectTilesetNode(string labelTileset, string labelCategory, string labelGroup)
		{
			//LogFile.WriteLine("");
			//LogFile.WriteLine("SelectTilesetNode");

			bool found = false;

			foreach (TreeNode nodeGroup in MapTree.Nodes)
			{
				if (found) break;

				//LogFile.WriteLine(". group= " + nodeGroup.Text);

				if (nodeGroup.Text == labelGroup)
				{
					var groupCollection = nodeGroup.Nodes;
					foreach (TreeNode nodeCategory in groupCollection)
					{
						if (found) break;

						//LogFile.WriteLine(". . category= " + nodeCategory.Text);

						if (nodeCategory.Text == labelCategory)
						{
							var categoryCollection = nodeCategory.Nodes;
							foreach (TreeNode nodeTileset in categoryCollection)
							{
								//LogFile.WriteLine(". . . tileset= " + nodeTileset.Text);

								if (nodeTileset.Text == labelTileset)
								{
									found = true;

									MapTree.SelectedNode = nodeTileset;
									break;
								}
							}
						}
					}
				}
			}
		}


		/// <summary>
		/// For an ungodly reason when the Maptree gains/loses focus
		/// tv_DrawNode() re-colors selected and focused nodes correctly but
		/// does not re-color a Searched node.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMapTreeFocusChanged(object sender, EventArgs e)
		{
			if (Searched != null)
				MapTree.Invalidate();
		}

//		private bool _bypassSaveAlert;	// when reloading the MapTree after making a tileset edit
										// the treeview's BeforeSelect event fires. This needlessly
										// asks to save the Map (if it had already changed) and
										// results in an endless cycle of confirmation dialogs ...
										// so bypass all that.
										//
										// Congratulations. Another programming language/framework
										// I've come to hate. The BeforeSelect event fires twice
										// (at least) rendering the boolean entirely obsolete.
		/// <summary>
		/// Asks user to save before switching Maps if applicable.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMapTreeBeforeSelect(object sender, CancelEventArgs e)
		{
			//LogFile.WriteLine("XCMainWindow.OnMapTreeBeforeSelect");
			//if (MapTree.SelectedNode != null) LogFile.WriteLine(". selected= " + MapTree.SelectedNode.Text);

			e.Cancel  = (SaveAlertMap()    == DialogResult.Cancel);
			e.Cancel |= (SaveAlertRoutes() == DialogResult.Cancel); // NOTE: that bitwise had better execute ....
		}

		/// <summary>
		/// Loads the selected Map.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMapTreeAfterSelected(object sender, TreeViewEventArgs e)
		{
			//LogFile.WriteLine("XCMainWindow.OnMapTreeAfterSelected");
			//if (MapTree.SelectedNode != null) LogFile.WriteLine(". selected= " + MapTree.SelectedNode.Text);

			ClearSearched();
			LoadSelectedDescriptor();

			_selected = e.Node;
		}

		/// <summary>
		/// Caches the currently selected treenode.
		/// </summary>
		private TreeNode _selected;

		/// <summary>
		/// If user clicks on an already selected node, for which the Mapfile
		/// has not been loaded, this handler offers to show a dialog for the
		/// user to browse to the file.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMapTreeNodeClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			if (e.Node == _selected)
			{
				var descriptor = e.Node.Tag as Descriptor;
				if (descriptor != null
					&& (   MainViewUnderlay.MapBase == null
						|| MainViewUnderlay.MapBase.Descriptor != descriptor))
				{
					ClearSearched();
					LoadSelectedDescriptor(true);
				}
			}
		}

		/// <summary>
		/// Reloads the Map/Routes/Terrains when a save is done in PckView or
		/// McdView (via TileView).
		/// @note Is double-purposed to reload the Map/Routes/Terrains when user
		/// chooses to reload the current Map et al. on the File menu.
		/// TODO: Neither event really needs to reload the Map/Routes (in fact
		/// it would be better if it didn't so that the SaveAlerts could be
		/// bypassed) - so this function ought be reworked to reload only the
		/// Terrains (MCDs/PCKs/TABs). But that's a headache and a half ...
		/// </summary>
		private void OnReloadDescriptor()
		{
			bool cancel  = (SaveAlertMap()    == DialogResult.Cancel);
				 cancel |= (SaveAlertRoutes() == DialogResult.Cancel); // NOTE: that bitwise had better execute ....

			if (!cancel)
				LoadSelectedDescriptor();
		}
		#endregion Events


		#region Methods
		/// <summary>
		/// Loads the Map that's selected in the Maptree.
		/// <param name="basepathDialog">true to force the find file dialog</param>
		/// </summary>
		private void LoadSelectedDescriptor(bool basepathDialog = false)
		{
			//LogFile.WriteLine("");
			//LogFile.WriteLine("XCMainWindow.LoadSelectedDescriptor");

			var descriptor = MapTree.SelectedNode.Tag as Descriptor;
			if (descriptor != null)
			{
				//LogFile.WriteLine(". descriptor= " + descriptor);

				bool treechanged = false;
				var @base = MapFileService.LoadDescriptor( // NOTE: LoadDescriptor() instantiates a MapFile but whatver.
														descriptor,
														ref treechanged,
														basepathDialog);
				if (treechanged) MaptreeChanged = true;

				if (@base != null)
				{
					miSaveAll   .Enabled =
					miSaveMap   .Enabled =
					miSaveRoutes.Enabled =
					miSaveAs    .Enabled =
					miScreenshot.Enabled =
					miModifySize.Enabled =
					miReload    .Enabled =
					miMapInfo   .Enabled = true;

					MainViewOverlay.FirstClick = false;

					if (descriptor.Pal == Palette.TftdBattle) // used by Mono only ->
					{
						MainViewOverlay.SpriteBrushes = Palette.BrushesTftdBattle;
					}
					else // default to ufo-battle palette
						MainViewOverlay.SpriteBrushes = Palette.BrushesUfoBattle;

					MainViewUnderlay.MapBase = @base;

					ObserverManager.ToolFactory.EnableScaleAutoButton();
					ObserverManager.ToolFactory.SetLevelButtonsEnabled(@base.Level, @base.MapSize.Levs);

					Text = TITLE + " " + descriptor.Basepath;
					if (MaptreeChanged) MaptreeChanged = MaptreeChanged; // maniacal laugh YOU figure it out.

					tsslMapLabel     .Text = descriptor.Label;
					tsslDimensions   .Text = @base.MapSize.ToString();
					tsslPosition     .Text =
					tsslSelectionSize.Text = String.Empty;

					MapChanged = (@base as MapFile).IsLoadChanged; // don't bother to reset IsLoadChanged.

					var routeview1 = ObserverManager.RouteView.Control;
					var routeview2 = ObserverManager.TopRouteView.ControlRoute;

					routeview1.ClearSelectedInfo();
					routeview2.ClearSelectedInfo();

					routeview1.DisableOg();
					routeview2.DisableOg();

					Options[MainViewOptionables.str_OpenDoors].Value = // close doors; not necessary but keeps user's head on straight.
					Optionables.OpenDoors = false;
					SetDoorSpritesFullPhase(false);
					if (_foptions != null && _foptions.Visible)
					{
//						(_foptions as OptionsForm).propertyGrid.SetSelectedValue(false);
						(_foptions as OptionsForm).propertyGrid.Refresh();
					}

					if (!menuViewers.Enabled) // show the forms that are flagged to show (in MainView's Options).
						MenuManager.StartSecondStageRockets();

					ObserverManager.SetObservers(@base); // reset all observer events

					if (RouteCheckService.CheckNodeBounds(@base as MapFile))
					{
						routeview1.RoutesChanged =
						routeview2.RoutesChanged = true;
					}

					Globals.Scale = Globals.Scale; // enable/disable the scale-in/scale-out buttons

					if (ScanG != null) // update ScanG viewer if open
						ScanG.LoadMapfile(@base);

					var tileview = ObserverManager.TileView.Control; // update MCD Info if open
					if (tileview.McdInfobox != null)
					{
						Tilepart part = tileview.SelectedTilepart;
						if (part != null)
							tileview.McdInfobox.UpdateData(
														part.Record,
														part.TerId,
														tileview.GetTerrainLabel());
						else
							tileview.McdInfobox.UpdateData();
					}

					if (RouteView.RoutesInfo != null)
						RouteView.RoutesInfo.Initialize(@base as MapFile);

					ResetQuadrantPanel();

					Inited = false;
					Activate();
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		private void ResetQuadrantPanel()
		{
			var quadrantpanel1 = ObserverManager.TopView     .Control   .QuadrantPanel;
			var quadrantpanel2 = ObserverManager.TopRouteView.ControlTop.QuadrantPanel;

			quadrantpanel1.Tile =
			quadrantpanel2.Tile = null;

			quadrantpanel1.Loc =
			quadrantpanel2.Loc = null;

			QuadrantDrawService.CurrentTilepart = ObserverManager.TileView.Control.SelectedTilepart;

			quadrantpanel1.Invalidate();
			quadrantpanel2.Invalidate();
		}


		/// <summary>
		/// Sets door-sprites to fullphase or firstphase.
		/// </summary>
		/// <param name="full">true to animate any doors</param>
		internal void SetDoorSpritesFullPhase(bool full)
		{
			if (MainViewUnderlay.MapBase != null) // NOTE: MapBase is null on MapView load.
			{
				foreach (Tilepart part in MainViewUnderlay.MapBase.Parts)
					part.ToggleDoorSprites(full);
			}
		}

		/// <summary>
		/// Sets door-sprites to their alternate sprite's firstphase.
		/// </summary>
		internal void SetDoorSpritesAlternate()
		{
			if (MainViewUnderlay.MapBase != null) // NOTE: MapBase is null on MapView load.
			{
				foreach (Tilepart part in MainViewUnderlay.MapBase.Parts)
					part.SpritesToAlternate();
			}
		}

		/// <summary>
		/// Shows the user a dialog-box asking to Save if the currently
		/// displayed Map has changed.
		/// NOTE: Is called when either (a) MapView is closing (b) another Map
		/// is about to load.
		/// </summary>
		/// <returns></returns>
		private DialogResult SaveAlertMap()
		{
			if (MainViewUnderlay.MapBase != null && MainViewUnderlay.MapBase.MapChanged)
			{
				switch (MessageBox.Show(
									this,
									"Do you want to save changes to the Map?",
									" Map Changed",
									MessageBoxButtons.YesNoCancel,
									MessageBoxIcon.Question,
									MessageBoxDefaultButton.Button1,
									0))
				{
					case DialogResult.Yes:		// save & clear MapChanged flag
						MainViewUnderlay.MapBase.SaveMap();
						goto case DialogResult.No;

					case DialogResult.No:		// don't save & clear MapChanged flag
						MapChanged = false;
						break;

					case DialogResult.Cancel:	// dismiss confirmation dialog & leave state unaffected
						return DialogResult.Cancel;
				}
			}
			return DialogResult.OK;
		}

		/// <summary>
		/// Shows the user a dialog-box asking to Save if the currently
		/// displayed Routes has changed.
		/// NOTE: Is called when either (a) MapView is closing (b) another Map
		/// is about to load.
		/// </summary>
		/// <returns></returns>
		private DialogResult SaveAlertRoutes()
		{
			if (MainViewUnderlay.MapBase != null && MainViewUnderlay.MapBase.RoutesChanged)
			{
				switch (MessageBox.Show(
									this,
									"Do you want to save changes to the Routes?",
									" Routes Changed",
									MessageBoxButtons.YesNoCancel,
									MessageBoxIcon.Question,
									MessageBoxDefaultButton.Button1,
									0))
				{
					case DialogResult.Yes:		// save & clear RoutesChanged flag
						MainViewUnderlay.MapBase.SaveRoutes();
						goto case DialogResult.No;

					case DialogResult.No:		// don't save & clear RoutesChanged flag
						ObserverManager.RouteView   .Control     .RoutesChanged =
						ObserverManager.TopRouteView.ControlRoute.RoutesChanged = false;
						break;

					case DialogResult.Cancel:	// dismiss confirmation dialog & leave state unaffected
						return DialogResult.Cancel;
				}
			}
			return DialogResult.OK;
		}

		/// <summary>
		/// Shows the user a dialog-box asking to Save the Maptree if it has
		/// changed.
		/// NOTE: Is called when either (a) MapView is closing (b) MapView is
		/// reloading due to a configuration change (ie. only if resource-paths
		/// have been changed, since the only other relevant option - if the
		/// tilesets-config file - is changed then saving the current one is
		/// pointless).
		/// </summary>
		/// <returns></returns>
		private DialogResult SaveAlertMaptree()
		{
			if (MaptreeChanged)
			{
				switch (MessageBox.Show(
									this,
									"Do you want to save changes to the Map Tree?",
									" Maptree Changed",
									MessageBoxButtons.YesNoCancel,
									MessageBoxIcon.Question,
									MessageBoxDefaultButton.Button1,
									0))
				{
					case DialogResult.Yes:		// save & clear MaptreeChanged flag
						OnSaveMaptreeClick(null, EventArgs.Empty);
						break;

					case DialogResult.No:		// don't save & clear MaptreeChanged flag
						MaptreeChanged = false; // kinda irrelevant since this class-object is about to disappear.
						break;

					case DialogResult.Cancel:	// dismiss confirmation dialog & leave state unaffected
						return DialogResult.Cancel;
				}
			}
			return DialogResult.OK;
		}

		/// <summary>
		/// Prints the currently selected location to the status bar.
		/// NOTE: The 'lev' should be inverted before it's passed in.
		/// </summary>
		internal void sb_PrintPosition()
		{
			if (MainViewOverlay.FirstClick)
			{
				MapFileBase @base = MainViewUnderlay.MapBase;
				MapLocation loc = @base.Location;
				tsslPosition.Text = String.Format(
												CultureInfo.CurrentCulture,
												"c {0}  r {1}  L {2}",
												loc.Col + 1, loc.Row + 1,
												@base.MapSize.Levs - @base.Level); // 1-based count.
			}
		}

		internal void sb_PrintScale()
		{
			tsslScale.Text = String.Format(
										CultureInfo.CurrentCulture,
										"scale {0:0.00}",
										Globals.Scale);
		}

		internal void sb_PrintSelectionSize(int tx, int ty)
		{
			tsslSelectionSize.Text = String.Format(
												CultureInfo.CurrentCulture,
												"{0} x {1}",
												tx, ty);
			ssMain.Refresh(); // fast update for selection-size.
		}
		#endregion Methods
	}



	/// <summary>
	/// Derived class for TreeView.
	/// </summary>
	internal sealed class CompositedTreeView
		:
			TreeView
	{
		#region Properties (override)
		/// <summary>
		/// Prevents flicker.
		/// </summary>
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.ExStyle |= 0x02000000; // enable 'WS_EX_COMPOSITED'
				return cp;
			}
		}
		#endregion Properties (override)
	}



	#region Delegates
	/// <summary>
	/// Good fuckin Lord I just wrote a "DontBeep" delegate.
	/// </summary>
	internal delegate void DontBeepEventHandler();
	#endregion
}
