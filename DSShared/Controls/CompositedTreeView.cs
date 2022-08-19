using System;
using System.Drawing;
using System.Windows.Forms;


namespace DSShared.Controls
{
	/// <summary>
	/// Derived class for TreeView.
	/// </summary>
	public sealed class CompositedTreeView
		:
			TreeView
	{
		#region Properties (override)
		/// <summary>
		/// Prevents flicker.
		/// </summary>
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.ExStyle |= 0x02000000; // enable 'WS_EX_COMPOSITED'
				return cp;
			}
		}
		#endregion Properties (override)


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		public CompositedTreeView()
		{
			Name          = "MapTree";
			Dock          = DockStyle.Left;
			DrawMode      = TreeViewDrawMode.OwnerDrawText;
			Indent        = 15;
			Margin        = new Padding(0);
			TabIndex      = 0;
			HideSelection = false;
			BackColor     = SystemColors.Control;

			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
		}
		#endregion cTor
	}
}
