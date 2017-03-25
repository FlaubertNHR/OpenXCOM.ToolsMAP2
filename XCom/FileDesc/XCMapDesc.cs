using System;

using XCom.Interfaces.Base;


namespace XCom
{
	public class XCMapDesc
		:
		IMapDesc
	{
		public XCMapDesc(
				string baseName,
				string basePath,
				string blankPath,
				string rmpPath,
				string[] dependencies,
				Palette palette)
			:
				base(baseName)
		{
			Palette      = palette;
			BaseName     = baseName;
			BasePath     = basePath;
			RmpPath      = rmpPath;
			BlankPath    = blankPath;
			Dependencies = dependencies;
			IsStatic     = false;
		}

		public string[] Dependencies
		{ get; set; }

		public Palette Palette
		{ get; protected set; }

		public string BaseName
		{ get; protected set; }

		public string BasePath
		{ get; protected set; }

		public string RmpPath
		{ get; protected set; }

		public string BlankPath
		{ get; protected set; }

		public bool IsStatic
		{ get; set; }

		public string FilePath
		{
			get { return BasePath
					   + BaseName
					   + XCMapFile.MapExt; }
		}

		public int CompareTo(object other)
		{
			var descOther = other as XCMapDesc;
			return (descOther != null) ? String.CompareOrdinal(BaseName, descOther.BaseName)
									   : 1;
		}
	}
}
