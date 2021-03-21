using System;
using System.Windows.Forms;


namespace DSShared
{
	public sealed partial class Infobox
	{
		#region Designer
		private Panel pa_Copyable;
		private RichTextBox rt_Copyable;
		private Button bu_Cancel;
		private Button bu_Okay;
		private Button bu_Retry;

		/// <summary>
		/// Required method for Designer support - do not modify the contents of
		/// this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.pa_Copyable = new System.Windows.Forms.Panel();
			this.rt_Copyable = new System.Windows.Forms.RichTextBox();
			this.bu_Cancel = new System.Windows.Forms.Button();
			this.bu_Okay = new System.Windows.Forms.Button();
			this.bu_Retry = new System.Windows.Forms.Button();
			this.pa_Copyable.SuspendLayout();
			this.SuspendLayout();
			// 
			// pa_Copyable
			// 
			this.pa_Copyable.Controls.Add(this.rt_Copyable);
			this.pa_Copyable.Dock = System.Windows.Forms.DockStyle.Top;
			this.pa_Copyable.Location = new System.Drawing.Point(0, 0);
			this.pa_Copyable.Margin = new System.Windows.Forms.Padding(0);
			this.pa_Copyable.Name = "pa_Copyable";
			this.pa_Copyable.Padding = new System.Windows.Forms.Padding(18, 9, 0, 5);
			this.pa_Copyable.Size = new System.Drawing.Size(394, 130);
			this.pa_Copyable.TabIndex = 1;
			this.pa_Copyable.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPaintPanel);
			// 
			// rt_Copyable
			// 
			this.rt_Copyable.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.rt_Copyable.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rt_Copyable.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.rt_Copyable.HideSelection = false;
			this.rt_Copyable.Location = new System.Drawing.Point(18, 9);
			this.rt_Copyable.Margin = new System.Windows.Forms.Padding(0);
			this.rt_Copyable.Name = "rt_Copyable";
			this.rt_Copyable.ReadOnly = true;
			this.rt_Copyable.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.rt_Copyable.Size = new System.Drawing.Size(376, 116);
			this.rt_Copyable.TabIndex = 0;
			this.rt_Copyable.Text = "";
			this.rt_Copyable.WordWrap = false;
			// 
			// bu_Cancel
			// 
			this.bu_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.bu_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.bu_Cancel.Location = new System.Drawing.Point(296, 150);
			this.bu_Cancel.Margin = new System.Windows.Forms.Padding(0, 3, 0, 2);
			this.bu_Cancel.Name = "bu_Cancel";
			this.bu_Cancel.Size = new System.Drawing.Size(95, 25);
			this.bu_Cancel.TabIndex = 4;
			this.bu_Cancel.Text = "cancel";
			this.bu_Cancel.UseVisualStyleBackColor = true;
			this.bu_Cancel.Click += new System.EventHandler(this.OnCancelClick);
			// 
			// bu_Okay
			// 
			this.bu_Okay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.bu_Okay.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.bu_Okay.Location = new System.Drawing.Point(196, 150);
			this.bu_Okay.Margin = new System.Windows.Forms.Padding(0);
			this.bu_Okay.Name = "bu_Okay";
			this.bu_Okay.Size = new System.Drawing.Size(95, 25);
			this.bu_Okay.TabIndex = 3;
			this.bu_Okay.Text = "ok";
			this.bu_Okay.UseVisualStyleBackColor = true;
			this.bu_Okay.Visible = false;
			this.bu_Okay.Click += new System.EventHandler(this.OnOkayClick);
			// 
			// bu_Retry
			// 
			this.bu_Retry.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.bu_Retry.DialogResult = System.Windows.Forms.DialogResult.Yes;
			this.bu_Retry.Location = new System.Drawing.Point(97, 150);
			this.bu_Retry.Margin = new System.Windows.Forms.Padding(0);
			this.bu_Retry.Name = "bu_Retry";
			this.bu_Retry.Size = new System.Drawing.Size(95, 25);
			this.bu_Retry.TabIndex = 2;
			this.bu_Retry.Text = "retry";
			this.bu_Retry.UseVisualStyleBackColor = true;
			this.bu_Retry.Visible = false;
			this.bu_Retry.Click += new System.EventHandler(this.OnRetryClick);
			// 
			// Infobox
			// 
			this.CancelButton = this.bu_Cancel;
			this.ClientSize = new System.Drawing.Size(394, 176);
			this.Controls.Add(this.pa_Copyable);
			this.Controls.Add(this.bu_Cancel);
			this.Controls.Add(this.bu_Okay);
			this.Controls.Add(this.bu_Retry);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Infobox";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.pa_Copyable.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion Designer
	}
}
