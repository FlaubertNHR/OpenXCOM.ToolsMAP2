using System;
using System.Windows.Forms;


namespace DSShared.Controls
{
	/// <summary>
	/// A derived Panel that flags DoubleBuffered and ResizeRedraw.
	/// </summary>
	public class BufferedPanel
		:
			Panel
	{
		#region cTor
		public BufferedPanel()
		{
			DoubleBuffered = true;
			ResizeRedraw = true;
		}
		#endregion cTor
	}


	/// <summary>
	/// A derived Panel that flags WS_EX_COMPOSITED.
	/// </summary>
	public class CompositedPanel
		:
			Panel
	{
		#region Properties (override)
		/// <summary>
		/// This works great. Absolutely kills flicker on redraws.
		/// </summary>
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.ExStyle |= 0x02000000;
				return cp;
			}
		}
		#endregion Properties (override)
	}
}
