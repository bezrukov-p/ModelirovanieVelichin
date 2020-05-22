using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LabTerver
{
    public partial class Form1 : Form
    {
        double p;// вероятность
        int number;// кол-во экспериментов
        //List<int> _eta = new List<int>(); //копии
        //List<int> _eta_2 = new List<int>();
        double R0 = 0;
        int intervals = 0;// кол-во итервалов
        int r = 0; //степеней свободы

        List<int> _eta_2;
        List<int> _eta_count;
        List<int> _eta;
        List<double> borders = new List<double>();

        public Form1()
        {
            InitializeComponent();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private double F_x(int x, double p)
        {
            double F = 0;
            for (int i = 0; i < (x - 1); i++)
                F += Math.Pow((1 - p), i) * p;
            return F;
        }

        private double F_x(double x, double p)
        {
            double F = 0;
            for (int i = 0; i < (x - 1); i++)
                F += Math.Pow((1 - p), i) * p;
            return F;
        }

        private double F_x_krishka(int x, List<int> eta_2,List<int> eta_count, List<int> eta)
        {
            double F;

            int count = 0;
            for(int i = 0; i < eta_2.Count();i++)
            {
                if (eta_2[i] >= x)
                    break;
                count += eta_count[i];
            }

            F = (double)count / (double)eta.Count();
            return F;
        }

        private int F_x_krishka_count(double x)
        {
            int count = 0;
            for (int i = 0; i < _eta_2.Count(); i++)
            {
                if (_eta_2[i] < x)
                    count += _eta_count[i];
                else
                    break;
            }
            return count;
        }

        private double D_x_module(double F_x,double F_x_krishka)
        {
            return Math.Abs(F_x - F_x_krishka);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // ДЛЯ ЛР1______________________________________________________________________
            p = Convert.ToDouble(textBox1.Text);
            number = Convert.ToInt32(textBox2.Text);

            List<int> eta = new List<int>();
            List<int> eta_tmp = new List<int>();
            for (int i = 0; i < number; i++)
                eta.Add(0);


            Random rand = new Random();
            for(int i = 0; i < number; i++)
            {
                do
                {
                    eta[i]++;
                }
                while (rand.NextDouble() > p);
            }
            eta.Sort();

            
            for (int i = 0; i < eta.Count(); i++)
                eta_tmp.Add(eta[i]);

            List<int> eta_2 = new List<int>();
            // в eta_2 не будет повторяющихся
            while (eta_tmp.Count != 0)
            {
                int tmp = eta_tmp[0];
                eta_2.Add(tmp);
                while (eta_tmp.Count() != 0 && eta_tmp[0] == tmp)
                {
                    eta_tmp.RemoveAt(0);
                }
            }

            //количество выпадений
            List<int> eta_count = new List<int>();
            for (int i = 0; i < eta_2.Count(); i++)
                eta_count.Add(0);
            for (int i = 0; i < eta_2.Count(); i++)
                for (int j = 0; j < eta.Count; j++)
                    if (eta_2[i] == eta[j])
                        eta_count[i]++;

            // частота выпадений
            List<double> eta_frequency = new List<double>();
            for(int i = 0; i < eta_2.Count;i++)
            {
                eta_frequency.Add((double)eta_count[i] / (double)number);
            }

            //вероятность
            List<double> eta_probability = new List<double>();
            for (int i = 0; i < eta_2.Count(); i++)
                eta_probability.Add((Math.Pow((1 - p), eta_2[i] - 1)) * p);


            //в таблицу
            dataGridView1.RowCount = eta_2.Count();
            for(int i = 0; i < dataGridView1.RowCount;i++)
            {
                dataGridView1.Rows[i].Cells[0].Value = eta_2[i];
                dataGridView1.Rows[i].Cells[1].Value = eta_count[i];
                dataGridView1.Rows[i].Cells[2].Value = eta_frequency[i];
                dataGridView1.Rows[i].Cells[3].Value = eta_probability[i];
            }


            //ДЛЯ ЛР2___________________________________________________________________________________
            double x_krishka = 0; // выборочное среднее
            double S_kvadrat = 0; // выборочная дисперсия
            double R_krishka = 0; // размах выборки
            double Me_krishka = 0; // выборочная медиана
            double E = 1 / p; //мат ожидание
            double D_eta = 0; // дисперсия

            int sum_xi = 0;
            for (int i = 0; i < eta.Count(); i++)
                sum_xi += eta[i];
            x_krishka = (double)sum_xi / (double)number;

            //for (int i = 0; i < eta.Count(); i++)
            //    S_kvadrat += Math.Pow(eta[i] - x_krishka, 2);///////////////////////////////////////////////////
            //S_kvadrat /= (double)number;

            for (int i = 0; i < eta.Count(); i++)
                S_kvadrat += (eta[i] - x_krishka)*(eta[i] - x_krishka);
            S_kvadrat /= (double)number;

            //for (int i = 0; i < eta_2.Count(); i++)
            //    S_kvadrat += Math.Pow(eta_2[i] - x_krishka, 2) * eta_count[i];
            //S_kvadrat /= (double)number;

            //R_krishka = Math.Abs(eta[0] - eta[eta.Count() - 1]);

            if (number % 2 == 0)
                Me_krishka = (double)(eta[eta.Count() / 2] + eta[eta.Count() / 2 - 1]) / 2;
            else
                Me_krishka = (double)(eta[(eta.Count() - 1) / 2]);


            // ТУТ ВЫЧИСЛЕНИЕ ДИСПЕРСИИ
            for(int i = 1; i < 10000; i++)
            {
                D_eta += Math.Pow((i - E), 2) * (Math.Pow((1 - p), i - 1) * p);
                //D_eta += Math.Pow((1 - p), i - 1) * p * Math.Pow(i - E, 2);
            }

            //в таблицу
            dataGridView2.Rows[0].Cells[0].Value = E;
            dataGridView2.Rows[0].Cells[1].Value = x_krishka;
            dataGridView2.Rows[0].Cells[2].Value = Math.Abs(E - x_krishka);
            dataGridView2.Rows[0].Cells[3].Value = D_eta;
            dataGridView2.Rows[0].Cells[4].Value = S_kvadrat;
            dataGridView2.Rows[0].Cells[5].Value = Math.Abs(D_eta - S_kvadrat);
            dataGridView2.Rows[0].Cells[6].Value = Me_krishka;
            dataGridView2.Rows[0].Cells[7].Value = R_krishka;

            dataGridView3.RowCount = eta_2.Count();
            for (int i = 0; i < dataGridView3.RowCount; i++)//
            {
                dataGridView3.Rows[i].Cells[0].Value = eta_2[i];
                dataGridView3.Rows[i].Cells[1].Value = eta_probability[i];
                dataGridView3.Rows[i].Cells[2].Value = eta_frequency[i];
            }

            //Макс. отклонение
            double max = 0;
            for(int i = 0; i < eta_2.Count(); i++)
            {
                double tmp;
                tmp = Math.Abs(eta_frequency[i] - eta_probability[i]);
                if (tmp > max)
                    max = tmp;
            }
            textBox3.Text = Convert.ToString(max);


            //D
            //double D = 0;
            //for (int i = 0; i < eta_2.Count(); i++)
            //{
            //    double tmp = Math.Max(D, Math.Abs(F_x(eta_2[i], p) - F_x_krishka(eta_2[i], eta_2, eta_count, eta)));
            //    if (D < tmp)
            //        D = tmp;
            //}
            //textBox4.Text = Convert.ToString(D);

            double D = 0;
            for (int i = 0; i < 50; i++)
            {
                double tmp = Math.Max(D, Math.Abs(F_x(i, p) - F_x_krishka(i, eta_2, eta_count, eta)));
                if (D < tmp)
                    D = tmp;
            }
            textBox4.Text = Convert.ToString(D);

            // Графики
            chart1.Series[0].Points.Clear();
            chart1.Series[1].Points.Clear();

            chart1.Series[1].Points.AddXY(0, 0);
            chart1.Series[1].Points.AddXY(eta_2[0], 0);
            for (int i = 0; i < eta_2.Count() - 1; i++)
            {  

                double y2;
                y2 = F_x_krishka(eta_2[i + 1], eta_2, eta_count, eta);
                chart1.Series[1].Points.AddXY(eta_2[i], y2);
                chart1.Series[1].Points.AddXY(eta_2[i + 1], y2);
            }
            chart1.Series[1].Points.AddXY(eta_2[eta_2.Count() - 1], 1);
            chart1.Series[1].Points.AddXY(eta_2[eta_2.Count() - 1] + 5, 1);

            chart1.Series[0].Points.AddXY(0, 0);
            chart1.Series[0].Points.AddXY(1, 0);
            for (int i = 1; i < 40; i++)
            {
                double y1;
                y1 = F_x(i + 1, p);
                chart1.Series[0].Points.AddXY(i, y1);
                chart1.Series[0].Points.AddXY(i + 1, y1);
            }

            //запомнить данные в классе, чтобы использовать в других методах
            _eta_2 = eta_2;
            _eta_count = eta_count;
            _eta = eta;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            borders.Clear();
            string borders_str = textBox5.Text;

            // borders - границы интервалов
            for(int i = 0; i < borders_str.Count(); i++)
            {
                string tmp_str = "";
                double tmp_double = 0;
                while (i != borders_str.Count() && borders_str[i] != ',')
                {
                    tmp_str += borders_str[i];
                    i++;
                }
                tmp_str = tmp_str.Replace('.', ',');
                tmp_double = Convert.ToDouble(tmp_str);
                borders.Add(tmp_double);

            }
            intervals = borders.Count() + 1;
            List<double> q_j = new List<double>();

            //вычисление q_j в интервалах
            double sum = 0;
            q_j.Add(F_x(borders[0], p));
            sum += q_j[0];
            for (int i = 1; i < intervals - 1; i++)
            {
                q_j.Add(F_x(borders[i], p) - F_x(borders[i - 1], p));
                sum += q_j[i];
            }
            q_j.Add(1 - sum);



            //заполнение таблицы
            dataGridView4.RowCount = intervals;
            for (int i = 0; i < intervals; i++)
            {
                dataGridView4.Rows[i].Cells[0].Value = i + 1;
                dataGridView4.Rows[i].Cells[1].Value = q_j[i];
            }

            
            //n_j 
            List<double> n_j = new List<double>();
            n_j.Add(F_x_krishka_count(borders[0]));
            for (int i = 1; i < intervals - 1; i++)
                n_j.Add(F_x_krishka_count(borders[i]) - F_x_krishka_count(borders[i - 1]));
            //вычисление последнего интервала
            int tmpsum = 0;
            for (int i = 0; i < _eta_2.Count(); i++)
            {
                if (_eta_2[i] >= borders[borders.Count() - 1])
                    tmpsum += _eta_count[i];
            }
            n_j.Add(tmpsum);

            //R0
            R0 = 0;
            for(int i = 0; i < intervals; i++)
            {
                R0 += (Math.Pow(n_j[i] - number * q_j[i], 2)) / (number * q_j[i]);
            }
            textBox7.Text = Convert.ToString(R0);

        }

        private double Gamma(double x)
        {
            if (x == 1)
                return 1;
            if (x == 1.0 / 2.0)
                return Math.Sqrt(Math.PI);
            double res = Gamma(x - 1) * (x - 1);
            return res;
        }

        private double g_x(double x)
        {
            double result = 0;
            r = intervals - 1;

            double gamma = 0;
            gamma = Gamma((double)r / 2.0);

            result = Math.Pow(2, -(double)r/2.0) * Math.Pow(gamma, -1) * Math.Pow(x, (double)r / 2.0 - 1) * Math.Exp(-x / 2.0);

            return result;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            double F_R0 = 0;
            double alpha = Convert.ToDouble(textBox6.Text);
            int n = 10000;

            for ( int i = 1; i < n; i++)
            {
                F_R0 += (g_x(R0 * (double)(i - 1) / (double)n) + g_x(R0 * (double)i / (double)n)) * R0 / ((double)2 * (double)n);
            }

            F_R0 = 1 - F_R0;
            textBox8.Text = Convert.ToString(F_R0);


            if (F_R0 < alpha)
                textBox9.Text = "Отвергнуть";
            else
                textBox9.Text = "Принять";
        }
    }
}
