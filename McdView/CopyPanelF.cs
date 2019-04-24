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
		#region Fields
		private readonly McdviewF _f;
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
				PartsPanel.Spriteset = (_spriteset = value);
				Text = "Copy panel - " + _spriteset.Label;
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

//					InvalidatePanels();
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
//			base.OnFormClosing(e);

			_f.CloseCopyPanel();
		}
		#endregion Events (override)


		#region Designer
		private System.Windows.Forms.GroupBox gb_Collection;

		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

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
			this.SuspendLayout();
			// 
			// gb_Collection
			// 
			this.gb_Collection.Dock = System.Windows.Forms.DockStyle.Top;
			this.gb_Collection.Location = new System.Drawing.Point(0, 0);
			this.gb_Collection.Margin = new System.Windows.Forms.Padding(0);
			this.gb_Collection.Name = "gb_Collection";
			this.gb_Collection.Padding = new System.Windows.Forms.Padding(0);
			this.gb_Collection.Size = new System.Drawing.Size(594, 175);
			this.gb_Collection.TabIndex = 0;
			this.gb_Collection.TabStop = false;
			this.gb_Collection.Text = " RECORD COLLECTION ";
			// 
			// CopyPanelF
			// 
			this.ClientSize = new System.Drawing.Size(594, 776);
			this.Controls.Add(this.gb_Collection);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.MaximizeBox = false;
			this.Name = "CopyPanelF";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.ResumeLayout(false);

		}
		#endregion Designer
	}
}
