using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

using MapView.Forms.MainWindow;


namespace MapView
{
	/// <summary>
	/// Displays the About box.
	/// </summary>
	internal sealed partial class About
		:
			Form
	{
		#region Fields
		private Point _locBase;
		private Point _loc;
		private Size _size;
		private double _lastPoint;
		#endregion Fields


		#region cTor
		internal About()
		{
			InitializeComponent();

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
										System.Globalization.CultureInfo.CurrentCulture,
										"{0:yyyy MMM d}  {0:HH}:{0:mm}:{0:ss} {0:zzz}",
										dt);
		}
		#endregion cTor


		#region Events (override)
		protected override void OnActivated(EventArgs e)
		{
			ShowHideManager._zOrder.Remove(this);
			ShowHideManager._zOrder.Add(this);
		}

		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			XCMainWindow.that._fabout = null;
			XCMainWindow.that.DecheckAbout();

			base.OnFormClosing(e);
		}
		#endregion Events (override)


		#region Events
		private void OnShown(object sender, EventArgs e)
		{
			string text = " About";

			string before = String.Format(
										System.Globalization.CultureInfo.CurrentCulture,
										"{0:n0}", GC.GetTotalMemory(false));
//			string after  = String.Format(
//										System.Globalization.CultureInfo.CurrentCulture,
//										"{0:n0}", GC.GetTotalMemory(true));

//			text += " - " + before + " \u2192 " + after + " bytes"; // '\u2192' = right arrow.
			text += " - " + before + " bytes allocated";

			Text = text;

			_size = new Size(Width, Height);

			_locBase = Location;
			MoveWindow();
		}

		private void OnTick(object sender, EventArgs e)
		{
			MoveWindow();
		}

		private void OnLocationChanged(object sender, EventArgs e)
		{
			var locPre = new Size(GetLocationStep(_lastPoint));
			_loc += new Size(Location - locPre);
		}

		private void OnKeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Escape:
				case Keys.Enter:
					Close();
					break;
			}
		}
		#endregion Events


		#region Methods
		private void MoveWindow()
		{
			_loc = GetLocationStep(_lastPoint += 0.035);

			bool IsInsideBounds = false;
			foreach (var screen in Screen.AllScreens)
			{
				IsInsideBounds = screen.Bounds.Contains(_loc)
							  && screen.Bounds.Contains(_loc + _size);

				if (IsInsideBounds)
					break;
			}

			if (!IsInsideBounds)
				_loc = _locBase;

			Location = _loc;
		}

		private Point GetLocationStep(double delta)
		{
			var loc = Location;
			loc.X = (int)(_loc.X + (Math.Sin(delta) * 50));
			loc.Y = (int)(_loc.Y + (Math.Cos(delta) * 50));
			return loc;
		}
		#endregion Methods
	}


	/// <summary>
	/// Lifted from StackOverflow.com:
	/// https://stackoverflow.com/questions/1600962/displaying-the-build-date#answer-1600990
	/// - what a fucking pain in the ass.
	/// </summary>
	public static class DateTimeExtension
	{
		public static DateTime GetLinkerTime(this Assembly assembly, TimeZoneInfo target = null)
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

			var linkTimeUtc = epoch.AddSeconds(secondsSince1970);

			var tz = target ?? TimeZoneInfo.Local;
			var localTime = TimeZoneInfo.ConvertTimeFromUtc(linkTimeUtc, tz);

			return localTime;
		}
	}
}
