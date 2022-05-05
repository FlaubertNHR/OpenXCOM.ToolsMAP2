using System;
using System.Windows.Forms;


namespace PckView
{
	internal sealed partial class CreateTabD
	{
		#region Designer
		private Label la_head;
		private TextBox tb_input;
		private Button bu_open;
		private RadioButton rb_2byte;
		private RadioButton rb_4byte;
		private Label la_warn;
		private Button bu_create;
		private Button bu_cancel;

		/// <summary>
		/// Required method for Designer support - do not modify the contents of
		/// this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.la_head = new System.Windows.Forms.Label();
			this.tb_input = new System.Windows.Forms.TextBox();
			this.bu_open = new System.Windows.Forms.Button();
			this.rb_2byte = new System.Windows.Forms.RadioButton();
			this.rb_4byte = new System.Windows.Forms.RadioButton();
			this.la_warn = new System.Windows.Forms.Label();
			this.bu_create = new System.Windows.Forms.Button();
			this.bu_cancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// la_head
			// 
			this.la_head.Dock = System.Windows.Forms.DockStyle.Top;
			this.la_head.Location = new System.Drawing.Point(0, 0);
			this.la_head.Margin = new System.Windows.Forms.Padding(0);
			this.la_head.Name = "la_head";
			this.la_head.Padding = new System.Windows.Forms.Padding(2, 3, 0, 0);
			this.la_head.Size = new System.Drawing.Size(397, 20);
			this.la_head.TabIndex = 0;
			this.la_head.Text = "Use this tool to create a TAB file from a PCK (compressed) data file.";
			// 
			// tb_input
			// 
			this.tb_input.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.tb_input.HideSelection = false;
			this.tb_input.Location = new System.Drawing.Point(4, 23);
			this.tb_input.Margin = new System.Windows.Forms.Padding(0);
			this.tb_input.Name = "tb_input";
			this.tb_input.Size = new System.Drawing.Size(363, 19);
			this.tb_input.TabIndex = 1;
			this.tb_input.WordWrap = false;
			this.tb_input.TextChanged += new System.EventHandler(this.OnInputTextchanged);
			// 
			// bu_open
			// 
			this.bu_open.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.bu_open.Location = new System.Drawing.Point(369, 22);
			this.bu_open.Margin = new System.Windows.Forms.Padding(0);
			this.bu_open.Name = "bu_open";
			this.bu_open.Size = new System.Drawing.Size(25, 21);
			this.bu_open.TabIndex = 2;
			this.bu_open.Text = "...";
			this.bu_open.UseVisualStyleBackColor = true;
			this.bu_open.Click += new System.EventHandler(this.OnOpenPckfileClick);
			// 
			// rb_2byte
			// 
			this.rb_2byte.Location = new System.Drawing.Point(10, 48);
			this.rb_2byte.Margin = new System.Windows.Forms.Padding(0);
			this.rb_2byte.Name = "rb_2byte";
			this.rb_2byte.Size = new System.Drawing.Size(100, 20);
			this.rb_2byte.TabIndex = 3;
			this.rb_2byte.Text = "2 byte offsets";
			this.rb_2byte.UseVisualStyleBackColor = true;
			this.rb_2byte.CheckedChanged += new System.EventHandler(this.OnTabwordLengthCheckedChanged);
			// 
			// rb_4byte
			// 
			this.rb_4byte.Location = new System.Drawing.Point(10, 68);
			this.rb_4byte.Margin = new System.Windows.Forms.Padding(0);
			this.rb_4byte.Name = "rb_4byte";
			this.rb_4byte.Size = new System.Drawing.Size(100, 20);
			this.rb_4byte.TabIndex = 4;
			this.rb_4byte.Text = "4 byte offsets";
			this.rb_4byte.UseVisualStyleBackColor = true;
			// 
			// la_warn
			// 
			this.la_warn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.la_warn.ForeColor = System.Drawing.Color.DarkSalmon;
			this.la_warn.Location = new System.Drawing.Point(0, 88);
			this.la_warn.Margin = new System.Windows.Forms.Padding(0);
			this.la_warn.Name = "la_warn";
			this.la_warn.Size = new System.Drawing.Size(397, 15);
			this.la_warn.TabIndex = 5;
			this.la_warn.Text = "This is not guaranteed to be valid.";
			this.la_warn.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// bu_create
			// 
			this.bu_create.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.bu_create.Enabled = false;
			this.bu_create.Location = new System.Drawing.Point(9, 104);
			this.bu_create.Margin = new System.Windows.Forms.Padding(0);
			this.bu_create.Name = "bu_create";
			this.bu_create.Size = new System.Drawing.Size(100, 21);
			this.bu_create.TabIndex = 6;
			this.bu_create.Text = "Create";
			this.bu_create.UseVisualStyleBackColor = true;
			this.bu_create.Click += new System.EventHandler(this.OnCreateClick);
			// 
			// bu_cancel
			// 
			this.bu_cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.bu_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.bu_cancel.Location = new System.Drawing.Point(313, 104);
			this.bu_cancel.Margin = new System.Windows.Forms.Padding(0);
			this.bu_cancel.Name = "bu_cancel";
			this.bu_cancel.Size = new System.Drawing.Size(75, 21);
			this.bu_cancel.TabIndex = 7;
			this.bu_cancel.Text = "cancel";
			this.bu_cancel.UseVisualStyleBackColor = true;
			// 
			// CreateTabD
			// 
			this.AcceptButton = this.bu_create;
			this.CancelButton = this.bu_cancel;
			this.ClientSize = new System.Drawing.Size(397, 128);
			this.Controls.Add(this.la_warn);
			this.Controls.Add(this.rb_4byte);
			this.Controls.Add(this.rb_2byte);
			this.Controls.Add(this.bu_create);
			this.Controls.Add(this.bu_cancel);
			this.Controls.Add(this.bu_open);
			this.Controls.Add(this.tb_input);
			this.Controls.Add(this.la_head);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MaximizeBox = false;
			this.Name = "CreateTabD";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Create tabfile";
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion Designer
	}
}
