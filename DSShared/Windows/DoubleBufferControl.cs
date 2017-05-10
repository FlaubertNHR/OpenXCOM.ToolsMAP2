using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;


namespace DSShared.Windows
{
	/// <summary>
	/// Parent class for MapObserverControl1 (TopView stuff).
	/// </summary>
	public class DoubleBufferControl
		:
			Control
	{
		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		public DoubleBufferControl()
		{
			SetStyle(ControlStyles.OptimizedDoubleBuffer
				   | ControlStyles.AllPaintingInWmPaint
				   | ControlStyles.UserPaint
				   | ControlStyles.ResizeRedraw, true);
		}
		#endregion


		#region EventCalls
		/// <summary>
		/// Paints a surface.
		/// </summary>
		protected override void OnPaint(PaintEventArgs e)
		{
//			if (DesignMode)
//			{
//				base.OnPaint(e);
//
//				e.Graphics.DrawLine(Pens.Black, 0, 0,      Width, Height);
//				e.Graphics.DrawLine(Pens.Black, 0, Height, Width, 0);
//
//				ControlPaint.DrawBorder3D(
//										e.Graphics,
//										ClientRectangle,
//										Border3DStyle.Flat);
//			}
//			else

			var graphics = e.Graphics;
			graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
//			graphics.SmoothingMode = SmoothingMode.HighQuality;
			RenderGraphics(graphics);
		}
		#endregion


		#region Methods
		/// <summary>
		/// Shunts rendering off to TopViewPanelParent and QuadrantPanel.
		/// </summary>
		protected virtual void RenderGraphics(Graphics graphics)
		{}
		#endregion
	}

/*	public class DoubleBufferControl
		:
			Control
	{
		private BufferedGraphicsContext context;
		private BufferedGraphics backBuffer;

		public DoubleBufferControl()
		{
			this.SuspendLayout();
			this.BackColor = System.Drawing.Color.Black;
			this.Resize += new System.EventHandler(this.doubleBufferControl_Resize);
			this.ResumeLayout(false);

			SetStyle(
					ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer,
					true);

			context = BufferedGraphicsManager.Current;
			context.MaximumBuffer = new Size(this.Width + 1, this.Height + 1);
			backBuffer = context.Allocate(this.CreateGraphics(), ClientRectangle);

			Application.ApplicationExit += new EventHandler(memoryCleanup);
		}

		private void memoryCleanup(object sender, EventArgs e)
		{
			if (backBuffer != null)
				backBuffer.Dispose();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (DesignMode)
			{
				base.OnPaint(e);
				e.Graphics.DrawLine(Pens.Black,0,0,Width,Height);
				e.Graphics.DrawLine(Pens.Black,0,Height,Width,0);
			}
			else
			{
				try
				{
					// draw to back buffer
					Render(backBuffer.Graphics);

					// paint the picture in from the back buffer into the form draw area
					backBuffer.Render(e.Graphics);
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message+"\n"+ex.StackTrace);
				}
			}
		}

		protected virtual void Render(Graphics backBuffer){}

		private void removePaintMethods()
		{
			this.DoubleBuffered = false;

			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, false);

			if (backBuffer != null)
				backBuffer.Dispose();
		}

		private void doubleBufferControl_Resize(object sender, EventArgs e)
		{
			context.MaximumBuffer = new Size(this.Width + 1, this.Height + 1);

			if (backBuffer != null)
				backBuffer.Dispose();

			backBuffer = context.Allocate(this.CreateGraphics(), ClientRectangle);

			this.Refresh();
		}
	} */
}
