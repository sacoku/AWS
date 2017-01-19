using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AWS.VIEW
{
    public partial class ControlForm : Form
    {
        ILog iLog = log4net.LogManager.GetLogger("Logger");
        private MainForm main = null;
        
        //데이타를 수집하기 위한 스레드 객체
        public Thread CollectThread = null;
        private DateTime startDate;
        private DateTime endDate;
        private bool useLastDataCall = false;

        public ControlForm(MainForm mainForm)
        {
            this.main = mainForm;
            InitializeComponent();
        }

        /// <summary>
        /// Version 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VersionButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < AWS.Config.AWSConfig.sCount; i++)
            {
                this.main.logger[i].SendCommnad(DateTime.Now, this.StrToByteArray("AV?"));
            }
        }

        public byte[] StrToByteArray(string str)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            return encoding.GetBytes(str);
        }

        /// <summary>
        /// Power Reset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonX3_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < AWS.Config.AWSConfig.sCount; i++)
                this.main.logger[i].SendCommnad(DateTime.Now, this.StrToByteArray("AR?"));
        }

        /// <summary>
        /// Buffer Clear
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BufferClearButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < AWS.Config.AWSConfig.sCount; i++)
                this.main.logger[i].SendCommnad(DateTime.Now, this.StrToByteArray("AC?"));
        }

        /// <summary>
        /// TimeSync
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimeSyncButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < AWS.Config.AWSConfig.sCount; i++)
                this.main.logger[i].SendCommnad(DateTime.Now, this.StrToByteArray("AT?"));
        }

        private void LocationNumberButton_Click(object sender, EventArgs e)
        {
            if (this.txtLocationNumber.Text == "")
            {
                MessageBox.Show("Please enter Location Number!");
                return;
            }
            else
            {
                for (int i = 0; i < AWS.Config.AWSConfig.sCount; i++)
                    this.main.logger[i].SendCommandLoggerID(DateTime.Now, ushort.Parse(this.txtLocationNumber.Text), this.StrToByteArray("AD?"));
            }
        }

        private void PasswordButton_Click(object sender, EventArgs e)
        {
            if (this.txtPassword.Text == "")
            {
                MessageBox.Show("Please enter Password!");
                return;
            }
            else
            {
                for (int i = 0; i < AWS.Config.AWSConfig.sCount; i++)
                    this.main.logger[i].SendCommandPassword(DateTime.Now, ushort.Parse(this.txtPassword.Text), this.StrToByteArray("AW?"));
            }
        }

        /// <summary>
        /// 과거 자료 요청
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonX1_Click(object sender, EventArgs e)
        {
            if (this.startNumericUpDown.Value.ToString() == "")
            {
                MessageBox.Show("시작 시간을 입력해 주세요");
                return;
            }

            if (this.endNumericUpDown.Value.ToString() == "")
            {
                MessageBox.Show("종료 시간을 입력해 주세요");
                return;
            }

            if (this.startNumericUpDown.Value >= this.endNumericUpDown.Value)
            {
                MessageBox.Show("시작 시간은 종료 시간보다 작아야 합니다.");
                return;
            }
           
            DateTime selectedDate = this.monthCalendar1.SelectionStart;

            if( (selectedDate.Year == DateTime.Now.Year) && (selectedDate.Month == DateTime.Now.Month) && (selectedDate.Day == DateTime.Now.Day))
            {
                if( (this.startNumericUpDown.Value >= DateTime.Now.Hour) || (this.endNumericUpDown.Value >= DateTime.Now.Hour))
                {
                    MessageBox.Show("과거 자료 요청은 현재 시간보다 작아야 합니다.");
                    return;
                }   
            }

            startDate = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, (int) startNumericUpDown.Value, 0, 0);
            endDate = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, (int)endNumericUpDown.Value -1, 59, 30);

            if (this.useLastDataCall != true)
                useLastDataCall = true;
            else
            {
                MessageBox.Show("이미 과거 자료 요청을 보내고 있습니다.");
                return;
            }

            CollectThread = new Thread(new ThreadStart(StartLastCollect));
            CollectThread.Name = "collectDataForOWI";
            CollectThread.Start();
        }

        private void StartLastCollect()
        {
            this.useLastDataCall = true;

            while (startDate < endDate)
            {
                try
                {
                    for (int i = 0; i < AWS.Config.AWSConfig.sCount; i++)
                        this.main.logger[i].SendCommand(startDate, this.StrToByteArray("AQ?"));
                    startDate = startDate.AddMinutes(+1);
                    Thread.Sleep(1000);
                }
                catch (Exception ex)
                {
                    iLog.Info("[ERROR] ControlForm : StartLastCollect " + ex.Message);
                }

            }
            this.useLastDataCall = false;
        }

        private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {
            DateTime selectedDate = this.monthCalendar1.SelectionStart;

            DateTime today = DateTime.Now;

            if (selectedDate.Subtract(today).Days < -5)
            {
                MessageBox.Show("5일 이전은 선택 할 수 없습니다.");
                return;
            }

            Debug.WriteLine(selectedDate.Subtract(today).Hours);

            if (selectedDate.Subtract(today).Hours > 0)
            {
                MessageBox.Show("오늘 이후는 선택 할 수 없습니다.");
                return;
            }

        }
    }
}
