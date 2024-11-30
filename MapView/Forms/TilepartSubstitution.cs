using System;
using System.Drawing;
using System.Windows.Forms;

using DSShared;

using XCom;


namespace MapView
{
	/// <summary>
	/// A dialog that allows the user to change or shift tilepart-ids.
	/// </summary>
	internal sealed class TilepartSubstitution
		:
			Form
	{
		#region Fields (static)
		private static int _x = -1;
		private static int _y;

		internal static int src0  = Int32.MaxValue;
		internal static int src1  = Int32.MaxValue;
		internal static int dst   = Int32.MaxValue;
		internal static int shift = Int32.MaxValue;

		internal static TilepartSubstitutionType rb_selected = TilepartSubstitutionType.Clear;
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
			if (file.IsMAP)
				_highid = Math.Min(file.Parts.Count - 1, MapFile.MaxTerrainIdMAP);
			else
				_highid = file.Parts.Count - 1;

            int valid   = Int32.MinValue;
			int invalid = Int32.MinValue;
			DeterHighIds(file, ref valid, ref invalid);

			string head = "Change all tileparts between setIds"
						+ Environment.NewLine
						+ "inclusive - if stop id is blank use start id"
						+ Environment.NewLine + Environment.NewLine
						+ "Highest placeable id in the terrainset = " + _highid;

			if (valid != Int32.MinValue)
				head += Environment.NewLine + "Highest valid tilepart id detected = " + valid;

			if (invalid != Int32.MinValue)
				head += Environment.NewLine + "Highest invalid tilepart id detected = " + invalid;

			la_head.Text = head;


			tb_Src0.BackColor =
			tb_Src1.BackColor = Color.Wheat;

			var   loc0 = new Point(_x,_y);
			Point loc1 = loc0 + Size;

			bool isInsideBounds = false;
			if (_x > -1)
			{
				foreach (var screen in Screen.AllScreens)
				{
					if (isInsideBounds = screen.Bounds.Contains(loc0)
									  && screen.Bounds.Contains(loc1))
					{
						break;
					}
				}
			}

			if (isInsideBounds)
				Location = loc0;
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
				case TilepartSubstitutionType.Clear: rb_clear.Checked = true; break;
				case TilepartSubstitutionType.Desti: rb_dst  .Checked = true; break;
				case TilepartSubstitutionType.Shift: ForceBlasterInitShift(); break; // rb_shift.Checked = true
			}

