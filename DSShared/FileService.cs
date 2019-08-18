using System;
using System.IO;


namespace DSShared
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
									head, // + " The operation will not proceed.",
									copyable))
			{
				f.ShowDialog();
			}
		}


		/// <summary>
		/// Reads all the bytes of a specified file and returns it in a buffer.
		/// The file will be closed.
		/// </summary>
		/// <param name="pfe">path-file-extension of the file to read</param>
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
								"File could not be read.",
								pfe + Environment.NewLine + Environment.NewLine + ex);
					return null;
				}
			}
			else
				ShowDialogError("File does not exist.", pfe);

			return bytes;
		}

		/// <summary>
		/// Opens a file for reading and returns it as a filestream. The file
		/// will not be closed.
		/// IMPORTANT: Dispose the stream in the calling function.
		/// </summary>
		/// <param name="pfe">path-file-extension of the file to be opened</param>
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
								"File could not be opened.",
								pfe + Environment.NewLine + Environment.NewLine + ex);
					return null;
				}
			}
			else
				ShowDialogError("File does not exist.", pfe);

			return fs;
		}

		/// <summary>
		/// Creates a file and returns a FileStream for writing to it after
		/// backing up a pre-existing file if it exists. The file will not be
		/// closed.
		/// IMPORTANT: Dispose the stream in the calling function.
		/// @note If file exists call this only to create a file_ext_[t.ext]
		/// file. Then call ReplaceFile() by passing in file_ext.
		/// </summary>
		/// <param name="pfe">path-file-extension of the file to be created</param>
		/// <returns>the filestream if valid else null</returns>
		public static FileStream CreateFile(string pfe)
		{
			FileStream fs = null;

			try
			{
				Directory.CreateDirectory(Path.GetDirectoryName(pfe));
				fs = File.Create(pfe);
			}
			catch (Exception ex)
			{
				ShowDialogError(
							"File could not be created.",
							pfe + Environment.NewLine + Environment.NewLine + ex);
				return null;
			}
			return fs;
		}

		/// <summary>
		/// Replaces an original file with a temporary file after making a
		/// backup of the original. If the original does not exist then the
		/// temporary file will be renamed without a backup.
		/// IMPORTANT: The replacement file must have the name and extension of
		/// the file to be replaced plus the 'GlobalsXC.TEMPExt' extension. In
		/// other words, the standard save-procedure is to write to its
		/// file_ext_[t.ext] then call ReplaceFile() by passing in the original
		/// file_ext.
		/// @note The backup will be in the 'GlobalsXC.MV_Backup' subdirectory.
		/// </summary>
		/// <param name="pfe">path-file-extension of the file to be replaced</param>
		/// <returns>true if everything goes according to plan</returns>
		public static bool ReplaceFile(string pfe)
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
									"File backup could not be deleted.",
									pfeBackup + Environment.NewLine + Environment.NewLine + ex);
						return false;
					}
				}

				try
				{
					Directory.CreateDirectory(dirBackup);
					File.Replace(
							pfe + GlobalsXC.TEMPExt,
							pfe,
							pfeBackup,
							true);
				}
				catch (Exception ex)
				{
					ShowDialogError(
								"File could not be replaced.",
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
								"File could not be moved.",
								pfe + GlobalsXC.TEMPExt + Environment.NewLine + Environment.NewLine + ex);
					return false;
				}
			}
			return true;
		}


/*		/// <summary>
		/// Deletes a file.
		/// </summary>
		/// <param name="pfe">path-file-extension of the file to be deleted</param>
		/// <returns>true if mission was successful or file doesn't exist thus
		/// doesn't need to be deleted</returns>
		public static bool DeleteFile(string pfe)
		{
			if (File.Exists(pfe))
			{
				try
				{
					File.Delete(pfe);
				}
				catch (Exception ex)
				{
					ShowDialogError(
								"File could not be deleted.",
								pfe + Environment.NewLine + Environment.NewLine + ex);
					return false;
				}
			}
			return true;
		} */

/*		/// <summary>
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
		} */
		#endregion Methods (static)
	}
}
