using System;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Reflection;
using System.Windows.Forms;

using DSShared;

using MapView.Forms.MainView;


namespace MapView
{
	/// <summary>
	/// The About box.
	/// </summary>
	internal sealed partial class About
		:
			Form
	{
		#region Fields
		private Point _lBase;
		private Point _l;

		private Random _rand = new Random();
		#endregion Fields


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		internal About()
		{
			InitializeComponent();

			string before = String.Format(
										CultureInfo.CurrentCulture,
										"{0:n0}", GC.GetTotalMemory(false));
			Text = "MapView || " + before + " bytes allocated";


			Assembly ass = Assembly.GetExecutingAssembly();

			var sb = new StringBuilder();
#if DEBUG
			sb.Append("debug ");
#else
			sb.Append("release ");
#endif
			DateTime dt = ass.GetLinkerTime();
			sb.Append(String.Format(
								CultureInfo.CurrentCulture,
								"{0:yyyy MMM d} {0:HH}:{0:mm}:{0:ss} UTC", // {0:zzz}
								dt));
			la_Date.Text = sb.ToString();


			sb.Clear();

			Version ver = ass.GetName().Version;
			sb.AppendLine("MapView   .exe " + ver.Major + "."
											+ ver.Minor + "."
											+ ver.Build + "."
											+ ver.Revision);

			ver = Assembly.Load("McdView").GetName().Version;
			sb.AppendLine("McdView   .exe " + ver.Major + "."
											+ ver.Minor + "."
											+ ver.Build + "."
											+ ver.Revision);

			ver = Assembly.Load("PckView").GetName().Version;
			sb.AppendLine("PckView   .exe " + ver.Major + "."
											+ ver.Minor + "."
											+ ver.Build + "."
											+ ver.Revision);

			ver = Assembly.Load("XCom").GetName().Version;
			sb.AppendLine("XCom      .dll " + ver.Major + "."
											+ ver.Minor + "."
											+ ver.Build + "."
											+ ver.Revision);

			ver = Assembly.Load("DSShared").GetName().Version;
			sb.AppendLine("DSShared  .dll " + ver.Major + "."
											+ ver.Minor + "."
											+ ver.Build + "."
											+ ver.Revision);

			ver = Assembly.Load("YamlDotNet").GetName().Version;
			sb.Append("YamlDotNet.dll " + ver.Major + "."
										+ ver.Minor + "."
										+ ver.Build + "."
										+ ver.Revision);

			tb_Ver.Text = sb.ToString();

			tb_Ver.SelectionStart = tb_Ver.SelectionLength = 0;
		}
		#endregion cTor


		#region Events (override)
		/// <summary>
		/// Sets the base-location on the shown event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnShown(EventArgs e)
		{
			_lBase = Location;
		}

		/// <summary>
		/// Adds this form to the zOrder on the activated event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnActivated(EventArgs e)
		{
			ShowHideManager._zOrder.Remove(this);
			ShowHideManager._zOrder.Add(this);
		}

		/// <summary>
		/// Handles <c>KeyDown</c> events.
		/// </summary>
		/// <param name="e"></param>
		/// <remarks>[Esc] or [Ctrl+b] closes the About box. [Enter] starts and
		/// stops brownian motion.
		/// 
		/// 
		/// Requires <c>KeyPreview</c> <c>true</c>.</remarks>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			switch (e.KeyData)
			{
				case Keys.Escape:
				case Keys.Control | Keys.B:
					Close();
					break;

				case Keys.Enter:
					if (t1.Enabled) t1.Enabled = false;
					else            t1.Enabled = true;
					break;
			}
		}

		/// <summary>
		/// Handles the <c>FormClosing</c> event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			if (!RegistryInfo.FastClose(e.CloseReason)
				&& !MainViewF.Quit)
			{
				MainViewF.that.DecheckAbout();
			}
			base.OnFormClosing(e);
		}
		#endregion Events (override)


		#region Events
		/// <summary>
		/// Does brownian movement on each tick if enabled.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnTick(object sender, EventArgs e)
		{
			// NOTE: Although the dialog doesn't have Minimize or Maximize
			// buttons it can be minimized if user does ShowDesktop per the OS.

			if (WindowState == FormWindowState.Normal)
			{
				_l = Location;
				_l.X += _rand.Next() % 3 - 1;
				_l.Y += _rand.Next() % 3 - 1;

				bool isInsideBounds = false;
				foreach (var screen in Screen.AllScreens)
				{
					if (isInsideBounds = screen.Bounds.Contains(_l)
									  && screen.Bounds.Contains(_l + Size))
					{
						break;
					}
				}

				if (!isInsideBounds)
					_l = _lBase;

				Location = _l;
			}
		}
		#endregion Events
	}
}
