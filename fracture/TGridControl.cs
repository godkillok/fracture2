using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace fracture
{
    public partial class TGridControl : UserControl
    {
        public TGridControl()
        {
            InitializeComponent();
            
        }
         public void SetGridDataView(DataTable dt)
       {
           this.gridControl1.DataSource = dt;

       }
         public void gridViewoption()
         {
             this.gridView1.PopulateColumns();
             this.gridView1.OptionsView.ColumnAutoWidth = false;
             this.gridView1.ScrollStyle = DevExpress.XtraGrid.Views.Grid.ScrollStyleFlags.LiveHorzScroll;
             this.gridView1.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Always; // or ScrollVisibility.Auto doesn't work neither; or remove this line, doesn't work 
             this.gridView1.OptionsView.RowAutoHeight = true;
             this.gridView1.BestFitColumns(true);
             this.gridView1.OptionsCustomization.AllowColumnMoving = false;

             this.gridView1.OptionsSelection.MultiSelect = true;
             this.gridView1.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.RowSelect;
             gridView1.OptionsView.EnableAppearanceEvenRow = true;                    //是否启用偶数行外观
             gridView1.OptionsView.EnableAppearanceOddRow = true;                     //是否启用奇数行外观
            
             DevExpress.XtraGrid.Views.Grid.GridViewAppearances Appearance1 = new DevExpress.XtraGrid.Views.Grid.GridViewAppearances(gridView1);
             Appearance1.EvenRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(249)))), ((int)(((byte)(254)))));
             Appearance1.OddRow.BackColor = System.Drawing.Color.White;
             Appearance1.FocusedRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(211)))), ((int)(((byte)(128)))));
             Appearance1.HeaderPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(229)))), ((int)(((byte)(249)))));
       


             //奇数行
             gridView1.Appearance.EvenRow.BackColor = Appearance1.EvenRow.BackColor;
         
             //偶数行
             gridView1.Appearance.OddRow.BackColor = Appearance1.OddRow.BackColor;
        
             //选中行
             gridView1.Appearance.FocusedRow.BackColor = Appearance1.FocusedRow.BackColor;   //选中的行
             gridView1.Appearance.FocusedCell.BackColor = Appearance1.FocusedRow.BackColor;  //选中的单元格
      
             
          
             //列标题颜色
             gridView1.Appearance.HeaderPanel.BackColor = Appearance1.HeaderPanel.BackColor;
           
             gridView1.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center; //行标题样式设置为居中对齐
         
         }
    
        private void eXCELToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string localFilePath = FileOperate.saveopenfile();
            if (localFilePath != null)
            {
                FileOperate.ExportToExcel(gridView1, localFilePath);
            }
        }

  

        private void SelectAll_Click(object sender, EventArgs e)
        {
            this.gridView1.SelectAll();
        }

        private void Copy_Click(object sender, EventArgs e)
        {
            this.gridView1.OptionsBehavior.CopyToClipboardWithColumnHeaders = false;
            this.gridView1.CopyToClipboard();
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            this.gridView1.DeleteSelectedRows();
        }

        private void Insert_Click(object sender, EventArgs e)
        {
            string[] data = ClipboardData.Split('\n');
            if (data.Length < 1) return;
            foreach (string row in data)
            {
                AddRow(row);
            }

        }

        void AddRow(string data)
        {
            if (data == string.Empty) return;
            gridView1.AddNewRow();
            string[] rowData = data.Split(new char[] { '\r', '\x09' });
            int rowHandle = gridView1.GetRowHandle(gridView1.DataRowCount);

            for (int i = 0; i < rowData.Length; i++)
            {
                if (i >= gridView1.Columns.Count) break;
                if (gridView1.IsNewItemRow(rowHandle))
                {
                    gridView1.SetRowCellValue(rowHandle, gridView1.Columns[i], rowData[i]);
                }
            }
        }
        string ClipboardData
        {
            get
            {
                IDataObject iData = Clipboard.GetDataObject();
                if (iData == null) return "";

                if (iData.GetDataPresent(DataFormats.Text))
                    return (string)iData.GetData(DataFormats.Text);
                return "";
            }
            set { Clipboard.SetDataObject(value); }
        }

        private void gridControl1_Click(object sender, EventArgs e)
        {

        }
    }
}
