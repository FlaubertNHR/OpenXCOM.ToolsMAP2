using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;


namespace DSShared
{
	/// <summary>
	/// A generic outputbox for Errors/Warnings/Info.
	/// The point is to stop wrapping long path-strings like the stock .NET
	/// MessageBox does. And to stop beeps.
	/// </summary>
	public sealed class Infobox
		:
			Form
	{
		#region Fields (static)
		private const int w_MinCutoff = 345;
		private const int h_MinCutoff = 450;

		private const string HEIGHT_TEST = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ~!@#$%^&*()_+-={}[]|\\;:'\"<>,.?";
		#endregion Fields (static)


		#region Designer (workaround)
		/// <summary>
		/// Since the programmers of .net couldn't figure out that when you set
		/// a label's height to 0 and invisible it ought maintain a height of 0,
		/// I need to *not* instantiate said label unless it is required.
		///
		/// Don't forget to do null-checks.
		/// </summary>
		private Label lbl_Head;
		#endregion Designer (workaround)


		#region cTor
		/// <summary>
		/// cTor.
		/// @note 'null' can be used instead of a blank string for the args
		/// </summary>
		/// <param name="title">a caption on the titlebar</param>
		/// <param name="label">info to be displayed with a proportional font</param>
		/// <param name="copyable">info to be displayed in a fixed-width font as
		/// readily copyable text</param>
		/// TODO: Store static location and size of the Infobox (if shown non-modally).
		public Infobox(
				string title,
				string label,
				string copyable = null)
		{
			InitializeComponent();

			SuspendLayout();

			int widthborder = Width  - ClientSize.Width; // cache these before things go wonky
			int heighttitle = Height - ClientSize.Height - widthborder;

			Text = title;

			int width  = 0;
			int height = 0;

			if (!String.IsNullOrEmpty(copyable)) // deter total width based on longest copyable line
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
				width += pnl_Copyable.Padding.Horizontal + 15; // +15 width of alleged scrollbar in the rtb.

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

			if (width < w_MinCutoff)
				width = w_MinCutoff;


			if (!String.IsNullOrEmpty(label))
			{
				lbl_Head = new Label();
				lbl_Head.Name     = "lbl_Head";
				lbl_Head.Location = new Point(0,0);
				lbl_Head.Size     = new Size(20,27);
				lbl_Head.Margin   = new Padding(0);
				lbl_Head.Padding  = new Padding(10,10,5,5);
				lbl_Head.Dock     = DockStyle.Top;
				lbl_Head.AutoSize = true;
				lbl_Head.TabIndex = 0;
				Controls.Add(this.lbl_Head);

				lbl_Head.MaximumSize = new Size(width, 0); // auto-calc 'lbl_Head.Height'
				lbl_Head.Text = label;
			}


			height = (lbl_Head != null ? lbl_Head.Height : 0)
				   + pnl_Copyable.Height
				   + btn_Okay    .Height
				   + btn_Okay    .Margin.Vertical;

			if (height > h_MinCutoff)
				height = h_MinCutoff;

			ClientSize = new Size(width, height);

			MinimumSize = new Size(width  + widthborder,
								   height + widthborder + heighttitle);

			ResumeLayout();
		}
		#endregion cTor


		#region Events (override)
		protected override void OnLoad(EventArgs e)
		{
			rtb_Copyable.AutoWordSelection = false; // <- needs to be here not in the designer to work right.
			rtb_Copyable.Select();

			// don't do this because the top of the text gets hidden if a scrollbar appears
//			rtb_Copyable.SelectionStart = rtb_Copyable.TextLength;
		}

		protected override void OnResize(EventArgs e)
		{
			if (lbl_Head != null)
				lbl_Head.MaximumSize = new Size(ClientSize.Width, 0);

			pnl_Copyable.Height = ClientSize.Height
								- (lbl_Head != null ? lbl_Head.Height : 0)
								- btn_Okay.Height
								- btn_Okay.Margin.Vertical;
			pnl_Copyable.Invalidate();

			base.OnResize(e);
		}
		#endregion Events (override)


		#region Events
		private void OnOkayClick(object sender, EventArgs e)
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
			var l = pnl_Copyable.Width     / 4; // top & bot half-borders ->
			var r = pnl_Copyable.Width * 3 / 4;
			var b = pnl_Copyable.Height - 1;
			e.Graphics.DrawLine(Pens.Gray, l,0, r,0);
			e.Graphics.DrawLine(Pens.Gray, l,b, r,b);

//			int w = pnl_Copyable.Width  - 1; // 4-sided border ->
//			int h = pnl_Copyable.Height - 1;
//			var tl = new Point(0, 0);
//			var tr = new Point(w, 0);
//			var br = new Point(w, h);
//			var bl = new Point(0, h);
//			var graphics = e.Graphics;
//			graphics.DrawLine(Pens.Black, tl, tr);
//			graphics.DrawLine(Pens.Black, tr, br);
//			graphics.DrawLine(Pens.Black, br, bl);
//			graphics.DrawLine(Pens.Black, bl, tl);
		} */
		#endregion Events


