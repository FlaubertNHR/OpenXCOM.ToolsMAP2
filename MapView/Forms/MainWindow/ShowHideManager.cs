using System.Collections.Generic;
using System.Windows.Forms;


namespace MapView.Forms.MainWindow
{
	internal sealed class ShowHideManager
	{
		#region Fields
		private readonly IEnumerable<Form> _ffs;
		private readonly List<Form> _visible = new List<Form>();
		#endregion Fields


		#region cTor
		internal ShowHideManager(IEnumerable<Form> ffs)
		{
			_ffs = ffs;
		}
		#endregion cTor


		#region Methods
		/// <summary>
		/// Hides visible viewers (except MainView) when opening PckView/McdView
		/// via TileView.
		/// </summary>
		internal void HideViewers()
		{
			_visible.Clear();
			foreach (var f in _ffs)
				if (f.Visible)
				{
					f.Hide();
					_visible.Add(f);
				}
		}

		/// <summary>
		/// Shows subsidiary viewers that were previously visible after closing
		/// PckView/McdView via TileView.
		/// </summary>
		internal void RestoreViewers()
		{
			foreach (var f in _visible)
			{
				f.Show();
				f.WindowState = FormWindowState.Normal;
			}
		}
		#endregion Methods
	}
}
