using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;


namespace TLBO
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            dataGridView1.RowCount = 2;
            dataGridView1.Rows[0].Cells[0].Value = "x";
            dataGridView1.Rows[1].Cells[0].Value = "y";
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            InitDataGridView1();

            if ((File.Exists("protocol.dt")))
            {
                File.Delete("protocol.dt");
            }
            FileStream vipoln_test = new FileStream("protocol.dt", FileMode.OpenOrCreate);
            vipoln_test.Close();
            FileStream fs = new FileStream("protocol.dt", FileMode.Append, FileAccess.Write);

            StreamWriter r = new StreamWriter(fs);
            r.Write("*********************************************************************************************************" + "\r\n"
                   + "**                                                                                                     **" + "\r\n"
                   + "**                            Протокол работы метода, на основе преподавания и обучения                  **" + "\r\n"
                   + "**                                           " + DateTime.Now.ToString() + "                                       **" + "\r\n"
                   + "**                                                                                                     **" + "\r\n"
                   + "*********************************************************************************************************" + "\r\n");
            r.Close();
            fs.Close();
        }

        public class AlgoritmInfo
        {
            public double MaxDf { get; set; }
            public double Deviation { get; set; }
        }
        private void InitDataGridView1()
        {
            dataGridView2.RowCount = 5;
            
            dataGridView2.Rows[0].Cells[0].Value = "Количество итераций:";
            dataGridView2.Rows[1].Cells[0].Value = "Количество человек в группе:";
            dataGridView2.Rows[2].Cells[0].Value = "Количество групп:";
            dataGridView2.Rows[3].Cells[0].Value = "Всего человек:";
            dataGridView2.Rows[4].Cells[0].Value = "Влияние прошлой позиции:";
            dataGridView1.Rows[0].Cells[1].Value = "0";
            dataGridView1.Rows[0].Cells[2].Value = "1";
            dataGridView1.Rows[1].Cells[1].Value = "0";
            dataGridView1.Rows[1].Cells[2].Value = "1";
            dataGridView2.Rows[0].Cells[1].Value = "1000";
            dataGridView2.Rows[1].Cells[1].Value = "10";
            dataGridView2.Rows[2].Cells[1].Value = "10";
            dataGridView2.Rows[3].Cells[1].Value = "100";
            dataGridView2.Rows[4].Cells[1].Value = "1";
            // чтобы при вводе в ячейку сразу вызывалось CellValueChanged
            dataGridView2.CurrentCellDirtyStateChanged += (s, e) =>
            {
                if (dataGridView2.IsCurrentCellDirty)
                    dataGridView2.CommitEdit(DataGridViewDataErrorContexts.Commit);
            };

            // чтобы после фиксации редактирования считалось новое количество учеников
            dataGridView2.CellValueChanged += DataGridView2_CellValueChanged;
            dataGridView2.Rows[3].Cells[1].ReadOnly = true;
            dataGridView3.RowCount = 7;


            dataGridView3.Rows[0].Cells[0].Value = "(x*; y*):";
            dataGridView3.Rows[1].Cells[0].Value = "f*:";
            dataGridView3.Rows[2].Cells[0].Value = "Лучшее известное решение:";
            dataGridView3.Rows[3].Cells[0].Value = "Δ:";
            dataGridView3.Rows[4].Cells[0].Value = "g1:";
            dataGridView3.Rows[5].Cells[0].Value = "g2:";
            dataGridView3.Rows[6].Cells[0].Value = "g3:";

        }
        private void DataGridView2_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1 && (e.RowIndex == 1 || e.RowIndex == 2))
            {
                if (int.TryParse(Convert.ToString(dataGridView2.Rows[1].Cells[1].Value), out var groupSize) &&
                    int.TryParse(Convert.ToString(dataGridView2.Rows[2].Cells[1].Value), out var groupCount))
                {
                    dataGridView2.Rows[3].Cells[1].Value = (groupSize * groupCount).ToString();
                }
                
            }
        }
        private Algoritm alg;
        double exact = 0;
        bool first = true;
        int numb = 0;
        bool[] flines = new bool[8];
        int n = 2;

        private double RunAlgorithmAndGetResult()
        {
            int z = 10;
            double[,] obl = new double[n, 2];
            int error = 0;

            // Получение области определения функции из dataGridView1
            if (z < 10)
            {
                try
                {
                    obl[0, 0] = Convert.ToDouble(dataGridView1.Rows[0].Cells[1].Value);
                    obl[0, 1] = Convert.ToDouble(dataGridView1.Rows[0].Cells[2].Value);
                    for (int k = 1; k < n; k++)
                    {
                        obl[k, 0] = Convert.ToDouble(dataGridView1.Rows[1].Cells[1].Value);
                        obl[k, 1] = Convert.ToDouble(dataGridView1.Rows[1].Cells[2].Value);
                    }
                    obl[0, 0] = Convert.ToDouble(dataGridView1.Rows[0].Cells[1].Value);  // x1 min
                    obl[0, 1] = Convert.ToDouble(dataGridView1.Rows[0].Cells[2].Value);  // x1 max
                    obl[1, 0] = Convert.ToDouble(dataGridView1.Rows[1].Cells[1].Value);  // x2 min
                    obl[1, 1] = Convert.ToDouble(dataGridView1.Rows[1].Cells[2].Value);  // x2 max
                }
                catch
                {
                    error = 1;
                }
                if ((obl[0, 1] - obl[0, 0] < 0) || (obl[1, 1] - obl[1, 0] < 0)) error = 1;
            }
            else
            {
                // Обработка других значений z (если необходимо)
                // ...
            }

            // Получение параметров алгоритма из dataGridView2
            int Ip = 0;
            int K = 0;
            int groupSize = 0;
            int groupCount = 0;
            double weight = 0;



            try
            {
                
                K = Convert.ToInt32(dataGridView2.Rows[0].Cells[1].Value);
                groupSize = Convert.ToInt32(dataGridView2.Rows[1].Cells[1].Value);
                groupCount = Convert.ToInt32(dataGridView2.Rows[2].Cells[1].Value);
                Ip = groupSize * groupCount;
                weight = Convert.ToDouble(dataGridView2.Rows[4].Cells[1].Value);

            }
            catch
            {
                error = 2;
            }

            if (error == 0)
            {
                Algoritm alg = new Algoritm();
                alg.Init(
    k: K,
    obl: obl,
    n: n,
    z_: z,
    groupSize: groupSize,
    groupCount: groupCount,
    weight: weight
);
                alg.Create_Pop();
                alg.Work();
                return alg.maxDf; // Возвращаем лучшее значение целевой функции
            }
            else
            {
                // Обработка ошибок (например, запись в лог)
                Console.WriteLine($"Ошибка: {error}");
                return double.NaN; // Возвращаем NaN, чтобы указать на ошибку
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //dataGridView2.Rows.Add();
            //создать начальную популяцию
            n = 2;




            

            exact = 263.87;

            A[0] = -7F;
            A[1] = -4F;
            A[2] = -2F;//0.5000001F;
            A[3] = -0.8F;
            A[4] = -0.1F;
            flag = true;



            A[5] = 0;
            A[6] = 0;
            A[7] = 0;
            for (int i = 0; i < 4; i++)
                dataGridView3.Rows[i].Cells[1].Value = "";
            for (int i = 0; i < 8; i++)
            {
                flines[i] = true;
                Ar[i] = A[i];
            }
            flines[5] = false;
            flines[6] = false;
            flines[7] = false;

            forma3.flines = flines;
            flag2 = false;


            showobl[0, 0] = Convert.ToDouble(dataGridView1.Rows[0].Cells[1].Value);
            showobl[0, 1] = Convert.ToDouble(dataGridView1.Rows[0].Cells[2].Value);
            showobl[1, 0] = Convert.ToDouble(dataGridView1.Rows[1].Cells[1].Value);
            showobl[1, 1] = Convert.ToDouble(dataGridView1.Rows[1].Cells[2].Value);

            showoblbase[0, 0] = Convert.ToDouble(dataGridView1.Rows[0].Cells[1].Value);
            showoblbase[0, 1] = Convert.ToDouble(dataGridView1.Rows[0].Cells[2].Value);
            showoblbase[1, 0] = Convert.ToDouble(dataGridView1.Rows[1].Cells[1].Value);
            showoblbase[1, 1] = Convert.ToDouble(dataGridView1.Rows[1].Cells[2].Value);

            //numericUpDown5.Value = 1;
            
            forma3.Ar = Ar;
            forma3.obl = showobl;

            dataGridView1.Refresh();

            int z = 10;

                double[,] obl = new double[n, 2];

                int error = 0;

                if (z < 11)
                {
                    try
                    {
                        obl[0, 0] = Convert.ToDouble(dataGridView1.Rows[0].Cells[1].Value);
                        obl[0, 1] = Convert.ToDouble(dataGridView1.Rows[0].Cells[2].Value);
                        for (int k = 1; k < n; k++)
                        {
                            obl[k, 0] = Convert.ToDouble(dataGridView1.Rows[1].Cells[1].Value);
                            obl[k, 1] = Convert.ToDouble(dataGridView1.Rows[1].Cells[2].Value);
                        }
                        obl[n - 1, 0] = obl[0, 0];
                        obl[n - 1, 1] = obl[0, 1];
                    }
                    catch
                    {
                        error = 1;
                    }
                    if ((obl[0, 1] - obl[0, 0] < 0) || (obl[1, 1] - obl[1, 0] < 0)) error = 1;
                }
                else
                {

                }

                int Ip = 0;
                int K = 0;
                int groupSize = 0;
                int groupCount = 0;
                double weight = 0;



                try
                {
                    
                    K = Convert.ToInt32(dataGridView2.Rows[0].Cells[1].Value);
                    groupSize = Convert.ToInt32(dataGridView2.Rows[1].Cells[1].Value);
                    groupCount = Convert.ToInt32(dataGridView2.Rows[2].Cells[1].Value);
                    Ip = groupSize * groupCount;
                    weight = Convert.ToDouble(dataGridView2.Rows[4].Cells[1].Value);
                if (K < 1)
                {
                    MessageBox.Show("Минимальное количество итераций =1");
                    return; // остановка метода
                }
                if (groupSize < 2)
                {
                    MessageBox.Show("Минимальное количество человек в группе =2");
                    return; // остановка метода
                }
                if (groupCount < 1)
                {
                    MessageBox.Show("Минимальное количество групп =1");
                    return; // остановка метода
                }

            }
                catch
                {
                    error = 2;
                }


                if (error == 0)
                {



                    List<double> disp = new List<double>();

                    //for (int d1 = 0; d1 < 1000; d1++)
                    //{

                    alg = new Algoritm();
                    alg.Init(
    k: K,
    obl: obl,
    n: n,
    z_: z,
    groupSize: groupSize,
    groupCount: groupCount,
    weight: weight
);

                    alg.Create_Pop();
                    numb++;
                    if ((File.Exists("protocol.dt")))
                    {
                        Report prot = new Report();
                        prot.Prot1(z, Ip, K, alg.Population, obl, numb);
                        if ((File.Exists("protocol.dt")))
                        {
                            FileStream fs = new FileStream("protocol.dt", FileMode.Append, FileAccess.Write);
                            StreamWriter r1 = new StreamWriter(fs);
                            r1.Write(prot.toprotocol);
                            r1.Close();
                            fs.Close();
                        }
                    }


                    alg.Work();



                    if ((File.Exists("protocol.dt")))
                    {
                        Report prot = new Report();
                        prot.Prot2(alg.Population, alg.kpop, 0, exact);

                        FileStream fs = new FileStream("protocol.dt", FileMode.Append, FileAccess.Write);
                        StreamWriter r1 = new StreamWriter(fs);
                        r1.Write(prot.toprotocol);
                        r1.Close();
                        fs.Close();

                    }





                


                    if (z < 10)
                    {
                        dataGridView3.Rows[0].Cells[1].Value = "(" + alg.bestX.ToString("F6") + "; " + alg.bestY.ToString("F6") + ")"; // Используем bestX и bestY
                        dataGridView3.Rows[1].Cells[1].Value = Math.Round(alg.maxDf, 6).ToString(); // Используем maxDf
                        dataGridView3.Rows[2].Cells[1].Value = z < 10 ? exact.ToString() : "";
                        dataGridView3.Rows[3].Cells[1].Value = z < 10 ? Math.Round((exact - Math.Round(alg.maxDf, 6)), 6).ToString() : "";
                    }
                    else if (z==10)
                    {
                        double minF = -alg.maxDf;
                        dataGridView3.Rows[0].Cells[1].Value = "(" + alg.bestX.ToString("F6") + "; " + alg.bestY.ToString("F6") + ")"; // Используем bestX и bestY
                        dataGridView3.Rows[1].Cells[1].Value = Math.Round(minF, 6).ToString(); // Используем maxDf
                        dataGridView3.Rows[2].Cells[1].Value = z < 11 ? exact.ToString() : "";
                        dataGridView3.Rows[3].Cells[1].Value = z < 11 ? Math.Round((exact - Math.Round(minF, 6)), 6).ToString() : "";

                    var (g1, g2, g3) = CalculateConstraints(alg.bestX, alg.bestY);
                    dataGridView3.Rows[4].Cells[1].Value = g1.ToString("F6");
                    dataGridView3.Rows[5].Cells[1].Value = g2.ToString("F6");
                    dataGridView3.Rows[6].Cells[1].Value = g3.ToString("F6");
                }
    {

    }


                    flag2 = true;
                    
                    //textBox1.BackColor = Color.Green;
                   
                }
                else switch (error)
                    {
                        case 1:
                            MessageBox.Show("Неверно введена область определения функции!");
                            break;
                        case 2:
                            MessageBox.Show("Неверно введены параметры алгоритма!");
                            break;
                    }
            

        }

        private (double g1, double g2, double g3) CalculateConstraints(double λ1, double λ2)
        {
            double g1 = 2 * ((Math.Sqrt(2) * λ1 + λ2) / (Math.Sqrt(2) * λ1 * λ1 + 2 * λ1 * λ2) - 1);
            double g2 = 2 * (λ2 / (Math.Sqrt(2) * λ1 * λ1 + 2 * λ1 * λ2) - 1);
            double g3 = 2 * (1 / (Math.Sqrt(2) * λ2 + λ1) - 1);
            return (g1, g2, g3);
        }
        private string vectorText(double[] x)
        {
            var s = "";
            var k = x.Length <= 2 ? 3 : 2;
            if (x.Length >= 7) k = 1;
            for (int i = 0; i < x.Length; i++)
            {
                s += Math.Round(x[i], k).ToString();
                if (i < x.Length - 1) s += "; ";
            }
            return s;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            n = 2;
            
            
            
            
                dataGridView1.Rows[0].Cells[1].Value = "0";
                dataGridView1.Rows[0].Cells[2].Value = "1";
                dataGridView1.Rows[1].Cells[1].Value = "0";
                dataGridView1.Rows[1].Cells[2].Value = "1";
                dataGridView2.Rows[0].Cells[1].Value = "1000";
                dataGridView2.Rows[1].Cells[1].Value = "10";
                dataGridView2.Rows[2].Cells[1].Value = "10";
                dataGridView2.Rows[3].Cells[1].Value = "100";
                dataGridView2.Rows[4].Cells[1].Value = "1";

                exact = 263.87;

                A[0] = -7F;
                A[1] = -4F;
                A[2] = -2F;//0.5000001F;
                A[3] = -0.8F;
                A[4] = -0.1F;
                flag = true;
                
            

            A[5] = 0;
            A[6] = 0;
            A[7] = 0;
            for (int i = 0; i < 4; i++)
                dataGridView3.Rows[i].Cells[1].Value = "";
            for (int i = 0; i < 8; i++)
            {
                flines[i] = true;
                Ar[i] = A[i];
            }
            flines[5] = false;
            flines[6] = false;
            flines[7] = false;

            forma3.flines = flines;
            flag2 = false;


            showobl[0, 0] = Convert.ToDouble(dataGridView1.Rows[0].Cells[1].Value);
            showobl[0, 1] = Convert.ToDouble(dataGridView1.Rows[0].Cells[2].Value);
            showobl[1, 0] = Convert.ToDouble(dataGridView1.Rows[1].Cells[1].Value);
            showobl[1, 1] = Convert.ToDouble(dataGridView1.Rows[1].Cells[2].Value);

            showoblbase[0, 0] = Convert.ToDouble(dataGridView1.Rows[0].Cells[1].Value);
            showoblbase[0, 1] = Convert.ToDouble(dataGridView1.Rows[0].Cells[2].Value);
            showoblbase[1, 0] = Convert.ToDouble(dataGridView1.Rows[1].Cells[1].Value);
            showoblbase[1, 1] = Convert.ToDouble(dataGridView1.Rows[1].Cells[2].Value);

            //numericUpDown5.Value = 1;
            
            forma3.Ar = Ar;
            forma3.obl = showobl;
            
            dataGridView1.Refresh();

        }

        float k = 1;
        float[] A = new float[8];
        float[] Ar = new float[8];
        double[,] showoblbase = new double[2, 2];
        double[,] showobl = new double[2, 2];
        bool flag = false;
        bool flag2 = false;

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            float w = 0;
            float h = 0;
            float x0 = w / 2;
            float y0 = h / 2;
            float a = 30;
            //dataGridView1.Rows[0].Cells[1].Value = "-500";
            //dataGridView1.Rows[0].Cells[2].Value = "500";

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

            
            // int a = Convert.ToInt32(pictureBox1.Height);
            // int b = Convert.ToInt32(pictureBox1.Width);
            if (flag == true)
            {
                double x1 = showobl[0, 0];//Convert.ToDouble(dataGridView1.Rows[0].Cells[1].Value);
                double x2 = showobl[0, 1];//Convert.ToDouble(dataGridView1.Rows[0].Cells[2].Value);
                double y1 = showobl[1, 0];//Convert.ToDouble(dataGridView1.Rows[1].Cells[1].Value);
                double y2 = showobl[1, 1];//Convert.ToDouble(dataGridView1.Rows[1].Cells[2].Value);



                int z = 10;
                flines = forma3.flines;
                Ar = forma3.Ar;
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

                //if (dxy>0)
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

                //double x30 = 7.2;
                //double x40 = 1;

                //float mw = k * (w) / ((float)(Math.Max(x2 - x1, y2 - y1)));
                //float mh = k * (h) / ((float)(Math.Max(x2 - x1, y2 - y1)));
                float mw = k * (w) / ((float)(x2 - x1));
                float mh = k * (h) / ((float)(y2 - y1));
                //for (int i = (x1); i < x2; i++)
                // for (int j = (y1); j < y2; j++)
                for (int ii = 0; ii < w; ii++)
                    for (int jj = 0; jj < h; jj++)
                    {
                        //double mj = j * mh;
                        // double ni = i * mw;
                        double i = (ii * (x2 - x1) / w + x1) / k;
                        double j = (jj * (y2 - y1) / h + y1) / k;
                        double i1 = ((ii + 1) * (x2 - x1) / w + x1) / k;
                        double j1 = ((jj + 1) * (y2 - y1) / h + y1) / k;
                        double i0 = ((ii - 1) * (x2 - x1) / w + x1) / k;
                        double j0 = ((jj - 1) * (y2 - y1) / h + y1) / k;
                        double f = function2d(i, j, z);// j / k * Math.Sin(Math.Sqrt(Math.Abs(j / k))) + i / k * Math.Sin(Math.Sqrt(Math.Abs(i / k)));
                        double f2 = function2d(i0, j, z); //(j - 1) / k * Math.Sin(Math.Sqrt(Math.Abs(j - 1) / k)) + i / k * Math.Sin(Math.Sqrt(Math.Abs(i / k)));
                        double f3 = function2d(i, j0, z); //(j / k) * Math.Sin(Math.Sqrt(Math.Abs(j / k))) + (i - 1) / k * Math.Sin(Math.Sqrt(Math.Abs((i - 1) / k)));
                        double f4 = function2d(i1, j, z); //(j + 1) / k * Math.Sin(Math.Sqrt(Math.Abs(j + 1) / k)) + (i / k) * Math.Sin(Math.Sqrt(Math.Abs(i / k)));
                        double f5 = function2d(i, j1, z); //(j / k) * Math.Sin(Math.Sqrt(Math.Abs(j / k))) + (i + 1) / k * Math.Sin(Math.Sqrt(Math.Abs((i + 1) / k)));
                        double f6 = function2d(i1, j1, z); //(j + 1) / k * Math.Sin(Math.Sqrt(Math.Abs(j + 1) / k)) + (i + 1) / k * Math.Sin(Math.Sqrt(Math.Abs((i + 1) / k)));
                        double f7 = function2d(i0, j1, z); //(j - 1) / k * Math.Sin(Math.Sqrt(Math.Abs(j - 1) / k)) + (i + 1) / k * Math.Sin(Math.Sqrt(Math.Abs((i + 1) / k)));
                        double f8 = function2d(i1, j0, z); //(j + 1) / k * Math.Sin(Math.Sqrt(Math.Abs(j + 1) / k)) + (i - 1) / k * Math.Sin(Math.Sqrt(Math.Abs((i - 1) / k)));
                        double f9 = function2d(i0, j0, z); //(j - 1) / k * Math.Sin(Math.Sqrt(Math.Abs(j - 1) / k)) + (i - 1) / k * Math.Sin(Math.Sqrt(Math.Abs((i - 1) / k)));
                                                           //double f10 = function(i0, j0, 0, 0, z); //(j - 1) / k * Math.Sin(Math.Sqrt(Math.Abs(j - 1) / k)) + (i - 1) / k * Math.Sin(Math.Sqrt(Math.Abs((i - 1) / k)));


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

                if (flag2 == true)
                {
                    {
                        for (int i = 0; i < alg.Population.Count; i++)
                            e.Graphics.FillEllipse(Brushes.Blue, (float)((alg.Population[i].x[0] * k - x1) * w / (x2 - x1) - 3), (float)(h - (alg.Population[i].x[1] * k - y1) * h / (y2 - y1) - 3), 5, 5);
                        //e.Graphics.FillEllipse(Brushes.Red, (float)((alg.Population[i0].x[0] * k - x1) * w / (x2 - x1) - 4), (float)(h - (alg.Population[i0].x[1] * k - y1) * h / (y2 - y1) - 4), 7, 7);

                        // Рисуем красную точку, используя bestX и bestY
                        e.Graphics.FillEllipse(Brushes.Red, (float)((alg.bestX * k - x1) * w / (x2 - x1) - 4), (float)(h - (alg.bestY * k - y1) * h / (y2 - y1) - 4), 7, 7);
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

        private float function2d(double x1, double x2, int f)
        {
            if (f == 13)
            {
                return function(x1, x2, 7.2, 0, f);
            }
            else if (f == 11)
            {
                return function(x1, x2, 42.3, 175.0, f);
            }
            else if (f == 10)
            {
                return function(x1, x2, 9.36, 0.2, f);
            }
            return function(x1, x2, 0, 0, f);
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
            }
            else if (f == 3)
            {
                funct = (float)(0.5 - (Math.Pow(Math.Sin(Math.Sqrt(x1 * x1 + x2 * x2)), 2) - 0.5) / (1 + 0.001 * (x1 * x1 + x2 * x2)));
            }
            else if (f == 4)
            {
                funct = (float)(-x1 * x1 - x2 * x2 + 10 * Math.Cos(2 * Math.PI * x1) + 10 * Math.Cos(2 * Math.PI * x2));


                //funct = (float)((-x1 * x1 + 10 * Math.Cos(2 * Math.PI * x1)) + (-x2 * x2 + 10 * Math.Cos(2*Math.PI * x2)));

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




        

        // private void numericUpDown5_ValueChanged(object sender, EventArgs e)
        // {
        //     k = (float)numericUpDown5.Value;
        //     pictureBox1.Refresh();
        // }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        Form2 forma2 = new Form2();

        private void button8_Click(object sender, EventArgs e)
        {
            if (forma2.IsDisposed) forma2 = new Form2();
            //forma2 = new Form2();
            forma2.averageF = alg.AverF;
            forma2.bestF = alg.BestF;
            forma2.Population = alg.Population;
            forma2.FillList();
            forma2.Show();
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //Process.Start("fish.pdf");
            MessageBox.Show("Здесь будет Справка");
        }


        Form4 forma4 = new Form4();


        private void button3_Click(object sender, EventArgs e)
        {
            
                int z = 10;

                double[,] obl = new double[n, 2];

                int error = 0;

                if (z < 10)
                {
                    try
                    {
                        obl[0, 0] = Convert.ToDouble(dataGridView1.Rows[0].Cells[1].Value);
                        obl[0, 1] = Convert.ToDouble(dataGridView1.Rows[0].Cells[2].Value);
                        for (int k = 1; k < n; k++)
                        {
                            obl[k, 0] = Convert.ToDouble(dataGridView1.Rows[1].Cells[1].Value);
                            obl[k, 1] = Convert.ToDouble(dataGridView1.Rows[1].Cells[2].Value);
                        }
                        obl[n - 1, 0] = obl[0, 0];
                        obl[n - 1, 1] = obl[0, 1];
                    }
                    catch
                    {
                        error = 1;
                    }
                    if ((obl[0, 1] - obl[0, 0] < 0) || (obl[1, 1] - obl[1, 0] < 0)) error = 1;
                }
                else
                {
                    if (z == 10)
                    {
                        obl[0, 0] = 0.1;
                        obl[0, 1] = 2;
                        obl[1, 0] = 0.1;
                        obl[1, 1] = 10;
                        obl[2, 0] = 0.1;
                        obl[2, 1] = 10;
                        obl[3, 0] = 0.1;
                        obl[3, 1] = 2;
                    }
                    else if (z == 11)
                    {
                        obl[0, 0] = 1;
                        obl[0, 1] = 99.99;
                        obl[1, 0] = 1;
                        obl[1, 1] = 99.99;
                        obl[2, 0] = 10;
                        obl[2, 1] = 200;
                        obl[3, 0] = 10;
                        obl[3, 1] = 200;
                    }
                    else if (z == 12)
                    {
                        obl[0, 0] = 2.6;
                        obl[0, 1] = 3.6;
                        obl[1, 0] = 0.7;
                        obl[1, 1] = 0.8;
                        obl[2, 0] = 17;
                        obl[2, 1] = 28.99;
                        obl[3, 0] = 7.3;
                        obl[3, 1] = 8.3;
                        obl[4, 0] = 7.8;
                        obl[4, 1] = 8.3;
                        obl[5, 0] = 2.9;
                        obl[5, 1] = 3.9;
                        obl[6, 0] = 5;
                        obl[6, 1] = 5.5;
                    }
                    else if (z == 13)
                    {
                        obl[0, 0] = 0.05;
                        obl[0, 1] = 0.2;
                        obl[1, 0] = 0.25;
                        obl[1, 1] = 1.3;
                        obl[2, 0] = 2;
                        obl[2, 1] = 15;

                    }
                }

                int Ip = 0;
                int K = 0;
                int groupSize = 0;
                int groupCount = 0;
                double weight = 0;
                
                try
                {
                   
                    K = Convert.ToInt32(dataGridView2.Rows[0].Cells[1].Value);
                    groupSize = Convert.ToInt32(dataGridView2.Rows[1].Cells[1].Value);
                    groupCount = Convert.ToInt32(dataGridView2.Rows[2].Cells[1].Value);
                    Ip = groupSize * groupCount;
                    weight = Convert.ToDouble(dataGridView2.Rows[4].Cells[1].Value);


                }
                catch
                {
                    error = 2;
                }
                /*if ((Ip <= 0) || (K <= 0) ||  (thr <= 0) || (wScale <= 0)) error = 2;*/

                if (error == 0)
                {
                    flag2 = false;
                    if ((forma4.IsDisposed) || (first == true))
                    {
                        first = false;
                        forma4 = new Form4();
                        alg = new Algoritm();
                        alg.Init(
    k: K,
    obl: obl,
    n: n,
    z_: z,
    groupSize: groupSize,
    groupCount: groupCount,
    weight: weight
);

                        forma4.algst = alg;

                        forma4.showobl = showobl;
                        
                        forma4.Ar = Ar;
                        forma4.flines = flines;
                        forma4.exact = exact;
                        forma4.Ip = Ip;
                        forma4.K = K;
                        forma4.obl = obl;
                        forma4.groupSize = groupSize;
                        forma4.groupCount = groupCount;
                        forma4.weight = weight;


                        forma4.n = n;


                        forma4.A = A;
                        forma4.Ar = Ar;
                        forma4.flines = flines;
                        forma4.obl = obl;
                        forma4.showobl = showobl;
                        forma4.showoblbase = showoblbase;
                        
                        forma4.Show();
                    }
                }
                else switch (error)
                    {
                        case 1:
                            MessageBox.Show("Неверно введена область определения функции!");
                            break;
                        case 2:
                            MessageBox.Show("Неверно введены параметры алгоритма!");
                            break;
                    }



            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Process.Start("protocol.dt");
        }

        Form3 forma3 = new Form3();

        

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
           
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            showobl[0, 0] = Convert.ToDouble(dataGridView1.Rows[0].Cells[1].Value);
            showobl[0, 1] = Convert.ToDouble(dataGridView1.Rows[0].Cells[2].Value);
            showobl[1, 0] = Convert.ToDouble(dataGridView1.Rows[1].Cells[1].Value);
            showobl[1, 1] = Convert.ToDouble(dataGridView1.Rows[1].Cells[2].Value);

            showoblbase[0, 0] = Convert.ToDouble(dataGridView1.Rows[0].Cells[1].Value);
            showoblbase[0, 1] = Convert.ToDouble(dataGridView1.Rows[0].Cells[2].Value);
            showoblbase[1, 0] = Convert.ToDouble(dataGridView1.Rows[1].Cells[1].Value);
            showoblbase[1, 1] = Convert.ToDouble(dataGridView1.Rows[1].Cells[2].Value);

            
        }

        private async void button2_Click_1(object sender, EventArgs e)
        {
            int z = 10;
            double[,] obl = new double[n, 2];
            int error = 0;

            try
            {
                obl[0, 0] = Convert.ToDouble(dataGridView1.Rows[0].Cells[1].Value);
                obl[0, 1] = Convert.ToDouble(dataGridView1.Rows[0].Cells[2].Value);
                for (int k = 1; k < n; k++)
                {
                    obl[k, 0] = Convert.ToDouble(dataGridView1.Rows[k].Cells[1].Value);
                    obl[k, 1] = Convert.ToDouble(dataGridView1.Rows[k].Cells[2].Value);
                }
            }
            catch { error = 1; }

            if ((obl[0, 1] - obl[0, 0] < 0) || (obl[1, 1] - obl[1, 0] < 0)) error = 1;

            int K = 0, groupSize = 0, groupCount = 0;
            double weight = 0;
            try
            {
                K = Convert.ToInt32(dataGridView2.Rows[0].Cells[1].Value);
                groupSize = Convert.ToInt32(dataGridView2.Rows[1].Cells[1].Value);
                groupCount = Convert.ToInt32(dataGridView2.Rows[2].Cells[1].Value);
                weight = Convert.ToDouble(dataGridView2.Rows[4].Cells[1].Value);
            }
            catch { error = 2; }

            if (error != 0)
            {
                MessageBox.Show(error == 1 ? "Неверно введена область определения функции!" : "Неверно введены параметры алгоритма!");
                return;
            }

            // Выполнение тяжёлой части в фоне
            var result = await Task.Run(() =>
            {
                double bestF = double.MaxValue;
                double bestX = 0, bestY = 0;
                Algoritm bestAlg = null;

                for (int i = 0; i < 500; i++)
                {
                    Algoritm alg = new Algoritm();
                    alg.Init(K, obl, n, z, groupSize, groupCount, weight);
                    alg.Create_Pop();
                    alg.Work();

                    if (-alg.maxDf < bestF)
                    {
                        bestF = -alg.maxDf;
                        bestX = alg.bestX;
                        bestY = alg.bestY;
                        bestAlg = alg;
                    }
                }

                return (bestAlg, bestX, bestY, bestF);
            });

            // Обработка результатов на UI-потоке
            if (result.bestAlg != null)
            {
                var bestAlg = result.bestAlg;
                double bestX = result.bestX;
                double bestY = result.bestY;
                double bestF = result.bestF;
                int Ip = groupSize * groupCount;

                string header =
                    "---------------------------------------------------------\n" +
                    $"Параметры алгоритма:\n" +
                    $"Итераций: {K}\n" +
                    $"Размер группы: {groupSize}\n" +
                    $"Количество групп: {groupCount}\n" +
                    $"Общее число человек: {Ip}\n" +
                    $"Вес позиции (weight): {weight}\n" +
                    $"Область определения: [x: {obl[0, 0]}..{obl[0, 1]}], [y: {obl[1, 0]}..{obl[1, 1]}]\n";

                Console.WriteLine(header);
                Console.WriteLine($"Лучшее из 100 запусков: λ1 = {bestX:F6}, λ2 = {bestY:F6}, f = {bestF:F6}");
                bestAlg.CheckConstraints(bestX, bestY);

                using (StreamWriter writer = new StreamWriter("results.txt", true))
                {
                    writer.WriteLine(header);
                    writer.WriteLine($"Лучшее из 100 запусков: λ1 = {bestX:F6}, λ2 = {bestY:F6}, f = {bestF:F6}");

                    double g1 = 2 * ((Math.Sqrt(2) * bestX + bestY) / (Math.Sqrt(2) * bestX * bestX + 2 * bestX * bestY) - 1);
                    double g2 = 2 * (bestY / (Math.Sqrt(2) * bestX * bestX + 2 * bestX * bestY) - 1);
                    double g3 = 2 * (1 / (Math.Sqrt(2) * bestY + bestX) - 1);

                    writer.WriteLine("Проверка ограничений:");
                    writer.WriteLine($"g1 = {g1:F6} {(g1 <= 0 ? "OK" : "НАРУШЕНО")}");
                    writer.WriteLine($"g2 = {g2:F6} {(g2 <= 0 ? "OK" : "НАРУШЕНО")}");
                    writer.WriteLine($"g3 = {g3:F6} {(g3 <= 0 ? "OK" : "НАРУШЕНО")}");

                    if (bestX < 0 || bestX > 1 || bestY < 0 || bestY > 1)
                        writer.WriteLine("НАРУШЕНЫ границы переменных!");
                    else
                        writer.WriteLine("Границы переменных: OK");
                }
            }
        }

        // 3. Новый метод, принимающий параметры алгоритма
        private AlgoritmInfo RunAlgorithm(
    Algoritm alg,
    int k,
    double[,] obl,
    int n,
    int z_,
    int groupSize,
    int groupCount,
    double weight,
    double exact
)
        {
            
            alg.Init(k, obl, n, z_, groupSize, groupCount, weight);
            alg.Create_Pop();
            alg.Work();

            double deviation = exact - alg.maxDf;  // или Math.Abs(exact - alg.maxDf);
            alg.Deviations.Add(deviation);

            AlgoritmInfo algoritmInfo = new AlgoritmInfo { Deviation = deviation, MaxDf = alg.maxDf };

            // Выводим значение функции (maxDf) в консоль
            Console.WriteLine($"maxDf = {alg.maxDf:F6}");
            return algoritmInfo;
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }
    }
}
