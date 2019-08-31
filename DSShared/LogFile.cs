using System;

#if DEBUG
using System.IO;
#endif


namespace DSShared
{
	public static class LogFile
	{
#if DEBUG
		private const string FILE = "debug.log";
		private static string _full;
#endif

		public static void SetLogFilePath(string path, bool append = false)
		{
#if DEBUG
			_full = Path.Combine(path, FILE);
			if (!append) CreateLog();
#endif
		}

		/// <summary>
		/// Creates a logfile or cleans the old one. The logfile should be
		/// created by calling SetLogFilePath() only.
		/// </summary>
		private static void CreateLog()
		{
#if DEBUG
			using (var sw = new StreamWriter(File.Open(
													_full,
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
		public static void WriteLine(string line)
		{
#if DEBUG
			using (var sw = new StreamWriter(File.Open(
													_full,
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
