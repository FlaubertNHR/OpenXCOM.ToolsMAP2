using System;
using System.IO;


namespace DSShared
{
	public static class GlobalsXC
	{
		#region Fields (static)
		public const string MapsDir     = "MAPS";
		public const string MapExt      = ".MAP";

		public const string RoutesDir   = "ROUTES";
		public const string RouteExt    = ".RMP";

		public const string TerrainDir  = "TERRAIN";
		public const string PckExt      = ".PCK";
		public const string TabExt      = ".TAB";
		public const string McdExt      = ".MCD";

		public const string GeodataDir  = "GEODATA";
		public const string DatExt      = ".DAT"; // for SCANG.DAT

		public const string UfographDir = "UFOGRAPH";

		public const string MV_Backup   = "MV_Backup";
		public const string TEMPExt     = ".t";


		// const-strings that appear in "MapTilesets.yml"
		public const string TILESETS = "tilesets";
		public const string GROUP    = "group";
		public const string CATEGORY = "category";
		public const string TYPE     = "type";
		public const string TERRAINS = "terrains";
		public const string BASEPATH = "basepath";
		public const string BYPASSRE = "bypassRecordsExceeded";


		public static readonly string[] CRandorLF = { "\r\n","\r","\n" }; // fxCop ca2105 - wants a strongly typed collection

		public const string PADDED_SEPARATOR = " - ";
		public const string PADDED_ASTERISK  = " *";

		public const string PckExt_lc = ".pck";

		public const string PngExt    = ".PNG";
		public const string GifExt    = ".GIF";
		#endregion Fields (static)


		#region Methods (static)
		/// <summary>
		/// Parses the sprite-shade value out of "settings/MapOptions.Cfg".
		/// </summary>
		/// <param name="dirAppL"></param>
		/// <returns></returns>
		public static string GetSpriteShade(string dirAppL)
		{
			string dir = Path.Combine(dirAppL, PathInfo.DIR_Settings);	// "settings"
			string pfe = Path.Combine(dir,     PathInfo.CFG_Options);	// "MapOptions.cfg"

			using (var fs = FileService.OpenFile(pfe))
			if (fs != null)
			using (var sr = new StreamReader(fs))
			{
				string line;
				while ((line = sr.ReadLine()) != null)
				{
					line = line.Trim();
					if (line.StartsWith("SpriteShade", StringComparison.Ordinal))
						return line.Substring(12);
				}
			}
			return null;
		}
		#endregion Methods (static)
	}
}
