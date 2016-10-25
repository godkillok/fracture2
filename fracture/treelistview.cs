using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList.Nodes.Operations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Global;
namespace fracture
{
    class treelistview
    {

        public static void InitTreeView(DevExpress.XtraTreeList.TreeList treeView, bool box = false)
        {

            treeView.OptionsView.ShowColumns = false;
            treeView.OptionsView.ShowIndicator = false;
            treeView.OptionsView.ShowVertLines = false;
            treeView.OptionsView.ShowHorzLines = false;
            treeView.OptionsBehavior.Editable = false;
            treeView.OptionsView.ShowCheckBoxes = box;
            if (box)
            {
                treeView.OptionsBehavior.AllowRecursiveNodeChecking = true;
            }
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
        public static void SetImageIndex(DevExpress.Utils.ImageCollection imageCollection1, TreeList treeList1, TreeListNode node, int nodeIndex, int parentIndex)
        {
            treeList1.StateImageList = imageCollection1;

            SetImageIndex(treeList1, null, 1, 0);
        }


        /// <summary> 
        /// 设置TreeList显示的图标 
        /// </summary> 
        /// <param name="tl">TreeList组件</param> 
        /// <param name="node">当前结点，从根结构递归时此值必须=null</param> 
        /// <param name="nodeIndex">根结点图标(无子结点)</param> 
        /// <param name="parentIndex">有子结点的图标</param> 
        /// 
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

        public static void AddWellNodes(DevExpress.XtraTreeList.TreeList treeView, string sSql="")
        {
            DataTable dt = null;
           
          //  string DabaBasePath = "provider=microsoft.jet.oledb.4.0; Data Source=" + Application.StartupPath + "\\Database.mdb";
            if (sSql=="") 
            sSql = "SELECT T_DM_UNIT_CUR_INFOR.DM_UNIT_ID AS ParentID, T_WELL_INFOR.WELL_ID as WELLID,T_WELL_INFOR.WELL_NAME as Name, T_WELL_INFOR.WELL_CC as wellCode FROM T_WELL_INFOR ,T_DM_UNIT_CUR_INFOR where T_WELL_INFOR.DM_UNIT_ID=T_DM_UNIT_CUR_INFOR.DM_UNIT_ID " +
"UNION SELECT PARENT_DM_UNIT_ID AS ParentID,DM_UNIT_ID as KeyID,DM_UNIT_NAME as Name , DM_UNIT_NAME as wellCode FROM T_DM_UNIT_CUR_INFOR";

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
            if (dt == null)
                return;
            treeView.DataSource = dt;
            treeView.ParentFieldName = "ParentID";

            treeView.KeyFieldName = "WELLID";
            treeView.Columns["Name"].Caption = "通讯录";
            treeView.Columns["wellCode"].Visible = false;


            treeView.Nodes[0].Expanded = true; // 只显示1级目录


        }

        public static void AddWellStimuNodes(DevExpress.XtraTreeList.TreeList treeView)
        {
            DataTable dt = null;
            //string DabaBasePath = "provider=microsoft.jet.oledb.4.0; Data Source=" + Application.StartupPath + "\\Database.mdb";

           

            //            string sSql = "SELECT T_DM_UNIT_CUR_INFOR.DM_UNIT_ID AS ParentID, T_WELL_INFOR.WELL_ID as WELLID,T_WELL_INFOR.WELL_NAME as Name, T_WELL_INFOR.WELL_CC as wellCode FROM T_WELL_INFOR ,T_DM_UNIT_CUR_INFOR where T_WELL_INFOR.DM_UNIT_ID=T_DM_UNIT_CUR_INFOR.DM_UNIT_ID " +
            //"UNION SELECT PARENT_DM_UNIT_ID AS ParentID,DM_UNIT_ID as KeyID,DM_UNIT_NAME as Name , DM_UNIT_NAME as wellCode FROM T_DM_UNIT_CUR_INFOR";
            string sSql = @"SELECT T_DM_UNIT_CUR_INFOR.DM_UNIT_ID AS ParentID, T_WELL_INFOR.WELL_ID as WELLID,T_WELL_INFOR.WELL_NAME as Name, T_WELL_INFOR.WELL_CC as wellCode FROM T_WELL_INFOR ,T_DM_UNIT_CUR_INFOR 
where T_WELL_INFOR.DM_UNIT_ID=T_DM_UNIT_CUR_INFOR.DM_UNIT_ID
UNION 
SELECT PARENT_DM_UNIT_ID AS ParentID,DM_UNIT_ID as KeyID,DM_UNIT_NAME as Name , DM_UNIT_NAME as wellCode FROM T_DM_UNIT_CUR_INFOR
UNION
SELECT T2.WELL_ID, T2.WELL_ID&t2.stimu_code & t2.end_date as stim,t2.stimu_code & t2.end_date as NAME, T2.end_date as wellCode FROM t_well_stimu_rec T2";
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
            if (dt == null)
                return;
            treeView.DataSource = dt;
            treeView.ParentFieldName = "ParentID";
            treeView.KeyFieldName = "WELLID";
            treeView.Columns["Name"].Caption = "通讯录";
            treeView.Columns["wellCode"].Visible = false;


            treeView.Nodes[0].Expanded = true; // 只显示1级目录


        }
    }

    class GetCheckedNodesOperation : TreeListOperation
    {
        public List<TreeListNode> CheckedNodes = new List<TreeListNode>();
        public GetCheckedNodesOperation() : base() { }
        public override void Execute(TreeListNode node)
        {
            if (node.CheckState != CheckState.Unchecked)
                CheckedNodes.Add(node);
        }
    }
}
