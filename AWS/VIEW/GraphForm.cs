using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace AWS.VIEW
{
	public partial class GraphForm : Form
	{
		private MainForm main = null;

		public GraphForm()
		{
			InitializeComponent();
		}

		public GraphForm(MainForm m)
		{
			InitializeComponent();
			main = m;
		}

		private void GraphForm_Load(object sender, EventArgs e)
		{
			chart1.Series.Clear(); //default series를 삭제한다.

			Series sSin = chart1.Series.Add("A"); //새로운 series 생성
			Series sSin2 = chart1.Series.Add("B"); //새로운 series 생성
			
			sSin.ChartType = SeriesChartType.Line; //그래프를 '선'으로 지정
			sSin2.ChartType = SeriesChartType.Line; //그래프를 '선'으로 지정

			chart1.ChartAreas[0].AxisX.LabelStyle.Format = "HH:mm";
			//chart1.ChartAreas[1].AxisX.LabelStyle.Format = "yyyy HH:mm";

			Random rnd = new Random();
			DateTime d = DateTime.Now;
			DateTime d2 = DateTime.Now;
			for (double k = 0; k < 200; k += 0.1)
			{
				sSin.Points.AddXY(d.ToString("yyyy HH:mm"), rnd.Next(1, 200));
				d = d.AddMinutes(+1);
			}

			//데이터 포인트 저장
			for (double k = 0; k < 100; k += 0.1)
			{
				sSin2.Points.AddXY(d.ToString("HH:mm"), rnd.Next(1, 200));
				d = d.AddMinutes(+1);
			}
		}
	}
}
