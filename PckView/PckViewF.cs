using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
#if !__MonoCS__
using System.Runtime.InteropServices;
#endif
using System.Windows.Forms;

using DSShared;
using DSShared.Controls;

using XCom;

using YamlDotNet.RepresentationModel; // read values (deserialization)


namespace PckView
{
	/// <summary>
	/// The mainform for PckView.
	/// </summary>
	public sealed partial class PckViewF
		:
			Form
#if !__MonoCS__
			, IMessageFilter
#endif
	{
#if !__MonoCS__
		#region P/Invoke declarations
		[DllImport("user32.dll")]
		static extern IntPtr WindowFromPoint(Point pt);

		[DllImport("user32.dll")]
		static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);
		#endregion P/Invoke declarations

		#region IMessageFilter
		/// <summary>
		/// Sends mousewheel messages to the control that the mouse-cursor is
		/// hovering over.
		/// </summary>
		/// <param name="m">the message</param>
		/// <returns>true if a mousewheel message was handled successfully</returns>
		/// <remarks>https://stackoverflow.com/questions/4769854/windows-forms-capturing-mousewheel#4769961</remarks>
		public bool PreFilterMessage(ref Message m)
		{
			if (m.Msg == 0x20a)
			{
				// WM_MOUSEWHEEL - find the control at screen position m.LParam
				var pos = new Point(m.LParam.ToInt32());

				IntPtr hWnd = WindowFromPoint(pos);
				if (hWnd != IntPtr.Zero && hWnd != m.HWnd && Control.FromHandle(hWnd) != null)
				{
					SendMessage(hWnd, m.Msg, m.WParam, m.LParam);
					return true;
				}
			}
			return false;
		}
		#endregion IMessageFilter
#endif

		#region Delegates
		internal delegate void PaletteChangedEvent();
		#endregion Delegates


		#region Events (static)
		internal static event PaletteChangedEvent PaletteChanged;
		#endregion Events (static)


		#region Fields (static)
		internal static string[] _args;

		private const string TITLE    = "PckView";

		private const string Total    = "Total ";
		private const string Selected = "Selected ";
		private const string Over     = "Over ";
		private const string None     = "n/a";

		internal static bool Quit;

		/// <summary>
		/// <c>SpriteShadeFloat</c> is used by <c><see cref="Ia"/></c> to draw
		/// sprites if <c><see cref="Shader"/></c> is enabled and is set to
		/// <c><see cref="ShaderOn"/></c>.
		/// </summary>
		internal static float SpriteShadeFloat;

		private  const int ShaderDisabled = 0;
		internal const int ShaderOn       = 1;
		private  const int ShaderOff      = 2;

		internal static bool BypassActivatedEvent;
		#endregion Fields (static)


		#region Fields
		/// <summary>
		/// <c>true</c> if PckView has been invoked via TileView.
		/// </summary>
		private bool IsInvoked;

//		private TabControl _tcTabs; // for OnCompareClick()

		private ToolStripMenuItem _miEditor;
		private ToolStripMenuItem _miAdd;
		private ToolStripMenuItem _miInsertBefor;
		private ToolStripMenuItem _miInsertAfter;
		private ToolStripMenuItem _miReplace;
		private ToolStripMenuItem _miMoveL;
		private ToolStripMenuItem _miMoveR;
		private ToolStripMenuItem _miDelete;
		private ToolStripMenuItem _miExport;
		private ToolStripMenuItem _miCreate;
		private ToolStripMenuItem _miClear;

		/// <summary>
		/// A <c>Dictionary</c> that contains
		/// <c><see cref="Palette">Palettes</see></c> that are available under
		/// the Palette menu.
		/// </summary>
		private readonly Dictionary<Palette, MenuItem> _itPalettes =
					 new Dictionary<Palette, MenuItem>();

		/// <summary>
		/// Status of the spriteshade.
		/// <list type="bullet">
		/// <item><c><see cref="ShaderDisabled"/></c></item>
		/// <item><c><see cref="ShaderOn"/></c></item>
		/// <item><c><see cref="ShaderOff"/></c></item>
		/// </list>
		/// </summary>
		internal int Shader;

		/// <summary>
		/// <c>ImageAttibutes</c> used to draw sprites with spriteshade.
		/// </summary>
		internal ImageAttributes Ia;


		private string _lastCreateDirectory;
		private string _lastBrowserDirectory;
		private string _lastSpriteDirectory;

		private bool _minimized;

		/// <summary>
		/// A placeholder sprite to draw instead of totally transparent sprite.
		/// </summary>
		internal Bitmap BlankSprite;

		/// <summary>
		/// A placeholder icon to draw instead of totally transparent icon.
		/// </summary>
		internal Bitmap BlankIcon;
		#endregion Fields


		#region Properties
		private SpritesetType _setType;
		/// <summary>
		/// The currently loaded
		/// <c><see cref="SpritesetType">SpritesetType</see></c>.
		/// </summary>
		/// <remarks>Sets <c><see cref="SpriteWidth"/></c> and
		/// <c><see cref="SpriteHeight"/></c>.</remarks>
		internal SpritesetType SetType
		{
			get { return _setType; }
			private set
			{
				switch (_setType = value)
				{
					case SpritesetType.Pck:
						SpriteWidth  = Spriteset.SpriteWidth32;
						SpriteHeight = Spriteset.SpriteHeight40;
						break;

					case SpritesetType.Bigobs:
						SpriteWidth  = Spriteset.SpriteWidth32;
						SpriteHeight = Spriteset.SpriteHeight48;
						break;

					case SpritesetType.ScanG:
						SpriteWidth  = Spriteset.ScanGside;
						SpriteHeight = Spriteset.ScanGside;
						break;

					case SpritesetType.LoFT:
						SpriteWidth  = Spriteset.LoFTside;
						SpriteHeight = Spriteset.LoFTside;
						break;
				}
			}
		}

		/// <summary>
		/// Sets the <c><see cref="SetType"/></c> externally if invoked via
		/// TileView.
		/// </summary>
		/// <param name="setType"></param>
		public void SetSpritesetType(SpritesetType setType)
		{
			SetType = setType;
		}

		/// <summary>
		/// The width of the sprites in the currently loaded
		/// <c><see cref="Spriteset"/></c>.
		/// </summary>
		/// <remarks>Set only by <c><see cref="SetType"/></c>.</remarks>
		internal int SpriteWidth
		{ get; private set; }

		/// <summary>
		/// The height of the sprites in the currently loaded
		/// <c><see cref="Spriteset"/></c>.
		/// </summary>
		/// <remarks>Set only by <c><see cref="SetType"/></c>.</remarks>
		internal int SpriteHeight
		{ get; private set; }


		/// <summary>
		/// The current <c><see cref="Palette"/></c> per the Palette menu.
		/// </summary>
		/// <remarks>Use
		/// <c><see cref="GetCurrentPalette()">GetCurrentPalette()</see></c> as
		/// appropriate since LoFTsets don't have a standard <c>Palette</c>.</remarks>
		internal Palette Pal
		{ get; private set; }

		/// <summary>
		/// The sprite-editor form.
		/// </summary>
		internal SpriteEditorF SpriteEditor
		{ get; private set; }

		/// <summary>
		/// The panel in which all sprites of a currently loaded spriteset are
		/// displayed.
		/// </summary>
		internal PckViewPanel TilePanel
		{ get; private set; }


		/// <summary>
		/// For reloading the Map when PckView is invoked via TileView.
		/// </summary>
		/// <remarks>Reload MapView's Map even if the <c>PCK+TAB</c> is saved as
		/// a different file; any modified terrain (etc) could be in the Map's
		/// terrainset or other resources.</remarks>
		public bool RequestReload
		{ get; private set; }


		/// <summary>
		/// The fullpath of the loaded spriteset. Shall not contain the
		/// file-extension for terrain/unit/bigobs files (since it's easier to
		/// add <c>.PCK</c> and <c>.TAB</c> strings later) but ScanG and LoFT
		/// files retain their <c>.DAT</c> extension.
		/// </summary>
		private string _path
		{ get; set; }

		private bool _changed;
		/// <summary>
		/// Sets the titlebar-text when a spriteset loads or gets changed.
		/// </summary>
		internal bool Changed
		{
			private get { return _changed; }
			set
			{
				if (TilePanel.Spriteset == null)
				{
					Text = TITLE;
					_changed = value;
				}
				else if (!value || _changed != value)
				{
					string text;
					switch (SetType)
					{
						case SpritesetType.Pck:
						case SpritesetType.Bigobs:
							text = GlobalsXC.PckExt_lc;
							break;

						default:
							text = String.Empty;
							break;
					}
					text = TITLE + GlobalsXC.PADDED_SEPARATOR + _path + text;

					if (value)
						text += GlobalsXC.PADDED_ASTERISK;

					Text = text;
					_changed = value;
				}
			}
		}

		/// <summary>
		/// <c>true</c> to bring subwindows front when a parent window takes
		/// focus.
		/// </summary>
		/// <remarks>This is optional since Windows10 fucks it up by not
		/// allowing focus to return to the <c>Form</c> on which focus needs to
		/// return.</remarks>
		internal bool Frontal
		{ get; private set; }
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor. Creates the PckView window.
		/// </summary>
		/// <param name="isInvoked"><c>true</c> if invoked via TileView</param>
		/// <param name="spriteshade">if <paramref name="isInvoked"/> is true
		/// you can pass in the <c>SpriteShade</c> value from MapView</param>
		public PckViewF(bool isInvoked = false, int spriteshade = 0)
		{
			IsInvoked = isInvoked;

			string dirAppL = Path.GetDirectoryName(Application.ExecutablePath);
#if DEBUG
			Logfile.SetPath(dirAppL, IsInvoked);
#endif

			InitializeComponent();

			// WORKAROUND: See note in MainViewF cTor.
			MaximumSize = new Size(0,0); // fu.net

			if (!IsInvoked)
				RegistryInfo.InitializeRegistry(dirAppL);

			RegistryInfo.RegisterProperties(this);
//			regInfo.AddProperty("SelectedPalette"); // + Transparency On/Off

			TilePanel = new PckViewPanel(this);
			TilePanel.Click       += OnPanelClick;
			TilePanel.DoubleClick += OnSpriteEditorClick;
			Controls.Add(TilePanel);

			SpriteEditor = new SpriteEditorF(this);
			SpriteEditor.FormClosing += OnSpriteEditorClosing;

			TilePanel.ContextMenuStrip = CreateContext();

			PrintSelected();
			PrintOver();


			// NOTE: Ben was a genius but this is fucked. He was devouring the
			// c#/.net language/framework when he set this 'pattern' up ... and
			// while his tactics were no doubt invaluable for research I've left
			// myself stuck with it so far ->

			string dirSetT = Path.Combine(dirAppL, PathInfo.DIR_Settings);	// path to the /settings dir
			var piConfig = new PathInfo(dirSetT, PathInfo.Pck_Config);		// define a PathInfo for 'PckConfig.yml'
			SharedSpace.SetShare(SharedSpace.PckConfigFile, piConfig);		// set a share for 'PckConfig.yml'

			// That's 3 variables just for 'PckConfig.yml' ...
			// - PathInfo.Pck_Config
			// - SharedSpace.PckConfigFile
			// - piConfig

			int palselected = 0;
			bool userconfig_spriteshade = true;

			LoadConfiguration(
							piConfig.Fullpath,
							ref userconfig_spriteshade,
							ref palselected);

			PopulatePaletteMenu(palselected); // WARNING: Palettes created here <-

			SpriteEditor._fpalette.PalPanel.SelectPalid((byte)0); // <- requires valid Pal


			BlankSprite = Properties.Resources.blanksprite;
			BlankIcon   = Properties.Resources.blankicon;


			miCreate.MenuItems.Add(miCreateTerrain);	// NOTE: These items are added to the Filemenu first
			miCreate.MenuItems.Add(miCreateBigobs);		// and get transfered to the Create submenu here.
			miCreate.MenuItems.Add(miCreateUnitUfo);
			miCreate.MenuItems.Add(miCreateUnitTftd);

			tssl_SpritesetLabel.Text = None;
			tssl_TilesTotal    .Text = Total + None;
			tssl_OffsetLast    .Text =
			tssl_OffsetAftr    .Text = String.Empty;

			ss_Status.Renderer = new CustomToolStripRenderer();


			SetSpriteshade(
						dirAppL,
						spriteshade,
						userconfig_spriteshade);

			LoadStartFile();

#if !__MonoCS__
			if (!IsInvoked)
			{
				// WindowFromPoint() is windows specific. If that ever works with
				// Mono remove this check. Better: Check if function exists or
				// replace function.
				if (Type.GetType("Mono.Runtime") == null)
				{
					Application.AddMessageFilter(this);
				}
				// else // "Mousewheel (partly) not handled. Reason: Mono runtime detected."
			}
#endif
		}


