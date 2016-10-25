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
using DevExpress.Spreadsheet.Export;
using DevExpress.XtraSpreadsheet;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Services;
using DevExpress.XtraSpreadsheet.Menu;
using System.IO;
using DevExpress.XtraCharts;
using System.Drawing.Imaging;
using DevExpress.XtraEditors.Repository;
namespace fracture
{
    public partial class frmwellevl : Form
    {
        string localFilePath;
        List<Bitmap> srcImage;
        AnimatedGifEncoder gif;
        ArrayList listDataSource;
        List<double> resultlistfen;
        List<string> wellname;
        List<DateTime> DataTimesource;
        List<double> resultlistfendraw;
        List<string> wellnamedraw;
        List<DateTime> listdate;
        int playnum = 0;
        bool flag_gif = false;
        public frmwellevl(string filepath)
        {
            InitializeComponent();
            ribbonControl1.SelectedPage = ribbonControl1.PageCategories[0].Pages[0];

            localFilePath = filepath;

            IWorkbook workbook = spreadsheetControl1.Document;
            if (File.Exists(localFilePath))
            {
                using (FileStream stream = new FileStream(localFilePath, FileMode.Open))
                {
                    workbook.LoadDocument(stream, DocumentFormat.Xls);
                }
            }
            else
            {
                return;
            }
            workbook.Worksheets.ActiveWorksheet = workbook.Worksheets[1];
            //workbook.Worksheets[0].Visible = t;
        }
        private void initialsheet()
        {
            IWorkbook workbook = spreadsheetControl1.Document;
            workbook.Worksheets.ActiveWorksheet = workbook.Worksheets[1];
        }
        private void btncal_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            List<SortField> fields = new List<SortField>();
            IWorkbook workbook = spreadsheetControl1.Document;
            Worksheet worksheet0 = workbook.Worksheets[0];//井位信息
            Worksheet worksheet1 = workbook.Worksheets[1];//井完整性评价
            Worksheet worksheet2 = workbook.Worksheets[3];//权重
            // First sorting field. First column (offset = 0) will be sorted using ascending order.
            SortField sortField1 = new SortField();
            sortField1.ColumnOffset = 0;
            sortField1.Comparer = worksheet1.Comparers.Ascending;
            fields.Add(sortField1);

            // Second sorting field. Second column (offset = 1) will be sorted using ascending order.
            SortField sortField2 = new SortField();
            sortField2.ColumnOffset = 1;
            sortField2.Comparer = worksheet1.Comparers.Ascending;
            fields.Add(sortField2);

            // Sort the range by sorting fields.
            // A rectangular range whose left column index is 0, top row index is 0, 
            // right column index is 3 and bottom row index is 2. This is the A1:D3 cell range.
            int[] datanum = new int[2];
            datacount(datanum, worksheet1);
            DevExpress.Spreadsheet.Range range = worksheet1.Range.FromLTRB(0, 1, datanum[1], datanum[0]);
            worksheet1.Sort(range, fields);
            List<double> welllocationx = new List<double>();
            List<double> welllocationy = new List<double>();
            List<double[]> getDatasource = new List<double[]>();
            wellname = new List<string>();//井完整性的井名
            DataTimesource = new List<DateTime>();//井完整性的时间
            resultlistfen = new List<double>();//井完整性的结果
            resultlistfendraw = new List<double>();//用于画结果图的评价结果，主要是用时间筛选
            wellnamedraw = new List<string>();//用于画结果图的井名
            List<double> result = new List<double>();
            List<double> weightData = new List<double>();
            List<string> wellfullname = new List<string>();
            int countcol = importdata(getDatasource, DataTimesource, wellname, worksheet1);//井完整性评价
            importdata(weightData, worksheet2);//权重
            importdata(welllocationx, welllocationy, wellfullname, worksheet0);//井坐标
            List<double> topten = new List<double>();
            for (int i = 0; i < getDatasource.Count; i++)
            {
                List<double> tempdata = new List<double>();
                for (int j = 0; j < countcol; j++)
                {
                    tempdata.Add(getDatasource[i][j]);
                }
                tempdata.Sort();
                topten.Add(tempdata[(int)(tempdata.Count * 0.1)]);
            }
            for (int i = 0; i < getDatasource.Count; i++)
            {
                double temp = 0;
                double temp2 = 0;
                for (int j = 0; j < countcol; j++)
                {
                    temp = temp + getDatasource[i][j] * weightData[j];
                }

                if (temp < 6)
                { temp2 = 0; worksheet1.Cells[i + 1, countcol + 3].Value = "安全"; }
                else if (temp < 7)
                { temp2 = 1; worksheet1.Cells[i + 1, countcol + 3].Value = "较安全"; }
                else if (temp < 8)
                { temp2 = 2; worksheet1.Cells[i + 1, countcol + 3].Value = "中等"; }
                else if (temp < 9)
                {
                    temp2 = 3; worksheet1.Cells[i + 1, countcol + 3].Value = "较危险";
                    string ss = "";
                    for (int j = 0; j < countcol; j++)
                    {
                        if (getDatasource[i][j] > topten[j])
                        {
                            ss = "2";
                        }

                        
                    }
                }
                else
                { temp2 = 4; worksheet1.Cells[i + 1, countcol + 3].Value = "危险"; }
                resultlistfen.Add(temp2);
                worksheet1.Cells[i + 1, countcol + 2].Value = temp;
                //worksheet2.Visible = false;
            }
            worksheet1.Cells[0, countcol + 2].Value = "综合得分";
            worksheet1.Cells[0, countcol + 3].Value = "评价结果";


