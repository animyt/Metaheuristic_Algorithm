using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskBand;

namespace TLBO
{
    public partial class Form4 : Form
    {

        public Form4()
        {
            InitializeComponent();
            InitDataGridView1();
            button16.Enabled = false;
        }

        public Algoritm algst = new Algoritm();
        public string Fname = "";
        public double[,] showobl = new double[2, 2];
        public int z;
        public float[] Ar = new float[8];
        public bool[] flines = new bool[8];
        public double exact = 0;
        public float[] A = new float[8];
        public double[,] showoblbase = new double[2, 2];
        public double[,] oblbase = new double[2, 2];

        bool flag = false;
        bool[] Red = new bool[10];
        Random R = new Random();

        int t = 0;



        public void order_pop()
        {
            //сортировка
            double temp;

            for (int i = 0; i < algst.Population.Count() - 1; i++)
            {
                for (int j = i + 1; j < algst.Population.Count(); j++)
                {
                    if (algst.Population[i].f < algst.Population[j].f)
                    {
                        temp = algst.Population[i].f;
                        algst.Population[i].f = algst.Population[j].f;
                        algst.Population[j].f = temp;


                        /*
                        temp = algst.Population[i].f;
                        algst.Population[i].f = algst.Population[j].f;
                        algst.Population[j].f = temp;
                         */
                    }
                }
            }

        }

        public void block_button()
        {
            button1.Enabled = false;
            button5.Enabled = false;
            button6.Enabled = false;
            
            button8.Enabled = false;

            button12.Enabled = false;

        }

        public void unblock_button()
        {
            button1.Enabled = true;
            button5.Enabled = true;
            button6.Enabled = true;
            
            button8.Enabled = true;

            button12.Enabled = true;


        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

            this.Close();
        }



        private void UpdateParamView()
        {
            //order_pop();
            // m = algst.Population.Count;
            dataGridView1.Rows[0].Cells[1].Value = t; 
            dataGridView1.Rows[1].Cells[1].Value = algst.Population.Count; 
            dataGridView1.Rows[2].Cells[1].Value = groupSize; 
            dataGridView1.Rows[3].Cells[1].Value = groupCount; 
            dataGridView1.Rows[4].Cells[1].Value = weight; 
            
            // Средняя приспособленность
            if (algst.AverF.Count > 0 && t < algst.AverF.Count)
            {
                dataGridView1.Rows[5].Cells[1].Value = Math.Round(algst.AverF[t], 6);
            }
            else
            {
                dataGridView1.Rows[5].Cells[1].Value = "N/A";
            }

            

            // Лучшее решение (координаты и значение)
            if (algst.Population.Count > 0)
            {
                var best = algst.Population.OrderByDescending(s => s.f).FirstOrDefault();
                if (best != null)
                {
                    dataGridView1.Rows[6].Cells[1].Value = "(" + Math.Round(best.x[0], 4).ToString() + "; " +
                    Math.Round(best.x[1], 4).ToString() + ")"; // (x*, y*) Лучшее решение
                    dataGridView1.Rows[7].Cells[1].Value = Math.Round(best.f, 6).ToString(); // f* Значение лучшего решения
                    dataGridView1.Rows[8].Cells[1].Value = exact.ToString(); // Точное решение
                    dataGridView1.Rows[9].Cells[1].Value = Math.Round((double)Math.Abs(exact - best.f), 6).ToString(); // Отклонение от точного решения
                    

                    

                }
                else
                {
                    dataGridView1.Rows[6].Cells[1].Value = "N/A";
                    dataGridView1.Rows[7].Cells[1].Value = "N/A";
                    dataGridView1.Rows[8].Cells[1].Value = "N/A";
                    dataGridView1.Rows[9].Cells[1].Value = "N/A";
                    
                }
            }
            else
            {
                dataGridView1.Rows[6].Cells[1].Value = "N/A";
                dataGridView1.Rows[7].Cells[1].Value = "N/A";
                dataGridView1.Rows[8].Cells[1].Value = "N/A";
                dataGridView1.Rows[9].Cells[1].Value = "N/A";
                
            }
        }


        private void InitDataGridView1()
        {
            dataGridView2.RowCount = 4;
            dataGridView2.Rows[0].Cells[0].Value = "(x*, y*)";
            dataGridView2.Rows[1].Cells[0].Value = "f*";
            dataGridView2.Rows[2].Cells[0].Value = "Точное решение:";
            dataGridView2.Rows[3].Cells[0].Value = "Отклонение от точного решения:";


            dataGridView1.RowCount = 10;
            dataGridView1.Rows[0].Cells[0].Value = "Номер итерации:";
            dataGridView1.Rows[1].Cells[0].Value = "Количество человек в группе:";
            dataGridView1.Rows[2].Cells[0].Value = "Количество групп:";
            dataGridView1.Rows[3].Cells[0].Value = "Всего человек:";
            dataGridView1.Rows[4].Cells[0].Value = "Влияние прошлой позиции:";            
            dataGridView1.Rows[5].Cells[0].Value = "Среднее значение f:";            
            dataGridView1.Rows[6].Cells[0].Value = "(x*, y*) Лучшее решение:";
            dataGridView1.Rows[7].Cells[0].Value = "f* Значение лучшего решения:";
            dataGridView1.Rows[8].Cells[0].Value = "Точное решение:";
            dataGridView1.Rows[9].Cells[0].Value = "Отклонение от точного решения:";
            
        }

