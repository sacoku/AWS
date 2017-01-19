using AWS.MODEL;
using AWS.UTIL;
using log4net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AWS.VIEW
{
    public partial class DisplayForm : Form
    {
        private MainForm main = null;   
        ILog iLog = log4net.LogManager.GetLogger("Logger");

        public DisplayForm(MainForm mainForm)
        {
            this.main = mainForm;
            InitializeComponent();

            this.Init();
        }

        private void Init()
        {
            this.lblWindDir.Parent = this.pictureBox1;
            this.lblWindDir.BackColor = Color.Transparent;
            this.lblWindDir.BringToFront();

            this.lblWindSpeed.Parent = this.pictureBox1;
            this.lblWindSpeed.BackColor = Color.Transparent;
            this.lblWindSpeed.BringToFront();

            this.lblHumidity.Parent = this.pictureBox1;
            this.lblHumidity.BackColor = Color.Transparent;
            this.lblHumidity.BringToFront();

            this.lblPress.Parent = this.pictureBox1;
            this.lblPress.BackColor = Color.Transparent;
            this.lblPress.BringToFront();

            this.lblTemp.Parent = this.pictureBox1;
            this.lblTemp.BackColor = Color.Transparent;
            this.lblTemp.BringToFront();

            this.lblVisibility.Parent = this.pictureBox1;
            this.lblVisibility.BackColor = Color.Transparent;
            this.lblVisibility.BringToFront();

            this.lblCurrentWeather.Parent = this.pictureBox1;
            this.lblCurrentWeather.BackColor = Color.Transparent;
            this.lblCurrentWeather.BringToFront();

            this.lblWaterOfSurface.Parent = this.pictureBox1;
            this.lblWaterOfSurface.BackColor = Color.Transparent;
            this.lblWaterOfSurface.BringToFront();

            this.lblFreezingOfSurface.Parent = this.pictureBox1;
            this.lblFreezingOfSurface.BackColor = Color.Transparent;
            this.lblFreezingOfSurface.BringToFront();

            this.lblRain.Parent = this.pictureBox1;
            this.lblRain.BackColor = Color.Transparent;
            this.lblRain.BringToFront();

            this.lblTempOfSurface.Parent = this.pictureBox1;
            this.lblTempOfSurface.BackColor = Color.Transparent;
            this.lblTempOfSurface.BringToFront();

            this.lblFreezingTempOfSurface.Parent = this.pictureBox1;
            this.lblFreezingTempOfSurface.BackColor = Color.Transparent;
            this.lblFreezingTempOfSurface.BringToFront();

            this.lblIsRain.Parent = this.pictureBox1;
            this.lblIsRain.BackColor = Color.Transparent;
            this.lblIsRain.BringToFront();

            this.lblFrictionOfSurface.Parent = this.pictureBox1;
            this.lblFrictionOfSurface.BackColor = Color.Transparent;
            this.lblFrictionOfSurface.BringToFront();

            this.lblSnowOfSurface.Parent = this.pictureBox1;
            this.lblSnowOfSurface.BackColor = Color.Transparent;
            this.lblSnowOfSurface.BringToFront();


            this.lblStatusOfSurface.Parent = this.pictureBox1;
            this.lblStatusOfSurface.BackColor = Color.Transparent;
            this.lblStatusOfSurface.BringToFront();

            this.lblSaltOfSurface.Parent = this.pictureBox1;
            this.lblSaltOfSurface.BackColor = Color.Transparent;
            this.lblSaltOfSurface.BringToFront();
                      
            this.lblCodeOfSurface.Parent = this.pictureBox1;
            this.lblCodeOfSurface.BackColor = Color.Transparent;
            this.lblCodeOfSurface.BringToFront();

            this.lblDateTime.Parent = this.pictureBox1;
            this.lblDateTime.BackColor = Color.Transparent;
            this.lblDateTime.BringToFront();

        }

        public void DisplayData(KMA2 kma)
        {
            KMA2 returnKMA = this.ByteChangAll2(kma);
                                        
            try
            {              
                this.lblTemp.Text =string.Format("{0:0.0}", (returnKMA.Sensor_0_Datas / 10.0) - 100.0);
                this.lblWindDir.Text = this.getDirection((returnKMA.Sensor_1_Datas / 10.0).ToString());
                this.lblWindSpeed.Text = string.Format("{0:0.0}", returnKMA.Sensor_2_Datas / 10.0);
                this.lblRain.Text = string.Format("{0:0.0}", (returnKMA.Sensor_5_Datas / 10.0));

                if (returnKMA.Sensor_7_Datas > 0)
                    this.lblIsRain.Text = "ON";
                else
                    this.lblIsRain.Text = "OFF";

                this.lblPress.Text = string.Format("{0:0.0}", (returnKMA.Sensor_6_Datas / 10.0)); //기압
                this.lblHumidity.Text = string.Format("{0:0}", returnKMA.Sensor_9_Datas / 10.0); // 습도
                this.lblWaterOfSurface.Text = (returnKMA.Spare_Sensor_0_Datas / 10.0).ToString(); //노면수막높이
                this.lblFreezingOfSurface.Text = string.Format("{0:0}", returnKMA.Spare_Sensor_1_Datas / 10); //노면결빙율             
                this.lblTempOfSurface.Text = string.Format("{0:0.0}", (returnKMA.Spare_Sensor_3_Datas / 10.0) - 100.0); //노면온도
                this.lblFreezingTempOfSurface.Text = string.Format("{0:0.0}", (returnKMA.Spare_Sensor_4_Datas / 10.0) - 100.0);  //노면동결온도
                this.lblSnowOfSurface.Text = string.Format("{0:0.0}", returnKMA.Spare_Sensor_5_Datas / 10.0); //노면적설
                this.lblSaltOfSurface.Text = string.Format("{0:0.0}", returnKMA.Spare_Sensor_6_Datas / 10.0); //노면염분온도 
                this.lblFrictionOfSurface.Text = (returnKMA.Spare_Sensor_7_Datas / 10.0).ToString(); //노면마찰
                this.lblVisibility.Text = returnKMA.Spare1_Sensor_1_Datas.ToString();   //시정
                this.lblCurrentWeather.Text = string.Format("{0:D2}", returnKMA.Spare2_Sensor_2_Datas);   //현천
                this.lblStatusOfSurface.Text = this.getStatusOfLoad(returnKMA.Spare_Sensor_2_Datas.ToString()); //노면상태
                this.lblCodeOfSurface.Text = this.getLoadStatusCode(returnKMA.Spare_Sensor_8_Datas.ToString()); // 노면날씨코드


                this.lblDateTime.Text = string.Format("{0:00}", returnKMA.Year + 2000) + "/" + string.Format("{0:00}", returnKMA.Month) + "/" + string.Format("{0:00}", returnKMA.Day) + "  " + string.Format("{0:00}", returnKMA.Hour) + ":" + string.Format("{0:00}", returnKMA.Minute);

            }
            catch (Exception ex)
            {
                iLog.Info("[ERROR] DisplayForm : DisplayData " + ex.Message);
            }
        }

        private String getLoadStatusCode(String code)
        {
            int loadCode = int.Parse(code);
            String result = "";

            if (loadCode == 0)
            {
                result = "normal\r\nroad\nweather";
            }
            else if (loadCode == 1)
            {
                result = "bad\r\nroad\r\nweather";

            }
            else if (loadCode == 2)
            {
                result = "very\r\nbad\r\nroad\r\nweather";
            }
            return result;
        }

        private String getStatusOfLoad(String code)
        {
            int loadCode = int.Parse(code);
            String result = "";

            if (loadCode == 0)
            {
                result = "Dry";
            }
            else if (loadCode == 1)
            {
                result = "Damp";

            }
            else if (loadCode == 2)
            {
                result = "Wet";
            }
            else if (loadCode == 3)
            {
                result = "Ice";
            }
            else if (loadCode == 4)
            {
                result = "Snow\r\nIce";

            }
            else if (loadCode == 6)
            {
                result = "Chemically\r\nWet";
            }
            else if (loadCode == 8)
            {
                result = "Snow";
            }
            else
            {
                result = "Undefined";
            }
            return result;
        }

        private String getDirection(String direction)
        {
            string result = "";
            double dir = double.Parse(direction);

            if((dir >= 348.8 ) && (dir <= 360.0))
                result = "북";
            else if ((dir >= 0.0) && (dir <= 11.3))
                result = "북";
            else if ((dir > 11.3) && (dir <= 33.8))
                result = "북북동";
            else if ((dir > 33.8) && (dir <= 56.3))
                result = "북동";
            else if ((dir > 56.3) && (dir <= 78.8))
                result = "동북동";
            else if ((dir > 78.8) && (dir <= 101.3))
                result = "둥";
            else if ((dir > 101.3) && (dir <= 123.8))
                result = "동남동";
            else if ((dir > 123.8) && (dir <= 146.3))
                result = "남동";
            else if ((dir > 146.3) && (dir <= 168.8))
                result = "남남동";
            else if ((dir > 168.8) && (dir <= 191.3))
                result = "남";
            else if ((dir > 191.3) && (dir <= 213.8))
                result = "남남서";
            else if ((dir > 213.8) && (dir <= 236.3))
                result = "남서";
            else if ((dir > 236.3) && (dir <= 258.8))
                result = "서남서";
            else if ((dir > 258.8) && (dir <= 281.3))
                result = "서";
            else if ((dir > 281.3) && (dir <= 303.8))
                result = "서북서";
            else if ((dir > 303.8) && (dir <= 326.3))
                result = "북서";
            else if ((dir > 326.3) && (dir <= 348.8))
                result = "북북서";

            return result;
        }

        /// <summary>
        /// KMA2 프로토콜 
        /// </summary>
        private KMA2 ByteChangAll2(KMA2 kma2)
        {
            try
            {
                kma2.LoggerID = CommonUtil.ByteChange(kma2.LoggerID);

                kma2.Sensor_0_Datas = CommonUtil.ByteChange(kma2.Sensor_0_Datas);
                kma2.Sensor_1_Datas = CommonUtil.ByteChange(kma2.Sensor_1_Datas);
                kma2.Sensor_2_Datas = CommonUtil.ByteChange(kma2.Sensor_2_Datas);
                kma2.Sensor_3_Datas = CommonUtil.ByteChange(kma2.Sensor_3_Datas);
                kma2.Sensor_4_Datas = CommonUtil.ByteChange(kma2.Sensor_4_Datas);
                kma2.Sensor_5_Datas = CommonUtil.ByteChange(kma2.Sensor_5_Datas);
                kma2.Sensor_6_Datas = CommonUtil.ByteChange(kma2.Sensor_6_Datas);
                kma2.Sensor_7_Datas = CommonUtil.ByteChange(kma2.Sensor_7_Datas);
                kma2.Sensor_8_Datas = CommonUtil.ByteChange(kma2.Sensor_8_Datas);
                kma2.Sensor_9_Datas = CommonUtil.ByteChange(kma2.Sensor_9_Datas);

                kma2.Sensor0_0_Datas = CommonUtil.ByteChange(kma2.Sensor0_0_Datas);
                kma2.Sensor1_1_Datas = CommonUtil.ByteChange(kma2.Sensor1_1_Datas);
                kma2.Sensor2_2_Datas = CommonUtil.ByteChange(kma2.Sensor2_2_Datas);
                kma2.Sensor3_3_Datas = CommonUtil.ByteChange(kma2.Sensor3_3_Datas);
                kma2.Sensor4_4_Datas = CommonUtil.ByteChange(kma2.Sensor4_4_Datas);
                kma2.Sensor5_5_Datas = CommonUtil.ByteChange(kma2.Sensor5_5_Datas);
                kma2.Sensor6_6_Datas = CommonUtil.ByteChange(kma2.Sensor6_6_Datas);
                kma2.Sensor7_7_Datas = CommonUtil.ByteChange(kma2.Sensor7_7_Datas);
                kma2.Sensor8_8_Datas = CommonUtil.ByteChange(kma2.Sensor8_8_Datas);
                kma2.Sensor9_9_Datas = CommonUtil.ByteChange(kma2.Sensor9_9_Datas);
                kma2.Sensor10_10_Datas = CommonUtil.ByteChange(kma2.Sensor10_10_Datas);
                kma2.Sensor11_11_Datas = CommonUtil.ByteChange(kma2.Sensor11_11_Datas);
                kma2.Sensor12_12_Datas = CommonUtil.ByteChange(kma2.Sensor12_12_Datas);

                kma2.Spare_Sensor_0_Datas = CommonUtil.ByteChange(kma2.Spare_Sensor_0_Datas);
                kma2.Spare_Sensor_1_Datas = CommonUtil.ByteChange(kma2.Spare_Sensor_1_Datas);
                kma2.Spare_Sensor_2_Datas = CommonUtil.ByteChange(kma2.Spare_Sensor_2_Datas);
                kma2.Spare_Sensor_3_Datas = CommonUtil.ByteChange(kma2.Spare_Sensor_3_Datas);
                kma2.Spare_Sensor_4_Datas = CommonUtil.ByteChange(kma2.Spare_Sensor_4_Datas);
                kma2.Spare_Sensor_5_Datas = CommonUtil.ByteChange(kma2.Spare_Sensor_5_Datas);
                kma2.Spare_Sensor_6_Datas = CommonUtil.ByteChange(kma2.Spare_Sensor_6_Datas);
                kma2.Spare_Sensor_7_Datas = CommonUtil.ByteChange(kma2.Spare_Sensor_7_Datas);
                kma2.Spare_Sensor_8_Datas = CommonUtil.ByteChange(kma2.Spare_Sensor_8_Datas);
                kma2.Spare_Sensor_9_Datas = CommonUtil.ByteChange(kma2.Spare_Sensor_9_Datas);

                kma2.Spare0_Sensor_0_Datas = CommonUtil.ByteChange(kma2.Spare0_Sensor_0_Datas);
                kma2.Spare1_Sensor_1_Datas = CommonUtil.ByteChange(kma2.Spare1_Sensor_1_Datas);
                kma2.Spare2_Sensor_2_Datas = CommonUtil.ByteChange(kma2.Spare2_Sensor_2_Datas);
                kma2.Spare3_Sensor_3_Datas = CommonUtil.ByteChange(kma2.Spare3_Sensor_3_Datas);
                kma2.Spare4_Sensor_4_Datas = CommonUtil.ByteChange(kma2.Spare4_Sensor_4_Datas);
                kma2.Spare5_Sensor_5_Datas = CommonUtil.ByteChange(kma2.Spare5_Sensor_5_Datas);
                kma2.Spare6_Sensor_6_Datas = CommonUtil.ByteChange(kma2.Spare6_Sensor_6_Datas);
                kma2.Spare7_Sensor_7_Datas = CommonUtil.ByteChange(kma2.Spare7_Sensor_7_Datas);
                kma2.Spare8_Sensor_8_Datas = CommonUtil.ByteChange(kma2.Spare8_Sensor_8_Datas);
                kma2.Spare9_Sensor_9_Datas = CommonUtil.ByteChange(kma2.Spare9_Sensor_9_Datas);               
            }
            catch (Exception ex)
            {
                iLog.Info("[ERROR] DisplayForm : ByteChangAll2 " + ex.Message);
            }
            return kma2;
        }
    }
}
