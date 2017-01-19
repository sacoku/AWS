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
            item2[0].Code = 0;
            item2[0].Name = "시간별 데이터";
            comboBox2.Items.Add(item2[0]);
            item2[1] = new ComboBoxItem();
            item2[1].Code = 0;
            item2[1].Name = "월별 데이터";
            comboBox2.Items.Add(item2[1]);

            initGrid(new DateTime(), 0);
            maskedTextBox1.Text = "00:00";
            maskedTextBox2.Text = "23:59:00";
        }

        /**
         * Description : 청송군에 맞추어 GRID 초기화 수정
         * Date : 2016-12-29
         * Writer : 김성현(sacoku)
         */
        private void initGrid(DateTime date, int dev_idx)
        {
            try
            {
                int colWidth = 50;
                this.SensortsReportGrid.Refresh();
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
                backHeader.Border = cellBorder; // DevAge.Drawing.RectangleBorder.RectangleBlack1Width;
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
                                         + " 수집 시간별 관측자료 ( " + date.ToString("yyyy년 MM월 dd일") + "  00:00:00 ~ 23:59:59 )");
                SensortsReportGrid[0, 0].ColumnSpan = columnNum;
                SensortsReportGrid[0, 0].View = viewColumnHeader;
                SensortsReportGrid[0, 0].View.Font = new Font(FontFamily.GenericSansSerif, 8, FontStyle.Bold);
                SensortsReportGrid[0, 0].View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;


                SourceGrid.Cells.ColumnHeader columnHeader;

                columnHeader = new SourceGrid.Cells.ColumnHeader("검지기");
                columnHeader.View = viewColumnHeader;
                columnHeader.View.Font = new Font(FontFamily.GenericSansSerif, 8, FontStyle.Bold);
                columnHeader.View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;

                SensortsReportGrid[1, 0] = columnHeader;
                SensortsReportGrid[1, 0].Column.Width = colWidth * 3;
                //SensortsReportGrid[1, 0].RowSpan = 2;           

                int idx = 1;
                columnHeader = new SourceGrid.Cells.ColumnHeader("기온");
                columnHeader.View = viewColumnHeader;
                columnHeader.View.Font = new Font(FontFamily.GenericSansSerif, 8, FontStyle.Bold);
                columnHeader.View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;
                
                SensortsReportGrid[1, idx] = columnHeader;
                SensortsReportGrid[1, idx].Column.Width = colWidth * 3;
                //SensortsReportGrid[1, 1].RowSpan = 2;
                SensortsReportGrid[1, idx].ColumnSpan = 3; idx += 3;

                columnHeader = new SourceGrid.Cells.ColumnHeader("풍향");
                columnHeader.View = viewColumnHeader;
                columnHeader.View.Font = new Font(FontFamily.GenericSansSerif, 8, FontStyle.Bold);
                columnHeader.View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;

                SensortsReportGrid[1, idx] = columnHeader;
                SensortsReportGrid[1, idx].Column.Width = colWidth * 3;
                //SensortsReportGrid[1, 2].RowSpan = 2;
                SensortsReportGrid[1, idx].ColumnSpan = 3;
                idx += 3;

                columnHeader = new SourceGrid.Cells.ColumnHeader("풍속");
                columnHeader.View = viewColumnHeader;
                columnHeader.View.Font = new Font(FontFamily.GenericSansSerif, 8, FontStyle.Bold);
                columnHeader.View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;

                SensortsReportGrid[1, idx] = columnHeader;
                SensortsReportGrid[1, idx].Column.Width = colWidth * 3;
                //SensortsReportGrid[1, 3].RowSpan = 2;
                SensortsReportGrid[1, idx].ColumnSpan = 3; idx += 3;

                columnHeader = new SourceGrid.Cells.ColumnHeader("강우");
                columnHeader.View = viewColumnHeader;
                columnHeader.View.Font = new Font(FontFamily.GenericSansSerif, 8, FontStyle.Bold);
                columnHeader.View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;

                SensortsReportGrid[1, idx] = columnHeader;
                SensortsReportGrid[1, idx].Column.Width = colWidth * 3;
                //SensortsReportGrid[1, idx].ColumnSpan = 2;
                idx += 1;

                columnHeader = new SourceGrid.Cells.ColumnHeader("강우감지");
                columnHeader.View = viewColumnHeader;
                columnHeader.View.Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold);
                columnHeader.View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;
                SensortsReportGrid[1, idx] = columnHeader;
                SensortsReportGrid[1, idx].Column.Width = colWidth * 3;
                idx += 1;

                columnHeader = new SourceGrid.Cells.ColumnHeader("습도");
                columnHeader.View = viewColumnHeader;
                columnHeader.View.Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold);
                columnHeader.View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;
                SensortsReportGrid[1, idx] = columnHeader;
                SensortsReportGrid[1, idx].Column.Width = colWidth * 3;
                SensortsReportGrid[1, idx].ColumnSpan = 3; idx += 3;


               columnHeader = new SourceGrid.Cells.ColumnHeader("일조");
                columnHeader.View = viewColumnHeader;
                columnHeader.View.Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold);
                columnHeader.View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;

                SensortsReportGrid[1, idx] = columnHeader;
                SensortsReportGrid[1, idx].Column.Width = colWidth * 3; //665 
                //SensortsReportGrid[1, idx].ColumnSpan = 2;
                idx += 1;


                columnHeader = new SourceGrid.Cells.ColumnHeader("시정");
                columnHeader.View = viewColumnHeader;
                columnHeader.View.Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold);
                columnHeader.View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;

                SensortsReportGrid[1, idx] = columnHeader;
                SensortsReportGrid[1, idx].Column.Width = colWidth * 3; //665 
                //SensortsReportGrid[1, idx].ColumnSpan = 2;


                /*
                            columnHeader = new SourceGrid.Cells.ColumnHeader("현천");
                            columnHeader.View = viewColumnHeader;
                            columnHeader.View.Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold);
                            columnHeader.View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;

                            SensortsReportGrid[1, 8] = columnHeader;
                            SensortsReportGrid[1, 8].Column.Width = 45; //665   
                            SensortsReportGrid[1, 8].RowSpan = 2;
                */


                columnHeader = new SourceGrid.Cells.ColumnHeader("시간");
                columnHeader.View = viewColumnHeader;
                columnHeader.View.Font = new Font(FontFamily.GenericSansSerif, 8, FontStyle.Bold);
                columnHeader.View.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;

                SensortsReportGrid[2, 0] = columnHeader;
                SensortsReportGrid[2, 0].Column.Width = colWidth + 20/* * 3*/;
                //SensortsReportGrid[1, 1].RowSpan = 2;            

                for (int i = 1; i <= columnNum - 1;)
                {
                    columnHeader = new SourceGrid.Cells.ColumnHeader("평균");
                    columnHeader.View = viewColumnHeader;
                    columnHeader.View.Font = new Font(FontFamily.GenericSansSerif, 8, FontStyle.Bold);
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
                            SensortsReportGrid[2, i].Column.Width = colWidth + 20 /* * 3*/;
                            i++;
                            break;
                           
                    }
                }
            } 
            catch(Exception e)
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
                view.Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold);
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
        }

        private void readData(DateTime dateTime, int dev_idx)
        {
            //DateTime readDateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0);
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
                return;
            }

            DataSet readDataSet = new DataSet();
            
            try
            {            
                String selectQuery = "SELECT												\n"
                                          + "         RECEIVETIME							\n"
                                          + "        ,TEMP										\n"
                                          + "        ,MIN_TEMP								\n"
                                          + "        ,MAX_TEMP								\n"
                                          + "        ,WD                                            \n"
                                          + "        ,MIN_WD                                     \n"
                                          + "        ,MAX_WD                                    \n"
                                          + "        ,WS                                             \n"
                                          + "        ,MIN_WS                                      \n"
                                          + "        ,MAX_WS                                     \n"
                                          + "        ,RAIN                                           \n"
                                          + "        ,ISRAIN                                        \n"
                                          + "        ,HUMIDITY                                   \n"
                                          + "        ,MIN_HUMIDITY                            \n"
                                          + "        ,MAX_HUMIDITY                           \n"
                                          + "        ,SUNSHINE                                  \n"
                                          + "        ,VISIBILITY                                   \n"
                                          + "FROM AWS_MIN                                  \n"
                                          + "WHERE DEV_IDX = " + dev_idx;

                iLog.Debug("[QUERY] \n" + selectQuery);
                con = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + DBPath);
                cmd = new OleDbCommand(selectQuery, con);
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

                    //while (readDateTime < nextDay)
                    {
                        for (int rows = 3; rows <(ts .TotalMinutes+1 + 3); rows++)
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
                                SensortsReportGrid[rows, 0].View.Font = new Font(FontFamily.GenericSansSerif, 7, FontStyle.Bold);

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
                                SensortsReportGrid[rows, 0].View.Font = new Font(FontFamily.GenericSansSerif, 7, FontStyle.Bold);

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
                    return;
                }
            }
            catch (Exception ex)
            {
                iLog.Info("[ERROR] " + ex.Message);
            }
            finally
            {
                //pwdDataSet = null;
                con.Close();
                cmd = null;
            }            
        }

        private void PrintButton_Click(object sender, EventArgs e)
        {
            if (this.SensortsReportGrid.Rows.Count <= 0)
                return;

            PrintPreviewDialog dlg = new PrintPreviewDialog();
            SourceGrid.Exporter.GridPrintDocument pd = new SourceGrid.Exporter.GridPrintDocument(this.SensortsReportGrid);

			iLog.Debug("Rows : " + this.SensortsReportGrid.Rows.Count);
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
    }
}
