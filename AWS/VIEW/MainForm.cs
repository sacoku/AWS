//-----------------------------------------------------------------------
// <copyright file="MainForm.cs" company="[Company Name]">
//     Copyright (c) [Company Name] Corporation.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
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
using System.Reflection;
using System.Runtime.CompilerServices;

#pragma warning disable 0168

namespace AWS
{
    public partial class MainForm : Form
    {
        public DisplayForm displayForm = null;
        public ReportForm reportForm = null;
		public GraphForm graphForm = null;

        public DataLogger[] logger = null;
		public DataLogger[] rLogger = null;

		public HistoryPopup historyForm = null;

		//데이타를 수집하기 위한 스레드 객체
		public Thread makeThread = null;
        private bool isFlag = true;
        public System.DateTime m_MakeFileDate;
        ILog iLog = log4net.LogManager.GetLogger("Logger");

        //add by shkim
        public DisplayForm2 displayForm2 = null;
        private int loggerCnt = 0;

		private object lockobj = new object();

        public MainForm()
        {
            InitializeComponent();

			try
			{
				this.init();

				this.displayForm = new DisplayForm(this);
				this.reportForm = new ReportForm(this);
				this.displayForm2 = new DisplayForm2(this);
				this.graphForm = new GraphForm(this);
				this.historyForm = new HistoryPopup(this);

				this.displayForm.ControlBox = false;
				this.displayForm2.ControlBox = false;
				this.graphForm.ControlBox = false;

				this.displayForm.MdiParent = this;
				this.reportForm.MdiParent = this;
				this.displayForm2.MdiParent = this;
				this.graphForm.MdiParent = this;

				this.displayForm2.Show();

				this.displayForm2.Dock = DockStyle.Fill;
				this.reportForm.Dock = DockStyle.Fill;
				this.graphForm.Dock = DockStyle.Fill;

				startLogger();

				iLog.Info("Program Start");

				this.CenterToScreen();

			} catch(Exception e)
			{
				iLog.Error(e.ToString());
			}

		}

		/// <summary>
		/// 시스템 초기화 함수.
		/// </summary>
		/// <remarks>
		/// 2017.01.02 sacoku 최초 작성
		/// </remarks>

		public void init()
        {
			try
			{
				//REGISTRY에서 HOME_PATH를 Load
				AWSConfig.HOME_PATH = CommonUtil.ReadReg("HOME_PATH");
				iLog.Info(AWSConfig.HOME_PATH + "\\Config\\AWSConfig.xml");
				AWSConfig.Load(AWSConfig.HOME_PATH + "\\Config\\AWSConfig.xml");
			}
			catch (Exception e)
			{
				iLog.Error(e.Message);
				iLog.Error(e.StackTrace);

				//최초 실행 또는 다른 이유로 HOME_PATH가 존재 하지 않을 경우 HOME_PATH 설정
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
					}
					catch (Exception e1)
					{
						iLog.Debug(e1.ToString());
						System.Windows.Forms.Application.Exit();
					}

				}
				else
				{
					System.Windows.Forms.Application.Exit();
				}
			}

			try
			{
				//프로그램 시작을 위한 설정/db파일 확인 및 생성
				checkAccessFile();

				m_MakeFileDate = new System.DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 22, 1, 0);

				makeThread = new Thread(new ThreadStart(makeDir));
				makeThread.Name = "make file";
				makeThread.Start();

				Assembly assemObj = Assembly.GetExecutingAssembly();
				Version v = assemObj.GetName().Version; // 현재 실행되는 어셈블리..dll의 버전 가져오기

				int majorV = v.Major; // 주버전
				int minorV = v.Minor; // 부버전
				int buildV = v.Build; // 빌드번호
				int revisionV = v.Revision; // 수정번호
				this.Text = string.Format("AWS v{0}.{1}.{2}.{3} (LastBuild:{4})",
										majorV,
										minorV,
										buildV,
										revisionV,
										new DateTime(2017, 02, 06).AddDays(buildV).AddSeconds(revisionV * 2).ToString("yyyy-MM-dd"));

