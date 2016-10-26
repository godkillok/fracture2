using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Data;
using System.Xml;
using System.IO;
namespace fracture
{
    class algorithm
    {


        public void Compertz(int math_predic_num, double[] GMData, double[,] Compertz_reuslt, List<double> parameter_result)
        {
            double a, b, c;
            int ii, k, i, sizegmdata;

            sizegmdata = GMData.GetLength(0);//sizegmdata = UBound(GMData)
            double[] x0 = new double[sizegmdata];
            double[] x1 = new double[sizegmdata];
            double[] z1 = new double[sizegmdata - 2];
            double[] z2 = new double[sizegmdata - 2];
            double[] z3 = new double[sizegmdata - 2];
            double[] z4 = new double[sizegmdata - 2];

            double[,] yn = new double[sizegmdata - 2, 1];
            double[,] bmatrix = new double[sizegmdata - 2, 4];
            double[] n0 = new double[sizegmdata];
            double[,] bcresult = new double[2, 1];
            //    ReDim Compertz_reuslt(UBound(GMData) + Val(Form3.math_predic_num.Text), 1)
            //Dim bmatrixT2() As Double
            //Dim bmatrixT() As Double



            for (i = 0; i < sizegmdata; i++)

                x0[i] = System.Math.Log(GMData[i] + 0.001);


            //x0.CopyTo(x1, 0);
            x1 = (double[])x0.Clone();

            for (k = 1; k < sizegmdata; k++)
                x1[k] = x1[k - 1] + x0[k];//'累加生成

            for (k = 0; k < sizegmdata - 2; k++)
            {
                z1[k] = x1[k + 2];
                z4[k] = 1;
            }

            for (i = 0; i < sizegmdata - 2; i++)
                z2[i] = i + 2;

            for (i = 0; i < sizegmdata - 2; i++)
            {
                ii = i % 2;
                if (ii == 0)
                    z3[i] = 1;
                else
                    z3[i] = 0;
            }

            for (i = 0; i < sizegmdata - 2; i++)
                yn[i, 0] = x0[i + 1] - x1[i + 1];

            for (i = 0; i < sizegmdata - 2; i++)
            {
                bmatrix[i, 0] = z1[i];
                bmatrix[i, 1] = z2[i];
                bmatrix[i, 2] = z3[i];
                bmatrix[i, 3] = z4[i];
            }

            //' au0=inv(B'*B)*B'*YN %解出参数（a,u）
            //''-----------------au0=inv(B'*B)*B'*YN
            int M, N, L;
            M = bmatrix.GetLength(1);
            N = bmatrix.GetLength(0);
            double[,] bmatrixT = new double[M, N];
            //'计算矩阵bmatrix的转置bmatrixT
            bmatrixT = Matrix_Transpotation(bmatrix);
            //'计算矩阵bmatrix*bmatrixT= bmatrixTbmatrix
            M = bmatrixT.GetLength(0);
            L = bmatrix.GetLength(1);
            double[,] bmatrixTbmatrix = new double[M, L];
            bmatrixTbmatrix = Matrix_Multiplication(bmatrixT, bmatrix);
            if (Matrix_Inversion(bmatrixTbmatrix) == false)// '矩阵bmatrixTbmatrix求逆
            {
                MessageBox.Show("求解失败！");
                return;
            }
            //      //'inv(B'*B)*B'
            M = bmatrixTbmatrix.GetLength(0);
            L = bmatrixT.GetLength(1);
            double[,] bmatrixT2 = new double[M, L];
            bmatrixT2 = Matrix_Multiplication(bmatrixTbmatrix, bmatrixT);

            M = bmatrixT2.GetLength(0);
            L = yn.GetLength(1);
            double[,] au = new double[M, L];
            au = Matrix_Multiplication(bmatrixT2, yn);
            //      //'au0= inv(B'*B)*B'*yn
            //''-----------------au0=inv(B'*B)*B'*YN
            //'求解参数 a

            a = au[0, 0];
            a = System.Math.Log(-a) / 2;
            //'求解参数 b,c
            //'主要通过线性拟合N(0)(t)=b*exp(-a*t)+c得到参数
            for (i = 0; i < sizegmdata; i++)
                n0[i] = System.Math.Exp(-a * (i + 1));

            bcresult = polyfit(n0, x0);// ' x, y, 参数列表 y=bx+c

            b = bcresult[1, 0];
            c = bcresult[0, 0];

            for (i = 0; i < sizegmdata + math_predic_num; i++)//int.Parse(math_predic_num.Text)
            {
                Compertz_reuslt[i, 0] = i;
                Compertz_reuslt[i, 1] = System.Math.Exp(c + b * System.Math.Exp(-a * (i + 1)));
            }

            parameter_result.Add(a);
            parameter_result.Add(b);
            parameter_result.Add(c);
        }
        public void GM1_1_Predict(int math_predic_num, double[] GMData, double[,] result, List<double> parameter_result) //'Data是原始序列字符,以逗号","分开
        {
            int sizegmdata = GMData.GetLength(0);//sizegmdata = UBound(GMData)
            double[] X_0 = new double[sizegmdata];
            double[] X_1 = new double[sizegmdata];
            double[,] yn = new double[sizegmdata - 1, 1];
            double[,] b = new double[sizegmdata - 1, 2];

            for (int i = 0; i < sizegmdata; i++)
                X_0[i] = GMData[i];

            Calculate_1_AGO(X_0, X_1); //'做一次累加生成X_1
            Calculate_Matrix_YN(X_0, yn);// '计算矩阵YN
            Calculate_Matrix_B(X_1, b); //'计算矩阵B

            int M, N, L;
            M = b.GetLength(1);
            N = b.GetLength(0);
            double[,] bT = new double[M, N];
            //'计算矩阵B的转置BT
            bT = Matrix_Transpotation(b);
            //'计算矩阵bmatrix*bmatrixT= bmatrixTbmatrix
            M = bT.GetLength(0);
            L = b.GetLength(1);
            double[,] BTB = new double[M, L];
            BTB = Matrix_Multiplication(bT, b);//'矩阵BT×B=BTB
            if (Matrix_Inversion(BTB) == false)// '矩阵bmatrixTbmatrix求逆
            {
                MessageBox.Show("求解失败！");
                return;
            }


            M = BTB.GetLength(0);
            L = bT.GetLength(1);
            double[,] BTBBT = new double[M, L];
            BTBBT = Matrix_Multiplication(BTB, bT);// '矩阵BTB×BT = BTBBT
            M = BTBBT.GetLength(0);
            L = yn.GetLength(1);
            double[,] a = new double[M, L];
            a = Matrix_Multiplication(BTBBT, yn);//'矩阵BTBBT×YN = A,A(1, 0)=u,A(0, 0)=a

            Predicted_Value(math_predic_num, X_1[0], a[1, 0], a[0, 0], sizegmdata, result, parameter_result); //'预测
        }
        public void HCZ(int math_predic_num, double[,] GMData, double[,] HCZ_result, List<double> parameter_result) //'Data是原始序列字符,以逗号","分开
        {
            int sizegmdata = GMData.GetLength(0);//sizegmdata = UBound(GMData)
            double[] X_0 = new double[sizegmdata];
            double[] X_1 = new double[sizegmdata];
            double[,] yn = new double[sizegmdata - 1, 1];
            double[,] bcresult = new double[2, 1];

            double capital_a, capital_b, small_a, small_b, HCZ_Nr;
            int num_time = GMData.GetLength(0);
            double[] temp_x = new double[num_time];
            double[] temp_y = new double[num_time];

            for (int i = 0; i < num_time; i++)
            {
                temp_y[i] = System.Math.Log(GMData[i, 3] / GMData[i, 1]) / System.Math.Log(10); //'log(q/Np)
                temp_x[i] = i + 1;
            }

            //'线性拟合log(Q / np) = a - BT
            bcresult = polyfit(temp_x, temp_y);// ' x, y, 参数列表 y=bx+c

            capital_b = -bcresult[1, 0];
            capital_a = bcresult[0, 0];
            small_a = System.Math.Pow(10, capital_a);
            small_b = 2.303 * capital_b;

            for (int i = 0; i < num_time; i++)
            {
                temp_y[i] = System.Math.Log(GMData[i, 1]) / System.Math.Log(10);// 'log(q/Np)
                temp_x[i] = System.Math.Exp(-small_b * (i + 1));
            }


            //'线性拟合log(Np) = afa - Beta*exp(-bt)
            bcresult = polyfit(temp_x, temp_y);// ' x, y, 参数列表 y=bx+c
            double afa = bcresult[0, 0];
            HCZ_Nr = System.Math.Pow(10, afa);
            //double[,] HCZ_result = new double[num_time + math_predic_num, 2];

            for (int i = 0; i < num_time + math_predic_num; i++)
            {
                HCZ_result[i, 0] = i + 1;
                HCZ_result[i, 1] = HCZ_Nr * System.Math.Exp(-(small_a / small_b) * System.Math.Exp(-small_b * (i + 1)));
            }

            parameter_result.Add(HCZ_Nr);
            parameter_result.Add(small_a);
            parameter_result.Add(small_b);
        }
        public double[] greyrela(double[,] x0, double fenbian = 0.5)
        {
            //        function output=grayrela()

            //%参考因子与比较因子共同存储在一个矩阵x0中,参考因子位于第一列
            int num_sample = x0.GetLength(0);
            int num_attribute = x0.GetLength(1);
            double[,] x1 = new double[num_sample, num_attribute];
            double[,] x2 = new double[num_sample, num_attribute];

            int[] pos = new int[num_sample];
            double[] kmatrix = new double[num_sample];
            double[,] dist_0i = new double[num_sample, num_attribute - 1];
            double[,] x2_sorted = new double[num_sample, num_attribute];
            double[,] coef_rela = new double[num_sample, num_attribute];
            double[] output = new double[num_attribute];
            double[] sig_j = new double[num_attribute];
            ////%斜率序列
            ////for (int i = 1; i < num_sample; i++)
            ////    for (int j = 0; j < num_attribute; j++)
            ////        x1[i, j] = x0[i, j] - x0[i - 1, j];


            ////%标准化

            //for (int i = 0; i < num_attribute; i++)
            //    for (int j = 0; j < num_sample; j++)
            //        x2[j, i] = x1[j, i] / std(x1, i);

            ////%排序
            //pos = sort(x2, 0);
            ////x2_sorted=x2(pos,:);
            //for (int i = 0; i < num_sample; i++)
            //    for (int j = 0; j < num_attribute; j++)
            //    {
            //        x2_sorted[i, j] = x2[pos[i], j];
            //    }
            ////% 判定关联性质
            ////n=length(x1(:,1));
            //for (int i = 0; i < num_sample; i++)
            //    kmatrix[i] = i+1;
            //for (int j = 0; j < num_attribute; j++)
            //    sig_j[j] = qiuhe(Matrix_Multiplication(kmatrix, x2_sorted, j)) - qiuhe(x2_sorted, j) * qiuhe(kmatrix) / num_sample;


            ////%caculation of distantion
            //  for (int j = 1; j < num_attribute; j++)
            //        for (int i = 0; i < num_sample; i++)
            //    dist_0i[i,j]=System.Math.Abs(Matrix_sign(sig_j,sig_j,j,0)*x2_sorted[i,j]-x2_sorted[i,0]);
            ////‘dist_0i(:,j)=abs(sign(sig_j(:,j)./sig_j(:,1)).*x2_sorted(:,j)-x2_sorted(:,1));



            ////%标准化
            for (int i = 0; i < num_attribute; i++)//从因素列开始
                for (int j = 0; j < num_sample; j++)
                    x2[j, i] = (x0[j, i] - avg(x0, i)) / std(x0, i);//zscore
            //x2[j, i] = (x0[j, i] - Matrix_min(x0, i)) / (Matrix_max(x0, i) - Matrix_min(x0, i));//
            // x2[j, i] = x0[j, i] / x0[0, i];//均一化

            //Xi 与X0 的绝对
            for (int j = 1; j < num_attribute; j++)
                for (int i = 0; i < num_sample; i++)
                    dist_0i[i, j - 1] = System.Math.Abs(x2[i, j] - x2[i, 0]);

            ////%计算关联系数
            for (int i = 0; i < num_sample; i++)
                for (int j = 1; j < num_attribute; j++)
                    coef_rela[i, j] = (Matrix_min(dist_0i) + (Matrix_max(dist_0i) * fenbian)) / (dist_0i[i, j - 1] + (Matrix_max(dist_0i) * fenbian));


            for (int j = 0; j < num_attribute; j++)
                output[j] = qiuhe(coef_rela, j) / num_attribute;

            return output;
        }
        public void Usher(int math_predic_num, int LoopCount, double fluid_type, double[,] PsoScope, double[] GMData, double[,] psoresult, List<double> parameter_result)
        {

            //MessageBox.Show(trynum.ToString());
        }
        /// <summary>
        /// 相关系数,或者拟合度，要求两个集合数量必须相同
        /// </summary>
        /// <param name="decdata">数组一</param>
        /// <param name="decresult">数组二</param>
        /// <returns></returns>
        private double correl(double[,] psodata, double[,] psoresult, int time_num)
        {
            List<double> decdata = new List<double>();
            List<double> decresult = new List<double>();
            for (int i = 0; i < time_num; i++)
            {
                decdata.Add(psodata[i, 1]);
                decresult.Add(psoresult[i, 1]);
            }

            int realdatanum = decdata.Count;

            //数组一
            double avg1 = average(decdata, realdatanum);
            //数组二
            double avg2 = average(decresult, realdatanum);

            double sumfenzi = 0;
            double sumfenmu_x = 0;
            double sumfenmu_y = 0;
            for (int i = 0; i < realdatanum; i++)
            {
                sumfenzi = sumfenzi + ((decdata[i] - avg1)) * ((decresult[i] - avg2));
                sumfenmu_x = sumfenmu_x + (decdata[i] - avg1) * (decdata[i] - avg1);
                sumfenmu_y = sumfenmu_y + (decresult[i] - avg2) * (decresult[i] - avg2);
            }
            double sumfenmu = System.Math.Pow(sumfenmu_x, 0.5) * System.Math.Pow(sumfenmu_y, 0.5);
            double cor = sumfenzi / sumfenmu;
            cor = cor * cor;
            return cor;
        }
        /// <summary>
        /// 求出数据平均值,并保留三位小数
        /// </summary>
        /// <param name="Valist">数据集合</param>
        /// <returns></returns>
        private double average(List<double> Valist, int realdatanum)
        {
            double sum = 0;
            for (int i = 0; i < realdatanum; i++)
            {
                sum = sum + Valist[i];
            }
            double revl = sum / realdatanum;
            return revl;
        }
        private void Predicted_Value(int math_predic_num, double X_1_0, double u_value, double a_value, int k, double[,] result, List<double> parameter_result)
        {
            //Predicted_Value(X_1[0], a[1, 0], a[0, 0], sizegmdata, result, parameter_result); //'预测
            int i;
            for (i = 1; i < k + 1 + math_predic_num; i++)
            {
                result[i - 1, 1] = (X_1_0 - u_value / a_value) * System.Math.Exp(-a_value * (i - 1)) * (1 - System.Math.Exp(a_value));
                result[i - 1, 0] = i;
            }
            result[0, 1] = X_1_0;
            parameter_result.Add(X_1_0);
            parameter_result.Add(u_value);
            parameter_result.Add(a_value);
        }
        private void Calculate_1_AGO(double[] X_0, double[] X_1)
        {
            //'做一次累加生成 1-AGO

            int i, k;
            double TempX = 0;
            k = X_0.GetLength(0);
            for (i = 0; i < k; i++)
            {
                TempX = TempX + X_0[i];
                X_1[i] = TempX;
            }

        }
        private void Calculate_Matrix_YN(double[] X_0, double[,] yn)
        {
            //'计算数据矩阵YN
            int i, k;
            k = X_0.GetLength(0) - 1;
            for (i = 0; i < k; i++)
                yn[i, 0] = X_0[i + 1];
        }
        private void Calculate_Matrix_B(double[] X_1, double[,] b)
        {
            //'计算数据矩阵B
            int i, k;
            k = X_1.GetLength(0) - 1;
            for (i = 0; i < k; i++)
            {
                b[i, 0] = -0.5 * (X_1[i] + X_1[i + 1]);
                b[i, 1] = 1;
            }
        }
        private double[,] Matrix_Transpotation(double[,] mtxA)
        {
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            // '  函数名：Matrix_Transpotation
            // '  功能：  计算矩阵的转置transpotation
            // '  参数：  m   - Integer型变量，矩阵的行数
            // '          n   - Integer型变量，矩阵的列数
            // '          mtxA  - Double型m x n二维数组，存放原矩阵
            // '          mtxAT  - Double型n x m二维数组，返回转置矩阵
            // ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            int i, j, M, N;
            M = mtxA.GetLength(1);
            N = mtxA.GetLength(0);
            double[,] mtxAT = new double[M, N];
            for (i = 0; i < M; i++)
                for (j = 0; j < N; j++)
                    mtxAT[i, j] = mtxA[j, i];
            return mtxAT;
        }
        private double[,] Matrix_Multiplication(double[,] mtxA, double[,] mtxB)
        {
            //    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            //'  函数名：Matrix_Multiplication
            //'  功能：  计算矩阵的乘法multiplication
            //'  参数：  m   - Integer型变量，相乘的左边矩阵的行数
            //'          n   - Integer型变量，相乘的左边矩阵的列数和右边矩阵的行数
            //'          l   -  Integer型变量，相乘的右边矩阵的列数
            //'          mtxA  - Double型m x n二维数组，存放相乘的左边矩阵
            //'          mtxB  - Double型n x l二维数组，存放相乘的右边矩阵
            //'          mtxC  - Double型m x l二维数组，返回矩阵乘积矩阵
            //''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            int j, i, k, N, M, L;


            M = mtxA.GetLength(0);
            N = mtxB.GetLength(0);
            L = mtxB.GetLength(1);
            double[,] mtxC = new double[M, L];


            for (i = 0; i < M; i++)
            {
                for (j = 0; j < L; j++)
                {
                    mtxC[i, j] = 0;
                    for (k = 0; k < N; k++)
                        mtxC[i, j] = mtxC[i, j] + mtxA[i, k] * mtxB[k, j];
                }
            }
            return mtxC;
        }
        private bool Matrix_Inversion(double[,] mtxA)
        {

            //''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            //'  函数名：Matrix_Inversion
            //'  功能：  矩阵求逆
            //'  参数：  n      - Integer型变量，矩阵的阶数
            //'          mtxA   - Double型二维数组，体积为n x n。存放原矩阵A；返回时存放其逆矩阵A-1。
            //'  返回值：Boolean型，失败为False，成功为True
            //'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            //' 局部变量
            int N;

            bool Matrix_Inversion;
            N = mtxA.GetLength(0);
            int[] nIs = new int[N];
            int[] nJs = new int[N];
            int j, i, k;

            double d, P;
            d = 0;
            //    //' 全选主元，消元
            for (k = 0; k < N; k++)
            {
                d = 0.0;
                for (i = k; i < N; i++)
                {
                    for (j = k; j < N; j++)
                    {
                        P = System.Math.Abs(mtxA[i, j]);

                        if (P > d)
                        {
                            d = P;
                            nIs[k] = i;
                            nJs[k] = j;
                        }
                    }
                }
                //        //' 求解失败
                if (d + 1.0 == 1.0)
                {
                    Matrix_Inversion = false;

                    return Matrix_Inversion;

                }

                if (nIs[k] != k)
                {
                    for (j = 0; j < N; j++)
                    {
                        P = mtxA[k, j];
                        mtxA[k, j] = mtxA[nIs[k], j];
                        mtxA[nIs[k], j] = P;
                    }

                }

                if (nJs[k] != k)
                {
                    for (i = 0; i < N; i++)
                    {
                        P = mtxA[i, k];
                        mtxA[i, k] = mtxA[i, nJs[k]];
                        mtxA[i, nJs[k]] = P;
                    }

                }

                mtxA[k, k] = 1 / mtxA[k, k];

                for (j = 0; j < N; j++)

                    if (j != k)
                        mtxA[k, j] = mtxA[k, j] * mtxA[k, k];


                for (i = 0; i < N; i++)
                {
                    if (i != k)
                    {
                        for (j = 0; j < N; j++)
                        {
                            if (j != k)
                                mtxA[i, j] = mtxA[i, j] - mtxA[i, k] * mtxA[k, j];
                        }

                    }
                }


                for (i = 0; i < N; i++)
                    if (i != k)
                        mtxA[i, k] = -mtxA[i, k] * mtxA[k, k];

            }
            //    //' 调整恢复行列次序
            for (k = N - 1; k >= 0; k--)
            {
                if (nJs[k] != k)
                    for (j = 0; j < N; j++)
                    {
                        P = mtxA[k, j];
                        mtxA[k, j] = mtxA[nJs[k], j];
                        mtxA[nJs[k], j] = P;
                    }

                if (nIs[k] != k)
                    for (i = 0; i < N; i++)
                    {
                        P = mtxA[i, k];
                        mtxA[i, k] = mtxA[i, nIs[k]];
                        mtxA[i, nIs[k]] = P;
                    }
            }

            //' 求解成功
            Matrix_Inversion = true;
            return Matrix_Inversion;
        }
        /// <summary>
        /// 线性拟合，y=b[0,1]x+b[0,0];polyfit(x,y)
        /// </summary>
        /// <param name="n0"></param>
        /// <param name="X"></param>
        /// <returns></returns>
        public double[,] polyfit(double[] n0, double[] X)
        {
            //      '''''''''''''''''''''''''''''''''''''''''''''''''
            //''''''''''''''以下是一元线性回归预测'''''''''''''''''''''''
            //'''''''''''''''''''''''''''''''''''''''''''''''''
            int i, N, M;
            N = n0.GetLength(0);
            M = X.GetLength(0);
            double[,] mtx0 = new double[N, 2];
            double[,] mtxx = new double[M, 1];


            for (i = 0; i < N; i++)
            {
                mtx0[i, 0] = 1;
                mtx0[i, 1] = n0[i];
            }

            for (i = 0; i < M; i++)
                mtxx[i, 0] = X[i];
            M = mtx0.GetLength(1);
            N = mtx0.GetLength(0);
            double[,] mtx0T = new double[M, N];

            mtx0T = Matrix_Transpotation(mtx0); //'计算矩阵X的转置XT

            M = mtx0T.GetLength(0);
            N = mtx0.GetLength(1);
            double[,] mtx0Tmtx0 = new double[M, N];

            mtx0Tmtx0 = Matrix_Multiplication(mtx0T, mtx0); //'矩阵XT×X=XTX

            if (Matrix_Inversion(mtx0Tmtx0) == false)// '矩阵BTB求逆,求逆后也是BTB
            {
                MessageBox.Show("求解失败！");

            }
            M = mtx0Tmtx0.GetLength(0);
            N = mtx0T.GetLength(1);
            double[,] mtx0Tmtx0mtx0T = new double[M, N];
            mtx0Tmtx0mtx0T = Matrix_Multiplication(mtx0Tmtx0, mtx0T);//'矩阵 XTX×XT = XTXXT
            M = mtx0Tmtx0mtx0T.GetLength(0);
            N = mtxx.GetLength(1);
            double[,] bcresult = new double[M, N];
            bcresult = Matrix_Multiplication(mtx0Tmtx0mtx0T, mtxx);// '矩阵 XTXXT×Y = B ,A(1, 0)=u,A(0, 0)=a
            return bcresult;
        }
        public int[] C_SelfAdapt_fcm(double[,] cluster_data)
        {
            //'[U_matrix,V_matrix,L_matrix,bestc]=C_SelfAdapt_fcm(cluster_data)
            int num_sample = cluster_data.GetLength(0);//sizegmdata = UBound(GMData)
            int num_attribute = cluster_data.GetLength(1);//sizegmdata = UBound(GMData)
            int i, j;
            int c_num;

            //'' initialization
            double[] L_matrix = new double[num_sample - 1];
            double[,] xba = new double[1, num_attribute];
            int[] guilei = new int[num_sample];
            for (i = 0; i < num_sample - 1; i++)
                L_matrix[i] = 0;

            //'' 主体循环
            double[,] Ulast = new double[num_sample - 1, num_sample];
            double[,] Vlast = new double[num_sample - 1, num_attribute];

            for (c_num = 2; c_num < num_sample - 1; c_num++)
            {
                double[,] U_matrix = new double[c_num, num_sample];
                double[,] V_matrix = new double[c_num, num_attribute];

                std_fcm(U_matrix, V_matrix, cluster_data, c_num);
                for (int k = 0; k < num_attribute; k++)
                    xba[0, k] = 0;

                for (i = 0; i < c_num; i++)
                    for (j = 0; j < num_sample; j++)
                        for (int k = 0; k < num_attribute; k++)
                            xba[0, k] = xba[0, k] + System.Math.Pow(U_matrix[i, j], 2) * cluster_data[j, k];


                for (int k = 0; k < num_attribute; k++)
                    xba[0, k] = xba[0, k] / num_sample;


                double fenzi = 0;

                for (i = 0; i < c_num; i++)
                {
                    fenzi = fenzi + sumfunc2(i, U_matrix) * System.Math.Pow(ED(V_matrix, i, xba, 0), 2);
                    double tems = sumfunc2(i, U_matrix);
                    tems = ED(V_matrix, i, xba, 0);
                }

                fenzi = fenzi / (c_num - 1);
                double fenmu = 0;

                for (i = 0; i < c_num; i++)
                    for (j = 0; j < num_sample; j++)
                        fenmu = fenmu + System.Math.Pow(U_matrix[i, j], 2) * System.Math.Pow(ED(cluster_data, j, V_matrix, i), 2);


                fenmu = fenmu / (num_sample - c_num);
                L_matrix[c_num - 1] = fenzi / fenmu;

                if (c_num > 2)
                {
                    if ((L_matrix[c_num - 2] > L_matrix[c_num - 3]) & (L_matrix[c_num - 2] > L_matrix[c_num - 1]))
                    {
                        //            'ReDim Ulast(UBound(U_matrix, 1), UBound(U_matrix, 2)) As Double
                        for (int i_U = 0; i_U < c_num; i_U++)
                            for (int j_U = 0; j_U < num_attribute; j_U++)
                            {
                                U_matrix[i_U, j_U] = Ulast[i_U, j_U];
                                V_matrix[i_U, j_U] = Vlast[i_U, j_U];
                            }

                        c_num = c_num - 1;

                        break;

                    }
                }

                for (int i_U = 0; i_U < c_num; i_U++)
                    for (int j_U = 0; j_U < num_attribute; j_U++)
                    {
                        Ulast[i_U, j_U] = U_matrix[i_U, j_U];
                        Vlast[i_U, j_U] = V_matrix[i_U, j_U];
                    }


                for (j = 0; j < num_sample; j++)
                {
                    double maxu;
                    maxu = U_matrix[0, j];
                    guilei[j] = 0;

                    for (i = 0; i < c_num; i++)
                    {
                        if (maxu < U_matrix[i, j])
                        {
                            maxu = U_matrix[i, j];
                            guilei[j] = i;
                        }

                    }
                }
            }

            int bestc = c_num;
            int Index = 1;

            while (L_matrix[Index] != 0)
                Index = Index + 1;
            double[] L_matrix_final = new double[Index];


            for (i = 0; i < Index; i++)
                L_matrix_final[i] = L_matrix[i];
            int cc = bestc;
            return guilei;
            //C_SelfAdapt_fcm = Array(U_matrix, V_matrix, L_matrix, bestc, guilei)
        }
        private void std_fcm(double[,] U_matrix, double[,] V_matrix, double[,] cluster_data, int c_num)
        {
            int j, i, S, t;
            int num_sample = cluster_data.GetLength(0);
            int num_attribute = cluster_data.GetLength(1);
            double epsilon = 0.001;
            int Iteration = 1;
            double temp_denominator;

            double[] U_temp = new double[num_sample];

            double[] temp_numerator = new double[num_attribute];
            double[,] new_U = new double[c_num, num_sample];
            double[,] U_abs = new double[c_num, num_sample];

            Random Rnd = new Random();
            //            double[,] U_matrix22 = new double[2, 8]{{0.466756814	,	0.431218447	,	0.402329868	,	0.856251116	,	0.373578783	,	0.218994118	,	0.433422713	,	0.070449632	},
            //{0.254007851	,	0.702530011	,	0.181840079	,	0.584201225	,	0.221694547	,	0.522232443	,	0.741303988	,	0.847333436	}};

            for (i = 0; i < c_num; i++)
                for (j = 0; j < num_sample; j++)
                {
                    U_matrix[i, j] = Rnd.NextDouble();
                    //U_matrix[i, j] = U_matrix22[i, j];
                }//U_matrix2[i, j];


            for (i = 1; i < c_num; i++)
                for (j = 1; j < num_attribute; j++)
                    V_matrix[i, j] = 0;

            //'' 主体循环
            while (true)
            {
                //    ' calculate new V
                for (S = 0; S < c_num; S++)
                {
                    for (int num_k = 0; num_k < num_attribute; num_k++)
                        temp_numerator[num_k] = 0;

                    for (int k = 0; k < num_sample; k++)
                        for (int num_k = 0; num_k < num_attribute; num_k++)
                        {
                            temp_numerator[num_k] = temp_numerator[num_k] + System.Math.Pow(U_matrix[S, k], 2) * cluster_data[k, num_k];
                        }

                    for (int num_k = 0; num_k < num_attribute; num_k++)
                        V_matrix[S, num_k] = temp_numerator[num_k] / sumfunc2(S, U_matrix);

                }

                //    ' calculat new U
                for (S = 0; S < c_num; S++)
                {
                    for (t = 0; t < num_sample; t++)
                    {
                        temp_denominator = 0;

                        for (j = 0; j < c_num; j++)
                            temp_denominator = temp_denominator + System.Math.Pow((ED(V_matrix, S, cluster_data, t) / ED(V_matrix, j, cluster_data, t)), 2);// '( ED(V(s, to ),X(t, to ))/ED(V(j, to ),X(t, to )) )^2


                        new_U[S, t] = System.Math.Pow(temp_denominator, -1);
                    }
                }

                //    ReDim As Double
                for (i = 0; i < c_num; i++)
                    for (j = 0; j < num_sample; j++)

                        //            ' 主体循环终止条件
                        U_abs[i, j] = System.Math.Abs(U_matrix[i, j] - new_U[i, j]);


                double maxu = U_abs[0, 0];
                for (i = 0; i < c_num; i++)
                    for (j = 0; j < num_sample; j++)
                    {
                        if (maxu < U_abs[i, j])
                            maxu = U_abs[i, j];
                    }


                if (maxu < epsilon)
                    break;

                for (i = 0; i < c_num; i++)
                {
                    for (j = 0; j < num_sample; j++)
                        U_matrix[i, j] = new_U[i, j];

                    Iteration = Iteration + 1;
                }
            }

            //std_fcm = Array(U_matrix, V_matrix, Iteration)
        }
        private double summatrix(double[,] U_matrix, int S)
        {
            int U_sample = U_matrix.GetLength(0);
            double sumresult = 0;
            for (int i = 0; i < U_sample; i++)
                sumresult = sumresult + U_matrix[i, S];
            return sumresult;
        }
        private double sumfunc2(int S, double[,] U_matrix)
        {
            //'function sumresult=sumfunc(s,U_matrix)
            //'计算向量的平方
            double sumresult = 0;
            int U_sample = U_matrix.GetLength(0);
            int U_attribute = U_matrix.GetLength(1);

            for (int i = 0; i < U_attribute; i++)

                sumresult = sumresult + System.Math.Pow(U_matrix[S, i], 2);


            return sumresult;
        }
        private double ED(double[,] V_matrix, int S, double[,] X_matrix, int t)
        {
            //'function dresult = ED(V_matrix,s,X_matrix,t)
            int V_attribute = X_matrix.GetLength(1);


            double dresult = 0;
            for (int i = 0; i < V_attribute; i++)
                dresult = dresult + System.Math.Pow((V_matrix[S, i] - X_matrix[t, i]), 2);
            dresult = System.Math.Pow(dresult, 0.5);
            return dresult;
        }
        //% 其中：
        private double std(double[,] xmatrix, int s)
        {
            double avg = 0;
            double stdresult = 0;
            int num_sample = xmatrix.GetLength(0);
            for (int i = 0; i < num_sample; i++)
                stdresult = stdresult + xmatrix[i, s];
            avg = stdresult / num_sample;
            stdresult = 0;
            for (int i = 0; i < num_sample; i++)
                stdresult = stdresult + System.Math.Pow((xmatrix[i, s] - avg), 2);
            stdresult = stdresult / (num_sample - 1);
            stdresult = System.Math.Pow((stdresult), 0.5);
            return stdresult;
        }
        private double avg(double[,] xmatrix, int s)
        {
            double avg = 0;
            double stdresult = 0;
            int num_sample = xmatrix.GetLength(0);
            for (int i = 0; i < num_sample; i++)
                stdresult = stdresult + xmatrix[i, s];
            avg = stdresult / (num_sample);
            return avg;
        }
        private double qiuhe(double[] input)
        {
            //function output=qiuhe(input)

            double output = 0;

            for (int i = 0; i < input.Length; i++)
                output = output + input[i];

            return output;
            //% 计算方法参考文献：王宁练：冰川平衡线变化的主导气候因子灰色关联分析 冰川冻土
        }
        private double qiuhe(double[,] input, int s)
        {
            //function output=qiuhe(input)

            double output = 0;

            for (int i = 0; i < input.GetLength(0); i++)
                output = output + input[i, s];

            return output;
            //% 计算方法参考文献：王宁练：冰川平衡线变化的主导气候因子灰色关联分析 冰川冻土
        }
        private int[] sort(double[,] arrmatrix, int s)
        {
            double[] arr = new double[arrmatrix.GetLength(0)];
            int i, j, temploc;
            double temp;
            bool done = false;
            int[] location = new int[arr.Length];
            for (i = 0; i < arr.GetLength(0); i++)
            {
                arr[i] = arrmatrix[i, s];
                location[i] = i;
            }
            j = 1;
            while ((j < arr.Length) && (!done))//判断长度    
            {
                done = true;
                for (i = 0; i < arr.Length - j; i++)
                {
                    if (arr[i] > arr[i + 1])
                    {
                        done = false;
                        temp = arr[i];
                        arr[i] = arr[i + 1];//交换数据    
                        arr[i + 1] = temp;
                        temploc = location[i];
                        location[i] = location[i + 1];//交换位置
                        location[i + 1] = temploc;
                    }
                }
                j++;
            }
            return location;
        }
        /// <summary>
        /// 排序算法，按从大到小顺序
        /// /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns> 返回排序结果和下标的排序
        private int[] sort(List<double> arr)
        {
            //double[] arr = new double[arrmatrix.Count];
            int i, j, temploc;
            double temp;
            bool done = false;
            int[] location = new int[arr.Count];
            for (i = 0; i < arr.Count; i++)
            {
                //arr[i] = arrmatrix[i];
                location[i] = i;
            }
            j = 1;
            while ((j < arr.Count) && (!done))//判断长度    
            {
                done = true;
                for (i = 0; i < arr.Count - j; i++)
                {
                    if (arr[i] > arr[i + 1])
                    {
                        done = false;
                        temp = arr[i];
                        arr[i] = arr[i + 1];//交换数据    
                        arr[i + 1] = temp;
                        temploc = location[i];
                        location[i] = location[i + 1];//交换位置
                        location[i + 1] = temploc;
                    }
                }
                j++;
            }
            return location;
        }
        /// <summary>距离平方反比加权插值法</summary>
        /// <param name="x">已知点列的X坐标</param>
        /// <param name="y">已知点列的Y坐标</param>
        /// <param name="z">已知点列的值</param>
        /// <param name="z">插值点列的X坐标</param>
        /// <param name="z">插值点列的Y坐标</param>
        /// <returns>插值结果</returns>
        public List<double> Interpolate(List<double> x, List<double> y, List<double> z, List<double> X, List<double> Y)
        {
            if (x.Count < 3 || x.Count != y.Count || x.Count != z.Count || X.Count != Y.Count)
                return null;

            int m0 = x.Count;
            int m1 = X.Count;
            int weight = -2;
            List<double> Z = new List<double>();

            //距离列表
            int r2 = x.Count;
            for (int i = 0; i < m1; i++)
            {
                List<double> vcc = new List<double>();
                List<double> r = new List<double>();
                for (int j = 0; j < m0; j++)
                {
                    double tmpDis = Math.Sqrt(Math.Pow(X[i] - x[j], 2) + Math.Pow(Y[i] - y[j], 2));
                    r.Add(tmpDis);
                }
                int[] rid = new int[r.Count];
                rid = sort(r);
                for (int j = 0; j < r.Count; j++)
                {
                    vcc.Add(z[rid[j]]);
                }
                if (r[0] == 0)
                {
                    Z.Add(vcc[0]);
                }
                else
                {
                    Z.Add(matrix_sum(matrix_point_multi(vcc, maxtrix_pow(r, weight), r2), r2) / (matrix_sum(maxtrix_pow(r, weight), r2)));
                }
            }
            return Z;
        }

