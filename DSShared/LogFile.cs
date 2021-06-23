using System;

#if DEBUG
using System.IO;
#endif


namespace DSShared
{
	public static class Logfile
	{
#if DEBUG
		private const string FILE = "debug.log";
		private static string _pfe;
#endif

		public static void SetPath(string path, bool append = false)
		{
#if DEBUG
			_pfe = Path.Combine(path, FILE);
			if (!append) Create();
#endif
		}

		/// <summary>
		/// Creates a logfile or cleans the old one. The logfile should be
		/// created by calling <c><see cref="SetPath()">SetPath()</see></c>
		/// only.
		/// </summary>
		private static void Create()
		{
#if DEBUG
			using (var sw = new StreamWriter(File.Open(
													_pfe,
													FileMode.Create,
													FileAccess.Write,
													FileShare.None)))
			{}
#endif
		}

		/// <summary>
		/// Writes a line to the logfile.
		/// </summary>
		/// <param name="line">the line to write</param>
		public static void Log(string line)
		{
#if DEBUG
			using (var sw = new StreamWriter(File.Open(
													_pfe,
													FileMode.Append,
													FileAccess.Write,
													FileShare.None)))
			{
				sw.WriteLine(line);
			}
#endif
		}
	}
}
