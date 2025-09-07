using System;
using System.Collections.Generic;
using System.Linq;

namespace TLBO
{
    public class Algoritm
    {
        public int Ip;
        public int ReplaceCount;
        public int ReplaceFrequency;
        public int K;
        public int N;
        public double Teach;
        public double Stud;
        public double[,] oblast;
        public int z;
        
        

        public double Mfend = 0;

        public List<double> AverF = new List<double>();
        public List<double> BestF = new List<double>();
        public double bestX;
        public double bestY;

        public List<double> Deviations = new List<double>(); // Отклонения от точного решения
        public double AverageDeviation = 0; // Среднее отклонение
        public double MinDeviation = double.MaxValue; // Наименьшее отклонение
        public double StandardDeviation = 0; // Среднеквадратическое отклонение
        public int SuccessCount = 0; // Количество успешных запусков
       
        

        Random R = new Random();

        
        private int m;
        private int k;

        public int Nf = 0;

        

        

        public double[,] memoryMatrix;

        public void Init(int ip, int k, double[,] obl, int n, int z_, int replaceCount, int replaceFrequency, double teach,double stud)
        {
            Ip = ip;
            K = k;
            ReplaceFrequency = replaceFrequency;
            ReplaceCount = replaceCount;
            N = n;
            oblast = obl;
            z = z_;
            Teach = teach;
            Stud = stud;
            
           
        }

        public class Student
        {
            public Algoritm A;
            public double[] x;
            public double f;

            public Student(Algoritm alg)
            {
                A = alg;
                x = new double[A.N];
                f = 0;
            }

            
        }

        public List<Student> Population = new List<Student>();

        public void Create_Pop()
        {
            Population = new List<Student>();
            memoryMatrix = new double[Ip, N];

            for (int i = 0; i < Ip; i++)
            {
                Student student = new Student(this);
                for (int j = 0; j < N; j++)
                {
                    student.x[j] = R.NextDouble() * (oblast[j, 1] - oblast[j, 0]) + oblast[j, 0];
                    memoryMatrix[i, j] = student.x[j];
                }
                student.f = Evaluate(student.x);
                Population.Add(student);
            }
            Analyze();
        }

        public void TeacherPhase()
        {
            double[] meanX = new double[N];
            for (int j = 0; j < N; j++)
            {
                double sum = 0;
                foreach (var student in Population)
                {
                    sum += student.x[j];
                }
                meanX[j] = sum / Population.Count;
            }

            Student teacher = Population.OrderByDescending(s => s.f).First();



            foreach (var student in Population)
            {
                if (student == teacher) continue;
                int TF = R.NextDouble() < 0.5 ? 1 : 2;

                double[] deltaX = new double[N];
                for (int j = 0; j < N; j++)
                {
                    double R_ = R.NextDouble();

                    deltaX[j] =  Teach * R_ * (teacher.x[j] - TF * meanX[j]);
                }

                double[] newX = new double[N];
                for (int j = 0; j < N; j++)
                {
                    newX[j] = student.x[j] + deltaX[j];
                    newX[j] = Math.Max(oblast[j, 0], Math.Min(oblast[j, 1], newX[j]));
                }

                double newf = Evaluate(newX);


                

                    Array.Copy(newX, student.x, N);
                    student.f = newf;
                    if (newf > maxDf)
                    {

                        maxDf = newf;
                        bestX = newX[0];
                        bestY = newX[1];
                    }
                
            }
        }
        public void LearnerPhase()
        {
            Student teacher = Population.OrderByDescending(s => s.f).First();

            for (int i = 0; i < Population.Count; i++)
            {
                // Исключаем учителя из фазы взаимодействия
                if (Population[i] == teacher) continue;
                int r;
                do
                {
                    r = R.Next(Population.Count);
                } while (r == i);

                if (Population[i].f < Population[r].f)
                {
                    for (int j = 0; j < N; j++)
                    {
                        double randomValueIf = R.NextDouble();
                        Population[i].x[j] += Stud * randomValueIf * (Population[i].x[j] - Population[r].x[j]); 
                        Population[i].x[j] = Math.Max(oblast[j, 0], Math.Min(oblast[j, 1], Population[i].x[j]));
                    }
                }
                else
                {
                    for (int j = 0; j < N; j++)
                    {
                        double randomValueElse = R.NextDouble();
                        Population[i].x[j] += Stud * randomValueElse * (Population[r].x[j] - Population[i].x[j]); 
                        Population[i].x[j] = Math.Max(oblast[j, 0], Math.Min(oblast[j, 1], Population[i].x[j]));
                    }
                }

                Population[i].f = Evaluate(Population[i].x);

                if (Population[i].f > maxDf)
                {
                    maxDf = Population[i].f;
                    bestX = Population[i].x[0];
                    bestY = Population[i].x[1];
                }
            }
        }

        public void ReproduceAndReplace(int currentIteration)
        {
            if ((currentIteration % ReplaceFrequency != 0) || (currentIteration == 0)) return; // Проверяем частоту замены

            // Сортируем популяцию по возрастанию значения целевой функции (от худшего к лучшему)
            Population = Population.OrderBy(s => s.f).ToList();

            int numReplaced = Math.Min(ReplaceCount, Population.Count); // Количество заменяемых учеников (не больше размера популяции)

            for (int i = 0; i < numReplaced; i++)
            {
                // Создаем нового ученика
                Student newStudent = new Student(this);
                for (int j = 0; j < N; j++)
                {
                    // Генерируем случайные координаты в пределах области поиска
                    newStudent.x[j] = R.NextDouble() * (oblast[j, 1] - oblast[j, 0]) + oblast[j, 0];
                }
                newStudent.f = Evaluate(newStudent.x); // Вычисляем значение целевой функции

                // Заменяем слабого ученика новым учеником
                Population[i] = newStudent;
            }

            // Сортируем популяцию обратно (по убыванию значения целевой функции)
            Population = Population.OrderByDescending(s => s.f).ToList();
        }

