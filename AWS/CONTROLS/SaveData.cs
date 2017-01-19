using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;

using AWS.MODEL;
using System.Windows.Forms;
using System.Collections;
using AWS;
using AWS.UTIL;
using System.Data.OleDb;
using log4net;
using System.Data;
using AWS.Config;
using AWS.CONTROLS;

namespace AWS.CONTROL
{
    class SaveData
    {        
        public KMA2 kma2 = null;
        public KMA2 lastKma2 = null;

        static ILog iLog = log4net.LogManager.GetLogger("Logger");

        public void SaveWeatherData()
        {
           
        }

        public String getResult(int dev_idx)
        {
            String result = String.Empty;
            DateTime receive;
            try
            {
                receive = new DateTime(((int)kma2.Year), (int)kma2.Month, (int)kma2.Day, (int)kma2.Hour, (int)kma2.Minute, 0);
            }
            catch (Exception ex)
            {
                result = "DateTime is wrong!! " + ex.Message;
                return result;
            }
          
            ByteChangAll2();

            try
            {
                string results = "," + string.Format("{0:0.0}", (kma2.Sensor_1_Datas / 10.0)) //풍향
                    + "," + string.Format("{0:0.0}", (kma2.Sensor_2_Datas / 10.0))  //풍속
                    + "," + string.Format("{0:0.0}", (kma2.Sensor_0_Datas / 10.0) - 100.0) //온도
                    + "," + string.Format("{0:0.0}", (kma2.Sensor_5_Datas / 10.0)) //강우
                    + "," + string.Format("{0:0.0}", (kma2.Sensor_6_Datas / 10.0)) //기압
                    + "," + string.Format("{0:0}", (kma2.Sensor_9_Datas / 10.0)) //습도
                    + "," + string.Format("{0:0.0}", (kma2.Sensor0_0_Datas)) //일사
                    + "," + string.Format("{0:0.0}", (kma2.Sensor1_1_Datas / 100)); //일조          

                result = string.Format("{0:0000}", kma2.LoggerID) + "," +
                        string.Format("{0:00}", receive.Hour) + ":" + string.Format("{0:00}", receive.Minute) + ":" + string.Format("{0:00}", DateTime.Now.Second) + results;

                saveData(kma2, dev_idx);
            }
            catch (Exception ex)
            {
                iLog.Debug(ex.Message);
            }

            return result;
        }

