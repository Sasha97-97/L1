﻿using System;
using System.Globalization;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace L1
{
    public partial class Form1 : Form
    {
        class Point
        {
            public double x, y, z, h; // координаты точки

            // Метод парсинга
            public static Point Parse(string str)
            { // Point-тип возвращаемого значения
                Point point = new Point(); // создаём объект класса "Точка"
                string[] s = str.Split(' '); // создаём массив строк, в каждой строке одна координата точки

                // каждое поле точки содежит координату типа double
                point.x = double.Parse(s[0]);
                point.y = double.Parse(s[1]);
                point.z = double.Parse(s[2]);
                point.h = double.Parse(s[3]);

                return point; // возвращаем объект класса "Точка"
            }
        }


        class Join
        {
            public int one, two; // координаты для начала и конца отрезков (соединений)

            public static Join Parse(string str)
            {
                Join join = new Join(); // создаём объект класса соединений
                string[] s = str.Split(' '); //создаём массив строк, в каждой строке одна координата соединения

                // каждое поле соединения содержит начало или конец соединения типа double
                join.one = int.Parse(s[0]);
                join.two = int.Parse(s[1]);

                return join; //возвращаем объект класса "Соединение"
            }

        }


        List<Point> points=new List<Point>(); // создаём список точек
        List<Join> joines =new List<Join>(); // создаём список соединений

        // чтение данных из файла
        void filereader() {
            string path = "..\\..\\myscene.txt";
            //string path = "C:\\Users\\Саша\\Documents\\Visual Studio 2010\\Projects\\L1\\L1\\myscene.txt"; // кладем путь файла в переменную path
            var data = File.ReadAllLines(path); // в data попадает массив строк

            points.Clear();
            points.Clear();

            // читаем точки 
            foreach (string str in data) {
                if (str == "*") break;
                points.Add(Point.Parse(str)); // в список точек добавляем точку
            }

            // читаем соединения
            for (int i = points.Count + 1; i < data.Length; i++) {
                var str = data[i]; // в str попадает i-я строка файла
                if (str == "**") break;
                joines.Add(Join.Parse(str)); // в список соединений добавляем соединение
            }
        }

        
        // функция вписывания сцены в picturebox
        void enter()
        {
            double maxx = points[0].x, maxy = points[0].y;
            double minx = points[0].x, miny = points[0].y;
            double km; // коэффициент масштабирования

            // находим точки с максимальными абсциссами и ординатами
            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].x > maxx) maxx = points[i].x;
                if (points[i].y > maxy) maxy = points[i].y;
                if (points[i].x < minx) minx = points[i].x;
                if (points[i].y < miny) miny = points[i].y;
            }
                // ищем коэффициент масштабирования
                if (pictureBox1.Height / (maxy - miny) < pictureBox1.Width / (maxx - minx))
                    km = pictureBox1.Height / (maxy - miny);
                else km = pictureBox1.Width / (maxx - minx);

                // смещаем, масштабируем
                for (int j = 0; j < points.Count; j++)
                {
                    points[j].x = points[j].x - minx;
                    points[j].y = points[j].y - miny;

                    points[j].x = points[j].x * km;
                    points[j].y = points[j].y * km;
                    points[j].z = points[j].z * km;
                }
            
        }


        void draw()
        {
            // от однородных к декартовым
            for (int i = 0; i < points.Count; i++)
            {
                points[i].x = points[i].x / points[i].h;
                points[i].y = points[i].y / points[i].h;
                points[i].z = points[i].z / points[i].h;
                points[i].h = 1;
            }
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Graphics g = Graphics.FromImage(pictureBox1.Image);
            g.Clear(Color.White);
            Pen pen = new Pen(Color.Black, 2); // создаём объект класса Pen
            for (int i = 0; i < joines.Count; i++)
            {
                g.DrawLine(pen, (float)points[joines[i].one - 1].x, (float)points[joines[i].one - 1].y,
                    (float)points[joines[i].two - 1].x, (float)points[joines[i].two - 1].y);
            }
        }
        // Параллельный перенос
        void transfer(double a, double b, double c) {
            for (int i = 0; i < points.Count; i++) {
                points[i].x = a + points[i].x * points[i].h;
                points[i].y = b + points[i].y * points[i].h;
                points[i].z = c + points[i].z * points[i].h;
            }
        }

        // Масштабирование по X
        void scaleX(double kx) {
            for (int i = 0; i < points.Count; i++) {
                points[i].x = points[i].x * kx;
            }
        }

        // Масштабирование по Y
        void scaleY(double ky) {
            for (int i = 0; i < points.Count; i++) {
                points[i].y = points[i].y * ky;
            }
        }

        // Масштабирование по Z
        void scaleZ(double kz) {
            for (int i = 0; i < points.Count; i++)
            {
                points[i].z = points[i].z * kz;
            }
        }

        // Поворот вокруг оси X
        void rotateX(double a) {
            for (int i=0; i < points.Count; i++) {
                double al = points[i].y * Math.Cos(a*Math.PI/180) - points[i].z * Math.Sin(a*Math.PI/180);
                double be = points[i].y * Math.Sin(a*Math.PI/180) + points[i].z * Math.Cos(a*Math.PI/180);

                points[i].y = al;
                points[i].z = be;
            }
        }

        // Поворот вокруг Y
        void rotateY(double a) {
            for (int i = 0; i < points.Count; i++) {
                double al = points[i].x * Math.Cos(a*Math.PI/180) + points[i].z * Math.Sin(a*Math.PI/180);
                double be = points[i].y + points[i].z;
                double ga = -points[i].x * Math.Sin(a*Math.PI/180) + points[i].z * Math.Cos(a*Math.PI/180);

                points[i].x = al;
                points[i].y = be;
                points[i].z = ga;
            }
        }

        // Поворот вокруг Z
        void rotateZ(double a) {
            for (int i = 0; i < points.Count; i++) {
                double al = points[i].x * Math.Cos(a*Math.PI/180) - points[i].y * Math.Sin(a*Math.PI/180);
                double be = points[i].x * Math.Sin(a*Math.PI/180) + points[i].y * Math.Cos(a*Math.PI/180);

                points[i].x = al;
                points[i].y = be;
            }
        }

        // Косой сдвиг оси X по оси Y
        void shiftXY(double k) {
            for (int i = 0; i < points.Count; i++) {
                points[i].x = points[i].x + points[i].y * k;
            }
        }

        // Косой сдвиг оси X по оси Z
        void shiftXZ(double k) {
            for (int i = 0; i < points.Count; i++) {
                points[i].x = points[i].x + points[i].z * k;
            }
        }

        // Косой сдвиг оси Y по оси X
        void shiftYX(double k) {
            for (int i = 0; i < points.Count; i++) {
                points[i].y = points[i].x * k + points[i].y;
            }
        }

        // Косой сдвиг оси Y по оси Z
        void shiftYZ(double k) {
            for (int i = 0; i < points.Count; i++) {
                points[i].y = points[i].y + points[i].z * k;
            }
        }

        // Косой сдвиг оси Z по оси X
        void shiftZX(double k)
        {
            for (int i = 0; i < points.Count; i++) {
                points[i].z = points[i].z + points[i].x * k;
            }
        }

        // Косой сдвиг оси Z по оси Y
        void shiftZY(double k) {
            for (int i = 0; i < points.Count; i++) {
                points[i].z = points[i].z + points[i].y * k;
            }
        }

        // OПП по оси X с фокусным расстоянием fx
        void OPPfx(double fx) {
            for (int i = 0; i < points.Count; i++) {
                points[i].h = points[i].x / fx + points[i].h;
            }
        }

        // OПП по оси Y с фокусным расстоянием fy
        void OPPfy(double fy) {
            for (int i = 0; i < points.Count; i++) {
                points[i].h = points[i].y / fy + points[i].h;
            }
        }

        // OПП по оси Z с фокусным расстоянием fz
        void OPPfz(double fz) {
            for (int i = 0; i < points.Count; i++) {
                points[i].h = points[i].z / fz + points[i].h;
            }
        }

        public Form1()
        {
            InitializeComponent();
            BackColor = Color.Orange; 
        }

        // КНОПКИ
        // Загрузка сцены
        private void button1_Click(object sender, EventArgs e)
        {
            filereader();
            enter();
            draw();
        }

        // Вписать
        private void button18_Click(object sender, EventArgs e)
        {
            enter();
        }

        // Параллельно переносим
        private void button2_Click(object sender, EventArgs e)
        {   
            double a, b, c;
            if (Double.TryParse(textBox1.Text, out a) && Double.TryParse(textBox1.Text, out b) && Double.TryParse(textBox1.Text, out c))
            {
                transfer(a, b, c);
                draw();
            }
            else MessageBox.Show("Необходимо ввести числа!");
        }

        // Масштабируем по X
        private void button3_Click(object sender, EventArgs e)
        {
            double kx;
            if (Double.TryParse(textBox4.Text, out kx))
            {
                scaleX(kx);
                draw();
            }
            else MessageBox.Show("Необходимо ввести число!");
        }

        // Масштабируем по Y
        private void button4_Click(object sender, EventArgs e)
        {
            double ky;
            if (Double.TryParse(textBox5.Text, out ky))
            {
            scaleY(ky);
            draw();
            }
            else MessageBox.Show("Необходимо ввести число!");
        }

        // Масштабируем по Z
        private void button5_Click(object sender, EventArgs e)
        {
            double kz;
            if (Double.TryParse(textBox5.Text, out kz))
            {
                scaleZ(kz);
                draw();
            }
            else MessageBox.Show("Необходимо ввести число!");
        }

        // Поворачиваем вокруг оси X
        private void button6_Click(object sender, EventArgs e)
        {
            double a;
            if (Double.TryParse(textBox7.Text, out a))
            {
                rotateX(a);
                draw();
            }
            else MessageBox.Show("Необходимо ввести число!");
        }
        // Поворачиваем вокруг осе Y
        private void button7_Click(object sender, EventArgs e)
        {
            double a;
            if (Double.TryParse(textBox7.Text, out a))
            {
                rotateY(a);
                draw();
            }
            else MessageBox.Show("Необходимо ввести число!");
        }

        // Поворачиваем вокруг осе Z
        private void button8_Click(object sender, EventArgs e)
        {
            double a;
            if (Double.TryParse(textBox7.Text, out a))
            {
                rotateZ(a);
                draw();
            }
            else MessageBox.Show("Необходимо ввести число!");
        }

        // Сдвигаем X по Y
        private void button9_Click(object sender, EventArgs e)
        {
            double k;
            if (Double.TryParse(textBox7.Text, out k))
            {
                shiftXY(k);
                draw();
            }
            else MessageBox.Show("Необходимо ввести число!");
        }

        // Сдвигаем X по Z
        private void button10_Click(object sender, EventArgs e)
        {
            double k;
            if (Double.TryParse(textBox8.Text, out k))
            {
                shiftXZ(k);
                draw();
            }
            else MessageBox.Show("Необходимо ввести число!");
        }

        // Сдвигаем Y по X
        private void button11_Click(object sender, EventArgs e)
        {
            double k;
            if (Double.TryParse(textBox8.Text, out k))
            {
                shiftYX(k);
                draw();
            }
            else MessageBox.Show("Необходимо ввести число!");
        }

        // Сдвигаем Y по Z
        private void button12_Click(object sender, EventArgs e)
        {
            double k;
            if (Double.TryParse(textBox8.Text, out k))
            {
            shiftYZ(k);
            draw();
            }
            else MessageBox.Show("Необходимо ввести число!");
        }

        // Сдвигаем Z по X
        private void button13_Click(object sender, EventArgs e)
        {
            double k;
            if (Double.TryParse(textBox8.Text, out k))
            {
                shiftZX(k);
                draw();
            }
            else MessageBox.Show("Необходимо ввести число!");
        }

        // Сдвигаем Z по X
        private void button14_Click(object sender, EventArgs e)
        {
            double k;
            if (Double.TryParse(textBox8.Text, out k))
            {
                shiftZY(k);
                draw();
            }
            else MessageBox.Show("Необходимо ввести число!");
        }

        // ОПП с fx
        private void button15_Click(object sender, EventArgs e)
        {
            double fx;
            if (Double.TryParse(textBox9.Text, out fx))
            {
                OPPfx(fx);
                draw();
            }
            else MessageBox.Show("Необходимо ввести число!");
        }

        // ОПП с fy
        private void button16_Click(object sender, EventArgs e)
        {
            double fy;

            if (Double.TryParse(textBox10.Text, out fy))
            {
            OPPfx(fy);
            draw();
            }
            else MessageBox.Show("Необходимо ввести число!");
        }

        // ОПП с fz
        private void button17_Click(object sender, EventArgs e)
        {
            double fz;
            if (Double.TryParse(textBox11.Text, out fz))
            {
                OPPfx(fz);
                draw();
            }
            else MessageBox.Show("Необходимо ввести число!");
        }

    }
}
