using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList.ViewInfo;
using System.Data.OleDb;
using System.Data.SqlClient;
using DevExpress.XtraVerticalGrid.Rows;
using Global;
namespace fracture
{
    public partial class Form2 : Form
    {
        private List<int> lstCheckedOfficeID;
        private string DBlocation;
        private OleDbConnection dbconn; //数据库连接
        private OleDbDataAdapter da;

        public Form2()
        {
            InitializeComponent();
                  InitTreeView(treeView);
           treeView.CustomDrawNodeCell += treeView_CustomDrawNodeCell;
            //treeView.AfterCollapse += treeView_AfterCollapse;
           treeView.MouseDoubleClick += treeView_MouseDoubleClick;
            treeView.AfterCheckNode += treeView_AfterCheckNode;
            treeView.BeforeCheckNode += treeView_BeforeCheckNode;
          // AddAllNodes(true);
      AddDataBaseNodes(true);
           //treeView.ExpandAll();
      SetImageIndex(treeView, null, 1, 0);
        }
            
        public static void InitTreeView(DevExpress.XtraTreeList.TreeList treeView) {
           
            treeView.OptionsView.ShowColumns =false;
            treeView.OptionsView.ShowIndicator = false;
            treeView.OptionsView.ShowVertLines = false;
            treeView.OptionsView.ShowHorzLines = false;
            treeView.OptionsBehavior.Editable = false;
            treeView.OptionsView.ShowCheckBoxes = true;
            treeView.OptionsBehavior.EnableFiltering = true;
            treeView.OptionsView.ShowAutoFilterRow = false;
            treeView.OptionsFilter.FilterMode = FilterMode.Extended;  

            //treeView.OptionsBehavior.DragNodes = true;
            treeView.OptionsBehavior.AllowIndeterminateCheckState = true;  
           // treeView.OptionsSelection.EnableAppearanceFocusedCell = false;
            treeView.OptionsFind.HighlightFindResults = true;
            //treeView.ShowCloseButton = ceShowCloseButton.Checked;
            //TreeList.OptionsFind.ShowClearButton = ceShowClearButton.Checked;
            //TreeList.OptionsFind.ShowFindButton = ceShowFindButton.Checked;
            treeView.ShowFindPanel();
            treeView.OptionsFind.ShowCloseButton = false;

            treeView.OptionsFind.ShowFindButton = false;
            treeView.OptionsFind.ShowClearButton = false;

        }
        void treeView_CustomDrawNodeCell(object sender, CustomDrawNodeCellEventArgs e) {
            if(e.Node.Id == 1) 
                e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
        }

  
        private void treeView_AfterCheckNode(object sender, DevExpress.XtraTreeList.NodeEventArgs e)
        {
            SetCheckedChildNodes(e.Node, e.Node.CheckState);
            SetCheckedParentNodes(e.Node, e.Node.CheckState);

        }

        private void treeView_BeforeCheckNode(object sender, DevExpress.XtraTreeList.CheckNodeEventArgs e)
        {
            e.State = (e.PrevState == CheckState.Checked ? CheckState.Unchecked : CheckState.Checked);
        }

        /// <summary>
        /// 设置子节点的状态
        /// </summary>
        /// <param name="node"></param>
        /// <param name="check"></param>
        private void SetCheckedChildNodes(DevExpress.XtraTreeList.Nodes.TreeListNode node, CheckState check)
        {
            for (int i = 0; i < node.Nodes.Count; i++)
            {
                node.Nodes[i].CheckState = check;
                SetCheckedChildNodes(node.Nodes[i], check);
            }
        }

