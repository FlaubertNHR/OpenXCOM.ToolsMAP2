using System;
using System.Collections.Generic;
using System.Windows.Forms;

using XCom;
using XCom.Interfaces;


namespace McdView
{
	/// <summary>
	/// The panel that displays the entire MCD recordset with each record's
	/// Sprite1 sprite.
	/// </summary>
	internal sealed class TerrainPanel_main
		:
			TerrainPanel
	{
		#region Fields
		// containers for InsertAfterLast ->
		private readonly List<Tilepart>      _ial_PartsList = new List<Tilepart>();
		private readonly Dictionary<int,int> _ial_PartIds   = new Dictionary<int,int>();
		private readonly Dictionary<int,int> _ial_SpriteIds = new Dictionary<int,int>();
		#endregion Fields


		#region Properties
		protected override int SelId
		{
			get { return _f.SelId; }
			set { _f.SelId = value; }
		}
		#endregion Properties


		#region cTor
		internal TerrainPanel_main(McdviewF f)
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
			var itAdd      = new ToolStripMenuItem("add",             null, OnAddClick); // d key - not allowed, is handled by KeyDown event.
			var itAddRange = new ToolStripMenuItem("add range ...",   null, OnAddRangeClick, Keys.Control | Keys.D);

			itAdd.ShortcutKeyDisplayString = "D";

			var itSep0     = new ToolStripSeparator();

			var itCut      = new ToolStripMenuItem("cut",             null, OnCutClick,    Keys.Control | Keys.X);
			var itCopy     = new ToolStripMenuItem("copy",            null, OnCopyClick,   Keys.Control | Keys.C);
			var itInsert   = new ToolStripMenuItem("insert after",    null, OnInsertClick, Keys.Control | Keys.V);
			var itDelete   = new ToolStripMenuItem("delete",          null, OnDeleteClick, Keys.Delete); // Delete key - wtf.

			var itSep1     = new ToolStripSeparator();

			var itFile     = new ToolStripMenuItem("append file ...", null, OnFileClick); // f key - not allowed, is handled by KeyDown event.

			itFile.ShortcutKeyDisplayString = "F";

			var itSep2     = new ToolStripSeparator();

			var itLeft     = new ToolStripMenuItem("swap left",       null, OnSwapLeftClick);  // - key - not allowed, is handled by KeyDown event.
			var itRight    = new ToolStripMenuItem("swap right",      null, OnSwapRightClick); // + key - not allowed, is handled by KeyDown event.

			itLeft .ShortcutKeyDisplayString = "-";
			itRight.ShortcutKeyDisplayString = "+";

			var itSep3     = new ToolStripSeparator();

			var itSelect   = new ToolStripMenuItem("select all",      null, OnSelectAllClick, Keys.Control | Keys.A);
			var itDeselect = new ToolStripMenuItem("deselect all",    null, OnDeselectAllClick); // Esc - not allowed, is handled by KeyDown event.

			itDeselect.ShortcutKeyDisplayString = "Esc";

			Context = new ContextMenuStrip();
			Context.Items.Add(itAdd);		//  0
			Context.Items.Add(itAddRange);	//  1
			Context.Items.Add(itSep0);		//  2
			Context.Items.Add(itCut);		//  3
			Context.Items.Add(itCopy);		//  4
			Context.Items.Add(itInsert);	//  5
			Context.Items.Add(itDelete);	//  6
			Context.Items.Add(itSep1);		//  7
			Context.Items.Add(itFile);		//  8
			Context.Items.Add(itSep2);		//  9
			Context.Items.Add(itLeft);		// 10
			Context.Items.Add(itRight);		// 11
			Context.Items.Add(itSep3);		// 12
			Context.Items.Add(itSelect);	// 13
			Context.Items.Add(itDeselect);	// 14
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
			bool parts = (Parts != null);
			bool selid = (SelId != -1);

			Context.Items[0].Enabled = parts;								// add
			Context.Items[1].Enabled = parts;								// add range

			Context.Items[3].Enabled = selid;								// cut
			Context.Items[4].Enabled = selid;								// copy
			Context.Items[5].Enabled = parts && _partsCopied.Count != 0;	// insert
			Context.Items[6].Enabled = selid;								// delete

			Context.Items[8].Enabled = (parts && false);					// file

			Context.Items[10].Enabled =          SelId > 0;					// left
			Context.Items[11].Enabled = selid && SelId != Parts.Length - 1;	// right

			Context.Items[13].Enabled = parts && Parts.Length != 0;			// select
			Context.Items[14].Enabled = selid;								// deselect
		}

