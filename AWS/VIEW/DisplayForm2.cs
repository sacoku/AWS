using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AWS.Config;
using AWS.MODEL;
using AWS.UTIL;
using log4net;

namespace AWS.VIEW
{
    public partial class DisplayForm2 : Form
    {
        private MainForm main = null;
        private Panel.DefaultForm[] frmArray = null;
        Label label1 = new Label();
        Label label2 = new Label();

        ILog iLog = log4net.LogManager.GetLogger("Logger");

        public DisplayForm2()
        {
            InitializeComponent();
        }

        public DisplayForm2(MainForm frm)
        {
            FontFamily familyName = new FontFamily("휴먼둥근헤드라인");
            System.Drawing.Font myFont = new System.Drawing.Font(familyName, 40, FontStyle.Regular, GraphicsUnit.Pixel);

            main = frm;
            InitializeComponent();

            typeof(TableLayoutPanel).GetProperty("DoubleBuffered",
                                                  System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(tableLayoutPanel1, true, null);
            typeof(TableLayoutPanel).GetProperty("DoubleBuffered",
                                                  System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(tableLayoutPanel2, true, null);

            frmArray = new Panel.DefaultForm[AWSConfig.sCount];

            label1.Text = "";

            label1.Parent = tableLayoutPanel2;
            label1.TextAlign = ContentAlignment.MiddleRight;
            label1.Font = myFont;
            label1.ForeColor = Color.Yellow;
            label1.Dock = DockStyle.Fill;
            label1.Anchor = AnchorStyles.Left;
            label1.AutoSize = true;
            tableLayoutPanel2.Controls.Add(label1);


            label2.Text = "****-**-** **:**";

            label2.TextAlign = ContentAlignment.MiddleRight;
            label2.Font = myFont;

            label2.Parent = tableLayoutPanel2;
            label2.ForeColor = Color.Yellow;
            label2.Dock = DockStyle.Fill;
            label2.Anchor = AnchorStyles.Right;
            label2.AutoSize = true;
            label2.BackColor = Color.Transparent;
            tableLayoutPanel2.Controls.Add(label2);
            

            for (int i=0;i<frmArray.Length;i++)
            {
                frmArray[i] = new Panel.DefaultForm(AWSConfig.sValue[i].Name + "\r\n("+ AWSConfig.sValue[i].Ip+ ")\r\n지점번호 : " + AWSConfig.sValue[i].Id, i);
                frmArray[i].TopLevel = false;
                frmArray[i].Parent = this.tableLayoutPanel1;
                frmArray[i].ControlBox = false;
                this.tableLayoutPanel1.Controls.Add(frmArray[i]);
                frmArray[i].Show();
                frmArray[i].Dock = DockStyle.Fill;
            }
        }

        public void DisplayData(KMA2 kma, int idx)
        {
            KMA2 returnKMA = this.ByteChangAll2(kma);

            try
            {
                this.label2.Text = string.Format("{0:00}", returnKMA.Year + 2000) + "/" + string.Format("{0:00}", returnKMA.Month) + "/" + string.Format("{0:00}", returnKMA.Day) + "  " + string.Format("{0:00}", returnKMA.Hour) + ":" + string.Format("{0:00}", returnKMA.Minute);
                frmArray[idx].DisplayData(kma);
            }
            catch (Exception ex)
            {
                iLog.Info("[ERROR] DisplayForm : DisplayData " + ex.Message);
            }
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
