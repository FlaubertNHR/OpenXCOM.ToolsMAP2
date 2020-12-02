using System;
using System.Diagnostics;
using System.Reflection;
#if !__MonoCS__
using System.Runtime.InteropServices;
#endif
using System.Windows.Forms;


namespace PckView
{
	internal sealed partial class About
		:
			Form
	{
		/// <summary>
		/// cTor.
		/// </summary>
		internal About()
		{
			InitializeComponent();

			var info = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);

			lblVersion.Text = info.FileMajorPart + "."
							+ info.FileMinorPart + "."
							+ info.FileBuildPart + "."
							+ info.FilePrivatePart;

			// NOTE: this won't work for .NET 4+ (always returns 'None')
			// if compiling against .NET 4+ use GetPEKinds() see:
			// http://stackoverflow.com/questions/36945117/referenced-assemblies-returns-none-as-processorarchitecture

			string arch = String.Empty;
#if !__MonoCS__
			switch (AssemblyName.GetAssemblyName(info.FileName).ProcessorArchitecture)
			{
				case ProcessorArchitecture.None:	// An unknown or unspecified combination of processor and bits-per-word.
					arch = " : None";
//					arch += (IsOS64Bit()) ? " : 64-bit"
//										  : " : 32-bit";
					break;
				case ProcessorArchitecture.MSIL:	// Neutral with respect to processor and bits-per-word.
					arch = " : MSIL";
					arch += (IsOS64Bit()) ? " : 64-bit"
										  : " : 32-bit";
					break;
				case ProcessorArchitecture.X86:		// A 32-bit Intel processor, either native or in the
					arch = " : X86";				// Windows on Windows environment on a 64-bit platform (WOW64).
					if (IsOS64Bit())
						arch += " : WoW64";
					break;
				case ProcessorArchitecture.IA64:	// A 64-bit Intel processor only.
					arch = " : IA64";
					break;
				case ProcessorArchitecture.Amd64:	// A 64-bit AMD processor only.
					arch = " : Amd64";
					break;
			}
#endif

#if DEBUG
			const string config = "Debug";
#else
			const string config = "Release";
#endif
			lblBuildConfig.Text = config + arch;
		}

		/// <summary>
		/// Closes the form on a KeyDown event. Overrides core .NET
		/// implementation.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			switch (e.KeyData)
			{
				case Keys.Escape:
				case Keys.Enter:
					Close();
					break;
			}
		}

#if !__MonoCS__
		// http://stackoverflow.com/questions/336633/how-to-detect-windows-64-bit-platform-with-net#answer-1840313
		// dwhiteho -> end.
		// w/ refactoring and tweaks to satisfy FxCop and docs. kL

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

				return (ret != false && isWow64);
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
			var handle = NativeMethods.LoadLibrary("kernel32");
			if (handle != IntPtr.Zero)
			{
				var fnPtr = NativeMethods.GetProcAddress(handle, "IsWow64Process");
				if (fnPtr != IntPtr.Zero)
					return (IsWow64ProcessDelegate)Marshal.GetDelegateForFunctionPointer(
																					(IntPtr)fnPtr,
																					typeof(IsWow64ProcessDelegate));
			}
			return null;
		}
#endif
	}
}
