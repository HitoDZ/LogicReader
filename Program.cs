using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace LogicReader
{
    internal class Program
    {
        static void FileWriter()
        {
            var file2 = new FileStream("iNFO.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            var writer = new StreamWriter(file2);

            writer.WriteLine(
                Screen.AllScreens.Select(screen => screen.Bounds)
                    .Aggregate(Rectangle.Union)
                    .Size
            );
            writer.WriteLine(Info.GetPhysicalMemory());
            writer.WriteLine("Mouse Buttons" + SystemInformation.MouseButtons); //VirtualScreen.Width   );
            //Console.WriteLine(Info.GetCdRomDrive());
            SystemInfo si = new SystemInfo(); //Create an object of SystemInfo class.
            si.getOperatingSystemInfo(writer);
            //Call get operating system info method which will display operating system information.
            //si.getProcessorInfo();
            writer.WriteLine(Directory.GetCurrentDirectory());
            writer.Close();
            file2.Close();
        }

        static Dictionary<string, string> PcInfo()
        {
            Dictionary<string, string> PC = new Dictionary<string, string>();
            
            PC.Add("ScreenSize",
                Screen.AllScreens.Select(screen => screen.Bounds)
                    .Aggregate(Rectangle.Union)
                    .Size.ToString()
            );
            
            PC.Add("PM",Info.GetPhysicalMemory());
            PC.Add("MB","Mouse Buttons" + SystemInformation.MouseButtons);
            PC.Add("Dir",Directory.GetCurrentDirectory());
           
            return PC;
        }


        public static void Main(string[] args)
        {   
            RegistryKey currentUserKey = Registry.CurrentUser;
            Dictionary<string,string> PC= PcInfo();
            RegistryKey findKey = currentUserKey.OpenSubKey("UserInfo");

            if (findKey != null)
            {
               
                
                foreach (var x in PC)
                {
                    if (findKey.GetValue(x.Key).ToString() != x.Value)
                        Console.WriteLine(x.Key+" WARNING wrong data --"+findKey.GetValue(x.Key).ToString()+"-- MUST BE --"+x.Value+"--");

                }

                findKey.Close();

                Console.ReadKey();

            }
            else
            {
                
                findKey = currentUserKey.CreateSubKey("UserInfo");
                foreach (var x in PC)
                {
                    findKey.SetValue(x.Key,x.Value);
                }
                findKey.Close();
                
                Console.ReadKey();
            }
        }
        
        public class SystemInfo
        {
            public void getOperatingSystemInfo(StreamWriter writer)
            {
                Console.WriteLine("Displaying operating system info....\n");
                //Create an object of ManagementObjectSearcher class and pass query as parameter.
                ManagementObjectSearcher mos = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
                Console.WriteLine(mos.Get());
                foreach (ManagementObject managementObject in mos.Get())
                {
                    if (managementObject["Caption"] != null)
                    {
                        writer.WriteLine("Operating System Name  :  " + managementObject["Caption"].ToString());   //Display operating system caption
                    }
                    if (managementObject["OSArchitecture"] != null)
                    {
                        writer.WriteLine("Operating System Architecture  :  " + managementObject["OSArchitecture"].ToString());   //Display operating system architecture.
                    }
                    if (managementObject["CSDVersion"] != null)
                    {
                        writer.WriteLine("Operating System Service Pack   :  " + managementObject["CSDVersion"].ToString());     //Display operating system version.
                    }
                    writer.WriteLine("!!!!!!"+managementObject["RegisteredUser"].ToString());
                    writer.WriteLine("!!!!!!"+managementObject["CSName"].ToString());
                    writer.WriteLine("!!!!!!"+managementObject["BuildType"].ToString());
                    writer.WriteLine("!!!!!!"+managementObject["WindowsDirectory"].ToString());
                    

                }
            }
        }
    }
}