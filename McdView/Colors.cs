using System.Drawing;


namespace McdView
{
	internal static class Colors
	{
		internal static readonly Pen PenText  = SystemPens.ControlText;
		internal static readonly Pen PenLight = SystemPens.ControlLight;

		internal static readonly Pen   PenControl   = SystemPens   .Control;
		internal static readonly Brush BrushControl = SystemBrushes.Control;

		internal static readonly Brush BrushHilight =
				new SolidBrush(Color.FromArgb(107, SystemColors.MenuHighlight));
		internal static readonly Brush BrushHilightsubsel =
				new SolidBrush(Color.FromArgb( 36, SystemColors.MenuHighlight));

		internal static readonly Brush BrushInvalid = Brushes.Firebrick;
	}
}
