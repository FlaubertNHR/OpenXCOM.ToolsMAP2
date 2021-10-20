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

			AssemblyName an = ass.GetName();
			string ver = an.Version.Major + "."
					   + an.Version.Minor + "."
					   + an.Version.Build + "."
					   + an.Version.Revision;
			sb.Append("PckView .exe " + ver);

			sb.AppendLine();
			an = Assembly.Load("XCom").GetName();
			ver = an.Version.Major + "."
				+ an.Version.Minor + "."
				+ an.Version.Build + "."
				+ an.Version.Revision;
			sb.Append("XCom    .dll " + ver);

			sb.AppendLine();
			an = Assembly.Load("DSShared").GetName();
			ver = an.Version.Major + "."
				+ an.Version.Minor + "."
				+ an.Version.Build + "."
				+ an.Version.Revision;
			sb.Append("DSShared.dll " + ver);

			tb_Ver.Text = sb.ToString();

			tb_Ver.SelectionStart = tb_Ver.SelectionLength = 0;

#if DEBUG
			string text = "debug ";
#else
			string text = "release ";
#endif
			DateTime dt = ass.GetLinkerTime();
			text += String.Format(
							CultureInfo.CurrentCulture,
							"{0:yyyy MMM d} {0:HH}:{0:mm}:{0:ss} UTC", // {0:zzz}
							dt);
			la_Date.Text = text;


			// NOTE: this won't work for .NET 4+ (always returns 'None')
			// if compiling against .NET 4+ use GetPEKinds() see:
			// http://stackoverflow.com/questions/36945117/referenced-assemblies-returns-none-as-processorarchitecture

			la_Architecture.Text = DateTimeExtension.GetArchitecture();
		}

		/// <summary>
		/// Closes this <c>About</c> on a <c>KeyDown</c> event.
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
	}
}
