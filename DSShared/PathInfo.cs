using System;
using System.IO;


namespace DSShared
{
	/// <summary>
	/// Class to help pass around (/settings) file-paths.
	/// </summary>
	public class PathInfo
	{
		#region Fields (static)
		// path-keys in SharedSpace
		public const string ShareOptions      = "MV_OptionsFile";	// -> MapOptions.cfg

		public const string ShareResources    = "MV_ResourcesFile";	// -> MapResources.yml
		public const string ShareTilesets     = "MV_TilesetsFile";	// -> MapTilesets.yml
		public const string ShareViewers      = "MV_ViewersFile";	// -> MapViewers.yml

		// Configuration files
		public const string ConfigOptions     = "MapOptions.cfg";	// stores user-settings for the viewers

		public const string ConfigResources   = "MapResources.yml";	// stores the installation paths of UFO/TFTD

		public const string ConfigTilesets    = "MapTilesets.yml";	// tilesets file configuration
		public const string ConfigTilesetsOld = "MapTilesets.old";	// tilesets file backup
		public const string ConfigTilesetsTpl = "MapTilesets.tpl";	// tilesets file template

		public const string ConfigViewers     = "MapViewers.yml";	// various window positions and sizes
		public const string ConfigViewersT    = "MapViewers.yml.t";	// tempfile to attempt write


		public const string NotConfigured     = "notconfigured"; // used in MapResources.yml in case UFO or TFTD installation is not configured.

		public const string SettingsDirectory = "settings";
		#endregion Fields (static)


		#region Fields
		private readonly string _file;
		#endregion Fields


		#region Properties
		private readonly string _path;
		/// <summary>
		/// Directory path.
		/// </summary>
		public string DirectoryPath
		{
			get { return _path; }
		}

		/// <summary>
		/// Gets the fullpath.
		/// kL_question: Can a file or directory end with "." (no, disallowed by Windows OS)
		/// </summary>
		public string Fullpath
		{
			get { return Path.Combine(_path, _file); }
		}
		#endregion Properties


		#region cTor
		/// <summary>
		/// Initializes a new instance of the <see cref="T:DSShared.PathInfo" /> class.
		/// </summary>
		/// <param name="path">the path</param>
		/// <param name="file">the file with any extension</param>
		public PathInfo(string path, string file)
		{
			_path = path;
			_file = file;
		}
		#endregion cTor


		#region Methods
		/// <summary>
		/// Checks if the file exists.
		/// </summary>
		/// <returns></returns>
		public bool FileExists()
		{
			return File.Exists(Fullpath);
		}

		/// <summary>
		/// Creates the directory if it does not exist.
		/// </summary>
		public void CreateDirectory()
		{
			Directory.CreateDirectory(_path);
		}
		#endregion Methods
	}
}
