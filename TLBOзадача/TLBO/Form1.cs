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
            dataGridView2.RowCount = 6;
            dataGridView2.Rows[0].Cells[0].Value = "Количество учеников:";                
            dataGridView2.Rows[1].Cells[0].Value = "Количество итераций:";                       
            dataGridView2.Rows[2].Cells[0].Value = "Число заменяемых учеников:";
            dataGridView2.Rows[3].Cells[0].Value = "Частота замены учеников:";
            dataGridView2.Rows[4].Cells[0].Value = "Коэффициент учителя:";
            dataGridView2.Rows[5].Cells[0].Value = "Коэффициент учеников:";
            dataGridView1.Rows[0].Cells[1].Value = "0";
            dataGridView1.Rows[0].Cells[2].Value = "1";
            dataGridView1.Rows[1].Cells[1].Value = "0";
            dataGridView1.Rows[1].Cells[2].Value = "1";
            dataGridView2.Rows[0].Cells[1].Value = "100";
            dataGridView2.Rows[1].Cells[1].Value = "1000";
            dataGridView2.Rows[2].Cells[1].Value = "1";
            dataGridView2.Rows[3].Cells[1].Value = "1";
            dataGridView2.Rows[4].Cells[1].Value = "1";
            dataGridView2.Rows[5].Cells[1].Value = "1";

            exact = 263.87;

            dataGridView3.RowCount = 7;
                      
                        
            dataGridView3.Rows[0].Cells[0].Value = "(x*; y*):";                                  
            dataGridView3.Rows[1].Cells[0].Value = "f*:";
            dataGridView3.Rows[2].Cells[0].Value = "Лучшее известное решение:";
            dataGridView3.Rows[3].Cells[0].Value = "Δ:";
            dataGridView3.Rows[4].Cells[0].Value = "g1:";
            dataGridView3.Rows[5].Cells[0].Value = "g2:";
            dataGridView3.Rows[6].Cells[0].Value = "g3:";
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
            int ReplaceCount = 0;
            int ReplaceFrequency = 0;
            double Teach = 0;
            double Stud = 0;

            try
            {
                Ip = Convert.ToInt32(dataGridView2.Rows[0].Cells[1].Value);
                K = Convert.ToInt32(dataGridView2.Rows[1].Cells[1].Value);
                ReplaceCount = Convert.ToInt32(dataGridView2.Rows[2].Cells[1].Value);
                ReplaceFrequency = Convert.ToInt32(dataGridView2.Rows[3].Cells[1].Value);
                Teach = Convert.ToDouble(dataGridView2.Rows[4].Cells[1].Value);
                Stud = Convert.ToDouble(dataGridView2.Rows[5].Cells[1].Value);

            }
            catch
            {
                error = 2;
            }

            if (error == 0)
            {
                Algoritm alg = new Algoritm();
                alg.Init(Ip, K, obl, n, z, ReplaceCount, ReplaceFrequency, Teach, Stud);
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
                int ReplaceCount = 0;
                int ReplaceFrequency = 0;
                double Teach = 0;
                double Stud = 0;



                try
                {
                    Ip = Convert.ToInt32(dataGridView2.Rows[0].Cells[1].Value);
                    K = Convert.ToInt32(dataGridView2.Rows[1].Cells[1].Value);
                    ReplaceCount = Convert.ToInt32(dataGridView2.Rows[2].Cells[1].Value);
                    ReplaceFrequency = Convert.ToInt32(dataGridView2.Rows[3].Cells[1].Value);
                    Teach = Convert.ToDouble(dataGridView2.Rows[4].Cells[1].Value);
                    Stud = Convert.ToDouble(dataGridView2.Rows[5].Cells[1].Value);
                if (K < 1)
                {
                    MessageBox.Show("Минимальное количество итераций =1");
                    return; // остановка метода
                }
                if (Ip < 2)
                {
                    MessageBox.Show("Минимальное количество учеников =2");
                    return; // остановка метода
                }
                if (ReplaceCount < 0)
                {
                    MessageBox.Show("Минимальное количество заменяемых учеников =0");
                    return; // остановка метода
                }
                if (ReplaceCount > Ip)
                {
                    MessageBox.Show("Количество заменяемых учеников не должно превышать общее количество учеников");
                    return; // остановка метода
                }
                if (ReplaceFrequency < 1)
                {
                    MessageBox.Show("Минимальная частота замены учеников =1");
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
                    alg.Init(Ip, K, obl, n, z, ReplaceCount, ReplaceFrequency, Teach, Stud);
                    
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








                    double minF = -alg.maxDf;
                    dataGridView3.Rows[0].Cells[1].Value = "(" + alg.bestX.ToString("F6") + "; " + alg.bestY.ToString("F6") + ")"; // Используем bestX и bestY
                    dataGridView3.Rows[1].Cells[1].Value = Math.Round(minF, 6).ToString(); // Используем maxDf
                    dataGridView3.Rows[2].Cells[1].Value = z < 11 ? exact.ToString() : "";
                    dataGridView3.Rows[3].Cells[1].Value = z < 11 ? Math.Round((exact - Math.Round(minF, 6)), 6).ToString() : "";
                    var (g1, g2, g3) = CalculateConstraints(alg.bestX, alg.bestY);
                    dataGridView3.Rows[4].Cells[1].Value = g1.ToString("F6");
                    dataGridView3.Rows[5].Cells[1].Value = g2.ToString("F6");
                    dataGridView3.Rows[6].Cells[1].Value = g3.ToString("F6");




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
                if (i < x.Length-1) s += "; ";
            }
            return s;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            n = 2;
            
            
            {
                dataGridView1.Rows[0].Cells[1].Value = "0";
                dataGridView1.Rows[0].Cells[2].Value = "1";
                dataGridView1.Rows[1].Cells[1].Value = "0";
                dataGridView1.Rows[1].Cells[2].Value = "1";
                dataGridView2.Rows[0].Cells[1].Value = "100";
                dataGridView2.Rows[1].Cells[1].Value = "1000";
                dataGridView2.Rows[2].Cells[1].Value = "1";
                dataGridView2.Rows[3].Cells[1].Value = "1";
                dataGridView2.Rows[4].Cells[1].Value = "1";
                dataGridView2.Rows[5].Cells[1].Value = "1";

                exact = 263.87;

                A[0] = -7F;
                A[1] = -4F;
                A[2] = -2F;//0.5000001F;
                A[3] = -0.8F;
                A[4] = -0.1F;
                flag = true;
                
            }

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
                funct =(float) (x1 * Math.Sin(Math.Sqrt(Math.Abs(x1))) + x2 * Math.Sin(Math.Sqrt(Math.Abs(x2))));
            }
            else if (f == 1)
            {
                funct = (float)(x1 * Math.Sin(4 * Math.PI * x1) - x2 * Math.Sin(4 * Math.PI * x2 + Math.PI) + 1);
            }
            else if (f == 2)
            { 
                double[] c6 = Cpow(x1,x2,6);
                funct = (float)(1 / (1 + Math.Sqrt((c6[0] - 1) * (c6[0] - 1) + c6[1] * c6[1])));
            }
            else if (f == 3)
            {
                funct = (float)(0.5-(Math.Pow(Math.Sin(Math.Sqrt(x1*x1+x2*x2)),2)-0.5)/(1+0.001*(x1*x1+x2*x2)));
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
                funct = (float)(-Math.Sqrt(Math.Abs(Math.Sin(Math.Sin(Math.Sqrt(Math.Abs(Math.Sin(x1-1)))+Math.Sqrt(Math.Abs(Math.Sin(x2+2)))))))+1);
            }
            else if (f == 8)
            {
                funct = (float)(-(1 - x1) * (1 - x1) - 100 * (x2 - x1 * x1) * (x2 - x1 * x1));
            }
            else if (f == 9)
            {
                funct = (float)(1-x1 * x1 - x2 * x2);
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
            if (true)
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
                int ReplaceCount = 0;
                int ReplaceFrequency=0;
                double Teach = 0;
                double Stud = 0;
                //int Klon = 0;
                //double parkl = 0;
                //double g = 0;
                //double eps = 0;
                //double sigma = 0;
                //int percent = 0;
                try
                {
                    Ip = Convert.ToInt32(dataGridView2.Rows[0].Cells[1].Value);
                    K = Convert.ToInt32(dataGridView2.Rows[1].Cells[1].Value);
                    ReplaceCount = Convert.ToInt32(dataGridView2.Rows[2].Cells[1].Value);
                    ReplaceFrequency = Convert.ToInt32(dataGridView2.Rows[3].Cells[1].Value);
                    Teach = Convert.ToDouble(dataGridView2.Rows[4].Cells[1].Value);
                    Stud = Convert.ToDouble(dataGridView2.Rows[5].Cells[1].Value);

                }
                catch
                {
                    error = 2;
                }
                /*if ((Ip <= 0) || (K <= 0) ||  (thr <= 0) || (wScale <= 0)) error = 2;*/

                if (error == 0)
                {
                    flag2 = false;
                    if ((forma4.IsDisposed)||(first==true))
                    {
                        first = false;
                        forma4 = new Form4();
                        alg = new Algoritm();
                        alg.Init(Ip, K, obl, n, z, ReplaceCount, ReplaceFrequency, Teach, Stud);
                        
                        forma4.algst = alg;
                        
                        forma4.showobl = showobl;
                        forma4.z = 10;
                        forma4.Ar = Ar;
                        forma4.flines = flines;
                        forma4.exact = exact;
                        forma4.Ip = Ip;
                        forma4.K = K;
                        forma4.obl = obl;
                        forma4.ReplaceCount = ReplaceCount;
                        forma4.ReplaceFrequency = ReplaceFrequency;
                        forma4.Teach = Teach;
                        forma4.Stud= Stud;

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
            else MessageBox.Show("Выберите целевую функцию!");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Process.Start("protocol.dt");
        }

        Form3 forma3 = new Form3();

        private void button7_Click(object sender, EventArgs e)
        {
            if (true)
            {
                if (forma3.IsDisposed) forma3 = new Form3();
                forma3.A = A;
                forma3.Ar = Ar;
                forma3.flines = flines;
                forma3.obl = showobl;
                forma3.oblbase = showoblbase;
                forma3.Show();
                //MessageBox.Show("Здесь будет открываться окно с настройками: \r\n 1) объяснение, какому уровню соответствуют разные цвета и возможность поменять уровень \r\n 2) возможность добавить уровень \r\n 3) возможность поменять показываемый диапазон");
            }
            else MessageBox.Show("Выберите целевую функцию!");
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

        private async void button2_Click_1(object sender, EventArgs e) // Это название метода, который вызывается при нажатии на кнопку
        {
            // 1. Получаем данные из UI элементов в основном потоке

            // Получаем индекс выбранного элемента в comboBox1
            int z = 10;

            // Создаем массивы для данных из DataGridView
            double[,] obl = new double[n, 2];

            // Получаем значения из dataGridView1
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
                MessageBox.Show("Ошибка при чтении области определения функции!");
                return; // Прерываем выполнение, если произошла ошибка
            }

            // Получаем параметры алгоритма из dataGridView2
            int Ip = 0;
            int K = 0;
            int ReplaceCount = 0;
            int ReplaceFrequency = 0;
            double Teach = 0;
            double Stud = 0;

            try
            {
                Ip = Convert.ToInt32(dataGridView2.Rows[0].Cells[1].Value);
                K = Convert.ToInt32(dataGridView2.Rows[1].Cells[1].Value);
                ReplaceCount = Convert.ToInt32(dataGridView2.Rows[2].Cells[1].Value);
                ReplaceFrequency = Convert.ToInt32(dataGridView2.Rows[3].Cells[1].Value);
                Teach = Convert.ToDouble(dataGridView2.Rows[4].Cells[1].Value);
                Stud = Convert.ToDouble(dataGridView2.Rows[5].Cells[1].Value);
            }
            catch
            {
                MessageBox.Show("Ошибка при чтении параметров алгоритма!");
                return; // Прерываем выполнение, если произошла ошибка
            }

            if (z == -1) //если не выбрана функция
            {
                MessageBox.Show("Выберите целевую функцию!");
                return;
            }


            // 2.  Запускаем алгоритм в отдельном потоке, передавая данные
            if (z >= 0)
            {
                int numRuns = 100;
                List<double> results = new List<double>();

                Algoritm alg = new Algoritm(); //  Создаём один экземпляр Algoritm для всех запусков
                
                alg.Deviations.Clear(); //  Очищаем Deviations

                // Отключаем кнопку, чтобы избежать повторных нажатий
                button2.Enabled = false;

                // Индикатор выполнения (ProgressBar)
                ProgressBar progressBar = new ProgressBar();
                progressBar.Minimum = 0;
                progressBar.Maximum = numRuns;
                progressBar.Value = 0;
                progressBar.Dock = DockStyle.Bottom; // Прикрепим ProgressBar к низу формы
                Controls.Add(progressBar);


                // 
                string filePath = "results.txt";
                try
                {

                    using (StreamWriter writer = new StreamWriter(filePath, true)) // true - дозапись в файл
                    {

                        // Запускаем алгоритм много раз
                        for (int i = 0; i < numRuns; i++)
                        {
                            // Запускаем алгоритм асинхронно, чтобы не блокировать UI
                            AlgoritmInfo algoritmInfo = await Task.Run(() => RunAlgorithm(alg, Ip, K, obl, n, z, ReplaceCount, ReplaceFrequency, Teach, Stud,  exact));

                            if (!double.IsNaN(algoritmInfo.MaxDf))
                            {
                                results.Add(algoritmInfo.MaxDf);

                                // Записываем значение целевой функции (maxDf) в файл и выводим в консоль
                                string outputString = $"Запуск {i + 1}: Результат = {algoritmInfo.MaxDf:F6}, Отклонение = {exact - algoritmInfo.MaxDf:F6}";
                                Console.WriteLine(outputString);
                                writer.WriteLine(outputString);

                            }
                            else
                            {
                                Console.WriteLine($"Запуск {i + 1}: Ошибка!");
                                writer.WriteLine($"Запуск {i + 1}: Ошибка!");
                            }

                            // Обновляем ProgressBar
                            progressBar.Value = i + 1;
                            Application.DoEvents(); // Даем UI возможность обновиться
                        }


                        

                        //  3. Вычисляем метрики после всех запусков
                        alg.CalculateMetrics();


                        writer.WriteLine($"--- Результаты для прикладной задачи ---"); // Заголовок
                        writer.WriteLine($"Среднее отклонение: {alg.AverageDeviation:F6}");
                        writer.WriteLine($"Наименьшее отклонение: {alg.MinDeviation:F6}");
                        writer.WriteLine($"Среднеквадратическое отклонение: {alg.StandardDeviation:F6}");
                        writer.WriteLine($"Количество успехов (в e окрестности): {alg.SuccessCount}");
                        writer.WriteLine("---------------------------------------------------");

                        Console.WriteLine($"--- Результаты для прикладной задачи ---"); // Заголовок
                        Console.WriteLine($"Среднее отклонение: {alg.AverageDeviation:F6}");
                        Console.WriteLine($"Наименьшее отклонение: {alg.MinDeviation:F6}");
                        Console.WriteLine($"Среднеквадратическое отклонение: {alg.StandardDeviation:F6}");
                        Console.WriteLine($"Количество успехов (в e окрестности): {alg.SuccessCount}");
                        Console.WriteLine("---------------------------------------------------");
                    }
                    MessageBox.Show($"Результаты записаны в файл: {filePath}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при записи в файл: {ex.Message}");
                }
                finally
                {
                    // Включаем кнопку обратно, независимо от того, произошла ошибка или нет
                    button2.Enabled = true;
                    Controls.Remove(progressBar);
                }
            }

        }

        // 3. Новый метод, принимающий параметры алгоритма
        private AlgoritmInfo RunAlgorithm(Algoritm alg, int ip, int k, double[,] obl, int n, int z_, int replaceCount, int replaceFrequency, double teach, double stud, double exact)
        {

            alg.Init(ip, k, obl, n, z_, replaceCount, replaceFrequency, teach, stud);
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

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
