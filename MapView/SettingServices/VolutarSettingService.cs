using System;
using System.IO;
using System.Windows.Forms;

using DSShared.Windows;


namespace MapView.OptionsServices
{
	/// <summary>
	/// Deals with Volutar's MCD Editor app. Or any app/file really.
	/// </summary>
	internal sealed class VolutarSettingService
	{
		#region Fields (static)
		private const string VolutarMcdEditorPath = "VolutarMcdEditorPath";
		#endregion


		#region Fields
		private readonly Options _options;
		#endregion


		#region Properties
		private string _fullpath;
		internal string FullPath
		{
			get
			{
				var option = _options.GetOption(VolutarMcdEditorPath, String.Empty);

				_fullpath = option.Value as String;
				if (!File.Exists(_fullpath))
				{
					using (var f = new FindFileForm("Enter the Volutar MCD Editor Path in full."))
					{
						if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
						{
							if (File.Exists(f.InputString))
							{
								_fullpath = f.InputString;
								option.Value = (object)f.InputString;
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
		#endregion


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="options"></param>
		internal VolutarSettingService(Options options)
		{
			_options = options;
		}
		#endregion


		#region Methods (static)
		public static void LoadOptions(Options options)
		{
			options.AddOption(
							VolutarMcdEditorPath,
							String.Empty,
							"Path to Volutar MCD Editor" + Environment.NewLine
								+ "note: The path specified can actually be "
								+ "used to start any valid program or to open "
								+ "a specific file with its associated application.",
							"McdViewer");
		}
		#endregion
	}
}
