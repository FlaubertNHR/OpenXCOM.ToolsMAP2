using System;

using XCom;


namespace PckView
{
	/// <summary>
	/// A static class that creates several of the <c>OpenFileDialog</c> and
	/// <c>SaveFileDialog</c> strings for titles and filters.
	/// </summary>
	internal static class FileDialogStrings
	{
		/// <summary>
		/// Gets a title for dialogs.
		/// </summary>
		/// <param name="setType">the <c><see cref="SpritesetType"></see></c>
		/// of the currently loaded <c><see cref="Spriteset"/></c></param>
		/// <param name="plural"><c>true</c> if multi-file open dialog</param>
		/// <returns></returns>
		internal static string GetTitle(SpritesetType setType, bool plural)
		{
			string title = String.Empty;
			switch (setType)
			{
				case SpritesetType.Pck:    title = "Select 32x40 8-bpp Image file"; break;
				case SpritesetType.Bigobs: title = "Select 32x48 8-bpp Image file"; break;
				case SpritesetType.ScanG:  title = "Select 4x4 8-bpp Image file";   break;
				case SpritesetType.LoFT:   title = "Select 16x16 8-bpp Image file"; break;
			}

			if (plural)
				title += "(s)";

			return title;
		}

		/// <summary>
		/// Gets a filter for dialogs.
		/// </summary>
		/// <returns></returns>
		internal static string GetFilter()
		{
			return "Image files (*.PNG *.GIF *.BMP)|*.PNG;*.GIF;*.BMP|"
				 + "PNG files (*.PNG)|*.PNG|GIF files (*.GIF)|*.GIF|BMP files (*.BMP)|*.BMP|"
				 + "All files (*.*)|*.*";
		}

		/// <summary>
		/// Gets a filter for Pckfiles.
		/// </summary>
		/// <returns></returns>
		internal static string GetFilterPck()
		{
			return "PCK files (*.PCK)|*.PCK|All files (*.*)|*.*";
		}

		/// <summary>
		/// Gets a filter for Datfiles.
		/// </summary>
		/// <returns></returns>
		internal static string GetFilterDat()
		{
			return "DAT files (*.DAT)|*.DAT|All files (*.*)|*.*";
		}

		/// <summary>
		/// Gets a filter for Pngfiles.
		/// </summary>
		/// <returns></returns>
		internal static string GetFilterPng()
		{
			return "PNG files (*.PNG)|*.PNG|All files (*.*)|*.*";
		}

		/// <summary>
		/// Gets an error hint.
		/// </summary>
		/// <param name="setType">the <c><see cref="SpritesetType"></see></c> of
		/// the currently loaded <c><see cref="Spriteset"/></c></param>
		/// <param name="spritesheet"><c>true</c> if the error occured when
		/// importing a spritesheet</param>
		/// <returns></returns>
		internal static string GetError(SpritesetType setType, bool spritesheet = false)
		{
			string error = "Image needs to be ";
			if (spritesheet) error += "divisible by ";

			switch (setType)
			{
				case SpritesetType.Pck:    return error + "32x40 8-bpp";
				case SpritesetType.Bigobs: return error + "32x48 8-bpp";
				case SpritesetType.ScanG:  return error + "4x4 8-bpp";
				case SpritesetType.LoFT:   return error + "16x16 8-bpp";
			}
			return String.Empty;
		}
	}
}
