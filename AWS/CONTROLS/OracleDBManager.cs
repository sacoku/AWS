using AWS.Config;
using AWS.MODEL;
using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable 0618

namespace AWS.CONTROLS
{
    class OracleDBManager
    {
        private static OracleDBManager _INSTANCE = null;
        private OracleConnection conn = null;

        ILog iLog = log4net.LogManager.GetLogger("Logger");

        public OracleDBManager() {; }

        public static OracleDBManager GetInstance()
        {
            if (_INSTANCE == null)
                _INSTANCE = new OracleDBManager();

            return _INSTANCE;
        }

        public OracleConnection Connect()
        {
            try
            {
                if (conn != null) Close();
                conn = new OracleConnection(AWS.Config.AWSConfig.oracleDbStr);
                conn.Open();

                iLog.Info("오라클에 접속되었습니다.");

                return conn;
            } catch(Exception e)
            {
                iLog.Error(e.Message);
                throw e;
            }
        }

        public void Close()
        {
            try
            {
                if (conn != null) conn.Close();
                conn = null;

                iLog.Info("오라클에 접속이 해제되었습니다.");
            } 
            catch(Exception e)
            {
                iLog.Error(e.Message);
                throw e;
            }
        }
		
		public void InsertAwsStampData(int idx, DateTime dt, KMA2 kma, double[] v_min, double[] v_max)
		{
			try
			{
				if (conn == null) throw new Exception("접속되어 있지 않습니다.");
				
				double atmo = double.Parse(String.Format("{0:0}", kma.Sensor_6_Datas * 100));
				double temp = double.Parse(String.Format("{0:F1}", (kma.Sensor_0_Datas) * 100));      //온도
				double wd = kma.Sensor_1_Datas * 100;                                                                         //풍향
				double ws = kma.Sensor_2_Datas * 100;                                                                         //풍속
				double m_wd = kma.Sensor_3_Datas * 100;
				double m_ws = kma.Sensor_4_Datas * 100;
				double rain = kma.Sensor_5_Datas * 100;                                                                       //강우
				double israin = kma.Sensor_7_Datas;                                                                              //강우감지
				double humidity = double.Parse(String.Format("{0:0}", kma.Sensor_9_Datas * 100));                 //습도

				int s_dc_input_v = kma.Voltage & 0x01;
				int s_batt_v = kma.Voltage & 0x02;
				int[] s_ac_v = { kma.Voltage & 0x04, kma.Voltage & 0x08 };
				int s_lock = kma.Voltage & 0x10;

				int s_s_wd = kma.SensorStatus1[0] & 0x01;
				int s_s_ws = kma.SensorStatus1[0] & 0x02;
				int s_s_temp = kma.SensorStatus1[0] & 0x04;
				int s_s_israin = kma.SensorStatus1[0] & 0x08;
				int s_s_rain = kma.SensorStatus1[0] & 0x10;
				int s_s_humidity = kma.SensorStatus1[0] & 0x20;
				int s_s_atmo = kma.SensorStatus1[0] & 0x40;
				int s_fan = (kma.Spare9_Sensor_9_Datas>0) ? 1: 0;


				string query = ""
								 + "INSERT INTO AWSTAMP" + AWSConfig.sValue[idx].LocNum + "  (				\n"
								 + "      AWSDATE																				\n"
								 + "      ,ATMO_LAVR																				\n"
								 + "      ,ATMO_LMIN																			\n"
								 + "      ,ATMO_LMAX																			\n"
								 + "      ,HUMI_LAVR																				\n"
								 + "      ,HUMI_LMIN																				\n"
								 + "      ,HUMI_LMAX																				\n"
								 + "      ,RAIN_LVALUE																			\n"
								 + "      ,TEMP_LAVR																				\n"
								 + "      ,TEMP_LMIN																				\n"
								 + "      ,TEMP_LMAX																				\n"
								 + "      ,WIND_LAVRDEG																		\n"
								 + "      ,WIND_LAVRVEL																		\n"
								 + "      ,WIND_LMAXDEG																		\n"
								 + "      ,WIND_LMAXVEL																		\n"
								 + "      ,AWSSTATE_BIT0																		\n"
								 + "      ,AWSSTATE_BIT1																		\n"
								 + "      ,AWSSTATE_BIT2																		\n"
								 + "      ,AWSSTATE_BIT3																		\n"
								 + "      ,AWSSTATE_BIT4																		\n"
//								 + "      ,AWSSTATE_FBATTERY																	\n"
								 + "      ,AWSSTATELOG_BIT0																	\n"
								 + "      ,AWSSTATELOG_BIT1																	\n"
								 + "      ,AWSSTATELOG_BIT2																	\n"
								 + "      ,AWSSTATELOG_BIT3																	\n"
								 + "      ,AWSSTATELOG_BIT4																	\n"
								 + "      ,AWSSTATELOG_BIT5																	\n"
								 + "      ,AWSSTATELOG_BIT6																	\n"
								 + "      ,AWSSTATELOG_BIT15																	\n"
								 + ") VALUES(																						\n"
								 + "		TO_DATE('" + dt.ToString("yyyy-MM-dd HH:mm:ss") + "', 'yyyy-mm-dd HH24:mi:ss')   \n"
								 + "       ," + double.Parse(String.Format("{0:F1}", atmo)) + "							\n"
								 + "       ," + double.Parse(String.Format("{0:F1}", (v_min[0] * 10) * 100.0)) + "		\n"
								 + "       ," + double.Parse(String.Format("{0:F1}", (v_max[0] * 10) * 100.0)) + "	\n"
								 + "       ," + double.Parse(String.Format("{0:F1}", humidity)) + "						\n"
								 + "       ," + double.Parse(String.Format("{0:F1}", (v_min[4] * 10) * 100.0)) + "		\n"
								 + "       ," + double.Parse(String.Format("{0:F1}", (v_max[4] * 10) * 100.0)) + "	\n"
								 + "       ," + double.Parse(String.Format("{0:F1}", rain)) + "								\n"
								 + "       ," + double.Parse(String.Format("{0:F1}", temp)) + "							\n"
								 + "       ," + double.Parse(String.Format("{0:F1}", ((v_min[1]+100) * 10) * 100.0)) + "	\n"
								 + "       ," + double.Parse(String.Format("{0:F1}", ((v_max[1]+100) * 10) * 100.0)) + "	\n"
								 + "       ," + double.Parse(String.Format("{0:F1}", wd)) + "								\n"
								 + "       ," + double.Parse(String.Format("{0:F1}", ws)) + "								\n"
								 + "       ," + double.Parse(String.Format("{0:F1}", m_wd)) + "							\n"
								 + "       ," + double.Parse(String.Format("{0:F1}", m_ws)) + "							\n"
								 + "       ," + s_dc_input_v + "																	\n"
								 + "       ," + s_batt_v + "																		\n"
								 + "       ," + s_ac_v[0] + "				                                                        \n"
								 + "       ," + s_ac_v[1] + "				                                                        \n"
								 + "       ," + s_lock + "																			\n"
								 + "       ," + s_s_wd + "																			\n"
								 + "       ," + s_s_ws + "																			\n"
								 + "       ," + s_s_temp + "																		\n"
								 + "       ," + s_s_israin + "																		\n"
								 + "       ," + s_s_rain + "																		\n"
								 + "       ," + s_s_humidity + "																	\n"
								 + "       ," + s_s_atmo + "																		\n"
								 + "       ," + s_fan + "																			\n"
								 + ")																										";

				iLog.Debug("[QUERY]\n" + query);

				OracleCommand cmd = new OracleCommand();
				cmd.Connection = conn;
				cmd.CommandText = query;

				cmd.ExecuteNonQuery();
			}
			catch (Exception e)
			{
				iLog.Error(e.Message);
				throw e;
			}
		}