		// NOTE on the edit-operations: Do not clone on copy/cut.
		// Clone only on insert.

		/// <summary>
		/// Adds a blank part to the parts-array.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnAddClick(object sender, EventArgs e)
		{
			if (Parts != null)
			{
				var array = new Tilepart[Parts.Length + 1];

				int id = SelId + 1;
				for (int i = 0; i != id; ++i)
					array[i] = Parts[i];

				array[id] = new Tilepart(id);

				for (int i = id + 1; i != array.Length; ++i)
				{
					array[i] = Parts[i - 1];
					array[i].TerId = i;
				}

				_bypassScrollZero = true;
				_f.Parts = array; // assign back to 'Parts' via McdviewF

				UpdateRefs();

				SubIds.Clear();
				SelId = id;

				_f.Changed = CacheLoad.Changed(_f.Parts);
			}
		}

		internal static int _add; // is set in 'addrangeinput' dialog.

		private void OnAddRangeClick(object sender, EventArgs e)
		{
			if (Parts != null)
			{
				using (var ari = new AddRangeInput())
				{
					if (ari.ShowDialog() == DialogResult.OK)
					{
						if (_add != 0) // input allows 0 but not neg
						{
							int length = Parts.Length + _add;
							var array = new Tilepart[length];

							int id = SelId + 1;
							int i;
							for (i = 0; i != id; ++i)
								array[i] = Parts[i];

							int j = i + _add;
							for (; i != j; ++i)
							{
								array[i] = new Tilepart(i);
							}

							for (; i != length; ++i)
							{
								array[i] = Parts[i - _add];
								array[i].TerId = i;
							}

							_bypassScrollZero = true;
							_f.Parts = array;

							UpdateRefs();

							SubIds.Clear();
							for (i = id; i != id + _add - 1; ++i)
								SubIds.Add(i);

							SelId = id + _add - 1;

							_f.Changed = CacheLoad.Changed(_f.Parts);
						}
					}
				}
			}
		}

