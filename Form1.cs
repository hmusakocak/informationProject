using iTin.Core.Hardware.Common;
using iTin.Hardware.Specification;
using iTin.Hardware.Specification.Dmi;
using iTin.Hardware.Specification.Dmi.Property;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Management;
using System.Management.Automation;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;


namespace sysinfo
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public ManagementObjectSearcher createmos(string a)
        {
            string query = String.Format("Select * from {0}", a);
            ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(query);
            if (a == "Win32_LogicalDisk")
            {
                query = String.Format("Select * from {0} where DriveType = '3'", a);
                managementObjectSearcher = new ManagementObjectSearcher(query);
                return managementObjectSearcher;
            }
            return managementObjectSearcher;
        }
        public ManagementObjectSearcher createmos(string a, short b)
        {
            string query = String.Format("Select * from {0} where DeviceID = 'VideoController{1}'", a, b);
            ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(query);
            return managementObjectSearcher;

        }


        public string storagedevices = "";
        public string freespacedisk;

        public string memorytype = " ";
        public string ramid;
        public double totalram = 0;
        public string ramrate;
        public DmiStructureCollection structures = DMI.CreateInstance().Structures;


        public SqlConnection conn = new SqlConnection("Server=52.166.136.119\\SERVICEDESK;Database=envanter;User Id=envanter_user;Password=Aeris**123;");

        public void SendEmailUsingGmail(string email)
        {

            MailMessage mail = new MailMessage("aerisenvanter@gmail.com", email)
            {
                From = new MailAddress("aerisenvanter@gmail.com", "Aeris Teknik-Envanter"),
                Subject = "System Analysis of " + label5.Text,
                Body = "<br><b><font size ='5'>ENVANTER ANALİZ</font></b>"
                        + "<hr><br>" + "CPU:     " + "<b>" + label1.Text
                        + "</b>" + "<br>" + "GPU1:     " + "<b>" + label2.Text
                        + "</b>" + "<br>" + "GPU2:     " + "<b>" + label3.Text
                        + "</b>" + "<br>" + "RAM:     " + "<b>" + label4.Text + " // " + ramid.Trim() + "<br><br><hr>"
                        + "</b>" + "<br>" + "Model:     " + "<b>" + label5.Text
                        + "</b>" + "<br>" + "Serial Number:     " + "<b>" + label6.Text
                        + "</b>" + "<br>" + "</b>" + "Domain:     " + "<b>" + label7.Text
                        + "</b>" + "<br>" + "</b>" + "Operating System:     " + "<b>" + label8.Text + "</b>"
                        + "</b>" + "<br>" + "</b>" + "Hostname:     " + "<b>" + label9.Text + "</b>"
                        + "</b>" + "<br>" + "</b>" + "Office Version:     " + "<b>" + label10.Text + "</b>"
                        + "</b>" + "<br>" + "</b>" + "Manufacturer:     " + "<b>" + label18.Text + "</b>" + "<br><br><hr>"
                        + "<br>" + "Storage:     " + "<b>" + storagedevices
                        + "<br>" + "</b>" + "Free Disk Space:     " + "<b>" + freespacedisk + "<br><br><hr>"
                        + "<br>" + "</b>" + "Note:     " + "<b>" + richTextBox1.Text + "</b>",
                IsBodyHtml = true,
                Priority = MailPriority.Normal
            };

            using (SmtpClient smtpClient =
                new SmtpClient("smtp.gmail.com", 587))
            {
                smtpClient.Credentials =
                    new NetworkCredential()
                    {
                        UserName = "aerisenvanter@gmail.com",
                        Password = "bnkvrvxapfcimhid"
                    };
                smtpClient.EnableSsl = true;

                smtpClient.Send(mail);
            }

        }
        private void Form1_Load(object sender, EventArgs e)
        {

            label10.Text = "Not found!";
            label14.Text = " ";

            try
            {
                Ping myPing = new Ping();
                String adres = "google.com";
                byte[] paketboyutu = new byte[32];
                int zamanasimi = 1000;
                PingReply cevap = myPing.Send(adres, zamanasimi, paketboyutu);
                label16.Visible = false;
            }
            catch (Exception)
            {
                label16.Text = "no internet connection!";
                label14.ForeColor = System.Drawing.Color.Red;
                label14.Visible = true;
                label14.Text = "Cannot record data and send mail!";
                button1.Enabled = false;
                textBox1.Enabled = false;
                richTextBox1.Enabled = false;
            }
            label7.Text = IPGlobalProperties.GetIPGlobalProperties().DomainName;

            if (label7.Text.Trim() == String.Empty)
            {
                foreach (var item in createmos("Win32_ComputerSystem").Get())
                {
                    label7.Text = item["Domain"].ToString();
                }
            }
            foreach (var item in createmos("Win32_ComputerSystem").Get())
            {
                label5.Text = item["Model"].ToString();
                label18.Text = item["Manufacturer"].ToString();
                label9.Text = item["Name"].ToString();
            }



            foreach (var item in createmos("Win32_OperatingSystem").Get())
            {
                label8.Text = item["Caption"].ToString();
            }




            PowerShell PowerShellInst = PowerShell.Create();
            PowerShellInst.AddScript("Get-PhysicalDisk|Sort-Object -Property model |Select Model, MediaType, Size, BusType");
            Collection<PSObject> PSOutput = PowerShellInst.Invoke();

            foreach (PSObject obj in PSOutput)
            {
                try
                {
                    if (obj == null || obj.Properties["BusType"].Value.ToString().ToLower() == "usb")
                    {
                        continue;
                    }
                    else
                    {
                        listBox1.Items.Add(obj.Properties["model"].Value.ToString()
                            + " // " + obj.Properties["MediaType"].Value.ToString() + " // "
                            + Math.Round(Convert.ToDouble(obj.Properties["Size"].Value) / Math.Pow(1024, 3)) + "gb");
                    }
                }
                catch (Exception)
                {

                    if (obj == null || obj.Properties["BusType"].Value.ToString().ToLower() == "usb")
                    {
                        continue;
                    }
                    else
                    {
                        listBox1.Items.Add(obj.Properties["Model"].Value.ToString()
                            + " // "
                            + Math.Round(Convert.ToDouble(obj.Properties["Size"].Value) / Math.Pow(1024, 3)) + "gb");
                    }
                }
            }


            foreach (var item in listBox1.Items)
            {
                storagedevices += "|| " + item.ToString() + " || ";
            }




            foreach (var item in createmos("Win32_Processor").Get())
            {
                if (item["Name"].ToString().Contains("@"))
                {
                    int a = item["Name"].ToString().IndexOf('@');
                    label1.Text = item["Name"].ToString().Substring(0, a);
                }
                else
                {
                    label1.Text = item["Name"].ToString();
                }
            }
            QueryPropertyResult cpusocket = structures.GetProperty(DmiProperty.Processor.SocketDesignation);
            label1.Text += " (Socket : " + cpusocket.Result.Value + ")";



            foreach (var item in createmos("Win32_LogicalDisk").Get())
            {
                listBox2.Items.Add(item["DeviceID"].ToString() + " "
                    + Math.Round(Convert.ToDouble(item["FreeSpace"]) / Math.Pow(1024, 3)) + "GB(FreeSpace)  --  "
                    + Math.Round(Convert.ToDouble(item["Size"]) / Math.Pow(1024, 3)) + " GB(Total)");
                freespacedisk += "|| " + item["DeviceID"].ToString() + " "
                    + Math.Round(Convert.ToDouble(item["FreeSpace"]) / Math.Pow(1024, 3))
                    + "(FreeSpace)GB  --  " + Math.Round(Convert.ToDouble(item["Size"]) / Math.Pow(1024, 3)) + " (Total)GB" + " || ";
            }
            string memall = " ";


            foreach (var item in createmos("Win32_PhysicalMemory").Get())
            {
                totalram += Convert.ToDouble(item["Capacity"]);
                label4.Text = (totalram / (Math.Pow(1024, 3))).ToString() + " GB//";
                memall += (Convert.ToDouble(item["Capacity"]) / (Math.Pow(1024, 3))).ToString() + "+";
                ramid += item["PartNumber"].ToString().Trim() + " // ";
            }
            label4.Text += memall.Trim();
            label4.Text = label4.Text.Remove(label4.Text.Length - 1);

            
            QueryPropertyResult formfactor = structures.GetProperty(DmiProperty.MemoryDevice.FormFactor);
            label4.Text += "//" + formfactor.Result.Value;
            
            QueryPropertyResult memorytype = structures.GetProperty(DmiProperty.MemoryDevice.MemoryType);
            label4.Text += "//" + memorytype.Result.Value;




            foreach (var item in createmos("Win32_PhysicalMemoryArray").Get())
            {
                ramrate = item["MemoryDevices"].ToString();
            }

            string ramlabel = "(" + createmos("Win32_PhysicalMemory").Get().Count + " of " + ramrate + " slot is full)";
            label4.Text += ramlabel;

            foreach (var item in createmos("Win32_BIOS").Get())
            {
                label6.Text = item["SerialNumber"].ToString();
            }

            if (createmos("Win32_VideoController", 2).Get().Count == 0)
            {
                foreach (var item in createmos("Win32_VideoController", 1).Get())
                {
                    label2.Text = item["Description"].ToString();
                }
                label3.Text = "Not found!";
            }
            else
            {
                foreach (var item in createmos("Win32_VideoController", 2).Get())
                {
                    label3.Text = item["Description"].ToString();
                }

                foreach (var item in createmos("Win32_VideoController", 1).Get())
                {
                    label2.Text = item["Description"].ToString();
                }
            }
            try
            {
                using (var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                using (var key = hklm.OpenSubKey("SOFTWARE\\Microsoft\\Office\\ClickToRun\\Configuration"))
                    if (key != null) { label10.Text = key.GetValue("ProductReleaseIds").ToString(); }
                    else
                    {
                        label10.Text = "Not Found!";
                    }

            }
            catch (Exception)
            {

                using (var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
                using (var key = hklm.OpenSubKey("SOFTWARE\\Microsoft\\Office\\ClickToRun\\Configuration"))
                    if (key != null) { label10.Text = key.GetValue("ProductReleaseIds").ToString(); }
                    else
                    {
                        label10.Text = "Not Found!";
                    }
            }





            string RunningOfficeVersion;

            if (new Office11().GetVersion() == null)
            {
                RunningOfficeVersion = "";
            }
            else
            {
                RunningOfficeVersion = new Office11().GetVersion();
            }

            if (new Office12().GetVersion() == null)
            {
                RunningOfficeVersion += "";
            }
            else
            {
                RunningOfficeVersion += ", " + new Office12().GetVersion();
            }

            if (new Office14().GetVersion() == null)
            {
                RunningOfficeVersion += "";
            }
            else
            {
                RunningOfficeVersion += ", " + new Office14().GetVersion();
            }

            if (new Office15().GetVersion() == null)
            {
                RunningOfficeVersion += "";
            }
            else
            {
                RunningOfficeVersion += ", " + new Office15().GetVersion();
            }

            if (new Office16().GetVersion() == null)
            {
                RunningOfficeVersion += "";
            }
            else
            {
                RunningOfficeVersion += ", " + new Office16().GetVersion();
            }

            if (new Office17().GetVersion() == null)
            {
                RunningOfficeVersion += "";
            }
            else
            {
                RunningOfficeVersion += ", " + new Office17().GetVersion();
            }

            if (RunningOfficeVersion == String.Empty)

            { RunningOfficeVersion = ".not found!"; }


            label10.Text += "(" + RunningOfficeVersion.Remove(0, 1).Trim() + ")";

        }

        private void button1_Click(object sender, EventArgs e)
        {

            conn.Open();

            SqlCommand date = new SqlCommand("select convert(varchar, getdate(), 103)", conn);

            var dt = date.ExecuteScalar().ToString();

            if (dt == null)
            { dt = "Not defined"; }

            if (ramid == null)
            { ramid = "Not defined"; }


            date.ExecuteNonQuery();


            SqlCommand query = new SqlCommand("select * from sys_info where serial_number=@p1 and hostname=@p2 and model=@p3", conn);
            query.Parameters.AddWithValue("@p1", label6.Text);
            query.Parameters.AddWithValue("@p2", label9.Text);
            query.Parameters.AddWithValue("@p3", label5.Text);

            SqlDataReader dr = query.ExecuteReader();

            if (dr.Read() == false)
            {

                SqlCommand cmd = new SqlCommand("insert into sys_info " +
                    "(model, serial_number, gpu1, gpu2, cpu, ram, storage, freespace, os, domain, hostname, officeversion, date, note, manufacturer) " +
                    "values (@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p12,@p13,@p14,@p15)", conn);

                cmd.Parameters.AddWithValue("@p1", label5.Text);

                cmd.Parameters.AddWithValue("@p2", label6.Text);

                cmd.Parameters.AddWithValue("@p3", label2.Text);

                cmd.Parameters.AddWithValue("@p4", label3.Text);

                cmd.Parameters.AddWithValue("@p5", label1.Text);

                cmd.Parameters.AddWithValue("@p6", label4.Text + " // " + ramid.Trim());

                if (storagedevices == String.Empty)
                {
                    cmd.Parameters.AddWithValue("@p7", "not defined");
                }
                else
                {
                    cmd.Parameters.AddWithValue("@p7", storagedevices);

                }

                cmd.Parameters.AddWithValue("@p8", freespacedisk);

                cmd.Parameters.AddWithValue("@p9", label8.Text);

                cmd.Parameters.AddWithValue("@p10", label8.Text);

                cmd.Parameters.AddWithValue("@p11", label9.Text);

                cmd.Parameters.AddWithValue("@p12", label10.Text);

                cmd.Parameters.AddWithValue("@p13", dt);

                cmd.Parameters.AddWithValue("@p15", label18.Text);

                if (richTextBox1.Text == String.Empty)
                {
                    cmd.Parameters.AddWithValue("@p14", "no note");
                }
                else
                {
                    cmd.Parameters.AddWithValue("@p14", richTextBox1.Text);
                }

                dr.Close();
                cmd.ExecuteNonQuery();
                conn.Close();

                if (textBox1.Text.Trim() == String.Empty)
                {
                    label14.ForeColor = System.Drawing.Color.Orange;
                    label14.Text = "Data is entered but e-mail not sent.";
                }
                else
                {
                    try
                    {
                        SendEmailUsingGmail(textBox1.Text);
                        label14.ForeColor = System.Drawing.Color.Green;
                        label14.Text = "Data is entered, e-mail sent.";
                    }
                    catch (Exception)
                    {
                        label14.ForeColor = System.Drawing.Color.Red;
                        label14.Text = "Data is entered, e-mail was incorrect";
                    }
                }
            }
            else
            {
                if (textBox1.Text.Trim() == String.Empty)
                {
                    label14.ForeColor = System.Drawing.Color.Red;
                    label14.Text = "Data is already in database.";
                }
                else
                {
                    try
                    {
                        SendEmailUsingGmail(textBox1.Text);
                        label14.ForeColor = System.Drawing.Color.Red;
                        label14.Text = "Data is already in database and e-mail sent again.";
                    }
                    catch (Exception)
                    {
                        label14.ForeColor = System.Drawing.Color.Red;
                        label14.Text = "Data is already in database but e-mail was incorrect.";
                    }
                }
            }
            conn.Close();
        }
    }
    public class Office17
    {

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]

        static extern uint RegOpenKeyEx(UIntPtr hKey, string lpSubKey, uint ulOptions, int samDesired, out int phkResult);
        [DllImport("Advapi32.dll")]
        static extern uint RegCloseKey(int hKey);
        [DllImport("advapi32.dll", EntryPoint = "RegQueryValueEx")]
        static extern int RegQueryValueEx(int hKey, string lpValueName, int lpReserved, ref uint lpType,
            System.Text.StringBuilder lpData, ref uint lpcbData);
        private static UIntPtr HKEY_LOCAL_MACHINE = new UIntPtr(0x80000002u);

        private Dictionary<string, string> LatestVersions = new Dictionary<string, string>();

        public Office17()
        {
            LatestVersions.Add("17.0", "Office 2019");

        }

        private string GetVersionNumberFromRegistry()
        {
            string regVersion;

            regVersion = GetVersionNumberFromRegistry("SOFTWARE\\Microsoft\\Office\\");
            if (regVersion == null)
                regVersion = GetVersionNumberFromRegistry("SOFTWARE\\Wow6432Node\\Microsoft\\Office\\");

            return regVersion;
        }
        private string GetVersionNumberFromRegistry(string key)
        {
            string version = null;
            foreach (string VerNo in LatestVersions.Keys)
            {
                string offPath = Reg64(HKEY_LOCAL_MACHINE, key + VerNo + "\\Excel\\InstallRoot", "Path");
                if (offPath != null)
                {
                    version = VerNo;
                    break;
                }
            }
            return version;
        }
        public string GetVersion()
        {
            string versionFromReg = GetVersionNumberFromRegistry();
            if (versionFromReg == null)
            { return null; }
            string versionInstalled = LatestVersions[versionFromReg];

            bool? Office64BitFromReg = Off64Bit("SOFTWARE\\Microsoft\\Office\\", versionFromReg);

            if (Office64BitFromReg == null)
                Office64BitFromReg = Off64Bit("SOFTWARE\\Wow6432Node\\Microsoft\\Office\\", versionFromReg);
            if (Office64BitFromReg.HasValue && Office64BitFromReg.Value)
                versionInstalled += " 64 bit";
            else if (Office64BitFromReg.HasValue && !Office64BitFromReg.Value)
                versionInstalled += " 32 bit";
            else
            {
                versionInstalled += " Unknown bit";
            }

            return versionInstalled;
        }

        private bool? Off64Bit(string key, string version)
        {
            bool? Office64BitFromReg = null;
            string Bitness = Reg64(HKEY_LOCAL_MACHINE, key + version + "\\Outlook", "Bitness");
            if (Bitness == "x86")
                Office64BitFromReg = false;
            else if ((Bitness == "x64"))
                Office64BitFromReg = true;
            return Office64BitFromReg;
        }

        private string Reg64(UIntPtr parent, string key, string prop)
        {
            int ikey = 0;
            int bit36_64 = 0x0100;
            int query = 0x0001;
            try
            {
                uint res = RegOpenKeyEx(HKEY_LOCAL_MACHINE, key, 0, query | bit36_64, out ikey);
                if (0 != res) return null;
                uint type = 0;
                uint data = 1024;
                StringBuilder buffer = new StringBuilder(1024);
                RegQueryValueEx(ikey, prop, 0, ref type, buffer, ref data);
                string ver = buffer.ToString();
                return ver;
            }
            finally
            {
                if (0 != ikey) RegCloseKey(ikey);
            }
        }
    }
    public class Office16
    {

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]

        static extern uint RegOpenKeyEx(UIntPtr hKey, string lpSubKey, uint ulOptions, int samDesired, out int phkResult);
        [DllImport("Advapi32.dll")]
        static extern uint RegCloseKey(int hKey);
        [DllImport("advapi32.dll", EntryPoint = "RegQueryValueEx")]
        static extern int RegQueryValueEx(int hKey, string lpValueName, int lpReserved, ref uint lpType,
            System.Text.StringBuilder lpData, ref uint lpcbData);
        private static UIntPtr HKEY_LOCAL_MACHINE = new UIntPtr(0x80000002u);

        private Dictionary<string, string> LatestVersions = new Dictionary<string, string>();

        public Office16()
        {
            LatestVersions.Add("16.0", "Office 2016 veya Office 365");

        }

        private string GetVersionNumberFromRegistry()
        {
            string regVersion;

            regVersion = GetVersionNumberFromRegistry("SOFTWARE\\Microsoft\\Office\\");
            if (regVersion == null)
                regVersion = GetVersionNumberFromRegistry("SOFTWARE\\Wow6432Node\\Microsoft\\Office\\");

            return regVersion;
        }
        private string GetVersionNumberFromRegistry(string key)
        {
            string version = null;
            foreach (string VerNo in LatestVersions.Keys)
            {
                string offPath = Reg64(HKEY_LOCAL_MACHINE, key + VerNo + "\\Excel\\InstallRoot", "Path");
                if (offPath != null)
                {
                    version = VerNo;
                    break;
                }
            }
            return version;
        }
        public string GetVersion()
        {
            string versionFromReg = GetVersionNumberFromRegistry();
            if (versionFromReg == null)
            { return null; }
            string versionInstalled = LatestVersions[versionFromReg];

            bool? Office64BitFromReg = Off64Bit("SOFTWARE\\Microsoft\\Office\\", versionFromReg);

            if (Office64BitFromReg == null)
                Office64BitFromReg = Off64Bit("SOFTWARE\\Wow6432Node\\Microsoft\\Office\\", versionFromReg);
            if (Office64BitFromReg.HasValue && Office64BitFromReg.Value)
                versionInstalled += " 64 bit";
            else if (Office64BitFromReg.HasValue && !Office64BitFromReg.Value)
                versionInstalled += " 32 bit";
            else
            {
                versionInstalled += " Unknown bit";
            }

            return versionInstalled;
        }

        private bool? Off64Bit(string key, string version)
        {
            bool? Office64BitFromReg = null;
            string Bitness = Reg64(HKEY_LOCAL_MACHINE, key + version + "\\Outlook", "Bitness");
            if (Bitness == "x86")
                Office64BitFromReg = false;
            else if ((Bitness == "x64"))
                Office64BitFromReg = true;
            return Office64BitFromReg;
        }

        private string Reg64(UIntPtr parent, string key, string prop)
        {
            int ikey = 0;
            int bit36_64 = 0x0100;
            int query = 0x0001;
            try
            {
                uint res = RegOpenKeyEx(HKEY_LOCAL_MACHINE, key, 0, query | bit36_64, out ikey);
                if (0 != res) return null;
                uint type = 0;
                uint data = 1024;
                StringBuilder buffer = new StringBuilder(1024);
                RegQueryValueEx(ikey, prop, 0, ref type, buffer, ref data);
                string ver = buffer.ToString();
                return ver;
            }
            finally
            {
                if (0 != ikey) RegCloseKey(ikey);
            }
        }
    }
    public class Office15
    {

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]

        static extern uint RegOpenKeyEx(UIntPtr hKey, string lpSubKey, uint ulOptions, int samDesired, out int phkResult);
        [DllImport("Advapi32.dll")]
        static extern uint RegCloseKey(int hKey);
        [DllImport("advapi32.dll", EntryPoint = "RegQueryValueEx")]
        static extern int RegQueryValueEx(int hKey, string lpValueName, int lpReserved, ref uint lpType,
            System.Text.StringBuilder lpData, ref uint lpcbData);
        private static UIntPtr HKEY_LOCAL_MACHINE = new UIntPtr(0x80000002u);

        private Dictionary<string, string> LatestVersions = new Dictionary<string, string>();

        public Office15()
        {
            LatestVersions.Add("15.0", "Office 2013");

        }

        private string GetVersionNumberFromRegistry()
        {
            string regVersion;

            regVersion = GetVersionNumberFromRegistry("SOFTWARE\\Microsoft\\Office\\");
            if (regVersion == null)
                regVersion = GetVersionNumberFromRegistry("SOFTWARE\\Wow6432Node\\Microsoft\\Office\\");

            return regVersion;
        }
        private string GetVersionNumberFromRegistry(string key)
        {
            string version = null;
            foreach (string VerNo in LatestVersions.Keys)
            {
                string offPath = Reg64(HKEY_LOCAL_MACHINE, key + VerNo + "\\Excel\\InstallRoot", "Path");
                if (offPath != null)
                {
                    version = VerNo;
                    break;
                }
            }
            return version;
        }
        public string GetVersion()
        {
            string versionFromReg = GetVersionNumberFromRegistry();
            if (versionFromReg == null)
            { return null; }
            string versionInstalled = LatestVersions[versionFromReg];

            bool? Office64BitFromReg = Off64Bit("SOFTWARE\\Microsoft\\Office\\", versionFromReg);

            if (Office64BitFromReg == null)
                Office64BitFromReg = Off64Bit("SOFTWARE\\Wow6432Node\\Microsoft\\Office\\", versionFromReg);
            if (Office64BitFromReg.HasValue && Office64BitFromReg.Value)
                versionInstalled += " 64 bit";
            else if (Office64BitFromReg.HasValue && !Office64BitFromReg.Value)
                versionInstalled += " 32 bit";
            else
            {
                versionInstalled += " Unknown bit";
            }

            return versionInstalled;
        }

        private bool? Off64Bit(string key, string version)
        {
            bool? Office64BitFromReg = null;
            string Bitness = Reg64(HKEY_LOCAL_MACHINE, key + version + "\\Outlook", "Bitness");
            if (Bitness == "x86")
                Office64BitFromReg = false;
            else if ((Bitness == "x64"))
                Office64BitFromReg = true;
            return Office64BitFromReg;
        }

        private string Reg64(UIntPtr parent, string key, string prop)
        {
            int ikey = 0;
            int bit36_64 = 0x0100;
            int query = 0x0001;
            try
            {
                uint res = RegOpenKeyEx(HKEY_LOCAL_MACHINE, key, 0, query | bit36_64, out ikey);
                if (0 != res) return null;
                uint type = 0;
                uint data = 1024;
                StringBuilder buffer = new StringBuilder(1024);
                RegQueryValueEx(ikey, prop, 0, ref type, buffer, ref data);
                string ver = buffer.ToString();
                return ver;
            }
            finally
            {
                if (0 != ikey) RegCloseKey(ikey);
            }
        }
    }
    public class Office14
    {

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]

        static extern uint RegOpenKeyEx(UIntPtr hKey, string lpSubKey, uint ulOptions, int samDesired, out int phkResult);
        [DllImport("Advapi32.dll")]
        static extern uint RegCloseKey(int hKey);
        [DllImport("advapi32.dll", EntryPoint = "RegQueryValueEx")]
        static extern int RegQueryValueEx(int hKey, string lpValueName, int lpReserved, ref uint lpType,
            System.Text.StringBuilder lpData, ref uint lpcbData);
        private static UIntPtr HKEY_LOCAL_MACHINE = new UIntPtr(0x80000002u);

        private Dictionary<string, string> LatestVersions = new Dictionary<string, string>();

        public Office14()
        {
            LatestVersions.Add("14.0", "Office 2010");

        }

        private string GetVersionNumberFromRegistry()
        {
            string regVersion;

            regVersion = GetVersionNumberFromRegistry("SOFTWARE\\Microsoft\\Office\\");
            if (regVersion == null)
                regVersion = GetVersionNumberFromRegistry("SOFTWARE\\Wow6432Node\\Microsoft\\Office\\");

            return regVersion;
        }
        private string GetVersionNumberFromRegistry(string key)
        {
            string version = null;
            foreach (string VerNo in LatestVersions.Keys)
            {
                string offPath = Reg64(HKEY_LOCAL_MACHINE, key + VerNo + "\\Excel\\InstallRoot", "Path");
                if (offPath != null)
                {
                    version = VerNo;
                    break;
                }
            }
            return version;
        }
        public string GetVersion()
        {
            string versionFromReg = GetVersionNumberFromRegistry();
            if (versionFromReg == null)
            { return null; }
            string versionInstalled = LatestVersions[versionFromReg];

            bool? Office64BitFromReg = Off64Bit("SOFTWARE\\Microsoft\\Office\\", versionFromReg);

            if (Office64BitFromReg == null)
                Office64BitFromReg = Off64Bit("SOFTWARE\\Wow6432Node\\Microsoft\\Office\\", versionFromReg);
            if (Office64BitFromReg.HasValue && Office64BitFromReg.Value)
                versionInstalled += " 64 bit";
            else if (Office64BitFromReg.HasValue && !Office64BitFromReg.Value)
                versionInstalled += " 32 bit";
            else
            {
                versionInstalled += " Unknown bit";
            }

            return versionInstalled;
        }

        private bool? Off64Bit(string key, string version)
        {
            bool? Office64BitFromReg = null;
            string Bitness = Reg64(HKEY_LOCAL_MACHINE, key + version + "\\Outlook", "Bitness");
            if (Bitness == "x86")
                Office64BitFromReg = false;
            else if ((Bitness == "x64"))
                Office64BitFromReg = true;
            return Office64BitFromReg;
        }

        private string Reg64(UIntPtr parent, string key, string prop)
        {
            int ikey = 0;
            int bit36_64 = 0x0100;
            int query = 0x0001;
            try
            {
                uint res = RegOpenKeyEx(HKEY_LOCAL_MACHINE, key, 0, query | bit36_64, out ikey);
                if (0 != res) return null;
                uint type = 0;
                uint data = 1024;
                StringBuilder buffer = new StringBuilder(1024);
                RegQueryValueEx(ikey, prop, 0, ref type, buffer, ref data);
                string ver = buffer.ToString();
                return ver;
            }
            finally
            {
                if (0 != ikey) RegCloseKey(ikey);
            }
        }
    }
    public class Office12
    {

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]

        static extern uint RegOpenKeyEx(UIntPtr hKey, string lpSubKey, uint ulOptions, int samDesired, out int phkResult);
        [DllImport("Advapi32.dll")]
        static extern uint RegCloseKey(int hKey);
        [DllImport("advapi32.dll", EntryPoint = "RegQueryValueEx")]
        static extern int RegQueryValueEx(int hKey, string lpValueName, int lpReserved, ref uint lpType,
            System.Text.StringBuilder lpData, ref uint lpcbData);
        private static UIntPtr HKEY_LOCAL_MACHINE = new UIntPtr(0x80000002u);

        private Dictionary<string, string> LatestVersions = new Dictionary<string, string>();

        public Office12()
        {
            LatestVersions.Add("12.0", "Office 2007");

        }

        private string GetVersionNumberFromRegistry()
        {
            string regVersion;

            regVersion = GetVersionNumberFromRegistry("SOFTWARE\\Microsoft\\Office\\");
            if (regVersion == null)
                regVersion = GetVersionNumberFromRegistry("SOFTWARE\\Wow6432Node\\Microsoft\\Office\\");

            return regVersion;
        }
        private string GetVersionNumberFromRegistry(string key)
        {
            string version = null;
            foreach (string VerNo in LatestVersions.Keys)
            {
                string offPath = Reg64(HKEY_LOCAL_MACHINE, key + VerNo + "\\Excel\\InstallRoot", "Path");
                if (offPath != null)
                {
                    version = VerNo;
                    break;
                }
            }
            return version;
        }
        public string GetVersion()
        {
            string versionFromReg = GetVersionNumberFromRegistry();
            if (versionFromReg == null)
            { return null; }
            string versionInstalled = LatestVersions[versionFromReg];

            bool? Office64BitFromReg = Off64Bit("SOFTWARE\\Microsoft\\Office\\", versionFromReg);

            if (Office64BitFromReg == null)
                Office64BitFromReg = Off64Bit("SOFTWARE\\Wow6432Node\\Microsoft\\Office\\", versionFromReg);
            if (Office64BitFromReg.HasValue && Office64BitFromReg.Value)
                versionInstalled += " 64 bit";
            else if (Office64BitFromReg.HasValue && !Office64BitFromReg.Value)
                versionInstalled += " 32 bit";
            else
            {
                versionInstalled += " Unknown bit";
            }

            return versionInstalled;
        }

        private bool? Off64Bit(string key, string version)
        {
            bool? Office64BitFromReg = null;
            string Bitness = Reg64(HKEY_LOCAL_MACHINE, key + version + "\\Outlook", "Bitness");
            if (Bitness == "x86")
                Office64BitFromReg = false;
            else if ((Bitness == "x64"))
                Office64BitFromReg = true;
            return Office64BitFromReg;
        }

        private string Reg64(UIntPtr parent, string key, string prop)
        {
            int ikey = 0;
            int bit36_64 = 0x0100;
            int query = 0x0001;
            try
            {
                uint res = RegOpenKeyEx(HKEY_LOCAL_MACHINE, key, 0, query | bit36_64, out ikey);
                if (0 != res) return null;
                uint type = 0;
                uint data = 1024;
                StringBuilder buffer = new StringBuilder(1024);
                RegQueryValueEx(ikey, prop, 0, ref type, buffer, ref data);
                string ver = buffer.ToString();
                return ver;
            }
            finally
            {
                if (0 != ikey) RegCloseKey(ikey);
            }
        }
    }
    public class Office11
    {

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]

        static extern uint RegOpenKeyEx(UIntPtr hKey, string lpSubKey, uint ulOptions, int samDesired, out int phkResult);
        [DllImport("Advapi32.dll")]
        static extern uint RegCloseKey(int hKey);
        [DllImport("advapi32.dll", EntryPoint = "RegQueryValueEx")]
        static extern int RegQueryValueEx(int hKey, string lpValueName, int lpReserved, ref uint lpType,
            System.Text.StringBuilder lpData, ref uint lpcbData);
        private static UIntPtr HKEY_LOCAL_MACHINE = new UIntPtr(0x80000002u);

        private Dictionary<string, string> LatestVersions = new Dictionary<string, string>();

        public Office11()
        {
            LatestVersions.Add("11.0", "Office 2003");

        }

        private string GetVersionNumberFromRegistry()
        {
            string regVersion;

            regVersion = GetVersionNumberFromRegistry("SOFTWARE\\Microsoft\\Office\\");
            if (regVersion == null)
                regVersion = GetVersionNumberFromRegistry("SOFTWARE\\Wow6432Node\\Microsoft\\Office\\");

            return regVersion;
        }
        private string GetVersionNumberFromRegistry(string key)
        {
            string version = null;
            foreach (string VerNo in LatestVersions.Keys)
            {
                string offPath = Reg64(HKEY_LOCAL_MACHINE, key + VerNo + "\\Excel\\InstallRoot", "Path");
                if (offPath != null)
                {
                    version = VerNo;
                    break;
                }
            }
            return version;
        }
        public string GetVersion()
        {
            string versionFromReg = GetVersionNumberFromRegistry();
            if (versionFromReg == null)
            { return null; }
            string versionInstalled = LatestVersions[versionFromReg];

            bool? Office64BitFromReg = Off64Bit("SOFTWARE\\Microsoft\\Office\\", versionFromReg);

            if (Office64BitFromReg == null)
                Office64BitFromReg = Off64Bit("SOFTWARE\\Wow6432Node\\Microsoft\\Office\\", versionFromReg);
            if (Office64BitFromReg.HasValue && Office64BitFromReg.Value)
                versionInstalled += " 64 bit";
            else if (Office64BitFromReg.HasValue && !Office64BitFromReg.Value)
                versionInstalled += " 32 bit";
            else
            {
                versionInstalled += " Unknown bit";
            }

            return versionInstalled;
        }

        private bool? Off64Bit(string key, string version)
        {
            bool? Office64BitFromReg = null;
            string Bitness = Reg64(HKEY_LOCAL_MACHINE, key + version + "\\Outlook", "Bitness");
            if (Bitness == "x86")
                Office64BitFromReg = false;
            else if ((Bitness == "x64"))
                Office64BitFromReg = true;
            return Office64BitFromReg;
        }

        private string Reg64(UIntPtr parent, string key, string prop)
        {
            int ikey = 0;
            int bit36_64 = 0x0100;
            int query = 0x0001;
            try
            {
                uint res = RegOpenKeyEx(HKEY_LOCAL_MACHINE, key, 0, query | bit36_64, out ikey);
                if (0 != res) return null;
                uint type = 0;
                uint data = 1024;
                StringBuilder buffer = new StringBuilder(1024);
                RegQueryValueEx(ikey, prop, 0, ref type, buffer, ref data);
                string ver = buffer.ToString();
                return ver;
            }
            finally
            {
                if (0 != ikey) RegCloseKey(ikey);
            }
        }
    }
}
