using System;
using System.Globalization;
using System.Windows.Forms;

using XCom;


namespace MapView
{
	internal sealed class MapResizeInputBox
		:
			Form
	{
		#region Fields
		private MapFile _file;
		#endregion Fields


		#region Properties
		private int _rows;
		internal int Rows
		{
			get { return _rows; }
		}

		private int _cols;
		internal int Cols
		{
			get { return _cols; }
		}

		private int _levs;
		internal int Levs
		{
			get { return _levs; }
		}

		internal MapResizeService.MapResizeZtype zType
		{
			get
			{
				if (cb_Ceil.Enabled && cb_Ceil.Checked)
					return MapResizeService.MapResizeZtype.MRZT_TOP;

				return MapResizeService.MapResizeZtype.MRZT_BOT;
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

			tb_Row0.Text =
			tb_Row1.Text = file.MapSize.Rows.ToString(CultureInfo.InvariantCulture);
			tb_Col0.Text =
			tb_Col1.Text = file.MapSize.Cols.ToString(CultureInfo.InvariantCulture);
			tb_Lev0.Text =
			tb_Lev1.Text = file.MapSize.Levs.ToString(CultureInfo.InvariantCulture);

			btn_Cancel.Select();
			DialogResult = DialogResult.Cancel;
		}
		#endregion cTor


		#region Events
		private void OnOkClick(object sender, EventArgs e)
		{
			if (   String.IsNullOrEmpty(tb_Col1.Text)
				|| String.IsNullOrEmpty(tb_Row1.Text)
				|| String.IsNullOrEmpty(tb_Lev1.Text))
			{
				ShowErrorDialog("All fields must have a value.", " Error");
			}
			else if (!Int32.TryParse(tb_Col1.Text, out _cols) || _cols < 1
				||   !Int32.TryParse(tb_Row1.Text, out _rows) || _rows < 1
				||   !Int32.TryParse(tb_Lev1.Text, out _levs) || _levs < 1)
			{
				ShowErrorDialog("Values must be positive integers greater than 0.", " Error");
			}
			else if (_cols % 10 != 0 || _rows % 10 != 0)
			{
				ShowErrorDialog("Columns and Rows must be evenly divisible by 10.", " Error");
			}
			else if (_cols == int.Parse(tb_Col0.Text, CultureInfo.InvariantCulture)
				&&   _rows == int.Parse(tb_Row0.Text, CultureInfo.InvariantCulture)
				&&   _levs == int.Parse(tb_Lev0.Text, CultureInfo.InvariantCulture))
			{
				ShowErrorDialog("The new size is the same as the old size.", " uh ...");
			}
			else // finally (sic) ->
			{
				DialogResult = DialogResult.OK;
				Close();
			}
		}

		/// <summary>
		/// Closes this form and discards any changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnCancelClick(object sender, EventArgs e)
		{
			Close();
		}

		/// <summary>
		/// Shows the AddToCeiling checkbox if the height has delta.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnLevelTextChanged(object sender, EventArgs e)
		{
			if (!String.IsNullOrEmpty(tb_Lev1.Text)) // NOTE: btnOkClick will deal with a blank string.
			{
				int height;
				if (Int32.TryParse(tb_Lev1.Text, out height))
				{
					if (height > 0)
					{
						int delta = (height - _file.MapSize.Levs);
						if (cb_Ceil.Enabled = (delta != 0))
						{
							if (delta > 0)
							{
								cb_Ceil.Text = "add to top";
							}
							else
								cb_Ceil.Text = "subtract from top";
						}
					}
					else
						ShowErrorDialog("Height must be 1 or more.", " Error");
				}
				else
					ShowErrorDialog("Height must a positive integer.", " Error");
			}
		}
		#endregion Events


		#region Methods
		/// <summary>
		/// Wrapper for MessageBox.Show().
		/// </summary>
		/// <param name="error">the error string to show</param>
		/// <param name="caption">the dialog's caption text</param>
		private void ShowErrorDialog(
				string error,
				string caption)
		{
			MessageBox.Show(
						this,
						error,
						caption,
						MessageBoxButtons.OK,
						MessageBoxIcon.Error,
						MessageBoxDefaultButton.Button1,
						0);
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
			this.tb_Col0.Location = new System.Drawing.Point(20, 55);
			this.tb_Col0.Name = "tb_Col0";
			this.tb_Col0.ReadOnly = true;
			this.tb_Col0.Size = new System.Drawing.Size(45, 19);
			this.tb_Col0.TabIndex = 4;
			// 
			// tb_Row0
			// 
			this.tb_Row0.Location = new System.Drawing.Point(70, 55);
			this.tb_Row0.Name = "tb_Row0";
			this.tb_Row0.ReadOnly = true;
			this.tb_Row0.Size = new System.Drawing.Size(45, 19);
			this.tb_Row0.TabIndex = 5;
			// 
			// tb_Lev0
			// 
			this.tb_Lev0.Location = new System.Drawing.Point(120, 55);
			this.tb_Lev0.Name = "tb_Lev0";
			this.tb_Lev0.ReadOnly = true;
			this.tb_Lev0.Size = new System.Drawing.Size(45, 19);
			this.tb_Lev0.TabIndex = 6;
			// 
			// tb_Row1
			// 
			this.tb_Row1.Location = new System.Drawing.Point(70, 95);
			this.tb_Row1.Name = "tb_Row1";
			this.tb_Row1.Size = new System.Drawing.Size(45, 19);
			this.tb_Row1.TabIndex = 11;
			// 
			// lbl_Col0
			// 
			this.lbl_Col0.Location = new System.Drawing.Point(20, 40);
			this.lbl_Col0.Name = "lbl_Col0";
			this.lbl_Col0.Size = new System.Drawing.Size(45, 15);
			this.lbl_Col0.TabIndex = 1;
			this.lbl_Col0.Text = "c";
			this.lbl_Col0.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lbl_Row0
			// 
			this.lbl_Row0.Location = new System.Drawing.Point(70, 40);
			this.lbl_Row0.Name = "lbl_Row0";
			this.lbl_Row0.Size = new System.Drawing.Size(45, 15);
			this.lbl_Row0.TabIndex = 2;
			this.lbl_Row0.Text = "r";
			this.lbl_Row0.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lbl_Lev0
			// 
			this.lbl_Lev0.Location = new System.Drawing.Point(120, 40);
			this.lbl_Lev0.Name = "lbl_Lev0";
			this.lbl_Lev0.Size = new System.Drawing.Size(45, 15);
			this.lbl_Lev0.TabIndex = 3;
			this.lbl_Lev0.Text = "L";
			this.lbl_Lev0.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// tb_Col1
			// 
			this.tb_Col1.Location = new System.Drawing.Point(20, 95);
			this.tb_Col1.Name = "tb_Col1";
			this.tb_Col1.Size = new System.Drawing.Size(45, 19);
			this.tb_Col1.TabIndex = 10;
			// 
			// tb_Lev1
			// 
			this.tb_Lev1.Location = new System.Drawing.Point(120, 95);
			this.tb_Lev1.Name = "tb_Lev1";
			this.tb_Lev1.Size = new System.Drawing.Size(45, 19);
			this.tb_Lev1.TabIndex = 12;
			this.tb_Lev1.TextChanged += new System.EventHandler(this.OnLevelTextChanged);
			// 
			// btn_Ok
			// 
			this.btn_Ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btn_Ok.Location = new System.Drawing.Point(120, 125);
			this.btn_Ok.Name = "btn_Ok";
			this.btn_Ok.Size = new System.Drawing.Size(80, 30);
			this.btn_Ok.TabIndex = 14;
			this.btn_Ok.Text = "Ok";
			this.btn_Ok.Click += new System.EventHandler(this.OnOkClick);
			// 
			// btn_Cancel
			// 
			this.btn_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btn_Cancel.Location = new System.Drawing.Point(210, 125);
			this.btn_Cancel.Margin = new System.Windows.Forms.Padding(0);
			this.btn_Cancel.Name = "btn_Cancel";
			this.btn_Cancel.Size = new System.Drawing.Size(80, 30);
			this.btn_Cancel.TabIndex = 15;
			this.btn_Cancel.Text = "Cancel";
			this.btn_Cancel.Click += new System.EventHandler(this.OnCancelClick);
			// 
			// cb_Ceil
			// 
			this.cb_Ceil.Checked = true;
			this.cb_Ceil.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cb_Ceil.Enabled = false;
			this.cb_Ceil.Location = new System.Drawing.Point(170, 95);
			this.cb_Ceil.Name = "cb_Ceil";
			this.cb_Ceil.Size = new System.Drawing.Size(120, 21);
			this.cb_Ceil.TabIndex = 13;
			this.cb_Ceil.Text = "top";
			this.cb_Ceil.UseVisualStyleBackColor = true;
			// 
			// lbl_Lev1
			// 
			this.lbl_Lev1.Location = new System.Drawing.Point(120, 80);
			this.lbl_Lev1.Name = "lbl_Lev1";
			this.lbl_Lev1.Size = new System.Drawing.Size(45, 15);
			this.lbl_Lev1.TabIndex = 9;
			this.lbl_Lev1.Text = "L";
			this.lbl_Lev1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lbl_Row1
			// 
			this.lbl_Row1.Location = new System.Drawing.Point(70, 80);
			this.lbl_Row1.Name = "lbl_Row1";
			this.lbl_Row1.Size = new System.Drawing.Size(45, 15);
			this.lbl_Row1.TabIndex = 8;
			this.lbl_Row1.Text = "r";
			this.lbl_Row1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lbl_Col1
			// 
			this.lbl_Col1.Location = new System.Drawing.Point(20, 80);
			this.lbl_Col1.Name = "lbl_Col1";
			this.lbl_Col1.Size = new System.Drawing.Size(45, 15);
			this.lbl_Col1.TabIndex = 7;
			this.lbl_Col1.Text = "c";
			this.lbl_Col1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lbl_Head
			// 
			this.lbl_Head.Dock = System.Windows.Forms.DockStyle.Top;
			this.lbl_Head.Location = new System.Drawing.Point(0, 0);
			this.lbl_Head.Name = "lbl_Head";
			this.lbl_Head.Padding = new System.Windows.Forms.Padding(3, 5, 0, 0);
			this.lbl_Head.Size = new System.Drawing.Size(304, 35);
			this.lbl_Head.TabIndex = 0;
			this.lbl_Head.Text = "Columns and Rows must be multiples of 10 (10, 20, 30, etc) and Levels must be 1 o" +
	"r more.";
			// 
			// MapResizeInputBox
			// 
			this.AcceptButton = this.btn_Ok;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.CancelButton = this.btn_Cancel;
			this.ClientSize = new System.Drawing.Size(304, 161);
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
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MapResizeInputBox";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Modify Map Size";
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion Designer
	}
}