        private void saveData(KMA2 kma2, int dev_idx)
        {
            AccessDBManager am = null;
            OracleDBManager om = null;
            DateTime receive;
            string folderName = AWSConfig.HOME_PATH + "\\AWSDATA\\" + AWSConfig.sValue[(int)dev_idx].Name + "\\";

            try
            {
                receive = new DateTime(2000 + ((int)kma2.Year), (int)kma2.Month, (int)kma2.Day, (int)kma2.Hour, (int)kma2.Minute, 0);
             
                folderName += String.Format("{0:D4}", DateTime.Now.Year) + @"\" + String.Format("{0:D2}", DateTime.Now.Month) + @"\";
            
                String todayAccessDBFile = folderName + "aws_" + String.Format("{0:D4}", DateTime.Now.Year) + String.Format("{0:D2}", DateTime.Now.Month)
                    + String.Format("{0:D2}", DateTime.Now.Day) + ".mdb";

                this.checkAccessFile(receive);

                am = new AccessDBManager();
                am.Connect(todayAccessDBFile);

                //최소/최대 값을 읽어옴...
                double[] max_value = am.GetSensorMaxData();
                double[] min_value = am.GetSensorMinData();

                //데이터 저장
                am.InsertSensorData(receive, dev_idx, kma2, min_value, max_value);

                om = new OracleDBManager();
                om.Connect();
				om.InsertAwsStampData(dev_idx, receive, kma2, min_value, max_value);
				om.UpdateLastTimeCall(receive, AWSConfig.sValue[dev_idx].LocNum);

				iLog.Info("현재자료 저장["+ todayAccessDBFile + "] : " + receive.Year + "/" + receive.Month + "/" + receive.Day + " " + receive.Hour + ":" + receive.Minute);

                /*
                double[] min_max_value = { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, };

                double temp = double.Parse(String.Format("{0:F1}", (kma2.Sensor_0_Datas / 10.0) - 100.0));      //온도
                double wd = kma2.Sensor_1_Datas / 10.0;                                                                         //풍향
                double ws = kma2.Sensor_2_Datas / 10.0;                                                                         //풍속
                double rain = kma2.Sensor_5_Datas / 10.0;                                                                       //강우
                double israin = kma2.Sensor_7_Datas;                                                                              //강우감지
                double humidity = double.Parse(String.Format("{0:0}", kma2.Sensor_9_Datas / 10.0));                 //습도
                double sunshine = double.Parse(string.Format("{0:0.0}", (kma2.Sensor1_1_Datas / 3600)));          //일조
                double visibility = kma2.Spare1_Sensor_1_Datas;                                                                 //시정

                
                //최대값을 구한다.
                

                string selectSql = "SELECT                             "
                                      + "         MIN(TEMP) AS MIN_TEMP            "
                                      + "        ,MIN(WS) AS MIN_WS                "
                                      + "        ,MIN(WD) AS MIN_WD               "
                                      + "        ,MIN(HUMIDITY) AS MIN_HUMIDITY     "
                                      + "        ,MAX(TEMP) AS MAX_TEMP            "
                                      + "        ,MAX(WS) AS MAX_WS                "
                                      + "        ,MAX(WD) AS MAX_WD               "
                                      + "        ,MAX(HUMIDITY) AS MAX_HUMIDITY     "
                                      + "FROM AWS_MIN                                   ";

                iLog.Debug(selectSql);

                con = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + todayAccessDBFile);
                cmd = new OleDbCommand(selectSql, con);
                OleDbDataAdapter myDataAdapter = new OleDbDataAdapter(cmd);
                DataSet readDataSet = new DataSet();

                con.Open();
                myDataAdapter.Fill(readDataSet, "aws_min");

                iLog.Debug("Rows Count : " + readDataSet.Tables[0].Rows.Count);

                if (readDataSet.Tables[0].Rows.Count > 0)
                {
                    DataRow row = readDataSet.Tables[0].Rows[0];

                    for (int i = 0; i < min_max_value.Length; i++)
                    {
                        min_max_value[i] = (row.ItemArray.GetValue(i).ToString() == "" ? 0 : double.Parse(row.ItemArray.GetValue(i).ToString()) );
                    }
                }

                con.Close();

                iLog.Debug("TEMP / MIN_TEMP / MAX TEMP : " + temp + " / " + min_max_value[0] + " / " + min_max_value[4]);
                iLog.Debug("WD / MIN_WD / MAX WD : " + ws + " / " + min_max_value[1] + " / " + min_max_value[5]);
                iLog.Debug("WS / MIN_WS / MAX WS : " + ws + " / " + min_max_value[2] + " / " + min_max_value[6]);
                iLog.Debug("HUMIDITY / MIN_HUMIDITY / MAX HUMIDITY : " + humidity + " / " + min_max_value[3] + " / " + min_max_value[7]);

                insertSql =   "INSERT INTO AWS_MIN(                          "
                            + "                      DEV_IDX                 "
                            + "                     ,RECEIVETIME             "
                            + "                     ,TEMP                    "
                            + "                     ,MIN_TEMP                "
                            + "                     ,MAX_TEMP                "
                            + "                     ,WD                      "
                            + "                     ,MIN_WD                      "
                            + "                     ,MAX_WD                      "
                            + "                     ,WS                      "
                            + "                     ,MIN_WS                  "
                            + "                     ,MAX_WS                  "
                            + "                     ,RAIN                    "
                            + "                     ,ISRAIN                  "
                            + "                     ,HUMIDITY                "
                            + "                     ,MIN_HUMIDITY            "
                            + "                     ,MAX_HUMIDITY            "
                            + "                     ,SUNSHINE                "
                            + "                     ,VISIBILITY              "
                            + ") VALUES(                                     "
                            +         dev_idx 
                            + ",'" + receive + "'"
                            + "," + temp
                            + "," + ((min_max_value[0]  != 0 && min_max_value[0] < temp) ? min_max_value[0] : temp)
                            + "," + ( (min_max_value[4] != 0 && min_max_value[4] > temp) ? min_max_value[4] : temp )
                            + "," + wd
                            + "," + ((min_max_value[1] != 0 && min_max_value[1] < wd) ? min_max_value[1] : wd)
                            + "," + ((min_max_value[5] != 0 && min_max_value[5] > wd) ? min_max_value[5] : wd)
                            + "," + ws
                            + "," + ( (min_max_value[2] != 0 && min_max_value[2] < ws) ? min_max_value[2] : ws )
                            + "," + ((min_max_value[6] != 0 && min_max_value[6] > ws) ? min_max_value[6] : ws)
                            + "," + rain
                            + "," + israin
                            + "," + humidity
                            + "," + ( (min_max_value[3] != 0 && min_max_value[3] < humidity) ? min_max_value[3] : humidity )
                            + "," + ((min_max_value[7] != 0 && min_max_value[7] > humidity) ? min_max_value[7] : humidity)
                            + "," + sunshine
                            + "," + visibility
                            + ") ";

                //Debug.WriteLine(insertSql);
                iLog.Debug(insertSql);

                con = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + todayAccessDBFile);
                cmd = new OleDbCommand(insertSql, con);
                con.Open();
                int n = cmd.ExecuteNonQuery();

                iLog.Info("현재자료 저장 : " + receive.Year + "/" + receive.Month + "/" + receive.Day + " " + receive.Hour + ":" + receive.Minute);
                */
            }
            catch (Exception ex)
            {
                iLog.Error("[ERROR] saveData : " + ex.Message);
                iLog.Error(ex.StackTrace.ToString());
            }
            finally
            {
                if(am != null) am.Close();
                if (om != null) om.Close();
                //if (con != null)
                //    con.Close();
            }
        }

