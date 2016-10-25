using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace fracture
{
    class PSO
    {
        public void PsoProcess(int SwarmSize, int LoopCount, int math_predic_num, double[,] psodata, int Dimension, double[,] PsoScope, double Evalue_type, double[,] psoresult, List<double> parameter_result)
        {
            //'初始化种群
           
            int ParticleSize = Dimension;
            int num_time = psodata.GetLength(0);
            //ProgressBar1.Min = 0
            //ProgressBar1.Max = LoopCount
            //ProgressBar1.Value = ProgressBar1.Min
            //ProgressBar1.Scrolling = ccScrollingSmooth
            double[,] ParSwarm = new double[SwarmSize, 2 * ParticleSize + 1];
            double[,] OptSwarm = new double[SwarmSize + 1, ParticleSize+1];
            //double [,] psoresult=new double [num_time+math_predic_num,2];
            InitSwarm(Evalue_type, ParSwarm, OptSwarm, SwarmSize, ParticleSize, PsoScope, psodata);

            double[] record = new double[LoopCount];
            for (int nIter = 1; nIter < LoopCount; nIter++)
            {
                //DoEvents
                ////ProgressBar1.Value = nIter
                //BaseStepPso LoopCount, nIter, psodata, Evalue_type, PsoScope 'ParSwarm, OptSwarm, AdaptFunc,
                BaseStepPso(LoopCount, nIter, psodata, SwarmSize, ParticleSize, Evalue_type, PsoScope, ParSwarm, OptSwarm);
                record[nIter] = OptSwarm[SwarmSize, ParticleSize ];
            }


            //'输出根据pso计算的结果
            for (int i = 0; i < num_time; i++)
                psoresult[i, 0] = psodata[i, 0];


           // diff = psodata[1, 0] - psodata[0, 0];

            //for (int i = num_time + 1; i < num_time + math_predic_num; i++)
            //{
            //    psoresult[i, 0] = psoresult[i - 1, 0] + diff;
            //}


            for (int i = 0; i < num_time + math_predic_num; i++)
            {
                //'psoresult第0列为时间，第1列为产油/产水量
                //'*********************************************
                if (Evalue_type == 0)
                    //'Print #2, OptSwarm(SwarmSize, 4), OptSwarm(SwarmSize, 0), OptSwarm(SwarmSize, 1), psodata(i, 0), OptSwarm(SwarmSize, 2), OptSwarm(SwarmSize, 3)
                    psoresult[i, 1] = OptSwarm[SwarmSize, 4] / (System.Math.Pow((1 + OptSwarm[SwarmSize, 0] * System.Math.Pow(OptSwarm[SwarmSize, 1], (System.Math.Pow((i+1), OptSwarm[SwarmSize, 2])))), OptSwarm[SwarmSize, 3]) + 0.0000001);

                if (Evalue_type == 1)
                    psoresult[i, 1] = 0.98 / (System.Math.Pow((1 + OptSwarm[SwarmSize, 0] * System.Math.Pow(OptSwarm[SwarmSize, 1], (System.Math.Pow((i + 1), OptSwarm[SwarmSize, 2])))), OptSwarm[SwarmSize, 3]) + 0.0000001);
                //'*****更改上面的代码，可以添加函数的类型，但是注意，有三个部分：
                //'InitSwarm的评价和BaseStepPso的评价和 PsoProcess的结果输出都需要更改*****
                //'*********************************************
            }

            //'输出拟合的参数(iPopindex)
            //ReDim parameter_result(ParticleSize) As Double

            for (int iPopindex = 0; iPopindex < ParticleSize; iPopindex++)
                parameter_result.Add(OptSwarm[SwarmSize, iPopindex]);
        }

        private void InitSwarm(double Evalue_type, double[,] ParSwarm, double[,] OptSwarm, int SwarmSize, int ParticleSize, double[,] PsoScope, double[,] psodata)
        {
            //'功能描述：初始化粒子群，限定粒子群的位置以及速度在指定的范围内
            int best_row_index = 0;
            double best_value;
            Random Rnd = new Random();
            for (int iPopindex = 0; iPopindex < SwarmSize; iPopindex++)
                for (int iDimindex = 0; iDimindex < 2 * ParticleSize + 1; iDimindex++)
                    ParSwarm[iPopindex, iDimindex] = Rnd.NextDouble();

            //'对每一个粒子计算其适应度函数的值
            //'对粒子群中位置,速度的范围进行调节
            for (int iPopindex = 0; iPopindex < SwarmSize; iPopindex++)
            {
                for (int iDimindex = 0; iDimindex < ParticleSize; iDimindex++)
                {
                    //'对粒子群中位置,速度的范围进行调节
                    ParSwarm[iPopindex, iDimindex] = ParSwarm[iPopindex, iDimindex] * (PsoScope[iDimindex, 1] - PsoScope[iDimindex, 0]) + PsoScope[iDimindex, 0];
                    ParSwarm[iPopindex, iDimindex + ParticleSize] = ParSwarm[iPopindex, iDimindex + ParticleSize] * (PsoScope[iDimindex, 1] - PsoScope[iDimindex, 0]) + PsoScope[iDimindex, 0];

                    if (ParSwarm[iPopindex, iDimindex] < PsoScope[iDimindex, 0])//PsoScope[iDimindex, 0] != 999 & 
                        ParSwarm[iPopindex, iDimindex] = PsoScope[iDimindex, 0];


                    if (ParSwarm[iPopindex, iDimindex] > PsoScope[iDimindex, 1])//PsoScope[iDimindex, 1] != 999 & 
                        ParSwarm[iPopindex, iDimindex] = PsoScope[iDimindex, 0];
                    //'Print #2, "初始位置"
                    //'Print #2, ParSwarm(iPopindex, iDimindex);
                }
                //ParSwarm[0, 4]=231498.8355;
                //ParSwarm[0, 0]=4.355915872;
                // ParSwarm[0, 1]=0.687478444;
                //ParSwarm[0, 2]=0.833998964;
                //ParSwarm[0, 3] = 4.661407426;

                //'*********************************************
                if (Evalue_type == 0)
                {
                    usher_oil_func(iPopindex, psodata, ParSwarm, SwarmSize, ParticleSize);
                }

                if (Evalue_type == 1)
                {
                    usher_WATER_func(iPopindex, psodata, ParSwarm, SwarmSize, ParticleSize);
                }

                //'*****更改上面的代码，可以添加函数的类型，但是注意，有三个部分：
                //'InitSwarm的评价和BaseStepPso的评价和 PsoProcess的结果输出都需要更改*****
                //'*********************************************
            }
            //'初始化粒子群最优解矩阵,粒子群最优解矩阵全部设为零
            for (int iPopindex = 0; iPopindex < SwarmSize + 1; iPopindex++)
                for (int iDimindex = 0; iDimindex < ParticleSize + 1; iDimindex++)
                    OptSwarm[iPopindex, iDimindex] = 0;

            //'寻找适应度函数值最大的解在矩阵中的位置(行数)
            best_value = ParSwarm[0, 2 * ParticleSize];

            for (int iPopindex = 0; iPopindex < SwarmSize; iPopindex++)
            {
                if (ParSwarm[iPopindex, 2 * ParticleSize] < best_value)
                {
                    best_value = ParSwarm[iPopindex, 2 * ParticleSize];
                    best_row_index = iPopindex;
                }
            }
            //'Print #2, "初始最优位置", best_value, best_row_index
            for (int iPopindex = 0; iPopindex < SwarmSize; iPopindex++)
                for (int iDimindex = 0; iDimindex < ParticleSize; iDimindex++)
                    OptSwarm[iPopindex, iDimindex] = ParSwarm[iPopindex, iDimindex];

            for (int iPopindex = 0; iPopindex < SwarmSize; iPopindex++)
                OptSwarm[iPopindex, ParticleSize ] = ParSwarm[iPopindex, 2 * ParticleSize];

            for (int iDimindex = 0; iDimindex < ParticleSize; iDimindex++)
                OptSwarm[SwarmSize, iDimindex] = ParSwarm[best_row_index, iDimindex];

            //'Print #2, "初始最优矩阵"
            OptSwarm[SwarmSize, ParticleSize ] = ParSwarm[best_row_index, 2 * ParticleSize];

        }

        private void BaseStepPso(int LoopCount, int CurCount, double[,] psodata, int SwarmSize, int ParticleSize, double Evalue_type, double[,] PsoScope, double[,] ParSwarm, double[,] OptSwarm)
        {
            double MaxW, MinW, c1, c2, a;
            Random Rnd = new Random();
            MaxW = 0.95;
            MinW = 0.4;
            //'*****更改下面的代码，可以更改惯性因子的变化*****
            //'线形递减策略
            double InerWt = MaxW - CurCount * ((MaxW - MinW) / LoopCount);
            //'*********************************************
            c1 = 2;
            c2 = 2;

            //'*****更改上面的代码，可以更改c1,c2的变化*****
            //'*********************************************
            //'    Print #2, "更新速度"
            for (int iPopindex = 0; iPopindex < SwarmSize; iPopindex++)
            {
                for (int iDimindex = 0; iDimindex < ParticleSize; iDimindex++)
                {
                    ParSwarm[iPopindex, iDimindex + ParticleSize] = InerWt * ParSwarm[iPopindex, iDimindex +ParticleSize] + c1 * Rnd.NextDouble() * (OptSwarm[iPopindex, iDimindex] - ParSwarm[iPopindex, iDimindex]) + c2 * Rnd.NextDouble() * (OptSwarm[SwarmSize, iDimindex] - ParSwarm[iPopindex, iDimindex]);

                    //'            Print #2, ParSwarm(iPopindex, iDimindex + ParticleSize + 1)
                    //'限制速度的代码
                    if (ParSwarm[iPopindex, iDimindex + ParticleSize] > PsoScope[iDimindex, 1] / 2)
                        ParSwarm[iPopindex, iDimindex + ParticleSize] = PsoScope[iDimindex, 1] / 2;

                    if (ParSwarm[iPopindex, iDimindex +ParticleSize] < -PsoScope[iDimindex, 1] / 2)
                        ParSwarm[iPopindex, iDimindex +ParticleSize] = -PsoScope[iDimindex, 1] / 2 + 0.0000000001;
                }
            }

            //'    Print #2, "速度完成"
            //'*********************************************
            a = 0.729;

            //'*****更改上面的代码，可以更改约束因子的变化*****
            //'限制位置的范围
            //'    Print #2, "更新位置"
            for (int iPopindex = 0; iPopindex < SwarmSize; iPopindex++)
            {
                for (int iDimindex = 0; iDimindex < ParticleSize; iDimindex++)
                {
                    ParSwarm[iPopindex, iDimindex] = ParSwarm[iPopindex, iDimindex] + a * ParSwarm[iPopindex, iDimindex +ParticleSize];

                    if (ParSwarm[iPopindex, iDimindex] < PsoScope[iDimindex, 0])//PsoScope[iDimindex, 0] != 999 &
                        ParSwarm[iPopindex, iDimindex] = PsoScope[iDimindex, 0];

                    if (ParSwarm[iPopindex, iDimindex] > PsoScope[iDimindex, 1])//PsoScope[iDimindex, 1] != 999 &
                        ParSwarm[iPopindex, iDimindex] = PsoScope[iDimindex, 0];
                }
            }

            //'    Print #2, "位置完成"
            //'    Print #2, "新的适应度值"
            //'计算每个粒子的新的适应度值
            for (int iPopindex = 0; iPopindex < SwarmSize; iPopindex++)
            {  //'*********************************************
                if (Evalue_type == 0)
                {
                    usher_oil_func(iPopindex, psodata, ParSwarm, SwarmSize, ParticleSize);
                }

                if (Evalue_type == 1)
                {
                    usher_WATER_func(iPopindex, psodata, ParSwarm, SwarmSize, ParticleSize);
                }

                //'*****更改上面的代码，可以添加函数的类型，但是注意，有三个部分：
                //'InitSwarm的评价和BaseStepPso的评价和 PsoProcess的结果输出都需要更改*****
                //'*********************************************
                //'        Print #2, ParSwarm(iPopindex, 2 * ParticleSize + 1)
            }

            //'    Print #2, "新的适应度值完成-----"
            //'更新每个粒子的最佳适应度值
            for (int iPopindex = 0; iPopindex < SwarmSize; iPopindex++)
            {
                if (ParSwarm[iPopindex, 2*ParticleSize] < OptSwarm[iPopindex, ParticleSize ])
                {
                    for (int iDimindex = 0; iDimindex < ParticleSize; iDimindex++)
                        OptSwarm[iPopindex, iDimindex] = ParSwarm[iPopindex, iDimindex];

                    OptSwarm[iPopindex, ParticleSize ] = ParSwarm[iPopindex, 2*ParticleSize];
                }
            }

            //'寻找适应度函数值最大的解在矩阵中的位置(行数)，进行全局最优的改变
            double best_value = ParSwarm[0, 2*ParticleSize];
            int best_row_index = 0;

            for (int iPopindex = 0; iPopindex < SwarmSize; iPopindex++)
            {
                if (ParSwarm[iPopindex, 2*ParticleSize] < best_value)
                {
                    best_value = ParSwarm[iPopindex, 2*ParticleSize];
                    best_row_index = iPopindex;
                }

            }

            //'进行全局最优的改变
            if (ParSwarm[best_row_index, 2*ParticleSize] < OptSwarm[SwarmSize, ParticleSize ])
            {
                for (int iDimindex = 0; iDimindex < ParticleSize; iDimindex++)
                    OptSwarm[SwarmSize, iDimindex] = ParSwarm[best_row_index, iDimindex];
                OptSwarm[SwarmSize, ParticleSize ] = ParSwarm[best_row_index, 2*ParticleSize];
            }
        }

        private void usher_oil_func(int iPopindex, double[,] psodata, double[,] ParSwarm, int SwarmSize, int ParticleSize)
        {
            double temp;
            int num_time = psodata.GetLength(0);
            try
            {
                ParSwarm[iPopindex, 2*ParticleSize] = 0;

                for (int i = 0; i < num_time; i++)
                {
                    temp = ParSwarm[iPopindex, 4] / System.Math.Pow((1 + ParSwarm[iPopindex, 0] * System.Math.Pow(ParSwarm[iPopindex, 1], (System.Math.Pow(psodata[i, 0], ParSwarm[iPopindex, 2])))), ParSwarm[iPopindex, 3]);
                    //'        Print #3, temp, psodata(i, 0), psodata(i, 1)
                    ParSwarm[iPopindex, 2*ParticleSize] = ParSwarm[iPopindex, 2*ParticleSize] + (temp - psodata[i, 1]) * (temp - psodata[i, 1]);
                }
                if (double.IsNaN(ParSwarm[iPopindex, 2 * ParticleSize]))
                {
                    ParSwarm[iPopindex, 2 * ParticleSize] = 1E+100;
                }
            }
            catch
            {
                ParSwarm[iPopindex, 2*ParticleSize] = 1E+100;
            }
        }

        private void usher_WATER_func(int iPopindex, double[,] psodata, double[,] ParSwarm, int SwarmSize, int ParticleSize)
        {
            double temp;
            int num_time = psodata.GetLength(0);
            try
            {
                ParSwarm[iPopindex, 2*ParticleSize] = 0;

                for (int i = 0; i < num_time; i++)
                {
                    temp = 0.98 / System.Math.Pow((1 + ParSwarm[iPopindex, 0] * System.Math.Pow(ParSwarm[iPopindex, 1], (System.Math.Pow(psodata[i, 0], ParSwarm[iPopindex, 2])))), ParSwarm[iPopindex, 3]);
                    
                    ParSwarm[iPopindex, 2*ParticleSize] = ParSwarm[iPopindex, 2*ParticleSize] + i * (temp - psodata[i, 1]) * (temp - psodata[i, 1]);
                }
                if (double.IsNaN(ParSwarm[iPopindex, 2 * ParticleSize]))
                {
                    ParSwarm[iPopindex, 2 * ParticleSize] = 1E+100;
                }
            }
            catch
            {
                ParSwarm[iPopindex, 2*ParticleSize] = 1E+100;
            }
        }
   
    
    }
}
