using System;
//using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using XCom;


namespace MapView
{
	/// <summary>
	/// 
	/// </summary>
	internal sealed class MapInfoDetailDialog
		:
			Form
	{
		#region Fields (static)
		private const int WIDTH_Min = 325;
		private const int WIDTH_Max = 900;
		private const int HIGHT_Min = 130;
		private const int HIGHT_Max = 500;

		private const int pad_HORI = 10; // pad real and imagined
		private const int pad_VERT = 10; // pad above Cancel button

		private static int _x = -1;
		private static int _y = -1;

		private const string HEIGHT_TEST = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";
		#endregion Fields (static)


		#region Fields
		private MapInfoDialog _finfo;
		#endregion Fields


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="finfo"></param>
		internal MapInfoDetailDialog(MapInfoDialog finfo)
		{
			InitializeComponent();
			_finfo = finfo;
		}


		/// <summary>
		/// Sets the text on the titlebar.
		/// </summary>
		/// <param name="text"></param>
		internal void SetTitleText(string text)
		{
			Text = text;
		}

		/// <summary>
		/// Sets the text of the header-label.
		/// @note Don't use lengthy texts or it will overrun the label-area.
		/// </summary>
		/// <param name="text"></param>
		internal void SetHeaderText(string text)
		{
			lbl_Header.Text = text;
		}

		/// <summary>
		/// Sets the copyable text that appears in the RichTextBox.
		/// </summary>
		/// <param name="text"></param>
		internal void SetCopyableText(string text)
		{
			rtb_Copyable.Text = text;
		}
		#endregion


		#region Events (override)
		/// <summary>
		/// Handles this dialog's load event. Niceties ...
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLoad(EventArgs e)
		{
			int w = GetWidth(rtb_Copyable.Text) + 30;					// +30 = parent panel's pad left+right +5
			pnl_Copyable.Height = GetHeight(rtb_Copyable.Text) + 20;	// +20 = parent panel's pad top+bot +5

			if      (w < WIDTH_Min) w = WIDTH_Min;
			else if (w > WIDTH_Max) w = WIDTH_Max;

			int h = pnl_Head.Height + pnl_Copyable.Height + btn_Cancel.Height;
			if      (h < HIGHT_Min) h = HIGHT_Min;
			else if (h > HIGHT_Max) h = HIGHT_Max;

			ClientSize = new Size(w + pad_HORI,
								  h + pad_VERT);

			if (_x == -1) _x = 200;
			if (_y == -1) _y = 100;

			Left = _x;
			Top  = _y;


			int widthborder = (Width  - ClientSize.Width) / 2;
			int heighttitle = (Height - ClientSize.Height - 2 * widthborder);

			MinimumSize = new Size(WIDTH_Min + pad_HORI + 2 * widthborder,
								   HIGHT_Min + pad_VERT + 2 * widthborder + heighttitle);

			rtb_Copyable.AutoWordSelection = false; // <- needs to be here not in the cTor or designer to work right.
			rtb_Copyable.Select();
		}

		protected override void OnResize(EventArgs e)
		{
			pnl_Copyable.Height = ClientSize.Height
								- pnl_Head  .Height
								- btn_Cancel.Height - pad_VERT;
			base.OnResize(e);
			pnl_Copyable.Invalidate();
		}

		/// <summary>
		/// Handles this dialog's closing event. Sets the static location and
		/// nulls the dialog in MapInfoDialog.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			_x = Left;
			_y = Top;

			_finfo._fdetail = null;

			base.OnFormClosing(e);
		}
		#endregion Events (override)


		#region Events
		/// <summary>
		/// Handles a click on the Cancel button. Closes this dialog harmlessly.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void click_btnCancel(object sender, EventArgs e)
		{
			Close();
		}

/*		/// <summary>
		/// Draws a 1px border around the copyable-panel.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void paint_CopyPanel(object sender, PaintEventArgs e)
		{
			int w = pnl_Copyable.Width  - 1;
			int h = pnl_Copyable.Height - 1;

			var tl = new Point(0, 0);
			var tr = new Point(w, 0);
			var br = new Point(w, h);
			var bl = new Point(0, h);

			var graphics = e.Graphics;
			graphics.DrawLine(Pencils.DarkLine, tl, tr);
			graphics.DrawLine(Pencils.DarkLine, tr, br);
			graphics.DrawLine(Pencils.DarkLine, br, bl);
			graphics.DrawLine(Pencils.DarkLine, bl, tl);
		} */
		#endregion Events


		#region Methods
		/// <summary>
		/// Deters width based on longest copyable line.
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		int GetWidth(string text)
		{
			string[] lines = text.Split(GlobalsXC.CRandorLF, StringSplitOptions.RemoveEmptyEntries);

			int width = 0, widthtest;
			foreach (var line in lines)
			{
				if ((widthtest = TextRenderer.MeasureText(line, rtb_Copyable.Font).Width) > width)
					width = widthtest;
			}
			return width;
		}