        private List<double> maxtrix_pow(List<double> maxtix, int weight)
        {
            List<double> maxtix2 = new List<double>();
            for (int i = 0; i < maxtix.Count; i++)
            {
                maxtix2.Add(System.Math.Pow(maxtix[i], weight));
            }
            return maxtix2;
        }
        private List<double> matrix_point_multi(List<double> maxtix1, List<double> maxtix2, int r2)
        {
            List<double> maxtix3 = new List<double>();
            for (int i = 0; i < r2; i++)
            {

                maxtix3.Add(maxtix1[i] * maxtix2[i]);
            }
            return maxtix3;
        }
        private double matrix_sum(List<double> maxtix, int r2)
        {
            double sum = 0;
            for (int i = 0; i < r2; i++)
            {

                sum = sum + maxtix[i];
            }
            return sum;
        }
        private double[] Matrix_Multiplication(double[] mtxA, double[,] mtxB, int s)
        {
            //    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            //'  函数名：Matrix_Multiplication
            //'  功能：  计算矩阵的乘法multiplication
            //'  参数：  m   - Integer型变量，相乘的左边矩阵的行数
            //'          n   - Integer型变量，相乘的左边矩阵的列数和右边矩阵的行数
            //'          l   -  Integer型变量，相乘的右边矩阵的列数
            //'          mtxA  - Double型m x n二维数组，存放相乘的左边矩阵
            //'          mtxB  - Double型n x l二维数组，存放相乘的右边矩阵
            //'          mtxC  - Double型m x l二维数组，返回矩阵乘积矩阵
            //''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            int i, N, M, L;

            M = mtxA.Length;
            N = mtxB.GetLength(0);
            L = mtxB.GetLength(1);
            double[] mtxC = new double[M];

            for (i = 0; i < M; i++)
            {
                mtxC[i] = mtxA[i] * mtxB[i, s];
            }

            return mtxC;
        }
        private double Matrix_sign(double[] mtxA, double[] mtxB, int frist, int second)
        {
            //    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            //'  函数名：Matrix_Multiplication
            //'  功能：  计算矩阵的乘法multiplication
            //'  参数：  m   - Integer型变量，相乘的左边矩阵的行数
            //'          n   - Integer型变量，相乘的左边矩阵的列数和右边矩阵的行数
            //'          l   -  Integer型变量，相乘的右边矩阵的列数
            //'          mtxA  - Double型m x n二维数组，存放相乘的左边矩阵
            //'          mtxB  - Double型n x l二维数组，存放相乘的右边矩阵
            //'          mtxC  - Double型m x l二维数组，返回矩阵乘积矩阵
            //''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            int i, N;


            N = mtxB.GetLength(0);

            double mtxC = 0;

            for (i = 0; i < N; i++)
            {
                mtxC = mtxA[frist] / mtxB[second];
            }
            if (mtxC < 0)
                mtxC = -1;
            else if (mtxC == 0)
                mtxC = 0;
            else if (mtxC > 0)
                mtxC = 1;

            return mtxC;
        }
        private double Matrix_min(double[,] mtxA)
        {
            int num_sample = mtxA.GetLength(0);
            int num_attribute = mtxA.GetLength(1);
            double mtxB = 0;
            mtxB = mtxA[0, 0];
            for (int j = 0; j < num_attribute; j++)

                for (int i = 0; i < num_sample; i++)
                {
                    if (mtxB > mtxA[i, j])
                        mtxB = mtxA[i, j];
                }

            return mtxB;

        }
        private double Matrix_max(double[,] mtxA)
        {
            int num_sample = mtxA.GetLength(0);
            int num_attribute = mtxA.GetLength(1);
            double mtxB = 0;
            mtxB = mtxA[0, 0];
            for (int j = 0; j < num_attribute; j++)

                for (int i = 0; i < num_sample; i++)
                {

                    if (mtxB < mtxA[i, j])
                        mtxB = mtxA[i, j];
                }

            return mtxB;

        }
        private double Matrix_min(double[,] mtxA, int s)
        {
            int num_sample = mtxA.GetLength(0);
            int num_attribute = mtxA.GetLength(1);
            double mtxB = 0;
            mtxB = mtxA[0, s];
            for (int j = s; j < s + 1; j++)

                for (int i = 0; i < num_sample; i++)
                {
                    if (mtxB > mtxA[i, j])
                        mtxB = mtxA[i, j];
                }

            return mtxB;

        }
        private double Matrix_max(double[,] mtxA, int s)
        {
            int num_sample = mtxA.GetLength(0);
            int num_attribute = mtxA.GetLength(1);
            double mtxB = 0;
            mtxB = mtxA[0, s];
            for (int j = s; j < s + 1; j++)

                for (int i = 0; i < num_sample; i++)
                {

                    if (mtxB < mtxA[i, j])
                        mtxB = mtxA[i, j];
                }

            return mtxB;

        }


