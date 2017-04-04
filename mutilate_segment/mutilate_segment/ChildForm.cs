using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZedGraph;

namespace mutilate_segment
{
    public partial class ChildForm : Form
    {
        PointPairList list1 = new PointPairList();
        PointPairList list2 = new PointPairList();
        PointPairList list_Atime = new PointPairList();
        PointPairList list_Vtime = new PointPairList();
        List<PointPairList> listCircle = new List<PointPairList>();  // 存储分圈后的每一圈，圈是由段组成
        CVParameter cv = new CVParameter();
        mutilate_segment ms;
        int current_seg = 1;
        public ChildForm()
        {
              InitializeComponent();
        }
        public void setpath(string path)
        {
            list1 = new PeakData().Read(path);
        }

        private void InitialGraph(ZedGraphControl zgc, string aTitle, string str_x, string str_y)
        {
            zgc.Font = new Font("Arial", 10, FontStyle.Underline);
            //得到GraphPane的引用
            GraphPane myPane = zgc.GraphPane;

            // 设置标题 
            myPane.Title.Text = aTitle;// "Cyclic Voltammetry-CV";
            myPane.XAxis.Title.Text = str_x;// "Potential /V";
            myPane.YAxis.Title.Text = str_y;// "Current /A";

            //对图像添加灰色网格 
            myPane.XAxis.MajorGrid.IsVisible = true;
            myPane.YAxis.MajorGrid.IsVisible = true;
            myPane.XAxis.MajorGrid.Color = Color.LightGray;
            myPane.YAxis.MajorGrid.Color = Color.LightGray;

            myPane.Title.FontSpec.Size = 10.0f;
            myPane.XAxis.Title.FontSpec.Size = 10.0f;
            myPane.XAxis.Scale.FontSpec.Size = 9.0f;
            myPane.X2Axis.Title.FontSpec.Size = 10.0f;
            myPane.X2Axis.Scale.FontSpec.Size = 9.0f;
            myPane.YAxis.Title.FontSpec.Size = 10.0f;
            myPane.YAxis.Scale.FontSpec.Size = 9.0f;



            myPane.CurveList.Clear();
            generateCircle();
            Console.WriteLine("原始数据共可以分为 " + listCircle.Count + "圈");
            LineItem myCurve1;
            for (int i = 0; i < listCircle.Count; i++) {
                myCurve1 = myPane.AddCurve("result", listCircle[i], Color.Red, SymbolType.None);
                //if (i == 0)
                //    myCurve1 = myPane.AddCurve("result", listCircle[i], Color.Red, SymbolType.None);
                //else {
                //    myCurve1 = myPane.AddCurve("result", listCircle[i], Color.Black, SymbolType.None);
                //}
            }

            
        
            zgc.AxisChange();
           
        }

        private void ChildForm_Load(object sender, EventArgs e)
        {
          
            zedGraphControl1.Size = new Size(this.ClientRectangle.Width, this.ClientRectangle.Height - 22);
            zedGraphControl1.PointValueEvent += new ZedGraphControl.PointValueHandler(this.MyPointValueHandler);
            zedGraphControl1.IsShowPointValues = true;
            InitialGraph(zedGraphControl1, "", "", "");
            this.comboBox1.SelectedIndexChanged -= this.comboBox1_SelectedIndexChanged_1;   //加载数据前先删除事件监听器
            for (int i = 0; i < listCircle.Count; i++)
            {
                this.comboBox1.Items.Add(i + 1);
            }
            this.comboBox1.AutoCompleteMode = AutoCompleteMode.Append;
            this.comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBox1.SelectedIndex = 0;
            this.comboBox1.SelectedIndexChanged += this.comboBox1_SelectedIndexChanged_1;//加载数据后先再添加事件监听器
            setCV();
            ms = new mutilate_segment(list1, cv);  //list1传给构造函数，使next,previous,all等按钮能获取到list1的值

        }

