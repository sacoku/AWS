using System;
using System.Drawing;
using System.Windows.Forms;
using AWS.CONTROL;
using AWS.VIEW;
using System.IO;
using System.Threading;
using AWS.CONTROLS;
using System.Diagnostics;
using log4net;
using AWS.Config;
using AWS.UTIL;
using System.Data.OleDb;

#pragma warning disable 0168

namespace AWS
{
    public partial class MainForm : Form
    {
        public DisplayForm displayForm = null;
        public ReportForm reportForm = null;
        public DataLogger[] logger = null;

        //데이타를 수집하기 위한 스레드 객체
        public Thread makeThread = null;
        private bool isFlag = true;
        public System.DateTime m_MakeFileDate;
        ILog iLog = log4net.LogManager.GetLogger("Logger");

        //add by shkim
        public DisplayForm2 displayForm2 = null;
        private int loggerCnt = 0;

        public MainForm()
        {
            InitializeComponent();

			//++ add by sacoku
			try
			{
                //REGISTRY에서 HOME_PATH를 불러옴..
                AWSConfig.HOME_PATH = CommonUtil.ReadReg("HOME_PATH");
                iLog.Info(AWSConfig.HOME_PATH + "\\Config\\AWSConfig.xml");
                AWSConfig.Load(AWSConfig.HOME_PATH + "\\Config\\AWSConfig.xml");
			}
			catch (Exception e)
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    AWSConfig.HOME_PATH = dialog.SelectedPath;
                    iLog.Debug("HOME_PATH" + AWSConfig.HOME_PATH);
                    //REGISTRY에 저장...
                    CommonUtil.WriteReg("HOME_PATH", AWSConfig.HOME_PATH);
                    try
                    {
                        AWSConfig.Load(AWSConfig.HOME_PATH + "\\Config\\AWSConfig.xml");
                    } catch(Exception e1)
                    {
                        Environment.Exit(-1);
                    }
                    
                }
                else
                    Environment.Exit(-1);
            }
			//add by sacoku ++

			try
			{

				this.displayForm = new DisplayForm(this);
				this.reportForm = new ReportForm(this);
				this.displayForm2 = new DisplayForm2(this);

				this.displayForm.ControlBox = false;
				this.displayForm2.ControlBox = false;

				this.displayForm.MdiParent = this;
				this.reportForm.MdiParent = this;
				this.displayForm2.MdiParent = this;

				//this.displayForm.Show();
				this.displayForm2.Show();

				this.displayForm2.Dock = DockStyle.Fill;
				this.reportForm.Dock = DockStyle.Fill;

				checkAccessFile();

				m_MakeFileDate = new System.DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 22, 1, 0);

				makeThread = new Thread(new ThreadStart(makeDir));
				makeThread.Name = "make file";
				makeThread.Start();

				this.init();

				//add by sacoku
				loggerCnt = AWS.Config.AWSConfig.sCount;
				logger = new DataLogger[loggerCnt];
				for (int i = 0; i < loggerCnt; i++)
				{
					if (AWS.Config.AWSConfig.sValue[i].enable)
						logger[i] = new DataLogger(this, i);
				}

