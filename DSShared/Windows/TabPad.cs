using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;


namespace DSShared.Windows
{
	// https://stackoverflow.com/questions/7768555/tabcontrol-and-borders-visual-glitch/7785745#7785745
	/// <summary>
	/// Dedicated to the pursuit of inelegant .NET workarounds.
	/// eg,
	/// public Form1()
	/// {
	/// 	InitializeComponent();
	/// 	var tab = new TabPad(tabControl1);
	/// }
	/// </summary>
	public sealed class TabPad
		:
			NativeWindow
	{
		#region Fields (static)
		private const int WM_PAINT = 0xF;
		#endregion Fields (static)


		#region Fields
		private readonly TabControl tabControl;
		#endregion Fields


		#region cTor
		public TabPad(TabControl tc)
		{
			tabControl = tc;
			tabControl.Selected += tabControl_Selected;

			AssignHandle(tc.Handle);
		}
		#endregion cTor


		#region Events
		void tabControl_Selected(object sender, TabControlEventArgs e)
		{
			tabControl.Invalidate();
		}
		#endregion Events


		#region Methods (override)
		protected override void WndProc(ref Message m)
		{
			base.WndProc(ref m);

			if (m.Msg == WM_PAINT)
			{
				using (Graphics g = Graphics.FromHwnd(m.HWnd))
				{
					if (tabControl.Parent != null) // replace the outside white borders ->
					{
						g.SetClip(new Rectangle(0, 0, tabControl.Width - 2, tabControl.Height - 1), CombineMode.Exclude);
						using (var sb = new SolidBrush(tabControl.Parent.BackColor))
							g.FillRectangle(sb, new Rectangle(
															0,
															tabControl.ItemSize.Height + 2,
															tabControl.Width,
															tabControl.Height - (tabControl.ItemSize.Height + 2)));
					}

					if (tabControl.SelectedTab != null) // replace the inside white borders ->
					{
						g.ResetClip();
						Rectangle r = tabControl.SelectedTab.Bounds;
						g.SetClip(r, CombineMode.Exclude);
						using (var sb = new SolidBrush(tabControl.SelectedTab.BackColor))
							g.FillRectangle(sb, new Rectangle(
															r.Left - 3,
															r.Top - 1,
															r.Width + 4,
															r.Height + 3));
					}
				}
			}
		}
		#endregion Methods (override)
	}



	/// <summary>
	/// Parent class for TabControl.
	/// </summary>
	public sealed class CompositedTabControl
		:
			TabControl
	{
		#region Events (override)
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


/*		// Can be SUPERCEDED BY DSShared.Windows.TabPad (NativeWindow) - do not
		// implement both.
		//
		// The following removes white padding on the interior of the tabpages.
		// This also effectively expands the control-area of each tabpage (to
		// the left and top at least). It leaves the white border on the
		// exterior to the right and below a page's 1px black border.
		// https://stackoverflow.com/questions/7768555/tabcontrol-and-borders-visual-glitch/7785745#32055608
		// - works fine on my w7 machine but who knows
		private struct RECT { public int Left, Top, Right, Bottom; }

		protected override void WndProc(ref Message m)
		{
			if (m.Msg == 0x1300 + 40)
			{
				var rc = (RECT)m.GetLParam(typeof(RECT));
				rc.Left   -= 3;
				rc.Top    -= 1;
				rc.Right  += 1; // NOTE: There's still a white margin outside the page to the right.
				rc.Bottom += 2; // NOTE: There's still a white margin outside the page to the bottom.
				System.Runtime.InteropServices.Marshal.StructureToPtr(rc, m.LParam, true);
			}
			base.WndProc(ref m);
		} */
		#endregion Events (override)
	}



	/// <summary>
	/// Used by StatusStrips to get rid of white borders.
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
		{}
	}
}
