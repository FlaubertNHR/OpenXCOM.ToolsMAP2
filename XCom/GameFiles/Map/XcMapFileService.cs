using System.Collections.Generic;
using System.IO;

using XCom.GameFiles.Map;
using XCom.Interfaces.Base;


namespace XCom
{
	public class XCMapFileService
	{
		private readonly XCTileFactory _tileFactory;


		public XCMapFileService(XCTileFactory tileFactory)
		{
			_tileFactory = tileFactory;
		}


		public IMap_Base Load(XCMapDesc desc)
		{
			if (desc != null && File.Exists(desc.FilePath))
			{
				var tiles = new List<TileBase>();

				var images = GameInfo.ImageInfo;
				foreach (string dep in desc.Dependencies)
				{
					var image = images[dep];
					if (image != null)
					{
						var MCD = image.GetMcdFile(desc.Palette, _tileFactory);
						foreach (XCTile tile in MCD)
							tiles.Add(tile);
					}
				}

				var RMP = new RouteFile(desc.BaseName, desc.RmpPath);
				var MAP = new XCMapFile(
										desc.BaseName,
										desc.BasePath,
										desc.BlankPath,
										tiles,
										desc.Dependencies,
										RMP);
				return MAP;
			}
			return null;
		}
	}
}
