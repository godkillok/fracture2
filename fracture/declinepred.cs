using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.Spreadsheet;
using DevExpress.XtraCharts;
using System.Drawing.Imaging;
using System.IO;
using System.Diagnostics;
using DevExpress.Spreadsheet.Charts;
using DevExpress.Spreadsheet.Drawings;
using DevExpress.XtraSpreadsheet;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Services;
using DevExpress.XtraSpreadsheet.Menu;
namespace fracture
{
    public partial class declinepred : Form
    {
        XYDiagram Diagram;
        ChartControl myChart;

        public bool selected_flag = false;
        List<double> getData;//输入的产油量
        List<double> cumoil;//累增油
        List<double> cumtime;//有效期
        List<double> cummonth;//有效期 
        List<double> initialq;//初始增油量
        List<double> initialqdaily;//初始增油量
        List<double> oil_qmonthly;//拟合油的结果
        List<int> startrow;//kaishi
        List<int> endrow;//jieshu
        List<string> wellname;//输入的产油量
        List<double> decData;//拟合段的时间和产油量
        List<double> forcastnp;
        List<double> forcasttime;
        List<double> forcastiniq;
        List<double> forcast_ann_np;
        List<double> forcast_ann_time;
        List<double> forcast_ann_iniq;
        List<double> forcast_cf_np;
        List<double> forcast_cf_time;
        List<double> forcast_cf_iniq;
        List<double[]> ployfitinfo;//拟合段信息汇总，包括：开始阶段，结束阶段，QI,DI,N,拟合相关系数
        List<DateTime> DataTime;//输入的时间
        List<DateTime> StartDataTime;//输入的时间
        List<DateTime> StartTime0;//输入的时间
        List<double> smresult;//平滑结果

        //XYDiagram Diagram { get { return myChart.Diagram as XYDiagram; } }
        bool dragging = false;
        public Point firstPoint = new Point(0, 0);  //鼠标第一点 ，对应屏幕
        public Point secondPoint = new Point(0, 0);  //鼠标第二点 
        DiagramCoordinates seccoords;//对应图中坐标系的值
        DiagramCoordinates firstcoords;//对应图中坐标系的值
        ControlCoordinates maxcoords;//图中对应p屏幕坐标系的值
        ControlCoordinates mincoords;//图中对应p屏幕坐标系的值
        int seriesnum;//图中series的数目
        int phase_num;//添加的阶段数
        Rectangle selectionRectangle = Rectangle.Empty;

        public declinepred()
        {
            InitializeComponent();
            intialspreadsheet();
            intialspreadsheet2();
            ribbonControl1.SelectedPage = ribbonControl1.PageCategories[0].Pages[0];
        }
        void intialspreadsheet()
        {

            Worksheet worksheet = spreadsheetControl1.Document.Worksheets[0];
            IWorkbook workbook = spreadsheetControl1.Document;
            workbook.Styles[DevExpress.Spreadsheet.BuiltInStyleId.Normal].Font.Name = "Times New Roman";
            workbook.Styles[DevExpress.Spreadsheet.BuiltInStyleId.Normal].Font.Size = 11;
            workbook.Styles[DevExpress.Spreadsheet.BuiltInStyleId.Normal].Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
            workbook.Styles[DevExpress.Spreadsheet.BuiltInStyleId.Normal].Font.FontStyle = DevExpress.Spreadsheet.SpreadsheetFontStyle.Bold;
            workbook.Styles[DevExpress.Spreadsheet.BuiltInStyleId.Normal].Borders.SetAllBorders(Color.Black, BorderLineStyle.Thin);
            workbook.Worksheets[0].Range["A1:Z1"].ColumnWidthInCharacters = 12;
            workbook.Worksheets[0].Columns[0].RowHeight = 70;
            worksheet.Rows[0].Fill.BackgroundColor = Color.FromArgb(125, 217, 217, 217);//Color.Navy;    ;//Color.Navy;
        }

        private void spreadsheetControl1_CellBeginEdit(object sender, SpreadsheetCellCancelEventArgs e)
        {
            Worksheet worksheet = spreadsheetControl1.Document.Worksheets.ActiveWorksheet;
            IWorkbook workbook = spreadsheetControl1.Document;
            if (workbook.Worksheets.ActiveWorksheet != workbook.Worksheets[0])
                return;
            if (e.RowIndex == 0)
                return;
            if (startrow == null)
                return;
            if (endrow == null)
                return;
            int str = startrow[e.RowIndex - 1];
            int end = endrow[e.RowIndex - 1];

            draw(str, end, e.RowIndex - 1);

        }

        void intialspreadsheet2()
        {
            IWorkbook workbook = spreadsheetControl1.Document;
            Worksheet worksheet1 = workbook.Worksheets.Add("incre oil");

        }
        private void importdata(List<double> getData, List<DateTime> DataTime, List<string> wellname, List<DateTime> StartDataTime)
        {
            Worksheet worksheet = spreadsheetControl1.Document.Worksheets[1];

            // Perform the export.
            int i;
            i = 1;
            while (worksheet[i, 0].Value.IsEmpty == false)
            {
                double tempdata;
                wellname.Add(worksheet[i, 0].Value.ToString());
                DataTime.Add(worksheet[i, 2].Value.DateTimeValue);
                StartDataTime.Add(worksheet[i, 1].Value.DateTimeValue);
                tempdata = worksheet[i, 3].Value.NumericValue;//产油
                getData.Add(tempdata);
                i++;
            }
        }

        private void importdata(List<double> cumoil, List<double> cumtime, List<double> initialqdaily, List<DateTime> StartTime0)
        {
            Worksheet worksheet = spreadsheetControl1.Document.Worksheets[0];

            // Perform the export.
            int i;
            i = 1;
            while (worksheet[i, 0].Value.IsEmpty == false)
            {
                cumoil.Add(worksheet[i, 4].Value.NumericValue);//累增油
                cumtime.Add(worksheet[i, 8].Value.NumericValue);//有效期
                initialqdaily.Add(worksheet[i, 7].Value.NumericValue);//初始增油量
                StartTime0.Add(worksheet[i, 1].Value.DateTimeValue);//开始时间

                i++;
            }
        }

