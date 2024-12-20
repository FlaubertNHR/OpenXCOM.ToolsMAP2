﻿using System;
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
	/// .NET MessageBox does. And to stop beeps. And to make it look nicer.</remarks>
	public sealed partial class Infobox
		:
			Form
	{
		#region Fields (static)
		private const int w_Min = 345;
		private const int h_Max = 470;
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
		/// <param name="head">info to be displayed with a proportional font -
		/// keep this brief</param>
		/// <param name="copyable">info to be displayed in a fixed-width font as
		/// readily copyable text</param>
		/// <param name="bt">an <see cref="InfoboxType"/> to deter the head's
		/// backcolor - is valid only with head-text specified</param>
		/// <param name="buttons">buttons to show</param>
		/// <remarks>Limit the length of 'head' to ~100 chars max or break it
		/// into lines if greater.</remarks>
		public Infobox(
				string title,
				string head,
				string copyable = null,
				InfoboxType bt = InfoboxType.Info,
				InfoboxButton buttons = InfoboxButton.Cancel)
		{
			// TODO: Store static location and size of the Infobox (if shown non-modally).

			InitializeComponent();

			Text = title;

			DialogResult = DialogResult.Cancel;

			switch (buttons)
			{
				case InfoboxButton.Cancel:
					bu_Cancel.Text = "ok";
					break;

				case InfoboxButton.CancelOkay:
					bu_Okay.Visible = true;
					break;

				case InfoboxButton.CancelOkayRetry:
					bu_Okay .Visible =
					bu_Retry.Visible = true;
					break;

				case InfoboxButton.CancelYesNo:
					bu_Okay .Text = "yes";
					bu_Retry.Text = "no";
					goto case InfoboxButton.CancelOkayRetry;
			}


			SuspendLayout();

			int widthBorder = Width  - ClientSize.Width; // cache these before things go wonky
			int heightTitle = Height - ClientSize.Height - widthBorder;

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

				height = rt_Copyable.Font.Height;
				pa_Copyable.Height = (height - 1) * (lines.Length + 1) + pa_Copyable.Padding.Vertical;

				rt_Copyable.Text = copyable + Environment.NewLine; // add a blank line to bot of the copyable text.
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

				switch (bt)
				{
					case InfoboxType.Info:  lbl_Head.BackColor = Color.Lavender;   break;
					case InfoboxType.Warn:  lbl_Head.BackColor = Color.Moccasin;   break;
					case InfoboxType.Error: lbl_Head.BackColor = Color.SandyBrown; break;
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
			{
				pa_Copyable.Height -= height - h_Max;	// <- The only way that (height > h_Max) is
				height = h_Max;							// because 'pa_Copyable' contains a lot of text.
			}

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

			int width = ClientSize.Width / 3 - 4; // ~2px padding on both sides of buttons

			bu_Retry.Width = bu_Okay.Width = bu_Cancel.Width = width;

			bu_Retry .Left =  4;
			bu_Okay  .Left =  7 + width;
			bu_Cancel.Left = 10 + width * 2;

			bu_Retry .Top =
			bu_Okay  .Top =
			bu_Cancel.Top = ClientSize.Height
						  - bu_Cancel .Height - bu_Cancel.Margin.Bottom;
		}

		/// <summary>
		/// Overrides the <c>Paint</c> event.
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
		/// Sets <c>DialogResult</c> to <c>OK</c> and closes this dialog.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnOkayClick(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
		}

		/// <summary>
		/// Sets <c>DialogResult</c> to <c>Retry</c> and closes this dialog.
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
		/// Takes an input-string and splices it with newlines every width in
		/// chars.
		/// </summary>
		/// <param name="text">input only a single trimmed sentence with no
		/// newlines and keep words shorter than width</param>
		/// <param name="width">desired width in chars - lines of output shall
		/// not exceed width</param>
		/// <returns>text split into lines no longer than width</returns>
		public static string SplitString(string text, int width = 60)
		{
			string[] array = text.Split(new[]{' '}, StringSplitOptions.RemoveEmptyEntries);
			var words = new List<string>(array);

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
	}


	#region Enums (public)
	public enum InfoboxType
	{ Info, Warn, Error }

	public enum InfoboxButton
	{ Cancel, CancelOkay, CancelOkayRetry, CancelYesNo }
	#endregion Enums (public)
}
