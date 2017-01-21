using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

using AWS.MODEL;
using System.IO;
using System.Collections;
using System.Drawing;
using log4net;
using AWS.CONTROLS;
using System.Net.Sockets;
using System.Net;
using System.Data.OleDb;
using System.Data;
using AWS.Config;

namespace AWS.CONTROL
{
    public class DataLogger
    {   
        static ILog iLog = log4net.LogManager.GetLogger("Logger");
        public DataLoggerEnvironment environment = null;
        public SerialPort comPort = null;

        public bool isCommand = false;

        //데이타를 받아서 처리하는 객체
        private Protocol protocol = null;
        public MainForm mainForm = null;
        //기상 프로토콜을 수신하면 파일에 저장하는 객체 
        private SaveData saveData = null;
        public System.DateTime m_DateTimeCommandDt;
        public System.DateTime m_CollectDt;
        //데이타를 수집하기 위한 스레드 객체
        public Thread m_CollectThread = null;

        public ushort newPassword = 0;
        public ushort newID = 0;
        public bool flag = true;

        private AsynchronousSocket ClientSocket = null;
        private Socket client = null;
        private IPEndPoint remoteEP = null;
        private IPHostEntry ipHostInfo = null;
        private Boolean bRealTimeRecovery = false;

        //데이타를 수집하기 위한 스레드 객체
        public Thread CollectThread = null;
        
        int iPanelIdx = -1;
        int iRetryConnCnt = 0;
        Boolean isPause = false;
        Boolean isCurrRequest = false;
        Boolean isLostRequest = false;
        Boolean bWatchdogIsRun = true;
		Boolean bIsReadyToRun = false;
        Thread thWatchDog = null;

        private object lockObject = new object();

        public DataLogger(MainForm main, int idx)
        {
            this.mainForm = main;
            this.environment = new DataLoggerEnvironment();

            this.protocol = new Protocol();
            this.saveData = new SaveData();

            protocol.AddProtocolItem(Marshal.SizeOf(typeof(KMAAnswer2)), true, new CheckFunction(HeaderTailCheck2), new CatchFunction(AnswerProtocolCatch2));	// 응답 프로토콜
            //로거 상태
            protocol.AddProtocolItem(Marshal.SizeOf(typeof(LoggerVersionInfo)), true, new CheckFunction(KMALoggerInfoHeaderTailCheck), new CatchFunction(KMALoggerInfoHeaderTailCatch));	// 응답 프로토콜
          
            protocol.AddProtocolItem(Marshal.SizeOf(typeof(KMA2)), true, new CheckFunction(KMA2HeaderTailCheck), new CatchFunction(KMA2Catch)); // WeatherProtocol

			//remove by sacoku
            //environment.IP = Properties.Settings.Default.IP.ToString();
            //environment.PORT = int.Parse(Properties.Settings.Default.PORT.ToString());
            //environment.LoggerID = ushort.Parse(Properties.Settings.Default.LOGGERID.ToString());
            //environment.PASSWORD = Properties.Settings.Default.PASSWORD.ToString();

            environment.IP = AWS.Config.AWSConfig.sValue[idx].Ip;
            environment.PORT = AWS.Config.AWSConfig.sValue[idx].Port;
            environment.LoggerID = AWS.Config.AWSConfig.sValue[idx].Id;
            environment.PASSWORD = AWS.Config.AWSConfig.sValue[idx].Passwd;

            bRealTimeRecovery = AWSConfig.IS_REALTIME_RECOVERY;

            this.iPanelIdx = idx;
            try
            {
                IPAddress ipAddress = IPAddress.Parse(environment.IP);
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, environment.PORT);
            
                // Create a TCP/IP socket.
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                ClientSocket = new AsynchronousSocket(client, remoteEP);
                ClientSocket.Received += new ReceiveDelegate(OnReceived);
            }
            catch (Exception ex)
            {
                //this.mainForm.displayStatus("컴포트 오픈 실패!", Color.Red);
                this.mainForm.displayStatus("[I:"+iPanelIdx+"] 접속 실패!", Color.Red);
                iLog.Error("[I: "+iPanelIdx+"][ERROR] 접속 실패 : " + ex.Message);
            }

            try
            {
                ClientSocket.Disconnected += new DisconnectDelegate(OnDisconnected);
            }
            catch (Exception E)
            {
                iLog.Error("[I: " + iPanelIdx + "][ERROR] : " + E.Message);
            }
                  