        public bool HyperbolicDec1(List<double> decline_data, List<double> result, double[] parameter_result, int profit_predic_num)
        {
            bool solved = false;
            double error_lm, error = 0;
            int num_time = decline_data.Count;
            double[] decline_time = new double[num_time];
            double[] decline_lnq = new double[num_time];
            for (int i = 0; i < num_time; i++)
            {
                decline_lnq[i] = System.Math.Log(decline_data[i]);
                decline_time[i] = i;
            }

            ////'  LM算法
            ////' 初始猜测s    4.0910   -0.1577    1.9000
            double a0 = decline_lnq[0];
            double b0 = -0.1577;
            double c0 = 1.9;

            //' 数据个数
            int Ndata = num_time;
            //' 参数维数
            int Nparams = 3;
            //' 迭代最大次数
            int n_iters = 500;
            //' LM算法的阻尼系数初值
            double lamda = 0.01;
            //' step1: 变量赋值
            double updateJ = 1;
            double a_est = a0;
            double b_est = b0;
            double c_est = c0;
            //' step2: 迭代
            double[,] Jacobi = new double[Ndata, Nparams];
            double[] lnq_est = new double[Ndata];
            double[,] diff = new double[Ndata, 1];
            double[,] Hessen_lm = new double[Nparams, Nparams];
            double[,] EYE = new double[Nparams, Nparams];
            double[] lnq_est_lm = new double[Ndata];
            double[] diff_lm = new double[Ndata];
            //double [] result=new double [Ndata];
            //double [] parameter_result =new double [3];
            int MO, NO, LO;
            MO = Jacobi.GetLength(1);
            NO = Jacobi.GetLength(0);
            double[,] JacobiT = new double[MO, NO];
            MO = JacobiT.GetLength(0);
            LO = Jacobi.GetLength(1);
            double[,] Hessen = new double[MO, LO];

            for (int it = 1; it < n_iters; it++)
            {
                if (updateJ == 1)
                {
                    //' 根据当前估计值，计算雅克比矩阵
                    for (int M = 0; M < Ndata; M++)
                    {
                        Jacobi[M, 0] = 1;
                        Jacobi[M, 1] = System.Math.Log(1 + c_est * decline_time[M]);
                        Jacobi[M, 2] = b_est * decline_time[M] / (1 + c_est * decline_time[M]);
                    }

                    //'根据当前参数，得到函数值
                    for (int M = 0; M < Ndata; M++)
                        lnq_est[M] = a_est + b_est * System.Math.Log(1 + c_est * decline_time[M]);


                    //' 计算误差
                    for (int M = 0; M < Ndata; M++)
                        diff[M, 0] = decline_lnq[M] - lnq_est[M];

                    //' 计算（拟）海塞矩阵
                    //'计算矩阵bmatrix的转置bmatrixT
                    JacobiT = Matrix_Transpotation(Jacobi);
                    //'计算矩阵bmatrix*bmatrixT= bmatrixTbmatrix

                    Hessen = Matrix_Multiplication(JacobiT, Jacobi);//'矩阵JacobiT×Jacobi=H

                    //' 若是第一次迭代，计算误差
                    if (it == 1)
                    {
                        error = 0;

                        for (int M = 0; M < Ndata; M++)
                            error = error + diff[M, 0] * diff[M, 0];

                    }
                }
                for (int M = 0; M < Nparams; M++)
                    EYE[M, M] = 1;


                //' 根据阻尼系数lamda混合得到Hessen矩阵
                for (int M = 0; M < Nparams; M++)
                    for (int N = 0; N < Nparams; N++)
                        Hessen_lm[M, N] = Hessen[M, N] + (lamda * EYE[M, N]);


                //' 计算步长dp，并根据步长计算新的可能的\参数估计值
                if (Matrix_Inversion(Hessen_lm) == false) // '矩阵H_lm求逆,求逆后也是H_lm
                {
                    //MessageBox.Show("双曲递减求解失败！");
                    return solved;
                }

                MO = JacobiT.GetLength(0);
                LO = diff.GetLength(1);
                double[,] grad = new double[MO, LO];
                grad = Matrix_Multiplication(JacobiT, diff);//'矩阵JacobiT×Jacobi=H

                MO = Hessen_lm.GetLength(0);
                LO = grad.GetLength(1);
                double[,] dp = new double[MO, LO];
                dp = Matrix_Multiplication(Hessen_lm, grad);//'矩阵JacobiT×Jacobi=H

                double a_lm = a_est + dp[0, 0];
                double b_lm = b_est + dp[1, 0];
                double c_lm = c_est + dp[2, 0];

                //' 计算新的可能估计值对应的y和计算残差e
                //On Error Resume Next

                for (int M = 0; M < Ndata; M++)
                    lnq_est_lm[M] = a_lm + System.Math.Log(1 + c_lm * decline_time[M]) * b_lm;

                for (int M = 0; M < Ndata; M++)
                    diff_lm[M] = decline_lnq[M] - lnq_est_lm[M];

                error_lm = 0;

                for (int M = 0; M < Ndata; M++)
                    error_lm = error_lm + diff_lm[M] * diff_lm[M];

                //' --------------这个很重要，因为这个能保证VB正常运行，并且保证得到正确的迭代值
                //If Err.Number <> 0 Then
                //    error_lm = error + 1
                //End If

                //Err.Number = 0
                //' --------------上述过程这个很重要，因为这个能保证VB正常运行，并且保证得到正确的迭代值
                //' 根据误差，决定如何更新参数和阻尼系数
                if (error_lm < error)
                {
                    lamda = lamda / 10;
                    a_est = a_lm;
                    b_est = b_lm;
                    c_est = c_lm;
                    error = error_lm;
                    updateJ = 1;
                }
                else
                {
                    updateJ = 0;
                    lamda = lamda * 10;
                }
            }

            ////'显示优化的结果
            double Qi = System.Math.Exp(a_est);
            double DN = -1 / b_est;
            double Di = c_est / DN;

            for (int M = 0; M < profit_predic_num; M++)
                lnq_est[M] = System.Math.Exp(a_est + b_est * System.Math.Log(1 + c_est * decline_time[M]));

            for (int i = 0; i < Ndata; i++)
                result.Add(lnq_est[i]);

            parameter_result[0] = DN;
            parameter_result[1] = Qi;
            parameter_result[2] = Di;
            solved = true;
            return solved;
        }
        public void ExponentialDec(List<double> decData, List<double> result, double[] parameter_result, int profit_predic_num)
        {
            int time_num = decData.Count;
            double[] qt = new double[time_num];
            double[] tmatix = new double[time_num];
            for (int i = 0; i < time_num; i++)
            {
                qt[i] = System.Math.Log10(decData[i]);
                tmatix[i] = i;
            }
            //所得的方程logQt=A－Bt进行一元线性回归
            double[,] bcresult = new double[2, 1];
            bcresult = polyfit(tmatix, qt);// ' x, y, 参数列表 y=bx+c
            double capital_b = -bcresult[1, 0];
            double capital_a = bcresult[0, 0];
            double n = 0;
            double Qi = System.Math.Pow(10, capital_a);
            double Di = 2.303 * capital_b;
            for (int i = 0; i < profit_predic_num; i++)
            {
                result.Add(Qi * System.Math.Exp((-Di) * i));
            }
            parameter_result[0] = n;
            parameter_result[1] = Qi;
            parameter_result[2] = Di;

        }
        /// <summary>
        /// 简易版的指数递减，双曲递减和调和递减的初始产量都拟合的不好，就用这个替代了
        /// </summary>
        /// <param name="decData"></param>
        /// <returns></returns>
        public double ExponentialDecsimple(List<double> decData)
        {

            int time_num = decData.Count;
            double[] qt = new double[time_num];
            double[] tmatix = new double[time_num];
            for (int i = 0; i < time_num; i++)
            {
                qt[i] = System.Math.Log10(decData[i]);
                tmatix[i] = i;
            }
            //所得的方程logQt=A－Bt进行一元线性回归
            double[,] bcresult = new double[2, 1];
            bcresult = polyfit(tmatix, qt);// ' x, y, 参数列表 y=bx+c
            double capital_a = bcresult[0, 0];
            double Qi = System.Math.Pow(10, capital_a);
            return Qi;

        }
        public void HarmonicDec(List<double> decData, List<double> result, double[] parameter_result, int profit_predic_num)
        {
            int time_num = decData.Count;
            double[] qt = new double[time_num];
            double[] qsum = new double[time_num];
            double temp_qsum = 0;
            for (int i = 0; i < time_num; i++)
            {
                temp_qsum = temp_qsum + decData[i];
                qt[i] = System.Math.Log10(decData[i]);
                qsum[i] = temp_qsum;
            }
            //所得的方程logQt=a－bNP进行一元线性回归
            double[,] bcresult = new double[2, 1];
            bcresult = polyfit(qsum, qt);// ' x, y, 参数列表 y=bx+c
            double capital_b = -bcresult[1, 0];
            double capital_a = bcresult[0, 0];
            double n = 1;
            double Qi = (System.Math.Pow(10, capital_a) + decData[0]) / 2;//
            double Di = 2.303 * capital_b * Qi;
            for (int i = 0; i < profit_predic_num; i++)
            {
                result.Add(Qi / ((Di) * i + 1));
            }
            parameter_result[0] = n;
            parameter_result[1] = Qi;
            parameter_result[2] = Di;


        }
        /// <summary>
        /// 相关系数,或者拟合度，要求两个集合数量必须相同
        /// </summary>
        /// <param name="array1">数组一</param>
        /// <param name="array2">数组二</param>
        /// <returns></returns>
        public double correl(double[] array1, double[] array2)
        {
            //数组一
            double avg1 = average(array1);
            //数组二
            double avg2 = average(array2);

            double sumfenzi = 0;
            double sumfenmu_x = 0;
            double sumfenmu_y = 0;
            for (int i = 0; i < array1.GetLength(0) && i < array2.GetLength(0); i++)
            {
                sumfenzi = sumfenzi + ((array1[i] - avg1)) * ((array2[i] - avg2));
                sumfenmu_x = sumfenmu_x + (array1[i] - avg1) * (array1[i] - avg1);
                sumfenmu_y = sumfenmu_y + (array2[i] - avg2) * (array2[i] - avg2);
            }
            double sumfenmu = System.Math.Pow(sumfenmu_x, 0.5) * System.Math.Pow(sumfenmu_y, 0.5);
            double cor = sumfenzi / sumfenmu;
            cor = cor * cor;
            return cor;
        }
        /// <summary>
        /// 求出数据平均值,并保留三位小数
        /// </summary>
        /// <param name="Valist">数据集合</param>
        /// <returns></returns>
        public double average(double[] Valist)
        {
            double sum = 0;
            foreach (double d in Valist)
            {
                sum = sum + d;
            }
            double revl = sum / Valist.GetLength(0);
            return revl;
        }



