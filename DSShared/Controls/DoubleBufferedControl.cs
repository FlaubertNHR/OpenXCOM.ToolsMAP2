using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;


namespace DSShared.Controls
{
	/// <summary>
	/// Inherited by ObserverControl_Top (TopControl and QuadrantControl).
	/// </summary>
	public class DoubleBufferedControl
		:
			Control
	{
		#region Properties (override)
/*		/// <summary>
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
		} */
		#endregion Properties (override)


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


		#region Events (override)
		/// <summary>
		/// Paints a surface.
		/// </summary>
		protected override void OnPaint(PaintEventArgs e)
		{
			e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
			RenderGraphics(e.Graphics);
		}
		#endregion Events (override)


		#region Methods (virtual)
		/// <summary>
		/// Shunts rendering off to TopControl and QuadrantControl.
		/// </summary>
		protected virtual void RenderGraphics(Graphics graphics)
		{}
		#endregion Methods (virtual)
	}
}
