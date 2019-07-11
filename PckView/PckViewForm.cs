using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using DSShared.Windows;

using PckView.Forms.SpriteBytes;

using XCom;
using XCom.Interfaces;


namespace PckView
{
	public sealed partial class PckViewForm
		:
			Form
	{
		#region Events (static)
		internal static event PaletteChangedEvent PaletteChanged;
		#endregion Events (static)


		#region Fields (static)
		private static readonly Palette DefaultPalette = Palette.UfoBattle;

		private const string Total    = "Total ";
		private const string Selected = "Selected ";
		private const string Over     = "Over ";
		private const string None     = "n/a";

		private const string PngExt = ".PNG";

		internal static bool Quit;
		#endregion Fields (static)


		#region Fields
		/// <summary>
		/// True if PckView has been invoked via TileView.
		/// </summary>
		private bool IsInvoked;

		private readonly PckViewPanel TilePanel;
		private readonly EditorForm Editor;

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

		private readonly Dictionary<Palette, MenuItem> _paletteItems =
					 new Dictionary<Palette, MenuItem>();

		private string _pfePck;
		private string _pfeTab;
		private string _pfePck0;
		private string _pfeTab0;
		#endregion Fields


		#region Properties (static)
		internal static PckViewForm that
		{ get; private set; }

		internal static Palette Pal
		{ get; set; }
		#endregion Properties (static)


		#region Properties
		private string Dir
		{ get; set; }

		private string Fil
		{ get; set; }

		private string Title
		{ get; set; }

		/// <summary>
		/// For reloading the Map when PckView is invoked via TileView.
		/// @note Reload MapView's Map even if the PCK+TAB is saved as a
		/// different file; the new terrain-label might also be in the Map's
		/// terrainset.
		/// </summary>
		public bool FireMvReload
		{ get; private set; }


		/// <summary>
		/// True if a Bigobs PCK+TAB set is currently loaded.
		/// @note Bigobs are 32x48 w/ 2-byte Tabword.
		/// </summary>
		private bool IsBigobs
		{ get; set; }

		/// <summary>
		/// True if a ScanG iconset is currently loaded.
		/// @note ScanGs are 4x4 w/ 0-byte Tabword.
		/// </summary>
		internal bool IsScanG
		{ get; private set; }

		private bool _changed;
		private bool Changed
		{
			get { return _changed; }
			set
			{
				if (_changed = value)
					Text = "PckView - " + Title + "*";
				else
					Text = "PckView - " + Title;
			}
		}
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor. Creates the PckView window.
		/// </summary>
		/// <param name="isInvoked"></param>
		public PckViewForm(bool isInvoked = false)
		{
			IsInvoked = isInvoked;

			InitializeComponent();

			// WORKAROUND: See note in 'XCMainWindow' cTor.
			MaximumSize = new Size(0,0); // fu.net

			if (!IsInvoked)
				RegistryInfo.InitializeRegistry(Path.GetDirectoryName(Application.ExecutablePath));

			RegistryInfo.RegisterProperties(this);
//			regInfo.AddProperty("SelectedPalette");

			that = this;

			TilePanel = new PckViewPanel(this);

			TilePanel.ContextMenuStrip = ViewerContextMenu();
			TilePanel.Click       += OnSpriteClick;
			TilePanel.DoubleClick += OnSpriteEditorClick;

			Controls.Add(TilePanel);
			TilePanel.BringToFront();

			PrintSelectedId();
			PrintOverId();

			tssl_SpritesetLabel.Text = None;

			PopulatePaletteMenu();

			Pal = DefaultPalette;
			Pal.SetTransparent(true);

			_paletteItems[Pal].Checked = true;

			Editor = new EditorForm();
			Editor.FormClosing += OnEditorFormClosing;


			miCreate.MenuItems.Add(miCreateTerrain);	// NOTE: These items are added to the Filemenu first
			miCreate.MenuItems.Add(miCreateBigobs);		// and get transfered to the Create submenu here.
			miCreate.MenuItems.Add(miCreateUnitUfo);
			miCreate.MenuItems.Add(miCreateUnitTftd);

			tssl_TilesTotal.Text = String.Format(
											CultureInfo.InvariantCulture,
											Total + None);

			tssl_OffsetLast.Text =
			tssl_OffsetAftr.Text = String.Empty;
		}


		// PckView shortcut table:
		// miCreateTerrain		CtrlR
		// miCreateBigobs		CtrlB
		// miCreateUnitUfo		CtrlU
		// miCreateUnitTftd		CtrlT
		// miOpen				CtrlO
		// miOpenBigobs			CtrlG
		// miOpenScanG			CtrlD
		// miSave				CtrlS
		// miSaveAs				CtrlE
		// miExportSprites		CtrlP
		// miExportSpritesheet	F5
		// miImportSpritesheet	F6
		// miQuit				CtrlQ
		// miCompare
		// miHq2x
		// miTransparent		F7
		// miBytes				F8
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
		private ContextMenuStrip ViewerContextMenu()
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


			var contextmenu = new ContextMenuStrip();

			contextmenu.Items.Add(_miEdit);
			contextmenu.Items.Add(new ToolStripSeparator());
			contextmenu.Items.Add(_miAdd);
			contextmenu.Items.Add(_miInsertBefor);
			contextmenu.Items.Add(_miInsertAfter);
			contextmenu.Items.Add(new ToolStripSeparator());
			contextmenu.Items.Add(_miReplace);
			contextmenu.Items.Add(_miMoveL);
			contextmenu.Items.Add(_miMoveR);
			contextmenu.Items.Add(new ToolStripSeparator());
			contextmenu.Items.Add(_miDelete);
			contextmenu.Items.Add(new ToolStripSeparator());
			contextmenu.Items.Add(_miExport);

			_miEdit       .Enabled =
			_miAdd        .Enabled =
			_miInsertBefor.Enabled =
			_miInsertAfter.Enabled =
			_miReplace    .Enabled =
			_miMoveL      .Enabled =
			_miMoveR      .Enabled =
			_miDelete     .Enabled =
			_miExport     .Enabled = false;

			return contextmenu;
		}