            listdate = DataTimesource.Distinct().ToList();

            for (int i = 0; i < listdate.Count; i++)
            {
                (timeselect.Edit as RepositoryItemComboBox).Items.Add(listdate[i].ToShortDateString());

            }
            timeselect.EditValue = (timeselect.Edit as RepositoryItemComboBox).Items[0];
            adddraw(wellfullname, welllocationx, welllocationy);
            adddraw2(listdate[0].ToShortDateString() + "井筒完整性评价结果图");
            //ComboBoxproperties.Items.AddRange
            for (int i = 0; i < DataTimesource.Count; i++)
            {
                if (DataTimesource[i] == listdate[0])
                {
                    resultlistfendraw.Add(resultlistfen[i]);// = new List<double>();//用于画结果图的评价结果，主要是用时间筛选
                    wellnamedraw.Add(wellname[i]);// = new List<string>();//用于画结果图的井名
                }
            }
        }
        public class Record
        {
            double xaxis, yaxis;
            string name;
            public Record(string name, double xaxis, double yaxis)
            {
                this.name = name;
                this.xaxis = xaxis;
                this.yaxis = yaxis;
            }
            public string Name
            {
                get { return name; }
                set { name = value; }
            }
            public double Xaxis
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
        public ArrayList list(List<string> wellfullname, List<double> welllocationx, List<double> welllocationy)
        {

            ArrayList list = new ArrayList();
            // Populate the list with records. 
            for (int i = 0; i < wellfullname.Count; i++)
            {
                double x = welllocationx[i];
                double y = welllocationy[i];
                Record d1 = new Record(wellfullname[i], x, y);
                list.Add(d1);
            }
            return list;
        }
        private void adddraw(List<string> wellfullname, List<double> welllocationx, List<double> welllocationy)
        {

            listDataSource = list(wellfullname, welllocationx, welllocationy);
            //画图数据源
            ChartControl myChart = chartControl1;
            myChart.Series.Clear();
            myChart.DataSource = listDataSource;
            myChart.CrosshairOptions.ShowCrosshairLabels = false;
            myChart.CrosshairOptions.ShowArgumentLine = false;
            myChart.ToolTipEnabled = DevExpress.Utils.DefaultBoolean.False;
            myChart.BorderOptions.Visibility = DevExpress.Utils.DefaultBoolean.False;


            // Create a series, and add it to the chart. 
            DevExpress.XtraCharts.Series series1 = new DevExpress.XtraCharts.Series("生产数据", ViewType.Point);
            myChart.Series.Add(series1);
            //series1.Label = "A";
            // Set the scale type for the series' arguments and values.
            series1.ArgumentScaleType = ScaleType.Numerical;
            series1.ValueScaleType = ScaleType.Numerical;

            //series1.Label.Shadow = true;
            // Adjust the series data members. 
            series1.ArgumentDataMember = "Xaxis";
            //series1.LegendText = "Name";
            series1.ValueDataMembers.AddRange(new string[] { "Yaxis" });
            //myChart.DateTimeScaleOptions.MeasureUnit = Month
            // Access the view-type-specific options of the series. 
            //((PointSeriesView)series1.View).PointMarkerOptions.BorderColor= Color.Navy;
            ((PointSeriesView)series1.View).Color = Color.FromArgb(125, Color.Black);
            //series1.LegendTextPattern = "{A}";
            series1.Label.LineVisibility = DevExpress.Utils.DefaultBoolean.False;
            series1.LabelsVisibility = DevExpress.Utils.DefaultBoolean.True;
            series1.Label.ResolveOverlappingMode = ResolveOverlappingMode.JustifyAllAroundPoint;
            //series1.Label.ResolveOverlappingMode = ResolveOverlappingMode.JustifyAllAroundPoint;
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
            legend.AlignmentHorizontal = DevExpress.XtraCharts.LegendAlignmentHorizontal.Left;
            legend.AlignmentVertical = DevExpress.XtraCharts.LegendAlignmentVertical.Bottom;

            // Define the limits for the legend to occupy the chart's space.
            legend.MaxHorizontalPercentage = 50;
            legend.MaxVerticalPercentage = 50;

            // Customize the legend text properties.
            legend.Antialiasing = false;
            legend.Font = new Font("Arial", 9, FontStyle.Bold);
            legend.TextColor = Color.Black;

            XYDiagram diagram = (XYDiagram)myChart.Diagram;
            diagram.EnableAxisXScrolling = true;
            diagram.EnableAxisYScrolling = true;
            diagram.EnableAxisXZooming = true;
            diagram.EnableAxisYZooming = true;

            // Customize the appearance of the axes' grid lines.
            diagram.AxisX.GridLines.Visible = true;
            diagram.AxisX.GridLines.MinorVisible = false;

            diagram.AxisY.GridLines.Visible = true;
            diagram.AxisY.GridLines.MinorVisible = true;

            //diagram.AxisY.Range.SetInternalMinMaxValues(1, 12);
            // Customize the appearance of the X-axis title.
            diagram.AxisX.Title.Visible = true;
            diagram.AxisX.Title.Alignment = StringAlignment.Center;
            diagram.AxisX.Title.Text = "X轴";
            diagram.AxisX.Title.TextColor = Color.Black;
            diagram.AxisX.Title.Antialiasing = true;
            diagram.AxisX.Title.Font = new Font("Microsoft YaHei", 10);

            // Customize the appearance of the Y-axis title.
            diagram.AxisY.Title.Visible = true;
            diagram.AxisY.Title.Alignment = StringAlignment.Center;
            diagram.AxisY.Title.Text = "Y轴";
            diagram.AxisY.Title.TextColor = Color.Black;
            diagram.AxisY.Title.Antialiasing = true;
            diagram.AxisY.Title.Font = new Font("Microsoft YaHei", 10);

            diagram.AxisY.WholeRange.AutoSideMargins = true;

        }
        private void chartControl1_CustomDrawSeriesPoint(object sender, CustomDrawSeriesPointEventArgs e)
        {
            string[] wellname = new string[listDataSource.Count];
            for (int i = 0; i < listDataSource.Count; i++)
            {
                Record lis = listDataSource[i] as Record;
                PointDrawOptions drawOptions = e.SeriesDrawOptions as PointDrawOptions;
                wellname[i] = lis.Name;
                int index = chartControl1.Series[0].Points.IndexOf(e.SeriesPoint);
                if (index == i)
                {
                    e.LabelText = wellname[i];
                    if (wellnamedraw != null)
                        if (wellnamedraw.Count > 0)
                        {
                            for (int j = 0; j < wellnamedraw.Count; j++)
                            {
                                if (wellnamedraw[j] == wellname[i])
                                {
                                    int a = (int)resultlistfendraw[j];
                                    switch (a)
                                    {
                                        case 0: e.SeriesDrawOptions.Color = Color.FromArgb(125, Color.Green); break;
                                        case 1: e.SeriesDrawOptions.Color = Color.FromArgb(125, Color.Blue); break;
                                        case 2: e.SeriesDrawOptions.Color = Color.FromArgb(125, Color.Yellow); break;
                                        case 3: e.SeriesDrawOptions.Color = Color.FromArgb(125, Color.Orange); break;
                                        case 4: e.SeriesDrawOptions.Color = Color.FromArgb(125, Color.Red); break;
                                    }
                                }

                            }

                        }
                }
            }

        }
        private void importdata(List<double> welllocationx, List<double> welllocationy, List<string> wellfullname, Worksheet worksheet)
        {
            int i = 1;

            while (worksheet[i, 0].Value.IsEmpty == false)
            {
                double[] xy = new double[2];
                wellfullname.Add(worksheet[i, 0].Value.ToString());
                xy[0] = worksheet[i, 1].Value.NumericValue;
                xy[1] = worksheet[i, 2].Value.NumericValue;
                welllocationx.Add(xy[1]);
                welllocationy.Add(xy[0]);
                i++;
            }

        }
        private void importdata(List<double> weightData, Worksheet worksheet)
        {
            int i = 1;
            while (worksheet[i, 0].Value.IsEmpty == false)
            {
                i++;
            }

            int weightrow = i - 1;
            i = 1;
            if (worksheet.Cells[weightrow, 0].Value.ToString() == "指标权重")
            {
                while (worksheet[weightrow, i].Value.IsEmpty == false)
                {
                    weightData.Add(worksheet[weightrow, i].Value.NumericValue);
                    i++;
                }

            }
        }