		#region Methods
		public void SetLabelColor(Color color)
		{
			if (lbl_Head != null)
				lbl_Head.ForeColor = color;
		}
		#endregion Methods


		#region Methods (static)
		/// <summary>
		/// Takes and input-string and splices it with newlines every length in
		/// chars.
		/// TODO: There's something borky with the algo. See testcase below ...
		/// </summary>
		/// <param name="text"></param>
		/// <param name="chars"></param>
		/// <returns></returns>
		public static string FormatString(string text, int chars)
		{
			var sb = new System.Text.StringBuilder();

			int pos = 0;
			var list = new List<int>();

			for (int i = 0; i != text.Length; ++i)
			{
				if (text[i] == ' ')
					pos = i;

				sb.Append(text[i]);

				if (i != 0 && (i % chars) == 0)
					list.Add(pos);
			}

			for (int i = list.Count - 1; i != -1; --i)
			{
				sb.Remove(list[i], 1);
				sb.Insert(list[i], Environment.NewLine);
			}
			return sb.ToString();
		}
/* FormatString() testcase:
- use 60-char width and "terrainset" in 'copyable' gets stuck out to the right

	string copyable = "WARNING: Saving the Map in its current state would forever lose"
					+ " those tilepart references. But if you know what terrain(s) have"
					+ " gone rogue they can be added to the Map's terrainset with the"
					+ " TilesetEditor. Or if you know how many records have been removed"
					+ " from the terrainset the ids of the missing parts can be shifted"
					+ " downward into an acceptable range by the TilepartSubstitution"
					+ " dialog under MainView's edit-menu.";
	copyable = FormatString(copyable, 60);

	using (var f = new Infobox("Warning", "test", copyable))
	{
		f.ShowDialog();
	}
		WARNING: Saving the Map in its current state would forever
		lose those tilepart references. But if you know what
		terrain(s) have gone rogue they can be added to the Map's terrainset <- Is sticking out.
		with the TilesetEditor. Or if you know how many records
		have been removed from the terrainset the ids of the missing
		parts can be shifted downward into an acceptable range by the
		TilepartSubstitution dialog under MainView's edit-menu.

- or this string w/ 55-char width:
		There are 2 tileparts that exceed the bounds of the
		Map's currently allocated MCD records. They will be
		replaced by temporary tileparts and displayed on the Map as <- Is sticking out.
		orange-red borkiness.
*/
		#endregion Methods (static)


		#region Designer
		private Button btn_Okay;
		private RichTextBox rtb_Copyable;
		private Panel pnl_Copyable;


		/// <summary>
		/// Required method for Designer support - do not modify the contents of
		/// this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.btn_Okay = new System.Windows.Forms.Button();
			this.rtb_Copyable = new System.Windows.Forms.RichTextBox();
			this.pnl_Copyable = new System.Windows.Forms.Panel();
			this.pnl_Copyable.SuspendLayout();
			this.SuspendLayout();
			// 
			// btn_Okay
			// 
			this.btn_Okay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btn_Okay.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btn_Okay.Location = new System.Drawing.Point(312, 144);
			this.btn_Okay.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
			this.btn_Okay.Name = "btn_Okay";
			this.btn_Okay.Size = new System.Drawing.Size(80, 30);
			this.btn_Okay.TabIndex = 2;
			this.btn_Okay.Text = "ok";
			this.btn_Okay.UseVisualStyleBackColor = true;
			this.btn_Okay.Click += new System.EventHandler(this.OnOkayClick);
			// 
			// rtb_Copyable
			// 
			this.rtb_Copyable.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.rtb_Copyable.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rtb_Copyable.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.rtb_Copyable.HideSelection = false;
			this.rtb_Copyable.Location = new System.Drawing.Point(20, 10);
			this.rtb_Copyable.Margin = new System.Windows.Forms.Padding(0);
			this.rtb_Copyable.Name = "rtb_Copyable";
			this.rtb_Copyable.ReadOnly = true;
			this.rtb_Copyable.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.rtb_Copyable.Size = new System.Drawing.Size(374, 98);
			this.rtb_Copyable.TabIndex = 0;
			this.rtb_Copyable.WordWrap = false;
			// 
			// pnl_Copyable
			// 
			this.pnl_Copyable.Controls.Add(this.rtb_Copyable);
			this.pnl_Copyable.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnl_Copyable.Location = new System.Drawing.Point(0, 0);
			this.pnl_Copyable.Margin = new System.Windows.Forms.Padding(0);
			this.pnl_Copyable.Name = "pnl_Copyable";
			this.pnl_Copyable.Padding = new System.Windows.Forms.Padding(20, 10, 0, 5);
			this.pnl_Copyable.Size = new System.Drawing.Size(394, 113);
			this.pnl_Copyable.TabIndex = 1;
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
