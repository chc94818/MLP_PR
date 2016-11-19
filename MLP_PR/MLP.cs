using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace MLP_PR
{
    //多層類神經感知機
    class MLP
    {
        //宣告-----------------------------------------------------------------------------------------------
        double[,] vT;
        double learn;
        double convergence;
        double[,] dataTrain;
        double[,] dataTest;
        double[] weight;
        double maxD;
        double minD;
        double testl = 0.5;
        double testc = 0.001;
        double maxTemp;
        double minTemp;
        double reguTemp;
        double reguNu;
        double[,] data;
        double rmse;

        double sum; //計算資料筆數
        double hit; //命中資料筆數
        double hitRate;//命中率
        List<Perceptron[]> mlp = new List<Perceptron[]>();//多層類神經網路LIST mlp[0] = 輸入層 mlp[n] = 第n層
        //--------------------------------------------------------------------------------------------------
        //MLP CONSTRUCT-------------------------------------------------------------------------------------
        //int[] layer 表示隱藏層跟輸出層的神經元個數
        public MLP(int[] layer,int dimension,double learn)
        {
            //儲存學習率
            this.learn = learn;

            //建立每一層的神經元
            //建立輸入層神經元
            Perceptron[] pTemp;
            pTemp = new Perceptron[dimension];//輸入層神經元數等於輸入維度
            int weightNum = dimension;//每一個神經元連接到上一層所需的連結數

            //初始化輸入層神經元
            for (int j = 0; j < pTemp.Length; j++)
            {
                pTemp[j] = new Perceptron();
                pTemp[j].weightInit(weightNum + 1,learn);//多一維度是 w0 = 閥值
            }
            mlp.Add(pTemp); //將初始成功的神經元放入mlp list

            for (int i = 0; i < layer.Length; i++)
            {
                pTemp = new Perceptron[layer[i]];
                if (i != 0)//除了輸入曾與第一層，其餘的權重數為上一層的神經元數量
                {
                    weightNum = layer[i - 1];
                }
                //初始化神經元
                for(int j = 0; j < pTemp.Length; j++)
                {
                    pTemp[j] = new Perceptron();
                    pTemp[j].weightInit(weightNum+1, learn);//多一維度是 w0 = 閥值
                }
                mlp.Add(pTemp); //將初始成功的神經元放入mlp list
            }
        }
        //--------------------------------------------------------------------------------------------------
        //訓練權重-------------------------------------------------------------------------------------------
        public void Train(List<int[]> trainData)
        {
            double eTemp;
            rmse = 0;

            //對每一組DATA做運算
            foreach (int[] td in trainData)
            {

                //前饋階段-------------------------------------------------------------------------------------------  
                //輸入資料為 訓練資料的 0~N-1，第N個為expect
                //輸入資料的 inputData[0] = -1 用來跟w0計算閥值
                double[] inputData = new double[td.Length];
                inputData[0] = -1;
                for (int i = 1; i < inputData.Length; i++)
                {
                    inputData[i]= td[i-1];
                }

                //前饋開始
                foreach (Perceptron[] p in mlp)
                {
                    double[] outputTemp = new double[p.Length+1];//多一個維度是-1 用來計算閥值
                    outputTemp[0] = -1;//第一個值為-1 用來跟w0計算閥值
                    for (int i = 0; i < p.Length; i++)
                    {
                        outputTemp[i+1] = p[i].cal(inputData);
                    }
                    inputData = outputTemp;                   
                }


                //--------------------------------------------------------------------------------------------------
                //倒傳遞階段-----------------------------------------------------------------------------------------
                //將期望結果換算成二進制
                int[] codeTemp = Encoder(td[td.Length-1]);
                double[] expect = new double[codeTemp.Length];
                for(int i = 0; i < expect.Length; i++)
                {
                    expect[i] = codeTemp[i];//獲得各輸出點期望結果
                }

                double[] deltaTemp = new double[mlp[mlp.Count - 1][0].getWeight().Length];//儲存給一上層的delta值
                //計算輸出層的delta並進入倒傳遞處理
                for(int i = 0; i < mlp[mlp.Count - 1].Length; i++)
                {               
                    double[] weight = mlp[mlp.Count - 1][i].getWeight(); //獲得上一輪權重
                    double expectDelta = expect[i] - mlp[mlp.Count - 1][i].getOutput();//計算其期望差異
                    mlp[mlp.Count - 1][i].backPropagation(expectDelta);//倒傳遞運算更新權重
                    //利用上一輪權重*DELTA計算上一層的期望差異
                    for(int j = 0; j < weight.Length; j++)
                    {
                        deltaTemp[j] += weight[j] * mlp[mlp.Count - 1][i].getDelta();
                    }
                }
                //計算輸入層跟隱藏層的delta並進入倒傳遞處理
                for (int i = mlp.Count-2; i >= 0; i--)
                {
                    double[] expectDelta = deltaTemp;//期望差異
                    for (int j = 0; j < mlp[i].Length; j++)
                    {
                        double[] weight = mlp[i][j].getWeight();//獲得上一輪權重
                        mlp[i][j].backPropagation(expectDelta[j]);//倒傳遞運算更新權重
                        //利用上一輪權重*DELTA計算上一層的期望差異
                        for (int k = 0; k < weight.Length; k++)
                        {
                            deltaTemp[k] += weight[k] * mlp[i][j].getDelta();
                        }
                    }
                }

                //--------------------------------------------------------------------------------------------------
            }
          
        }
        //--------------------------------------------------------------------------------------------------

        //測試結果-------------------------------------------------------------------------------------------
        public void Test(List<int[]> testData)
        {
            //SUM = 測試總次數  HIT = 命中次數   hitRate = 命中率
            sum = 0;
            hit = 0;
            hitRate = 0;
            double eTemp;
            rmse = 0;
            //對每一組DATA做運算
            foreach (int[] td in testData)
            {
                sum++;//計算了一筆資料
                //前饋階段-------------------------------------------------------------------------------------------  
                //輸入資料為 測試資料的 0~N-1，第N個為expect
                //輸入資料的 inputData[0] = -1 用來跟w0計算閥值
                double[] inputData = new double[td.Length];
                inputData[0] = -1;
                for (int i = 1; i < inputData.Length; i++)
                {
                    inputData[i] = td[i - 1];
                }

                //前饋開始
                foreach (Perceptron[] p in mlp)
                {
                    double[] outputTemp = new double[p.Length + 1];//多一個維度是-1 用來計算閥值
                    outputTemp[0] = -1;//第一個值為-1 用來跟w0計算閥值
                    for (int i = 0; i < p.Length; i++)
                    {
                        outputTemp[i + 1] = p[i].cal(inputData);
                    }
                    inputData = outputTemp;//這一輪的輸出結果 = 下一輪的輸入
                }


                //--------------------------------------------------------------------------------------------------
                //判斷答案階段-----------------------------------------------------------------------------------------
                //將計算結果換算成十進制
                int[] encode = new int[mlp[mlp.Count - 1].Length];
                for (int i = 0; i < mlp[mlp.Count - 1].Length; i++)
                {
                    encode[i] = (int)(mlp[mlp.Count-1][i].getOutput()+0.5);//換算成結果
                }

                int result = Decoder(encode);//換算成十進制結果
                if(result== td[td.Length - 1])//結果相同則命中
                {
                    hit++;//命中了一筆資料
                }             

                //--------------------------------------------------------------------------------------------------
             }
            hitRate = hit*100 / sum;//計算命中率
            Console.WriteLine("hit rate is : " + hitRate+" %");
        }
        //--------------------------------------------------------------------------------------------------


        //解碼器，將輸出的二進位碼解碼成十進位----------------------------------------------------------------

        public int Decoder(int[] code)
        {
            int pattern = 0;
            for(int i = 0;i<code.Length;i++)
            {
                if (code[i]==1)
                {
                    pattern += (int)Math.Pow(2, i);
                }
              
            }


            return pattern;
        }
        //--------------------------------------------------------------------------------------------------
        //加碼器，將輸入的十進位碼加碼成二進位-----------------------------------------------------------------

        public int[] Encoder(int pattern)
        {
            int[] code =new int[mlp[mlp.Count-1].Length];
            for(int i = 0; i < code.Length; i++)
            {
                code[i] = pattern % 2;
                pattern /= 2;
            }

            return code;
        }
        //--------------------------------------------------------------------------------------------------


        /*








    public double cal(double learnTemp,Perceptron[,] temp, List<Double> ex)
    {
        int count = 1;
        int ctemp = 2;
        while (ctemp < ex.Count)
        {
            count++;
            ctemp *= 2;
        }
        vT = new double[ex.Count,count + 1];
        for (int i = 0; i < vT.Length; i++)
        {
            vT[i,0] = ex[i];
            int x = i;
            for (int j = count; j > 0; j--)
            {
                if (x >= Math.Pow(2, j - 1))
                {
                    x -= (int)Math.Pow(2, j - 1);
                    vT[i,j] = 1;
                }
                else
                {
                    vT[i,j] = 0;
                }
            }

        }


        p = temp;
        learn = learnTemp;
        convergence = testc;

        if (learn == 0)
        {
            learn = testl;
        }


        dataTrain = new double[data.Length / 3 * 2,data.GetLength(1) + 1];
        dataTest = new double[data.Length - data.Length / 3 * 2,data.GetLength(1) + 1];
        maxD = -100;
        minD = 100;

        for (int i = 0; i < dataTrain.Length; i++)
        {
            for (int j = 0; j < data.GetLength(1); j++)
            {
                dataTrain[i,0] = -1;
                dataTrain[i,j + 1] = data[i,j];
            }
            if (dataTrain[i, data.GetLength(1) - 1] > maxD)
            {
                maxD = dataTrain[i, data.GetLength(1) - 1];
                // System.out.println("max:"+dataTrain[i][dataTrain[i].length-1]);
            }
            if (dataTrain[i,data.GetLength(1) - 1] < minD)
            {
                minD = dataTrain[i,data.GetLength(1) - 1];
                // System.out.println("min:"+
                // dataTrain[i][dataTrain[i].length-1]);
            }
        }
        for (int i = dataTrain.Length; i < data.GetLength(0); i++)
        {
            for (int j = 0; j < data.GetLength(1); j++)
            {
                dataTest[i - dataTrain.Length,0] = -1;
                dataTest[i - dataTrain.Length,j + 1] = data[i,j];
            }
            if (dataTest[i - dataTrain.Length,dataTest.GetLength(1) - 1] > maxD)
            {
                maxD = dataTest[i - dataTrain.Length,dataTest.GetLength(1) - 1];
                // System.out.println("max:"+maxD);
            }
            if (dataTest[i - dataTrain.Length,dataTrain.GetLength(1) - 1] < minD)
            {
                minD = dataTest[i - dataTrain.Length,dataTest.GetLength(1) - 1];
                // System.out.println("max:"+minD);
            }
        }

        // ªì©l¤Ænode
        for (int i = 0; i < p.GetLength(1); i++)
        {
            p[p.GetLength(0) - 1,i].randomInit(dataTrain.GetLength(1) - 2);
        }
        for (int i = p.GetLength(0) - 2; i > -1; i--)
        {
            for (int j = 0; j < p.GetLength(1); j++)
            {
                p[i,j].randomInit(p.GetLength(1));
            }
        }

        double e;
        double s = 0;
        rmse = 1000;
        while (rmse > convergence && s < 10000)
        {
            rmse = 0;
            s++;
            for (int d = 0; d < dataTrain.Length; d++)
            {

                e = dataTrain[d][dataTrain[d].length - 1];
                double[] eA = new double[vT[0].length];
                for (int x = 0; x < vT.Length; x++)
                {
                    if (e == vT[x][0])
                    {
                        eA = vT[x];
                        break;
                    }
                }

                rmse += train(dataTrain[d], p.length - 1, eA);
            }

            rmse /= dataTrain.length;
            System.out.println("s : " + s);
            System.out.println("rmse : " + rmse);
        }
        double ans = test(dataTest);
        System.out.println("rate: " + ans);
        return ans;
    }



    public double test(double[][] input)
    {
        for (int i = 0; i < vT.length; i++)
        {
            for (int j = 0; j < vT[0].length; j++)
            {
                System.out.print(vT[i][j] + " ");
            }
            System.out.println();
        }

        double c = 0;
        double idRate = 0;
        double[] eTemp;
        System.out.println("============================================");
        for (int i = 0; i < input.length; i++)
        {
            eTemp = testWeight(input[i], p.length - 1);
            //System.out.print("output : ");
            for (int j = 1; j < eTemp.length; j++)
            {
                if (eTemp[j] > 0.5)
                {
                    eTemp[j] = 1;
                }
                else
                {
                    eTemp[j] = 0;
                }
                ///System.out.print(eTemp[j] + " ");

            }
            //System.out.println();
            for (int v = 0; v < vT.length; v++)
            {
                for (int j = 1; j < eTemp.length; j++)
                {
                    //System.out.println(vT[v][j] + " " + eTemp[j]);
                    if (eTemp[j] != vT[v][j])
                    {
                        break;
                    }
                    if (j == eTemp.length - 1)
                    {
                        if (vT[v][0] == input[i][input[i].length - 1])
                        {
                            c++;
                        }
                    }

                }
            }

            //System.out.println("ans : "+input[i][input[i].length - 1]);

        }
        idRate = c / dataTest.length * 100;
        return idRate;
    }

    public double[] testWeight(double[] input, int layer)
    {
        double[] nextInput = new double[p[layer].length + 1];
        nextInput[0] = -1;
        for (int i = 0; i < p[layer].length; i++)
        {
            // System.out.println("²Ä" + (layer) + "¼h,²Ä" + (i + 1) + "­Ó");
            nextInput[i + 1] = p[layer][i].cal(input);
        }
        if (layer == 0)
        {
            return nextInput;
        }
        else
        {
            nextInput = testWeight(nextInput, layer - 1);
        }
        return nextInput;
    }*/
    }
}
