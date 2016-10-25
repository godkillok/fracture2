using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using DevExpress.Spreadsheet;
using DevExpress.Spreadsheet.Export;
using DevExpress.XtraSpreadsheet;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Services;
using DevExpress.XtraSpreadsheet.Menu;
using System.Data;
using System.Linq;
using System.Text;
using System.Collections;
using DevExpress.XtraCharts;
using DevExpress.XtraBars.Ribbon;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Diagnostics;
using DevExpress.Spreadsheet.Charts;
using DevExpress.Spreadsheet.Drawings;

namespace fracture
{
    public partial class MathMethod : Form
    {
        List<double[]> getDatasource;//记录下原始的数据
        List<double> parameter_result;//记录下拟合的参数
        List<DateTime> DataTimesource;//记录下原始的时间
        List<string> wellname;//记录井名

        string localFilePath;
        public MathMethod(string filepath)
        {
            InitializeComponent();
            localFilePath = filepath;
            IWorkbook workbook = spreadsheetControl1.Document;
            if (File.Exists(localFilePath))
            {
                using (FileStream stream = new FileStream(localFilePath, FileMode.Open))
                {
                    workbook.LoadDocument(stream, DocumentFormat.Xls);
                    workbook.Worksheets.ActiveWorksheet = workbook.Worksheets[1];
                }
            }
            else
            {
                return;
            }
            //workbook.Worksheets[0].Visible = false;

            //spreadsheetControl1.Options.TabSelector.Visibility = DevExpress.XtraSpreadsheet.SpreadsheetElementVisibility.Hidden;

        }