        /// <summary>
        /// 试凑法，求解双曲递减
        /// </summary>
        /// <param name="decline_data"></param>
        /// <param name="result"></param>
        /// <param name="parameter_result"></param>
        /// <param name="profit_predic_num"></param>
        /// <returns></returns>
        public void HyperbolicDec2(List<double> decData, List<double> result, double[] parameter_result, int profit_predic_num)
        {
            int time_num = decData.Count;
            double[] qt_temp = new double[time_num];
            double[] qt = new double[time_num];
            double[] tmatix = new double[time_num];
            double Qi = (ExponentialDecsimple(decData) + decData[0]) / 2;
            for (int i = 0; i < time_num; i++)
            {
                qt_temp[i] = Qi / decData[i];
                tmatix[i] = i;
            }
            double bestn = goldmin(qt_temp, tmatix);


            for (int i = 0; i < time_num; i++)
            {
                qt[i] = System.Math.Pow(qt_temp[i], bestn);
            }
            //所得的方程Qt=a+bt进行一元线性回归
            double[,] bcresult = new double[2, 1];
            bcresult = polyfit(tmatix, qt);// ' x, y, 参数列表 y=bx+a
            double capital_b = bcresult[1, 0];
            double n = bestn;
            //double Qi = (ExponentialDecsimple(decData)+decData[0]);
            double Di = capital_b / n;
            for (int i = 0; i < profit_predic_num; i++)
            {
                result.Add(Qi / (System.Math.Pow((1 + n * Di * (i)), 1 / n)));
            }
            parameter_result[0] = n;
            parameter_result[1] = Qi;
            parameter_result[2] = Di;

        }