		// PckView shortcut table:
		// miCreateTerrain		CtrlR
		// miCreateBigobs		CtrlB
		// miCreateUnitUfo		CtrlU
		// miCreateUnitTftd		CtrlT
		// miOpen				CtrlO
		// miOpenBigobs			CtrlG
		// miOpenScanG			CtrlD
		// miOpenLoFT			CtrlM
		// miSave				CtrlS
		// miSaveAs				CtrlE
		// miExportSprites		CtrlP
		// miExportSpritesheet	F5
		// miImportSheetReplace	F6
		// miImportSheetAdd		F7
		// miQuit				CtrlQ
		// miCompare
		// miTransparent		F9
		// miSpriteShade		F10
		// palette items		Ctrl1..Ctrl8
		// miBytesTable			F11
		// miHelp				F1
		//
		// CONTEXT:
		// Edit					Enter
		// Add ...				d
		// InsertBefore ...		b
		// InsertAfter ...		a
		// Replace ...			r
		// MoveLeft				-
		// MoveRight			+
		// Delete				Delete
		// ExportSprite ...		p
		// create               t
		// clear                c

		/// <summary>
		/// Builds the RMB contextmenu.
		/// </summary>
		/// <returns></returns>
		private ContextMenuStrip CreateContext()
		{
			_miEditor      = new ToolStripMenuItem("Edit",              null, OnSpriteEditorClick);			// OnKeyDown Enter
			_miAdd         = new ToolStripMenuItem("Add ...",           null, OnAddSpritesClick);			// OnKeyDown d
			_miInsertBefor = new ToolStripMenuItem("Insert before ...", null, OnInsertSpritesBeforeClick);	// OnKeyDown b
			_miInsertAfter = new ToolStripMenuItem("Insert after ...",  null, OnInsertSpritesAfterClick);	// OnKeyDown a
			_miReplace     = new ToolStripMenuItem("Replace ...",       null, OnReplaceSpriteClick);		// OnKeyDown r
			_miMoveL       = new ToolStripMenuItem("Move left",         null, OnMoveLeftSpriteClick);		// OnKeyDown -
			_miMoveR       = new ToolStripMenuItem("Move right",        null, OnMoveRightSpriteClick);		// OnKeyDown +
			_miDelete      = new ToolStripMenuItem("Delete",            null, OnDeleteSpriteClick);			// OnKeyDown Delete
			_miExport      = new ToolStripMenuItem("Export sprite ...", null, OnExportSpriteClick);			// OnKeyDown p
			_miCreate      = new ToolStripMenuItem("create",            null, OnCreateSpriteClick);			// OnKeyDown t
			_miClear       = new ToolStripMenuItem("clear",             null, OnClearSpriteClick);			// OnKeyDown c

			_miEditor     .ShortcutKeyDisplayString = "Enter";
			_miAdd        .ShortcutKeyDisplayString = "d";
			_miInsertBefor.ShortcutKeyDisplayString = "b";
			_miInsertAfter.ShortcutKeyDisplayString = "a";
			_miReplace    .ShortcutKeyDisplayString = "r";
			_miMoveL      .ShortcutKeyDisplayString = "-";
			_miMoveR      .ShortcutKeyDisplayString = "+";
			_miDelete     .ShortcutKeyDisplayString = "Del";
			_miExport     .ShortcutKeyDisplayString = "p";
			_miCreate     .ShortcutKeyDisplayString = "t";
			_miClear      .ShortcutKeyDisplayString = "c";


			var context = new ContextMenuStrip();

			context.Items.Add(_miEditor);
			context.Items.Add(new ToolStripSeparator());
			context.Items.Add(_miAdd);
			context.Items.Add(_miInsertBefor);
			context.Items.Add(_miInsertAfter);
			context.Items.Add(new ToolStripSeparator());
			context.Items.Add(_miReplace);
			context.Items.Add(_miMoveL);
			context.Items.Add(_miMoveR);
			context.Items.Add(new ToolStripSeparator());
			context.Items.Add(_miDelete);
			context.Items.Add(new ToolStripSeparator());
			context.Items.Add(_miExport);
			context.Items.Add(new ToolStripSeparator());
			context.Items.Add(_miCreate);
			context.Items.Add(_miClear);

			_miAdd        .Enabled =
			_miInsertBefor.Enabled =
			_miInsertAfter.Enabled =
			_miReplace    .Enabled =
			_miMoveL      .Enabled =
			_miMoveR      .Enabled =
			_miDelete     .Enabled =
			_miExport     .Enabled =
			_miCreate     .Enabled =
			_miClear      .Enabled = false;

			return context;
		}

		/// <summary>
		/// Loads user-configuration from 'settings/PckConfig.yml'.
		/// </summary>
		/// <param name="pfe"></param>
		/// <param name="spriteshade"></param>
		/// <param name="pal"></param>
		private void LoadConfiguration(
				string pfe,
				ref bool spriteshade,
				ref int pal)
		{
			using (var fs = FileService.OpenFile(pfe, true)) // don't warn user if not found.
			if (fs != null)
			using (var sr = new StreamReader(pfe))
			{
				var ys = new YamlStream();
				ys.Load(sr);

				bool val_bool; int val_int;

				var root = ys.Documents[0].RootNode as YamlMappingNode;
				foreach (var node in root.Children)
				{
					string label = (node.Key as YamlScalarNode).Value;

					var keyvals = root.Children[new YamlScalarNode(label)] as YamlMappingNode;
					foreach (var keyval in keyvals)
					{
						switch (keyval.Key.ToString())
						{
							case "transparent":
								if (Boolean.TryParse(keyval.Value.ToString(), out val_bool))
									miTransparent.Checked = val_bool;
								break;

							case "spriteshade":
								if (Boolean.TryParse(keyval.Value.ToString(), out val_bool))
									spriteshade = val_bool;
								break;

							case "palette":
								if (Int32.TryParse(keyval.Value.ToString(), out val_int)
									&& val_int > -1 && val_int < 8)
								{
									pal = val_int;
								}
								break;

							case "front":
								if (Boolean.TryParse(keyval.Value.ToString(), out val_bool)
									&& (Frontal = miBringToFront.Checked = val_bool))
								{
									SpriteEditor.ShowInTaskbar =
									SpriteEditor._fpalette.ShowInTaskbar = false;
								}
								break;


							case "grid":
								if (Int32.TryParse(keyval.Value.ToString(), out val_int)
									&& val_int > 0 && val_int < 3)
								{
									switch (val_int)
									{
										case 1: SpriteEditor.OnGridDarkClick( null, EventArgs.Empty); break;
										case 2: SpriteEditor.OnGridLightClick(null, EventArgs.Empty); break;
									}
								}
								break;

							case "scale":
								if (Int32.TryParse(keyval.Value.ToString(), out val_int)
									&& val_int > 0 && val_int < 11)
								{
									SpriteEditor.SetScale(val_int);
								}
								break;

							case "edit":
								switch (keyval.Value.ToString())
								{
									case SpriteEditorF.EditEnabled:
										SpriteEditor.OnEditModeMouseClick(null, EventArgs.Empty);
										break;
								}
								break;
						}
					}
				}
			}
		}

		/// <summary>
		/// Adds <c><see cref="Palette">Palettes</see></c> as <c>MenuItems</c>
		/// to the Palettes menu on the main menubar.
		/// </summary>
		/// <param name="sel">the palette to select</param>
		private void PopulatePaletteMenu(int sel)
		{
			// instantiate the palettes iff not invoked by MapView - else the
			// palettes have already been instantiated

			var pals = new List<Palette>();

			pals.Add(Palette.UfoBattle);
			pals.Add(Palette.UfoGeo);
			pals.Add(Palette.UfoGraph);
			pals.Add(Palette.UfoResearch);
			pals.Add(Palette.TftdBattle);
			pals.Add(Palette.TftdGeo);
			pals.Add(Palette.TftdGraph);
			pals.Add(Palette.TftdResearch);


			MenuItem it;
			Palette pal;

			for (int i = 0; i != pals.Count; ++i)
			{
				pal = pals[i];
				it = new MenuItem(pal.Label, OnPaletteClick);	// I believe these will be disposed
				it.Tag = pal;									// when the Form gets closed since
				miPaletteMenu.MenuItems.Add(it);				// they are owned by 'miPaletteMenu'
				_itPalettes[pal] = it;							// which is owned/disposed by the Form.

				switch (i)
				{
					case 0: it.Shortcut = Shortcut.Ctrl1; break;
					case 1: it.Shortcut = Shortcut.Ctrl2; break;
					case 2: it.Shortcut = Shortcut.Ctrl3; break;
					case 3: it.Shortcut = Shortcut.Ctrl4; break;
					case 4: it.Shortcut = Shortcut.Ctrl5; break;
					case 5: it.Shortcut = Shortcut.Ctrl6; break;
					case 6: it.Shortcut = Shortcut.Ctrl7; break;
					case 7: it.Shortcut = Shortcut.Ctrl8; break;
				}
			}

			OnPaletteClick(_itPalettes[pals[sel]], EventArgs.Empty);
		}

