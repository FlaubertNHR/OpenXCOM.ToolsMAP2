using System;
using System.IO;
using System.Windows.Forms;

using DSShared;

using MapView.Forms.Observers;


namespace MapView.Volutar
{
	/// <summary>
	/// Deals with Volutar's MCD Editor app. Or any app/file really.
	/// </summary>
	internal sealed class VolutarService
	{
		#region Fields (static)
		internal const string VOLUTAR = "VolutarMcdEditorPath";
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
					using (var volutar = new FindVolutarF())
					{
						if (volutar.ShowDialog() == DialogResult.OK)
						{
							if (File.Exists(volutar.InputString))
							{
								_fullpath = volutar.InputString;
								TileView.Optionables.VolutarMcdEditorPath = _fullpath;
								option.Value = (object)_fullpath;

								if (TileView._foptions != null && TileView._foptions.Visible)
								   (TileView._foptions as OptionsForm).propertyGrid.Refresh();
							}
							else
							{
								using (var f = new Infobox(
														"Error",
														"File not found.",
														null,
														InfoboxType.Error))
								{
									f.ShowDialog();
								}
							}
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
		/// <summary>
		/// Adds the Volutar-option with default-value to TileView's options.
		/// </summary>
		/// <param name="options"></param>
		internal static void AddVolutarOptionDefault(Options options)
		{
			options.AddOptionDefault(
								VOLUTAR,
								String.Empty,
								OnVolutarChanged);
		}
		#endregion Methods (static)


		#region Events (static)
		/// <summary>
		/// Volutar never changes.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="val"></param>
		private static void OnVolutarChanged(string key, object val)
		{
			TileView.Optionables.VolutarMcdEditorPath = (String)val;
		}
		#endregion Events (static)
	}
}