        private void importdata(List<double> forcastnp, List<double> forcasttime, List<double> forcastiniq, List<double> forcast_ann_np, List<double> forcast_ann_time, List<double> forcast_ann_iniq)
        {
            Worksheet worksheet = spreadsheetControl1.Document.Worksheets[0];

            // Perform the export.
            int i;
            i = 1;
            while (worksheet[i, 0].Value.IsEmpty == false)
            {
                forcastnp.Add(worksheet[i, 13].Value.NumericValue);//预测累油SVM
                forcasttime.Add(worksheet[i, 14].Value.NumericValue);//预测时间SVM
                forcastiniq.Add(worksheet[i, 15].Value.NumericValue);//预测初期SVM
                forcast_ann_np.Add(worksheet[i, 16].Value.NumericValue);//预测累油ANN
                forcast_ann_time.Add(worksheet[i, 17].Value.NumericValue);//预测时间ANN
                forcast_ann_iniq.Add(worksheet[i, 18].Value.NumericValue);//预测初期ANN
                forcast_cf_np.Add(worksheet[i, 19].Value.NumericValue);//预测累油CF
                forcast_cf_time.Add(worksheet[i, 20].Value.NumericValue);//预测时间CF
                forcast_cf_iniq.Add(worksheet[i, 21].Value.NumericValue);//预测初期CF

                i++;
            }
        }
        public class Record
        {
            DateTime xaxis; double yaxis;
            public Record(DateTime xaxis, double yaxis)
            {
                this.xaxis = xaxis;
                this.yaxis = yaxis;
            }
            public DateTime Xaxis
            {
                get { return xaxis; }
                set { xaxis = value; }
            }

            public double Yaxis
            {
                get { return yaxis; }
                set { yaxis = value; }
            }
        }
        public ArrayList list(List<double> getData, List<DateTime> DataTime)
        {

            ArrayList list = new ArrayList();
            // Populate the list with records. 
            for (int i = 0; i < getData.Count; i++)
            {
                Record d1 = new Record(DataTime[i], getData[i]);
                list.Add(d1);
            }
            return list;
        }

        private void btnrefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            //List<double> getData = new List<double>();
            //List<DateTime> DataTime = new List<DateTime>();
            getData = new List<double>();
            DataTime = new List<DateTime>();
            wellname = new List<string>();
            StartDataTime = new List<DateTime>();
            ployfitinfo = new List<double[]>();
            importdata(getData, DataTime, wellname, StartDataTime);//输入数据
            forcastnp = new List<double>();//SVM预测
            forcasttime = new List<double>();//SVM预测
            forcastiniq = new List<double>();//SVM预测
            forcast_ann_np = new List<double>();//ANN预测
            forcast_ann_time = new List<double>();//ANN预测
            forcast_ann_iniq = new List<double>();//ANN预测
            forcast_cf_np = new List<double>();//CF预测
            forcast_cf_time = new List<double>();//CF预测
            forcast_cf_iniq = new List<double>();//CF预测
            importdata(forcastnp, forcasttime, forcastiniq, forcast_ann_np, forcast_ann_time, forcast_ann_iniq);
            cal_sm();//平滑数据
            draw_pre();//画图准备
            cal_cum();//计算累增油和有效期（天）和月份
        }
        private void cal_cum()
        {
            cumoil = new List<double>();//累增油
            cumtime = new List<double>();//有效期
            cummonth = new List<double>();//有效月份
            spreadsheetControl1.Visible = false;
            Worksheet worksheet = spreadsheetControl1.Document.Worksheets[0];

            int num_line = getData.Count;

            double time2 = 0;
            double np = 0;
            int i = 0;
            while (i < num_line - 1)
            {
                if ((wellname[i] == wellname[i + 1] & StartDataTime[i] == StartDataTime[i + 1]))
                {
                    int year = DataTime[i].Year;
                    int month = DataTime[i].Month;
                    int dayCount = DateTime.DaysInMonth(year, month);
                    time2 = time2 + dayCount;
                    np = np + getData[i];

                }
                else
                {

                    int year = DataTime[i].Year;
                    int month = DataTime[i].Month;
                    int dayCount = DateTime.DaysInMonth(year, month);
                    time2 = time2 + dayCount;
                    np = np + getData[i];
                    cumoil.Add(np);
                    cumtime.Add(time2);
                    time2 = 0;
                    np = 0;
                }
                i++;
            }
            {
                int year = DataTime[i].Year;
                int month = DataTime[i].Month;
                int dayCount = DateTime.DaysInMonth(year, month);
                time2 = time2 + dayCount;
                np = np + getData[i];
                cumoil.Add(np);
                cumtime.Add(time2);
            }
            for (i = 0; i < cumoil.Count; i++)
            {
                worksheet[(int)(i + 1), 4].Value = cumoil[i];
                worksheet[(int)(i + 1), 6].Value = cumtime[i];
                worksheet[(int)(i + 1), 8].Value = endrow[i] - startrow[i] + 1;

            }
            spreadsheetControl1.Visible = true;

        }

