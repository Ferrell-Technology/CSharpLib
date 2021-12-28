using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
namespace CSharpLib.Windows
{
    /// <summary>
    /// Contains Windows-specific methods.
    /// </summary>
    /// <exception cref="UnsupportedOperatingSystemException"></exception>
    public class Win32
    {
        private static List<string> processes = new List<string>();
        private static Dictionary<string, int> output = new Dictionary<string, int>();
        private static List<int> ids = new List<int>();
        private static List<string> users = new List<string>();
        private static List<string> drives = new List<string>();
        private static List<object> programs = new List<object>();
        private static List<string> Programs = new List<string>();
        [DllImport("shell32.dll")]
        private static extern int FindExecutable(string lpFile, string lpDirectory, [Out] StringBuilder lpResult);
        /// <summary>
        /// Returns the application associated with the specified files' extension.
        /// </summary>
        /// <param name="file">The file to associated application from.</param>
        /// <returns></returns>
        public static string GetAppFromExtension(string file)
        {
            StringBuilder builder = new StringBuilder("");
            FindExecutable(file, Path.GetDirectoryName(file), builder);
            return builder.ToString();
        }
        /// <summary>
        /// Sets the window position of an application that is not already running (window dimensions are set when application is opened).
        /// </summary>
        /// <param name="process">The Process instance containing the assembly name.</param>
        /// <param name="position">The position instance to set the application's window to.</param>
        /// <exception cref="Exception"></exception>
        public static void SetWindowPosition(Process process, Position position)
        {
            if (process.StartInfo.FileName != "")
            {
                process.Start();
                Thread.Sleep(100);
                DLLInterops.MoveWindow(process.MainWindowHandle, position.X, position.Y, position.Width, position.Height, true);
            }
            else
            {
                throw new Exception("The specified process instance does not contain an assembly to start.");
            }
        }
        /// <summary>
        /// Repositions an actively running application's window.
        /// </summary>
        /// <param name="processName">The process name of the application.</param>
        /// <param name="position">The Position instance to set the application's window to.</param>
        public static void MoveWindow(string processName, Position position)
        {
            Process[] processes = Process.GetProcesses();
            Process prcs = new Process();
            foreach (Process p in processes)
            {
                if (p.ProcessName == processName)
                {
                    prcs = p;
                    break;
                }
            }
            DLLInterops.SetWindowPos(prcs.MainWindowHandle, 0, position.X, position.Y, position.Width, position.Height, DLLInterops.SWP_NOZORDER);
        }
        /// <summary>
        /// Checks whether a file is in use (locked).
        /// </summary>
        /// <param name="file">The FileInfo instance containing the file to check.</param>
        /// <returns></returns>
        public static bool IsFileInUse(FileInfo file)
        {
            try
            {
                using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (IOException)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Checks whether a file is in use (locked).
        /// </summary>
        /// <param name="file">The location of the file to check.</param>
        /// <returns></returns>
        public static bool IsFileInUse(string file)
        {
            FileInfo f = new FileInfo(file);
            try
            {
                using (FileStream stream = f.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (IOException)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Returns a list of current processes.
        /// </summary>
        /// <returns></returns>
        public static string[] GetCurrentProcesses()
        {
            processes.Clear();
            foreach (Process p in Process.GetProcesses())
            {
                processes.Add(p.ProcessName);
            }
            return processes.ToArray();
        }
        /// <summary>
        /// Returns a dictionary in which the keys are the device's current processes and their corresponding values are their IDs.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, int> GetProcesses()
        {
            output.Clear();
            processes.Clear();
            ids.Clear();
            foreach (Process p in Process.GetProcesses())
            {
                processes.Add(p.ProcessName);
            }
            foreach (Process p in Process.GetProcesses())
            {
                ids.Add(p.Id);
            }
            int counter = 0;
            while (counter < processes.Count)
            {
                if (!output.Keys.Contains(processes[counter]))
                    output.Add(processes[counter], ids[counter]);
                counter = counter + 1;
            }
            return output;
        }
        /// <summary>
        /// Checks whether a process, by name, is running and returns true or false.
        /// </summary>
        /// <param name="processName">The name of the process to check.</param>
        /// <returns></returns>
        public static bool IsRunning(string processName)
        {
            if (Process.GetProcessesByName(processName).Length > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Checks whether a process, by ID, is running and returns true or false.
        /// </summary>
        /// <param name="processId">The ID of the process to check.</param>
        /// <returns></returns>
        public static bool IsRunning(int processId)
        {
            return Process.GetProcesses().Any(x => x.Id == processId);
        }
        /// <summary>
        /// Terminates the specified process and all its children from the processes' ID.
        /// </summary>
        /// <param name="processId">The ID of the process to terminate.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void KillProcess(int processId)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (processId == 0)
                {
                    return;
                }
                ManagementObjectSearcher s = new ManagementObjectSearcher("Select * From Win32_Process Where ParentProcessID=" + processId);
                ManagementObjectCollection c = s.Get();
                foreach (ManagementObject m in c)
                {
                    KillProcess(Convert.ToInt32(m["ProcessID"]));
                }
                try
                {
                    Process p = Process.GetProcessById(processId);
                    p.Kill();
                }
                catch (ArgumentException)
                {
                    throw new ArgumentException("The specified process has already exited.", "processId");
                }
            }         
        }
        /// <summary>
        /// Terminates the specified process using its name (without extension).
        /// </summary>
        /// <param name="processName">The name of the process to stop.</param>
        public static void KillProcess(string processName)
        {
            foreach (var p in Process.GetProcessesByName(processName))
            {
                p.Kill();
            }
        }
        /// <summary>
        /// Returns a string array of local users.
        /// </summary>
        /// <returns></returns>
        public static string[] GetUsers()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                users.Clear();
                SelectQuery q = new SelectQuery("Win32_UserAccount");
                ManagementObjectSearcher s = new ManagementObjectSearcher(q);
                foreach (ManagementObject obj in s.Get())
                {
                    users.Add(obj["Name"].ToString());
                }
                s.Dispose();
                return users.ToArray();
            }
            return null;   
        }
        /// <summary>
        /// Returns a string array of logical drives.
        /// </summary>
        /// <returns></returns>
        public static string[] GetLogicalDrives()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                drives.Clear();
                SelectQuery q = new SelectQuery("Win32_LogicalDisk");
                ManagementObjectSearcher s = new ManagementObjectSearcher(q);
                foreach (ManagementObject obj in s.Get())
                {
                    drives.Add(obj["Name"].ToString());
                }
                s.Dispose();
                return drives.ToArray();
            }
            return null;
        }
        /// <summary>
        /// Returns a string list of the computer's installed programs.
        /// </summary>
        /// <returns></returns>
        public static List<string> GetInstalledPrograms()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                programs.Clear();
                Programs.Clear();
                string registry_key = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registry_key))
                {
                    foreach (string subkey_name in key.GetSubKeyNames())
                    {
                        using (RegistryKey subkey = key.OpenSubKey(subkey_name))
                        {
                            programs.Add(subkey.GetValue("DisplayName"));
                        }
                    }
                }
                foreach (object program in programs)
                {
                    if (program != null)
                    {
                        Programs.Add(program.ToString());
                    }
                }
                return Programs;
            }
            return null;
        }
        /// <summary>
        /// Checks whether the specified program is installed on the machine.
        /// </summary>
        /// <param name="softwareName">The name of the app.</param>
        /// <param name="remoteMachine">Determines whether the app to search for is on a remote machine.</param>
        /// <param name="strComparison">The app names' string comparison.</param>
        /// <returns></returns>
        public static bool IsInstalled(string softwareName, string remoteMachine = null, StringComparison strComparison = StringComparison.Ordinal)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                string uninstallRegKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
                RegistryView[] enumValues = (RegistryView[])Enum.GetValues(typeof(RegistryView));
                //Starts from 1, because first one is Default, so we dont need it...
                for (int i = 1; i < enumValues.Length; i++)
                {
                    //This one key is all what we need, because RegView will do the rest for us
                    using (RegistryKey key = (string.IsNullOrWhiteSpace(remoteMachine))
                        ? RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, enumValues[i]).OpenSubKey(uninstallRegKey)
                        : RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, remoteMachine, enumValues[i]).OpenSubKey(uninstallRegKey))
                    {
                        if (key != null)
                        {
                            if (key.GetSubKeyNames()
                                #pragma warning disable CA1416
                                .Select(keyName => key.OpenSubKey(keyName))
                                #pragma warning disable CA1416
                                .Select(subKey => subKey.GetValue("DisplayName") as string)
                                #pragma warning restore CA1416
                              //SomeTimes we really need the case sensitive/insensitive option...
                                .Any(displayName => displayName != null && displayName.IndexOf(softwareName, strComparison) >= 0))
                            { return true; }
                        }
                    }
                }
                return false;
            }
            return false;
        }
        /// <summary>
        /// Registers an application with the device's 'Add and Remove Programs' list in Control Panel.
        /// </summary>
        /// <param name="appName">The application name.</param>
        /// <param name="publisher">The publisher name.</param>
        /// <param name="installLocation">The installation location of the application.</param>
        /// <param name="displayicon">The icon to display in the list with your application.</param>
        /// <param name="displayVersion">The version to display with our application.</param>
        /// <param name="uninstallString">The location of the application assembly.</param>
        public static void RegisterControlPanelProgram(string appName, string publisher, string installLocation, string displayicon, string displayVersion, string uninstallString)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                try
                {
                    string Install_Reg_Loc = @"Software\Microsoft\Windows\CurrentVersion\Uninstall";
                    RegistryKey hKey = (Registry.LocalMachine).OpenSubKey(Install_Reg_Loc, true);
                    RegistryKey appKey = hKey.CreateSubKey(appName);
                    appKey.SetValue("DisplayName", (object)appName, RegistryValueKind.String);
                    appKey.SetValue("Publisher", (object)publisher, RegistryValueKind.String);
                    appKey.SetValue("InstallLocation", (object)installLocation, RegistryValueKind.ExpandString);
                    appKey.SetValue("DisplayIcon", (object)displayicon, RegistryValueKind.String);
                    appKey.SetValue("UninstallString", (object)uninstallString, RegistryValueKind.ExpandString);
                    appKey.SetValue("DisplayVersion", (object)displayVersion, RegistryValueKind.String);
                }
                catch
                {
                    return;
                }
            }
                
        }
        /// <summary>
        /// Removes an application from the 'Add and Remove Programs' list in Control Panel.
        /// </summary>
        /// <param name="applicationName">The name of the application to remove.</param>
        public static void RemoveControlPanelProgram(string applicationName)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                string InstallerRegLoc = @"Software\Microsoft\Windows\CurrentVersion\Uninstall";
                RegistryKey homeKey = (Registry.LocalMachine).OpenSubKey(InstallerRegLoc, true);
                RegistryKey appSubKey = homeKey.OpenSubKey(applicationName);
                if (null != appSubKey)
                {
                    homeKey.DeleteSubKey(applicationName);
                }
            }            
        }
        /// <summary>
        /// Gets the full directory path of the user currently logged in.
        /// </summary>
        /// <returns></returns>
        public static string GetUserPath()
        {
            string path = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;
            if (Environment.OSVersion.Version.Major >= 6)
            {
                path = Directory.GetParent(path).ToString();
            }
            return path;
        }
        /// <summary>
        /// Shuts down the computer.
        /// </summary>
        public static void Shutdown()
        {
            var psi = new ProcessStartInfo("shutdown", "/s /t 0");
            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;
            Process.Start(psi);
        }
        /// <summary>
        /// Restarts the computer.
        /// </summary>
        public static void Restart()
        {
            var psi = new ProcessStartInfo("shutdown.exe", "-r -t 0");
            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;
            Process.Start(psi);
        }
        /// <summary>
        /// Suspends/sleeps the computer.
        /// </summary>
        public static void Suspend()
        {
            Application.SetSuspendState(PowerState.Suspend, true, true);
        }
        /// <summary>
        /// Hibernates the computer.
        /// </summary>
        public static void Hibernate()
        {
            Application.SetSuspendState(PowerState.Hibernate, true, true);
        }
    }
    /// <summary>
    /// Contains methods for the Graphical User Interface (GUI) in Windows.
    /// </summary>
    /// <exception cref="UnsupportedOperatingSystemException"></exception>
    public class GUI
    {
        [DllImport("User32", CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(int uiAction, int uiParam, string pvParam, uint fWinIni);
        /// <summary>
        /// Sets the desktop wallpaper with the specified image.
        /// </summary>
        /// <param name="image">The image file to set.</param>
        public static void SetDesktopWallpaper(string image)
        {
            SystemParametersInfo(0x0014, 0, image, 0x0001);
        }
        /// <summary>
        /// Closes a graphical Windows application.
        /// </summary>
        /// <param name="processName">The name of the application to close.</param>
        public static void CloseApp(string processName)
        {
            Process[] p = Process.GetProcessesByName(processName);
            foreach (Process s in p)
            {
                 s.CloseMainWindow();
            }
        }
    }
    /// <summary>
    /// Contains methods for the system clock.
    /// </summary>
    public class Clock
    {
        /// <summary>
        /// The current year.
        /// </summary>
        public int Year { get; private set; }
        /// <summary>
        /// The current month.
        /// </summary>
        public int Month { get; private set; }
        /// <summary>
        /// The current day.
        /// </summary>
        public int Day { get; private set; }
        /// <summary>
        /// The current hour.
        /// </summary>
        public int Hour { get; private set; }
        /// <summary>
        /// The current minute at the moment when the class instance is created.
        /// </summary>
        public int Minute { get; private set; }
        /// <summary>
        /// The millisecond at the moment when the class instance is created.
        /// </summary>
        public int Millisecond { get; private set; }
        /// <summary>
        /// The current second at the moment when the class instance is created.
        /// </summary>
        public int Second { get; private set; }
        /// <summary>
        /// The current day of the week.
        /// </summary>
        public string DayofWeek { get; private set; }
        /// <summary>
        /// The current period (AM/PM).
        /// </summary>
        public string Period { get; private set; }
        /// <summary>
        /// Starts a new instance of the Clock class.
        /// </summary>
        /// <exception cref="UnsupportedOperatingSystemException"></exception>
        public Clock()
        {
            DateTime time = DateTime.Now;
            Year = time.Year;
            Month = time.Month;
            Day = time.Day;
            Hour = time.Hour;
            Minute = time.Minute;
            Millisecond = time.Millisecond;
            Second = time.Second;
            DayofWeek = time.DayOfWeek.ToString();
            Period = time.ToString("tt", CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// Updates the initially retrieved time.
        /// </summary>
        public void Refresh()
        {
            Year = 0;
            Month = 0;
            Day = 0;
            Hour = 0;
            Minute = 0;
            Millisecond = 0;
            Second = 0;
            DayofWeek = "";
            Period = "";
            DateTime time = DateTime.Now;
            Year = time.Year;
            Month = time.Month;
            Day = time.Day;
            Hour = time.Hour;
            Minute = time.Minute;
            Millisecond = time.Millisecond;
            Second = time.Second;
            DayofWeek = time.DayOfWeek.ToString();
            Period = time.ToString("tt", CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// Represents orders of time elements.
        /// </summary>
        public enum Order
        {
            /// <summary>
            /// Represents Month-Day-Year configuration.
            /// </summary>
            MonthDayYear,
            /// <summary>
            /// Represents Day-Month-Year configuration.
            /// </summary>
            DayMonthYear
        }
        /// <summary>
        /// Returns the current time based on the specified configuration and separated by the given separator.
        /// </summary>
        /// <param name="order">The order in which the time elements will be placed.</param>
        /// <param name="seperator">The separator character.</param>
        public static string GetTime(Order order, char seperator)
        {
            if (order == Order.DayMonthYear)
            {
                DateTime time = DateTime.Now;
                string[] currentTime = new string[] { time.Day.ToString(), time.Month.ToString(), time.Year.ToString() };
                return string.Join(seperator.ToString(), currentTime);
            }
            else if (order == Order.MonthDayYear)
            {
                DateTime time = DateTime.Now;
                string[] currentTime = new string[] { time.Month.ToString(), time.Day.ToString(), time.Year.ToString() };
                return string.Join(seperator.ToString(), currentTime);
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// Returns the current time in the Month-Day-Year configuration and separated using the specified character.
        /// </summary>
        /// <param name="seperator">The separator character.</param>
        /// <returns></returns>
        public static string GetTime(char seperator)
        {
            DateTime time = DateTime.Now;
            string[] currentTime = new string[] { time.Month.ToString(), time.Day.ToString(), time.Year.ToString() };
            return string.Join(seperator.ToString(), currentTime);
        }
    }
    /// <summary>
    /// Contains methods for manipulating the Windows command prompt (CMD).
    /// </summary>
    public class CMD
    {
        /// <summary>
        /// Represents areas of the Windows CMD.
        /// </summary>
        public enum ConsoleArea
        {
            /// <summary>
            /// Represents the console's foreground.
            /// </summary>
            Foreground,
            /// <summary>
            /// Represents the console's background.
            /// </summary>
            Background
        }
        /// <summary>
        /// Returns the current color of the specified console area.
        /// </summary>
        /// <param name="area">The area of the console.</param>
        /// <returns></returns>
        public static string GetConsoleColor(ConsoleArea area)
        {
            if (area == ConsoleArea.Foreground)
            {
                ConsoleColor color = Console.ForegroundColor;
                return color.ToString();
            }
            else if (area == ConsoleArea.Background)
            {
                ConsoleColor color = Console.BackgroundColor;
                return color.ToString();
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// Sets the specified console area with the given console color and clears the console.
        /// </summary>
        /// <param name="area">The area of the console.</param>
        /// <param name="color">The color to set the area as.</param>
        public static void SetConsoleColor(ConsoleArea area, ConsoleColor color)
        {
            if (area == ConsoleArea.Foreground)
            {
                Console.ForegroundColor = color;
                Console.Clear();
            }
            else if (area == ConsoleArea.Background)
            {
                Console.BackgroundColor = color;
                Console.Clear();
            }
        }
        /// <summary>
        /// Resets the console's foreground and background colors to their original colors.
        /// </summary>
        public static void ResetConsoleColors()
        {
            Console.ResetColor();
        }
        /// <summary>
        /// Enters the specified number of newlines in the console.
        /// </summary>
        /// <param name="numberofLines">The number of lines to enter.</param>
        public static void NewLine(int numberofLines)
        {
            int counter = 0;
            while (counter < numberofLines)
            {
                Console.Write(Environment.NewLine);
                counter = counter + 1;
            }
        }
        /// <summary>
        /// Displays an image in the console.
        /// </summary>
        /// <param name="imageLocation">The location of the image file.</param>
        /// <param name="imageSize">The image size to display.</param>
        /// <param name="consoleLocation">The point on the console buffer to display the image.</param>
        public static void DisplayImageInConsole(string imageLocation, Size imageSize, Point consoleLocation)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Image image = Image.FromFile(imageLocation);
                Form form = new Form();
                form.BackgroundImage = image;
                form.BackgroundImageLayout = ImageLayout.Stretch;
                form.FormBorderStyle = FormBorderStyle.None;
                var parent = Process.GetCurrentProcess().MainWindowHandle;
                var child = form.Handle;
                DLLInterops.SetParent(child, parent);
                DLLInterops.MoveWindow(child, consoleLocation.X, consoleLocation.Y, imageSize.Width, imageSize.Height, true);
                Application.Run(form);
            }
            
        }
    }
    /// <summary>
    /// Class for defining a window's position.
    /// </summary>
    public class Position
    {
        /// <summary>
        /// The X coordinate.
        /// </summary>
        public int X { get; private set; }
        /// <summary>
        /// The Y coordinate.
        /// </summary>
        public int Y { get; private set; }
        /// <summary>
        /// The width of a window.
        /// </summary>
        public int Width { get; private set; }
        /// <summary>
        /// The height of a window.
        /// </summary>
        public int Height { get; private set; }
        /// <summary>
        /// Start a new instance of the Position class.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <param name="width">The width of a window.</param>
        /// <param name="height">The height of a window.</param>
        public Position(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }
    /// <summary>
    /// Class for reading and retrieving the icons of files in Windows.
    /// </summary>
    public class IconReader
    {
        /// <summary>
        /// Options to specify the size of icons to return.
        /// </summary>
        public enum IconSize
        {
            /// <summary>
            /// Large icon - 32 pixels by 32 pixels.
            /// </summary>
            Large = 0,
            /// <summary>
            /// Small icon - 16 pixels by 16 pixels.
            /// </summary>
            Small = 1
        }
        /// <summary>
        /// Options to specify whether a returned folder icon should be in the open or closed state.
        /// </summary>
        public enum FolderType
        {
            /// <summary>
            /// Open folder.
            /// </summary>
            Open = 0,
            /// <summary>
            /// Closed folder.
            /// </summary>
            Closed = 1
        }
        /// <summary>
        /// Returns an icon for a given file.
        /// </summary>
        /// <param name="name">The file path.</param>
        /// <param name="size">Determines the size of the returned icon.</param>
        /// <param name="linkOverlay">Determines whether to include the link icon.</param>
        /// <returns>System.Drawing.Icon</returns>
        public static Icon GetFileIcon(string name, IconSize size, bool linkOverlay)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Shell32.SHFILEINFO shfi = new Shell32.SHFILEINFO();
                uint flags = Shell32.SHGFI_ICON | Shell32.SHGFI_USEFILEATTRIBUTES;
                if (true == linkOverlay) flags += Shell32.SHGFI_LINKOVERLAY;
                /* Check the size specified for return. */
                if (IconSize.Small == size)
                {
                    flags += Shell32.SHGFI_SMALLICON;
                }
                else
                {
                    flags += Shell32.SHGFI_LARGEICON;
                }
                Shell32.SHGetFileInfo(name,
                    Shell32.FILE_ATTRIBUTE_NORMAL,
                    ref shfi,
                    (uint)System.Runtime.InteropServices.Marshal.SizeOf(shfi),
                    flags);
                // Copy (clone) the returned icon to a new object, thus allowing us to clean-up properly
                System.Drawing.Icon icon = (System.Drawing.Icon)System.Drawing.Icon.FromHandle(shfi.hIcon).Clone();
                User32.DestroyIcon(shfi.hIcon);     // Cleanup
                return icon;
            }
            return null;
        }
        private const uint SHGFI_ICON = 0x000000100;
        private const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;
        private const uint SHGFI_OPENICON = 0x000000002;
        private const uint SHGFI_SMALLICON = 0x000000001;
        private const uint SHGFI_LARGEICON = 0x000000000;
        private const uint FILE_ATTRIBUTE_DIRECTORY = 0x00000010;
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };
        /// <summary>
        /// Returns the Windows folder icon.
        /// </summary>
        /// <param name="size">Determines the size of the returned icon.</param>
        /// <param name="folderType">Determines the folder type.</param>
        /// <returns>System.Drawing.Icon</returns>
        public static Icon GetFolderIcon(IconSize size, FolderType folderType)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Need to add size check, although errors generated at present!    
                uint flags = SHGFI_ICON | SHGFI_USEFILEATTRIBUTES;
                if (FolderType.Open == folderType)
                {
                    flags += SHGFI_OPENICON;
                }
                if (IconSize.Small == size)
                {
                    flags += SHGFI_SMALLICON;
                }
                else
                {
                    flags += SHGFI_LARGEICON;
                }
                // Get the folder icon    
                var shfi = new SHFILEINFO();
                var res = SHGetFileInfo(@"C:\Windows",
                    FILE_ATTRIBUTE_DIRECTORY,
                    out shfi,
                    (uint)Marshal.SizeOf(shfi),
                    flags);
                if (res == IntPtr.Zero)
                    throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
                // Load the icon from an HICON handle  
                Icon.FromHandle(shfi.hIcon);
                // Now clone the icon, so that it can be successfully stored in an ImageList
                var icon = (Icon)Icon.FromHandle(shfi.hIcon).Clone();
                User32.DestroyIcon(shfi.hIcon);        // Cleanup    
                return icon;
            }
            return null;
        }
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, out SHFILEINFO psfi, uint cbFileInfo, uint uFlags);
        private class Shell32
        {
            public const int MAX_PATH = 256;
            [StructLayout(LayoutKind.Sequential)]
            public struct SHITEMID
            {
                public ushort cb;
                [MarshalAs(UnmanagedType.LPArray)]
                public byte[] abID;
            }
            [StructLayout(LayoutKind.Sequential)]
            public struct ITEMIDLIST
            {
                public SHITEMID mkid;
            }
            [StructLayout(LayoutKind.Sequential)]
            public struct BROWSEINFO
            {
                public IntPtr hwndOwner;
                public IntPtr pidlRoot;
                public IntPtr pszDisplayName;
                [MarshalAs(UnmanagedType.LPTStr)]
                public string lpszTitle;
                public uint ulFlags;
                public IntPtr lpfn;
                public int lParam;
                public IntPtr iImage;
            }
            // Browsing for directory.
            public const uint BIF_RETURNONLYFSDIRS = 0x0001;
            public const uint BIF_DONTGOBELOWDOMAIN = 0x0002;
            public const uint BIF_STATUSTEXT = 0x0004;
            public const uint BIF_RETURNFSANCESTORS = 0x0008;
            public const uint BIF_EDITBOX = 0x0010;
            public const uint BIF_VALIDATE = 0x0020;
            public const uint BIF_NEWDIALOGSTYLE = 0x0040;
            public const uint BIF_USENEWUI = (BIF_NEWDIALOGSTYLE | BIF_EDITBOX);
            public const uint BIF_BROWSEINCLUDEURLS = 0x0080;
            public const uint BIF_BROWSEFORCOMPUTER = 0x1000;
            public const uint BIF_BROWSEFORPRINTER = 0x2000;
            public const uint BIF_BROWSEINCLUDEFILES = 0x4000;
            public const uint BIF_SHAREABLE = 0x8000;
            [StructLayout(LayoutKind.Sequential)]
            public struct SHFILEINFO
            {
                public const int NAMESIZE = 80;
                public IntPtr hIcon;
                public int iIcon;
                public uint dwAttributes;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
                public string szDisplayName;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = NAMESIZE)]
                public string szTypeName;
            };
            public const uint SHGFI_ICON = 0x000000100;     // get icon
            public const uint SHGFI_DISPLAYNAME = 0x000000200;     // get display name
            public const uint SHGFI_TYPENAME = 0x000000400;     // get type name
            public const uint SHGFI_ATTRIBUTES = 0x000000800;     // get attributes
            public const uint SHGFI_ICONLOCATION = 0x000001000;     // get icon location
            public const uint SHGFI_EXETYPE = 0x000002000;     // return exe type
            public const uint SHGFI_SYSICONINDEX = 0x000004000;     // get system icon index
            public const uint SHGFI_LINKOVERLAY = 0x000008000;     // put a link overlay on icon
            public const uint SHGFI_SELECTED = 0x000010000;     // show icon in selected state
            public const uint SHGFI_ATTR_SPECIFIED = 0x000020000;     // get only specified attributes
            public const uint SHGFI_LARGEICON = 0x000000000;     // get large icon
            public const uint SHGFI_SMALLICON = 0x000000001;     // get small icon
            public const uint SHGFI_OPENICON = 0x000000002;     // get open icon
            public const uint SHGFI_SHELLICONSIZE = 0x000000004;     // get shell size icon
            public const uint SHGFI_PIDL = 0x000000008;     // pszPath is a pidl
            public const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;     // use passed dwFileAttribute
            public const uint SHGFI_ADDOVERLAYS = 0x000000020;     // apply the appropriate overlays
            public const uint SHGFI_OVERLAYINDEX = 0x000000040;     // Get the index of the overlay
            public const uint FILE_ATTRIBUTE_DIRECTORY = 0x00000010;
            public const uint FILE_ATTRIBUTE_NORMAL = 0x00000080;
            [DllImport("Shell32.dll")]
            public static extern IntPtr SHGetFileInfo(
                string pszPath,
                uint dwFileAttributes,
                ref SHFILEINFO psfi,
                uint cbFileInfo,
                uint uFlags
                );
        }
        private class User32
        {
            [DllImport("User32.dll")]
            public static extern int DestroyIcon(IntPtr hIcon);
        }
    }
    internal class DLLInterops
    {
        internal const short SWP_NOMOVE = 0X2;
        internal const short SWP_NOSIZE = 1;
        internal const short SWP_NOZORDER = 0X4;
        internal const int SWP_SHOWWINDOW = 0x0040;
        [DllImport("user32.dll")]
        internal static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        internal static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int cx, int cy, int wFlags);
    }
}
