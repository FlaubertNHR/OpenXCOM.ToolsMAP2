using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using XCom;


namespace McdView
{
	/// <summary>
	/// The panel that displays the entire spriteset.
	/// </summary>
	internal sealed class SpriteCollectionPanel
		:
			Panel
	{
		#region Fields
		private readonly McdviewF _f;
		#endregion Fields


		#region Properties
		internal SpriteCollection SpriteCollection
		{ private get; set; }
		#endregion Properties


		#region cTor
		internal SpriteCollectionPanel(McdviewF f)
		{
			_f = f;

			SetStyle(ControlStyles.OptimizedDoubleBuffer
				   | ControlStyles.AllPaintingInWmPaint
				   | ControlStyles.UserPaint
				   | ControlStyles.ResizeRedraw, true);

			BackColor = SystemColors.Desktop;
			Location = new Point(5, 15);
			Margin = new Padding(0);
			Name = "pnl_Collection";
			Size = new Size(670, 60);
			TabIndex = 0;
			Paint += OnPaintPanel;
		}
		#endregion cTor


		#region Events
		private void OnPaintPanel(object sender, PaintEventArgs e)
		{
			if (SpriteCollection != null && SpriteCollection.Count != 0)
			{
				var graphics = e.Graphics;
				graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

				for (int i = 0; i != SpriteCollection.Count; ++i)
				{
					
				}
			}
		}
		#endregion Events
	}
}
