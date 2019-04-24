using System;
using System.Windows.Forms;
using System.Drawing;


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
			this.SuspendLayout();
			// 
			// CopyPanelF
			// 
			this.ClientSize = new System.Drawing.Size(594, 776);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.MaximizeBox = false;
			this.Name = "CopyPanelF";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Copy panel";
			this.ResumeLayout(false);

		}
		#endregion Designer
	}
}
