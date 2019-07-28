using System;
using System.Windows.Forms;


namespace MapView.Volutar
{
	/// <summary>
	/// A generic form providing a textbox for user-input.
	/// </summary>
	public sealed partial class FindVolutarF
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


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		public FindVolutarF()
		{
			InitializeComponent();
		}
		#endregion cTor


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
