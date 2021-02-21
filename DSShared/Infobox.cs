using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;


namespace DSShared
{
	/// <summary>
	/// A generic outputbox for Info/Warnings/Errors.
	/// The point is to stop wrapping long path-strings like the stock .NET
	/// MessageBox does. And to stop beeps.
	/// </summary>
	public sealed class Infobox
		:
			Form
	{
		public enum BoxType
		{ Info, Warn, Error }


		#region Fields (static)
		private const int w_Min = 345;
		private const int h_Max = 463;

		private const string HEIGHT_TEST = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ~!@#$%^&*()_+-={}[]|\\;:'\"<>,.?";
		#endregion Fields (static)


		#region Designer (workaround)
		/// <summary>
		/// Since the programmers of .net couldn't figure out that when you set
		/// a label's height to 0 and invisible it ought maintain a height of 0,
		/// I need to *not* instantiate said label unless it is required.
		/// </summary>
		/// <remarks>Don't forget to do null-checks.</remarks>
		private Label lbl_Head;
		#endregion Designer (workaround)


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="title">a caption on the titlebar</param>
		/// <param name="head">info to be displayed with a proportional font</param>
		/// <param name="copyable">info to be displayed in a fixed-width font as
		/// readily copyable text</param>
		/// <param name="type">a <see cref="BoxType"/> to deter the head's
		/// backcolor - is valid only with head-text specified</param>
		/// <remarks>Limit the length of 'head' to ~100 chars max or break it
		/// into lines if greater.</remarks>
		public Infobox(
				string title,
				string head,
				string copyable = null,
				BoxType type = BoxType.Info)
		{
			// TODO: Store static location and size of the Infobox (if shown non-modally).

			InitializeComponent();

			SuspendLayout();

			int widthBorder = Width  - ClientSize.Width; // cache these before things go wonky
			int heightTitle = Height - ClientSize.Height - widthBorder;

			Text = title;

			int width  = 0;
			int height = 0;

			int widthScroller = SystemInformation.VerticalScrollBarWidth;

			if (!String.IsNullOrWhiteSpace(copyable)) // deter total width based on longest copyable line
			{
				string[] lines = copyable.Split(GlobalsXC.CRandorLF, StringSplitOptions.None);

				Size size;

				int test;
				foreach (var line in lines)
				{
					size = TextRenderer.MeasureText(line, rtb_Copyable.Font);
					if ((test = size.Width) > width)
						width = test;
				}
				width += pnl_Copyable.Padding.Horizontal + widthScroller;

				height = TextRenderer.MeasureText(HEIGHT_TEST, rtb_Copyable.Font).Height;
				pnl_Copyable.Height = height * (lines.Length + 1) + pnl_Copyable.Padding.Vertical;

				copyable += Environment.NewLine; // add a blank line to bot of the copyable text.
				rtb_Copyable.Text = copyable;
			}
			else
			{
				pnl_Copyable.Height =
				rtb_Copyable.Height = 0;

				pnl_Copyable.Visible =
				rtb_Copyable.Visible = false;
			}

			if (width < w_Min)
				width = w_Min;


			if (!String.IsNullOrWhiteSpace(head))
			{
				lbl_Head = new Label();
				lbl_Head.Name      = "lbl_Head";
				lbl_Head.Dock      = DockStyle.Top;
				lbl_Head.Margin    = new Padding(0);
				lbl_Head.Padding   = new Padding(8,0,0,0);
				lbl_Head.TextAlign = ContentAlignment.MiddleLeft;
				lbl_Head.TabIndex  = 0;
				lbl_Head.Paint += OnPaintHead;
				Controls.Add(this.lbl_Head);

				switch (type)
				{
					case BoxType.Info:
						lbl_Head.BackColor = Color.PowderBlue;
						break;

					case BoxType.Error:
						lbl_Head.BackColor = Color.SandyBrown;
						break;

					case BoxType.Warn:
						lbl_Head.BackColor = Color.Moccasin;
						break;
				}

				lbl_Head.Text = head;

				Size size = TextRenderer.MeasureText(head, lbl_Head.Font);
				lbl_Head.Height = size.Height + 10;

				if (size.Width + lbl_Head.Padding.Horizontal + widthScroller > width)
					width = size.Width + lbl_Head.Padding.Horizontal + widthScroller;
			}


			height = (lbl_Head != null ? lbl_Head.Height : 0)
				   + pnl_Copyable.Height
				   + btn_Okay    .Height
				   + btn_Okay    .Margin.Vertical;

			if (height > h_Max)
				height = h_Max;

			ClientSize = new Size(width, height);

			MinimumSize = new Size(width  + widthBorder,
								   height + widthBorder + heightTitle);

			ResumeLayout();
		}
		#endregion cTor


		#region Events (override)
		/// <summary>
		/// Overrides the load event.
		/// </summary>
		/// <param name="e"></param>
		/// <remarks>AutoWordSelection needs to be done here not in the designer
		/// or cTor to work right.</remarks>
		protected override void OnLoad(EventArgs e)
		{
			rtb_Copyable.AutoWordSelection = false;
			rtb_Copyable.Select();
		}

		/// <summary>
		/// Overrides the resize event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnResize(EventArgs e)
		{
			pnl_Copyable.Height = ClientSize.Height
								- (lbl_Head != null ? lbl_Head.Height : 0)
								- btn_Okay.Height
								- btn_Okay.Margin.Vertical;
			pnl_Copyable.Invalidate();

			base.OnResize(e);
		}

		/// <summary>
		/// Overrides the paint event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{
			e.Graphics.DrawLine(Pens.Black, 0,0, 0, Height - 1);
		}
		#endregion Events (override)


