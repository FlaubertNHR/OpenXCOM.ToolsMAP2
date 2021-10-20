using System;
using System.IO;
using System.Reflection;


namespace DSShared
{
	/// <summary>
	/// Determines date/time of an assembly.
	/// </summary>
	/// <remarks>Lifted from StackOverflow.com:
	/// https://stackoverflow.com/questions/1600962/displaying-the-build-date#answer-1600990
	/// - what a fucking pain in the ass.</remarks>
	public static class DateTimeExtension
	{
		/// <summary>
		/// Gets the UTC-time that an assembly was assembled.
		/// </summary>
		/// <param name="assembly"></param>
		/// <returns><c>DateTime</c> since 1970</returns>
//		public static DateTime GetLinkerTime(this Assembly assembly, TimeZoneInfo target = null)
		public static DateTime GetLinkerTime(this Assembly assembly)
		{
			var filePath = assembly.Location;
			const int c_PeHeaderOffset = 60;
			const int c_LinkerTimestampOffset = 8;

			var buffer = new byte[2048];

			using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
				stream.Read(buffer, 0, 2048);

			var offset = BitConverter.ToInt32(buffer, c_PeHeaderOffset);
			var secondsSince1970 = BitConverter.ToInt32(buffer, offset + c_LinkerTimestampOffset);
			var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

			return epoch.AddSeconds(secondsSince1970);
//			var linkTimeUtc = epoch.AddSeconds(secondsSince1970);

//			var tz = target ?? TimeZoneInfo.Local;
//			var localTime = TimeZoneInfo.ConvertTimeFromUtc(linkTimeUtc, tz);

//			return localTime;
		}
	}
}