        //鼠标悬停节点事件
        private string MyPointValueHandler(ZedGraphControl sender, GraphPane pane, CurveItem curve, int iPt) { 
            PointPair pt = curve[iPt];
           // curve.Color = Color.Green; 
            Console.WriteLine("curveItem中的线段 "+pane.CurveList.Count);
            Console.WriteLine("listCircle中的线段 " + listCircle.Count);
            int flag = -1;  // flag为选定的圈的下标
            for (int i = 0; i < listCircle.Count; i++) {
              PointPairList showList = listCircle.ElementAt(i);
                for (int j = 0; j < showList.Count; j++) {
                    if (showList.ElementAt(j).X == pt.X && showList.ElementAt(j).Y == pt.Y)
                    {
                        pane.CurveList[i].Color = Color.Yellow;
                        flag = i;
                        //   return "第" + (i + 1) + "圈 " + pt.X.ToString() + "," + pt.Y.ToString();  //因为i的下标是从零开始的,所以是i+1圈
                        break;
                       
                    } 

                }
                for (int other = 0; other < listCircle.Count; other++)
                    if (other != flag)
                    pane.CurveList[other].Color = Color.Black;
            }
            //for (int i = 0; i < listPointPair.Count; i++)
            //{
            //    PointPairList eachsegment = listPointPair[i];
            //    for (int j = 0; j < eachsegment.Count; j++)
            //    {
            //        if (eachsegment.ElementAt(j).X == pt.X && eachsegment.ElementAt(j).Y == pt.Y)
            //        {
            //            Console.WriteLine("第 " + (i + 1) + " 段被选中");
            //            for (int k = 0; k < eachsegment.Count; k++)
            //            {
            //                curve.AddPoint(eachsegment.ElementAt(k).X, eachsegment.ElementAt(k).Y);
            //            }
            //            curve.Color = Color.Green;
            //            LineItem mycurve1 = zedGraphControl1.GraphPane.AddCurve("result", eachsegment, Color.Blue, SymbolType.None);
            //            break;
            //        }
            //    }
            //}
            return pt.X.ToString() + "," + pt.Y.ToString();
        }

        private void setCV()
        {
            cv.InitialPotential = -0.1f;
            cv.FirstVertexPotential = 0.6f;
            cv.SecondVertexPotential = -0.2f;
            cv.FinalPotential = 0.3f;
            cv.ScanRate = 0.1f;
            cv.SweepSegments = 7;
            cv.SampleInterval = 0.01f;
            cv.Sensitivity = 5;
            cv.EnableTerminatePotential = 1f;
          

        }

        //自己定义的一个方法setCV_new，设置cv的参数，
        //setCV_new方法里的参数来源是邦哥给的图片里的参数数据
        private void setCV_new()
        {
            cv.InitialPotential = 0f;
            cv.FirstVertexPotential = 0.5f;
            cv.SecondVertexPotential = -0.1f;
            cv.FinalPotential = 0f;
            cv.ScanRate = 0.05f;
            cv.SweepSegments = 6;
            cv.SampleInterval = 0.001f; 
            cv.EnableTerminatePotential = 0f;   //代表是使能终止电位为否
            cv.FinalPotential = 0f;

        }

        private void createGraph()
        {


            GraphPane myPane = this.zedGraphControl1.GraphPane;
            myPane.CurveList.Clear();
            myPane.AddCurve("result", list2, Color.Black, SymbolType.None);
            zedGraphControl1.AxisChange();
            zedGraphControl1.Refresh();
            
        
        }

        private void createGraph(String lable, PointPairList list, Color color)
        {
            GraphPane myPane = this.zedGraphControl1.GraphPane;
            LineItem myCurve;
            myPane.CurveList.Clear();
            myCurve = myPane.AddCurve(lable, list, color, SymbolType.None);
            zedGraphControl1.AxisChange();
            zedGraphControl1.Refresh();


        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
           
           
            
            createGraph();
        }

 
        //previous
        private void button1_Click(object sender, EventArgs e)
        {
           
                int s=ms.get_previous();
                list2 = ms.get_list_segement();
                textBox1.Text = s.ToString();
                createGraph();
        }