        private void draw(int start, int end, int rowindex)
        {


            List<double> drawdata = new List<double>();
            List<DateTime> drawDataTime = new List<DateTime>();

            for (int i = start; i <= end; i++)
            {
                drawdata.Add(getData[i]);
                drawDataTime.Add(DataTime[i]);
            }
            ArrayList listDataSource = list(drawdata, drawDataTime);      //画图数据源

            if (drawdata.Count == 0)
                return;
            // Bind the chart to the list. 
            // ChartControl myChart = myChart;
            //ChartControl myChart = new ChartControl();
            myChart = chartControl1;
            myChart.Series.Clear();
            myChart.Titles.Clear();

            myChart.CrosshairOptions.ShowCrosshairLabels = false;
            myChart.CrosshairOptions.ShowArgumentLine = false;
            myChart.ToolTipEnabled = DevExpress.Utils.DefaultBoolean.False;
            myChart.BorderOptions.Visibility = DevExpress.Utils.DefaultBoolean.False;


            // Create a series, and add it to the chart. 
            DevExpress.XtraCharts.Series series1 = new DevExpress.XtraCharts.Series("历史数据", ViewType.Point);
            myChart.Series.Add(series1);

            series1.DataSource = listDataSource;
            seriesnum = myChart.Series.Count;
            phase_num = 1;
            // Set the scale type for the series' arguments and values.
            series1.ArgumentScaleType = ScaleType.DateTime;
            series1.ValueScaleType = ScaleType.Numerical;


            // Adjust the series data members. 
            series1.ArgumentDataMember = "Xaxis";
            series1.ValueDataMembers.AddRange(new string[] { "Yaxis" });
            //myChart.DateTimeScaleOptions.MeasureUnit = Month
            // Access the view-type-specific options of the series. 
            ((PointSeriesView)series1.View).Color = Color.FromArgb(125, Color.Red);

            //series1.LegendTextPattern = "{A}";
            series1.LabelsVisibility = DevExpress.Utils.DefaultBoolean.False;
            //series1.Label.ResolveOverlappingMode = ResolveOverlappingMode.JustifyAllAroundPoint;

            if (smoothcheck.Checked == true)
            {
                drawdata = new List<double>();
                drawDataTime = new List<DateTime>();

                for (int i = start; i <= end; i++)
                {
                    drawdata.Add(smresult[i]);
                    drawDataTime.Add(DataTime[i]);
                }
                ArrayList listDataSource2 = list(drawdata, drawDataTime);      //画图数据源
                if (drawdata.Count == 0)
                    return;
                // Bind the chart to the list. 
                // ChartControl myChart = myChart;
                //ChartControl myChart = new ChartControl();



                myChart.CrosshairOptions.ShowCrosshairLabels = false;
                myChart.CrosshairOptions.ShowArgumentLine = false;
                myChart.ToolTipEnabled = DevExpress.Utils.DefaultBoolean.False;
                myChart.BorderOptions.Visibility = DevExpress.Utils.DefaultBoolean.False;


                // Create a series, and add it to the chart. 
                DevExpress.XtraCharts.Series series2 = new DevExpress.XtraCharts.Series("平滑点", ViewType.ScatterLine);
                myChart.Series.Add(series2);
                series2.DataSource = listDataSource2;
                series2.ArgumentScaleType = ScaleType.DateTime;
                series2.ValueScaleType = ScaleType.Numerical;


                // Adjust the series data members. 
                series2.ArgumentDataMember = "Xaxis";
                series2.ValueDataMembers.AddRange(new string[] { "Yaxis" });
                //myChart.DateTimeScaleOptions.MeasureUnit = Month
                // Access the view-type-specific options of the series. 
                ((PointSeriesView)series2.View).Color = Color.FromArgb(125, Color.Blue);

                //series1.LegendTextPattern = "{A}";
                series2.LabelsVisibility = DevExpress.Utils.DefaultBoolean.False;
            }


            if (showfit.Checked == true)
            {
                drawdata = new List<double>();
                drawDataTime = new List<DateTime>();

                for (int i = start; i <= end; i++)
                {
                    drawdata.Add(oil_qmonthly[i]);
                    drawDataTime.Add(DataTime[i]);
                }
                ArrayList listDataSource3 = list(drawdata, drawDataTime);      //画图数据源
                if (drawdata.Count == 0)
                    return;
                // Bind the chart to the list. 
                // ChartControl myChart = myChart;
                //ChartControl myChart = new ChartControl();



                myChart.CrosshairOptions.ShowCrosshairLabels = false;
                myChart.CrosshairOptions.ShowArgumentLine = false;
                myChart.ToolTipEnabled = DevExpress.Utils.DefaultBoolean.False;
                myChart.BorderOptions.Visibility = DevExpress.Utils.DefaultBoolean.False;


                // Create a series, and add it to the chart. 
                DevExpress.XtraCharts.Series series3 = new DevExpress.XtraCharts.Series("神经网络预测", ViewType.ScatterLine);
                myChart.Series.Add(series3);
                series3.DataSource = listDataSource3;
                series3.ArgumentScaleType = ScaleType.DateTime;
                series3.ValueScaleType = ScaleType.Numerical;


                // Adjust the series data members. 
                series3.ArgumentDataMember = "Xaxis";
                series3.ValueDataMembers.AddRange(new string[] { "Yaxis" });
                //myChart.DateTimeScaleOptions.MeasureUnit = Month
                // Access the view-type-specific options of the series. 
                ((PointSeriesView)series3.View).Color = Color.FromArgb(125, Color.Green);

                //series1.LegendTextPattern = "{A}";
                series3.LabelsVisibility = DevExpress.Utils.DefaultBoolean.False;
            }
           

           
      
            DevExpress.XtraCharts.Legend legend = myChart.Legend;

            // Display the chart control's legend.
            legend.Visible = true;

            // Define its margins and alignment relative to the diagram.
            legend.Margins.All = 8;


            // Define the layout of items within the legend.
            legend.Direction = LegendDirection.TopToBottom;
            legend.EquallySpacedItems = true;
            legend.HorizontalIndent = 8;
            legend.VerticalIndent = 8;
            legend.TextVisible = true;
            legend.TextOffset = 8;
            legend.MarkerVisible = true;
            legend.MarkerSize = new Size(10, 10);
            legend.Padding.All = 4;
            legend.AlignmentHorizontal = DevExpress.XtraCharts.LegendAlignmentHorizontal.Right;
            legend.AlignmentVertical = DevExpress.XtraCharts.LegendAlignmentVertical.Top;

            // Define the limits for the legend to occupy the chart's space.
            legend.MaxHorizontalPercentage = 50;
            legend.MaxVerticalPercentage = 50;

            // Customize the legend text properties.
            legend.Antialiasing = false;
            legend.Font = new Font("Arial", 9, FontStyle.Bold);
            legend.TextColor = Color.Black;

            XYDiagram diagram = (XYDiagram)myChart.Diagram;
            Diagram = (XYDiagram)myChart.Diagram;

            // Enable the diagram's scrolling.
            diagram.EnableAxisXScrolling = false;
            diagram.EnableAxisYScrolling = false;

            // Customize the appearance of the axes' grid lines.
            diagram.AxisX.GridLines.Visible = true;
            diagram.AxisX.GridLines.MinorVisible = false;

            diagram.AxisY.GridLines.Visible = true;
            diagram.AxisY.GridLines.MinorVisible = true;

            //diagram.AxisY.Range.SetInternalMinMaxValues(1, 12);
            // Customize the appearance of the X-axis title.
            diagram.AxisX.Title.Visible = true;
            diagram.AxisX.Title.Alignment = StringAlignment.Center;
            diagram.AxisX.Title.Text = "生产时间";
            diagram.AxisX.Title.TextColor = Color.Black;
            diagram.AxisX.Title.Antialiasing = true;
            diagram.AxisX.Title.Font = new Font("Microsoft YaHei", 10);

            // Customize the appearance of the Y-axis title.
            diagram.AxisY.Title.Visible = true;
            diagram.AxisY.Title.Alignment = StringAlignment.Center;
            diagram.AxisY.Title.Text = "增油量，m^3/mon";
            diagram.AxisY.Title.TextColor = Color.Black;
            diagram.AxisY.Title.Antialiasing = true;
            diagram.AxisY.Title.Font = new Font("Microsoft YaHei", 10);
            DevExpress.XtraCharts.ChartTitle chartTitle1 = new DevExpress.XtraCharts.ChartTitle();
            chartTitle1.Text = wellname[start] + "@" + StartDataTime[start].ToShortDateString();
            chartTitle1.Font = new Font("Times New Roman", 15);
            chartControl1.Titles.AddRange(new DevExpress.XtraCharts.ChartTitle[] { chartTitle1 });

        }
        private void smoothdata(List<double> smresult, List<double> getData, List<DateTime> DataTime, List<string> wellname, List<DateTime> StartDataTime)
        {
            int a = 1;

            List<double> cumoil2 = new List<double>();//累增油
            List<double> cumtime2 = new List<double>();//有效期

            List<double> initialqdaily2 = new List<double>();//初始增油量


            List<DateTime> StartTime2 = new List<DateTime>();//输入的时间

            importdata(cumoil2, cumtime2, initialqdaily2, StartTime2);
            List<double> smdata = new List<double>();
            initialq = new List<double>();
            int num_line = getData.Count;
            for (int i = 0; i < num_line - 1; i++)
            {
                if (wellname[i] == wellname[i + 1] & StartDataTime[i] == StartDataTime[i + 1])
                {
                    smdata.Add(getData[i]);
                }
                else
                {
                    //if (wellname[i] != wellname[i - 1] || StartDataTime[i] != StartDataTime[i - 1])
                    //{
                    //    smdata.Add(getData[i]);
                    //}

                    smdata.Add(getData[i]);
                    if (i == num_line - 2)
                    {
                        smdata.Add(getData[i + 1]);
                    }
                    int num_time = smdata.Count;
                    if (num_time >= 5)
                    {
                        int Nb = num_time;
                        double[] bvertical = new double[num_time];
                        bvertical[0] = (69 * smdata[0] + 4 * (smdata[1] + smdata[3]) - 6 * smdata[2] - smdata[4]) / 70;
                        bvertical[1] = (2 * (smdata[0] + smdata[4]) + 27 * smdata[1] + 12 * smdata[2] - 8 * smdata[3]) / 35;

                        for (int j = 2; j <= Nb - 3; j++)
                            bvertical[j] = (-3 * (smdata[j - 2] + smdata[j + 2]) + 12 * (smdata[j - 1] + smdata[j + 1]) + 17 * smdata[j]) / 35;

                        bvertical[Nb - 2] = (2 * (smdata[Nb - 1] + smdata[Nb - 5]) + 27 * smdata[Nb - 2] + 12 * smdata[Nb - 3] - 8 * smdata[Nb - 4]) / 35;
                        bvertical[Nb - 1] = (69 * smdata[Nb - 1] + 4 * (smdata[Nb - 2] + smdata[Nb - 4]) - 6 * smdata[Nb - 3] - smdata[Nb - 5]) / 70;

                        for (int bij = 0; bij < num_time; bij++)
                            smdata[bij] = bvertical[bij];

                    }
                    else if (num_time >= 3)
                    {
                        smdata[0] = (smdata[0] + smdata[1]) / 2;
                        smdata[num_time - 1] = (smdata[num_time - 2] + smdata[num_time - 1]) / 2;

                        for (int j = 1; j < num_time - 1; j++)
                            smdata[j] = (smdata[j - 1] + smdata[j] + smdata[j + 1]) / 3;

                    }

                    smresult.AddRange(smdata);
                    int avg_num = (int)Math.Round((smdata.Count - 1) * 0.9);
                    smdata.Sort();

                    initialq.Add(smdata[avg_num]);
                    if (initialq[initialq.Count - 1] * cumtime2[initialq.Count - 1] < cumoil2[initialq.Count - 1])
                        initialq[initialq.Count - 1] = smdata[smdata.Count - 1];
                    //List listCustomer2 = listCustomer.OrderByDescending(s => s.id).ToList(); 
                    smdata.Clear();
                }

            }
            {


                smdata.Add(getData[num_line - 1]);

                int num_time = smdata.Count;
                if (num_time >= 5)
                {
                    int Nb = num_time;
                    double[] bvertical = new double[num_time];
                    bvertical[0] = (69 * smdata[0] + 4 * (smdata[1] + smdata[3]) - 6 * smdata[2] - smdata[4]) / 70;
                    bvertical[1] = (2 * (smdata[0] + smdata[4]) + 27 * smdata[1] + 12 * smdata[2] - 8 * smdata[3]) / 35;

                    for (int j = 2; j <= Nb - 3; j++)
                        bvertical[j] = (-3 * (smdata[j - 2] + smdata[j + 2]) + 12 * (smdata[j - 1] + smdata[j + 1]) + 17 * smdata[j]) / 35;

                    bvertical[Nb - 2] = (2 * (smdata[Nb - 1] + smdata[Nb - 5]) + 27 * smdata[Nb - 2] + 12 * smdata[Nb - 3] - 8 * smdata[Nb - 4]) / 35;
                    bvertical[Nb - 1] = (69 * smdata[Nb - 1] + 4 * (smdata[Nb - 2] + smdata[Nb - 4]) - 6 * smdata[Nb - 3] - smdata[Nb - 5]) / 70;

                    for (int bij = 0; bij < num_time; bij++)
                        smdata[bij] = bvertical[bij];

                }
                else if (num_time >= 3)
                {
                    smdata[0] = (smdata[0] + smdata[1]) / 2;
                    smdata[num_time - 1] = (smdata[num_time - 2] + smdata[num_time - 1]) / 2;

                    for (int j = 1; j < num_time - 1; j++)
                        smdata[j] = (smdata[j - 1] + smdata[j] + smdata[j + 1]) / 3;

                }

                smresult.AddRange(smdata);
                int avg_num = (int)Math.Round((smdata.Count - 1) * 0.9);
                smdata.Sort();
                initialq.Add(smdata[avg_num]);
                smdata.Clear();

            }





        }
        private void chartControl1_CustomPaint(object sender, CustomPaintEventArgs e)
        {
            if (Diagram == null)
                return;
            Graphics g = e.Graphics;
            Brush solidcor = new SolidBrush(Color.FromArgb(205, Color.LightGoldenrodYellow));
            g.DrawRectangle(new Pen(Color.FromArgb(205, Color.Yellow)), selectionRectangle);
            g.FillRectangle(solidcor, selectionRectangle);
            //maxcoords = Diagram.DiagramToPoint((DateTime)Diagram.AxisX.WholeRange.MaxValue, (double)Diagram.AxisY.WholeRange.MaxValue);
            //mincoords = Diagram.DiagramToPoint((DateTime)Diagram.AxisX.WholeRange.MinValue, (double)Diagram.AxisY.WholeRange.MinValue);
            maxcoords = Diagram.DiagramToPoint((DateTime)Diagram.AxisX.WholeRange.MaxValue, (double)Diagram.AxisY.WholeRange.MaxValue + Diagram.AxisY.WholeRange.SideMarginsValue);
            mincoords = Diagram.DiagramToPoint((DateTime)Diagram.AxisX.WholeRange.MinValue, (double)Diagram.AxisY.WholeRange.MinValue);//- Diagram.AxisY.WholeRange.SideMarginsValue


            //maxcoords = Diagram.DiagramToPoint((double)Diagram.AxisX.VisualRange.MaxValue, (double)Diagram.AxisY.VisualRange.MaxValue);
            //mincoords = Diagram.DiagramToPoint((double)Diagram.AxisX.VisualRange.MinValue, (double)Diagram.AxisY.VisualRange.MinValue);
            //MessageBox.Show(Diagram.AxisX.VisualRange.MaxValue.ToString());    
        }
        private void chartControl1_MouseDown(object sender, MouseEventArgs e)
        {

            if (Diagram == null)
                return;
            if (e.Button != MouseButtons.Left)
                return;

            DiagramCoordinates coords = Diagram.PointToDiagram(e.Location);
            if (!coords.IsEmpty)
            {
                dragging = true;
                myChart.Capture = true;
                firstPoint = new Point(e.X, e.Y);

                if (e.X > maxcoords.Point.X)
                {
                    firstPoint.X = maxcoords.Point.X;
                }
                if (e.X < mincoords.Point.X)
                {
                    firstPoint.X = mincoords.Point.X;
                }
                if (e.Y < maxcoords.Point.Y)
                {
                    firstPoint.Y = maxcoords.Point.Y;
                }
                if (e.Y > mincoords.Point.Y)
                {
                    firstPoint.Y = mincoords.Point.Y;
                }
            }
        }
        private void chartControl1_MouseUp(object sender, MouseEventArgs e)
        {
            if (Diagram == null)
                return;
            dragging = false;
            myChart.Capture = false;
            secondPoint = new Point(e.X, e.Y);
            int minX = Math.Min(firstPoint.X, secondPoint.X);
            int minY = Math.Min(firstPoint.Y, secondPoint.Y);
            int maxX = Math.Max(firstPoint.X, secondPoint.X);
            int maxY = Math.Max(firstPoint.Y, secondPoint.Y);
            if (e.X > maxcoords.Point.X)
            {
                secondPoint.X = maxcoords.Point.X;
            }
            if (e.X < mincoords.Point.X)
            {
                secondPoint.X = mincoords.Point.X;
            }

            if (e.Y < maxcoords.Point.Y)
            {
                secondPoint.Y = maxcoords.Point.Y;
            }
            if (e.Y > mincoords.Point.Y)
            {
                secondPoint.Y = mincoords.Point.Y;
            }

            seccoords = Diagram.PointToDiagram(secondPoint);
            firstcoords = Diagram.PointToDiagram(firstPoint);
            selectionRectangle = Rectangle.Empty;
            selected_flag = true;
            polyfit_decline();
            selected_flag = false;
        }
        private void polyfit_decline()
        {

        }
        private void addseries(List<double> Result, List<DateTime> resultTime, string seriname, Color col, double[] parameter_result, int[] selectinfo, double correlative)
        {

            //importdata(Result, resultTime);//输入数据
            ArrayList listDataSource = list(Result, resultTime);      //画图数据源

            if (resultTime.Count == 0 | Result.Count == 0)
                return;
            // Bind the chart to the list. 
            myChart.CrosshairOptions.ShowCrosshairLabels = false;
            myChart.CrosshairOptions.ShowArgumentLine = false;
            myChart.ToolTipEnabled = DevExpress.Utils.DefaultBoolean.False;
            myChart.BorderOptions.Visibility = DevExpress.Utils.DefaultBoolean.False;
            //MessageBox.Show(myChart.Series.Count.ToString());
            int seiresi = seriesnum;
            int modelnum = 0;
            seiresi = seriesnum;



            while (myChart.Series.Count >= seriesnum + modelnum)
            {

            }
            // Create a series, and add it to the chart. 
            DevExpress.XtraCharts.Series series1 = new DevExpress.XtraCharts.Series(seriname, ViewType.Line);

            //添加下拉框的内容

            //添加拟合段信息
            double[] plinfo = new double[6];
            plinfo[0] = selectinfo[0];
            plinfo[1] = selectinfo[1];
            plinfo[2] = parameter_result[0];
            plinfo[3] = parameter_result[1];
            plinfo[4] = parameter_result[2];
            plinfo[5] = correlative;
            ployfitinfo.Add(plinfo);

            myChart.Series.Add(series1);
            series1.DataSource = listDataSource;

            // Set the scale type for the series' arguments and values.
            series1.ArgumentScaleType = ScaleType.DateTime;
            series1.ValueScaleType = ScaleType.Numerical;
            series1.CrosshairHighlightPoints = DevExpress.Utils.DefaultBoolean.False;

            // Adjust the series data members. 
            series1.ArgumentDataMember = "Xaxis";
            series1.ValueDataMembers.AddRange(new string[] { "Yaxis" });
            //myChart.DateTimeScaleOptions.MeasureUnit = Month
            // Access the view-type-specific options of the series. 

            ((PointSeriesView)series1.View).Color = col;// Color.FromArgb(125, Color.Blue);

            //series1.LegendTextPattern = "{A}";
            series1.LabelsVisibility = DevExpress.Utils.DefaultBoolean.False;

        }
        private void chartControl1_MouseMove(object sender, MouseEventArgs e)
        {

            if (Diagram == null)
                return;

            if (dragging && e.Button != MouseButtons.Right)
            {

                //获取新的右下角坐标   
                secondPoint = new Point(e.X, e.Y);
                if (e.X > maxcoords.Point.X)
                {
                    secondPoint.X = maxcoords.Point.X;
                }
                if (e.X < mincoords.Point.X)
                {
                    secondPoint.X = mincoords.Point.X;
                }
                if (e.Y < maxcoords.Point.Y)
                {
                    secondPoint.Y = maxcoords.Point.Y;
                }
                if (e.Y > mincoords.Point.Y)
                {
                    secondPoint.Y = mincoords.Point.Y;
                }
                int minX = Math.Min(firstPoint.X, secondPoint.X);
                int minY = maxcoords.Point.Y;
                int maxX = Math.Max(firstPoint.X, secondPoint.X);
                int maxY = mincoords.Point.Y;



                selectionRectangle = new Rectangle(minX, minY, maxX - minX, maxY - minY);
            }

        }

