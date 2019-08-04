using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;


namespace DSShared.Controls
{
	/// <summary>
	/// Used by ToolStrips/StatusStrips to get rid of white borders and draw a
	/// 3d border.
	/// </summary>
	public class CustomToolStripRenderer
		:
			ToolStripProfessionalRenderer
	{
		protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
		{
			e.Graphics.FillRectangle(Brushes.Snow, e.AffectedBounds);
		}

		protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
		{
			using (var path3d = new GraphicsPath())
			{
				path3d.AddLine(e.ToolStrip.Width, 0, 0,0);
				path3d.AddLine(0,0, 0, e.ToolStrip.Height);

				e.Graphics.DrawPath(Pens.Gray, path3d);
			}
		}
	}
}
