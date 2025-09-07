using System;
using System.Collections.Generic;
using System.Linq;

namespace TLBO
{
    public enum Role { Leader, Advocate, Believer }

    public class Algoritm
    {
        public int K;
        public int N;
        public double[,] oblast;
        public int z;
        public int GroupCount;
        public int GroupSize;
        public double Weight;

        public double Mfend = 0;
        public List<double> AverF = new List<double>();
        public List<double> BestF = new List<double>();
        public double bestX;
        public double bestY;
        public List<double> Deviations = new List<double>();
        public double AverageDeviation = 0;
        public double MinDeviation = double.MaxValue;
        public double StandardDeviation = 0;
        public int SuccessCount = 0;

        Random R = new Random();
        private int m;
        private int k;
        public int Nf = 0;
        public double maxDf = 0;
        double Mf;

        public List<Group> Groups = new List<Group>();
        public Student GlobalBestLeader;

        public class Student
        {
            public double[] x;
            public double f;
            public Role Role;

            public Student(int dimension)
            {
                x = new double[dimension];
                f = 0;
            }
        }

        public class Group
        {
            public List<Student> Members = new List<Student>();

            public Student Leader => Members.FirstOrDefault(m => m.Role == Role.Leader);
        }

        public void Init(int k, double[,] obl, int n, int z_, int groupSize, int groupCount, double weight)
        {
            K = k;
            N = n;
            oblast = obl;
            z = z_;
            GroupCount = groupCount;
            GroupSize = groupSize;
            Weight = weight;
        }

        public void Create_Pop()
        {
            Groups = new List<Group>();

            for (int g = 0; g < GroupCount; g++)
            {
                Group group = new Group();

                for (int i = 0; i < GroupSize; i++)
                {
                    Student student = new Student(N);
                    for (int j = 0; j < N; j++)
                    {
                        student.x[j] = R.NextDouble() * (oblast[j, 1] - oblast[j, 0]) + oblast[j, 0];
                    }
                    student.f = Evaluate(student.x);
                    group.Members.Add(student);
                }
                Groups.Add(group);
            }
            Analyze();
        }

        public List<Student> Population
        {
            get => Groups.SelectMany(g => g.Members).ToList();
        }
        public int kpopp
        {
            get { return k; }
            set { k = value; }
        }
        public int kpop => GroupCount * GroupSize;
        public void AssignRoles()
        {
            foreach (var group in Groups)
            {
                group.Members = group.Members.OrderByDescending(s => s.f).ToList();
                group.Members[0].Role = Role.Leader;
                group.Members[1].Role = Role.Advocate;
                for (int i = 2; i < group.Members.Count; i++)
                {
                    group.Members[i].Role = Role.Believer;
                }
            }

            GlobalBestLeader = Groups.Select(g => g.Leader).OrderByDescending(s => s.f).First();
        }

        public void UpdateSearchDirections()
        {
            foreach (var group in Groups)
            {
                var leader = group.Members.FirstOrDefault(s => s.Role == Role.Leader);
                var advocate = group.Members.FirstOrDefault(s => s.Role == Role.Advocate);
                var believers = group.Members.Where(s => s.Role == Role.Believer).ToList();

                foreach (var student in group.Members)
                {
                    if (student == GlobalBestLeader)
                        continue;
                    double[] newX = new double[N];

                    if (student.Role == Role.Leader)
                    {
                        double w1 = R.NextDouble() * (1.0 - 0.34) + 0.34;

                        // 2) определить допустимый диапазон для w2
                        double minW2 = (1.0 - w1) / 2.0;
                        double maxW2 = Math.Min(w1, 1.0 - w1);

                        // 3) взять w2 равномерно из этого диапазона
                        double w2 = minW2 + R.NextDouble() * (maxW2 - minW2);

                        // 4) завершить третьим весом
                        double w3 = 1.0 - w1 - w2;

                        for (int j = 0; j < N; j++)
                        {
                            double believersAvg = believers.Count > 0 ? believers.Select(b => b.x[j]).Average() : 0;
                            newX[j] = -student.x[j]*Weight +w1 * GlobalBestLeader.x[j] + w2 * advocate.x[j] + w3 * believersAvg;
                        }
                    }
                    else if (student.Role == Role.Advocate)
                    {
                        double w1 = R.NextDouble() * (1.0 - 0.51) + 0.51;  // [0.5,1.0)
                        double w2 = 1.0 - w1;

                        for (int j = 0; j < N; j++)
                        {
                            double believersAvg = believers.Count > 0 ? believers.Select(b => b.x[j]).Average() : 0;
                            newX[j] = -student.x[j] * Weight + w1 * leader.x[j] + w2 * believersAvg;
                        }
                    }
                    else if (student.Role == Role.Believer)
                    {
                        double w1 = R.NextDouble() * (1.0 - 0.51) + 0.51;  // [0.5,1.0)
                        double w2 = 1.0 - w1;

                        for (int j = 0; j < N; j++)
                        {
                            newX[j] = -student.x[j] * Weight + w1 * leader.x[j] + w2 * advocate.x[j];
                        }
                    }

                    for (int j = 0; j < N; j++)
                    {
                        newX[j] = Math.Max(oblast[j, 0], Math.Min(oblast[j, 1], newX[j]));
                    }

                    Array.Copy(newX, student.x, N);
                    student.f = Evaluate(student.x);

                    if (student.f > maxDf)
                    {
                        maxDf = student.f;
                        bestX = student.x[0];
                        bestY = student.x[1];
                    }
                }
            }
        }

        public void Work()
        {
            maxDf = double.MinValue;

            AssignRoles();

            for (m = 0; m < K; m++)
            {
                k = m;
                UpdateSearchDirections();
                AssignRoles();
                Analyze();
            }
        }

        public void Analyze(bool final = false)
        {
            RecalcMaxf();
            if (!final) return;
            Mfend = Math.Round(Mf, 6);
        }

        public void RecalcMaxf()
        {
            double S = 0.0;
            foreach (var group in Groups)
            {
                foreach (var member in group.Members)
                {
                    if (member.f > maxDf)
                    {
                        maxDf = member.f;
                        bestX = member.x[0];
                        bestY = member.x[1];
                    }
                    S += member.f;
                }
            }
            BestF.Add(maxDf);
            Mf = S / (GroupCount * GroupSize);
            AverF.Add(Mf);
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

        public void TeacherPhase() { }
        public void LearnerPhase() { }
        public void ReproduceAndReplace() { }
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
