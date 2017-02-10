//-----------------------------------------------------------------------
// <copyright file="GraphForm.cs" company="[Company Name]">
//     Copyright (c) [Company Name] Corporation.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using AWS.Config;
using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using ZedGraph;

namespace AWS.VIEW
{
	public partial class GraphForm : Form
	{
		static ILog iLog = log4net.LogManager.GetLogger("Logger");

		private MainForm main = null;
		private Boolean isAlive = true;
		private LineItem[] lineItem = null;
		public Dictionary<string, PointPairList> ptDic = new Dictionary<string, PointPairList>();
		public int nItemCode = 0;
		public Boolean bRealTime = true;

		public class ComboBoxItem
		{
			#region Fields
			private int code;
			private string name;
			#endregion

			#region Properties
			public int Code
			{
				get { return code; }
				set { code = value; }
			}
			public string Name
			{
				get { return name; }
				set { name = value; }
			}
			#endregion

			public override string ToString()
			{
				return this.name;
			}
		}

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
			for (int i = 0; i < AWS.Config.AWSConfig.sCount; i++)
			{
				ComboBoxItem item = new ComboBoxItem();
				item.Code = i;
				item.Name = AWS.Config.AWSConfig.sValue[i].Name;
				comboBox1.Items.Add(item);
			}
			comboBox1.SelectedIndex = 0;

			
			ComboBoxItem itm = new ComboBoxItem();
			itm.Code = 0;
			itm.Name = "실시간 그래프";
			comboBox2.Items.Add(itm);

			ComboBoxItem itm2 = new ComboBoxItem();
			itm2.Code = 1;
			itm2.Name = "기간별 조회 그래프";
			comboBox2.Items.Add(itm2);

			comboBox2.SelectedIndex = 0;

			InitGraph();
			//zedGraph.IsShowPointValues = true;
			new Thread(() =>
			{
				while(isAlive)
				{
					if (bRealTime)
					{
						readTimeData(DateTime.Now, nItemCode);
						zedGraph.GraphPane.AxisChange();
						zedGraph.Refresh();
					}

					Thread.Sleep(5000);
				}
			}).Start();

		}

		public void InitGraph()
		{
			float FontSize = 8.0f;

			try
			{
				FontSpec fSpec = new FontSpec("굴림", FontSize, Color.Black, false, false, false);
				fSpec.IsAntiAlias = true;
				fSpec.Border.IsVisible = false;

				zedGraph.GraphPane.Title.Text = AWS.Config.AWSConfig.sValue[nItemCode].Name + " 기상 그래프";
				zedGraph.GraphPane.XAxis.Title.Text = "시간, ";
				zedGraph.GraphPane.YAxis.Title.Text = "풍향, m/s";
				zedGraph.GraphPane.Y2Axis.Title.Text = "풍속, m/s2";
				zedGraph.GraphPane.Y2Axis.IsVisible = true;

				ptDic.Add("풍향", new PointPairList());
				ptDic.Add("풍속", new PointPairList());
				ptDic.Add("기온", new PointPairList());
				ptDic.Add("습도", new PointPairList());
				ptDic.Add("강우량", new PointPairList());
				ptDic.Add("일조", new PointPairList());
				ptDic.Add("시정", new PointPairList());

				zedGraph.GraphPane.XAxis.Type = ZedGraph.AxisType.Date;
				zedGraph.GraphPane.XAxis.Scale.Format = "HH:mm:ss\nyy/MM/dd";
				zedGraph.GraphPane.XAxis.Scale.MinorUnit = DateUnit.Second;
				zedGraph.GraphPane.XAxis.Scale.MajorUnit = DateUnit.Minute;
				zedGraph.GraphPane.XAxis.Scale.Min = new XDate(DateTime.Now.AddHours(-1));
				zedGraph.GraphPane.XAxis.Scale.Max = new XDate(DateTime.Now.AddHours(+1));

				zedGraph.GraphPane.Title.FontSpec = fSpec;
				zedGraph.GraphPane.XAxis.Title.FontSpec = fSpec;
				zedGraph.GraphPane.XAxis.Scale.FontSpec = fSpec;
				zedGraph.GraphPane.YAxis.Title.FontSpec = fSpec;
				zedGraph.GraphPane.YAxis.Scale.FontSpec = fSpec;
				zedGraph.GraphPane.Y2Axis.Title.FontSpec = fSpec;
				zedGraph.GraphPane.Y2Axis.Scale.FontSpec = fSpec;
				zedGraph.GraphPane.Legend.FontSpec = fSpec;

				lineItem = new LineItem[7];

				lineItem[0] = setPoint(ptDic["풍향"], "풍향", Color.Black, 1, 0);
				lineItem[1] = setPoint(ptDic["풍속"], "풍속", Color.Red, 2, 0);

				int idx = zedGraph.GraphPane.AddYAxis("기온, ℃");
				zedGraph.GraphPane.YAxisList[idx].Title.FontSpec = fSpec;
				zedGraph.GraphPane.YAxisList[idx].Scale.FontSpec = fSpec;
				lineItem[2] = setPoint(ptDic["기온"], "기온", Color.Green, 1, idx); // IsVisible = false; ;


				idx = zedGraph.GraphPane.AddYAxis("습도, %");
				zedGraph.GraphPane.YAxisList[idx].Title.FontSpec = fSpec;
				zedGraph.GraphPane.YAxisList[idx].Scale.FontSpec = fSpec;
				lineItem[3] = setPoint(ptDic["습도"], "습도", Color.Blue, 1, idx); //.IsVisible = false;

				idx = zedGraph.GraphPane.AddYAxis("강우량, mm");
				zedGraph.GraphPane.YAxisList[idx].Title.FontSpec = fSpec;
				zedGraph.GraphPane.YAxisList[idx].Scale.FontSpec = fSpec;
				lineItem[4] = setPoint(ptDic["강우량"], "강우량", Color.Crimson, 1, idx); //.IsVisible = false;

				idx = zedGraph.GraphPane.AddY2Axis("일조, hour");
				zedGraph.GraphPane.Y2AxisList[idx].Title.FontSpec = fSpec;
				zedGraph.GraphPane.Y2AxisList[idx].Scale.FontSpec = fSpec;
				zedGraph.GraphPane.Y2AxisList[idx].IsVisible = true;
				lineItem[5] = setPoint(ptDic["일조"], "일조", Color.Indigo, 2, idx); //.IsVisible = false;

				idx = zedGraph.GraphPane.AddY2Axis("시정, m");
				zedGraph.GraphPane.Y2AxisList[idx].Title.FontSpec = fSpec;
				zedGraph.GraphPane.Y2AxisList[idx].Scale.FontSpec = fSpec;
				zedGraph.GraphPane.Y2AxisList[idx].IsVisible = true;
				lineItem[6] = setPoint(ptDic["시정"], "시정", Color.OrangeRed, 2, idx); //.IsVisible = false;

				zedGraph.GraphPane.AxisChange();
				zedGraph.Refresh();
			} catch(Exception ex)
			{
				iLog.Error(ex.ToString());
			}
		}

