using System;
using System.Collections.Generic;
using System.Text;

namespace AWS.MODEL
{
    public class DataLoggerEnvironment
    {
        private string  _strIP;
        private int     _strPort;
        private ushort _iLoggerID;
        private string password;
       
      
        public string IP 
        {
            get { return this._strIP;  }
            set { this._strIP = value; }
        }

        public int PORT
        {
            get { return this._strPort; }
            set { this._strPort = value; }
        }

        public ushort LoggerID
        {
            get { return this._iLoggerID; }
            set { this._iLoggerID = value; }
        }

        public string PASSWORD
        {
            get { return this.password; }
            set { this.password = value; }
        }
    }
}
