using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace MLP_PR
{
    //主程式
    class Program
    {
        const double learn = 0.05; //學習率  
        //程式進入點--------------------------------------------------------------------------
        static void Main(string[] args)
        {
            fileControl fc = new fileControl();//建立fileControl來進行檔案操作
            List<int[]> trainData = new List<int[]>();//存取訓練資料 int[] = {p1,p2,p3,...,pn,expect}
            trainData = fc.trainDataRead();//讀取訓練資料
            //printList(trainData);//印出訓練資料
            MLP pr = new MLP(new int[] { 100, 8 },100, learn);
            for(int i = 0; i < 200; i++)
            {
                pr.Train(trainData);
            }
           
            pr.Test(trainData);
            pr.exportWeight();
           
            Console.ReadLine();//讓console等待，不要直接結束
        }
        //------------------------------------------------------------------------------------

        //印出LIST----------------------------------------------------------------------------
        static void printList(List<int[]> list)
        {
            //逐個取用LIST中的資料
            foreach (int[] td in list)
            {
                //印出一列資料
                for (int i = 0; i < td.Length; i++)
                {
                    Console.Write(td[i]);
                }
                Console.Write("\r\n");
            }

        }
        //------------------------------------------------------------------------------------
    }
}
