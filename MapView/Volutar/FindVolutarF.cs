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
			using (var ofd = new OpenFileDialog())
			{
				ofd.Title      = "Find MCDEdit.exe";
				ofd.Filter     = "Executable files|*.EXE|All files|*.*";
//				ofd.DefaultExt = "EXE";
				ofd.FileName   = "MCDEdit.exe";

				if (ofd.ShowDialog(this) == DialogResult.OK)
					tbInput.Text = ofd.FileName;
			}
		}
		#endregion Events
	}
}
