using System;
using System.Collections.Generic;
using System.Text;

namespace TLBO
{
    class Report
    {

        public string toprotocol = "";

        public void Prot1(int f, int Ip, int K, List<Algoritm.Student> pop, double[,] obl, int numb)
        {
            string pr = "";
            string fname = "";
            if (f == 0) fname = "функция Швефеля";
            else if (f == 1) fname = "Мульти-функция";
            else if (f == 2) fname = "функция Рута";
            else if (f == 3) fname = "функция Шафера";
            else if (f == 4) fname = "функция Растригина";
            else if (f == 5) fname = "функция Аклея";
            else if (f == 6) fname = "функция Skin";
            else if (f == 7) fname = "функция Trapfall";
            else if (f == 8) fname = "функция Розенброка";
            else if (f == 9) fname = "функция Параболическая";

            pr += "\r\n" + "\r\n" + "\r\n" + "\r\n" + "****************************************************************************" + "\r\n"
                  + "**                                                                        **" + "\r\n"
                  + "**                            Работа № "+ numb.ToString()+"                                  **" + "\r\n"
                  + "**                                                                        **" + "\r\n"
                  + "****************************************************************************" + "\r\n"+"\r\n";
            pr += "\r\n" +"П А Р А М Е Т Р Ы    А Л Г О Р И Т М А" + "\r\n" + "\r\n";
            pr += "Целевая функция: " + fname + "\r\n" + "Множество допустимых решений:  " + obl[0, 0].ToString() + " < x1 < " + obl[0, 1].ToString();
            pr += ",   " + obl[1, 0].ToString() + " < x2 < " + obl[1, 1].ToString();
           
            pr += "\r\n" + "Размер начальной популяции: "+ Ip.ToString();

            pr += "\r\n" + "Максимальное количество популяций: " + K.ToString();
      
         
            
           
            pr += "\r\n" + "\r\n" + "\r\n";
            pr += "НАЧАЛЬНАЯ ПОПУЛЯЦИЯ" + "\r\n" + "\r\n";
            
            {
                pr += "|-----------------|-----------------|-----------------|----------------------------|\r\n";
                pr += "|   Номер особи   |        x1       |        x2       |      Приспособленность     |\r\n";
                pr += "|-----------------|-----------------|-----------------|----------------------------|\r\n";
                for (int i = 0; i < Ip; i++)
                {
                    string g = Convert.ToString(i + 1);
                    pr += "|  ";
                    if (g.Length == 1) pr += "  ";
                    else if (g.Length == 2) pr += " ";
                    pr += Convert.ToString(i + 1) + "            |  ";
                    for (int j = 0; j < 2; j++)
                    {
                        string prob = "";
                        string prob1 = "";
                        string prob2 = "";
                        if (pop[i].x[j] > 0) prob1 = " ";
                        else prob2 = " ";
                        if (j < 2)
                        {
                            int length = 14 - Math.Round(pop[i].x[j], 6).ToString().Length;
                            for (int l = 0; l < length; l++) prob += " ";
                        }
                       
                        pr += prob1+ Math.Round(pop[i].x[j],6)  +prob2+ prob + "|  ";

                    }

                    string prob0 = "";
                    string prob10 = "";
                    string prob20 = "";
                    if (pop[i].f > 0) prob10 = " ";
                    else prob20 = " ";
                   
                        int length2 = 25 - Math.Round(pop[i].f, 6).ToString().Length;
                        for (int l = 0; l < length2; l++) prob0 += " ";
                    
                    pr += prob10 + Math.Round(pop[i].f, 6) + prob20 + prob0 + "|  ";

                    pr += "\r\n";
                }
                pr += "|-----------------|-----------------|-----------------|----------------------------|\r\n";
            }
           
            toprotocol = pr;
        }

        public void Prot2(List<Algoritm.Student> pop, int K, double L, double exact)
        {
            string pr = "";
            pr += "\r\n" + "\r\n";
            pr += "КОНЕЧНАЯ ПОПУЛЯЦИЯ" + "\r\n" + "\r\n";

                pr += "|-----------------|-----------------|-----------------|----------------------------|\r\n";
                pr += "|   Номер особи   |        x1       |        x2       |      Приспособленность     |\r\n";
                pr += "|-----------------|-----------------|-----------------|----------------------------|\r\n";
                for (int i = 0; i < pop.Count; i++)
                {

                    string g = Convert.ToString(i + 1);
                    pr += "|  ";
                    if (g.Length == 1) pr += "  ";
                    else if (g.Length == 2) pr += " ";
                    pr += Convert.ToString(i + 1) + "            |  ";
                    for (int j = 0; j < 2; j++)
                    {
                        string prob = "";
                        string prob1 = "";
                        string prob2 = "";
                        if (pop[i].x[j] > 0) prob1 = " ";
                        else prob2 = " ";
                        if (j < 2)
                        {
                            int length = 14 - Math.Round(pop[i].x[j], 6).ToString().Length;
                            for (int l = 0; l < length; l++) prob += " ";
                        }
                        
                        pr += prob1 + Math.Round(pop[i].x[j], 6) + prob2 + prob + "|  ";
                    }

                    string prob0 = "";
                    string prob10 = "";
                    string prob20 = "";
                    if (pop[i].f > 0) prob10 = " ";
                    else prob20 = " ";
                    
                    {
                        int length2 = 25 - Math.Round(pop[i].f, 6).ToString().Length;
                        for (int l = 0; l < length2; l++) prob0 += " ";
                    }
                    pr += prob10 + Math.Round(pop[i].f, 6) + prob20 + prob0 + "|  ";

                    pr += "\r\n";
                    
                }
                pr += "|-----------------|-----------------|-----------------|----------------------------|\r\n";
            
            pr += "\r\n";
            pr += "Количество сформированных популяций: " + K.ToString() + "\r\n";
            //pr += "Количество локальных поисков: " + L.ToString() + "\r\n";
            pr += "Размер конечной популяции: " + pop.Count.ToString() +"\r\n";
            

            //!!!сортировка
            int i0 = 0;
            double f0 = 0;
            if (pop.Count > 0)
            {

                f0 = pop[0].f;
                for (int i = 0; i < pop.Count; i++)
                {
                    if (pop[i].f > f0)
                    {
                        f0 = pop[i].f;
                        i0 = i;
                    }

                }
            }
            //!!!
            pr += "Наилучшая приспособленность: " + Math.Round(pop[i0].f,8).ToString() + "\r\n";
            pr += "Точка максимума: x1 = " + Math.Round(pop[i0].x[0],8).ToString();
            pr += ", x2 = " + Math.Round(pop[i0].x[1],8).ToString() + "\r\n";
            pr += "Точное решение: " + exact.ToString() + "\r\n";
            pr += "Отклонение от точного решения: " + Math.Round(Math.Abs(pop[i0].f-exact), 8).ToString();
            
            toprotocol = pr;
        }

    }
}
