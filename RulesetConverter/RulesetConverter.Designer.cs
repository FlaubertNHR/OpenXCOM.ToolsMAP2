﻿using System;
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
		private Label lbl_GameType;
		private CheckBox cb_Basepath;
		private Button btn_Basepath;
		private Label lbl_Basepath;
		private Label lbl_Label;
		private TextBox tb_Label;

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
			this.lbl_GameType = new System.Windows.Forms.Label();
			this.cb_Basepath = new System.Windows.Forms.CheckBox();
			this.btn_Basepath = new System.Windows.Forms.Button();
			this.lbl_Basepath = new System.Windows.Forms.Label();
			this.lbl_Label = new System.Windows.Forms.Label();
			this.tb_Label = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// btn_Cancel
			// 
			this.btn_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btn_Cancel.Location = new System.Drawing.Point(315, 142);
			this.btn_Cancel.Margin = new System.Windows.Forms.Padding(0);
			this.btn_Cancel.Name = "btn_Cancel";
			this.btn_Cancel.Size = new System.Drawing.Size(85, 30);
			this.btn_Cancel.TabIndex = 13;
			this.btn_Cancel.Text = "Cancel";
			this.btn_Cancel.UseVisualStyleBackColor = true;
			this.btn_Cancel.Click += new System.EventHandler(this.OnCancelClick);
			// 
			// btn_Convert
			// 
			this.btn_Convert.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btn_Convert.Enabled = false;
			this.btn_Convert.Location = new System.Drawing.Point(95, 140);
			this.btn_Convert.Margin = new System.Windows.Forms.Padding(0);
			this.btn_Convert.Name = "btn_Convert";
			this.btn_Convert.Size = new System.Drawing.Size(85, 30);
			this.btn_Convert.TabIndex = 12;
			this.btn_Convert.Text = "Convert";
			this.btn_Convert.UseVisualStyleBackColor = true;
			this.btn_Convert.Click += new System.EventHandler(this.OnConvertClick);
			// 
			// tb_Input
			// 
			this.tb_Input.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.tb_Input.Location = new System.Drawing.Point(5, 50);
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
			this.btn_Input.Location = new System.Drawing.Point(460, 49);
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
			this.lbl_Input.Location = new System.Drawing.Point(0, 30);
			this.lbl_Input.Margin = new System.Windows.Forms.Padding(0);
			this.lbl_Input.Name = "lbl_Input";
			this.lbl_Input.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.lbl_Input.Size = new System.Drawing.Size(492, 20);
			this.lbl_Input.TabIndex = 1;
			this.lbl_Input.Text = "File to convert";
			this.lbl_Input.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lbl_Info
			// 
			this.lbl_Info.Dock = System.Windows.Forms.DockStyle.Top;
			this.lbl_Info.Location = new System.Drawing.Point(0, 0);
			this.lbl_Info.Margin = new System.Windows.Forms.Padding(0);
			this.lbl_Info.Name = "lbl_Info";
			this.lbl_Info.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.lbl_Info.Size = new System.Drawing.Size(492, 30);
			this.lbl_Info.TabIndex = 0;
			this.lbl_Info.Text = "Info";
			this.lbl_Info.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// rb_Ufo
			// 
			this.rb_Ufo.Checked = true;
			this.rb_Ufo.Location = new System.Drawing.Point(75, 70);
			this.rb_Ufo.Margin = new System.Windows.Forms.Padding(0);
			this.rb_Ufo.Name = "rb_Ufo";
			this.rb_Ufo.Size = new System.Drawing.Size(50, 20);
			this.rb_Ufo.TabIndex = 5;
			this.rb_Ufo.TabStop = true;
			this.rb_Ufo.Text = "UFO";
			this.rb_Ufo.UseVisualStyleBackColor = true;
			this.rb_Ufo.CheckedChanged += new System.EventHandler(this.OnTypeChanged);
			// 
			// rb_Tftd
			// 
			this.rb_Tftd.Location = new System.Drawing.Point(125, 70);
			this.rb_Tftd.Margin = new System.Windows.Forms.Padding(0);
			this.rb_Tftd.Name = "rb_Tftd";
			this.rb_Tftd.Size = new System.Drawing.Size(55, 20);
			this.rb_Tftd.TabIndex = 6;
			this.rb_Tftd.Text = "TFTD";
			this.rb_Tftd.UseVisualStyleBackColor = true;
			this.rb_Tftd.CheckedChanged += new System.EventHandler(this.OnTypeChanged);
			// 
			// lbl_GameType
			// 
			this.lbl_GameType.Location = new System.Drawing.Point(5, 70);
			this.lbl_GameType.Name = "lbl_GameType";
			this.lbl_GameType.Size = new System.Drawing.Size(65, 20);
			this.lbl_GameType.TabIndex = 4;
			this.lbl_GameType.Text = "GameType";
			this.lbl_GameType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// cb_Basepath
			// 
			this.cb_Basepath.Location = new System.Drawing.Point(10, 95);
			this.cb_Basepath.Margin = new System.Windows.Forms.Padding(0);
			this.cb_Basepath.Name = "cb_Basepath";
			this.cb_Basepath.Size = new System.Drawing.Size(105, 20);
			this.cb_Basepath.TabIndex = 9;
			this.cb_Basepath.Text = "add Basepath";
			this.cb_Basepath.UseVisualStyleBackColor = true;
			this.cb_Basepath.CheckedChanged += new System.EventHandler(this.OnBasepathCheckChanged);
			// 
			// btn_Basepath
			// 
			this.btn_Basepath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btn_Basepath.Enabled = false;
			this.btn_Basepath.Location = new System.Drawing.Point(460, 114);
			this.btn_Basepath.Margin = new System.Windows.Forms.Padding(0);
			this.btn_Basepath.Name = "btn_Basepath";
			this.btn_Basepath.Size = new System.Drawing.Size(30, 26);
			this.btn_Basepath.TabIndex = 11;
			this.btn_Basepath.Text = "...";
			this.btn_Basepath.UseVisualStyleBackColor = true;
			this.btn_Basepath.Click += new System.EventHandler(this.OnFindBasepathClick);
			// 
			// lbl_Basepath
			// 
			this.lbl_Basepath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.lbl_Basepath.Location = new System.Drawing.Point(10, 120);
			this.lbl_Basepath.Name = "lbl_Basepath";
			this.lbl_Basepath.Size = new System.Drawing.Size(445, 15);
			this.lbl_Basepath.TabIndex = 10;
			this.lbl_Basepath.Text = "basepath";
			// 
			// lbl_Label
			// 
			this.lbl_Label.Location = new System.Drawing.Point(180, 70);
			this.lbl_Label.Margin = new System.Windows.Forms.Padding(0);
			this.lbl_Label.Name = "lbl_Label";
			this.lbl_Label.Size = new System.Drawing.Size(35, 20);
			this.lbl_Label.TabIndex = 7;
			this.lbl_Label.Text = "ufo_";
			this.lbl_Label.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// tb_Label
			// 
			this.tb_Label.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.tb_Label.Location = new System.Drawing.Point(215, 70);
			this.tb_Label.Margin = new System.Windows.Forms.Padding(0);
			this.tb_Label.Name = "tb_Label";
			this.tb_Label.Size = new System.Drawing.Size(240, 19);
			this.tb_Label.TabIndex = 8;
			this.tb_Label.WordWrap = false;
			this.tb_Label.TextChanged += new System.EventHandler(this.OnTypeTextChanged);
			// 
			// RulesetConverter
			// 
			this.AcceptButton = this.btn_Convert;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btn_Cancel;
			this.ClientSize = new System.Drawing.Size(492, 174);
			this.Controls.Add(this.tb_Label);
			this.Controls.Add(this.lbl_Label);
			this.Controls.Add(this.lbl_Basepath);
			this.Controls.Add(this.btn_Basepath);
			this.Controls.Add(this.cb_Basepath);
			this.Controls.Add(this.lbl_GameType);
			this.Controls.Add(this.rb_Tftd);
			this.Controls.Add(this.rb_Ufo);
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
			this.Text = "RulesetConverter";
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion Designer
	}
}
