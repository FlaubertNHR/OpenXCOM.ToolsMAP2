using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;


namespace XCom
{
	/// <summary>
	/// A generic outputbox for Errors/Warnings/Info.
	/// The point is to stop wrapping long path-strings like the stock .NET
	/// MessageBox does.
	/// </summary>
	internal sealed class Infobox
		:
			Form
	{
		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		internal Infobox(string title, string label, string copyable)
		{
			InitializeComponent();

			Text = title;
			lbl_Info.Text = label;
			rtb_Info.Text = copyable;


			string[] lines = copyable.Split( // deter width based on copyable text
										new[]{ "\r\n", "\r", "\n" },
										StringSplitOptions.RemoveEmptyEntries);

			int width = 0, test;
			foreach (var line in lines)
			{
				if ((test = TextRenderer.MeasureText(line, rtb_Info.Font).Width) > width)
					width = test;
			}
			if (width < 350) width = 350;
			var size = new Size(width, Int32.MaxValue);


			lines = label.Split(
							new[]{ "\r\n", "\r", "\n" },
							StringSplitOptions.None);

			float heightF;
			int
				expectation = 0,
				height = 0,
				total = 0;

			Graphics graphics = CreateGraphics();
			foreach (var line in lines) // deter height of label control
			{
				heightF = graphics.MeasureString(line, lbl_Info.Font, size).Height; // NOTE: TextRenderer ain't workin right for that.
				height = (int)Math.Ceiling(heightF);

				if (expectation == 0)
					expectation = height; // IMPORTANT: 1st line shall not be blank.
				else if (height == 0)
					height = expectation;

				total += height;
			}
			lbl_Info.Height = total + 25; // +25 for padding real and imagined.

			rtb_Info.Height = TextRenderer.MeasureText( // deter height of copyable control
													rtb_Info.Text,
													rtb_Info.Font,
													size).Height;

			ClientSize = new Size(
								width + 20,
								lbl_Info.Height + rtb_Info.Height + btn_Okay.Height + 15);


			DialogResult = DialogResult.OK;
		}
		#endregion


		#region Events (override)
		protected override void OnLoad(EventArgs e)
		{
			rtb_Info.AutoWordSelection = false;
			rtb_Info.Select();
			rtb_Info.SelectionStart = 0;
		}
		#endregion Events (override)



		#region Windows Form Designer generated code
		private Container components = null;
		private Button btn_Okay;
		private Label lbl_Info;
		private System.Windows.Forms.RichTextBox rtb_Info;
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
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
			this.btn_Okay = new System.Windows.Forms.Button();
			this.lbl_Info = new System.Windows.Forms.Label();
			this.rtb_Info = new System.Windows.Forms.RichTextBox();
			this.SuspendLayout();
			// 
			// btn_Okay
			// 
			this.btn_Okay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btn_Okay.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btn_Okay.Location = new System.Drawing.Point(407, 247);
			this.btn_Okay.Margin = new System.Windows.Forms.Padding(0);
			this.btn_Okay.Name = "btn_Okay";
			this.btn_Okay.Size = new System.Drawing.Size(80, 25);
			this.btn_Okay.TabIndex = 0;
			this.btn_Okay.Text = "ok";
			this.btn_Okay.UseVisualStyleBackColor = true;
			// 
			// lbl_Info
			// 
			this.lbl_Info.Dock = System.Windows.Forms.DockStyle.Top;
			this.lbl_Info.Location = new System.Drawing.Point(0, 0);
			this.lbl_Info.Margin = new System.Windows.Forms.Padding(0);
			this.lbl_Info.Name = "lbl_Info";
			this.lbl_Info.Padding = new System.Windows.Forms.Padding(10, 10, 15, 5);
			this.lbl_Info.Size = new System.Drawing.Size(494, 40);
			this.lbl_Info.TabIndex = 1;
			this.lbl_Info.Text = "lbl_Info";
			// 
			// rtb_Info
			// 
			this.rtb_Info.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.rtb_Info.Dock = System.Windows.Forms.DockStyle.Top;
			this.rtb_Info.Font = new System.Drawing.Font("Courier New", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.rtb_Info.HideSelection = false;
			this.rtb_Info.Location = new System.Drawing.Point(0, 40);
			this.rtb_Info.Margin = new System.Windows.Forms.Padding(0);
			this.rtb_Info.Name = "rtb_Info";
			this.rtb_Info.ReadOnly = true;
			this.rtb_Info.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
			this.rtb_Info.ShowSelectionMargin = true;
			this.rtb_Info.Size = new System.Drawing.Size(494, 130);
			this.rtb_Info.TabIndex = 2;
			this.rtb_Info.Text = "rtb_Info";
			this.rtb_Info.WordWrap = false;
			// 
			// Infobox
			// 
			this.AcceptButton = this.btn_Okay;
			this.AutoScroll = true;
			this.CancelButton = this.btn_Okay;
			this.ClientSize = new System.Drawing.Size(494, 276);
			this.Controls.Add(this.rtb_Info);
			this.Controls.Add(this.btn_Okay);
			this.Controls.Add(this.lbl_Info);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Infobox";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.ResumeLayout(false);

		}
		#endregion
	}
}
