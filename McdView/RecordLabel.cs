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
		/// Initializes '_f', 'StatusLabel', and 'DescriptLabel'.
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
		/// Clears labels when mouseover leaves this RecordLabel.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseLeave(EventArgs e)
		{
			StatusLabel.Text = String.Empty;

			if (!_f.isAssociatedTextboxFocused(this))
				DescriptLabel.Text = String.Empty;
		}

		/// <summary>
		/// Selects the PartsPanel when this RecordLabel is clicked.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnClick(EventArgs e)
		{
			_f.PartsPanel.Select();
		}
		#endregion Events (override)
	}
}
