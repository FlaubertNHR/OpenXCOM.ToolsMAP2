using System;
using System.Windows.Forms;


namespace DSShared.Controls
{
	/// <summary>
	/// Derived class for PropertyGrid.
	/// </summary>
	public class CompositedPropertyGrid
		:
			PropertyGrid
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
	}
}
