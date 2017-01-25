using AWS.UTIL.DataGridControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Forms;

using System.Reflection;
using System.Data.OleDb;
using System.IO;
using System.Collections;
using AWS.CONTROLS;
using System.Diagnostics;
using log4net;
using AWS.Config;

namespace AWS.VIEW
{
    public partial class ReportForm : Form
    {
        private MainForm main = null;
        static ILog iLog = log4net.LogManager.GetLogger("Logger");
        int columnNum = 17;

		enum RPT_VIEW_MODE : int
		{
			TIME_MODE =0,
			DAILY_MODE =1,
			MONTHLY_MODE =2
		};

		int eMode = (int)RPT_VIEW_MODE.DAILY_MODE;

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

        public ReportForm(MainForm mainForm)
        {
            this.main = mainForm;

            InitializeComponent();

            for (int i = 0; i < AWS.Config.AWSConfig.sCount; i++)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Code = i;
                item.Name = AWS.Config.AWSConfig.sValue[i].Name;
                comboBox1.Items.Add(item);
            }
            comboBox1.SelectedIndex = 0;

            ComboBoxItem[] item2 = new ComboBoxItem[3];
            item2[0] = new ComboBoxItem();
            item2[0].Code = (int)RPT_VIEW_MODE.TIME_MODE;
            item2[0].Name = "정시 수집별 관측자료";
            comboBox2.Items.Add(item2[0]);
			item2[1] = new ComboBoxItem();
			item2[1].Code = (int)RPT_VIEW_MODE.DAILY_MODE;
			item2[1].Name = "수집 시간별 관측자료";
			comboBox2.Items.Add(item2[1]);
			item2[2] = new ComboBoxItem();
            item2[2].Code = (int)RPT_VIEW_MODE.MONTHLY_MODE;
			item2[2].Name = "일별 통계 자료";
            comboBox2.Items.Add(item2[2]);
			comboBox2.SelectedIndex = 0;

			initGrid(DateTime.Now, 0);
			maskedTextBox1.Text = "00:00";
            maskedTextBox2.Text = "23:59:00";
        }

		private void initGrid(DateTime date, int dev_id)
		{
			if (eMode == (int)RPT_VIEW_MODE.TIME_MODE)
			{
				initTimeGrid(date, dev_id);
			}
			else if (eMode == (int)RPT_VIEW_MODE.DAILY_MODE)
			{
				initDayGrid(date, dev_id);
			}
			else if (eMode == (int)RPT_VIEW_MODE.MONTHLY_MODE)
			{
				initMonthGrid(date, dev_id);
			}
		}