		#region Events
		/// <summary>
		/// Paints border lines left/top on the head.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnPaintHead(object sender, PaintEventArgs e)
		{
			e.Graphics.DrawLine(Pens.Black, 0,0, 0, lbl_Head.Height - 1);
			e.Graphics.DrawLine(Pens.Black, 1,0, lbl_Head.Width - 1, 0);
		}

		/// <summary>
		/// Paints border lines left/top on the copyable panel.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnPaintPanel(object sender, PaintEventArgs e)
		{
			e.Graphics.DrawLine(Pens.Black, 0,0, 0, pnl_Copyable.Height - 1);
			e.Graphics.DrawLine(Pens.Black, 1,0, pnl_Copyable.Width - 1, 0);
		}

		/// <summary>
		/// Closes the dialog.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnOkayClick(object sender, EventArgs e)
		{
			Close();
		}
		#endregion Events


		#region Methods (static)
		/// <summary>
		/// Takes an input-string and splices it with newlines every length in
		/// chars.
		/// </summary>
		/// <param name="text">input only a trimmed sentence with no newlines
		/// and keep words shorter than width</param>
		/// <param name="width">desired width in chars - lines of output will
		/// not exceed width</param>
		/// <returns>text split into lines of maximum width</returns>
		public static string SplitString(string text, int width = 60)
		{
			string[] words = text.Split(new[]{' '}, StringSplitOptions.RemoveEmptyEntries);
			IList<string> list = new List<string>(words);

			var sb = new StringBuilder();

			int tally = 0;

			string word;
			for (int i = 0; i != list.Count; ++i)
			{
				word = list[i];

				if (i == 0)
				{
					sb.Append(word);
					tally = word.Length;
				}
				else if (tally + word.Length < width - 1)
				{
					sb.Append(" " + word);
					tally += word.Length + 1;
				}
				else
				{
					sb.AppendLine();
					sb.Append(word);
					tally = word.Length;
				}
			}
			return sb.ToString();
		}
		#endregion Methods (static)


		#region Designer
		private RichTextBox rtb_Copyable;
		private Panel pnl_Copyable;
		private Button btn_Okay;

		/// <summary>
		/// Required method for Designer support - do not modify the contents of
		/// this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.rtb_Copyable = new System.Windows.Forms.RichTextBox();
			this.pnl_Copyable = new System.Windows.Forms.Panel();
			this.btn_Okay = new System.Windows.Forms.Button();
			this.pnl_Copyable.SuspendLayout();
			this.SuspendLayout();
			// 
			// rtb_Copyable
			// 
			this.rtb_Copyable.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.rtb_Copyable.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rtb_Copyable.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.rtb_Copyable.HideSelection = false;
			this.rtb_Copyable.Location = new System.Drawing.Point(18, 9);
			this.rtb_Copyable.Margin = new System.Windows.Forms.Padding(0);
			this.rtb_Copyable.Name = "rtb_Copyable";
			this.rtb_Copyable.ReadOnly = true;
			this.rtb_Copyable.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.rtb_Copyable.Size = new System.Drawing.Size(376, 116);
			this.rtb_Copyable.TabIndex = 0;
			this.rtb_Copyable.Text = "";
			this.rtb_Copyable.WordWrap = false;
			// 
			// pnl_Copyable
			// 
			this.pnl_Copyable.Controls.Add(this.rtb_Copyable);
			this.pnl_Copyable.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnl_Copyable.Location = new System.Drawing.Point(0, 0);
			this.pnl_Copyable.Margin = new System.Windows.Forms.Padding(0);
			this.pnl_Copyable.Name = "pnl_Copyable";
			this.pnl_Copyable.Padding = new System.Windows.Forms.Padding(18, 9, 0, 5);
			this.pnl_Copyable.Size = new System.Drawing.Size(394, 130);
			this.pnl_Copyable.TabIndex = 1;
			this.pnl_Copyable.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPaintPanel);
			// 
			// btn_Okay
			// 
			this.btn_Okay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btn_Okay.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btn_Okay.Location = new System.Drawing.Point(312, 144);
			this.btn_Okay.Margin = new System.Windows.Forms.Padding(0, 2, 0, 3);
			this.btn_Okay.Name = "btn_Okay";
			this.btn_Okay.Size = new System.Drawing.Size(80, 30);
			this.btn_Okay.TabIndex = 2;
			this.btn_Okay.Text = "ok";
			this.btn_Okay.UseVisualStyleBackColor = true;
			this.btn_Okay.Click += new System.EventHandler(this.OnOkayClick);
			// 
			// Infobox
			// 
			this.AcceptButton = this.btn_Okay;
			this.CancelButton = this.btn_Okay;
			this.ClientSize = new System.Drawing.Size(394, 176);
			this.Controls.Add(this.pnl_Copyable);
			this.Controls.Add(this.btn_Okay);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Infobox";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.pnl_Copyable.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion Designer
	}
}
