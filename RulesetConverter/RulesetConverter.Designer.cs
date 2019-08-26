namespace RulesetConverter
{
	partial class RulesetConverter
	{
		#region Designer
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		private System.Windows.Forms.Button btn_Cancel;
		private System.Windows.Forms.Button btn_Convert;
		private System.Windows.Forms.TextBox tb_Input;
		private System.Windows.Forms.Button btn_Input;
		private System.Windows.Forms.Label lbl_Input;
		private System.Windows.Forms.Label lbl_Info;
		private System.Windows.Forms.RadioButton rb_Ufo;
		private System.Windows.Forms.RadioButton rb_Tftd;
		private System.Windows.Forms.Label lbl_GameType;
		private System.Windows.Forms.CheckBox cb_Basepath;
		private System.Windows.Forms.Button btn_Basepath;
		private System.Windows.Forms.Label lbl_Basepath;
		

		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
				components.Dispose();

			base.Dispose(disposing);
		}


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
			this.SuspendLayout();
			// 
			// btn_Cancel
			// 
			this.btn_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btn_Cancel.Location = new System.Drawing.Point(215, 147);
			this.btn_Cancel.Margin = new System.Windows.Forms.Padding(0);
			this.btn_Cancel.Name = "btn_Cancel";
			this.btn_Cancel.Size = new System.Drawing.Size(85, 30);
			this.btn_Cancel.TabIndex = 11;
			this.btn_Cancel.Text = "Cancel";
			this.btn_Cancel.UseVisualStyleBackColor = true;
			this.btn_Cancel.Click += new System.EventHandler(this.OnCancelClick);
			// 
			// btn_Convert
			// 
			this.btn_Convert.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btn_Convert.Enabled = false;
			this.btn_Convert.Location = new System.Drawing.Point(95, 147);
			this.btn_Convert.Margin = new System.Windows.Forms.Padding(0);
			this.btn_Convert.Name = "btn_Convert";
			this.btn_Convert.Size = new System.Drawing.Size(85, 30);
			this.btn_Convert.TabIndex = 10;
			this.btn_Convert.Text = "Convert";
			this.btn_Convert.UseVisualStyleBackColor = true;
			this.btn_Convert.Click += new System.EventHandler(this.OnConvertClick);
			// 
			// tb_Input
			// 
			this.tb_Input.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.tb_Input.Location = new System.Drawing.Point(0, 65);
			this.tb_Input.Margin = new System.Windows.Forms.Padding(0);
			this.tb_Input.Name = "tb_Input";
			this.tb_Input.ReadOnly = true;
			this.tb_Input.Size = new System.Drawing.Size(365, 19);
			this.tb_Input.TabIndex = 2;
			// 
			// btn_Input
			// 
			this.btn_Input.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btn_Input.Location = new System.Drawing.Point(365, 64);
			this.btn_Input.Margin = new System.Windows.Forms.Padding(0);
			this.btn_Input.Name = "btn_Input";
			this.btn_Input.Size = new System.Drawing.Size(25, 21);
			this.btn_Input.TabIndex = 3;
			this.btn_Input.Text = "...";
			this.btn_Input.UseVisualStyleBackColor = true;
			this.btn_Input.Click += new System.EventHandler(this.OnFindInputClick);
			// 
			// lbl_Input
			// 
			this.lbl_Input.Dock = System.Windows.Forms.DockStyle.Top;
			this.lbl_Input.Location = new System.Drawing.Point(0, 45);
			this.lbl_Input.Margin = new System.Windows.Forms.Padding(0);
			this.lbl_Input.Name = "lbl_Input";
			this.lbl_Input.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.lbl_Input.Size = new System.Drawing.Size(392, 20);
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
			this.lbl_Info.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.lbl_Info.Size = new System.Drawing.Size(392, 45);
			this.lbl_Info.TabIndex = 0;
			this.lbl_Info.Text = "This app inputs an OxC ruleset and converts any tilesets it has out to MapTileset" +
	"s.tpl, which is a YAML configuration (template) file for MapView 2+";
			this.lbl_Info.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// rb_Ufo
			// 
			this.rb_Ufo.Checked = true;
			this.rb_Ufo.Location = new System.Drawing.Point(80, 85);
			this.rb_Ufo.Margin = new System.Windows.Forms.Padding(0);
			this.rb_Ufo.Name = "rb_Ufo";
			this.rb_Ufo.Size = new System.Drawing.Size(50, 20);
			this.rb_Ufo.TabIndex = 5;
			this.rb_Ufo.TabStop = true;
			this.rb_Ufo.Text = "UFO";
			this.rb_Ufo.UseVisualStyleBackColor = true;
			// 
			// rb_Tftd
			// 
			this.rb_Tftd.Location = new System.Drawing.Point(130, 85);
			this.rb_Tftd.Margin = new System.Windows.Forms.Padding(0);
			this.rb_Tftd.Name = "rb_Tftd";
			this.rb_Tftd.Size = new System.Drawing.Size(60, 20);
			this.rb_Tftd.TabIndex = 6;
			this.rb_Tftd.Text = "TFTD";
			this.rb_Tftd.UseVisualStyleBackColor = true;
			// 
			// lbl_GameType
			// 
			this.lbl_GameType.Location = new System.Drawing.Point(5, 85);
			this.lbl_GameType.Name = "lbl_GameType";
			this.lbl_GameType.Size = new System.Drawing.Size(70, 20);
			this.lbl_GameType.TabIndex = 4;
			this.lbl_GameType.Text = "GameType";
			this.lbl_GameType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// cb_Basepath
			// 
			this.cb_Basepath.Location = new System.Drawing.Point(5, 105);
			this.cb_Basepath.Margin = new System.Windows.Forms.Padding(0);
			this.cb_Basepath.Name = "cb_Basepath";
			this.cb_Basepath.Size = new System.Drawing.Size(105, 20);
			this.cb_Basepath.TabIndex = 7;
			this.cb_Basepath.Text = "add Basepath";
			this.cb_Basepath.UseVisualStyleBackColor = true;
			this.cb_Basepath.CheckedChanged += new System.EventHandler(this.OnBasepathCheckChanged);
			// 
			// btn_Basepath
			// 
			this.btn_Basepath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btn_Basepath.Enabled = false;
			this.btn_Basepath.Location = new System.Drawing.Point(365, 119);
			this.btn_Basepath.Margin = new System.Windows.Forms.Padding(0);
			this.btn_Basepath.Name = "btn_Basepath";
			this.btn_Basepath.Size = new System.Drawing.Size(25, 21);
			this.btn_Basepath.TabIndex = 9;
			this.btn_Basepath.Text = "...";
			this.btn_Basepath.UseVisualStyleBackColor = true;
			this.btn_Basepath.Click += new System.EventHandler(this.OnFindBasepathClick);
			// 
			// lbl_Basepath
			// 
			this.lbl_Basepath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.lbl_Basepath.Location = new System.Drawing.Point(20, 125);
			this.lbl_Basepath.Name = "lbl_Basepath";
			this.lbl_Basepath.Size = new System.Drawing.Size(345, 15);
			this.lbl_Basepath.TabIndex = 8;
			// 
			// RulesetConverter
			// 
			this.AcceptButton = this.btn_Convert;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btn_Cancel;
			this.ClientSize = new System.Drawing.Size(392, 179);
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
			this.MinimumSize = new System.Drawing.Size(400, 205);
			this.Name = "RulesetConverter";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "RulesetConverter";
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion Designer
	}
}
