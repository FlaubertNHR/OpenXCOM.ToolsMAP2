using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;


namespace DSShared.Windows
{
	/// <summary>
	/// Inherited by MapObserverControl_TopPanel (TopPanel and QuadrantPanel).
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
			SetStyle(ControlStyles.OptimizedDoubleBuffer
				   | ControlStyles.AllPaintingInWmPaint
				   | ControlStyles.UserPaint
				   | ControlStyles.ResizeRedraw, true);
		}
		#endregion cTor


		#region Events (override)
		/// <summary>
		/// Paints a surface.
		/// </summary>
		protected override void OnPaint(PaintEventArgs e)
		{
			var graphics = e.Graphics;
			graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

			RenderGraphics(graphics);
		}
		#endregion Events (override)


		#region Methods (virtual)
		/// <summary>
		/// Shunts rendering off to TopPanel and QuadrantPanel.
		/// </summary>
		protected virtual void RenderGraphics(Graphics graphics)
		{}
		#endregion Methods (virtual)
	}
}