        public void saveLastData(object o)
        {
            DateTime receive;
            AccessDBManager am = null;
            OracleDBManager om = null;
            string folderName = AWSConfig.HOME_PATH + "\\AWSDATA\\" + AWSConfig.sValue[(int)o].Name + "\\";

            try
            {
                receive = new DateTime(2000 + ((int)lastKma2.Year), (int)lastKma2.Month, (int)lastKma2.Day, (int)lastKma2.Hour, (int)lastKma2.Minute, 0);

                folderName += String.Format("{0:D4}", DateTime.Now.Year) + @"\" + String.Format("{0:D2}", DateTime.Now.Month) + @"\";

                String todayAccessDBFile = folderName + "aws_" + String.Format("{0:D4}", lastKma2.Year + 2000) + String.Format("{0:D2}", lastKma2.Month)
                    + String.Format("{0:D2}", lastKma2.Day) + ".mdb";

                this.checkAccessFile(receive);

                am = new AccessDBManager();
                am.Connect(todayAccessDBFile);

                //최소/최대 값을 읽어옴...
                double[] max_value = am.GetSensorMaxData();
                double[] min_value = am.GetSensorMinData();

                //데이터 저장
                am.InsertSensorData(receive, (int)o, lastKma2, min_value, max_value);

                om = new OracleDBManager();
                om.Connect();
                om.InsertAwsStampData((int)o, receive, lastKma2, min_value, max_value);
                //om.UpdateLastTimeCall(receive, AWSConfig.sValue[(int)o].LocNum);

                iLog.Info("과거자료 저장 : " + receive.Year + "/" + receive.Month + "/" + receive.Day + " " + receive.Hour + ":" + receive.Minute);

                /*
                selectSql = "SELECT RECEIVETIME FROM AWS_MIN WHERE DEV_IDX = " + (int)o;
                iLog.Debug(selectSql);

                con = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + todayAccessDBFile);
                cmd = new OleDbCommand(selectSql, con);
                con.Open();
                int n = cmd.ExecuteNonQuery();
                con.Close();

                if (n <= 0)
                {
                    double temp = double.Parse(String.Format("{0:F1}", (lastKma2.Sensor_0_Datas / 10.0) - 100.0));
                    double wd = lastKma2.Sensor_1_Datas / 10.0;
                    double ws = lastKma2.Sensor_2_Datas / 10.0;
                    double rain = lastKma2.Sensor_5_Datas / 10.0;
                    double israin = lastKma2.Sensor_7_Datas;
                    double humidity = double.Parse(String.Format("{0:0}", lastKma2.Sensor_9_Datas / 10.0));
                    double sunshine = double.Parse(string.Format("{0:0.0}", (lastKma2.Sensor1_1_Datas / 3600)));
                    double visibility = lastKma2.Spare1_Sensor_1_Datas;

                    insertSql = "INSERT INTO AWS_MIN(                        "
                            + "                      DEV_IDX                 "
                            + "                     ,RECEIVETIME             "
                            + "                     ,TEMP                    "
                            + "                     ,WD                      "
                            + "                     ,WS                      "
                            + "                     ,RAIN                    "
                            + "                     ,ISRAIN                  "
                            + "                     ,HUMIDITY                "
                            + "                     ,SUNSHINE                "
                            + "                     ,VISIBILITY              "
                            + ") VALUES(                                     "
                            + (int)o
                            + ",'" + receive + "'"
                            + "," + temp
                            + "," + wd
                            + "," + ws
                            + "," + rain
                            + "," + israin
                            + "," + humidity
                            + "," + sunshine
                            + "," + visibility
                            + ") ";

                    //Debug.WriteLine(insertSql);
                    iLog.Debug(insertSql);

                    con = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + todayAccessDBFile);
                    cmd = new OleDbCommand(insertSql, con);
                    con.Open();
                   
                    cmd.ExecuteNonQuery();
                    iLog.Info("과거자료 저장 : " + receive.Year + "/" + receive.Month + "/" + receive.Day + " " + receive.Hour + ":" + receive.Minute);
                }
                */
            }
            catch (Exception ex)
            {
                iLog.Error("[ERROR] saveLastData : saveLastData " + ex.Message);
            }
            finally
            {
                if (am != null) am.Close();
                if (om != null) om.Close();
            }
        }

