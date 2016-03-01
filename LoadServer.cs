using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace BadouSquidClient
{
    class LoadServer
    {
        public string ReadINI(string section, string key, string fileName)
        {
            StringBuilder temp = new StringBuilder(1024);
        //    GetPrivateProfileString(section, key, "NullValue", temp, 1024, fileName);
            return temp.ToString();
        }

        public void UpdateMenu()

        {

            //添加菜单一 
       //     ToolStripMenuItem subItem;
       //     subItem = AddContextMenu("服务器", contextMenuStrip1.Items, null);



            string ServerindexString = ReadINI("DefaultStartup", "index", "F:\\C#\\SquidClient巴豆\\pac\\pac\\bin\\Release\\SquidConfig.ini");

      //      MessageBox.Show(ServerindexString);

            int ServerindexInt = int.Parse(ServerindexString);

            int j = 0;

            for (int i = 0; i < ServerindexInt; i++)
            {


                j = j + 1;
                string ServerAddress = ReadINI("User", "Server" + j.ToString(), "F:\\C#\\SquidClient巴豆\\pac\\pac\\bin\\Release\\SquidConfig.ini");
       
                //    MessageBox.Show(ServerAddress);

                //   AddContextMenu(ServerAddress, subItem.DropDownItems, new EventHandler(MenuClicked));



            }

        //    AddContextMenu("编辑服务器", subItem.DropDownItems, new EventHandler(EditServer));

        }

    }
}
