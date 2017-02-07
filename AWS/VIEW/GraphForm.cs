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
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using ZedGraph;

namespace AWS.VIEW
{
	public partial class GraphForm : Form
	{
		static ILog iLog = log4net.LogManager.GetLogger("Logger");

		private MainForm main = null;
		public Dictionary<string, PointPairList> ptDic = new Dictionary<string, PointPairList>();

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
			/*
			zedGraph.GraphPane.Title.FontSpec.Size = 8.0f; 
			zedGraph.GraphPane.Title.Text = "기상 그래프";
			zedGraph.GraphPane.XAxis.Title.Text = "시간, ";
			zedGraph.GraphPane.YAxis.Title.Text = "풍향, m/s";
			zedGraph.GraphPane.Y2Axis.Title.Text = "풍속, m/s2";
			zedGraph.GraphPane.Y2Axis.IsVisible = true;

			zedGraph.GraphPane.Legend.FontSpec.Size = 8.0f;

			zedGraph.GraphPane.YAxis.Scale.MaxAuto = zedGraph.GraphPane.YAxis.Scale.MinAuto = true;
			zedGraph.GraphPane.Y2Axis.Scale.MaxAuto = zedGraph.GraphPane.Y2Axis.Scale.MinAuto = true;

			zedGraph.GraphPane.YAxis.Title.FontSpec.Size = 8.0f;
			zedGraph.GraphPane.YAxis.Scale.FontSpec.Size = 8.0f;
			zedGraph.GraphPane.XAxis.Title.FontSpec.Size = 8.0f;
			zedGraph.GraphPane.XAxis.Scale.FontSpec.Size = 8.0f;
			zedGraph.GraphPane.Y2Axis.Title.FontSpec.Size = 8.0f;
			zedGraph.GraphPane.Y2Axis.Scale.FontSpec.Size = 8.0f;
			zedGraph.GraphPane.X2Axis.Title.FontSpec.Size = 8.0f;
			zedGraph.GraphPane.X2Axis.Scale.FontSpec.Size = 8.0f;
						
			YAxis yAxis3 = new YAxis("기온, ℃");
			zedGraph.GraphPane.YAxisList.Add(yAxis3);
			yAxis3.Scale.Min = -40;
			yAxis3.Scale.Max = 40;
			yAxis3.Title.FontSpec.Size = 8.0f; // * (this.Size.Width / 100);
			yAxis3.Scale.FontSpec.Size = 8.0f; // * (this.Size.Width / 100);

			YAxis yAxis4 = new YAxis("습도, %");
			zedGraph.GraphPane.YAxisList.Add(yAxis4);
			yAxis3.Scale.Min = 0;
			yAxis3.Scale.Max = 100;
			yAxis4.Title.FontSpec.Size = 8.0f; // * (this.Size.Width / 100);
			yAxis4.Scale.FontSpec.Size = 8.0f; // * (this.Size.Width / 100);

			Y2Axis yAxis5 = new Y2Axis("강우량, mm");
			zedGraph.GraphPane.Y2AxisList.Add(yAxis5);
			yAxis5.Scale.MinAuto = yAxis5.Scale.MaxAuto = true;
			yAxis5.Title.FontSpec.Size = 8.0f; // * (this.Size.Width / 100);
			yAxis5.Scale.FontSpec.Size = 8.0f; // * (this.Size.Width / 100);

			Y2Axis yAxis6 = new Y2Axis("일조, hour");
			zedGraph.GraphPane.Y2AxisList.Add(yAxis6);
			yAxis6.Scale.MinAuto = yAxis6.Scale.MaxAuto = true;
			yAxis6.Title.FontSpec.Size = 8.0f; // * (this.Size.Width / 100);
			yAxis6.Scale.FontSpec.Size = 8.0f; // * (this.Size.Width / 100);

			YAxis yAxis7 = new YAxis("시정, m");
			zedGraph.GraphPane.YAxisList.Add(yAxis7);
			yAxis7.Scale.MinAuto = yAxis4.Scale.MaxAuto = true;
			yAxis7.Title.FontSpec.Size = 8.0f; // * (this.Size.Width / 100);
			yAxis7.Scale.FontSpec.Size = 8.0f; // * (this.Size.Width / 100);

			yAxis3.IsVisible = true;
			yAxis4.IsVisible = true;
			yAxis5.IsVisible = true;
			yAxis6.IsVisible = true;
			yAxis7.IsVisible = true;

			zedGraph.GraphPane.XAxis.MajorGrid.IsVisible = true;
			zedGraph.GraphPane.YAxis.MajorGrid.IsVisible = true;
			zedGraph.GraphPane.Y2Axis.MajorGrid.IsVisible = true;
			zedGraph.GraphPane.XAxis.MajorGrid.Color = Color.White;
			zedGraph.GraphPane.YAxis.MajorGrid.Color = Color.White;
			zedGraph.GraphPane.Y2Axis.MajorGrid.Color = Color.White;

			zedGraph.GraphPane.XAxis.MajorGrid.DashOff = 5;
			zedGraph.GraphPane.XAxis.MajorGrid.IsVisible = true;
			zedGraph.GraphPane.XAxis.MajorTic.IsOpposite = false;
			zedGraph.GraphPane.XAxis.MinorTic.IsAllTics = false;

			zedGraph.GraphPane.XAxis.Scale.FontSpec.Angle = 90;
			zedGraph.GraphPane.XAxis.Scale.FontSpec.Family = "Arial, Narrow";
			zedGraph.GraphPane.XAxis.Scale.FontSpec.FontColor = Color.Fuchsia;
			zedGraph.GraphPane.XAxis.Scale.FontSpec.IsBold = true;
			zedGraph.GraphPane.XAxis.Scale.Format = "HH:mm:ss\nyy/MM/dd";
			zedGraph.GraphPane.XAxis.Scale.IsSkipCrossLabel = true;
			zedGraph.GraphPane.XAxis.Scale.IsPreventLabelOverlap = true;
			

			// Set the initial viewed range
			zedGraph.GraphPane.XAxis.Scale.Min = new XDate(new DateTime(2017, 02, 07, 0, 0, 0));
			zedGraph.GraphPane.XAxis.Scale.Max = new XDate(new DateTime(2017, 02, 07, 23, 59, 59));
			zedGraph.GraphPane.XAxis.Scale.MinorUnit = DateUnit.Second;
			zedGraph.GraphPane.XAxis.Scale.MajorUnit = DateUnit.Minute;
			zedGraph.GraphPane.XAxis.Scale.MinorStep = 1;
			zedGraph.GraphPane.XAxis.Scale.MajorStep = 10;
			//zedGraph.GraphPane.XAxis.Scale.MinGrace = 1;
			//zedGraph.GraphPane.XAxis.Scale.MaxGrace = 10;
			//zedGraph.GraphPane.XAxis.Title.FontSpec.FontColor = Color.DarkViolet;

			zedGraph.GraphPane.XAxis.Type = ZedGraph.AxisType.Date;
			zedGraph.GraphPane.Legend.Position = ZedGraph.LegendPos.TopCenter;
			//zedGraph.GraphPane.Legend.Location.AlignH = AlignH.Right;
			//zedGraph.GraphPane.Legend.Location.AlignV = AlignV.Top;
			zedGraph.GraphPane.Chart.Fill = new Fill(Color.Gray);	

			ptDic.Add("풍향", new PointPairList());
			ptDic.Add("풍속", new PointPairList());
			ptDic.Add("기온", new PointPairList());
			ptDic.Add("습도", new PointPairList());
			ptDic.Add("강우량", new PointPairList());
			ptDic.Add("일조", new PointPairList());
			ptDic.Add("시정", new PointPairList());

			setPoint(ptDic["풍향"], "풍향", Color.Red, 1, 0);
			setPoint(ptDic["풍속"], "풍속", Color.PeachPuff, 2, 0);
			setPoint(ptDic["기온"], "기온", Color.Blue, 1, 1);
			setPoint(ptDic["습도"], "습도", Color.Black, 1, 2);
			setPoint(ptDic["강우량"], "강우량", Color.Green, 2, 1);
			setPoint(ptDic["일조"], "일조", Color.Maroon, 2, 2);
			setPoint(ptDic["시정"], "시정", Color.Navy, 1, 3);

			readTimeData(DateTime.Now, 1);

			zedGraph.GraphPane.AxisChange();
			zedGraph.Refresh();
			*/