        private void pictureBox3_Paint(object sender, PaintEventArgs e)
        {
            Pen p1 = new Pen(Color.Black, 2);
            Pen p2 = new Pen(Color.Gray, 2);
            Pen p3 = new Pen(Color.Red, 2);
            Pen p4 = new Pen(Color.Gray, 1);
            Font f1 = new Font("TimesNewRoman", 12, FontStyle.Bold);
            Font f2 = new Font("TimesNewRoman", 9);





            if (Red[0] == true)
            {
                e.Graphics.DrawLine(p3, 58, 30, 58, 147);
                e.Graphics.DrawLine(p3, 58, 145, 63, 135);
                e.Graphics.DrawLine(p3, 57, 145, 52, 135);
            }

            else
            {
                e.Graphics.DrawLine(p2, 58, 30, 58, 147);
                e.Graphics.DrawLine(p2, 58, 145, 63, 135);
                e.Graphics.DrawLine(p2, 57, 145, 52, 135);
            }

            if (Red[1] == true)
            {
                e.Graphics.DrawLine(p3, 96, 165, 251, 165);
                e.Graphics.DrawLine(p3, 248, 165, 238, 170);
                e.Graphics.DrawLine(p3, 248, 164, 238, 159);
            }
            else
            {
                e.Graphics.DrawLine(p2, 96, 165, 251, 165);
                e.Graphics.DrawLine(p2, 248, 165, 238, 170);
                e.Graphics.DrawLine(p2, 248, 164, 238, 159);
            }






            



            if ((((Red[9] == true) )||((Red[9] == true)&& ((t - 1) == 0)))||  (Red[9] == true))
            {
                e.Graphics.DrawLine(p3, 270, 205, 96, 205);
                e.Graphics.DrawLine(p3, 108, 205, 118, 210);
                e.Graphics.DrawLine(p3, 108, 204, 118, 199);
            }
            else
            {
                e.Graphics.DrawLine(p2, 270, 205, 96, 205);
                e.Graphics.DrawLine(p2, 108, 205, 118, 210);
                e.Graphics.DrawLine(p2, 108, 204, 118, 199);
            }




            




            e.Graphics.DrawLine(p2, 0, 233, 400, 233);
            e.Graphics.DrawLine(p2, 0, 2, 400, 2);
            e.Graphics.DrawLine(p2, 362, 2, 362, 380);
            e.Graphics.DrawLine(p2, 1, 2, 1, 380);
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        public int Ip;
        public int K;
        public int groupSize;
        public int groupCount;
        public double Teach;
        public double Stud;
        public double[,] obl;
        public int klon;
        public double parkl;
        public int n;
        public double weight;
        
        
        

        //public double eps;
        //public double sigma;
        //public double g;
        //public int percent;

        public int numb;
        int m = 0;
        bool endreport = false;


        public void order_button()
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            dataGridView2.Rows[0].Cells[1].Value = " ";
            dataGridView2.Rows[1].Cells[1].Value = " ";
            dataGridView2.Rows[2].Cells[1].Value = " ";
            dataGridView2.Rows[3].Cells[1].Value = " ";

            //создание начальной популяции
            endreport = false;
            button16.Enabled = false;
            button15.Enabled = true;
            unblock_button();
            Red[0] = true;
            for (int i = 1; i < 10; i++)
                Red[i] = false;
            m = 0;
            algst = new Algoritm(); //
            algst.Init(
    k: K,
    obl: obl,
    n: n,
    z_: z,
    groupSize: groupSize,
    groupCount: groupCount,
    weight: weight
);

            algst.Create_Pop();
            algst.AssignRoles();
            flag = true;
            t = 1;
            // Заполнение dataGridView1 начальными значениями
            dataGridView1.Rows[0].Cells[1].Value = t; 
            dataGridView1.Rows[1].Cells[1].Value = algst.Population.Count;
            dataGridView1.Rows[2].Cells[1].Value = groupSize;
            dataGridView1.Rows[3].Cells[1].Value = groupCount;
            dataGridView1.Rows[4].Cells[1].Value = weight;
            

            // Вычисляем и отображаем среднюю приспособленность
            if (algst.AverF.Count > 0)
            {
                dataGridView1.Rows[5].Cells[1].Value = Math.Round(algst.AverF[t-1], 6); // Средняя приспособленность
            }
            else
            {
                dataGridView1.Rows[5].Cells[1].Value = "N/A";
            }

            

            // Отображаем лучшее решение (координаты и значение)
            if (algst.Population.Count > 0)
            {
                var best = algst.Population.OrderByDescending(s => s.f).FirstOrDefault();
                if (best != null)
                {
                    dataGridView1.Rows[6].Cells[1].Value = "(" + Math.Round(best.x[0], 4).ToString() + "; " + Math.Round(best.x[1], 4).ToString() + ")"; // (x*, y*) Лучшее решение
                    dataGridView1.Rows[7].Cells[1].Value = Math.Round(best.f, 6).ToString(); // Значение лучшего решения
                    dataGridView1.Rows[8].Cells[1].Value = exact.ToString(); // Точное решение
                    dataGridView1.Rows[9].Cells[1].Value = Math.Round((double)Math.Abs(exact - best.f), 6).ToString(); // Отклонение от точного решения
                    

                }
            }
            else
            {
                dataGridView1.Rows[6].Cells[1].Value = "N/A";
                dataGridView1.Rows[7].Cells[1].Value = "N/A";
                dataGridView1.Rows[8].Cells[1].Value = "N/A";
                dataGridView1.Rows[9].Cells[1].Value = "N/A";
                
            }

            numb++;

            if ((File.Exists("protocol.dt")))
            {
                Report prot = new Report();
                prot.Prot1(z, Ip, K, algst.Population, algst.oblast, numb);
                if ((File.Exists("protocol.dt")))
                {
                    FileStream fs = new FileStream("protocol.dt", FileMode.Append, FileAccess.Write);
                    StreamWriter r1 = new StreamWriter(fs);
                    r1.Write(prot.toprotocol);
                    r1.Close();
                    fs.Close();
                }
            }

            if (Red[1] == true)
            {
                MessageBox.Show("Перейдите к шагу \"Фаза учеников\".");
            }
            
            


            pictureBox3.Refresh();
            pictureBox1.Refresh();
            algst.RecalcMaxf();
            pictureBox2.Refresh();

        }

