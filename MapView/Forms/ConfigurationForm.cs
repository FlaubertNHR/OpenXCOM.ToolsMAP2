using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

using DSShared;

using YamlDotNet.Serialization; // write values (serialization)


namespace MapView
{
	internal sealed partial class ConfigurationForm
		:
			Form
	{
		#region Fields
		private PathInfo _piResources = SharedSpace.GetShareObject(PathInfo.ShareResources) as PathInfo;
		private PathInfo _piTilesets  = SharedSpace.GetShareObject(PathInfo.ShareTilesets)  as PathInfo;
		#endregion Fields


		#region Properties
		private string Ufo
		{
			get { return tbUfo.Text; }
			set { tbUfo.Text = value; }
		}

		private string Tftd
		{
			get { return tbTftd.Text; }
			set { tbTftd.Text = value; }
		}
		#endregion Properties


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="restart">true if MapView needs to restart to effect
		/// changes (default false)</param>
		internal ConfigurationForm(bool restart = false)
		{
			InitializeComponent();

			if (restart)
			{
				toolTip1.SetToolTip(cbResources, "auto restart! Create paths to"
											   + " stock UFO/TFTD installations");
				toolTip1.SetToolTip(rbTilesets, "auto restart! WARNING : This will replace"
											  + " any custom tileset configuration");
			}

			// WORKAROUND: See note in MainViewF cTor.
			MaximumSize = new System.Drawing.Size(0,0); // fu.net

			if (!_piResources.FileExists())
			{
				cbResources.Enabled = false;
			}
			else
				cbResources.Checked = false;

			if (!_piTilesets.FileExists())
			{
				cbTilesets   .Enabled =
				rbTilesets   .Enabled =
				rbTilesetsTpl.Enabled = false;
			}
			else
			{
				cbTilesets.Checked = false;
				rbTilesetsTpl.Select();
			}

			string ufo = SharedSpace.GetShareString(SharedSpace.ResourceDirectoryUfo);
			if (!String.IsNullOrEmpty(ufo))
				Ufo = ufo;

			string tftd = SharedSpace.GetShareString(SharedSpace.ResourceDirectoryTftd);
			if (!String.IsNullOrEmpty(tftd))
				Tftd = tftd;
		}
		#endregion cTor


		#region Events
		private void OnResourcesCheckedChanged(object sender, EventArgs e)
		{
			gbResources.Visible = cbResources.Checked;

			btnOk.Enabled = cbResources.Checked
						 || cbTilesets .Checked;
		}

		private void OnTilesetsCheckedChanged(object sender, EventArgs e)
		{
			rbTilesets   .Visible =
			rbTilesetsTpl.Visible = cbTilesets.Checked;

			btnOk.Enabled = cbTilesets .Checked
						 || cbResources.Checked;
		}

		/// <summary>
		/// Opens a dialog to find a UFO installation/resource folder.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnFindUfoClick(object sender, EventArgs e)
		{
			using (var fbd = new FolderBrowserDialog())
			{
				fbd.Description = "Select UFO Resources directory."
							  + Environment.NewLine + Environment.NewLine
							  + "- the parent of MAPS, ROUTES, TERRAIN, and UFOGRAPH (typically)";

				fbd.SelectedPath = Path.GetDirectoryName(Application.ExecutablePath);


				if (fbd.ShowDialog(this) == DialogResult.OK)
					Ufo = fbd.SelectedPath;
			}
		}

		/// <summary>
		/// Opens a dialog to find a TFTD installation/resource folder.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnFindTftdClick(object sender, EventArgs e)
		{
			using (var fbd = new FolderBrowserDialog())
			{
				fbd.Description = "Select TFTD Resources directory"
							  + Environment.NewLine + Environment.NewLine
							  + "- the parent of MAPS, ROUTES, TERRAIN, and UFOGRAPH (typically)";

				fbd.SelectedPath = Path.GetDirectoryName(Application.ExecutablePath);


				if (fbd.ShowDialog(this) == DialogResult.OK)
					Tftd = fbd.SelectedPath;
			}
		}

		/// <summary>
		/// Applies new configuration settings and closes this dialog.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnAcceptClick(object sender, EventArgs e)
		{
			if (!rbTilesets.Enabled || !rbTilesets.Checked
				|| MessageBox.Show(
								this,
								"ARE YOU SURE YOU WANT TO REPLACE YOUR TILESET CONFIGURATION FILE!"
									+ Environment.NewLine + Environment.NewLine
									+ "The file contains all data for the MapTree",
								" Replace tileset data",
								MessageBoxButtons.YesNo,
								MessageBoxIcon.Exclamation,
								MessageBoxDefaultButton.Button2,
								0) == DialogResult.Yes)
			{
				if (cbResources.Checked) // handle resource path(s) configuration ->
				{
					Ufo  = Ufo .Trim();
					Tftd = Tftd.Trim();

					if (Ufo.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
						Ufo = Ufo.Substring(0, Ufo.Length - 1);

					if (Tftd.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
						Tftd = Tftd.Substring(0, Tftd.Length - 1);

					if (String.IsNullOrEmpty(Ufo) && String.IsNullOrEmpty(Tftd))
					{
						ShowError("Both folders cannot be blank.");
						return;
					}

					if (!String.IsNullOrEmpty(Ufo) && !Directory.Exists(Ufo))
					{
						ShowError("The UFO folder does not exist.");
						return;
					}

					if (!String.IsNullOrEmpty(Tftd) && !Directory.Exists(Tftd))
					{
						ShowError("The TFTD folder does not exist.");
						return;
					}


					// check for a valid CursorSprite
					string CursorPck = SharedSpace.CursorFilePrefix + GlobalsXC.PckExt;
					string CursorTab = SharedSpace.CursorFilePrefix + GlobalsXC.TabExt;

					if (   (!File.Exists(Path.Combine(Ufo,  CursorPck)) || !File.Exists(Path.Combine(Ufo,  CursorTab)))
						&& (!File.Exists(Path.Combine(Tftd, CursorPck)) || !File.Exists(Path.Combine(Tftd, CursorTab))))
					{
						using (var f = new Infobox(
												"Error",
												"A valid UFO or TFTD resource directory must exist with the XCOM cursor files.",
												@"<basepath>" + Path.DirectorySeparatorChar + CursorPck
													+ Environment.NewLine +
												@"<basepath>" + Path.DirectorySeparatorChar + CursorTab))
						{
							f.ShowDialog(this);
						}
						return;
					}


					// create "settings/MapResources.yml"
					string pfeT;
					if (File.Exists(_piResources.Fullpath))
						pfeT = _piResources.Fullpath + GlobalsXC.TEMPExt;
					else
						pfeT = _piResources.Fullpath;

					bool fail = true;
					using (var fs = FileService.CreateFile(pfeT))
					if (fs != null)
					{
						fail = false;

						using (var sw = new StreamWriter(fs))
						{
							object node = new
							{
								ufo  = (!String.IsNullOrEmpty(Ufo)  ? Ufo  : PathInfo.NotConfigured),
								tftd = (!String.IsNullOrEmpty(Tftd) ? Tftd : PathInfo.NotConfigured)
							};
							var ser = new Serializer();
							ser.Serialize(sw, node);
						}
					}

					if (!fail && pfeT != _piResources.Fullpath)
						FileService.ReplaceFile(_piResources.Fullpath);

					DialogResult = DialogResult.OK; // close Configurator and reload MapView
				}

				if (cbTilesets.Checked) // deal with MapTilesets.yml/.tpl ->
				{
					// create "settings/MapTilesets.[yml|tpl]"
					string pfe, pfeT;
					if (rbTilesets.Checked)
					{
						if (File.Exists(pfe = _piTilesets.Fullpath))
							pfeT = pfe + GlobalsXC.TEMPExt;
						else
							pfeT = pfe;
					}
					else // rbTilesetsTpl.Checked
					{
//						string dir = SharedSpace.GetShareString(SharedSpace.SettingsDirectory);
//						pfe = pfeT = Path.Combine(dir, PathInfo.TPL_Tilesets);

						pfe  =
						pfeT = Path.Combine(_piTilesets.DirectoryPath, PathInfo.TPL_Tilesets);
					}

					bool fail = true;
					using (var fs = FileService.CreateFile(pfeT))
					if (fs != null)
					{
						fail = false;

						using (var sw = new StreamWriter(fs))
						using (var sr = new StreamReader(Assembly.GetExecutingAssembly()
																 .GetManifestResourceStream(PathInfo.MAN_Tilesets)))
						{
							string line;
							while ((line = sr.ReadLine()) != null)
								sw.WriteLine(line);
						}
					}

					if (!fail && pfeT != pfe)
						FileService.ReplaceFile(pfe);

					if (rbTilesets.Checked)
					{
						DialogResult = DialogResult.OK; // close Configurator and reload MapView
					}
					else if (!fail) // rbTilesetsTpl.Checked
					{
						using (var f = new Infobox(
												"Info",
												"Tileset template has been created.",
												pfe))
						{
							f.ShowDialog(this);
						}
					}
				}
			}
		}

		/// <summary>
		/// Closes the form.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnCancelClick(object sender, EventArgs e)
		{
			Close();
		}
		#endregion Events


		#region Methods
		/// <summary>
		/// Wrapper for Infobox.ShowDialog().
		/// </summary>
		/// <param name="error">an error-string to show</param>
		private void ShowError(string error)
		{
			using (var f = new Infobox("Error", error))
				f.ShowDialog(this);
		}
		#endregion Methods
	}
}
