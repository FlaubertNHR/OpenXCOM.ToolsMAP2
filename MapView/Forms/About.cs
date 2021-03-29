using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

using DSShared;

using MapView.Forms.MainView;


namespace MapView
{
	/// <summary>
	/// The About box.
	/// </summary>
	internal sealed partial class About
		:
			Form
	{
		#region Fields
		private Point _lBase;
		private Point _l;

		private Random _rand = new Random();
		#endregion Fields


		#region cTor
		/// <summary>
		/// cTor.
		/// </summary>
		internal About()
		{
			InitializeComponent();

			string text = "About";
			string before = String.Format(
										CultureInfo.CurrentCulture,
										"{0:n0}", GC.GetTotalMemory(false));
//			string after  = String.Format(
//										CultureInfo.CurrentCulture,
//										"{0:n0}", GC.GetTotalMemory(true));

//			text += " - " + before + " \u2192 " + after + " bytes"; // '\u2192' = right arrow.
			text += " - " + before + " bytes allocated";
			Text = text;

			var an = Assembly.GetExecutingAssembly().GetName();
			string ver = an.Version.Major + "."
					   + an.Version.Minor + "."
					   + an.Version.Build + "."
					   + an.Version.Revision;

			lblVersion.Text = "MapView " + ver;
#if DEBUG
			lblVersion.Text += " debug";
#else
			lblVersion.Text += " release";
#endif

			DateTime dt = Assembly.GetExecutingAssembly().GetLinkerTime();

			lblVersion.Text += Environment.NewLine + Environment.NewLine
							 + String.Format(
										CultureInfo.CurrentCulture,
										"{0:yyyy MMM d}  {0:HH}:{0:mm}:{0:ss} UTC", // {0:zzz}
										dt);
		}
		#endregion cTor


		#region Events (override)
		/// <summary>
		/// Sets the base-location on the shown event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnShown(EventArgs e)
		{
			_lBase = Location;
		}

		/// <summary>
		/// Adds this form to the zOrder on the activated event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnActivated(EventArgs e)
		{
			ShowHideManager._zOrder.Remove(this);
			ShowHideManager._zOrder.Add(this);
		}

		/// <summary>
		/// Handles keyup events.
		/// </summary>
		/// <param name="e"></param>
		/// <remarks>[Esc] closes the About box. [Enter] starts and stops
		/// brownian movement.</remarks>
		protected override void OnKeyUp(KeyEventArgs e)
		{
			switch (e.KeyData)
			{
				case Keys.Escape:
					Close();
					break;

				case Keys.Enter:
					if (t1.Enabled) t1.Enabled = false;
					else            t1.Enabled = true;
					break;
			}
		}

		/// <summary>
		/// Handles the formclosing event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			if (!RegistryInfo.FastClose(e.CloseReason))
			{
				MainViewF.that._fabout = null;
				MainViewF.that.DecheckAbout();
			}
			base.OnFormClosing(e);
		}
		#endregion Events (override)


		#region Events
		/// <summary>
		/// Does brownian movement on each tick if enabled.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnTick(object sender, EventArgs e)
		{
			_l = Location;
			_l.X += _rand.Next() % 3 - 1;
			_l.Y += _rand.Next() % 3 - 1;

			bool isInsideBounds = false;
			foreach (var screen in Screen.AllScreens)
			{
				if (isInsideBounds = screen.Bounds.Contains(_l)
								  && screen.Bounds.Contains(_l + Size))
				{
					break;
				}
			}

			if (!isInsideBounds)
				_l = _lBase;

			Location = _l;
		}
		#endregion Events
	}


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
		/// <returns>DateTime since 1970</returns>
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
