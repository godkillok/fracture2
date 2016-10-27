using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace fracture
{
    class FileOperate
    {
        public static void ExportToExcel(DevExpress.XtraGrid.Views.Base.BaseView bv,string fileName)
        {
            //this.gridControl1.ExportToXlsx(fileName);   
            try
            {
                //去除文件后缀名
                string fileNameWithoutSuffix = fileName.Substring(0, fileName.LastIndexOf("."));
                //后缀名
                string aLastName = fileName.Substring(fileName.LastIndexOf(".") + 1, (fileName.Length - fileName.LastIndexOf(".") - 1));   //扩展名
  
                if ((aLastName=="xls")  & (!string.IsNullOrEmpty(fileName)))
                {
                    bv.ExportToXls(fileName);
                    //ExportTo(bv, new DevExpress.XtraExport.ExportXlsProvider(fileName));
                }
                if ((aLastName == "xlsx") & !string.IsNullOrEmpty(fileName))
                {
                    bv.ExportToXlsx(fileName);
                   // ExportTo(bv, new DevExpress.XtraExport.ExportXlsProvider(fileName));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
        public static void ExportTo(DevExpress.XtraGrid.Views.Base.BaseView bv, DevExpress.XtraExport.IExportProvider provider)
        {
            Cursor currentCursor = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;

            DevExpress.XtraGrid.Export.BaseExportLink link = bv.CreateExportLink(provider);
            link.ExportTo(true);

            Cursor.Current = currentCursor;
        }
        public static void ImportTo(DevExpress.XtraGrid.Views.Base.BaseView bv, DevExpress.XtraExport.IExportProvider provider)
        {
            Cursor currentCursor = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;

            DevExpress.XtraGrid.Export.BaseExportLink link = bv.CreateExportLink(provider);
            link.ExportTo(true);

            Cursor.Current = currentCursor;
        }
        public static string saveopenfile ()
    {
         SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.RestoreDirectory = true;
            fileDialog.Title = "导出Excel";
            fileDialog.Filter = "Microsoft Excel Document|*.xlsx|Microsoft Excel|*.xls";
            DialogResult dialogResult = fileDialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                if (System.IO.File.Exists(fileDialog.FileName))
                {
                    System.IO.File.Delete(fileDialog.FileName);
                }
                string localFilePath = fileDialog.FileName.ToString();
                return localFilePath;
            }
            return null;
}

        public static string saveopenfile(string title,string filter)
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.RestoreDirectory = true;
            fileDialog.Title = title;
            fileDialog.Filter = filter;
            DialogResult dialogResult = fileDialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                if (System.IO.File.Exists(fileDialog.FileName))
                {
                    System.IO.File.Delete(fileDialog.FileName);
                }
                string localFilePath = fileDialog.FileName.ToString();
                return localFilePath;
            }
            return null;
        }

        public static string importopenfile(string title, string filter)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.RestoreDirectory = true;
            fileDialog.Title = title;
            fileDialog.Filter = filter;
            DialogResult dialogResult = fileDialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                if (System.IO.File.Exists(fileDialog.FileName))
                {
                    System.IO.File.Delete(fileDialog.FileName);
                }
                string localFilePath = fileDialog.FileName.ToString();
                return localFilePath;
            }
            return null;
        }

        public static void exportfilecharttemplete(DevExpress.XtraCharts.ChartControl chartControl1)
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.RestoreDirectory = true;
            fileDialog.Title = "图形模板";
            fileDialog.Filter = "模板文件|*.xml";
            DialogResult dialogResult = fileDialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                if (System.IO.File.Exists(fileDialog.FileName))
                {
                    System.IO.File.Delete(fileDialog.FileName);
                }
                string localFilePath = fileDialog.FileName.ToString();
                chartControl1.SaveToFile(localFilePath);
            }
  
        }
        public static void importfilecharttemplete(DevExpress.XtraCharts.ChartControl chartControl1)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.RestoreDirectory = true;
            fileDialog.Title = "图形模板";
            fileDialog.Filter = "模板文件|*.xml";
            DialogResult dialogResult = fileDialog.ShowDialog( );
            if (dialogResult == DialogResult.OK)
            {
                if (System.IO.File.Exists(fileDialog.FileName))

                { 
                //获得文件路径
                string localFilePath = fileDialog.FileName.ToString();
           
                chartControl1.LoadFromFile(localFilePath);
                }
            }
        
        
        }

        public static void savechart(DevExpress.XtraCharts.ChartControl chartControl1)
        {
            ImageFormat format = ImageFormat.Bmp;
            //myChart.ExportToImage(fileName, format);

            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.RestoreDirectory = true;
            fileDialog.Title = "导出图片";
            fileDialog.Filter = "Bitmaps|*.BMP|JPEG Files|*.JPEG|Windows Metafile Format|*.WMF|PNG Files|*.PNG|Enhanced MetaFile|*.EMF";
            DialogResult dialogResult = fileDialog.ShowDialog();
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
    }
}