				iLog.Info("Program Start");

			} catch(Exception e)
			{
				iLog.Error(e.Message);
				iLog.Error(e.StackTrace);
			}

		}

        public void init()
        {
 
        }

        /// <summary>
        /// 데이터 표출 화면 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            try
            {
                Form activeChild = this.ActiveMdiChild;

                if (activeChild.Name != "DisplayForm2")
                {
                    //this.displayForm.Show();
                    //this.displayForm.BringToFront();
                    //this.displayForm.StartPosition = FormStartPosition.CenterScreen;
                    this.displayForm2.Show();
                    this.displayForm2.BringToFront();                    
                    this.displayForm2.StartPosition = FormStartPosition.CenterScreen;
                }
            }
            catch (Exception ex) 
            {
                iLog.Error("toolStripButton1_Click : " + ex.Message);
            }
        }

        /// <summary>
        /// 레포트 표출 화면 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>        
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            try
            {
                Form activeChild = this.ActiveMdiChild;

                if (activeChild.Name != "ReportForm")
                {
                    this.reportForm.StartPosition = FormStartPosition.CenterScreen;
                    this.reportForm.MdiParent = this;
                    this.reportForm.Show();
                    this.reportForm.BringToFront();
                }
                else
                    this.reportForm.Hide();             
            }
            catch (Exception ex)
            {
                iLog.Error("toolStripButton2_Click : " + ex.Message);
            }
        }
       
        /// <summary>
        /// 제어 표출 화면 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>        
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                Debug.WriteLine("toolStripButton5_Click : " + ex.Message);
            }
        }
        
        /// <summary>
        /// 환경 설정 폼 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            try
            {
				System.Diagnostics.Process ps = new System.Diagnostics.Process();
				ps.StartInfo.FileName = "notepad";
				ps.StartInfo.Arguments = AWS.Config.AWSConfig.HOME_PATH + "\\Config\\AWSConfig.xml";
				ps.StartInfo.WorkingDirectory = AWS.Config.AWSConfig.HOME_PATH + "\\Config\\";
				ps.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;

				ps.Start();
			}
			catch (Exception ex)
            {
				iLog.Error(ex.Message);
            }
        }

        /// <summary>
        /// 화면 왼쪽 하단에 제어 전송 및 수신 상태를 보여주는 메소드
        /// </summary>
        /// <param name="status"></param>
        /// <param name="foreColor"></param>
        public void displayStatus(String status, Color foreColor)
        {
            try
            {
                this.toolStripStatusLabel.Text = status;
                this.toolStripStatusLabel.ForeColor = foreColor;
            }
            catch (Exception ex)
            {
                iLog.Debug(ex.Message);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            
            if (e.CloseReason == CloseReason.WindowsShutDown) return;

            if (this.logger != null)
            {
                for (int i = 0; i < loggerCnt; i++)
                {
                    if(logger[i] != null)
                        logger[i].CloseLogger();
                }
            }

            isFlag = false;
            iLog.Info("Program Exit");
            System.Windows.Forms.Application.Exit();
        }

        public void loggerReset()
        {
            if (this.logger != null)
            {
                for (int i = 0; i < loggerCnt; i++)
                {
                    if (this.logger[i] != null)
                    {
                        if (this.logger[i].comPort.IsOpen)
                        {
                            this.logger[i].comPort.Close();
                            logger = null;
                        }
                    }
                }
            }

            for (int i = 0; i < loggerCnt; i++)
            {
                if (AWS.Config.AWSConfig.sValue[i].enable)
                    this.logger[i] = new DataLogger(this,i);
            }
           
        }

        public void setTXRX(int image)
        {
            try
            {
                Thread.Sleep(1);

                if (image == 1)  // RED, TX
                {
                    this.toolStripStatusLabel1.Image = AWS.Properties.Resources.red_circle_render_by_rincagamine_d67j48d;
                    this.toolStripStatusLabel2.Text = "TX";
                }
                else //BLUE, RX 
                {
                    this.toolStripStatusLabel1.Image = AWS.Properties.Resources.Ski_trail_rating_symbol_blue_circle;
                    this.toolStripStatusLabel2.Text = "RX";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("setTXRX : " + ex.Message);
            }
        }

        private void checkAccessFile()
        {
            string folderName = AWSConfig.HOME_PATH + "\\AccessFile";

            if (!Directory.Exists(folderName))
                Directory.CreateDirectory(folderName);

            AccessDBManager.GetInstance().CreateMinDatabase(folderName + @"\" +  "aws.mdb");
			AccessDBManager.GetInstance().CreateMonthDatabase(folderName + @"\" + "aws_month.mdb");
		}

        private void makeDir()
        {
            while (isFlag)
            {
                if ((m_MakeFileDate.Day == DateTime.Now.Day) && (m_MakeFileDate.Hour == 22) && (m_MakeFileDate.Minute > 0))
                {
                    for (int i = 0; i < AWS.Config.AWSConfig.sCount; i++)
                    {
                        string folderName = AWSConfig.HOME_PATH + "\\AWSDATA";
                        string pathString = System.IO.Path.Combine(folderName, "SubFolder");

                        if (!Directory.Exists(folderName))
                            Directory.CreateDirectory(folderName);

                        string devName = AWS.Config.AWSConfig.sValue[i].Name;

                        folderName += @"\" + devName;

                        if (!Directory.Exists(folderName))
                            Directory.CreateDirectory(folderName);

                        String year = String.Format("{0:D4}", DateTime.Now.Year);

                        folderName += @"\" + year;

                        if (!Directory.Exists(folderName))
                            Directory.CreateDirectory(folderName);

                        String month = String.Format("{0:D2}", DateTime.Now.Month);
                        String day = String.Format("{0:D2}", DateTime.Now.Day);

                        folderName += @"\" + month;
                        if (!Directory.Exists(folderName))
                            Directory.CreateDirectory(folderName);

                        string accessFile = AWSConfig.HOME_PATH + "\\AccessFile\\aws.mdb";

                        DateTime today = DateTime.Now;

                        String todayAccessDBFile = folderName + @"\" + "aws_" + String.Format("{0:D4}", today.Year)
                            + String.Format("{0:D2}", today.Month) + String.Format("{0:D2}", today.Day) + ".mdb";

                        //오늘
                        if (!File.Exists(todayAccessDBFile))
                        {
                            System.IO.File.Copy(accessFile, todayAccessDBFile, true);
                        }

                        DateTime answer = today.AddDays(1);
                        String tomorrowAccessDBFile = folderName + @"\" + "aws_" + String.Format("{0:D4}", answer.Year)
                          + String.Format("{0:D2}", answer.Month) + String.Format("{0:D2}", answer.Day) + ".mdb";

                        //내일 access file 
                        if (!File.Exists(tomorrowAccessDBFile))
                        {
                            System.IO.File.Copy(accessFile, tomorrowAccessDBFile, true);
                        }

                        m_MakeFileDate = m_MakeFileDate.AddDays(+1);
                    }
                }
                Thread.Sleep(1000);
            }
        }

        private void DataSyncMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("데이터 동기화를 진행합니다.\r계속 하시겠습니까?", "진행 취소", MessageBoxButtons.YesNo) == DialogResult.No)
                return;

            DateTime currentTime = DateTime.Now;
            String fileName = String.Format("{0:yyyyMMdd}", currentTime);
            String year = String.Format("{0:yyyy}", currentTime);
            String month = String.Format("{0:MM}", currentTime);
            String day = String.Format("{0:dd}", currentTime);

            iLog.Debug("dev count : " + AWS.Config.AWSConfig.sCount);
            for (int i = 0; i < AWS.Config.AWSConfig.sCount; i++)
            {
                if (AWS.Config.AWSConfig.sValue[i].enable == false)
                {
                    iLog.Debug(i+ " DEV가 Disable 상태 입니다.");
                    continue;
                }

                new System.Threading.Thread((idx) => { logger[(int)idx].RecoverLostData(currentTime, true); } ).Start(i);

                Thread.Sleep(1000);
            }
        }

        private void PrintMenuItem_Click(object sender, EventArgs e)
        {

        }
    } 
}