        private void Form4_Load(object sender, EventArgs e)
        {
            label1.Text = Fname + " (изображение линий уровня) и популяции:";
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            float w = pictureBox1.Width;
            float h = pictureBox1.Height;
            float x0 = w / 2;
            float y0 = h / 2;
            float a = 30;
            float k = 1;


            List<PointF> points = new List<PointF>();
            Pen p1 = new Pen(Color.PaleGreen, 1);
            Pen p2 = new Pen(Color.GreenYellow, 1);
            Pen p3 = new Pen(Color.YellowGreen, 1);
            Pen p4 = new Pen(Color.Yellow, 1);
            Pen p5 = new Pen(Color.Orange, 1);
            Pen p6 = new Pen(Color.OrangeRed, 1);
            Pen p7 = new Pen(Color.Red, 1);
            Pen p8 = new Pen(Color.Brown, 1);
            Pen p9 = new Pen(Color.Maroon, 1);
            Pen p10 = new Pen(Color.Black, 1);
            Pen p11 = new Pen(Color.Blue, 4);

            Font font1 = new Font("TimesNewRoman", 10, FontStyle.Bold);
            Font font2 = new Font("TimesNewRoman", 8);

            pictureBox1.BackColor = Color.White;

            //if (flag == true)
            {
                double x1 = showobl[0, 0];
                double x2 = showobl[0, 1];
                double y1 = showobl[1, 0];
                double y2 = showobl[1, 1];



                // z = comboBox1.SelectedIndex;
                // flines = forma3.flines;
                // Ar = forma3.Ar;
                double a1 = Ar[0];//5
                double a3 = Ar[1];//4
                double a5 = Ar[2];//3
                double a7 = Ar[3];//2
                double a9 = Ar[4];//1

                double a10 = Ar[5];//6
                double a11 = Ar[6];//7
                double a12 = Ar[7];//8

                double dx = x2 - x1;
                double dy = y2 - y1;
                double dxy = dx - dy;


                double bxy = Math.Max(dx, dy);
                double step = 0;
                if (bxy < 1.1) step = 0.1;
                else if (bxy < 2.1) step = 0.2;
                else if (bxy < 5.1) step = 0.5;
                else if (bxy < 10.1) step = 1;
                else if (bxy < 20.1) step = 2;
                else if (bxy < 50.1) step = 5;
                else if (bxy < 100.1) step = 10;
                else if (bxy < 200.1) step = 20;
                else if (bxy < 500.1) step = 50;
                else if (bxy < 1000.1) step = 100;
                else if (bxy < 2000.1) step = 200;
                else step = 1000;

                //if (dxy > 0)
                //{
                //    y1 = y1 - dxy / 2;
                //    y2 = y2 + dxy / 2;
                //}
                //else if (dxy < 0)
                //{
                //    x1 = x1 - Math.Abs(dxy) / 2;
                //    x2 = x2 + Math.Abs(dxy) / 2;
                //}
                //x1 = x1 - 0.05 * Math.Abs(x2 - x1);
                //x2 = x2 + 0.05 * Math.Abs(x2 - x1);
                //y1 = y1 - 0.05 * Math.Abs(y2 - y1);
                //y2 = y2 + 0.05 * Math.Abs(y2 - y1);


                double x30 = 7.2;
                double x40 = 1;
                //double x30 = 0;
                //double x40 = 0;

                //if (algst.Population.Count > 0)
                //{
                //    for (int i = 0; i < algst.Population.Count; i++)
                //    {
                //        var fish = algst.Population[i];
                //        x30 += fish.x[2];
                //        x40 += fish.x[3];
                //    }
                //    x30 /= algst.Population.Count;
                //    x40 /= algst.Population.Count;
                //}
                //else
                //{
                //    x30 = x40 = 1;
                //}

                float mw = k * (w) / ((float)(x2 - x1));
                float mh = k * (h) / ((float)(y2 - y1));
                //for (int i = (x1); i < x2; i++)
                // for (int j = (y1); j < y2; j++)
                for (int ii = 0; ii < w; ii++)
                    for (int jj = 0; jj < h; jj++)
                    {
                        //double mj = j * mh;
                        // double ni = i * mw;
                        //double i = (ii * (Math.Max(x2 - x1, y2 - y1)) / w + x1) / k;
                        //double j = (jj * (Math.Max(x2 - x1, y2 - y1)) / h + y1) / k;
                        //double i1 = ((ii + 1) * (Math.Max(x2 - x1, y2 - y1)) / w + x1) / k;
                        //double j1 = ((jj + 1) * (Math.Max(x2 - x1, y2 - y1)) / h + y1) / k;
                        //double i0 = ((ii - 1) * (Math.Max(x2 - x1, y2 - y1)) / w + x1) / k;
                        //double j0 = ((jj - 1) * (Math.Max(x2 - x1, y2 - y1)) / h + y1) / k;
                        double i = (ii * (x2 - x1) / w + x1) / k;
                        double j = (jj * (y2 - y1) / h + y1) / k;
                        double i1 = ((ii + 1) * (x2 - x1) / w + x1) / k;
                        double j1 = ((jj + 1) * (y2 - y1) / h + y1) / k;
                        double i0 = ((ii - 1) * (x2 - x1) / w + x1) / k;
                        double j0 = ((jj - 1) * (y2 - y1) / h + y1) / k;
                        double f = function(i, j, x30, x40, z);// j / k * Math.Sin(Math.Sqrt(Math.Abs(j / k))) + i / k * Math.Sin(Math.Sqrt(Math.Abs(i / k)));
                        double f2 = function(i0, j, x30, x40, z); //(j - 1) / k * Math.Sin(Math.Sqrt(Math.Abs(j - 1) / k)) + i / k * Math.Sin(Math.Sqrt(Math.Abs(i / k)));
                        double f3 = function(i, j0, x30, x40, z); //(j / k) * Math.Sin(Math.Sqrt(Math.Abs(j / k))) + (i - 1) / k * Math.Sin(Math.Sqrt(Math.Abs((i - 1) / k)));
                        double f4 = function(i1, j, x30, x40, z); //(j + 1) / k * Math.Sin(Math.Sqrt(Math.Abs(j + 1) / k)) + (i / k) * Math.Sin(Math.Sqrt(Math.Abs(i / k)));
                        double f5 = function(i, j1, x30, x40, z); //(j / k) * Math.Sin(Math.Sqrt(Math.Abs(j / k))) + (i + 1) / k * Math.Sin(Math.Sqrt(Math.Abs((i + 1) / k)));
                        double f6 = function(i1, j1, x30, x40, z); //(j + 1) / k * Math.Sin(Math.Sqrt(Math.Abs(j + 1) / k)) + (i + 1) / k * Math.Sin(Math.Sqrt(Math.Abs((i + 1) / k)));
                        double f7 = function(i0, j1, x30, x40, z); //(j - 1) / k * Math.Sin(Math.Sqrt(Math.Abs(j - 1) / k)) + (i + 1) / k * Math.Sin(Math.Sqrt(Math.Abs((i + 1) / k)));
                        double f8 = function(i1, j0, x30, x40, z); //(j + 1) / k * Math.Sin(Math.Sqrt(Math.Abs(j + 1) / k)) + (i - 1) / k * Math.Sin(Math.Sqrt(Math.Abs((i - 1) / k)));
                        double f9 = function(i0, j0, x30, x40, z); //(j - 1) / k * Math.Sin(Math.Sqrt(Math.Abs(j - 1) / k)) + (i - 1) / k * Math.Sin(Math.Sqrt(Math.Abs((i - 1) / k)));


                        // if (((f2 < a1) || (f3 < a1) || (f4 < a1) || (f5 < a1) || (f6 < a1) || (f7 < a1) || (f8 < a1) || (f9 < a1)) && (f > a1)) e.Graphics.DrawRectangle(p1, (float)(x0 + mw * j / k), (float)(y0 - mh * i / k), 1, 1);
                        // else if (((f2 < a3) || (f3 < a3) || (f4 < a3) || (f5 < a3) || (f6 < a3) || (f7 < a3) || (f8 < a3) || (f9 < a3)) && (f > a3)) e.Graphics.DrawRectangle(p3, (float)(x0 + mw * j / k), (float)(y0 - mh * i / k), 1, 1);
                        // else if (((f2 < a5) || (f3 < a5) || (f4 < a5) || (f5 < a5) || (f6 < a5) || (f7 < a5) || (f8 < a5) || (f9 < a5)) && (f > a5)) e.Graphics.DrawRectangle(p5, (float)(x0 + mw * j / k), (float)(y0 - mh * i / k), 1, 1);
                        // else if (((f2 < a7) || (f3 < a7) || (f4 < a7) || (f5 < a7) || (f6 < a7) || (f7 < a7) || (f8 < a7) || (f9 < a7)) && (f > a7)) e.Graphics.DrawRectangle(p7, (float)(x0 + mw * j / k), (float)(y0 - mh * i / k), 1, 1);
                        // else if (((f2 < a9) || (f3 < a9) || (f4 < a9) || (f5 < a9) || (f6 < a9) || (f7 < a9) || (f8 < a9) || (f9 < a9)) && (f > a9)) e.Graphics.DrawRectangle(p9, (float)(x0 + mw * j / k), (float)(y0 - mh * i / k), 1, 1);
                        if (((f2 < a1) || (f3 < a1) || (f4 < a1) || (f5 < a1) || (f6 < a1) || (f7 < a1) || (f8 < a1) || (f9 < a1)) && (f > a1) && (flines[4] == true)) e.Graphics.FillRectangle(Brushes.PaleGreen, (float)(ii), (float)(h - jj), 1, 1);
                        else if (((f2 < a3) || (f3 < a3) || (f4 < a3) || (f5 < a3) || (f6 < a3) || (f7 < a3) || (f8 < a3) || (f9 < a3)) && (f > a3) && (flines[3] == true)) e.Graphics.FillRectangle(Brushes.YellowGreen, (float)(ii), (float)(h - jj), 1, 1);
                        else if (((f2 < a5) || (f3 < a5) || (f4 < a5) || (f5 < a5) || (f6 < a5) || (f7 < a5) || (f8 < a5) || (f9 < a5)) && (f > a5) && (flines[2] == true)) e.Graphics.FillRectangle(Brushes.Orange, (float)(ii), (float)(h - jj), 1, 1);
                        else if (((f2 < a7) || (f3 < a7) || (f4 < a7) || (f5 < a7) || (f6 < a7) || (f7 < a7) || (f8 < a7) || (f9 < a7)) && (f > a7) && (flines[1] == true)) e.Graphics.FillRectangle(Brushes.Red, (float)(ii), (float)(h - jj), 1, 1);
                        else if (((f2 < a9) || (f3 < a9) || (f4 < a9) || (f5 < a9) || (f6 < a9) || (f7 < a9) || (f8 < a9) || (f9 < a9)) && (f > a9) && (flines[0] == true)) e.Graphics.FillRectangle(Brushes.Maroon, (float)(ii), (float)(h - jj), 1, 1);
                        else if (((f2 < a10) || (f3 < a10) || (f4 < a10) || (f5 < a10) || (f6 < a10) || (f7 < a10) || (f8 < a10) || (f9 < a10)) && (f > a10) && (flines[5] == true)) e.Graphics.FillRectangle(Brushes.Pink, (float)(ii), (float)(h - jj), 1, 1);
                        else if (((f2 < a11) || (f3 < a11) || (f4 < a11) || (f5 < a11) || (f6 < a11) || (f7 < a11) || (f8 < a11) || (f9 < a11)) && (f > a11) && (flines[6] == true)) e.Graphics.FillRectangle(Brushes.Violet, (float)(ii), (float)(h - jj), 1, 1);
                        else if (((f2 < a12) || (f3 < a12) || (f4 < a12) || (f5 < a12) || (f6 < a12) || (f7 < a12) || (f8 < a12) || (f9 < a12)) && (f > a12) && (flines[7] == true)) e.Graphics.FillRectangle(Brushes.MediumOrchid, (float)(ii), (float)(h - jj), 1, 1);

                    }

                //if (flag == true)
                {
                    //!!!сортировка
                    int i0 = 0;
                    double f0 = 0;
                    if (algst.Population.Count > 0)
                    {

                        f0 = algst.Population[0].f;
                        for (int i = 0; i < algst.Population.Count; i++)
                        {
                            if (algst.Population[i].f > f0)
                            {
                                f0 = algst.Population[i].f;
                                i0 = i;
                            }

                        }
                    }
                    //!!!

                    for (int i = 0; i < algst.Population.Count; i++)
                    {
                        var stu = algst.Population[i];
                        float r = 5;
                        Brush stuColor = Brushes.Blue; // по умолчанию — верующие

                        if (stu == algst.GlobalBestLeader)
                        {
                            r = 7;
                            stuColor = Brushes.Red; // глобальный лидер — красный
                        }
                        else if (stu.Role == Role.Leader)
                        {
                            r = 6;
                            stuColor = Brushes.Magenta; // локальные лидеры — розовые
                        }
                        else if (stu.Role == Role.Advocate)
                        {
                            r = 5;
                            stuColor = Brushes.Green; // адвокаты — зелёные
                        }
                        // верующие останутся синими (цвет задан по умолчанию)

                        e.Graphics.FillEllipse(stuColor,
                            (float)((stu.x[0] * k - x1) * w / (x2 - x1) - r / 2),
                            (float)(h - (stu.x[1] * k - y1) * h / (y2 - y1) - r / 2),
                            r, r);
                    }
                }
                



                for (int i = -6; i < 12; i++)
                {
                    e.Graphics.DrawLine(p10, (float)((x1 - i * step) * w / (x1 - x2)), h - a - 5, (float)((x1 - i * step) * w / (x1 - x2)), h - a + 5);
                    e.Graphics.DrawLine(p10, a - 5, (float)(h - (y1 - i * step) * h / (y1 - y2)), a + 5, (float)(h - (y1 - i * step) * h / (y1 - y2)));
                    e.Graphics.DrawString((i * step).ToString(), font2, Brushes.Black, (float)((x1 - i * step) * w / (x1 - x2)), h - a + 5);
                    e.Graphics.DrawString((i * step).ToString(), font2, Brushes.Black, a - 30, (float)(h - 5 - (y1 - i * step) * h / (y1 - y2)));
                }
            }
            e.Graphics.DrawLine(p10, 0, h - a, w, h - a);
            e.Graphics.DrawLine(p10, a, h, a, 0);
            e.Graphics.DrawLine(p10, a, 0, a - 5, 10);
            e.Graphics.DrawLine(p10, a, 0, a + 5, 10);
            e.Graphics.DrawLine(p10, w - 5, h - a, w - 15, h - a - 5);
            e.Graphics.DrawLine(p10, w - 5, h - a, w - 15, h - a + 5);
            e.Graphics.DrawString("x", font1, Brushes.Black, w - 20, h - a + 5);
            e.Graphics.DrawString("y", font1, Brushes.Black, a - 20, 1);


        }