		/// <summary>
		/// Deters the status of <c><see cref="Shader"/></c> and the value for
		/// <c><see cref="SpriteShadeFloat"/></c>.
		/// </summary>
		/// <param name="dirAppL">path to the application directory</param>
		/// <param name="spriteshade">spriteshade passed into constructor</param>
		/// <param name="userconfig_spriteshade"><c>true</c> if user-config turns on spriteshade</param>
		private void SetSpriteshade(
				string dirAppL,
				int spriteshade,
				bool userconfig_spriteshade)
		{
			bool @set = false;
			if (IsInvoked)
			{
				@set = spriteshade > 0;
			}
			else
			{
				string shade = GlobalsXC.GetSpriteShade(dirAppL); // get shade from MapView's options
				if (shade != null)
				{
					@set = Int32.TryParse(shade, out spriteshade)
						&& spriteshade > 0;
				}
			}

			if (@set)
			{
				miSpriteShade.Enabled = true;

				if (miSpriteShade.Checked = userconfig_spriteshade)
					Shader = ShaderOn;
				else
					Shader = ShaderOff;

				SpriteShadeFloat = (float)Math.Min(spriteshade, 99)
								 * GlobalsXC.SpriteShadeCoefficient;

				Ia = new ImageAttributes();
				Ia.SetGamma(SpriteShadeFloat, ColorAdjustType.Bitmap);
			}
		}

		/// <summary>
		/// Loads a file when PckView is started by a file association in
		/// FileExplorer.
		/// </summary>
		private void LoadStartFile()
		{
			if (_args != null && _args.Length != 0)
			{
				string file = Path.GetFileNameWithoutExtension(_args[0]).ToLower();
				switch (Path.GetExtension(_args[0]).ToLower())
				{
					case ".pck":
						// NOTE: LoadSpriteset() will check for a TAB file and
						// issue an error if not found.

						LoadSpriteset(_args[0], file.Contains("bigobs"));
						break;

					case ".dat":
						if (file.Contains("scang"))
						{
							LoadScanG(_args[0]);
						}
						else if (file.Contains("loftemps"))
						{
							LoadLoFT(_args[0]);
						}
						break;
				}
			}
		}
		#endregion cTor


		#region Events (override)
		/// <summary>
		/// Overrides the <c>Activated</c> handler. Brings all forms to top when
		/// this <c>PckViewF</c> is activated.
		/// </summary>
		/// <param name="e"></param>
		/// <seealso cref="OnSpriteEditorClick()"><c>OnSpriteEditorClick()</c></seealso>
		protected override void OnActivated(EventArgs e)
		{
			if (!BypassActivatedEvent && Frontal
				&& (SpriteEditor.Visible || SpriteEditor._fpalette.Visible))
			{
				BypassActivatedEvent = true;

				if (SpriteEditor.Visible)
					SpriteEditor.BringToFront();

				if (SpriteEditor._fpalette.Visible)
					SpriteEditor._fpalette.BringToFront();

				// TopMost true/false is the only thing I've found so far that
				// will actually bring focus back to the main PckView window.

				TopMost = true;
				TopMost = false;

				BypassActivatedEvent = false;
			}
		}

		/// <summary>
		/// Overrides the <c>Resize</c> handler. Minimizes and restores this
		/// <c>PckViewF</c> along with <c><see cref="SpriteEditor"/></c> and
		/// <c><see cref="SpriteEditorF._fpalette">SpriteEditorF._fpalette</see></c>
		/// synchronistically.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);