        private void button2_Click(object sender, EventArgs e)
        {
                int s= ms.get_next();
                textBox1.Text = s.ToString();                
                list2 = ms.get_list_segement();
                createGraph(); 
        }

        private void button3_Click(object sender, EventArgs e)
        {
                int i = Int32.Parse(textBox1.Text);                
                ms.get_goto(i);
                list2 = ms.get_list_segement();
                createGraph();

         
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            list2 = ms.get_all();
            createGraph();
        }

        
        //生成时间电压图
        private void button6_Click(object sender, EventArgs e)
        {
            CreateChart(this.zedGraphControl1);

        }

        //分段算法
        //add by lq  

        private List<PointPairList> apart_segment() {
            List<PointPairList> listPointPair = new List<PointPairList>();   //存储分段后每一段
            int startPoint = 0;  //分段的起始点
            int temp = 0;       //分段的起始点和中止点之间的累计变量
                                //   int flag = 1;
            for (int i = 0; i < list1.Count; i++)
            {
                if (i == 0)
                {
                    startPoint = i;
                    Console.WriteLine("第一段初始点：" + list1.ElementAt(i).X + " " + list1.ElementAt(i).Y);
                }
                else if (i != list1.Count - 1)
                {
                    if (((list1.ElementAt(i).X > list1.ElementAt(i - 1).X) && (list1.ElementAt(i).X > list1.ElementAt(i + 1).X))
           || ((list1.ElementAt(i).X < list1.ElementAt(i - 1).X) && (list1.ElementAt(i).X < list1.ElementAt(i + 1).X)))
                    {   //得到拐点
                        int endpoint = i;
                        PointPairList ppt = new PointPairList();
                        for (temp = startPoint; temp < endpoint; temp++)
                        {
                            ppt.Add(list1.ElementAt(temp).X, list1.ElementAt(temp).Y);
                        }
                        listPointPair.Add(ppt);
                        startPoint = i;
                        //myCurve1_segment = myPane.AddCurve("segment"+flag, ppt, Color.Green, SymbolType.None);
                        //flag++;
                        Console.WriteLine("拐点坐标值：" + list1.ElementAt(i).X + " " + list1.ElementAt(i).Y);
                    }
                }
                else
                {   //在最后一段上，不考虑使能终止电位对最后一段的影响
                    PointPairList ppt = new PointPairList();
                    for (temp = startPoint; temp < list1.Count; temp++)
                    {
                        ppt.Add(list1.ElementAt(temp).X, list1.ElementAt(temp).Y);
                    }
                    listPointPair.Add(ppt);
                    // myCurve1_segment = myPane.AddCurve("segment" + flag, ppt, Color.Green, SymbolType.None);
                    //  flag++;
                    Console.WriteLine("最后一点坐标值：" + list1.ElementAt(list1.Count - 1).X + " " + list1.ElementAt(list1.Count - 1).Y);
                }


            }
            Console.WriteLine(listPointPair.Count);
            Console.WriteLine(list1.Count);
            return listPointPair;
        }


