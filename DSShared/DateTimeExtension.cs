using System;
using System.IO;
using System.Reflection;
//#if !__MonoCS__
using System.Diagnostics;
using System.Runtime.InteropServices;
//#endif


namespace DSShared
{
	/// <summary>
	/// Determines date/time of an assembly and/or the architecture on which the
	/// OS is.
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


		// http://stackoverflow.com/questions/336633/how-to-detect-windows-64-bit-platform-with-net#answer-1840313
		// dwhiteho -> end.
		// w/ refactoring and tweaks to satisfy FxCop and docs. kL

		public static string GetArchitecture()
		{
#if !__MonoCS__
			string arch;
			switch (Assembly.GetExecutingAssembly().GetName().ProcessorArchitecture)
			{
				default:
//				case ProcessorArchitecture.None:								// An unknown or unspecified combination of processor and bits-per-word.
					arch = "None : " + (IsOS64Bit() ? "64-bit" : "32-bit");
					break;
				case ProcessorArchitecture.MSIL:								// Neutral with respect to processor and bits-per-word.
					arch = "MSIL : " + (IsOS64Bit() ? "64-bit" : "32-bit");
					break;
				case ProcessorArchitecture.X86:									// A 32-bit Intel processor, either native or in the
					arch = "X86" + (IsOS64Bit() ? " : WoW64" : String.Empty);	// Windows on Windows environment on a 64-bit platform (WOW64).
					break;
				case ProcessorArchitecture.IA64:								// A 64-bit Intel processor only.
					arch = "IA64";
					break;
				case ProcessorArchitecture.Amd64:								// A 64-bit AMD processor only.
					arch = "Amd64";
					break;
			}
			return arch;
#else
			return "n/a";
#endif
		}

#if !__MonoCS__
		/// <summary>
		/// Checks if the user's operating system is 64-bit.
		/// </summary>
		/// <returns></returns>
		private static bool IsOS64Bit()
		{
			return IntPtr.Size == 8
			   || (IntPtr.Size == 4 && Is32BitProcessOn64BitProcessor());
		}

		/// <summary>
		/// Checks if this program is running as a 32-bit process in a 64-bit
		/// operating system.
		/// </summary>
		/// <returns></returns>
		private static bool Is32BitProcessOn64BitProcessor()
		{
			var fnDelegate = GetIsWow64ProcessDelegate();
			if (fnDelegate != null)
			{
				bool isWow64;
				bool ret = fnDelegate.Invoke(Process.GetCurrentProcess().Handle, out isWow64);

				return ret && isWow64;
			}
			return false;
		}


		private delegate bool IsWow64ProcessDelegate([In] IntPtr handle, [Out] out bool isWow64Process);

		private static class NativeMethods
		{
			[DllImport("kernel32",
				SetLastError = true,
				CallingConvention = CallingConvention.Winapi,
				BestFitMapping = false,
				ThrowOnUnmappableChar = true)]
			public extern static IntPtr LoadLibrary(string libraryName);

			[DllImport("kernel32",
				SetLastError = true,
				CallingConvention = CallingConvention.Winapi,
				BestFitMapping = false,
				ThrowOnUnmappableChar = true)]
			public extern static IntPtr GetProcAddress(IntPtr hwnd, string procedureName);
		}

		/// <summary>
		/// Helper for Is32BitProcessOn64BitProcessor().
		/// </summary>
		/// <returns></returns>
		private static IsWow64ProcessDelegate GetIsWow64ProcessDelegate()
		{
			IntPtr handle = NativeMethods.LoadLibrary("kernel32");
			if (handle != IntPtr.Zero)
			{
				IntPtr fnPtr = NativeMethods.GetProcAddress(handle, "IsWow64Process");
				if (fnPtr != IntPtr.Zero)
					return (IsWow64ProcessDelegate)Marshal.GetDelegateForFunctionPointer(
																					fnPtr,
																					typeof(IsWow64ProcessDelegate));
			}
			return null;
		}
#endif
	}
}
