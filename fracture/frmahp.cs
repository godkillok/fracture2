using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using DevExpress.Spreadsheet;
using DevExpress.Spreadsheet.Export;
using DevExpress.XtraSpreadsheet;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Services;
using DevExpress.XtraSpreadsheet.Menu;
namespace fracture
{
    public partial class frmahp : Form
    {
        string localFilePath;
        public frmahp(string filepath)
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
            workbook.Worksheets.ActiveWorksheet = workbook.Worksheets[3];
          //  initialsheet();

        }
        private void initialsheet()
        {
            IWorkbook workbook = spreadsheetControl1.Document;
            Worksheet worksheet1;
            Worksheet worksheet2;
            var sheets = workbook.Worksheets;

            
            if (workbook.Worksheets.Count < 3)
            {
                worksheet1 = workbook.Worksheets.Add("指标权重");

                //workbook.Worksheets[0].Visible = false;
                //workbook.Worksheets[1].Visible = false;

               // spreadsheetControl1.Options.TabSelector.Visibility = DevExpress.XtraSpreadsheet.SpreadsheetElementVisibility.Hidden;
                worksheet2 = workbook.Worksheets[1];
                int j = 0;
                List<string> para_name = new List<string>();
                while (worksheet2[0, j + 2].Value.IsEmpty == false & worksheet2[0, j + 2].Value != "评价结果" & worksheet2[0, j+2].Value.ToString() != "综合得分")
                {
                    para_name.Add(worksheet2[0, j + 2].Value.ToString());
                    j++;
                }
                int countcol = j;

                for (int i = 0; i < para_name.Count; i++)
                {
                    worksheet1.Cells[0, i + 1].Value = para_name[i];
                    worksheet1.Cells[i + 1, 0].Value = para_name[i];
                }
                worksheet1.Cells[para_name.Count + 1, 0].Value = "指标权重";
                DevExpress.Spreadsheet.Range range = worksheet1.Range.FromLTRB(0, 0, para_name.Count + 1, 0);
                range.Fill.BackgroundColor = Color.FromArgb(217, 217, 217);
                for (int i = 0; i < para_name.Count + 1; i++)
                {
                    range = worksheet1.Range.FromLTRB(i, i, i, para_name.Count + 1);
                    range.Fill.BackgroundColor = Color.FromArgb(217, 217, 217);
                }
                worksheet1.Range["A1:Z1"].ColumnWidthInCharacters = 12;
                worksheet1.Range["A1:A500"].RowHeight = 80;
            }
            else
            {
                worksheet1 = workbook.Worksheets[2];
                workbook.Worksheets.ActiveWorksheet = workbook.Worksheets[2];
            }
        }
        private void btncal_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Worksheet worksheet = spreadsheetControl1.Document.Worksheets[3];
            //workbook.Worksheets[2];
            //worksheet.Name = "井基础信息";
            //spreadsheetintial_basicinfo(worksheet, workbook);
            //worksheet = workbook.Worksheets.Add("井筒完整性");0
            //spreadsheetintial_wellinfo(worksheet, workbook);1
            //worksheet = workbook.Worksheets.Add("安全等级设置");2
            //spreadsheetintial_safeinfo(worksheet, workbook);
            //worksheet = workbook.Worksheets.Add("指标权重");3
            //spreadsheetintial_weight(worksheet, workbook);
            //worksheet = workbook.Worksheets.Add("地面风险");4
            //spreadsheetintial_ground(worksheet, workbook);
            //worksheet = workbook.Worksheets.Add("储层风险");5
            List<double[]> getData = new List<double[]>();
            int para_num = importdata(getData);
            double[,] a_b = new double[para_num, para_num];//{ { 1, 0.333, 2 }, { 3, 1, 5 }, { 0.5, 0.2, 1 } }层次分析法判断矩阵是
            for (int i = 0; i < para_num; i++)
            {
                for (int j = 0; j < para_num; j++)
                {
                    if (j > i)
                    {

                        a_b[i, j] = getData[i][j];
                    }
                    if (i == j)
                        a_b[i, j] = 1;
                }
            }
            for (int i = 0; i < para_num; i++)
            {
                for (int j = 0; j < para_num; j++)
                {
                    if (j < i)
                    {
                        if (a_b[j, i] != 0)
                            a_b[i, j] = 1 / a_b[j, i];
                        else
                            a_b[i, j] = 0;
                    }
                }
            }

