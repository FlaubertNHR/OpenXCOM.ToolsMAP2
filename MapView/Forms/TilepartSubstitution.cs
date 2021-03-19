using System;
using System.Drawing;
using System.Windows.Forms;

using DSShared;

using XCom;


namespace MapView
{
	internal sealed class TilepartSubstitution
		:
			Form
	{
		internal enum RadioSelected
		{ Clear, Desti, Shift }


		#region Fields (static)
		private static int _x = -1;
		private static int _y;

		internal static int src0  = Int32.MaxValue;
		internal static int src1  = Int32.MaxValue;
		internal static int dst   = Int32.MaxValue;
		internal static int shift = Int32.MaxValue;

		internal static RadioSelected rb_selected = RadioSelected.Clear;
		#endregion Fields (static)


		#region Fields
		/// <summary>
		/// Holds the value of the currently active textbox. The value in the
		/// textbox will revert to '_text' on user-input error(s).
		/// </summary>
		private string _text;

		/// <summary>
		/// Bypasses eventhandler code when the dialog starts.
		/// </summary>
		private bool _init;

		/// <summary>
		/// The highest tilepart-id allowed as output.
		/// </summary>
		private int _highid;
		#endregion Fields


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="file"></param>
		internal TilepartSubstitution(MapFile file)
		{
			InitializeComponent();

			_highid = Math.Min(file.Parts.Count - 1, MapFile.MaxTerrainId);

			la_head.Text = "Change all tileparts between setIds"
						 + Environment.NewLine
						 + "inclusive - if stop is blank use start id"
						 + Environment.NewLine + Environment.NewLine
						 + "Highest placeable id in the terrainset = " + _highid
						 + Environment.NewLine
						 + "Highest (valid) tilepart id detected = " + HighId(file);

			tb_Src0.BackColor =
			tb_Src1.BackColor = Color.Wheat;

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


			_init = true;
			rb_clear.Select(); // prep radiobutton controls for Tab switching ->
			rb_dst  .Select(); // do this before applying 'rb_selected'
			rb_shift.Select(); // and yes the '_init' thing is req'd.

			if (src0  != Int32.MaxValue) tb_Src0 .Text = src0 .ToString();
			if (src1  != Int32.MaxValue) tb_Src1 .Text = src1 .ToString();
			if (dst   != Int32.MaxValue) tb_dst  .Text = dst  .ToString();
			if (shift != Int32.MaxValue) tb_shift.Text = shift.ToString();
			_init = false;

			switch (rb_selected) // fire rb_CheckedChanged()
			{
				case RadioSelected.Clear: rb_clear.Checked = true; break;
				case RadioSelected.Desti: rb_dst  .Checked = true; break;
				case RadioSelected.Shift: ForceBlasterInitShift(); break; //rb_shift.Checked = true;
			}

			tb_Src0.Select(); // set '_text'
			tb_Src0.SelectionStart = tb_Src0.Text.Length;
		}

		/// <summary>
		/// Gets the highest valid setId placed in the current tileset.
		/// </summary>
		/// <returns></returns>
		private static int HighId(MapFile file)
		{
			int id = -1;

			MapTile tile;
			Tilepart part;

			int cols = file.MapSize.Cols;
			int rows = file.MapSize.Rows;
			int levs = file.MapSize.Levs;

			for (int lev = 0; lev != levs; ++lev)
			for (int row = 0; row != rows; ++row)
			for (int col = 0; col != cols; ++col)
			{
				if (!(tile = file[col, row, lev]).Vacant)
				{
					if (  (part = tile.Floor)   != null
						&& part.Record.PartType != PartType.Invalid // bypass crippled parts
						&& part.SetId > id)
					{
						id = part.SetId;
					}

					if (  (part = tile.West)    != null
						&& part.Record.PartType != PartType.Invalid
						&& part.SetId > id)
					{
						id = part.SetId;
					}

					if (  (part = tile.North)   != null
						&& part.Record.PartType != PartType.Invalid
						&& part.SetId > id)
					{
						id = part.SetId;
					}

					if (  (part = tile.Content) != null
						&& part.Record.PartType != PartType.Invalid
						&& part.SetId > id)
					{
						id = part.SetId;
					}
				}
			}
			return id;
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
		/// Overrides the keydown event. Closes this dialog on [Ctrl+u].
		/// </summary>
		/// <param name="e"></param>
		/// <remarks>Requires KeyPreview true.</remarks>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.KeyData == (Keys.Control | Keys.U))
				Close();
		}

		/// <summary>
		/// Overrides the paint event.
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
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnPaintHead(object sender, PaintEventArgs e)
		{
			e.Graphics.DrawLine(Pens.Black, 0,0, 0, ClientSize.Height - 1);
			e.Graphics.DrawLine(Pens.Black, 1,0, ClientSize.Width - 1, 0);
		}

		/// <summary>
		/// holy fc genius
		/// https://stackoverflow.com/questions/24997658/tab-index-is-not-working-on-radio-buttons#answer-31063124
		/// @note The radiobuttons need to be selected (once) before this works.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void rb_TabStopChanged(object sender, EventArgs e)
		{
			(sender as Control).TabStop = true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void rb_CheckedChanged(object sender, EventArgs e)
		{
			if (!_init)
			{
				var rb = sender as RadioButton;
				if (rb.Checked)
				{
					if (rb == rb_clear)
					{
						rb_selected = RadioSelected.Clear;

						tb_dst  .Enabled =
						tb_shift.Enabled = false;

						tb_dst  .BackColor =
						tb_shift.BackColor = SystemColors.Control;
					}
					else if (rb == rb_dst)
					{
						rb_selected = RadioSelected.Desti;

						tb_dst  .Enabled = true;
						tb_shift.Enabled = false;

						tb_dst  .BackColor = Color.LightGreen;
						tb_shift.BackColor = SystemColors.Control;
					}
					else //if (rb == rb_shift)
					{
						rb_selected = RadioSelected.Shift;

						tb_dst  .Enabled = false;
						tb_shift.Enabled = true;

						tb_dst  .BackColor = SystemColors.Control;
						tb_shift.BackColor = Color.LightGreen;
					}

					Enable();
				}
			}
		}

		/// <summary>
		/// Lord knows why rb_CheckedChanged() does not fire when trying to init
		/// w/ 'RadioSelected.Shift'.
		/// @note thanks ...
		/// </summary>
		private void ForceBlasterInitShift()
		{
			_init = true; // safety.
			rb_shift.Checked = true; // that doesn't fire rb_CheckedChanged() - it should. But it doesn't ...
			_init = false;

			tb_dst  .Enabled = false;
			tb_shift.Enabled = true;

			tb_dst  .BackColor = SystemColors.Control;
			tb_shift.BackColor = Color.LightGreen;

			Enable();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tb_Activated(object sender, EventArgs e)
		{
			_text = (sender as Control).Text;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tb_TextChanged(object sender, EventArgs e)
		{
			if (!_init)
			{
				bool fail = false;

				var tb = sender as TextBox;
				string text = tb.Text;

				int result = Int32.MaxValue;

				if (!String.IsNullOrEmpty(text)
					&& (!rb_shift.Checked || text != "-"))		// allow shift to be a "-"
				{
					if (!Int32.TryParse(text, out result))		// shall be an integer
					{
						fail = true;
					}
					else if (tb == tb_Src0 || tb == tb_Src1)	// shall be a positive integer
					{
						fail = result < 0;
					}
					else if (tb == tb_dst)						// shall be a positive integer less than or equal to '_highid'
					{
						fail = result < 0
							|| result > _highid;
					}

					if (fail)
					{
						_init = true;
						tb.Text = _text;
						_init = false;

						tb.SelectionStart = _text.Length;
					}
				}

				if (!fail)
				{
					_text = text;
					SetStatics(tb, result);

					Enable();
				}
			}
		}
		#endregion Events


		#region Methods
		/// <summary>
		/// Sets the values of static variables that are used both for
		/// re-insantiation and for MapView to deal with on return w/
		/// 'DialogResult.OK'.
		/// </summary>
		/// <param name="tb"></param>
		/// <param name="result"></param>
		private void SetStatics(object tb, int result)
		{
			if      (tb == tb_Src0) src0  = result;
			else if (tb == tb_Src1) src1  = result;
			else if (tb == tb_dst)  dst   = result;
			else                    shift = result; // (tb == tb_shift)
		}

		/// <summary>
		/// Dis/enables the Ok button.
		/// @note The input values src0/src1 are allowed to exceed the MaxId in
		/// the current terrainset but the output shall not be allowed to exceed
		/// the MaxId.
		/// </summary>
		private void Enable()
		{
			bu_ok.Enabled =  src0 != Int32.MaxValue
						 && (src1 == Int32.MaxValue || src1 >= src0)
						 && (    rb_clear.Checked
							 || (rb_dst  .Checked && dst   != Int32.MaxValue && (dst != src0 || (src1 != Int32.MaxValue && src1 != src0)))
							 || (rb_shift.Checked && shift != Int32.MaxValue && shift != 0
								 && shift + src0 > -1 && ((src1 == Int32.MaxValue && shift + src0 <= _highid)
													   || (src1 != Int32.MaxValue && shift + src1 <= _highid))));
		}
		#endregion Methods



		#region Designer
		private Label la_head;
		private TextBox tb_Src0;
		private Label la_dotdot;
		private TextBox tb_Src1;
		private RadioButton rb_clear;
		private RadioButton rb_dst;
		private TextBox tb_dst;
		private RadioButton rb_shift;
		private TextBox tb_shift;
		private Button bu_cancel;
		private Button bu_ok;
		private Label la_start;
		private Label la_stop;
		private Label la_note;

		/// <summary>
		/// Required method for Designer support - do not modify the contents of
		/// this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.la_head = new System.Windows.Forms.Label();
			this.tb_Src0 = new System.Windows.Forms.TextBox();
			this.tb_dst = new System.Windows.Forms.TextBox();
			this.bu_ok = new System.Windows.Forms.Button();
			this.la_dotdot = new System.Windows.Forms.Label();
			this.tb_Src1 = new System.Windows.Forms.TextBox();
			this.tb_shift = new System.Windows.Forms.TextBox();
			this.rb_clear = new System.Windows.Forms.RadioButton();
			this.rb_dst = new System.Windows.Forms.RadioButton();
			this.rb_shift = new System.Windows.Forms.RadioButton();
			this.bu_cancel = new System.Windows.Forms.Button();
			this.la_start = new System.Windows.Forms.Label();
			this.la_stop = new System.Windows.Forms.Label();
			this.la_note = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// la_head
			// 
			this.la_head.BackColor = System.Drawing.Color.Lavender;
			this.la_head.Dock = System.Windows.Forms.DockStyle.Top;
			this.la_head.Location = new System.Drawing.Point(0, 0);
			this.la_head.Margin = new System.Windows.Forms.Padding(0);
			this.la_head.Name = "la_head";
			this.la_head.Size = new System.Drawing.Size(274, 75);
			this.la_head.TabIndex = 0;
			this.la_head.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.la_head.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPaintHead);
			// 
			// tb_Src0
			// 
			this.tb_Src0.HideSelection = false;
			this.tb_Src0.Location = new System.Drawing.Point(50, 80);
			this.tb_Src0.Margin = new System.Windows.Forms.Padding(0);
			this.tb_Src0.Name = "tb_Src0";
			this.tb_Src0.Size = new System.Drawing.Size(50, 19);
			this.tb_Src0.TabIndex = 2;
			this.tb_Src0.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb_Src0.WordWrap = false;
			this.tb_Src0.TextChanged += new System.EventHandler(this.tb_TextChanged);
			this.tb_Src0.Enter += new System.EventHandler(this.tb_Activated);
			// 
			// tb_dst
			// 
			this.tb_dst.Enabled = false;
			this.tb_dst.HideSelection = false;
			this.tb_dst.Location = new System.Drawing.Point(110, 130);
			this.tb_dst.Margin = new System.Windows.Forms.Padding(0);
			this.tb_dst.Name = "tb_dst";
			this.tb_dst.Size = new System.Drawing.Size(50, 19);
			this.tb_dst.TabIndex = 8;
			this.tb_dst.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb_dst.WordWrap = false;
			this.tb_dst.TextChanged += new System.EventHandler(this.tb_TextChanged);
			this.tb_dst.Enter += new System.EventHandler(this.tb_Activated);
			// 
			// bu_ok
			// 
			this.bu_ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.bu_ok.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.bu_ok.Enabled = false;
			this.bu_ok.Location = new System.Drawing.Point(172, 193);
			this.bu_ok.Margin = new System.Windows.Forms.Padding(0);
			this.bu_ok.Name = "bu_ok";
			this.bu_ok.Size = new System.Drawing.Size(95, 25);
			this.bu_ok.TabIndex = 12;
			this.bu_ok.Text = "Ok";
			this.bu_ok.UseVisualStyleBackColor = true;
			// 
			// la_dotdot
			// 
			this.la_dotdot.Location = new System.Drawing.Point(105, 85);
			this.la_dotdot.Margin = new System.Windows.Forms.Padding(0);
			this.la_dotdot.Name = "la_dotdot";
			this.la_dotdot.Size = new System.Drawing.Size(15, 15);
			this.la_dotdot.TabIndex = 3;
			this.la_dotdot.Text = "..";
			this.la_dotdot.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tb_Src1
			// 
			this.tb_Src1.HideSelection = false;
			this.tb_Src1.Location = new System.Drawing.Point(160, 80);
			this.tb_Src1.Margin = new System.Windows.Forms.Padding(0);
			this.tb_Src1.Name = "tb_Src1";
			this.tb_Src1.Size = new System.Drawing.Size(50, 19);
			this.tb_Src1.TabIndex = 5;
			this.tb_Src1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb_Src1.WordWrap = false;
			this.tb_Src1.TextChanged += new System.EventHandler(this.tb_TextChanged);
			this.tb_Src1.Enter += new System.EventHandler(this.tb_Activated);
			// 
			// tb_shift
			// 
			this.tb_shift.Enabled = false;
			this.tb_shift.HideSelection = false;
			this.tb_shift.Location = new System.Drawing.Point(110, 150);
			this.tb_shift.Margin = new System.Windows.Forms.Padding(0);
			this.tb_shift.Name = "tb_shift";
			this.tb_shift.Size = new System.Drawing.Size(50, 19);
			this.tb_shift.TabIndex = 10;
			this.tb_shift.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tb_shift.WordWrap = false;
			this.tb_shift.TextChanged += new System.EventHandler(this.tb_TextChanged);
			this.tb_shift.Enter += new System.EventHandler(this.tb_Activated);
			// 
			// rb_clear
			// 
			this.rb_clear.Location = new System.Drawing.Point(15, 110);
			this.rb_clear.Margin = new System.Windows.Forms.Padding(0);
			this.rb_clear.Name = "rb_clear";
			this.rb_clear.Size = new System.Drawing.Size(95, 20);
			this.rb_clear.TabIndex = 6;
			this.rb_clear.Text = "clear parts";
			this.rb_clear.UseVisualStyleBackColor = true;
			this.rb_clear.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
			this.rb_clear.TabStopChanged += new System.EventHandler(this.rb_TabStopChanged);
			// 
			// rb_dst
			// 
			this.rb_dst.Location = new System.Drawing.Point(15, 130);
			this.rb_dst.Margin = new System.Windows.Forms.Padding(0);
			this.rb_dst.Name = "rb_dst";
			this.rb_dst.Size = new System.Drawing.Size(95, 20);
			this.rb_dst.TabIndex = 7;
			this.rb_dst.Text = "assign setId";
			this.rb_dst.UseVisualStyleBackColor = true;
			this.rb_dst.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
			this.rb_dst.TabStopChanged += new System.EventHandler(this.rb_TabStopChanged);
			// 
			// rb_shift
			// 
			this.rb_shift.Location = new System.Drawing.Point(15, 150);
			this.rb_shift.Margin = new System.Windows.Forms.Padding(0);
			this.rb_shift.Name = "rb_shift";
			this.rb_shift.Size = new System.Drawing.Size(95, 20);
			this.rb_shift.TabIndex = 9;
			this.rb_shift.Text = "shift +/-";
			this.rb_shift.UseVisualStyleBackColor = true;
			this.rb_shift.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
			this.rb_shift.TabStopChanged += new System.EventHandler(this.rb_TabStopChanged);
			// 
			// bu_cancel
			// 
			this.bu_cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.bu_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.bu_cancel.Location = new System.Drawing.Point(7, 198);
			this.bu_cancel.Margin = new System.Windows.Forms.Padding(0);
			this.bu_cancel.Name = "bu_cancel";
			this.bu_cancel.Size = new System.Drawing.Size(85, 20);
			this.bu_cancel.TabIndex = 11;
			this.bu_cancel.Text = "Cancel";
			this.bu_cancel.UseVisualStyleBackColor = true;
			// 
			// la_start
			// 
			this.la_start.Location = new System.Drawing.Point(15, 85);
			this.la_start.Margin = new System.Windows.Forms.Padding(0);
			this.la_start.Name = "la_start";
			this.la_start.Size = new System.Drawing.Size(35, 15);
			this.la_start.TabIndex = 1;
			this.la_start.Text = "start";
			this.la_start.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// la_stop
			// 
			this.la_stop.Location = new System.Drawing.Point(125, 85);
			this.la_stop.Margin = new System.Windows.Forms.Padding(0);
			this.la_stop.Name = "la_stop";
			this.la_stop.Size = new System.Drawing.Size(30, 15);
			this.la_stop.TabIndex = 4;
			this.la_stop.Text = "stop";
			this.la_stop.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// la_note
			// 
			this.la_note.ForeColor = System.Drawing.Color.PaleVioletRed;
			this.la_note.Location = new System.Drawing.Point(58, 177);
			this.la_note.Margin = new System.Windows.Forms.Padding(0);
			this.la_note.Name = "la_note";
			this.la_note.Size = new System.Drawing.Size(214, 15);
			this.la_note.TabIndex = 13;
			this.la_note.Text = "REMINDER: there is no Undo function";
			this.la_note.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// TilepartSubstitution
			// 
			this.AcceptButton = this.bu_ok;
			this.CancelButton = this.bu_cancel;
			this.ClientSize = new System.Drawing.Size(274, 221);
			this.Controls.Add(this.la_stop);
			this.Controls.Add(this.la_start);
			this.Controls.Add(this.bu_ok);
			this.Controls.Add(this.bu_cancel);
			this.Controls.Add(this.tb_shift);
			this.Controls.Add(this.rb_shift);
			this.Controls.Add(this.tb_dst);
			this.Controls.Add(this.rb_dst);
			this.Controls.Add(this.rb_clear);
			this.Controls.Add(this.tb_Src1);
			this.Controls.Add(this.la_dotdot);
			this.Controls.Add(this.tb_Src0);
			this.Controls.Add(this.la_head);
			this.Controls.Add(this.la_note);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TilepartSubstitution";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Tilepart Substitution";
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion Designer
	}
}
