using System;


namespace XCom
{
	internal delegate void BufferChangedEvent(Zerg zerg);


	/// <summary>
	/// The console is not used. But it works. Good luck.
	/// </summary>
	public static class Console
	{
		internal static event BufferChangedEvent BufferChanged;

		private static Zerg _zerg;
		private static int _zergs;

		public static void Init(int zergs)
		{
			if (_zerg == null)
			{
				_zerg = new Zerg();
				_zerg.Pos = new Zerg();

				Zerg zerg = _zerg.Pos;
				Zerg zergPre = _zerg;
				zerg.Pre = zergPre;
				for (int i = 2; i < zergs; ++i)
				{
					zerg.Pos = new Zerg();
					zerg = zerg.Pos;

					zergPre  =
					zerg.Pre = zergPre.Pos;
				}

				zerg.Pos = _zerg;
				_zerg.Pre = zerg;
			}
			else
			{
				if (zergs > _zergs)
				{
					Zerg zerg = _zerg;
					Zerg zergPre = _zerg.Pre;

					for (int i = 0; i < zergs - _zergs; ++i)
					{
						var zergPos = new Zerg();
						zergPos.Pos = zerg;

						zergPos.Pre = zergPre;
						zergPre.Pos = zergPos;

						zergPre  =
						zerg.Pre = zergPos;
					}
				}
				else
				{
					for (int i = 0; i < _zergs - zergs; ++i)
					{
						_zerg.Pre = _zerg.Pre.Pre;
						_zerg.Pre.Pos = _zerg;
					}
				}
			}

			_zergs = zergs;
		}

		public static void zergrush(string bullshit)
		{
			_zerg = _zerg.Pre;
			_zerg.Bullshit = bullshit;

			if (BufferChanged != null)
				BufferChanged(_zerg);
		}
	}


	/// <summary>
	/// class Zerg.
	/// </summary>
	internal class Zerg
	{
		public string Bullshit
		{ get; set; }

		public Zerg Pos
		{ get; set; }

		public Zerg Pre
		{ get; set; }


		/// <summary>
		/// cTor.
		/// </summary>
		public Zerg()
		{
			Bullshit = String.Empty;
		}
	}
}