        private int importdata(List<double[]> getData, List<DateTime> DataTime, List<string> wellname)
        {
            Worksheet worksheet = spreadsheetControl1.Document.Worksheets.ActiveWorksheet;

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
        private void datacount(int[] datacount)
        {
            Worksheet worksheet = spreadsheetControl1.Document.Worksheets.ActiveWorksheet;

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
        private void btncal_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Worksheet worksheet = spreadsheetControl1.Document.Worksheets.ActiveWorksheet;
            //'预测的对象类型
            // Create sorting fields.
            List<SortField> fields = new List<SortField>();

            // First sorting field. First column (offset = 0) will be sorted using ascending order.
            SortField sortField1 = new SortField();
            sortField1.ColumnOffset = 0;
            sortField1.Comparer = worksheet.Comparers.Ascending;
            fields.Add(sortField1);

            // Second sorting field. Second column (offset = 1) will be sorted using ascending order.
            SortField sortField2 = new SortField();
            sortField2.ColumnOffset = 1;
            sortField2.Comparer = worksheet.Comparers.Ascending;
            fields.Add(sortField2);

            // Sort the range by sorting fields.
            // A rectangular range whose left column index is 0, top row index is 0, 
            // right column index is 3 and bottom row index is 2. This is the A1:D3 cell range.
        int [] datanum=new int[2];
            datacount(datanum);
            DevExpress.Spreadsheet.Range range = worksheet.Range.FromLTRB(0, 1, datanum[1], datanum[0]);
            worksheet.Sort(range, fields);

            double fluid_type;
            int math_predic_num = int.Parse(predic_num.Text);
            //List<double> result = new List<double>();
            parameter_result = new List<double>();
            getDatasource = new List<double[]>();
            wellname = new List<string>();
            DataTimesource = new List<DateTime>();
            List<double[]> resultlist = new List<double[]>();
               int countcol=importdata(getDatasource, DataTimesource, wellname);

            List<string> listwellname;
            ;
            listwellname = wellname.Distinct().ToList();

            if (getDatasource.Count == 0)
                return;
            for (int k = 0; k < listwellname.Count; k++)
            {
                List<double[]> perwelldata = new List<double[]>();
                List<DateTime> perwelldatetime = new List<DateTime>();
                for (int i = 0; i < getDatasource.Count; i++)
                {
                    if (wellname[i] == listwellname[k])
                    {
                        perwelldata.Add(getDatasource[i]);
                        perwelldatetime.Add(DataTimesource[i]);
                    }
                }
                int inputnum = getDatasource.Count;


                fluid_type = 0;
                predict_np_time_method(perwelldata, math_predic_num, countcol, resultlist, fluid_type);
            }

            DateTime lastone = DataTimesource[DataTimesource.Count - 1];
            List<DateTime>  resultTime = new List<DateTime>();

            for (int i = 1; i <= int.Parse(predic_num.Text); i++)
            {
                if (DataTimesource[1].Year - DataTimesource[0].Year == 0)
                {
                    int span = DataTimesource[1].Month - DataTimesource[0].Month;
                    resultTime.Add(lastone.AddMonths(i * span));
                }
                else
                {
                    int span = DataTimesource[1].Year - DataTimesource[0].Year;
                    resultTime.Add(lastone.AddYears(i * span));
                }

            }

            for (int k = 0; k < listwellname.Count; k++)
            {
                for (int i=0;i<math_predic_num;i++)
                {
                    worksheet.Cells[wellname.Count + math_predic_num * k + 1 + i, 0].Value = listwellname[k];
                    worksheet.Cells[wellname.Count + math_predic_num * k + 1 + i, 1].Value = resultTime[i];
                    for (int j = 0; j < countcol; j++)
                    {
                        worksheet.Cells[wellname.Count + math_predic_num * k + 1 + i, 2 + j].Value = resultlist[j + countcol * k][i];
                    
                    }
                }

            }
            range = worksheet.Range.FromLTRB(0, 1, datanum[1], datanum[0] + math_predic_num * listwellname.Count);
            worksheet.Sort(range, fields);

        }

        private void predict_np_time_method(List<double[]> perwelldata, int math_predic_num, int datacol, List<double[]> resultlist, double fluid_type)
        {
            int i, datanum;
            datanum = perwelldata.Count;

            double[] GMData = new double[datanum];
            algorithm alg = new algorithm();
            int LoopCount = int.Parse(txtiters.Text);
            List<double> parameter_result = new List<double>();
            if (model.SelectedIndex == 0)
            {
                //usher
                for (int k = 0; k < datacol; k++)
                {
                    for (i = 0; i < datanum; i++)
                        GMData[i] = perwelldata[i][k];
                    double[,] result = new double[datanum + math_predic_num, 2];

                    int Dimension = 5;
                    double[,] PsoScope = new double[Dimension, 2];
                    PsoScope[0, 0] = double.Parse(textEdit0.Text);//'下界
                    PsoScope[0, 1] = double.Parse(textEdit1.Text);//'上界
                    PsoScope[1, 0] = double.Parse(textEdit2.Text);//'下界
                    PsoScope[1, 1] = double.Parse(textEdit3.Text);//'上界
                    PsoScope[2, 0] = double.Parse(textEdit4.Text);//'下界
                    PsoScope[2, 1] = double.Parse(textEdit5.Text);//'上界
                    PsoScope[3, 0] = double.Parse(textEdit6.Text);//'下界
                    PsoScope[3, 1] = double.Parse(textEdit7.Text);//'上界
                    PsoScope[4, 0] = GMData[GMData.GetLength(0) - 1] * 1.1;
                    PsoScope[4, 1] = GMData[GMData.GetLength(0) - 1] * double.Parse(txtnp.Text);
                    alg.Usher(math_predic_num, LoopCount, fluid_type, PsoScope, GMData, result, parameter_result);
                    double[] resulty = new double[datanum + math_predic_num];
                    for (i = 0; i < math_predic_num; i++)
                    {
                        resulty[i] = result[i + datanum, 1];
                    }
                    resultlist.Add(resulty);
                }
            }
            else if (model.SelectedIndex == 1)
            {
                //gomperz
                for (int k = 0; k < datacol; k++)
                {
                    for (i = 0; i < datanum; i++)
                        GMData[i] = perwelldata[i][k];
                    double[,] result = new double[datanum + math_predic_num, 2];

                    alg.Compertz(math_predic_num, GMData, result, parameter_result);
                    double[] resulty = new double[datanum + math_predic_num];
                    for (i = 0; i < math_predic_num; i++)
                    {
                        resulty[i] = result[i + datanum, 1];
                    }
                    resultlist.Add(resulty);
                }
            }

            else if (model.SelectedIndex == 2)
            {
                //GM11

                for (int k = 0; k < datacol; k++)
                {
                    for (i = 0; i < datanum; i++)
                        GMData[i] = perwelldata[i][k];
                    double[,] result = new double[datanum + math_predic_num, 2];
                    alg.GM1_1_Predict(math_predic_num, GMData, result, parameter_result);
                    double[] resulty = new double[datanum + math_predic_num];
                    for (i = 0; i < math_predic_num; i++)
                    {
                        resulty[i] = result[i + datanum, 1];
                    }
                    resultlist.Add(resulty);
                }
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

        private void btnrefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //getDatasource.Clear;

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

        private void btnoutpic_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

            }
        }

        private void model_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (model.SelectedIndex == 0)//usher模型
            {
                usher_set.Visible = true;
            }
            else
                usher_set.Visible = false;
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //System.Diagnostics.Process.Start(Application.StartupPath.ToString() + "\\help\\数学模型.pdf");
        }

        private void spreadsheetCommandBarButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            IWorkbook workbook = spreadsheetControl1.Document;
            DocumentFormat format = DocumentFormat.Xls;
            workbook.SaveDocument(localFilePath, format);
        }

        private void MathMethod_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (localFilePath == "")
                return;
            IWorkbook workbook = spreadsheetControl1.Document;
            DocumentFormat format = DocumentFormat.Xls;
            spreadsheetControl1.CloseCellEditor(DevExpress.XtraSpreadsheet.CellEditorEnterValueMode.ActiveCell);
            workbook.SaveDocument(localFilePath, format);
        }

    }
}
