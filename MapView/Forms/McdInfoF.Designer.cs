using System;
using System.ComponentModel;
using System.Windows.Forms;


namespace MapView
{
	partial class McdInfoF
	{
		#region Designer
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private IContainer components;

		private RichTextBox rtbInfo;
		private BindingSource bsInfo;


		/// <summary>
		/// Cleans up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
				components.Dispose();

			base.Dispose(disposing);
		}


		/// <summary>
		/// Required method for Designer support - do not modify the contents of
		/// this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.rtbInfo = new System.Windows.Forms.RichTextBox();
			this.bsInfo = new System.Windows.Forms.BindingSource(this.components);
			((System.ComponentModel.ISupportInitialize)(this.bsInfo)).BeginInit();
			this.SuspendLayout();
			// 
			// rtbInfo
			// 
			this.rtbInfo.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rtbInfo.Font = new System.Drawing.Font("Courier New", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.rtbInfo.Location = new System.Drawing.Point(0, 0);
			this.rtbInfo.Name = "rtbInfo";
			this.rtbInfo.ReadOnly = true;
			this.rtbInfo.ShowSelectionMargin = true;
			this.rtbInfo.Size = new System.Drawing.Size(512, 609);
			this.rtbInfo.TabIndex = 0;
			this.rtbInfo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown_rtb);
			this.rtbInfo.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OnKeyUp_rtb);
			// 
			// bsInfo
			// 
			this.bsInfo.DataSource = typeof(XCom.McdRecord);
			// 
			// McdInfoF
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(512, 609);
			this.Controls.Add(this.rtbInfo);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "McdInfoF";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "MCD Info";
			((System.ComponentModel.ISupportInitialize)(this.bsInfo)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion Designer
	}
}
