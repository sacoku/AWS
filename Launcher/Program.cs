using Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using UTIL;

namespace Launcher
{
    static class Program
    {
        static System.Threading.Mutex singleton = new System.Threading.Mutex(true, "Launcher");

        /// <summary>
        /// The main entry point for the application.
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

            try
            {
                AWSConfig.HOME_PATH = CommonUtil.ReadReg("HOME_PATH");
                //AWSConfig.Load(AWSConfig.HOME_PATH + "\\Config\\AWSConfig.xml");
            }
            catch (Exception e)
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    AWSConfig.HOME_PATH = dialog.SelectedPath;
                    //REGISTRY에 저장...
                    CommonUtil.WriteReg("HOME_PATH", AWSConfig.HOME_PATH);
                }
                else
                {
                    System.Windows.Forms.Application.Exit();
                }
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
