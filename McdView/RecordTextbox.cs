using System;
using System.Windows.Forms;


namespace McdView
{
	internal sealed class RecordTextbox
		:
			TextBox
	{
		#region Fields (static)
		private static ToolStripStatusLabel StatusLabel;
		private static Label DescriptLabel;
		#endregion Fields (static)


		#region Methods (static)
		/// <summary>
		/// Initializes 'StatusLabel' and 'DescriptLabel'.
		/// </summary>
		/// <param name="status"></param>
		/// <param name="descript"></param>
		internal static void SetStaticVars(
				ToolStripStatusLabel status,
				Label descript)
		{
			StatusLabel   = status;
			DescriptLabel = descript;
		}
		#endregion Methods (static)


		#region Events (override)
		/// <summary>
		/// Clears labels when mouseover leaves this RecordTextbox.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseLeave(EventArgs e)
		{
			StatusLabel.Text = String.Empty;
			if (!Focused)
				DescriptLabel.Text = String.Empty;
		}

		/// <summary>
		/// Clears description when this RecordTextbox loses focus.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLeave(EventArgs e)
		{
			DescriptLabel.Text = String.Empty;
		}
		#endregion Events (override)
	}
}