		/// <summary>
		/// Adds the palettes as menuitems to the palettes menu on the main
		/// menubar.
		/// </summary>
		private void PopulatePaletteMenu()
		{
			var pals = new List<Palette>();

			pals.Add(Palette.UfoBattle);
			pals.Add(Palette.UfoGeo);
			pals.Add(Palette.UfoGraph);
			pals.Add(Palette.UfoResearch);
			pals.Add(Palette.TftdBattle);
			pals.Add(Palette.TftdGeo);
			pals.Add(Palette.TftdGraph);
			pals.Add(Palette.TftdResearch);

			for (int i = 0; i != pals.Count; ++i)
			{
				var pal = pals[i];

				var itPal = new MenuItem(pal.Label);
				itPal.Tag = pal;
				miPaletteMenu.MenuItems.Add(itPal);

				itPal.Click += OnPaletteClick;
				_paletteItems[pal] = itPal;

				switch (i)
				{
					case 0: itPal.Shortcut = Shortcut.Ctrl1; break;
					case 1: itPal.Shortcut = Shortcut.Ctrl2; break;
					case 2: itPal.Shortcut = Shortcut.Ctrl3; break;
					case 3: itPal.Shortcut = Shortcut.Ctrl4; break;
					case 4: itPal.Shortcut = Shortcut.Ctrl5; break;
					case 5: itPal.Shortcut = Shortcut.Ctrl6; break;
					case 6: itPal.Shortcut = Shortcut.Ctrl7; break;
					case 7: itPal.Shortcut = Shortcut.Ctrl8; break;
				}
			}
		}
		#endregion cTor


		#region Events (override)
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
			if (CheckQuit())
			{
				RegistryInfo.UpdateRegistry(this);

				Quit = true;

				Editor.ClosePalette();	// these are needed when PckView
				Editor.Close();			// is invoked via TileView.

				SpriteBytesManager.HideBytesTable();

				if (!IsInvoked)
					RegistryInfo.FinalizeRegistry();
			}
			else
				e.Cancel = true;

