using System;
using System.Drawing;
using System.Windows.Forms;

using XCom;
using XCom.Interfaces.Base;


namespace MapView
{
	public class MapViewPanel
		:
		Panel
	{
		private MapView _mapView;

		private readonly HScrollBar _horiz;
		private readonly VScrollBar _vert;

		private static MapViewPanel _instance;


		private MapViewPanel()
		{
			ImageUpdate += update; // FIX: "Subscription to static events without unsubscription may cause memory leaks."

			_horiz = new HScrollBar();
			_vert  = new VScrollBar();

			_horiz.Scroll += horiz_Scroll;
			_horiz.Dock = DockStyle.Bottom;

			_vert.Scroll += vert_Scroll;
			_vert.Dock = DockStyle.Right;

			Controls.AddRange(new Control[]{ _vert, _horiz });

			SetView(new MapView());
		}


		public void SetView(MapView view)
		{
			if (_mapView != null)
			{
				view.Map = _mapView.Map;
				Controls.Remove(_mapView);
			}

			_mapView = view;

			_mapView.Location = new Point(0, 0);
			_mapView.BorderStyle = BorderStyle.Fixed3D;

			_vert.Minimum = 0;
			_vert.Value = _vert.Minimum;

			_mapView.Width = ClientSize.Width - _vert.Width - 1;

			Controls.Add(_mapView);
		}

		public void Cut_click(object sender, EventArgs e)
		{
			_mapView.Copy();
			_mapView.ClearSelection();
		}

		public void Copy_click(object sender, EventArgs e)
		{
			_mapView.Copy();
		}

		public void Paste_click(object sender, EventArgs e)
		{
			_mapView.Paste();
		}

		public static MapViewPanel Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new MapViewPanel();
					LogFile.WriteLine("MapView panel created");
				}
				return _instance;
			}
		}

		public IMap_Base BaseMap
		{
			get { return _mapView.Map; }
		}

		private void update(object sender, EventArgs e)
		{
			_mapView.Refresh();
		}

		public void OnResize()
		{
			OnResize(EventArgs.Empty);
		}

		protected override void OnResize(EventArgs eventargs)
		{
			base.OnResize(eventargs);

			if (Globals.AutoPckImageScale)
				SetupMapSize();

			_vert.Value  = _vert.Minimum;
			_horiz.Value = _horiz.Minimum;

			vert_Scroll(null, null);
			horiz_Scroll(null, null);

			int h = 0;
			int w = 0;

			_vert.Visible = (_mapView.Height > ClientSize.Height);
			if (_vert.Visible)
			{
				_vert.Maximum = _mapView.Height - ClientSize.Height + _horiz.Height;
				w = _vert.Width;
			}
			else
				_horiz.Width = ClientSize.Width;

			_horiz.Visible = (_mapView.Width > ClientSize.Width);
			if (_horiz.Visible)
			{
				_horiz.Maximum = Math.Max(
									_mapView.Width - ClientSize.Width + _vert.Width,
									_horiz.Minimum);
				h = _horiz.Height;
			}
			else
				_vert.Height = ClientSize.Height;

			_mapView.Viewable = new Size(Width - w, Height - h);
			_mapView.Refresh();
		}

		private void vert_Scroll(object sender, ScrollEventArgs e)
		{
			_mapView.Location = new Point(
										_mapView.Left,
										-(_vert.Value) + 1);
			_mapView.Refresh();
		}

		private void horiz_Scroll(object sender, ScrollEventArgs e)
		{
			_mapView.Location = new Point(
										-(_horiz.Value),
										_mapView.Top);
			_mapView.Refresh();
		}

		public void SetMap(IMap_Base baseMap)
		{
			_mapView.Map = baseMap;
			_mapView.Focus();

			OnResize(null);
		}

		public void SetupMapSize()
		{
			if (Globals.AutoPckImageScale)
			{
				var size = _mapView.GetMapSize(1);

				var wP = Width  / (double)size.Width;
				var hP = Height / (double)size.Height;

				Globals.PckImageScale = (wP > hP) ? hP : wP;

				if (Globals.PckImageScale > Globals.MaxPckImageScale)
					Globals.PckImageScale = Globals.MaxPckImageScale;

				if (Globals.PckImageScale < Globals.MinPckImageScale)
					Globals.PckImageScale = Globals.MinPckImageScale;
			}

			_mapView.SetupMapSize();
		}

		public void ForceResize()
		{
			OnResize(null);
		}

		public MapView MapView
		{
			get { return _mapView; }
		}


		/*** Timer stuff ***/
		public static event EventHandler ImageUpdate;

		private static Timer _timer;
		private static bool _started;
		private static int _current;

		public static void Start()
		{
			if (_timer == null)
			{
				_timer = new Timer();
				_timer.Interval = 100;
				_timer.Tick += tick;
				_timer.Start();
				_started = true;
			}

			if (!_started)
			{
				_timer.Start();
				_started = true;
			}
		}

		public static void Stop()
		{
			if (_timer == null)
			{
				_timer = new Timer();
				_timer.Interval = 100;
				_timer.Tick += tick;
				_started = false;
			}

			if (_started)
			{
				_timer.Stop();
				_started = false;
			}
		}

		public static bool Updating
		{
			get { return _started; }
		}

		public static int Interval
		{
			get { return _timer.Interval; }
			set { _timer.Interval = value; }
		}

		private static void tick(object sender, EventArgs e)
		{
			_current = (_current + 1) % 8;

			if (ImageUpdate != null)
				ImageUpdate(null, null);
		}

		public static int Current
		{
			get { return _current; }
			set { _current = value; }
		}
	}
}
