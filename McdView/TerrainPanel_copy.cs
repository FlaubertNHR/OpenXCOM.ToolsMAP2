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
			var itInsert   = new ToolStripMenuItem("insert after last", null, OnInsertAfterLastClick, Keys.Control | Keys.I);

			var itSep0     = new ToolStripSeparator();

			var itCopy     = new ToolStripMenuItem("copy for insertion", null, OnCopyClick, Keys.Control | Keys.C);

			var itSep1     = new ToolStripSeparator();

			var itSelect   = new ToolStripMenuItem("select all",   null, OnSelectAllClick, Keys.Control | Keys.A);
			var itDeselect = new ToolStripMenuItem("deselect all", null, OnDeselectAllClick); // Esc - not allowed, is handled by KeyDown event.

			itDeselect.ShortcutKeyDisplayString = "Esc"; // ie. the "ShortcutKeyDisplayOnlyString"

			Context = new ContextMenuStrip();
			Context.Items.Add(itInsert);	// 0
			Context.Items.Add(itSep0);		// 1
			Context.Items.Add(itCopy);		// 2
			Context.Items.Add(itSep1);		// 3
			Context.Items.Add(itSelect);	// 4
			Context.Items.Add(itDeselect);	// 5
			Context.Opening += OnOpening_Context;

			ContextMenuStrip = Context;
		}
		#endregion cTor


		#region Events (context)
		/// <summary>
		/// Determines which contextmenu commands are enabled when the menu
		/// is opened.
		/// IMPORTANT: The conditions shall be synched w/ KeyInput() and/or
		/// their respective shortcut handlers.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnOpening_Context(object sender, EventArgs e)
		{
			bool selid = (SelId != -1);

			Context.Items[0].Enabled = selid && _f.Parts != null;			// insert after last
			Context.Items[2].Enabled = selid;								// copy
			Context.Items[4].Enabled = Parts != null && Parts.Length != 0;	// select
			Context.Items[5].Enabled = selid;								// deselect
		}

		/// <summary>
		/// A special insert-operation. Inserts selected parts after the last
		/// tilepart in Main.
		/// @note This is for instant insertion of parts via the CopyPanel to
		/// the MainPanel.
		/// </summary>
		private void OnInsertAfterLastClick(object sender, EventArgs e)
		{
			if (SelId != -1 && _f.Parts != null)
			{
				OnCopyClick(sender, e);
				_f.PartsPanel.InsertAfterLast();
			}
		}
		#endregion Events (context)


		#region Events (override)
		/// <summary>
		/// @note Shortcuts on the contextmenu items happen regardless of
		/// key-suppression or call to base; neither do they need to call
		/// KeyInput().
		/// </summary>
		/// <param name="e"></param>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			e.SuppressKeyPress = true;
			KeyInput(e);
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
				// IMPORTANT: The conditions shall be synched w/ OnOpening_Context().

				case Keys.Escape:
					SelId = -1;
					break;
			}
		}
		#endregion Methods
	}
}
