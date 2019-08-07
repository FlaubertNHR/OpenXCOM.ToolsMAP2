using System;
using System.Windows.Forms;


namespace DSShared.Controls
{
	/// <summary>
	/// A derived Panel that flags DoubleBuffered and ResizeRedraw.
	/// </summary>
	public class BufferedPanel
		:
			Panel
	{
		#region cTor
		public BufferedPanel()
		{
			DoubleBuffered = true;
			ResizeRedraw = true;
		}
		#endregion cTor
	}



	/// <summary>
	/// A derived Panel that flags WS_EX_COMPOSITED.
	/// </summary>
	public class CompositedPanel
		:
			Panel
	{
		#region Properties (override)
		/// <summary>
		/// This works great. Absolutely kills flicker on redraws.
		/// </summary>
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.ExStyle |= 0x02000000;
				return cp;
			}
		}
		#endregion Properties (override)
	}
}

/*
		/// <summary>
		/// Calls SetDoubleBuffered(object) on an array of objects.
		/// </summary>
		/// <param name="controls"></param>
		internal static void SetDoubleBuffered(object[] controls)
		{
			foreach (var control in controls)
				SetDoubleBuffered(control);
		}

		/// <summary>
		/// Some controls, such as the DataGridView, do not allow setting the
		/// DoubleBuffered property. It is set as a protected property. This
		/// method is a work-around to allow setting it. Call this in the
		/// constructor just after InitializeComponent().
		/// https://stackoverflow.com/questions/118528/horrible-redraw-performance-of-the-datagridview-on-one-of-my-two-screens#answer-16625788
		/// @note I wonder if this works on Mono. It stops the redraw-flick when
		/// setting the sprite-phase on return from SpritesetviewF on my system
		/// (Win7-64). Also stops flicker on the IsoLoft panel. etc.
		/// </summary>
		/// <param name="control">the Control on which to set DoubleBuffered to true</param>
		private static void SetDoubleBuffered(object control)
		{
			// if not remote desktop session then enable double-buffering optimization
			if (!SystemInformation.TerminalServerSession)
			{
				// set instance non-public property with name "DoubleBuffered" to true
				typeof(Control).InvokeMember("DoubleBuffered",
											 BindingFlags.SetProperty
										   | BindingFlags.Instance
										   | BindingFlags.NonPublic,
											 null,
											 control,
											 new object[] { true });
			}
		}
*/