		/// <summary>
		/// Cuts a currently selected part from the parts-array.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnCutClick(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				OnCopyClick(  null, EventArgs.Empty);
				OnDeleteClick(null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Inserts the copy-array into the parts-array after the currently
		/// selected part or at the start of the array if there is no selected
		/// part.
		/// @note If inserted part(s)' refs are less than the insertion point
		/// then the refs are okay; if the refs are equal to or greater than the
		/// insertion point then the refs need to be advanced.
		/// @note The refs shall be deleted if inserted parts are from an
		/// MCD-file that has a different label than the currently loaded
		/// MCD-file.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void OnInsertClick(object sender, EventArgs e)
		{
			if (Parts != null && _partsCopied.Count != 0)
			{
				bool isTer = (_partsCopiedLabel == _f.Label); // null refs if the terrain-labels differ

				int id = SelId + 1;

				var array = new Tilepart[Parts.Length + _partsCopied.Count];

				for (int i = 0, j = 0; i != array.Length; ++i, ++j)
				{
					if (i == id)
					{
						for (int pos = 0; pos != _partsCopied.Count; ++pos, ++i)
						{
							array[i] = _partsCopied[pos].CreateInsert();
							array[i].TerId = i;

							if (!isTer)
							{
								array[i].Record.DieTile = (byte)0;
								array[i].Dead = null;

								array[i].Record.Alt_MCD = (byte)0;
								array[i].Altr = null;
							}
						}
					}

					if (i == array.Length)
						break;

					array[i] = Parts[j];
					array[i].TerId = i;
				}

				_bypassScrollZero = true;
				_f.Parts = array;

				UpdateRefs();

				SubIds.Clear();
				for (int i = id; i != id + _partsCopied.Count - 1; ++i)
					SubIds.Add(i);

				SelId = id + _partsCopied.Count - 1;

				_f.Changed = CacheLoad.Changed(_f.Parts);
			}
		}

		/// <summary>
		/// Updates refs when parts are added or inserted.
		/// </summary>
		private void UpdateRefs()
		{
			for (int i = 0; i != Parts.Length; ++i)
			{
				if (Parts[i].Dead != null)
				{
					Parts[i].Record.DieTile = (byte)Parts[i].Dead.TerId;
				}
				else
					Parts[i].Record.DieTile = (byte)0;

				if (Parts[i].Altr != null)
				{
					Parts[i].Record.Alt_MCD = (byte)Parts[i].Altr.TerId;
				}
				else
					Parts[i].Record.Alt_MCD = (byte)0;
			}
		}


		private bool _spritesetChanged;

		/// <summary>
		/// Clones a tilepart and its sprites from the CopyPanel's partset (and
		/// spriteset) to the Main partset (and spriteset).
		/// </summary>
		/// <param name="id_src">the id of the part in the CopyPanel partset</param>
		/// <param name="id_dst">the id of the part in the Main partset</param>
		private void addPart(int id_src, int id_dst)
		{
			if (!_ial_PartIds.ContainsKey(id_src))
			{
				_ial_PartIds.Add(id_src, id_dst);

				Tilepart part_src = _f.CopyPanel.Parts[id_src];
				McdRecord record_src = part_src.Record;

				int spriteId;
				for (int phase = 0; phase != 8; ++phase)
				{
					switch (phase)
					{
						default: spriteId = record_src.Sprite1; break; //case 0
						case 1:  spriteId = record_src.Sprite2; break;
						case 2:  spriteId = record_src.Sprite3; break;
						case 3:  spriteId = record_src.Sprite4; break;
						case 4:  spriteId = record_src.Sprite5; break;
						case 5:  spriteId = record_src.Sprite6; break;
						case 6:  spriteId = record_src.Sprite7; break;
						case 7:  spriteId = record_src.Sprite8; break;
					}

					if (!_ial_SpriteIds.ContainsKey(spriteId))
					{
						_spritesetChanged = true;

						var sprite_src = _f.CopyPanel.Spriteset[spriteId] as PckImage;
						var sprite_dst = sprite_src.Duplicate(_f.Spriteset, _f.Spriteset.Count);
						_ial_SpriteIds.Add(spriteId, _f.Spriteset.Count);

						_f.Spriteset.Sprites.Add(sprite_dst);
					}
				}

				Tilepart part_dst = part_src.CreateInsert();
				part_dst.TerId = id_dst;
				_ial_PartsList.Add(part_dst);

				McdRecord record_dst = _ial_PartsList[id_dst].Record;

				for (int phase = 0; phase != 8; ++phase)
				{
					switch (phase)
					{
						default: record_dst.Sprite1 = (byte)_ial_SpriteIds[record_dst.Sprite1]; break; //case 0
						case 1:  record_dst.Sprite2 = (byte)_ial_SpriteIds[record_dst.Sprite2]; break;
						case 2:  record_dst.Sprite3 = (byte)_ial_SpriteIds[record_dst.Sprite3]; break;
						case 3:  record_dst.Sprite4 = (byte)_ial_SpriteIds[record_dst.Sprite4]; break;
						case 4:  record_dst.Sprite5 = (byte)_ial_SpriteIds[record_dst.Sprite5]; break;
						case 5:  record_dst.Sprite6 = (byte)_ial_SpriteIds[record_dst.Sprite6]; break;
						case 6:  record_dst.Sprite7 = (byte)_ial_SpriteIds[record_dst.Sprite7]; break;
						case 7:  record_dst.Sprite8 = (byte)_ial_SpriteIds[record_dst.Sprite8]; break;
					}
				}
	
				if (record_dst.DieTile != 0)
				{
					addPart(record_dst.DieTile, _ial_PartsList.Count);
				}
	
				if (record_dst.Alt_MCD != 0)
				{
					addPart(record_dst.Alt_MCD, _ial_PartsList.Count);
				}
			}
		}

		/// <summary>
		/// Iterates over the final Parts-array and changes vals/refs for any
		/// dead-parts or altr-parts.
		/// </summary>
		/// <param name="idStart"></param>
		private void ChangeRefs(int idStart)
		{
			int id_src, id_dst;

			for (int i = idStart; i != Parts.Length; ++i)
			{
				if ((id_src = Parts[i].Record.DieTile) != 0)
				{
					Parts[i].Record.DieTile = (byte)(id_dst = _ial_PartIds[id_src]);
					Parts[i].Dead = Parts[id_dst];
				}

				if ((id_src = Parts[i].Record.Alt_MCD) != 0)
				{
					Parts[i].Record.Alt_MCD = (byte)(id_dst = _ial_PartIds[id_src]);
					Parts[i].Altr = Parts[id_dst];
				}
			}
		}


		/// <summary>
		/// A special insert-operation via the CopyPanel. Selects the last
		/// tilepart and inserts the CopyPanel's selected tileparts as well as
		/// those parts' subparts.
		/// Called by TerrainPanel_copy.OnInsertAfterLastClick().
		/// </summary>
		internal void InsertAfterLast()
		{
			_ial_PartsList.Clear();
			_ial_PartIds  .Clear();
			_ial_SpriteIds.Clear();

			SelId = Parts.Length - 1;

			int id = SelId + 1;

			for (int i = 0; i != Parts.Length; ++i) // add parts that already exist ->
			{
				_ial_PartsList.Add(Parts[i]);
			}

			for (int i = 0; i != _partsCopied.Count; ++i) // add parts that have been selected ->
			{
				addPart(_partsCopied[i].TerId, _ial_PartsList.Count);
			}


			_bypassScrollZero = true;
			_f.Parts = _ial_PartsList.ToArray();

			ChangeRefs(id);

			SubIds.Clear();
			for (int i = id; i != Parts.Length - 1; ++i)
				SubIds.Add(i);

			SelId = Parts.Length - 1;

			_f.Changed = CacheLoad.Changed(_f.Parts);
		}


		/// <summary>
		/// Deletes a currently selected part 'SelId' and any sub-selected parts
		/// 'SubIds'.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnDeleteClick(object sender, EventArgs e)
		{
			if (SelId != -1)
			{
				var array = new Tilepart[Parts.Length - (1 + SubIds.Count)]; // ie. SelId + SubIds

				for (int i = 0, j = 0; i != Parts.Length; ++i)
				{
					if (i == SelId || SubIds.Contains(i))
					{
						++j;
					}
					else
					{
						array[i - j] = Parts[i];
						array[i - j].TerId = i - j;
					}
				}

				var sels = new HashSet<int>(SubIds);
				sels.Add(SelId);

				SubIds.Clear();
				SelId = -1;

				_bypassScrollZero = true;
				_f.Parts = array;

				foreach (var sel in sels)
				{
					if (sel != 0)
						ClearRefs(sel); // TODO: figure out how to keep refs OnCut
				}

				UpdateRefs();

				_f.Changed = CacheLoad.Changed(_f.Parts);
			}
		}

		/// <summary>
		/// Nulls all dead- and altr-refs to a given part-id.
		/// </summary>
		/// <param name="id"></param>
		private void ClearRefs(int id)
		{
			Tilepart part;
			McdRecord record;

			for (int i = 0; i != Parts.Length; ++i)
			{
				record = (part = Parts[i]).Record;

				if (record.DieTile == id)
					part.Dead = null;

				if (record.Alt_MCD == id)
					part.Altr = null;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnFileClick(object sender, EventArgs e)
		{
			if (Parts != null)
			{}
		}

		/// <summary>
		/// Swaps a part with the part to its left.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnSwapLeftClick(object sender, EventArgs e)
		{
			if (SelId > 0)
			{
				var array = new Tilepart[Parts.Length];

				int id = SelId;
				for (int i = 0; i != id - 1; ++i)
					array[i] = Parts[i];

				array[id - 1] = Parts[id];
				array[id - 1].TerId = id - 1;

				array[id] = Parts[id - 1];
				array[id].TerId = id;

				for (int i = id + 1; i != Parts.Length; ++i)
					array[i] = Parts[i];

				_bypassScrollZero = true;
				_f.Parts = array;

				SwapRefs(id, id - 1);

				SelId = id - 1; // does refresh.

				_f.Changed = CacheLoad.Changed(_f.Parts);
			}
		}

		/// <summary>
		/// Swaps a part with the part to its right.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnSwapRightClick(object sender, EventArgs e)
		{
			if (SelId != -1 && SelId != Parts.Length - 1)
			{
				var array = new Tilepart[Parts.Length];

				int id = SelId;
				for (int i = 0; i != id; ++i)
					array[i] = Parts[i];

				array[id] = Parts[id + 1];
				array[id].TerId = id;

				array[id + 1] = Parts[id];
				array[id + 1].TerId = id + 1;

				for (int i = id + 2; i != Parts.Length; ++i)
					array[i] = Parts[i];

				_bypassScrollZero = true;
				_f.Parts = array;

				SwapRefs(id, id + 1);

				SelId = id + 1; // does refresh.

				_f.Changed = CacheLoad.Changed(_f.Parts);
			}
		}

		/// <summary>
		/// Swaps two dead- and altr-refs. Any refs that point to 'a' point to
		/// 'b' and vice versa.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		private void SwapRefs(int a, int b)
		{
			Tilepart part;
			McdRecord record;

			int id;

			for (int i = 0; i != Parts.Length; ++i)
			{
				record = (part = Parts[i]).Record;

				if ((id = record.DieTile) != 0)
				{
					if (id == a)
					{
						record.DieTile = (byte)b;
						part.Dead = Parts[b];
					}
					else if (id == b)
					{
						record.DieTile = (byte)a;
						part.Dead = Parts[a];
					}
				}

				if ((id = record.Alt_MCD) != 0)
				{
					if (id == a)
					{
						record.Alt_MCD = (byte)b;
						part.Altr = Parts[b];
					}
					else if (id == b)
					{
						record.Alt_MCD = (byte)a;
						part.Altr = Parts[a];
					}
				}
			}
		}
		#endregion Events (context)


		#region Methods
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
				// IMPORTANT: The conditions shall be synched w/ OnOpening_Context().
				//
				// NOTE: Escape for deselect all is handled by the caller: McdviewF.OnKeyDown().

				case Keys.D:									// add
					if (!e.Control)
						OnAddClick(null, EventArgs.Empty);
					break;

				case Keys.OemMinus: // drugs ...
				case Keys.Subtract:								// swap left
					OnSwapLeftClick(null, EventArgs.Empty);
					break;

				case Keys.Oemplus: // drugs ...
				case Keys.Add:									// swap right
					OnSwapRightClick(null, EventArgs.Empty);
					break;

				case Keys.F:									// append file
					OnFileClick(null, EventArgs.Empty);
					break;
			}
		}
		#endregion Methods
	}
}