            double[] wvaule;//最后的权重向量
            wvaule = ahpsysytem(a_b);// "评判结果向量元素

            if (wvaule == null)
            {
                MessageBox.Show("未通过一致性检验，请检查判断矩阵！");
                return;
            }
            //worksheet.Visible = false;
            for (int i = 1; i < para_num + 1; i++)
                for (int j = 1; j < para_num + 1; j++)
                {
                    worksheet.Cells[i, j].Value = a_b[i-1, j-1];
                }
            for (int i = 1; i < para_num + 1; i++)
            {
                worksheet.Cells[para_num + 1, i].Value = wvaule[i - 1];
            }
            //worksheet.Visible = true;
            IWorkbook workbook = spreadsheetControl1.Document;
            DocumentFormat format = DocumentFormat.Xls;
            workbook.SaveDocument(localFilePath, format);
        }
        private int importdata(List<double[]> getData)
        {
            Worksheet worksheet = spreadsheetControl1.Document.Worksheets.ActiveWorksheet;

            // Perform the export.
            int i;
            i = 1;
            int j = 1;
            while (worksheet[0, j].Value.IsEmpty == false)
            {
                j++;
            }
            int countcol = j - 1;
            while (worksheet[i, 0].Value.IsEmpty == false && worksheet[i, 0].Value.ToString() != "指标权重")
            {
                double[] tempdata = new double[countcol];
                for (int k = 0; k < countcol; k++)
                {
                    tempdata[k] = worksheet[i, k + 1].Value.NumericValue;//累油
                }
                getData.Add(tempdata);
                i++;
            }
            return countcol;

        }
        public double[] ahpsysytem(double[,] a_b)
        {
            double[] ri = new double[14] { 0, 0, 0.58, 0.89, 1.12, 1.26, 1.36, 1.41, 1.46, 1.49, 1.52, 1.54, 1.56, 1.58 };
            double[] wvaule;
            double lamudamax;
            lamuda2(a_b, out wvaule, out lamudamax);
            int N;
            N = a_b.GetUpperBound(0) + 1;
            double cindex = (lamudamax - N) / (N - 1);
            int NN = ri.GetUpperBound(0) + 1;
            if (NN < N)
            { N = NN; }
            double crindex = cindex / ri[N - 1];

            if ((crindex < 0.1) && (cindex < 0.1))
            {
            }
            else
            {
                wvaule = null;
            }
            return wvaule;

        }
        private void lamuda2(double[,] a_b, out double[] wvaule, out double lamudamax)
        {//求积法
            lamudamax = 0;
            int M, N, i, j;
            N = a_b.GetUpperBound(0) + 1; M = a_b.GetUpperBound(1) + 1;
            double[] rowsum = new double[N];
            wvaule = new double[N];
            double[] rwmatirx = new double[N];
            //1）计算判断矩阵A每行元素乘积的n次方根；
            for (i = 0; i < N; i++)
            {
                rowsum[i] = 1;
                for (j = 0; j < M; j++)
                {
                    rowsum[i] = rowsum[i] * a_b[i, j];
                }
                wvaule[i] = Math.Pow(rowsum[i], (1.0 / N));
            }
            //2)将wvaule 归一化，得到 ；W=（w1，w2，…wn ）T即为A的特征向量的近似值；
            double sumrow = 0;
            for (i = 0; i < N; i++)
            {
                sumrow = wvaule[i] + sumrow;
            }
            for (i = 0; i < N; i++)
            {
                wvaule[i] = wvaule[i] / sumrow;
            }

            //3）求特征向量W对应的最大特征值：
            for (i = 0; i < N; i++)
            {
                double summatirx = 0;
                for (j = 0; j < N; j++)
                {
                    summatirx = summatirx + a_b[i, j] * wvaule[j];
                }
                rwmatirx[i] = summatirx;
            }
            for (i = 0; i < N; i++)
            {
                lamudamax = lamudamax + rwmatirx[i] / wvaule[i];
            }
            lamudamax = lamudamax / N;
        }

        private void spreadsheetCommandBarButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            IWorkbook workbook = spreadsheetControl1.Document;
            DocumentFormat format = DocumentFormat.Xls;
           
            workbook.SaveDocument(localFilePath, format);
        }

        private void frmahp_FormClosing(object sender, FormClosingEventArgs e)
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
