using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Management;
using System.Runtime.InteropServices;
namespace CSharpLib
{
    /// <summary>
    /// Contains methods for file actions.
    /// </summary>
    /// <exception cref="FileNotFoundException"></exception>
    public static class Files
    {
        /// <summary>
        /// Compresses the file from the FileInfo class, and puts the compressed (.gz) file in the same directory.
        /// </summary>
        /// <param name="fileToCompress">The FileInfo class containing the file to be compressed.</param>
        public static void Compress(this FileInfo fileToCompress)
        {
            using (FileStream originalFileStream = fileToCompress.OpenRead())
            {
                if ((System.IO.File.GetAttributes(fileToCompress.FullName) & FileAttributes.Hidden) != FileAttributes.Hidden & fileToCompress.Extension != ".gz")
                {
                    using (FileStream compressedFileStream = System.IO.File.Create(fileToCompress.FullName + ".gz"))
                    {
                        using (GZipStream compressionStream = new GZipStream(compressedFileStream, CompressionMode.Compress))
                        {
                            StreamWriter w = new StreamWriter(compressionStream);
                            w.Write(compressedFileStream);
                            w.Close();
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Decompresses the file from the FileInfo class, and puts the decompressed file in the same directory.
        /// </summary>
        /// <param name="fileToDecompress">The FileInfo class containing the .gz file to be decompressed.</param>
        public static void Decompress(this FileInfo fileToDecompress)
        {
            using (FileStream originalFileStream = fileToDecompress.OpenRead())
            {
                string currentFileName = fileToDecompress.FullName;
                string newFileName = currentFileName.Remove(currentFileName.Length - fileToDecompress.Extension.Length);
                using (FileStream decompressedFileStream = File.Create(newFileName))
                {
                    using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                    {
                        StreamWriter w = new StreamWriter(decompressedFileStream);
                        w.Write(decompressionStream);
                        w.Close();
                    }
                }
            }
        }
        /// <summary>
        /// Returns the size, in bytes, of the specified file.
        /// </summary>
        /// <param name="file">The file to be analyzed.</param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        public static long GetFileSize(this FileInfo file)
        {
            if (File.Exists(file.FullName))
            {
                return file.Length;
            }
            else
            {
                throw new FileNotFoundException("The specified file does not exist.");
            }
        }
        /// <summary>
        /// Renames a file.
        /// </summary>
        /// <param name="file">The full or relative path of the file.</param>
        /// <param name="newName">The new name for the file, including the file extension.</param>
        /// <exception cref="FileNotFoundException"></exception>
        public static void RenameFile(this FileInfo file, string newName)
        {
            if (File.Exists(file.FullName))
            {
                FileSystem.RenameFile(file.FullName, newName);
            }
            else
            {
                throw new FileNotFoundException("The specified file does not exist.", file.Name);
            }
        }
        /// <summary>
        /// Returns a formatted string representing the size, up to TB, of the file defined in the FileInfo instance (i.e. 10 KB).
        /// </summary>
        /// <param name="info">The FileInfo instance.</param>
        /// <returns></returns>
        public static string FormatBytes(this FileInfo info)
        {
            long bytes = info.Length;
            string[] Suffix = { "B", "KB", "MB", "GB", "TB" };
            int i;
            double dblSByte = bytes;
            for (i = 0; i < Suffix.Length && bytes >= 1024; i++, bytes /= 1024)
            {
                dblSByte = bytes / 1024.0;
            }
            return string.Format("{0:0.##} {1}", dblSByte, Suffix[i]);
        }
        /// <summary>
        /// Removes an attribute from the file defined in the FileInfo instance.
        /// </summary>
        /// <param name="info">The FileInfo instance.</param>
        /// <param name="attributeToRemove">The attribute to remove.</param>
        /// <returns></returns>
        public static void RemoveAttribute(this FileInfo info, FileAttributes attributeToRemove) { info.Attributes = info.Attributes & ~attributeToRemove; }
        /// <summary>
        /// Adds an attribute to the file defined in the FileInfo instance.
        /// </summary>
        /// <param name="info">The FileInfo instance.</param>
        /// <param name="attributeToAdd">The attribute to add.</param>
        public static void SetAttribute(this FileInfo info, FileAttributes attributeToAdd) { info.Attributes = info.Attributes |= attributeToAdd; }
        /// <summary>
        /// Gets the size, in bytes, of the file on disk.
        /// </summary>
        /// <param name="file">THe FileInfo instance.</param>
        /// <returns></returns>
        public static long GetFileSizeOnDisk(this FileInfo file)
        {
            uint dummy, sectorsPerCluster, bytesPerSector;
            int result = Directories.GetDiskFreeSpaceW(file.Directory.Root.FullName, out sectorsPerCluster, out bytesPerSector, out dummy, out dummy);
            if (result == 0)
            {
                throw new Win32Exception();
            }

            uint clusterSize = sectorsPerCluster * bytesPerSector;
            uint hosize;
            uint losize = GetCompressedFileSizeW(file.FullName, out hosize);
            long size;
            size = (long)hosize << 32 | losize;
            return ((size + clusterSize - 1) / clusterSize) * clusterSize;
        }
        /// <summary>
        /// Checks whether a file is empty.
        /// </summary>
        /// <param name="file">The file to check.</param>
        /// <returns></returns>
        public static bool IsEmpty(this FileInfo file)
        {
            if (file.Length == 0)
            {
                return true;
            }
            return false;
        }
        [DllImport("kernel32.dll")]
        private static extern uint GetCompressedFileSizeW([In, MarshalAs(UnmanagedType.LPWStr)] string lpFileName,
           [Out, MarshalAs(UnmanagedType.U4)] out uint lpFileSizeHigh);
    }
    /// <summary>
    /// Contains methods for directory actions.
    /// </summary>
    /// <exception cref="DirectoryNotFoundException"></exception>
    public static class Directories
    {
        /// <summary>
        /// Checks whether a directory is empty.
        /// </summary>
        /// <param name="info">The directory to check.</param>
        /// <returns></returns>
        public static bool IsEmpty(this DirectoryInfo info)
        {
            IEnumerable<string> items = Directory.EnumerateFileSystemEntries(info.FullName);
            using (IEnumerator<string> en = items.GetEnumerator())
            {
                return !en.MoveNext();
            }
        }
        /// <summary>
        /// Renames a directory.
        /// </summary>
        /// <param name="info">The DirectoryInfo instance.</param>
        /// <param name="newName">The new name for the directory.</param>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public static void RenameDirectory(this DirectoryInfo info, string newName)
        {
            if (Directory.Exists(info.FullName))
            {
                FileSystem.RenameDirectory(info.FullName, newName);
            }
            else
            {
                throw new DirectoryNotFoundException("The specified directory does not exist.");
            }
        }
        /// <summary>
        /// Copies a directory and all its contents to a new location.
        /// </summary>
        /// <param name="info">The DirectoryInfo instance.</param>
        /// <param name="outputFolder">The output directory.</param>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public static void CopyDirectory(this DirectoryInfo info, string outputFolder, bool copySubDirectories = true)
        {
            if (Directory.Exists(info.FullName))
            {
                DirectoryInfo dir = new DirectoryInfo(info.FullName);
                DirectoryInfo[] dirs = dir.GetDirectories();     
                Directory.CreateDirectory(outputFolder);
                FileInfo[] files = dir.GetFiles();
                foreach (FileInfo file in files)
                {
                    string tempPath = Path.Combine(outputFolder, file.Name);
                    file.CopyTo(tempPath, false);
                }
                if (copySubDirectories)
                {
                    foreach (DirectoryInfo subdir in dirs)
                    {
                        string tempPath = Path.Combine(outputFolder, subdir.Name);
                        CopyDirectory(subdir, tempPath, copySubDirectories);
                    }
                }
            }
            else
            {
                throw new DirectoryNotFoundException("Thre specified directory does not exist.");
            }
        }
        /// <summary>
        /// Returns the size, in bytes, of the specified directory.
        /// </summary>
        /// <param name="d">The DirectoryInfo class containing the direstory path.</param>
        /// <returns></returns>
        public static long GetDirectorySize(this DirectoryInfo d)
        {
            long size = 0;
            // Add file sizes.
            FileInfo[] fis = d.GetFiles();
            foreach (FileInfo fi in fis)
            {
                size += fi.Length;
            }
            // Add subdirectory sizes.
            DirectoryInfo[] dis = d.GetDirectories();
            foreach (DirectoryInfo di in dis)
            {
                size += GetDirectorySize(di);
            }
            return size;
        }
        /// <summary>
        /// Gets the size, in bytes, of the directory on disk.
        /// </summary>
        /// <param name="directory">The DirectoryInfo instance.</param>
        /// <returns></returns>
        public static long GetDirectorySizeOnDisk(this DirectoryInfo directory)
        {
            long size = 0;
            // Add file sizes.
            FileInfo[] fis = directory.GetFiles();
            foreach (FileInfo fi in fis)
            {
                size += Files.GetFileSizeOnDisk(fi);
            }
            // Add subdirectory sizes.
            DirectoryInfo[] dis = directory.GetDirectories();
            foreach (DirectoryInfo di in dis)
            {
                size += GetDirectorySize(di);
            }
            return size;
        }
        [DllImport("kernel32.dll", SetLastError = true, PreserveSig = true)]
        internal static extern int GetDiskFreeSpaceW([In, MarshalAs(UnmanagedType.LPWStr)] string lpRootPathName,
           out uint lpSectorsPerCluster, out uint lpBytesPerSector, out uint lpNumberOfFreeClusters,
           out uint lpTotalNumberOfClusters);
        /// <summary>
        /// Checks if the path is a directory.
        /// </summary>
        /// <param name="path">The path of the item to check.</param>
        /// <returns></returns>
        public static bool IsADirectory(string path)
        {
            FileAttributes attr = File.GetAttributes(path);
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Returns a formatted string representing the size, up to TB, of the directory defined in the FileInfo instance (i.e. 10 KB).
        /// </summary>
        /// <param name="info">The DirectoryInfo instance.</param>
        /// <returns></returns>
        public static string FormatBytes(this DirectoryInfo info)
        {
            long bytes = info.GetDirectorySize();
            string[] Suffix = { "B", "KB", "MB", "GB", "TB" };
            int i;
            double dblSByte = bytes;
            for (i = 0; i < Suffix.Length && bytes >= 1024; i++, bytes /= 1024)
            {
                dblSByte = bytes / 1024.0;
            }
            return String.Format("{0:0.##} {1}", dblSByte, Suffix[i]);
        }
        /// <summary>
        /// Removes an attribute from the file defined in the DirectoryInfo instance.
        /// </summary>
        /// <param name="info">The DirectoryInfo instance.</param>
        /// <param name="attributeToRemove">The attribute to remove.</param>
        /// <returns></returns>
        public static void RemoveAttribute(this DirectoryInfo info, FileAttributes attributeToRemove) { info.Attributes = info.Attributes & ~attributeToRemove; }
        /// <summary>
        /// Adds an attribute to the file defined in the DirectoryInfo instance.
        /// </summary>
        /// <param name="info">The DirectoryInfo instance.</param>
        /// <param name="attributeToAdd">The attribute to add.</param>
        public static void SetAttribute(this DirectoryInfo info, FileAttributes attributeToAdd) { info.Attributes = info.Attributes |= attributeToAdd; }
    }
    /// <summary>
    /// Contains methods for drive actions.
    /// </summary>
    public static class Drives
    {
        /// <summary>
        /// Formats a drive based on a DriveInfo instance (Windows only).
        /// </summary>
        /// <param name="info">The DriveInfo instance.</param>
        /// <param name="quickFormat">Determines whether the drive should be quick formatted.</param>
        /// <param name="enableCompression">Determines whether compression should be enabled for the drive.</param>
        /// <param name="clusterSize">The drive cluster size, default is 4096.</param>
        /// <returns></returns>
        public static bool Format(this DriveInfo info, bool quickFormat, bool enableCompression, int clusterSize = 4096)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (info.Name.Length != 2 || info.Name[1] != ':' || !char.IsLetter(info.Name[0]))
                {
                    return false;
                }
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(@"select * from Win32_Volume WHERE DriveLetter = '" + info.Name + "'");
                foreach (ManagementObject vi in searcher.Get())
                {
                    vi.InvokeMethod("Format", new object[] {info.DriveFormat, quickFormat, clusterSize, info.VolumeLabel, enableCompression});
                }
                return true;
            }
            else
            {
                return false;
            }
        } 
    }
    /// <summary>
    /// Class containing extensions for native C# keywords.
    /// </summary>
    public static class SystemExtensions
    {
        /// <summary>
        /// Returns a string containing the long with commas added.
        /// </summary>
        /// <param name="num">The long to change.</param>
        /// <returns></returns>
        public static string Commify(this long num) { return num.ToString("N0"); }
        /// <summary>
        /// Returns a string containing the int with commas added.
        /// </summary>
        /// <param name="num">The int to change.</param>
        /// <returns></returns>
        public static string Commify(this int num) { return num.ToString("N0"); }
        /// <summary>
        /// Returns a string containing the uint with commas added.
        /// </summary>
        /// <param name="num">The uint to change.</param>
        /// <returns></returns>
        public static string Commify(this uint num) { return num.ToString("N0"); }
        /// <summary>
        /// Returns a string containing the double with commas added.
        /// </summary>
        /// <param name="num">The double to change.</param>
        /// <returns></returns>
        public static string Commify(this double num) { return num.ToString("N0"); }
        /// <summary>
        /// Returns a string containing the float with commas added.
        /// </summary>
        /// <param name="num">The float to change.</param>
        /// <returns></returns>
        public static string Commify(this float num) { return num.ToString("N0"); }
        /// <summary>
        /// Returns a string containing the short with commas added.
        /// </summary>
        /// <param name="num">The short to change.</param>
        /// <returns></returns>
        public static string Commify(this short num) { return num.ToString("N0");  }
        /// <summary>
        /// Returns a string containing the ulong with commas added.
        /// </summary>
        /// <param name="num">The ulong to change.</param>
        /// <returns></returns>
        public static string Commify(this ulong num) { return num.ToString("N0"); }
        /// <summary>
        /// Returns a string containing the ushort with commas added.
        /// </summary>
        /// <param name="num">The ushort to change.</param>
        /// <returns></returns>
        public static string Commify(this ushort num) { return num.ToString("N0"); }
    }
}
