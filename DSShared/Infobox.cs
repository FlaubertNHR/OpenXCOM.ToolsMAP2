using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;


namespace DSShared
{
	/// <summary>
	/// A generic outputbox for Info/Warnings/Errors.
	/// </summary>
	/// <remarks>The point is to stop wrapping long path-strings like the stock
	/// .NET MessageBox does. And to stop beeps.</remarks>
	public sealed class Infobox
		:
			Form
	{
		#region Enums
		public enum BoxType
		{ Info, Warn, Error }

		public enum Buttons
		{ Cancel, CancelOkay, CancelOkayRetry, CancelYesNo }
		#endregion Enums


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
		/// <param name="buttons">buttons to show</param>
		/// <remarks>Limit the length of 'head' to ~100 chars max or break it
		/// into lines if greater.</remarks>
		public Infobox(
				string title,
				string head,
				string copyable = null,
				BoxType type = BoxType.Info,
				Buttons buttons = Buttons.Cancel)
		{
			// TODO: Store static location and size of the Infobox (if shown non-modally).

			InitializeComponent();

			DialogResult = DialogResult.Cancel;

			switch (buttons)
			{
				case Buttons.Cancel:
					bu_Cancel.Text = "ok";
					break;

				case Buttons.CancelOkay:
					bu_Okay.Visible = true;
					break;

				case Buttons.CancelOkayRetry:
					bu_Okay .Visible =
					bu_Retry.Visible = true;
					break;

				case Buttons.CancelYesNo:
					bu_Okay .Text = "yes";
					bu_Retry.Text = "no";
					goto case Buttons.CancelOkayRetry;
			}


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
					size = TextRenderer.MeasureText(line, rt_Copyable.Font);
					if ((test = size.Width) > width)
						width = test;
				}
				width += pa_Copyable.Padding.Horizontal + widthScroller;

				height = TextRenderer.MeasureText(HEIGHT_TEST, rt_Copyable.Font).Height;
				pa_Copyable.Height = height * (lines.Length + 1) + pa_Copyable.Padding.Vertical;

				copyable += Environment.NewLine; // add a blank line to bot of the copyable text.
				rt_Copyable.Text = copyable;
			}
			else
			{
				pa_Copyable.Height =
				rt_Copyable.Height = 0;

				pa_Copyable.Visible =
				rt_Copyable.Visible = false;
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
					case BoxType.Info:  lbl_Head.BackColor = Color.PowderBlue; break;
					case BoxType.Warn:  lbl_Head.BackColor = Color.Moccasin;   break;
					case BoxType.Error: lbl_Head.BackColor = Color.SandyBrown; break;
				}

				lbl_Head.Text = head;

				Size size = TextRenderer.MeasureText(head, lbl_Head.Font);
				lbl_Head.Height = size.Height + 10;

				if (size.Width + lbl_Head.Padding.Horizontal + widthScroller > width)
					width = size.Width + lbl_Head.Padding.Horizontal + widthScroller;
			}


			height = (lbl_Head != null ? lbl_Head.Height : 0)
				   + pa_Copyable.Height
				   + bu_Cancel  .Height + bu_Cancel.Margin.Vertical;

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
			rt_Copyable.AutoWordSelection = false;
			ActiveControl = bu_Cancel;
		}

		/// <summary>
		/// Overrides the resize event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);

			if (pa_Copyable.Visible)
			{
				pa_Copyable.Height = ClientSize.Height
								   - (lbl_Head != null ? lbl_Head.Height : 0)
								   - bu_Cancel.Height - bu_Cancel.Margin.Vertical;
				pa_Copyable.Invalidate();
			}

			int width = ClientSize.Width / 3;

			switch (ClientSize.Width % 3)
			{
				case 0:
					bu_Retry .Left  = 0;
					bu_Retry .Width = width;
					bu_Okay  .Left  = width;
					bu_Okay  .Width = width;
					bu_Cancel.Left  = width * 2;
					bu_Cancel.Width = width;
					break;

				case 1:
					bu_Retry .Left  = 0;
					bu_Retry .Width = width;
					bu_Okay  .Left  = width;
					bu_Okay  .Width = width + 1;
					bu_Cancel.Left  = width * 2 + 1;
					bu_Cancel.Width = width;
					break;

				case 2:
					bu_Retry .Left  = 0;
					bu_Retry .Width = width + 1;
					bu_Okay  .Left  = width;
					bu_Okay  .Width = width;
					bu_Cancel.Left  = width * 2 + 1;
					bu_Cancel.Width = width + 1;
					break;
			}

			bu_Cancel.Top =
			bu_Okay  .Top =
			bu_Retry .Top = ClientSize.Height
						  - bu_Cancel .Height - bu_Cancel.Margin.Bottom;
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
			e.Graphics.DrawLine(Pens.Black, 0,0, 0, pa_Copyable.Height - 1);
			e.Graphics.DrawLine(Pens.Black, 1,0, pa_Copyable.Width - 1, 0);

			// test
//			e.Graphics.DrawLine(Pens.Black, 0, pa_Copyable.Height - 1, pa_Copyable.Width - 1, pa_Copyable.Height - 1);
		}

		/// <summary>
		/// Closes this dialog.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnCancelClick(object sender, EventArgs e)
		{
			Close();
		}

		/// <summary>
		/// Sets DialogResult to OK and closes this dialog.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnOkayClick(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
		}

		/// <summary>
		/// Sets DialogResult to Yes and closes this dialog.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnRetryClick(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Retry;
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
			string[] array = text.Split(new[]{' '}, StringSplitOptions.RemoveEmptyEntries);
			IList<string> words = new List<string>(array);

			var sb = new StringBuilder();

			int tally = 0;

			string word;
			for (int i = 0; i != words.Count; ++i)
			{
				word = words[i];

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