        private float function(double x1, double x2, double x3, double x4, int f)
        {
            float funct = 0;
            if (f == 0)
            {
                funct = (float)(x1 * Math.Sin(Math.Sqrt(Math.Abs(x1))) + x2 * Math.Sin(Math.Sqrt(Math.Abs(x2))));
            }
            else if (f == 1)
            {
                funct = (float)(x1 * Math.Sin(4 * Math.PI * x1) - x2 * Math.Sin(4 * Math.PI * x2 + Math.PI) + 1);
            }
            else if (f == 2)
            {
                double[] c6 = Cpow(x1, x2, 6);
                funct = (float)(1 / (1 + Math.Sqrt((c6[0] - 1) * (c6[0] - 1) + c6[1] * c6[1])));
                exact = 1;
                if (funct > 0.6)
                {
                    int stop = 0;

                }
            }
            else if (f == 3)
            {
                funct = (float)(0.5 - (Math.Pow(Math.Sin(Math.Sqrt(x1 * x1 + x2 * x2)), 2) - 0.5) / (1 + 0.001 * (x1 * x1 + x2 * x2)));
            }
            else if (f == 4)
            {
                funct = (float)((-x1 * x1 + 10 * Math.Cos(2 * Math.PI * x1)) + (-x2 * x2 + 10 * Math.Cos(2 * Math.PI * x2)));
            }
            else if (f == 5)
            {
                funct = (float)(-Math.E + 20 * Math.Exp(-0.2 * Math.Sqrt((x1 * x1 + x2 * x2) / 2)) + Math.Exp((Math.Cos(2 * Math.PI * x1) + Math.Cos(2 * Math.PI * x2)) / 2));
            }
            else if (f == 6)
            {
                funct = (float)(Math.Pow(Math.Cos(2 * x1 * x1) - 1.1, 2) + Math.Pow(Math.Sin(0.5 * x1) - 1.2, 2) - Math.Pow(Math.Cos(2 * x2 * x2) - 1.1, 2) + Math.Pow(Math.Sin(0.5 * x2) - 1.2, 2));
            }
            else if (f == 7)
            {
                funct = (float)(-Math.Sqrt(Math.Abs(Math.Sin(Math.Sin(Math.Sqrt(Math.Abs(Math.Sin(x1 - 1))) + Math.Sqrt(Math.Abs(Math.Sin(x2 + 2))))))) + 1);
            }
            else if (f == 8)
            {
                funct = (float)(-(1 - x1) * (1 - x1) - 100 * (x2 - x1 * x1) * (x2 - x1 * x1));
            }
            else if (f == 9)
            {
                funct = (float)(1 - x1 * x1 - x2 * x2);
            }

            return funct;
        }

