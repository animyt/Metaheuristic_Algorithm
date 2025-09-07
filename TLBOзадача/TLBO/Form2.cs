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
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();

        }

        public List<double> averageF;
        public List<double> bestF;
        public List<Algoritm.Student> Population;


        public void FillList()
        {
            listView1.Items.Clear(); // Очищаем список перед заполнением
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

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            float w = pictureBox1.Width;
            float h = pictureBox1.Height;
            Pen p1 = new Pen(Color.Black, 1);
            Pen p2 = new Pen(Color.Green, 1);
            Pen p3 = new Pen(Color.Blue, 1);
            Font f1 = new Font("TimesNewRoman", 8);
            Font f2 = new Font("TimesNewRoman", 8, FontStyle.Bold);
            float x0 = 30;
            float y0 = h - 30;

            // Отрисовка осей
            e.Graphics.DrawLine(p1, x0, y0, w, y0);
            e.Graphics.DrawLine(p1, x0, y0, x0, 0);
            e.Graphics.DrawLine(p1, x0, 0, x0 - 5, 10);
            e.Graphics.DrawLine(p1, x0, 0, x0 + 5, 10);
            e.Graphics.DrawLine(p1, w - 5, y0, w - 15, y0 + 5);
            e.Graphics.DrawLine(p1, w - 5, y0, w - 15, y0 - 5);

            // Определение минимального и максимального значений
            double minY = 0;
            double maxY = 0;
            if (averageF != null && averageF.Count > 0 && bestF != null && bestF.Count > 0)
            {
                minY = Math.Min(averageF.Min(), bestF.Min());
                maxY = Math.Max(averageF.Max(), bestF.Max()) * 1.15; // Увеличиваем maxY на 10%
            }
            else
            {
                // Если averageF или bestF пусты, устанавливаем значения по умолчанию
                minY = -10;
                maxY = 10;
            }

            float mh = 0;

            // Обработка особого случая (все значения одинаковы)
            if (maxY == minY)
            {
                mh = 1; // Предотвращаем деление на ноль
                        // Отображаем одно значение на оси Y (примерно посередине)
                float yPos = y0 - h / 2;
                e.Graphics.DrawLine(p1, x0 - 2, yPos, x0 + 2, yPos);
                e.Graphics.DrawString(minY.ToString("F2"), f1, Brushes.Black, x0 - 29, yPos - 8);
            }
            else
            {
                mh = (float)((h - 60) / (maxY - minY)); // Масштаб по высоте

                // Отображение значений на оси Y
                for (int i = -6; i < 12; i++)
                {
                    double value = minY + (maxY - minY) * (i + 6) / 18.0; // Равномерное распределение значений
                    float yPos = y0 - (float)(mh * (value - minY));
                    e.Graphics.DrawLine(p1, x0 - 2, yPos, x0 + 2, yPos);
                    e.Graphics.DrawString(value.ToString("F2"), f1, Brushes.Black, x0 - 29, yPos - 8);
                }
            }

            float mx = (w - 60) / (Math.Max(averageF.Count, bestF.Count)); // Масштаб по ширине

            // Количество значений, отображаемых на оси X
            int numLabels = 15;

            // Шаг между значениями
            double step = (double)Math.Max(averageF.Count, bestF.Count) / numLabels;

            // Отрисовка значений на оси X
            for (int j = 0; j <= numLabels; j++)
            {
                int i = (int)(step * j); // Номер итерации для отображения

                if (i < Math.Max(averageF.Count, bestF.Count)) // Проверка, что номер итерации не выходит за границы данных
                {
                    e.Graphics.DrawLine(p1, (float)(x0 + (mx) * (i)), y0 + 2, (float)(x0 + mx * (i)), y0 - 2);
                    e.Graphics.DrawString(Convert.ToString(i), f1, Brushes.Black, (float)(x0 + mx * (i)), (float)(y0 + 4));
                }
            }


            // Отрисовка графиков
            for (int i = 0; i < Math.Max(averageF.Count, bestF.Count) - 1; i++)
            {
                if (i < averageF.Count - 1)
                {
                    float y1 = y0 - (float)(mh * (averageF[i] - minY));
                    float y2 = y0 - (float)(mh * (averageF[i + 1] - minY));
                    e.Graphics.DrawLine(p2, (float)(x0 + mx * i), y1, (float)(x0 + mx * (i + 1)), y2);
                }

                if (i < bestF.Count - 1)
                {
                    float y1 = y0 - (float)(mh * (bestF[i] - minY));
                    float y2 = y0 - (float)(mh * (bestF[i + 1] - minY));
                    e.Graphics.DrawLine(p3, (float)(x0 + mx * i), y1, (float)(x0 + mx * (i + 1)), y2);
                }


            }

            // Надписи осей
            e.Graphics.DrawString("k", f2, Brushes.Black, w - 15, y0 + 4);
            e.Graphics.DrawString("f", f2, Brushes.Black, x0 - 29, 2);
        }



    }
}