        public void Analyze(bool final = false)
        {
            RecalcMaxf();
            if (!final) return;
            Mfend = Math.Round(Mf, 6);
        }

        // текущая итерация
        public int kpop
        {
            get { return k; }
            set { k = value; }
        }

        public void Work()
        {
            maxDf = double.MinValue;
            
            for (m = 0; m < K; m++)
            {
                k = m;
                TeacherPhase();
                LearnerPhase();
                ReproduceAndReplace(m);
                Analyze();
            }
        }

        public void RecalcMaxf()
        {

            var S = 0.0;
            for (int j = 0; j < Population.Count; j++)
            {
                Student member = Population[j];
                if (member.f > maxDf)
                {
                    maxDf = member.f;
                    bestX = member.x[0];
                    bestY = member.x[1];
                }
                S += member.f;
            }
            BestF.Add(maxDf);
            Mf = S / Population.Count;
            AverF.Add(Mf);
            Population = Population.OrderByDescending(s => s.f).ToList();
        }

        private double Evaluate(double[] x)
        {
            double func = 0;
            if (z == 0)
                func = x[0] * Math.Sin(Math.Sqrt(Math.Abs(x[0]))) + x[1] * Math.Sin(Math.Sqrt(Math.Abs(x[1])));
            else if (z == 1)
                func = x[0] * Math.Sin(4 * Math.PI * x[0]) - x[1] * Math.Sin(4 * Math.PI * x[1] + Math.PI) + 1;
            else if (z == 2)
            {
                double[] c6 = Cpow(x[0], x[1], 6);
                func = (float)(1 / (1 + Math.Sqrt((c6[0] - 1) * (c6[0] - 1) + c6[1] * c6[1])));
            }
            else if (z == 3)
            {
                func = (float)(0.5 - (Math.Pow(Math.Sin(Math.Sqrt(x[0] * x[0] + x[1] * x[1])), 2) - 0.5) / (1 + 0.001 * (x[0] * x[0] + x[1] * x[1])));
            }
            else if (z == 4)
            {
                func = (float)((-x[0] * x[0] + 10 * Math.Cos(2 * Math.PI * x[0])) + (-x[1] * x[1] + 10 * Math.Cos(Math.PI * x[1])));
            }
            else if (z == 5)
            {
                func = (float)(-Math.E + 20 * Math.Exp(-0.2 * Math.Sqrt((x[0] * x[0] + x[1] * x[1]) / 2)) + Math.Exp((Math.Cos(2 * Math.PI * x[0]) + Math.Cos(2 * Math.PI * x[1])) / 2));
            }
            else if (z == 6)
            {
                func = (float)(Math.Pow(Math.Cos(2 * x[0] * x[0]) - 1.1, 2) + Math.Pow(Math.Sin(0.5 * x[0]) - 1.2, 2) - Math.Pow(Math.Cos(2 * x[1] * x[1]) - 1.1, 2) + Math.Pow(Math.Sin(0.5 * x[1]) - 1.2, 2));
            }
            else if (z == 7)
            {
                func = (float)(-Math.Sqrt(Math.Abs(Math.Sin(Math.Sin(Math.Sqrt(Math.Abs(Math.Sin(x[0] - 1))) + Math.Sqrt(Math.Abs(Math.Sin(x[1] + 2))))))) + 1);
            }
            else if (z == 8)
            {
                func = (float)(-(1 - x[0]) * (1 - x[0]) - 100 * (x[1] - x[0] * x[0]) * (x[1] - x[0] * x[0]));
            }
            else if (z == 9)
            {
                func = (float)(1 - x[0] * x[0] - x[1] * x[1]);
            }

            Nf++;
            return func;
        }

        private static double[] Cpow(double x, double y, int p)
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

        

            

            

            
        public double maxDf = 0;
        double Mf;

        
        public void CalculateMetrics()
        {
            // 1. Среднее значение отклонения от точного
            if (Deviations.Count > 0)
            {
                AverageDeviation = Deviations.Average();
            }
            else
            {
                AverageDeviation = double.NaN; //  Если нет данных, то NaN
            }

            // 2. Наименьшее значение отклонения
            if (Deviations.Count > 0)
            {
                MinDeviation = Deviations.Min();
            }
            else
            {
                MinDeviation = double.NaN; //  Если нет данных, то NaN
            }


            // 3. Среднеквадратическое отклонение
            if (Deviations.Count > 1) // Для расчета СКО нужно минимум 2 значения
            {
                double sumOfSquaresOfDifferences = Deviations.Select(dev => (dev - AverageDeviation) * (dev - AverageDeviation)).Sum();
                StandardDeviation = Math.Sqrt(sumOfSquaresOfDifferences / (Deviations.Count));
            }
            else
            {
                StandardDeviation = double.NaN; //  Если недостаточно данных, то NaN
            }


            // 4. Количество успехов (в пределах погрешности)
            double epsilonX = 0.001 * (oblast[0, 1] - oblast[0, 0]);
            double epsilonY = 0.001 * (oblast[1, 1] - oblast[1, 0]);

            // Используем наибольшее значение Epsilon для критерия успеха
            double Epsilon = Math.Max(epsilonX, epsilonY); // Или как вы хотите объединить окрестности
            Console.WriteLine($"Значение эпсилон {Epsilon}");
            // 4. Количество успехов (в пределах погрешности)
            SuccessCount = Deviations.Count(dev => Math.Abs(dev) <= Epsilon); //  В пределах погрешности

        }
    }
}