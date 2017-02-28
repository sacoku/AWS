using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Launcher
{
    public partial class Form1 : Form
    {
        private Timer timer;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer = new Timer();
            timer.Enabled = true;
            timer.Interval = 10 * 1000;
            timer.Tick += new EventHandler((s, e2) =>
            {
                Process[] process = Process.GetProcessesByName("AWS");

                if(process.Length > 0)
                {
                    ;
                } else
                {
                    Process p = new Process();
                    //p.StartInfo = new ProcessStartInfo(@"D:\[01]Work\[02]Dev\[ZZ]REPOSITORY\AWS\AWS\bin\x86\Release\AWS.exe", string.Empty);
                    p.StartInfo = new ProcessStartInfo(@"C:\AWS\BIN\AWS.exe", string.Empty);
                    p.StartInfo.RedirectStandardOutput = false;
                    p.StartInfo.RedirectStandardError = true;
                    p.StartInfo.UseShellExecute = false;
                    //p.StartInfo.CreateNoWindow = !showWindow;
                    //p.ErrorDataReceived += (s, a) => addLogLine(a.Data);
                    p.Start();
                    richTextBox1.Text +=  DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " 프로그램이 다시 시작 되었습니다.\r\n";
                    //p.BeginErrorReadLine();

                }
            });

        }
    }
}
