using System;
using System.Collections.Generic;
using System.Windows.Forms;

using XCom.Interfaces;


namespace McdView
{
	/// <summary>
	/// The panel that displays the entire MCD recordset with each record's
	/// Sprite1 sprite.
	/// </summary>
	internal sealed class TerrainPanel_copy
		:
			TerrainPanel
	{
		#region Properties
		protected override int SelId
		{
			get { return _fcopy.SelId; }
			set { _fcopy.SelId = value; }
		}
		#endregion Properties


		#region cTor
		internal TerrainPanel_copy(McdviewF f, CopyPanelF fcopy)
			:
				base(f, fcopy)
		{
			CreateContext();
		}

		/// <summary>
		/// Builds and assigns an RMB context-menu.
		/// </summary>
		private void CreateContext()
		{
			var itInsert   = new MenuItem("insert after last",  OnInsertClick, Shortcut.CtrlI);	// Ctrl+i key

			var itSep0     = new MenuItem("-");

			var itCopy     = new MenuItem("copy for insertion", OnCopyClick);	// Ctrl+c key

			var itSep1     = new MenuItem("-");

			var itSelect   = new MenuItem("select all",   OnSelectAllClick);	// Ctrl+a key
			var itDeselect = new MenuItem("deselect all", OnDeselectAllClick);	// Esc

			Context = new ContextMenu();
			Context.MenuItems.AddRange(new []
										{
											itInsert,	// 0
											itSep0,		// 1
											itCopy,		// 2
											itSep1,		// 3
											itSelect,	// 4
											itDeselect	// 5
										});
			ContextMenu = Context;

			Context.Popup += OnPopup_Context;
		}
		#endregion cTor


		#region Events (context)
		/// <summary>
		/// Determines which contextmenu commands are enabled when the menu
		/// is opened.
		/// IMPORTANT: The conditions shall be synched w/ KeyInput().
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnPopup_Context(object sender, EventArgs e)
		{
			bool selid = (SelId != -1);

			Context.MenuItems[0].Enabled = selid;								// insert after last
			Context.MenuItems[2].Enabled = selid;								// copy
			Context.MenuItems[4].Enabled = Parts != null && Parts.Length != 0;	// select
			Context.MenuItems[5].Enabled = selid;								// deselect
		}

		/// <summary>
		/// Selects the last part in the Main window's parts-array and inserts
		/// selected parts after that part.
		/// @note This is for automatic insertion of parts via the CopyPanel.
		/// </summary>
		private void OnInsertClick(object sender, EventArgs e)
		{
			OnCopyClick(sender, e);

			if (_f.Parts != null)
			{
				_f.SelId = _f.Parts.Length - 1;
				_f.PartsPanel.OnInsertClick(null, EventArgs.Empty);
			}
		}
		#endregion Events (context)


		#region Events (override)
		protected override void OnKeyDown(KeyEventArgs e)
		{
			e.SuppressKeyPress = true;
			KeyInput(e);

//			base.OnKeyDown(e);
		}
		#endregion Events (override)


		#region Methods
		/// <summary>
		/// Takes keyboard-input from the KeyDown event to select a part or
		/// parts.
		/// </summary>
		/// <param name="e"></param>
		internal void KeyInput(KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Left:
				case Keys.Up:
				case Keys.Back:
					if (SelId != -1)
					{
						if (!e.Control)
						{
							if (!e.Shift)
							{
								SubIds.Clear();
								if (SelId == 0)
									Invalidate();
							}
							else if (SelId != 0)
								SubIds.Remove(SelId);
						}
						else if (SelId != 0)
							SubIds.Add(SelId);

						if (SelId != 0)
							SelId -= 1;
					}
					break;

				case Keys.Right:
				case Keys.Down:
					if (SelId != -1)
						goto case Keys.Space;
					break;
				case Keys.Space: // at present only the spacebar can change the selected id from #-1 to #0
					if (!e.Control)
					{
						if (!e.Shift)
						{
							SubIds.Clear();
							if (SelId == Parts.Length - 1)
								Invalidate();
						}
						else if (SelId != Parts.Length - 1)
							SubIds.Remove(SelId);
					}
					else if (SelId != Parts.Length - 1)
						SubIds.Add(SelId);

					if (SelId != Parts.Length - 1)
						SelId += 1;
					break;

				case Keys.PageUp:
					if (SelId != -1)
					{
						int id = SelId - (Width / (XCImage.SpriteWidth32 + 1));
						if (id < 0) id = 0;
	
						if (!e.Control)
						{
							if (!e.Shift)
							{
								SubIds.Clear();
								if (SelId == 0)
									Invalidate();
							}
							else if (SelId != 0)
							{
								for (int i = SelId; i != id; --i)
									SubIds.Remove(i);
							}
						}
						else if (SelId != 0)
						{
							for (int i = SelId; i != id; --i)
								SubIds.Add(i);
						}
	
						if (SelId != 0)
							SelId = id;
					}
					break;

				case Keys.PageDown:
					if (SelId != -1)
					{
						int id = SelId + (Width / (XCImage.SpriteWidth32 + 1));
						if (id > Parts.Length - 1) id = Parts.Length - 1;
	
						if (!e.Control)
						{
							if (!e.Shift)
							{
								SubIds.Clear();
								if (SelId == Parts.Length - 1)
									Invalidate();
							}
							else if (SelId != Parts.Length - 1)
							{
								for (int i = SelId; i != id; ++i)
									SubIds.Remove(i);
							}
						}
						else if (SelId != Parts.Length - 1)
						{
							for (int i = SelId; i != id; ++i)
								SubIds.Add(i);
						}
	
						if (SelId != Parts.Length - 1)
							SelId = id;
					}
					break;

				case Keys.Home:
					if (SelId != -1)
					{
						if (!e.Control)
						{
							if (!e.Shift)
							{
								SubIds.Clear();
								if (SelId == 0)
									Invalidate();
							}
							else if (SelId != 0)
							{
								for (int i = SelId; i != 0; --i)
									SubIds.Remove(i);
							}
						}
						else if (SelId != 0)
						{
							for (int i = SelId; i != 0; --i)
								SubIds.Add(i);
						}

						SelId = 0;
					}
					break;

				case Keys.End:
					if (SelId != -1)
					{
						if (!e.Control)
						{
							if (!e.Shift)
							{
								SubIds.Clear();
								if (SelId == Parts.Length - 1)
									Invalidate();
							}
							else if (SelId != Parts.Length - 1)
							{
								for (int i = SelId; i != Parts.Length - 1; ++i)
									SubIds.Remove(i);
							}
						}
						else if (SelId != Parts.Length - 1)
						{
							for (int i = SelId; i != Parts.Length - 1; ++i)
								SubIds.Add(i);
						}

						SelId = Parts.Length - 1;
					}
					break;


				// Edit functions (keyboard) follow ...
				// IMPORTANT: The conditions shall be synched w/ OnPopup_Context().

				case Keys.I:												// insert after last
					if (e.Control && SelId != -1)
						OnInsertClick(null, EventArgs.Empty);
					break;

				case Keys.C:												// copy
					if (e.Control && SelId != -1)
						OnCopyClick(null, EventArgs.Empty);
					break;

				case Keys.A:												// select all
					if (e.Control && Parts != null && Parts.Length != 0)
						OnSelectAllClick(null, EventArgs.Empty);
					break;

				case Keys.Escape:
					SelId = -1;
					break;
			}
		}
		#endregion Methods
	}
}
