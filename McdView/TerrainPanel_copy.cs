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
		#region cTor
		internal TerrainPanel_copy(McdviewF f)
			:
				base(f)
		{
			CreateContext();
		}

		/// <summary>
		/// Builds and assigns an RMB context-menu.
		/// </summary>
		private void CreateContext()
		{
/*			var itAdd         = new MenuItem("add",             OnAddClick);			// d key
			var itAddRange    = new MenuItem("add range ...",   OnAddRangeClick);		// Ctrl+d key

			var itSep0        = new MenuItem("-");

			var itCut         = new MenuItem("cut",             OnCutClick);			// Ctrl+x key
			var itCopy        = new MenuItem("copy",            OnCopyClick);			// Ctrl+c key
			var itInsert      = new MenuItem("insert after",    OnInsertClick);			// Ctrl+v key
			var itDelete      = new MenuItem("delete",          OnDeleteClick);			// Delete key

			var itSep1        = new MenuItem("-");

			var itFile        = new MenuItem("append file ...", OnFileClick);			// f key

			var itSep2        = new MenuItem("-");

			var itLeft        = new MenuItem("swap left",       OnSwapLeftClick);		// - key
			var itRight       = new MenuItem("swap right",      OnSwapRightClick);		// + key

			var itSep3        = new MenuItem("-");

			var itSelect      = new MenuItem("select all",      OnSelectAllClick);		// Ctrl+a key
			var itDeselect    = new MenuItem("deselect all",    OnDeselectAllClick);	// Esc

			Context = new ContextMenu();
			Context.MenuItems.AddRange(new []
										{
											itAdd,			//  0
											itAddRange,		//  1
											itSep0,			//  2
											itCut,			//  3
											itCopy,			//  4
											itInsert,		//  5
											itDelete,		//  6
											itSep1,			//  7
											itFile,			//  8
											itSep2,			//  9
											itLeft,			// 10
											itRight,		// 11
											itSep3,			// 12
											itSelect,		// 13
											itDeselect		// 14
										});
			ContextMenu = Context;

			Context.Popup += OnPopup_Context; */
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
/*			bool parts = (Parts != null);
			bool selid = (SelId != -1);

			Context.MenuItems[0].Enabled = parts;								// add
			Context.MenuItems[1].Enabled = parts;								// add range

			Context.MenuItems[3].Enabled = selid;								// cut
			Context.MenuItems[4].Enabled = selid;								// copy
			Context.MenuItems[5].Enabled = parts && _copyparts.Count != 0;		// insert
			Context.MenuItems[6].Enabled = selid;								// delete

			Context.MenuItems[8].Enabled = (parts && false);					// file

			Context.MenuItems[10].Enabled =          SelId > 0;					// left
			Context.MenuItems[11].Enabled = selid && SelId != Parts.Length - 1;	// right

			Context.MenuItems[13].Enabled = parts && Parts.Length != 0;			// select
			Context.MenuItems[14].Enabled = selid;								// deselect */
		}

/*		/// <summary>
		/// Closes the contextmenu.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnIdClick(object sender, EventArgs e)
		{
			Context.Dispose();
		} */

		/// <summary>
		/// Copies a currently selected part along with any sub-selected parts
		/// to the copy-array.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnCopyClick(object sender, EventArgs e)
		{
			_copyparts.Clear();

			_copylabel = _f.Label;

			var sels = new List<int>(SubIds);
			sels.Add(SelId);
			sels.Sort();

			foreach (int id in sels)
				_copyparts.Add(Parts[id].Clone());
		}

		/// <summary>
		/// Selects the last part and sub-selects all other parts.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnSelectAllClick(object sender, EventArgs e)
		{
			for (int i = 0; i != Parts.Length - 1; ++i)
				SubIds.Add(i);

			if (SelId != Parts.Length - 1)
				SelId  = Parts.Length - 1;
			else
				Invalidate();
		}

		/// <summary>
		/// Deselects a currently selected part as well as any sub-selected
		/// parts.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnDeselectAllClick(object sender, EventArgs e)
		{
			SubIds.Clear();
			SelId = -1;
		}
		#endregion Events (context)

/*
		#region Events (override)
		/// <summary>
		/// This is required in order to accept arrow-keyboard-input via
		/// McdviewF.OnKeyDown().
		/// </summary>
		/// <param name="keyData"></param>
		/// <returns></returns>
		protected override bool IsInputKey(Keys keyData)
		{
			switch (keyData)
			{
				case Keys.Left:
				case Keys.Up:
				case Keys.Right:
				case Keys.Down:
				case Keys.Shift | Keys.Left:
				case Keys.Shift | Keys.Up:
				case Keys.Shift | Keys.Right:
				case Keys.Shift | Keys.Down:
					return true;
			}
			return base.IsInputKey(keyData);
		}
		#endregion Events (override)
*/

		#region Methods
/*		/// <summary>
		/// Scrolls the panel to ensure that the currently selected part is
		/// fully displayed.
		/// </summary>
		internal void ScrollToPart()
		{
			if (SelId != -1 && Scroller.Enabled)
			{
				int x = SelId * (XCImage.SpriteWidth32 + 1);
				if (x < Scroller.Value)
				{
					Scroller.Value = x;
				}
				else if ((x += XCImage.SpriteWidth32) > Width + Scroller.Value)
				{
					Scroller.Value = x - Width;
				}
			}
		} */

		/// <summary>
		/// Takes keyboard-input from the Form's KeyDown event to select a part
		/// or parts.
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
				case Keys.C:													// copy
					if (e.Control && SelId != -1)
						OnCopyClick(null, EventArgs.Empty);
					break;
				case Keys.A:													// select all
					if (e.Control && Parts != null && Parts.Length != 0)
						OnSelectAllClick(null, EventArgs.Empty);
					break;

				// NOTE: Escape for deselect all is handled by the caller: McdviewF.OnKeyDown().
			}
		}

/*		/// <summary>
		/// Gets the loc of the currently selected tile relative to the table.
		/// </summary>
		/// <returns></returns>
		private int GetTileLeft()
		{
			return SelId * (XCImage.SpriteWidth32 + 1);
		}
		/// <summary>
		/// Gets the loc+width of the currently selected tile relative to the table.
		/// </summary>
		/// <returns></returns>
		private int GetTileRight()
		{
			return SelId * (XCImage.SpriteWidth32 + 1) + XCImage.SpriteWidth32;
		} */
		#endregion Methods
	}
}