        private double goldmin(double[] qt, double[] tmatix)
        {
            //% 黄金分割法求解函数最小值
            //% 输入
            //% f 待优化函数
            //% a,b 区间
            //% s 精度
            //% 输出
            //% x 最优解
            //% y 最优解对应的最小值
            //%%

            //% 黄金分割比,0.618
            double g = (System.Math.Pow(5, 0.5) - 1) / 2;
            double a = 0;
            double b = 1;//n的上下限
            double s = 0.01;//精度
            double x2 = 0.2; //a + g * (b - a); //第二个尝试的n'
            double x1 = 0.3; //a + b - x2;//第一个尝试的n'
            double y1 = goldtry(x1, qt, tmatix);
            double y2 = goldtry(x2, qt, tmatix);
            int iterator = 1;
            while (System.Math.Abs(b - a) > s | iterator > 500)
            {
                iterator++;
                if (y1 < y2)
                {
                    b = x2;
                    x2 = x1;
                    y2 = y1;
                    x1 = a + 0.382 * (b - a);
                    y1 = goldtry(x1, qt, tmatix);
                }

                else if (y1 == y2)
                {
                    a = x1;
                    b = x2;
                    x2 = a + g * (b - a);
                    x1 = a + 0.382 * (b - a); ;
                    x2 = a + b - x1;
                    y1 = goldtry(x1, qt, tmatix);
                    y2 = goldtry(x2, qt, tmatix);

                }

                else
                {
                    a = x1;
                    x1 = x2;
                    x2 = a + g * (b - a);
                    y2 = goldtry(x2, qt, tmatix);
                }
            }
            double bestx = (a + b) / 2;
            return bestx;

        }

