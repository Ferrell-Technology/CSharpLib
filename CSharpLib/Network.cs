using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
namespace CSharpLib.Net
{
    /// <summary>
    /// Contains methods for network actions.
    /// </summary>
    public class NetworkInfo
    {
        private static List<string> ips = new List<string>();
        private static List<string> adapters = new List<string>();
        private static Dictionary<string, string> inter = new Dictionary<string, string>();
        /// <summary>
        /// Checks whether a network connection exists and returns true or false.
        /// </summary>
        public bool IsConnected { get; }
        /// <summary>
        /// Returns a string array of the device's IP addresses.
        /// </summary>
        public string[] GetIPAddresses { get; }
        /// <summary>
        /// Returns a string array of connected network adapters.
        /// </summary>
        public string[] GetNetworkAdapters { get; }
        /// <summary>
        /// Returns a dictionary in which the keys are network interface names and their corresponding values are their IP addresses.
        /// </summary>
        public Dictionary<string, string> IPConfig { get; }
        /// <summary>
        /// Returns the computer's host name.
        /// </summary>
        public string GetHostName { get; }
        /// <summary>
        /// Starts a new instance of the NetworkInfo class.
        /// </summary>
        public NetworkInfo()
        {
            ips.Clear();
            adapters.Clear();
            inter.Clear();
            IsConnected = Connected();
            GetIPAddresses = GetAddresses();
            GetNetworkAdapters = GetAdapters();
            IPConfig = Config();
            GetHostName = HostName();
        }
        #region Code
        private bool Connected()
        {
            return NetworkInterface.GetIsNetworkAvailable();
        }
        private string[] GetAddresses()
        {
            ips.Clear();
            IPAddress[] IPS = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress ip in IPS)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    ips.Add(ip.ToString());
                }
            }
            return ips.ToArray();
        }
        private string[] GetAdapters()
        {
            adapters.Clear();
            foreach (NetworkInterface i in NetworkInterface.GetAllNetworkInterfaces())
            {
                adapters.Add(i.Name);
            }
            return adapters.ToArray();
        }
        private string HostName()
        {
            return Dns.GetHostName();
        }
        private Dictionary<string, string> Config()
        {
            ips.Clear();
            inter.Clear();
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    ips.Add(ni.Name);
                    foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            ips.Add(ip.Address.ToString());
                        }
                    }
                }
            }
            if (ips.Count % 2 == 0)
            {
                int count = 0;
                while (count < ips.Count)
                {
                    inter.Add(ips[count], ips[count + 1]);
                    count = count + 2;
                }
            }
            return inter;
        }
        #endregion
    }
    /// <summary>
    /// Contains methods for FTP (File Transfer Protocol).
    /// </summary>
    public class FTPClient
    {
        /// <summary>
        /// Gets the error message from a method in the event it fails to run.
        /// </summary>
        public static string Error { get; private set; }
        private static string ServerAddress { get; set; }
        private static string Username { get; set; }
        private static string Password { get; set; }
        /// <summary>
        /// Starts an instance of the FTPCLient class and logs in to the server with the specified credentials.
        /// </summary>
        /// <param name="serverAddress">The address of the server.</param>
        /// <param name="username">The username to access the server.</param>
        /// <param name="password">The password to access the server.</param>
        public FTPClient(string serverAddress, string username, string password)
        {
            ServerAddress = serverAddress;
            Username = username;
            Password = password;
        }
        /// <summary>
        /// Starts an instance of the FTPClient class and connects to a public server (no credentials).
        /// </summary>
        /// <param name="serverAddress">The address of the server.</param>
        public FTPClient(string serverAddress)
        {
            ServerAddress = serverAddress;
            Username = "";
            Password = "";
        }
        /// <summary>
        /// Tests whether the FTP server is "up" and returns true or false.
        /// </summary>
        /// <returns></returns>
        public bool TestConnection()
        {
            Error = string.Empty;
            WebResponse response = null;
            StreamReader reader = null;
            try
            {
                FtpWebRequest reqFTP;
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ServerAddress + "/"));
                reqFTP.UseBinary = true;
                if (Username != "" && Password != "")
                {
                    reqFTP.Credentials = new NetworkCredential(Username, Password);
                }
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                reqFTP.Proxy = null;
                reqFTP.KeepAlive = false;
                reqFTP.UsePassive = false;
                response = reqFTP.GetResponse();
                reader = new StreamReader(response.GetResponseStream());
                reader.Close();
                return true;
            }
            catch (Exception ex)
            {
                Error = ex.Message;
                return false;
            }
        }
        /// <summary>
        /// Checks whether a file exists on the remote FTP server and returns true or false.
        /// </summary>
        /// <param name="FolderToCheck">The remote folder containing the file to check.</param>
        /// <param name="FileToCheck">The remote file.</param>
        /// <returns></returns>
        public bool FileExists(string FolderToCheck, string FileToCheck)
        {
            Error = string.Empty;
            StringBuilder result = new StringBuilder();
            WebResponse response = null;
            StreamReader reader = null;
            bool bSearchFound = false;
            try
            {
                FtpWebRequest reqFTP;
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ServerAddress + FolderToCheck));
                reqFTP.UseBinary = true;
                if (Username != "" && Password != "")
                {
                    reqFTP.Credentials = new NetworkCredential(Username, Password);
                }
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                reqFTP.Proxy = null;
                reqFTP.KeepAlive = false;
                reqFTP.UsePassive = false;
                response = reqFTP.GetResponse();
                reader = new StreamReader(response.GetResponseStream());
                string line = reader.ReadLine();
                while (line != null)
                {
                    result.Append(line);
                    result.Append("\n");
                    if (FileToCheck.Trim() == line.Trim())
                    {
                        bSearchFound = true;
                    }
                    line = reader.ReadLine();
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Error = ex.Message;
                bSearchFound = false;
            }
            return bSearchFound;
        }
        /// <summary>
        /// Deletes a file on an FTP server.
        /// </summary>
        /// <param name="FTPFolder">The remote folder containing the file to delete.</param>
        /// <param name="FileToDelete">The remote file to delete.</param>
        /// <returns></returns>
        public bool DeleteFile(string FTPFolder, string FileToDelete)
        {
            Error = string.Empty;
            //string[] downloadFiles;
            //StringBuilder result = new StringBuilder();
            WebResponse response = null;
            //StreamReader reader = null;
            bool bDeleted = false;
            try
            {
                FtpWebRequest reqFTP;
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ServerAddress + FTPFolder + FileToDelete));
                reqFTP.UseBinary = true;
                if (Username != "" && Password != "")
                {
                    reqFTP.Credentials = new NetworkCredential(Username, Password);
                }
                reqFTP.Method = WebRequestMethods.Ftp.DeleteFile;
                reqFTP.Proxy = null;
                reqFTP.KeepAlive = false;
                reqFTP.UsePassive = false;
                response = reqFTP.GetResponse();
                bDeleted = true;
            }
            catch (Exception ex)
            {
                Error = ex.Message;
                bDeleted = false;
            }
            return bDeleted;
        }
        /// <summary>
        /// Downloads the specified file from an FTP server.
        /// </summary>
        /// <param name="filename">The remote file to download.</param>
        /// <param name="DownloadPath">The local path to download the file to.</param>
        /// <param name="RemoteFolder">The remote folder containing the file to download.</param>
        /// <param name="deleteFile">Determines whether or not to delete the file on the server after it is downloaded.</param>
        public void DownloadFile(string filename, string DownloadPath, string RemoteFolder, bool deleteFile = false)
        {
            Error = string.Empty;
            try
            {
                // Create a WebClient
                WebClient request = new WebClient();
                // Setup our credentials
                if (Username != "" && Password != "")
                {
                    request.Credentials = new NetworkCredential(Username, Password);
                }
                // Download the data into a Byte array
                string RemoteCompletePath = "ftp://" + ServerAddress + RemoteFolder + filename;
                byte[] fileData = request.DownloadData(RemoteCompletePath);
                // Create a Filestream that we'll write the byte array to 
                FileStream file = System.IO.File.Create(DownloadPath + filename);
                // Write the full byte array to the file
                file.Write(fileData, 0, fileData.Length);
                // Close the file so that other processes can access it.
                file.Close();
                // Once downloaded, delete from FTP location
                if (deleteFile)
                    DeleteFile(RemoteFolder, filename);
            }
            catch (WebException e)
            {
                Error = e.Message;
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    return;
                }
                else
                {
                    return;
                }
            }
        }
        /// <summary>
        /// Uploads the specified file to an FTP server.
        /// </summary>
        /// <param name="filename">The local file to upload.</param>
        /// <param name="UploadPath">The path of the local file to upload.</param>
        /// <param name="RemoteFolder">The remote folder to upload the file to.</param>
        public void UploadFile(string filename, string UploadPath, string RemoteFolder)
        {
            Error = string.Empty;
            try
            {
                // Get a new FtpWebRequest object.
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://" + ServerAddress + RemoteFolder + filename);
                // Method will be UploadFile
                request.Method = WebRequestMethods.Ftp.UploadFile;
                // Set our credentials
                if (Username != "" && Password != "")
                {
                    request.Credentials = new NetworkCredential(Username, Password);
                }
                // Setup a stream for the request and a stream for the file we'll be uploading.
                Stream ftpStream = request.GetRequestStream();
                FileStream file = System.IO.File.OpenRead(UploadPath + filename);
                // Setup variables we'll use to read the file
                int length = 1024;
                byte[] buffer = new byte[length];
                int bytesRead = 0;
                // Write the file to the request stream
                do
                {
                    bytesRead = file.Read(buffer, 0, length);
                    ftpStream.Write(buffer, 0, bytesRead);
                }
                while (bytesRead != 0);
                // Close the streams.
                file.Close();
                ftpStream.Close();
                // Delete the export file once uploaded
                System.IO.File.Delete(UploadPath + filename);
            }
            catch (WebException e)
            {
                Error = e.Message;
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    return;
                }
                else
                {
                    return;
                }
            }
        }
        /// <summary>
        /// Reads a text file on an FTP server without downloading it.
        /// </summary>
        /// <param name="RemoteFile">The remote text file to read.</param>
        /// <returns></returns>
        public string ReadTextFile(string RemoteFile)
        {
            WebClient request = new WebClient();
            string url = "ftp://" + ServerAddress + "/" + RemoteFile;
            if (Username != "" && Password != "")
            {
                request.Credentials = new NetworkCredential(Username, Password);
            }
            return request.DownloadString(url);
        }
        /// <summary>
        /// Reads a non-text (binary) file on an FTP server without downloading it.
        /// </summary>
        /// <param name="RemoteFile">The remote binary file to read.</param>
        /// <returns></returns>
        public byte[] ReadBinaryFile(string RemoteFile)
        {
            WebClient request = new WebClient();
            string url = "ftp://" + ServerAddress + "/" + RemoteFile;
            if (Username != "" && Password != "")
            {
                request.Credentials = new NetworkCredential(Username, Password);
            }
            return request.DownloadData(url);
        }
    }
    /// <summary>
    /// Represents an HTTP request.
    /// </summary>
    public class HTTPRequest
    {
        /// <summary>
        /// The request's URL.
        /// </summary>
        public string URL { get; private set; }
        /// <summary>
        /// The request's HTTP method.
        /// </summary>
        public string HTTPMethod { get; private set; }
        /// <summary>
        /// The request's user host name.
        /// </summary>
        public string UserHostName { get; private set; }
        /// <summary>
        /// The request's user agent.
        /// </summary>
        public string UserAgent { get; private set; }
        /// <summary>
        /// Starts a new instance of the HTTPRequest class.
        /// </summary>
        /// <param name="url">The requests's URL.</param>
        /// <param name="httpmethod">The request's HTTP method.</param>
        /// <param name="userhostname">The request's user host name.</param>
        /// <param name="useragent">The requests's user agent.</param>
        public HTTPRequest(string url, string httpmethod, string userhostname, string useragent)
        {
            URL = url;
            HTTPMethod = httpmethod;
            UserHostName = userhostname;
            UserAgent = useragent;
        }
    }
    public class WorldWideWeb
    {
        /// <summary>
        /// Checks whether the specified URL is online by sending it a WebRequest.
        /// </summary>
        /// <param name="url">The URL string.</param>
        /// <returns>A boolean indicating whether the URL is online.</returns>
        public static bool UrlResponds(string url)
        {
            try
            {
                WebRequest req = WebRequest.Create(url);
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                resp.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// Gets a website's favicon based on the specified URL.
        /// </summary>
        /// <param name="url">The URL string.</param>
        /// <returns>Returns the website's favicon as an Image; returns null if the website doesn't have one.</returns>
        public static Image GetFavIcon(string url)
        {
            Image urlFavIco = null;
            try
            {
                if (UrlResponds(url))
                {
                    Uri thisUrl = new Uri(url);
                    if (thisUrl.HostNameType == UriHostNameType.Dns)
                    {
                        string iconUrl = "http://" + thisUrl.Host + "/favicon.ico";
                        WebRequest req = WebRequest.Create(iconUrl);
                        WebResponse resp = req.GetResponse();
                        urlFavIco = Image.FromStream(resp.GetResponseStream());
                        resp.Close();
                    }
                }
            }
            catch
            {
                return null;
            }
            return urlFavIco;
        }
    }
}
