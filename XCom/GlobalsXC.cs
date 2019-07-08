namespace XCom
{
	public static class GlobalsXC
	{
		#region Fields (static)
		public const string TerrainDir = "TERRAIN";
		public const string PckExt = ".PCK";
		public const string TabExt = ".TAB";
		public const string McdExt = ".MCD";

		public const string MapsDir = "MAPS";
		public const string MapExt = ".MAP";

		public const string RoutesDir = "ROUTES";
		public const string RouteExt = ".RMP";

		public const string GeodataDir = "GEODATA";
		public const string DatExt = ".DAT"; // for SCANG.DAT

		public const string MV_Backup = "MV_Backup";


		// const-strings that appear in MapTilesets.yml
		public const string TILESETS = "tilesets";
		public const string GROUP    = "group";
		public const string CATEGORY = "category";
		public const string TYPE     = "type";
		public const string TERRAINS = "terrains";
		public const string BASEPATH = "basepath";


		public static string[] CRandorLF = { "\r\n", "\r", "\n" };
		#endregion
	}
}
