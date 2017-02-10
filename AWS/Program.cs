using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AWS
{
    static class Program
    {
        static System.Threading.Mutex singleton = new System.Threading.Mutex(true, "AWS");

		public static MainForm mf = null;

        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (!singleton.WaitOne(TimeSpan.Zero, true))
            {
                //there is already another instance running!
                MessageBox.Show("이미 실행중입니다.");
                return;
            }
            string appPath = Path.GetDirectoryName(Application.ExecutablePath);

            XmlConfigurator.Configure(new System.IO.FileInfo(appPath + @"\logconfig.xml"));
          
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
			mf = new MainForm();
            Application.Run(mf);
        }
    }
}
