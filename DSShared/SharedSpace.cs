using System;
using System.Collections.Generic;


namespace DSShared
{
	public static class SharedSpace
	{
		#region Fields (static)
		public const string MapOptionsFile = "MapOptionsFile";		// -> MapOptions.cfg
		public const string PckConfigFile  = "PckConfigFile";		// -> PckConfig.yml

		public const string MapResourcesFile = "MapResourcesFile";	// -> MapResources.yml
		public const string MapTilesetsFile  = "MapTilesetsFile";	// -> MapTilesets.yml
		public const string MapViewersFile   = "MapViewersFile";	// -> MapViewers.yml

		public const string ApplicationDirectory  = "ApplicationDirectory";
		public const string SettingsDirectory     = "SettingsDirectory";
		public const string ResourceDirectoryUfo  = "ResourceDirectoryUfo";
		public const string ResourceDirectoryTftd = "ResourceDirectoryTftd";


		private static readonly Dictionary<string, object> _shares =
							new Dictionary<string, object>();
		#endregion Fields (static)


		// TODO: Since SharedSpace holds only string-values factor away the
		// boxing and just use strings. Actually, Palettes is a dictionary ...
		// but it should be changed into a variable that's local to PckView
		// anyway.
		//
		// NOTE: which means that SharedSpace and PathInfo have very similar
		// usages and ought be merged.
		//
		// NOTE: PathInfo objects are returned as objects also.


		#region Methods (static)
		/// <summary>
		/// Allocates a key-val pair in the <c>SharedSpace</c>. This does not
		/// replace the value of an existing key unless its current value is
		/// <c>null</c>.
		/// TODO: But that's just silly.
		/// </summary>
		/// <param name="key">the key to look for</param>
		/// <param name="val">the <c>object</c> to add if the key doesn't exist
		/// or its value is <c>null</c></param>
		/// <remarks>Ensure <paramref name="key"/> is not <c>null</c> or blank.</remarks>
		public static void SetShare(string key, object val)
		{
			if (!_shares.ContainsKey(key))
			{
				_shares.Add(key, val);
			}
			else if (_shares[key] == null)
			{
				_shares[key] = val;
			}
		}

		/// <summary>
		/// Gets the value of a specified key as an <c>object</c>.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		/// <remarks>Ensure <paramref name="key"/> is not <c>null</c> or blank.</remarks>
		public static object GetShareObject(string key)
		{
			object val;
			if (_shares.TryGetValue(key, out val))
				return val;

			return null;
		}

		/// <summary>
		/// Gets the value of a specified key as a <c>string</c>.
		/// </summary>
		/// <param name="key"></param>
		/// <returns>the value associated with the key; <c>null</c> if key
		/// doesn't exist</returns>
		/// <remarks>Ensure <paramref name="key"/> is not <c>null</c> or blank.</remarks>
		public static string GetShareString(string key)
		{
			object val;
			if (_shares.TryGetValue(key, out val))
				return val as String;

			return null;
		}
		#endregion Methods (static)
	}
}
