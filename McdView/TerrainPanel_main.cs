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
			var itAdd      = new MenuItem("add",             OnAddClick);			// d key
			var itAddRange = new MenuItem("add range ...",   OnAddRangeClick);		// Ctrl+d key

			var itSep0     = new MenuItem("-");

			var itCut      = new MenuItem("cut",             OnCutClick);			// Ctrl+x key
			var itCopy     = new MenuItem("copy",            OnCopyClick);			// Ctrl+c key
			var itInsert   = new MenuItem("insert after",    OnInsertClick);		// Ctrl+v key
			var itDelete   = new MenuItem("delete",          OnDeleteClick);		// Delete key

			var itSep1     = new MenuItem("-");

			var itFile     = new MenuItem("append file ...", OnFileClick);			// f key

			var itSep2     = new MenuItem("-");

			var itLeft     = new MenuItem("swap left",       OnSwapLeftClick);		// - key
			var itRight    = new MenuItem("swap right",      OnSwapRightClick);		// + key

			var itSep3     = new MenuItem("-");

			var itSelect   = new MenuItem("select all",      OnSelectAllClick);		// Ctrl+a key
			var itDeselect = new MenuItem("deselect all",    OnDeselectAllClick);	// Esc

			Context = new ContextMenu();
			Context.MenuItems.AddRange(new []
										{
											itAdd,		//  0
											itAddRange,	//  1
											itSep0,		//  2
											itCut,		//  3
											itCopy,		//  4
											itInsert,	//  5
											itDelete,	//  6
											itSep1,		//  7
											itFile,		//  8
											itSep2,		//  9
											itLeft,		// 10
											itRight,	// 11
											itSep3,		// 12
											itSelect,	// 13
											itDeselect	// 14
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
			bool parts = (Parts != null);
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
			Context.MenuItems[14].Enabled = selid;								// deselect
		}

//		/// <summary>
//		/// Closes the contextmenu.
//		/// </summary>
//		/// <param name="sender"></param>
//		/// <param name="e"></param>
//		private void OnIdClick(object sender, EventArgs e)
//		{
//			Context.Dispose();
//		}

		/// <summary>
		/// Adds a blank part to the parts-array.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnAddClick(object sender, EventArgs e)
		{
			var array = new Tilepart[Parts.Length + 1];

			int id = SelId + 1;
			for (int i = 0; i != id; ++i)
				array[i] = Parts[i];

			McdRecord record = McdRecordFactory.CreateRecord();

			array[id] = new Tilepart(
									id,
									Spriteset,
									record);
			array[id].Dead      =
			array[id].Alternate = null;

			for (int i = id + 1; i != array.Length; ++i)
			{
				array[i] = Parts[i - 1];
				array[i].TerId = i; // not used in McdView but keep things consistent ....
			}

			_bypassScrollZero = true;
			_f.Parts = array; // assign back to 'Parts' via McdviewF

			ShiftRefs(id, 1);

			SubIds.Clear();
			SelId = id;

			_f.Changed = CacheLoad.Changed(_f.Parts);
		}

		internal static int _add;
		private void OnAddRangeClick(object sender, EventArgs e)
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

						McdRecord record;
						int j = i + _add;
						for (; i != j; ++i)
						{
							record = McdRecordFactory.CreateRecord();

							array[i] = new Tilepart(
												i,
												Spriteset,
												record);
							array[i].Dead =
							array[i].Alternate = null;
						}

						for (; i != length; ++i)
						{
							array[i] = Parts[i - _add];
							array[i].TerId = i;
						}

						_bypassScrollZero = true;
						_f.Parts = array;

						ShiftRefs(id, _add);

						SubIds.Clear();
						SelId = id;

						_f.Changed = CacheLoad.Changed(_f.Parts);
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
			OnCopyClick(  null, EventArgs.Empty);
			OnDeleteClick(null, EventArgs.Empty);
		}

		/// <summary>
		/// Inserts the copy-array into the parts-array after the currently
		/// selected part or at the start of the array if there is no selected
		/// part.
		/// NOTE: If inserted part(s)' refs are less than the insertion point
		/// then the refs are okay; if the refs are equal to or greater than the
		/// insertion point then the refs need to be advanced. But only if the
		/// inserted parts are from the same recordset; if not then the refs
		/// shall be deleted. This behavior is warranteed only on the first
		/// insert of copied parts; the refs can still go wonky on 2+ inserts.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal void OnInsertClick(object sender, EventArgs e)
		{
			bool isTer = (_copylabel == _f.Label); // null refs if the terrain-labels differ

			int id = SelId + 1;

			var array = new Tilepart[Parts.Length + _copyparts.Count];

			for (int i = 0, j = 0; i != array.Length; ++i, ++j)
			{
				if (i == id)
				{
					for (int pos = 0; pos != _copyparts.Count; ++pos, ++i)
					{
						array[i] = _copyparts[pos].Clone(_f.Spriteset);
						array[i].TerId = i;

						if (!isTer)
						{
							array[i].Record.DieTile = (byte)0;
							array[i].Dead = null;

							array[i].Record.Alt_MCD = (byte)0;
							array[i].Alternate = null;
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

			ShiftRefs(id, _copyparts.Count);

			SubIds.Clear();
			SelId = id;

			_f.Changed = CacheLoad.Changed(_f.Parts);
		}

		/// <summary>
		/// Updates refs when parts are added.
		/// Shifts references to death- and alternate-parts by a given amount
		/// starting at references at or greater than a given ID.
		/// </summary>
		/// <param name="start"></param>
		/// <param name="shift"></param>
		private void ShiftRefs(int start, int shift)
		{
			if (start + shift != Parts.Length) // don't bother shifting any refs if the parts are inserted at the end of the array
			{
				Tilepart part;
				McdRecord record;

				int id;

				for (int i = 0; i != Parts.Length; ++i)
				{
					part = Parts[i];
					record = part.Record;

					if ((id = record.DieTile) != 0 && id >= start)
					{
						if ((id = record.DieTile + shift) < Parts.Length)
						{
							record.DieTile = (byte)id;
							part.Dead = Parts[id];
						}
						else
						{
							record.DieTile = (byte)0;
							part.Dead = null;
						}
					}

					if ((id = record.Alt_MCD) != 0 && id >= start)
					{
						if ((id = record.Alt_MCD + shift) < Parts.Length)
						{
							record.Alt_MCD = (byte)id;
							part.Alternate = Parts[id];
						}
						else
						{
							record.Alt_MCD = (byte)0;
							part.Alternate = null;
						}
					}
				}
			}
		}

		/// <summary>
		/// Deletes a currently selected part 'SelId' and any sub-selected parts
		/// 'SubIds'.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnDeleteClick(object sender, EventArgs e)
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

			SubIds.Add(SelId);
			var sels = new List<int>(SubIds);

			SubIds.Clear();
			SelId = -1;

			_bypassScrollZero = true;
			_f.Parts = array;

			for (int i = 0; i != sels.Count; ++i)
				ClearRefs(sels[i]);

			UpdateRefs(sels);

			_f.Changed = CacheLoad.Changed(_f.Parts);
		}

		/// <summary>
		/// Nulls all death- and alternate-references to a given part.
		/// </summary>
		/// <param name="id"></param>
		private void ClearRefs(int id)
		{
			if (id != 0) // ie. DeathId or AlternateId is not already null-part.
			{
				Tilepart part;
				McdRecord record;

				for (int i = 0; i != Parts.Length; ++i)
				{
					part = Parts[i];
					record = part.Record;

					if (record.DieTile == id)
					{
						record.DieTile = (byte)0;
						part.Dead = null;
					}

					if (record.Alt_MCD == id)
					{
						record.Alt_MCD = (byte)0;
						part.Alternate = null;
					}
				}
			}
		}

		/// <summary>
		/// Updates refs when a part or parts get deleted.
		/// </summary>
		/// <param name ="sels">a list of IDs that got deleted</param>
		private void UpdateRefs(IList<int> sels)
		{
			Tilepart part;
			McdRecord record;

			int pos, id;

			for (int i = 0; i != Parts.Length; ++i)
			{
				part = Parts[i];
				record = part.Record;

				pos = sels.Count - 1; // start with the last entry in 'sels'

				if ((id = record.DieTile) != 0)// && (pos = sels.FindIndex(val => val == @ref)) != -1)
				{
					while (pos != -1 && id > sels[pos--])
						--id;

					if (id != record.DieTile)
					{
						record.DieTile = (byte)id;
						part.Dead = Parts[id];
					}
				}

				pos = sels.Count - 1;

				if ((id = record.Alt_MCD) != 0)// && (pos = sels.FindIndex(val => val == @ref)) != -1)
				{
					while (pos != -1 && id > sels[pos--])
						--id;

					if (id != record.Alt_MCD)
					{
						record.Alt_MCD = (byte)id;
						part.Alternate = Parts[id];
					}
				}
			}
		}

		private void OnFileClick(object sender, EventArgs e)
		{
		}

		/// <summary>
		/// Swaps a part with the part to its left.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnSwapLeftClick(object sender, EventArgs e)
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

		/// <summary>
		/// Swaps a part with the part to its right.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnSwapRightClick(object sender, EventArgs e)
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

		/// <summary>
		/// Swaps two death- and alternate-references. Any references that point
		/// to 'a' point to 'b' and vice versa.
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
				part   = Parts[i];
				record = part.Record;

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
						part.Alternate = Parts[b];
					}
					else if (id == b)
					{
						record.Alt_MCD = (byte)a;
						part.Alternate = Parts[a];
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
				// IMPORTANT: The conditions shall be synched w/ OnPopup_Context().

				case Keys.D:
					if (Parts != null)
					{
						if (!e.Control)											// add
							OnAddClick(null, EventArgs.Empty);
						else													// add range
							OnAddRangeClick(null, EventArgs.Empty);
					}
					break;


				case Keys.X:													// cut
					if (e.Control && SelId != -1)
						OnCutClick(null, EventArgs.Empty);
					break;

				case Keys.C:													// copy
					if (e.Control && SelId != -1)
						OnCopyClick(null, EventArgs.Empty);
					break;

				case Keys.V:													// insert
					if (e.Control && Parts != null && _copyparts.Count != 0)
						OnInsertClick(null, EventArgs.Empty);
					break;

				case Keys.Delete:												// delete
					if (SelId != -1)
						OnDeleteClick(null, EventArgs.Empty);
					break;


				case Keys.OemMinus: // drugs ...
				case Keys.Subtract:												// swap left
					if (SelId > 0)
						OnSwapLeftClick(null, EventArgs.Empty);
					break;

				case Keys.Oemplus: // drugs ...
				case Keys.Add:													// swap right
					if (SelId != -1 && SelId != Parts.Length - 1)
						OnSwapRightClick(null, EventArgs.Empty);
					break;


				case Keys.A:													// select all
					if (e.Control && Parts != null && Parts.Length != 0)
						OnSelectAllClick(null, EventArgs.Empty);
					break;

				// NOTE: Escape for deselect all is handled by the caller: McdviewF.OnKeyDown().

				case Keys.F:													// append file
					if (Parts != null)
						OnFileClick(null, EventArgs.Empty);
					break;
			}
		}
		#endregion Methods
	}
}
