using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Windows;
using log4net;

namespace AWS.Config
{
    static class AWSConfig
    {
        public static int sCount = 0;
        public static int[] enableItemCount= null;

        public static int CDP = 0;
        public static int TSP = 0;
		public static int RCSOD = 5;
		public static int RCSD = 5;
		public static Boolean IS_REALTIME_RECOVERY = false;
        public static string oracleDbStr = "";
        public static SensorStatus[] sValue = null;
        public static String HOME_PATH = "C:\\AWS\\";
        static ILog iLog = log4net.LogManager.GetLogger("Logger");

        public static Boolean Load(String fileName)
        {
            Boolean bRet = false;

            XmlDocument xml = new XmlDocument();

            try
            {
                xml.Load(fileName);

                XmlNodeList xnList = xml.SelectNodes("AWS");
                if(xnList == null)
                {
                    throw new Exception("NullPointer Exception");
                }

                iLog.Debug("[Configuration File(" + fileName + ") Load...");

                for (int i=0;i<xnList[0].ChildNodes.Count;i++)
                {
                    switch(xnList[0].ChildNodes[i].Name)
                    {
                        case "CONFIG":
                            {
                                XmlNode xn = xnList[0].ChildNodes[i];
                                CDP = Convert.ToInt32(xn["COLL_DATA_PERIOD"].InnerText);
                                TSP = Convert.ToInt32(xn["TIME_SYNC_PERIOD"].InnerText);
                                IS_REALTIME_RECOVERY = Convert.ToBoolean(xn["REALTIME_RECOVERY"].InnerText);
								RCSD = Convert.ToInt32(xn["RECOVERY_SCAN_DELAY"].InnerText);
								RCSOD = Convert.ToInt32(xn["RECOVERY_SCAN_ONCE_DELAY"].InnerText);
								oracleDbStr = xn["ORACLE_DB"].InnerText;
                            }
                            break;
                        case "DEVICES":
                            {
                                
                                if (xnList[0].ChildNodes[i].ChildNodes.Count > 0)
                                {
                                    sValue = new SensorStatus[xnList[0].ChildNodes[i].ChildNodes.Count];
                                    sCount = xnList[0].ChildNodes[i].ChildNodes.Count;
                                    enableItemCount = new int[sCount];
                                    
                                    int j = 0;
                                    foreach(XmlNode xn1 in xnList[0].ChildNodes[i].ChildNodes)
                                    {
                                        sValue[j].Name = xn1["NAME"].InnerText;
										sValue[j].LocNum = xn1["LOCNUM"].InnerText;
										sValue[j].Ip = xn1["IP"].InnerText;
                                        sValue[j].Port = Convert.ToInt32(xn1["PORT"].InnerText);
                                        sValue[j].Id = Convert.ToUInt16(xn1["ID"].InnerText);
                                        sValue[j].Passwd = xn1["PASSWD"].InnerText;
                                        sValue[j].enable = Convert.ToBoolean(xn1["ENABLE"].InnerText);
                                        sValue[j].SensorValues = new SensorStatus.NODE[10];

                                        //iLog.Debug("Name : " + sValue[j].Name);
                                        //iLog.Debug("Dev Ip : " + sValue[j].Ip);

                                        int k = 0;
                                        foreach (XmlNode xn2 in xn1["SENSORS"].ChildNodes)
                                        {
                                            if (Convert.ToBoolean(xn2.InnerText) == true)
                                            {
                                                sValue[j].SensorValues[k] = new SensorStatus.NODE();
                                                sValue[j].SensorValues[k].id = xn2.Name;
                                                sValue[j].SensorValues[k].name = xn2.Attributes["name"].Value;
                                                sValue[j].SensorValues[k].status = true;                                                
                                                sValue[j].SensorCnt++;                                                
                                                k++;
                                            }
                                        }
                                        j++;
                                        
                                    }   
                                }
                                
                            }
                            break;
                    }
                }

                iLog.Info("[Configuration File Load Completed...");
            } catch(Exception e)
            {
                MessageBox.Show(e.Message);
                throw e;
            }
            

            return bRet;
        }        
    }

    public struct SensorStatus
    {
        public struct NODE
        {
            public String name;
            public String id;
            public Boolean status;
        }

        public String Name;
		public String LocNum;
        public String Ip;
        public int Port;
        public ushort Id;
        public String Passwd;
        public Boolean enable;

        //public NODE Press;
        //public NODE Humidity;
        //public NODE Rainfall;
        //public NODE Temp;
        //public NODE Windir;
        //public NODE Winspeed;
        //public NODE Rainstatus;

        public int SensorCnt;
        public NODE[] SensorValues;
    }
}
