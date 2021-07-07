using System;
using System.Windows.Forms;

using XCom;


namespace McdView
{
	/// <summary>
	/// The panel that displays the entire MCD recordset with each record's
	/// Sprite1 sprite.
	/// </summary>
	internal sealed class TerrainPanel_copier
		:
			TerrainPanel
	{
		#region Properties
		protected override int SelId
		{
			get { return _fcopier.SelId; }
			set { _fcopier.SelId = value; }
		}
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="f"></param>
		/// <param name="fcopier"></param>
		internal TerrainPanel_copier(McdviewF f, CopierF fcopier)
			:
				base(f, fcopier)
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
		/// @note This is for instant insertion of parts via the Copier to
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
			e.Handled = e.SuppressKeyPress = true;
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
			switch (e.KeyData)
			{
				default:
					if (SelId != -1 || e.KeyData == Keys.Space) // only key [Space] can change the selected-id from -1 to 0.
					{
						switch (e.KeyData)
						{
							case Keys.Left:
							case Keys.Up:
							case Keys.Back:
								SubIds.Clear();
								if (SelId != 0)
								{
									SelId -= 1;
								}
								else
									Invalidate();
								break;

							case Keys.Control | Keys.Left:
							case Keys.Control | Keys.Up:
							case Keys.Control | Keys.Back:
								if (SelId != 0)
								{
									SubIds.Add(SelId);
									SelId -= 1;
								}
								break;

							case Keys.Shift | Keys.Left:
							case Keys.Shift | Keys.Up:
							case Keys.Shift | Keys.Back:
								if (SelId != 0)
								{
									SubIds.Remove(SelId);
									SelId -= 1;
								}
								break;


							case Keys.Right:
							case Keys.Down:
							case Keys.Space:
								SubIds.Clear();
								if (SelId != Parts.Length - 1)
								{
									SelId += 1;
								}
								else
									Invalidate();
								break;

							case Keys.Control | Keys.Right:
							case Keys.Control | Keys.Down:
							case Keys.Control | Keys.Space:
								if (SelId != Parts.Length - 1)
								{
									SubIds.Add(SelId);
									SelId += 1;
								}
								break;

							case Keys.Shift | Keys.Right:
							case Keys.Shift | Keys.Down:
							case Keys.Shift | Keys.Space:
								if (SelId != Parts.Length - 1)
								{
									SubIds.Remove(SelId);
									SelId += 1;
								}
								break;


							case Keys.PageUp:
								SubIds.Clear();
								if (SelId != 0)
								{
									int id = SelId - (Width / (Spriteset.SpriteWidth32 + 1));
									if (id < 0) id = 0;

									SelId = id;
								}
								else
									Invalidate();
								break;

							case Keys.Control | Keys.PageUp:
								if (SelId != 0)
								{
									int id = SelId - (Width / (Spriteset.SpriteWidth32 + 1));
									if (id < 0) id = 0;

									for (int i = SelId; i != id; --i)
										SubIds.Add(i);

									SelId = id;
								}
								break;

							case Keys.Shift | Keys.PageUp:
								if (SelId != 0)
								{
									int id = SelId - (Width / (Spriteset.SpriteWidth32 + 1));
									if (id < 0) id = 0;

									for (int i = SelId; i != id; --i)
										SubIds.Remove(i);

									SelId = id;
								}
								break;


							case Keys.PageDown:
								SubIds.Clear();
								if (SelId != Parts.Length - 1)
								{
									int id = SelId + (Width / (Spriteset.SpriteWidth32 + 1));
									if (id > Parts.Length - 1) id = Parts.Length - 1;

									SelId = id;
								}
								else
									Invalidate();
								break;

							case Keys.Control | Keys.PageDown:
								if (SelId != Parts.Length - 1)
								{
									int id = SelId + (Width / (Spriteset.SpriteWidth32 + 1));
									if (id > Parts.Length - 1) id = Parts.Length - 1;

									for (int i = SelId; i != id; ++i)
										SubIds.Add(i);

									SelId = id;
								}
								break;

							case Keys.Shift | Keys.PageDown:
								if (SelId != Parts.Length - 1)
								{
									int id = SelId + (Width / (Spriteset.SpriteWidth32 + 1));
									if (id > Parts.Length - 1) id = Parts.Length - 1;

									for (int i = SelId; i != id; ++i)
										SubIds.Remove(i);

									SelId = id;
								}
								break;


							case Keys.Home:
								SubIds.Clear();
								if (SelId != 0)
								{
									SelId = 0;
								}
								else
									Invalidate();
								break;

							case Keys.Control | Keys.Home:
								if (SelId != 0)
								{
									for (int i = SelId; i != 0; --i)
										SubIds.Add(i);

									SelId = 0;
								}
								break;

							case Keys.Shift | Keys.Home:
								if (SelId != 0)
								{
									for (int i = SelId; i != 0; --i)
										SubIds.Remove(i);

									SelId = 0;
								}
								break;


							case Keys.End:
								SubIds.Clear();
								if (SelId != Parts.Length - 1)
								{
									SelId = Parts.Length - 1;
								}
								else
									Invalidate();
								break;

							case Keys.Control | Keys.End:
								if (SelId != Parts.Length - 1)
								{
									for (int i = SelId; i != Parts.Length - 1; ++i)
										SubIds.Add(i);

									SelId = Parts.Length - 1;
								}
								break;

							case Keys.Shift | Keys.End:
								if (SelId != Parts.Length - 1)
								{
									for (int i = SelId; i != Parts.Length - 1; ++i)
										SubIds.Remove(i);

									SelId = Parts.Length - 1;
								}
								break;
						}
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