			InitGraph();
			zedGraph.IsShowPointValues = true;

		}

		public void InitGraph()
		{
			float FontSize = 8.0f;

			FontSpec fSpec = new FontSpec("굴림", FontSize, Color.Black, false, false, false);
			fSpec.IsAntiAlias = true;
			fSpec.Border.IsVisible = false;
			
			zedGraph.GraphPane.Title.Text = "기상 그래프";
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

			zedGraph.GraphPane.Title.FontSpec = fSpec;
			zedGraph.GraphPane.XAxis.Title.FontSpec = fSpec;
			zedGraph.GraphPane.XAxis.Scale.FontSpec = fSpec;
			zedGraph.GraphPane.YAxis.Title.FontSpec = fSpec;
			zedGraph.GraphPane.YAxis.Scale.FontSpec = fSpec;
			zedGraph.GraphPane.Y2Axis.Title.FontSpec = fSpec;
			zedGraph.GraphPane.Y2Axis.Scale.FontSpec = fSpec;
			zedGraph.GraphPane.Legend.FontSpec = fSpec;

			setPoint(ptDic["풍향"], "풍향", Color.Black, 1, 0);
			setPoint(ptDic["풍속"], "풍속", Color.Red, 2, 0);

			int idx = zedGraph.GraphPane.AddYAxis("기온, ℃");
			zedGraph.GraphPane.YAxisList[idx].Title.FontSpec = fSpec;
			zedGraph.GraphPane.YAxisList[idx].Scale.FontSpec = fSpec;
			setPoint(ptDic["기온"], "기온", Color.Green, 1, idx).IsVisible = false; ;


			idx = zedGraph.GraphPane.AddYAxis("습도, %");
			zedGraph.GraphPane.YAxisList[idx].Title.FontSpec = fSpec;
			zedGraph.GraphPane.YAxisList[idx].Scale.FontSpec = fSpec;
			setPoint(ptDic["습도"], "습도", Color.Blue, 1, idx).IsVisible = false;

			idx = zedGraph.GraphPane.AddYAxis("강우량, mm");
			zedGraph.GraphPane.YAxisList[idx].Title.FontSpec = fSpec;
			zedGraph.GraphPane.YAxisList[idx].Scale.FontSpec = fSpec;
			setPoint(ptDic["강우량"], "강우량", Color.Coral, 1, idx).IsVisible = false;

			idx = zedGraph.GraphPane.AddY2Axis("일조, hour");
			zedGraph.GraphPane.Y2AxisList[idx].Title.FontSpec = fSpec;
			zedGraph.GraphPane.Y2AxisList[idx].Scale.FontSpec = fSpec;
			zedGraph.GraphPane.Y2AxisList[idx].IsVisible = true;
			setPoint(ptDic["일조"], "일조", Color.Crimson, 2, idx).IsVisible = false;

			idx = zedGraph.GraphPane.AddY2Axis("시정, m");
			zedGraph.GraphPane.Y2AxisList[idx].Title.FontSpec = fSpec;
			zedGraph.GraphPane.Y2AxisList[idx].Scale.FontSpec = fSpec;
			zedGraph.GraphPane.Y2AxisList[idx].IsVisible = true;
			setPoint(ptDic["시정"], "시정", Color.Cyan, 2, idx).IsVisible = false;

			readTimeData(DateTime.Now, 0);

			zedGraph.GraphPane.AxisChange();
			zedGraph.Refresh();
		}

