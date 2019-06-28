using System;
using System.IO;
using System.Windows.Forms;

using DSShared.Windows;


namespace MapView.Volutar
{
	/// <summary>
	/// Deals with Volutar's MCD Editor app. Or any app/file really.
	/// </summary>
	internal sealed class VolutarService
	{
		#region Fields (static)
		private const string VOLUTAR = "VolutarMcdEditorPath";
		#endregion Fields (static)


		#region Fields
		private readonly Options Options;
		#endregion Fields


		#region Properties
		private string _fullpath;
		internal string FullPath
		{
			get
			{
				var option = Options.GetOption(VOLUTAR, String.Empty);

				_fullpath = option.Value as String;
				if (!File.Exists(_fullpath))
				{
					using (var f = new FindFileForm("Enter the Volutar MCDEdit.exe path in full."))
					{
						if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
						{
							if (File.Exists(f.InputString))
							{
								option.Value = (object)(_fullpath = f.InputString);
							}
							else
								MessageBox.Show(
											"File not found.",
											" Error",
											MessageBoxButtons.OK,
											MessageBoxIcon.Error,
											MessageBoxDefaultButton.Button1,
											0);
						}
					}
				}
				return _fullpath;
			}
		}
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="options"></param>
		internal VolutarService(Options options)
		{
			Options = options;
		}
		#endregion cTor


		#region Methods (static)
		internal static void LoadOptions(Options options)
		{
			options.AddOption(
							VOLUTAR,
							String.Empty,
							"Path to Volutar MCD Edit" + Environment.NewLine
								+ "The path specified can actually be used to start"
								+ " any valid application or to open a specific file"
								+ " with its associated application.",
							"McdViewer");
		}
		#endregion Methods (static)
	}
}