			if (WindowState == FormWindowState.Minimized)
			{
				_minimized = true;

				if (SpriteEditor.Visible)
					SpriteEditor.WindowState = FormWindowState.Minimized;

				if (SpriteEditor._fpalette.Visible)
					SpriteEditor._fpalette.WindowState = FormWindowState.Minimized;
			}
			else if (_minimized)
			{
				_minimized = false;

				if (SpriteEditor.Visible)
					SpriteEditor.WindowState = FormWindowState.Normal;

				if (SpriteEditor._fpalette.Visible)
					SpriteEditor._fpalette.WindowState = FormWindowState.Normal;
			}
		}

		/// <summary>
		/// Overrides the <c>Shown</c> handler. Focuses
		/// <c><see cref="TilePanel"/></c> after the app loads.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnShown(EventArgs e)
		{
			TilePanel.Select();
			base.OnShown(e);
		}

		/// <summary>
		/// Overrides the <c>FormClosing</c> handler. Closes the app after a
		/// .net call to close (roughly).
		/// </summary>
		/// <param name="e"></param>
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			if (!RegistryInfo.FastClose(e.CloseReason))
			{
				if (RequestSpritesetClose())
				{
					RegistryInfo.UpdateRegistry(this);

					SaveConfiguration();

					Quit = true;

					SpriteEditor.ClosePalette();	// these are needed when PckView is invoked via TileView
					SpriteEditor.Close();			// it's also just good procedure

					TilePanel.Destroy();

//					Ia.Dispose(); // fxCop ca2213 - wants this in Dispose() despite not caring about BlankSprite or BlankIcon.

					BlankSprite.Dispose();
					BlankIcon  .Dispose();

					ByteTableManager.HideTable();

					if (!IsInvoked)
						RegistryInfo.WriteRegistry();
				}
				else
					e.Cancel = true;
			}
			base.OnFormClosing(e);
		}

		/// <summary>
		/// Saves user-configuration to 'settings/PckConfig.yml'.
		/// </summary>
		private void SaveConfiguration()
		{
			string pfe = ((PathInfo)SharedSpace.GetShareObject(SharedSpace.PckConfigFile)).Fullpath; // gfl

			string pfeT;
			if (File.Exists(pfe))
				pfeT = pfe + GlobalsXC.TEMPExt;
			else
				pfeT = pfe;

			bool fail = true;
			using (var fs = FileService.CreateFile(pfeT))
			if (fs != null)
			{
				fail = false;

				int pal = 0;
				foreach (var entry in _itPalettes)
				{
					if ((entry.Value as MenuItem).Checked)
						break;
					++pal;
				}

				using (var sw = new StreamWriter(fs))
				{
					sw.WriteLine("PckConfig:");

					// PckViewF
					sw.WriteLine("  transparent: " + miTransparent.Checked);
					sw.WriteLine("  spriteshade: " + miSpriteShade.Checked);
					sw.WriteLine("  palette: "     + pal);
					sw.WriteLine("  front: "       + miBringToFront.Checked);

					// SpriteEditorF
					sw.WriteLine("  grid: "  + SpriteEditor.GetGridConfig());
					sw.WriteLine("  scale: " + SpriteEditor._scaler);
					sw.WriteLine("  edit: "  + SpriteEditorF.Mode);
				}
			}

			if (!fail && pfeT != pfe)
				FileService.ReplaceFile(pfe);
		}

		/// <summary>
		/// Overrides the <c>KeyDown</c> handler. Context and navigation
		/// shortcuts.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			//Logfile.Log("PckViewF.OnKeyDown() " + e.KeyData);

			switch (e.KeyData)
			{
				// Context shortcuts ->

				case Keys.Enter:											// edit
					e.Handled = e.SuppressKeyPress = true;
					OnSpriteEditorClick(null, EventArgs.Empty);
					break;

				case Keys.D:												// add
					if (_miAdd.Enabled)
					{
						e.Handled = e.SuppressKeyPress = true;
						OnAddSpritesClick(null, EventArgs.Empty);
					}
					break;

				case Keys.B:												// insert before
					if (_miInsertBefor.Enabled)
					{
						e.Handled = e.SuppressKeyPress = true;
						OnInsertSpritesBeforeClick(null, EventArgs.Empty);
					}
					break;

				case Keys.A:												// insert after
					if (_miInsertAfter.Enabled)
					{
						e.Handled = e.SuppressKeyPress = true;
						OnInsertSpritesAfterClick(null, EventArgs.Empty);
					}
					break;

				case Keys.R:												// replace
					if (_miReplace.Enabled)
					{
						e.Handled = e.SuppressKeyPress = true;
						OnReplaceSpriteClick(null, EventArgs.Empty);
					}
					break;

				case Keys.OemMinus: // drugs ...
				case Keys.Subtract:											// move left
					if (_miMoveL.Enabled)
					{
						e.Handled = e.SuppressKeyPress = true;
						OnMoveLeftSpriteClick(null, EventArgs.Empty);
					}
					break;

				case Keys.Oemplus: // drugs ...
				case Keys.Add:												// move right
					if (_miMoveR.Enabled)
					{
						e.Handled = e.SuppressKeyPress = true;
						OnMoveRightSpriteClick(null, EventArgs.Empty);
					}
					break;

				case Keys.Delete:											// delete
					if (_miDelete.Enabled)
					{
						e.Handled = e.SuppressKeyPress = true;
						OnDeleteSpriteClick(null, EventArgs.Empty);
					}
					break;

				case Keys.P:												// export
					if (_miExport.Enabled)
					{
						e.Handled = e.SuppressKeyPress = true;
						OnExportSpriteClick(null, EventArgs.Empty);
					}
					break;

				case Keys.T:												// create
					if (_miCreate.Enabled)
					{
						e.Handled = e.SuppressKeyPress = true;
						OnCreateSpriteClick(null, EventArgs.Empty);
					}
					break;

				case Keys.C:												// clear
					if (_miClear.Enabled)
					{
						e.Handled = e.SuppressKeyPress = true;
						OnClearSpriteClick(null, EventArgs.Empty);
					}
					break;


				// Navigation shortcuts ->

				// TODO: [Home] [End] [Ctrl+Home] [Ctrl+End] [PgUp] [PgDn]

				case Keys.Left:
					if (TilePanel.Spriteset != null)
					{
						e.Handled = e.SuppressKeyPress = true;
						TilePanel.SelectAdjacentHori(-1);
					}
					break;

				case Keys.Right:
					if (TilePanel.Spriteset != null)
					{
						e.Handled = e.SuppressKeyPress = true;
						TilePanel.SelectAdjacentHori(+1);
					}
					break;

				case Keys.Up:
					if (TilePanel.Spriteset != null)
					{
						e.Handled = e.SuppressKeyPress = true;
						TilePanel.SelectAdjacentVert(-1);
					}
					break;

				case Keys.Down:
					if (TilePanel.Spriteset != null)
					{
						e.Handled = e.SuppressKeyPress = true;
						TilePanel.SelectAdjacentVert(+1);
					}
					break;


				case Keys.Escape:
					if (TilePanel.Spriteset != null)
					{
						e.Handled = e.SuppressKeyPress = true;
						if (SetSelected(-1)) TilePanel.Invalidate();
					}
					break;
			}

			base.OnKeyDown(e);
		}
		#endregion Events (override)


		#region Events
		/// <summary>
		/// Bring back the dinosaurs.
		/// </summary>
		/// <param name="sender"><c><see cref="TilePanel"/></c> - <c>Click</c></param>
		/// <param name="e"></param>
		/// <seealso cref="PckViewPanel"><c>PckViewPanel</c></seealso>
		/// <remarks>This fires after <c>PckViewPanel.OnMouseDown()</c>.</remarks>
		private void OnPanelClick(object sender, EventArgs e)
		{
			EnableContext();
		}

		/// <summary>
		/// Opens the currently selected sprite in
		/// <c><see cref="SpriteEditorF"/></c>.
		/// </summary>
		/// <param name="sender">
		/// <list type="bullet">
		/// <item><c><see cref="_miEditor"/></c> - <c>Click</c></item>
		/// <item><c><see cref="TilePanel"/></c> - <c>DoubleClick</c></item>
		/// <item><c>null</c> - <c><see cref="OnKeyDown()">OnKeyDown()</see></c> <c>[Enter]</c></item>
		/// </list></param>
		/// <param name="e"></param>
		/// <seealso cref="PckViewPanel"><c>PckViewPanel</c></seealso>
		/// <seealso cref="OnActivated()"><c>OnActivated()</c></seealso>
		private void OnSpriteEditorClick(object sender, EventArgs e)
		{
			if (TilePanel.Spriteset != null)
			{
				if (!_miEditor.Checked)
				{
					BypassActivatedEvent = true;

					_miEditor.Checked = true;
					SpriteEditor.Show();

					SpriteEditor.OnShowPaletteClick(null, EventArgs.Empty);

					TopMost = true;
					TopMost = false;

					BypassActivatedEvent = false;
				}
				else
					SpriteEditor.BringToFront();
			}
		}

		/// <summary>
		/// Dechecks <c><see cref="_miEditor"/></c>.
		/// </summary>
		/// <param name="sender"><c><see cref="SpriteEditor"/></c></param>
		/// <param name="e"></param>
		/// <remarks>This fires after the editor's <c>FormClosing</c> event.</remarks>
		/// <seealso cref="SpriteEditorF"><c>SpriteEditorF</c></seealso>
		private void OnSpriteEditorClosing(object sender, CancelEventArgs e)
		{
			_miEditor.Checked = false;
		}


		/// <summary>
		/// Adds sprite(s) to
		/// <c><see cref="PckViewPanel.Spriteset">PckViewPanel.Spriteset</see></c>.
		/// </summary>
		/// <param name="sender">
		/// <list type="bullet">
		/// <item><c><see cref="_miAdd"/></c> - <c>Click</c></item>
		/// <item><c>null</c> - <c><see cref="OnKeyDown()">OnKeyDown()</see></c> <c>[d]</c></item>
		/// </list></param>
		/// <param name="e"></param>
		private void OnAddSpritesClick(object sender, EventArgs e)
		{
			using (var ofd = new OpenFileDialog())
			{
				ofd.Title  = FileDialogStrings.GetTitle(SetType, true);
				ofd.Filter = FileDialogStrings.GetFilter();

				if (Directory.Exists(_lastSpriteDirectory))
					ofd.InitialDirectory = _lastSpriteDirectory;
				else
				{
					string dir = Path.GetDirectoryName(_path);
					if (Directory.Exists(dir))
						ofd.InitialDirectory = dir;
				}

				ofd.Multiselect =
				ofd.RestoreDirectory = true;


				if (ofd.ShowDialog(this) == DialogResult.OK)
				{
					_lastSpriteDirectory = Path.GetDirectoryName(ofd.FileName);

					var bs = new List<Bitmap>();

					bool valid = true; // first run a check against all sprites and if any are borked set error.
					for (int i = 0; valid && i != ofd.FileNames.Length; ++i)
					{
						valid = false;

//						var b = new Bitmap(ofd.FileNames[i]);	// <- .net.bork. Creates a 32-bpp Argb image if source is
																// 8-bpp PNG w/transparency; GIF,BMP however retains 8-bpp format.

						byte[] bindata = FileService.ReadFile(ofd.FileNames[i]);
						if (bindata != null)
						{
							Bitmap b = SpriteLoader.LoadImageData(bindata, ofd.FileNames[i]);

							if (b != null)
							{
								bs.Add(b);

								if (!(valid = b.Width       == SpriteWidth
										   && b.Height      == SpriteHeight
										   && b.PixelFormat == PixelFormat.Format8bppIndexed))
								{
									ShowBitmapError(ofd.FileNames[i], b);
								}
							}
						}
					}

					if (valid)
					{
						int id = TilePanel.Spriteset.Count;

						foreach (var b in bs)
						{
							XCImage sprite = SpriteService.CreateSanitarySprite(
																			b,
																			TilePanel.Spriteset.Count,
																			GetCurrentPalette(),
																			SpriteWidth,
																			SpriteHeight,
																			SetType);
							TilePanel.Spriteset.Sprites.Add(sprite);
						}

						SpritesetCountChanged(id);
					}

					foreach (var b in bs)
						b.Dispose();
				}
			}
		}

		/// <summary>
		/// Inserts sprite(s) into
		/// <c><see cref="PckViewPanel.Spriteset">PckViewPanel.Spriteset</see></c>
		/// before the selected sprite.
		/// </summary>
		/// <param name="sender">
		/// <list type="bullet">
		/// <item><c><see cref="_miInsertBefor"/></c> - <c>Click</c></item>
		/// <item><c>null</c> - <c><see cref="OnKeyDown()">OnKeyDown()</see></c> <c>[b]</c></item>
		/// </list></param>
		/// <param name="e"></param>
		private void OnInsertSpritesBeforeClick(object sender, EventArgs e)
		{
			using (var ofd = new OpenFileDialog())
			{
				ofd.Title  = FileDialogStrings.GetTitle(SetType, true);
				ofd.Filter = FileDialogStrings.GetFilter();

				if (Directory.Exists(_lastSpriteDirectory))
					ofd.InitialDirectory = _lastSpriteDirectory;
				else
				{
					string dir = Path.GetDirectoryName(_path);
					if (Directory.Exists(dir))
						ofd.InitialDirectory = dir;
				}

				ofd.Multiselect =
				ofd.RestoreDirectory = true;


				if (ofd.ShowDialog(this) == DialogResult.OK)
				{
					_lastSpriteDirectory = Path.GetDirectoryName(ofd.FileName);

					if (InsertSprites(TilePanel.Selid, ofd.FileNames))
					{
						SpritesetCountChanged(TilePanel.Selid + ofd.FileNames.Length);
					}
				}
			}
		}

		/// <summary>
		/// Inserts sprite(s) into
		/// <c><see cref="PckViewPanel.Spriteset">PckViewPanel.Spriteset</see></c>
		/// after the selected sprite.
		/// </summary>
		/// <param name="sender">
		/// <list type="bullet">
		/// <item><c><see cref="_miInsertAfter"/></c> - <c>Click</c></item>
		/// <item><c>null</c> - <c><see cref="OnKeyDown()">OnKeyDown()</see></c> <c>[a]</c></item>
		/// </list></param>
		/// <param name="e"></param>
		private void OnInsertSpritesAfterClick(object sender, EventArgs e)
		{
			using (var ofd = new OpenFileDialog())
			{
				ofd.Title  = FileDialogStrings.GetTitle(SetType, true);
				ofd.Filter = FileDialogStrings.GetFilter();

				if (Directory.Exists(_lastSpriteDirectory))
					ofd.InitialDirectory = _lastSpriteDirectory;
				else
				{
					string dir = Path.GetDirectoryName(_path);
					if (Directory.Exists(dir))
						ofd.InitialDirectory = dir;
				}

				ofd.Multiselect =
				ofd.RestoreDirectory = true;


				if (ofd.ShowDialog(this) == DialogResult.OK)
				{
					_lastSpriteDirectory = Path.GetDirectoryName(ofd.FileName);

					if (InsertSprites(TilePanel.Selid + 1, ofd.FileNames))
					{
						SpritesetCountChanged(TilePanel.Selid);
					}
				}
			}
		}

		/// <summary>
		/// Inserts sprites into the currently loaded spriteset starting at a
		/// given Id.
		/// </summary>
		/// <param name="id">the id to start inserting at</param>
		/// <param name="files">an array of filenames</param>
		/// <returns><c>true</c> if sprites are inserted</returns>
		/// <remarks>Helper for
		/// <list type="bullet">
		/// <item><c><see cref="OnInsertSpritesBeforeClick()">OnInsertSpritesBeforeClick()</see></c></item>
		/// <item><c><see cref="OnInsertSpritesAfterClick()">OnInsertSpritesAfterClick()</see></c></item>
		/// </list></remarks>
		private bool InsertSprites(int id, string[] files)
		{
			var bs = new List<Bitmap>();

			bool valid = true; // first run a check against all sprites and if any are borked exit w/ false.
			for (int i = 0; valid && i != files.Length; ++i)
			{
				valid = false;

				byte[] bindata = FileService.ReadFile(files[i]);
				if (bindata != null)
				{
					Bitmap b = SpriteLoader.LoadImageData(bindata, files[i]);

					if (b != null)
					{
						bs.Add(b);

						if (!(valid = b.Width       == SpriteWidth
								   && b.Height      == SpriteHeight
								   && b.PixelFormat == PixelFormat.Format8bppIndexed))
						{
							ShowBitmapError(files[i], b);
						}
					}
				}
			}

			if (valid)
			{
				int length = bs.Count;
				for (int i = id; i != TilePanel.Spriteset.Count; ++i)
					TilePanel.Spriteset[i].Id = i + length;

				foreach (var b in bs)
				{
					XCImage sprite = SpriteService.CreateSanitarySprite(
																	b,
																	id,
																	GetCurrentPalette(),
																	SpriteWidth,
																	SpriteHeight,
																	SetType);
					TilePanel.Spriteset.Sprites.Insert(id++, sprite);
				}
			}

			foreach (var b in bs)
				b.Dispose();

			return valid;
		}

		/// <summary>
		/// Finishes an operation that changed the spriteset-count.
		/// </summary>
		/// <param name="id">sprite-id to select</param>
		private void SpritesetCountChanged(int id)
		{
			SetSelected(id, true);

			PrintTotal();

			TilePanel.ForceResize();
			TilePanel.Invalidate();

			Changed = true;
		}

		/// <summary>
		/// Replaces the selected sprite in
		/// <c><see cref="PckViewPanel.Spriteset">PckViewPanel.Spriteset</see></c>
		/// with a different sprite.
		/// </summary>
		/// <param name="sender">
		/// <list type="bullet">
		/// <item><c><see cref="_miReplace"/></c> - <c>Click</c></item>
		/// <item><c>null</c> - <c><see cref="OnKeyDown()">OnKeyDown()</see></c> <c>[r]</c></item>
		/// </list></param>
		/// <param name="e"></param>
		private void OnReplaceSpriteClick(object sender, EventArgs e)
		{
			using (var ofd = new OpenFileDialog())
			{
				ofd.Title  = FileDialogStrings.GetTitle(SetType, false);
				ofd.Filter = FileDialogStrings.GetFilter();

				if (Directory.Exists(_lastSpriteDirectory))
					ofd.InitialDirectory = _lastSpriteDirectory;
				else
				{
					string dir = Path.GetDirectoryName(_path);
					if (Directory.Exists(dir))
						ofd.InitialDirectory = dir;
				}

				ofd.RestoreDirectory = true;


				if (ofd.ShowDialog(this) == DialogResult.OK)
				{
					_lastSpriteDirectory = Path.GetDirectoryName(ofd.FileName);

					byte[] bindata = FileService.ReadFile(ofd.FileName);
					if (bindata != null) // else error was shown by FileService.
					{
						using (Bitmap b = SpriteLoader.LoadImageData(bindata, ofd.FileName))
						{
							if (b != null) // else error was shown by SpriteLoader.
							{
								if (   b.Width       != SpriteWidth
									|| b.Height      != SpriteHeight
									|| b.PixelFormat != PixelFormat.Format8bppIndexed)
								{
									ShowBitmapError(ofd.FileName, b);
								}
								else
								{
									XCImage sprite = SpriteService.CreateSanitarySprite(
																					b,
																					TilePanel.Selid,
																					GetCurrentPalette(),
																					SpriteWidth,
																					SpriteHeight,
																					SetType);

									TilePanel.Spriteset[TilePanel.Selid].Dispose();
									TilePanel.Spriteset[TilePanel.Selid] = sprite;

									SetSelected(TilePanel.Selid, true);

									TilePanel.Refresh();
									Changed = true;
								}
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Displays an errorbox to the user about incorrect image dimensions
		/// and/or pixel-format.
		/// </summary>
		/// <param name="pfe">path-file-extension</param>
		/// <param name="b">a <c>Bitmap</c></param>
		/// <param name="spritesheet"><c>true</c> if the error occured when
		/// importing a spritesheet</param>
		private void ShowBitmapError(string pfe, Image b, bool spritesheet = false)
		{
			using (var f = new Infobox(
									"Image error",
									FileDialogStrings.GetError(SetType, spritesheet),
									pfe + Environment.NewLine + Environment.NewLine
										+ b.Width + "x" + b.Height + " " + b.PixelFormat,
									InfoboxType.Error))
			{
				f.ShowDialog(this);
			}
		}

		/// <summary>
		/// Moves a sprite one slot to the left.
		/// </summary>
		/// <param name="sender">
		/// <list type="bullet">
		/// <item><c><see cref="_miMoveL"/></c> - <c>Click</c></item>
		/// <item><c>null</c> - <c><see cref="OnKeyDown()">OnKeyDown()</see></c> <c>[-]</c></item>
		/// </list></param>
		/// <param name="e"></param>
		private void OnMoveLeftSpriteClick(object sender, EventArgs e)
		{
			MoveSprite(-1);
		}

		/// <summary>
		/// Moves a sprite one slot to the right.
		/// </summary>
		/// <param name="sender">
		/// <list type="bullet">
		/// <item><c><see cref="_miMoveR"/></c> - <c>Click</c></item>
		/// <item><c>null</c> - <c><see cref="OnKeyDown()">OnKeyDown()</see></c> <c>[+]</c></item>
		/// </list></param>
		/// <param name="e"></param>
		private void OnMoveRightSpriteClick(object sender, EventArgs e)
		{
			MoveSprite(+1);
		}

		/// <summary>
		/// Moves a sprite to the left or right by one slot.
		/// </summary>
		/// <param name="dir">-1 to move left, +1 to move right</param>
		private void MoveSprite(int dir)
		{
			int id = TilePanel.Selid;

			var sprite = TilePanel.Spriteset[id];

			TilePanel.Spriteset[id] = TilePanel.Spriteset[id + dir];
			TilePanel.Spriteset[id + dir] = sprite;

			TilePanel.Spriteset[id].Id = id;
			TilePanel.Spriteset[id + dir].Id = id + dir;

			SetSelected(id + dir);

			TilePanel.Refresh();
			Changed = true;
		}

		/// <summary>
		/// Deletes the selected sprite in
		/// <c><see cref="PckViewPanel.Spriteset">PckViewPanel.Spriteset</see></c>.
		/// </summary>
		/// <param name="sender">
		/// <list type="bullet">
		/// <item><c><see cref="_miDelete"/></c> - <c>Click</c></item>
		/// <item><c>null</c> - <c><see cref="OnKeyDown()">OnKeyDown()</see></c> <c>[Del]</c></item>
		/// </list></param>
		/// <param name="e"></param>
		private void OnDeleteSpriteClick(object sender, EventArgs e)
		{
			int id = TilePanel.Selid;

			TilePanel.Spriteset.Sprites[id].Dispose();
			TilePanel.Spriteset.Sprites.RemoveAt(id);

			for (int i = id; i != TilePanel.Spriteset.Count; ++i)
				TilePanel.Spriteset[i].Id = i;

			if (id == TilePanel.Spriteset.Count)
				id = -1;

			SpritesetCountChanged(id);
		}

		/// <summary>
		/// Exports the selected sprite in
		/// <c><see cref="PckViewPanel.Spriteset">PckViewPanel.Spriteset</see></c>
		/// to a <c>PNG</c> file.
		/// </summary>
		/// <param name="sender">
		/// <list type="bullet">
		/// <item><c><see cref="_miExport"/></c> - <c>Click</c></item>
		/// <item><c>null</c> - <c><see cref="OnKeyDown()">OnKeyDown()</see></c> <c>[p]</c></item>
		/// </list></param>
		/// <param name="e"></param>
		private void OnExportSpriteClick(object sender, EventArgs e)
		{
			int count = TilePanel.Spriteset.Count;
			string digits = String.Empty;
			do
			{ digits += "0"; }
			while ((count /= 10) != 0);

			string suffix = String.Format(
										"_{0:" + digits + "}",
										TilePanel.Selid);

			using (var sfd = new SaveFileDialog())
			{
				sfd.Title      = "Export sprite to 8-bpp PNG file";
				sfd.Filter     = FileDialogStrings.GetFilterPng();
				sfd.DefaultExt = GlobalsXC.PngExt;
				sfd.FileName   = TilePanel.Spriteset.Label.ToUpperInvariant() + suffix;

				if (!Directory.Exists(_lastSpriteDirectory))
				{
					string dir = Path.GetDirectoryName(_path);
					if (Directory.Exists(dir))
						sfd.InitialDirectory = dir;
				}
				else
					sfd.InitialDirectory = _lastSpriteDirectory;

				sfd.RestoreDirectory = true;


				if (sfd.ShowDialog(this) == DialogResult.OK)
				{
					_lastSpriteDirectory = Path.GetDirectoryName(sfd.FileName);

					// TODO: Ask to overwrite an existing file.
					SpriteService.ExportSprite(
											sfd.FileName,
											TilePanel.Spriteset[TilePanel.Selid].Sprite);
				}
			}
		}

		/// <summary>
		/// Creates a blank sprite at the end of
		/// <c><see cref="PckViewPanel.Spriteset">PckViewPanel.Spriteset</see></c>.
		/// </summary>
		/// <param name="sender">
		/// <list type="bullet">
		/// <item><c><see cref="_miCreate"/></c> - <c>Click</c></item>
		/// <item><c>null</c> - <c><see cref="OnKeyDown()">OnKeyDown()</see></c> <c>[t]</c></item>
		/// </list></param>
		/// <param name="e"></param>
		/// <remarks>The routine is bloated but it works.</remarks>
		private void OnCreateSpriteClick(object sender, EventArgs e)
		{
			using (var b = SpriteService.CreateTransparent(
														SpriteWidth,
														SpriteHeight,
														GetCurrentPalette().Table))
			{
				XCImage sprite = SpriteService.CreateSanitarySprite(
																b,
																TilePanel.Spriteset.Count,
																GetCurrentPalette(),
																SpriteWidth,
																SpriteHeight,
																SetType);
				TilePanel.Spriteset.Sprites.Add(sprite);

				SpritesetCountChanged(TilePanel.Spriteset.Count - 1);
			}
		}

		/// <summary>
		/// Clears the selected sprite in
		/// <c><see cref="PckViewPanel.Spriteset">PckViewPanel.Spriteset</see></c>.
		/// </summary>
		/// <param name="sender">
		/// <list type="bullet">
		/// <item><c><see cref="_miClear"/></c> - <c>Click</c></item>
		/// <item><c>null</c> - <c><see cref="OnKeyDown()">OnKeyDown()</see></c> <c>[c]</c></item>
		/// </list></param>
		/// <param name="e"></param>
		private void OnClearSpriteClick(object sender, EventArgs e)
		{
			XCImage sprite = TilePanel.Spriteset[TilePanel.Selid];
			if (!sprite.Istid())
			{
				byte[] bindata = sprite.GetBindata();
				for (int i = 0; i != bindata.Length; ++i)
					bindata[i] = Palette.Tid; // works for LoTFsets also.

				Bitmap b = SpriteService.CreateSprite(
												SpriteWidth,
												SpriteHeight,
												bindata,
												GetCurrentPalette().Table);
				sprite.Dispose();
				sprite.Sprite = b;

				SetSelected(TilePanel.Selid, true);

				TilePanel.Invalidate();
				Changed = true;
			}
		}


		/// <summary>
		/// Creates a brand sparkling new (blank)
		/// <c><see cref="PckViewPanel.Spriteset">PckViewPanel.Spriteset</see></c>.
		/// </summary>
		/// <param name="sender">
		/// <list type="bullet">
		/// <item><c><see cref="miCreateTerrain"/></c></item>
		/// <item><c><see cref="miCreateBigobs"/></c></item>
		/// <item><c><see cref="miCreateUnitUfo"/></c></item>
		/// <item><c><see cref="miCreateUnitTftd"/></c></item>
		/// </list></param>
		/// <param name="e"></param>
		/// <remarks><c>ScanG.dat</c> and <c>LoFTemps.dat</c> cannot be created.</remarks>
		private void OnCreateClick(object sender, EventArgs e)
		{
			if (RequestSpritesetClose())
			{
				using (var sfd = new SaveFileDialog())
				{
					sfd.Filter     = FileDialogStrings.GetFilterPck();
					sfd.DefaultExt = GlobalsXC.PckExt;

					string text;
					if (sender == miCreateBigobs)			// Bigobs support for XCImage/PckSprite
						text = "Bigobs";
					else if (sender == miCreateUnitTftd)	// Tftd Unit support for XCImage/PckSprite
						text = "Tftd Unit";
					else if (sender == miCreateUnitUfo)		// Ufo Unit support for XCImage/PckSprite
						text = "Ufo Unit";
					else //if (sender == miCreateTerrain)	// Terrain support for XCImage/PckSprite
						text = "Terrain";

					sfd.Title = "Create " + text + " pck+tab files";

					if (Directory.Exists(_lastCreateDirectory))
						sfd.InitialDirectory = _lastCreateDirectory;
					else if (_path != null)
					{
						string dir = Path.GetDirectoryName(_path);
						if (Directory.Exists(dir))
							sfd.InitialDirectory = dir;
					}


					if (sfd.ShowDialog(this) == DialogResult.OK)
					{
						string pfe = sfd.FileName;
						_lastCreateDirectory = Path.GetDirectoryName(pfe);

						string label = Path.GetFileNameWithoutExtension(pfe);
						string pf    = Path.Combine(Path.GetDirectoryName(pfe), label);

						string pfePck = pf + GlobalsXC.PckExt;
						string pfeTab = pf + GlobalsXC.TabExt;

						string pfePckT = pfePck;
						string pfeTabT = pfeTab;
						if (File.Exists(pfePck)) pfePckT += GlobalsXC.TEMPExt;
						if (File.Exists(pfeTab)) pfeTabT += GlobalsXC.TEMPExt;

						// NOTE: Use 'fail' to allow the files to unlock - for
						// ReplaceFile() if necessary - after they get created.
						bool fail = true;

						using (var fsPck = FileService.CreateFile(pfePckT)) // create 0-byte file
						if (fsPck != null)
						using (var fsTab = FileService.CreateFile(pfeTabT)) // create 0-byte file
						if (fsTab != null)
							fail = false;

						if (!fail
							&& (pfePckT == pfePck || FileService.ReplaceFile(pfePck))
							&& (pfeTabT == pfeTab || FileService.ReplaceFile(pfeTab)))
						{
							if (sender == miCreateBigobs)
								SetType = SpritesetType.Bigobs;
							else
								SetType = SpritesetType.Pck;

//							if (!_itPalettes[pal].Checked)
//							{
//								miTransparent.Checked = true;
//								OnPaletteClick(_itPalettes[pal], EventArgs.Empty);
//							}
//							else if (!miTransparent.Checked)
//							{
//								OnTransparencyClick(null, EventArgs.Empty);
//							}

							TilePanel.Spriteset = new Spriteset(
															label,
															Pal,
															Spriteset.SpriteWidth32,
															((SetType == SpritesetType.Bigobs) ? Spriteset.SpriteHeight48
																							   : Spriteset.SpriteHeight40),
															((sender == miCreateUnitTftd) ? SpritesetManager.TAB_WORD_LENGTH_4
																						  : SpritesetManager.TAB_WORD_LENGTH_2));
							_path = pf;
							Changed = false;
						}
					}
				}
			}
		}

		/// <summary>
		/// Opens a spriteset of a terrain or a unit.
		/// </summary>
		/// <param name="sender"><c><see cref="miOpen"/></c></param>
		/// <param name="e"></param>
		private void OnOpenPckClick(object sender, EventArgs e)
		{
			if (RequestSpritesetClose())
			{
				using (var ofd = new OpenFileDialog())
				{
					ofd.Title  = "Select a PCK (terrain/unit) file";
					ofd.Filter = FileDialogStrings.GetFilterPck();

					if (_path != null)
					{
						string dir = Path.GetDirectoryName(_path);
						if (Directory.Exists(dir))
							ofd.InitialDirectory = dir;
					}


					if (ofd.ShowDialog(this) == DialogResult.OK)
						LoadSpriteset(ofd.FileName);
				}
			}
		}

		/// <summary>
		/// Opens a spriteset of bigobs.
		/// </summary>
		/// <param name="sender"><c><see cref="miOpenBigobs"/></c></param>
		/// <param name="e"></param>
		private void OnOpenBigobsClick(object sender, EventArgs e)
		{
			if (RequestSpritesetClose())
			{
				using (var ofd = new OpenFileDialog())
				{
					ofd.Title    = "Select a PCK (bigobs) file";
					ofd.Filter   = FileDialogStrings.GetFilterPck();
					ofd.FileName = "BIGOBS.PCK";

					if (_path != null)
					{
						string dir = Path.GetDirectoryName(_path);
						if (Directory.Exists(dir))
							ofd.InitialDirectory = dir;
					}


					if (ofd.ShowDialog(this) == DialogResult.OK)
						LoadSpriteset(ofd.FileName, true);
				}
			}
		}

		/// <summary>
		/// Opens a spriteset of ScanG icons.
		/// </summary>
		/// <param name="sender"><c><see cref="miOpenScanG"/></c></param>
		/// <param name="e"></param>
		private void OnOpenScanGClick(object sender, EventArgs e)
		{
			if (RequestSpritesetClose())
			{
				using (var ofd = new OpenFileDialog())
				{
					ofd.Title    = "Select a ScanG file";
					ofd.Filter   = FileDialogStrings.GetFilterDat();
					ofd.FileName = "SCANG.DAT";


					if (ofd.ShowDialog(this) == DialogResult.OK)
						LoadScanG(ofd.FileName);
				}
			}
		}

		/// <summary>
		/// Opens a spriteset of LoFT icons.
		/// </summary>
		/// <param name="sender"><c><see cref="miOpenLoFT"/></c></param>
		/// <param name="e"></param>
		private void OnOpenLoFTClick(object sender, EventArgs e)
		{
			if (RequestSpritesetClose())
			{
				using (var ofd = new OpenFileDialog())
				{
					ofd.Title    = "Select a LoFTemps file";
					ofd.Filter   = FileDialogStrings.GetFilterDat();
					ofd.FileName = "LOFTEMPS.DAT";


					if (ofd.ShowDialog(this) == DialogResult.OK)
						LoadLoFT(ofd.FileName);
				}
			}
		}

		/// <summary>
		/// Opens a dialog to create a Tabfile based on a Pckfile.
		/// </summary>
		/// <param name="sender"><c><see cref="miCreateTab"/></c></param>
		/// <param name="e"></param>
		private void OnCreateTabClick(object sender, EventArgs e)
		{
			using (var ctd = new CreateTabD())
			{
				ctd.ShowDialog(this);
			}
		}

		/// <summary>
		/// Saves all the sprites to the currently loaded <c>PCK+TAB</c> files
		/// if terrain/unit/bigobs or to the currently loaded <c>DAT</c> file if
		/// ScanG or LoFT.
		/// </summary>
		/// <param name="sender"><c><see cref="miSave"/></c></param>
		/// <param name="e"></param>
		private void OnSaveClick(object sender, EventArgs e)
		{
			if (TilePanel.Spriteset != null)
			{
				switch (SetType)
				{
					case SpritesetType.Pck: // save Pck+Tab terrain/unit/bigobs ->
					case SpritesetType.Bigobs:
						if (TilePanel.Spriteset.WriteSpriteset(_path))
						{
							Changed = false;
							RequestReload = true;
						}
						break;

					case SpritesetType.ScanG:
						if (TilePanel.Spriteset.WriteScanG(_path))
						{
							Changed = false;
							// TODO: FireMvReloadScanG file
						}
						break;

					case SpritesetType.LoFT:
						if (TilePanel.Spriteset.WriteLoFT(_path))
						{
							Changed = false;
						}
						break;
				}
			}
		}

		/// <summary>
		/// Saves all the sprites to potentially different <c>PCK+TAB</c> files
		/// if terrain/unit/bigobs or to a potentially different <c>DAT</c> file
		/// if ScanG or LoFT.
		/// </summary>
		/// <param name="sender"><c><see cref="miSaveAs"/></c></param>
		/// <param name="e"></param>
		private void OnSaveAsClick(object sender, EventArgs e)
		{
			if (TilePanel.Spriteset != null)
			{
				using (var sfd = new SaveFileDialog())
				{
					switch (SetType)
					{
						case SpritesetType.Pck:
						case SpritesetType.Bigobs:
							sfd.Title = "Save Pck+Tab as ...";

							sfd.Filter     = FileDialogStrings.GetFilterPck();
							sfd.DefaultExt = GlobalsXC.PckExt;
							sfd.FileName   = Path.GetFileName(_path) + GlobalsXC.PckExt;
							break;

						case SpritesetType.ScanG:
							sfd.Title = "Save ScanG as ...";
							goto case SpritesetType.non;

						case SpritesetType.LoFT:
							sfd.Title = "Save LoFTemps as ...";
							goto case SpritesetType.non;

						case SpritesetType.non: // not Type.non - is only a label
							sfd.Filter     = FileDialogStrings.GetFilterDat();
							sfd.DefaultExt = GlobalsXC.DatExt;
							sfd.FileName   = Path.GetFileName(_path);
							break;
					}

					if (!Directory.Exists(_lastBrowserDirectory))
					{
						string dir = Path.GetDirectoryName(_path);
						if (Directory.Exists(dir))
							sfd.InitialDirectory = dir;
					}
					else
						sfd.InitialDirectory = _lastBrowserDirectory;


					if (sfd.ShowDialog(this) == DialogResult.OK)
					{
						string pfe = sfd.FileName;
						string dir = Path.GetDirectoryName(pfe);
						_lastBrowserDirectory = dir;

						switch (SetType)
						{
							case SpritesetType.Pck:
							case SpritesetType.Bigobs:
							{
								string label = Path.GetFileNameWithoutExtension(pfe);
								string pf    = Path.Combine(dir, label);

								if (TilePanel.Spriteset.WriteSpriteset(pf))
								{
									_path = pf;
									Changed = false;
									RequestReload = true;
								}
								break;
							}

							case SpritesetType.ScanG:
								if (TilePanel.Spriteset.WriteScanG(pfe))
								{
									_path = pfe;
									Changed = false;
									// TODO: FireMvReloadScanG file
								}
								break;

							case SpritesetType.LoFT:
								if (TilePanel.Spriteset.WriteLoFT(pfe))
								{
									_path = pfe;
									Changed = false;
								}
								break;
						}
					}
				}
			}
		}


		/// <summary>
		/// Exports all sprites in the currently loaded spriteset to <c>PNG</c>
		/// files.
		/// </summary>
		/// <param name="sender"><c><see cref="miExportSprites"/></c></param>
		/// <param name="e"></param>
		private void OnExportSpritesClick(object sender, EventArgs e)
		{
			if (TilePanel.Spriteset != null)
			{
				int count = TilePanel.Spriteset.Count;
				if (count != 0)
				{
					using (var fbd = new FolderBrowserDialog())
					{
						string label = TilePanel.Spriteset.Label.ToUpperInvariant();

						fbd.Description = "Export spriteset to 8-bpp PNG files"
										+ Environment.NewLine + Environment.NewLine
										+ "\t" + label;

						if (!Directory.Exists(_lastSpriteDirectory))
						{
							string dir = Path.GetDirectoryName(_path);
							if (Directory.Exists(dir))
								fbd.SelectedPath = dir;
						}
						else
							fbd.SelectedPath = _lastSpriteDirectory;


						if (fbd.ShowDialog(this) == DialogResult.OK)
						{
							_lastSpriteDirectory = fbd.SelectedPath;

							string digits = String.Empty;
							int digittest = count;
							do
							{ digits += "0"; }
							while ((digittest /= 10) != 0);

							XCImage sprite;
							for (int id = 0; id != count; ++id)
							{
								sprite = TilePanel.Spriteset[id];
								string suffix = String.Format(
															"_{0:" + digits + "}",
															sprite.Id);
								string pfe = Path.Combine(_lastSpriteDirectory, label + suffix + GlobalsXC.PngExt);
								SpriteService.ExportSprite(pfe, sprite.Sprite);

								// TODO: Ask to overwrite an existing file.
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Exports all sprites in the currently loaded spriteset to a
		/// spritesheet file in <c>PNG</c> format.
		/// </summary>
		/// <param name="sender"><c><see cref="miExportSpritesheet"/></c></param>
		/// <param name="e"></param>
		private void OnExportSpritesheetClick(object sender, EventArgs e)
		{
			if (TilePanel.Spriteset != null && TilePanel.Spriteset.Count != 0)
			{
				using (var fbd = new FolderBrowserDialog()) // TODO: That should be a SaveFileDialog.
				{
					string label = TilePanel.Spriteset.Label.ToUpperInvariant();

					fbd.Description = "Export spriteset to an 8-bpp PNG spritesheet file"
									+ Environment.NewLine + Environment.NewLine
									+ "\t" + label;

					if (!Directory.Exists(_lastSpriteDirectory))
					{
						string dir = Path.GetDirectoryName(_path);
						if (Directory.Exists(dir))
							fbd.SelectedPath = dir;
					}
					else
						fbd.SelectedPath = _lastSpriteDirectory;


					if (fbd.ShowDialog(this) == DialogResult.OK)
					{
						_lastSpriteDirectory = fbd.SelectedPath;

						string pfe = Path.Combine(_lastSpriteDirectory, label + GlobalsXC.PngExt);
						// TODO: Ask to overwrite an existing file.
						SpriteService.ExportSpritesheet(
													pfe,
													TilePanel.Spriteset,
													GetCurrentPalette());
					}
				}
			}
		}

		/// <summary>
		/// Imports a spritesheet that replaces or appends to the current
		/// spriteset.
		/// </summary>
		/// <param name="sender">
		/// <list type="bullet">
		/// <item><c><see cref="miImportSheetReplace"/></c></item>
		/// <item><c><see cref="miImportSheetAdd"/></c></item>
		/// </list></param>
		/// <param name="e"></param>
		private void OnImportSpritesheetClick(object sender, EventArgs e)
		{
			if (TilePanel.Spriteset != null)
			{
				using (var ofd = new OpenFileDialog())
				{
					bool @add = (sender as MenuItem == miImportSheetAdd);

					if (@add)
						ofd.Title = "Import an 8-bpp spritesheet file (add)";
					else
						ofd.Title = "Import an 8-bpp spritesheet file (replace)";

					ofd.Filter = FileDialogStrings.GetFilter();

					if (!Directory.Exists(_lastSpriteDirectory))
					{
						string dir = Path.GetDirectoryName(_path);
						if (Directory.Exists(dir))
							ofd.InitialDirectory = dir;
					}
					else
						ofd.InitialDirectory = _lastSpriteDirectory;


					if (ofd.ShowDialog(this) == DialogResult.OK)
					{
						byte[] bindata = FileService.ReadFile(ofd.FileName);
						if (bindata != null) // else error was shown by FileService.
						{
							using (Bitmap b = SpriteLoader.LoadImageData(bindata, ofd.FileName))
							{
								if (b != null) // else error was shown by SpriteLoader.
								{
									if (   b.Width  % SpriteWidth  != 0
										|| b.Height % SpriteHeight != 0
										|| b.PixelFormat != PixelFormat.Format8bppIndexed)
									{
										ShowBitmapError(ofd.FileName, b, true);
									}
									else
									{
										int selid;
										if (!@add)
										{
											TilePanel.Spriteset.Dispose();
											selid = -1;
										}
										else
											selid = TilePanel.Spriteset.Count;

										SpriteService.ImportSpritesheet(
																	TilePanel.Spriteset.Sprites,
																	b,
																	GetCurrentPalette(),
																	SpriteWidth,
																	SpriteHeight,
																	SetType);

										SpritesetCountChanged(selid);
									}
								}
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Closes the app.
		/// </summary>
		/// <param name="sender"><c><see cref="miQuit"/></c></param>
		/// <param name="e"></param>
		private void OnQuitClick(object sender, EventArgs e)
		{
			Close();
		}

		/// <summary>
		/// Changes the current palette.
		/// </summary>
		/// <param name="sender">
		/// <list type="bullet">
		/// <item><c><see cref="_itPalettes">_itPalettes[pal]</see></c></item>
		/// </list></param>
		/// <param name="e"></param>
		/// <remarks>LoFTsets don't need their palette set since their palette
		/// is set on creation and don't change.</remarks>
		private void OnPaletteClick(object sender, EventArgs e)
		{
			var it = sender as MenuItem;
			if (!it.Checked)
			{
				if (Pal != null)
					_itPalettes[Pal].Checked = false;

				it.Checked = true;

				Pal = it.Tag as Palette;
				Pal.SetTransparent(miTransparent.Checked);

				if (TilePanel.Spriteset != null && SetType != SpritesetType.LoFT)
					TilePanel.Spriteset.Pal = Pal;

				PaletteChanged(); // TODO: That probably doesn't need to fire if a LoFTset is loaded.

				SpriteEditor._fpalette.Text = "Palette - " + Pal.Label;
			}
		}

		/// <summary>
		/// Toggles transparency of the currently loaded palette.
		/// </summary>
		/// <param name="sender"><c><see cref="miTransparent"/></c></param>
		/// <param name="e"></param>
		/// <remarks>LoFTsets don't need their palette set since their palette
		/// is set on creation and don't change.</remarks>
		private void OnTransparencyClick(object sender, EventArgs e)
		{
			Pal.SetTransparent(miTransparent.Checked = !miTransparent.Checked);

			if (TilePanel.Spriteset != null && SetType != SpritesetType.LoFT)
				TilePanel.Spriteset.Pal = Pal;

			PaletteChanged(); // TODO: That probably doesn't need to fire if a LoFTset is loaded.
		}

		/// <summary>
		/// Toggles usage of the sprite-shade value in MapView's options.
		/// </summary>
		/// <param name="sender"><c><see cref="miSpriteShade"/></c></param>
		/// <param name="e"></param>
		/// <remarks><c>(int)spriteshade</c> was converted to
		/// <c><see cref="SpriteShadeFloat"/></c> in the cTor.
		/// <c><see cref="Shader"/></c> values:
		/// <list type="bullet">
		/// <item><c><see cref="ShaderDisabled"/></c> - sprite-shade was
		/// not found by the cTor thus it cannot be enabled</item>
		/// <item><c><see cref="ShaderOn"/></c> - draw sprites/swatches w/
		/// the <c>SpriteShadeFloat</c> value</item>
		/// <item><c><see cref="ShaderOff"/></c> - user toggled
		/// sprite-shade off</item>
		/// </list></remarks>
		private void OnSpriteshadeClick(object sender, EventArgs e)
		{
			if (Shader != ShaderDisabled)
			{
				if (miSpriteShade.Checked = !miSpriteShade.Checked)
				{
					Shader = ShaderOn;
				}
				else
					Shader = ShaderOff;

				TilePanel                      .Invalidate();
				SpriteEditor.SpritePanel       .Invalidate();
				SpriteEditor._fpalette.PalPanel.Invalidate();
			}
		}


		/// <summary>
		/// Shows a richtextbox with all the bytes of the currently selected
		/// sprite laid out in a fairly readable fashion.
		/// </summary>
		/// <param name="sender"><c><see cref="miBytesTable"/></c></param>
		/// <param name="e"></param>
		private void OnByteTableClick(object sender, EventArgs e)
		{
			if (miBytesTable.Checked = !miBytesTable.Checked)
			{
				XCImage sprite;
				if (TilePanel.Spriteset != null && TilePanel.Selid != -1)
					sprite = TilePanel.Spriteset[TilePanel.Selid];
				else
					sprite = null;

				ByteTableManager.LoadTable(
										sprite,
										SetType,
										BytesClosingCallback);
			}
			else
				ByteTableManager.HideTable();
		}

		/// <summary>
		/// Callback for
		/// <c><see cref="ByteTableManager.LoadTable()">ByteTableManager.LoadTable()</see></c>.
		/// </summary>
		private void BytesClosingCallback()
		{
			miBytesTable.Checked = false;
		}

		/// <summary>
		/// Brings all windows to front when this <c>PckViewF</c> takes focus.
		/// </summary>
		/// <param name="sender"><c><see cref="miBringToFront"/></c></param>
		/// <param name="e"></param>
		private void OnBringToFrontClick(object sender, EventArgs e)
		{
			if (Frontal = (miBringToFront.Checked = !miBringToFront.Checked))
			{
				OnActivated(EventArgs.Empty);

				SpriteEditor.ShowInTaskbar =
				SpriteEditor._fpalette.ShowInTaskbar = false;
			}
			else
			{
				SpriteEditor.ShowInTaskbar =
				SpriteEditor._fpalette.ShowInTaskbar = true;
			}
		}


		/// <summary>
		/// Shows the CHM helpfile.
		/// </summary>
		/// <param name="sender"><c><see cref="miHelp"/></c></param>
		/// <param name="e"></param>
		private void OnHelpClick(object sender, EventArgs e)
		{
			string help = Path.GetDirectoryName(Application.ExecutablePath);
				   help = Path.Combine(help, "MapView.chm");
			Help.ShowHelp(this, "file://" + help, HelpNavigator.Topic, "html/pckview.htm");
		}

		/// <summary>
		/// Shows <c><see cref="About"/></c>.
		/// </summary>
		/// <param name="sender"><c><see cref="miAbout"/></c></param>
		/// <param name="e"></param>
		private void OnAboutClick(object sender, EventArgs e)
		{
			new About().ShowDialog(this);
		}


		/// <summary>
		/// is disabled.
		/// </summary>
		/// <param name="sender"><c><see cref="miCompare"/></c></param>
		/// <param name="e"></param>
		private void OnCompareClick(object sender, EventArgs e) // disabled in designer w/ Visible=FALSE
		{
/*			var original = TileTable.Spriteset; // store original spriteset

			OnOpenClick(null, EventArgs.Empty); // load a second spriteset
			var spriteset = TileTable.Spriteset;

			TileTable.Spriteset = original; // revert to original spriteset

			if (Controls.Contains(TileTable))
			{
				Controls.Remove(TileTable); // ...

				_tcTabs = new TabControl(); // create tabs
				_tcTabs.Dock = DockStyle.Fill;
				pnlView.Controls.Add(_tcTabs); // add the tabs to the stock panel

				var tabpage = new TabPage(); // create a page
				tabpage.Controls.Add(TileTable); // add the viewer to the page
				tabpage.Text = "Original";
				_tcTabs.TabPages.Add(tabpage); // add the page to the tab-control

				var viewpanel = new PckViewPanel(); // create a second viewer
				viewpanel.ContextMenu = ViewerContextMenu(); // ...
				viewpanel.Dock = DockStyle.Fill;
				viewpanel.Spriteset = spriteset; // assign the second spriteset to the second viewer

				tabpage = new TabPage(); // create a second page
				tabpage.Controls.Add(viewpanel); // add the second viewer to the second page
				tabpage.Text = "Other";
				_tcTabs.TabPages.Add(tabpage); // add the second page to the tab-control.

				// that sounds like a bad idea. Sounds like plenty of stuff
				// would have to be tested and tracked, or disabled to ensure
				// that things still work correctly when some monkey goes, "Oh
				// cool... watch this!" **sproing***
			} */
		}
		#endregion Events


		#region Methods (load)
		/// <summary>
		/// Loads <c>PCK+TAB</c> <c><see cref="Spriteset"/></c> files.
		/// </summary>
		/// <param name="pfePck">path-file-extension of a <c>PCK</c> file</param>
		/// <param name="isBigobs"><c>true</c> if Bigobs, <c>false</c> if
		/// terrain or unit Pck</param>
		/// <remarks>Pckfiles require their corresponding Tabfile. The
		/// load-routine does not handle Pckfiles that do not use a Tabfile -
		/// eg. single-image Bigobs in the <c>UFOGRAPH</c> directory.
		/// 
		/// 
		/// May be called from
		/// <c>MapView.Forms.Observers.TileView.OnPckEditorClick()</c>.</remarks>
		public void LoadSpriteset(string pfePck, bool isBigobs = false)
		{
			string label = Path.GetFileNameWithoutExtension(pfePck);
			string dir   = Path.GetDirectoryName(pfePck);

			Spriteset spriteset = SpritesetManager.CreateSpriteset(
																label,
																dir,
																Pal, // user can change the palette with the Palette menu
																false,
																Spriteset.SpriteWidth32,
																(isBigobs ? Spriteset.SpriteHeight48 : Spriteset.SpriteHeight40));
			if (spriteset != null)
			{
				if (isBigobs)
					SetType = SpritesetType.Bigobs;
				else
					SetType = SpritesetType.Pck;

				TilePanel.Spriteset = spriteset;

//				if (!_itPalettes[pal].Checked)
//				{
//					miTransparent.Checked = true;
//					OnPaletteClick(_itPalettes[pal], EventArgs.Empty);
//				}
//				else if (!miTransparent.Checked)
//				{
//					OnTransparencyClick(null, EventArgs.Empty);
//				}

				_path = Path.Combine(dir, label);
				Changed = false;
			}
		}

		/// <summary>
		/// Loads a ScanG iconset.
		/// </summary>
		/// <param name="pfeScanG">path-file-extension of SCANG.DAT</param>
		private void LoadScanG(string pfeScanG)
		{
			using (var fs = FileService.OpenFile(pfeScanG))
			if (fs != null)
			{
				if (((int)fs.Length % ScanGicon.Length_ScanG) != 0)
				{
					using (var f = new Infobox(
											"ScanG load error",
											Infobox.SplitString("The file appears to be corrupted." 
													+ " Its length is not consistent with ScanG data."),
											pfeScanG,
											InfoboxType.Error))
					{
						f.ShowDialog(this);
					}
				}
				else
				{
					TilePanel.Spriteset = new Spriteset(
													Path.GetFileNameWithoutExtension(pfeScanG),
													fs,
													SetType = SpritesetType.ScanG);

//					if (!_itPalettes[Palette.UfoBattle].Checked)
//					{
//						miTransparent.Checked = true;
//						OnPaletteClick(
//									_itPalettes[Palette.UfoBattle],
//									EventArgs.Empty);
//					}
//					else if (!miTransparent.Checked)
//					{
//						OnTransparencyClick(null, EventArgs.Empty);
//					}

					_path = pfeScanG;
					Changed = false;
				}
			}
		}

		/// <summary>
		/// Loads a LoFT iconset.
		/// </summary>
		/// <param name="pfeLoFT">path-file-extension of LOFTEMPS.DAT</param>
		private void LoadLoFT(string pfeLoFT)
		{
			using (var fs = FileService.OpenFile(pfeLoFT))
			if (fs != null)
			{
				if (((int)fs.Length % LoFTicon.Length_LoFT) != 0)
				{
					using (var f = new Infobox(
											"LoFT load error",
											Infobox.SplitString("The file appears to be corrupted."
													+ " Its length is not consistent with LoFT data."),
											pfeLoFT,
											InfoboxType.Error))
					{
						f.ShowDialog(this);
					}
				}
				else
				{
					TilePanel.Spriteset = new Spriteset(
													Path.GetFileNameWithoutExtension(pfeLoFT),
													fs,
													SetType = SpritesetType.LoFT);

//					if (!_itPalettes[Palette.TftdGeo].Checked) // 'Palette.TftdGeo' has white palid #1 (255,255,255)
//					{
//						miTransparent.Checked = false;
//						OnPaletteClick(
//									_itPalettes[Palette.TftdGeo],
//									EventArgs.Empty);
//					}
//					else if (miTransparent.Checked)
//					{
//						OnTransparencyClick(null, EventArgs.Empty);
//					}

					if (SpriteEditor._fpalette.Visible)
						SpriteEditor._fpalette.Close(); // actually Hide() + uncheck the SpriteEditor's it

					_path = pfeLoFT;
					Changed = false;
				}
			}
		}
		#endregion Methods (load)


		#region Methods
		/// <summary>
		/// Sets the current <c><see cref="Palette"/></c>. Called only from
		/// TileView to set the <c>Palette</c> externally.
		/// </summary>
		/// <param name="pal"></param>
		public void SetPalette(Palette pal)
		{
			OnPaletteClick(_itPalettes[pal], EventArgs.Empty);
		}

		/// <summary>
		/// Gets the currently selected <c><see cref="Palette"/></c> unless a
		/// LoFTset is loaded in which case return <c>Palette.Binary</c>.
		/// </summary>
		/// <returns>the current <c>Palette</c> or the <c>Palette.Binary</c> if
		/// a LoFTset is loaded</returns>
		internal Palette GetCurrentPalette()
		{
			if (SetType == SpritesetType.LoFT)
				return Palette.Binary;

			return Pal;
		}

		/// <summary>
		/// Enables or disables various menus and initializes the statusbar.
		/// </summary>
		/// <remarks>Called only when the spriteset changes in
		/// <c><see cref="PckViewPanel.Spriteset">PckViewPanel.Spriteset</see></c></remarks>
		internal void EnableInterface()
		{
			SpriteEditor.SpritePanel.Sprite = null;

			miSave              .Enabled =								// File ->
			miSaveAs            .Enabled =
			miExportSprites     .Enabled =
			miExportSpritesheet .Enabled =
			miImportSheetReplace.Enabled =
			miImportSheetAdd    .Enabled =
			miPaletteMenu       .Enabled =								// Main
			_miAdd              .Enabled =								// context ->
			_miCreate           .Enabled = TilePanel.Spriteset != null;

			EnableContext();

			SpriteEditor.OnLoad(null, EventArgs.Empty); // resize the Editor to the sprite-size

			PrintTotal();
			PrintSelected();
			PrintOver();

			PrintSpritesetLabel();

			// NOTE: Although the palette 'Pal' does not change here the
			// palette-viewer might need to change its statusbar description if
			// either palid #254 or #255 is currently selected.
			PaletteChanged(); // TODO: That probably doesn't need to fire if a LoFTset is loaded.
		}

		/// <summary>
		/// Enables or disables several context its.
		/// </summary>
		private void EnableContext()
		{
			bool enabled = TilePanel.Selid != -1;

			_miInsertBefor.Enabled = // Context ->
			_miInsertAfter.Enabled =
			_miReplace    .Enabled =
			_miDelete     .Enabled =
			_miExport     .Enabled =
			_miClear      .Enabled = enabled;

			_miMoveL.Enabled = enabled && TilePanel.Selid != 0;
			_miMoveR.Enabled = enabled && TilePanel.Selid != TilePanel.Spriteset.Count - 1;
		}

		/// <summary>
		/// Sets the currently selected sprite-id.
		/// </summary>
		/// <param name="id">the sprite-id to select</param>
		/// <param name="force"><c>true</c> to force init even if
		/// <c><see cref="PckViewPanel.Selid">PckViewPanel.Selid</see></c>
		/// doesn't change</param>
		/// <returns><c>true</c> if currently selected sprite-id changed or is
		/// forced</returns>
		/// <remarks>Can be called by TileView to set <c>PckViewPanel.Selid</c>
		/// externally.</remarks>
		public bool SetSelected(int id, bool force = false)
		{
			if (id != TilePanel.Selid || force)
			{
				TilePanel.Selid = id;

				if (id != -1 && id < TilePanel.Spriteset.Count)
				{
					SpriteEditor.SpritePanel.Sprite = TilePanel.Spriteset[id];
				}
				else
					SpriteEditor.SpritePanel.Sprite = null;

				TilePanel.ScrollToTile();

				EnableContext();

				PrintSelected();

				return true;
			}
			return false;
		}

		/// <summary>
		/// Updates the status-information for the sprite that is currently
		/// selected.
		/// </summary>
		internal void PrintSelected()
		{
			string selected;

			int id = TilePanel.Selid;
			if (id != -1)
			{
				selected = id.ToString();
				if (SetType == SpritesetType.ScanG)
				{
					if (id > 34)
						selected += " [" + (id - 35) + "]";
					else
						selected += " [0]";
				}
			}
			else
				selected = None;

			tssl_TileSelected.Text = Selected + selected;

			PrintOffsets();
		}

		/// <summary>
		/// Prints last and after offsets to the statubar.
		/// </summary>
		/// <remarks>Helper for
		/// <c><see cref="PrintSelected()">PrintSelected()</see></c>.</remarks>
		private void PrintOffsets()
		{
			if (   TilePanel.Spriteset != null
				&& TilePanel.Spriteset.TabwordLength == SpritesetManager.TAB_WORD_LENGTH_2)
			{
				int id;
				if (TilePanel.Selid != -1) id = TilePanel.Selid;
				else                       id = TilePanel.Spriteset.Count - 1;

				uint last, aftr;
				TilePanel.Spriteset.GetTabOffsets(out last, out aftr, id);

				tssl_OffsetLast.ForeColor = (last > UInt16.MaxValue) ? Color.Crimson : SystemColors.ControlText;
				tssl_OffsetAftr.ForeColor = (aftr > UInt16.MaxValue) ? Color.Crimson : SystemColors.ControlText;

				tssl_OffsetLast.Text = last.ToString();
				tssl_OffsetAftr.Text = aftr.ToString();

				tssl_Offset    .Visible =
				tssl_OffsetLast.Visible =
				tssl_OffsetAftr.Visible = true;

				tssl_SpritesetLabel.BorderSides = ToolStripStatusLabelBorderSides.Right;
			}
			else
			{
				tssl_OffsetLast.Text =
				tssl_OffsetAftr.Text = String.Empty;

				tssl_Offset    .Visible =
				tssl_OffsetLast.Visible =
				tssl_OffsetAftr.Visible = false;

				tssl_SpritesetLabel.BorderSides = ToolStripStatusLabelBorderSides.None;
			}
		}

		/// <summary>
		/// Updates the status-information for the sprite that the cursor is
		/// currently over.
		/// </summary>
		internal void PrintOver()
		{
			string text;
			if (TilePanel.Ovid != -1)
				text = TilePanel.Ovid.ToString();
			else
				text = None;

			tssl_TileOver.Text = Over + text;
		}

		/// <summary>
		/// Prints the quantity of sprites in the currently loaded spriteset to
		/// the statusbar.
		/// </summary>
		private void PrintTotal()
		{
			if (TilePanel.Spriteset != null)
				tssl_TilesTotal.Text = Total + TilePanel.Spriteset.Count;
			else
				tssl_TilesTotal.Text = String.Empty;
		}

		/// <summary>
		/// Prints the label of the currently loaded spriteset to the statubar.
		/// </summary>
		/// <remarks>Helper for
		/// <c><see cref="EnableInterface()">EnableInterface()</see></c></remarks>
		private void PrintSpritesetLabel()
		{
			string text;
			if (TilePanel.Spriteset != null)
			{
				text = TilePanel.Spriteset.Label;

				switch (SetType)
				{
					case SpritesetType.Pck:    text += " (32x40)"; break;
					case SpritesetType.Bigobs: text += " (32x48)"; break;
					case SpritesetType.ScanG:  text += " (4x4)";   break;
					case SpritesetType.LoFT:   text += " (16x16)"; break;
				}
			}
			else
				text = String.Empty;

			tssl_SpritesetLabel.Text = text;
		}


		/// <summary>
		/// Checks state of <c><see cref="Changed"/></c> and/or asks user if the
		/// spriteset ought be closed anyway.
		/// </summary>
		/// <returns><c>true</c> if state is NOT changed or if
		/// <c>DialogResult.OK</c></returns>
		private bool RequestSpritesetClose()
		{
			if (Changed)
			{
				using (var f = new Infobox(
										"Spriteset changed",
										"The spriteset has changed. Do you really want to close it?",
										null,
										InfoboxType.Warn,
										InfoboxButtons.CancelOkay))
				{
					return (f.ShowDialog(this) == DialogResult.OK);
				}
			}
			return true;
		}
		#endregion Methods
	}
}
