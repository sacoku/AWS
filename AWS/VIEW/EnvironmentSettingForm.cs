using AWS.MODEL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AWS.VIEW
{
    public partial class EnvironmentSettingForm : Form
    {
        private MainForm main = null;

        public EnvironmentSettingForm(MainForm mainForm)
        {
            this.main = mainForm;
            InitializeComponent();
            this.ReadInit();
        }

        /// <summary>
        /// Environment의 폼을 활성화되면 트리뷰를 초기화 하는 메소드 
        /// </summary>
        public void ReadInit()
        {
            try
            {
                this.txtIP.Text = Properties.Settings.Default.IP.ToString();
                this.txtPort.Text = Properties.Settings.Default.PORT.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        /// <summary>
        /// 추가 버튼을 클릭하면 발생하는 이벤트 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (this.txtIP.Text == "")
            {
                MessageBox.Show("IP를 입력해 주세요");
                return;
            }

            if (this.txtPort.Text == "")
            {
                MessageBox.Show("포트를 입력해 주세요");
                return;
            }

            try
            {
                Properties.Settings.Default.IP = this.txtIP.Text;
                Properties.Settings.Default.PORT = this.txtPort.Text;
                Properties.Settings.Default.Save();

                MessageBox.Show("성공적으로 저장했습니다.");

                DataLoggerEnvironment environment = new DataLoggerEnvironment();

                environment.IP = this.txtIP.Text.Trim();
                environment.PORT = int.Parse(this.txtPort.Text.Trim());              
            }           
            catch (Exception ex)
            {
                Debug.WriteLine("EnvironmentForm 201:" + ex.Message);               
            }

            try
            {
                this.main.loggerReset();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Logger Reset Fail :" + ex.Message);
            }

        }
    }
}