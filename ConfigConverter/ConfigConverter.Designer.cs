using System;
using System.Windows.Forms;


namespace ConfigConverter
{
	internal sealed partial class ConfigConverter
	{
		#region Designer
		private Button btnCancel;
		private Button btnConvert;
		private TextBox tbInput;
		private Button btnInput;
		private Label lblInput;
		private Label lblResult;
		private Label lblInfo;


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
			this.SuspendLayout();
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(215, 127);
			this.btnCancel.Margin = new System.Windows.Forms.Padding(0);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(85, 30);
			this.btnCancel.TabIndex = 6;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.OnCancelClick);
			// 
			// btnConvert
			// 
			this.btnConvert.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnConvert.Enabled = false;
			this.btnConvert.Location = new System.Drawing.Point(95, 127);
			this.btnConvert.Margin = new System.Windows.Forms.Padding(0);
			this.btnConvert.Name = "btnConvert";
			this.btnConvert.Size = new System.Drawing.Size(85, 30);
			this.btnConvert.TabIndex = 5;
			this.btnConvert.Text = "Convert";
			this.btnConvert.UseVisualStyleBackColor = true;
			this.btnConvert.Click += new System.EventHandler(this.OnConvertClick);
			// 
			// tbInput
			// 
			this.tbInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.tbInput.Location = new System.Drawing.Point(0, 85);
			this.tbInput.Margin = new System.Windows.Forms.Padding(0);
			this.tbInput.Name = "tbInput";
			this.tbInput.ReadOnly = true;
			this.tbInput.Size = new System.Drawing.Size(365, 19);
			this.tbInput.TabIndex = 2;
			// 
			// btnInput
			// 
			this.btnInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnInput.Location = new System.Drawing.Point(365, 84);
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
			this.lblInput.Location = new System.Drawing.Point(0, 55);
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
			this.lblResult.Location = new System.Drawing.Point(5, 110);
			this.lblResult.Margin = new System.Windows.Forms.Padding(0);
			this.lblResult.Name = "lblResult";
			this.lblResult.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.lblResult.Size = new System.Drawing.Size(385, 15);
			this.lblResult.TabIndex = 4;
			// 
			// lblInfo
			// 
			this.lblInfo.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblInfo.Location = new System.Drawing.Point(0, 0);
			this.lblInfo.Margin = new System.Windows.Forms.Padding(0);
			this.lblInfo.Name = "lblInfo";
			this.lblInfo.Padding = new System.Windows.Forms.Padding(3, 2, 0, 2);
			this.lblInfo.Size = new System.Drawing.Size(392, 55);
			this.lblInfo.TabIndex = 0;
			this.lblInfo.Text = "This app inputs MapEdit.dat and converts it out to MapTilesets.yml, a YAML config" +
	"uration file for MapView 2+\r\n\r\nImages.dat and Paths.pth must be in the directory" +
	" with MapEdit.dat";
			this.lblInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// ConfigConverter
			// 
			this.AcceptButton = this.btnConvert;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(392, 159);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnConvert);
			this.Controls.Add(this.tbInput);
			this.Controls.Add(this.lblInput);
			this.Controls.Add(this.lblResult);
			this.Controls.Add(this.lblInfo);
			this.Controls.Add(this.btnInput);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MaximizeBox = false;
			this.MinimumSize = new System.Drawing.Size(400, 185);
			this.Name = "ConfigConverter";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "ConfigConverter2";
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion Designer
	}
}
