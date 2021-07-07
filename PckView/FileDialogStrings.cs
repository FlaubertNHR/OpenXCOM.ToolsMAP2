using System;

using XCom;


namespace PckView
{
	/// <summary>
	/// A static class that creates several of the OpenFileDialog and
	/// SaveFileDialog strings for titles and filters.
	/// </summary>
	internal static class FileDialogStrings
	{
		/// <summary>
		/// Gets title for dialogs.
		/// </summary>
		/// <param name="t">the SsType of the currently loaded spriteset</param>
		/// <param name="plural">true if multi-file open dialog</param>
		/// <returns></returns>
		internal static string GetTitle(Spriteset.SsType t, bool plural)
		{
			string title = String.Empty;
			switch (t)
			{
				case Spriteset.SsType.Pck:    title = "Select 32x40 8-bpp Image file"; break;
				case Spriteset.SsType.Bigobs: title = "Select 32x48 8-bpp Image file"; break;
				case Spriteset.SsType.ScanG:  title = "Select 4x4 8-bpp Image file";   break;
				case Spriteset.SsType.LoFT:   title = "Select 16x16 8-bpp Image file"; break;
			}

			if (plural)
				title += "(s)";

			return title;
		}

		/// <summary>
		/// Gets filter for dialogs.
		/// </summary>
		/// <returns></returns>
		internal static string GetFilter()
		{
			return "Image files (*.PNG *.GIF *.BMP)|*.PNG;*.GIF;*.BMP|"
				 + "PNG files (*.PNG)|*.PNG|GIF files (*.GIF)|*.GIF|BMP files (*.BMP)|*.BMP|"
				 + "All files (*.*)|*.*";
		}

		/// <summary>
		/// Gets filter for PCK files.
		/// </summary>
		/// <returns></returns>
		internal static string GetFilterPck()
		{
			return "PCK files (*.PCK)|*.PCK|All files (*.*)|*.*";
		}

		/// <summary>
		/// Gets filter for DAT files.
		/// </summary>
		/// <returns></returns>
		internal static string GetFilterDat()
		{
			return "DAT files (*.DAT)|*.DAT|All files (*.*)|*.*";
		}

		/// <summary>
		/// Gets filter for PNG files.
		/// </summary>
		/// <returns></returns>
		internal static string GetFilterPng()
		{
			return "PNG files (*.PNG)|*.PNG|All files (*.*)|*.*";
		}

		/// <summary>
		/// Gets error hint.
		/// </summary>
		/// <param name="setType"></param>
		/// <returns></returns>
		internal static string GetError(Spriteset.SsType setType)
		{
			switch (setType)
			{
				case Spriteset.SsType.Pck:    return "Image needs to be 32x40 8-bpp";
				case Spriteset.SsType.Bigobs: return "Image needs to be 32x48 8-bpp";
				case Spriteset.SsType.ScanG:  return "Image needs to be 4x4 8-bpp";
				case Spriteset.SsType.LoFT:   return "Image needs to be 16x16 8-bpp";
			}
			return String.Empty;
		}
	}
}
