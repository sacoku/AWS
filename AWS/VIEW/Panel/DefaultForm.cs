using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using log4net;
using AWS.MODEL;
using System.Threading;

namespace AWS.VIEW.Panel
{
    public partial class DefaultForm : Form
    {
        static ILog iLog = log4net.LogManager.GetLogger("Logger");
        private int idx = 0;
        private String DeviceName;

        private PictureBox[] pictureBox = null; 
        private Label[] ibName = null;
        private Label[] ibValue = null;

		private int nCallCnt = 0;
        private Label iblDevName = new Label();
		private Thread DataCollWatch = null;

		public DefaultForm(String dName, int idx)
        {
            this.DeviceName = dName;
            this.idx = idx;
            InitializeComponent();
        }

        private void DefaultForm_Load(object sender, EventArgs e)
        {
            FontFamily familyName = new FontFamily("휴먼둥근헤드라인");
            System.Drawing.Font myFont = new System.Drawing.Font(familyName, 25, FontStyle.Regular, GraphicsUnit.Pixel);
            System.Drawing.Font myFont2 = new System.Drawing.Font(familyName, 30, FontStyle.Regular, GraphicsUnit.Pixel);
            int max = 0;
            for (int i = 0; i < AWS.Config.AWSConfig.sCount; i++)
            {
                if (max < AWS.Config.AWSConfig.sValue[i].SensorCnt)
                    max = AWS.Config.AWSConfig.sValue[i].SensorCnt;
            }

            int itemCount = max;

            if (itemCount > 0)
            {
                int rCnt = (itemCount / 2) + ((itemCount % 2 == 0) ? 0 : 1);
                int cCnt = 2;
                GenerateTable(cCnt, rCnt);
            }

            typeof(TableLayoutPanel).GetProperty( "DoubleBuffered",
                                                  System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(tableLayoutPanel1, true, null);
            typeof(TableLayoutPanel).GetProperty( "DoubleBuffered",
                                                  System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(tableLayoutPanel2, true, null);

            ibName = new Label[AWS.Config.AWSConfig.sValue[idx].SensorCnt];
            ibValue = new Label[AWS.Config.AWSConfig.sValue[idx].SensorCnt];
            pictureBox = new PictureBox[AWS.Config.AWSConfig.sValue[idx].SensorCnt];

            iblDevName.Dock = DockStyle.Fill;
            iblDevName.Anchor = AnchorStyles.None;
            iblDevName.Text = DeviceName;
            iblDevName.Parent = tableLayoutPanel1;
            iblDevName.TextAlign = ContentAlignment.MiddleCenter;
            iblDevName.Font = myFont;
            iblDevName.AutoSize = true;
            iblDevName.ForeColor = Color.Red;
            iblDevName.BackColor = Color.Transparent;

            for (int j = 0; j < AWS.Config.AWSConfig.sValue[idx].SensorCnt; j++)
            {
                pictureBox[j] = new PictureBox();
                pictureBox[j].BackgroundImage = pictureBox1.BackgroundImage; // System.Drawing.Image.FromFile("D:\\00. Work\\ZZ. Private Work\\03. 황인태\\기상청\\작업중\\AWSForLoad\\img\\ItemBox.png");
                pictureBox[j].SizeMode = pictureBox1.SizeMode;
                pictureBox[j].BackgroundImageLayout = pictureBox1.BackgroundImageLayout;
                pictureBox[j].Dock = pictureBox1.Dock;
                

                ibName[j] = new Label();
                ibValue[j] = new Label();
                ibName[j].Parent = pictureBox[j];
                ibValue[j].Parent = pictureBox[j];
                

                ibName[j].BackColor = ibValue[j].BackColor = Color.Transparent;
                ibName[j].Text = AWS.Config.AWSConfig.sValue[idx].SensorValues[j].name;
                //ibName[j].Dock = DockStyle.Fill;
                ibName[j].AutoSize = false;
                ibName[j].Size = new Size(pictureBox[j].BackgroundImage.Size.Width, pictureBox[j].BackgroundImage.Size.Height/3);
                ibName[j].Font = myFont;
                ibName[j].TextAlign = ContentAlignment.MiddleCenter;
                ibName[j].Anchor = AnchorStyles.Top;
                ibName[j].Dock = DockStyle.Top;
                
                ibValue[j].Text = "\r\n-";
                ibValue[j].Dock = DockStyle.Fill;
                ibValue[j].Font = myFont2;
                ibValue[j].TextAlign = ContentAlignment.MiddleCenter;
                ibValue[j].ForeColor = Color.Yellow;
                                
                tableLayoutPanel2.Controls.Add(pictureBox[j]);
            }

			int nPrevCnt = 0;
			DataCollWatch = new Thread(() =>
			   {
				   Thread.Sleep(1000 * 60 * 3);

				   nPrevCnt = nCallCnt;
				   while (true)
				   {
					   try
					   {
						   if (nCallCnt == nPrevCnt)
						   {
							   iblDevName.ForeColor = Color.Red;
						   }
						   else
						   {
							   nPrevCnt = nCallCnt;
						   }
						   Thread.Sleep(1000 * 60 * 5);
					   } 
					   catch(Exception ex)
					   {
					   iLog.Error(ex.ToString());
					   }
				   }
			   }
			); DataCollWatch.Start();

		}

        private void GenerateTable(int columnCount, int rowCount)
        {
            //Clear out the existing controls, we are generating a new table layout
            tableLayoutPanel2.Controls.Clear();

            //Clear out the existing row and column styles
            tableLayoutPanel2.ColumnStyles.Clear();
            tableLayoutPanel2.RowStyles.Clear();

            //Now we will generate the table, setting up the row and column counts first
            tableLayoutPanel2.ColumnCount = columnCount;
            tableLayoutPanel2.RowCount = rowCount;

            for (int x = 0; x < columnCount; x++)
            {
                //First add a column
                tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

                float RowPercent = 100F / rowCount;
                for (int y = 0; y < rowCount; y++)
                {
                    //Next, add a row.  Only do this when once, when creating the first column
                    if (x == 0)
                    {
                        tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, RowPercent));
                    }
                }
            }
        }