        private int importdata(List<double[]> getData, List<DateTime> DataTime, List<string> wellname, Worksheet worksheet)
        {


            // Perform the export.
            int i;
            i = 1;
            int j = 0;
            while (worksheet[0, j].Value.IsEmpty == false & worksheet[0, j].Value.ToString() != "综合得分" & worksheet[0, j].Value.ToString() != "评价结果")
            {
                j++;
            }
            int countcol = j;
            while (worksheet[i, 0].Value.IsEmpty == false)
            {
                double[] tempdata = new double[countcol - 2];
                wellname.Add(worksheet[i, 0].Value.ToString());
                DataTime.Add(worksheet[i, 1].Value.DateTimeValue);
                for (int k = 0; k < countcol - 2; k++)
                {
                    tempdata[k] = worksheet[i, k + 2].Value.NumericValue;//累油
                }
                getData.Add(tempdata);
                i++;
            }
            return countcol - 2;


        }
        private void datacount(int[] datacount, Worksheet worksheet)
        {


            // Perform the export.
            int i;
            i = 0;
            int j = 0;
            while (worksheet[0, j].Value.IsEmpty == false)
            {
                j++;
            }
            datacount[1] = j;
            while (worksheet[i, 0].Value.IsEmpty == false)
            {

                i++;
            }
            datacount[0] = i;

        }

