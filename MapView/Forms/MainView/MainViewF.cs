using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

using DSShared;
using DSShared.Controls;

using MapView.Forms.MainView;
using MapView.Forms.Observers;

using XCom;

using YamlDotNet.RepresentationModel; // read values (deserialization)


namespace MapView
{
	/// <summary>
	/// Instantiates a MainView screen as the basis for all user-interaction.
	/// </summary>
	internal sealed partial class MainViewF
		:
			Form
	{
		#region Delegates
		/// <summary>
		/// Good fuckin Lord I just wrote a "DontBeep" delegate.
		/// </summary>
		internal delegate void DontBeepEventHandler();
		#endregion Delegates


		#region Events
		internal event DontBeepEventHandler DontBeepEvent;
		#endregion Events


		#region Fields (static)
		private const string TITLE = "Map Editor ||";

		private const int TREELEVEL_GROUP    = 0;
		private const int TREELEVEL_CATEGORY = 1;
		private const int TREELEVEL_TILESET  = 2;
		#endregion Fields (static)


		#region Fields
		private CompositedTreeView MapTree;

		internal Options Options;
		#endregion Fields


		#region Properties (static)
		internal static MainViewF that
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

		internal static Spriteset MonotoneSprites
		{ get; private set; }
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
					if (!Text.EndsWith(GlobalsXC.PADDED_ASTERISK, StringComparison.Ordinal))
						Text += GlobalsXC.PADDED_ASTERISK;
				}
				else if (Text.EndsWith(GlobalsXC.PADDED_ASTERISK, StringComparison.Ordinal))
					Text = Text.Substring(0, Text.Length - GlobalsXC.PADDED_ASTERISK.Length);
			}
		}

		/// <summary>
		/// Sets the MapChanged flag. This is only an intermediary that adds an
		/// asterisk to the file-label in MainView's statusbar; the real
		/// MapChanged flag is stored in <see cref="MapFile"/>. reasons.
		/// </summary>
		internal bool MapChanged
		{
			set
			{
				string text = tsslMapLabel.Text;
				if (MainViewUnderlay.MapFile.MapChanged = value) // shuffle the value down to MapFile.MapChanged ...
				{
					if (!text.EndsWith(GlobalsXC.PADDED_ASTERISK, StringComparison.Ordinal))
						text += GlobalsXC.PADDED_ASTERISK;
				}
				else if (text.EndsWith(GlobalsXC.PADDED_ASTERISK, StringComparison.Ordinal))
					text = text.Substring(0, text.Length - GlobalsXC.PADDED_ASTERISK.Length);

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
		internal MainViewF()
		{
			Cursor.Current = Cursors.AppStarting;


			string dirAppL = Path.GetDirectoryName(Application.ExecutablePath);
			string dirSetT = Path.Combine(dirAppL, PathInfo.DIR_Settings);
#if DEBUG
			LogFile.SetLogFilePath(dirAppL); // creates a logfile/wipes the old one.
#endif

			LogFile.WriteLine("Instantiating MAIN MapView window ...");

			// TODO: Either move all this SharedSpace stuff to DSShared so it
			// can be implemented/instantiated for Mcd/PckView also, or better
			// get rid of it (or at least de-spaghettify it as much as possible).

			SharedSpace.SetShare(SharedSpace.ApplicationDirectory, dirAppL);
			SharedSpace.SetShare(SharedSpace.SettingsDirectory,    dirSetT);

			LogFile.WriteLine("App paths cached.");


			// TODO: The .NET framework already has a class for "PathInfo":
			// https://docs.microsoft.com/en-us/dotnet/api/system.io.fileinfo?view=netframework-4.8
			// ie. FileInfo.

			var piOptions   = new PathInfo(dirSetT, PathInfo.CFG_Options);		// define a PathInfo for MapOptions.cfg
			var piResources = new PathInfo(dirSetT, PathInfo.YML_Resources);	// define a PathInfo for MapResources.yml
			var piTilesets  = new PathInfo(dirSetT, PathInfo.YML_Tilesets);		// define a PathInfo for MapTilesets.yml
			var piViewers   = new PathInfo(dirSetT, PathInfo.YML_Viewers);		// define a PathInfo for MapViewers.yml

			SharedSpace.SetShare(PathInfo.ShareOptions,   piOptions);			// set share for MapOptions.cfg
			SharedSpace.SetShare(PathInfo.ShareResources, piResources);			// set share for MapResources.yml
			SharedSpace.SetShare(PathInfo.ShareTilesets,  piTilesets);			// set share for MapTilesets.yml
			SharedSpace.SetShare(PathInfo.ShareViewers,   piViewers);			// set share for MapViewers.yml

			LogFile.WriteLine("PathInfo cached.");


			// Check if MapTilesets.yml and MapResources.yml exist yet, show the
			// Configuration window if not.
			// NOTE: MapResources.yml and MapTilesets.yml are created by ConfigurationForm.
			if (!piResources.FileExists() || !piTilesets.FileExists())
			{
				LogFile.WriteLine("Resources or Tilesets file does not exist: run configurator.");

				using (var f = new ConfigurationForm())
					f.ShowDialog(this);
			}
			else
				LogFile.WriteLine("Resources and Tilesets files exist.");


			// Exit app if either MapResources.yml or MapTilesets.yml doesn't exist
			if (!piResources.FileExists() || !piTilesets.FileExists()) // safety. The Configurator shall demand that both these files get created.
			{
				LogFile.WriteLine("Resources or Tilesets file does not exist: quit MapView.");
				using (var dialog = new Infobox(
											"Error",
											"Cannot find configuration files. The application will exit.",
											null,
											Infobox.BoxType.Error))
				{
					dialog.ShowDialog(this);
				}
				Process.GetCurrentProcess().Kill();
			}


			// Check if settings/MapViewers.yml exists yet, if not create it
			if (!piViewers.FileExists())
			{
				if (CopyViewersFile(piViewers.Fullpath))
					LogFile.WriteLine("Viewers file created.");
				else
					LogFile.WriteLine("Viewers file could not be created.");
			}
			else
				LogFile.WriteLine("Viewers file exists.");



			InitializeComponent(); // ffs. This fires OnActivated but the Optionables aren't ready yet.
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
			MapTree.AfterSelect    += OnMapTreeAfterSelect;
			MapTree.NodeMouseClick += OnMapTreeNodeMouseClick;
//			MapTree.NodeMouseClick += (sender, args) => MapTree.SelectedNode = args.Node;

			Controls.Add(MapTree);
			splitter.Collapsible = MapTree;

			MapTree.TreeViewNodeSorter = StringComparer.Ordinal;


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


			QuadrantDrawService.initQuadrantDrawService();


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


			Palette.UfoBattle .SetTransparent(true); // WARNING: ufo/tftd Palettes created here ->
			LogFile.WriteLine("ufo-battle Palette instantiated.");
			Palette.TftdBattle.SetTransparent(true);
			LogFile.WriteLine("tftd-battle Palette instantiated.");
			LogFile.WriteLine("Palette transparencies set.");

			MonotoneSprites = EmbeddedService.CreateMonotoneSpriteset();	// sprites for TileView's eraser and QuadrantControl's blank quads.
																			// NOTE: transparency of the 'UfoBattle' palette must be set first.
			LogFile.WriteLine("Monotone sprites loaded.");


			ObserverManager.CreateViewers(); // adds each subsidiary viewer's options and Options-type etc.
			LogFile.WriteLine("ObserverManager initialized.");

			ObserverManager.TileView.Control.ReloadDescriptor += OnReloadDescriptor;


			tscPanel.ContentPanel.Controls.Add(MainViewUnderlay);

			tsTools.SuspendLayout();
			ObserverManager.ToolFactory.CreateSearchTools(tsTools);
			ObserverManager.ToolFactory.CreateScalerTools(tsTools);
			ObserverManager.ToolFactory.CreateEditorTools(tsTools);
			tsTools.ResumeLayout();
			LogFile.WriteLine("MainView toolstrip created.");

			MenuManager.Initialize(menuViewers);
			MenuManager.PopulateMenu();
			LogFile.WriteLine("Viewers menu populated.");


			PathInfo piScanGufo  = null;
			PathInfo piScanGtftd = null;

			// Read MapResources.yml to get the resources dir (for both UFO and TFTD).
			// NOTE: MapResources.yml is created by ConfigurationForm
			using (var fs = FileService.OpenFile(piResources.Fullpath))
			if (fs != null)
			using (var sr = new StreamReader(fs))
			{
				var str = new YamlStream();
				str.Load(sr);

				string key, val;

				var nodeRoot = str.Documents[0].RootNode as YamlMappingNode;
				foreach (var node in nodeRoot.Children)
				{
					switch (node.Key.ToString())
					{
						// NOTE: These do not check if Directory.Exists().

						case "ufo":
							key = SharedSpace.ResourceDirectoryUfo;
							val = node.Value.ToString();
							if (val == PathInfo.NotConfigured)
								val = null;
							else
								piScanGufo = new PathInfo(Path.Combine(val, SharedSpace.ScanGfile));

							SharedSpace.SetShare(key, val);
							break;

						case "tftd":
							key = SharedSpace.ResourceDirectoryTftd;
							val = node.Value.ToString();
							if (val == PathInfo.NotConfigured)
								val = null;
							else
								piScanGtftd = new PathInfo(Path.Combine(val, SharedSpace.ScanGfile));

							SharedSpace.SetShare(key, val);
							break;
					}
				}
			}


			// Setup an XCOM cursor-sprite.
			// NOTE: This is the only stock XCOM resource that is required for
			// MapView to start. See ConfigurationForm ...
			// TODO: give user the option to choose which cursor-spriteset to use.
			Spriteset cuboidufo  = null;
			Spriteset cuboidtftd = null;
			string label = SharedSpace.CursorFilePrefix;
			string dir;

			if (!String.IsNullOrEmpty(dir = SharedSpace.GetShareString(SharedSpace.ResourceDirectoryUfo))
				&& Directory.Exists(Path.Combine(dir, GlobalsXC.UfographDir)))
			{
				cuboidufo = SpritesetManager.LoadSpriteset(
														label,
														dir,
														SpritesetManager.TAB_WORD_LENGTH_2,
														Palette.UfoBattle,
														true);

				if (cuboidufo != null)
				{
					if (cuboidufo.Fail != Spriteset.FAIL_non)
					{
						cuboidufo = null;
						LogFile.WriteLine("UFO Cursor failed to load.");
					}
					else
						LogFile.WriteLine("UFO Cursor loaded.");
				}
			}
			else
				LogFile.WriteLine("UFO Cursor directory not found.");

			if (!String.IsNullOrEmpty(dir = SharedSpace.GetShareString(SharedSpace.ResourceDirectoryTftd))
				&& Directory.Exists(Path.Combine(dir, GlobalsXC.UfographDir)))
			{
				cuboidtftd = SpritesetManager.LoadSpriteset(
														label,
														dir,
														SpritesetManager.TAB_WORD_LENGTH_4,
														Palette.TftdBattle,
														true);

				if (cuboidtftd != null)
				{
					if (cuboidtftd.Fail != Spriteset.FAIL_non)
					{
						cuboidtftd = null;
						LogFile.WriteLine("TFTD Cursor failed to load.");
					}
					else
						LogFile.WriteLine("TFTD Cursor loaded.");
				}
			}
			else
				LogFile.WriteLine("TFTD Cursor directory not found.");

			// Exit app if a cuboid-targeter did not get instantiated
			if (cuboidtftd != null) // NOTE: The TFTD cursorsprite takes precedence over the UFO cursorsprite.
			{
				CuboidSprite.Cursorset = cuboidtftd;
			}
			else if (cuboidufo != null)
			{
				CuboidSprite.Cursorset = cuboidufo;
			}
			else
			{
				LogFile.WriteLine("Targeter not instantiated: quit MapView.");

				string copyable = SharedSpace.CursorFilePrefix + GlobalsXC.PckExt
								+ Environment.NewLine
								+ SharedSpace.CursorFilePrefix + GlobalsXC.TabExt;

				using (var dialog = new Infobox(
											"Error",
											"Cannot find CURSOR spriteset. The application will exit.",
											copyable,
											Infobox.BoxType.Error))
				{
					dialog.ShowDialog(this);
				}
				Process.GetCurrentProcess().Kill();
			}


			// NOTE: ScanG's are conditional loads iff File exists.
			if (piScanGufo != null && piScanGufo.FileExists()
				&& SpritesetManager.LoadScanGufo(SharedSpace.GetShareString(SharedSpace.ResourceDirectoryUfo)))
			{
				LogFile.WriteLine("ScanG UFO loaded.");
			}
			else
				LogFile.WriteLine("ScanG UFO not found.");

			if (piScanGtftd != null && piScanGtftd.FileExists()
				&& SpritesetManager.LoadScanGtftd(SharedSpace.GetShareString(SharedSpace.ResourceDirectoryTftd)))
			{
				LogFile.WriteLine("ScanG TFTD loaded.");
			}
			else
				LogFile.WriteLine("ScanG TFTD not found.");


			TileGroupManager.LoadTileGroups(piTilesets.Fullpath); // load resources from YAML.
			LogFile.WriteLine("Tilesets loaded/Descriptors created.");


			if (piOptions.FileExists()) // NOTE: load user-options before MenuManager.StartSecondaryStageBoosters() in LoadSelectedDescriptor()
			{
				if (OptionsManager.LoadUserOptions(piOptions.Fullpath))
					LogFile.WriteLine("User options loaded.");
				else
					LogFile.WriteLine("User options could not be opened.");
			}
			else
				LogFile.WriteLine("User options NOT loaded - no options file to load.");


			CreateTree();
			LogFile.WriteLine("Maptree instantiated.");


			splitter.SetClickableRectangle();
			ShiftSplitter();


			DontBeepEvent += FireContext;

			ssMain.Renderer = new CustomToolStripRenderer();


			Cursor.Current = Cursors.Default;
			LogFile.WriteLine("About to show MainView ..." + Environment.NewLine);
		}
		#endregion cTor


		#region Methods (static)
		/// <summary>
		/// Transposes all the default viewer positions and sizes from the
		/// embedded MapViewers manifest to a file: settings/MapViewers.yml.
		/// </summary>
		/// <param name="fullpath"></param>
		/// <returns>true on success</returns>
		private static bool CopyViewersFile(string fullpath)
		{
			using (var fs = FileService.CreateFile(fullpath))
			if (fs != null)
			using (var sw = new StreamWriter(fs))
			using (var sr = new StreamReader(Assembly.GetExecutingAssembly()
													 .GetManifestResourceStream(PathInfo.MAN_Viewers)))
			{
				string line;
				while ((line = sr.ReadLine()) != null)
					sw.WriteLine(line);

				return true;
			}
			return false;
		}
		#endregion Methods (static)


		#region Create tree
		/// <summary>
		/// Creates the Map-tree on the left side of MainView.
		/// </summary>
		private void CreateTree()
		{
			//LogFile.WriteLine("");
			//LogFile.WriteLine("MainViewF.CreateTree");

			MapTree.BeginUpdate();
			MapTree.Nodes.Clear();

			var groups = TileGroupManager.TileGroups;
			//LogFile.WriteLine(". groups= " + groups);

			SortableTreeNode nodeGroup;
			TileGroup tileGroup;

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
		/// A TreeNode that can be sorted.
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

			/// <summary>
			/// Required by IComparable.
			/// </summary>
			/// <param name="other"></param>
			/// <returns></returns>
			public int CompareTo(object other)
			{
				return String.Compare(
									Text,
									(other as SortableTreeNode).Text,
									StringComparison.Ordinal);
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
				widthTest = TextRenderer.MeasureText(node0.Text, MapTree.Font).Width + 30;
				if (widthTest > width)
					width = widthTest;

				foreach (TreeNode node1 in node0.Nodes)
				{
					widthTest = TextRenderer.MeasureText(node1.Text, MapTree.Font).Width + 60;
					if (widthTest > width)
						width = widthTest;

					foreach (TreeNode node2 in node1.Nodes)
					{
						widthTest = TextRenderer.MeasureText(node2.Text, MapTree.Font).Width + 90;
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
						if (!MainViewF.Quit)
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
		/// Handles CL-args after Configurator restart - selects a node in the
		/// Maptree.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);

			switch (Program.Args.Length)
			{
				case 0:
					if (MapTree.SelectedNode != null)
						MapTree.SelectedNode.Collapse(false);
					break;

				case 1:
					SelectGroupNode(Program.Args[TREELEVEL_GROUP]);
					break;

				case 2:
					SelectCategoryNode(
									Program.Args[TREELEVEL_CATEGORY],
									Program.Args[TREELEVEL_GROUP]);
					break;

				case 3:
					SelectTilesetNode(
									Program.Args[TREELEVEL_TILESET],
									Program.Args[TREELEVEL_CATEGORY],
									Program.Args[TREELEVEL_GROUP]);
					break;
			}

			TopMost = true;		// NOTE: MapView could be hidden behind other
			TopMost = false;	//       open windows after a forced reload.
		}


		private static bool FirstActivated; // on 1st Activated keep the tree focused, on 2+ focus the panel
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

			if (   MainViewF.Optionables != null
				&& MainViewF.Optionables.BringAllToFront)
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

			if (FirstActivated)
				MainViewOverlay.Focus();
			else
				FirstActivated = true;

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
		/// This has nothing to do with the Registry anymore, but it saves
		/// MainView's Options as well as its screen-location and -size to YAML
		/// when the app closes.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			if (!RegistryInfo.FastClose(e.CloseReason))
			{
				Quit = true;

				if (SaveAlertMap() == DialogResult.Cancel) // NOTE: do not short-circuit these ->
					Quit = false;

				if (SaveAlertRoutes() == DialogResult.Cancel)
					Quit = false;

				if (SaveAlertMaptree() == DialogResult.Cancel)
					Quit = false;

				if (Quit)
				{
					SafeQuit();

					// kL_note: This is for storing MainView's location and size in
					// the Windows Registry:
//					if (PathsEditor.SaveRegistry)
//					{
//						using (var keySoftware = Registry.CurrentUser.CreateSubKey(DSShared.Windows.RegistryInfo.SoftwareRegistry))
//						using (var keyMapView = keySoftware.CreateSubKey(DSShared.Windows.RegistryInfo.MapViewRegistry))
//						using (var keyMainView = keyMapView.CreateSubKey("MainView"))
//						{
//							keyMainView.SetValue("Left",   Left);
//							keyMainView.SetValue("Top",    Top);
//							keyMainView.SetValue("Width",  Width);
//							keyMainView.SetValue("Height", Height - SystemInformation.CaptionButtonSize.Height); ps. not
//							keyMainView.Close();
//							keyMapView.Close();
//							keySoftware.Close();
//						}
//					}
				}
				else
					e.Cancel = true;
			}
			base.OnFormClosing(e);
		}

		/// <summary>
		/// Saves out Options and Registry as well as closes any open viewers
		/// and miscellany.
		/// </summary>
		private void SafeQuit()
		{
			OptionsManager.SaveOptions();	// save MV_OptionsFile // TODO: do SaveOptions() every time an Options form closes.
			OptionsManager.CloseOptions();	// close any open Options windows

			if ( ScanG   != null)  ScanG  .Close(); // close ScanG
			if (_fcolors != null) _fcolors.Close(); // close ColorsHelp
			if (_fabout  != null) _fabout .Close(); // close About
			if (_finfo   != null) _finfo  .Close(); // close MapInfo and its Detail dialog

			// NOTE: TopView's PartslotTest dialog is closed when TopView closes.
			// TODO: McdRecordsExceeded dialog is pseudo-static ... close it (if it was instantiated).

			if (ObserverManager.TileView.Control.McdInfobox != null)
				ObserverManager.TileView.Control.McdInfobox.Close(); // close TileView's McdInfo dialog

			if (RouteView.RoutesInfo != null)
				RouteView.RoutesInfo.Close();	// close RouteView's SpawnInfo dialog

			ObserverManager.CloseViewers();		// close secondary viewers (TileView, TopView, RouteView, TopRouteView)

			QuadrantDrawService.Dispose();

			RegistryInfo.UpdateRegistry(this);	// save MainView's location and size
			RegistryInfo.WriteRegistry();		// write all registered windows' locations and sizes to file
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
			//LogFile.WriteLine("MainViewF.ProcessCmdKey() " + keyData);

			bool invalidate = false;
			bool focusearch = false;

			switch (keyData)
			{
				case Keys.Tab:
					invalidate = true;
					focusearch = MainViewOverlay.Focused;
					break;

				case Keys.Shift | Keys.Tab:
					invalidate = true;
					focusearch = MapTree.Focused;
					break;

				case Keys.Shift | Keys.F3:
					invalidate = true;
					focusearch = true;
					break;
			}

			if (invalidate)
			{
				MainViewOverlay.Invalidate();

				if (focusearch)
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
			//LogFile.WriteLine("MainViewF.OnKeyDown() " + e.KeyData);

			string key; object val = null;
			ToolStripMenuItem it = null;
			int id = MenuManager.MI_non;

			switch (e.KeyData)
			{
				case Keys.Space: // open Context
					if (MapTree.Focused && _selected != null)
					{
						e.SuppressKeyPress = true;

						_dontbeeptype = DontBeepType.OpenContext;
						BeginInvoke(DontBeepEvent);
					}
					break;

				case Keys.Enter: // load Descriptor (do NOT reload)
					if (MapTree.Focused && _selected != null)
					{
						e.SuppressKeyPress = true;

						if (_selected.Level == TREELEVEL_TILESET)
						{
							var descriptor = _selected.Tag as Descriptor;
							if (   MainViewUnderlay.MapFile == null
								|| MainViewUnderlay.MapFile.Descriptor != descriptor)
							{
								_dontbeeptype = DontBeepType.LoadDescriptor;
								BeginInvoke(DontBeepEvent);
							}
						}
						else if (!_selected.IsExpanded)
						{
							_selected.Expand();
						}
						else
							_selected.Collapse();
					}
					break;

				case Keys.Shift | Keys.Enter: // open MapBrowserDialog
					if (MapTree.Focused
						&& _selected != null
						&& _selected.Level == TREELEVEL_TILESET)
					{
						e.SuppressKeyPress = true;

						var descriptor = _selected.Tag as Descriptor;
						if (MapFileService.MapfileExists(descriptor) == null)
						{
							_dontbeeptype = DontBeepType.MapBrowserDialog;
							BeginInvoke(DontBeepEvent);
						}
					}
					break;

				case Keys.F9: // cycle LayerSelectionBorder
					key = MainViewOptionables.str_LayerSelectionBorder;
					val = (Optionables.LayerSelectionBorder + 1) % 3;
					Options[key].Value = val;
					Optionables.OnOptionChanged(key,val);
					break;

				case Keys.F9 | Keys.Control: // toggle OneTileDraw
					key = MainViewOptionables.str_OneTileDraw;
					val = !Optionables.OneTileDraw;
					Options[key].Value = val;
					Optionables.OnOptionChanged(key,val);
					break;

				case Keys.F10:				// cycle tiletoner option forward
					key = MainViewOptionables.str_SelectedTileToner;
					val = Optionables.GetNextTileToner(+1);
					Options[key].Value = val;
					Optionables.OnOptionChanged(key,val);
					break;

				case Keys.Shift | Keys.F10:	// cycle tiletoner option reverse
					key = MainViewOptionables.str_SelectedTileToner;
					val = Optionables.GetNextTileToner(-1);
					Options[key].Value = val;
					Optionables.OnOptionChanged(key,val);
					break;

				case Keys.F2:
					key = MainViewOptionables.str_AnimateSprites;
					val = !MainViewF.Optionables.AnimateSprites;
					Options[key].Value = val;
					Optionables.OnSpriteStateChanged(key,val);
					break;

				case Keys.F3:
					key = MainViewOptionables.str_OpenDoors;
					val = !MainViewF.Optionables.OpenDoors;
					Options[key].Value = val;
					Optionables.OnSpriteStateChanged(key,val);
					break;

				case Keys.F4:
					key = MainViewOptionables.str_GridVisible;
					val = !MainViewF.Optionables.GridVisible;
					Options[key].Value = val;
					Optionables.OnOptionChanged(key,val);
					break;

				// toggle TopView tilepart visibilities ->
				case Keys.Control | Keys.F1:
					it = ObserverManager.TopView.Control.Floor;
					break;

				case Keys.Control | Keys.F2:
					it = ObserverManager.TopView.Control.West;
					break;

				case Keys.Control | Keys.F3:
					it = ObserverManager.TopView.Control.North;
					break;

				case Keys.Control | Keys.F4:
					it = ObserverManager.TopView.Control.Content;
					break;

				// focus viewer (show/hide shortcuts are handled by menuitems directly) ->
				case Keys.Control | Keys.F5: id = MenuManager.MI_TILE;     break;
				case Keys.Control | Keys.F6: id = MenuManager.MI_TOP;      break;
				case Keys.Control | Keys.F7: id = MenuManager.MI_ROUTE;    break;
				case Keys.Control | Keys.F8: id = MenuManager.MI_TOPROUTE; break;

				case Keys.Subtract:
				case Keys.Add:
				case Keys.Home:
				case Keys.End:
				case Keys.PageUp:
				case Keys.PageDown:
				case Keys.Shift | Keys.Home:
				case Keys.Shift | Keys.End:
				case Keys.Shift | Keys.PageUp:
				case Keys.Shift | Keys.PageDown:
					if (MainViewOverlay.Focused)
					{
						e.SuppressKeyPress = true;
						MainViewOverlay.Navigate(e.KeyData);
					}
					break;
			}

			if (val != null)
			{
				e.SuppressKeyPress = true;

				if (_foptions != null && _foptions.Visible)
					(_foptions as OptionsForm).propertyGrid.Refresh();
			}
			else if (menuViewers.Enabled)
			{
				if (it != null)
				{
					e.SuppressKeyPress = true;
					ObserverManager.TopView.Control.OnQuadrantVisibilityClick(it, EventArgs.Empty);
				}
				else if (id != -1)
				{
					e.SuppressKeyPress = true;
					MenuManager.OnMenuItemClick(menuViewers.MenuItems[id], EventArgs.Empty);
				}
			}

			base.OnKeyDown(e);
		}
		#endregion Events (override)


		#region Events
		/// <summary>
		/// Draws treenodes on the Maptree.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
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

				int width = TextRenderer.MeasureText(e.Node.Text, e.Node.TreeView.Font).Width;
				while (width / 70 != 0)
				{
					width -= 70;
					++rect.Width;
				}

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


		/// <summary>
		/// Handles a save-all click.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnSaveAllClick(object sender, EventArgs e)
		{
			if (MainViewUnderlay.MapFile != null)
			{
				if (MainViewUnderlay.MapFile.SaveMap())
				{
					MapChanged = false;

					if (MainViewUnderlay.MapFile.ForceReload)
						ForceMapReload();
				}

				if (MainViewUnderlay.MapFile.SaveRoutes())
					RouteView.RoutesChangedCoordinator = false;
			}
			MaptreeChanged = !TileGroupManager.WriteTileGroups();
		}

		/// <summary>
		/// Handles a save-Mapfile click.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void OnSaveMapClick(object sender, EventArgs e)
		{
			if (MainViewUnderlay.MapFile != null
				&& MainViewUnderlay.MapFile.SaveMap())
			{
				MapChanged = false;

				if (MainViewUnderlay.MapFile.ForceReload)
					ForceMapReload();
			}
		}

		/// <summary>
		/// Handles a save-Routesfile click.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void OnSaveRoutesClick(object sender, EventArgs e)
		{
			if (   MainViewUnderlay.MapFile != null
				&& MainViewUnderlay.MapFile.SaveRoutes())
			{
				RouteView.RoutesChangedCoordinator = false;
			}
		}


		private string _lastExportDirectory;

		/// <summary>
		/// Handles an export-Routesfile click.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnExportMapRoutesClick(object sender, EventArgs e)
		{
			if (   MainViewUnderlay.MapFile != null
				&& MainViewUnderlay.MapFile.Descriptor != null)
			{
				using (var sfd = new SaveFileDialog())
				{
					sfd.Title      = "Export Map (and Routes) ...";
					sfd.Filter     = "Map files (*.MAP)|*.MAP|All files (*.*)|*.*";
					sfd.DefaultExt = GlobalsXC.MapExt;
					sfd.FileName   = MainViewUnderlay.MapFile.Descriptor.Label;

					if (!Directory.Exists(_lastExportDirectory))
					{
						string path = Path.Combine(MainViewUnderlay.MapFile.Descriptor.Basepath, GlobalsXC.MapsDir);
						if (Directory.Exists(path))
							sfd.InitialDirectory = path;
					}
					else
						sfd.InitialDirectory = _lastExportDirectory;


					if (sfd.ShowDialog() == DialogResult.OK)
					{
						string pfe = sfd.FileName;
						_lastExportDirectory = Path.GetDirectoryName(pfe);

						// NOTE: GetDirectoryName() will return a string ending with a
						// path-separator if it's the root dir, and without one if it's
						// not. But Path.Combine() doesn't figure out the difference.
						// woohoo ...

						if (_lastExportDirectory.EndsWith(GlobalsXC.MapsDir, StringComparison.OrdinalIgnoreCase))
						{
							string dir       = _lastExportDirectory.Substring(0, _lastExportDirectory.Length - GlobalsXC.MapsDir.Length - 1);
							string dirMaps   = Path.Combine(dir, GlobalsXC.MapsDir);
							string dirRoutes = Path.Combine(dir, GlobalsXC.RoutesDir);
							string label     = Path.GetFileNameWithoutExtension(pfe);

							MainViewUnderlay.MapFile.ExportMap(   Path.Combine(dirMaps,   label));
							MainViewUnderlay.MapFile.ExportRoutes(Path.Combine(dirRoutes, label));
						}
						else
						{
							using (var f = new Infobox(
													"Error",
													Infobox.SplitString("Maps must be saved to a directory MAPS."
															+ " Routes can then be saved to its sibling directory ROUTES."),
													null,
													Infobox.BoxType.Error))
							{
								f.ShowDialog(this);
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Saves the tree to "settings/MapTilesets.yml".
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnSaveMaptreeClick(object sender, EventArgs e)
		{
			MaptreeChanged = !TileGroupManager.WriteTileGroups();
		}

		/// <summary>
		/// Reloads the current tileset.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnReloadClick(object sender, EventArgs e)
		{
			OnReloadDescriptor();
		}

		/// <summary>
		/// Reloads the Map/Routes/Terrains when a save is done in PckView or
		/// McdView (via TileView).
		/// 
		/// TODO: Neither event really needs to reload the Map/Routes (in fact
		/// it would be better if it didn't so that the SaveAlerts could be
		/// bypassed) - so this function ought be reworked to reload only the
		/// Terrains (MCDs/PCKs/TABs). But that's a headache and a half ...
		/// 
		/// TODO: Actually there should be a separate ReloadTerrains() funct.
		/// </summary>
		/// <remarks>Is double-purposed to reload the Map/Routes/Terrains when
		/// user chooses to reload the current Map et al. on the File menu.</remarks>
		private void OnReloadDescriptor()
		{
			//LogFile.WriteLine("MainViewF.OnReloadDescriptor()");

			bool cancel  = (SaveAlertMap()    == DialogResult.Cancel);
				 cancel |= (SaveAlertRoutes() == DialogResult.Cancel); // NOTE: that bitwise had better execute ....

			if (!cancel)
			{
				_loadReady = LOADREADY_STAGE_2;
				LoadSelectedDescriptor();
			}
		}

		/// <summary>
		/// Call this only after the Mapfile was saved successfully.
		/// </summary>
		private void ForceMapReload()
		{
			//LogFile.WriteLine("MainViewF.ForceMapReload()");

			MainViewUnderlay.MapFile.ForceReload = false;

			_loadReady = LOADREADY_STAGE_2;
			LoadSelectedDescriptor(false, true);
		}


		private string _lastScreenshotDirectory;

		/// <summary>
		/// Handles a screenshot click.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnScreenshotClick(object sender, EventArgs e)
		{
			MapFile file = MainViewUnderlay.MapFile;
			if (file != null)
			{
				using (var sfd = new SaveFileDialog())
				{
					if (Optionables.Png_notGif)
					{
						sfd.Title      = "Save PNG Screenshot";
						sfd.Filter     = "PNG files|*.PNG|All files (*.*)|*.*";
						sfd.DefaultExt = GlobalsXC.PngExt;
					}
					else
					{
						sfd.Title      = "Save GIF Screenshot";
						sfd.Filter     = "GIF files|*.GIF|All files (*.*)|*.*";
						sfd.DefaultExt = GlobalsXC.GifExt;
					}

					string digits = String.Empty;
					int levs = file.Levs;
					do
					{ digits += "0"; }
					while ((levs /= 10) != 0);

					string suffix = String.Format(
												"_L{0:" + digits + "}",
												file.Levs - file.Level);
					sfd.FileName = file.Descriptor.Label + suffix;

					if (!Directory.Exists(_lastScreenshotDirectory))
					{
						string path = Path.Combine(MainViewUnderlay.MapFile.Descriptor.Basepath, GlobalsXC.MapsDir);
						if (Directory.Exists(path))
							sfd.InitialDirectory = path;
					}
					else
						sfd.InitialDirectory = _lastScreenshotDirectory;

					sfd.RestoreDirectory = true;


					if (sfd.ShowDialog(this) == DialogResult.OK)
					{
						_lastScreenshotDirectory = Path.GetDirectoryName(sfd.FileName);
						Screenshot(sfd.FileName);
					}
				}
			}
		}

		/// <summary>
		/// Takes a screenshot of the currently loaded Map.
		/// </summary>
		/// <param name="fullpath"></param>
		public void Screenshot(string fullpath)
		{
			const int ConstHalfWidth  = 16;
			const int ConstHalfHeight =  8;
			const int Layers          = 24;


			MapFile file = MainViewUnderlay.MapFile;
			int level = file.Level;

			int width = file.Rows + file.Cols;
			using (var b = BitmapService.CreateTransparent(
														width * ConstHalfWidth,
														width * ConstHalfHeight + (file.Levs - level) * Layers,
														file.Descriptor.Pal.Table))
			{
				if (b != null)
				{
					var start = new Point(
										(file.Rows - 1) * ConstHalfWidth,
									   -(level * Layers));

					MapTileArray tiles = file.Tiles;
					if (tiles != null)
					{
						Tilepart part;
						MapTile tile;

						for (int l = file.Levs - 1; l >= level; --l)
						{
							for (int
									r = 0,
										startX = start.X,
										startY = start.Y + l * Layers;
									r != file.Rows;
									++r,
										startX -= ConstHalfWidth,
										startY += ConstHalfHeight)
							{
								for (int
										c = 0,
											x = startX,
											y = startY;
										c != file.Cols;
										++c,
											x += ConstHalfWidth,
											y += ConstHalfHeight)
								{
									if (!(tile = tiles.GetTile(c,r,l)).Vacant)
									for (int i = 0; i != MapTile.QUADS; ++i)
									if ((part = tile[(PartType)i]) != null)
									{
										BitmapService.Insert(
															part[0].Sprite,
															b,
															x,
															y - part.Record.TileOffset);
									}
								}
							}
						}
					}


					Bitmap bout;
					if (Optionables.CropBackground)
					{
						Rectangle rect = BitmapService.GetNontransparentRectangle(b);
						bout           = BitmapService.CropToRectangle(b, rect);
					}
					else
						bout = b;

					using (bout) // -> workaround the inability to re-assign a using-variable inside a using-statement.
					{
						ColorPalette pal = bout.Palette;
						pal.Entries[Palette.Tid] = Optionables.BackgroundColor;
						bout.Palette = pal;

						ImageFormat format;
						if (Optionables.Png_notGif) format = ImageFormat.Png;
						else                        format = ImageFormat.Gif;

						bout.Save(fullpath, format);
					}
				}
			}
		}


		/// <summary>
		/// Closes Everything.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void OnQuitClick(object sender, EventArgs e)
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
			MapFile file = MainViewUnderlay.MapFile;
			if (file != null)
			{
				using (var f = new MapResizeInputBox(file))
				{
					if (f.ShowDialog(this) == DialogResult.OK)
					{
						RouteCheckService.Base1_xy = MainViewF.Optionables.Base1_xy; // send the base1-count options to 'XCom' ->
						RouteCheckService.Base1_z  = MainViewF.Optionables.Base1_z;

						int changes = file.MapResize(
													f.Cols,
													f.Rows,
													f.Levs,
													f.zType);

						if ((changes & MapFile.CHANGED_MAP) != 0 && !file.MapChanged)
							MapChanged = true;

						if ((changes & MapFile.CHANGED_NOD) != 0)
						{
							if (!file.RoutesChanged)
								RouteView.RoutesChangedCoordinator = true;

							foreach (RouteNode node in RouteCheckService.Invalids)
							{
								if (RouteView.RoutesInfo != null)
									RouteView.RoutesInfo.DeleteNode(node);

								file.Routes.DeleteNode(node);
							}
						}

						MainViewUnderlay.ForceResize();

						MainViewOverlay.FirstClick = false;

						ObserverManager.RouteView   .Control     .ClearSelectedInfo();
						ObserverManager.TopRouteView.ControlRoute.ClearSelectedInfo();

						ObserverManager.ToolFactory.EnableLevelers(file.Level, file.Levs);

						tsslDimensions   .Text = file.GetSizeString();
						tsslPosition     .Text =
						tsslSelectionSize.Text = String.Empty;

						ObserverManager.SetMapfile(file);

						ObserverManager.TopView     .Control   .TopControl.ClearSelectorLozenge();
						ObserverManager.TopRouteView.ControlTop.TopControl.ClearSelectorLozenge();

						if (ScanG != null) // update ScanG viewer if open
							ScanG.LoadMapfile(file);

						ResetQuadrantPanel();
					}
				}
			}
		}

		/// <summary>
		/// Opens a dialog that allows user to replace a tilepart throughout the
		/// Map with another tilepart.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnTilepartSubstitutionClick(object sender, EventArgs e)
		{
			using (var f = new TilepartSubstitution(MainViewUnderlay.MapFile))
			{
				if (f.ShowDialog() == DialogResult.OK)
				{
					int src0, src1, dst, shift;

					src0 = TilepartSubstitution.src0;

					if (TilepartSubstitution.src1 == Int32.MaxValue)
					{
						src1 = TilepartSubstitution.src0;
					}
					else
						src1 = TilepartSubstitution.src1;

					switch (TilepartSubstitution.rb_selected)
					{
						default: // ie. TilepartSubstitution.RadioSelected.Clear
							dst   =
							shift = Int32.MaxValue;
							break;

						case TilepartSubstitution.RadioSelected.Desti:
							dst   = TilepartSubstitution.dst;
							shift = Int32.MaxValue;
							break;

						case TilepartSubstitution.RadioSelected.Shift:
							dst   = Int32.MaxValue;
							shift = TilepartSubstitution.shift;
							break;
					}

					MainViewOverlay.SubstituteTileparts(src0, src1, dst, shift);
				}
			}
		}

		/// <summary>
		/// Clears all 'Descriptor.BypassRecordsExceeded' flags in the Maptree.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnClearRecordsExceededClick(object sender, EventArgs e)
		{
			int count = 0;

			foreach (var @group in TileGroupManager.TileGroups)
			foreach (var category in @group.Value.Categories)
			foreach (var descriptor in category.Value.Values)
			if (descriptor.BypassRecordsExceeded)
			{
				descriptor.BypassRecordsExceeded = false;
				++count;
			}

			string info;
			if (count != 0)
			{
				if (!MainViewF.that.MaptreeChanged)
					 MainViewF.that.MaptreeChanged = true;

				if (count == 1) info = count + " flag cleared.";
				else            info = count + " flags cleared.";
			}
			else
				info = "There were no flags set.";

			using (var dialog = new Infobox(
										"flags cleared",
										info))
			{
				dialog.ShowDialog(this);
			}
		}


		/// <summary>
		/// Gets a string of Changed objects - Map, Routes, and/or the Maptree.
		/// </summary>
		/// <returns></returns>
		private string GetChangedInfo()
		{
			string info = String.Empty;

			if (MainViewUnderlay.MapFile != null)
			{
				if (MainViewUnderlay.MapFile.MapChanged)
					info = "Map";

				if (MainViewUnderlay.MapFile.RoutesChanged)
				{
					if (info != String.Empty) info += " and ";
					info += "Routes";
				}
			}

			if (MaptreeChanged)
			{
				if (info != String.Empty) info += " and ";
				info += "Maptree";
			}

			return info;
		}

		/// <summary>
		/// Opens the Configuration Editor.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnConfiguratorClick(object sender, EventArgs e)
		{
			string info = GetChangedInfo();

			if (info != String.Empty)
			{
				string head = Infobox.SplitString("Accepting the Configuration Editor can"
							+ " restart MapView. The current " + info + " should be saved or"
							+ " else any changes will be lost. How do you wish to proceed?");

				string copyable = "cancel - return to state"                               + Environment.NewLine
								+ "ok     - risk losing changes and open the Configurator" + Environment.NewLine
								+ "retry  - save changes and open the Configurator"        + Environment.NewLine;

				using (var f = new Infobox(
										"Changes detected",
										head,
										copyable,
										Infobox.BoxType.Warn,
										Infobox.Buttons.CancelOkayRetry))
				{
					switch (f.ShowDialog(this))
					{
						default: // DialogResult.Cancel
							return;

						case DialogResult.Retry:
							if (MainViewUnderlay.MapFile != null)
							{
								if (MainViewUnderlay.MapFile.MapChanged
									&& MainViewUnderlay.MapFile.SaveMap())
								{
									MapChanged = false;

									if (MainViewUnderlay.MapFile.ForceReload)	// NOTE: Forcing reload is probably not necessary here
										ForceMapReload();						// because the current Map is *probably* going to change. I think ...
								}

								if (MainViewUnderlay.MapFile.RoutesChanged
									&& MainViewUnderlay.MapFile.SaveRoutes())
								{
									RouteView.RoutesChangedCoordinator = false;
								}
							}

							if (MaptreeChanged)
							{
//								MaptreeChanged = !TileGroupInfo.WriteTileGroups(); // <- that could cause endless recursion.
								// TODO: if(TileGroupManager.WriteTileGroups()) MaptreeChanged=false;
								TileGroupManager.WriteTileGroups();
								MaptreeChanged = false;
							}
							break;

						case DialogResult.OK:
							// The process will be killed or Canceled so don't bother to change these ->
//							MapChanged =
//							ObserverManager.RouteView   .Control     .RoutesChanged =
//							ObserverManager.TopRouteView.ControlRoute.RoutesChanged =
//							MaptreeChanged = false;
							break;
					}
				}
			}

			Configurator();
		}

		/// <summary>
		/// Opens the Configurator dialog, then does a restart if necessary.
		/// </summary>
		private void Configurator()
		{
			using (var f = new ConfigurationForm(true))
			{
				if (f.ShowDialog(this) == DialogResult.OK)
				{
					SafeQuit();

					string args = String.Empty;

					var node0 = MapTree.SelectedNode;
					if (node0 != null)
					{
						var node1 = node0.Parent;
						if (node1 != null)
						{
							var node2 = node1.Parent;
							if (node2 != null)
							{
								args = node2.Text + " " + node1.Text + " " + node0.Text;
							}
							else
								args = node1.Text + " " + node0.Text;
						}
						else
							args = node0.Text;
					}

					Process.Start(new ProcessStartInfo(Application.ExecutablePath, args));
					Process.GetCurrentProcess().Kill();
				}
			}
		}


		/// <summary>
		/// Selects a selected-tile toner color. It does this by changing the
		/// option and firing MainViewOptionables.OnOptionChanged() which calls
		/// SetTileToner() which sets an alternate set of sprites with the
		/// toner-palette and also checks the item in MainView's Toner menu.
		/// so bite
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnTonerClick(object sender, EventArgs e)
		{
			var it = sender as MenuItem;
			if (!it.Checked)
			{
				object val;
				if      (it == miNone)  val = MainViewOptionables.TONER_NONE;
				else if (it == miGray)  val = MainViewOptionables.TONER_GRAY;
				else if (it == miRed)   val = MainViewOptionables.TONER_RED;
				else if (it == miGreen) val = MainViewOptionables.TONER_GREEN;
				else                    val = MainViewOptionables.TONER_BLUE; // (it == miBlue)

				Options[MainViewOptionables.str_SelectedTileToner].Value = val;
				Optionables.OnOptionChanged(MainViewOptionables.str_SelectedTileToner, val);

				if (_foptions != null && _foptions.Visible)
					(_foptions as OptionsForm).propertyGrid.Refresh();
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
			Help.ShowHelp(MainViewF.that, "file://" + help);
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
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks>This handler is not a toggle. The dialog will be focused if
		/// already open.</remarks>
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
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks>This handler is a toggle. The dialog will be closed if it's
		/// open.</remarks>
		private void OnMapInfoClick(object sender, EventArgs e)
		{
			if (!miMapInfo.Checked)
			{
				if (MainViewUnderlay.MapFile != null) // safety.
				{
					miMapInfo.Checked = true;
					_finfo = new MapInfoDialog(this);
				}
			}
			else
				_finfo.Close();
		}

		/// <summary>
		/// Dechecks the MapInfo item when the MapInfo dialog closes.
		/// </summary>
		internal void DecheckMapInfo()
		{
			miMapInfo.Checked = false;
		}


		/// <summary>
		/// Zooms in.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void OnScaleInClick(object sender, EventArgs e)
		{
			if (Globals.Scale < Globals.ScaleMaximum)
			{
				Globals.Scale += Math.Min(
										Globals.ScaleMaximum - Globals.Scale,
										Globals.ScaleDelta);
				Scale();
			}
		}

		/// <summary>
		/// Zooms out.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void OnScaleOutClick(object sender, EventArgs e)
		{
			if (Globals.Scale > Globals.ScaleMinimum)
			{
				Globals.Scale -= Math.Min(
										Globals.Scale - Globals.ScaleMinimum,
										Globals.ScaleDelta);
				Scale();
			}
		}

		/// <summary>
		/// Zooms in/out.
		/// </summary>
		private void Scale()
		{
			Globals.AutoScale = false;
			ObserverManager.ToolFactory.DecheckAutoscale();

			MainViewUnderlay.SetOverlaySize();
			MainViewUnderlay.UpdateScrollers();

			Invalidate();
		}

		/// <summary>
		/// Handles a click on the toolstrip's auto-scale button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void OnAutoScaleClick(object sender, EventArgs e)
		{
			if (Globals.AutoScale = ObserverManager.ToolFactory.ToggleAutoscale())
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
		#endregion Events


		#region Events (load)
		// __Sequence of Events__
		// MainViewF.OnMapTreeMouseDown()
		// MainViewF.OnMapTreeNodeMouseClick()
		// MainViewF.OnMapTreeBeforeSelect()
		// MainViewF.OnMapTreeAfterSelect()
		// MainViewF.LoadSelectedDescriptor()

		/// <summary>
		/// Cache of the currently selected treenode. Is used to determine if
		/// a MapBrowserDialog should popup on [Shift+Enter] - iff the MAP+RMP
		/// files are invalid.
		/// </summary>
		private TreeNode _selected;

		/// <summary>
		/// just because you have billions of dollars doesn't mean you can beep
		/// </summary>
		private DontBeepType _dontbeeptype;
		private enum DontBeepType
		{
			OpenContext,
			LoadDescriptor,
			MapBrowserDialog
		}

		/// <summary>
		/// This flag is used to bypass checks when accessing the TilesetEditor
		/// while maintaining the current state of the actual changed-flags.
		/// </summary>
		private bool BypassChanged;

		/// <summary>
		/// By keeping this value below 2 until either (a) a leftclick is
		/// confirmed on a treenode with a tileset or (b) keydown [Enter] the
		/// Maptree can be navigated by keyboard without loading every darn Map
		/// whose treenode gets selected during keyboard navigation.
		/// </summary>
		private int _loadReady;
		const int LOADREADY_STAGE_0 = 0; // totally undecided
		const int LOADREADY_STAGE_1 = 1; // definitely a leftclick, but still not sure if it's on a Tileset node
		const int LOADREADY_STAGE_2 = 2; // a tileset node is currently selected in the Maptree - ok to load descriptor


		/// <summary>
		/// Bypasses ornery system-beeps that can happen on keydown events.
		/// - [Space] opens the Context menu
		/// - [Enter] loads a Descriptor
		/// - [Shift+Enter] opens the MapBrowserDialog if a tileset's files are
		///                 invalid; will also load a Descriptor if the files
		///                 are valid.
		/// </summary>
		private void FireContext()
		{
			switch (_dontbeeptype)
			{
				case DontBeepType.OpenContext:
				{
					var nodebounds = _selected.Bounds;
					var args = new MouseEventArgs(
												MouseButtons.Right,
												1,
												nodebounds.X + 15, nodebounds.Y + 5,
												0);
					OnMapTreeMouseDown(null, args);
					break;
				}

				case DontBeepType.LoadDescriptor:
				{
					_loadReady = LOADREADY_STAGE_2;
					OnMapTreeAfterSelect(null, new TreeViewEventArgs(_selected));
					break;
				}

				case DontBeepType.MapBrowserDialog:
				{
					var args = new TreeNodeMouseClickEventArgs(
															_selected,
															MouseButtons.None,
															0, 0,0);
					OnMapTreeNodeMouseClick(null, args);
					break;
				}
			}
		}


		/// <summary>
		/// Opens a context-menu on RMB-click.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks>A MouseDown event occurs *before* the treeview's
		/// BeforeSelect and AfterSelected events occur .... A MouseClick event
		/// occurs *after* the treeview's BeforeSelect and AfterSelected events
		/// occur. So the selected Map will change *before* a context-menu is
		/// shown, which is good. A MouseClick event won't work if the tree is
		/// blank. So use MouseDown.</remarks>
		private void OnMapTreeMouseDown(object sender, MouseEventArgs e)
		{
			//LogFile.WriteLine("MainViewF.OnMapTreeMouseDown() BypassChanged= " + BypassChanged);

			switch (e.Button)
			{
				case MouseButtons.Right:
					if (MainViewUnderlay.MapFile == null					// prevent a bunch of problems, like looping dialogs when returning from
						|| BypassChanged									// the Tileset Editor and the Maptree-node gets re-selected, causing
						|| (   !MainViewUnderlay.MapFile.MapChanged			// this class-object to react as if a different Map is going to load ...
							&& !MainViewUnderlay.MapFile.RoutesChanged))	// vid. LoadSelectedDescriptor()
					{
						BypassChanged = false;

						cmMapTreeMenu.MenuItems.Clear();

						cmMapTreeMenu.MenuItems.Add("Add Group ...", OnAddGroupClick);

						if (MapTree.SelectedNode != null)
						{
							switch (MapTree.SelectedNode.Level)
							{
								case TREELEVEL_GROUP:
									cmMapTreeMenu.MenuItems.Add("-");
									cmMapTreeMenu.MenuItems.Add("Edit Group ...",   OnEditGroupClick);
									cmMapTreeMenu.MenuItems.Add("Delete Group",     OnDeleteGroupClick);
									cmMapTreeMenu.MenuItems.Add("-");
									cmMapTreeMenu.MenuItems.Add("Add Category ...", OnAddCategoryClick);
									break;

								case TREELEVEL_CATEGORY:
									cmMapTreeMenu.MenuItems.Add("-");
									cmMapTreeMenu.MenuItems.Add("Edit Category ...", OnEditCategoryClick);
									cmMapTreeMenu.MenuItems.Add("Delete Category",   OnDeleteCategoryClick);
									cmMapTreeMenu.MenuItems.Add("-");
									cmMapTreeMenu.MenuItems.Add("Add Tileset ...",   OnAddTilesetClick);
									break;

								case TREELEVEL_TILESET:
									cmMapTreeMenu.MenuItems.Add("-");
									cmMapTreeMenu.MenuItems.Add("Edit Tileset ...",  OnEditTilesetClick);
									cmMapTreeMenu.MenuItems.Add("Delete Tileset",    OnDeleteTilesetClick);
									break;
							}
						}

						cmMapTreeMenu.Show(MapTree, e.Location);
					}
					else
					{
						string info = GetChangedInfo();

						string head = Infobox.SplitString("Modifying the Maptree can cause the Tilesets"
									+ " to reload. The current " + info + " should be saved or else any"
									+ " changes will be lost. How do you wish to proceed?");

						string copyable = "cancel - return to state"                               + Environment.NewLine
										+ "ok     - risk losing changes and show the Maptree-menu" + Environment.NewLine
										+ "retry  - save changes and show the Maptree-menu"        + Environment.NewLine;

						using (var f = new Infobox(
												"Changes detected",
												head,
												copyable,
												Infobox.BoxType.Warn,
												Infobox.Buttons.CancelOkayRetry))
						{
							switch (f.ShowDialog(this))
							{
								default: // DialogResult.Cancel:
									return;

								case DialogResult.Retry:
									if (MainViewUnderlay.MapFile.MapChanged
										&& MainViewUnderlay.MapFile.SaveMap())
									{
										MapChanged = false;

										if (MainViewUnderlay.MapFile.ForceReload)	// NOTE: Forcing reload is probably not necessary here
											ForceMapReload();						// because the current Map is *probably* going to change. I think ...
									}

									if (MainViewUnderlay.MapFile.RoutesChanged
										&& MainViewUnderlay.MapFile.SaveRoutes())
									{
										RouteView.RoutesChangedCoordinator = false;
									}
									break;

								case DialogResult.OK:
									BypassChanged = true;
									break;
							}
						}

						OnMapTreeMouseDown(null, e); // RECURSE^
					}
					break;

				case MouseButtons.Left:
					_loadReady = LOADREADY_STAGE_1;
					break;
			}
		}


		/// <summary>
		/// Adds a group to the Maptree.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnAddGroupClick(object sender, EventArgs e)
		{
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

					TileGroupManager.AddTileGroup(f.Label);

					CreateTree();
					SelectGroupNode(f.Label);
				}
			}
		}

		/// <summary>
		/// Edits the label of a group on the Maptree.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnEditGroupClick(object sender, EventArgs e)
		{
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

					TileGroupManager.EditTileGroup(
												f.Label,
												labelGroup);
					CreateTree();
					SelectGroupNode(f.Label);
				}
			}
		}

		/// <summary>
		/// Deletes a group from the Maptree.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnDeleteGroupClick(object sender, EventArgs e)
		{
			// TODO: Make a custom box for delete Group/Category/Tileset.

			string head = Infobox.SplitString("Are you sure you want to remove"
						+ " this Map group? This will also remove all its categories"
						+ " and tilesets, but files on disk are unaffected.");

			string labelGroup = MapTree.SelectedNode.Text;

			using (var f = new Infobox(
									"Warning",
									head,
									"group - " + labelGroup,
									Infobox.BoxType.Warn,
									Infobox.Buttons.CancelOkay))
			{
				if (f.ShowDialog(this) == DialogResult.OK)
				{
					MaptreeChanged = true;

					TileGroupManager.DeleteTileGroup(labelGroup);

					CreateTree();
					SelectGroupNodeTop();
				}
			}
		}

		/// <summary>
		/// Adds a category to a group on the Maptree.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnAddCategoryClick(object sender, EventArgs e)
		{
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

					var @group = TileGroupManager.TileGroups[labelGroup];
					@group.AddCategory(f.Label);

					CreateTree();
					SelectCategoryNode(f.Label, @group.Label);
				}
			}
		}

		/// <summary>
		/// Edits the label of a category on the Maptree.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnEditCategoryClick(object sender, EventArgs e)
		{
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

					var @group = TileGroupManager.TileGroups[labelGroup];
					@group.EditCategory(f.Label, labelCategory);

					CreateTree();
					SelectCategoryNode(f.Label, @group.Label);
				}
			}
		}

		/// <summary>
		/// Deletes a category from the Maptree.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnDeleteCategoryClick(object sender, EventArgs e)
		{
			// TODO: Make a custom box for delete Group/Category/Tileset.

			string head = Infobox.SplitString("Are you sure you want to remove"
						+ " this Map category? This will also remove all its"
						+ " tilesets, but files on disk are unaffected.");

			string labelGroup    = MapTree.SelectedNode.Parent.Text;
			string labelCategory = MapTree.SelectedNode.Text;

			string copyable = "group    - " + labelGroup + Environment.NewLine
							+ "category - " + labelCategory;

			using (var f = new Infobox(
									"Warning",
									head,
									copyable,
									Infobox.BoxType.Warn,
									Infobox.Buttons.CancelOkay))
			{
				if (f.ShowDialog(this) == DialogResult.OK)
				{
					MaptreeChanged = true;

					var @group = TileGroupManager.TileGroups[labelGroup];
					@group.DeleteCategory(labelCategory);

					CreateTree();
					SelectCategoryNodeTop(labelGroup);
				}
			}
		}

		/// <summary>
		/// Adds a tileset and its characteristics to the Maptree.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnAddTilesetClick(object sender, EventArgs e)
		{
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
						MaptreeChanged = true;

						BypassChanged = true;
						CreateTree();

						// CreateTree() fires OnMapTreeBeforeSelect() which clears 'BypassChanged'
						// so it needs to be set again - the wonders of the .net Framework in action!
						// NOTE: just don't release it until the file loads
//						BypassChanged = true;
						SelectTilesetNode(f.TilesetLabel, labelCategory, labelGroup);
					}
				}
			}
		}

		/// <summary>
		/// Edits the characteristics of a tileset on the Maptree.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnEditTilesetClick(object sender, EventArgs e)
		{
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
						//LogFile.WriteLine("return to OnEditTilesetClick() w/ DialogResult.OK");

						MaptreeChanged = true;

						BypassChanged = true;
						CreateTree();

						// CreateTree() fires OnMapTreeBeforeSelect() which clears 'BypassChanged'
						// so it needs to be set again - the wonders of the .net Framework in action!
						// Solution: just don't release it until the file loads
//						BypassChanged = true;
						SelectTilesetNode(f.TilesetLabel, labelCategory, labelGroup);
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
			var @group = TileGroupManager.TileGroups[labelGroup];

			string key = null;
			switch (@group.GroupType)
			{
				case GameType.Ufo:  key = SharedSpace.ResourceDirectoryUfo;  break;
				case GameType.Tftd: key = SharedSpace.ResourceDirectoryTftd; break;
			}

			if (SharedSpace.GetShareString(key) == null)
			{
				switch (@group.GroupType)
				{
					case GameType.Ufo:  key = "UFO";  break;
					case GameType.Tftd: key = "TFTD"; break;
				}

				using (var f = new Infobox(
										"Error",
										key + " is not configured.",
										null,
										Infobox.BoxType.Error))
				{
					f.ShowDialog(this);
				}
				return false;
			}
			return true;
		}

		/// <summary>
		/// Deletes a tileset from the Maptree.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnDeleteTilesetClick(object sender, EventArgs e)
		{
			// TODO: Make a custom box for delete Group/Category/Tileset.

			string head = Infobox.SplitString("Are you sure you want to remove"
						+ " this Map tileset? Files on disk are unaffected.");

			string labelGroup    = MapTree.SelectedNode.Parent.Parent.Text;
			string labelCategory = MapTree.SelectedNode.Parent.Text;
			string labelTileset  = MapTree.SelectedNode.Text;

			string copyable = "group    - " + labelGroup    + Environment.NewLine
							+ "category - " + labelCategory + Environment.NewLine
							+ "tileset  - " + labelTileset;

			using (var f = new Infobox(
									"Warning",
									head,
									copyable,
									Infobox.BoxType.Warn,
									Infobox.Buttons.CancelOkay))
			{
				if (f.ShowDialog(this) == DialogResult.OK)
				{
					MaptreeChanged = true;

					var @group = TileGroupManager.TileGroups[labelGroup];
					@group.DeleteTileset(labelTileset, labelCategory);

					CreateTree();
					SelectTilesetNodeTop(labelCategory);
				}
			}
		}


		// TODO: consolidate the select node functions into a single function.

		/// <summary>
		/// Selects the top treenode in the Maps tree if one exists.
		/// </summary>
		private void SelectGroupNodeTop()
		{
			if (MapTree.Nodes.Count != 0)
				MapTree.SelectedNode = MapTree.Nodes[0];
		}

		/// <summary>
		/// Selects the top category treenode in the Maps tree if one exists
		/// under a given group treenode.
		/// @note Assumes that the parent-group node is valid.
		/// </summary>
		/// <param name="labelGroup"></param>
		private void SelectCategoryNodeTop(string labelGroup)
		{
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
		/// @note Assumes that the parent-parent-group and parent-category nodes
		/// are valid.
		/// </summary>
		/// <param name="labelCategory"></param>
		private void SelectTilesetNodeTop(string labelCategory)
		{
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
		/// <returns>true if node is found</returns>
		private void SelectGroupNode(string labelGroup)
		{
			foreach (TreeNode nodeGroup in MapTree.Nodes)
			{
				if (nodeGroup.Text == labelGroup)
				{
					MapTree.SelectedNode = nodeGroup;
					nodeGroup.Expand();
					return;
				}
			}
		}

		/// <summary>
		/// Selects a treenode in the Maps tree given a category-label.
		/// </summary>
		/// <param name="labelCategory"></param>
		/// <param name="labelGroup"></param>
		/// <returns>true if node is found</returns>
		private void SelectCategoryNode(string labelCategory, string labelGroup)
		{
			foreach (TreeNode nodeGroup in MapTree.Nodes)
			{
				if (nodeGroup.Text == labelGroup)
				{
					var groupCollection = nodeGroup.Nodes;
					foreach (TreeNode nodeCategory in groupCollection)
					{
						if (nodeCategory.Text == labelCategory)
						{
							MapTree.SelectedNode = nodeCategory;
							nodeCategory.Expand();
							return;
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
		/// <returns>true if node is found</returns>
		private void SelectTilesetNode(string labelTileset, string labelCategory, string labelGroup)
		{
			foreach (TreeNode nodeGroup in MapTree.Nodes)
			{
				if (nodeGroup.Text == labelGroup)
				{
					var groupCollection = nodeGroup.Nodes;
					foreach (TreeNode nodeCategory in groupCollection)
					{
						if (nodeCategory.Text == labelCategory)
						{
							var categoryCollection = nodeCategory.Nodes;
							foreach (TreeNode nodeTileset in categoryCollection)
							{
								if (nodeTileset.Text == labelTileset)
								{
									_loadReady = LOADREADY_STAGE_2;
									MapTree.SelectedNode = nodeTileset;
									return;
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

		/// <summary>
		/// If user clicks on an already selected node, for which the Mapfile
		/// has not been loaded, this handler offers to show a dialog for the
		/// user to browse to the file.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMapTreeNodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			//LogFile.WriteLine("MainViewF.OnMapTreeNodeMouseClick() _loadReady= " + _loadReady);

			if (e.Node == _selected)
			{
				var descriptor = e.Node.Tag as Descriptor;
				if (descriptor != null)
				{
					if (   MainViewUnderlay.MapFile == null
						|| MainViewUnderlay.MapFile.Descriptor != descriptor)
					{
						ClearSearched();

						_loadReady = LOADREADY_STAGE_2;
						LoadSelectedDescriptor(true);
					}
				}
			}
		}

		/// <summary>
		/// Asks user to save before switching Maps if applicable.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMapTreeBeforeSelect(object sender, CancelEventArgs e)
		{
			//LogFile.WriteLine("MainViewF.OnMapTreeBeforeSelect() BypassChanged= " + BypassChanged);

			if (!BypassChanged) // is true on TilesetEditor DialogResult.OK
			{
				e.Cancel  = (SaveAlertMap()    == DialogResult.Cancel);
				e.Cancel |= (SaveAlertRoutes() == DialogResult.Cancel); // NOTE: that bitwise had better execute ....
			}
//			else
//				BypassChanged = false;
		}

		/// <summary>
		/// Loads the selected Map.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMapTreeAfterSelect(object sender, TreeViewEventArgs e)
		{
			//LogFile.WriteLine("MainViewF.OnMapTreeAfterSelect() _loadReady= " + _loadReady);

			ClearSearched();

			if (_loadReady == LOADREADY_STAGE_1)
				_loadReady  = LOADREADY_STAGE_2;

			LoadSelectedDescriptor();

			_selected = e.Node;
		}
		#endregion Events (load)


		#region Methods
		/// <summary>
		/// Loads the Map that's selected in the Maptree.
		/// <param name="browseMapfile">true to force the find Mapfile dialog</param>
		/// <param name="keepRoutes">true to keep the current Routes (use this
		/// only when reloading the current Mapfile)</param>
		/// </summary>
		private void LoadSelectedDescriptor(bool browseMapfile = false, bool keepRoutes = false)
		{
			//LogFile.WriteLine("");
			//LogFile.WriteLine("");
			//LogFile.WriteLine("MainViewF.LoadSelectedDescriptor() _loadReady= " + _loadReady);
			//LogFile.WriteLine(". browseMapfile= " + browseMapfile);

			if (TopView._finfobox != null && !TopView._finfobox.IsDisposed) // close the PartslotTest dialog
			{
				TopView._finfobox.Close();
				TopView._finfobox = null;
			}

			if (_loadReady == LOADREADY_STAGE_2)
			{
				BypassChanged = false;

				var descriptor = MapTree.SelectedNode.Tag as Descriptor;
				if (descriptor != null)
				{
					bool treechanged = false;

					RouteNodes routes; bool routesChanged;
					if (keepRoutes)
					{
						routes = MainViewUnderlay.MapFile.Routes;
						routesChanged = MainViewUnderlay.MapFile.RoutesChanged;
					}
					else
					{
						routes = null;
						routesChanged = false;
					}

					MapFile file = MapFileService.LoadDescriptor( // NOTE: LoadDescriptor() instantiates a MapFile but whatver.
															descriptor,
															ref treechanged,
															browseMapfile,
															Optionables.IgnoreRecordsExceeded,
															routes);
					if (treechanged) MaptreeChanged = true;

					if (file != null)
					{
						miSaveAll             .Enabled =
						miSaveMap             .Enabled =
						miSaveRoutes          .Enabled =
						miExport              .Enabled =
						miReload              .Enabled =
						miScreenshot          .Enabled =
						miModifySize          .Enabled =
						miTilepartSubstitution.Enabled =
						miMapInfo             .Enabled = true;

						MainViewOverlay.FirstClick = false;

						if (descriptor.GroupType == GameType.Tftd)
						{
							MenuManager.EnableScanG(SpritesetManager.GetScanGtftd() != null);
							MainViewOverlay.SpriteBrushes = Palette.BrushesTftdBattle; // used by Mono only
						}
						else // default to ufo-battle palette
						{
							MenuManager.EnableScanG(SpritesetManager.GetScanGufo() != null);
							MainViewOverlay.SpriteBrushes = Palette.BrushesUfoBattle; // used by Mono only
						}

						MainViewUnderlay.MapFile = file;

						ObserverManager.ToolFactory.EnableAutoscale();
						ObserverManager.ToolFactory.EnableLevelers(file.Level, file.Levs);

						Text = TITLE + " " + descriptor.Basepath;
						if (MaptreeChanged) MaptreeChanged = MaptreeChanged; // maniacal laugh YOU figure it out.

						tsslMapLabel     .Text = descriptor.Label;
						tsslDimensions   .Text = file.GetSizeString();
						tsslPosition     .Text =
						tsslSelectionSize.Text = String.Empty;

						if (!file.MapChanged) MapChanged = (file.TerrainsetCountExceeded != 0);
						file.TerrainsetCountExceeded = 0; // TODO: Perhaps do that when the Mapfile is saved.

						var routeview1 = ObserverManager.RouteView.Control;
						var routeview2 = ObserverManager.TopRouteView.ControlRoute;

						routeview1.ClearSelectedInfo();
						routeview2.ClearSelectedInfo();

						RouteView.DisableOg();
						RouteView.DisableOg();

						Options[MainViewOptionables.str_OpenDoors].Value = // close doors; not necessary but keeps user's head on straight.
						Optionables.OpenDoors = false;
						SetDoorSpritesFullPhase(false);
						if (_foptions != null && _foptions.Visible)
						{
							(_foptions as OptionsForm).propertyGrid.Refresh();
						}

						SetTileToner(Optionables.SelectedTileToner); // create toned spriteset(s) for selected-tile(s)

						if (!menuViewers.Enabled) // show the forms that are flagged to show (in MainView's Options).
							MenuManager.StartSecondaryStageBoosters();

						ObserverManager.SetMapfile(file); // and reset all observer events

						RouteCheckService.Base1_xy = MainViewF.Optionables.Base1_xy; // send the base1-count options to 'XCom' ->
						RouteCheckService.Base1_z  = MainViewF.Optionables.Base1_z;

						if (RouteCheckService.CheckNodeBounds(file) == DialogResult.Yes)
						{
							RouteView.RoutesChangedCoordinator = true;

							foreach (RouteNode node in RouteCheckService.Invalids)
								file.Routes.DeleteNode(node);
						}

						if (routesChanged && !file.RoutesChanged)
							RouteView.RoutesChangedCoordinator = true;

						Globals.Scale = Globals.Scale; // enable/disable the scale-in/scale-out buttons

						if (ScanG != null) // update ScanG viewer if open
							ScanG.LoadMapfile(file);

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

						if (RouteView.RoutesInfo != null) // update RoutesInfo if open
							RouteView.RoutesInfo.Initialize(file);

						ResetQuadrantPanel(); // update the Quadrant panel

						FirstActivated = false;
						Activate();
					}
				}
			}
			_loadReady = LOADREADY_STAGE_0;
		}

		/// <summary>
		/// Sets the toner to be used for drawing the parts of selected tiles.
		/// </summary>
		/// <param name="toner"></param>
		internal void SetTileToner(int toner)
		{
			miNone .Checked = (toner == MainViewOptionables.TONER_NONE);
			miGray .Checked = (toner == MainViewOptionables.TONER_GRAY);
			miRed  .Checked = (toner == MainViewOptionables.TONER_RED);
			miGreen.Checked = (toner == MainViewOptionables.TONER_GREEN);
			miBlue .Checked = (toner == MainViewOptionables.TONER_BLUE);

			if (SpritesetManager.Spritesets.Count != 0)
			{
				ColorPalette table;
				switch (toner)
				{
					case MainViewOptionables.TONER_NONE:
						return; // sprites shall draw their standard, no-toned palette version

					// NOTE: All loaded spritesets will have the same palette.
					// - this demonstrates how redundant palette-pointers are.
					//   palette this palette that, here have a palette ...
					//   i know you want one

					default: // case TONER_GRAY
						table = SpritesetManager.Spritesets[0].Pal.GrayScale.Table;
						break;

					case MainViewOptionables.TONER_RED:
						table = SpritesetManager.Spritesets[0].Pal.RedScale.Table;
						break;

					case MainViewOptionables.TONER_GREEN:
						table = SpritesetManager.Spritesets[0].Pal.GreenScale.Table;
						break;

					case MainViewOptionables.TONER_BLUE:
						table = SpritesetManager.Spritesets[0].Pal.BlueScale.Table;
						break;
				}

				foreach (var spriteset in SpritesetManager.Spritesets)
				{
					for (int id = 0; id != spriteset.Count; ++id)
					{
						(spriteset[id] as PckSprite).SpriteToned.Palette = table; // lovely.
					}
				}
			}
		}

		/// <summary>
		/// Resets the QuadrantControl when either a Map loads or gets resized.
		/// </summary>
		private static void ResetQuadrantPanel()
		{
			QuadrantControl p1 = ObserverManager.TopView     .Control   .QuadrantControl;
			QuadrantControl p2 = ObserverManager.TopRouteView.ControlTop.QuadrantControl;

			p1.Tile =
			p2.Tile = null;

			p1.SelectedLocation =
			p2.SelectedLocation = null;

			QuadrantDrawService.CurrentTilepart = ObserverManager.TileView.Control.SelectedTilepart;

			p1.Invalidate();
			p2.Invalidate();
		}


		/// <summary>
		/// Sets door-sprites to fullphase or firstphase.
		/// </summary>
		/// <param name="full">true to animate any doors</param>
		internal void SetDoorSpritesFullPhase(bool full)
		{
			if (MainViewUnderlay.MapFile != null) // NOTE: MapFile is null on MapView load.
			{
				foreach (Tilepart part in MainViewUnderlay.MapFile.Parts)
					part.ToggleDoorSprites(full);
			}
		}

		/// <summary>
		/// Sets door-sprites to their alternate sprite's firstphase.
		/// </summary>
		internal void SetDoorSpritesAlternate()
		{
			if (MainViewUnderlay.MapFile != null) // NOTE: MapFile is null on MapView load.
			{
				foreach (Tilepart part in MainViewUnderlay.MapFile.Parts)
					part.SetSprite1_alt();
			}
		}


		/// <summary>
		/// Shows the user a dialog-box asking to Save if the currently
		/// displayed Map has changed.
		/// </summary>
		/// <returns>DialogResult.OK if things can proceed; DialogResult.Cancel
		/// if user chose to cancel or the Mapfile was not written successfully</returns>
		/// <remarks>Is called when either (a) MapView is closing (b) a Map is
		/// about to load/reload.</remarks>
		private DialogResult SaveAlertMap()
		{
			//LogFile.WriteLine("MainViewF.SaveAlertMap()");

			if (MainViewUnderlay.MapFile != null && MainViewUnderlay.MapFile.MapChanged)
			{
				using (var f = new Infobox(
										"Map Changed",
										"Do you want to save changes to the Map?",
										null,
										Infobox.BoxType.Warn,
										Infobox.Buttons.CancelYesNo)) // cancel/ok/retry
				{
					switch (f.ShowDialog(this))
					{
						case DialogResult.Cancel:	// close dialog and maintain state
							return DialogResult.Cancel;

						case DialogResult.OK:		// Yes. save Mapfile and clear MapChanged flag
							if (MainViewUnderlay.MapFile.SaveMap())
								goto case DialogResult.Retry;

							return DialogResult.Cancel;

						case DialogResult.Retry:	// No. don't save just clear MapChanged flag
							MapChanged = false;
							break;
					}
				}
			}
			return DialogResult.OK;
		}

		/// <summary>
		/// Shows the user a dialog-box asking to Save if the currently
		/// displayed Routes has changed.
		/// </summary>
		/// <returns>DialogResult.OK if things can proceed; DialogResult.Cancel
		/// if user chose to cancel or the Routefile was not written
		/// successfully</returns>
		/// <remarks>Is called when either (a) MapView is closing (b) another
		/// Map is about to load.</remarks>
		private DialogResult SaveAlertRoutes()
		{
			if (MainViewUnderlay.MapFile != null && MainViewUnderlay.MapFile.RoutesChanged)
			{
				using (var f = new Infobox(
										"Routes Changed",
										"Do you want to save changes to the Routes?",
										null,
										Infobox.BoxType.Warn,
										Infobox.Buttons.CancelYesNo)) // cancel/ok/retry
				{
					switch (f.ShowDialog(this))
					{
						case DialogResult.Cancel:	// close dialog and maintain state
							return DialogResult.Cancel;

						case DialogResult.OK:		// Yes. save Routes and clear RoutesChanged flag
							if (MainViewUnderlay.MapFile.SaveRoutes())
								goto case DialogResult.Retry;

							return DialogResult.Cancel;

						case DialogResult.Retry:	// No. don't save just clear RoutesChanged flag
							RouteView.RoutesChangedCoordinator = false;
							break;
					}
				}
			}
			return DialogResult.OK;
		}

		/// <summary>
		/// Shows the user a dialog-box asking to Save the Maptree if it has
		/// changed.
		/// </summary>
		/// <returns>DialogResult.OK if things can proceed; DialogResult.Cancel
		/// if user chose to cancel or MapTilesets was not written successfully.</returns>
		/// <remarks>Is called when either (a) MapView is closing (b) MapView is
		/// reloading due to a configuration change (ie. only if resource-paths
		/// have been changed, since the only other relevant option - if the
		/// tilesets-config file - is changed then saving the current one is
		/// pointless).</remarks>
		private DialogResult SaveAlertMaptree()
		{
			if (MaptreeChanged)
			{
				using (var f = new Infobox(
										"Maptree Changed",
										"Do you want to save changes to the Map Tree?",
										null,
										Infobox.BoxType.Warn,
										Infobox.Buttons.CancelYesNo)) // cancel/ok/retry
				{
					switch (f.ShowDialog(this))
					{
						case DialogResult.Cancel:	// close dialog and maintain state
							return DialogResult.Cancel;

						case DialogResult.OK:		// Yes. save MapTilesets then check MaptreeChanged flag
							OnSaveMaptreeClick(null, EventArgs.Empty);
							if (MaptreeChanged) // ie. failed
								return DialogResult.Cancel;
							break;

						case DialogResult.Retry:	// No. don't save just clear MaptreeChanged flag
							MaptreeChanged = false;
							break;
					}
				}
			}
			return DialogResult.OK;
		}


		/// <summary>
		/// Prints the currently selected location to the statusbar.
		/// </summary>
		internal void sb_PrintPosition()
		{
			if (MainViewOverlay.FirstClick)
			{
				MapFile file = MainViewUnderlay.MapFile;

				int c = file.Location.Col;
				int r = file.Location.Row;
				int l = file.Levs - file.Level;

				if (Optionables.Base1_xy) { ++c; ++r; }
				if (!Optionables.Base1_z) { --l; }

				tsslPosition.Text = "c " + c + "  r " + r + "  L " + l;
			}
		}

		/// <summary>
		/// Prints the current scale to the statusbar.
		/// </summary>
		internal void sb_PrintScale()
		{
			tsslScale.Text = String.Format(
										CultureInfo.CurrentCulture,
										"scale {0:0.00}",
										Globals.Scale);
		}

		/// <summary>
		/// Prints the current selection-size to the statusbar.
		/// </summary>
		/// <param name="tx"></param>
		/// <param name="ty"></param>
		internal void sb_PrintSelectionSize(int tx, int ty)
		{
			tsslSelectionSize.Text = tx + " x " + ty;
			ssMain.Refresh(); // fast update for selection-size.
		}
		#endregion Methods
	}
}