        /**
         * Description : 청송군에 맞추어 GRID 초기화 수정
         * Date : 2016-12-29
         * Writer : 김성현(sacoku)
         */
        private void initTimeGrid(DateTime date, int dev_idx)
        {
            try
            {
                int colWidth = 50;
				columnNum = 17;

				SensortsReportGrid.RowsCount = 0;
				this.SensortsReportGrid.Refresh();
				SensortsReportGrid.RowsCount = 3;

				//Create the grid
				SensortsReportGrid.BorderStyle = BorderStyle.FixedSingle;

                SensortsReportGrid.ColumnsCount = columnNum;
                SensortsReportGrid.FixedRows = 3;

                //Border
                DevAge.Drawing.BorderLine border = new DevAge.Drawing.BorderLine(Color.DarkGray, 1);
                DevAge.Drawing.RectangleBorder cellBorder = new DevAge.Drawing.RectangleBorder(border, border);

                //ColumnHeader view
                SourceGrid.Cells.Views.ColumnHeader viewColumnHeader = new SourceGrid.Cells.Views.ColumnHeader();
                DevAge.Drawing.VisualElements.ColumnHeader backHeader = new DevAge.Drawing.VisualElements.ColumnHeader();
                backHeader.BackColor = Color.LightGray;
                backHeader.Border = cellBorder;
                viewColumnHeader.Background = backHeader;
                viewColumnHeader.ForeColor = Color.Black;
                viewColumnHeader.Font = new Font("굴림", 8, FontStyle.Underline);

                //Views
                CellBackColorAlternate viewNormal = new CellBackColorAlternate(Color.LightGray, Color.Gray);
                viewNormal.Border = cellBorder;
                CheckBoxBackColorAlternate viewCheckBox = new CheckBoxBackColorAlternate(Color.Khaki, Color.DarkKhaki);
                viewCheckBox.Border = cellBorder;

                //Editors
                SourceGrid.Cells.Editors.TextBox editorString = new SourceGrid.Cells.Editors.TextBox(typeof(string));
                SourceGrid.Cells.Editors.TextBoxUITypeEditor editorDateTime = new SourceGrid.Cells.Editors.TextBoxUITypeEditor(typeof(DateTime));

                //1 Header Row
                SensortsReportGrid[0, 0] = new MyHeader(" " + AWS.Config.AWSConfig.sValue[dev_idx].Name 
                                         + " 정시 수집별 관측자료 ( " + date.ToString("yyyy년 MM월 dd일") + "  00:00:00 ~ 23:59:59 )");
                SensortsReportGrid[0, 0].ColumnSpan = columnNum;
                SensortsReportGrid[0, 0].View = viewColumnHeader;
                SensortsReportGrid[0, 0].View.Font = new Font("굴림", 8, FontStyle.Bold);
                SensortsReportGrid[0, 0].View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;

				SourceGrid.Cells.ColumnHeader columnHeader;

                columnHeader = new SourceGrid.Cells.ColumnHeader("검지기");
                columnHeader.View = viewColumnHeader;
                columnHeader.View.Font = new Font("굴림", 8, FontStyle.Bold);
                columnHeader.View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;

                SensortsReportGrid[1, 0] = columnHeader;
                SensortsReportGrid[1, 0].Column.Width = colWidth * 3;

                int idx = 1;
                columnHeader = new SourceGrid.Cells.ColumnHeader("기온");
                columnHeader.View = viewColumnHeader;
                columnHeader.View.Font = new Font("굴림", 8, FontStyle.Bold);
                columnHeader.View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;
                
                SensortsReportGrid[1, idx] = columnHeader;
                SensortsReportGrid[1, idx].Column.Width = colWidth * 3;
                SensortsReportGrid[1, idx].ColumnSpan = 3; idx += 3;

                columnHeader = new SourceGrid.Cells.ColumnHeader("풍향");
                columnHeader.View = viewColumnHeader;
                columnHeader.View.Font = new Font("굴림", 8, FontStyle.Bold);
                columnHeader.View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;

                SensortsReportGrid[1, idx] = columnHeader;
                SensortsReportGrid[1, idx].Column.Width = colWidth * 3;
                SensortsReportGrid[1, idx].ColumnSpan = 3;
                idx += 3;

                columnHeader = new SourceGrid.Cells.ColumnHeader("풍속");
                columnHeader.View = viewColumnHeader;
                columnHeader.View.Font = new Font("굴림", 8, FontStyle.Bold);
                columnHeader.View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;

                SensortsReportGrid[1, idx] = columnHeader;
                SensortsReportGrid[1, idx].Column.Width = colWidth * 3;
                SensortsReportGrid[1, idx].ColumnSpan = 3; idx += 3;

                columnHeader = new SourceGrid.Cells.ColumnHeader("강우");
                columnHeader.View = viewColumnHeader;
                columnHeader.View.Font = new Font("굴림", 8, FontStyle.Bold);
                columnHeader.View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;

                SensortsReportGrid[1, idx] = columnHeader;
                SensortsReportGrid[1, idx].Column.Width = colWidth * 3;
                idx += 1;

                columnHeader = new SourceGrid.Cells.ColumnHeader("강우감지");
                columnHeader.View = viewColumnHeader;
                columnHeader.View.Font = new Font("굴림", 8, FontStyle.Bold);
                columnHeader.View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;
                SensortsReportGrid[1, idx] = columnHeader;
                SensortsReportGrid[1, idx].Column.Width = colWidth * 3;
                idx += 1;

                columnHeader = new SourceGrid.Cells.ColumnHeader("습도");
                columnHeader.View = viewColumnHeader;
                columnHeader.View.Font = new Font("굴림", 8, FontStyle.Bold);
                columnHeader.View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;
                SensortsReportGrid[1, idx] = columnHeader;
                SensortsReportGrid[1, idx].Column.Width = colWidth * 3;
                SensortsReportGrid[1, idx].ColumnSpan = 3; idx += 3;


               columnHeader = new SourceGrid.Cells.ColumnHeader("일조");
                columnHeader.View = viewColumnHeader;
                columnHeader.View.Font = new Font("굴림", 8, FontStyle.Bold);
                columnHeader.View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;

                SensortsReportGrid[1, idx] = columnHeader;
                SensortsReportGrid[1, idx].Column.Width = colWidth * 3; //665 
                idx += 1;


                columnHeader = new SourceGrid.Cells.ColumnHeader("시정");
                columnHeader.View = viewColumnHeader;
                columnHeader.View.Font = new Font("굴림", 8, FontStyle.Bold);
                columnHeader.View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;

                SensortsReportGrid[1, idx] = columnHeader;
                SensortsReportGrid[1, idx].Column.Width = colWidth * 3; //665 
				idx += 1;
				columnNum = idx;

				columnHeader = new SourceGrid.Cells.ColumnHeader("시간");
                columnHeader.View = viewColumnHeader;
                columnHeader.View.Font = new Font("굴림", 8, FontStyle.Bold);
                columnHeader.View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;

                SensortsReportGrid[2, 0] = columnHeader;
                SensortsReportGrid[2, 0].Column.Width = colWidth + 20/* * 3*/;

                for (int i = 1; i <= columnNum - 1;)
                {
                    columnHeader = new SourceGrid.Cells.ColumnHeader("평균");
                    columnHeader.View = viewColumnHeader;
                    columnHeader.View.Font = new Font("굴림", 8, FontStyle.Bold);
                    columnHeader.View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;

                    SensortsReportGrid[2, i] = columnHeader;
                    SensortsReportGrid[2, i].Column.Width = colWidth;


                    switch (i)
                    {
                        case 1:
                        case 4:
                        case 7:
                        case 12:
                            {

                                i++;

                                columnHeader = new SourceGrid.Cells.ColumnHeader("최소");
                                columnHeader.View = viewColumnHeader;
                                columnHeader.View.Font = new Font(FontFamily.GenericSansSerif, 8, FontStyle.Bold);
                                columnHeader.View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;

                                SensortsReportGrid[2, i] = columnHeader;
                                SensortsReportGrid[2, i].Column.Width = colWidth;
                                i++;

                                columnHeader = new SourceGrid.Cells.ColumnHeader("최고");
                                columnHeader.View = viewColumnHeader;
                                columnHeader.View.Font = new Font(FontFamily.GenericSansSerif, 8, FontStyle.Bold);
                                columnHeader.View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;

                                SensortsReportGrid[2, i] = columnHeader;
                                SensortsReportGrid[2, i].Column.Width = colWidth;
                                i++;
                            }
                            break;
                        default:
                            SensortsReportGrid[2, i].Column.Width = colWidth + 20;
                            i++;
                            break;
                           
                    }
                }

				//SensortsReportGrid.AutoSizeCells();
				//SensortsReportGrid.AutoStretchColumnsToFitWidth = true;
				//SensortsReportGrid.Columns.StretchToFit();
			}
			catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

		private void initDayGrid(DateTime date, int dev_idx)
		{
			try
			{
				int colWidth = 30;
				columnNum = 8;

				SensortsReportGrid.RowsCount = 0;
				this.SensortsReportGrid.Refresh();
				SensortsReportGrid.RowsCount = 2;
				//Create the grid
				SensortsReportGrid.BorderStyle = BorderStyle.FixedSingle;

				SensortsReportGrid.ColumnsCount = columnNum;
				SensortsReportGrid.FixedRows = 2;

				//Border
				DevAge.Drawing.BorderLine border = new DevAge.Drawing.BorderLine(Color.DarkGray, 1);
				DevAge.Drawing.RectangleBorder cellBorder = new DevAge.Drawing.RectangleBorder(border, border);

				//ColumnHeader view
				SourceGrid.Cells.Views.ColumnHeader viewColumnHeader = new SourceGrid.Cells.Views.ColumnHeader();
				DevAge.Drawing.VisualElements.ColumnHeader backHeader = new DevAge.Drawing.VisualElements.ColumnHeader();
				backHeader.BackColor = Color.LightGray;
				backHeader.Border = cellBorder;
				viewColumnHeader.Background = backHeader;
				viewColumnHeader.ForeColor = Color.Black;
				viewColumnHeader.Font = new Font("굴림", 8, FontStyle.Bold);

				//Views
				CellBackColorAlternate viewNormal = new CellBackColorAlternate(Color.LightGray, Color.Gray);
				viewNormal.Border = cellBorder;
				CheckBoxBackColorAlternate viewCheckBox = new CheckBoxBackColorAlternate(Color.Khaki, Color.DarkKhaki);
				viewCheckBox.Border = cellBorder;

				//Editors
				SourceGrid.Cells.Editors.TextBox editorString = new SourceGrid.Cells.Editors.TextBox(typeof(string));
				SourceGrid.Cells.Editors.TextBoxUITypeEditor editorDateTime = new SourceGrid.Cells.Editors.TextBoxUITypeEditor(typeof(DateTime));

				//1 Header Row
				SensortsReportGrid[0, 0] = new MyHeader(" " + AWS.Config.AWSConfig.sValue[dev_idx].Name
										 + " 수집 시간별 관측자료 ( " + date.ToString("yyyy년 MM월") + ")");
				SensortsReportGrid[0, 0].ColumnSpan = columnNum;
				SensortsReportGrid[0, 0].View = viewColumnHeader;
				SensortsReportGrid[0, 0].View.Font = new Font("굴림", 8, FontStyle.Bold);
				SensortsReportGrid[0, 0].View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;


				SourceGrid.Cells.ColumnHeader columnHeader;

				columnHeader = new SourceGrid.Cells.ColumnHeader("시간");
				columnHeader.View = viewColumnHeader;
				columnHeader.View.Font = new Font("굴림", 8, FontStyle.Bold);
				columnHeader.View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;

				SensortsReportGrid[1, 0] = columnHeader;
				SensortsReportGrid[1, 0].Column.Width = colWidth * 3;

				int idx = 1;
				columnHeader = new SourceGrid.Cells.ColumnHeader("기온");
				columnHeader.View = viewColumnHeader;
				columnHeader.View.Font = new Font("굴림", 8, FontStyle.Bold);
				columnHeader.View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;

				SensortsReportGrid[1, idx] = columnHeader;
				SensortsReportGrid[1, idx].Column.Width = colWidth * 3;
				idx += 1;

				columnHeader = new SourceGrid.Cells.ColumnHeader("풍향");
				columnHeader.View = viewColumnHeader;
				columnHeader.View.Font = new Font("굴림", 8, FontStyle.Bold);
				columnHeader.View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;

				SensortsReportGrid[1, idx] = columnHeader;
				SensortsReportGrid[1, idx].Column.Width = colWidth * 3;
				idx += 1;

				columnHeader = new SourceGrid.Cells.ColumnHeader("풍속");
				columnHeader.View = viewColumnHeader;
				columnHeader.View.Font = new Font("굴림", 8, FontStyle.Bold);
				columnHeader.View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;

				SensortsReportGrid[1, idx] = columnHeader;
				SensortsReportGrid[1, idx].Column.Width = colWidth * 3;
				idx += 1;

				columnHeader = new SourceGrid.Cells.ColumnHeader("강우");
				columnHeader.View = viewColumnHeader;
				columnHeader.View.Font = new Font("굴림", 8, FontStyle.Bold);
				columnHeader.View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;

				SensortsReportGrid[1, idx] = columnHeader;
				SensortsReportGrid[1, idx].Column.Width = colWidth * 3;
				idx += 1;

				columnHeader = new SourceGrid.Cells.ColumnHeader("습도");
				columnHeader.View = viewColumnHeader;
				columnHeader.View.Font = new Font("굴림", 8, FontStyle.Bold);
				columnHeader.View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;
				SensortsReportGrid[1, idx] = columnHeader;
				SensortsReportGrid[1, idx].Column.Width = colWidth * 3;
				idx += 1;


				columnHeader = new SourceGrid.Cells.ColumnHeader("일조");
				columnHeader.View = viewColumnHeader;
				columnHeader.View.Font = new Font("굴림", 8, FontStyle.Bold);
				columnHeader.View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;

				SensortsReportGrid[1, idx] = columnHeader;
				SensortsReportGrid[1, idx].Column.Width = colWidth * 3; //665 
				idx += 1;


				columnHeader = new SourceGrid.Cells.ColumnHeader("시정");
				columnHeader.View = viewColumnHeader;
				columnHeader.View.Font = new Font("굴림", 8, FontStyle.Bold);
				columnHeader.View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;

				SensortsReportGrid[1, idx] = columnHeader;
				SensortsReportGrid[1, idx].Column.Width = colWidth * 3; //665 
				idx += 1;
				columnNum = idx;

				//SensortsReportGrid.AutoSizeCells();
				//SensortsReportGrid.AutoStretchColumnsToFitWidth = true;
				//SensortsReportGrid.Columns.StretchToFit();
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message);
			}
		}

