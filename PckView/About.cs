using System;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

using DSShared;


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

//			var info = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
//			tb_Ver.Text = info.FileMajorPart + "."
//						+ info.FileMinorPart + "."
//						+ info.FileBuildPart + "."
//						+ info.FilePrivatePart;

			Assembly ass = Assembly.GetExecutingAssembly();

			var sb = new StringBuilder();

			Version ver = ass.GetName().Version;
			sb.AppendLine("PckView   .exe " + ver.Major + "."
											+ ver.Minor + "."
											+ ver.Build + "."
											+ ver.Revision);

			ver = Assembly.Load("XCom").GetName().Version;
			sb.AppendLine("XCom      .dll " + ver.Major + "."
											+ ver.Minor + "."
											+ ver.Build + "."
											+ ver.Revision);

			ver = Assembly.Load("DSShared").GetName().Version;
			sb.AppendLine("DSShared  .dll " + ver.Major + "."
											+ ver.Minor + "."
											+ ver.Build + "."
											+ ver.Revision);

			ver = Assembly.Load("YamlDotNet").GetName().Version;
			sb.Append("YamlDotNet.dll " + ver.Major + "."
										+ ver.Minor + "."
										+ ver.Build + "."
										+ ver.Revision);

			tb_Ver.Text = sb.ToString();

			tb_Ver.SelectionStart = tb_Ver.SelectionLength = 0;


			sb.Clear();
#if DEBUG
			sb.Append("debug ");
#else
			sb.Append("release ");
#endif
			DateTime dt = ass.GetLinkerTime();
			sb.Append(String.Format(
								CultureInfo.CurrentCulture,
								"{0:yyyy MMM d} {0:HH}:{0:mm}:{0:ss} UTC", // {0:zzz}
								dt));
			la_Date.Text = sb.ToString();


			// NOTE: this won't work for .NET 4+ (always returns 'None')
			// if compiling against .NET 4+ use GetPEKinds() see:
			// http://stackoverflow.com/questions/36945117/referenced-assemblies-returns-none-as-processorarchitecture

			la_Architecture.Text = DateTimeExtension.GetArchitecture();
		}

		/// <summary>
		/// Closes this <c>About</c> on a <c>KeyDown</c> event.
		/// </summary>
		/// <param name="e"></param>
		/// <remarks>Requires <c>KeyPreview</c> <c>true</c> since
		/// <c><see cref="tb_Ver"/></c> always takes focus.</remarks>
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
	}
}