        private double[] Cpow(double x, double y, int p)
        {
            double[] Cp = new double[2];
            Cp[0] = x; Cp[1] = y;
            double x0 = 0;
            double y0 = 0;
            for (int i = 1; i < p; i++)
            {
                x0 = Cp[0] * x - Cp[1] * y;
                y0 = Cp[1] * x + Cp[0] * y;
                Cp[0] = x0; Cp[1] = y0;
            }
            return Cp;
        }

        private void pictureBox2_Paint(object sender, PaintEventArgs e)
        {
            int temp = 0;
            if (exact == 0)
            {
                temp = 1;
                exact = exact + 1;
            }


            float w = pictureBox2.Width;
            float h = pictureBox2.Height;
            Pen p1 = new Pen(Color.Black, 1);
            Pen p2 = new Pen(Color.Green, 1);
            Pen p3 = new Pen(Color.Blue, 1);
            Font f1 = new Font("TimesNewRoman", 7);
            Font f2 = new Font("TimesNewRoman", 7, FontStyle.Bold);
            float x0 = 25;
            float y0 = h - 20;
            e.Graphics.DrawLine(p1, x0, y0, w, y0);
            e.Graphics.DrawLine(p1, x0, y0, x0, 0);
            //стрелка
            e.Graphics.DrawLine(p1, x0, 0, x0 - 5, 10);
            e.Graphics.DrawLine(p1, x0, 0, x0 + 5, 10);
            //
            e.Graphics.DrawLine(p1, w - 5, y0, w - 15, y0 + 5);
            e.Graphics.DrawLine(p1, w - 5, y0, w - 15, y0 - 5);



            float mx = (w - 60) / (t + 5);
            float mh = 0;
            try { mh = (float)((h - 60) / ((1.1 * exact - Math.Min(0, algst.AverF[0])))); }
            catch { mh = (float)((h - 60) / (1.1 * exact)); }

            double a = 1;
            /* if (algst.K < 31) a = 2;
             else if (algst.K < 101) a = 5;
             else if (algst.K < 151) a = 10;
             else if (algst.K < 301) a = 20;
             else if (algst.K < 501) a = 50;
             else if (algst.K < 1001) a = 100;
             else if (algst.K < 2001) a = 200;
             else a = 1000;*/
            if (t < 31) a = 2;
            else if (t < 101) a = 5;
            else if (t < 151) a = 10;
            else if (t < 301) a = 20;
            else if (t < 501) a = 50;
            else if (t < 1001) a = 100;
            else if (t < 2001) a = 200;
            else a = 1000;

            double b = 0;
            try { b = 1.1 * exact - Math.Min(0, algst.AverF[0]); }
            catch { b = 1.1 * exact; }
            double c = 1;
            if (b < 0.1) c = 0.01;
            else if (b < 0.2) c = 0.02;
            else if (b < 1) c = 0.1;
            else if (b < 2) c = 0.2;
            else if (b < 11) c = 1;
            else if (b < 21) c = 2;
            else if (b < 51) c = 5;
            else if (b < 101) c = 10;
            else if (b < 200) c = 20;
            else if (b < 1000) c = 100;
            else if (b < 2000) c = 200;
            else c = 500;

            for (int i = 0; i < algst.K; i++)
            {

                //float s = i / a;
                if (Math.Floor((decimal)(i / a)) - (decimal)(i / a) == 0)
                {
                    e.Graphics.DrawLine(p1, (float)(x0 + (mx) * (i)), y0 + 2, (float)(x0 + mx * (i)), y0 - 2);
                    e.Graphics.DrawString(Convert.ToString(i), f1, Brushes.Black, (float)(x0 + mx * (i)), (float)(y0 + 4));

                }
            }
            if (Math.Floor((decimal)((algst.K) / a)) - (decimal)((algst.K) / a) == 0)
            {
                e.Graphics.DrawLine(p1, (float)(x0 + (mx) * (algst.K)), y0 + 2, (float)(x0 + mx * (algst.K)), y0 - 2);
                e.Graphics.DrawString(Convert.ToString(algst.K), f1, Brushes.Black, (float)(x0 + mx * (algst.K)), (float)(y0 + 4));
            }

            if (flag == true)
            {
                //e.Graphics.FillEllipse(Brushes.Green, (float)(x0), (float)(y0 - 1 - mh * (algst.AverF[0] - Math.Min(0, algst.AverF[0]))), 3, 3);
                //e.Graphics.FillEllipse(Brushes.Blue, (float)(x0), (float)(y0 - 1 - mh * (algst.BestF[0] - Math.Min(0, algst.AverF[0]))), 3, 3);

                // Используем t для ограничения количества отображаемых итераций
                for (int i = 0; i < t; i++)
                {
                    // Проверяем, что i не больше, чем размер списка algst.AverF
                    if (i < algst.AverF.Count - 1)
                    {
                        e.Graphics.DrawLine(p2, (float)(x0 + mx * i), (float)(y0 - mh * (algst.AverF[i] - Math.Min(0, algst.AverF[0]))), (float)(x0 + mx * (i + 1)), (float)(y0 - mh * (algst.AverF[i + 1] - Math.Min(0, algst.AverF[0]))));
                        e.Graphics.DrawLine(p3, (float)(x0 + mx * i), (float)(y0 - mh * (algst.BestF[i] - Math.Min(0, algst.AverF[0]))), (float)(x0 + mx * (i + 1)), (float)(y0 - mh * (algst.BestF[i + 1] - Math.Min(0, algst.AverF[0]))));
                    }
                }
            }

            float zero = 0;
            try { zero = (float)(y0 + mh * Math.Min(0, algst.AverF[0])); }
            catch { zero = (float)(y0); }

            for (int i = -6; i < 12; i++)
            {
                e.Graphics.DrawLine(p1, (float)(x0 + 2), (float)(zero - mh * c * i), (float)(x0 - 2), (float)(zero - mh * c * i));
                if ((zero - mh * c * i - 8 > 11) && (zero - mh * c * i - 8 < h - 20)) e.Graphics.DrawString(Convert.ToString((c * i)), f1, Brushes.Black, (float)(x0 - 24), (float)(zero - mh * c * i - 8));
            }
            e.Graphics.DrawString("k", f2, Brushes.Black, (float)(w - 15), (float)(y0 + 4));
            e.Graphics.DrawString("f", f2, Brushes.Black, (float)(x0 - 24), (float)(2));
            if (temp == 1)
            {
                exact = exact - 1;
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            if ((Red[0] == false) && (Red[1]==true))
                {
                MessageBox.Show("Перейдите к шагу \"Обновление Ролей\".");
            }

            //фаза учителя
            if ((Red[0] == true) || (Red[9] == true))
            {
                Red[0] = false;
                //Red[5] = false;
                Red[9] = false;
                Red[1] = true;
                algst.UpdateSearchDirections();
                algst.RecalcMaxf();
                pictureBox3.Refresh();
                pictureBox1.Refresh();
                pictureBox2.Refresh();
            }

            if (Red[0] == false && Red[1] == false && Red[3] == false && Red[6] == false && Red[9] == false)
            {
                MessageBox.Show("Запустите цикл по кнопке \"Создание начальной популяции\".");
            }
            
            

        }

        private void button6_Click(object sender, EventArgs e)
        {
            if ((Red[9] == true)&& (Red[1] == false))
            {
                MessageBox.Show("Перейдите к шагу \"Соревнование за лидерство\".");
            }
            
                // фаза ученика
                if (true)
                {
                if (Red[1] == true)
                {
                    Red[1] = false;
                    Red[9] = true;
                    algst.AssignRoles();
                    GoToNextStep();
                    pictureBox3.Refresh();
                    pictureBox1.Refresh();
                    algst.RecalcMaxf();
                    pictureBox2.Refresh();
                }

                if (t-1 == K)
                {
                    dataGridView1.Rows[0].Cells[1].Value = t-1;
                    //------------------
                    int i00 = 0;
                    double f00 = 0;
                    if (algst.Population.Count > 0)
                    {

                        f00 = algst.Population[0].f;
                        for (int ii = 0; ii < algst.Population.Count; ii++)
                        {
                            if (algst.Population[ii].f > f00)
                            {
                                f00 = algst.Population[ii].f;
                                i00 = ii;
                            }

                        }
                    }
                    //------------------
                    dataGridView2.Rows[0].Cells[1].Value = "(" + Math.Round(algst.Population[i00].x[0], 4).ToString() + "; " + Math.Round(algst.Population[i00].x[1], 4).ToString() + ")";
                    dataGridView2.Rows[1].Cells[1].Value = Math.Round(algst.Population[i00].f, 6).ToString();
                    dataGridView2.Rows[2].Cells[1].Value = exact.ToString();
                    dataGridView2.Rows[3].Cells[1].Value = Math.Round((double)Math.Abs(exact - algst.Population[i00].f), 6).ToString();
                    block_button();
                    MessageBox.Show(String.Format("Переход к следующему шагу невозможен! Исчерпано количество итераций: {0} итерация из {1}.\r\nОтвет получен.\r\nДля повторного поиска перейдите к шагу \"Создание начальной популяции\"", t - 1, K));
                    button16.Enabled = true;
                    //
                    pictureBox3.Refresh();
                    algst.kpopp = t;
                    if (endreport == false)
                    {
                        endreport = true;
                        if ((File.Exists("protocol.dt")))
                        {
                            Report prot = new Report();
                            prot.Prot2(algst.Population, algst.kpop, 0, exact);

                            FileStream fs = new FileStream("protocol.dt", FileMode.Append, FileAccess.Write);
                            StreamWriter r1 = new StreamWriter(fs);
                            r1.Write(prot.toprotocol);
                            r1.Close();
                            fs.Close();

                        }
                    }
                }

                if (Red[0] == false && Red[1] == false && Red[3] == false && Red[6] == false && Red[9] == false)
                {
                    MessageBox.Show("Запустите цикл по кнопке \"Создание начальной популяции\".");
                }
                else if (Red[0] == true)
                {
                    MessageBox.Show("Перейдите к шагу \"Соревнование за лидерство\".");
                }
                
                }
            
                

            
        }

        

        private void GoToNextStep()
        {
            t++;
            UpdateParamView();

            
        }

        private void button8_Click(object sender, EventArgs e)
        {
            //условия окончания локального поиска
            
        }

        private void button10_Click(object sender, EventArgs e)
        {
            //условия глобального поиска
           
        }

        private void button11_Click(object sender, EventArgs e)
        {
            // ? //   добавление
            if (Red[7] == true)
            {
                Red[7] = false;
                Red[9] = true;
                // algst.Dobavlenie();
                //t++;
                //order_pop();
                dataGridView1.Rows[0].Cells[1].Value = t;
                dataGridView1.Rows[1].Cells[1].Value = algst.Population.Count;
                dataGridView1.Rows[2].Cells[1].Value = groupSize;
                dataGridView1.Rows[3].Cells[1].Value = groupCount;
                dataGridView1.Rows[4].Cells[1].Value = weight;
                

                dataGridView1.Rows[5].Cells[1].Value = Math.Round(algst.AverF[t], 6);
                
                dataGridView1.Rows[6].Cells[1].Value = "(" + Math.Round(algst.Population[0].x[0], 4).ToString() + "; " + Math.Round(algst.Population[0].x[1], 4).ToString() + ")";
                dataGridView1.Rows[7].Cells[1].Value = Math.Round(algst.Population[0].f, 6).ToString();
                dataGridView1.Rows[8].Cells[1].Value = exact.ToString();
                dataGridView1.Rows[9].Cells[1].Value = Math.Round((double)Math.Abs(exact - algst.Population[0].f), 6).ToString();
                    
                pictureBox3.Refresh();
                pictureBox1.Refresh();
            }
        }




        private void button12_Click(object sender, EventArgs e)
        {
            //ответ
            if ((Red[9] == true)|| (Red[0] == true))
            {
                Red[9] = false;
                Red[0] = false;
                //блокировать кнопки
                //Red[2] = true;
                //



                //order_pop();

                int i0 = 0;
                double f0 = 0;
                if (algst.Population.Count > 0)
                {

                    f0 = algst.Population[0].f;
                    for (int ii = 0; ii < algst.Population.Count; ii++)
                    {
                        if (algst.Population[ii].f > f0)
                        {
                            f0 = algst.Population[ii].f;
                            i0 = ii;
                        }

                    }
                }
                //------------------

                dataGridView2.Rows[0].Cells[1].Value = "(" + Math.Round(algst.Population[i0].x[0], 4).ToString() + "; " + Math.Round(algst.Population[i0].x[1], 4).ToString() + ")";
                dataGridView2.Rows[1].Cells[1].Value = Math.Round(algst.Population[i0].f, 6).ToString();
                dataGridView2.Rows[2].Cells[1].Value = exact.ToString();
                dataGridView2.Rows[3].Cells[1].Value = Math.Round((double)Math.Abs(exact - algst.Population[i0].f), 6).ToString();
                pictureBox3.Refresh();
                algst.kpopp = t;
                if (endreport == false)
                {
                    endreport = true;
                    if ((File.Exists("protocol.dt")))
                    {
                        Report prot = new Report();
                        prot.Prot2(algst.Population, algst.kpop, 0, exact);

                        FileStream fs = new FileStream("protocol.dt", FileMode.Append, FileAccess.Write);
                        StreamWriter r1 = new StreamWriter(fs);
                        r1.Write(prot.toprotocol);
                        r1.Close();
                        fs.Close();

                    }
                }
                block_button();
                MessageBox.Show("Ответ получен.\r\nДля повторного поиска перейдите к шагу \"Создание начальной популяции\".");
                button16.Enabled = true;
            }

            else if (Red[0] == false && Red[1] == false && Red[3] == false && Red[6] == false && Red[9] == false)
            {
                MessageBox.Show("Получить ответ невозможно! \r\nЗапустите цикл по кнопке \"Создание начальной популяции\".");
            }            
            else if (Red[1] == true)
            {
                MessageBox.Show("Получить ответ невозможно! \r\nПерейдите к шагу \"Соревнование за лидерство\".");
            }
            
            

        }


        public double Mfend = 0;
       
        private void button1_Click(object sender, EventArgs e)
        {
            if ((Red[0] == true) ||  Red[9] == true)
            {
                Red[0] = false;
                Red[5] = false;
                Red[9] = false;
                pictureBox3.Refresh();

                for (int i = t; i < K+1; i++)
                {
                    algst.UpdateSearchDirections();
                    algst.AssignRoles();

                    algst.RecalcMaxf();



                    dataGridView1.Rows[0].Cells[1].Value = t;
                    dataGridView1.Rows[1].Cells[1].Value = algst.Population.Count;
                    dataGridView1.Rows[2].Cells[1].Value = groupSize;
                    dataGridView1.Rows[3].Cells[1].Value = groupCount;
                    dataGridView1.Rows[4].Cells[1].Value = weight;
                    

                    dataGridView1.Rows[5].Cells[1].Value = Math.Round(algst.AverF[t], 6);
                    

                    //------------------
                    int i0 = 0;
                    double f0 = 0;
                    if (algst.Population.Count > 0)
                    {

                        f0 = algst.Population[0].f;
                        for (int ii = 0; ii < algst.Population.Count; ii++)
                        {
                            if (algst.Population[ii].f > f0)
                            {
                                f0 = algst.Population[ii].f;
                                i0 = ii;
                            }

                        }
                    }
                    //------------------


                    dataGridView1.Rows[6].Cells[1].Value = "(" + Math.Round(algst.Population[i0].x[0], 4).ToString() + "; " + Math.Round(algst.Population[i0].x[1], 4).ToString() + ")";
                    dataGridView1.Rows[7].Cells[1].Value = Math.Round(algst.Population[i0].f, 6).ToString();
                    dataGridView1.Rows[8].Cells[1].Value = exact.ToString();
                    dataGridView1.Rows[9].Cells[1].Value = Math.Round((double)Math.Abs(exact - algst.Population[i0].f), 6).ToString();
                    
                    dataGridView1.Refresh();
                    pictureBox1.Refresh();
                    algst.RecalcMaxf();
                    pictureBox2.Refresh();
                    pictureBox3.Refresh();

                    t++;
                    algst.kpopp = t;
                }



                //------------------
                int i00 = 0;
                double f00 = 0;
                if (algst.Population.Count > 0)
                {

                    f00 = algst.Population[0].f;
                    for (int ii = 0; ii < algst.Population.Count; ii++)
                    {
                        if (algst.Population[ii].f > f00)
                        {
                            f00 = algst.Population[ii].f;
                            i00 = ii;
                        }

                    }
                }
                //------------------
                dataGridView2.Rows[0].Cells[1].Value = "(" + Math.Round(algst.Population[i00].x[0], 4).ToString() + "; " + Math.Round(algst.Population[i00].x[1], 4).ToString() + ")";
                dataGridView2.Rows[1].Cells[1].Value = Math.Round(algst.Population[i00].f, 6).ToString();
                dataGridView2.Rows[2].Cells[1].Value = exact.ToString();
                dataGridView2.Rows[3].Cells[1].Value = Math.Round((double)Math.Abs(exact - algst.Population[i00].f), 6).ToString();
                block_button();
                MessageBox.Show("Ответ получен.\r\nДля повторного поиска перейдите к шагу \"Создание начальной популяции\".");

                if (endreport == false)
                {
                    endreport = true;
                    if ((File.Exists("protocol.dt")))
                    {
                        Report prot = new Report();
                        prot.Prot2(algst.Population, algst.kpop, 0, exact);

                        FileStream fs = new FileStream("protocol.dt", FileMode.Append, FileAccess.Write);
                        StreamWriter r1 = new StreamWriter(fs);
                        r1.Write(prot.toprotocol);
                        r1.Close();
                        fs.Close();

                    }
                }
                button16.Enabled = true;
            }
            

            else if (Red[0] == false && Red[1] == false && Red[3] == false && Red[6] == false && Red[9] == false)
            {
                MessageBox.Show("Получить ответ невозможно! \r\nЗапустите цикл по кнопке \"Создание начальной популяции\".");
            }
            else if (Red[1] == true)
            {
                MessageBox.Show("Получить ответ невозможно! \r\nПерейдите к шагу \"Соревнование за лидерство\".");
            }
            
            

            
            //else if (Red[8] == true)
            //{
            //    MessageBox.Show("Ответ получен.");
            //    Red[8] = false;
            //    pictureBox3.Refresh();
            //}
        }

       
        Form5 forma5 = new Form5();  

        private void button15_Click(object sender, EventArgs e)
        {
            if (forma5.IsDisposed) forma5 = new Form5();
            forma5.algst = algst;
            forma5.Population = algst.Population;
            // TODO: !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //throw new Exception("Не готово!");
            forma5.FillList();
            
            forma5.Show();
        }

        Form3 forma3 = new Form3();   

        private void button3_Click(object sender, EventArgs e)
        {
            if (forma3.IsDisposed) forma3 = new Form3();
            forma3.A = A;
            forma3.Ar = Ar;
            forma3.flines = flines;
            forma3.obl = showobl;
            forma3.oblbase = showoblbase;
            forma3.Show();
        }

        private void button3_Paint(object sender, PaintEventArgs e)
        {
            pictureBox1.Refresh();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            Process.Start("fish.pdf");
            //MessageBox.Show("Здесь будет открываться файл со справкой");
        }

        private void button16_Click(object sender, EventArgs e)
        {
            Process.Start("protocol.dt");
        }


        private void button8_Click_1(object sender, EventArgs e)
        {
            

            if (Red[0] == true || Red[9] == true)
            {
                int error = 0;
                int tt = 0;
                try 
                {
                    tt = Convert.ToInt32(textBox1.Text);
                    if (tt <= 0) error = 1;
                }
                catch
                {
                    if (textBox1.Text == "") MessageBox.Show("Не задано количество итераций!");
                    else error = 1;
                }

                if (error == 1) MessageBox.Show("Неверно задано количество итераций!");

                int dv = t + tt;
                for (int i = t; i < dv; i++)
                {
                    algst.UpdateSearchDirections();
                    algst.AssignRoles();
                    
                    algst.RecalcMaxf();
                    t++;
                    algst.kpopp = t;

                    //---сортировка-----
                    int i0 = 0;
                    double f0 = 0;
                    if (algst.Population.Count > 0)
                    {

                        f0 = algst.Population[0].f;
                        for (int k = 0; k < algst.Population.Count; k++)
                        {
                            if (algst.Population[k].f > f0)
                            {
                                f0 = algst.Population[k].f;
                                i0 = k;
                            }

                        }
                    }
                    //----------

                    dataGridView1.Rows[0].Cells[1].Value = t;
                    dataGridView1.Rows[1].Cells[1].Value = algst.Population.Count;
                    dataGridView1.Rows[2].Cells[1].Value = groupSize;
                    dataGridView1.Rows[3].Cells[1].Value = groupCount;
                    dataGridView1.Rows[4].Cells[1].Value = weight;
                    
                    dataGridView1.Rows[5].Cells[1].Value = Math.Round(algst.AverF[t], 6);
                    


                    dataGridView1.Rows[6].Cells[1].Value = "(" + Math.Round(algst.Population[i0].x[0], 4).ToString() + "; " + Math.Round(algst.Population[i0].x[1], 4).ToString() + ")";
                    dataGridView1.Rows[7].Cells[1].Value = Math.Round(algst.Population[i0].f, 6).ToString();
                    dataGridView1.Rows[8].Cells[1].Value = exact.ToString();
                    dataGridView1.Rows[9].Cells[1].Value = Math.Round((double)Math.Abs(exact - algst.Population[i0].f), 6).ToString();

                    dataGridView1.Refresh();
                    pictureBox1.Refresh();
                    algst.RecalcMaxf();
                    pictureBox2.Refresh();
                    pictureBox3.Refresh();

                    if (t - 1 == K)
                    {
                        dataGridView1.Rows[0].Cells[1].Value = t - 1;
                        //------------------
                        int i00 = 0;
                        double f00 = 0;
                        if (algst.Population.Count > 0)
                        {

                            f00 = algst.Population[0].f;
                            for (int ii = 0; ii < algst.Population.Count; ii++)
                            {
                                if (algst.Population[ii].f > f00)
                                {
                                    f00 = algst.Population[ii].f;
                                    i00 = ii;
                                }

                            }
                        }
                        //------------------
                        dataGridView2.Rows[0].Cells[1].Value = "(" + Math.Round(algst.Population[i00].x[0], 4).ToString() + "; " + Math.Round(algst.Population[i00].x[1], 4).ToString() + ")";
                        dataGridView2.Rows[1].Cells[1].Value = Math.Round(algst.Population[i00].f, 6).ToString();
                        dataGridView2.Rows[2].Cells[1].Value = exact.ToString();
                        dataGridView2.Rows[3].Cells[1].Value = Math.Round((double)Math.Abs(exact - algst.Population[i00].f), 6).ToString();
                        block_button();
                        MessageBox.Show(String.Format("Исчерпано количество итераций: {0} итерация из {1}.\r\nОтвет получен.\r\nДля повторного поиска перейдите к шагу \"Создание начальной популяции\"", t-1, K));
                        //
                        algst.RecalcMaxf();
                        pictureBox3.Refresh();
                        algst.kpopp = t;
                        button16.Enabled = true;
                        if (endreport == false)
                        {
                            endreport = true;
                            if ((File.Exists("protocol.dt")))
                            {
                                Report prot = new Report();
                                prot.Prot2(algst.Population, algst.kpop, 0, exact);

                                FileStream fs = new FileStream("protocol.dt", FileMode.Append, FileAccess.Write);
                                StreamWriter r1 = new StreamWriter(fs);
                                r1.Write(prot.toprotocol);
                                r1.Close();
                                fs.Close();

                            }
                        }
                        break;
                    }

                    
                    
                }



            }


            else if (Red[0] == false && Red[1] == false && Red[3] == false && Red[6] == false && Red[9] == false)
            {
                MessageBox.Show("Добавить заданное количество итераций невозможно! \r\nЗапустите цикл по кнопке \"Создание начальной популяции\".");
            }
            else if (Red[1] == true)
            {
                MessageBox.Show("Добавить заданное количество итераций невозможно! \r\nПерейдите к шагу \"Соревнование за лидерство\".");
            }
            
            

            
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        
    }

    

}