			tb_Src0.Select(); // set '_text'
			tb_Src0.SelectionStart = tb_Src0.Text.Length;
		}

		/// <summary>
		/// Deters the highest valid and invalid setId(s) placed in the current
		/// tileset.
		/// </summary>
		/// <returns></returns>
		private static void DeterHighIds(MapFile file, ref int valid, ref int invalid)
		{
			MapTile tile; Tilepart part;

			for (int lev = 0; lev != file.Levs; ++lev)
			for (int row = 0; row != file.Rows; ++row)
			for (int col = 0; col != file.Cols; ++col)
			{
				if (!(tile = file.GetTile(col, row, lev)).Vacant)
				{
					if ((part = tile.Floor) != null)
					{
						if (part.Record.PartType != PartType.Invalid) // not a crippled part
						{
							if (part.SetId > valid)
								valid = part.SetId;
						}
						else if (part.SetId > invalid)
							invalid = part.SetId;
					}

					if ((part = tile.West) != null)
					{
						if (part.Record.PartType != PartType.Invalid) // not a crippled part
						{
							if (part.SetId > valid)
								valid = part.SetId;
						}
						else if (part.SetId > invalid)
							invalid = part.SetId;
					}

					if ((part = tile.North) != null)
					{
						if (part.Record.PartType != PartType.Invalid) // not a crippled part
						{
							if (part.SetId > valid)
								valid = part.SetId;
						}
						else if (part.SetId > invalid)
							invalid = part.SetId;
					}

					if ((part = tile.Content) != null)
					{
						if (part.Record.PartType != PartType.Invalid) // not a crippled part
						{
							if (part.SetId > valid)
								valid = part.SetId;
						}
						else if (part.SetId > invalid)
							invalid = part.SetId;
					}
				}
			}
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
		/// Overrides the <c>KeyDown</c> handler. Closes this dialog on
		/// <c>[Ctrl+u]</c>.
		/// </summary>
		/// <param name="e"></param>
		/// <remarks>Requires KeyPreview true.</remarks>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.KeyData == (Keys.Control | Keys.U))
				Close();
		}

		/// <summary>
		/// Overrides the <c>Paint</c> handler.
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
		/// holy fc genius
		/// https://stackoverflow.com/questions/24997658/tab-index-is-not-working-on-radio-buttons#answer-31063124
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks>The radiobuttons need to be selected (once) before this
		/// works.</remarks>
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
						rb_selected = TilepartSubstitutionType.Clear;

						tb_dst  .Enabled =
						tb_shift.Enabled = false;

						tb_dst  .BackColor =
						tb_shift.BackColor = SystemColors.Control;
					}
					else if (rb == rb_dst)
					{
						rb_selected = TilepartSubstitutionType.Desti;

						tb_dst  .Enabled = true;
						tb_shift.Enabled = false;

						tb_dst  .BackColor = Color.LightGreen;
						tb_shift.BackColor = SystemColors.Control;
					}
					else // rb == rb_shift
					{
						rb_selected = TilepartSubstitutionType.Shift;

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
		/// Lord knows why
		/// <c><see cref="rb_CheckedChanged()">rb_CheckedChanged()</see></c>
		/// does not fire when trying to init w/ RadioSelected.Shift.
		/// <br/><br/>
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

				if (text.Length != 0
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
		/// <c>DialogResult.OK</c>.
		/// </summary>
		/// <param name="tb"></param>
		/// <param name="result"></param>
		private void SetStatics(object tb, int result)
		{
			if      (tb == tb_Src0) src0  = result;
			else if (tb == tb_Src1) src1  = result;
			else if (tb == tb_dst)  dst   = result;
			else                    shift = result; // tb == tb_shift
		}

		/// <summary>
		/// Dis/enables the Ok button.
		/// </summary>
		/// <remarks>The input values src0/src1 are allowed to exceed the MaxId
		/// in the current terrainset but the output shall not be allowed to
		/// exceed the MaxId.</remarks>
		private void Enable()
		{
			bu_ok.Enabled =  src0 != Int32.MaxValue
						 && (src1 == Int32.MaxValue || src1 >= src0)
						 && (    rb_clear.Checked
							 || (rb_dst  .Checked &&   dst != Int32.MaxValue && (dst != src0 || (src1 != Int32.MaxValue && src1 != src0)))
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
			this.la_head.Size = new System.Drawing.Size(274, 90);
			this.la_head.TabIndex = 0;
			this.la_head.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.la_head.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPaintHead);
			// 
			// tb_Src0
			// 
			this.tb_Src0.HideSelection = false;
			this.tb_Src0.Location = new System.Drawing.Point(50, 95);
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
			this.tb_dst.Location = new System.Drawing.Point(110, 145);
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
			this.bu_ok.Location = new System.Drawing.Point(172, 208);
			this.bu_ok.Margin = new System.Windows.Forms.Padding(0);
			this.bu_ok.Name = "bu_ok";
			this.bu_ok.Size = new System.Drawing.Size(95, 25);
			this.bu_ok.TabIndex = 12;
			this.bu_ok.Text = "Ok";
			this.bu_ok.UseVisualStyleBackColor = true;
			// 
			// la_dotdot
			// 
			this.la_dotdot.Location = new System.Drawing.Point(105, 100);
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
			this.tb_Src1.Location = new System.Drawing.Point(160, 95);
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
			this.tb_shift.Location = new System.Drawing.Point(110, 165);
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
			this.rb_clear.Location = new System.Drawing.Point(15, 125);
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
			this.rb_dst.Location = new System.Drawing.Point(15, 145);
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
			this.rb_shift.Location = new System.Drawing.Point(15, 165);
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
			this.bu_cancel.Location = new System.Drawing.Point(7, 213);
			this.bu_cancel.Margin = new System.Windows.Forms.Padding(0);
			this.bu_cancel.Name = "bu_cancel";
			this.bu_cancel.Size = new System.Drawing.Size(85, 20);
			this.bu_cancel.TabIndex = 11;
			this.bu_cancel.Text = "Cancel";
			this.bu_cancel.UseVisualStyleBackColor = true;
			// 
			// la_start
			// 
			this.la_start.Location = new System.Drawing.Point(15, 100);
			this.la_start.Margin = new System.Windows.Forms.Padding(0);
			this.la_start.Name = "la_start";
			this.la_start.Size = new System.Drawing.Size(35, 15);
			this.la_start.TabIndex = 1;
			this.la_start.Text = "start";
			this.la_start.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// la_stop
			// 
			this.la_stop.Location = new System.Drawing.Point(125, 100);
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
			this.la_note.Location = new System.Drawing.Point(58, 192);
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
			this.ClientSize = new System.Drawing.Size(274, 236);
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


	internal enum TilepartSubstitutionType
	{ Clear, Desti, Shift }
}
