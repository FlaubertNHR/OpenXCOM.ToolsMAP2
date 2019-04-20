using System;

using XCom;


namespace XCom
{
	internal delegate void BufferChangedThing(Zerg zerg);


	public static class XConsole
	{
		internal static event BufferChangedThing BufferChanged;

		private static Zerg _zerg;
		private static int _zergs;

		public static void Init(int zergs)
		{
			if (_zerg == null)
			{
				_zerg = new Zerg();
				_zerg.Post = new Zerg();

				Zerg zerg = _zerg.Post;
				Zerg zergPre = _zerg;
				zerg.Pre = zergPre;
				for (int i = 2; i < zergs; ++i)
				{
					zerg.Post = new Zerg();
					zerg = zerg.Post;

					zergPre  =
					zerg.Pre = zergPre.Post;
				}

				zerg.Post = _zerg;
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
						var zergPost = new Zerg();
						zergPost.Post = zerg;

						zergPost.Pre = zergPre;
						zergPre.Post = zergPost;

						zergPre  =
						zerg.Pre = zergPost;
					}
				}
				else
				{
					for (int i = 0; i < _zergs - zergs; ++i)
					{
						_zerg.Pre = _zerg.Pre.Pre;
						_zerg.Pre.Post = _zerg;
					}
				}
			}

			_zergs = zergs;
		}

		public static void AdZerg(string zergBull)
		{
			_zerg = _zerg.Pre;
			_zerg.ZergBull = zergBull;

			if (BufferChanged != null)
				BufferChanged(_zerg);
		}

//		private xConsole()
//		{}

/*		public static void AddToLine(string st)
		{
			_zerg.ZergString += st;

			if (BufferChanged != null)
				BufferChanged(_zerg);
		} */

/*		public static void SetLine(string st)
		{
			_zerg.ZergString = st;

			if (BufferChanged != null)
				BufferChanged(_zerg);
		} */

//		public static xConsole that
//		{
//			get
//			{
//				if (console == null)
//					console = new xConsole(20);
//				return console;
//			}
//		}

//		public void KeyDown(object sender, KeyEventArgs e)
//		{
//			switch(e.KeyCode)
//			{
//				case Keys.Enter:
//					AddLine(command);
//					if (ExecuteCommand != null)
//						ExecuteCommand(command);
//					command = String.Empty;
//					break;
//
//				case Keys.ShiftKey:
//					shift = true;
//					break;
//
//				case Keys.Back:
//					if (command.Length > 0)
//						command = command.Substring(0, command.Length - 1);
//					break;
//
//				case Keys.Oemtilde:
//					if (ExecuteCommand != null)
//						ExecuteCommand("flipLast");
//					break;
//
//				default:
//					char ch = (((char)e.KeyValue) + String.Empty).ToLower()[0];
//
/*
//					if (myFont.KeyValid(ch))
//					{
//						if (shift)
//							command += myFont.ShiftValue(ch);
//						else
//							command += ch;
//					}
//					else
//					{
//						ch = e.KeyCode.ToString()[0];
//						System.Console.WriteLine(e.KeyCode.ToString());
//						if (myFont.KeyValid(ch))
//						{
//							if (shift)
//								command += myFont.ShiftValue(ch);
//							else
//								command += ch;
//						}
//					}
*/
//					break;
//			}
//		}

//		public void KeyUp(object sender, KeyEventArgs e)
//		{
//			switch(e.KeyCode)
//			{
//				case Keys.ShiftKey:
//					shift = false;
//					break;
//			}
//		}
	}


	/// <summary>
	/// class Zerg.
	/// </summary>
	internal class Zerg
	{
		public string ZergBull
		{ get; set; }

		public Zerg Post
		{ get; set; }

		public Zerg Pre
		{ get; set; }


		/// <summary>
		/// cTor
		/// </summary>
		public Zerg()
		{
			ZergBull = String.Empty;
		}
	}
}