        //分圈算法
        // add by lq 
        private void generateCircle() {
            setCV_new();
            List<PointPairList> listPointPair = apart_segment();   //listPointPair存储的是每段
            if (cv.InitialPotential == cv.FirstVertexPotential)
            {  //初始电位和第一峰电位相等，则前两段为第一圈
                listPointPair.ElementAt(0).Add(listPointPair.ElementAt(1));
                listCircle.Add(listPointPair.ElementAt(0));  //添加了第一圈 
                if(listPointPair.Count - 2 > 0){
                    for (int i = 2; i < listPointPair.Count;)
                    { 
                        if (cv.EnableTerminatePotential == 1 && (cv.FinalPotential == cv.FirstVertexPotential || cv.FinalPotential == cv.SecondVertexPotential))
                        {       //  最后一圈为2段一圈
                          listPointPair.ElementAt(i).Add(listPointPair.ElementAt(i+1));
                            listCircle.Add(listPointPair.ElementAt(i));  //剩余的圈数按两段为一圈的方式添加
                            i = i + 2;
                        }
                        else
                        {  //  最后一圈为3段一圈
                            int temp = listPointPair.Count - 5;
                            if (temp > 0)  //表示第一圈和最后一圈之间还有圈数，并且一定为偶数圈
                            {
                                if (i < listPointPair.Count - 3)
                                {
                                    listPointPair.ElementAt(i).Add(listPointPair.ElementAt(i + 1));
                                    listCircle.Add(listPointPair.ElementAt(i));  //第一圈和最后一圈之间的圈数按两段为一圈的方式添加
                                    i = i + 2;
                                }
                                else {
                                    listPointPair.ElementAt(i).Add(listPointPair.ElementAt(i + 1));
                                    listPointPair.ElementAt(i).Add(listPointPair.ElementAt(i + 2));
                                    listCircle.Add(listPointPair.ElementAt(i));  //最后一圈为最后三段
                                    i = i + 3;
                                }
                            }
                            else
                            {   //此处的temp应该只能为零 ，表示第一圈和最后一圈之间没有圈数，即整个曲线只包含两圈，分别是第一圈和最后一圈，并且最后一圈由最后三段组成
                                listPointPair.ElementAt(i).Add(listPointPair.ElementAt(i + 1));
                                listPointPair.ElementAt(i).Add(listPointPair.ElementAt(i + 2));
                                listCircle.Add(listPointPair.ElementAt(i));  //最后一圈为最后三段
                                i = i + 3;
                            }
                        }
                    }
                }
             

            }
            else {
                //初始电位和第一峰电位不相等，则前三段为第一圈
                listPointPair.ElementAt(0).Add(listPointPair.ElementAt(1));
                listPointPair.ElementAt(0).Add(listPointPair.ElementAt(2));
                listCircle.Add(listPointPair.ElementAt(0));  //添加了第一圈 
                if (listPointPair.Count - 3 > 0)
                {
                    for (int i = 3; i < listPointPair.Count;)
                    {
                        if (cv.EnableTerminatePotential == 1 && (cv.FinalPotential == cv.FirstVertexPotential || cv.FinalPotential == cv.SecondVertexPotential))
                        {       //  最后一圈为2段一圈
                            listPointPair.ElementAt(i).Add(listPointPair.ElementAt(i + 1));
                            listCircle.Add(listPointPair.ElementAt(i));  //剩余的圈数按两段为一圈的方式添加
                            i = i + 2;
                        }
                        else
                        {  //  最后一圈为3段一圈
                            int temp = listPointPair.Count - 6;
                            if (temp > 0)  //表示第一圈和最后一圈之间还有圈数，并且一定为偶数圈
                            {
                                if (i < listPointPair.Count - 3)
                                {
                                    listPointPair.ElementAt(i).Add(listPointPair.ElementAt(i + 1));
                                    listCircle.Add(listPointPair.ElementAt(i));  //第一圈和最后一圈之间的圈数按两段为一圈的方式添加
                                    i = i + 2;
                                }
                                else
                                {
                                    listPointPair.ElementAt(i).Add(listPointPair.ElementAt(i + 1));
                                    listPointPair.ElementAt(i).Add(listPointPair.ElementAt(i + 2));
                                    listCircle.Add(listPointPair.ElementAt(i));  //最后一圈为最后三段
                                    i = i + 3;
                                }
                            }
                            else
                            {   //此处的temp应该只能为零 ，表示第一圈和最后一圈之间没有圈数，即整个曲线只包含两圈，分别是第一圈和最后一圈，并且最后一圈由最后三段组成
                                listPointPair.ElementAt(i).Add(listPointPair.ElementAt(i + 1));
                                listPointPair.ElementAt(i).Add(listPointPair.ElementAt(i + 2));
                                listCircle.Add(listPointPair.ElementAt(i));  //最后一圈为最后三段
                                i = i + 3;
                            }
                        }
                    }
                }
            }
        }
       
