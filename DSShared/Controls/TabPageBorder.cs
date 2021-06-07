using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;


namespace DSShared.Controls
{
	// https://stackoverflow.com/questions/7768555/tabcontrol-and-borders-visual-glitch/7785745#7785745
	/// <summary>
	/// Dedicated to the pursuit of inelegant .NET workarounds.
	/// eg,
	/// public Form1()
	/// {
	/// 	InitializeComponent();
	/// 	var tpBorder = new TabPageBorder(tabControl);	// ca1804: "Use this variable or remove it."
	/// 	// or
	/// 	new TabPageBorder(tabControl1);					// "Possible unassigned object creating by 'new' expression."
	/// 	// so ->
	/// 	var tpBorder = new TabPageBorder(tabControl);	// foff thx!
	/// 	tpBorder.TabPageBorder_init();
	/// }
	/// </summary>
	public sealed class TabPageBorder
		:
			NativeWindow
	{
		#region Fields (static)
		private const int WM_PAINT = 0xF;
		#endregion Fields (static)


		#region Fields
		private readonly TabControl _tabControl;
		#endregion Fields


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="tc"></param>
		public TabPageBorder(TabControl tc)
		{
			_tabControl = tc;
		}
		#endregion cTor


		#region Methods (init)
		/// <summary>
		/// Subscribes events.
		/// </summary>
		public void TabPageBorder_init()
		{
			_tabControl.HandleCreated   += tabControl_HandleCreated;	// TODO: Does this object need to unsubscribe
			_tabControl.HandleDestroyed += tabControl_HandleDestroyed;	//       the TabControl events.
//			_tabControl.Selected        += tabControl_Selected;
		}
		#endregion Methods (init)


		#region Events
		private void tabControl_HandleCreated(object sender, EventArgs e)
		{
			AssignHandle(_tabControl.Handle);
		}

		private void tabControl_HandleDestroyed(object sender, EventArgs e)
		{
			ReleaseHandle();
		}

//		void tabControl_Selected(object sender, TabControlEventArgs e)
//		{
//			_tabControl.Invalidate();
//		}
		#endregion Events


		#region Methods (override)
		protected override void WndProc(ref Message m)
		{
			base.WndProc(ref m);

			if (m.Msg == WM_PAINT)
			{
				using (Graphics graphics = Graphics.FromHwnd(m.HWnd))
				{
					if (_tabControl.Parent != null) // replace the outside white borders ->
					{
						graphics.SetClip(new Rectangle(0,0, _tabControl.Width - 2, _tabControl.Height - 1), CombineMode.Exclude);
						using (var brush = new SolidBrush(_tabControl.Parent.BackColor))
							graphics.FillRectangle(brush, new Rectangle(
																	0,
																	_tabControl.ItemSize.Height + 2,
																	_tabControl.Width,
																	_tabControl.Height - (_tabControl.ItemSize.Height + 2)));
					}

					if (_tabControl.SelectedTab != null) // replace the inside white borders ->
					{
						graphics.ResetClip();
						Rectangle rect = _tabControl.SelectedTab.Bounds;
						graphics.SetClip(rect, CombineMode.Exclude);
						using (var brush = new SolidBrush(_tabControl.SelectedTab.BackColor))
							graphics.FillRectangle(brush, new Rectangle(
																	rect.Left   - 3,
																	rect.Top    - 1,
																	rect.Width  + 4,
																	rect.Height + 3));
					}
				}
			}
		}
		#endregion Methods (override)
	}



	/// <summary>
	/// Derived class for a TabControl.
	/// </summary>
	public sealed class CompositedTabControl
		:
			TabControl
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


//		// Can be SUPERCEDED BY DSShared.Windows.TabPageBorder (NativeWindow) - do not
//		// implement both.
//		//
//		// The following removes white padding on the interior of the tabpages.
//		// This also effectively expands the control-area of each tabpage (to
//		// the left and top at least). It leaves the white border on the
//		// exterior to the right and below a page's 1px black border.
//		// https://stackoverflow.com/questions/7768555/tabcontrol-and-borders-visual-glitch/7785745#32055608
//		// - works fine on my w7 machine but who knows
//		private struct RECT { public int Left, Top, Right, Bottom; }
//
//		protected override void WndProc(ref Message m)
//		{
//			if (m.Msg == 0x1300 + 40)
//			{
//				var rc = (RECT)m.GetLParam(typeof(RECT));
//				rc.Left   -= 3;
//				rc.Top    -= 1;
//				rc.Right  += 1; // NOTE: There's still a white margin outside the page to the right.
//				rc.Bottom += 2; // NOTE: There's still a white margin outside the page to the bottom.
//				System.Runtime.InteropServices.Marshal.StructureToPtr(rc, m.LParam, true);
//			}
//			base.WndProc(ref m);
//		}
	}
}
