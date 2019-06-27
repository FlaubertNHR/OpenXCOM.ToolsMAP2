using System;
using System.Windows.Forms;


namespace DSShared.Windows
{
	/// <summary>
	/// A generic form providing a textbox for user-input.
	/// </summary>
	public sealed partial class FindFileForm
		:
			Form
	{
		#region Properties
		/// <summary>
		/// Gets the text in the textbox.
		/// </summary>
		public string InputString
		{
			get { return tbInput.Text; }
		}
		#endregion Properties


		#region cTors
		/// <summary>
		/// cTor[1]. Main constructor.
		/// </summary>
		/// <param name="notice">text that will be shown above the textbox</param>
		/// <param name="caption">caption for the titlebar</param>
		/// <param name="input">initial value of the textbox</param>
		public FindFileForm(
				string notice,
				string caption,
				string input)
		{
			InitializeComponent();
			
			Text = caption;
			lblNotice.Text = notice;

			tbInput.Text = input;
		}

		/// <summary>
		/// cTor[2]. Auxiliary constructor. Because.
		/// </summary>
		/// <param name="notice"></param>
		public FindFileForm(string notice)
			:
				this(
					notice,
					" Input",
					String.Empty)
		{}
		#endregion cTors


		#region Events
		private void OnLoad_form(object sender, EventArgs e)
		{
			ActiveControl = btnCancel;
		}

		private void btnFindFile_Click(object sender, EventArgs e)
		{
			using (var f = openFileDialog)
			{
				f.Title = "Find MCDEdit.exe";
				if (f.ShowDialog() == DialogResult.OK)
					tbInput.Text = f.FileName;
			}
		}
		#endregion Events
	}
}