		private void initMonthGrid(DateTime date, int dev_idx)
		{
			try
			{
				int colWidth = 30;
				columnNum = 8;

				SensortsReportGrid.RowsCount = 0;
				this.SensortsReportGrid.Refresh();
				SensortsReportGrid.RowsCount = 2;
				//Create the grid
				SensortsReportGrid.BorderStyle = BorderStyle.FixedSingle;

				SensortsReportGrid.ColumnsCount = columnNum;
				SensortsReportGrid.FixedRows = 2;

				//Border
				DevAge.Drawing.BorderLine border = new DevAge.Drawing.BorderLine(Color.DarkGray, 1);
				DevAge.Drawing.RectangleBorder cellBorder = new DevAge.Drawing.RectangleBorder(border, border);

				//ColumnHeader view
				SourceGrid.Cells.Views.ColumnHeader viewColumnHeader = new SourceGrid.Cells.Views.ColumnHeader();
				DevAge.Drawing.VisualElements.ColumnHeader backHeader = new DevAge.Drawing.VisualElements.ColumnHeader();
				backHeader.BackColor = Color.LightGray;
				backHeader.Border = cellBorder;
				viewColumnHeader.Background = backHeader;
				viewColumnHeader.ForeColor = Color.Black;
				viewColumnHeader.Font = new Font("굴림", 8, FontStyle.Bold);

				//Views
				CellBackColorAlternate viewNormal = new CellBackColorAlternate(Color.LightGray, Color.Gray);
				viewNormal.Border = cellBorder;
				CheckBoxBackColorAlternate viewCheckBox = new CheckBoxBackColorAlternate(Color.Khaki, Color.DarkKhaki);
				viewCheckBox.Border = cellBorder;

				//Editors
				SourceGrid.Cells.Editors.TextBox editorString = new SourceGrid.Cells.Editors.TextBox(typeof(string));
				SourceGrid.Cells.Editors.TextBoxUITypeEditor editorDateTime = new SourceGrid.Cells.Editors.TextBoxUITypeEditor(typeof(DateTime));

				//1 Header Row
				SensortsReportGrid[0, 0] = new MyHeader(" " + AWS.Config.AWSConfig.sValue[dev_idx].Name
										 + " 일별 통계 자료 ( " + date.ToString("yyyy년 MM월") + ")");
				SensortsReportGrid[0, 0].ColumnSpan = columnNum;
				SensortsReportGrid[0, 0].View = viewColumnHeader;
				SensortsReportGrid[0, 0].View.Font = new Font("굴림", 8, FontStyle.Bold);
				SensortsReportGrid[0, 0].View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;


				SourceGrid.Cells.ColumnHeader columnHeader;

				columnHeader = new SourceGrid.Cells.ColumnHeader("일시");
				columnHeader.View = viewColumnHeader;
				columnHeader.View.Font = new Font("굴림", 8, FontStyle.Bold);
				columnHeader.View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;

				SensortsReportGrid[1, 0] = columnHeader;
				SensortsReportGrid[1, 0].Column.Width = colWidth * 3;

				int idx = 1;
				columnHeader = new SourceGrid.Cells.ColumnHeader("기온");
				columnHeader.View = viewColumnHeader;
				columnHeader.View.Font = new Font("굴림", 8, FontStyle.Bold);
				columnHeader.View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;

				SensortsReportGrid[1, idx] = columnHeader;
				SensortsReportGrid[1, idx].Column.Width = colWidth * 3;
				idx += 1;

				columnHeader = new SourceGrid.Cells.ColumnHeader("풍향");
				columnHeader.View = viewColumnHeader;
				columnHeader.View.Font = new Font("굴림", 8, FontStyle.Bold);
				columnHeader.View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;

				SensortsReportGrid[1, idx] = columnHeader;
				SensortsReportGrid[1, idx].Column.Width = colWidth * 3;
				idx += 1;

				columnHeader = new SourceGrid.Cells.ColumnHeader("풍속");
				columnHeader.View = viewColumnHeader;
				columnHeader.View.Font = new Font("굴림", 8, FontStyle.Bold);
				columnHeader.View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;

				SensortsReportGrid[1, idx] = columnHeader;
				SensortsReportGrid[1, idx].Column.Width = colWidth * 3;
				idx += 1;

				columnHeader = new SourceGrid.Cells.ColumnHeader("강우");
				columnHeader.View = viewColumnHeader;
				columnHeader.View.Font = new Font("굴림", 8, FontStyle.Bold);
				columnHeader.View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;

				SensortsReportGrid[1, idx] = columnHeader;
				SensortsReportGrid[1, idx].Column.Width = colWidth * 3;
				idx += 1;

				columnHeader = new SourceGrid.Cells.ColumnHeader("습도");
				columnHeader.View = viewColumnHeader;
				columnHeader.View.Font = new Font("굴림", 8, FontStyle.Bold);
				columnHeader.View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;
				SensortsReportGrid[1, idx] = columnHeader;
				SensortsReportGrid[1, idx].Column.Width = colWidth * 3;
				idx += 1;


				columnHeader = new SourceGrid.Cells.ColumnHeader("일조");
				columnHeader.View = viewColumnHeader;
				columnHeader.View.Font = new Font("굴림", 8, FontStyle.Bold);
				columnHeader.View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;

				SensortsReportGrid[1, idx] = columnHeader;
				SensortsReportGrid[1, idx].Column.Width = colWidth * 3; //665 
				idx += 1;


				columnHeader = new SourceGrid.Cells.ColumnHeader("시정");
				columnHeader.View = viewColumnHeader;
				columnHeader.View.Font = new Font("굴림", 8, FontStyle.Bold);
				columnHeader.View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;

				SensortsReportGrid[1, idx] = columnHeader;
				SensortsReportGrid[1, idx].Column.Width = colWidth * 3; //665 
				idx += 1;
				columnNum = idx;

				//SensortsReportGrid.AutoSizeCells();
				//SensortsReportGrid.AutoStretchColumnsToFitWidth = true;
				//SensortsReportGrid.Columns.StretchToFit();
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message);
			}
		}

		private class CellBackColorAlternate : SourceGrid.Cells.Views.Cell
        {
            public CellBackColorAlternate(Color firstColor, Color secondColor)
            {
                FirstBackground = new DevAge.Drawing.VisualElements.BackgroundSolid(firstColor);
                SecondBackground = new DevAge.Drawing.VisualElements.BackgroundSolid(secondColor);
            }

            private DevAge.Drawing.VisualElements.IVisualElement mFirstBackground;
            public DevAge.Drawing.VisualElements.IVisualElement FirstBackground
            {
                get { return mFirstBackground; }
                set { mFirstBackground = value; }
            }

            private DevAge.Drawing.VisualElements.IVisualElement mSecondBackground;
            public DevAge.Drawing.VisualElements.IVisualElement SecondBackground
            {
                get { return mSecondBackground; }
                set { mSecondBackground = value; }
            }

            protected override void PrepareView(SourceGrid.CellContext context)
            {
                base.PrepareView(context);

                if (Math.IEEERemainder(context.Position.Row, 2) == 0)
                    Background = FirstBackground;
                else
                    Background = SecondBackground;
            }
        }

        private class MyHeader : SourceGrid.Cells.ColumnHeader
        {
            public MyHeader(object value) : base(value)
            {
                //1 Header Row
                SourceGrid.Cells.Views.ColumnHeader view = new SourceGrid.Cells.Views.ColumnHeader();
                view.Font = new Font("굴림", 10, FontStyle.Bold);
                view.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;
                View = view;

                AutomaticSortEnabled = false;
            }
        }


        private class CheckBoxBackColorAlternate : SourceGrid.Cells.Views.CheckBox
        {
            public CheckBoxBackColorAlternate(Color firstColor, Color secondColor)
            {
                FirstBackground = new DevAge.Drawing.VisualElements.BackgroundSolid(firstColor);
                SecondBackground = new DevAge.Drawing.VisualElements.BackgroundSolid(secondColor);
            }

            private DevAge.Drawing.VisualElements.IVisualElement mFirstBackground;
            public DevAge.Drawing.VisualElements.IVisualElement FirstBackground
            {
                get { return mFirstBackground; }
                set { mFirstBackground = value; }
            }

            private DevAge.Drawing.VisualElements.IVisualElement mSecondBackground;
            public DevAge.Drawing.VisualElements.IVisualElement SecondBackground
            {
                get { return mSecondBackground; }
                set { mSecondBackground = value; }
            }

            protected override void PrepareView(SourceGrid.CellContext context)
            {
                base.PrepareView(context);

                if (Math.IEEERemainder(context.Position.Row, 2) == 0)
                    Background = FirstBackground;
                else
                    Background = SecondBackground;
            }
        }

        private void btnRetrieve_Click(object sender, EventArgs e)
        {
            DateTime dateTime = this.dateTimePicker1.Value.Date;
            DateTime currentTime = DateTime.Now;

            int result = DateTime.Compare(dateTime, currentTime);

            if (result > 0)
            {
                MessageBox.Show("날짜를 다시 선택해 주세요");
                return;
            }

            ComboBoxItem item  = comboBox1.SelectedItem as ComboBoxItem;

			this.readData(dateTime, item.Code);
			//this.readDataMonth(dateTime, item.Code);
		}

		private void readData(DateTime dateTime, int dev_idx)
		{
			Boolean ret = false;
			
			if (eMode == (int)RPT_VIEW_MODE.TIME_MODE)
			{
				ret = readTimeData(dateTime, dev_idx);
			}
			else if (eMode == (int)RPT_VIEW_MODE.DAILY_MODE)
			{
				ret = readDayData(dateTime, dev_idx);
			}
			else if (eMode == (int)RPT_VIEW_MODE.MONTHLY_MODE)
			{
				ret = readMonthData(dateTime, dev_idx);
			}

			if (ret) MessageBox.Show("데이터 로드가 완료 되었습니다.");
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

                iLog.Debug("[QUERY] \n" + selectQuery);
                con = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + DBPath);
                cmd = new OleDbCommand(selectQuery.ToString(), con);
                OleDbDataAdapter myDataAdapter = new OleDbDataAdapter(cmd);

                con.Open();
                myDataAdapter.Fill(readDataSet, "aws_min");
                
                if (readDataSet.Tables[0].Rows.Count > 0)
                {
                    this.initGrid(dateTime, dev_idx);

                    DateTime readDateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day,
                                                              int.Parse(maskedTextBox1.Text.Substring(0, 2)), int.Parse(maskedTextBox1.Text.Substring(3, 2)), 0);
                    DateTime readDateTime2 = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day,
                                                              int.Parse(maskedTextBox2.Text.Substring(0, 2)), int.Parse(maskedTextBox2.Text.Substring(3, 2)), 0);

                    TimeSpan ts = readDateTime2 - readDateTime;

					SensortsReportGrid.RowsCount = 1443; //+ readDataSet.Tables[0].Rows.Count;
					//while (readDateTime < nextDay)
					int rows = 3;
					{
                        for (; rows <(ts .TotalMinutes+1 + 3); rows++)
                        {
                            DataRow[] result = readDataSet.Tables[0].Select("receivetime = '" + readDateTime + "'");

                            SourceGrid.Cells.Views.Cell yellowView = new SourceGrid.Cells.Views.Cell();
                            yellowView.BackColor = Color.Gray;
                            yellowView.ForeColor = Color.White;

                            if (result == null || result.Length <= 0)
                            {
                                SourceGrid.Cells.Cell l_Cell = new SourceGrid.Cells.Cell(readDateTime.ToString("HH:mm:ss"), typeof(string));
                                l_Cell.View = yellowView;
                                
                                SensortsReportGrid[rows, 0] = l_Cell;
                                SensortsReportGrid[rows, 0].View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;
                                SensortsReportGrid[rows, 0].View.Font = new Font("굴림", 7, FontStyle.Regular);

                                for (int i = 1; i < columnNum; i++)
                                {
                                    SourceGrid.Cells.Cell Cell = new SourceGrid.Cells.Cell("");
                                    Cell.View = yellowView;

                                    SensortsReportGrid[rows, i] = new SourceGrid.Cells.Cell(" ", typeof(string));
                                    SensortsReportGrid[rows, i] = Cell;
                                }
                            }
                            else
                            {
                                SensortsReportGrid[rows, 0] = new SourceGrid.Cells.Cell(readDateTime.ToString("HH:mm:ss"), typeof(string));
                                SensortsReportGrid[rows, 0].View.BackColor = Color.White;
                                SensortsReportGrid[rows, 0].View.ForeColor = Color.Black;
                                SensortsReportGrid[rows, 0].View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;
                                SensortsReportGrid[rows, 0].View.Font = new Font("굴림", 7, FontStyle.Regular);

                                for(int i=1;i<= columnNum - 1; i++)
                                {
                                    SensortsReportGrid[rows, i] = new SourceGrid.Cells.Cell(result[0][i] == null ? "" : result[0][i].ToString(), typeof(string));
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
                iLog.Info("[ERROR] " + ex.Message);
            }
            finally
            {
				if(con != null) con.Close();
                cmd = null;
            }

			return true;
		}

		private Boolean readDayData(DateTime dateTime, int dev_idx)
		{
			OleDbConnection con = null;
			OleDbCommand cmd = null;

			String fileName = String.Format("{0:yyyyMM}", dateTime);

			String year = String.Format("{0:yyyy}", dateTime);
			String month = String.Format("{0:MM}", dateTime);
			String day = String.Format("{0:dd}", dateTime);

			string DBPath = AWSConfig.HOME_PATH + "\\AWSDATA\\" + AWS.Config.AWSConfig.sValue[(int)dev_idx].Name + "\\" + year + @"\" + @"\aws_" + fileName + ".mdb";

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
								.Append("   AWS_DATE						\n")
								.Append("  ,TEMP							\n")
								.Append("  ,WD								\n")
								.Append("  ,WS								\n")
								.Append("  ,RAIN							\n")
								.Append("  ,HUMIDITY						\n")
								.Append("  ,SUNSHINE						\n")
								.Append("  ,VISIBILITY						\n")
								.Append("FROM AWS_MONTH						\n");

				iLog.Debug("[QUERY] \n" + selectQuery);
				con = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + DBPath);
				cmd = new OleDbCommand(selectQuery.ToString(), con);
				OleDbDataAdapter myDataAdapter = new OleDbDataAdapter(cmd);

				con.Open();
				myDataAdapter.Fill(readDataSet, "aws_month");

				if (readDataSet.Tables[0].Rows.Count > 0)
				{
					this.initGrid(dateTime, dev_idx);

					DateTime readDateTime = new DateTime(dateTime.Year, dateTime.Month, 1, 0, 0, 0);
					DateTime dt = new DateTime(dateTime.Year, dateTime.Month, 1, 0, 0, 0);

					SensortsReportGrid.RowsCount = 2 + readDataSet.Tables[0].Rows.Count;
					int rows = 2;
					while (readDateTime.Month == dt.Month)
					{
						DataRow[] result = readDataSet.Tables[0].Select("aws_date = '" + dt.ToString("yyyy-MM-dd") + "'");

						SourceGrid.Cells.Views.Cell yellowView = new SourceGrid.Cells.Views.Cell();
						yellowView.BackColor = Color.Gray;
						yellowView.ForeColor = Color.White;

						if (result == null || result[0][1].ToString() == "")
						{
							SourceGrid.Cells.Cell l_Cell = new SourceGrid.Cells.Cell(dt.ToString("yyyy-MM-dd"), typeof(string));
							l_Cell.View = yellowView;

							SensortsReportGrid[rows, 0] = l_Cell;
							SensortsReportGrid[rows, 0].View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;
							SensortsReportGrid[rows, 0].View.Font = new Font("굴림", 7, FontStyle.Regular);

							for (int i = 1; i < columnNum; i++)
							{
								SourceGrid.Cells.Cell Cell = new SourceGrid.Cells.Cell("");
								Cell.View = yellowView;

								SensortsReportGrid[rows, i] = new SourceGrid.Cells.Cell(" ", typeof(string));
								SensortsReportGrid[rows, i] = Cell;
							}
						}
						else
						{
							SensortsReportGrid[rows, 0] = new SourceGrid.Cells.Cell(dt.ToString("yyyy-MM-dd"), typeof(string));
							SensortsReportGrid[rows, 0].View.BackColor = Color.White;
							SensortsReportGrid[rows, 0].View.ForeColor = Color.Black;
							SensortsReportGrid[rows, 0].View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;
							SensortsReportGrid[rows, 0].View.Font = new Font("굴림", 7, FontStyle.Regular);
							for (int i = 1; i <= columnNum - 1; i++)
							{
								SensortsReportGrid[rows, i] = new SourceGrid.Cells.Cell(result[0][i] == null ? "" : result[0][i].ToString(), typeof(string));
							}

							result = null;
						}
						dt = dt.AddDays(+1);
						rows++;
					}

					SensortsReportGrid.RowsCount = rows - 1;
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
				iLog.Error("[ERROR] " + ex.Message);
				iLog.Error("[ERROR] " + ex.StackTrace);
			}
			finally
			{
				if (con != null) con.Close();
				cmd = null;
			}

			return false;
		}

		private Boolean readMonthData(DateTime dateTime, int dev_idx)
		{
			OleDbConnection con = null;
			OleDbCommand cmd = null;

			String fileName = String.Format("{0:yyyyMM}", dateTime);

			String year = String.Format("{0:yyyy}", dateTime);
			String month = String.Format("{0:MM}", dateTime);
			String day = String.Format("{0:dd}", dateTime);

			string DBPath = AWSConfig.HOME_PATH + "\\AWSDATA\\" + AWS.Config.AWSConfig.sValue[(int)dev_idx].Name + "\\" + year + @"\" + @"\aws_" + fileName + ".mdb";

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
								.Append("   AWS_DATE						\n")
								.Append("  ,TEMP							\n")
								.Append("  ,WD								\n")
								.Append("  ,WS								\n")
								.Append("  ,RAIN							\n")
								.Append("  ,HUMIDITY						\n")
								.Append("  ,SUNSHINE						\n")
								.Append("  ,VISIBILITY						\n")
								.Append("FROM AWS_MONTH						\n");

				iLog.Debug("[QUERY] \n" + selectQuery);
				con = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + DBPath);
				cmd = new OleDbCommand(selectQuery.ToString(), con);
				OleDbDataAdapter myDataAdapter = new OleDbDataAdapter(cmd);

				con.Open();
				myDataAdapter.Fill(readDataSet, "aws_month");

				if (readDataSet.Tables[0].Rows.Count > 0)
				{
					this.initGrid(dateTime, dev_idx);

					DateTime readDateTime = new DateTime(dateTime.Year, dateTime.Month, 1, 0,0,0);
					DateTime dt = new DateTime(dateTime.Year, dateTime.Month, 1, 0, 0, 0);

					SensortsReportGrid.RowsCount = 2 + readDataSet.Tables[0].Rows.Count;
					int rows = 2;
					while (readDateTime.Month == dt.Month)
					{
						DataRow[] result = readDataSet.Tables[0].Select("aws_date = '" + dt.ToString("yyyy-MM-dd") + "'");

						SourceGrid.Cells.Views.Cell yellowView = new SourceGrid.Cells.Views.Cell();
						yellowView.BackColor = Color.Gray;
						yellowView.ForeColor = Color.White;

						if (result == null || result[0][1].ToString() == "")
						{
							SourceGrid.Cells.Cell l_Cell = new SourceGrid.Cells.Cell(dt.ToString("yyyy-MM-dd"), typeof(string));
							l_Cell.View = yellowView;

							SensortsReportGrid[rows, 0] = l_Cell;
							SensortsReportGrid[rows, 0].View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;
							SensortsReportGrid[rows, 0].View.Font = new Font("굴림", 7, FontStyle.Regular);

							for (int i = 1; i < columnNum; i++)
							{
								SourceGrid.Cells.Cell Cell = new SourceGrid.Cells.Cell("");
								Cell.View = yellowView;

								SensortsReportGrid[rows, i] = new SourceGrid.Cells.Cell(" ", typeof(string));
								SensortsReportGrid[rows, i] = Cell;
							}
						}
						else
						{
							SensortsReportGrid[rows, 0] = new SourceGrid.Cells.Cell(dt.ToString("yyyy-MM-dd"), typeof(string));
							SensortsReportGrid[rows, 0].View.BackColor = Color.White;
							SensortsReportGrid[rows, 0].View.ForeColor = Color.Black;
							SensortsReportGrid[rows, 0].View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;
							SensortsReportGrid[rows, 0].View.Font = new Font("굴림", 7, FontStyle.Regular);
							for (int i = 1; i <= columnNum-1; i++)
							{
								SensortsReportGrid[rows, i] = new SourceGrid.Cells.Cell(result[0][i] == null ? "" : result[0][i].ToString(), typeof(string));
							}

							result = null;
						}
						dt = dt.AddDays(+1);
						rows++;
					}

					SensortsReportGrid.RowsCount = rows - 1;
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
				iLog.Error("[ERROR] " + ex.Message);
				iLog.Error("[ERROR] " + ex.StackTrace);
			}
			finally
			{
				if (con != null) con.Close();
				cmd = null;
			}

			return false;
		}

		private void PrintButton_Click(object sender, EventArgs e)
        {
            if (this.SensortsReportGrid.Rows.Count <= 0)
                return;

            PrintPreviewDialog dlg = new PrintPreviewDialog();
            SourceGrid.Exporter.GridPrintDocument pd = new SourceGrid.Exporter.GridPrintDocument(this.SensortsReportGrid);
			
			pd.RangeToPrint = new SourceGrid.Range(0, 0, this.SensortsReportGrid.Rows.Count - 1, this.SensortsReportGrid.Columns.Count - 1);

            pd.DefaultPageSettings.Margins.Left = 50;
            pd.DefaultPageSettings.Margins.Right = 50;
            pd.DefaultPageSettings.Margins.Top = 20;
            pd.DefaultPageSettings.Margins.Bottom = 20;
            pd.DefaultPageSettings.Landscape = true;

			pd.PageFooterText = "\tPage [PageNo] from [PageCount]";
            dlg.Document = pd;
			((Form)dlg).StartPosition = FormStartPosition.CenterParent;
            dlg.ShowDialog(this);
        }

        private void SensorsDocument_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {

        }

		private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
		{
			ComboBoxItem item = comboBox2.SelectedItem as ComboBoxItem;
			eMode = item.Code;
			initGrid(DateTime.Now, 0);
		}
	}
}