        private void barEditItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void btnfresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            IWorkbook workbook = spreadsheetControl1.Document;
            Worksheet worksheet0 = workbook.Worksheets[0];//井完整性评价
            List<double> welllocationx = new List<double>();
            List<double> welllocationy = new List<double>();
            List<string> wellfullname = new List<string>();
            importdata(welllocationx, welllocationy, wellfullname, worksheet0);//井坐标
            adddraw(wellfullname, welllocationx, welllocationy);
            adddraw2("井位图");
        }

        private void btnplay_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (listdate == null)
                return;
            if (listdate.Count == 0)
                return;
            playnum = 0;
            timer1.Enabled = true;
            timer1.Interval = 1000;
        }
        private void adddraw2(string txt)
        {

            ChartTitle chartTitle1 = new ChartTitle();
            //画图数据源
            ChartControl myChart = chartControl1;
            chartTitle1.Text = txt;
            //DevExpress.XtraCharts.Series series1 = new DevExpress.XtraCharts.Series("生产数据", ViewType.Point);
            myChart.Titles.Clear();
            chartTitle1.Font = new Font("微软雅黑", 12, FontStyle.Bold);
            myChart.Titles.Add(chartTitle1);

            //myChart.Titles.Font = new Font("Arial", 9, FontStyle.Bold);

        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            if (playnum < listdate.Count)
            {
                for (int i = 0; i < DataTimesource.Count; i++)
                {
                    if (DataTimesource[i] == listdate[playnum])
                    {
                        resultlistfendraw.Add(resultlistfen[i]);// = new List<double>();//用于画结果图的评价结果，主要是用时间筛选
                        wellnamedraw.Add(wellname[i]);// = new List<string>();//用于画结果图的井名
                    }
                }
                adddraw2(listdate[playnum].ToShortDateString() + "井筒完整性评价结果图");
                if (flag_gif == true)
                {

                    Bitmap curBitmap = null;

                    ImageFormat format = ImageFormat.Bmp;

                    using (MemoryStream s = new MemoryStream())
                    {
                        chartControl1.ExportToImage(s, format);
                        curBitmap = (Bitmap)System.Drawing.Image.FromStream(s);
                    }
                    srcImage.Add(curBitmap);

                }
                playnum = playnum + 1;
            }
            if (flag_gif == true & playnum == listdate.Count)
            {
                for (int i = 0; i < listdate.Count; i++)
                {
                    gif.AddFrame(srcImage[i]);
                }

                gif.Finish();

                flag_gif = false;
                MessageBox.Show("Done!");
            }


        }

        private void timeselect_EditValueChanged(object sender, EventArgs e)
        {
            if (listdate == null)
                return;
            if (listdate.Count == 0)
                return;
            string k = timeselect.EditValue.ToString();
            for (int i = 0; i < DataTimesource.Count; i++)
            {
                if (DataTimesource[i].ToShortDateString() == k)
                {
                    resultlistfendraw.Add(resultlistfen[i]);// = new List<double>();//用于画结果图的评价结果，主要是用时间筛选
                    wellnamedraw.Add(wellname[i]);// = new List<string>();//用于画结果图的井名
                }
            }
            //playnum = k;
            adddraw2(timeselect.EditValue.ToString() + "井筒完整性评价结果图");//listdate[playnum].ToShortDateString()
        }

        private void btnstart_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (listdate == null)
                return;
            if (listdate.Count == 0)
                return;
            int k = 0;
            for (int i = 0; i < DataTimesource.Count; i++)
            {
                if (DataTimesource[i] == listdate[k])
                {
                    resultlistfendraw.Add(resultlistfen[i]);// = new List<double>();//用于画结果图的评价结果，主要是用时间筛选
                    wellnamedraw.Add(wellname[i]);// = new List<string>();//用于画结果图的井名
                }
            }
            playnum = k;
            adddraw2(listdate[playnum].ToShortDateString() + "井筒完整性评价结果图");
        }

        private void btnend_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (listdate == null)
                return;
            if (listdate.Count == 0)
                return;
            int k = listdate.Count - 1;
            for (int i = 0; i < DataTimesource.Count; i++)
            {
                if (DataTimesource[i] == listdate[k])
                {
                    resultlistfendraw.Add(resultlistfen[i]);// = new List<double>();//用于画结果图的评价结果，主要是用时间筛选
                    wellnamedraw.Add(wellname[i]);// = new List<string>();//用于画结果图的井名
                }
            }
            playnum = k;
            adddraw2(listdate[playnum].ToShortDateString() + "井筒完整性评价结果图");

        }

        private void btnsavegif_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (listdate == null)
                return;
            if (listdate.Count == 0)
                return;
            string fname = "";
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            //saveFileDialog.InitialDirectory = "c:\\";//注意这里写路径时要用c:\\而不是c:\
            saveFileDialog.Filter = "图形文件|*.gif|所有文件|*.*";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (System.IO.File.Exists(saveFileDialog.FileName))
                {
                    System.IO.File.Delete(saveFileDialog.FileName);
                }
                fname = saveFileDialog.FileName;
                if (System.IO.File.Exists(saveFileDialog.FileName))
                {
                    MessageBox.Show("文件存在,且正被使用", "文件另存为", MessageBoxButtons.OK);
                    return;
                }

                save_gif(fname);

            }


        }

        public void save_gif(string outputPath)
        {
            playnum = 0;
            timer1.Enabled = true;
            timer1.Interval = 1000;

            flag_gif = true;
            gif = new AnimatedGifEncoder();
            gif.Start(outputPath);
            gif.SetDelay(1000);
            gif.SetRepeat(0);
            timer1.Start();
            srcImage = new List<Bitmap>();
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ImageFormat format = ImageFormat.Bmp;
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

                chartControl1.ExportToImage(localFilePath, format);
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void spreadsheetCommandBarButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            IWorkbook workbook = spreadsheetControl1.Document;
            DocumentFormat format = DocumentFormat.Xls;

            workbook.SaveDocument(localFilePath, format);
        }

        private void frmwellevl_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (localFilePath == "")
                return;
            IWorkbook workbook = spreadsheetControl1.Document;
            DocumentFormat format = DocumentFormat.Xls;
            spreadsheetControl1.CloseCellEditor(DevExpress.XtraSpreadsheet.CellEditorEnterValueMode.ActiveCell);
            workbook.SaveDocument(localFilePath, format);
        }

        private void timeselect_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

      

     






    }
}
