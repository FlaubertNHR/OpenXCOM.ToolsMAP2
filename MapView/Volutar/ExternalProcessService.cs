using System;
using System.IO;
using System.Windows.Forms;

using DSShared;

using MapView.Forms.Observers;


namespace MapView.ExternalProcess
{
	internal sealed class ExternalProcessService
	{
		#region Fields (static)
		internal const string PROCESS = "ExternalProcess";
		#endregion Fields (static)


		#region Fields
		private readonly Options Options;
		#endregion Fields


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="options"></param>
		internal ExternalProcessService(Options options)
		{
			Options = options;
		}
		#endregion cTor


		#region Methods
		/// <summary>
		/// Invokes <c><see cref="ExternalProcessF"/></c> for the user to input a
		/// fullpath (file or application) if one doesn't exist in
		/// <c><see cref="TileViewOptionables.ExternalProcess">TileViewOptionables.ExternalProcess</see></c>
		/// yet.
		/// </summary>
		/// <returns></returns>
		internal string GetFullpath()
		{
			Option option = Options[PROCESS];

			string fullpath = option.Value as String;
			if (!File.Exists(fullpath))
			{
				using (var volutar = new ExternalProcessF())
				{
					if (volutar.ShowDialog() == DialogResult.OK)
					{
						if (File.Exists(volutar.InputString))
						{
							option.SetValue(PROCESS, (fullpath = volutar.InputString));

							if (TileView._foptions != null && TileView._foptions.Visible)
								TileView._foptions.propertyGrid.Refresh();
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
			return fullpath;
		}
		#endregion Methods
	}
}
