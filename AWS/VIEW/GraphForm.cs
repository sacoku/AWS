using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using ZedGraph;

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
			zedGraph.GraphPane.Title.Text = "기상 그래프";
			zedGraph.GraphPane.XAxis.Title.Text = "Time, s";
			zedGraph.GraphPane.YAxis.Title.Text = "Velocity, m/s";
			zedGraph.GraphPane.Y2Axis.Title.Text = "Acceleration, m/s2";
			zedGraph.GraphPane.Y2Axis.IsVisible = true;

			YAxis yAxis3 = new YAxis("온도, m");
			zedGraph.GraphPane.YAxisList.Add(yAxis3);
			yAxis3.IsVisible = true;

			Y2Axis yAxis4 = new Y2Axis("Energy");
			zedGraph.GraphPane.Y2AxisList.Add(yAxis4);
			yAxis4.IsVisible = true;

			YAxis yAxis5 = new YAxis("Distance, m");
			zedGraph.GraphPane.YAxisList.Add(yAxis5);
			yAxis5.IsVisible = true;

			zedGraph.GraphPane.XAxis.MajorGrid.IsVisible = true;
			zedGraph.GraphPane.YAxis.MajorGrid.IsVisible = true;
			zedGraph.GraphPane.Y2Axis.MajorGrid.IsVisible = true;
			//zedGraph.GraphPane.XAxis.MajorGrid.Color = Color.White;
			//zedGraph.GraphPane.YAxis.MajorGrid.Color = Color.White;
			//zedGraph.GraphPane.Y2Axis.MajorGrid.Color = Color.White;

			zedGraph.GraphPane.XAxis.MajorGrid.DashOff = 5;                          // Seta a Intensidade da Linha no Eixo X.
			zedGraph.GraphPane.XAxis.MajorGrid.IsVisible = true;                     // Setta Linhas no Eixo X ou seja na Vertical.
			zedGraph.GraphPane.XAxis.MajorTic.IsOpposite = false;
			zedGraph.GraphPane.XAxis.MinorTic.IsAllTics = false; // mudei aqui para ver se para de pular o grafico.

			zedGraph.GraphPane.XAxis.Scale.FontSpec.Angle = 90;                      // Setta o Angulo do Scale do Eixo X.
			zedGraph.GraphPane.XAxis.Scale.FontSpec.Family = "Arial, Narrow";          // Setta a Fonte da Scale no Eixo X.
			zedGraph.GraphPane.XAxis.Scale.FontSpec.FontColor = Color.Fuchsia;       // Setta a Cor da Legenda do Dado que Entrara no Eixo X.
			zedGraph.GraphPane.XAxis.Scale.FontSpec.IsBold = true;                   // Setta Negrito na Scale no Eixo X.
			zedGraph.GraphPane.XAxis.Scale.FontSpec.Size = 10;                       // Setta o Tamanho da Fonte da Scale no Eixo X.
			zedGraph.GraphPane.XAxis.Scale.Format = "HH:mm:ss\nyy/MM/dd";            // Setta o formato de data e hora e o \n faz uma mudança de linha.
			zedGraph.GraphPane.XAxis.Scale.IsSkipCrossLabel = true;
			zedGraph.GraphPane.XAxis.Scale.IsPreventLabelOverlap = true;

			// Set the initial viewed range
			zedGraph.GraphPane.XAxis.Scale.Min = new XDate(DateTime.Now);            // We want to use time from now
			zedGraph.GraphPane.XAxis.Scale.Max = new XDate(DateTime.Now.AddMinutes(1));        // to 5 min per default
			zedGraph.GraphPane.XAxis.Scale.MinorUnit = DateUnit.Second;              // set the minimum x unit to time/seconds
			zedGraph.GraphPane.XAxis.Scale.MajorUnit = DateUnit.Minute;              // set the maximum x unit to time/minutes
			zedGraph.GraphPane.XAxis.Scale.MinorStep = 1;                            // Setta os tracinhos da reta.
			zedGraph.GraphPane.XAxis.Scale.MajorStep = 10;                            // Setta o intervalo do tempo na Scale X.
																					  //myPane02m.XAxis.Scale.MinGrace = 1;
																					  //myPane02m.XAxis.Scale.MaxGrace = 10;

			zedGraph.GraphPane.XAxis.Title.FontSpec.FontColor = Color.DarkViolet;    // Setta a Cor do Titulo no Eixo X.
			zedGraph.GraphPane.XAxis.Title.Text = "Date & Time";                     // Setta a Legenda Date & Time no Eixo X.

			zedGraph.GraphPane.XAxis.Type = ZedGraph.AxisType.Date;                             // Setta o Tipo do Eixo X como Data.
			zedGraph.GraphPane.Legend.Position = ZedGraph.LegendPos.TopCenter;
			//myPane02m.Legend.Location.AlignH = AlignH.Right;
			//myPane02m.Legend.Location.AlignV = AlignV.Top;


			zedGraph.GraphPane.YAxis.Scale.Min = -10;
			zedGraph.GraphPane.YAxis.Scale.Max = 10;

			PointPairList examplePointPairLitst = new PointPairList();

			LineItem exampleLineItem = zedGraph.GraphPane.AddCurve("EXAMPLE", examplePointPairLitst, Color.Yellow, SymbolType.None);

			exampleLineItem.Line.Width = 5;
			exampleLineItem.Symbol.Fill = new Fill(Color.Black);
			exampleLineItem.Clear();

			Random rand = new Random(1);
			for (int i = 0; i < 3; i++)
			{
				examplePointPairLitst.Add(i, rand.NextDouble());
			}

			//zedGraph.GraphPane.Chart.Fill = new Fill(Color.Black);
			zedGraph.GraphPane.AxisChange();
			zedGraph.Invalidate();

		}
	}
}
