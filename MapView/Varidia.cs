using System;
using System.IO;


namespace MapView
{
	/// <summary>
	/// Varidia is [still] used to read/write/parse/interpret
	/// user-settings/Options.
	/// </summary>
	internal static class Varidia
	{
		#region Methods (static)
		/// <summary>
		/// Read a line from 'MVOptions' cfg.
		/// </summary>
		/// <param name="sr"></param>
		/// <returns>KeyvalPair else null</returns>
		internal static KeyvalPair getKeyvalPair(TextReader sr)
		{
			string line = String.Empty;
			do // get a good line - not a comment (#) or blank-string
			{
				if (sr.Peek() == -1) // zilch, exit.
					return null;

				line = sr.ReadLine().Trim();
			}
			while (line.Length == 0 || line[0] == '#');

			if (line != null)
			{
				int pos = line.IndexOf(':');
				return (pos > 0) ? new KeyvalPair(line.Substring(0, pos), line.Substring(pos + 1))
								 : new KeyvalPair(line, String.Empty);
/*				if (pos > 0)
				{
					string key = line.Substring(0, pos);

					if (pos + 1 < line.Length)
						return new KeyvalPair(key, line.Substring(pos + 1));

					return new KeyvalPair(key, String.Empty);
				} */
			}
			return null;
		}
		#endregion Methods (static)
	}


	/// <summary>
	/// KeyvalPair - helper class for Varidia.
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
		public override string ToString()
		{
			return _key + ":" + _val;
		}
		#endregion
	}
}
