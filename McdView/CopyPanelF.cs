using System;
using System.Windows.Forms;
using System.Drawing;

using XCom;


namespace McdView
{
	/// <summary>
	/// A form with a panel that enables the user to copy MCD records from a
	/// different MCD-set than the one that's currently loaded in McdView to the
	/// internal copy-buffer of McdView for pasting into the currently loaded
	/// MCD-set.
	/// </summary>
	internal sealed class CopyPanelF
		:
			Form
	{
		#region Fields (static)
		internal static Point Loc = new Point(-1,-1);
		#endregion Fields (static)


		#region Fields
		private readonly McdviewF _f;

		internal string Label;
		#endregion Fields


		#region Properties
		internal TerrainPanel_copy PartsPanel
		{ get; private set; }

		private Tilepart[] _parts;
		/// <summary>
		/// An array of 'Tileparts'. Each entry's record is referenced w/ 'Record'.
		/// </summary>
		internal Tilepart[] Parts
		{
			get { return _parts; }
			set
			{
				PartsPanel.Parts = (_parts = value);
			}
		}

		private SpriteCollection _spriteset;
		internal SpriteCollection Spriteset
		{
			get { return _spriteset; }
			set
			{
				string text = "Copy panel - ";

				if ((PartsPanel.Spriteset = (_spriteset = value)) != null)
					text += _spriteset.Label;
				else
					text += "spriteset invalid";

				Text = text;
				PartsPanel.Select();
			}
		}

		private int _selId = -1;
		/// <summary>
		/// The currently selected 'Parts' ID.
		/// </summary>
		internal int SelId
		{
			get { return _selId; }
			set
			{
				if (_selId != value)
				{
					if ((_selId = value) != -1)
					{
//						PopulateTextFields();
						PartsPanel.ScrollToPart();
					}
					else
					{
//						ClearTextFields();
					}

					PartsPanel.Invalidate();
				}

				if (PartsPanel.SubIds.Remove(_selId)) // safety. The SelId shall never be in the SubIds.
					PartsPanel.Invalidate();
			}
		}
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="f"></param>
		internal CopyPanelF(McdviewF f)
		{
			InitializeComponent();

			SetStyle(ControlStyles.OptimizedDoubleBuffer
				   | ControlStyles.AllPaintingInWmPaint
				   | ControlStyles.UserPaint
				   | ControlStyles.ResizeRedraw, true);

			_f = f;

			Location = new Point(
							_f.Location.X + 20,
							_f.Location.Y + 20);

			PartsPanel = new TerrainPanel_copy(_f, this);
			gb_Collection.Controls.Add(PartsPanel);
			PartsPanel.Width = gb_Collection.Width - 10;

			McdviewF.SetDoubleBuffered(PartsPanel);

			PartsPanel.Select();

		}
		#endregion cTor


		#region Events (override)
		/// <summary>
		/// Closes (and disposes) this CopyPanelF object.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			Loc = new Point(Location.X, Location.Y);
			_f.CloseCopyPanel();

			base.OnFormClosing(e);
		}

		/// <summary>
		/// Positions this CopyPanelF wrt/ McdviewF.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLoad(EventArgs e)
		{
			if (Loc.X == -1)
				Location = new Point(_f.Location.X + 20, _f.Location.Y + 20);
			else
				Location = new Point(Loc.X, Loc.Y);

			base.OnLoad(e);
		}

		/// <summary>
		/// yah whatever. I dislike .NET keyboard gobbledy-gook.
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="keyData"></param>
		/// <returns></returns>
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (keyData == (Keys.Control | Keys.O))
			{
				_f.OpenCopyPanel();
				return true;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}
		#endregion Events (override)


		#region Events
		private void OnClick_Open(object sender, EventArgs e)
		{
			_f.OpenCopyPanel();
		}
		#endregion Events


		#region Designer
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		private System.Windows.Forms.GroupBox gb_Collection;
		private System.Windows.Forms.ToolStrip ts_CopyPanel;
		private System.Windows.Forms.ToolStripButton tsb_Open;

		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
				components.Dispose();

			base.Dispose(disposing);
		}

		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The
		/// Forms designer might not be able to load this method if it was
		/// changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.gb_Collection = new System.Windows.Forms.GroupBox();
			this.ts_CopyPanel = new System.Windows.Forms.ToolStrip();
			this.tsb_Open = new System.Windows.Forms.ToolStripButton();
			this.ts_CopyPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// gb_Collection
			// 
			this.gb_Collection.Dock = System.Windows.Forms.DockStyle.Top;
			this.gb_Collection.Location = new System.Drawing.Point(0, 25);
			this.gb_Collection.Margin = new System.Windows.Forms.Padding(0);
			this.gb_Collection.Name = "gb_Collection";
			this.gb_Collection.Padding = new System.Windows.Forms.Padding(0);
			this.gb_Collection.Size = new System.Drawing.Size(594, 175);
			this.gb_Collection.TabIndex = 0;
			this.gb_Collection.TabStop = false;
			this.gb_Collection.Text = " RECORD COLLECTION ";
			// 
			// ts_CopyPanel
			// 
			this.ts_CopyPanel.CanOverflow = false;
			this.ts_CopyPanel.Font = new System.Drawing.Font("Consolas", 7F);
			this.ts_CopyPanel.GripMargin = new System.Windows.Forms.Padding(0);
			this.ts_CopyPanel.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.ts_CopyPanel.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.tsb_Open});
			this.ts_CopyPanel.Location = new System.Drawing.Point(0, 0);
			this.ts_CopyPanel.Name = "ts_CopyPanel";
			this.ts_CopyPanel.Padding = new System.Windows.Forms.Padding(0);
			this.ts_CopyPanel.ShowItemToolTips = false;
			this.ts_CopyPanel.Size = new System.Drawing.Size(594, 25);
			this.ts_CopyPanel.TabIndex = 1;
			// 
			// tsb_Open
			// 
			this.tsb_Open.AutoToolTip = false;
			this.tsb_Open.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tsb_Open.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsb_Open.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
			this.tsb_Open.Name = "tsb_Open";
			this.tsb_Open.Size = new System.Drawing.Size(49, 23);
			this.tsb_Open.Text = "Open ...";
			this.tsb_Open.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tsb_Open.Click += new System.EventHandler(this.OnClick_Open);
			// 
			// CopyPanelF
			// 
			this.ClientSize = new System.Drawing.Size(594, 776);
			this.Controls.Add(this.gb_Collection);
			this.Controls.Add(this.ts_CopyPanel);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.MaximizeBox = false;
			this.Name = "CopyPanelF";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.ts_CopyPanel.ResumeLayout(false);
			this.ts_CopyPanel.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion Designer
	}
}
