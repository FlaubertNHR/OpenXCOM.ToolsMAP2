using System;
using System.Collections.Generic;

using YamlDotNet.RepresentationModel; // deserialize


namespace RulesetConverter
{
	internal static class TerrainService
	{
		internal static void CreateTerrains(
				ICollection<Tileset> list,
				YamlMappingNode nodeRoot,
				string @group)
		{
			IDictionary<YamlNode, YamlNode> keyvals;
			YamlScalarNode keylabel;
			YamlSequenceNode terrainset, tilesets;
			string category;
			var terrains = new List<string>();

			var key = new YamlScalarNode("terrains");
			if (nodeRoot.Children.ContainsKey(key))
			{
				var battlesets = nodeRoot.Children[key] as YamlSequenceNode;
				foreach (YamlMappingNode battlefield in battlesets)
				{
					keyvals = battlefield.Children;
					if (keyvals != null && keyvals.Count != 0)
					{
						key = new YamlScalarNode("name");
						if (keyvals.ContainsKey(key))
						{
							category = keyvals[key].ToString();

							if (!String.IsNullOrEmpty(category))
							{
								terrains.Clear();

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
