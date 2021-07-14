using System;
using System.Collections.Generic;
using System.IO;

using YamlDotNet.RepresentationModel; // deserialize


namespace RulesetConverter
{
	internal static class CraftService
	{
		internal static void CreateTerrains(
				ICollection<Tileset> list,
				YamlMappingNode nodeRoot,
				string @group,
				string category,
				string target)
		{
			IDictionary<YamlNode, YamlNode> keyvals;
			YamlScalarNode keylabel;
			YamlSequenceNode terrainset, tilesets;
			string craftid;
			var terrains = new List<string>();

			var key = new YamlScalarNode(target); // "crafts"
			if (nodeRoot.Children.ContainsKey(key))
			{
				var crafts = nodeRoot.Children[key] as YamlSequenceNode;
				foreach (YamlMappingNode craft in crafts)
				{
					keyvals = craft.Children;
					if (keyvals != null && keyvals.Count != 0)
					{
						key = new YamlScalarNode("type");
						if (keyvals.ContainsKey(key))
						{
							craftid = keyvals[key].ToString();

							if (!String.IsNullOrEmpty(craftid))
							{
								terrains.Clear();

								key = new YamlScalarNode("battlescapeTerrainData");
								if (keyvals.ContainsKey(key))
								{
									keyvals = (keyvals[key] as YamlMappingNode).Children;
									if (keyvals != null && keyvals.Count != 0)
									{
										key = new YamlScalarNode("mapDataSets");
										if (keyvals.ContainsKey(key))
										{
											terrainset = keyvals[key] as YamlSequenceNode;
											foreach (var terrain in terrainset)
											{
												if (terrain.ToString().ToUpperInvariant() != "BLANKS")
													terrains.Add(terrain.ToString());
											}
										}

										if (terrains.Count != 0)
										{
											key = new YamlScalarNode("mapBlocks");
											if (keyvals.ContainsKey(key))
											{
												keylabel = new YamlScalarNode("name");
		
												tilesets = keyvals[key] as YamlSequenceNode;
												foreach (var tileset in tilesets)
												{
													list.Add(new Tileset(
																	tileset[keylabel].ToString(),
																	@group,
																	category,
																	new List<string>(terrains))); // copy that, Roger.
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}
	}
}
