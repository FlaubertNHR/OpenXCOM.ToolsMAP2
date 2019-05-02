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
		/// Selects the last part in the Main window's parts-array and inserts
		/// selected parts after that part.
		/// @note This is for instant insertion of parts via the CopyPanel to
		/// the MainPanel.
		/// </summary>
		private void OnInsertAfterLastClick(object sender, EventArgs e)
		{
			if (SelId != -1 && _f.Parts != null)
			{
				bool refsdead = false;
				bool refsalt  = false;

				if (_fcopy.gb_IalOptions.Enabled)
				{
					if (   _fcopy.cb_IalSprites.Enabled
						&& _fcopy.cb_IalSprites.Checked)
					{
						
					}

					if (   _fcopy.cb_IalDeadpart.Enabled
						&& _fcopy.cb_IalDeadpart.Checked)
					{
						refsdead = true;

						if (   _fcopy.cb_IalDeadsubs.Enabled
							&& _fcopy.cb_IalDeadsubs.Checked)
						{
						}

						if (   _fcopy.cb_IalDeadsprites.Enabled
							&& _fcopy.cb_IalDeadsprites.Checked)
						{
						}
					}

					if (   _fcopy.cb_IalAltpart.Enabled
						&& _fcopy.cb_IalAltpart.Checked)
					{
						refsalt = true;

						if (   _fcopy.cb_IalAltsubs.Enabled
							&& _fcopy.cb_IalAltsubs.Checked)
						{
						}

						if (   _fcopy.cb_IalAltsprites.Enabled
							&& _fcopy.cb_IalAltsprites.Checked)
						{
						}
					}
				}

				OnCopyClick(sender, e);

				if (refsdead || refsalt)
				{
					SegregateParts(refsdead, refsalt);
					// TODO: get the Dead and Alt parts of the Dead and Alt parts ad nausea.
				}

				_f.PartsPanel.InsertAfterLast(refsdead, refsalt);
			}
		}

		private void SegregateParts(bool refsdead, bool refsalt)
		{
			_copydeads.Clear();
			_copyaltrs .Clear();

			var ids     = new HashSet<int>();
			var idsDead = new HashSet<int>();
			var idsAlt  = new HashSet<int>();

			foreach (var part in _copyparts)
				ids.Add(part.TerId);

			if (refsdead)
			{
				foreach (var part in _copyparts)
				{
					if (part.Dead != null
						&& !ids.Contains(part.Dead.TerId))
					{
						idsDead.Add(part.Dead.TerId);
					}
				}
			}

			if (refsalt)
			{
				foreach (var part in _copyparts)
				{
					if (part.Alternate != null
						&& !ids.Contains(part.Alternate.TerId))
					{
						idsAlt.Add(part.Alternate.TerId);
					}
				}
				idsAlt.ExceptWith(idsDead);
			}


			foreach (var id in idsDead)
				_copydeads.Add(Parts[id]);

			foreach (var id in idsAlt)
				_copyaltrs.Add(Parts[id]);
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
