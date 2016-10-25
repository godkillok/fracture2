using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Global
{
    public class Globalname
    {
        public static string localFilePath = "";
        public static string DabaBasePath = "";
        public void openproject()
        {
            XmlDocument doc = new XmlDocument();
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.RestoreDirectory = true;
            fileDialog.Title = "打开工程";
            fileDialog.Filter = "工程文件|*.xml";
            DialogResult dialogResult = fileDialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                localFilePath = fileDialog.FileName.ToString();
                doc.Load(localFilePath);
                localFilePath = Path.GetDirectoryName(localFilePath);
                DabaBasePath = "provider=microsoft.jet.oledb.4.0; Data Source=" + localFilePath + "\\project\\Database.mdb";

            }
                   }
        public void newproject()
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.RestoreDirectory = true;
            fileDialog.Title = "新建工程";
            fileDialog.Filter = "工程文件|*.xml";
            DialogResult dialogResult = fileDialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                if (System.IO.File.Exists(fileDialog.FileName))
                {
                    System.IO.File.Delete(fileDialog.FileName);
                }
                //获得文件路径
                string temp_path = fileDialog.FileName.ToString();
                string path = Path.GetDirectoryName(temp_path) + "\\" + Path.GetFileNameWithoutExtension(temp_path) + '\\' + Path.GetFileName(temp_path);

                if (!Directory.Exists(Path.GetDirectoryName(path)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                }
                createxml(path);
                localFilePath = Path.GetDirectoryName(path);
                createdatebase();
                DabaBasePath = "provider=microsoft.jet.oledb.4.0; Data Source=" + localFilePath + "\\project\\Database.mdb";
            }

            //string ext = ".CSSM";              //文件扩展名  
            //string des = "CssmFile";           //文件描述，最好用英文
            //string ico = Application.StartupPath.ToString() + "Green-Co2.ico";  //图标文件路径

            //RegistryKey k1 = Registry.ClassesRoot;
            //k1.CreateSubKey(ext).SetValue("", des);
            //RegistryKey k2 = k1.CreateSubKey(des);
            //k2.CreateSubKey("DefaultIcon").SetValue("", ico + ",0");
            //k1.Close();
            //k2.Close();
            //RegFile("CssmFile", "CSSM", "Green-Co2.ico");

        }
        private void createxml(string path)
        {
            XmlDocument doc = new XmlDocument();
            XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", "GB2312", null);
            doc.AppendChild(dec);
            //创建一个根节点（一级）
            XmlElement node = doc.CreateElement("First");
            doc.AppendChild(node);
            //创建节点（2级）
            XmlElement element1 = doc.CreateElement("pathroot");
            element1.InnerText = "路径";
            node.AppendChild(element1);
            XmlElement element2 = doc.CreateElement("createtime");
            element2.InnerText = DateTime.Now.ToShortDateString();
            node.AppendChild(element2);
            doc.Save(path);
        }
        public void RegFile(string fileTypeName, string fileExt, string fileIcon)
        {
            RegistryKey key = Registry.ClassesRoot.OpenSubKey("." + fileExt);
            if (key == null)
            {
                key = Registry.ClassesRoot.CreateSubKey("." + fileExt);
                key.SetValue("", fileTypeName + "." + fileExt);
                key.SetValue("Content Type", "application/" + fileExt);
                key = Registry.ClassesRoot.CreateSubKey(fileTypeName + "." + fileExt);
                key.SetValue("", fileTypeName);
                RegistryKey keySub = key.CreateSubKey("DefaultIcon");
                keySub.SetValue("", System.Windows.Forms.Application.StartupPath + "\\" + fileIcon);
                keySub = key.CreateSubKey("shell\\open\\command");
                keySub.SetValue("", "\"" + System.Windows.Forms.Application.ExecutablePath + "\" \"%1\"");
            }
        }
        private void createdatebase()
        {
            string path_source = Application.StartupPath + "\\config\\Database.mdb";
            string strDestination = localFilePath + "\\project\\Database.mdb";
            string strPath = Path.GetDirectoryName(strDestination);
            if (!Directory.Exists(strPath))
            {
                Directory.CreateDirectory(strPath);
            }
            File.Copy(path_source, strDestination, true);//允许覆盖目的地的同名文件

        }
    }
}

