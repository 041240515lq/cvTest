using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace mutilate_segment
{
    public partial class Form1 : Form
    {

        ChildForm cf;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
         
    
        }

        private void openToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = "数据文件|*.txt";
            of.AddExtension = true;  //设置标题  
            of.Title = "打开文件";  //如果用户点击了打开按钮
            
            of.RestoreDirectory = true;
            of.Multiselect = false;
            if (of.ShowDialog() == DialogResult.OK)
            {
                cf = new ChildForm();
                cf.MdiParent = this;
                cf.setpath(of.FileName);
                cf.Show();

            }
        }
    }
}