        /// <summary>
        /// 设置父节点的状态
        /// </summary>
        /// <param name="node"></param>
        /// <param name="check"></param>
        private void SetCheckedParentNodes(DevExpress.XtraTreeList.Nodes.TreeListNode node, CheckState check)
        {
            if (node.ParentNode != null)
            {
                bool b = false;
                CheckState state;
                for (int i = 0; i < node.ParentNode.Nodes.Count; i++)
                {
                    state = (CheckState)node.ParentNode.Nodes[i].CheckState;
                    if (!check.Equals(state))
                    {
                        b = !b;
                        break;
                    }
                }
                node.ParentNode.CheckState = b ? CheckState.Indeterminate : check;
                SetCheckedParentNodes(node.ParentNode, check);
            }
        }
        void AddDataBaseNodes(bool showall)
        {
            DataTable dt = null; 
          //  string DabaBasePath="provider=microsoft.jet.oledb.4.0; Data Source=" + Application.StartupPath + "\\Database.mdb";
           


            string sSql = "SELECT T_DM_UNIT_CUR_INFOR.DM_UNIT_ID AS ParentID, T_WELL_INFOR.WELL_ID as KeyID,T_WELL_INFOR.WELL_NAME as Name FROM T_WELL_INFOR ,T_DM_UNIT_CUR_INFOR where T_WELL_INFOR.DM_UNIT_ID=T_DM_UNIT_CUR_INFOR.DM_UNIT_ID " +
"UNION SELECT PARENT_DM_UNIT_ID AS ParentID,DM_UNIT_ID as KeyID,DM_UNIT_NAME as Name FROM T_DM_UNIT_CUR_INFOR";

            try
            {

                dt = OleDbHelper.getTable(sSql,  Globalname.DabaBasePath);
                if (dt == null || dt.Rows.Count == 0)
                {
                    double a = 1;
                }
            }
            catch (Exception ex)
            {
                //Common.DisplayMsg(this.Text, ex.Message.ToString());
            }

            treeView.Nodes.Clear();
            treeView.DataSource = dt;
            treeView.ParentFieldName = "ParentID";
     
            treeView.KeyFieldName = "KeyID";
     treeView.Columns["Name"].Caption = "通讯录";

          

            this.treeView.Nodes[0].Expanded = true; // 只显示1级目录

         
        }
     
        /// <summary> 
        /// 设置TreeList显示的图标 
        /// </summary> 
        /// <param name="tl">TreeList组件</param> 
        /// <param name="node">当前结点，从根结构递归时此值必须=null</param> 
        /// <param name="nodeIndex">根结点图标(无子结点)</param> 
        /// <param name="parentIndex">有子结点的图标</param> 
        public static void SetImageIndex(TreeList tl, TreeListNode node, int nodeIndex, int parentIndex)
        {
            if (node == null)
            {
                foreach (TreeListNode N in tl.Nodes)
                    SetImageIndex(tl, N, nodeIndex, parentIndex);
            }
            else
            {
                if (node.HasChildren || node.ParentNode == null)
                {
                    //node.SelectImageIndex = parentIndex; 
                    node.StateImageIndex = parentIndex;
                    node.ImageIndex = parentIndex;
                }
                else
                {
                    //node.SelectImageIndex = nodeIndex; 
                    node.StateImageIndex = nodeIndex;
                    node.ImageIndex = nodeIndex;
                }

                foreach (TreeListNode N in node.Nodes)
                {
                    SetImageIndex(tl, N, nodeIndex, parentIndex);
                }
            }
        }

 
        public event EventHandler TreeViewItemClick;
        void treeView_MouseDoubleClick(object sender, MouseEventArgs e) {
            if(TreeViewItemClick != null)
                TreeViewItemClick(sender, EventArgs.Empty);
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
         
             lstCheckedOfficeID = new List<int>();

             if (treeView.Nodes.Count > 0)
             {
                 foreach (TreeListNode root in treeView.Nodes)
                 {
                     GetCheckedID(root);
                 }
             }


             string idStr = string.Empty;
             foreach (int id in lstCheckedOfficeID)
             {
                 idStr += id + " ";
             }
            
             }
   
         private void GetCheckedID(TreeListNode parentNode)
         {
             if (parentNode.Nodes.Count == 0)
             {
                 return;//递归终止www.2cto.com
             }


             foreach (TreeListNode node in parentNode.Nodes)
             {
                 if (node.CheckState == CheckState.Checked)
                 {
                     DataRowView drv = treeView.GetDataRecordByNode(node) as DataRowView;//关键代码
                     if (drv != null)
                     {
                         int GroupID= (int)drv["GroupID"];
                         lstCheckedOfficeID.Add(GroupID);
                     }
                 }
             }
         }
     
    }
}
