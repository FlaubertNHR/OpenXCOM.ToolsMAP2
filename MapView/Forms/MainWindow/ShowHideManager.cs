using System.Collections.Generic;
using System.Windows.Forms;


namespace MapView.Forms.MainWindow
{
	internal static class ShowHideManager
	{
		#region Fields (static)
		internal static List<Form> _fOrder = new List<Form>();
		private static readonly List<Form> _visible = new List<Form>();
		#endregion Fields (static)


		#region Methods (static)
		/// <summary>
		/// Hides visible viewers (except MainView) when opening PckView/McdView
		/// via TileView.
		/// </summary>
		internal static void HideViewers()
		{
			_visible.Clear();

			foreach (var f in _fOrder) // don't screw with the iteration of '_fOrder'
				if (f.Visible)
					_visible.Add(f);

			foreach (var f in _visible)
				if (f.Name != "XCMainWindow")
					f.Hide();
		}

		/// <summary>
		/// Shows subsidiary viewers that were previously visible after closing
		/// PckView/McdView via TileView.
		/// </summary>
		internal static void RestoreViewers()
		{
			foreach (var f in _visible)
			{
				if (f.Name != "XCMainWindow")
				{
					f.Show();
					f.WindowState = FormWindowState.Normal;
				}
				else // bring MainView to its previous position in the z-order
				{
					((XCMainWindow)f)._bypassActivatedEvent = true;
					f.TopMost = true;
					f.TopMost = false;
					((XCMainWindow)f)._bypassActivatedEvent = false;
				}
			}
		}
		#endregion Methods (static)
	}
}