		public DateTime GetLastTimeCall(String localcode)
		{
			DateTime dt = new DateTime();

			try
			{
				if (conn == null) throw new Exception("접속되어 있지 않습니다.");

				string query = "SELECT TO_CHAR(LASTCALLTIME, 'yyyy-mm-dd HH24:mi:ss') FROM LOCALINFO			\n"
								 + "WHERE														\n"
								 + "SLOCALCODE = '"+localcode+"'						    ";

				iLog.Debug("[QUERY]\n" + query);

				OracleCommand cmd = new OracleCommand();
				cmd.Connection = conn;
				cmd.CommandText = query;

				OracleDataAdapter myDataAdapter = new OracleDataAdapter(cmd);
				DataSet readDataSet = new DataSet();

				myDataAdapter.Fill(readDataSet, "LOCALINFO");

				if (readDataSet.Tables[0].Rows.Count > 0)
				{
					DataRow row = readDataSet.Tables[0].Rows[0];
					String data = row.ItemArray.GetValue(0).ToString();
					dt = DateTime.Parse(data);
				}

				return dt;
			}
			catch(Exception e)
			{
				iLog.Error(e.Message);
				throw e;
			}
		}

		public void UpdateLastTimeCall(DateTime dt, String localcode)
		{
			try
			{
				if (conn == null) throw new Exception("접속되어 있지 않습니다.");

				string query = "UPDATE LOCALINFO														\n"
								 + "SET																			\n"
								 + "LASTCALLTIME = TO_DATE('" + dt.ToString("yyyy-MM-dd HH:mm:ss") + "', 'yyyy-mm-dd HH24:mi:ss')	\n"
								 + "WHERE																		\n"
								 + "SLOCALCODE = '" + localcode + "'									    ";

				iLog.Debug("[QUERY]\n" + query);
				
				OracleCommand cmd = new OracleCommand();
				cmd.Connection = conn;
				cmd.CommandText = query;

				cmd.ExecuteNonQuery();
			}
			catch (Exception e)
			{
				iLog.Error(e.Message);
				throw e;
			}
		}
	}
}