        private void checkAccessFile(DateTime receiveDate)
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

                String year = String.Format("{0:D4}", receiveDate.Year);

                folderName += @"\" + year;

                if (!Directory.Exists(folderName))
                    Directory.CreateDirectory(folderName);

                String month = String.Format("{0:D2}", receiveDate.Month);
                String day = String.Format("{0:D2}", receiveDate.Day);

                folderName += @"\" + month;
                if (!Directory.Exists(folderName))
                    Directory.CreateDirectory(folderName);

                string accessFile = AWSConfig.HOME_PATH + "\\AccessFile\\aws.mdb";

                DateTime today = receiveDate;

                String todayAccessDBFile = folderName + @"\" + "aws_" + String.Format("{0:D4}", today.Year)
                    + String.Format("{0:D2}", today.Month) + String.Format("{0:D2}", today.Day) + ".mdb";

                //오늘
                if (!File.Exists(todayAccessDBFile))
                {
                    System.IO.File.Copy(accessFile, todayAccessDBFile, true);
                }
            }

        }

        /// <summary>
        /// KMA2 프로토콜 
        /// </summary>
        private void ByteChangAll2()
        {
            try
            {
                this.kma2.LoggerID = CommonUtil.ByteChange(kma2.LoggerID);

                this.kma2.Sensor_0_Datas = CommonUtil.ByteChange(kma2.Sensor_0_Datas);
                this.kma2.Sensor_1_Datas = CommonUtil.ByteChange(kma2.Sensor_1_Datas);
                this.kma2.Sensor_2_Datas = CommonUtil.ByteChange(kma2.Sensor_2_Datas);
                this.kma2.Sensor_3_Datas = CommonUtil.ByteChange(kma2.Sensor_3_Datas);
                this.kma2.Sensor_4_Datas = CommonUtil.ByteChange(kma2.Sensor_4_Datas);
                this.kma2.Sensor_5_Datas = CommonUtil.ByteChange(kma2.Sensor_5_Datas);
                this.kma2.Sensor_6_Datas = CommonUtil.ByteChange(kma2.Sensor_6_Datas);
                this.kma2.Sensor_7_Datas = CommonUtil.ByteChange(kma2.Sensor_7_Datas);
                this.kma2.Sensor_8_Datas = CommonUtil.ByteChange(kma2.Sensor_8_Datas);
                this.kma2.Sensor_9_Datas = CommonUtil.ByteChange(kma2.Sensor_9_Datas);

                this.kma2.Sensor0_0_Datas = CommonUtil.ByteChange(kma2.Sensor0_0_Datas);
                this.kma2.Sensor1_1_Datas = CommonUtil.ByteChange(kma2.Sensor1_1_Datas);
                this.kma2.Sensor2_2_Datas = CommonUtil.ByteChange(kma2.Sensor2_2_Datas);
                this.kma2.Sensor3_3_Datas = CommonUtil.ByteChange(kma2.Sensor3_3_Datas);
                this.kma2.Sensor4_4_Datas = CommonUtil.ByteChange(kma2.Sensor4_4_Datas);
                this.kma2.Sensor5_5_Datas = CommonUtil.ByteChange(kma2.Sensor5_5_Datas);
                this.kma2.Sensor6_6_Datas = CommonUtil.ByteChange(kma2.Sensor6_6_Datas);
                this.kma2.Sensor7_7_Datas = CommonUtil.ByteChange(kma2.Sensor7_7_Datas);
                this.kma2.Sensor8_8_Datas = CommonUtil.ByteChange(kma2.Sensor8_8_Datas);
                this.kma2.Sensor9_9_Datas = CommonUtil.ByteChange(kma2.Sensor9_9_Datas);
                this.kma2.Sensor10_10_Datas = CommonUtil.ByteChange(kma2.Sensor10_10_Datas);
                this.kma2.Sensor11_11_Datas = CommonUtil.ByteChange(kma2.Sensor11_11_Datas);
                this.kma2.Sensor12_12_Datas = CommonUtil.ByteChange(kma2.Sensor12_12_Datas);

                this.kma2.Spare_Sensor_0_Datas = CommonUtil.ByteChange(kma2.Spare_Sensor_0_Datas);
                this.kma2.Spare_Sensor_1_Datas = CommonUtil.ByteChange(kma2.Spare_Sensor_1_Datas);
                this.kma2.Spare_Sensor_2_Datas = CommonUtil.ByteChange(kma2.Spare_Sensor_2_Datas);
                this.kma2.Spare_Sensor_3_Datas = CommonUtil.ByteChange(kma2.Spare_Sensor_3_Datas);
                this.kma2.Spare_Sensor_4_Datas = CommonUtil.ByteChange(kma2.Spare_Sensor_4_Datas);
                this.kma2.Spare_Sensor_5_Datas = CommonUtil.ByteChange(kma2.Spare_Sensor_5_Datas);
                this.kma2.Spare_Sensor_6_Datas = CommonUtil.ByteChange(kma2.Spare_Sensor_6_Datas);
                this.kma2.Spare_Sensor_7_Datas = CommonUtil.ByteChange(kma2.Spare_Sensor_7_Datas);
                this.kma2.Spare_Sensor_8_Datas = CommonUtil.ByteChange(kma2.Spare_Sensor_8_Datas);
                this.kma2.Spare_Sensor_9_Datas = CommonUtil.ByteChange(kma2.Spare_Sensor_9_Datas);

                this.kma2.Spare0_Sensor_0_Datas = CommonUtil.ByteChange(kma2.Spare0_Sensor_0_Datas);
                this.kma2.Spare1_Sensor_1_Datas = CommonUtil.ByteChange(kma2.Spare1_Sensor_1_Datas);
                this.kma2.Spare2_Sensor_2_Datas = CommonUtil.ByteChange(kma2.Spare2_Sensor_2_Datas);
                this.kma2.Spare3_Sensor_3_Datas = CommonUtil.ByteChange(kma2.Spare3_Sensor_3_Datas);
                this.kma2.Spare4_Sensor_4_Datas = CommonUtil.ByteChange(kma2.Spare4_Sensor_4_Datas);
                this.kma2.Spare5_Sensor_5_Datas = CommonUtil.ByteChange(kma2.Spare5_Sensor_5_Datas);
                this.kma2.Spare6_Sensor_6_Datas = CommonUtil.ByteChange(kma2.Spare6_Sensor_6_Datas);
                this.kma2.Spare7_Sensor_7_Datas = CommonUtil.ByteChange(kma2.Spare7_Sensor_7_Datas);
                this.kma2.Spare8_Sensor_8_Datas = CommonUtil.ByteChange(kma2.Spare8_Sensor_8_Datas);
                this.kma2.Spare9_Sensor_9_Datas = CommonUtil.ByteChange(kma2.Spare9_Sensor_9_Datas);
            }
            catch (Exception ex)
            {
                iLog.Error("[ERROR] saveData : ByteChangAll2 " + ex.Message);
            }
        }

        /// KMA2 프로토콜 
        /// </summary>
        public void ByteChangAllLastData()
        {
            try
            {
                this.lastKma2.LoggerID = CommonUtil.ByteChange(lastKma2.LoggerID);

                this.lastKma2.Sensor_0_Datas = CommonUtil.ByteChange(lastKma2.Sensor_0_Datas);
                this.lastKma2.Sensor_1_Datas = CommonUtil.ByteChange(lastKma2.Sensor_1_Datas);
                this.lastKma2.Sensor_2_Datas = CommonUtil.ByteChange(lastKma2.Sensor_2_Datas);
                this.lastKma2.Sensor_3_Datas = CommonUtil.ByteChange(lastKma2.Sensor_3_Datas);
                this.lastKma2.Sensor_4_Datas = CommonUtil.ByteChange(lastKma2.Sensor_4_Datas);
                this.lastKma2.Sensor_5_Datas = CommonUtil.ByteChange(lastKma2.Sensor_5_Datas);
                this.lastKma2.Sensor_6_Datas = CommonUtil.ByteChange(lastKma2.Sensor_6_Datas);
                this.lastKma2.Sensor_7_Datas = CommonUtil.ByteChange(lastKma2.Sensor_7_Datas);
                this.lastKma2.Sensor_8_Datas = CommonUtil.ByteChange(lastKma2.Sensor_8_Datas);
                this.lastKma2.Sensor_9_Datas = CommonUtil.ByteChange(lastKma2.Sensor_9_Datas);

                this.lastKma2.Sensor0_0_Datas = CommonUtil.ByteChange(lastKma2.Sensor0_0_Datas);
                this.lastKma2.Sensor1_1_Datas = CommonUtil.ByteChange(lastKma2.Sensor1_1_Datas);
                this.lastKma2.Sensor2_2_Datas = CommonUtil.ByteChange(lastKma2.Sensor2_2_Datas);
                this.lastKma2.Sensor3_3_Datas = CommonUtil.ByteChange(lastKma2.Sensor3_3_Datas);
                this.lastKma2.Sensor4_4_Datas = CommonUtil.ByteChange(lastKma2.Sensor4_4_Datas);
                this.lastKma2.Sensor5_5_Datas = CommonUtil.ByteChange(lastKma2.Sensor5_5_Datas);
                this.lastKma2.Sensor6_6_Datas = CommonUtil.ByteChange(lastKma2.Sensor6_6_Datas);
                this.lastKma2.Sensor7_7_Datas = CommonUtil.ByteChange(lastKma2.Sensor7_7_Datas);
                this.lastKma2.Sensor8_8_Datas = CommonUtil.ByteChange(lastKma2.Sensor8_8_Datas);
                this.lastKma2.Sensor9_9_Datas = CommonUtil.ByteChange(lastKma2.Sensor9_9_Datas);
                this.lastKma2.Sensor10_10_Datas = CommonUtil.ByteChange(lastKma2.Sensor10_10_Datas);
                this.lastKma2.Sensor11_11_Datas = CommonUtil.ByteChange(lastKma2.Sensor11_11_Datas);
                this.lastKma2.Sensor12_12_Datas = CommonUtil.ByteChange(lastKma2.Sensor12_12_Datas);

                this.lastKma2.Spare_Sensor_0_Datas = CommonUtil.ByteChange(lastKma2.Spare_Sensor_0_Datas);
                this.lastKma2.Spare_Sensor_1_Datas = CommonUtil.ByteChange(lastKma2.Spare_Sensor_1_Datas);
                this.lastKma2.Spare_Sensor_2_Datas = CommonUtil.ByteChange(lastKma2.Spare_Sensor_2_Datas);
                this.lastKma2.Spare_Sensor_3_Datas = CommonUtil.ByteChange(lastKma2.Spare_Sensor_3_Datas);
                this.lastKma2.Spare_Sensor_4_Datas = CommonUtil.ByteChange(lastKma2.Spare_Sensor_4_Datas);
                this.lastKma2.Spare_Sensor_5_Datas = CommonUtil.ByteChange(lastKma2.Spare_Sensor_5_Datas);
                this.lastKma2.Spare_Sensor_6_Datas = CommonUtil.ByteChange(kma2.Spare_Sensor_6_Datas);
                this.lastKma2.Spare_Sensor_7_Datas = CommonUtil.ByteChange(lastKma2.Spare_Sensor_7_Datas);
                this.lastKma2.Spare_Sensor_8_Datas = CommonUtil.ByteChange(lastKma2.Spare_Sensor_8_Datas);
                this.lastKma2.Spare_Sensor_9_Datas = CommonUtil.ByteChange(lastKma2.Spare_Sensor_9_Datas);

                this.lastKma2.Spare0_Sensor_0_Datas = CommonUtil.ByteChange(lastKma2.Spare0_Sensor_0_Datas);
                this.lastKma2.Spare1_Sensor_1_Datas = CommonUtil.ByteChange(lastKma2.Spare1_Sensor_1_Datas);
                this.lastKma2.Spare2_Sensor_2_Datas = CommonUtil.ByteChange(lastKma2.Spare2_Sensor_2_Datas);
                this.lastKma2.Spare3_Sensor_3_Datas = CommonUtil.ByteChange(lastKma2.Spare3_Sensor_3_Datas);
                this.lastKma2.Spare4_Sensor_4_Datas = CommonUtil.ByteChange(lastKma2.Spare4_Sensor_4_Datas);
                this.lastKma2.Spare5_Sensor_5_Datas = CommonUtil.ByteChange(lastKma2.Spare5_Sensor_5_Datas);
                this.lastKma2.Spare6_Sensor_6_Datas = CommonUtil.ByteChange(lastKma2.Spare6_Sensor_6_Datas);
                this.lastKma2.Spare7_Sensor_7_Datas = CommonUtil.ByteChange(lastKma2.Spare7_Sensor_7_Datas);
                this.lastKma2.Spare8_Sensor_8_Datas = CommonUtil.ByteChange(lastKma2.Spare8_Sensor_8_Datas);
                this.lastKma2.Spare9_Sensor_9_Datas = CommonUtil.ByteChange(lastKma2.Spare9_Sensor_9_Datas);
            }
            catch (Exception ex)
            {
                iLog.Error("[ERROR] saveData : ByteChangAll2 " + ex.Message);
            }
        }
    }
 }
