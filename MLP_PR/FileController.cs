using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace MLP_PR
{
    /*控制檔案輸入輸出用，此處用來讀取TRAINING DATA並輸出訓練好的權重*/
    public class fileControl
    {
        //宣告-------------------------------------------------------------------------------------------
        StreamWriter swWriter;
        StreamReader srReader;
        const string FILE_TRAIN = "train.data";//TRAIN DATA的檔案名稱
        const string FILE_WEIGHT = "weight.data";//WEIGHT DATA的檔案名稱
        //--------------------------------------------------------------------------------------------------
        //寫入WEIGHT檔案-------------------------------------------------------------------------------------------
        public void weightDataWrite(List<string> weightData)
        {
            try
            {
                swWriter = new StreamWriter(FILE_WEIGHT);//建立streamWriter     
                foreach(string wString in weightData)
                {
                    swWriter.WriteLine(wString); //寫入數據
                }         
               
                swWriter.Close();//關閉streamWriter
            }
            catch (Exception e)//例外處理
            {

                throw e;
            }
        }
        //--------------------------------------------------------------------------------------------------
        //讀取WEIGHT檔案-------------------------------------------------------------------------------------------
        public List<double[]> WeightDataRead()
        {
            List<double[]> rList = new List<double[]>();//回傳此list
            string sLine = "";//用來暫存每一行資料
            try
            {
                srReader = new StreamReader(FILE_WEIGHT);//建立streamReader

                //逐行讀取DATA
                while ((sLine = srReader.ReadLine()) != null)
                {
                    string[] temp = sLine.Split(' ');//將讀取的DATA依空白分隔
                    double[] data = new double[temp.Length];//用來暫存轉成double格式的DATA
                    //根據分隔開的每一個string轉換成double
                    for (int i = 0; i < temp.Length; i++)
                    {
                        double.TryParse(temp[i], out data[i]);
                    }
                    rList.Add(data);//將DATA存入回傳用LIST
                }
                srReader.Close();//關閉streamReader
            }
            catch (Exception e)//例外處理
            {
                throw e;
            }
            return rList;

        }
        //--------------------------------------------------------------------------------------------------
        //讀取TRAIN檔案-------------------------------------------------------------------------------------------
        public List<int[]> trainDataRead()
        {
            List<int[]> rList = new List<int[]>();//回傳此list
            string sLine = "";//用來暫存每一行資料
            try
            {
                srReader = new StreamReader(FILE_TRAIN);//建立streamReader

                //逐行讀取DATA
                while ((sLine = srReader.ReadLine()) != null)
                {
                    string[] temp = sLine.Split(' ');//將讀取的DATA依空白分隔
                    int[] data = new int[temp.Length];//用來暫存轉成INT格式的DATA
                    //根據分隔開的每一個string轉換成INT
                    for(int i = 0; i < temp.Length; i++)
                    {
                        Int32.TryParse(temp[i], out data[i]);
                    }
                    rList.Add(data);//將DATA存入回傳用LIST
                }
                srReader.Close();//關閉streamReader
            }
            catch (Exception e)//例外處理
            {
                throw e;
            }
            return rList;

        }
        //--------------------------------------------------------------------------------------------------

    }

}
