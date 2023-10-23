using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using static BrowserPivot.win32api;


namespace BrowserPivot
{
    class file_managent
    {
        public static bool isCancelled = false;

        private static void copy_file(string sourceFilePath, string destinationFilePath)
        {
            try
            {
                bool success = CopyFileEx(sourceFilePath, destinationFilePath, IntPtr.Zero, IntPtr.Zero, ref isCancelled, COPY_FILE_FLAGS.COPY_FILE_RESTARTABLE);
                if (!success)
                {
                    int error = Marshal.GetLastWin32Error();
                    Win32Exception ex = new Win32Exception(error);
                    Console.WriteLine($"Failed to copy file {sourceFilePath}. Error code: {error}, Message: {ex.Message}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);   
            }
        }

        public static void copy_folder(string sourceFolderPath, string destinationFolderPath)
        {
            if (!Directory.Exists(destinationFolderPath))
                Directory.CreateDirectory(destinationFolderPath);

            IntPtr hFind = IntPtr.Zero;
            WIN32_FIND_DATA findData = new WIN32_FIND_DATA();

            // 枚举源文件夹中的文件和子文件夹
            hFind = FindFirstFileEx(sourceFolderPath + @"\*", FINDEX_INFO_LEVELS.FindExInfoStandard, out findData,
                FINDEX_SEARCH_OPS.FindExSearchNameMatch, IntPtr.Zero, FIND_FIRST_EX_LARGE_FETCH);
            if (hFind != new IntPtr(-1))
            {
                do
                {
                    try
                    {
                        if ((Convert.ToUInt32(findData.dwFileAttributes) & FILE_ATTRIBUTE_DIRECTORY) == FILE_ATTRIBUTE_DIRECTORY)
                        {
                            // 复制子文件夹
                            if (findData.cFileName != "." && findData.cFileName != "..")
                            {
                                string sourceSubFolderPath = sourceFolderPath + @"\" + findData.cFileName;
                                string destinationSubFolderPath = destinationFolderPath + @"\" + findData.cFileName;
                                copy_folder(sourceSubFolderPath, destinationSubFolderPath);
                            }
                        }
                        else
                        {
                            // 复制文件
                            string sourceFilePath = sourceFolderPath + @"\" + findData.cFileName;
                            string destinationFilePath = destinationFolderPath + @"\" + findData.cFileName;
                            copy_file(sourceFilePath, destinationFilePath);
                        }

                        // 检查是否已取消复制操作
                        if (isCancelled)
                        {
                            break;
                        }
                    }
                    catch { }

                } while (FindNextFile(hFind, out findData));

                FindClose(hFind);
            }
        }
    }
}