        private String getDirection(String direction)
        {
            string result = "";
            double dir = double.Parse(direction);

            if ((dir >= 348.8) && (dir <= 360.0))
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

        public void DisplayData(KMA2 kma)
        {
            try
            {
				iblDevName.ForeColor = Color.Yellow;
				nCallCnt++;
				for (int i = 0; i < ibValue.Length; i++)
                {
                    switch(AWS.Config.AWSConfig.sValue[idx].SensorValues[i].id)
                    {
                        case "TEMP":
                            ibValue[i].Text = "\r\n" + string.Format("{0:0.0}", (kma.Sensor_0_Datas / 10.0) - 100.0);
                            break;
                        case "WINDIR":
                            ibValue[i].Text = "\r\n" + this.getDirection((kma.Sensor_1_Datas / 10.0).ToString());
                            break;
                        case "WINSPEED":
                            ibValue[i].Text = "\r\n" + string.Format("{0:0.0}", kma.Sensor_2_Datas / 10.0);
                            break;
                        case "RAINSTATUS":
                            if (kma.Sensor_7_Datas > 0)
                                this.ibValue[i].Text = "\r\n" + "유";
                            else
                                this.ibValue[i].Text = "\r\n" + "무";
                            break;
                        case "HUMIDITY":
                            ibValue[i].Text = "\r\n" + string.Format("{0:0}", kma.Sensor_9_Datas / 10.0);
                            break;
                        case "RAINFALL":
                            ibValue[i].Text = "\r\n" + string.Format("{0:0.0}", (kma.Sensor_5_Datas / 10.0));
                            break;
                        case "SUNSHINE":
                            ibValue[i].Text = "\r\n" + string.Format("{0:0.0}", (kma.Sensor1_1_Datas/3600));
                            break;
                        case "VISIBILITY":
                            ibValue[i].Text = "\r\n" + string.Format("{0:0}", (kma.Spare1_Sensor_1_Datas));
                            break;
                            //case "CWEATHER":
                            //    ibValue[i].Text = "\r\n" + string.Format("{0:0}", (kma.Spare2_Sensor_2_Datas));
                            //    break;




                    }
                }
                /*
                this.lblTemp.Text = string.Format("{0:0.0}", (returnKMA.Sensor_0_Datas / 10.0) - 100.0);
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
                */
            }
            catch (Exception ex)
            {
                iLog.Error(ex.ToString());
            }
        }

		private void DefaultForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			try
			{
				DataCollWatch.Abort();
			} catch(Exception ex)
			{
				iLog.Error(ex.ToString());
			}
		}
	}
}
