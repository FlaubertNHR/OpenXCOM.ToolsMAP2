using System;
using System.Windows.Forms;


namespace McdView
{
	internal sealed class RecordTextbox
		:
			TextBox
	{
		#region Fields (static)
//		private static McdviewF _f;

		private static ToolStripStatusLabel StatusLabel;
		private static Label DescriptionLabel;
		#endregion Fields (static)


/*		#region cTor
		/// <summary>
		/// cTor. Instantiates a RecordTextbox.
		/// </summary>
		internal RecordTextbox()
		{}
		#endregion cTor */


		#region Methods (static)
//		internal static void SetParent(McdviewF f)
//		{
//			_f = f;
//		}

		internal static void SetStatusLabel(ToolStripStatusLabel label)
		{
			StatusLabel = label;
		}

		internal static void SetDescriptionLabel(Label label)
		{
			DescriptionLabel = label;
		}
		#endregion Methods (static)


		#region Events (override)
//		protected override void OnClick(EventArgs e)
//		{
//			_f.PartsPanel.Select();
//		}

		/// <summary>
		/// Handles mouseover leaving this RecordTextbox.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseLeave(EventArgs e)
		{
			StatusLabel.Text = String.Empty;
			if (!Focused)
				DescriptionLabel.Text = String.Empty;
		}

		/// <summary>
		/// Handles this RecordTextbox losing focus.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLeave(EventArgs e)
		{
			DescriptionLabel.Text = String.Empty;
		}
		#endregion Events (override)
	}
}