			base.OnFormClosing(e);
		}

		/// <summary>
		/// Deletes the currently selected sprite w/ a keydown event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			//LogFile.WriteLine("PckViewForm.OnKeyDown() " + e.KeyCode);

			// Context shortcuts ->

			switch (e.KeyCode)
			{
				case Keys.Enter:												// edit
					if (_miEdit.Enabled)
					{
						e.SuppressKeyPress = true;
						OnSpriteEditorClick(null, EventArgs.Empty);
					}
					break;

				case Keys.D:													// add
					if (_miAdd.Enabled)
					{
						e.SuppressKeyPress = true;
						OnAddSpritesClick(null, EventArgs.Empty);
					}
					break;

				case Keys.B:													// insert before
					if (_miInsertBefor.Enabled)
					{
						e.SuppressKeyPress = true;
						OnInsertSpritesBeforeClick(null, EventArgs.Empty);
					}
					break;

				case Keys.A:													// insert after
					if (_miInsertAfter.Enabled)
					{
						e.SuppressKeyPress = true;
						OnInsertSpritesAfterClick(null, EventArgs.Empty);
					}
					break;

				case Keys.R:													// replace
					if (_miReplace.Enabled)
					{
						e.SuppressKeyPress = true;
						OnReplaceSpriteClick(null, EventArgs.Empty);
					}
					break;

				case Keys.OemMinus: // drugs ...
				case Keys.Subtract:												// move left
					if (_miMoveL.Enabled)
					{
						e.SuppressKeyPress = true;
						OnMoveLeftSpriteClick(null, EventArgs.Empty);
					}
					break;

				case Keys.Oemplus: // drugs ...
				case Keys.Add:													// move right
					if (_miMoveR.Enabled)
					{
						e.SuppressKeyPress = true;
						OnMoveRightSpriteClick(null, EventArgs.Empty);
					}
					break;

				case Keys.Delete:												// delete
					if (_miDelete.Enabled)
					{
						e.SuppressKeyPress = true;
						OnDeleteSpriteClick(null, EventArgs.Empty);
					}
					break;

				case Keys.P:													// export
					if (_miExport.Enabled)
					{
						e.SuppressKeyPress = true;
						OnExportSpriteClick(null, EventArgs.Empty);
					}
					break;


				// Navigation shortcuts ->

				case Keys.Left:
					if (TilePanel.Spriteset != null && TilePanel.idSel > 0)
					{
						TilePanel.SelectAdjacentHori(-1);
						PrintSelectedId();
					}
					break;

				case Keys.Right:
					if (TilePanel.Spriteset != null && TilePanel.idSel != TilePanel.Spriteset.Count - 1)
					{
						TilePanel.SelectAdjacentHori(+1);
						PrintSelectedId();
					}
					break;

				case Keys.Up:
					if (TilePanel.Spriteset != null)
					{
						TilePanel.SelectAdjacentVert(-1);
						PrintSelectedId();
					}
					break;

				case Keys.Down:
					if (TilePanel.Spriteset != null)
					{
						TilePanel.SelectAdjacentVert(+1);
						PrintSelectedId();
					}
					break;

				case Keys.Escape:
					if (TilePanel.Spriteset != null)
					{
						TilePanel.idSel = -1;
						EditorPanel.that.Sprite = null;
						PrintSelectedId();
						TilePanel.Invalidate();
					}
					break;
			}

			base.OnKeyDown(e);
		}
		#endregion Events (override)


		#region Events
		/// <summary>
		/// Enables (or disables) various menuitems.
		/// </summary>
		/// <param name="valid">true if the spriteset is valid</param>
		internal void SpritesetChanged(bool valid)
		{
			// under File menu
			miSave             .Enabled =
			miSaveAs           .Enabled =
			miExportSprites    .Enabled =
			miExportSpritesheet.Enabled =
			miImportSpritesheet.Enabled =

			// on Main menu
			miPaletteMenu      .Enabled =
			miTransparentMenu  .Enabled =
			miBytesMenu        .Enabled =

			// on Context menu
			_miAdd             .Enabled = valid;

			Editor.OnLoad(null, EventArgs.Empty); // resize the Editor to the spriteset's sprite-size
			OnSpriteClick(null, EventArgs.Empty); // enable/disable items on the contextmenu

			PrintSpritesetLabel(valid);
			PrintTotal(valid);
		}

		/// <summary>
		/// Bring back the dinosaurs. Enables (or disables) several contextmenu
		/// items. Called when the tile-panel's Click event is raised.
		/// @note This fires after PckViewPanel.OnMouseDown(). Thought you'd
		/// like to know.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnSpriteClick(object sender, EventArgs e)
		{
			bool enabled = (TilePanel.idSel != -1);

			// on Context menu
			_miEdit       .Enabled =
			_miInsertBefor.Enabled =
			_miInsertAfter.Enabled =
			_miReplace    .Enabled =
			_miDelete     .Enabled =
			_miExport     .Enabled = enabled;

			_miMoveL.Enabled = enabled && (TilePanel.idSel != 0);
			_miMoveR.Enabled = enabled && (TilePanel.idSel != TilePanel.Spriteset.Count - 1);
		}

		/// <summary>
		/// Opens the currently selected sprite in the sprite-editor.
		/// Called when the context's Click event or the viewer-panel's
		/// DoubleClick event is raised or [Enter] is pressed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnSpriteEditorClick(object sender, EventArgs e)
		{
			if (TilePanel.Spriteset != null && TilePanel.idSel != -1)
			{
				EditorPanel.that.Sprite = TilePanel.Spriteset[TilePanel.idSel];

				if (!_miEdit.Checked)
				{
					_miEdit.Checked = true;
					Editor.Show();
				}
				else
					Editor.BringToFront();
			}
		}

		/// <summary>
		/// Cancels closing the editor and hides it instead.
		/// @note This fires after the editor's FormClosing event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnEditorFormClosing(object sender, CancelEventArgs e)
		{
			_miEdit.Checked = false;
		}


		/// <summary>
		/// Displays an errorbox to the user about incorrect Bitmap dimensions
		/// and/or pixel-format.
		/// </summary>
		/// <param name="hint">true to suggest proper dimensions/format</param>
		private void ShowBitmapError(bool hint = true)
		{
			string error = "Detected incorrect Dimensions and/or PixelFormat.";

			if (hint)
			{
				error += Environment.NewLine + Environment.NewLine;
				if (IsScanG)
					error += String.Format(
										CultureInfo.CurrentCulture,
										"Image needs to be 4x4 8-bpp");
				else if (IsBigobs)
					error += String.Format(
										CultureInfo.CurrentCulture,
										"Image needs to be 32x48 8-bpp");
				else
					error += String.Format(
										CultureInfo.CurrentCulture,
										"Image needs to be 32x40 8-bpp");
			}

			MessageBox.Show(
						error,
						" Error",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error,
						MessageBoxDefaultButton.Button1,
						0);
		}

		/// <summary>
		/// Adds a sprite or sprites to the collection.
		/// Called when the contextmenu's Click event is raised.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnAddSpritesClick(object sender, EventArgs e)
		{
			using (var ofd = new OpenFileDialog())
			{
				ofd.Multiselect = true;

				if (IsScanG)
					ofd.Title = "Add 4x4 8-bpp Image file(s)";
				else if (IsBigobs)
					ofd.Title = "Add 32x48 8-bpp Image file(s)";
				else
					ofd.Title = "Add 32x40 8-bpp Image file(s)";

				ofd.Filter = "Image files (*.PNG *.GIF *.BMP)|*.PNG;*.GIF;*.BMP|"
						   + "PNG files (*.PNG)|*.PNG|GIF files (*.GIF)|*.GIF|BMP files (*.BMP)|*.BMP|"
						   + "All files (*.*)|*.*";

				if (ofd.ShowDialog() == DialogResult.OK)
				{
					var bs = new Bitmap[ofd.FileNames.Length]; // first run a check against all sprites and if any are borked set error.
					for (int i = 0; i != ofd.FileNames.Length; ++i)
					{
//						var b = new Bitmap(ofd.FileNames[i]);	// <- .net.bork. Creates a 32-bpp Argb image if source is
																// 8-bpp PNG w/tranparency; GIF,BMP however retains 8-bpp format.

						byte[] bindata = File.ReadAllBytes(ofd.FileNames[i]);
						Bitmap b = BitmapHandler.LoadBitmap(bindata);

						if (   b.Width  == XCImage.SpriteWidth
							&& b.Height == XCImage.SpriteHeight
							&& b.PixelFormat == PixelFormat.Format8bppIndexed)
						{
							bs[i] = b;
						}
						else
						{
							ShowBitmapError();
							return;
						}
					}

					int id = (TilePanel.Spriteset.Count - 1);
					foreach (var b in bs)
					{
						var sprite = BitmapService.CreateSprite(
															b,
															++id,
															Pal,
															XCImage.SpriteWidth,
															XCImage.SpriteHeight,
															IsScanG);
						TilePanel.Spriteset.Sprites.Add(sprite);
					}

					InsertSpritesFinish();
				}
			}
		}

		/// <summary>
		/// Inserts sprites into the currently loaded spriteset before the
		/// currently selected sprite.
		/// Called when the contextmenu's Click event is raised.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnInsertSpritesBeforeClick(object sender, EventArgs e)
		{
			using (var ofd = new OpenFileDialog())
			{
				ofd.Multiselect = true;

				if (IsScanG)
					ofd.Title = "Add 4x4 8-bpp Image file(s)";
				else if (IsBigobs)
					ofd.Title = "Add 32x48 8-bpp Image file(s)";
				else
					ofd.Title = "Add 32x40 8-bpp Image file(s)";

				ofd.Filter = "Image files (*.PNG *.GIF *.BMP)|*.PNG;*.GIF;*.BMP|"
						   + "PNG files (*.PNG)|*.PNG|GIF files (*.GIF)|*.GIF|BMP files (*.BMP)|*.BMP|"
						   + "All files (*.*)|*.*";

				if (ofd.ShowDialog() == DialogResult.OK)
				{
					if (InsertSprites(TilePanel.idSel, ofd.FileNames))
					{
						TilePanel.idSel += ofd.FileNames.Length;
						EditorPanel.that.Sprite = TilePanel.Spriteset[TilePanel.idSel];

						InsertSpritesFinish();
					}
					else
						ShowBitmapError();
				}
			}
		}

		/// <summary>
		/// Inserts sprites into the currently loaded spriteset after the
		/// currently selected sprite.
		/// Called when the contextmenu's Click event is raised.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnInsertSpritesAfterClick(object sender, EventArgs e)
		{
			using (var ofd = new OpenFileDialog())
			{
				ofd.Multiselect = true;

				if (IsScanG)
					ofd.Title = "Add 4x4 8-bpp Image file(s)";
				else if (IsBigobs)
					ofd.Title = "Add 32x48 8-bpp Image file(s)";
				else
					ofd.Title = "Add 32x40 8-bpp Image file(s)";

				ofd.Filter = "Image files (*.PNG *.GIF *.BMP)|*.PNG;*.GIF;*.BMP|"
						   + "PNG files (*.PNG)|*.PNG|GIF files (*.GIF)|*.GIF|BMP files (*.BMP)|*.BMP|"
						   + "All files (*.*)|*.*";

				if (ofd.ShowDialog() == DialogResult.OK)
				{
					if (InsertSprites(TilePanel.idSel + 1, ofd.FileNames))
						InsertSpritesFinish();
					else
						ShowBitmapError();
				}
			}
		}

		/// <summary>
		/// Inserts sprites into the currently loaded spriteset starting at a
		/// given Id.
		/// Helper for OnInsertSpriteBeforeClick() and OnInsertSpriteAfterClick().
		/// </summary>
		/// <param name="id">the terrain-id to start inserting at</param>
		/// <param name="files">an array of filenames</param>
		/// <returns>true if all sprites are inserted successfully</returns>
		private bool InsertSprites(int id, string[] files)
		{
			var bs = new Bitmap[files.Length]; // first run a check against all sprites and if any are borked exit w/ false.
			for (int i = 0; i != files.Length; ++i)
			{
				byte[] bindata = File.ReadAllBytes(files[i]);
				Bitmap b = BitmapHandler.LoadBitmap(bindata);

				if (   b.Width  == XCImage.SpriteWidth
					&& b.Height == XCImage.SpriteHeight
					&& b.PixelFormat == PixelFormat.Format8bppIndexed)
				{
					bs[i] = b;
				}
				else
					return false;
			}

			int length = files.Length;
			for (int i = id; i != TilePanel.Spriteset.Count; ++i)
				TilePanel.Spriteset[i].Id = i + length;

			foreach (var b in bs)
			{
				var sprite = BitmapService.CreateSprite(
													b,
													id,
													Pal,
													XCImage.SpriteWidth,
													XCImage.SpriteHeight,
													IsScanG);
				TilePanel.Spriteset.Sprites.Insert(id++, sprite);
			}
			return true;
		}

		/// <summary>
		/// Finishes the insert-sprite operation.
		/// </summary>
		private void InsertSpritesFinish()
		{
			OnSpriteClick(null, EventArgs.Empty);

			PrintTotal(true);

			TilePanel.ForceResize();
			TilePanel.Invalidate();

			Changed = true;
		}

		/// <summary>
		/// Replaces the selected sprite in the collection with a different
		/// sprite.
		/// Called when the contextmenu's Click event is raised.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnReplaceSpriteClick(object sender, EventArgs e)
		{
			using (var ofd = new OpenFileDialog())
			{
				if (IsScanG)
					ofd.Title = "Add 4x4 8-bpp Image file(s)";
				else if (IsBigobs)
					ofd.Title = "Add 32x48 8-bpp Image file";
				else
					ofd.Title = "Add 32x40 8-bpp Image file";

				ofd.Filter = "Image files (*.PNG *.GIF *.BMP)|*.PNG;*.GIF;*.BMP|"
						   + "PNG files (*.PNG)|*.PNG|GIF files (*.GIF)|*.GIF|BMP files (*.BMP)|*.BMP|"
						   + "All files (*.*)|*.*";

				if (ofd.ShowDialog() == DialogResult.OK)
				{
					byte[] bindata = File.ReadAllBytes(ofd.FileName);
					Bitmap b = BitmapHandler.LoadBitmap(bindata);

					if (   b.Width  == XCImage.SpriteWidth
						&& b.Height == XCImage.SpriteHeight
						&& b.PixelFormat == PixelFormat.Format8bppIndexed)
					{
						var sprite = BitmapService.CreateSprite(
															b,
															TilePanel.idSel,
															Pal,
															XCImage.SpriteWidth,
															XCImage.SpriteHeight,
															IsScanG);
						TilePanel.Spriteset[TilePanel.idSel] =
						EditorPanel.that.Sprite = sprite;

						TilePanel.Refresh();
						Changed = true;
					}
					else
						ShowBitmapError();
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
			var sprite = TilePanel.Spriteset[TilePanel.idSel];

			TilePanel.Spriteset[TilePanel.idSel]       = TilePanel.Spriteset[TilePanel.idSel + dir];
			TilePanel.Spriteset[TilePanel.idSel + dir] = sprite;

			TilePanel.Spriteset[TilePanel.idSel].Id = TilePanel.idSel;
			TilePanel.idSel += dir;
			TilePanel.Spriteset[TilePanel.idSel].Id = TilePanel.idSel;

			EditorPanel.that.Sprite = TilePanel.Spriteset[TilePanel.idSel];

			PrintSelectedId();

			OnSpriteClick(null, EventArgs.Empty);

			TilePanel.Refresh();
			Changed = true;
		}

		/// <summary>
		/// Deletes the selected sprite from the collection.
		/// Called when the contextmenu's Click event is raised.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnDeleteSpriteClick(object sender, EventArgs e)
		{
			TilePanel.Spriteset.Sprites.RemoveAt(TilePanel.idSel);

			for (int i = TilePanel.idSel; i != TilePanel.Spriteset.Count; ++i)
				TilePanel.Spriteset[i].Id = i;

			if (TilePanel.idSel == TilePanel.Spriteset.Count)
			{
				EditorPanel.that.Sprite = null;
				TilePanel.idSel = -1;
			}
			InsertSpritesFinish();
		}

		/// <summary>
		/// Exports the selected sprite in the collection to a PNG file.
		/// Called when the contextmenu's Click event is raised.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnExportSpriteClick(object sender, EventArgs e)
		{
			string digits = String.Empty;

			int count = TilePanel.Spriteset.Count;
			do
			{ digits += "0"; }
			while ((count /= 10) != 0);

			var sprite = TilePanel.Spriteset[TilePanel.idSel];
			string suffix = String.Format(
										CultureInfo.InvariantCulture,
										"_{0:" + digits + "}",
										TilePanel.idSel);

			using (var sfd = new SaveFileDialog())
			{
				sfd.Title      = "Export sprite to 8-bpp PNG file";
				sfd.Filter     = "PNG files (*.PNG)|*.PNG|All files (*.*)|*.*";
				sfd.DefaultExt = "PNG";
				sfd.FileName   = TilePanel.Spriteset.Label + suffix;

				if (sfd.ShowDialog() == DialogResult.OK)
					BitmapService.ExportSprite(sfd.FileName, sprite.Sprite);
			}
		}


		/// <summary>
		/// Creates a brand sparkling new (blank) PCK sprite collection.
		/// Called when the mainmenu's file-menu Click event is raised.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnCreateClick(object sender, EventArgs e)
		{
			if (CheckQuit())
			{
				using (var sfd = new SaveFileDialog())
				{
					IsScanG = false;

					int tabwordLength;
					if (IsBigobs = (sender == miCreateBigobs)) // Bigobs support for XCImage/PckImage
					{
						sfd.Title = "Create a PCK (bigobs) file";
						XCImage.SpriteHeight = 48;
						tabwordLength = ResourceInfo.TAB_WORD_LENGTH_2;
					}
					else
					{
						XCImage.SpriteHeight = 40;

						if (sender == miCreateUnitTftd) // Tftd Unit support for XCImage/PckImage
						{
							sfd.Title = "Create a PCK (tftd unit) file";
							tabwordLength = ResourceInfo.TAB_WORD_LENGTH_4;
						}
						else
						{
							tabwordLength = ResourceInfo.TAB_WORD_LENGTH_2;

							if (sender == miCreateUnitUfo) // Ufo Unit support for XCImage/PckImage
								sfd.Title = "Create a PCK (ufo unit) file";
							else
								sfd.Title = "Create a PCK (terrain) file";
						}
					}

					sfd.Filter     = "PCK files (*.PCK)|*.PCK|All files (*.*)|*.*";
					sfd.DefaultExt = "PCK";

					if (sfd.ShowDialog() == DialogResult.OK)
					{
						string pfePck = sfd.FileName;
						string pfeTab = pfePck.Substring(0, pfePck.Length - 4) + GlobalsXC.TabExt;

						using (var bwPck = new BinaryWriter(File.Create(pfePck))) // blank files are ok.
						using (var bwTab = new BinaryWriter(File.Create(pfeTab)))
						{}


						Dir = Path.GetDirectoryName(pfePck);
						Fil = Path.GetFileNameWithoutExtension(pfePck);

						var pal = DefaultPalette;
						var spriteset = new SpriteCollection(
														Fil,
														pal,
														tabwordLength);

						OnPaletteClick(_paletteItems[pal], EventArgs.Empty);

						TilePanel.Spriteset = spriteset;
						OnSpriteClick(null, EventArgs.Empty);

						Title = pfePck;
						Changed = false;
					}
				}
			}
		}

		/// <summary>
		/// Opens a PCK sprite collection.
		/// Called when the mainmenu's file-menu Click event is raised.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnOpenClick(object sender, EventArgs e)
		{
			if (CheckQuit())
			{
				using (var ofd = new OpenFileDialog())
				{
					ofd.Title  = "Select a PCK (terrain/unit) file";
					ofd.Filter = "PCK files (*.PCK)|*.PCK|All files (*.*)|*.*";

					if (ofd.ShowDialog() == DialogResult.OK)
					{
						IsBigobs =
						IsScanG  = false;
						LoadSpriteset(ofd.FileName);
					}
				}
			}
		}

		/// <summary>
		/// Opens a PCK sprite collection.
		/// Called when the mainmenu's file-menu Click event is raised.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnOpenBigobsClick(object sender, EventArgs e)
		{
			if (CheckQuit())
			{
				using (var ofd = new OpenFileDialog())
				{
					ofd.Title  = "Select a PCK (bigobs) file";
					ofd.Filter = "PCK files (*.PCK)|*.PCK|All files (*.*)|*.*";

					if (ofd.ShowDialog() == DialogResult.OK)
					{
						IsBigobs = true;
						IsScanG  = false;
						LoadSpriteset(ofd.FileName);
					}
				}
			}
		}

		private void OnOpenScanGClick(object sender, EventArgs e)
		{
			if (CheckQuit())
			{
				using (var ofd = new OpenFileDialog())
				{
					ofd.Title  = "Select a ScanG file";
					ofd.Filter = "DAT files (*.DAT)|*.DAT|All files (*.*)|*.*";

					if (ofd.ShowDialog() == DialogResult.OK)
					{
						IsBigobs = false;
						IsScanG  = true;
						LoadScanG(ofd.FileName);
					}
				}
			}
		}

		/// <summary>
		/// Saves all the sprites to the currently loaded PCK+TAB files.
		/// Called when the mainmenu's file-menu Click event is raised.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnSaveClick(object sender, EventArgs e)
		{
			if (TilePanel.Spriteset != null)
			{
				if (IsScanG)
				{
					if (!SpriteCollection.SaveScanG(
												Dir,
												Fil,
												TilePanel.Spriteset))
					{
						string error = String.Format(
												CultureInfo.CurrentCulture,
												"An I/O error occurred.");
						MessageBox.Show(
									error,
									" Error",
									MessageBoxButtons.OK,
									MessageBoxIcon.Error,
									MessageBoxDefaultButton.Button1,
									0);
					}
					else
					{
						Changed = false;
						// TODO: FireMvReloadScanG file
					}
				}
				else // save Pck+Tab terrain/unit/bigobs ->
				{
					BackupSpritesetFiles();

					if (SpriteCollection.SaveSpriteset(
													Dir,
													Fil,
													TilePanel.Spriteset,
													TilePanel.Spriteset.TabwordLength))
					{
						Changed = false;
						FireMvReload = true;
					}
					else
					{
						ShowSaveError();
						RevertFiles();
					}
				}
			}
		}

		/// <summary>
		/// Saves all the sprites to potentially different PCK+TAB files.
		/// Called when the mainmenu's file-menu Click event is raised.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnSaveAsClick(object sender, EventArgs e)
		{
			if (TilePanel.Spriteset != null)
			{
				using (var sfd = new SaveFileDialog())
				{
					sfd.Title = "Save as";
					sfd.FileName = Fil;

					if (IsScanG)
						sfd.Filter = "DAT files (*.DAT)|*.DAT|All files (*.*)|*.*";
					else
						sfd.Filter = "PCK files (*.PCK)|*.PCK|All files (*.*)|*.*";

					if (sfd.ShowDialog() == DialogResult.OK)
					{
						string dir = Path.GetDirectoryName(sfd.FileName);
						string fil = Path.GetFileNameWithoutExtension(sfd.FileName);

						if (IsScanG)
						{
							if (!SpriteCollection.SaveScanG(
														dir,
														fil,
														TilePanel.Spriteset))
							{
								string error = String.Format(
														CultureInfo.CurrentCulture,
														"An I/O error occurred.");
								MessageBox.Show(
											error,
											" Error",
											MessageBoxButtons.OK,
											MessageBoxIcon.Error,
											MessageBoxDefaultButton.Button1,
											0);
							}
							else
							{
								LoadScanG(sfd.FileName); // TODO: Maintain current Palette.
								// TODO: FireMvReloadScanG file
							}
						}
						else
						{
							// TODO: rework the following conglomeration

							bool overwrite;
							if (   dir.Equals(Dir, StringComparison.OrdinalIgnoreCase)
								&& fil.Equals(Fil, StringComparison.OrdinalIgnoreCase))
							{
								BackupSpritesetFiles();
								overwrite = true;
							}
							else
								overwrite = false;

							if (SpriteCollection.SaveSpriteset(
															dir,
															fil,
															TilePanel.Spriteset,
															TilePanel.Spriteset.TabwordLength))
							{
								LoadSpriteset(Path.Combine(dir, fil + GlobalsXC.PckExt));
								FireMvReload = true;
							}
							else
							{
								ShowSaveError();

								if (!overwrite)
								{
									File.Delete(Path.Combine(dir, fil + GlobalsXC.PckExt));
									File.Delete(Path.Combine(dir, fil + GlobalsXC.TabExt));
								}
								else
									RevertFiles();
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Exports all sprites in the currently loaded spriteset to PNG files.
		/// Called when the mainmenu's file-menu Click event is raised.
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
						string file = TilePanel.Spriteset.Label.ToUpperInvariant();

						fbd.Description = String.Format(
													CultureInfo.CurrentCulture,
													"Export spriteset to 8-bpp PNG files"
														+ Environment.NewLine + Environment.NewLine
														+ "\t" + file);

						if (fbd.ShowDialog() == DialogResult.OK)
						{
							string path = fbd.SelectedPath;

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
															CultureInfo.InvariantCulture,
															"_{0:" + digits + "}",
															sprite.Id);
								string fullpath = Path.Combine(path, file + suffix + PngExt);
								BitmapService.ExportSprite(fullpath, sprite.Sprite);
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Exports all sprites in the currently loaded spriteset to a PNG
		/// spritesheet file.
		/// Called when the mainmenu's file-menu Click event is raised.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnExportSpritesheetClick(object sender, EventArgs e)
		{
			if (TilePanel.Spriteset != null && TilePanel.Spriteset.Count != 0)
			{
				using (var fbd = new FolderBrowserDialog())
				{
					string file = TilePanel.Spriteset.Label.ToUpperInvariant();

					fbd.Description = String.Format(
												CultureInfo.CurrentCulture,
												"Export spriteset to an 8-bpp PNG spritesheet file"
													+ Environment.NewLine + Environment.NewLine
													+ "\t" + file);

					if (fbd.ShowDialog() == DialogResult.OK)
					{
						string fullpath = Path.Combine(fbd.SelectedPath, file + PngExt);
						if (!File.Exists(fullpath))
						{
							BitmapService.ExportSpritesheet(fullpath, TilePanel.Spriteset, Pal, 8);
						}
						else
							MessageBox.Show(
										this,
										file + PngExt + " already exists.",
										" Error",
										MessageBoxButtons.OK,
										MessageBoxIcon.Error,
										MessageBoxDefaultButton.Button1,
										0);
					}
				}
			}
		}

		/// <summary>
		/// Imports (and replaces) the current spriteset from a BMP spritesheet.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnImportSpritesheetClick(object sender, EventArgs e)
		{
			if (TilePanel.Spriteset != null)
			{
				using (var ofd = new OpenFileDialog())
				{
					ofd.Title = "Import an 8-bpp spritesheet file";
					ofd.Filter = "Image files (*.PNG *.GIF *.BMP)|*.PNG;*.GIF;*.BMP|"
							   + "PNG files (*.PNG)|*.PNG|GIF files (*.GIF)|*.GIF|BMP files (*.BMP)|*.BMP|"
							   + "All files (*.*)|*.*";

					if (ofd.ShowDialog() == DialogResult.OK)
					{
						TilePanel.Spriteset.Sprites.Clear();

						byte[] bindata = File.ReadAllBytes(ofd.FileName);
						Bitmap b = BitmapHandler.LoadBitmap(bindata);

						if (   b.Width  % XCImage.SpriteWidth  == 0
							&& b.Height % XCImage.SpriteHeight == 0
							&& b.PixelFormat == PixelFormat.Format8bppIndexed)
						{
							SpriteCollection spriteset = BitmapService.CreateSpriteCollection(
																							b,
																							Pal,
																							XCImage.SpriteWidth,
																							XCImage.SpriteHeight,
																							IsScanG);
							for (int i = 0; i != spriteset.Count; ++i)
								TilePanel.Spriteset.Sprites.Add(spriteset[i]);

							InsertSpritesFinish();
						}
						else
							ShowBitmapError(false);
					}
				}
			}
		}

		/// <summary>
		/// Closes the app.
		/// Called when the mainmenu's file-menu Click event is raised.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnQuitClick(object sender, EventArgs e)
		{
			Close();
		}

		/// <summary>
		/// Changes the current palette.
		/// Called when the mainmenu's palette-menu Click event is raised by
		/// mouseclick or hotkey.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnPaletteClick(object sender, EventArgs e)
		{
			var pal = ((MenuItem)sender).Tag as Palette;
			if (pal != Pal)
			{
				_paletteItems[Pal].Checked = false;

				Pal = pal;
				Pal.SetTransparent(miTransparent.Checked);

				_paletteItems[Pal].Checked = true;

				TilePanel.Spriteset.Pal = Pal;

				if (PaletteChanged != null)
					PaletteChanged();
			}
		}

		/// <summary>
		/// Toggles transparency of the currently loaded palette.
		/// Called when the mainmenu's transparency-menu Click event is raised
		/// by mouseclick or hotkey.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnTransparencyClick(object sender, EventArgs e)
		{
			Pal.SetTransparent(miTransparent.Checked = !miTransparent.Checked);

			TilePanel.Spriteset.Pal = Pal;

			PalettePanel.that.PrintStatusPaletteId();	// update the palette-panel's statusbar
														// in case palette-id #0 is currently selected.
			if (PaletteChanged != null)
				PaletteChanged();
		}

		/// <summary>
		/// Shows a richtextbox with all the bytes of the currently selected
		/// sprite laid out in a fairly readable fashion.
		/// Called when the mainmenu's bytes-menu Click event is raised.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnBytesClick(object sender, EventArgs e)
		{
			if (!miBytes.Checked)
			{
				if (TilePanel.idSel != -1)
				{
					miBytes.Checked = true;
					SpriteBytesManager.LoadBytesTable(
												TilePanel.Spriteset[TilePanel.idSel],
												BytesClosingCallback);
				}
			}
			else
			{
				miBytes.Checked = false;
				SpriteBytesManager.HideBytesTable();
			}
		}

		/// <summary>
		/// Callback for LoadBytesTable().
		/// </summary>
		private void BytesClosingCallback()
		{
			miBytes.Checked = false;
		}

		/// <summary>
		/// Shows the CHM helpfile.
		/// Called when the mainmenu's help-menu Click event is raised.
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
		/// Shows the about-box.
		/// Called when the mainmenu's help-menu Click event is raised.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnAboutClick(object sender, EventArgs e)
		{
			new About().ShowDialog(this);
		}

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


		#region Methods
		/// <summary>
		/// Sets the current palette.
		/// NOTE: Called only from TileView to set the palette externally.
		/// </summary>
		/// <param name="palette"></param>
		public void SetPalette(string palette)
		{
			foreach (var pal in _paletteItems.Keys)
			{
				if (pal.Label.Equals(palette))
				{
					OnPaletteClick(_paletteItems[pal], EventArgs.Empty);
					break;
				}
			}
		}

		/// <summary>
		/// Sets the currently selected id.
		/// NOTE: Called only from TileView to set 'idSel' externally.
		/// </summary>
		/// <param name="id"></param>
		public void SetSelectedId(int id)
		{
			TilePanel.idSel = id;
			PrintSelectedId();
		}


		/// <summary>
		/// Loads a PCK file.
		/// NOTE: May be called from MapView.Forms.MapObservers.TileViews.TileView.OnPckEditorClick()
		/// - with a string like that you'd think this was .NET itself.
		/// </summary>
		/// <param name="pfePck">fullpath of a PCK file; check existence of the
		/// file before call</param>
		public void LoadSpriteset(string pfePck)
		{
			//LogFile.WriteLine("PckViewForm.LoadSpriteset");
			//LogFile.WriteLine(". " + pfePck);

			SpriteCollection spriteset = null;

			Dir = Path.GetDirectoryName(pfePck);
			Fil = Path.GetFileNameWithoutExtension(pfePck);

			string pfeTab = pfePck.Substring(0, pfePck.Length - 4) + GlobalsXC.TabExt;

			if (File.Exists(pfeTab))	// TODO: This check needs to be bypassed to open PCK-files that don't have a corresponding TAB-file.
			{							// Ie. single-image Bigobs in the UFOGRAPH directory.
				XCImage.SpriteWidth = 32;

				if (IsBigobs) // Bigobs support for XCImage/PckImage ->
					XCImage.SpriteHeight = 48;
				else
					XCImage.SpriteHeight = 40;

				using (var fsPck = File.OpenRead(pfePck)) // try 2-byte tab-offsets in .TAB file
				using (var fsTab = File.OpenRead(pfeTab)) // ie, UFO/TFTD terrain-sprites or Bigobs, UFO unit-sprites
				{
					//LogFile.WriteLine(". try 2-byte offsets");
					spriteset = new SpriteCollection(
												fsPck,
												fsTab,
												ResourceInfo.TAB_WORD_LENGTH_2,
												Palette.UfoBattle,
												Fil);
				}

				if (spriteset.Borked && !spriteset.BorkedBigobs)
				{
					using (var fsPck = File.OpenRead(pfePck)) // try 4-byte tab-offsets in .TAB file
					using (var fsTab = File.OpenRead(pfeTab)) // ie, TFTD unit-sprites
					{
						//LogFile.WriteLine(". try 4-byte offsets");
						spriteset = new SpriteCollection(
													fsPck,
													fsTab,
													ResourceInfo.TAB_WORD_LENGTH_4,
													Palette.TftdBattle,
													Fil);
					}
				}

				if (spriteset.Borked)
				{
					spriteset = null;

					MessageBox.Show(
								"The quantity of sprites in the PCK file"
									+ " does not match the quantity of"
									+ " sprites expected by the TAB file.",
								" Error",
								MessageBoxButtons.OK,
								MessageBoxIcon.Error,
								MessageBoxDefaultButton.Button1,
								0);
				}

				if (spriteset.BorkedBigobs)
				{
					spriteset = null;

					string error = String.Empty;
					if (IsBigobs)
						error = String.Format(
										CultureInfo.CurrentCulture,
										"Cannot load Terrain or Units in a 32x48 spriteset.");
					else
						error = String.Format(
										CultureInfo.CurrentCulture,
										"Cannot load Bigobs in a 32x40 spriteset.");
					MessageBox.Show(
								error,
								" Error",
								MessageBoxButtons.OK,
								MessageBoxIcon.Error,
								MessageBoxDefaultButton.Button1,
								0);
				}
				else
				{
					if (spriteset != null)
						spriteset.Label = Fil;

					OnPaletteClick(
								_paletteItems[DefaultPalette],
								EventArgs.Empty);
				}
			}
			else
			{
				MessageBox.Show(
							this,
							"Tab file does not exist"
								+ Environment.NewLine + Environment.NewLine
								+ pfeTab,
							" Error",
							MessageBoxButtons.OK,
							MessageBoxIcon.Error,
							MessageBoxDefaultButton.Button1,
							0);
			}

			if ((TilePanel.Spriteset = spriteset) == null)
			{
				Title = String.Empty;

				Dir =
				Fil = null;
			}
			else
				Title = pfePck;

			Changed = false;
		}

		private void LoadScanG(string pfeScanG)
		{
			Dir = Path.GetDirectoryName(pfeScanG);
			Fil = Path.GetFileNameWithoutExtension(pfeScanG);

			XCImage.SpriteWidth  = 4;
			XCImage.SpriteHeight = 4;

			SpriteCollection spriteset = null;

			using (var fs = File.OpenRead(pfeScanG))
				spriteset = new SpriteCollection(Fil, fs);

			OnPaletteClick(
						_paletteItems[DefaultPalette],
						EventArgs.Empty);

			TilePanel.Spriteset = spriteset;

			Title = pfeScanG;
			Changed = false;
		}


		/// <summary>
		/// Backs up the PCK+TAB files before trying a Save or SaveAs.
		/// @note See also McdView.McdViewF.OnClick_SaveSpriteset().
		/// @note A possible internal reason that a spriteset is invalid is that
		/// if the total length of its compressed PCK-data exceeds 2^16 bits
		/// (roughly). That is, the TAB file tracks the offsets and needs to
		/// know the total length of the PCK file, but UFO's TAB file stores the
		/// offsets in only 2-byte format (2^16 bits) so the arithmetic explodes
		/// with an overflow as soon as an offset for one of the sprites becomes
		/// too large. (Technically, the total PCK data can exceed 2^16 bits;
		/// but the *start offset* for a sprite cannot -- at least that's how it
		/// works in MapView I/II. Other apps like XCOM, OpenXcom, MCDEdit will
		/// use their own routines.)
		/// @note It appears that TFTD's terrain files suffer this limitation
		/// also (2-byte TabwordLength).
		/// </summary>
		private void BackupSpritesetFiles()
		{
			// TODO: Don't be such a nerd; see McdView's safety save routine.

			Directory.CreateDirectory(Dir); // in case user deleted the dir.

			_pfePck = Path.Combine(Dir, Fil + GlobalsXC.PckExt);
			_pfeTab = Path.Combine(Dir, Fil + GlobalsXC.TabExt);

			_pfePck0 =
			_pfeTab0 = String.Empty;

			string dirBackup = Path.Combine(Dir, GlobalsXC.MV_Backup);

			if (File.Exists(_pfePck))
			{
				Directory.CreateDirectory(dirBackup);

				_pfePck0 = Path.Combine(dirBackup, Fil + GlobalsXC.PckExt);
				File.Copy(_pfePck, _pfePck0, true);
			}

			if (File.Exists(_pfeTab))
			{
				Directory.CreateDirectory(dirBackup);

				_pfeTab0 = Path.Combine(dirBackup, Fil + GlobalsXC.TabExt);
				File.Copy(_pfeTab, _pfeTab0, true);
			}
		}

		/// <summary>
		/// Reverts to the backup files if the TAB-offsets grow too large when
		/// making a Save/SaveAs.
		/// </summary>
		private void RevertFiles()
		{
			if (!String.IsNullOrEmpty(_pfePck0))
				File.Copy(_pfePck0, _pfePck, true);

			if (!String.IsNullOrEmpty(_pfeTab0))
				File.Copy(_pfeTab0, _pfeTab, true);
		}

		/// <summary>
		/// Shows user an error if saving goes bad due to the 2-byte TAB
		/// limitation.
		/// </summary>
		private void ShowSaveError()
		{
			MessageBox.Show(
						this,
						"The size of the encoded sprite-data has grown too large"
							+ " to be stored accurately by the Tab file. Try"
							+ " deleting sprite(s) or (less effective) using"
							+ " more transparency over all sprites."
							+ Environment.NewLine + Environment.NewLine
							+ "Files have *not* been saved.",
						" Error - excessive pixel data (overflow condition)",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error,
						MessageBoxDefaultButton.Button1,
						0);
		}


		/// <summary>
		/// Updates the status-information for the sprite that the cursor is
		/// currently over.
		/// </summary>
		internal void PrintOverId()
		{
			string over;
			if (TilePanel.idOver != -1)
				over = TilePanel.idOver.ToString(CultureInfo.InvariantCulture);
			else
				over = None;

			tssl_TileOver.Text = String.Format(
										CultureInfo.InvariantCulture,
										Over + "{0}", over);
		}

		/// <summary>
		/// Updates the status-information for the sprite that is currently
		/// selected.
		/// </summary>
		/// <param name="valid"></param>
		internal void PrintSelectedId(bool valid = true)
		{
			string selected;

			int id = TilePanel.idSel;
			if (id != -1)
			{
				selected = id.ToString(CultureInfo.InvariantCulture);
				if (IsScanG)
				{
					if (id > 34)
						selected += " [" + (id - 35).ToString(CultureInfo.InvariantCulture) + "]";
					else
						selected += " [0]";
				}
			}
			else
				selected = None;

			tssl_TileSelected.Text = String.Format(
												CultureInfo.InvariantCulture,
												Selected + "{0}", selected);


			valid = valid
				 && TilePanel.Spriteset != null
				 && TilePanel.Spriteset.TabwordLength == ResourceInfo.TAB_WORD_LENGTH_2;

			tss_Sep1       .Visible =
			tssl_Offset    .Visible =
			tssl_OffsetLast.Visible =
			tssl_OffsetAftr.Visible = valid;

			if (valid)
			{
				PrintOffsets();
			}
			else
			{
				tssl_OffsetLast.Text =
				tssl_OffsetAftr.Text = String.Empty;
			}
		}

		/// <summary>
		/// Prints the quantity of sprites in the currently loaded spriteset to
		/// the statusbar.
		/// </summary>
		/// <param name="valid">true if the spriteset is valid</param>
		private void PrintTotal(bool valid)
		{
			PrintOverId();
			PrintSelectedId(valid);

			if (valid)
			{
				tssl_TilesTotal.Text = String.Format(
												CultureInfo.InvariantCulture,
												Total + "{0}", TilePanel.Spriteset.Count);
			}
			else
				tssl_TilesTotal.Text = String.Empty;
		}

		/// <summary>
		/// 
		/// </summary>
		private void PrintOffsets()
		{
			int spriteId;
			if (TilePanel.idSel != -1)
				spriteId = TilePanel.idSel;
			else
				spriteId = TilePanel.Spriteset.Count - 1;

			uint last, aftr;
			SpriteCollection.Test2byteSpriteset(TilePanel.Spriteset, out last, out aftr, spriteId);

			tssl_OffsetLast.ForeColor = (last > UInt16.MaxValue) ? Color.Crimson : SystemColors.ControlText;
			tssl_OffsetAftr.ForeColor = (aftr > UInt16.MaxValue) ? Color.Crimson : SystemColors.ControlText;

			tssl_OffsetLast.Text = last.ToString();
			tssl_OffsetAftr.Text = aftr.ToString();
		}

		/// <summary>
		/// Prints the label of the currently loaded spriteset to the statubar.
		/// </summary>
		/// <param name="valid">true if the spriteset is valid</param>
		private void PrintSpritesetLabel(bool valid)
		{
			string text;
			if (valid)
			{
				text = TilePanel.Spriteset.Label;

				if      (IsBigobs) text += " (32x48)"; // TODO: Use bitflags.
				else if (IsScanG)  text += " (4x4)";
				else               text += " (32x40)";
			}
			else
				text = String.Empty;

			tssl_SpritesetLabel.Text = text;
		}


		/// <summary>
		/// Checks state of the 'Changed' flag and/or asks user if the spriteset
		/// ought be closed anyway.
		/// </summary>
		/// <returns>true if state is NOT changed or 'DialogResult.Yes'</returns>
		private bool CheckQuit()
		{
			return !Changed
				|| MessageBox.Show(
								this,
								"The spriteset has changed. Do you really want to close it?",
								" Spriteset changed",
								MessageBoxButtons.YesNo,
								MessageBoxIcon.Exclamation,
								MessageBoxDefaultButton.Button2,
								0) == DialogResult.Yes;
		}
		#endregion Methods
	}


	#region Delegates
	internal delegate void PaletteChangedEvent();
	#endregion Delegates
}
