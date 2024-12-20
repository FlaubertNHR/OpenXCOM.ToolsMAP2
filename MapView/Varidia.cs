﻿using System;
using System.IO;


namespace MapView
{
	/// <summary>
	/// <c>Varidia</c> is [still] used to read/write/parse/interpret
	/// user-settings/Options.
	/// </summary>
	internal static class Varidia
	{
		#region Methods (static)
		/// <summary>
		/// Read a line from <c><see cref="DSShared.PathInfo"/>.CFG_Options</c>.
		/// </summary>
		/// <param name="tr"></param>
		/// <returns><c>KeyvalPair</c> else <c>null</c></returns>
		internal static KeyvalPair getKeyvalPair(TextReader tr)
		{
			string line = null;
			do // get a good line - not a comment (#) or blank-string
			{
				if (tr.Peek() == -1) // zilch, exit.
					return null;

				line = tr.ReadLine().Trim();
			}
			while (line.Length == 0 || line[0] == '#');

			if (line != null)
			{
				int pos = line.IndexOf(':');
				if (pos == -1) // is an option header
				{
					return new KeyvalPair(line, String.Empty);
				}

				if (pos != 0) // is a keyval pair
				{
					return new KeyvalPair(line.Substring(0, pos), line.Substring(pos + 1));
				}
			}
			return null;
		}
		#endregion Methods (static)
	}


	/// <summary>
	/// <c>KeyvalPair</c> - helper class for <c><see cref="Varidia"/></c>.
	/// </summary>
	public class KeyvalPair
	{
		#region Properties
		private readonly string _key;
		internal string Key
		{
			get { return _key; }
		}

		private readonly string _val;
		internal string Value
		{
			get { return _val; }
		}
		#endregion


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="val"></param>
		internal KeyvalPair(string key, string val)
		{
			_key = key;
			_val = val;
		}
		#endregion


		#region Methods (override)
		/// <summary>
		/// Overrides <c>Object.ToString()</c>.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return _key + ":" + _val;
		}
		#endregion
	}
}
