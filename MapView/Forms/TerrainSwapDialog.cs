using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using DSShared;

using XCom;


namespace MapView
{
	/// <summary>
	/// A dialog for changing the order of terrains in a
	/// <c><see cref="MapFile"/></c>.
	/// </summary>
	internal sealed class TerrainSwapDialog
		:
			Form
	{
		#region Fields (static)
		private static int _x = -1;
		private static int _y;
		#endregion Fields (static)


		#region Fields
		MapFile _file;
		Descriptor _descriptor;

		Dictionary<int, Tuple<string,string>> _terrains = new Dictionary<int, Tuple<string,string>>();

		int _terCount;

		int[] _partCounts;
		int[] _order;
		#endregion Fields


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="file"></param>
		internal TerrainSwapDialog(MapFile file)
		{
			_descriptor = (_file = file).Descriptor;

			foreach (var terrain in _descriptor.Terrains)
			{
				_terrains.Add(terrain.Key, terrain.Value);
			}

			_terCount = _descriptor.Terrains.Count;

			_partCounts = new int[_terCount];
			for (int i = 0; i != _terCount; ++i)
				_partCounts[i] = _file.PartCounts[i];

			_order = new int[_terCount];
			for (int i = 0; i != _terCount; ++i)
				_order[i] = i;


			InitializeComponent();

			la_head.Text = "This operation makes changes to and re-saves both the Maptree"
						 + " and the current Map. The tileset reloads if accepted.";

			var loc = new Point(_x,_y);
			bool isInsideBounds = false;
			if (_x > -1)
			{
				foreach (var screen in Screen.AllScreens)
				{
					if (isInsideBounds = screen.Bounds.Contains(loc)
									  && screen.Bounds.Contains(loc + Size))
					{
						break;
					}
				}
			}

			if (isInsideBounds)
				Location = loc;
			else
				CenterToScreen();

			bu_cancel.Select();


			// list the current terrains ->
			lb_allocated.BeginUpdate();
			for (int i = 0; i != _terCount; ++i)
			{
				lb_allocated.Items.Add(new tce(_terrains[i], _partCounts[i], _order[i]));
			}
			lb_allocated.EndUpdate();
			lb_allocated.SelectedIndex = 0;
		}
		#endregion cTor


		#region Events (override)
		/// <summary>
		/// Stores this dialog's current location.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			if (!RegistryInfo.FastClose(e.CloseReason))
			{
				_x = Location.X;
				_y = Location.Y;
			}
			base.OnFormClosing(e);
		}

