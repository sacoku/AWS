//-----------------------------------------------------------------------
// <copyright file="AccessDBManager.cs" company="[Company Name]">
//     Copyright (c) [Company Name] Corporation.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using AWS.MODEL;
using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWS.CONTROLS
{
    class AccessDBManager
    {
        private static AccessDBManager _INSTANCE = null;

        private OleDbConnection conn = null;
		ILog iLog = null;  

        public AccessDBManager(int dev_idx)
		{
			iLog = log4net.LogManager.GetLogger("Dev" + dev_idx);
		}

		public AccessDBManager()
		{
			iLog = log4net.LogManager.GetLogger("Logger");
		}

		public static AccessDBManager GetInstance()
        {
            if (_INSTANCE == null)
                _INSTANCE = new AccessDBManager();
            return _INSTANCE;
        }

        public OleDbConnection Connect(string dataSourceFile)
        {
            try
            {
                if (conn != null) Close();
                conn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + dataSourceFile);
                conn.Open();
                return conn;
            } catch(Exception e)
            {
                iLog.Error(e.Message);
                throw e;
            }
        }

        public void Close()
        {
            if (conn != null) conn.Close();
            conn = null;
        }

        public bool CreateMinDatabase(string fullFilename)
        {
            bool succeeded = false;
            OleDbConnection conn = null;

            try
            {
                if (!File.Exists(fullFilename))
                {
                    string newDB = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fullFilename;
                    Type objClassType = Type.GetTypeFromProgID("ADOX.Catalog");
                    if (objClassType != null)
                    {
                        object obj = Activator.CreateInstance(objClassType);
                        // Create MDB file 
                        obj.GetType().InvokeMember("Create", System.Reflection.BindingFlags.InvokeMethod, null, obj,
                                  new object[] { "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + newDB + ";" });
                        succeeded = true;
                        // Clean up
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                        obj = null;
                        try
                        {
                            conn = new OleDbConnection();
                            OleDbCommand connCmd = new OleDbCommand();

                            conn.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + fullFilename;
                            conn.Open();
                            connCmd.Connection = conn;

                            connCmd.CommandText = "CREATE TABLE AWS_MIN(											\n"
                                                     + "                   DEV_IDX INT								\n"
                                                     + "                  ,RECEIVETIME DATETIME PRIMARY KEY			\n"
													 + "                  ,ATMO DOUBLE								\n"
													 + "                  ,MIN_ATMO DOUBLE							\n"
													 + "                  ,MAX_ATMO DOUBLE							\n"
													 + "                  ,TEMP DOUBLE								\n"
                                                     + "                  ,MIN_TEMP DOUBLE							\n"
                                                     + "                  ,MAX_TEMP DOUBLE							\n"
                                                     + "                  ,WD DOUBLE								\n"
                                                     + "                  ,MIN_WD DOUBLE							\n"
                                                     + "                  ,MAX_WD DOUBLE							\n"
                                                     + "                  ,WS DOUBLE								\n"
                                                     + "                  ,MIN_WS DOUBLE							\n"
                                                     + "                  ,MAX_WS DOUBLE							\n"
                                                     + "                  ,RAIN DOUBLE								\n"
                                                     + "                  ,ISRAIN DOUBLE							\n"
                                                     + "                  ,HUMIDITY DOUBLE							\n"
                                                     + "                  ,MIN_HUMIDITY DOUBLE						\n"
                                                     + "                  ,MAX_HUMIDITY DOUBLE						\n"
                                                     + "                  ,SUNSHINE DOUBLE							\n"
                                                     + "                  ,VISIBILITY DOUBLE						\n"
                                                     + ")";
							//iLog.Debug(connCmd.CommandText);
							iLog.Info("AWS_MIN 테이블 생성 쿼리 실행.");
                            connCmd.ExecuteNonQuery();

							iLog.Info("Database 파일을 생성했습니다.[" + fullFilename + "]");
                        }
                        catch (Exception ex)
                        {
                            iLog.Error(ex.Message);
                        }
                        finally
                        {
                            conn.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                iLog.Error("Could not create database file: " + fullFilename + "\n\n" + ex.Message);
            }

            return succeeded;
        }

		public bool CreateMonthDatabase(string fullFilename)
		{
			bool succeeded = false;
			OleDbConnection conn = null;

			try
			{
				if (!File.Exists(fullFilename))
				{
					string newDB = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fullFilename;
					Type objClassType = Type.GetTypeFromProgID("ADOX.Catalog");
					if (objClassType != null)
					{
						object obj = Activator.CreateInstance(objClassType);
						// Create MDB file 
						obj.GetType().InvokeMember("Create", System.Reflection.BindingFlags.InvokeMethod, null, obj,
								  new object[] { "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + newDB + ";" });
						succeeded = true;
						// Clean up
						System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
						obj = null;
						try
						{
							conn = new OleDbConnection();
							OleDbCommand connCmd = new OleDbCommand();

							conn.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + fullFilename;
							conn.Open();
							connCmd.Connection = conn;

							connCmd.CommandText = "CREATE TABLE AWS_MONTH(											\n"
													 + "                   AWS_DATE DATE PRIMARY KEY				\n"
													 + "                  ,ATMO DOUBLE								\n"
													 + "                  ,TEMP DOUBLE								\n"
													 + "                  ,WD DOUBLE								\n"
													 + "                  ,WS DOUBLE								\n"
													 + "                  ,RAIN DOUBLE								\n"
													 + "                  ,HUMIDITY DOUBLE							\n"
													 + "                  ,SUNSHINE DOUBLE							\n"
													 + "                  ,VISIBILITY DOUBLE						\n"
													 + "                  ,CHG_TIME DATETIME						\n"
													 + ")";
							//iLog.Debug(connCmd.CommandText);
							iLog.Info("AWS_MONTH 테이블 생성 쿼리 실행.");
							connCmd.ExecuteNonQuery();

							iLog.Info("Database 파일을 생성했습니다.[" + fullFilename + "]");
						}
						catch (Exception ex)
						{
							iLog.Error(ex.Message);
						}
						finally
						{
							conn.Close();
						}
					}
				}
			}
			catch (Exception ex)
			{
				iLog.Error("Could not create database file: " + fullFilename + "\n\n" + ex.Message);
			}

			return succeeded;
		}

		public void InsertMonthBaseData(string fullFilename)
		{
			try
			{
				if (conn == null) throw new Exception("접속되어 있지 않습니다.");

				DateTime CurrDateTime = DateTime.Now;
				DateTime dt = new DateTime(CurrDateTime.Year, CurrDateTime.Month, 1, 0, 0, 0);


				while (CurrDateTime.Month == dt.Month)
				{
					string sql = "INSERT INTO AWS_MONTH( AWS_DATE )					\n"
							   + "VALUES('"+ dt.ToString("yyyy-MM-dd") +"')";

					//iLog.Debug("[QUERY]\n" + sql);
					iLog.Info("AWS_MONTH INSERT 쿼리 실행.");
					OleDbCommand cmd = new OleDbCommand(sql, conn);
					//conn.Open();
					cmd.ExecuteNonQuery();

					dt = dt.AddDays(+1);
				}

			}
			catch (Exception ex)
			{
				iLog.Error(ex.Message);
			}
		}

		public double[] GetSensorMaxData()
        {
            double[] value = null;
            try
            {
                if (conn == null) throw new Exception("접속되어 있지 않습니다.");

                string sql = "SELECT                                                   \n"
							 + "         MAX(ATMO) AS MIN_ATMO                 \n"
							 + "        ,MAX(TEMP) AS MIN_TEMP                 \n"
							 + "        ,MAX(WD) AS MIN_WD                      \n"
							 + "        ,MAX(WS) AS MIN_WS                       \n"
                             + "        ,MAX(HUMIDITY) AS MIN_HUMIDITY     \n"
                             + "FROM AWS_MIN                                          ";

				//iLog.Debug("[QUERY]\n" + sql);
				iLog.Info("AWS_MIN 최대값 조회 쿼리 실행.");

				OleDbCommand cmd = new OleDbCommand(sql, conn);
                OleDbDataAdapter myDataAdapter = new OleDbDataAdapter(cmd);
                DataSet readDataSet = new DataSet();

                //conn.Open();
                myDataAdapter.Fill(readDataSet, "aws_min");
                if (readDataSet.Tables[0].Rows.Count > 0)
                {
                    DataRow row = readDataSet.Tables[0].Rows[0];

                    value = new double[row.ItemArray.Length];
                    for (int i = 0; i < row.ItemArray.Length; i++)
                    {
                        value[i] = (row.ItemArray.GetValue(i).ToString() == "" ? 0 : double.Parse(row.ItemArray.GetValue(i).ToString()));
                    }
                }

                return value;
            }
            catch (Exception e)
            {
                iLog.Error(e.Message);
                throw e;
            } finally
            {
                //conn.Close();
            }
        }

        public double[] GetSensorMinData()
        {
            double[] value = null;
            try
            {
                if (conn == null) throw new Exception("접속되어 있지 않습니다.");
                
                string sql = "SELECT                                                   \n"
							 + "         MIN(ATMO) AS MIN_ATMO                 \n"
							 + "        ,MIN(TEMP) AS MIN_TEMP                 \n"
							 + "        ,MIN(WD) AS MIN_WD                      \n"
							 + "        ,MIN(WS) AS MIN_WS                       \n"
                             + "        ,MIN(HUMIDITY) AS MIN_HUMIDITY     \n"
                             + "FROM AWS_MIN                                          ";

				//iLog.Debug("[QUERY]\n" + sql);
				iLog.Info("AWS_MIN 최소값 조회 쿼리 실행.");

				OleDbCommand cmd = new OleDbCommand(sql, conn);
                OleDbDataAdapter myDataAdapter = new OleDbDataAdapter(cmd);
                DataSet readDataSet = new DataSet();

                //conn.Open();
                myDataAdapter.Fill(readDataSet, "aws_min");
                if (readDataSet.Tables[0].Rows.Count > 0)
                {
                    DataRow row = readDataSet.Tables[0].Rows[0];

                    value = new double[row.ItemArray.Length];
                    for (int i = 0; i < row.ItemArray.Length; i++)
                    {
                        value[i] = (row.ItemArray.GetValue(i).ToString() == "" ? 0 : double.Parse(row.ItemArray.GetValue(i).ToString()));
                    }
                }

                return value;
            } 
            catch(Exception e)
            {
                iLog.Error(e.Message);
                throw e;
            } finally
            {
                //conn.Close();
            }
        }

        public void InsertSensorData(DateTime dt, int dev_idx, KMA2 kma, double[] min_values, double[] max_values)
        {
            try
            {
                if (conn == null) throw new Exception("접속되어 있지 않습니다.");

				double atmo = double.Parse(String.Format("{0:0}", kma.Sensor_6_Datas / 10.0));
				double temp = double.Parse(String.Format("{0:F1}", (kma.Sensor_0_Datas / 10.0) - 100.0));      //온도
                double wd = kma.Sensor_1_Datas / 10.0;                                                                         //풍향
                double ws = kma.Sensor_2_Datas / 10.0;                                                                         //풍속
                double rain = kma.Sensor_5_Datas / 10.0;                                                                       //강우
                double israin = kma.Sensor_7_Datas;                                                                              //강우감지
                double humidity = double.Parse(String.Format("{0:0}", kma.Sensor_9_Datas / 10.0));                 //습도
                double sunshine = double.Parse(string.Format("{0:0.0}", (kma.Sensor1_1_Datas / 3600)));          //일조
                double visibility = kma.Spare1_Sensor_1_Datas;

                string sql = "INSERT INTO AWS_MIN(													\n"
                            + "                      DEV_IDX													\n"
                            + "                     ,RECEIVETIME												\n"
							+ "                     ,ATMO														\n"
							+ "                     ,MIN_ATMO												\n"
							+ "                     ,MAX_ATMO												\n"
							+ "                     ,TEMP														\n"
                            + "                     ,MIN_TEMP													\n"
                            + "                     ,MAX_TEMP												\n"
                            + "                     ,WD															\n"
                            + "                     ,MIN_WD													\n"
                            + "                     ,MAX_WD													\n"
                            + "                     ,WS															\n"
                            + "                     ,MIN_WS													\n"
                            + "                     ,MAX_WS													\n"
                            + "                     ,RAIN														\n"
                            + "                     ,ISRAIN														\n"
                            + "                     ,HUMIDITY													\n"
                            + "                     ,MIN_HUMIDITY											\n"
                            + "                     ,MAX_HUMIDITY											\n"
                            + "                     ,SUNSHINE													\n"
                            + "                     ,VISIBILITY													\n"
                            + ") VALUES(																		\n"
                            + dev_idx
                            + ",'" + dt + "'"
							+ "," + atmo
							+ "," + ((min_values[0] != 0 && min_values[0] < atmo) ? min_values[0] : atmo)
							+ "," + ((max_values[0] != 0 && max_values[0] > atmo) ? max_values[0] : atmo)
							+ "," + temp
                            + "," + ((min_values[1] != 0 && min_values[1] < temp) ? min_values[1] : temp)
                            + "," + ((max_values[1] != 0 && max_values[1] > temp) ? max_values[1] : temp)
                            + "," + wd
                            + "," + ((min_values[2] != 0 && min_values[2] < wd) ? min_values[2] : wd)
                            + "," + ((max_values[2] != 0 && max_values[2] > wd) ? max_values[2] : wd)
                            + "," + ws
                            + "," + ((min_values[3] != 0 && min_values[3] < ws) ? min_values[3] : ws)
                            + "," + ((max_values[3] != 0 && max_values[3] > ws) ? max_values[3] : ws)
                            + "," + rain
                            + "," + israin
                            + "," + humidity
                            + "," + ((min_values[4] != 0 && min_values[4] < humidity) ? min_values[4] : humidity)
                            + "," + ((max_values[4] != 0 && max_values[4] > humidity) ? max_values[4] : humidity)
                            + "," + sunshine
                            + "," + visibility
                            + ") ";
				//iLog.Debug("[QUERY]\n" + sql);

				OleDbCommand cmd = new OleDbCommand(sql, conn);
				iLog.Info("AWS_MIN 데이터 INSERT 쿼리 실행.");
				//conn.Open();
				cmd.ExecuteNonQuery();
            }
            catch(Exception e)
            {
                iLog.Error(e.ToString());
            } finally
            {
                //conn.Close();
            }
        }

		public double[] GetSensorAvgData()
		{
			double[] value = null;
			try
			{
				if (conn == null) throw new Exception("접속되어 있지 않습니다.");

				string sql = "SELECT										\n"
							 + "         AVG(ATMO) AS AVG_ATMO				\n"
							 + "        ,AVG(TEMP) AS AVG_TEMP				\n"
							 + "        ,AVG(WD) AS AVG_WD					\n"
							 + "        ,AVG(WS) AS AVG_WS					\n"
							 + "        ,AVG(RAIN) AS AVG_RAIN				\n"
							 + "        ,AVG(HUMIDITY) AS AVG_HUMIDITY		\n"
							 + "        ,AVG(SUNSHINE) AS AVG_SUNSHINE		\n"
							+ "         ,AVG(VISIBILITY) AS AVG_VISIBILITY	\n"
							 + "FROM AWS_MIN								  ";

				//iLog.Debug("[QUERY]\n" + sql);
				iLog.Info("AWS_MIN 데이터 평균 조회 쿼리 실행.");

				OleDbCommand cmd = new OleDbCommand(sql, conn);
				OleDbDataAdapter myDataAdapter = new OleDbDataAdapter(cmd);
				DataSet readDataSet = new DataSet();

				//conn.Open();
				myDataAdapter.Fill(readDataSet, "aws_min");
				if (readDataSet.Tables[0].Rows.Count > 0)
				{
					DataRow row = readDataSet.Tables[0].Rows[0];
					value = new double[row.ItemArray.Length];
					for (int i = 0; i < row.ItemArray.Length; i++)
					{
						value[i] = (row.ItemArray.GetValue(i).ToString() == "" ? 0 : double.Parse(row.ItemArray.GetValue(i).ToString()));
					}
				}

				return value;
			}
			catch (Exception e)
			{
				iLog.Error(e.Message);
				iLog.Error(e.StackTrace);
				throw e;
			}
			finally
			{
				//conn.Close();
			}
		}

		public void UpdateMonthData(DateTime dt, double[] values)
		{
			try
			{
				if (conn == null) throw new Exception("접속되어 있지 않습니다.");

				string sql = 
							string.Format("UPDATE AWS_MONTH SET						\n" +
										  "         ATMO = {0:0}	  				\n" +
										  "        ,TEMP = {1:F1}					\n" +
										  "        ,WD = {2:0.0}					\n" +
										  "        ,WS = {3:0.0}					\n" +
										  "        ,RAIN = {4:0.0}					\n" +
									  	  "        ,HUMIDITY = {5:0}				\n" +
										  "        ,SUNSHINE = {6:0.0}				\n" +
										  "        ,VISIBILITY = {7:0}				\n" +
										  "WHERE AWS_DATE = #{8}#					  ",
							values[0],
							values[1],
							values[2],
							values[3],
							values[4],
							values[5],
							values[6],
							values[7],
							dt.ToString("yyyy-MM-dd"));
				//iLog.Debug("[QUERY]\n" + sql);
				iLog.Info("AWS_MONTH UPDATE 쿼리 실행.["+ dt.ToString("yyyy-MM-dd") + "]");

				OleDbCommand cmd = new OleDbCommand(sql, conn);
				//conn.Open();
				cmd.ExecuteNonQuery();
			}
			catch (Exception e)
			{
				iLog.Error(e.Message);
				iLog.Error(e.StackTrace);
			}
			finally
			{
				//conn.Close();
			}
		}
	}
}