        private void btnNull_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            int seiresi = seriesnum;
            while (myChart.Series.Count > seriesnum)
            {
                myChart.Series.RemoveAt(seiresi);
            }
        }

        private void btnSelect_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            if (Diagram == null)
                return;
        }

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            seriesnum = myChart.Series.Count;
            phase_num = phase_num + 1;
        }

        private void btnmanul_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {


        }
        private void redrawseries(int seiresi, List<double> Result, List<DateTime> resultTime)
        {

            //importdata(Result, resultTime);//输入数据
            ArrayList listDataSource = list(Result, resultTime);      //画图数据源

            if (resultTime.Count == 0 | Result.Count == 0)
                return;
            // Bind the chart to the list. 

            //MessageBox.Show(myChart.Series.Count.ToString());

            DevExpress.XtraCharts.Series series1 = myChart.Series[seiresi];

            //myChart.Series.Add(series1);
            series1.DataSource = listDataSource;


        }
        private void cal_sm()
        {
            smresult = new List<double>();
            smoothdata(smresult, getData, DataTime, wellname, StartDataTime);//平滑数据
            if (smresult.Count == 0)
                return;
            spreadsheetControl1.Visible = false;
            Worksheet worksheet = spreadsheetControl1.Document.Worksheets[1];
            Worksheet worksheet0 = spreadsheetControl1.Document.Worksheets[0];
            for (int i = 0; i < smresult.Count; i++)
            {
                worksheet[(int)(i + 1), 8].Value = smresult[i];
            }
            for (int i = 0; i < initialq.Count; i++)
            {
                worksheet0[(int)(i + 1), 7].Value = initialq[i];
            }
            for (int i = 0; i < initialq.Count; i++)
            {
                worksheet0[(int)(i + 1), 11].Value = initialq[i] / 30;
            }
            worksheet.Columns["G"].NumberFormat = "0.00";
            spreadsheetControl1.Visible = true;
        }
        private void btncal_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            cal_sm();
        }
        private void writeresult(double startrow, List<DateTime> resultTime, List<double> result, double n, double Qi, double Di, double correlative)
        {
            spreadsheetControl1.Visible = false;


            Worksheet worksheet = spreadsheetControl1.Document.Worksheets.ActiveWorksheet;


            intialspreadsheet();
            for (int i = 2; i <= 6; i++)
                worksheet.Columns[i].ClearContents();
            // Perform the export.
            worksheet[0, 2].Value = "平滑处理后结果";
            for (int i = 0; i < getData.Count; i++)
            {
                worksheet[i + 1, 2].Value = getData[i];
            }
            worksheet[0, 3].Value = "预测时间";
            worksheet[0, 4].Value = "预测产油量";
            for (int i = 0; i < resultTime.Count; i++)
            {
                worksheet[(int)(i + startrow + 1), 3].Value = resultTime[i];
                worksheet[(int)(i + startrow + 1), 4].Value = result[i];
            }
            worksheet[0, 5].Value = "初始递减产量";
            worksheet[0, 6].Value = Qi;
            worksheet[0, 6].NumberFormat = "0.00";
            worksheet[1, 5].Value = "递减指数";
            worksheet[1, 6].Value = n;
            worksheet[1, 6].NumberFormat = "0.00";
            worksheet[2, 5].Value = "初始递减率";
            worksheet[2, 6].Value = Di;
            worksheet[2, 6].NumberFormat = "0.00";
            worksheet[3, 5].Value = "相关系数";
            worksheet[3, 6].Value = correlative;
            worksheet[3, 6].NumberFormat = "0.00";
            worksheet.Columns[0].NumberFormat = "YYYY-MM-DD";
            worksheet.Columns[3].NumberFormat = "YYYY-MM-DD";
            worksheet.Columns["B"].NumberFormat = "0.00";
            worksheet.Columns["C"].NumberFormat = "0.00";
            worksheet.Columns["E"].NumberFormat = "0.00";
            spreadsheetControl1.Visible = true;

        }

        /// <summary>
        /// 相关系数,或者拟合度，要求两个集合数量必须相同
        /// </summary>
        /// <param name="decdata">数组一</param>
        /// <param name="decresult">数组二</param>
        /// <returns></returns>
        private double correl(List<double> decdata, List<double> decresult)
        {
            int realdatanum = decdata.Count;

            //数组一
            double avg1 = average(decdata, realdatanum);
            //数组二
            double avg2 = average(decresult, realdatanum);

            double sumfenzi = 0;
            double sumfenmu_x = 0;
            double sumfenmu_y = 0;
            for (int i = 0; i < realdatanum; i++)
            {
                sumfenzi = sumfenzi + ((decdata[i] - avg1)) * ((decresult[i] - avg2));
                sumfenmu_x = sumfenmu_x + (decdata[i] - avg1) * (decdata[i] - avg1);
                sumfenmu_y = sumfenmu_y + (decresult[i] - avg2) * (decresult[i] - avg2);
            }
            double sumfenmu = System.Math.Pow(sumfenmu_x, 0.5) * System.Math.Pow(sumfenmu_y, 0.5);
            double cor = sumfenzi / sumfenmu;
            cor = cor * cor;
            return cor;
        }
        /// <summary>
        /// 求出数据平均值,并保留三位小数
        /// </summary>
        /// <param name="Valist">数据集合</param>
        /// <returns></returns>
        private double average(List<double> Valist, int realdatanum)
        {
            double sum = 0;
            for (int i = 0; i < realdatanum; i++)
            {
                sum = sum + Valist[i];
            }
            double revl = sum / realdatanum;
            return revl;
        }
        private void btnOutpic_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            ImageFormat format = ImageFormat.Bmp;
            //myChart.ExportToImage(fileName, format);

            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.RestoreDirectory = true;
            fileDialog.Title = "导出图片";
            fileDialog.Filter = "Bitmaps|*.BMP|JPEG Files|*.JPEG|Windows Metafile Format|*.WMF|PNG Files|*.PNG|Enhanced MetaFile|*.EMF";
            DialogResult dialogResult = fileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                if (System.IO.File.Exists(fileDialog.FileName))
                {
                    System.IO.File.Delete(fileDialog.FileName);
                }
                //获得文件路径
                string localFilePath = fileDialog.FileName.ToString();
                //去除文件后缀名
                string fileNameWithoutSuffix = localFilePath.Substring(0, localFilePath.LastIndexOf("."));
                //后缀名
                string aLastName = localFilePath.Substring(localFilePath.LastIndexOf(".") + 1, (localFilePath.Length - localFilePath.LastIndexOf(".") - 1));   //扩展名
                switch (aLastName)
                {
                    case "BMP": format = ImageFormat.Bmp; break;
                    case "WMF": format = ImageFormat.Wmf; break;
                    case "JPEG": format = ImageFormat.Jpeg; break;
                    case "PNG": format = ImageFormat.Png; break;
                    case "EMF": format = ImageFormat.Emf; break;
                }

                myChart.ExportToImage(localFilePath, format);
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btn_excel_output_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Worksheet worksheet = spreadsheetControl1.Document.Worksheets.ActiveWorksheet;
            IWorkbook workbook = spreadsheetControl1.Document;
            spreadsheetControl1.Visible = false;
            //Create a chart and specify its location.
            Chart chart = worksheet.Charts.Add(ChartType.ScatterSmooth);
            int topcell = 4;
            int leftcell = 5;
            chart.TopLeftCell = worksheet.Cells[topcell, leftcell];
            chart.BottomRightCell = worksheet.Cells[topcell + 20, leftcell + 7];

            ///画图数据原
            DevExpress.Spreadsheet.Range range1;
            DevExpress.Spreadsheet.Range range2;

            range1 = worksheet.Range.FromLTRB(0, 1, 0, getData.Count);
            range2 = worksheet.Range.FromLTRB(2, 1, 2, getData.Count);
            // Add chart series using worksheet ranges as the data sources.
            chart.Series.Add(worksheet["B1"], range1, range2);//原始序列

            // A rectangular range whose left column index is 0, top row index is 0, 
            // right column index is 3 and bottom row index is 2. This is the A1:D3 cell range.
            range1 = worksheet.Range.FromLTRB(3, 1, 3, getData.Count);
            range2 = worksheet.Range.FromLTRB(4, 1, 4, getData.Count);
            // Add chart series using worksheet ranges as the data sources.
            chart.Series.Add(worksheet["E1"], range1, range2);//结果序列



            DevExpress.Spreadsheet.Charts.Axis axis = chart.PrimaryAxes[0];
            axis.Scaling.AutoMax = true;
            //axis.Scaling.Max = worksheet.Cells[resulty.Count, 5].Value.NumericValue;
            axis.Scaling.AutoMin = false;
            axis.Scaling.Min = worksheet.Cells[1, 0].Value.NumericValue;
            axis.Title.SetValue("生产时间");
            axis.NumberFormat.FormatCode = "YYYY-MM-DD";
            chart.PrimaryAxes[1].Title.SetValue("产油量,t/mon");


            chart.Legend.Position = LegendPosition.Top;

            chart.Title.Visible = true;
            chart.Title.SetValue("递减曲线");
            chart.Title.Font.Size = 11;
            chart.Series[0].Marker.Symbol = MarkerStyle.Circle;
            chart.Series[0].Marker.Size = 5;
            //chart.Series[0].Outline.SetNoFill();
            chart.Series[0].ChangeType(ChartType.ScatterMarkers);

            chart.Series[1].Outline.SetSolidFill(Color.Red);
            chart.Series[0].Marker.Outline.SetSolidFill(Color.Orange);
            chart.Series[0].Marker.Fill.SetSolidFill(Color.FromArgb(125, Color.Orange));


            chart.PlotArea.Outline.SetSolidFill(Color.Black);

            DocumentFormat format = DocumentFormat.Xlsx;
            //myChart.ExportToImage(fileName, format);

            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.RestoreDirectory = true;
            fileDialog.Title = "导出结果图表";
            fileDialog.Filter = "EXCEL|*.xlsx|EXCEL 97-03|*.xls";
            DialogResult dialogResult = fileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                if (System.IO.File.Exists(fileDialog.FileName))
                {
                    System.IO.File.Delete(fileDialog.FileName);
                }

                //获得文件路径
                string localFilePath = fileDialog.FileName.ToString();
                //去除文件后缀名
                string fileNameWithoutSuffix = localFilePath.Substring(0, localFilePath.LastIndexOf("."));
                //后缀名
                string aLastName = localFilePath.Substring(localFilePath.LastIndexOf(".") + 1, (localFilePath.Length - localFilePath.LastIndexOf(".") - 1));   //扩展名
                switch (aLastName)
                {
                    case "xlsx": format = DocumentFormat.Xlsx; break;
                    case "xls": format = DocumentFormat.Xls; break;
                }
                workbook.SaveDocument(localFilePath, format);
                while (worksheet.Charts.Count > 0)
                { worksheet.Charts[0].Delete(); }
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            while (worksheet.Charts.Count > 0)
            { worksheet.Charts[0].Delete(); }
            spreadsheetControl1.Visible = true;
        }

        private void barCheckItem2_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void spreadsheetCommandBarButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Worksheet worksheet = spreadsheetControl1.Document.Worksheets.ActiveWorksheet;
            IWorkbook workbook = spreadsheetControl1.Document;
            DocumentFormat format = DocumentFormat.Xlsx;
            //myChart.ExportToImage(fileName, format);

            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.RestoreDirectory = true;
            fileDialog.Title = "导出结果图表";
            fileDialog.Filter = "EXCEL|*.xlsx|EXCEL 97-03|*.xls";
            DialogResult dialogResult = fileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                if (System.IO.File.Exists(fileDialog.FileName))
                {
                    System.IO.File.Delete(fileDialog.FileName);
                }

                //获得文件路径
                string localFilePath = fileDialog.FileName.ToString();
                //去除文件后缀名
                string fileNameWithoutSuffix = localFilePath.Substring(0, localFilePath.LastIndexOf("."));
                //后缀名
                string aLastName = localFilePath.Substring(localFilePath.LastIndexOf(".") + 1, (localFilePath.Length - localFilePath.LastIndexOf(".") - 1));   //扩展名
                switch (aLastName)
                {
                    case "xlsx": format = DocumentFormat.Xlsx; break;
                    case "xls": format = DocumentFormat.Xls; break;
                }
                workbook.SaveDocument(localFilePath, format);
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnhelp_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            System.Diagnostics.Process.Start(Application.StartupPath.ToString() + "\\help\\递减分析.pdf");
        }

        private void decMethodchecked_Click(object sender, EventArgs e)
        {

        }

        private void groupControl1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void draw_pre()
        {

            spreadsheetControl1.Visible = false;
            Worksheet worksheet = spreadsheetControl1.Document.Worksheets[0];
            startrow = new List<int>();
            endrow = new List<int>();
            bool flag_frist = true;
            int num_line = getData.Count;
            int j = 0;
            int jI = 0;
            for (int i = 0; i < num_line - 1; i++)
            {


                if ((wellname[i] == wellname[i + 1] & StartDataTime[i] == StartDataTime[i + 1]))
                {
                    if (flag_frist == true)
                    {
                        j = j + 1;
                        worksheet[(int)(j), 9].Value = i;
                        startrow.Add(i);
                        flag_frist = false;
                    }

                }
                else
                {
                    if (wellname[i] != wellname[i - 1] || StartDataTime[i] != StartDataTime[i - 1])
                    {
                        if (flag_frist == true)
                        {
                            j = j + 1;
                            worksheet[(int)(j), 9].Value = i;
                            startrow.Add(i);
                            flag_frist = false;
                        }

                    }

                    {
                        jI++;
                        endrow.Add(i);
                        worksheet[(int)(jI), 10].Value = i;
                        flag_frist = true;
                    }
                }

            }
            {
                jI++;
                endrow.Add(num_line - 1);
                worksheet[(int)(jI), 10].Value = num_line - 1;
                flag_frist = true;
            }
            spreadsheetControl1.Visible = true;
        }

        private void btndraw_pre_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            draw_pre();
        }

        private void btnfit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            double decline_n = 0;
            cumoil = new List<double>();
            cumtime = new List<double>();
            initialqdaily = new List<double>();
            StartTime0 = new List<DateTime>();
            List<double> oil_q = new List<double>();
            List<double> oil_DI = new List<double>();
            oil_qmonthly = new List<double>();
            importdata(cumoil, cumtime, initialqdaily, StartTime0);
            for (int welli = 0; welli < cumoil.Count; welli++)
            {
                DateTime currendate;
                DateTime nextdate;
                double efftime = cumtime[welli];
                double effoil = cumoil[welli];
                double iniq = initialqdaily[welli];
                double sum = 0;
                double inia = 0.01;
                double inib = 0.1;
                int loopi = 1;
                double Di = 0;
                while (loopi < 10 && Math.Abs(sum - effoil) / effoil > 0.3)
                {
                    loopi++;
                    oil_q = new List<double>();
                    Di = newton(efftime, effoil, iniq, decline_n, inia, inib);//调用newton函数，就是一个求解递减率（超越函数）的程序
                    //将递减年限，递减期的总产量以及稳产的最后一年的产量，传值进行求解

                    if (Di < 0)
                    {
                        //MessageBox.Show("递减率＜0,失败！");

                    }

                    if (Di > 1)
                    {
                        // MessageBox.Show("递减率>1,失败！");

                    }

                    for (int i = 0; i < efftime; i++) //递减阶段的产量（更准确的说是采油速度）
                    {
                        if (decline_n == 0)
                            oil_q.Add(iniq * System.Math.Exp(-Di * (i)));
                        //else if (decline_n == 1)
                        //    oil_q[i] = oil_q[(int)stable_time - 1] / (1 + Di * (i - stable_time + 1));//'只考虑调和递减
                        //else if (decline_n > 0 & decline_n < 1)
                        //    oil_q[i] = oil_q[(int)stable_time - 1] * System.Math.Pow((1 + Di * (i - stable_time + 1) * decline_n), (-1 / decline_n));//'只考虑调和递减

                    }
                    sum = 0;
                    foreach (double n in oil_q)
                    {
                        sum += n;
                    }

                    if ((sum - effoil) / effoil > 0.3)
                    {
                        if (inia < 0.1)
                            inia = inia * 10;
                        else
                            inia = inia + 0.05;

                        if (inib > 0.1)
                            inib = inib * 10;
                        else
                            inib = inib + 0.05;
                    }
                    else if ((sum - effoil) / effoil < -0.3)
                    {

                        inia = inia / 10;


                        if (inib > 0.05)
                            inib = inib - 0.05;
                        else if (inib > 0.01)
                            inib = inib - 0.01;
                    }
                }
                oil_DI.Add(Di);
                oil_qmonthly.AddRange(oil_q);
                //int ij = 0;
                //currendate = StartTime0[welli];
                //double monthoil = 0;
                //int year = StartTime0[welli].Year;
                //int month = StartTime0[welli].Month;
                //int dayCount = DateTime.DaysInMonth(year, month);
                //currendate = DateTime.Parse(currendate.ToString("yyyy-MM-01")).AddMonths(1);
                //while (ij < efftime)
                //{
                //    nextdate = DateTime.Parse(currendate.ToString("yyyy-MM-01")).AddMonths(1);
                //    System.TimeSpan t3 = nextdate - currendate;
                //    currendate = DateTime.Parse(currendate.ToString("yyyy-MM-01")).AddMonths(1);
                //    int days = (int)t3.TotalDays;

                //    int endday = ij + days;
                //    if (endday>oil_q.Count)
                //        endday = oil_q.Count;
                //    for (int ijk = ij; ijk < endday; ijk++)
                //    {
                //        monthoil = monthoil + oil_q[ijk];
                //    }
                //    oil_qmonthly.Add(monthoil);
                //        ij = ij + days;
                //        monthoil = 0;
                //}
            }
            Worksheet worksheet = spreadsheetControl1.Document.Worksheets[1];
            Worksheet worksheet0 = spreadsheetControl1.Document.Worksheets[0];
            spreadsheetControl1.Visible = false;
            for (int i = 0; i < oil_qmonthly.Count; i++)
            {
                worksheet[(int)(i + 1), 9].Value = oil_qmonthly[i];
            }
            for (int i = 0; i < oil_DI.Count; i++)
            {
                worksheet0[(int)(i + 1), 12].Value = oil_DI[i];
            }
            spreadsheetControl1.Visible = true;
        }

        private double newton(double decre_time, double qsum, double qi, double decline_n, double inia, double inib)
        {
            //'    Print stable_q
            //'    Print decre_time
            //' '主要用于求解超越方程
            //' '要调用程序gfun(I,x(i),qdecresum,q)
            //' 'x01,xo2 初始值，esp 给定精度
            //' '弦截法迭代公式：x(k+1)=x(k)-(x(k)-x(k-1))*f(x(k))/[f(x(k)-f(x(k-1))]]

            double[] Di = new double[10002];

            Di[1] = inia;
            Di[2] = inib;//'根据油田递减率∈（0，1），给定的初始值，考虑到海上油田的递减率
            int i = 2;
            for (i = 2; i < 10000; i++)
            {
                Di[i + 1] = Di[i] - gfun(decre_time, Di[i], qsum, qi, decline_n) * (Di[i] - Di[i - 1]) / (1E-27 + gfun(decre_time, Di[i], qsum, qi, decline_n) - gfun(decre_time, Di[i - 1], qsum, qi, decline_n));

                //'调用函数gfun(I,di(i),qdecresum,q)来构造exp（D0*t）函数
                //'弦截法迭代公式：x(k+1)=x(k)-(x(k)-x(k-1))*f(x(k))/[f(x(k)-f(x(k-1))]
                //'b(i) = Di(i + 1) - Di(i)
                if (System.Math.Abs(Di[i + 1] - Di[i]) < 0.000001)//'如果两次的di比较接近，那么说明求得解是正确的，停止迭代
                    break;

            }

            double newton2 = Di[i];
            return newton2;
        }

        private double gfun(double decre_time, double a, double qsum, double qi, double decline_n)
        {
            double f = 0;

            //'gfun(decre_time, Di(i), qdecresum, stable_q)
            for (int i = 0; i < decre_time; i++)
            {
                if (decline_n == 0)
                    f = f + System.Math.Exp(-a * i); //'定义符号表达式,只考虑指数递减
                else if (decline_n == 1)
                    f = f + 1 / (1 + i * a);//'只考虑调和递减
                else if (decline_n > 0 & decline_n < 1)
                    f = f + System.Math.Pow((1 / (1 + i * a * decline_n)), (1 / decline_n));//'只考虑双曲递减
            }



            double Y = qi * f;
            double gfun2 = qsum - Y;
            return gfun2;
            //'    Print #2, gfun
            //'    Close #2
        }

        private void autopic_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.RestoreDirectory = true;
            fileDialog.Title = "导出图片";

            DialogResult dialogResult = fileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                string localFilePath = fileDialog.FileName.ToString();
                //去除文件后缀名
                string fileNameWithoutSuffix = localFilePath.Substring(0, localFilePath.LastIndexOf("\\"));
                for (int i = 0; i < startrow.Count; i++)
                {
                    int str = startrow[i];
                    int end = endrow[i];

                    draw(str, end, i);

                    ImageFormat format = ImageFormat.Bmp;
                    //myChart.ExportToImage(fileName, format);

                    //获得文件路径

                    localFilePath = fileNameWithoutSuffix + "\\" + i.ToString() + ".bmp";//Application.StartupPath.ToString()


                    myChart.ExportToImage(localFilePath, format);

                }
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void spreadsheetControl1_Click(object sender, EventArgs e)
        {

        }




    }
}