        private double goldtry(double n, double[] qt, double[] tmatix)
        {
            int time_num = qt.GetLength(0);
            double[] qtcopy = new double[qt.GetLength(0)];
            for (int i = 0; i < time_num; i++)
            {
                qtcopy[i] = System.Math.Pow(qt[i], n);
            }
            //所得的方程Qt=a+bt进行一元线性回归
            double[,] bcresult = new double[2, 1];
            bcresult = polyfit(tmatix, qtcopy);// ' x, y, 参数列表 y=bx+a
            double capital_b = bcresult[1, 0];
            double capital_a = bcresult[0, 0];
            double[] fit_qt = new double[time_num];
            for (int i = 0; i < time_num; i++)
            {
                fit_qt[i] = capital_b * tmatix[i] + capital_a;
            }
            double fit_cor = correl(fit_qt, qtcopy);

            return fit_cor;
        }

        public System.Drawing.Color GetRandomColor()
        {
            Random RandomNum_First = new Random((int)DateTime.Now.Ticks);
            //  对于C#的随机数，没什么好说的
            System.Threading.Thread.Sleep(RandomNum_First.Next(50));
            Random RandomNum_Sencond = new Random((int)DateTime.Now.Ticks);

            //  为了在白色背景上显示，尽量生成深色
            int int_Red = RandomNum_First.Next(256);
            int int_Green = RandomNum_Sencond.Next(256);
            int int_Blue = (int_Red + int_Green > 400) ? 0 : 400 - int_Red - int_Green;
            int_Blue = (int_Blue > 255) ? 255 : int_Blue;

            return System.Drawing.Color.FromArgb(int_Red, int_Green, int_Blue);
        }
        /// <summary>
        /// 计算KGB压裂模型
        /// </summary>
        /// <param name="tf">施工时间</param>
        /// <param name="G">岩石弹性模量</param>
        /// <param name="Q">施工排量</param>
        /// <param name="v">岩石泊松比</param>
        /// <param name="mu">压裂液粘度</param>
        /// <param name="Zi"> In-situ stress </param>
        public static DataTable KGD(double tf, double G, double Q, double v, double mu, double Zi = 0)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("L", typeof(double));
            dt.Columns.Add("W0", typeof(double));
            dt.Columns.Add("Pwi", typeof(double));
            dt.Columns.Add("time", typeof(double));
            double limitL = .48 * Math.Pow((8 * G * Math.Pow(Q, 3) / ((1 - v) * mu)), (1.0 / 6)) * Math.Pow(tf, (2.0 / 3));   // %used for axis limits
            double limitW0 = 1.32 * Math.Pow((8 * (1 - v) * mu * Math.Pow(Q, 3) / G), (1.0 / 6)) * Math.Pow(tf, (1.0 / 3));
            // for (double t0 = tf / 50; t0 < tf; t0 = t0 + tf / 50)
            //double lratio = 28.7 / limitL;
            double lratio = limitL;
            double wratio = limitW0;
            double t0 = tf;
            {

                //time step for visualization

                List<double> L = new List<double>();
                List<double> W0 = new List<double>();
                List<double> Pwi = new List<double>();
                List<double> time = new List<double>();
                for (double t = 0; t < t0; t = t + tf / 50)
                {
                    //ngr(i)=0;
                    L.Add((.48 * Math.Pow((8 * G * Math.Pow(Q, 3) / ((1 - v) * mu)), (1.0 / 6)) * Math.Pow(t, (2.0 / 3))) * lratio); //  %Fracture Length
                    W0.Add((1.32 * Math.Pow((8 * (1 - v) * mu * Math.Pow(Q, 3) / G), (1.0 / 6)) * Math.Pow(t, (1.0 / 3))) * wratio);   //%maximum fracture opening width
                    Pwi.Add(Zi + 0.96 * Math.Pow((2 * Q * mu * Math.Pow(G, 3) / (Math.Pow((1 - v), 3) * Math.Pow(L.Last(), 2))), (1.0 / 4))); //%wellbore pressure
                    time.Add(t);
                    dt.Rows.Add(new object[] { L.Last(), W0.Last(), Pwi.Last(), time.Last() });
                }
            }


