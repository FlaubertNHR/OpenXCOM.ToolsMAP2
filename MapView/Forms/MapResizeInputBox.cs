using System;
using System.Drawing;
using System.Windows.Forms;

using DSShared;

using XCom;


namespace MapView
{
	internal sealed class MapResizeInputBox
		:
			Form
	{
		#region Fields (static)
		private static int _x = -1;
		private static int _y;
		#endregion Fields (static)


		#region Fields
		private MapFile _file;
		#endregion Fields


		#region Properties
		private int _cols;
		internal int Cols
		{
			get { return _cols; }
		}

		private int _rows;
		internal int Rows
		{
			get { return _rows; }
		}

		private int _levs;
		internal int Levs
		{
			get { return _levs; }
		}

		internal MapResizeZtype zType
		{
			get
			{
				// NOTE: When the form closes Visible==true/false is unreliable
				// but Enabled==true/false still works.

				if (cb_Ceil.Enabled && cb_Ceil.Checked)
					return MapResizeZtype.MRZT_TOP;

				return MapResizeZtype.MRZT_BOT;
			}
		}
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="file"></param>
		internal MapResizeInputBox(MapFile file)
		{
			InitializeComponent();

			_file = file;

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


			tb_Row0.BackColor =
			tb_Col0.BackColor =
			tb_Lev0.BackColor = Color.LavenderBlush;

			tb_Row1.BackColor =
			tb_Col1.BackColor =
			tb_Lev1.BackColor = Color.Azure;

			tb_Row0.Text =
			tb_Row1.Text = file.Rows.ToString();
			tb_Col0.Text =
			tb_Col1.Text = file.Cols.ToString();
			tb_Lev0.Text =
			tb_Lev1.Text = file.Levs.ToString();

			btn_Cancel.Select();
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
		/// <c>[Ctrl+z]</c>.
		/// </summary>
		/// <param name="e"></param>
		/// <remarks>Requires <c>KeyPreview</c> <c>true</c>.</remarks>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.KeyData == (Keys.Control | Keys.Z))
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
		/// <param name="sender"><c><see cref="lbl_Head"/></c></param>
		/// <param name="e"></param>
		private void OnPaintHead(object sender, PaintEventArgs e)
		{
			e.Graphics.DrawLine(Pens.Black, 0,0, 0, ClientSize.Height - 1);
			e.Graphics.DrawLine(Pens.Black, 1,0, ClientSize.Width - 1, 0);
		}

		/// <summary>
		/// Handles an Ok click.
		/// </summary>
		/// <param name="sender"><c><see cref="btn_Ok"/></c></param>
		/// <param name="e"></param>
		private void OnOkClick(object sender, EventArgs e)
		{
			if (   String.IsNullOrEmpty(tb_Col1.Text)
				|| String.IsNullOrEmpty(tb_Row1.Text)
				|| String.IsNullOrEmpty(tb_Lev1.Text))
			{
				ShowError("All fields must have a value.");
			}
			else if (!Int32.TryParse(tb_Col1.Text, out _cols) || _cols < 1 || _cols > Byte.MaxValue
				||   !Int32.TryParse(tb_Row1.Text, out _rows) || _rows < 1 || _rows > Byte.MaxValue
				||   !Int32.TryParse(tb_Lev1.Text, out _levs) || _levs < 1 || _levs > Byte.MaxValue)
			{
				ShowError("Values must be greater than 0 and less than 256.");
			}
			else if (_cols % 10 != 0 || _rows % 10 != 0)
			{
				ShowError("Columns and Rows must be multiples of 10.");
			}
			else if (_cols == int.Parse(tb_Col0.Text)
				&&   _rows == int.Parse(tb_Row0.Text)
				&&   _levs == int.Parse(tb_Lev0.Text))
			{
				DialogResult = DialogResult.Cancel;
			}
			else // finally (sic) ->
			{
				DialogResult = DialogResult.OK;
			}
		}

		/// <summary>
		/// Shows the ceiling-type checkbox if the levels have delta.
		/// </summary>
		/// <param name="sender"><c><see cref="tb_Lev1"/></c></param>
		/// <param name="e"></param>
		private void OnLevelTextChanged(object sender, EventArgs e)
		{
			if (!String.IsNullOrEmpty(tb_Lev1.Text)) // NOTE: btnOkClick will deal with a blank string.
			{
				int levels;
				if (Int32.TryParse(tb_Lev1.Text, out levels))
				{
					if (levels > 0)
					{
						int delta = (levels - _file.Levs);
						if (cb_Ceil.Visible = cb_Ceil.Enabled = (delta != 0))
						{
							if (delta > 0) cb_Ceil.Text = "add to top";
							else           cb_Ceil.Text = "remove from top";
						}
					}
					else
					{
						ShowError("Height must be 1 or more.");
						tb_Lev1.Text = tb_Lev0.Text; // recurse
					}
				}
				else
				{
					ShowError("Height must be a positive integer.");
					tb_Lev1.Text = tb_Lev0.Text; // recurse
				}
			}
		}
		#endregion Events


		#region Methods
		/// <summary>
		/// Wrapper for <see cref="Infobox"/>.
		/// </summary>
		/// <param name="head">the error string to show</param>
		private void ShowError(string head)
		{
			using (var f = new Infobox(
									"Error",
									head,
									null,
									InfoboxType.Error))
			{
				f.ShowDialog(this);
			}
		}
		#endregion Methods



		#region Designer
		private Label lbl_Lev1;
		private Label lbl_Col0;
		private Label lbl_Row0;
		private Label lbl_Lev0;
		private TextBox tb_Col1;
		private TextBox tb_Row1;
		private TextBox tb_Lev1;
		private Button btn_Ok;
		private Button btn_Cancel;
		private TextBox tb_Col0;
		private TextBox tb_Row0;
		private TextBox tb_Lev0;
		private Label lbl_Row1;
		private Label lbl_Col1;
		private Label lbl_Head;
		private CheckBox cb_Ceil;

		/// <summary>
		/// Required method for Designer support - do not modify the contents of
		/// this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.tb_Col0 = new System.Windows.Forms.TextBox();
			this.tb_Row0 = new System.Windows.Forms.TextBox();
			this.tb_Lev0 = new System.Windows.Forms.TextBox();
			this.tb_Row1 = new System.Windows.Forms.TextBox();
			this.lbl_Col0 = new System.Windows.Forms.Label();
			this.lbl_Row0 = new System.Windows.Forms.Label();
			this.lbl_Lev0 = new System.Windows.Forms.Label();
			this.tb_Col1 = new System.Windows.Forms.TextBox();
			this.tb_Lev1 = new System.Windows.Forms.TextBox();
			this.btn_Ok = new System.Windows.Forms.Button();
			this.btn_Cancel = new System.Windows.Forms.Button();
			this.cb_Ceil = new System.Windows.Forms.CheckBox();
			this.lbl_Lev1 = new System.Windows.Forms.Label();
			this.lbl_Row1 = new System.Windows.Forms.Label();
			this.lbl_Col1 = new System.Windows.Forms.Label();
			this.lbl_Head = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// tb_Col0
			// 
			this.tb_Col0.Location = new System.Drawing.Point(10, 50);
			this.tb_Col0.Margin = new System.Windows.Forms.Padding(0);
			this.tb_Col0.Name = "tb_Col0";
			this.tb_Col0.ReadOnly = true;
			this.tb_Col0.Size = new System.Drawing.Size(45, 19);
			this.tb_Col0.TabIndex = 4;
			// 
			// tb_Row0
			// 
			this.tb_Row0.Location = new System.Drawing.Point(60, 50);
			this.tb_Row0.Margin = new System.Windows.Forms.Padding(0);
			this.tb_Row0.Name = "tb_Row0";
			this.tb_Row0.ReadOnly = true;
			this.tb_Row0.Size = new System.Drawing.Size(45, 19);
			this.tb_Row0.TabIndex = 5;
			// 
			// tb_Lev0
			// 
			this.tb_Lev0.Location = new System.Drawing.Point(110, 50);
			this.tb_Lev0.Margin = new System.Windows.Forms.Padding(0);
			this.tb_Lev0.Name = "tb_Lev0";
			this.tb_Lev0.ReadOnly = true;
			this.tb_Lev0.Size = new System.Drawing.Size(45, 19);
			this.tb_Lev0.TabIndex = 6;
			// 
			// tb_Row1
			// 
			this.tb_Row1.Location = new System.Drawing.Point(60, 90);
			this.tb_Row1.Margin = new System.Windows.Forms.Padding(0);
			this.tb_Row1.Name = "tb_Row1";
			this.tb_Row1.Size = new System.Drawing.Size(45, 19);
			this.tb_Row1.TabIndex = 11;
			// 
			// lbl_Col0
			// 
			this.lbl_Col0.Location = new System.Drawing.Point(10, 35);
			this.lbl_Col0.Margin = new System.Windows.Forms.Padding(0);
			this.lbl_Col0.Name = "lbl_Col0";
			this.lbl_Col0.Size = new System.Drawing.Size(45, 15);
			this.lbl_Col0.TabIndex = 1;
			this.lbl_Col0.Text = "c";
			this.lbl_Col0.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lbl_Row0
			// 
			this.lbl_Row0.Location = new System.Drawing.Point(60, 35);
			this.lbl_Row0.Margin = new System.Windows.Forms.Padding(0);
			this.lbl_Row0.Name = "lbl_Row0";
			this.lbl_Row0.Size = new System.Drawing.Size(45, 15);
			this.lbl_Row0.TabIndex = 2;
			this.lbl_Row0.Text = "r";
			this.lbl_Row0.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lbl_Lev0
			// 
			this.lbl_Lev0.Location = new System.Drawing.Point(110, 35);
			this.lbl_Lev0.Margin = new System.Windows.Forms.Padding(0);
			this.lbl_Lev0.Name = "lbl_Lev0";
			this.lbl_Lev0.Size = new System.Drawing.Size(45, 15);
			this.lbl_Lev0.TabIndex = 3;
			this.lbl_Lev0.Text = "L";
			this.lbl_Lev0.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// tb_Col1
			// 
			this.tb_Col1.Location = new System.Drawing.Point(10, 90);
			this.tb_Col1.Margin = new System.Windows.Forms.Padding(0);
			this.tb_Col1.Name = "tb_Col1";
			this.tb_Col1.Size = new System.Drawing.Size(45, 19);
			this.tb_Col1.TabIndex = 10;
			// 
			// tb_Lev1
			// 
			this.tb_Lev1.Location = new System.Drawing.Point(110, 90);
			this.tb_Lev1.Margin = new System.Windows.Forms.Padding(0);
			this.tb_Lev1.Name = "tb_Lev1";
			this.tb_Lev1.Size = new System.Drawing.Size(45, 19);
			this.tb_Lev1.TabIndex = 12;
			this.tb_Lev1.TextChanged += new System.EventHandler(this.OnLevelTextChanged);
			// 
			// btn_Ok
			// 
			this.btn_Ok.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.btn_Ok.Location = new System.Drawing.Point(182, 128);
			this.btn_Ok.Margin = new System.Windows.Forms.Padding(0);
			this.btn_Ok.Name = "btn_Ok";
			this.btn_Ok.Size = new System.Drawing.Size(95, 25);
			this.btn_Ok.TabIndex = 14;
			this.btn_Ok.Text = "Ok";
			this.btn_Ok.Click += new System.EventHandler(this.OnOkClick);
			// 
			// btn_Cancel
			// 
			this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btn_Cancel.Location = new System.Drawing.Point(7, 133);
			this.btn_Cancel.Margin = new System.Windows.Forms.Padding(0);
			this.btn_Cancel.Name = "btn_Cancel";
			this.btn_Cancel.Size = new System.Drawing.Size(85, 20);
			this.btn_Cancel.TabIndex = 15;
			this.btn_Cancel.Text = "Cancel";
			// 
			// cb_Ceil
			// 
			this.cb_Ceil.Checked = true;
			this.cb_Ceil.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cb_Ceil.Enabled = false;
			this.cb_Ceil.Location = new System.Drawing.Point(160, 90);
			this.cb_Ceil.Margin = new System.Windows.Forms.Padding(0);
			this.cb_Ceil.Name = "cb_Ceil";
			this.cb_Ceil.Size = new System.Drawing.Size(120, 20);
			this.cb_Ceil.TabIndex = 13;
			this.cb_Ceil.Text = "ceil";
			this.cb_Ceil.UseVisualStyleBackColor = true;
			this.cb_Ceil.Visible = false;
			// 
			// lbl_Lev1
			// 
			this.lbl_Lev1.Location = new System.Drawing.Point(110, 75);
			this.lbl_Lev1.Margin = new System.Windows.Forms.Padding(0);
			this.lbl_Lev1.Name = "lbl_Lev1";
			this.lbl_Lev1.Size = new System.Drawing.Size(45, 15);
			this.lbl_Lev1.TabIndex = 9;
			this.lbl_Lev1.Text = "L";
			this.lbl_Lev1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lbl_Row1
			// 
			this.lbl_Row1.Location = new System.Drawing.Point(60, 75);
			this.lbl_Row1.Margin = new System.Windows.Forms.Padding(0);
			this.lbl_Row1.Name = "lbl_Row1";
			this.lbl_Row1.Size = new System.Drawing.Size(45, 15);
			this.lbl_Row1.TabIndex = 8;
			this.lbl_Row1.Text = "r";
			this.lbl_Row1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lbl_Col1
			// 
			this.lbl_Col1.Location = new System.Drawing.Point(10, 75);
			this.lbl_Col1.Margin = new System.Windows.Forms.Padding(0);
			this.lbl_Col1.Name = "lbl_Col1";
			this.lbl_Col1.Size = new System.Drawing.Size(45, 15);
			this.lbl_Col1.TabIndex = 7;
			this.lbl_Col1.Text = "c";
			this.lbl_Col1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lbl_Head
			// 
			this.lbl_Head.BackColor = System.Drawing.Color.Lavender;
			this.lbl_Head.Dock = System.Windows.Forms.DockStyle.Top;
			this.lbl_Head.Location = new System.Drawing.Point(0, 0);
			this.lbl_Head.Margin = new System.Windows.Forms.Padding(0);
			this.lbl_Head.Name = "lbl_Head";
			this.lbl_Head.Padding = new System.Windows.Forms.Padding(3, 2, 0, 0);
			this.lbl_Head.Size = new System.Drawing.Size(284, 30);
			this.lbl_Head.TabIndex = 0;
			this.lbl_Head.Text = "Columns and Rows must be exact multiples of 10 and Levels must be 1 or more.";
			this.lbl_Head.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPaintHead);
			// 
			// MapResizeInputBox
			// 
			this.AcceptButton = this.btn_Ok;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.CancelButton = this.btn_Cancel;
			this.ClientSize = new System.Drawing.Size(284, 156);
			this.Controls.Add(this.lbl_Head);
			this.Controls.Add(this.lbl_Lev1);
			this.Controls.Add(this.lbl_Row1);
			this.Controls.Add(this.lbl_Col1);
			this.Controls.Add(this.cb_Ceil);
			this.Controls.Add(this.btn_Cancel);
			this.Controls.Add(this.btn_Ok);
			this.Controls.Add(this.tb_Lev1);
			this.Controls.Add(this.tb_Col1);
			this.Controls.Add(this.lbl_Lev0);
			this.Controls.Add(this.lbl_Row0);
			this.Controls.Add(this.lbl_Col0);
			this.Controls.Add(this.tb_Row1);
			this.Controls.Add(this.tb_Col0);
			this.Controls.Add(this.tb_Lev0);
			this.Controls.Add(this.tb_Row0);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MapResizeInputBox";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Modify Mapsize";
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion Designer
	}
}
