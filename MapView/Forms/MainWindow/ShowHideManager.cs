using System.Collections.Generic;
using System.Windows.Forms;


namespace MapView.Forms.MainWindow
{
	internal static class ShowHideManager
	{
		#region Fields (static)
		/// <summary>
		/// A list of Forms that updates in various Activated events.
		/// - MainView
		/// - TileView
		/// - TopView
		/// - RouteView
		/// - TopRouteView
		/// - ColorsHelp
		/// - About
		/// @note The list is maintained in reverse order - the first entry is
		/// the earliest form activated, the last entry is the latest. It works
		/// well like that.
		/// </summary>
		internal static List<Form> _zOrder = new List<Form>();

		/// <summary>
		/// Exclusively for use when invoking McdView or PckView from TileView.
		/// </summary>
		private static readonly List<Form> _visible = new List<Form>();
		#endregion Fields (static)


		#region Methods (static)
		/// <summary>
		/// Hides visible viewers (except MainView) when invoking PckView or
		/// McdView via TileView. Is used in conjunction with RestoreViewers().
		/// </summary>
		internal static void HideViewers()
		{
			_visible.Clear();

			foreach (var f in _zOrder) // don't screw with the iteration of '_fOrder'
				if (f.Visible)
					_visible.Add(f);

			foreach (var f in _visible)
				if (f.Name != "XCMainWindow")
					f.Hide();
		}

		/// <summary>
		/// Shows subsidiary viewers that were previously visible after closing
		/// PckView or McdView via TileView. Is used in conjunction with
		/// HideViewers().
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
					XCMainWindow.BypassActivatedEvent = true;
					f.TopMost = true;
					f.TopMost = false;
					XCMainWindow.BypassActivatedEvent = false;
				}
			}
		}


		/// <summary>
		/// Gets the current z-order of visible Forms and returns it in a List.
		/// @note Used by MinimizeAll, RestoreAll, and MainView activation
		/// (BringAllToFront).
		/// </summary>
		/// <returns></returns>
		internal static List<Form> getZorderList()
		{
			var zOrder = new List<Form>();
			foreach (var f in _zOrder) // don't screw with the iteration of '_fOrder'
				if (f.Visible)
					zOrder.Add(f);

			return zOrder;
		}
		#endregion Methods (static)
	}
}
