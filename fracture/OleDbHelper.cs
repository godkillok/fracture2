using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Aspose;
using Aspose.Cells;
using fracture.Properties;
namespace fracture
{
    public class datatypedic
    {
        private string datatypeint;
        public string DataTypeint
        {
            get { return datatypeint; }
            set { datatypeint = value; }
        }

        private string datatypestring;
        public string DataTypestring
        {
            get { return datatypestring; }
            set { datatypestring = value; }
        }


    }
    /// <summary>
    /// OleDbHelper类封装对Access数据库的添加、删除、修改和选择等操作
    /// </summary>
    //
    public class OleDbHelper
    {
        protected static OleDbConnection conn = new OleDbConnection();
        protected static OleDbCommand comm = new OleDbCommand();

        /// <summary>
        /// 打开数据库
        /// </summary>
        private static void openConnection(string DabaBasePath)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.ConnectionString = DabaBasePath;
                comm.Connection = conn;
                try
                {
                    conn.Open();
                }
                catch (Exception e)
                { throw new Exception(e.Message); }

            }

        }

        /// <summary>
        /// 根据sql语句获取表信息
        /// </summary>
        /// <param name="sqlstr">sql语句</param>
        /// <returns></returns>
        public static DataTable getTable(string sqlstr, string DabaBasePath = "")
        {
            if (DabaBasePath == "")
                DabaBasePath = "provider=microsoft.jet.oledb.4.0; Data Source=" + Application.StartupPath + "\\case\\demoproject\\project\\Database.mdb";
            DataTable dt = new DataTable();
            OleDbDataAdapter da = new OleDbDataAdapter();
            try
            {
                openConnection(DabaBasePath);
                comm.CommandType = CommandType.Text;
                comm.CommandText = sqlstr;
                da.SelectCommand = comm;
                da.Fill(dt);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                closeConnection();
            }
            return dt;
        }

        /// <summary>
        /// 关闭数据库
        /// </summary>
        private static void closeConnection()
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
                conn.Dispose();
                comm.Dispose();
            }
        }

        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="sqlstr"></param>
        public static void excuteSql(string sqlstr, string DabaBasePath)
        {
            try
            {
                openConnection(DabaBasePath);
                comm.CommandType = CommandType.Text;
                comm.CommandText = sqlstr.ToString();
                comm.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            { closeConnection(); }
        }

        /// <summary>
        /// 返回Mdb数据库中所有表表名
        /// </summary>
        public static string[] GetShemaTableName(string database_path)
        {
            try
            {
                //获取数据表
                OleDbConnection conn = new OleDbConnection();
                conn.ConnectionString = database_path;
                conn.Open();
                DataTable shemaTable = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                int n = shemaTable.Rows.Count;
                string[] strTable = new string[n];
                int m = shemaTable.Columns.IndexOf("TABLE_NAME");
                for (int i = 0; i < n; i++)
                {
                    DataRow m_DataRow = shemaTable.Rows[i];
                    strTable[i] = m_DataRow.ItemArray.GetValue(m).ToString();
                }
                return strTable;
            }
            catch (OleDbException ex)
            {

                return null;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
        }


        /// <summary>
        /// 返回某一表的
        /// </summary>
        public static DataTable GetTableColumn(string database_path, string varTableName, DataTable dt_source)
        {
            if (database_path == "")
                database_path = Application.StartupPath + "\\case\\demoproject\\project\\Database.mdb";
            string connectionstring = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + database_path;
            DataTable dt = new DataTable();
            //try
            //{
            conn = new OleDbConnection();
            conn.ConnectionString = connectionstring;
            conn.Open();
            dt = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, new object[] { null, null, varTableName, null });
            int n = dt.Rows.Count;
            string[] strTable = new string[n];
            List<datatypedic> dt2 = readxml();
            dt.Columns.Add("DATA_TYPE_字符类型", typeof(string));
            int m = dt.Columns.IndexOf("COLUMN_NAME");
            for (int i = 0; i < n; i++)
            {
                DataRow m_DataRow = dt.Rows[i];
                //strTable[i] = m_DataRow.ItemArray.GetValue(m).ToString();
                foreach (datatypedic idt in dt2)
                    if (idt.DataTypeint == dt.Rows[i]["DATA_TYPE"].ToString())
                        dt.Rows[i]["DATA_TYPE_字符类型"] = idt.DataTypestring;
            }
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < dt_source.Rows.Count; j++)
                    if (dt.Rows[i]["COLUMN_NAME"].ToString() == dt_source.Rows[j]["ID"].ToString())
                    {
                        dt_source.Rows[j]["ID_字符类型"] = dt.Rows[i]["DATA_TYPE_字符类型"];

                    }

            }



            return dt_source;
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            //finally
            //{
            //    conn.Close();
            //}
        }
        public static List<datatypedic> readxml()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Application.StartupPath + "\\Config\\DataTypeDiction.xml");
            XmlNode xn = doc.SelectSingleNode("DataTypeMaps");
            XmlNodeList xnl = doc.GetElementsByTagName("DataType");
            //XmlNodeList xnl2 = xn.ChildNodes;
            List<datatypedic> bookModeList = new List<datatypedic>();
            foreach (XmlNode xn1 in xnl)
            {

                datatypedic dataTypedic = new datatypedic();

                // 将节点转换为元素，便于得到节点的属性值

                XmlElement xe = (XmlElement)xn1;

                // 得到Type和ISBN两个属性的属性值

                dataTypedic.DataTypeint = xe.GetAttribute("code").ToString();

                dataTypedic.DataTypestring = xe.GetAttribute("name").ToString();

                bookModeList.Add(dataTypedic);
            }
            return bookModeList;
        }

        /// <summary>
        /// 把数据从Excel装载到DataTable
        /// </summary>
        /// <param name="pathName">带路径的Excel文件名</param>
        /// <param name="sheetName">工作表名</param>
        /// <param name="tbContainer">将数据存入的DataTable</param>
        /// <returns></returns>
        public static DataTable ExcelToDataTable(string sheetName, string Sql, string pathName = "")
        {
            if (pathName == "")
                pathName = Application.StartupPath + "\\Config\\ACCESS.xlsx";

            DataTable tbContainer = new DataTable();
            string strConn = string.Empty;
            if (string.IsNullOrEmpty(sheetName)) { sheetName = "Sheet1"; }
            FileInfo file = new FileInfo(pathName);
            if (!file.Exists) { throw new Exception("文件不存在"); }
            string extension = file.Extension;
            switch (extension)
            {
                case ".xls":
                    strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pathName + ";Extended Properties='Excel 8.0;HDR=Yes;IMEX=1;'";
                    break;
                case ".xlsx":
                    strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + pathName + ";Extended Properties='Excel 12.0;HDR=Yes;IMEX=1;'";
                    break;
                default:
                    strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pathName + ";Extended Properties='Excel 8.0;HDR=Yes;IMEX=1;'";
                    break;
            }

            //链接Excel
            OleDbConnection cnnxls = new OleDbConnection(strConn);
            //读取Excel里面有 表Sheet1
            //OleDbDataAdapter oda = new OleDbDataAdapter(string.Format("select " + columname + " from [{0}$]", sheetName), cnnxls);
            OleDbDataAdapter oda = new OleDbDataAdapter(string.Format(Sql), cnnxls);
            DataSet ds = new DataSet();
            //将Excel里面有表内容装载到内存表中！

            oda.Fill(tbContainer);
            return tbContainer;
        }

        public static DataTable ExcelSheetName(string pathName)
        {

            string strConn = string.Empty;

            FileInfo file = new FileInfo(pathName);
            if (!file.Exists) { throw new Exception("文件不存在"); }
            string extension = file.Extension;
            switch (extension)
            {
                case ".xls":
                    strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pathName + ";Extended Properties='Excel 8.0;HDR=Yes;IMEX=1;'";
                    break;
                case ".xlsx":
                    strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + pathName + ";Extended Properties='Excel 12.0;HDR=Yes;IMEX=1;'";
                    break;
                default:
                    strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pathName + ";Extended Properties='Excel 8.0;HDR=Yes;IMEX=1;'";
                    break;
            }

            //链接Excel
            OleDbConnection cnnxls = new OleDbConnection(strConn);
            cnnxls.Open();
            DataTable shemaTable = cnnxls.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
            int n = shemaTable.Rows.Count;
            string[] strTable = new string[n];
            int m = shemaTable.Columns.IndexOf("TABLE_NAME");
            DataTable tblDatas = new DataTable("Datas");
            tblDatas.Columns.Add("sheetname", Type.GetType("System.String"));
            for (int i = 0; i < n; i++)
            {
                DataRow m_DataRow = shemaTable.Rows[i];

                if (!m_DataRow.ItemArray.GetValue(m).ToString().Contains("_xlnm#_FilterDatabase"))

                    tblDatas.Rows.Add(new object[] { m_DataRow.ItemArray.GetValue(m).ToString().Substring(0, m_DataRow.ItemArray.GetValue(m).ToString().Length - 1) });
            }
            return tblDatas;
        }

        public static DataTable ExcelSheetField(string pathName, string sheetName)
        {
            sheetName = sheetName + "$";
            DataTable dt = new DataTable();
            string strConn = string.Empty;

            FileInfo file = new FileInfo(pathName);
            if (!file.Exists) { throw new Exception("文件不存在"); }
            string extension = file.Extension;
            switch (extension)
            {
                case ".xls":
                    strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pathName + ";Extended Properties='Excel 8.0;HDR=Yes;IMEX=1;'";
                    break;
                case ".xlsx":
                    strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + pathName + ";Extended Properties='Excel 12.0;HDR=Yes;IMEX=1;'";
                    break;
                default:
                    strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pathName + ";Extended Properties='Excel 8.0;HDR=Yes;IMEX=1;'";
                    break;
            }

            //链接Excel
            OleDbConnection cnnxls = new OleDbConnection(strConn);
            cnnxls.Open();
            dt = cnnxls.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, new object[] { null, null, sheetName, null });
            int n = dt.Rows.Count;

            int m = dt.Columns.IndexOf("COLUMN_NAME");
            DataTable tblDatas = new DataTable(sheetName);
            tblDatas.Columns.Add("COLUMN_NAME", Type.GetType("System.String"));
            for (int i = 0; i < n; i++)
            {
                DataRow m_DataRow = dt.Rows[i];
                if (m_DataRow.ItemArray.GetValue(m).ToString().Substring(0, 1) != "F")
                    tblDatas.Rows.Add(new object[] { m_DataRow.ItemArray.GetValue(m).ToString().Substring(0, m_DataRow.ItemArray.GetValue(m).ToString().Length) });
            }
            return tblDatas;
        }

        public static void UpdateDataTable(string sqlstr, string DabaBasePath, DataTable table)
        {


            OleDbDataAdapter da = new OleDbDataAdapter();
            try
            {
                openConnection(DabaBasePath);
                comm.CommandType = CommandType.Text;
                comm.CommandText = sqlstr;
                da.SelectCommand = comm;
                OleDbCommandBuilder builder = new OleDbCommandBuilder(da);
                da.Update(table);
                table.AcceptChanges();

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                closeConnection();
            }

        }
        public static void InsertDataTable(string ExcelpathName, string sheetName, string EXCELSql, string accessFilePath, DataTable LinkSourceTarget)
        {
            try
            {
                DataTable dt = ExcelToDataTable(ExcelpathName, sheetName, EXCELSql);

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string AcessSql = "insert into " + LinkSourceTarget.Rows[0]["TABLE_ID"].ToString() + " (";
                        for (int ii = 0; ii < LinkSourceTarget.Rows.Count - 1; ii++)
                        {
                            if (LinkSourceTarget.Rows[ii]["源字段"].ToString() != "")
                                if (dt.Rows[i][LinkSourceTarget.Rows[ii]["源字段"].ToString()].ToString() != "")
                                    AcessSql = AcessSql + LinkSourceTarget.Rows[ii]["ID"] + ",";
                        }
                        if (LinkSourceTarget.Rows[LinkSourceTarget.Rows.Count - 1]["源字段"].ToString() != "")
                            if (dt.Rows[i][LinkSourceTarget.Rows[LinkSourceTarget.Rows.Count - 1]["源字段"].ToString()].ToString() != "")
                                AcessSql = AcessSql + LinkSourceTarget.Rows[LinkSourceTarget.Rows.Count - 1]["ID"];

                        AcessSql = AcessSql + ")  values('";
                        for (int ii = 0; ii < LinkSourceTarget.Rows.Count - 1; ii++)
                        {
                            if (LinkSourceTarget.Rows[ii]["源字段"].ToString() != "")
                                if (dt.Rows[i][LinkSourceTarget.Rows[ii]["源字段"].ToString()].ToString() != "")
                                    AcessSql = AcessSql + dt.Rows[i][LinkSourceTarget.Rows[ii]["源字段"].ToString()] + "','";
                        }
                        if (LinkSourceTarget.Rows[LinkSourceTarget.Rows.Count - 1]["源字段"].ToString() != "")
                            if (dt.Rows[i][LinkSourceTarget.Rows[LinkSourceTarget.Rows.Count - 1]["源字段"].ToString()].ToString() != "")
                                AcessSql = AcessSql + (dt.Rows[i][LinkSourceTarget.Rows[LinkSourceTarget.Rows.Count - 1]["源字段"].ToString()]) + "'";

                        AcessSql = AcessSql + ");";
                        updateAccessTable(AcessSql, accessFilePath);
                    }
                    MessageBox.Show("数据导入成功！");
                }
                else
                {
                    MessageBox.Show("请检查你的Excel中是否存在数据");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        //更新或者插入数据到access数据库

        //dataFilePath指定access文件的路径

        //sql指定数据库的更新或者插入语句

        //返回值int表示此次更新影响的行数

        public static int updateAccessTable(string sql, string dataFilePath)
        {

            // 连接数据库 

            OleDbConnection connct = new OleDbConnection();

            string oleDB = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + dataFilePath;

            connct.ConnectionString = oleDB;

            //打开数据库

            connct.Open();

            //执行命令

            OleDbCommand myCommand = new OleDbCommand(sql, connct);

            int res = myCommand.ExecuteNonQuery();



            // 关闭连接 

            connct.Close();

            return res;

        }
        public void ConvertDataTableToXML(DataTable xmlDS, string dg)
        {
            //MemoryStream stream = null;
            XmlTextWriter writer = null;
            try
            {
                //stream = new MemoryStream();
                writer = new XmlTextWriter(dg, Encoding.Default);
                xmlDS.WriteXml(writer);
                //int count = (int)stream.Length;
                //byte[] arr = new byte[count];
                //stream.Seek(0, SeekOrigin.Begin);
                //stream.Read(arr, 0, count);
                //UTF8Encoding utf = new UTF8Encoding();
                //return utf.GetString(arr).Trim();
            }
            catch
            {
                //return String.Empty;
            }
            finally
            {
                if (writer != null) writer.Close();
            }
        }
        public DataTable ConvertXMLToDataSet(string xmlData)
        {
            StringReader stream = null;
            XmlTextReader reader = null;
            try
            {
                DataTable xmlDS = new DataTable();
                stream = new StringReader(xmlData);
                reader = new XmlTextReader(stream);
                xmlDS.ReadXml(reader);
                return xmlDS;
            }
            catch (Exception ex)
            {
                string strTest = ex.Message;
                return null;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }

        public static DataTable ExcelToDataTable2(string sheetName, string pathName = "")
        {
            if (pathName == "")
                pathName = Application.StartupPath + "\\Config\\ACCESS.xlsx";
            sheetName = sheetName.Substring(0, sheetName.Length - 1);//主要用于剔除在对excelSQL加的$
            Aspose.Cells.Workbook workbook1 = new Aspose.Cells.Workbook();
            workbook1.Open(pathName);
            Aspose.Cells.Worksheet cellSheet1 = workbook1.Worksheets[sheetName];
            Cells cells1 = cellSheet1.Cells;

            return cells1.ExportDataTableAsString(0, 0, cells1.MaxDataRow + 1, cells1.MaxDataColumn + 1, true);
        }
        
        /// <summary>
        /// 目前暂时有问题
        /// </summary>
        /// <param name="ExcelpathName"></param>
        /// <param name="sheetName"></param>
        /// <param name="EXCELSql"></param>
        /// <param name="accessFilePath"></param>
        /// <param name="LinkSourceTarget"></param>
        public static void InsertDataTable2(string ExcelpathName, string sheetName, string EXCELSql, string accessFilePath, DataTable LinkSourceTarget)
        {
            try
            {
            if (accessFilePath == "")
                accessFilePath = Application.StartupPath +@"\case\demoproject\project\Database.mdb";
           
            DataTable dt = ExcelToDataTable2(sheetName, ExcelpathName);
            //ExcelToDataTable(string sheetName, string Sql,string pathName="")
            if (dt.Rows.Count > 0)
            {


                String tableName = LinkSourceTarget.Rows[0]["TABLE_ID"].ToString();

                String connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + accessFilePath;

                //string SQL = "SELECT * FROM " + tableName;
                OleDbConnection conn = new OleDbConnection(connectionString);
                OleDbDataAdapter adapt = new OleDbDataAdapter();

              
                //OleConn.Open();
                //DataTable dt = new DataTable();
                //OleDbDataAdapter da = new OleDbDataAdapter();

                //openConnection(connectionString);
                //    comm.CommandType = CommandType.Text;



                //  string AcessSql = "insert into " + tableName + "(id,ItemGuid,StartTime) values (@id,@ItemGuid,@StartTime)";
                string AcessSql = "insert into " + tableName + "(";
                for (int ii = 0; ii < LinkSourceTarget.Rows.Count - 1; ii++)
                {
                    if (LinkSourceTarget.Rows[ii]["源字段"].ToString() != "")
                        AcessSql = AcessSql + LinkSourceTarget.Rows[ii]["ID"].ToString() + ",";
                }
                if (LinkSourceTarget.Rows[LinkSourceTarget.Rows.Count - 1]["源字段"].ToString() != "")
                    AcessSql = AcessSql + LinkSourceTarget.Rows[LinkSourceTarget.Rows.Count - 1]["ID"].ToString();
                else
                    AcessSql = AcessSql.Substring(0, AcessSql.Length - 1);
                AcessSql = AcessSql + ") values (@";
                for (int ii = 0; ii < LinkSourceTarget.Rows.Count - 1; ii++)
                {
                    if (LinkSourceTarget.Rows[ii]["源字段"].ToString() != "")
                        AcessSql = AcessSql + LinkSourceTarget.Rows[ii]["源字段"].ToString() + ",@";
                }
                if (LinkSourceTarget.Rows[LinkSourceTarget.Rows.Count - 1]["源字段"].ToString() != "")
                    AcessSql = AcessSql + LinkSourceTarget.Rows[LinkSourceTarget.Rows.Count - 1]["源字段"].ToString();
                else
                    AcessSql = AcessSql.Substring(0, AcessSql.Length -2);

                AcessSql = AcessSql + ")";

                //OleDbConnection OleConn = new OleDbConnection(connectionString);
                //OleDbDataAdapter OleAdp = new OleDbDataAdapter(SQL, OleConn);
                //OleAdp.InsertCommand = new OleDbCommand(AcessSql);
                var cmd = new OleDbCommand(AcessSql, conn);
                //var cmd = new OleDbCommand(AcessSql, conn);
                // string cmdstring = "";

                for (int ii = 0; ii < LinkSourceTarget.Rows.Count; ii++)
                {
                    if (LinkSourceTarget.Rows[ii]["源字段"].ToString() != "")
                    {
                        string cmdstring = "@" + LinkSourceTarget.Rows[ii]["ID"].ToString();
                        string cmdstring2 = LinkSourceTarget.Rows[ii]["源字段"].ToString();
                        // OleAdp.InsertCommand.Parameters.Add(cmdstring,OleDbType.,8,cmdstring2);
                        OleDbType fTYPE = new OleDbType();
                        switch (LinkSourceTarget.Rows[ii]["ID_字符类型"].ToString())
                        {
                               
                            case "Empty": fTYPE = OleDbType.Empty; break;
                            case "SmallInt": fTYPE = OleDbType.SmallInt; break;
                            case "Integer": fTYPE = OleDbType.Integer; break;
                            case "Single": fTYPE = OleDbType.Single; break;
                            case "Double": fTYPE = OleDbType.Double; break;
                            case "Currency": fTYPE = OleDbType.Currency; break;
                            case "Date": fTYPE = OleDbType.Date; break;
                            case "BSTR": fTYPE = OleDbType.BSTR; break;
                            case "IDispatch": fTYPE = OleDbType.IDispatch; break;
                            case "Error": fTYPE = OleDbType.Error; break;
                            case "Boolean": fTYPE = OleDbType.Boolean; break;
                            case "Variant": fTYPE = OleDbType.Variant; break;
                            case "IUnknown": fTYPE = OleDbType.IUnknown; break;
                            case "Decimal": fTYPE = OleDbType.Decimal; break;
                            case "TinyInt": fTYPE = OleDbType.TinyInt; break;
                            case "UnsignedTinyInt": fTYPE = OleDbType.UnsignedTinyInt; break;
                            case "UnsignedSmallInt": fTYPE = OleDbType.UnsignedSmallInt; break;
                            case "UnsignedInt": fTYPE = OleDbType.UnsignedInt; break;
                            case "BigInt": fTYPE = OleDbType.BigInt; break;
                            case "UnsignedBigInt": fTYPE = OleDbType.UnsignedBigInt; break;
                            case "Binary": fTYPE = OleDbType.Binary; break;
                            case "Char": fTYPE = OleDbType.Char; break;
                            case "WChar": fTYPE = OleDbType.WChar; break;
                            case "Numeric": fTYPE = OleDbType.Numeric; break;
                            case "DBDate": fTYPE = OleDbType.DBDate; break;
                            case "DBTime": fTYPE = OleDbType.DBTime; break;
                            case "DBTimeStamp": fTYPE = OleDbType.DBTimeStamp; break;
                            case "PropVariant": fTYPE = OleDbType.PropVariant; break;
                            case "VarNumeric": fTYPE = OleDbType.VarNumeric; break;
                            case "VarChar": fTYPE = OleDbType.VarChar; break;
                            case "LongVarWChar": fTYPE = OleDbType.LongVarWChar; break;
                            case "VarBinary": fTYPE = OleDbType.VarBinary; break;
                            case "LongVarChar": fTYPE = OleDbType.LongVarChar; break;
                        }
                        //OleAdp.InsertCommand.Parameters.Add(cmdstring, fTYPE, 255, cmdstring2);
                        cmd.Parameters.Add(cmdstring, fTYPE, 255, cmdstring2);
                    }
                }

              

                //OleAdp.InsertCommand.Connection = OleConn;
                //OleAdp.InsertCommand.Connection.Open();
                //int count = OleAdp.Update(dt);
                //OleAdp.InsertCommand.Connection.Close();
                adapt.InsertCommand = cmd;
                OleDbCommandBuilder builder = new OleDbCommandBuilder(adapt);
                builder.QuotePrefix = "[";
                builder.QuoteSuffix = "]";
                int count = adapt.Update(dt);
                adapt.InsertCommand.Connection.Close();
                MessageBox.Show("数据导入成功！" + count.ToString());
                }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
            }
        }


    }

}
