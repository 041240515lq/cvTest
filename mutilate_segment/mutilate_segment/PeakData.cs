using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZedGraph;
using System.Windows.Forms;
using System.IO;

namespace mutilate_segment
{
    class PeakData
    {
        private PointPairList list1 = new PointPairList();


        public PointPairList Read(string filePath)
        {
            try
            {
                string[] stringlines = File.ReadAllLines(filePath, Encoding.Default);


              
                foreach (string s in stringlines)
                {
                    double x, y;
                    String[] getXY = s.Split(' ');
                    x = Convert.ToDouble(getXY[0].Trim());
                    y = Convert.ToDouble(getXY[1].Trim());
                   // Console.WriteLine(x+"|"+y);                   
                  //  Console.WriteLine(s);
                    list1.Add(x, y);
                }



 

           

            }
            catch (IOException e)
            {
                Console.WriteLine(e.ToString());
            }

            return list1;
        }

    }
}
