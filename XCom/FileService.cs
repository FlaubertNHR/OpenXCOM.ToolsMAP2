using System;
using System.IO;


namespace XCom
{
	/// <summary>
	/// A class that handles file reading/writing with generic exception
	/// handling.
	/// </summary>
	public static class FileService
	{
		#region Methods (static)
		/// <summary>
		/// Shows an error in an Infobox modally.
		/// </summary>
		/// <param name="head"></param>
		/// <param name="copyable"></param>
		private static void ShowDialogError(string head, string copyable)
		{
			using (var f = new Infobox(
									"IO Error",
									head + " The operation will not proceed.",
									copyable))
			{
				f.ShowDialog();
			}
		}


		/// <summary>
		/// Creates a file and returns a FileStream for writing to it.
		/// IMPORTANT: Dispose the stream in the calling function.
		/// </summary>
		/// <param name="pfe">path-file-extension</param>
		/// <returns>the filestream if valid else null</returns>
		public static FileStream CreateFile(string pfe)
		{
			FileStream fs = null;

			if (BackupFile(pfe))
			{
				try
				{
					Directory.CreateDirectory(Path.GetDirectoryName(pfe));
					fs = File.Create(pfe);
				}
				catch (Exception ex)
				{
					ShowDialogError(
								"The file could not be created.",
								pfe + Environment.NewLine + Environment.NewLine + ex);
					return null;
				}
			}
			return fs;
		}

		/// <summary>
		/// Copies an existing file to a MapView backup directory.
		/// @note The backup will be a copy in the 'GlobalsXC.MV_Backup'
		/// subdirectory.
		/// </summary>
		/// <param name="pfe">path-file-extension</param>
		/// <returns>true if mission was successful or src doesn't exist thus
		/// doesn't need to be backed up</returns>
		private static bool BackupFile(string pfe)
		{
			if (File.Exists(pfe))
			{
				string dirBackup = Path.Combine(Path.GetDirectoryName(pfe), GlobalsXC.MV_Backup);
				string pfeBackup = Path.Combine(dirBackup, Path.GetFileName(pfe));

				if (File.Exists(pfeBackup))
				{
					try
					{
						File.Delete(pfeBackup);
					}
					catch (Exception ex)
					{
						ShowDialogError(
									"The backup file could not be deleted.",
									pfeBackup + Environment.NewLine + Environment.NewLine + ex);
						return false;
					}
				}

				try
				{
					Directory.CreateDirectory(dirBackup);
					File.Copy(pfe, pfeBackup);
				}
				catch (Exception ex)
				{
					ShowDialogError(
								"The file could not be backed up.",
								pfe + Environment.NewLine + Environment.NewLine + ex);
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Opens a file.
		/// IMPORTANT: Dispose the stream in the calling function.
		/// </summary>
		/// <param name="pfe">path-file-extension</param>
		/// <returns>the filestream if valid else null</returns>
		public static FileStream OpenFile(string pfe)
		{
			FileStream fs = null;

			if (File.Exists(pfe))
			{
				try
				{
					fs = File.OpenRead(pfe);
				}
				catch (Exception ex)
				{
					ShowDialogError(
								"The file could not be opened.",
								pfe + Environment.NewLine + Environment.NewLine + ex);
					return null;
				}
			}
			else
				ShowDialogError("The file does not exist.", pfe);

			return fs;
		}

		/// <summary>
		/// Replaces an original file with a temporary file after making a
		/// backup of the original. If the original does not exist then the
		/// temporary file will be renamed without a backup.
		/// IMPORTANT: The replacement file must have the name and extension of
		/// the file to be replaced plus the 'GlobalsXC.TEMPExt' extension.
		/// @note The backup will be in the 'GlobalsXC.MV_Backup' subdirectory.
		/// </summary>
		/// <param name="pfe">path-file_extension of the file to be replaced</param>
		/// <returns>true if everything goes according to plan</returns>
		public static bool ReplaceFile(string pfe)
		{
			string dirBackup = Path.Combine(Path.GetDirectoryName(pfe), GlobalsXC.MV_Backup);
			string pfeBackup = Path.Combine(dirBackup, Path.GetFileName(pfe));

			if (File.Exists(pfe))
			{
				if (File.Exists(pfeBackup))
				{
					try
					{
						File.Delete(pfeBackup);
					}
					catch (Exception ex)
					{
						ShowDialogError(
									"The backup file could not be deleted.",
									pfeBackup + Environment.NewLine + Environment.NewLine + ex);
						return false;
					}
				}

				try
				{
					File.Replace(
							pfe + GlobalsXC.TEMPExt,
							pfe,
							pfeBackup,
							true);
				}
				catch (Exception ex)
				{
					ShowDialogError(
								"The file could not be replaced.",
								pfe + Environment.NewLine + Environment.NewLine + ex);
					return false;
				}
			}
			else
			{
				try
				{
					File.Move(pfe + GlobalsXC.TEMPExt, pfe);
				}
				catch (Exception ex)
				{
					ShowDialogError(
								"The file could not be moved.",
								pfe + GlobalsXC.TEMPExt + Environment.NewLine + Environment.NewLine + ex);
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Reads all the bytes of a specified file.
		/// </summary>
		/// <param name="pfe">path-file-extension</param>
		/// <returns>an array of bytes else null</returns>
		public static byte[] ReadFile(string pfe)
		{
			byte[] bytes = null;

			if (File.Exists(pfe))
			{
				try
				{
					bytes = File.ReadAllBytes(pfe);
				}
				catch (Exception ex)
				{
					ShowDialogError(
								"The file could not be read.",
								pfe + Environment.NewLine + Environment.NewLine + ex);
					return null;
				}
			}
			else
				ShowDialogError("The file does not exist.", pfe);

			return bytes;
		}
		#endregion Methods (static)
	}
}
