using System;
using System.Windows.Forms;

namespace pac
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            //http://stackoverflow.com/questions/6486195/ensuring-only-one-application-instance
            bool result;
            var mutex = new System.Threading.Mutex(true, "UniqueAppId", out result);

            if (!result)
            {
                MessageBox.Show("BadouSquidClient客户端已经运行!", "提示");
                return;
            }

            Application.Run(new Form1());

            GC.KeepAlive(mutex);                // mutex shouldn't be released - important line
        }
    }
}
