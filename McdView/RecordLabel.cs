using System;
using System.Windows.Forms;


namespace McdView
{
	internal sealed class RecordLabel
		:
			Label
	{
		#region Fields (static)
		private static McdviewF _f;

		private static ToolStripStatusLabel StatusLabel;
		private static Label DescriptLabel;
		#endregion Fields (static)


		#region Methods (static)
		/// <summary>
		/// Initializes <c><see cref="_f"/></c>,
		/// <c><see cref="StatusLabel"/></c>, and
		/// <c><see cref="DescriptLabel"/></c>.
		/// </summary>
		/// <param name="status"></param>
		/// <param name="descript"></param>
		/// <param name="f"></param>
		internal static void SetStaticVars(
					ToolStripStatusLabel status,
					Label descript,
					McdviewF f)
		{
			StatusLabel   = status;
			DescriptLabel = descript;

			_f = f;
		}
		#endregion Methods (static)


		#region Events (override)
		/// <summary>
		/// Clears labels when mouseover leaves this <c>RecordLabel</c>.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseLeave(EventArgs e)
		{
			StatusLabel.Text = String.Empty;

			var tb = Tag as RecordTextbox;
			if (!((tb != null && tb.Focused) || _f.isScanG_tb_focused(this)))
				DescriptLabel.Text = String.Empty;
		}

		/// <summary>
		/// Selects the <c><see cref="McdviewF.PartsPanel"/></c> when this
		/// <c>RecordLabel</c> is clicked.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnClick(EventArgs e)
		{
			_f.PartsPanel.Select();
		}
		#endregion Events (override)
	}
}