		public LineItem setPoint(PointPairList point, String Label, Color color, int YIndex, int YAxisIndex)
		{
			LineItem lineItem = null; 
			try
			{
				lineItem = zedGraph.GraphPane.AddCurve(Label, point, color, SymbolType.None);
				lineItem.Line.Width = 1;
				lineItem.Line.IsSmooth = true;
				lineItem.Symbol.Size = 2;
				//LineItem1.Symbol.Fill = new Fill(Color.Black);
				lineItem.Clear();
				if (YIndex == 1)
				{
					lineItem.IsY2Axis = false;
					lineItem.YAxisIndex = YAxisIndex;
				}
				else
				{
					lineItem.IsY2Axis = true;
					lineItem.YAxisIndex = YAxisIndex;
				}
			}
			catch(Exception ex)
			{
				iLog.Error(ex.ToString());
			}

			return lineItem;
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private Boolean readTimeData(DateTime dateTime, int dev_idx)
		{
			OleDbConnection con = null;
			OleDbCommand cmd = null;

			String fileName = String.Format("{0:yyyyMMdd}", dateTime);

			String year = String.Format("{0:yyyy}", dateTime);
			String month = String.Format("{0:MM}", dateTime);
			String day = String.Format("{0:dd}", dateTime);

			string DBPath = AWSConfig.HOME_PATH + "\\AWSDATA\\" + AWS.Config.AWSConfig.sValue[(int)dev_idx].Name + "\\" + year + @"\" + month + @"\aws_" + fileName + ".mdb";

			if (!File.Exists(DBPath))
			{
				MessageBox.Show("Date file isn't exist!");
				return false;
			}

			DataSet readDataSet = new DataSet();

			try
			{
				zedGraph.GraphPane.Title.Text = AWS.Config.AWSConfig.sValue[nItemCode].Name + " 기상 그래프";

				StringBuilder selectQuery = new StringBuilder()
								.Append("SELECT								\n")
								.Append("   RECEIVETIME						\n")
								.Append("  ,TEMP							\n")
								.Append("  ,MIN_TEMP						\n")
								.Append("  ,MAX_TEMP						\n")
								.Append("  ,WD								\n")
								.Append("  ,MIN_WD							\n")
								.Append("  ,MAX_WD							\n")
								.Append("  ,WS								\n")
								.Append("  ,MIN_WS							\n")
								.Append("  ,MAX_WS							\n")
								.Append("  ,RAIN							\n")
								.Append("  ,ISRAIN							\n")
								.Append("  ,HUMIDITY						\n")
								.Append("  ,MIN_HUMIDITY					\n")
								.Append("  ,MAX_HUMIDITY					\n")
								.Append("  ,SUNSHINE						\n")
								.Append("  ,VISIBILITY						\n")
								.Append("FROM AWS_MIN						\n")
								.Append("WHERE DEV_IDX = ").Append(dev_idx.ToString());

				//iLog.Debug("[QUERY] \n" + selectQuery);
				con = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + DBPath);
				cmd = new OleDbCommand(selectQuery.ToString(), con);
				OleDbDataAdapter myDataAdapter = new OleDbDataAdapter(cmd);

				con.Open();
				myDataAdapter.Fill(readDataSet, "aws_min");

				if (readDataSet.Tables[0].Rows.Count > 0)
				{

					DateTime readDateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
					
					int rows = 0;
					{
						zedGraph.GraphPane.XAxis.Scale.Min = new XDate(DateTime.Now.AddMinutes(-30));
						zedGraph.GraphPane.XAxis.Scale.Max = new XDate(DateTime.Now.AddMinutes(+5));

						foreach (LineItem item in lineItem)
						{
							if(item != null) item.Clear();
						}

						for (; rows < 1443; rows++)
						{
							DataRow[] result = readDataSet.Tables[0].Select("receivetime = '" + readDateTime + "'");

							if (result == null || result.Length <= 0)
							{
								for (int i = 0; i < 7; i++)
								{
									switch (i)
									{
										case 0:
											if(ptDic["기온"] != null) ptDic["기온"].Add(new XDate(readDateTime), 0.0f);
											break;
										case 1:
											if(ptDic["풍향"] != null) ptDic["풍향"].Add(new XDate(readDateTime), 0.0f);
											break;
										case 2:
											if(ptDic["풍속"] != null) ptDic["풍속"].Add(new XDate(readDateTime), 0.0f);
											break;
										case 3:
											if(ptDic["강우량"] != null) ptDic["강우량"].Add(new XDate(readDateTime), 0.0f);
											break;
										case 4:
											if(ptDic["습도"] != null) ptDic["습도"].Add(new XDate(readDateTime), 0.0f);
											break;
										case 5:
											if(ptDic["일조"] != null) ptDic["일조"].Add(new XDate(readDateTime), 0.0f);
											break;
										case 6:
											if(ptDic["시정"] != null) ptDic["시정"].Add(new XDate(readDateTime), 0.0f);
											break;
									}
								}
							}
							else
							{

								for (int i = 0; i < 7; i++)
								{
									switch (i)
									{
										case 0:
											ptDic["기온"].Add(new XDate(readDateTime), Convert.ToDouble(result[0][i + 1]));
											break;
										case 1:
											ptDic["풍향"].Add(new XDate(readDateTime), Convert.ToDouble(result[0][i + 3]));
											break;
										case 2:
											ptDic["풍속"].Add(new XDate(readDateTime), Convert.ToDouble(result[0][i + 5]));
											break;
										case 3:
											ptDic["강우량"].Add(new XDate(readDateTime), Convert.ToDouble(result[0][i + 7]));
											break;
										case 4:
											ptDic["습도"].Add(new XDate(readDateTime), Convert.ToDouble(result[0][i + 8]));
											break;
										case 5:
											ptDic["일조"].Add(new XDate(readDateTime), Convert.ToDouble(result[0][i + 9]));
											break;
										case 6:
											ptDic["시정"].Add(new XDate(readDateTime), Convert.ToDouble(result[0][i + 10]));
											break;
									}
								}


								result = null;
							}
							readDateTime = readDateTime.AddMinutes(+1);
						}
					}
				}
				else
				{
					con.Close();
					return false;
				}
			}
			catch (Exception ex)
			{
				iLog.Error(ex.ToString());
			}
			finally
			{
				if (con != null) con.Close();
				cmd = null;
			}

			return true;
		}

		private void GraphForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			isAlive = false;
		}

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			ComboBoxItem item = comboBox1.SelectedItem as ComboBoxItem;
			this.nItemCode = item.Code;
			//readTimeData(DateTime.Now, nItemCode);
		}

		private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
		{
			ComboBoxItem item = comboBox2.SelectedItem as ComboBoxItem;

			if(item.Code == 1)
			{
				label1.Visible = true;
				label2.Visible = true;
				dateTimePicker1.Visible = true;
				dateTimePicker2.Visible = true;
				button1.Visible = true;
				bRealTime = false;
			} else
			{
				label1.Visible = false;
				label2.Visible = false;
				dateTimePicker1.Visible = false;
				dateTimePicker2.Visible = false;
				button1.Visible = false;
				bRealTime = true;
			}
		}
	}
}