		/// <summary>
		/// Deters height based on line-height * lines.
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		int GetHeight(string text)
		{
			string[] lines = text.Split(GlobalsXC.CRandorLF, StringSplitOptions.None);

			return TextRenderer.MeasureText(HEIGHT_TEST, rtb_Copyable.Font).Height
				 * lines.Length;
		}
		#endregion Methods



		#region Designer
		private Container components = null;

		private Panel pnl_Head;
		private RichTextBox rtb_Copyable;
		private Panel pnl_Copyable;
		private Button btn_Cancel;
		private Label lbl_Header;

		/// <summary>
		/// Cleans up any resources being used.
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
			this.pnl_Head = new System.Windows.Forms.Panel();
			this.lbl_Header = new System.Windows.Forms.Label();
			this.pnl_Copyable = new System.Windows.Forms.Panel();
			this.rtb_Copyable = new System.Windows.Forms.RichTextBox();
			this.btn_Cancel = new System.Windows.Forms.Button();
			this.pnl_Head.SuspendLayout();
			this.pnl_Copyable.SuspendLayout();
			this.SuspendLayout();
			// 
			// pnl_Head
			// 
			this.pnl_Head.Controls.Add(this.lbl_Header);
			this.pnl_Head.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnl_Head.Location = new System.Drawing.Point(0, 0);
			this.pnl_Head.Margin = new System.Windows.Forms.Padding(0);
			this.pnl_Head.Name = "pnl_Head";
			this.pnl_Head.Size = new System.Drawing.Size(392, 26);
			this.pnl_Head.TabIndex = 0;
			// 
			// lbl_Header
			// 
			this.lbl_Header.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lbl_Header.Location = new System.Drawing.Point(0, 0);
			this.lbl_Header.Margin = new System.Windows.Forms.Padding(0);
			this.lbl_Header.Name = "lbl_Header";
			this.lbl_Header.Padding = new System.Windows.Forms.Padding(4, 0, 0, 0);
			this.lbl_Header.Size = new System.Drawing.Size(392, 26);
			this.lbl_Header.TabIndex = 0;
			this.lbl_Header.Text = "lbl_Header";
			this.lbl_Header.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// pnl_Copyable
			// 
			this.pnl_Copyable.BackColor = System.Drawing.Color.Goldenrod;
			this.pnl_Copyable.Controls.Add(this.rtb_Copyable);
			this.pnl_Copyable.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnl_Copyable.Location = new System.Drawing.Point(0, 26);
			this.pnl_Copyable.Margin = new System.Windows.Forms.Padding(0);
			this.pnl_Copyable.Name = "pnl_Copyable";
			this.pnl_Copyable.Padding = new System.Windows.Forms.Padding(6, 1, 1, 1);
			this.pnl_Copyable.Size = new System.Drawing.Size(392, 109);
			this.pnl_Copyable.TabIndex = 1;
			// 
			// rtb_Copyable
			// 
			this.rtb_Copyable.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.rtb_Copyable.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rtb_Copyable.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.rtb_Copyable.HideSelection = false;
			this.rtb_Copyable.Location = new System.Drawing.Point(6, 1);
			this.rtb_Copyable.Margin = new System.Windows.Forms.Padding(0);
			this.rtb_Copyable.Name = "rtb_Copyable";
			this.rtb_Copyable.ReadOnly = true;
			this.rtb_Copyable.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.rtb_Copyable.Size = new System.Drawing.Size(385, 107);
			this.rtb_Copyable.TabIndex = 0;
			this.rtb_Copyable.Text = "rtb_Copyable";
			// 
			// btn_Cancel
			// 
			this.btn_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btn_Cancel.Location = new System.Drawing.Point(300, 240);
			this.btn_Cancel.Margin = new System.Windows.Forms.Padding(0);
			this.btn_Cancel.Name = "btn_Cancel";
			this.btn_Cancel.Size = new System.Drawing.Size(85, 30);
			this.btn_Cancel.TabIndex = 2;
			this.btn_Cancel.Text = "Cancel";
			this.btn_Cancel.UseVisualStyleBackColor = true;
			this.btn_Cancel.Click += new System.EventHandler(this.click_btnCancel);
			// 
			// MapInfoDetailDialog
			// 
			this.CancelButton = this.btn_Cancel;
			this.ClientSize = new System.Drawing.Size(392, 274);
			this.Controls.Add(this.btn_Cancel);
			this.Controls.Add(this.pnl_Copyable);
			this.Controls.Add(this.pnl_Head);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MaximizeBox = false;
			this.Name = "MapInfoDetailDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "MapDetail";
			this.pnl_Head.ResumeLayout(false);
			this.pnl_Copyable.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion Designer
	}
}
