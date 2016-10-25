using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraCharts.Designer;
using System.IO;


namespace fracture
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        CustomClass cc = new CustomClass();//空构造函数，一边测试属性值改变

        private void button1_Click(object sender, EventArgs e)
        {
            ChartDesigner designer = new ChartDesigner(chartControl1);
            designer.ShowDialog();

        }

        private void button2_Click(object sender, EventArgs e)
        {
             FileOperate.importfilecharttemplete(chartControl1);
       
        }

        private void button3_Click(object sender, EventArgs e)
        {
          FileOperate.exportfilecharttemplete(chartControl1);
   
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cc.Changed += new CustomClass.ChangedEventHandler(cc_Changed);//调用者端订阅事件，为Changed事件提供了具体的事件函数cc_Changed
        }

        private void button4_Click(object sender, EventArgs e)
        {
            cc.Cid = 1;
            cc.Cname = "Lee";//给CustomClass的属性赋值，赋值是引发事件
            string str = cc.Cid.ToString() + cc.Cname;
            MessageBox.Show(str);
        }
        private void cc_Changed()//事件  注：被自定义事件绑定的事件函数
        {
            textBox1.Text = cc.Cid.ToString();
            textBox2.Text = cc.Cname;
        }
    
    }
}
