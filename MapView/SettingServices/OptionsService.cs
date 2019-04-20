using System;
using System.Collections.Generic;
using System.IO;

using DSShared;

using XCom;


namespace MapView.OptionsServices
{
	internal static class OptionsService
	{
		internal static void SaveOptions(IDictionary<string, Options> options)
		{
			using (var sw = new StreamWriter(((PathInfo)SharedSpace.that[PathInfo.ShareOptions]).Fullpath))
			{
				foreach (string key in options.Keys)
				{
					if (options.ContainsKey(key))
						options[key].SaveOptions(key, sw);
				}
			}
		}
	}
}
