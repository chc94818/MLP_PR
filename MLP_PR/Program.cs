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
            trainData = fc.fileRead();//讀取訓練資料
            //printList(trainData);//印出訓練資料
            MLP pr = new MLP(new int[] { 100, 8 },100, learn);
            for(int i = 0; i < 200; i++)
            {
                pr.Train(trainData);
            }
           
            pr.Test(trainData);
            /* Decoder and Encoder test
            int[] testCode = { 0, 1, 0, 0, 0, 0, 0, 0, 0, 0 };
            Console.Write(pr.Decoder(testCode));
            Console.Write("\r\n");
            testCode = pr.Encoder(255);
            for(int i = testCode.Length-1; i >= 0; i--)
            {
                Console.Write(testCode[i]);
            }
            Console.Write("\r\n");
            */
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
