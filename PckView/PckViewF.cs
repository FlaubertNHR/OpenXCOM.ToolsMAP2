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

		internal static float SpriteShadeFloat;

		internal const int SPRITESHADE_ON       =  0;
		internal const int SPRITESHADE_DISABLED = -1;
		internal const int SPRITESHADE_OFF      = -2;

		internal static bool BypassActivatedEvent;
		#endregion Fields (static)


		#region Fields
		/// <summary>
		/// <c>true</c> if PckView has been invoked via TileView.
		/// </summary>
		private bool IsInvoked;

//		private TabControl _tcTabs; // for OnCompareClick()

		private ToolStripMenuItem _miEdit;
		private ToolStripMenuItem _miAdd;
		private ToolStripMenuItem _miInsertBefor;
		private ToolStripMenuItem _miInsertAfter;
		private ToolStripMenuItem _miReplace;
		private ToolStripMenuItem _miMoveL;
		private ToolStripMenuItem _miMoveR;
		private ToolStripMenuItem _miDelete;
		private ToolStripMenuItem _miExport;

		private readonly Dictionary<Palette, MenuItem> _itPalettes =
					 new Dictionary<Palette, MenuItem>();

		internal int SpriteShade = SPRITESHADE_DISABLED;
		internal readonly ImageAttributes Ia = new ImageAttributes();


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
		private Spriteset.SsType _setType;
		/// <summary>
		/// The currently loaded
		/// <c><see cref="Spriteset.SsType">Spriteset.SsType</see></c>.
		/// </summary>
		/// <remarks>Sets <c><see cref="SpriteWidth"/></c> and
		/// <c><see cref="SpriteHeight"/></c>.</remarks>
		internal Spriteset.SsType SetType
		{
			get { return _setType; }
			private set
			{
				switch (_setType = value)
				{
					case Spriteset.SsType.Pck:
						SpriteWidth  = Spriteset.SpriteWidth32;
						SpriteHeight = Spriteset.SpriteHeight40;
						break;

					case Spriteset.SsType.Bigobs:
						SpriteWidth  = Spriteset.SpriteWidth32;
						SpriteHeight = Spriteset.SpriteHeight48;
						break;

					case Spriteset.SsType.ScanG:
						SpriteWidth  = Spriteset.ScanGside;
						SpriteHeight = Spriteset.ScanGside;
						break;

					case Spriteset.SsType.LoFT:
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
		public void SetSpritesetType(Spriteset.SsType setType)
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
						case Spriteset.SsType.Pck:
						case Spriteset.SsType.Bigobs:
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
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor. Creates the PckView window.
		/// </summary>
		/// <param name="isInvoked">true if invoked via TileView</param>
		/// <param name="spriteshade">if 'isInvoked' is true you can pass in a
		/// SpriteShade value from MapView</param>
		public PckViewF(bool isInvoked = false, int spriteshade = -1)
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

			PopulatePaletteMenu(); // WARNING: Palettes created here <-

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


			bool @set = false;
			if (IsInvoked)
			{
				@set = (spriteshade > 0);
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
				miSpriteShade.Checked = true;

				SpriteShade = Math.Min(spriteshade, 100);
				SpriteShadeFloat = (float)SpriteShade * 0.03F;

				Ia.SetGamma(SpriteShadeFloat, ColorAdjustType.Bitmap);
			}


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

#if !__MonoCS__
			if (!isInvoked)
				Application.AddMessageFilter(this);
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
		// miImportSpritesheet	F6
		// miQuit				CtrlQ
		// miCompare
		// miTransparent		F7
		// miSpriteShade		F8
		// palette items		Ctrl1..Ctrl8
		// miBytes				F9
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

		/// <summary>
		/// Builds the RMB contextmenu.
		/// </summary>
		/// <returns></returns>
		private ContextMenuStrip CreateContext()
		{
			_miEdit        = new ToolStripMenuItem("Edit",              null, OnSpriteEditorClick);			// OnKeyDown Enter
			_miAdd         = new ToolStripMenuItem("Add ...",           null, OnAddSpritesClick);			// OnKeyDown d
			_miInsertBefor = new ToolStripMenuItem("Insert before ...", null, OnInsertSpritesBeforeClick);	// OnKeyDown b
			_miInsertAfter = new ToolStripMenuItem("Insert after ...",  null, OnInsertSpritesAfterClick);	// OnKeyDown a
			_miReplace     = new ToolStripMenuItem("Replace ...",       null, OnReplaceSpriteClick);		// OnKeyDown r
			_miMoveL       = new ToolStripMenuItem("Move left",         null, OnMoveLeftSpriteClick);		// OnKeyDown -
			_miMoveR       = new ToolStripMenuItem("Move right",        null, OnMoveRightSpriteClick);		// OnKeyDown +
			_miDelete      = new ToolStripMenuItem("Delete",            null, OnDeleteSpriteClick);			// OnKeyDown Delete
			_miExport      = new ToolStripMenuItem("Export sprite ...", null, OnExportSpriteClick);			// OnKeyDown p

			_miEdit       .ShortcutKeyDisplayString = "Enter";
			_miAdd        .ShortcutKeyDisplayString = "d";
			_miInsertBefor.ShortcutKeyDisplayString = "b";
			_miInsertAfter.ShortcutKeyDisplayString = "a";
			_miReplace    .ShortcutKeyDisplayString = "r";
			_miMoveL      .ShortcutKeyDisplayString = "-";
			_miMoveR      .ShortcutKeyDisplayString = "+";
			_miDelete     .ShortcutKeyDisplayString = "Del";
			_miExport     .ShortcutKeyDisplayString = "p";


			var context = new ContextMenuStrip();

			context.Items.Add(_miEdit);
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

			_miAdd        .Enabled =
			_miInsertBefor.Enabled =
			_miInsertAfter.Enabled =
			_miReplace    .Enabled =
			_miMoveL      .Enabled =
			_miMoveR      .Enabled =
			_miDelete     .Enabled =
			_miExport     .Enabled = false;

			return context;
		}

		/// <summary>
		/// Adds the palettes as menuitems to the palettes menu on the main
		/// menubar.
		/// </summary>
		private void PopulatePaletteMenu()
		{
			// instantiate the palettes
			// iff not invoked by MapView - else the palettes have already been
			// instantiated and these are just pointers in which case
			// 'BypassTonescales' is irrelevant

			Palette.BypassTonescales(true);

			var pals = new List<Palette>();
			pals.Add(Palette.UfoBattle);
			pals.Add(Palette.UfoGeo);
			pals.Add(Palette.UfoGraph);
			pals.Add(Palette.UfoResearch);
			pals.Add(Palette.TftdBattle);
			pals.Add(Palette.TftdGeo);
			pals.Add(Palette.TftdGraph);
			pals.Add(Palette.TftdResearch);

			Palette.BypassTonescales(false);


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

			OnPaletteClick(_itPalettes[Palette.UfoBattle], EventArgs.Empty);
		}
		#endregion cTor


		#region Events (override)
		/// <summary>
		/// Brings all forms to top when this is activated.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnActivated(EventArgs e)
		{
			if (!BypassActivatedEvent)
			{
				BypassActivatedEvent = true;

				if (SpriteEditor._fpalette.Visible)
				{
					SpriteEditor._fpalette.TopMost = true;
					SpriteEditor._fpalette.TopMost = false;
				}

				if (SpriteEditor.Visible)
				{
					SpriteEditor.TopMost = true;
					SpriteEditor.TopMost = false;
				}

				TopMost = true;		// req'd else this form won't activate at all
				TopMost = false;	// unless user closes the other forms

				BypassActivatedEvent = false;
			}
			base.OnActivated(e);
		}

		/// <summary>
		/// Minimizes and restores this along with the SpriteEditor and
		/// PaletteViewer synchronistically.
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
		/// Focuses the viewer-panel after the app loads.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnShown(EventArgs e)
		{
			TilePanel.Select();
			base.OnShown(e);
		}

		/// <summary>
		/// Closes the app after a .NET call to close (roughly).
		/// </summary>
		/// <param name="e"></param>
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			if (!RegistryInfo.FastClose(e.CloseReason))
			{
				if (RequestSpritesetClose())
				{
					RegistryInfo.UpdateRegistry(this);

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
		/// Handles keydown events at the form level - context and navigation
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
		/// Bring back the dinosaurs. Called when the tile-panel's click-event
		/// is raised.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks>This fires after PckViewPanel.OnMouseDown() - thought you'd
		/// like to know.</remarks>
		private void OnPanelClick(object sender, EventArgs e)
		{
			EnableContext();
		}

		/// <summary>
		/// Opens the currently selected sprite in the sprite-editor. Called
		/// when the Context menu's click-event or the viewer-panel's
		/// DoubleClick event is raised or [Enter] is pressed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnSpriteEditorClick(object sender, EventArgs e)
		{
			if (!_miEdit.Checked)
			{
				_miEdit.Checked = true;

				SpriteEditor.Show();

				if (SetType != Spriteset.SsType.LoFT)
					SpriteEditor._fpalette.Show();
			}
		}

		/// <summary>
		/// Dechecks the context's Edit it.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks>This fires after the editor's FormClosing event.</remarks>
		private void OnSpriteEditorClosing(object sender, CancelEventArgs e)
		{
			_miEdit.Checked = false;
		}


		/// <summary>
		/// Displays an errorbox to the user about incorrect Bitmap dimensions
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
//									"Detected incorrect Dimensions and/or PixelFormat.",
									FileDialogStrings.GetError(SetType, spritesheet),
									pfe + Environment.NewLine + Environment.NewLine
										+ b.Width + "x" + b.Height + " " + b.PixelFormat,
									InfoboxType.Error))
			{
				f.ShowDialog(this);
			}
		}

		/// <summary>
		/// Adds a sprite or sprites to the collection. Called when the Context
		/// menu's click-event is raised.
		/// </summary>
		/// <param name="sender"></param>
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
							Bitmap b = SpriteLoader.CreateSprite(bindata);

							if (b != null)
							{
								bs.Add(b);

								if (!(valid = (b.Width  == SpriteWidth
											&& b.Height == SpriteHeight
											&& b.PixelFormat == PixelFormat.Format8bppIndexed)))
								{
									ShowBitmapError(ofd.FileNames[i], b);
								}
							}
						}
					}

					if (valid)
					{
						int id = (TilePanel.Spriteset.Count - 1);
						foreach (var b in bs)
						{
							XCImage sprite = SpriteService.CreateSanitarySprite(
																			b,
																			++id,
																			GetCurrentPalette(),
																			SpriteWidth,
																			SpriteHeight,
																			SetType);
							TilePanel.Spriteset.Sprites.Add(sprite);
						}

						SpritesetCountChanged(TilePanel.Selid);
					}

					foreach (var b in bs)
						b.Dispose();
				}
			}
		}

		/// <summary>
		/// Inserts sprites into the currently loaded spriteset before the
		/// currently selected sprite. Called when the Context menu's click-
		/// event is raised.
		/// </summary>
		/// <param name="sender"></param>
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
		/// Inserts sprites into the currently loaded spriteset after the
		/// currently selected sprite. Called when the Context menu's click-
		/// event is raised.
		/// </summary>
		/// <param name="sender"></param>
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
		/// <param name="id">the terrain-id to start inserting at</param>
		/// <param name="files">an array of filenames</param>
		/// <returns>true if all sprites are inserted successfully</returns>
		/// <remarks>Helper for <see cref="OnInsertSpritesBeforeClick"/> and
		/// <see cref="OnInsertSpritesAfterClick"/></remarks>
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
					Bitmap b = SpriteLoader.CreateSprite(bindata);

					if (b != null)
					{
						bs.Add(b);

						if (!(valid = (b.Width       == SpriteWidth
									&& b.Height      == SpriteHeight
									&& b.PixelFormat == PixelFormat.Format8bppIndexed)))
						{
							ShowBitmapError(files[i], b);
						}
					}
				}
			}

			if (valid)
			{
				int length = files.Length;
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
		/// Replaces the selected sprite in the collection with a different
		/// sprite. Called when the Context menu's click-event is raised.
		/// </summary>
		/// <param name="sender"></param>
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
						using (Bitmap b = SpriteLoader.CreateSprite(bindata))
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
		/// Moves a sprite one slot to the left.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMoveLeftSpriteClick(object sender, EventArgs e)
		{
			MoveSprite(-1);
		}

		/// <summary>
		/// Moves a sprite one slot to the right.
		/// </summary>
		/// <param name="sender"></param>
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
		/// Deletes the selected sprite from the collection. Called when the
		/// Context menu's click-event is raised.
		/// </summary>
		/// <param name="sender"></param>
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
		/// Exports the selected sprite in the collection to a Pngfile. Called
		/// when the Context menu's click-event is raised.
		/// </summary>
		/// <param name="sender"></param>
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
		/// Creates a brand sparkling new (blank) sprite-collection. Called when
		/// the File menu's click-event is raised.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks>ScanG.dat and LoFTemps.dat cannot be created.</remarks>
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

						using (var fsPck = FileService.CreateFile(pfePckT))
						if (fsPck != null)
						using (var fsTab = FileService.CreateFile(pfeTabT))
						if (fsTab != null)
							fail = false;

						if (!fail
							&& (pfePckT == pfePck || FileService.ReplaceFile(pfePck))
							&& (pfeTabT == pfeTab || FileService.ReplaceFile(pfeTab)))
						{
							if (sender == miCreateBigobs)
								SetType = Spriteset.SsType.Bigobs;
							else
								SetType = Spriteset.SsType.Pck;

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
															((SetType == Spriteset.SsType.Bigobs) ? Spriteset.SpriteHeight48
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
		/// Opens a sprite-collection of a terrain or a unit. Called when the
		/// File menu's click-event is raised.
		/// </summary>
		/// <param name="sender"></param>
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
		/// Opens a sprite-collection of bigobs. Called when the File menu's
		/// click-event is raised.
		/// </summary>
		/// <param name="sender"></param>
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
		/// Opens a sprite-collection of ScanG icons. Called when the File
		/// menu's click-event is raised.
		/// </summary>
		/// <param name="sender"></param>
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
		/// Opens a sprite-collection of LoFT icons. Called when the File menu's
		/// click-event is raised.
		/// </summary>
		/// <param name="sender"></param>
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
		/// Saves all the sprites to the currently loaded PCK+TAB files if
		/// terrain/unit/bigobs or to the currently loaded DAT file if ScanG or
		/// LoFT. Called when the File menu's click-event is raised.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnSaveClick(object sender, EventArgs e)
		{
			if (TilePanel.Spriteset != null)
			{
				switch (SetType)
				{
					case Spriteset.SsType.Pck: // save Pck+Tab terrain/unit/bigobs ->
					case Spriteset.SsType.Bigobs:
						if (TilePanel.Spriteset.WriteSpriteset(_path))
						{
							Changed = false;
							RequestReload = true;
						}
						break;

					case Spriteset.SsType.ScanG:
						if (TilePanel.Spriteset.WriteScanG(_path))
						{
							Changed = false;
							// TODO: FireMvReloadScanG file
						}
						break;

					case Spriteset.SsType.LoFT:
						if (TilePanel.Spriteset.WriteLoFT(_path))
						{
							Changed = false;
						}
						break;
				}
			}
		}

		/// <summary>
		/// Saves all the sprites to potentially different PCK+TAB files if
		/// terrain/unit/bigobs or to a potentially different DAT file if ScanG
		/// or LoFT. Called when the File menu's click-event is raised.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnSaveAsClick(object sender, EventArgs e)
		{
			if (TilePanel.Spriteset != null)
			{
				using (var sfd = new SaveFileDialog())
				{
					switch (SetType)
					{
						case Spriteset.SsType.Pck:
						case Spriteset.SsType.Bigobs:
							sfd.Title = "Save Pck+Tab as ...";

							sfd.Filter     = FileDialogStrings.GetFilterPck();
							sfd.DefaultExt = GlobalsXC.PckExt;
							sfd.FileName   = Path.GetFileName(_path) + GlobalsXC.PckExt;
							break;

						case Spriteset.SsType.ScanG:
							sfd.Title = "Save ScanG as ...";
							goto case Spriteset.SsType.non;

						case Spriteset.SsType.LoFT:
							sfd.Title = "Save LoFTemps as ...";
							goto case Spriteset.SsType.non;

						case Spriteset.SsType.non: // not Type.non - is only a label
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
							case Spriteset.SsType.Pck:
							case Spriteset.SsType.Bigobs:
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

							case Spriteset.SsType.ScanG:
								if (TilePanel.Spriteset.WriteScanG(pfe))
								{
									_path = pfe;
									Changed = false;
									// TODO: FireMvReloadScanG file
								}
								break;

							case Spriteset.SsType.LoFT:
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
		/// Exports all sprites in the currently loaded spriteset to PNG files.
		/// Called when the File menu's click-event is raised.
		/// </summary>
		/// <param name="sender"></param>
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
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks>Called when the File menu's click-event is raised.</remarks>
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
		/// Imports (and replaces) the current spriteset from an external
		/// spritesheet.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks>Called when the File menu's click-event is raised.</remarks>
		private void OnImportSpritesheetClick(object sender, EventArgs e)
		{
			if (TilePanel.Spriteset != null)
			{
				using (var ofd = new OpenFileDialog())
				{
					ofd.Title = "Import an 8-bpp spritesheet file";
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
							using (Bitmap b = SpriteLoader.CreateSprite(bindata))
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
										// TODO: user-choice to Add a spritesheet ... instead of replacing the current one.
										TilePanel.Spriteset.Dispose();

										SpriteService.ImportSpritesheet(
																	TilePanel.Spriteset.Sprites,
																	b,
																	GetCurrentPalette(),
																	SpriteWidth,
																	SpriteHeight,
																	SetType);

										SpritesetCountChanged(-1);
									}
								}
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Closes the app. Called when the File menu's click-event is raised.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnQuitClick(object sender, EventArgs e)
		{
			Close();
		}

		/// <summary>
		/// Changes the current palette. Called when the Palette menu's click-
		/// event is raised whether by mouse or keyboard.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks>LoFTsets don't need their palette set; their palette is set
		/// on creation and don't change.</remarks>
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

				if (TilePanel.Spriteset != null && SetType != Spriteset.SsType.LoFT)
					TilePanel.Spriteset.Pal = Pal;

				PaletteChanged(); // TODO: That probably doesn't need to fire if a LoFTset is loaded.

				SpriteEditor._fpalette.Text = "Palette - " + Pal.Label;
			}
		}

		/// <summary>
		/// Toggles transparency of the currently loaded palette. Called when
		/// the Palette menu's click-event is raised whether by mouse or
		/// keyboard.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks>LoFTsets don't need their palette set; their palette is set
		/// on creation and don't change.</remarks>
		private void OnTransparencyClick(object sender, EventArgs e)
		{
			Pal.SetTransparent(miTransparent.Checked = !miTransparent.Checked);

			if (TilePanel.Spriteset != null && SetType != Spriteset.SsType.LoFT)
				TilePanel.Spriteset.Pal = Pal;

			PaletteChanged(); // TODO: That probably doesn't need to fire if a LoFTset is loaded.
		}

		/// <summary>
		/// Toggles usage of the sprite-shade value of MapView's options. Called
		/// when the Palette menu's click-event is raised.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks>'SpriteShade' is no longer the sprite-shade value.
		/// 'SpriteShade' was converted to 'SpriteShadeFloat' in the cTor, hence
		/// it can and does take a new definition here:
		/// -2 user toggled sprite-shade off;
		/// -1 sprite-shade was not found by the cTor thus it cannot be enabled;
		///  0 draw sprites/swatches w/ the 'SpriteShadeFloat' val.</remarks>
		private void OnSpriteshadeClick(object sender, EventArgs e)
		{
			if (SpriteShade != SPRITESHADE_DISABLED)
			{
				if (miSpriteShade.Checked = !miSpriteShade.Checked)
				{
					SpriteShade = SPRITESHADE_ON;
				}
				else
					SpriteShade = SPRITESHADE_OFF;

				TilePanel                      .Invalidate();
				SpriteEditor.SpritePanel       .Invalidate();
				SpriteEditor._fpalette.PalPanel.Invalidate();
			}
		}


		/// <summary>
		/// Shows a richtextbox with all the bytes of the currently selected
		/// sprite laid out in a fairly readable fashion. Called when the Bytes
		/// menu's click-event is raised.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnBytesClick(object sender, EventArgs e)
		{
			if (miBytes.Checked = !miBytes.Checked)
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
		/// Callback for LoadBytesTable().
		/// </summary>
		private void BytesClosingCallback()
		{
			miBytes.Checked = false;
		}

		/// <summary>
		/// Shows the CHM helpfile. Called when the Help menu's click-event is
		/// raised.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnHelpClick(object sender, EventArgs e)
		{
			string help = Path.GetDirectoryName(Application.ExecutablePath);
				   help = Path.Combine(help, "MapView.chm");
			Help.ShowHelp(this, "file://" + help, HelpNavigator.Topic, "html/pckview.htm");
		}

		/// <summary>
		/// Shows the about-box. Called when the Help menu's click-event is
		/// raised.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnAboutClick(object sender, EventArgs e)
		{
			new About().ShowDialog(this);
		}

		/// <summary>
		/// is disabled.
		/// </summary>
		/// <param name="sender"></param>
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
					SetType = Spriteset.SsType.Bigobs;
				else
					SetType = Spriteset.SsType.Pck;

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
													SetType = Spriteset.SsType.ScanG);

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
													SetType = Spriteset.SsType.LoFT);

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
			if (SetType == Spriteset.SsType.LoFT)
				return Palette.Binary;

			return Pal;
		}

		/// <summary>
		/// Enables or disables various menus and initializes the statusbar.
		/// </summary>
		/// <remarks>Called only when the spriteset changes in
		/// <see cref="PckViewPanel.Spriteset"/></remarks>
		internal void EnableInterface()
		{
			SpriteEditor.SpritePanel.Sprite = null;

			miSave             .Enabled =									// File ->
			miSaveAs           .Enabled =
			miExportSprites    .Enabled =
			miExportSpritesheet.Enabled =
			miImportSpritesheet.Enabled =
			miPaletteMenu      .Enabled =									// Main
			_miAdd             .Enabled = (TilePanel.Spriteset != null);	// context

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
			bool enabled = (TilePanel.Selid != -1);

			_miInsertBefor.Enabled = // Context ->
			_miInsertAfter.Enabled =
			_miReplace    .Enabled =
			_miDelete     .Enabled =
			_miExport     .Enabled = enabled;

			_miMoveL.Enabled = enabled && (TilePanel.Selid != 0);
			_miMoveR.Enabled = enabled && (TilePanel.Selid != TilePanel.Spriteset.Count - 1);
		}

		/// <summary>
		/// Sets the currently selected sprite-id.
		/// </summary>
		/// <param name="id">the sprite-id to select</param>
		/// <param name="force">true to force init even if <see cref="PckViewPanel.Selid"/>
		/// doesn't change</param>
		/// <returns>true if currently selected sprite-id changed or is forced</returns>
		/// <remarks>Can be called by TileView to set <see cref="PckViewPanel.Selid"/>
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
				if (SetType == Spriteset.SsType.ScanG)
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
		/// <remarks>Helper for <see cref="PrintSelected"/></remarks>
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
		/// <remarks>Helper for <see cref="EnableInterface"/></remarks>
		private void PrintSpritesetLabel()
		{
			string text;
			if (TilePanel.Spriteset != null)
			{
				text = TilePanel.Spriteset.Label;

				switch (SetType)
				{
					case Spriteset.SsType.Pck:    text += " (32x40)"; break;
					case Spriteset.SsType.Bigobs: text += " (32x48)"; break;
					case Spriteset.SsType.ScanG:  text += " (4x4)";   break;
					case Spriteset.SsType.LoFT:   text += " (16x16)"; break;
				}
			}
			else
				text = String.Empty;

			tssl_SpritesetLabel.Text = text;
		}


		/// <summary>
		/// Checks state of the 'Changed' flag and/or asks user if the spriteset
		/// ought be closed anyway.
		/// </summary>
		/// <returns>true if state is NOT changed or 'DialogResult.OK'</returns>
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