        public void CreateChart(ZedGraphControl zgc)
        {

            GraphPane myPane = zgc.GraphPane;

            // Set the titles and axis labels
            myPane.Title.Text = "电压-时间-电流";
            myPane.XAxis.Title.Text = "Time, s";
            myPane.YAxis.Title.Text = "电压, V";
            myPane.Y2Axis.Title.Text = "电流, A";
            myPane.CurveList.Clear();
            setCV_new();
            list_Atime.Clear();  //清除上一次点击生成的数据
            for (int i = 0; i < list1.Count; i++)
            {
                double x_temp = (i * cv.SampleInterval) / cv.ScanRate;
                list_Atime.Add(x_temp, list1.ElementAt(i).Y);
            }
            list_Vtime.Clear();
            for (int i = 0; i < list1.Count; i++)
            {
                double x_temp = (i * cv.SampleInterval) / cv.ScanRate;
                list_Vtime.Add(x_temp, list1.ElementAt(i).X);
            }

            // Generate a red curve with diamond symbols, and "Velocity" in the legend
            LineItem myCurve = myPane.AddCurve("V-T",
               list_Vtime, Color.Red, SymbolType.None);
            // Fill the symbols with white
            myCurve.Symbol.Fill = new Fill(Color.White);

            // Generate a blue curve with circle symbols, and "Acceleration" in the legend
            myCurve = myPane.AddCurve("A-T",
               list_Atime, Color.Blue, SymbolType.None);
            // Fill the symbols with white
            myCurve.Symbol.Fill = new Fill(Color.White);
            // Associate this curve with the Y2 axis
            myCurve.IsY2Axis = true;
            // Show the x axis grid
            myPane.XAxis.MajorGrid.IsVisible = true;

            // Make the Y axis scale red
            myPane.YAxis.Scale.FontSpec.FontColor = Color.Red;
            myPane.YAxis.Title.FontSpec.FontColor = Color.Red;
            // turn off the opposite tics so the Y tics don't show up on the Y2 axis
            myPane.YAxis.MajorTic.IsOpposite = false;
            myPane.YAxis.MinorTic.IsOpposite = false;
            // Don't display the Y zero line
            myPane.YAxis.MajorGrid.IsZeroLine = false;
            // Align the Y axis labels so they are flush to the axis
            myPane.YAxis.Scale.Align = AlignP.Inside; 
            // Enable the Y2 axis display
            myPane.Y2Axis.IsVisible = true;
            // Make the Y2 axis scale blue
            myPane.Y2Axis.Scale.FontSpec.FontColor = Color.Blue;
            myPane.Y2Axis.Title.FontSpec.FontColor = Color.Blue;
            // turn off the opposite tics so the Y2 tics don't show up on the Y axis
            myPane.Y2Axis.MajorTic.IsOpposite = false;
            myPane.Y2Axis.MinorTic.IsOpposite = false;
            // Display the Y2 axis grid lines
            myPane.Y2Axis.MajorGrid.IsVisible = true;
            // Align the Y2 axis labels so they are flush to the axis
            myPane.Y2Axis.Scale.Align = AlignP.Inside; 

            // Fill the axis background with a gradient
            myPane.Chart.Fill = new Fill(Color.White, Color.LightGoldenrodYellow, 45.0f);

            zgc.AxisChange();
            zgc.Refresh();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            

            createGraph("第"+this.comboBox1.Text+"圈",listCircle[this.comboBox1.SelectedIndex],Color.Blue);
            //在下拉框的值选择后删除MyPointValueHandler时间的监听，若不删除，当悬浮于选择的目标曲线时会报异常
            zedGraphControl1.PointValueEvent -= new ZedGraphControl.PointValueHandler(this.MyPointValueHandler);
        }
    }
}
