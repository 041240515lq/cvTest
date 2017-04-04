using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZedGraph;

namespace mutilate_segment
{
    class mutilate_segment
    {

        PointPairList list_all;
        PointPairList list_part;
        int current_no;
        CVParameter cv;
        public mutilate_segment(PointPairList list1,CVParameter a_cv)
        {
            cv = a_cv;
            list_all = list1;
            current_no = 1;
            list_part = new PointPairList();
        }
        

        public void get_goto(int no)
        {
            if (current_no < cv.SweepSegments + 1 && current_no > 0)
            { 
                current_no = no;
                get_segment();
              
            }
            
        }
        public int get_next()
        {
            if (current_no<cv.SweepSegments)
            {
                current_no++;
            }
            get_segment();
            return current_no;
        }
        public int get_previous()
        {
            if (current_no > 1)
            {
                current_no--;
            }
            get_segment();
            return current_no;
            
        }
        public PointPairList get_list_segement()
        {
            return list_part;
        
        }
        public PointPairList get_all()
        {
            return list_all;
        }

        public  void get_segment()
        {

            int begin_index = 0;
            int end_index = 0;
            if (current_no == 1)//第一段
            {
                begin_index = 0;
                int t = (int)(Math.Abs(cv.FirstVertexPotential - cv.InitialPotential) / cv.SampleInterval);
                end_index = t;

            }
            else if (current_no == (cv.SweepSegments))
            {
                if (cv.SweepSegments % 2 == 0)//偶数段
                {

                    if (cv.EnableTerminatePotential > 0)
                    {
                        int t = (int)(Math.Abs(cv.FirstVertexPotential - cv.FinalPotential) / cv.SampleInterval);
                        begin_index = list_all.Count - t - 1;
                        end_index = list_all.Count;

                    }
                    else
                    {
                        int t = (int)(Math.Abs(cv.SecondVertexPotential - cv.FirstVertexPotential) / cv.SampleInterval);
                        begin_index = list_all.Count - t - 1;
                        end_index = list_all.Count;

                    }
                }
                else
                {
                    if (cv.EnableTerminatePotential > 0)
                    {
                        int t = (int)(Math.Abs(cv.SecondVertexPotential - cv.FinalPotential) / cv.SampleInterval);
                        begin_index = list_all.Count - t - 1;
                        end_index = list_all.Count;
                        Console.WriteLine(t.ToString());

                    }
                    else
                    {
                        int t = (int)(Math.Abs(cv.SecondVertexPotential - cv.FirstVertexPotential) / cv.SampleInterval);
                        begin_index = list_all.Count - t - 1;
                        end_index = list_all.Count;
                           
                    }
                }


            }
            else //中间段
            {
                int t = (int)(Math.Abs(cv.SecondVertexPotential - cv.FirstVertexPotential) / cv.SampleInterval);
                int first = (int)(Math.Abs(cv.FirstVertexPotential - cv.InitialPotential) / cv.SampleInterval) + 1;
                begin_index = first + (current_no-2) * t;
                end_index = first + (current_no-1) * t;
            }
            list_part.Clear();
            for (int i = begin_index; i < end_index; i++)
            {
                list_part.Add(list_all[i]);
            }

        }
    }     
}
