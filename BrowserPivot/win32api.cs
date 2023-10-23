using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BrowserPivot
{
    public class win32api
    {
        // 定义 WIN32_FIND_DATA 结构体
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct WIN32_FIND_DATA
        {
            public FileAttributes dwFileAttributes;
            public FILETIME ftCreationTime;
            public FILETIME ftLastAccessTime;
            public FILETIME ftLastWriteTime;
            public uint nFileSizeHigh;
            public uint nFileSizeLow;
            public uint dwReserved0;
            public uint dwReserved1;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string cFileName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
            public string cAlternateFileName;
        }

        // 定义常量和枚举
        public const uint INVALID_HANDLE_VALUE = 0xFFFFFFFF;
        public const uint FILE_ATTRIBUTE_DIRECTORY = 0x10;
        public const uint FIND_FIRST_EX_LARGE_FETCH = 0x2;

        [Flags]
        public enum COPY_FILE_FLAGS : uint
        {
            COPY_FILE_RESTARTABLE = 0x00000002,
            COPY_FILE_COPY_SYMLINK = 0x00000800,
            COPY_FILE_FAIL_IF_EXISTS = 0x00000001,
            COPY_FILE_OPEN_SOURCE_FOR_WRITE = 0x00000004,
            COPY_FILE_ALLOW_DECRYPTED_DESTINATION = 0x00000008,
            COPY_FILE_COPY_HARD_LINK = 0x00001000,
            COPY_FILE_COPY_SECURITY = 0x00002000,
            COPY_FILE_NO_BUFFERING = 0x00001000,
            COPY_FILE_REQUEST_SECURITY_PRIVILEGES = 0x00002000
        }

        public enum FINDEX_INFO_LEVELS
        {
            FindExInfoStandard = 0,
            FindExInfoBasic = 1,
            FindExInfoMaxInfoLevel = 2
        }

        public enum FINDEX_SEARCH_OPS
        {
            FindExSearchNameMatch = 0,
            FindExSearchLimitToDirectories = 1,
            FindExSearchLimitToDevices = 2,
            FindExSearchMaxSearchOp = 3
        }


        // 定义 FindFirstFileEx 函数
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr FindFirstFileEx(string lpFileName, FINDEX_INFO_LEVELS fInfoLevelId,
            out WIN32_FIND_DATA lpFindFileData, FINDEX_SEARCH_OPS fSearchOp, IntPtr lpSearchFilter, uint dwAdditionalFlags);

        // 定义 FindNextFile 函数
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool FindNextFile(IntPtr hFindFile, out WIN32_FIND_DATA lpFindFileData);

        // 定义 FindClose 函数
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool FindClose(IntPtr hFindFile);

        // 定义 CopyFileEx 函数
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool CopyFileEx(string lpExistingFileName, string lpNewFileName,
            IntPtr lpProgressRoutine, IntPtr lpData, ref bool pbCancel, COPY_FILE_FLAGS dwCopyFlags);

    }
}
