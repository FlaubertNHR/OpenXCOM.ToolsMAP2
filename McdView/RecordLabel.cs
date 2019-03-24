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
		private static Label DescriptionLabel;
		#endregion Fields (static)


/*		#region cTor
		/// <summary>
		/// cTor. Instantiates a RecordLabel.
		/// </summary>
		internal RecordLabel()
		{}
		#endregion cTor */


		#region Methods (static)
		internal static void SetParent(McdviewF f)
		{
			_f = f;
		}

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
		protected override void OnClick(EventArgs e)
		{
			_f.PartsPanel.Select();
		}

		/// <summary>
		/// Handles mouseover leaving this RecordLabel.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseLeave(EventArgs e)
		{
			StatusLabel.Text = String.Empty;

			if (!_f.isAssociatedTextboxFocused(this))
				DescriptionLabel.Text = String.Empty;

//			TextBox tb = _f.GetTextbox(this);
//			if (tb == null || !tb.Focused)
//				DescriptionLabel.Text = String.Empty;
		}
		#endregion Events (override)
	}
}
