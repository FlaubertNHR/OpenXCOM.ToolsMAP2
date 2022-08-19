using System;
using System.Windows.Forms;


namespace DSShared.Controls
{
	/// <summary>
	/// Inherited by <c>TopControl</c> and <c>QuadrantControl</c>.
	/// </summary>
	public class DoubleBufferedControl
		:
			Control
	{
		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		public DoubleBufferedControl()
		{
			DoubleBuffered = true;
			ResizeRedraw = true;
		}
		#endregion cTor
	}
}
