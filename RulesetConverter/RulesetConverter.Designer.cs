namespace RulesetConverter
{
	partial class RulesetConverter
	{
		#region Designer
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnConvert;
		private System.Windows.Forms.TextBox tbInput;
		private System.Windows.Forms.Button btnInput;
		private System.Windows.Forms.Label lblInput;
		private System.Windows.Forms.Label lblResult;
		private System.Windows.Forms.Label lblInfo;
		private System.Windows.Forms.RadioButton rb_Ufo;
		private System.Windows.Forms.RadioButton rb_Tftd;
		private System.Windows.Forms.Label lbl_GameType;
		

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
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnConvert = new System.Windows.Forms.Button();
			this.tbInput = new System.Windows.Forms.TextBox();
			this.btnInput = new System.Windows.Forms.Button();
			this.lblInput = new System.Windows.Forms.Label();
			this.lblResult = new System.Windows.Forms.Label();
			this.lblInfo = new System.Windows.Forms.Label();
			this.rb_Ufo = new System.Windows.Forms.RadioButton();
			this.rb_Tftd = new System.Windows.Forms.RadioButton();
			this.lbl_GameType = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(215, 137);
			this.btnCancel.Margin = new System.Windows.Forms.Padding(0);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(85, 30);
			this.btnCancel.TabIndex = 9;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.OnCancelClick);
			// 
			// btnConvert
			// 
			this.btnConvert.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnConvert.Enabled = false;
			this.btnConvert.Location = new System.Drawing.Point(95, 137);
			this.btnConvert.Margin = new System.Windows.Forms.Padding(0);
			this.btnConvert.Name = "btnConvert";
			this.btnConvert.Size = new System.Drawing.Size(85, 30);
			this.btnConvert.TabIndex = 8;
			this.btnConvert.Text = "Convert";
			this.btnConvert.UseVisualStyleBackColor = true;
			this.btnConvert.Click += new System.EventHandler(this.OnConvertClick);
			// 
			// tbInput
			// 
			this.tbInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.tbInput.Location = new System.Drawing.Point(0, 70);
			this.tbInput.Margin = new System.Windows.Forms.Padding(0);
			this.tbInput.Name = "tbInput";
			this.tbInput.ReadOnly = true;
			this.tbInput.Size = new System.Drawing.Size(365, 19);
			this.tbInput.TabIndex = 2;
			// 
			// btnInput
			// 
			this.btnInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnInput.Location = new System.Drawing.Point(365, 69);
			this.btnInput.Margin = new System.Windows.Forms.Padding(0);
			this.btnInput.Name = "btnInput";
			this.btnInput.Size = new System.Drawing.Size(25, 21);
			this.btnInput.TabIndex = 3;
			this.btnInput.Text = "...";
			this.btnInput.UseVisualStyleBackColor = true;
			this.btnInput.Click += new System.EventHandler(this.OnFindInputClick);
			// 
			// lblInput
			// 
			this.lblInput.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblInput.Location = new System.Drawing.Point(0, 45);
			this.lblInput.Margin = new System.Windows.Forms.Padding(0);
			this.lblInput.Name = "lblInput";
			this.lblInput.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.lblInput.Size = new System.Drawing.Size(392, 25);
			this.lblInput.TabIndex = 1;
			this.lblInput.Text = "File to convert";
			this.lblInput.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// lblResult
			// 
			this.lblResult.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.lblResult.Location = new System.Drawing.Point(5, 120);
			this.lblResult.Margin = new System.Windows.Forms.Padding(0);
			this.lblResult.Name = "lblResult";
			this.lblResult.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.lblResult.Size = new System.Drawing.Size(385, 15);
			this.lblResult.TabIndex = 7;
			// 
			// lblInfo
			// 
			this.lblInfo.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblInfo.Location = new System.Drawing.Point(0, 0);
			this.lblInfo.Margin = new System.Windows.Forms.Padding(0);
			this.lblInfo.Name = "lblInfo";
			this.lblInfo.Padding = new System.Windows.Forms.Padding(3, 2, 0, 2);
			this.lblInfo.Size = new System.Drawing.Size(392, 45);
			this.lblInfo.TabIndex = 0;
			this.lblInfo.Text = "This app inputs an OxC ruleset and converts any tilesets it has out to MapTileset" +
	"s.tpl, which is a template for a YAML configuration file for MapView 2+";
			this.lblInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// rb_Ufo
			// 
			this.rb_Ufo.Checked = true;
			this.rb_Ufo.Location = new System.Drawing.Point(95, 95);
			this.rb_Ufo.Margin = new System.Windows.Forms.Padding(0);
			this.rb_Ufo.Name = "rb_Ufo";
			this.rb_Ufo.Size = new System.Drawing.Size(60, 20);
			this.rb_Ufo.TabIndex = 5;
			this.rb_Ufo.TabStop = true;
			this.rb_Ufo.Text = "UFO";
			this.rb_Ufo.UseVisualStyleBackColor = true;
			// 
			// rb_Tftd
			// 
			this.rb_Tftd.Location = new System.Drawing.Point(160, 95);
			this.rb_Tftd.Margin = new System.Windows.Forms.Padding(0);
			this.rb_Tftd.Name = "rb_Tftd";
			this.rb_Tftd.Size = new System.Drawing.Size(60, 20);
			this.rb_Tftd.TabIndex = 6;
			this.rb_Tftd.Text = "TFTD";
			this.rb_Tftd.UseVisualStyleBackColor = true;
			// 
			// lbl_GameType
			// 
			this.lbl_GameType.Location = new System.Drawing.Point(15, 95);
			this.lbl_GameType.Name = "lbl_GameType";
			this.lbl_GameType.Size = new System.Drawing.Size(75, 20);
			this.lbl_GameType.TabIndex = 4;
			this.lbl_GameType.Text = "GameType";
			this.lbl_GameType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// RulesetConverter
			// 
			this.AcceptButton = this.btnConvert;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(392, 169);
			this.Controls.Add(this.lbl_GameType);
			this.Controls.Add(this.rb_Tftd);
			this.Controls.Add(this.rb_Ufo);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnConvert);
			this.Controls.Add(this.tbInput);
			this.Controls.Add(this.lblInput);
			this.Controls.Add(this.lblResult);
			this.Controls.Add(this.lblInfo);
			this.Controls.Add(this.btnInput);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "RulesetConverter";
			this.Text = "RulesetConverter";
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion Designer
	}
}
