using System.Windows.Forms;
using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using System.Threading;
using System.Net;
using System.Xml;
using System.Reflection;
using System.ComponentModel;
using BadouSquidClient;
using System.Text.RegularExpressions;
using System.Collections;
using squidproxy;

//using System.Management;
namespace pac
{
    public partial class Form1 : Form
    {

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFileName);
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("Kernel32.dll")]
        private extern static int GetPrivateProfileSectionNamesA(byte[] buffer, int iLen, string fileName);

        [DllImport("wininet.dll")]
        public static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);
        public const int INTERNET_OPTION_SETTINGS_CHANGED = 39;
        public const int INTERNET_OPTION_REFRESH = 37;
        static bool _settingsReturn, _refreshReturn;
        string globalURL;
        public string inipath;
        string PacURL;



        public static void NotifyIE()
        {
            // These lines implement the Interface in the beginning of program 
            // They cause the OS to refresh the settings, causing IP to realy update
            _settingsReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
            _refreshReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);
        }

        public Form1()
        {
            InitializeComponent();
        }
        private void MarkStartup()
        {

            string path = Application.ExecutablePath;
            RegistryKey runKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
            string[] keyValueNames = runKey.GetValueNames();

            foreach (string keyValueName in keyValueNames)

            {

                try
                {
                    if (keyValueName == "squidproxy")

                    {
                        RebootSystem.Checked = true;
                        // toolStripMenuItem2.Text = "取消随系统启动";
                        //  runKey.Close();
                        // runKey.DeleteValue("GGA");
                    }


                    else if (keyValueName != "squidproxy")

                    {
                        RebootSystem.Checked = false;
                        //  toolStripMenuItem2.Text = "随系统启动";
                        //  runKey.Close();
                        //  runKey.SetValue("GGA", path);

                    }
                }



                catch (Exception e)
                {
                    MessageBox.Show(e.Message.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
            runKey.Close();
        }




        private void SetStartup()
        {

            string path = Application.ExecutablePath;
            RegistryKey runKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
            string[] keyValueNames = runKey.GetValueNames();

            foreach (string keyValueName in keyValueNames)

            {

                try
                {
                    if (keyValueName == "squidproxy")

                    {
                        //  runKey.Close();
                        runKey.DeleteValue("squidproxy");
                        //  toolStripMenuItem2.Text = "随系统启动";
                        RebootSystem.Checked = false;


                    }


                    else if (keyValueName != "squidproxy")

                    {
                        //  runKey.Close();
                        runKey.SetValue("squidproxy", path);
                        //   toolStripMenuItem2.Text = "取消随系统启动";
                        RebootSystem.Checked = true;

                    }
                }



                catch (Exception e)
                {
                    MessageBox.Show(e.Message.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
            runKey.Close();
        }

        private void RegularURL(string PacURL)


        {

            string p = @"(http|https)://(?<domain>[^(:|/]*)";
            Regex reg = new Regex(p, RegexOptions.IgnoreCase);
            Match m = reg.Match(PacURL);
            globalURL = m.Groups["domain"].Value;

            //  MessageBox.Show(Result);
        }



        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            // MessageBox.Show("Download completed!");

            notifyIcon1.Visible = true;
            notifyIcon1.ShowBalloonTip(3000, "更新提示:", "下载完成,请重启客户端", ToolTipIcon.Info);
        }
        private void CheckUpdate()

        {

            // version info from xml file  
            Version newVersion = null;
            // and in this variable we will put the url we  
            // would like to open so that the user can  
            // download the new version  
            // it can be a homepage or a direct  
            // link to zip/exe file  
            string url = "";
            XmlTextReader reader = null;
            try
            {
                // provide the XmlTextReader with the URL of  
                // our xml document  
                var StrxmlURL = squidproxy.Properties.Resources.xmlURL;
                // MessageBox.Show(StrxmlURL);

                reader = new XmlTextReader(StrxmlURL);
                // simply (and easily) skip the junk at the beginning  
                reader.MoveToContent();
                // internal - as the XmlTextReader moves only  
                // forward, we save current xml element name  
                // in elementName variable. When we parse a  
                // text node, we refer to elementName to check  
                // what was the node name  
                string elementName = "";
                // we check if the xml starts with a proper  
                // "ourfancyapp" element node  
                if ((reader.NodeType == XmlNodeType.Element) &&
                    (reader.Name == "SquidproxyAPP"))
                {
                    while (reader.Read())
                    {
                        // when we find an element node,  
                        // we remember its name  
                        if (reader.NodeType == XmlNodeType.Element)
                            elementName = reader.Name;
                        else
                        {
                            // for text nodes...  
                            if ((reader.NodeType == XmlNodeType.Text) &&
                                (reader.HasValue))
                            {
                                // we check what the name of the node was  
                                switch (elementName)
                                {
                                    case "version":
                                        // thats why we keep the version info  
                                        // in xxx.xxx.xxx.xxx format  
                                        // the Version class does the  
                                        // parsing for us  
                                        newVersion = new Version(reader.Value);
                                        break;
                                    case "url":
                                        url = reader.Value;
                                        break;

                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }

            Version curVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            // compare the versions  
            if (curVersion.CompareTo(newVersion) < 0)
            {

                string message = "已经存在一个新的版本" + newVersion + ", 是否更新?";
                string caption = "更新提示";
                DialogResult result;
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                //  MessageBox.Show("new version is available");
                result = MessageBox.Show(message, caption, buttons);


                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    string path = System.Environment.CurrentDirectory;
                    // MessageBox.Show("为你下载新版");
                    string Downloadfilename = "SquidSquid" + newVersion + ".exe";
                    var myStringWebResource = squidproxy.Properties.Resources.myStringWebResource;

                    //System.Net.WebClient unreasonably slow 
                    //http://stackoverflow.com/questions/4415443/system-net-webclient-unreasonably-slow

                    WebClient webClient = new WebClient();
                    webClient.Proxy = null;
                    ServicePointManager.DefaultConnectionLimit = 25;
                    webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
                    //  webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
                    webClient.DownloadFileAsync(new Uri(myStringWebResource), @path + "/" + Downloadfilename);


                }

                else

                {

                    Console.ReadLine();


                    //     MessageBox.Show(path);
                }


            }
            else

            {
                notifyIcon1.Visible = true;
                notifyIcon1.ShowBalloonTip(3000, "版本提示:", "已经是最新版本", ToolTipIcon.Info);
            }



        }

        private void CancelProxySetting()
        {

            try
            {
                RegistryKey registry =
                    Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings",
                        true);
              
                    registry.SetValue("ProxyEnable", 0);
                    registry.SetValue("ProxyServer", "");
                    registry.SetValue("AutoConfigURL", "");
              
                //Set AutoDetectProxy Off
                IEAutoDetectProxy(false);
                NotifyIE();
                //Must Notify IE first, or the connections do not chanage
                CopyProxySettingFromLan();
            }
            catch (Exception e)
            {
      
                // TODO this should be moved into views
                MessageBox.Show("Failed to update registry");
            }
        }

        private void DeleteLANSettting()
        {
            RegistryKey loca = Registry.CurrentUser;
            RegistryKey run = loca.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Internet Settings");
            //  Software\Microsoft\\Windows\CurrentVersion\Internet Settings\Connections",



            string[] keyValueNames = run.GetValueNames();

            // run.Close();

            //bool result = false;

            foreach (string keyValueName in keyValueNames)

            {

                if (keyValueName == "ProxyServer")

                {
                    run.DeleteValue("ProxyServer");
                    // result = true;

                    break;

                }

                else

                {
                    Console.WriteLine("Test");

                }
                if (keyValueName == "AutoConfigURL")

                {
                    run.DeleteValue("AutoConfigURL");
                    break;

                }

                else

                {
                    Console.WriteLine("Test");
                }

            }


            try

            {
                run.SetValue("ProxyEnable", 0);
                loca.Close();
            }

            catch (Exception ee)

            {
                MessageBox.Show(ee.Message.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private static void IEAutoDetectProxy(bool set)
        {
            RegistryKey registry =
                Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\Connections",
                    true);
            byte[] defConnection = (byte[])registry.GetValue("DefaultConnectionSettings");
            byte[] savedLegacySetting = (byte[])registry.GetValue("SavedLegacySettings");
            if (set)
            {
                defConnection[8] = Convert.ToByte(defConnection[8] & 8);
                savedLegacySetting[8] = Convert.ToByte(savedLegacySetting[8] & 8);
            }
            else
            {
                defConnection[8] = Convert.ToByte(defConnection[8] & ~8);
                savedLegacySetting[8] = Convert.ToByte(savedLegacySetting[8] & ~8);
            }
            registry.SetValue("DefaultConnectionSettings", defConnection);
            registry.SetValue("SavedLegacySettings", savedLegacySetting);
        }

        private void AdslSetSquidProxy(string SquidGobal)

        {
            RegistryKey loca = Registry.CurrentUser;
            RegistryKey run = loca.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Internet Settings");
            //  Software\Microsoft\\Windows\CurrentVersion\Internet Settings\Connections",


            try

            {
                run.SetValue("ProxyEnable", 1);
                run.SetValue("ProxyServer", SquidGobal + ":25");
                run.SetValue("AutoConfigURL", "");

                NotifyIE();
                //Must Notify IE first, or the connections do not chanage

                IEAutoDetectProxy(false);
                //Must Notify IE first, or the connections do not chanage
                CopyProxySettingFromLan();

            }

            catch (Exception ee)

            {
                MessageBox.Show(ee.Message.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }



        }

        public static void SetNotifyIconText(NotifyIcon ni, string text)
        {
            if (text.Length >= 128) throw new ArgumentOutOfRangeException("Text limited to 127 characters");
            Type t = typeof(NotifyIcon);
            BindingFlags hidden = BindingFlags.NonPublic | BindingFlags.Instance;
            t.GetField("text", hidden).SetValue(ni, text);
            if ((bool)t.GetField("added", hidden).GetValue(ni))
                t.GetMethod("UpdateIcon", hidden).Invoke(ni, new object[] { true });
        }


      
        private static void CopyProxySettingFromLan()
        {
            RegistryKey registry =
                Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\Connections",
                    true);
            var defaultValue = registry.GetValue("DefaultConnectionSettings");
            try
            {
                var connections = registry.GetValueNames();
                foreach (String each in connections)
                {
                    if (!(each.Equals("DefaultConnectionSettings")
                        || each.Equals("LAN Connection")
                        || each.Equals("SavedLegacySettings")))
                    {
                        //set all the connections's proxy as the lan
                        registry.SetValue(each, defaultValue);
                    }
                }
                NotifyIE();
                //Must Notify IE first, or the connections do not chanage

            }
            catch (IOException e)
            {
                MessageBox.Show(e.Message.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetSquidProxy1(string ServerAdress)
        {
            RegistryKey loca = Registry.CurrentUser;
            RegistryKey run = loca.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Internet Settings");
            //  Software\Microsoft\\Windows\CurrentVersion\Internet Settings\Connections",


            try

            {

                run.SetValue("ProxyEnable", 0);
                run.SetValue("ProxyServer", "");
                run.SetValue("AutoConfigURL", ServerAdress);

                NotifyIE();
                //Must Notify IE first, or the connections do not chanage

                IEAutoDetectProxy(false);
                //Must Notify IE first, or the connections do not chanage
                CopyProxySettingFromLan();

                //Must Notify IE first, or the connections do not chanage

                loca.Close();
            }

            catch (Exception ee)

            {
                MessageBox.Show(ee.Message.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        public string ReadINI(string section, string key, string fileName)
        {
            StringBuilder temp = new StringBuilder(1024);
            GetPrivateProfileString(section, key, "NullValue", temp, 1024, fileName);
            return temp.ToString();
        }



        void MenuClicked(object sender, EventArgs e)
        {

            //    MessageBox.Show(((sender as ToolStripMenuItem).Text));
            //   SetSquidProxy1((sender as ToolStripMenuItem).Text);
            string path = System.Environment.CurrentDirectory;
            string GlobalStatus = ReadINI("ProxyMode", "Global", @path + "\\" + "SquidConfig.ini");
            string SmartStatus = ReadINI("ProxyMode", "Smart", @path + "\\" + "SquidConfig.ini");

            if (GlobalStatus == "1")

            {
                PacURL = ((sender as ToolStripMenuItem).Text);

                RegularURL(PacURL);

                AdslSetSquidProxy(globalURL);
                string NotifyShowContent = "Squid V2.0.0.12" + Environment.NewLine + "代理模式: 全局代理" + Environment.NewLine + "服务器:" + globalURL;
                char[] myChar = NotifyShowContent.ToCharArray();
                SetNotifyIconText(notifyIcon1, new string(myChar));

            }
            else if (SmartStatus == "1")

            {
                //    MessageBox.Show(((sender as ToolStripMenuItem).Text));
                SetSquidProxy1((sender as ToolStripMenuItem).Text);

                string NotifyShowContent = "Squid V2.0.0.12" + Environment.NewLine + "代理模式: 智能代理" + Environment.NewLine + "服务器:" + ((sender as ToolStripMenuItem).Text);
                char[] myChar = NotifyShowContent.ToCharArray();

                SetNotifyIconText(notifyIcon1, new string(myChar));


            }

            IniWriteValue("DefaultStartup", "Server", ((sender as ToolStripMenuItem).Text));




        }

        void GlobalWriteInIClicked(object sender, EventArgs e)

        {
            IniWriteValue("ProxyMode", "Global", "1");
            IniWriteValue("ProxyMode", "Smart", "0");

            //   SetSquidProxy1((sender as ToolStripMenuItem).Text);
            string path = System.Environment.CurrentDirectory;
            string GlobalStatus = ReadINI("ProxyMode", "Global", @path + "\\" + "SquidConfig.ini");
            string SmartStatus = ReadINI("ProxyMode", "Smart", @path + "\\" + "SquidConfig.ini");

            if (GlobalStatus == "1")

            {
                string DefaultServer = ReadINI("DefaultStartup", "Server", @path + "\\" + "SquidConfig.ini");

                string PacURL = DefaultServer;

                RegularURL(PacURL);

                AdslSetSquidProxy(globalURL);

                string NotifyShowContent = "Squid V2.0.0.12" + Environment.NewLine + "代理模式: 全局" + Environment.NewLine + "服务器:" + globalURL;
                char[] myChar = NotifyShowContent.ToCharArray();

                SetNotifyIconText(notifyIcon1, new string(myChar));


            }
            else if (SmartStatus == "1")

            {
                string DefaultServer = ReadINI("DefaultStartup", "Server", @path + "\\" + "SquidConfig.ini");

                SetSquidProxy1(DefaultServer);

                string NotifyShowContent = "Squid V2.0.0.12" + Environment.NewLine + "代理模式: 智能" + Environment.NewLine + "服务器:" + DefaultServer;
                char[] myChar = NotifyShowContent.ToCharArray();

                SetNotifyIconText(notifyIcon1, new string(myChar));

            }


        }

        void SmartWriteINIClicked(object sender, EventArgs e)

        {

            IniWriteValue("ProxyMode", "Global", "0");
            IniWriteValue("ProxyMode", "Smart", "1");

            //   SetSquidProxy1((sender as ToolStripMenuItem).Text);
            string path = System.Environment.CurrentDirectory;
            string GlobalStatus = ReadINI("ProxyMode", "Global", @path + "\\" + "SquidConfig.ini");
            string SmartStatus = ReadINI("ProxyMode", "Smart", @path + "\\" + "SquidConfig.ini");

            if (GlobalStatus == "1")

            {
                string DefaultServer = ReadINI("DefaultStartup", "Server", @path + "\\" + "SquidConfig.ini");

                string PacURL = DefaultServer;

                RegularURL(PacURL);

                AdslSetSquidProxy(globalURL);

                // 
                string NotifyShowContent = "Squid V2.0.0.12" + Environment.NewLine + "代理模式: 全局" + Environment.NewLine + "服务器:" + globalURL;
                char[] myChar = NotifyShowContent.ToCharArray();

                SetNotifyIconText(notifyIcon1, new string(myChar));



            }
            else if (SmartStatus == "1")

            {

                string DefaultServer = ReadINI("DefaultStartup", "Server", @path + "\\" + "SquidConfig.ini");

                SetSquidProxy1(DefaultServer);



                string NotifyShowContent = "Squid V2.0.0.12" + Environment.NewLine + "代理模式: 智能" + Environment.NewLine + "服务器:" + DefaultServer;
                char[] myChar = NotifyShowContent.ToCharArray();

                SetNotifyIconText(notifyIcon1, new string(myChar));



            }


        }



        void LoadSetting(object sender, EventArgs e)
        {


            contextMenuStrip1.Items.Clear();
            //添加菜单一 
            ToolStripMenuItem AboutME;
            AboutME = AddContextMenu("关于", contextMenuStrip1.Items, null);

            AboutME.Click += AboutMe_Click;



            ToolStripMenuItem StartupSystem;
            StartupSystem = AddContextMenu("随系统启动", contextMenuStrip1.Items, null);

            StartupSystem.Click += RebootSystem_Click;


            ToolStripMenuItem CheckedUpdate;
            CheckedUpdate = AddContextMenu("检查更新", contextMenuStrip1.Items, null);


            CheckedUpdate.Click += CheckUpdate_Click;


            //ToolStripMenuItem subItem2;
            //subItem2 = AddContextMenu("重新加载配置", contextMenuStrip1.Items, new EventHandler(LoadSetting));\


            string path = System.Environment.CurrentDirectory;
            string IsFirstRun = ReadINI("DefaultStartup", "FirstRun", @path + "\\" + "SquidConfig.ini");

            if (IsFirstRun == "1")


            {


           }

            else

            {
           
                ToolStripMenuItem ProxyMode;
                ToolStripMenuItem subItem;

                ProxyMode = AddContextMenu("代理模式", contextMenuStrip1.Items, null);
                AddContextMenu("全局代理", ProxyMode.DropDownItems, new EventHandler(GlobalWriteInIClicked));
                AddContextMenu("智能代理", ProxyMode.DropDownItems, new EventHandler(SmartWriteINIClicked));



                //添加菜单一 
            
                subItem = AddContextMenu("服务器", contextMenuStrip1.Items, null);

                ToolStripMenuItem subItem1;
                subItem1 = AddContextMenu("-", contextMenuStrip1.Items, null);

                subItem1 = AddContextMenu("退出", contextMenuStrip1.Items, null);

                subItem1.Click += tested;


                string ServerindexString = ReadINI("DefaultStartup", "index", path + "\\" + "SquidConfig.ini");

                //  MessageBox.Show(ServerindexString);

                int ServerindexInt = int.Parse(ServerindexString);

                int j = 0;

                for (int i = 0; i < ServerindexInt; i++)
                {


                    j = j + 1;
                    string ServerAddress = ReadINI("User", "Server" + j.ToString(), path + "\\" + "SquidConfig.ini");

                    //   MessageBox.Show(ServerAddress);

                    //   EditServer.listBox1.Items.Add(ServerAddress);

                    AddContextMenu(ServerAddress, subItem.DropDownItems, new EventHandler(MenuClicked));




                }

                AddContextMenu("编辑服务器", subItem.DropDownItems, new EventHandler(EditServer));




            }


            //   contextMenuStrip1.Items.Clear();
            //  contextMenuStrip1.Items.Clear();

            // subItem.DropDownItems.Clear();
            //   SetSquidProxy1((sender as ToolStripMenuItem).Text);

            string GlobalStatus = ReadINI("ProxyMode", "Global", @path + "\\" + "SquidConfig.ini");
            string SmartStatus = ReadINI("ProxyMode", "Smart", @path + "\\" + "SquidConfig.ini");

            if (GlobalStatus == "1")

            {
                string DefaultServer = ReadINI("DefaultStartup", "Server", @path + "\\" + "SquidConfig.ini");

                string PacURL = DefaultServer;

                RegularURL(PacURL);

                AdslSetSquidProxy(globalURL);

                string NotifyShowContent = "Squid V2.0.0.12" + Environment.NewLine + "代理模式: 全局" + Environment.NewLine + "服务器:" + globalURL;
                char[] myChar = NotifyShowContent.ToCharArray();

                SetNotifyIconText(notifyIcon1, new string(myChar));


            }
            else if (SmartStatus == "1")

            {

                string DefaultServer = ReadINI("DefaultStartup", "Server", @path + "\\" + "SquidConfig.ini");

                string path1 = System.Environment.CurrentDirectory;

                string ServerindexString1 = @path1 + "\\" + "SquidConfig.ini";

                byte[] buffer = new byte[65535];
                int rel = GetPrivateProfileSectionNamesA(buffer, buffer.GetUpperBound(0), ServerindexString1);
                int iCnt, iPos;
                System.Collections.ArrayList arrayList = new ArrayList();
                string tmp;
                if (rel > 0)
                {
                    iCnt = 0; iPos = 0;
                    for (iCnt = 0; iCnt < rel; iCnt++)
                    {
                        if (buffer[iCnt] == 0x00)
                        {
                            tmp = System.Text.ASCIIEncoding.Default.GetString(buffer, iPos, iCnt - iPos).Trim();
                            iPos = iCnt + 1;
                            if (tmp != "")
                                arrayList.Add(tmp);
                            // MessageBox.Show(tmp);
                        }
                    }
                }


                bool exists = ((IList)arrayList).Contains("User");
                if (exists)
                {

                    string NotifyShowContent = "Squid V2.0.0.12" + Environment.NewLine + "代理模式: 智能" + Environment.NewLine + "服务器:" + DefaultServer;
                    char[] myChar = NotifyShowContent.ToCharArray();

                    SetNotifyIconText(notifyIcon1, new string(myChar));


                    // MessageBox.Show("test");
                    SetSquidProxy1(DefaultServer);

                }
                // 存在
                else
                // 不存在
                {
                    //  MessageBox.Show("no exists");

                    notifyIcon1.Text = "Squid V2.0.0.12" + Environment.NewLine + "代理模式: 智能" + Environment.NewLine + "服务器: 未加入";
                }

            }




        }

        public void IniWriteValue(string Section, string Key, string Value)
        {


            //  MessageBox.Show(inipath);
            string path = System.Environment.CurrentDirectory;
            inipath = path + "/" + "SquidConfig.ini";
            WritePrivateProfileString(Section, Key, Value, inipath);
        }





        public void prikazi()
        {
            MessageBox.Show("ok");
        }


        public void LoadServerMenu()

        {

            contextMenuStrip1.Items.Clear();

            //添加菜单一 
            ToolStripMenuItem AboutME;
            AboutME = AddContextMenu("关于", contextMenuStrip1.Items, null);

            AboutME.Click += AboutMe_Click;






            ToolStripMenuItem StartupSystem;
            StartupSystem = AddContextMenu("随系统启动", contextMenuStrip1.Items, null);

            StartupSystem.Click += RebootSystem_Click;


            ToolStripMenuItem CheckedUpdate;
            CheckedUpdate = AddContextMenu("检查更新", contextMenuStrip1.Items, null);


            CheckedUpdate.Click += CheckUpdate_Click;


            string path = System.Environment.CurrentDirectory;
            string IsFirstRun = ReadINI("DefaultStartup", "FirstRun", @path + "\\" + "SquidConfig.ini");

            if (IsFirstRun == "1")


            {
                ToolStripMenuItem ProxyMode;
                ProxyMode = AddContextMenu("代理模式", contextMenuStrip1.Items, null);
                AddContextMenu("全局代理", ProxyMode.DropDownItems, new EventHandler(GlobalWriteInIClicked));
                AddContextMenu("智能代理", ProxyMode.DropDownItems, new EventHandler(SmartWriteINIClicked));


                //添加菜单一 
                ToolStripMenuItem subItem;
                subItem = AddContextMenu("服务器", contextMenuStrip1.Items, null);


                string ServerindexString = ReadINI("DefaultStartup", "index", path + "\\" + "SquidConfig.ini");

                //  MessageBox.Show(ServerindexString);

                int ServerindexInt = int.Parse(ServerindexString);

                int j = 0;

                for (int i = 0; i < ServerindexInt; i++)
                {


                    j = j + 1;
                    string ServerAddress = ReadINI("User", "Server" + j.ToString(), path + "\\" + "SquidConfig.ini");

                    //   MessageBox.Show(ServerAddress);

                    //   EditServer.listBox1.Items.Add(ServerAddress);

                    AddContextMenu(ServerAddress, subItem.DropDownItems, new EventHandler(MenuClicked));




                }

                AddContextMenu("编辑服务器", subItem.DropDownItems, new EventHandler(EditServer));

            }

            else

            {
                ToolStripMenuItem ProxyMode;
                ProxyMode = AddContextMenu("代理模式", contextMenuStrip1.Items, null);
                AddContextMenu("全局代理", ProxyMode.DropDownItems, new EventHandler(GlobalWriteInIClicked));
                AddContextMenu("智能代理", ProxyMode.DropDownItems, new EventHandler(SmartWriteINIClicked));


                //添加菜单一 
                ToolStripMenuItem subItem;
                subItem = AddContextMenu("服务器", contextMenuStrip1.Items, null);


                string ServerindexString = ReadINI("DefaultStartup", "index", path + "\\" + "SquidConfig.ini");

                //  MessageBox.Show(ServerindexString);

                int ServerindexInt = int.Parse(ServerindexString);

                int j = 0;

                for (int i = 0; i < ServerindexInt; i++)
                {


                    j = j + 1;
                    string ServerAddress = ReadINI("User", "Server" + j.ToString(), path + "\\" + "SquidConfig.ini");

                    //   MessageBox.Show(ServerAddress);

                    //   EditServer.listBox1.Items.Add(ServerAddress);

                    AddContextMenu(ServerAddress, subItem.DropDownItems, new EventHandler(MenuClicked));




                }

                AddContextMenu("编辑服务器", subItem.DropDownItems, new EventHandler(EditServer));


            }

        
           

            //   contextMenuStrip1.Items.Clear();
            //  contextMenuStrip1.Items.Clear();

            // subItem.DropDownItems.Clear();

            string GlobalStatus = ReadINI("ProxyMode", "Global", @path + "\\" + "SquidConfig.ini");
            string SmartStatus = ReadINI("ProxyMode", "Smart", @path + "\\" + "SquidConfig.ini");

            if (GlobalStatus == "1")

            {
                string DefaultServer = ReadINI("DefaultStartup", "Server", @path + "\\" + "SquidConfig.ini");

                string PacURL = DefaultServer;

                RegularURL(PacURL);

                AdslSetSquidProxy(globalURL);


                string NotifyShowContent = "Squid V2.0.0.12" + Environment.NewLine + "代理模式: 全局" + Environment.NewLine + "服务器:" + globalURL;
                char[] myChar = NotifyShowContent.ToCharArray();

                SetNotifyIconText(notifyIcon1, new string(myChar));


            }
            else if (SmartStatus == "1")

            {

                string DefaultServer = ReadINI("DefaultStartup", "Server", @path + "\\" + "SquidConfig.ini");



                //   MessageBox.Show("存在ini");

                string path1 = System.Environment.CurrentDirectory;

                string ServerindexString1 = @path1 + "\\" + "SquidConfig.ini";

                byte[] buffer = new byte[65535];
                int rel = GetPrivateProfileSectionNamesA(buffer, buffer.GetUpperBound(0), ServerindexString1);
                int iCnt, iPos;
                System.Collections.ArrayList arrayList = new ArrayList();
                string tmp;
                if (rel > 0)
                {

                    iCnt = 0; iPos = 0;
                    for (iCnt = 0; iCnt < rel; iCnt++)
                    {
                        if (buffer[iCnt] == 0x00)
                        {
                            tmp = System.Text.ASCIIEncoding.Default.GetString(buffer, iPos, iCnt - iPos).Trim();
                            iPos = iCnt + 1;
                            if (tmp != "")
                                arrayList.Add(tmp);
                            // MessageBox.Show(tmp);
                        }
                    }
                }


                bool exists = ((IList)arrayList).Contains("User");
                if (exists)
                {
                    string NotifyShowContent = "Squid V2.0.0.12" + Environment.NewLine + "代理模式: 智能" + Environment.NewLine + "服务器:" + DefaultServer;
                    char[] myChar = NotifyShowContent.ToCharArray();



                    SetNotifyIconText(notifyIcon1, new string(myChar));

                    SetSquidProxy1(DefaultServer);

                }
                // 存在
                else
                // 不存在
                {
                    //  MessageBox.Show("no exists");
                 //   IniWriteValue("DefaultStartup", "FirstRun", "1");

                    notifyIcon1.Text = "Squid V2.0.0.12" + Environment.NewLine + "代理模式: 智能" + Environment.NewLine + "服务器: 未加入";
                }



            }



        }

        void EditServer(object sender, EventArgs e)
        {
            //EditServer form = new EditServer();
            //form.Show();

            EditServer form2 = new EditServer();

            //   form2.DisableButton += new EventHandler(form2_DisableButton);

            form2.DisableButton += new EventHandler(LoadSetting);

            form2.ShowDialog();

        }


        void tested(object sender, EventArgs e)
        {

            CancelProxySetting();
            notifyIcon1.Icon = null;
            notifyIcon1.Dispose();
            Application.DoEvents();
            System.Environment.Exit(0);

        }


        ToolStripMenuItem AddContextMenu(string text, ToolStripItemCollection cms, EventHandler callback)
        {
            if (text == "-")
            {
                ToolStripSeparator tsp = new ToolStripSeparator();
                cms.Add(tsp);

                return null;
            }
            else if (!string.IsNullOrEmpty(text))
            {
                ToolStripMenuItem tsmi = new ToolStripMenuItem(text);
                if (callback != null) tsmi.Click += callback;
                cms.Add(tsmi);

                return tsmi;
            }

            return null;
        }

        public void ReadSections()
        {

            string path = System.Environment.CurrentDirectory;

            string ServerindexString = @path + "\\" + "SquidConfig.ini";


            if (File.Exists(ServerindexString))

            {
                //   MessageBox.Show("存在ini");

                byte[] buffer = new byte[65535];
                int rel = GetPrivateProfileSectionNamesA(buffer, buffer.GetUpperBound(0), ServerindexString);
                int iCnt, iPos;
                System.Collections.ArrayList arrayList = new ArrayList();
                string tmp;
                if (rel > 0)
                {
                    iCnt = 0; iPos = 0;
                    for (iCnt = 0; iCnt < rel; iCnt++)
                    {
                        if (buffer[iCnt] == 0x00)
                        {
                            tmp = System.Text.ASCIIEncoding.Default.GetString(buffer, iPos, iCnt - iPos).Trim();
                            iPos = iCnt + 1;
                            if (tmp != "")
                                arrayList.Add(tmp);
                            // MessageBox.Show(tmp);
                        }
                    }
                }


                //foreach (string test in arrayList)
                //{

                //        MessageBox.Show(test);

                //}

                bool exists = ((IList)arrayList).Contains("User");
                if (exists)
                {
                    // MessageBox.Show("exists");

                }
                // 存在
                else
                // 不存在
                {
                    //  MessageBox.Show("no exists");


                    EditServer form2 = new EditServer();

                    //   form2.DisableButton += new EventHandler(form2_DisableButton);

                    form2.DisableButton += new EventHandler(LoadSetting);

                    form2.ShowDialog();
                }

            }

            else

            {
                //  MessageBox.Show("不存在ini");

                IniWriteValue("DefaultStartup", "index", "0");
                IniWriteValue("DefaultStartup", "FirstRun", "1");
                IniWriteValue("ProxyMode", "Global", "0");
                IniWriteValue("ProxyMode", "Smart", "1");


                EditServer form2 = new EditServer();

                //   form2.DisableButton += new EventHandler(form2_DisableButton);

                form2.DisableButton += new EventHandler(LoadSetting);

                form2.ShowDialog();

            }


        }


        private void Form1_Load(object sender, EventArgs e)
        {

            CheckUpdate();



            //ToolStripMenuItem subItem2;
            //subItem2 = AddContextMenu("重新加载配置", contextMenuStrip1.Items, new EventHandler(LoadSetting));


            ToolStripMenuItem subItem1;


            ReadSections();

            LoadServerMenu();// LoadServerMenu


            subItem1 = AddContextMenu("-", contextMenuStrip1.Items, null);

            subItem1 = AddContextMenu("退出", contextMenuStrip1.Items, null);

            subItem1.Click += tested;


            // setting up proxy
            //  SetSquidProxy1();
            //  Server2.Checked = true;


            // setting up proxy

            //  Server1.Checked = false;



            this.Hide();
            this.ShowInTaskbar = false;

            //Check Latest Version 

            //Check Latest Version 

            FormBorderStyle = FormBorderStyle.None;

            //mark the Menu while reboot status
            MarkStartup();


            //loading string which intrduce squid Technology
            //var squid = squidproxy.Properties.Resources.test;
            //richTextBox1.Text = squid;
            //  toolTip p = new toolTip();

            //p.showalways = true;

            //p.settooltip(pictureBox1, "地址");

            ToolTip toolTip1 = new ToolTip();

            // Set up the delays for the ToolTip.
            toolTip1.AutoPopDelay = 2000;
            toolTip1.InitialDelay = 500;
            toolTip1.ReshowDelay = 500;
            // Force the ToolTip text to be displayed whether or not the form is active.
            toolTip1.ShowAlways = true;

            // Set up the ToolTip text for the Button and Checkbox.
            toolTip1.SetToolTip(this.pictureBox3, "访问项目Github");
            toolTip1.SetToolTip(this.pictureBox5, "访问项目G+社区");
            toolTip1.SetToolTip(this.pictureBox7, "访问项目Twitter");
            //  toolTip1.SetToolTip(this.checkBox1, "My checkBox1");

        }

    

        private void RebootSystem_Click(object sender, EventArgs e)
        {
            SetStartup();
        }



        private void CheckUpdate_Click(object sender, EventArgs e)
        {
            CheckUpdate();
        }



        private void AboutMe_Click(object sender, EventArgs e)
        {
            this.Visible = true;
         //   this.WindowState = FormWindowState.Normal;

            this.notifyIcon1.Visible = true;

            // FadeOut(this, 0);
        }



        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.WindowState = FormWindowState.Minimized;
        }


        private void richTextBox1_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText); // call default browser  
        }

        private void button1_Click(object sender, EventArgs e)
        {
            EditServer frm = new EditServer();
            frm.Show();

        }



  

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            pictureBox1.Visible =  false;
            pictureBox2.Visible = true;

        }

        private void pictureBox2_MouseEnter(object sender, EventArgs e)
        {
            pictureBox1.Visible = true;
            pictureBox2.Visible = false;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.ShowInTaskbar = false;
        }

        private void pictureBox4_MouseEnter(object sender, EventArgs e)
        {
            pictureBox3.Visible = true;
            pictureBox4.Visible = false;
        }

        private void pictureBox3_MouseLeave(object sender, EventArgs e)
        {
            pictureBox3.Visible = false;
            pictureBox4.Visible = true;
        }

        private void pictureBox6_MouseEnter(object sender, EventArgs e)
        {
            pictureBox5.Visible = true;
            pictureBox6.Visible = false;
        }

        private void pictureBox5_MouseLeave(object sender, EventArgs e)
        {
            pictureBox5.Visible = false;
            pictureBox6.Visible = true;
        }

        private void pictureBox8_MouseEnter(object sender, EventArgs e)
        {
            pictureBox7.Visible = true;
            pictureBox8.Visible = false;
        }

        private void pictureBox7_MouseLeave(object sender, EventArgs e)
        {
            pictureBox7.Visible = false;
            pictureBox8.Visible = true;
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/squidproxy/squidproxy");
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://plus.google.com/communities/101513261063592651175");
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://twitter.com/squidgfw");
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                this.notifyIcon1.Visible = true;
            }
        }





    }



}