		public LineItem setPoint(PointPairList point, String Label, Color color, int YIndex, int YAxisIndex)
		{
			LineItem LineItem1 = zedGraph.GraphPane.AddCurve(Label, point, color, SymbolType.Triangle);

			LineItem1.Line.Width = 1;
			LineItem1.Line.IsSmooth = true;
			LineItem1.Symbol.Size = 2;
			//LineItem1.Symbol.Fill = new Fill(Color.Black);
			LineItem1.Clear();
			if (YIndex == 1)
			{
				LineItem1.IsY2Axis = false;
				LineItem1.YAxisIndex = YAxisIndex;
			}
			else
			{
				LineItem1.IsY2Axis = true;
				LineItem1.YAxisIndex = YAxisIndex;
			}

			return LineItem1;
		}

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
						for (; rows < 1443; rows++)
						{
							DataRow[] result = readDataSet.Tables[0].Select("receivetime = '" + readDateTime + "'");

							if (result == null || result.Length <= 0)
							{
								/*
								for (int i = 0; i < 7; i++)
								{
									switch (i)
									{
										case 0:
											ptDic["기온"].Add(new XDate(readDateTime), 0.0f);
											break;
										case 1:
											ptDic["풍향"].Add(new XDate(readDateTime), 0.0f);
											break;
										case 2:
											ptDic["풍속"].Add(new XDate(readDateTime), 0.0f);
											break;
										case 3:
											ptDic["강우량"].Add(new XDate(readDateTime), 0.0f);
											break;
										case 4:
											ptDic["습도"].Add(new XDate(readDateTime), 0.0f);
											break;
										case 5:
											ptDic["일조"].Add(new XDate(readDateTime), 0.0f);
											break;
										case 6:
											ptDic["시정"].Add(new XDate(readDateTime), 0.0f);
											break;
									}
								}
								*/
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
					MessageBox.Show("데이터가 없습니다.");
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
	}
}
