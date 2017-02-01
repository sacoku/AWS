using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.IO;
using log4net;

namespace AWS.CONTROLS
{
    // 델리게이트(delegate) 선언
    public delegate void DisconnectDelegate(object sender, EventArgs e);		// 접속해제시 Delegate
    public delegate void ReceiveDelegate(object sender, ReceivedEventArgs e);	// 데이터 받을시에 Delegate
    
    // MessageEventArgs 정의
    public class ReceivedEventArgs : EventArgs
    {   
        private byte[] m_Buffer = null;

        public byte[] Data
        {
            get { return m_Buffer; }
            set { m_Buffer = value; }
        }

        public ReceivedEventArgs()
            : base()
        {
        }

        public ReceivedEventArgs(byte[] data)
            : base()
        {
            m_Buffer = data;
        }
    }


    public class AsynchronousSocket
    {
        public const int BufferSize = 1024;		// Size of receive buffer.
        public Socket workSocket = null;		// Client  socket.
        private byte[] buffer = null; 			// Receive buffer.
        
        public event DisconnectDelegate Disconnected = null;		// 접속해제시 이벤트
        public event ReceiveDelegate Received = null;       // 데이터 받았을때 이벤트
        private IPEndPoint iep = null;

		ILog iLog = null;

        public AsynchronousSocket(Socket sock, IPEndPoint remoteEP, int idx)
        {
			iLog = log4net.LogManager.GetLogger("Dev" + idx);

			buffer = new byte[BufferSize];
            workSocket = sock;
            workSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);

            iep = remoteEP;
            workSocket.BeginConnect(remoteEP, new AsyncCallback(Connected), sock);
        }

        private void Connected(IAsyncResult iar)
        {
            workSocket = (Socket)iar.AsyncState;
            try
            {
                workSocket.EndConnect(iar);

                // 접속된 소켓에 비동기 Receive 설정
                this.workSocket.BeginReceive(buffer, 0, AsynchronousSocket.BufferSize, 0, new AsyncCallback(ReadCallback), this);
            }
            catch (SocketException ex)
            {
                iLog.Error(ex.ToString());
            }
        }

        /// <summary>
        /// 데이터를 받았을때 실행되는 비동기 함수
        /// </summary>
        /// <param name="ar"></param>
        public void ReadCallback(IAsyncResult ar)
        {
            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            AsynchronousSocket sock = (AsynchronousSocket)ar.AsyncState;
            try
            {
                int nBytesRec = 0;

				if (sock.workSocket == null)
					return;

				if (sock.workSocket != null)
                {
                    nBytesRec = sock.workSocket.EndReceive(ar);
                }
                if (nBytesRec > 0)
                {
                    // 여기서 데이터를 처리한다.
                    byte[] byReturn = new byte[nBytesRec];
                    Array.Copy(buffer, byReturn, nBytesRec);
                    ReceivedEventArgs ea = new ReceivedEventArgs(byReturn);
                    if (Received != null) Received(this, ea);

                    // 데이터를 다시 받도록한다.
					if(buffer != null)
						workSocket.BeginReceive(buffer, 0, AsynchronousSocket.BufferSize, 0, new AsyncCallback(ReadCallback), this);
                }
                else
                {
                    // If no data was recieved then the connection is probably dead
                    workSocket.BeginConnect(iep, new AsyncCallback(Connected), workSocket);
                }
            }
            catch (Exception E)
            {
                // 여기서 원격지 접속이 강제로 해제 되었을때를 처리한다. 
                //Debug.WriteLine("니기미 " + E.ToString());
                iLog.Error(E.ToString());
                if (Disconnected != null)
                {
                    if (E.Message == "현재 연결은 원격 호스트에 의해 강제로 끊겼습니다")
                    {
                        //Debug.WriteLine("니기미 원격이다....");
                        Disconnected(this, new EventArgs());

                    }
                }
            }
        }
        /// <summary>
        /// 데이터를 보낼때 사용하는 비동기 함수
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="data"></param>
        public void Send(byte[] data)
        {
            workSocket.Send(data, 0, data.Length, SocketFlags.None);
        }

        public bool Connected()
        {
            return this.workSocket.Connected;
        }

        public int SendLength(byte[] data, int length)
        {
            // Begin sending the data to the remote device.
            return workSocket.Send(data, 0, length, SocketFlags.None);
        }

        public void Close()
        {
            try
            {
                if (workSocket != null)
                {
					buffer = null;

					if (workSocket.Connected == true)
                    {
						workSocket.Shutdown(SocketShutdown.Both);

					}
					
					workSocket.Close();
                    //hanji 소켓이 안 없어질때가 발생함.. 안 죽었으면 날려버린다.  ㅋ
					
                    if (workSocket != null)
                    {
                        workSocket = null;
                    }
                }
            }
            catch (Exception E)
            {
                iLog.Error(E.ToString());
            }
        }
    }
}