		/// <summary>
		/// Overrides the <c>Paint</c> handler. Draws a black line down the left
		/// border.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{
			e.Graphics.DrawLine(Pens.Black, 0,0, 0, ClientSize.Height - 1);
		}
		#endregion Events (override)


		#region Events
		/// <summary>
		/// Paints a left/top border on the head-label.
		/// </summary>
		/// <param name="sender"><c><see cref="la_head"/></c></param>
		/// <param name="e"></param>
		private void OnPaintHead(object sender, PaintEventArgs e)
		{
			e.Graphics.DrawLine(Pens.Black, 0,0, 0, ClientSize.Height - 1);
			e.Graphics.DrawLine(Pens.Black, 1,0, ClientSize.Width - 1, 0);
		}

		/// <summary>
		/// Enables/disables buttons when the selected index in
		/// <c><see cref="lb_allocated"/></c> changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks>The index does not have to change for the
		/// <c>SelectedIndexChanged</c> event to fire. Selecting or reselecting
		/// it is enough whether by click or assignment.</remarks>
		private void OnTerrainlistIndexChanged(object sender, EventArgs e)
		{
			int id = lb_allocated.SelectedIndex;
			bu_up  .Enabled = (id != 0);
			bu_down.Enabled = (id != _terCount - 1);

			bool enabled = false;
			for (int i = 0; i != _terCount; ++i)
			{
				if (_terrains[i].Item1 != _descriptor.Terrains[i].Item1)
				{
					enabled = true;
					break;
				}
			}
			bu_ok.Enabled = enabled;
		}

		/// <summary>
		/// Copies the order of the terrains in
		/// <c><see cref="lb_allocated"/></c> to the Mapfile's
		/// <c><see cref="Descriptor"/></c>. Also re-arranges the <c>SetIds</c>
		/// of the Mapfile's <c>Parts</c> list to correspond. The <c>SetIds</c>
		/// of the <c>Parts</c> list are used when writing/saving the Mapfile.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks>The Okay button shall be disabled if there is no difference
		/// between the terrains in <c>lb_allocated</c> and the terrains in the
		/// Map's <c>Descriptor</c>.</remarks>
		private void OnOkayClick(object sender, EventArgs e)
		{
			var parts = _file.Parts;
			for (int i = 0; i != parts.Count; ++i)
			{
				ShiftId(parts[i]); // NOTE: Do shifting before terrain re-arranging.
			}

			_descriptor.Terrains.Clear();

			tce tce;
			for (int i = 0; i != _terCount; ++i)
			{
				tce = lb_allocated.Items[i] as tce;
				_descriptor.Terrains.Add(i, new Tuple<string,string>(tce.Terrain, tce.Basepath));
			}
		}

		/// <summary>
		/// Shifts the <c>SetId</c> of a specified <c><see cref="Tilepart"/></c>.
		/// </summary>
		/// <param name="part"></param>
		/// <remarks>Helper for
		/// <c><see cref="OnOkayClick()">OnOkayClick()</see></c></remarks>
		private void ShiftId(Tilepart part)
		{
			Tuple<string,string> terrain = _file.GetTerrain(part);
			string label    = terrain.Item1;
			string basepath = terrain.Item2;

			var partCounts = new int[_terCount];

			int order0 = -1, order1 = -1; tce tce;
			for (int i = 0; i != _terCount; ++i)
			{
				tce = lb_allocated.Items[i] as tce;
				if (label == tce.Terrain && basepath == tce.Basepath)
				{
					order0 = tce.Order;
					order1 = i;
				}
				partCounts[i] = tce.PartCount;
			}

			// NOTE: order0 and order1 shall be valid list-ids.

			for (int i = 0; i != _terCount; ++i) // subtract part-ids until the terrain becomes the 'first' terrain ->
			{
				if (order0 == i) break;
				part.SetId -= _partCounts[i];
			}

			for (int i = 0; i != _terCount; ++i) // add part-ids until the terrain goes to its 'final' position in the terrainset.
			{
				if (order1 == i) break;
				part.SetId += partCounts[i];
			}
		}
		#endregion Events


		#region Methods
		/// <summary>
		/// Shifts a terrain up in the terrains-list.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnTerrainUpClick(object sender, EventArgs e)
		{
			ShiftTerrainEntry(-1);
		}

		/// <summary>
		/// Shifts a terrain down in the terrains-list.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnTerrainDownClick(object sender, EventArgs e)
		{
			ShiftTerrainEntry(+1);
		}

		/// <summary>
		/// Shifts a terrain up/down in the terrains-list.
		/// </summary>
		/// <param name="dir"></param>
		/// <remarks>Helper for
		/// <c><see cref="OnTerrainUpClick()">OnTerrainUpClick()</see></c> and
		/// <c><see cref="OnTerrainDownClick()">OnTerrainDownClick()</see></c></remarks>
		private void ShiftTerrainEntry(int dir)
		{
			int id0 = lb_allocated.SelectedIndex;
			int id1 = id0 + dir;

			Tuple<string,string> terrain = _terrains[id1];
			_terrains[id1] = _terrains[id0];
			_terrains[id0] = terrain;

			lb_allocated.BeginUpdate();
			object it = lb_allocated.Items[id1];
			lb_allocated.Items[id1] = lb_allocated.Items[id0];
			lb_allocated.Items[id0] = it;
			lb_allocated.EndUpdate();

			lb_allocated.SelectedIndex = id1;
			lb_allocated.Select();
		}
		#endregion Methods


		#region Designer
		Label la_head;
		ListBox lb_allocated;
		Button bu_up;
		Button bu_down;
		Label la_note;
		Button bu_cancel;
		Button bu_ok;

		/// <summary>
		/// borkity bork.
		/// </summary>
		private void InitializeComponent()
		{
			this.la_head = new System.Windows.Forms.Label();
			this.lb_allocated = new System.Windows.Forms.ListBox();
			this.bu_up = new System.Windows.Forms.Button();
			this.bu_down = new System.Windows.Forms.Button();
			this.la_note = new System.Windows.Forms.Label();
			this.bu_cancel = new System.Windows.Forms.Button();
			this.bu_ok = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// la_head
			// 
			this.la_head.BackColor = System.Drawing.Color.Lavender;
			this.la_head.Dock = System.Windows.Forms.DockStyle.Top;
			this.la_head.Location = new System.Drawing.Point(0, 0);
			this.la_head.Margin = new System.Windows.Forms.Padding(0);
			this.la_head.Name = "la_head";
			this.la_head.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.la_head.Size = new System.Drawing.Size(292, 43);
			this.la_head.TabIndex = 0;
			this.la_head.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.la_head.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPaintHead);
			// 
			// lb_allocated
			// 
			this.lb_allocated.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.lb_allocated.FormattingEnabled = true;
			this.lb_allocated.IntegralHeight = false;
			this.lb_allocated.ItemHeight = 12;
			this.lb_allocated.Location = new System.Drawing.Point(0, 43);
			this.lb_allocated.Margin = new System.Windows.Forms.Padding(0);
			this.lb_allocated.Name = "lb_allocated";
			this.lb_allocated.Size = new System.Drawing.Size(230, 177);
			this.lb_allocated.TabIndex = 1;
			this.lb_allocated.SelectedIndexChanged += new System.EventHandler(this.OnTerrainlistIndexChanged);
			// 
			// bu_up
			// 
			this.bu_up.Location = new System.Drawing.Point(234, 47);
			this.bu_up.Margin = new System.Windows.Forms.Padding(0);
			this.bu_up.Name = "bu_up";
			this.bu_up.Size = new System.Drawing.Size(55, 25);
			this.bu_up.TabIndex = 2;
			this.bu_up.Text = "up";
			this.bu_up.UseVisualStyleBackColor = true;
			this.bu_up.Click += new System.EventHandler(this.OnTerrainUpClick);
			// 
			// bu_down
			// 
			this.bu_down.Location = new System.Drawing.Point(234, 77);
			this.bu_down.Margin = new System.Windows.Forms.Padding(0);
			this.bu_down.Name = "bu_down";
			this.bu_down.Size = new System.Drawing.Size(55, 25);
			this.bu_down.TabIndex = 3;
			this.bu_down.Text = "down";
			this.bu_down.UseVisualStyleBackColor = true;
			this.bu_down.Click += new System.EventHandler(this.OnTerrainDownClick);
			// 
			// la_note
			// 
			this.la_note.ForeColor = System.Drawing.Color.PaleVioletRed;
			this.la_note.Location = new System.Drawing.Point(70, 225);
			this.la_note.Margin = new System.Windows.Forms.Padding(0);
			this.la_note.Name = "la_note";
			this.la_note.Size = new System.Drawing.Size(214, 15);
			this.la_note.TabIndex = 4;
			this.la_note.Text = "REMINDER: there is no Undo function";
			this.la_note.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// bu_cancel
			// 
			this.bu_cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.bu_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.bu_cancel.Location = new System.Drawing.Point(9, 250);
			this.bu_cancel.Margin = new System.Windows.Forms.Padding(0);
			this.bu_cancel.Name = "bu_cancel";
			this.bu_cancel.Size = new System.Drawing.Size(85, 20);
			this.bu_cancel.TabIndex = 5;
			this.bu_cancel.Text = "Cancel";
			this.bu_cancel.UseVisualStyleBackColor = true;
			// 
			// bu_ok
			// 
			this.bu_ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.bu_ok.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.bu_ok.Enabled = false;
			this.bu_ok.Location = new System.Drawing.Point(189, 245);
			this.bu_ok.Margin = new System.Windows.Forms.Padding(0);
			this.bu_ok.Name = "bu_ok";
			this.bu_ok.Size = new System.Drawing.Size(95, 25);
			this.bu_ok.TabIndex = 6;
			this.bu_ok.Text = "Ok";
			this.bu_ok.UseVisualStyleBackColor = true;
			this.bu_ok.Click += new System.EventHandler(this.OnOkayClick);
			// 
			// TerrainSwapDialog
			// 
			this.AcceptButton = this.bu_ok;
			this.CancelButton = this.bu_cancel;
			this.ClientSize = new System.Drawing.Size(292, 274);
			this.Controls.Add(this.la_note);
			this.Controls.Add(this.la_head);
			this.Controls.Add(this.lb_allocated);
			this.Controls.Add(this.bu_up);
			this.Controls.Add(this.bu_down);
			this.Controls.Add(this.bu_cancel);
			this.Controls.Add(this.bu_ok);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TerrainSwapDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Terrain Swap";
			this.ResumeLayout(false);

		}
		#endregion Designer
	}



	/// <summary>
	/// An object for parsing out a terrain-string to show in the terrain-
	/// listbox while retaining a reference to other stuff about the terrain.
	/// </summary>
	internal sealed class tce // TerrainCountEntry
	{
		internal string Terrain
		{ get; private set; }

		internal string Basepath
		{ get; private set; }

		internal int PartCount
		{ get; private set; }

		internal int Order
		{ get; private set; }

		internal tce(Tuple<string,string> terrain, int partCount, int order)
		{
			Terrain   = terrain.Item1;
			Basepath  = terrain.Item2;
			PartCount = partCount;
			Order     = order;
		}

		/// <summary>
		/// Required for <c>lb_allocated.DisplayMember</c> to work.
		/// </summary>
		/// <returns>the <c><see cref="Terrain"/></c> string</returns>
		public override string ToString()
		{
			return Terrain;
		}
	}
}
