using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TLBO
{
    public partial class Form5 : Form
    {
        public Form5()
        {
            InitializeComponent();
            dataGridView1.RowCount = 6;
            dataGridView1.Rows[0].Cells[0].Value = "Размер начальной популяции";
            dataGridView1.Rows[1].Cells[0].Value = "Kоличество итераций";
            dataGridView1.Rows[2].Cells[0].Value = "Пороговый вес";
            dataGridView1.Rows[3].Cells[0].Value = "Максимальный вес";
            dataGridView1.Rows[4].Cells[0].Value = "stepInd0";
            dataGridView1.Rows[5].Cells[0].Value = "stepInd1";
         
        }
        public Algoritm algst = new Algoritm();
        public List<Algoritm.Student> Population;

        public void FillList()
        {
            algst.RecalcMaxf();
            for (int i = 0; i < Population.Count; i++)
            {
                ListViewItem Item = new ListViewItem();
                Item.SubItems.Add((i + 1).ToString());
                Item.SubItems.Add(Math.Round(Population[i].x[0], 4).ToString());
                Item.SubItems.Add(Math.Round(Population[i].x[1], 4).ToString());
                Item.SubItems.Add(Math.Round(Population[i].f, 4).ToString());
                listView1.Items.Add(Item);
            }
            
        }

        public void FillData(int a1,int a2,double a3,double a4,double a5,double a6)
        {
            dataGridView1.Rows[0].Cells[1].Value = a1;
            dataGridView1.Rows[1].Cells[1].Value = a2;
            dataGridView1.Rows[2].Cells[1].Value = a3;
            dataGridView1.Rows[3].Cells[1].Value = a4;
            dataGridView1.Rows[4].Cells[1].Value = a5;
            dataGridView1.Rows[5].Cells[1].Value = a6;
           
        
        
        }


        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
