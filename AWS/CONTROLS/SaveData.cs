//-----------------------------------------------------------------------
// <copyright file="SaveData.cs" company="[Company Name]">
//     Copyright (c) [Company Name] Corporation.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
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
		private int dev_idx = 0;
        public KMA2 kma2 = null;
        public KMA2 lastKma2 = null;

		ILog iLog = null;  

		public SaveData(int dev_idx)
		{
			this.dev_idx = dev_idx;
			iLog = log4net.LogManager.GetLogger("Dev" + dev_idx);
		}

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
                iLog.Error(ex.ToString());
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

				String MonthAccessDBFile = folderName + @"..\" + "aws_" + String.Format("{0:D4}", DateTime.Now.Year) + String.Format("{0:D2}", DateTime.Now.Month)
					+ ".mdb";

				this.checkAccessFile(receive);

                am = new AccessDBManager(this.dev_idx);
                am.Connect(todayAccessDBFile);

                //최소/최대 값을 읽어옴...
                double[] max_value = am.GetSensorMaxData();
                double[] min_value = am.GetSensorMinData();

                //데이터 저장
                am.InsertSensorData(receive, dev_idx, kma2, min_value, max_value);
				
				double[] d = am.GetSensorAvgData();

				if (d != null)
				{
					am.Close();
					am.Connect(MonthAccessDBFile);
					am.UpdateMonthData(receive, d);
				}

                om = new OracleDBManager(dev_idx);
                om.Connect();
				om.InsertAwsStampData(dev_idx, receive, kma2, min_value, max_value);
				om.UpdateLastTimeCall(receive, AWSConfig.sValue[dev_idx].LocNum);

				iLog.Info(string.Format("현재자료 저장[{0}] : {1}/{2:00}/{3:00} {4:00}/{5:00}",
							todayAccessDBFile, 
							receive.Year,
							receive.Month,
							receive.Day,
							receive.Hour,
							receive.Minute));
            }
            catch (Exception ex)
            {
                iLog.Error(ex.ToString());
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

                folderName += String.Format("{0:D4}", receive.Year) + @"\" + String.Format("{0:D2}", receive.Month) + @"\";

                String todayAccessDBFile = folderName + "aws_" + String.Format("{0:D4}", lastKma2.Year + 2000) + String.Format("{0:D2}", lastKma2.Month)
                    + String.Format("{0:D2}", lastKma2.Day) + ".mdb";

                this.checkAccessFile(receive);

                am = new AccessDBManager(dev_idx);
                am.Connect(todayAccessDBFile);

                //최소/최대 값을 읽어옴...
                double[] max_value = am.GetSensorMaxData();
                double[] min_value = am.GetSensorMinData();

                //데이터 저장
                am.InsertSensorData(receive, (int)o, lastKma2, min_value, max_value);
				
                om = new OracleDBManager(dev_idx);
                om.Connect();
                om.InsertAwsStampData((int)o, receive, lastKma2, min_value, max_value);
                //om.UpdateLastTimeCall(receive, AWSConfig.sValue[(int)o].LocNum);

				iLog.Info(string.Format("과거자료 저장[{0}] : {1}/{2:00}/{3:00} {4:00}:{5:00}",
							todayAccessDBFile,
							receive.Year,
							receive.Month,
							receive.Day,
							receive.Hour,
							receive.Minute));
			}
            catch (Exception ex)
            {
                iLog.Error(ex.ToString());
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
				string monthAccessFile = AWSConfig.HOME_PATH + "\\AccessFile\\aws_month.mdb";

				DateTime today = receiveDate;

                String todayAccessDBFile = folderName + @"\" + "aws_" + String.Format("{0:D4}", today.Year)
                    + String.Format("{0:D2}", today.Month) + String.Format("{0:D2}", today.Day) + ".mdb";

				String monthAccessDBFile = folderName + @"\..\" + "aws_" + String.Format("{0:D4}", today.Year)
					+ String.Format("{0:D2}", today.Month) + ".mdb";

				//오늘
				if (!File.Exists(todayAccessDBFile))
                {
                    System.IO.File.Copy(accessFile, todayAccessDBFile, true);
                }

				if (!File.Exists(monthAccessDBFile))
				{
					iLog.Debug(monthAccessDBFile + "파일을 생성했습니다.");
					System.IO.File.Copy(monthAccessFile, monthAccessDBFile, true);

					AccessDBManager am = null;
					try
					{
						am = new AccessDBManager(dev_idx);
						am.Connect(monthAccessDBFile);
						am.InsertMonthBaseData(monthAccessDBFile);
					}
					catch(Exception e)
					{
						iLog.Error(e.Message);
						iLog.Error(e.StackTrace);
					} finally
					{
						am.Close();
					}
					
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
                iLog.Error(ex.ToString());
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
                iLog.Error(ex.ToString());
            }
        }
    }
 }
