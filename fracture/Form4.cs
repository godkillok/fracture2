using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace fracture
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 输入参数
        /// </summary>
        private double[] v;//评判集向量(一维矩阵),输入参数，相当于等级进行数值化，如优 95 中85 良 65  差35 
        private double[] u;//权重分配向量(一维矩阵),相当于评价指标的权重，
        private double[,] r; //单因素评判矩阵，行数与权重分配矩阵列数相同，列数与评判集矩阵列数相同,TGP:行数是评价考虑的因素，列数是要分的等级数
        private double[,] rr;//r的原始数据
        private int methodID;//模糊综合评价模型种类
        private int preTreatmentID;//数据预处理，多tool_menu：标准差变换和极差变换
        /// <summary>
        /// 输出计算结果
        /// </summary>
        private double[] b;//评判结果向量(一维矩阵)
        private double result;//以结果向量为权表述的计算结果
        /// <summary>
        /// 计算主程序
        /// </summary>
        /// 
        public void inputdata()
        {
            preTreatmentID = 0;
            methodID = 0;
            v = new double[4] { 100, 85, 70, 60 };
            u = new double[3] { 0.3, 0.3, 0.4 };
            r = new double[3, 4] { { 0.5, 0.3, 0.2, 0 }, { 0.3, 0.4, 0.2, 0.1 }, { 0.2, 0.2, 0.3, 0.2 } };
            rr = new double[r.GetUpperBound(0) + 1, r.GetUpperBound(1) + 1];
            for (int i = 0; i < r.GetUpperBound(0) + 1; i++)
                for (int j = 0; j < r.GetUpperBound(1) + 1; j++)
                    rr[i, j] = r[i, j];
            memoEdit1.Text += inputAsString();

            memoEdit1.Text += "\r\n" + "计算中....\r\n";
            //标准化
        }
        public void fuzzysystem()
        {
            inputdata();



            switch (preTreatmentID)
            {
                case 0: r = Standardization(r); break;
                case 1: r = Difference(r); break;
            }
            //string sR = "";
            //for (int i = 0; i < r.GetUpperBound(0) + 1; i++)
            //{
            //    for (int j = 0; j < r.GetUpperBound(1) + 1; j++)
            //        sR = sR + "  " + r[i, j] + "  ";
            //    sR = sR + "\n";
            //}
            //MessageBox.Show(sR);
            switch (methodID)
            {
                case 0: b = Judge1(r, u); break;
                case 1: b = Judge2(r, u); break;
                case 2: b = Judge3(r, u); break;
                case 3: b = Judge4(r, u); break;
            }
            result = 0;
            double d1 = 0, d2 = 0;
            for (int i = 0; i < b.Length; i++)
            {
                d1 = d1 + b[i] * v[i];
                d2 = d2 + b[i];
            }
            result = d1 / d2;
            memoEdit1.Text += outputAsString();

        }

        public string inputAsString()
        {
            string sR = "";
            sR = sR + "-------------输入参数------------------\r\n\r\n";
            //显示评判集各元素
            sR = sR + "共有" + (r.GetUpperBound(1) + 1) + "个评判因素....\r\n";
            for (int i = 0; i < r.GetUpperBound(1) + 1; i++)
                sR = sR + "  " + v[i] + "  ";
            sR = sR + "\r\n\r\n";
            //显示权向量各元素
            sR = sR + "共有" + (r.GetUpperBound(0) + 1) + "个评判等级......\r\n";
            for (int i = 0; i < r.GetUpperBound(0) + 1; i++)
                sR = sR + "  " + u[i] + "  ";
            sR = sR + "\r\n\r\n";
            //采用标准化的方法
            sR = sR + "-------------数据标准化的方法--------------\r\n\r\n";
            switch (preTreatmentID)
            {
                case 0: sR = sR + "离差标准化" + "\r\n"; break;
                case 1: sR = sR + "标准差标准化" + "\r\n"; break;
            }
            sR = sR + "\r\n\r\n";
            //显示评判矩阵各元素
            sR = sR + "评判矩阵为" + (r.GetUpperBound(0) + 1) + "行，" + (r.GetUpperBound(1) + 1) + "列矩阵....\r\n";
            for (int i = 0; i < r.GetUpperBound(0) + 1; i++)
            {
                for (int j = 0; j < r.GetUpperBound(1) + 1; j++)
                    sR = sR + "  " + rr[i, j] + "  ";
                sR = sR + "\r\n";
            }
            //显示综合评判方法
            switch (methodID)
            {
                case 0: sR = sR + "\r\n" + "------综合评判方法：模糊变换M(∧∨)------" + "\r\n"; break;
                case 1: sR = sR + "\r\n" + "------综合评判方法：以乘代替取小M(·∨)------" + "\r\n"; break;
                case 2: sR = sR + "\r\n" + "------综合评判方法：以加代替取大M(∧+)------" + "\r\n"; break;
                case 3: sR = sR + "\r\n" + "------综合评判方法：加权平均M(·+)------" + "\r\n"; break;
            }


            return sR;
        }

        public string outputAsString()
        {
            string sR = "";
            //显示评判结果
            sR = sR + "\r\n-------------评判结果输出--------------\r\n\r\n";
            sR = sR + "评判结果向量元素\r\n";
            for (int i = 0; i < r.GetUpperBound(1) + 1; i++)
                sR = sR + "  " + b[i] + "  ";
            sR = sR + "\r\n\r\n";
            //以结果向量为权表述的计算结果
            sR = sR + "以结果向量为权表述的计算结果，综合得分:" + result;
            sR = sR + "\r\n";
            return sR;
        }

        /// <summary>
        /// 0-1标准化(0-1 normalization)，离差标准化
        /// r:评判矩阵，u：权重矩阵, r; //单因素评判矩阵，行数与权重分配矩阵列数相同，列数与评判集矩阵列数相同,TGP:行数是评价考虑的因素，列数是要评价的对象
        /// 返回结果为证券结果矩阵b
        /// </summary>
        private double[,] Standardization(double[,] r)
        {
            int M, N, i, j;
            N = r.GetUpperBound(0) + 1; M = r.GetUpperBound(1) + 1;
            double[] array = new double[M];
            for (i = 0; i < N; i++)
            {
                for (j = 0; j < M; j++)
                {
                    array[j] = r[i, j];

                }
                double maxvalue = GetMax(array);
                double minvalue = GetMin(array);
                for (j = 0; j < M; j++)
                {
                    r[i, j] = (r[i, j] - minvalue) / (maxvalue - minvalue);
                }
            }
            return r;
        }

        /// <summary>
        /// Z-score 标准化(zero-mean normalization)
        /// r:评判矩阵，u：权重矩阵, r; //单因素评判矩阵，行数与权重分配矩阵列数相同，列数与评判集矩阵列数相同,
        /// TGP: r; 行数是评价考虑的因素，列数是要评价的对象
        /// 返回结果为证券结果矩阵b
        /// </summary>
        private double[,] Difference(double[,] r)
        {
            int M, N, i, j;
            N = r.GetUpperBound(0) + 1; M = r.GetUpperBound(1) + 1;
            double[] array = new double[M];
            //r的一行是一个因素在不同的等级的概率，所以需要进行规划化
            for (i = 0; i < N; i++)
            {
                for (j = 0; j < M; j++)
                {
                    array[j] = r[i, j];

                }
                double aver = Getaver(array);
                double Deviation = GetDeviation(array);
                for (j = 0; j < M; j++)
                {
                    r[i, j] = (r[i, j] - aver) / (Deviation);
                }
            }
            return r;
        }
        /// <summary>
        /// 数组中最大的值
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        private static double GetMax(double[] array)
        {
            double max = array[0];
            for (int i = 0; i < array.Length; i++)
            {
                max = max > array[i] ? max : array[i];

            }
            return max;
        }
        private static double GetMin(double[] array)
        {
            double min = array[0];
            for (int i = 0; i < array.Length; i++)
            {
                min = min < array[i] ? min : array[i];

            }
            return min;
        }
        private static double Getaver(double[] array)
        {
            double Getaver = 0;
            for (int i = 0; i < array.Length; i++)
            {
                Getaver = Getaver + array[i];

            }
            Getaver = Getaver / array.Length;

            return Getaver;
        }
        private static double GetDeviation(double[] array)
        {
            double variance = 0;
            for (int i = 0; i < array.Length; i++)
            {
                variance += Math.Pow(array[i] - Getaver(array), 2);

            }//求方差
            double GetDeviation = Math.Pow(variance, 0.5);//求标准差
            return GetDeviation;
        }

        /// <summary>
        /// 评判方法：模糊变换M(∧∨)
        /// r:评判矩阵，u：权重矩阵,
        //r:单因素评判矩阵，行数与权重分配矩阵列数相同，列数列数与评判集矩阵列数相同
        /// 返回结果为证券结果矩阵b
        /// </summary>
        private double[] Judge1(double[,] r, double[] u)
        {
            int M, N, i, j;
            N = r.GetUpperBound(0) + 1; M = r.GetUpperBound(1) + 1;
            double[] b = new double[M];
            for (j = 0; j < M; j++)
                for (i = 0; i < N; i++)
                    if (u[i] < r[i, j]) r[i, j] = u[i];//取小
            for (j = 0; j < M; j++)
            {
                b[j] = 0;
                for (i = 0; i < N; i++)
                    if (b[j] < r[i, j]) b[j] = r[i, j];//取大
            }
            return b;
        }

        /// <summary>
        /// 评判方法：以乘代替取小M(·∨)
        /// r:评判矩阵，u：权重矩阵
        /// 返回结果为证券结果矩阵b
        /// </summary>
        private double[] Judge2(double[,] r, double[] u)
        {
            int M, N, i, j;
            N = r.GetUpperBound(0) + 1; M = r.GetUpperBound(1) + 1;
            double[] b = new double[M];
            for (j = 0; j < M; j++)
                for (i = 0; i < N; i++)
                    r[i, j] = u[i] * r[i, j];//取乘
            for (j = 0; j < M; j++)
            {
                b[j] = 0;
                for (i = 0; i < N; i++)
                    if (b[j] < r[i, j]) b[j] = r[i, j];//取大
            }
            return b;
        }

        /// <summary>
        /// 评判方法：以加代替取大M(∧+)
        /// r:评判矩阵，u：权重矩阵
        /// 返回结果为证券结果矩阵b
        /// </summary>
        private double[] Judge3(double[,] r, double[] u)
        {
            int M, N, i, j;
            N = r.GetUpperBound(0) + 1; M = r.GetUpperBound(1) + 1;
            double[] b = new double[M];
            for (j = 0; j < M; j++)
                for (i = 0; i < N; i++)
                    if (u[i] < r[i, j]) r[i, j] = u[i];//取小
            for (j = 0; j < M; j++)
            {
                b[j] = 0;
                for (i = 0; i < N; i++)
                    b[j] = b[j] + r[i, j];//取加
            }
            return b;
        }

        /// <summary>
        /// 评判方法：加权平均M(·+)
        /// r:评判矩阵，u：权重矩阵
        /// 返回结果为证券结果矩阵b
        /// </summary>
        private double[] Judge4(double[,] r, double[] u)
        {
            int M, N, i, j;
            N = r.GetUpperBound(0) + 1; M = r.GetUpperBound(1) + 1;
            double[] b = new double[M];
            for (j = 0; j < M; j++)
                for (i = 0; i < N; i++)
                    r[i, j] = r[i, j] * u[i];//取乘
            for (j = 0; j < M; j++)
            {
                b[j] = 0;
                for (i = 0; i < N; i++)
                    b[j] = b[j] + r[i, j];//取加
            }
            return b;
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            memoEdit1.Text = "";
            //fuzzysystem();
            double[,] a_b = new double[3, 3] { { 1, 0.333, 2 }, { 3, 1, 5 }, { 0.5, 0.2, 1 } };//层次分析法判断矩阵是
            double[] wvaule;//最后的权重向量
            wvaule=ahpsysytem(a_b);
            string sR = "";
            if (wvaule == null)
            {
                MessageBox.Show("未通过一致性检验，请检查判断矩阵！");
                return;
            }
            //显示评判结果
            sR = sR + "\r\n-------------评判结果输出--------------\r\n\r\n";
            sR = sR + "评判结果向量元素\r\n";
            for (int i = 0; i < wvaule.GetUpperBound(0) + 1; i++)
                sR = sR + "  " + wvaule[i] + "  ";
            memoEdit1.Text = sR;
        }
        public double[] ahpsysytem(double [,] a_b)
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
            double crindex = cindex / ri[N-1];

            if ((crindex < 0.1) && (cindex < 0.1))
            {
            }
            else
            {
                wvaule=null;
            }
              return wvaule; 
     
        }
        private void lamuda1(double[,] a_b, out double[] wvaule, out double lamudamax)
        {//求和法,正确性待验证
            lamudamax = 0;
            int M, N, i, j;

            N = a_b.GetUpperBound(0) + 1; M = a_b.GetUpperBound(1) + 1;
            double[] rowsum = new double[N];
           wvaule = new double[N];
            double[] rwmatirx = new double[N];
            //1）将判断矩阵A按列归一化（即列元素之和为1）
            for (j = 0; j < M; j++)
            {
                double sumcol = 0;
                for (i = 0; i < N; i++)
                {
                    sumcol = sumcol + a_b[i, j];
                }
                for (i = 0; i < N; i++)
                {
                    a_b[i, j] = a_b[i, j] / sumcol;
                }

            }
            //2）将归一化的矩阵按行求和
            for (i = 0; i < N; i++)
            {
                rowsum[i] = 0;
                for (j = 0; j < M; j++)
                {
                    rowsum[i] = rowsum[i] + a_b[i, j];
                }
            }
            //3）将ci归一化：得到特征向量W即为A的特征向量的近似值；
            double sumrow = 0;
            for (i = 0; i < N; i++)
            {
                sumrow = sumrow + rowsum[i];
            }
            for (i = 0; i < N; i++)
            {
                wvaule[i] =  rowsum[i]/sumrow;
            }
            //4）求特征向量W对应的最大特征值：
          
            for (i = 0; i < N; i++)
            { double summatirx=0;
                for (j = 0; j < N; j++)
                {
          summatirx=summatirx+a_b[i,j]*wvaule[j];
                }
                rwmatirx[i] = summatirx;
            }
            for (i = 0; i < N; i++)
            {
                lamudamax = lamudamax+rwmatirx[i] / wvaule[i];
            }

            lamudamax = lamudamax / N;
           
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
                    rowsum[i] = rowsum[i] *a_b[i, j];
                }
                wvaule[i] = Math.Pow(rowsum[i], (1.0/N));
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

        void memoEdit1_TextChanged(object sender, EventArgs e)
        {
            BeginInvoke(new MethodInvoker(delegate
            {
                SetSelection();
            }));
        }
        private void SetSelection()
        {
            memoEdit1.SelectionStart = memoEdit1.Text.Length;
            memoEdit1.ScrollToCaret();
        }

        private void dockPanel1_Click(object sender, EventArgs e)
        {
            dockPanel1.Hide();
        }


    }
}