            m_CollectDt = new System.DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 35);
            m_DateTimeCommandDt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 30);          // 각시간당 20초에 시간 설정 명령을 보내기 위해서 설정

            try
            {
				Thread.Sleep(5000);	//Delay를 주고 시작한다.
				CollectThread = new Thread(new ThreadStart(StartCollect));
                CollectThread.Name = "collectDataForOWI";
                CollectThread.Start();
            }
            catch (Exception E)
            {
                // 접속이 갑자기 끊겼다 붙었다 하면 여기서 에러가 발생하여 리소스가 증가한다. 예외처리.. hanji
                if (E.Message == "현재 연결은 원격 호스트에 의해 강제로 끊겼습니다")
                {

                }

                iLog.Error("[I: " + iPanelIdx + "][ERROR] : " + E.Message);
            }

			try
			{
				//add by sacoku - 실시간 데이터 복구 모드 추가
				if (bRealTimeRecovery)
				{
					iLog.Info("복구 모드가 실시간으로 동작합니다.");
					StartWatchDog();
				}
			}
			catch(Exception e)
			{
				iLog.Error("[I: " + iPanelIdx + "][ERROR] : " + e.Message);
			}
        }

        public void OnDisconnected(object sender, EventArgs e)
        {
            try
            {
                ClientSocket.Close();

                reConnect();
            }
            catch (Exception E)
            {
                Debug.WriteLine(E.ToString());
            }

        }

        private void reConnect()
        {
            IPAddress ipAddress = IPAddress.Parse(environment.IP);
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, environment.PORT);

            iLog.Info("[I: " + iPanelIdx + "] " + (iRetryConnCnt + 1) + "번째 재접속 합니다");
            // Create a TCP/IP socket.
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            ClientSocket = new AsynchronousSocket(client, remoteEP);
            ClientSocket.Received += new ReceiveDelegate(OnReceived);

            iRetryConnCnt++;
        }

        private void makeReportFile()
        {

        }

        public byte[] StrToByteArray(string str)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            return encoding.GetBytes(str);
        }

        /// <summary>
        /// KMA2 데이타 요청
        /// </summary>
        public void StartCollect()
        {
            int nFailCnt = 0;

            while (flag)
            {
                //add by sacoku 161227
                if(!ClientSocket.Connected())
                {
                    if (iRetryConnCnt > 5)
                    {
                        iLog.Error("재접속 횟수가 10회를 초과하여 접속 시도를 종료합니다.");
                        this.CloseLogger();
                        return;
                    }

                    reConnect();
                    Thread.Sleep(5000);
                    continue;
                }
                
                if (isPause)
                {
                    Thread.Sleep(5000);
                    continue;
                }

                iRetryConnCnt = 0;
                bool bFlag = false;
                if (m_DateTimeCommandDt < DateTime.Now)
                {
                    m_DateTimeCommandDt = DateTime.Now.AddHours(1);
                    DateTime SyncDateTime = DateTime.Now;              

                    this.SendCommand(SyncDateTime, this.StrToByteArray("AT?"));
                    iLog.Info("[I: " + iPanelIdx + "][MESSAGE TO LOGGER] TIMESYNC MESSAGE");
                    bFlag = true;
                }

                // 현재 자료 요구
                if (bFlag == false)
                {
                    try
                    {
                        // 현재 자료를 요구 할때 만약 1분 이상시간이 지나면 시간을 바꾸어서 새로운 자료를 요구한다.
                        //System.TimeSpan tSpan = new TimeSpan(0, 0, 1, 0, 0);

                        if ((m_CollectDt.Minute <= DateTime.Now.Minute) && (DateTime.Now.Second > 30))
                        {
                            m_CollectDt = new System.DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 35);

                            // 현재 데이터를 요구한다
                            this.WeatherSendCommand(m_CollectDt, this.StrToByteArray("AB?"));
                            iLog.Info( "[I: " 
								     + iPanelIdx 
									 + "][MESSAGE TO LOGGER] CURRENT DATA CALL " 
									 + m_CollectDt.Year 
									 + "/" 
									 + m_CollectDt.Month 
									 + "/" 
									 + m_CollectDt.Day 
									 + " " 
									 + m_CollectDt.Hour 
									 + ":" 
									 + m_CollectDt.Minute + " 데이터 요청");

                            isCurrRequest = true;

							Thread.Sleep(1000);
							while (isCurrRequest == true)
                            {   
                                if (nFailCnt > 60)
                                {
                                    iLog.Info("[I: " + iPanelIdx + "] 현재 데이터 요청 응답이 없습니다. 재시도 합니다..");
                                    nFailCnt = 0;
                                    isCurrRequest = false;
                                    break;
                                } else
                                    nFailCnt++;

								Thread.Sleep(1000);
							}
                        }

						bIsReadyToRun = true;
					}
                    catch (Exception E)
                    {
                        iLog.Error("[I: " + iPanelIdx + "][ERROR] DataLogger : StartCollect2 " + E.Message);
                    }

                    iLog.Info(    "[I: " + iPanelIdx 
								+ "] m_CollectDt TIME : " 
								+ m_CollectDt.Year 
								+ "/" 
								+ m_CollectDt.Month 
								+ "/" 
								+ m_CollectDt.Day 
								+ " " 
								+ m_CollectDt.Hour 
								+ ":" 
								+ m_CollectDt.Minute 
								+ ":" + m_CollectDt.Second);

                    bFlag = true;
                } else
                {
                    iLog.Info("요청 대기중입니다.");
                }
                Thread.Sleep(AWS.Config.AWSConfig.CDP * 1000);
            }
        }

        public DateTime GetNowDateTimeSecZero()
        {
            return new System.DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
        }

        private void OnReceived(object sender, ReceivedEventArgs e)
        {

            for (int i = 0; i < e.Data.Length; i++)
            {
                this.protocol.ProtocolProcessing(e.Data[i]);
            }
        }

        public void SendCommand(DateTime Dt, byte[] command)
        {
            SendCommand((byte)(Dt.Year - 2000), (byte)Dt.Month, (byte)Dt.Day, (byte)Dt.Hour, (byte)Dt.Minute, (byte)Dt.Second, (ushort)this.environment.LoggerID, (ushort)1111, command);
        }


        public void SendCommandPassword(DateTime Dt, ushort Password, byte[] command)
        {
            try
            {
                System.DateTime TempDT;
                byte year = 0, month = 0, day = 0, hour = 0, minute = 0, second = 0;
                if (Dt.Hour == 0 && Dt.Minute == 0 && Dt.Second == 0)
                {
                    TempDT = Dt - new System.TimeSpan(0, 1, 0, 0, 0);
                    year = (byte)(TempDT.Year - 2000);
                    month = (byte)TempDT.Month;
                    day = (byte)TempDT.Day;
                    hour = (byte)24;
                    minute = (byte)0;
                    second = (byte)0;
                }
                else
                {
                    year = (byte)(Dt.Year - 2000);
                    month = (byte)Dt.Month;
                    day = (byte)Dt.Day;
                    hour = (byte)Dt.Hour;
                    minute = (byte)Dt.Minute;
                    second = (byte)Dt.Second;
                }

                SendCommand(year, month, day, hour, minute, second, (ushort)this.environment.LoggerID, (ushort)Password, command);
            }
            catch (Exception E)
            {
                iLog.Error("[I: " + iPanelIdx + "][ERROR] : " + E.Message);
            }
        }

        public void SendCommnad(DateTime Dt, byte[] command)
        {
            try
            {
                System.DateTime TempDT;
                byte year = 0, month = 0, day = 0, hour = 0, minute = 0, second = 0;

                if (Dt.Hour == 0 && Dt.Minute == 0 && Dt.Second == 0)
                {
                    TempDT = Dt - new System.TimeSpan(0, 1, 0, 0, 0);
                    year = (byte)(TempDT.Year - 2000);
                    month = (byte)TempDT.Month;
                    day = (byte)TempDT.Day;
                    hour = (byte)24;
                    minute = (byte)0;
                    second = (byte)0;
                }
                else
                {
                    year = (byte)(Dt.Year - 2000);
                    month = (byte)Dt.Month;
                    day = (byte)Dt.Day;
                    hour = (byte)Dt.Hour;
                    minute = (byte)Dt.Minute;
                    second = (byte)Dt.Second;
                }

                SendCommand(year, month, day, hour, minute, second, this.ByteChange(this.environment.LoggerID), this.ByteChange(this.environment.LoggerID), command);
            }
            catch (Exception E)
            {
                iLog.Error("[I: " + iPanelIdx + "][ERROR] : " + E.Message);
            }
        }

        public void SendCommandLoggerID(DateTime Dt, ushort LoggerID, byte[] command)
        {
            try
            {
                System.DateTime TempDT;
                byte year = 0, month = 0, day = 0, hour = 0, minute = 0, second = 0;

                if (Dt.Hour == 0 && Dt.Minute == 0 && Dt.Second == 0)
                {
                    TempDT = Dt - new System.TimeSpan(0, 1, 0, 0, 0);
                    year = (byte)(TempDT.Year - 2000);
                    month = (byte)TempDT.Month;
                    day = (byte)TempDT.Day;
                    hour = (byte)24;
                    minute = (byte)0;
                    second = (byte)0;
                }
                else
                {
                    year = (byte)(Dt.Year - 2000);
                    month = (byte)Dt.Month;
                    day = (byte)Dt.Day;
                    hour = (byte)Dt.Hour;
                    minute = (byte)Dt.Minute;
                    second = (byte)Dt.Second;
                }

                SendCommand(year, month, day, hour, minute, second, this.ByteChange(this.environment.LoggerID), LoggerID, command);
            }
            catch (Exception E)
            {
                iLog.Error("[I: " + iPanelIdx + "][ERROR] : " + E.Message);
            }
        }
        
        public void WeatherSendCommand(DateTime Dt, byte[] command)
        {
            try
            {
                System.DateTime TempDT;
                byte year = 0, month = 0, day = 0, hour = 0, minute = 0, second = 0;

                // 10초 단위에 데이터를 불러오기 때문에 초가 항상 10초이다.
                // 그래서 초단위는 계산하지 않는다.                
                if (Dt.Hour == 0 && Dt.Minute == 0)
                {
                    TempDT = Dt - new System.TimeSpan(0, 1, 0, 0, 0);
                    year = (byte)(TempDT.Year - 2000);
                    month = (byte)TempDT.Month;
                    day = (byte)TempDT.Day;
                    hour = (byte)24;
                    minute = (byte)0;
                    second = (byte)0;
                }
                else
                {
                    year = (byte)(Dt.Year - 2000);
                    month = (byte)Dt.Month;
                    day = (byte)Dt.Day;
                    hour = (byte)Dt.Hour;
                    minute = (byte)Dt.Minute;
                    second = (byte)0;
                }

                SendCommand(year, month, day, hour, minute, second, this.environment.LoggerID, ushort.Parse(this.environment.PASSWORD), command);
            }
            catch (Exception E)
            {
                iLog.Error("[I: " + iPanelIdx + "][ERROR] : " + E.Message);
            }
        }
        
        public void SendCommand(byte year, byte month, byte day, byte hour, byte minute, byte second, ushort loggerID, ushort password, byte[] command)
        {
            try
            {
                lock (lockObject)
                {

                    KMACommand2 protocolCommand = new KMACommand2();
                    protocolCommand.Header[0] = 0xFA;
                    protocolCommand.Header[1] = 0xFB;
                    protocolCommand.Tail[0] = 0xFF;
                    protocolCommand.Tail[1] = 0xFE;
                    protocolCommand.Year = year;
                    protocolCommand.Month = month;
                    protocolCommand.Day = day;
                    protocolCommand.Hour = hour;
                    protocolCommand.Minute = minute;
                    protocolCommand.Second = second;
                    protocolCommand.LoggerID = ByteChange((ushort)loggerID);
                    protocolCommand.Password = ByteChange((ushort)password);
                    protocolCommand.Command[0] = command[0];
                    protocolCommand.Command[1] = command[1];
                    protocolCommand.Command[2] = command[2];

                    byte[] datas = protocolCommand.GetByte();
                    datas[25] = 0x00;
                    datas[26] = 0x00;

                    for (int i = 2; i < datas.Length - 4; i++)
                    {
                        datas[25] ^= datas[i];
                        datas[26] += datas[i];
                    }

                    if (this.ClientSocket.Connected())
                    {
                        //AWS.UTIL.CommonUtil.DumpBytes(datas, dumpbytes);
                        ClientSocket.Send(datas);
                    }
                    else
                        reConnect();


                    this.mainForm.setTXRX(1);
                    // 현재 데이터를 요구한다
                    string[] data = new string[2];
                    data[0] = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");

                    string test = Encoding.ASCII.GetString(command);

                    if (Encoding.ASCII.GetString(command) == "AI?")
                    {
                        data[1] = "순간 자료 요청";
                    }
                    else if (Encoding.ASCII.GetString(command) == "AB?")
                    {
                        data[1] = "데이터로거에 1분 자료 요청";
                    }
                    else if (Encoding.ASCII.GetString(command) == "AR?")
                    {
                        data[1] = "데이터로거에 리셋 요청";
                    }
                    else if (Encoding.ASCII.GetString(command) == "AD?")
                    {
                        data[1] = "데이터로거에 지점번호 수정 요청";
                    }
                    else if (Encoding.ASCII.GetString(command) == "AT?")
                    {
                        data[1] = "데이터로거에 시간 동기화 요청";
                    }
                    else if (Encoding.ASCII.GetString(command) == "AW?")
                    {
                        data[1] = "데이터로거에 패스워드 수정 요청";
                    }
                    else if (Encoding.ASCII.GetString(command) == "AC?")
                    {
                        data[1] = "데이터로거에 버퍼 클리어 요청";
                    }
                    else if (Encoding.ASCII.GetString(command) == "AV?")
                    {
                        data[1] = "데이터로거에 정보 요청";
                    }
                    else if (Encoding.ASCII.GetString(command) == "AQ?")
                    {
                        data[1] = "데이터로거에 과거자료 요청";
                    }

                    this.mainForm.displayStatus("[I: " + iPanelIdx + "]" + data[1], Color.Red);
                    iLog.Debug("[I: " + iPanelIdx + "]" + data[0] + " " + data[1]);
                }
            }
            catch (Exception e)
            {
                iLog.Error("[I: " + iPanelIdx + "][ERROR] : SendCommand " + e.Message);
                if (!this.ClientSocket.Connected())
                    reConnect();
            }
        }

        /// <summary>
        /// CheckSumCheck 생성
        /// </summary>
        public void CreateCheckSum(byte[] data, int HeaderSize, int TailSize)
        {
            data[data.Length - TailSize - 2] = 0;	// Xor
            data[data.Length - TailSize - 1] = 0;	// Add
            for (int i = 0 + HeaderSize; i < data.Length - TailSize - 2; i++)
            {
                data[data.Length - TailSize - 2] ^= data[i];
                data[data.Length - TailSize - 1] += data[i];
            }
        } 


        public bool KMALoggerInfoHeaderTailCheck(object sender, CircleQueue queue)
        {
            if (queue.Buffer[(queue.Sp + 6 + queue.Size) % queue.Size] != 0x20) return false;
            if (queue.Buffer[(queue.Sp - 3 + queue.Size) % queue.Size] != 0x20) return false;		// Header Check
            if (queue.Buffer[(queue.Sp - 2 + queue.Size) % queue.Size] != 0x20) return false;
            if (queue.Buffer[(queue.Sp - 1 + queue.Size) % queue.Size] != 0x20) return false;

            return true;
        }

        /// <summary>
        /// HeaderTailCheck 함수
        /// </summary>
        public bool HeaderTailCheck2(object sender, CircleQueue queue)
        {
            if (queue.Buffer[(queue.Sp + 0 + queue.Size) % queue.Size] != 0xFA) return false;		// Header Check
            if (queue.Buffer[(queue.Sp + 1 + queue.Size) % queue.Size] != 0xFB) return false;
            if (queue.Buffer[(queue.Sp - 1 + queue.Size) % queue.Size] != 0xFE) return false;		// Header Tail
            if (queue.Buffer[(queue.Sp - 2 + queue.Size) % queue.Size] != 0xFF) return false;
            return true;
        }

        public bool KMA2HeaderTailCheck(object sender, CircleQueue queue)
        {
            if (queue.Buffer[(queue.Sp + 0 + queue.Size) % queue.Size] != 0xFA) return false;		// Header Check
            if (queue.Buffer[(queue.Sp + 1 + queue.Size) % queue.Size] != 0xFB) return false;
            if (queue.Buffer[(queue.Sp - 1 + queue.Size) % queue.Size] != 0xFE) return false;		// Header Tail
            if (queue.Buffer[(queue.Sp - 2 + queue.Size) % queue.Size] != 0xFF) return false;
            return true;
        }

        public bool ReseponseHeaderTailCheck(object sender, CircleQueue queue)
        {
            if (queue.Buffer[(queue.Sp + 0 + queue.Size) % queue.Size] != (byte)'O')
                return false;		// Header Check
            if (queue.Buffer[(queue.Sp + 1 + queue.Size) % queue.Size] != (byte)'K')
                return false;
            if (queue.Buffer[(queue.Sp - 2 + queue.Size) % queue.Size] != (byte)'A')
                return false;		// Header Tail
            if (queue.Buffer[(queue.Sp - 1 + queue.Size) % queue.Size] != (byte)'Y')
                return false;
            return true;
        }

        public bool ReseponseProtocolCatch(object sender, byte[] Data)
        {
            string result = Encoding.ASCII.GetString(Data);
            string[] data = new string[2];
            data[0] = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            data[1] = result + " 수신";

            this.mainForm.displayStatus(result, Color.Blue);

            return true;
        }

        public bool KMALoggerInfoHeaderTailCatch(object sender, byte[] Data)
        {
            string[] data = new string[2];
            data[0] = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            data[1] = " 데이터로거 정보 수신";

            return true;

        }

        /// <summary>
        /// 응답 프로토콜을 받았을때의 처리
        /// </summary>
        public bool AnswerProtocolCatch2(object sender, byte[] Data)
        {
            try
            {
                KMAAnswer2 protocolAnswer = new KMAAnswer2();
                protocolAnswer = KMAAnswer2.SetByte(Data);

                string[] data = new string[2];
               
                // 응답 결과를 확인한다.(OKAY가 오면 성공)
                if (protocolAnswer.Answer[0] == (byte)'O' || protocolAnswer.Answer[1] == (byte)'K' || protocolAnswer.Answer[2] == (byte)'A' || protocolAnswer.Answer[3] == (byte)'Y')
                {
                    if (protocolAnswer.Type == (byte)'D') //ID 세팅 성공
                    {
                        string original;

                        data[0] = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                        data[1] = "지점번호를 성공적으로 수정했습니다.";
                        this.mainForm.displayStatus(data[1], Color.Red);
                                     
                        original = this.environment.LoggerID.ToString();

                        ushort tempID = this.environment.LoggerID;
                        this.environment.LoggerID = newID;

                        Properties.Settings.Default.LOGGERID = newID.ToString();
                        Properties.Settings.Default.Save();
                    }
                    else if (protocolAnswer.Type == (byte)'W') //패스워드 세팅 성공
                    {
                        data[0] = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                        data[1] = "패스워드가 성공적으로 수정되었습니다.";
                        this.environment.PASSWORD = this.newPassword.ToString();
                        this.mainForm.displayStatus(data[1], Color.Blue);

                        Properties.Settings.Default.PASSWORD = this.newPassword.ToString();
                        Properties.Settings.Default.Save();
                    }
                    else if (protocolAnswer.Type == (byte)'T') //날짜 시간 세팅 성공
                    {
                        data[0] = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                        data[1] = "시간이 성공적으로 수정되었습니다.";
                        this.mainForm.displayStatus(data[1], Color.Blue);
                    }
                    else if (protocolAnswer.Type == (byte)'R') //날짜 시간 세팅 성공
                    {
                        data[0] = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                        data[1] = "데이터로거를 성공적으로 재시작했습니다.";
                        this.mainForm.displayStatus(data[1], Color.Blue);
                    }
                    else if (protocolAnswer.Type == (byte)'C') //날짜 시간 세팅 성공
                    {
                        data[0] = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                        data[1] = "데이터로거의 버퍼를 성공적으로 클리어했습니다.";
                        this.mainForm.displayStatus(data[1], Color.Blue);
                    }
                }
                this.mainForm.displayStatus("[I: " + iPanelIdx + "]" + data[1], Color.Blue);
                this.mainForm.setTXRX(0);
                isCommand = false;

                iLog.Debug("[I: " + iPanelIdx + "]" + data[0] + " " + data[1]);
            }
            catch (Exception E)
            {
                iLog.Info("[I: " + iPanelIdx + "][ERROR] DataLogger : AnswerProtocolCatch2 " + E.Message);
                return false;
            }

            return true;
        }

        public void CloseLogger()
        {
            this.flag = false;
            bWatchdogIsRun = false;
            if(thWatchDog != null) thWatchDog.Join();
            if (this.ClientSocket != null)
                this.ClientSocket.Close();
        }     
    

        public bool KMA2Catch(object sender, byte[] Data)
        {
            bool bResult = false;

            try
            {
                this.saveData.kma2 = new KMA2(); // 기상데이터 프로토콜 처리 클래스 생성
                saveData.kma2 = KMA2.SetByte(Data); // 기상데이터 프로토콜에 데이터를 넣는다.  

                iLog.Debug(   " 데이터 저장 [" 
							+ ((int)saveData.kma2.Year) 
							+ "/" 
							+ (int)saveData.kma2.Month 
							+ "/" 
							+ (int)saveData.kma2.Day 
							+ "/" 
							+ (int)saveData.kma2.Hour 
							+ "/" 
							+ (int)saveData.kma2.Minute 
							+ "]");

                DateTime receive = new DateTime( 2000 + ((int)saveData.kma2.Year)
												, (int)saveData.kma2.Month
												, (int)saveData.kma2.Day
												, (int)saveData.kma2.Hour
												, (int)saveData.kma2.Minute, 0);

                //현재데이터
                if (   (receive.Year == DateTime.Now.Year) 
					&& (receive.Month == DateTime.Now.Month) 
					&& (receive.Day == DateTime.Now.Day)
                    && (receive.Hour == DateTime.Now.Hour) 
					&& (receive.Minute == DateTime.Now.Minute) )
				{
					isCurrRequest = false;
					this.mainForm.setTXRX(0);
                    this.m_CollectDt += new TimeSpan(0, 0, 1, 0, 0);
                    String[] dispalyData = new String[2];

                    KMA2 displayKMA2 = new KMA2();
                    displayKMA2 = KMA2.SetByte(Data);

                    // 데이터를 받으면 무조건 시간을 증가 시킨다.
                    String resultData = this.saveData.getResult(iPanelIdx);

                    String[] receivedTime = resultData.Split(',');

                    dispalyData[0] = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                    dispalyData[1] = receivedTime[1] + "현재 데이터 수신";

                    iLog.Info("[I: " + iPanelIdx + "][MESSAGE FROM LOGGER] " + receivedTime[1] + " 현재 데이터 수신");

                    this.mainForm.displayStatus("[I: " + iPanelIdx + "]" + dispalyData[0] + " " + dispalyData[1], Color.Blue);

                    //SafeInvokeHelper.Invoke(this.mainForm.displayForm, "DisplayData", displayKMA2);
                    SafeInvokeHelper.Invoke(this.mainForm.displayForm2, "DisplayData", displayKMA2, iPanelIdx);

                    Thread WeatherProcThread = new Thread(new ThreadStart(saveData.SaveWeatherData));
                    WeatherProcThread.Name = "WeatherThread";
                    WeatherProcThread.IsBackground = true;
                    WeatherProcThread.Start();
                } 
                else
                {
					isLostRequest = false;
					this.saveData.kma2 = new KMA2(); // 기상데이터 프로토콜 처리 클래스 생성
                    saveData.lastKma2 = KMA2.SetByte(Data); // 기상데이터 프로토콜에 데이터를 넣는다.  
                    saveData.ByteChangAllLastData();

                    // 데이터를 받으면 무조건 시간을 증가 시킨다.
                    String resultData = this.saveData.getResult(iPanelIdx);
                    String[] dispalyData = new String[2];
                    String[] receivedTime = resultData.Split(',');

                    String result = string.Format("{0:00}", saveData.lastKma2.Year + 2000) 
								  + "/" 
								  + string.Format("{0:00}", saveData.lastKma2.Month) 
								  + "/" 
								  + string.Format("{0:00}", saveData.lastKma2.Day) 
								  + " " 
								  +  string.Format("{0:00}", saveData.lastKma2.Hour) 
								  + ":" 
								  + string.Format("{0:00}", saveData.lastKma2.Minute);

                    dispalyData[0] = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                    dispalyData[1] = result + "과거 데이터 수신";

                    iLog.Info("[I: " + iPanelIdx + "][MESSAGE FROM LOGGER] " + result + " 과거 데이터 수신");

                    Thread WeatherProcThread = new Thread(new ParameterizedThreadStart(saveData.saveLastData));
                    WeatherProcThread.Name = "WeatherThread";
                    WeatherProcThread.IsBackground = true;
                    WeatherProcThread.Start(iPanelIdx);
                }

                bResult = true;
            }
            catch (Exception E)
            {
                iLog.Error("[I: " + iPanelIdx + "][ERROR] DataLogger : KMA2Catch " + E.Message);
                bResult = false;
            }
            
            return bResult;
        }

        public ushort ByteChange(ushort data)
        {
            byte[] Temp = System.BitConverter.GetBytes(data);
            byte[] Temp2 = new byte[2];
            Temp2[0] = Temp[1];
            Temp2[1] = Temp[0];

            return System.BitConverter.ToUInt16(Temp2, 0);
        }

        public void Pause()
        {
            isPause = true;
            iLog.Info(iPanelIdx + " 장치가 Pause 상태입니다.");
        }

        public void Resume()
        {
            isPause = false;
            iLog.Info(iPanelIdx + " 장치가 Resume 상태입니다.");
        }

        public void StartWatchDog()
        {
            bWatchdogIsRun = true;

			thWatchDog = new Thread((idx) => 
            {
                while (bWatchdogIsRun)
                {
					if (bIsReadyToRun)
					{
						DateTime currentTime = DateTime.Now;
						RecoverLostData(currentTime, false);
					}
                    Thread.Sleep(1000);
                }
            });

			thWatchDog.Start();
        }

        public void RecoverLostData(DateTime dt, Boolean isPauseMode)
        {
            int nFailCnt = 0;
            OleDbConnection con = null;
            OleDbCommand cmd = null;

            String fileName = String.Format("{0:yyyyMMdd}", dt);
            String year = String.Format("{0:yyyy}", dt);
            String month = String.Format("{0:MM}", dt);
            String day = String.Format("{0:dd}", dt);

            try
            {
                string DBPath = AWSConfig.HOME_PATH + "\\AWSDATA\\" + AWSConfig.sValue[(int)iPanelIdx].Name + "\\" + year + "\\" + month + "\\aws_" + fileName + ".mdb";
                if (!File.Exists(DBPath))
                {
                    MessageBox.Show(DBPath + " Date file isn't exist!");
                    return;
                }

                iLog.Info("Watchdog을 수행합니다.");
                DataSet readDataSet = new DataSet();

				StringBuilder selectQuery = new StringBuilder()
									.Append("SELECT										\n")
									.Append("         RECEIVETIME						\n")
									.Append("        ,TEMP								\n")
									.Append("        ,WD								\n")
									.Append("        ,WS								\n")
									.Append("        ,RAIN								\n")
									.Append("        ,ISRAIN							\n")
									.Append("        ,HUMIDITY							\n")
									.Append("        ,SUNSHINE							\n")
									.Append("        ,VISIBILITY						\n")
									.Append("FROM AWS_MIN								\n")
									.Append("WHERE DEV_IDX = ").Append(iPanelIdx.ToString());

                iLog.Debug(selectQuery);
                con = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + DBPath);
                cmd = new OleDbCommand(selectQuery.ToString(), con);
                OleDbDataAdapter myDataAdapter = new OleDbDataAdapter(cmd);

                con.Open();
                myDataAdapter.Fill(readDataSet, "aws_min");

                if (readDataSet.Tables[0].Rows.Count > 0)
                {

                    DateTime startDateTime = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);
                    TimeSpan ts = dt - startDateTime;
                    int lostCnt = 0;

                    if(isPauseMode) Pause();

                    for (int rows = 0; rows < (ts.TotalMinutes + 1); rows++)
                    {
						if (bWatchdogIsRun == false) break;
                        DataRow[] result = readDataSet.Tables[0].Select("receivetime = '" + startDateTime + "'");

                        if (result == null || result.Length <= 0)
                        {
                            SendCommand(startDateTime, AWS.UTIL.CommonUtil.StrToByteArray("AQ?"));
                            isLostRequest = true;
                            iLog.Info(    "[I: " + iPanelIdx 
										+ "][MESSAGE TO LOGGER] LOST DATA CALL " 
										+ startDateTime.Year 
										+ "/" 
										+ startDateTime.Month 
										+ "/" 
										+ startDateTime.Day 
										+ " " 
										+ startDateTime.Hour 
										+ ":" + startDateTime.Minute 
										+ " 데이터 요청");

							Thread.Sleep(1000);
							while (isLostRequest == true)
                            {
                                if (nFailCnt >= 60)
                                {
                                    iLog.Info("[I: " + iPanelIdx + "] 과거자료 요청 재시도 합니다.");
                                    nFailCnt = 0;
									break;
                                }
                                else
                                    nFailCnt++;

								Thread.Sleep(1000);
							}
                        }

						if (isLostRequest == false)
						{
							startDateTime = startDateTime.AddMinutes(+1);
							lostCnt++;
						}
                    }

                    if(isPauseMode) Resume();
                }
            }
            catch (Exception e)
            {
                iLog.Error(e.Message);
                iLog.Error(e.StackTrace.ToString());
                throw e;
            } 
            finally
            {
                con.Close();
            }
        }
    }
}