				//minor버젼이 홀수 일 경우 차트기능 enable
				if ((minorV % 2) == 0) toolStripButton3.Visible = false;
				else toolStripButton3.Visible = true;
			}
			catch(Exception ex)
			{
				iLog.Error(ex.ToString());
			}
		}

		public void startLogger()
		{
			try
			{
				loggerCnt = AWS.Config.AWSConfig.sCount;
				logger = new DataLogger[loggerCnt];
				rLogger = new DataLogger[loggerCnt];
				for (int i = 0; i < loggerCnt; i++)
				{
					if (AWS.Config.AWSConfig.sValue[i].enable)
					{
						Object lockObj = new object();
						logger[i] = new DataLogger(this, i, false, lockObj);
						if (AWSConfig.IS_REALTIME_RECOVERY)
						{
							rLogger[i] = new DataLogger(this, i, true, lockObj);
							rLogger[i].KMACatchComplete = new AWS.CONTROL.OnComplete(historyForm.OnComplete);
						}
					}

				}
			} catch(Exception ex)
			{
				iLog.Error(ex.ToString());
			}
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
                    this.displayForm2.Show();
                    this.displayForm2.BringToFront();                    
                    this.displayForm2.StartPosition = FormStartPosition.CenterScreen;
                }
            }
            catch (Exception ex) 
            {
                iLog.Error(ex.ToString());
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
                iLog.Error(ex.ToString());
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
                Debug.WriteLine(ex.ToString());
            }
        }
        
        /// <summary>
        /// 차트 표출 화면
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton5_Click(object sender, EventArgs e)
        {
			try
			{
				Form activeChild = this.ActiveMdiChild;

				if (activeChild.Name != "GraphForm")
				{
					this.graphForm.Show();
					this.graphForm.BringToFront();
					this.graphForm.StartPosition = FormStartPosition.CenterScreen;
				}
			}
			catch (Exception ex)
			{
				iLog.Error(ex.ToString());
			}
		}

		/// <summary>
		/// 데이터 복원 POPUP 표출
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void toolStripButton5_Click_1(object sender, EventArgs e)
		{
			try
			{
				//Form activeChild = this.ActiveMdiChild;

				historyForm.FormClosing += (s, ea) =>
				{
					base.OnFormClosing(ea);
					if (ea.CloseReason == CloseReason.UserClosing)
					{
						ea.Cancel = true;
						historyForm.Hide();
						historyForm.Stop();
					}
				};

				if (historyForm != null) historyForm.Show();
				
			}
			catch (Exception ex)
			{
				iLog.Error(ex.ToString());
			}
		}

		/// <summary>
		/// 설정 화면 표출
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void toolStripButton4_Click(object sender, EventArgs e)
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
				iLog.Error(ex.ToString());
			}
		}

		/// <summary>
		/// 화면 왼쪽 하단에 제어 전송 및 수신 상태를 보여주는 메소드
		/// </summary>
		/// <param name="status"></param>
		/// <param name="foreColor"></param>
//		[MethodImpl(MethodImplOptions.Synchronized)]
		public void displayStatus(String status, Color foreColor)
        {
            try
            {
				lock (lockobj)
				{
					this.toolStripStatusLabel.Text = "STATUS : " + status;
					this.toolStripStatusLabel.ForeColor = foreColor;
				}
            }
            catch (Exception ex)
            {
                iLog.Error(ex.ToString());
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

					if (AWSConfig.IS_REALTIME_RECOVERY && rLogger[i] != null)
						rLogger[i].CloseLogger();
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
				{
					Object lockObj = new object();
					this.logger[i] = new DataLogger(this, i, true, lockObj);
					if (AWSConfig.IS_REALTIME_RECOVERY)
					{
						rLogger[i] = new DataLogger(this, i, true, lockObj);
					}
				}
            }
           
        }

//		[MethodImpl(MethodImplOptions.Synchronized)]
/*
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
*/

        private void checkAccessFile()
        {
			try
			{
				string folderName = AWSConfig.HOME_PATH + "\\AccessFile";

				if (!Directory.Exists(folderName))
					Directory.CreateDirectory(folderName);

				AccessDBManager.GetInstance().CreateMinDatabase(folderName + @"\" + "aws.mdb");
				AccessDBManager.GetInstance().CreateMonthDatabase(folderName + @"\" + "aws_month.mdb");
			} catch(Exception ex)
			{
				iLog.Error(ex.ToString());
			}
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
	} 
}