            double h = 1.1 * limitL;
            //第一张图time，L
            //plot(time,L);
            //ylabel('Length [ft]');
            //xlabel('time [min]');
            //axis([0 tf 0 limitL]);
            //第2张图time，w0
            //plot(time,W0);
            //ylabel('max width [in]');
            //xlabel('time [min]');
            //axis([0 tf 0 limitW0]);

            //第3张图
            // fL=fliplr(L);
            //  plot3(fL,W0,ngr,'b');

            //  plot3(fL,-W0,ngr,'b');
            //  hold;
            //  axis([0 limitL -2*limitW0 2*limitW0 -1 1]);
            //  grid on;
            // ylabel('Max WIDTH [in]')
            // xlabel('length [ft]')
            // set(gca,'ztick',[]);
            // daspect([1,1.0/2,1]);
            //view(-62,26);
            //[~,h3]=suplabel(sprintf('time = %0.1g   min',t0));grid on;
            return dt;
        }

        public static double kt(double Q, double C, double w, double t)
    {
   double  x=2*C*Math.Sqrt(Math.PI*t)/w;

   double A = Q * w / 4 / Math.PI / Math.Pow(C, 2) * (Math.Exp(Math.Pow(x, 2)) * erfc(x) + 2 * x / Math.Sqrt(Math.PI) - 1);
    return A;
    }
        /// <summary>
        /// 误差函数
        /// </summary>
        /// <param name="B12"></param>
        /// <returns></returns>
        public static double erfc(double B12)
        {
            double A = 0.254829592 * B12 - 0.284496736 * Math.Pow(B12, 2) + 1.42143741 * Math.Pow(B12, 3) - 1.453152027 * Math.Pow(B12, 4) + 1.06140429 * Math.Pow(B12, 5);
            return A;
        }

