using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;


namespace PckView
{
/// <summary>
	/// The statusbar for 'PaletteF'.
	/// </summary>
	internal sealed class PaletteLabelBar
		:
			Label
	{
		private readonly GraphicsPath _path3d = new GraphicsPath();

		/// <summary>
		/// Handles a resize event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);

			_path3d.Reset();

			_path3d.AddLine(Width, 0, 0,0);
			_path3d.AddLine(0,0, 0, Height);
		}

		/// <summary>
		/// Handles the paint event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			e.Graphics.DrawPath(Pens.Gray, _path3d);
		}

		/// <summary>
		/// Disposes the GraphicsPath.
		/// @note I have no idea if this is really necessary despite hundreds of
		/// hours reading about Dispose() et al. This class is a visual control
		/// so it gets disposed when its parent closes, but does its private
		/// field '_path3d' get disposed reliably ...
		/// </summary>
		internal void Destroy()
		{
			_path3d.Dispose();
		}
	}
}
