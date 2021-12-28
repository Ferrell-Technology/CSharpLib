using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Text;
using System.Runtime.InteropServices;
namespace CSharpLib.Computer
{
    /// <summary>
    /// Interfaces as a miniature user account.
    /// </summary>
    public class User
    {
        private static bool loggedin = false;
        private static StringBuilder userpath = new StringBuilder();
        /// <summary>
        /// Creates a new user with the specified username and password.
        /// </summary>
        /// <param name="username">The username of the account to create.</param>
        /// <param name="password">The password of the account to create.</param>
        /// <exception cref="UserAlreadyExistsException"></exception>
        public User(string username, string password)
        {
            CheckRootDirectory();
            if (Directory.Exists(Path.Combine(VARS.UserClassPath, username)))
            {
                throw new UserAlreadyExistsException("The specified user already exists.");
            }
            else if (!Directory.Exists(Path.Combine(VARS.UserClassPath, username)))
            {
                CreateUserFolder(username);
                loggedin = true;
                userpath.Remove(0, userpath.Length);
                userpath.Append(Path.Combine(VARS.UserClassPath, username) + @"\");
                StreamWriter writer = new StreamWriter(userpath.ToString() + username + ".txt");
                writer.Write(Encryption.Encrypt(password, "CSL"));
                writer.Close();
            }
        }
        private User(string username, string password, bool c = false)
        {
            CheckRootDirectory();
            if (!Directory.Exists(Path.Combine(VARS.UserClassPath, username)))
            {
                throw new UserNotFoundException("The specified user was not found.");
            }
            else if (Directory.Exists(Path.Combine(VARS.UserClassPath, username)) && loggedin == false)
            {
                userpath.Remove(0, userpath.Length);
                userpath.Append(Path.Combine(VARS.UserClassPath, username) + @"\");
                if (password == Encryption.Decrypt(System.IO.File.ReadAllText(userpath.ToString() + username + ".txt"), "CSL"))
                {
                    loggedin = true;
                }
                else
                {
                    throw new InvalidPasswordException("The specified password does not match the username.");
                }
            }
        }
        /// <summary>
        /// Logs in an existing user.
        /// </summary>
        /// <param name="username">The username of the account.</param>
        /// <param name="password">The password of the account.</param>
        public static User Login(string username, string password)
        {
            return new User(username, password, true);
        }
        /// <summary>
        /// Logs the current user out.
        /// </summary>
        public void Logout()
        {
            loggedin = false;
            userpath.Remove(0, userpath.Length);
        }
        /// <summary>
        /// Removes the specified user.
        /// </summary>
        /// <param name="username">The username of the account to remove.</param>
        /// <param name="password">The password of the account to remove.</param>
        /// <exception cref="UserNotFoundException"></exception>
        /// <exception cref="InvalidPasswordException"></exception>
        public static void RemoveUser(string username, string password)
        {
            if (!Directory.Exists(Path.Combine(VARS.UserClassPath, username)))
            {
                throw new UserNotFoundException("The specified user doesn't exist.");
            }
            else if (Directory.Exists(Path.Combine(VARS.UserClassPath, username)))
            {
                userpath.Remove(0, userpath.Length);
                userpath.Append(Path.Combine(VARS.UserClassPath, username) + @"\");
                if (password == Encryption.Decrypt(System.IO.File.ReadAllText(userpath.ToString() + username + ".txt"), "CSL"))
                {
                    Directory.Delete(Path.Combine(VARS.UserClassPath, username), true);
                }
                else
                {
                    throw new InvalidPasswordException("The spedified password does not match the username.");
                }
            }
        }
        /// <summary>
        /// Returns a string array of registered user accounts.
        /// </summary>
        /// <returns></returns>
        public static string[] GetUserAccounts()
        {
            List<string> dirs = new List<string>();
            string[] dirnames = Directory.GetDirectories(userpath.ToString());
            foreach (string dir in dirnames)
            {
                string[] split = dir.Split('\\');
                dirs.Add(Collections.GetLastElement(split));
            }
            return dirs.ToArray();
        }
        /// <summary>
        /// Copies an existing file to the root directory of the user currently logged in.
        /// </summary>
        /// <param name="file">The file to add.</param>
        /// <exception cref="FileNotFoundException"></exception>
        public void AddFile(string file)
        {
            if (File.Exists(file))
            {
                File.Copy(file, userpath.ToString() + Path.GetFileName(file));
            }
            else
            {
                throw new FileNotFoundException("The specified file was not found.");
            }
        }
        /// <summary>
        /// Adds an existing file to the root directory on the user currently logged in.
        /// </summary>
        /// <param name="file">The file to add.</param>
        /// <param name="move">Determines how the file will be added: set to true to move the file; false to copy it.</param>
        /// <exception cref="FileNotFoundException"></exception>
        public void AddFile(string file, bool move)
        {
            if (File.Exists(file))
            {
                if (move)
                {
                    File.Move(file, userpath.ToString() + Path.GetFileName(file));
                }
                else
                {
                    File.Copy(file, userpath.ToString() + Path.GetFileName(file));
                }              
            }
            else
            {
                throw new FileNotFoundException("The specified file was not found.");
            }
        }
        /// <summary>
        /// Adds a file to the user currently logged in.
        /// </summary>
        /// <param name="file">The file to add.</param>
        /// <param name="directory">The subdirectory to place the file. For multiple directory levels, set it like this: subDir1/subDir2/subDir3 etc.</param>
        /// <param name="move">Determines how the file will be added: set to true to move the file; false to copy it.</param>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public void AddFile(string file, string directory, bool move)
        {
            if (File.Exists(file))
            {
                if (Directory.Exists(Path.Combine(userpath.ToString(), directory)))
                {
                    if (move)
                    {
                        File.Move(file, Path.Combine(userpath.ToString(), directory, Path.GetFileName(file)));
                    }
                    else
                    {
                        File.Copy(file, Path.Combine(userpath.ToString(), directory, Path.GetFileName(file)));
                    }
                }
                else
                {
                    throw new DirectoryNotFoundException("The specified directory was not found.");
                }           
            }
            else
            {
                throw new FileNotFoundException("The specified file was not found.");
            }
        }
        /// <summary>
        /// Adds a list of existing files to the user currently logged in.
        /// </summary>
        /// <param name="files">The list of files to add.</param>
        /// <param name="directory">The subdirectory to place the file. For multiple directory levels, set it like this: subDir1/subDir2/subDir3 etc.</param>
        /// <param name="move">Determines how the file will be added: set to true to move the file; false to copy it.</param>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public void AddFile(List<string> files, string directory, bool move)
        {
            if (!Directory.Exists(Path.Combine(userpath.ToString(), directory)))
            {
                throw new DirectoryNotFoundException("The specified directory was not found.");
            }
            else
            {
                foreach (string file in files)
                {
                    if (File.Exists(file))
                    {
                        if (move)
                        {
                            File.Move(file, Path.Combine(userpath.ToString(), directory, Path.GetFileName(file)));
                        }
                        else
                        {
                            File.Copy(file, Path.Combine(userpath.ToString(), directory, Path.GetFileName(file)));
                        }
                    }
                    else
                    {
                        throw new FileNotFoundException("The file " + file + " was not found.");
                    }
                }
            }
        }
        /// <summary>
        /// Adds a list of existing files to the root directory of the user currently logged in.
        /// </summary>
        /// <param name="files">The list of files to add.</param>
        /// <param name="move">Determines how the file will be added: set to true to move the file; false to copy it.</param>
        /// <exception cref="FileNotFoundException"></exception>
        public void AddFile(List<string> files, bool move)
        {
            foreach (string file in files)
            {
                if (File.Exists(file))
                {
                    if (move)
                    {
                        File.Move(file, userpath.ToString() + Path.GetFileName(file));
                    }
                    else
                    {
                        File.Copy(file, userpath.ToString() + Path.GetFileName(file));
                    }
                }
                else
                {
                    throw new FileNotFoundException("The file " + file + " was not found.");
                }
            }
        }
        /// <summary>
        /// Copies a list of files to the root directory of the user currently logged in.
        /// </summary>
        /// <param name="files">The list of files to add.</param>
        /// <exception cref="FileNotFoundException"></exception>
        public void AddFile(List<string> files)
        {
            foreach (string file in files)
            {
                if (File.Exists(file))
                {
                    File.Copy(file, userpath.ToString() + Path.GetFileName(file));
                }
                else
                {
                    throw new FileNotFoundException("The file " + file + " was not found.");
                }
            }
        }
        /// <summary>
        /// Writes to the specified file in the current user or creates it if it doesn't exist.
        /// </summary>
        /// <param name="filename">The file to write to.</param>
        /// <param name="texttowrite">The text to write to the file.</param>
        /// <exception cref="LoginErrorException"></exception>
        public void WriteFile(string filename, string[] texttowrite)
        {
            if (loggedin == true)
            {
                StreamWriter writer = new StreamWriter(userpath.ToString() + filename);
                foreach (string line in texttowrite)
                {
                    writer.WriteLine(line);
                }
                writer.Close();
            }
            else
            {
                throw new LoginErrorException("This method cannot run because no user is logged in.");
            }
        }
        /// <summary>
        /// Reads the given file and returns a string array of lines.
        /// </summary>
        /// <param name="filename">The file to read.</param>
        /// <returns></returns>
        /// <exception cref="LoginErrorException"></exception>
        public string[] ReadFile(string filename)
        {
            if (loggedin == true)
            {
                string file = userpath.ToString() + filename;
                if (System.IO.File.Exists(file))
                {
                    return System.IO.File.ReadAllLines(file);
                }
                else
                {
                    throw new FileNotFoundException("The specified file does not exist.");
                }
            }
            else
            {
                throw new LoginErrorException("This method cannot run because no user is logged in.");
            }
        }
        /// <summary>
        /// Returns the size, in bytes, of the user account.
        /// </summary>
        /// <param name="username">The username of the account.</param>
        /// <returns></returns>
        /// <exception cref="UserNotFoundException"></exception>
        public long GetUserSize(string username)
        {
            if (Directory.Exists(Path.Combine(VARS.UserClassPath, username)))
            {
                DirectoryInfo d = new DirectoryInfo(Path.Combine(VARS.UserClassPath, username));
                return Directories.GetDirectorySize(d);
            }
            else
            {
                throw new UserNotFoundException("The specified user doesn't exist.");
            }
        }
        /// <summary>
        /// Creates a directory in the current user account.
        /// </summary>
        /// <param name="directoryName">The name of the directory to create.</param>
        /// <exception cref="DirectoryAlreadyExistsException"></exception>
        /// <exception cref="LoginErrorException"></exception>
        public void CreateDirectory(string directoryName)
        {
            if (loggedin == true)
            {
                string dir = Path.Combine(userpath.ToString(), directoryName);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                else
                {
                    throw new DirectoryAlreadyExistsException("The specified directory already exists.");
                }
            }
            else
            {
                throw new LoginErrorException("This method cannot run because no user is logged in.");
            }
        }
        /// <summary>
        /// Returns a list of files in the root directory of the current user account.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="LoginErrorException"></exception>
        public string[] GetFiles()
        {
            if (loggedin == true)
            {
                List<string> files = new List<string>();
                foreach (string file in Directory.GetFiles(userpath.ToString()))
                {
                    files.Add(Path.GetFileName(file));
                }
                return files.ToArray();
            }
            else
            {
                throw new LoginErrorException("This method cannot run because no user is logged in.");
            }
        }
        private static bool CreateUserFolder(string username)
        {
            string userdir = Path.Combine(VARS.UserClassPath, username);
            if (Directory.Exists(userdir))
            {
                return false;
            }
            else if (!Directory.Exists(userdir))
            {
                Directory.CreateDirectory(userdir);
                return true;
            }
            return false;
        }
        private static void CheckRootDirectory()
        {
            if (!Directory.Exists(VARS.UserRootDir))
            {
                Directory.CreateDirectory(VARS.UserRootDir);
            }
        }
    }
    /// <summary>
    /// Contains information about the operating system. (Windows only)
    /// </summary>
    public class OSInfo
    {
        /// <summary>
        /// The operating system's boot directory.
        /// </summary>
        public string BootDirectory { get; }
        /// <summary>
        /// The operating system's scratch directory.
        /// </summary>
        public string ScratchDirectory { get; }
        /// <summary>
        /// The operating system's temporary directory.
        /// </summary>
        public string TempDirectory { get; }
        /// <summary>
        /// The operating system's boot device.
        /// </summary>
        public string BootDevice { get; }
        /// <summary>
        /// The operating system's build number.
        /// </summary>
        public string BuildNumber { get; }
        /// <summary>
        /// The name of the operating system.
        /// </summary>
        public string SystemName { get; }
        /// <summary>
        /// The operating system's maximum number of processes.
        /// </summary>
        public string MaxNumberofProcesses { get; }
        /// <summary>
        /// The maximum size of a process.
        /// </summary>
        public string MaxProcessMemorySize { get; }
        /// <summary>
        /// The current number of processes.
        /// </summary>
        public string NumberofProcesses { get; }
        /// <summary>
        /// The current number of users.
        /// </summary>
        public string NumberofUsers { get; }
        /// <summary>
        /// The operating system's architecture.
        /// </summary>
        public string OSArchitecture { get; }
        /// <summary>
        /// The system directory of the operating system.
        /// </summary>
        public string SystemDirectory { get; }
        /// <summary>
        /// The drive that contains the operating system.
        /// </summary>
        public string SystemDrive { get; }
        /// <summary>
        /// The Windows directory of the operating system.
        /// </summary>
        public string WindowsDirectory { get; }
        /// <summary>
        /// Starts a new instance of the OSInfo class. (Windows only)
        /// </summary>
        public OSInfo()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                ManagementClass osinfo = new ManagementClass("win32_bootconfiguration");
                ManagementObjectCollection moc = osinfo.GetInstances();
                foreach (ManagementObject m in moc)
                {
                    BootDirectory = m.Properties["BootDirectory"].Value.ToString();
                    ScratchDirectory = m.Properties["ScratchDirectory"].Value.ToString();
                    TempDirectory = m.Properties["TempDirectory"].Value.ToString();
                    break;
                }
                moc.Dispose();
                osinfo.Dispose();
                ManagementClass info = new ManagementClass("win32_operatingsystem");
                ManagementObjectCollection c = info.GetInstances();
                foreach (ManagementObject i in c)
                {
                    BootDevice = i.Properties["bootdevice"].Value.ToString();
                    BuildNumber = i.Properties["buildnumber"].Value.ToString();
                    SystemName = i.Properties["csname"].Value.ToString();
                    MaxNumberofProcesses = i.Properties["maxnumberofprocesses"].Value.ToString();
                    MaxProcessMemorySize = i.Properties["maxprocessmemorysize"].Value.ToString();
                    NumberofProcesses = i.Properties["numberofprocesses"].Value.ToString();
                    NumberofUsers = i.Properties["numberofusers"].Value.ToString();
                    OSArchitecture = i.Properties["osarchitecture"].Value.ToString();
                    SystemDirectory = i.Properties["systemdirectory"].Value.ToString();
                    SystemDrive = i.Properties["systemdrive"].Value.ToString();
                    WindowsDirectory = i.Properties["windowsdirectory"].Value.ToString();
                    break;
                }
                c.Dispose();
                info.Dispose();
            }         
        }
    }
    /// <summary>
    /// Contains information about the system hardware. (Windows only)
    /// </summary>
    public class HardwareInfo
    {
        /// <summary>
        /// The ID of the processor.
        /// </summary>
        public string ProcessorID { get; }
        /// <summary>
        /// The clock speed of the processor.
        /// </summary>
        public string ProcessorClockSpeed { get; }
        /// <summary>
        /// The processor's maximum clock speed.
        /// </summary>
        public string ProcessorMaxClockSpeed { get; }
        /// <summary>
        /// The name of the processor.
        /// </summary>
        public string ProcessorName { get; }
        /// <summary>
        /// The processor status.
        /// </summary>
        public string ProcessorStatus { get; }
        /// <summary>
        /// The processor architecture.
        /// </summary>
        public string ProcessorArchitecture { get; }
        /// <summary>
        /// The BIOS manufacturer.
        /// </summary>
        public string BIOSManufacturer { get; }
        /// <summary>
        /// The BIOS name.
        /// </summary>
        public string BIOSName { get; }
        /// <summary>
        /// The major version of the BIOS.
        /// </summary>
        public string BIOSMajorVersion { get; }
        /// <summary>
        /// The minor version of the BIOS.
        /// </summary>
        public string BIOSMinorVersion { get; }
        /// <summary>
        /// Starts a new instance of the HardwareInfo class. (Windows only)
        /// </summary>
        public HardwareInfo()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) 
            {
                ManagementClass processorid = new ManagementClass("win32_processor");
                ManagementObjectCollection moc = processorid.GetInstances();
                foreach (ManagementObject m in moc)
                {
                    ProcessorID = m.Properties["processorID"].Value.ToString();
                    ProcessorClockSpeed = m.Properties["CurrentClockSpeed"].Value.ToString();
                    ProcessorMaxClockSpeed = m.Properties["MaxClockSpeed"].Value.ToString();
                    ProcessorName = m.Properties["Name"].Value.ToString();
                    ProcessorStatus = m.Properties["Status"].Value.ToString();
                    ProcessorArchitecture = m.Properties["Architecture"].Value.ToString();
                    break;
                }
                moc.Dispose();
                processorid.Dispose();
                ManagementClass biosinfo = new ManagementClass("win32_bios");
                ManagementObjectCollection boc = biosinfo.GetInstances();
                foreach (ManagementObject m in boc)
                {
                    BIOSManufacturer = m.Properties["manufacturer"].Value.ToString();
                    BIOSName = m.Properties["name"].Value.ToString();
                    BIOSMajorVersion = m.Properties["systembiosmajorversion"].Value.ToString();
                    BIOSMinorVersion = m.Properties["systembiosminorversion"].Value.ToString();
                    break;
                }
                boc.Dispose();
                biosinfo.Dispose();
            } 
        }
    }
}
