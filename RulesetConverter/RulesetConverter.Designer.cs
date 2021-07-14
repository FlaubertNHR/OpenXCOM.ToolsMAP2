using System;
using System.Windows.Forms;


namespace RulesetConverter
{
	internal sealed partial class RulesetConverter
	{
		#region Designer
		private Button btn_Cancel;
		private Button btn_Convert;
		private TextBox tb_Input;
		private Button btn_Input;
		private Label lbl_Input;
		private Label lbl_Info;
		private RadioButton rb_Ufo;
		private RadioButton rb_Tftd;
		private Label la_Group;
		private CheckBox cb_Basepath;
		private Button btn_Basepath;
		private Label lbl_Basepath;
		private Label lbl_Label;
		private TextBox tb_Label;
		private Label la_Target;
		private Panel pa_Target;
		private Panel pa_Group;
		private RadioButton rb_Terrains;
		private RadioButton rb_Ufos;
		private RadioButton rb_Crafts;

		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor.
		/// The Forms designer might not be able to load this method if it was
		/// changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.btn_Cancel = new System.Windows.Forms.Button();
			this.btn_Convert = new System.Windows.Forms.Button();
			this.tb_Input = new System.Windows.Forms.TextBox();
			this.btn_Input = new System.Windows.Forms.Button();
			this.lbl_Input = new System.Windows.Forms.Label();
			this.lbl_Info = new System.Windows.Forms.Label();
			this.rb_Ufo = new System.Windows.Forms.RadioButton();
			this.rb_Tftd = new System.Windows.Forms.RadioButton();
			this.la_Group = new System.Windows.Forms.Label();
			this.cb_Basepath = new System.Windows.Forms.CheckBox();
			this.btn_Basepath = new System.Windows.Forms.Button();
			this.lbl_Basepath = new System.Windows.Forms.Label();
			this.lbl_Label = new System.Windows.Forms.Label();
			this.tb_Label = new System.Windows.Forms.TextBox();
			this.la_Target = new System.Windows.Forms.Label();
			this.pa_Target = new System.Windows.Forms.Panel();
			this.rb_Ufos = new System.Windows.Forms.RadioButton();
			this.rb_Crafts = new System.Windows.Forms.RadioButton();
			this.rb_Terrains = new System.Windows.Forms.RadioButton();
			this.pa_Group = new System.Windows.Forms.Panel();
			this.pa_Target.SuspendLayout();
			this.pa_Group.SuspendLayout();
			this.SuspendLayout();
			// 
			// btn_Cancel
			// 
			this.btn_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btn_Cancel.Location = new System.Drawing.Point(315, 178);
			this.btn_Cancel.Margin = new System.Windows.Forms.Padding(0);
			this.btn_Cancel.Name = "btn_Cancel";
			this.btn_Cancel.Size = new System.Drawing.Size(85, 30);
			this.btn_Cancel.TabIndex = 10;
			this.btn_Cancel.Text = "Cancel";
			this.btn_Cancel.UseVisualStyleBackColor = true;
			this.btn_Cancel.Click += new System.EventHandler(this.OnCancelClick);
			// 
			// btn_Convert
			// 
			this.btn_Convert.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btn_Convert.Enabled = false;
			this.btn_Convert.Location = new System.Drawing.Point(95, 178);
			this.btn_Convert.Margin = new System.Windows.Forms.Padding(0);
			this.btn_Convert.Name = "btn_Convert";
			this.btn_Convert.Size = new System.Drawing.Size(85, 30);
			this.btn_Convert.TabIndex = 9;
			this.btn_Convert.Text = "Convert";
			this.btn_Convert.UseVisualStyleBackColor = true;
			this.btn_Convert.Click += new System.EventHandler(this.OnConvertClick);
			// 
			// tb_Input
			// 
			this.tb_Input.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.tb_Input.Location = new System.Drawing.Point(5, 60);
			this.tb_Input.Margin = new System.Windows.Forms.Padding(0);
			this.tb_Input.Name = "tb_Input";
			this.tb_Input.ReadOnly = true;
			this.tb_Input.Size = new System.Drawing.Size(450, 19);
			this.tb_Input.TabIndex = 2;
			this.tb_Input.WordWrap = false;
			// 
			// btn_Input
			// 
			this.btn_Input.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btn_Input.Location = new System.Drawing.Point(460, 59);
			this.btn_Input.Margin = new System.Windows.Forms.Padding(0);
			this.btn_Input.Name = "btn_Input";
			this.btn_Input.Size = new System.Drawing.Size(30, 26);
			this.btn_Input.TabIndex = 3;
			this.btn_Input.Text = "...";
			this.btn_Input.UseVisualStyleBackColor = true;
			this.btn_Input.Click += new System.EventHandler(this.OnFindInputClick);
			// 
			// lbl_Input
			// 
			this.lbl_Input.Dock = System.Windows.Forms.DockStyle.Top;
			this.lbl_Input.Location = new System.Drawing.Point(0, 35);
			this.lbl_Input.Margin = new System.Windows.Forms.Padding(0);
			this.lbl_Input.Name = "lbl_Input";
			this.lbl_Input.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
			this.lbl_Input.Size = new System.Drawing.Size(492, 22);
			this.lbl_Input.TabIndex = 1;
			this.lbl_Input.Text = "File to convert";
			this.lbl_Input.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// lbl_Info
			// 
			this.lbl_Info.Dock = System.Windows.Forms.DockStyle.Top;
			this.lbl_Info.Location = new System.Drawing.Point(0, 0);
			this.lbl_Info.Margin = new System.Windows.Forms.Padding(0);
			this.lbl_Info.Name = "lbl_Info";
			this.lbl_Info.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
			this.lbl_Info.Size = new System.Drawing.Size(492, 35);
			this.lbl_Info.TabIndex = 0;
			this.lbl_Info.Text = "Info";
			this.lbl_Info.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// rb_Ufo
			// 
			this.rb_Ufo.Checked = true;
			this.rb_Ufo.Location = new System.Drawing.Point(60, 2);
			this.rb_Ufo.Margin = new System.Windows.Forms.Padding(0);
			this.rb_Ufo.Name = "rb_Ufo";
			this.rb_Ufo.Size = new System.Drawing.Size(50, 20);
			this.rb_Ufo.TabIndex = 1;
			this.rb_Ufo.TabStop = true;
			this.rb_Ufo.Text = "UFO";
			this.rb_Ufo.UseVisualStyleBackColor = true;
			this.rb_Ufo.CheckedChanged += new System.EventHandler(this.OnTypeChanged);
			// 
			// rb_Tftd
			// 
			this.rb_Tftd.Location = new System.Drawing.Point(115, 2);
			this.rb_Tftd.Margin = new System.Windows.Forms.Padding(0);
			this.rb_Tftd.Name = "rb_Tftd";
			this.rb_Tftd.Size = new System.Drawing.Size(55, 20);
			this.rb_Tftd.TabIndex = 2;
			this.rb_Tftd.Text = "TFTD";
			this.rb_Tftd.UseVisualStyleBackColor = true;
			this.rb_Tftd.CheckedChanged += new System.EventHandler(this.OnTypeChanged);
			// 
			// la_Group
			// 
			this.la_Group.Location = new System.Drawing.Point(5, 2);
			this.la_Group.Margin = new System.Windows.Forms.Padding(0);
			this.la_Group.Name = "la_Group";
			this.la_Group.Size = new System.Drawing.Size(45, 20);
			this.la_Group.TabIndex = 0;
			this.la_Group.Text = "Group";
			this.la_Group.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// cb_Basepath
			// 
			this.cb_Basepath.Location = new System.Drawing.Point(10, 135);
			this.cb_Basepath.Margin = new System.Windows.Forms.Padding(0);
			this.cb_Basepath.Name = "cb_Basepath";
			this.cb_Basepath.Size = new System.Drawing.Size(105, 20);
			this.cb_Basepath.TabIndex = 6;
			this.cb_Basepath.Text = "add Basepath";
			this.cb_Basepath.UseVisualStyleBackColor = true;
			this.cb_Basepath.CheckedChanged += new System.EventHandler(this.OnBasepathCheckChanged);
			// 
			// btn_Basepath
			// 
			this.btn_Basepath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btn_Basepath.Enabled = false;
			this.btn_Basepath.Location = new System.Drawing.Point(460, 154);
			this.btn_Basepath.Margin = new System.Windows.Forms.Padding(0);
			this.btn_Basepath.Name = "btn_Basepath";
			this.btn_Basepath.Size = new System.Drawing.Size(30, 26);
			this.btn_Basepath.TabIndex = 8;
			this.btn_Basepath.Text = "...";
			this.btn_Basepath.UseVisualStyleBackColor = true;
			this.btn_Basepath.Click += new System.EventHandler(this.OnFindBasepathClick);
			// 
			// lbl_Basepath
			// 
			this.lbl_Basepath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.lbl_Basepath.Location = new System.Drawing.Point(5, 160);
			this.lbl_Basepath.Margin = new System.Windows.Forms.Padding(0);
			this.lbl_Basepath.Name = "lbl_Basepath";
			this.lbl_Basepath.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
			this.lbl_Basepath.Size = new System.Drawing.Size(450, 15);
			this.lbl_Basepath.TabIndex = 7;
			this.lbl_Basepath.Text = "basepath";
			this.lbl_Basepath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lbl_Label
			// 
			this.lbl_Label.Location = new System.Drawing.Point(180, 2);
			this.lbl_Label.Margin = new System.Windows.Forms.Padding(0);
			this.lbl_Label.Name = "lbl_Label";
			this.lbl_Label.Size = new System.Drawing.Size(30, 20);
			this.lbl_Label.TabIndex = 3;
			this.lbl_Label.Text = "ufo_";
			this.lbl_Label.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// tb_Label
			// 
			this.tb_Label.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.tb_Label.Location = new System.Drawing.Point(210, 2);
			this.tb_Label.Margin = new System.Windows.Forms.Padding(0);
			this.tb_Label.Name = "tb_Label";
			this.tb_Label.Size = new System.Drawing.Size(240, 19);
			this.tb_Label.TabIndex = 4;
			this.tb_Label.WordWrap = false;
			this.tb_Label.TextChanged += new System.EventHandler(this.OnTypeTextChanged);
			// 
			// la_Target
			// 
			this.la_Target.Location = new System.Drawing.Point(5, 2);
			this.la_Target.Margin = new System.Windows.Forms.Padding(0);
			this.la_Target.Name = "la_Target";
			this.la_Target.Size = new System.Drawing.Size(45, 20);
			this.la_Target.TabIndex = 0;
			this.la_Target.Text = "Target";
			this.la_Target.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// pa_Target
			// 
			this.pa_Target.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.pa_Target.Controls.Add(this.rb_Ufos);
			this.pa_Target.Controls.Add(this.rb_Crafts);
			this.pa_Target.Controls.Add(this.rb_Terrains);
			this.pa_Target.Controls.Add(this.la_Target);
			this.pa_Target.Location = new System.Drawing.Point(5, 110);
			this.pa_Target.Margin = new System.Windows.Forms.Padding(0);
			this.pa_Target.Name = "pa_Target";
			this.pa_Target.Size = new System.Drawing.Size(485, 25);
			this.pa_Target.TabIndex = 5;
			// 
			// rb_Ufos
			// 
			this.rb_Ufos.Location = new System.Drawing.Point(200, 2);
			this.rb_Ufos.Margin = new System.Windows.Forms.Padding(0);
			this.rb_Ufos.Name = "rb_Ufos";
			this.rb_Ufos.Size = new System.Drawing.Size(50, 20);
			this.rb_Ufos.TabIndex = 3;
			this.rb_Ufos.Text = "ufos";
			this.rb_Ufos.UseVisualStyleBackColor = true;
			// 
			// rb_Crafts
			// 
			this.rb_Crafts.Location = new System.Drawing.Point(135, 2);
			this.rb_Crafts.Margin = new System.Windows.Forms.Padding(0);
			this.rb_Crafts.Name = "rb_Crafts";
			this.rb_Crafts.Size = new System.Drawing.Size(55, 20);
			this.rb_Crafts.TabIndex = 2;
			this.rb_Crafts.Text = "crafts";
			this.rb_Crafts.UseVisualStyleBackColor = true;
			// 
			// rb_Terrains
			// 
			this.rb_Terrains.Checked = true;
			this.rb_Terrains.Location = new System.Drawing.Point(60, 2);
			this.rb_Terrains.Margin = new System.Windows.Forms.Padding(0);
			this.rb_Terrains.Name = "rb_Terrains";
			this.rb_Terrains.Size = new System.Drawing.Size(65, 20);
			this.rb_Terrains.TabIndex = 1;
			this.rb_Terrains.TabStop = true;
			this.rb_Terrains.Text = "terrains";
			this.rb_Terrains.UseVisualStyleBackColor = true;
			// 
			// pa_Group
			// 
			this.pa_Group.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.pa_Group.Controls.Add(this.la_Group);
			this.pa_Group.Controls.Add(this.rb_Ufo);
			this.pa_Group.Controls.Add(this.tb_Label);
			this.pa_Group.Controls.Add(this.rb_Tftd);
			this.pa_Group.Controls.Add(this.lbl_Label);
			this.pa_Group.Location = new System.Drawing.Point(5, 85);
			this.pa_Group.Margin = new System.Windows.Forms.Padding(0);
			this.pa_Group.Name = "pa_Group";
			this.pa_Group.Size = new System.Drawing.Size(485, 25);
			this.pa_Group.TabIndex = 4;
			// 
			// RulesetConverter
			// 
			this.AcceptButton = this.btn_Convert;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btn_Cancel;
			this.ClientSize = new System.Drawing.Size(492, 209);
			this.Controls.Add(this.pa_Group);
			this.Controls.Add(this.pa_Target);
			this.Controls.Add(this.lbl_Basepath);
			this.Controls.Add(this.btn_Basepath);
			this.Controls.Add(this.cb_Basepath);
			this.Controls.Add(this.btn_Cancel);
			this.Controls.Add(this.btn_Convert);
			this.Controls.Add(this.tb_Input);
			this.Controls.Add(this.lbl_Input);
			this.Controls.Add(this.lbl_Info);
			this.Controls.Add(this.btn_Input);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MaximizeBox = false;
			this.MinimumSize = new System.Drawing.Size(400, 200);
			this.Name = "RulesetConverter";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "OxC/e Ruleset Converter";
			this.pa_Target.ResumeLayout(false);
			this.pa_Group.ResumeLayout(false);
			this.pa_Group.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion Designer
	}
}
