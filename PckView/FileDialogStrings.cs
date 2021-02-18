using System;


namespace PckView
{
	internal static class FileDialogStrings
	{
		/// <summary>
		/// Gets an open-file title for dialogs.
		/// </summary>
		/// <param name="t">the SetType of the currently loaded spriteset</param>
		/// <param name="plural">true if multi-file open dialog</param>
		/// <returns></returns>
		internal static string GetTitle(PckViewF.Type t, bool plural)
		{
			string title;
			switch (t)
			{
				default:                   title = "Select 32x40 8-bpp Image file"; break; // PckViewF.Type.Pck
				case PckViewF.Type.Bigobs: title = "Select 32x48 8-bpp Image file"; break;
				case PckViewF.Type.ScanG:  title = "Select 4x4 8-bpp Image file";   break;
				case PckViewF.Type.LoFT:   title = "Select 16x16 8-bpp Image file"; break;
			}

			if (plural)
				title += "(s)";

			return title;
		}

		/// <summary>
		/// Gets the open-file filter for dialogs.
		/// </summary>
		/// <returns></returns>
		internal static string GetFilter()
		{
			return "Image files (*.PNG *.GIF *.BMP)|*.PNG;*.GIF;*.BMP|"
				 + "PNG files (*.PNG)|*.PNG|GIF files (*.GIF)|*.GIF|BMP files (*.BMP)|*.BMP|"
				 + "All files (*.*)|*.*";
		}

		/// <summary>
		/// Gets the open-file filter for PCK files.
		/// </summary>
		/// <returns></returns>
		internal static string GetFilterPck()
		{
			return "PCK files (*.PCK)|*.PCK|All files (*.*)|*.*";
		}

		/// <summary>
		/// Gets the open-file filter for PCK files.
		/// </summary>
		/// <returns></returns>
		internal static string GetFilterDat()
		{
			return "DAT files (*.DAT)|*.DAT|All files (*.*)|*.*";
		}

		/// <summary>
		/// Gets the open-file filter for PNG files.
		/// </summary>
		/// <returns></returns>
		internal static string GetFilterPng()
		{
			return "PNG files (*.PNG)|*.PNG|All files (*.*)|*.*";
		}
	}
}
