using System;
using System.IO;


namespace DSShared
{
	/// <summary>
	/// Class to help pass around (/settings) file-paths.
	/// </summary>
	public sealed class PathInfo
	{
		#region Fields (static)
		// path-keys in SharedSpace
		public const string ShareOptions   = "MV_OptionsFile";		// -> MapOptions.cfg

		public const string ShareResources = "MV_ResourcesFile";	// -> MapResources.yml
		public const string ShareTilesets  = "MV_TilesetsFile";		// -> MapTilesets.yml
		public const string ShareViewers   = "MV_ViewersFile";		// -> MapViewers.yml

		public const string MAN_Viewers    = "MapView._Embedded.MapViewers.yml";
		public const string MAN_Tilesets   = "MapView._Embedded.MapTilesets.yml";

		// Configuration files
		public const string CFG_Options    = "MapOptions.cfg";		// stores user-settings for the viewers

		public const string YML_Resources  = "MapResources.yml";	// stores the installation paths of UFO/TFTD

		public const string YML_Tilesets   = "MapTilesets.yml";		// tilesets file configuration
		public const string TPL_Tilesets   = "MapTilesets.tpl";		// tilesets file template

		public const string YML_Viewers    = "MapViewers.yml";		// various window positions and sizes


		public const string NotConfigured  = "notconfigured";		// used in MapResources.yml in case UFO or TFTD installation is not configured.

		public const string DIR_Settings   = "settings";
		#endregion Fields (static)


		#region Fields
		private readonly string _file;
		#endregion Fields


		#region Properties
		private readonly string _dir;
		/// <summary>
		/// Directory path.
		/// </summary>
		public string DirectoryPath
		{
			get { return _dir; }
		}

		/// <summary>
		/// Gets the fullpath.
		/// kL_question: Can a file or directory end with "." (no, disallowed by
		/// Windows OS)
		/// </summary>
		public string Fullpath
		{
			get { return Path.Combine(_dir, _file); }
		}
		#endregion Properties


		#region cTor
		/// <summary>
		/// Initializes a new instance of the <see cref="T:DSShared.PathInfo" /> class.
		/// </summary>
		/// <param name="dir">a path to the directory</param>
		/// <param name="file">the file with any extension</param>
		public PathInfo(string dir, string file)
		{
			_dir  = dir;
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
			Directory.CreateDirectory(_dir);
		}
		#endregion Methods
	}
}
