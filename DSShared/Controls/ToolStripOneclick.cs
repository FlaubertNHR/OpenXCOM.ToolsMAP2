using System;
using System.Windows.Forms;


namespace DSShared.Controls
{
	public sealed class ToolStripOneclick
		:
			ToolStrip
	{
		#region Fields (static)
		const int WM_MOUSEACTIVATE = 0x0021;
		#endregion Fields (static)


		#region Methods (override)
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		protected override void WndProc(ref Message m)
		{
			if (m.Msg == WM_MOUSEACTIVATE && CanFocus && !Focused)
				Focus();

			base.WndProc(ref m);
		}
		#endregion Methods (override)
	}
}
