using System.Drawing;


namespace McdView
{
	internal static class Colors
	{
		internal readonly static Pen PenText  = SystemPens.ControlText;
		internal readonly static Pen PenLight = SystemPens.ControlLight;

		internal readonly static Pen   PenControl   = SystemPens   .Control;
		internal readonly static Brush BrushControl = SystemBrushes.Control;

		internal readonly static Brush BrushHilight =
				new SolidBrush(Color.FromArgb(107, SystemColors.MenuHighlight));
		internal readonly static Brush BrushHilightsubsel =
				new SolidBrush(Color.FromArgb( 36, SystemColors.MenuHighlight));

		internal readonly static Brush BrushInvalid = Brushes.Firebrick;
	}
}