        public static double PKN_w(double Q, double vis, double L, double E = 2.555e10, double v = 0.27)
        {
            double tem = Math.Pow(1 / 60 * (1 - v * v) * Q * vis * L / E, 0.25);
            //# 返回值为平均缝宽
            return tem * Math.PI / 2 * 1.26;
        }
        //        # C 为综合滤失系数 m/sqrt(min)
       


        /// <summary>
        /// PKN压裂模型(Assuming no leak off)  
        /// </summary>
        /// <param name="tf">施工时间</param>
        /// <param name="G">岩石弹性模量</param>
        /// <param name="Q">施工排量</param>
        /// <param name="v">岩石泊松比</param>
        /// <param name="mu">压裂液粘度</param>
        /// <param name="h">缝高</param>
        public static double PKN(double tf, double G, double Q, double v, double mu, double h)
        {
            DataTable dt = new DataTable();
            double error = 0.01;
            double C = 0.0007;
            double w = 0.01;
            dt.Columns.Add("L", typeof(double));
            dt.Columns.Add("W0", typeof(double));
            dt.Columns.Add("Pwi", typeof(double));
            dt.Columns.Add("time", typeof(double));
            
            double L = kt(Q, C, w, tf) / 2 / h;
            for (int i = 0; i < 100; i++)// range(100):
            {
                double temL = L;
                w = PKN_w(Q, mu, L);
                L = kt(Q, C, w, tf) / 2 / h;
                //# print("L=%s\tw=%s" % (L, w))
                if (Math.Abs(temL - L) < error)
                    break;
            }

            return L;
            ////for (double t0 = tf / 50; t0 < tf; t0 = t0 + tf / 50)//      %time step for visualization
            //double limitL = .68 * Math.Pow((G * Math.Pow(Q, 3) / ((1 - v) * mu * Math.Pow(h, 4))), (1.0 / 5)) * Math.Pow(tf, (4.0 / 5));//  %used for axis limits
            //double limitW0 = 2.5 * Math.Pow(((1 - v) * mu * Math.Pow(Q, 2) / (G * h)), (1.0 / 5)) * Math.Pow(tf, (1.0 / 5));
            //double lratio = 41.1 / limitL;
            //double wratio = 9.38 / limitW0;


            //double t0 = tf;
            //{

            //    List<double> L = new List<double>();
            //    List<double> W0 = new List<double>();
            //    List<double> Pwi = new List<double>();
            //    List<double> time = new List<double>();
            //    for (double t = 0; t < t0; t = t + tf / 50)
            //    {
            //        //ngr(i)=0;
            //        L.Add((.68 * Math.Pow((G * Math.Pow(Q, 3) / ((1 - v) * mu * Math.Pow(h, 4))), (1.0 / 5)) * Math.Pow(t, (4.0 / 5))) * lratio);  // %Fracture Length
            //        W0.Add((2.5 * Math.Pow(((1 - v) * mu * Math.Pow(Q, 2) / (G * h)), (1.0 / 5)) * Math.Pow(t, (1.0 / 5))) * wratio);    //  %Fracture opening width
            //        Pwi.Add(2.5 * Math.Pow((Math.Pow(Q, 2) * mu * Math.Pow(G, 4) / (Math.Pow((1 - v), 4) * Math.Pow(h, 6))), (1.0 / 5)) * Math.Pow(t, (1.0 / 5))); //  %wellbore net pressure
            //        time.Add(t);
            //        dt.Rows.Add(new object[] { L.Last(), W0.Last(), Pwi.Last(), time.Last() });
            //    }
            //    //timer=fliplr(time);

            //    double h2 = 0.2 * limitL;
            //    //z=h2/time.Last()*time+h2/2;
            //    double A = G * Math.Pow(Q, 3) / ((1 - v) * mu * Math.Pow(h2, 4));
            //    double a = Math.Pow(.68, (5.0 / 4)) * A;
            //    double B = ((1 - v) * mu * Math.Pow(Q, 2)) / (G * h2);
            //    double b = Math.Pow(2.5, 5) * B;
            //    double c = h2 / time.Last();
            //    //Z=z-h2/2-t0*c;


            //    //第一幅图时间与L
            //    //subplot(221)
            //    //  plot(time,L)
            //    //  ylabel('Length [ft]')
            //    //  xlabel('time [min]')
            //    //  axis([0 tf 0 limitL])
            //    //  grid on;
            //    //第2幅图时间与WW
            //    //plot(time,W0)
            //    //ylabel('max width [in]')
            //    //xlabel('time [min]')
            //    //axis([0 tf 0 limitW0])
            //    //grid on;
            //    //第4副图画的是那个半椭圆形
            //    //  fL=fliplr(L);
            //    //  plot3(fL,W0,ngr,'b');
            //    //  hold;
            //    //  plot3(fL,-W0,ngr,'b');
            //    //  hold;
            //    //  axis([0 limitL -2*limitW0 2*limitW0 -1 1]);
            //    //  grid on;
            //    //  ylabel('width [in]') 
            //    //  xlabel('Length [ft]')
            //    //  set(gca,'ztick',[]);
            //    //  daspect([1,1.0/2,1]);

            //    //[~,h3]=suplabel(sprintf('time = %0.1g   min \n PKN RESULTS \n',t0));grid on;
            //    //hold off;
            //}
            //return dt;
        }
        /// <summary>
        /// 计算度及用量
        /// </summary>
        /// <param name="widthf">裂缝宽度</param>
        /// <param name="heightf">裂缝高度</param>
        /// <param name="lengthf">裂缝长度</param>
        /// <param name="porsand">平均砂比</param>
        /// <param name="vs">压裂液体积</param>
        /// <param name="lengthm">高渗基质长</param>
        ///  <param name="widthm">高渗基质宽</param>
        /// <param name="heightm">高渗基质高</param>
        /// <param name="lengthgel">堵剂充填范围</param>
        /// <param name="pormbef">原地层基质孔隙度</param>
        /// <param name="wgel">封堵宽度 </param>
        /// <param name="wm">基质宽度</param>

        public static double gel(double widthf, double heightf, double lengthf, double porsand, double vs,
          double lengthm, double lengthgel, double pormbef,
            double widthm, double heightm, double wgel, double wm)
        {
            //堵剂充填体积 = 裂缝内充填体积 + 基质内充填体积    
            //裂缝内堵剂充填体积 = 裂缝体积×裂缝孔隙度  
            double gel1 = 2 * widthf * heightf * lengthf * (1 - porsand);
            double vm = 2 * widthm * heightm * lengthm;
            //下式中porm为压裂前低渗基质的孔隙度；vs为压裂液体积。 
            double porm = vs * lengthm / (2 * vm * lengthgel) + pormbef;
            //基质内堵剂充填体积 = 高渗基质体积×高渗基质孔隙度×封堵宽度比  
            double gel2 = 2 * widthm * heightm * lengthm * porm * (wgel / wm);
            //    则堵剂用量为： 
            double gel3 = gel1 + gel2;
            return gel3;
        }
        /// <summary>
        /// 考虑基于堵剂向基质中滤失      
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="vgel"></param>
        /// <returns></returns>

        public static double gel(double a, double b, double c, double vgel)
        {
            double gel1 = 0;
            double cleak = a + b * Math.Exp(-vgel / c);
            gel1 = vgel * (1 + cleak);
            return gel1;
        }
        /// <summary>
        /// 堵剂用量
        /// </summary>
        /// <param name="ts">初凝时间</param>
        /// <param name="tf">终凝时间</param>
        /// <param name="vgel">堵剂注入体积</param>
        /// <param name="type"> 触变凝胶堵剂/聚合物凝胶堵剂</param>
        /// <returns></returns>
        public static double gel(double ts, double tf, double vgel, string type)
        {
            switch (type)
            {
                //    触变凝胶堵剂注入体积一般较小
                case "触变型":
                    {
                        vgel = vgel / ts;
                        break;
                    }
                //  聚合物凝胶堵剂可适当延长注入时间
                default:
                    {
                        double t = (ts + tf) / 2;
                        vgel = vgel / t;
                        break;
                    }
            }
            return vgel;
        }



    }
}
