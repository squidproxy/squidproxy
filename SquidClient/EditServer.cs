using pac;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace squidproxy
{



    public partial class EditServer : Form
    {

        public event EventHandler DisableButton;
        public event EventHandler SettingUpSmartProxy;


        private Form1 form1;

        int pictureIndex;

        private List<Image> girls = new List<Image>();
        //  private Timer timer = new Timer();
        private int index = 0;

        //  private static extern bool SetProcessDPIAware();
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="INIPath">文件路径</param>
        /// 
        public string inipath;

        string GrourpServer = null;

        private Form1 parentForm;



        private int operationState = 0;
        public EditServer()
        {
            this.parentForm = parentForm;
            //  SetProcessDPIAware();
            InitializeComponent();

        }


        public EditServer(Form1 form1)
            : this()
        {

            this.form1 = form1;
        }

        private void UpdateMemu()

        {
            string path = System.Environment.CurrentDirectory;
            string iniFilePath = path + "\\" + "SquidConfig.ini";

            Writeini writeini = new Writeini();
            writeini.IniParser(iniFilePath);


            ToolStripMenuItem subItem;
            //   subItem = AddContextMenu("服务器", contextMenuStrip1.Items, null);




            string ServerindexString = writeini.GetSetting("DefaultStartup", "index");

            //  string ServerindexString = ReadINI("DefaultStartup", "index", @path + "\\" + "SquidConfig.ini");



            //  MessageBox.Show(path + "\\" + "SquidConfig.ini");

            //    MessageBox.Show(ServerindexString);

            int ServerindexInt = int.Parse(ServerindexString);

            int j = 0;

            for (int i = 0; i < ServerindexInt; i++)
            {


                j = j + 1;



                string ServerAddress = writeini.GetSetting("User", "Server" + j.ToString());



                // string ServerAddress = ReadINI("User", "Server" + j.ToString(), @path + "\\" + "SquidConfig.ini");

                //  MessageBox.Show(ServerAddress);

                listBox1.Items.Add(ServerAddress);

                // AddContextMenu(ServerAddress, subItem.DropDownItems, new EventHandler(MenuClicked));

            }

        }
 
   

        private void button1_Click(object sender, System.EventArgs e)



        {

            string path = System.Environment.CurrentDirectory;

            string iniFilePath = path + "\\" + "SquidConfig.ini";

            Writeini writeini = new Writeini();

            writeini.IniParser(iniFilePath);


            //  MessageBox.Show("string is legal");


            string strTest = textBox1.Text;

            //正则表达式  
            string strExp1 = @"^https://";
            string strExp = @"^http://";

            //创建正则表达式对象  
            Regex myRegex = new Regex(strExp);
            Regex myRegex1 = new Regex(strExp1);

            if (myRegex.IsMatch(strTest) || myRegex1.IsMatch(strTest))
            {
                listBox1.Items.Clear();
                UpdateMemu();

                string index = writeini.GetSetting("DefaultStartup", "index");

                // string index = IniReadValue("DefaultStartup", "index", "");

             //   MessageBox.Show(index);

                int index1 = Convert.ToInt32(index);

                index1++;


                string GrourpServer = "Server" + index1.ToString();


                writeini.AddSetting("DefaultStartup", "index", index1.ToString());
                writeini.AddSetting("DefaultStartup", "FirstRun", "0");
                writeini.AddSetting("User", GrourpServer, textBox1.Text);
                writeini.AddSetting("DefaultStartup", "Server", textBox1.Text);


                writeini.SaveiniFile();

                listBox1.Items.Add(textBox1.Text);
                textBox1.Text = "";

                //  添加服务器菜单设置代理 

                string GlobalStatus = writeini.GetSetting("ProxyMode", "Global");

                string SmartStatus = writeini.GetSetting("ProxyMode", "smart");

                //   string GlobalStatus = ReadINI("ProxyMode", "Global", @path + "\\" + "SquidConfig.ini");
                //   string SmartStatus = ReadINI("ProxyMode", "Smart", @path + "\\" + "SquidConfig.ini");

                if (GlobalStatus == "1")

                {
                    string DefaultServer = writeini.GetSetting("DefaultStartup", "Server");

                    //  string DefaultServer = ReadINI("DefaultStartup", "Server", @path + "\\" + "SquidConfig.ini");

                    string PacURL = DefaultServer;


                }
                else if (SmartStatus == "1")

                {
                    string DefaultServer = writeini.GetSetting("DefaultStartup", "Server");

                    // string DefaultServer = ReadINI("DefaultStartup", "Server", @path + "\\" + "SquidConfig.ini");

                }

                //  添加服务器菜单设置代理 


                //int j = 0;

                //foreach (string item in listBox1.Items)
                //{
                //    j = j + 1;
                //    MessageBox.Show("Server"+j.ToString()+"="+item);
                //}



            }

            else

            {
                MessageBox.Show("输入的服务器地址不匹配");

            }


            //Synchronize the contextMenuStrip text when User add a Server IP
            EventHandler handler = this.DisableButton;
            if (handler != null)
                handler(this, EventArgs.Empty);

            //Synchronize the contextMenuStrip text when User add a Server IP
            // this.Close();


        }
        public void DeleteKey(string sectionName, string keyName)

        {

            string path = System.Environment.CurrentDirectory;

            WritePrivateProfileString(sectionName, keyName, null, @path + "\\" + "SquidConfig.ini");

        }

        public string ReadINI(string section, string key, string fileName)
        {
            StringBuilder temp = new StringBuilder(1024);
            GetPrivateProfileString(section, key, "NullValue", temp, 1024, fileName);
            return temp.ToString();
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

        void EditServer1(object sender, EventArgs e)
        {
            EditServer form = new EditServer();
            form.Show();
        }
        void MenuClicked(object sender, EventArgs e)
        {

            //    MessageBox.Show(((sender as ToolStripMenuItem).Text));
            //  SetSquidProxy1((sender as ToolStripMenuItem).Text);
        }




        public void LoadServerMenu()

        {

            //添加菜单一 
            ToolStripMenuItem subItem;
            subItem = AddContextMenu("服务器", contextMenuStrip1.Items, null);




            // subItem1 = AddContextMenu("重新加载配置", contextMenuStrip1.Items, new EventHandler(LoadSetting));


            string path = System.Environment.CurrentDirectory;

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

            AddContextMenu("编辑服务器", subItem.DropDownItems, new EventHandler(EditServer1));


            //  contextMenuStrip1.Items.Clear();

            // subItem.DropDownItems.Clear();

            string Server1 = ReadINI("User", "Server1", path + "\\" + "SquidConfig.ini");

            //  SetSquidProxy1(Server1);

        }


        private void EditServer_Load(object sender, EventArgs e)
        {




            this.ImageShow.Image = global::squidproxy.Properties.Resources._2;

            timer1.Enabled = true;

            // listBox1.MultiColumn = true;

            FormBorderStyle = FormBorderStyle.FixedDialog;
            ToolStripMenuItem subItem;
            //   subItem = AddContextMenu("服务器", contextMenuStrip1.Items, null);


            string path = System.Environment.CurrentDirectory;

            string iniFilePath = path + "\\" + "SquidConfig.ini";

            Writeini writeini = new Writeini();
            writeini.IniParser(iniFilePath);


            string ServerindexString = writeini.GetSetting("DefaultStartup", "index");


            //  string ServerindexString = ReadINI("DefaultStartup", "index", @path + "\\" + "SquidConfig.ini");



            //  MessageBox.Show(path + "\\" + "SquidConfig.ini");

            //  MessageBox.Show(ServerindexString);

            int ServerindexInt = int.Parse(ServerindexString);

            int j = 0;

            for (int i = 0; i < ServerindexInt; i++)
            {


                j = j + 1;

                string ServerAddress = writeini.GetSetting("User", "Server" + j.ToString());

                //  string ServerAddress = ReadINI("User", "Server" + j.ToString(), path + "\\" + "SquidConfig.ini");

                //  MessageBox.Show(ServerAddress);

                listBox1.Items.Add(ServerAddress);

                // AddContextMenu(ServerAddress, subItem.DropDownItems, new EventHandler(MenuClicked));



            }


            //  AddContextMenu("编辑服务器", subItem.DropDownItems, new EventHandler(EditServer));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string path = System.Environment.CurrentDirectory;

            string iniFilePath = path + "\\" + "SquidConfig.ini";

            Writeini writeini = new Writeini();

            writeini.IniParser(iniFilePath);


            //  MessageBox.Show("string is legal");


            this.listBox1.Items.Remove(this.listBox1.SelectedItem); //del Server Frome Listbox


            string ServerindexString = writeini.GetSetting("DefaultStartup", "index");

            int ServerindexInt = int.Parse(ServerindexString);
            int ServerindexIntdel = ServerindexInt - 1; //Setting index value


            //  string GrourpServer = "Server" + index1.ToString();

            writeini.AddSetting("DefaultStartup", "index", ServerindexIntdel.ToString());
            //  writeini.AddSetting("DefaultStartup", "index", index1.ToString());
            writeini.AddSetting("DefaultStartup", "FirstRun", "0");
            // writeini.AddSetting("User", GrourpServer, textBox1.Text);
            //   writeini.AddSetting("DefaultStartup", "Server", textBox1.Text);


            writeini.SaveiniFile();

            listBox1.Items.Add(textBox1.Text);
            textBox1.Text = "";

            //  添加服务器菜单设置代理 

            string GlobalStatus = writeini.GetSetting("ProxyMode", "Global");

            string SmartStatus = writeini.GetSetting("ProxyMode", "smart");

            //   string GlobalStatus = ReadINI("ProxyMode", "Global", @path + "\\" + "SquidConfig.ini");
            //   string SmartStatus = ReadINI("ProxyMode", "Smart", @path + "\\" + "SquidConfig.ini");


            MessageBox.Show("Server");




            //for (int i = 0; i < this.listBox1.Items.Count; i++)

            //{

            int j = 0;

            foreach (string item in listBox1.Items)
            {
                j = j + 1;
                //   MessageBox.Show("Server" + j.ToString() + "=" + item);

                //   IniWriteValue("User1", "Server" + j.ToString(), item); //Update Server list



                writeini.AddSetting("User", "Server" + j.ToString(), item);



                writeini.SaveiniFile();

                //   IniWriteValue("User", GrourpServer, textBox1.Text);

                //write update data when listbox item change
                // Listbox Server 


            }

            //}


            listBox1.Items.Clear();

            UpdateMemu();


            if (GlobalStatus == "1")

            {
                string DefaultServer = writeini.GetSetting("DefaultStartup", "Server");

                //  string DefaultServer = ReadINI("DefaultStartup", "Server", @path + "\\" + "SquidConfig.ini");

                string PacURL = DefaultServer;


            }
            else if (SmartStatus == "1")

            {
                string DefaultServer = writeini.GetSetting("DefaultStartup", "Server");

                // string DefaultServer = ReadINI("DefaultStartup", "Server", @path + "\\" + "SquidConfig.ini");

            }

            //  添加服务器菜单设置代理 


            //int j = 0;

            //foreach (string item in listBox1.Items)
            //{
            //    j = j + 1;
            //    MessageBox.Show("Server"+j.ToString()+"="+item);
            //}








            //Synchronize the contextMenuStrip text when User add a Server IP
            EventHandler handler = this.DisableButton;
            if (handler != null)
                handler(this, EventArgs.Empty);

            //Synchronize the contextMenuStrip text when User add a Server IP
            // this.Close();





            //if (listBox1.Items.Count == 0)


            //{


            //    MessageBox.Show("没有可以删除的服务器");

            //}

            //else

            //{


            //    int ischoose = listBox1.SelectedItems.Count;

            //    if (ischoose == 0)

            //    {
            //        MessageBox.Show("请选择要删除的服务器");

            //    }
            //    else

            //    {




            //string ServerindexString = writeini.GetSetting("DefaultStartup", "index");

            //int ServerindexInt = int.Parse(ServerindexString);
            //int ServerindexIntdel = ServerindexInt - 1; //Setting index value
            //writeini.AddSetting("DefaultStartup", "index", ServerindexIntdel.ToString());
            //MessageBox.Show(ServerindexString);

            // // WritePrivateProfileString("User", null, null, @path + "\\" + "SquidConfig.ini");

            //// string ServerindexString = ReadINI("DefaultStartup", "index", @path + "\\" + "SquidConfig.ini");

            ////    MessageBox.Show(ServerindexString);









            //   IniWriteValue("DefaultStartup", "index", ServerindexIntdel.ToString()); //Update Server list



            // Listbox Server 


            //write update data when listbox item change




            //textBox1.Text = "";

            ////  LoadServerMenu();

            ////Synchronize the contextMenuStrip text when User add a Server IP
            //EventHandler handler = this.DisableButton;
            //if (handler != null)
            //    handler(this, EventArgs.Empty);

            ////Synchronize the contextMenuStrip text when User add a Server IP


        }




           private void button4_Click(object sender, EventArgs e)
        {
            string path = System.Environment.CurrentDirectory;

            string iniFilePath = path + "\\" + "SquidConfig.ini";

            Writeini writeini = new Writeini();



            writeini.IniParser(iniFilePath);



            writeini.AddSetting("DefaultStartup", "index", "0");
            //  writeini.AddSetting("DefaultStartup", "index", index1.ToString());
            writeini.AddSetting("DefaultStartup", "FirstRun", "0");

            writeini.SaveiniFile();

            int Listboxrow = listBox1.Items.Count;


            MessageBox.Show("行数"+Listboxrow.ToString());

           
            for ( int del=1 ; del <= Listboxrow; del++)
            {
                writeini.DeleteSetting("User", "Server" + del.ToString());

                MessageBox.Show("User", "Server" + del.ToString());

                writeini.SaveiniFile();

            }



            //  listBox1.Items.Clear();

            //IniWriteValue("DefaultStartup", "index", "0");
            //DeleteKey("User", null);
            textBox1.Text = "";

            //LoadServerMenu();


            ////Synchronize the contextMenuStrip text when User add a Server IP
            //EventHandler handler = this.DisableButton;
            //if (handler != null)
            //    handler(this, EventArgs.Empty);

            //Synchronize the contextMenuStrip text when User add a Server IP


            listBox1.Items.Clear();

        }




     
        private void button1_Click_1(object sender, EventArgs e)
        {
            LoadServerMenu();
        }

        private void button1_Click_2(object sender, EventArgs e)
        {

            try

            {
                string item = listBox1.SelectedItem.ToString();
                int i = listBox1.SelectedIndex;
                if (i == 0)
                    return;

                listBox1.Items.Remove(listBox1.SelectedIndex);


            }

            catch (Exception)

            {
                MessageBox.Show("请选择一个服务器");
            }

        }



        private void timer1_Tick(object sender, EventArgs e)
        {




            pictureIndex = pictureIndex + 1;

            if (pictureIndex >= 2)

            {
                pictureIndex = -1;

            }

            if (pictureIndex == 0)

            {

                this.ImageShow.Image = global::squidproxy.Properties.Resources._1;

            }

            else
                if (pictureIndex == 1)

            {

                this.ImageShow.Image = global::squidproxy.Properties.Resources._2;
            }

            else
                if (pictureIndex == 2)

            {

                ImageShow.Image = squidproxy.Properties.Resources._3;
                //}


            }


        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            pictureIndex = pictureIndex + 1;

            if (pictureIndex >= 6)

            {
                pictureIndex = -1;

            }

            if (pictureIndex == 0)

            {

                this.ImageShow.Image = global::squidproxy.Properties.Resources._1;

            }

            else
                if (pictureIndex == 1)

            {

                this.ImageShow.Image = global::squidproxy.Properties.Resources._2;
            }

            else
                if (pictureIndex == 2)

            {

                this.ImageShow.Image = global::squidproxy.Properties.Resources._3;
                //}

            }

            else
                if (pictureIndex == 3)

            {

                this.ImageShow.Image = global::squidproxy.Properties.Resources._4;
                //}

            }

            else
                if (pictureIndex == 4)

            {

                this.ImageShow.Image = global::squidproxy.Properties.Resources._5;
                //}

            }

            else
                if (pictureIndex == 5)

            {

                this.ImageShow.Image = global::squidproxy.Properties.Resources._6;
                //}

            }


        }

        private void button1_Click_3(object sender, EventArgs e)
        {

        }

        private void button1_Click_4(object sender, EventArgs e)
        {
            EventHandler handler = this.DisableButton;
            if (handler != null)
                handler(this, EventArgs.Empty);
            this.Close();


        }

        private void button1_Click_5(object sender, EventArgs e)
        {
            string path = System.Environment.CurrentDirectory;

            string iniFilePath = path + "\\" + "SquidConfig.ini";

            Writeini writeini = new Writeini();

            writeini.IniParser(iniFilePath);


            //  MessageBox.Show("string is legal");


            string strTest = textBox1.Text;

            //正则表达式  
            string strExp1 = @"^https://";
            string strExp = @"^http://";

            //创建正则表达式对象  
            Regex myRegex = new Regex(strExp);
            Regex myRegex1 = new Regex(strExp1);

            if (myRegex.IsMatch(strTest) || myRegex1.IsMatch(strTest))
            {
                listBox1.Items.Clear();
                UpdateMemu();

                string index = writeini.GetSetting("DefaultStartup", "index");

                // string index = IniReadValue("DefaultStartup", "index", "");

            //    MessageBox.Show(index);

                int index1 = Convert.ToInt32(index);

                index1++;


                string GrourpServer = "Server" + index1.ToString();


                writeini.AddSetting("DefaultStartup", "index", index1.ToString());
                writeini.AddSetting("DefaultStartup", "FirstRun", "0");
                writeini.AddSetting("User", GrourpServer, textBox1.Text);
                writeini.AddSetting("DefaultStartup", "Server", textBox1.Text);


                writeini.SaveiniFile();

                listBox1.Items.Add(textBox1.Text);
                textBox1.Text = "";

                //  添加服务器菜单设置代理 

                string GlobalStatus = writeini.GetSetting("ProxyMode", "Global");

                string SmartStatus = writeini.GetSetting("ProxyMode", "smart");

                //   string GlobalStatus = ReadINI("ProxyMode", "Global", @path + "\\" + "SquidConfig.ini");
                //   string SmartStatus = ReadINI("ProxyMode", "Smart", @path + "\\" + "SquidConfig.ini");

                if (GlobalStatus == "1")

                {
                    string DefaultServer = writeini.GetSetting("DefaultStartup", "Server");

                    //  string DefaultServer = ReadINI("DefaultStartup", "Server", @path + "\\" + "SquidConfig.ini");

                    string PacURL = DefaultServer;


                }
                else if (SmartStatus == "1")

                {
                    string DefaultServer = writeini.GetSetting("DefaultStartup", "Server");

                    // string DefaultServer = ReadINI("DefaultStartup", "Server", @path + "\\" + "SquidConfig.ini");

                }

                //  添加服务器菜单设置代理 


                //int j = 0;

                //foreach (string item in listBox1.Items)
                //{
                //    j = j + 1;
                //    MessageBox.Show("Server"+j.ToString()+"="+item);
                //}



            }

            else

            {
                MessageBox.Show("输入的服务器地址不匹配");

            }


            //Synchronize the contextMenuStrip text when User add a Server IP
            EventHandler handler = this.DisableButton;
            if (handler != null)
                handler(this, EventArgs.Empty);

            //Synchronize the contextMenuStrip text when User add a Server IP
            // this.Close();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            string path = System.Environment.CurrentDirectory;

            string iniFilePath = path + "\\" + "SquidConfig.ini";

            Writeini writeini = new Writeini();

            writeini.IniParser(iniFilePath);


         


            int ischoose = listBox1.SelectedItems.Count;

            if (ischoose == 0)

            {
                MessageBox.Show("请选择要删除的服务器");

            }
            else

            {
                //  MessageBox.Show("string is legal");


                this.listBox1.Items.Remove(this.listBox1.SelectedItem); //del Server Frome Listbox


                string ServerindexString = writeini.GetSetting("DefaultStartup", "index");

                int ServerindexInt = int.Parse(ServerindexString);
                int ServerindexIntdel = ServerindexInt - 1; //Setting index value


                //  string GrourpServer = "Server" + index1.ToString();

                writeini.AddSetting("DefaultStartup", "index", ServerindexIntdel.ToString());
                //  writeini.AddSetting("DefaultStartup", "index", index1.ToString());
                writeini.AddSetting("DefaultStartup", "FirstRun", "0");
                // writeini.AddSetting("User", GrourpServer, textBox1.Text);
                //   writeini.AddSetting("DefaultStartup", "Server", textBox1.Text);


                writeini.SaveiniFile();

                listBox1.Items.Add(textBox1.Text);
                textBox1.Text = "";

                //  添加服务器菜单设置代理 

                string GlobalStatus = writeini.GetSetting("ProxyMode", "Global");

                string SmartStatus = writeini.GetSetting("ProxyMode", "smart");

                //   string GlobalStatus = ReadINI("ProxyMode", "Global", @path + "\\" + "SquidConfig.ini");
                //   string SmartStatus = ReadINI("ProxyMode", "Smart", @path + "\\" + "SquidConfig.ini");


            //    MessageBox.Show("Server");




                //for (int i = 0; i < this.listBox1.Items.Count; i++)

                //{

                int j = 0;

                foreach (string item in listBox1.Items)
                {
                    j = j + 1;
                    //   MessageBox.Show("Server" + j.ToString() + "=" + item);

                    //   IniWriteValue("User1", "Server" + j.ToString(), item); //Update Server list



                    writeini.AddSetting("User", "Server" + j.ToString(), item);



                    writeini.SaveiniFile();

                    //   IniWriteValue("User", GrourpServer, textBox1.Text);

                    //write update data when listbox item change
                    // Listbox Server 


                }

                //}


                listBox1.Items.Clear();

                UpdateMemu();


                if (GlobalStatus == "1")

                {
                    string DefaultServer = writeini.GetSetting("DefaultStartup", "Server");

                    //  string DefaultServer = ReadINI("DefaultStartup", "Server", @path + "\\" + "SquidConfig.ini");

                    string PacURL = DefaultServer;


                }
                else if (SmartStatus == "1")

                {
                    string DefaultServer = writeini.GetSetting("DefaultStartup", "Server");

                    // string DefaultServer = ReadINI("DefaultStartup", "Server", @path + "\\" + "SquidConfig.ini");

                }

                //  添加服务器菜单设置代理 


                //int j = 0;

                //foreach (string item in listBox1.Items)
                //{
                //    j = j + 1;
                //    MessageBox.Show("Server"+j.ToString()+"="+item);
                //}








                //Synchronize the contextMenuStrip text when User add a Server IP
                EventHandler handler = this.DisableButton;
                if (handler != null)
                    handler(this, EventArgs.Empty);

                //Synchronize the contextMenuStrip text when User add a Server IP
                // this.Close();





                //if (listBox1.Items.Count == 0)


                //{


                //    MessageBox.Show("没有可以删除的服务器");

                //}

                //else

                //{

            }





            //string ServerindexString = writeini.GetSetting("DefaultStartup", "index");

            //int ServerindexInt = int.Parse(ServerindexString);
            //int ServerindexIntdel = ServerindexInt - 1; //Setting index value
            //writeini.AddSetting("DefaultStartup", "index", ServerindexIntdel.ToString());
            //MessageBox.Show(ServerindexString);

            // // WritePrivateProfileString("User", null, null, @path + "\\" + "SquidConfig.ini");

            //// string ServerindexString = ReadINI("DefaultStartup", "index", @path + "\\" + "SquidConfig.ini");

            ////    MessageBox.Show(ServerindexString);









            //   IniWriteValue("DefaultStartup", "index", ServerindexIntdel.ToString()); //Update Server list



            // Listbox Server 


            //write update data when listbox item change




            //textBox1.Text = "";

            ////  LoadServerMenu();

            ////Synchronize the contextMenuStrip text when User add a Server IP
            //EventHandler handler = this.DisableButton;
            //if (handler != null)
            //    handler(this, EventArgs.Empty);

            ////Synchronize the contextMenuStrip text when User add a Server IP
        }

        private void button3_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/squidproxy/squidproxy");
            this.Hide();
            this.ShowInTaskbar = false;
        }
    }
}
