using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace minesweeper
{
    public partial class msForm : Form
    {
        /*Ініціалізація глобальних змінних */

        Field[,] field;                                                                                 // class Field - покращенний class Button
        DateTime date,stopWatch;                                                                        // Ці змінні використовуються для відображення
        Timer timer;                                                                                    // поточного часу при проходженні рівня
        int fieldHeight, fieldWidth, minesQuantity, levelDifficulty, nodeCounter=0, mineCounter=0;      // Розміри поля, к-сть мін, відкритих комірок та поставлених флагів
        bool firstClick = false;                                                                        // Чи було зроблено перше натискання на комірку     
        
        /* Наступні елементи потрібні для інтерфейсу головногом меню */ 

        Button btnEasyLvl, btnLowmediumLvl, btnHighmediumLvl, btnHardLvl;   
        Label lStopwatch, lMines, easyLvlLabelStats, easyLvlLabelTime, lowmediumLvlLabelStats, lowmediumLvlLabelTime, highmediumLvlLabelStats, highmediumLvlLabelTime, hardLvlLabelStats, hardLvlLabelTime;
        PictureBox minesLogo, easyLvlCrown, easyLvlStopwatch, lowmediumLvlCrown, lowmediumLvlStopwatch, highmediumLvlCrown, highmediumLvlStopwatch, hardLvlCrown, hardLvlStopwatch;

        

        /* Наступні елементи потрібні для збереження інформації про рекорди на кожному рівні */

        long[] easyLvlInfo = new long[4], lowmediumLvlInfo = new long[4], highmediumLvlInfo = new long[4], hardLvlInfo = new long[4];
        string text;
        string[] info;

        public msForm()
        {
            InitializeComponent();
        }

        /* 
         * Далі йде опис вже загаданого class-y Field. Він наслідає властивості Button, але також має
         * Value - чим є комірка: міною чи числом 
         * Cheked - чи комірка перевірена (натиснута, відкрита)
         * Flagged - чи стоїть у комірці флаг
         * X,Y - положення комірки  
         * */
        public class Field : Button
        {
            private bool isChecked, isFlagged;
            private int x, y;
            private char VALUE;
            public bool Checked { get { return isChecked; } set { isChecked = value; } }
            public bool Flagged { get { return isFlagged; } set { isFlagged = value; } }
            public int X { get { return x; } set { x = value; } }
            public int Y { get { return y; } set { y = value; } }
            public char Value { get { return VALUE; } set { VALUE = value; } }
        }

        //Подія при запуску форми. Тут створюються елемети головного меню
        private void msForm_Load(object sender, EventArgs e)
        {
            this.ControlBox = false;
            try
            { this.BackgroundImage = Image.FromFile(@"images\bg.png"); }
            catch { this.BackColor = Color.Black; }
            
            this.Size = new Size(720, 650);


            /* Buttom for easy level */

            btnEasyLvl = new Button();
            this.Controls.Add(btnEasyLvl);
            btnEasyLvl.BackColor = Color.FromName("Maroon");
            try { btnEasyLvl.Image = Image.FromFile(@"images\btnEasyLevel.png"); }
            catch
            {
                btnEasyLvl.Text = "CAN I PLAY, DADDY?";
                btnEasyLvl.ForeColor = Color.White;
            }
            btnEasyLvl.Size = new Size(200, 165);
            btnEasyLvl.FlatAppearance.BorderSize = 0;
            btnEasyLvl.FlatStyle = FlatStyle.Popup;
            btnEasyLvl.Click += new EventHandler(btnEasyLvl_Click);
            btnEasyLvl.Left = 100;
            btnEasyLvl.Top = 70;

            /* Buttom for low-medium level */

            btnLowmediumLvl = new Button();
            this.Controls.Add(btnLowmediumLvl);
            btnLowmediumLvl.BackColor = Color.FromName("Maroon");
            try  { btnLowmediumLvl.Image = Image.FromFile(@"images\btnLowmediumLevel.png"); }
            catch
            {
                btnLowmediumLvl.Text = "DONT HURT ME!";
                btnLowmediumLvl.ForeColor = Color.White;
            }            
            btnLowmediumLvl.Size = new Size(200, 165);
            btnLowmediumLvl.FlatAppearance.BorderSize = 0;
            btnLowmediumLvl.FlatStyle = FlatStyle.Popup;
            btnLowmediumLvl.Click += new EventHandler(btnLowmediumLvl_Click);
            btnLowmediumLvl.Left = 100;
            btnLowmediumLvl.Top = 320;

            /* Buttom for high-medium level */

            btnHighmediumLvl = new Button();
            this.Controls.Add(btnHighmediumLvl);
            btnHighmediumLvl.BackColor = Color.FromName("Maroon");
            try { btnHighmediumLvl.Image = Image.FromFile(@"images\btnHighmediumLevel.png"); }
            catch
            {
                btnHighmediumLvl.Text = "BRING 'EM ON!";
                btnHighmediumLvl.ForeColor = Color.White;
            }
            btnHighmediumLvl.Size = new Size(200, 165);
            btnHighmediumLvl.FlatAppearance.BorderSize = 0;
            btnHighmediumLvl.FlatStyle = FlatStyle.Popup;
            btnHighmediumLvl.Click += new EventHandler(btnHighmediumLvl_Click);
            btnHighmediumLvl.Top = 70;
            btnHighmediumLvl.Left = 400;

            /* Buttom for hard level */

            btnHardLvl = new Button();
            this.Controls.Add(btnHardLvl);
            btnHardLvl.BackColor = Color.FromName("Maroon");
            try { btnHardLvl.Image = Image.FromFile(@"images\btnHardLevel.png"); }
            catch
            {
                btnHardLvl.Text = "I AM DEATH INCARNATE!";
                btnHardLvl.ForeColor = Color.White;
            }
            btnHardLvl.Size = new Size(200, 165);
            btnHardLvl.FlatAppearance.BorderSize = 0;
            btnHardLvl.FlatStyle = FlatStyle.Popup;
            btnHardLvl.Click += new EventHandler(btnHardLvl_Click);
            btnHardLvl.Top = 320;
            btnHardLvl.Left = 400;

            /* Reading "leaderboards" info */

            try
            {
                info = File.ReadAllLines(@"saves\times.txt");
            }
            catch
            {
                info = new string[4];
                for (int i = 0; i < 4; i++)
                    info[i] = "0 0 0 0\r\n";
            }


            readInfo(easyLvlInfo, 0);
            readInfo(lowmediumLvlInfo, 1);
            readInfo(highmediumLvlInfo, 2);
            readInfo(hardLvlInfo, 3);


            /* Crating additional Labels and PictureBoxes for displaying leaderboards info */

            /* For easy level */

            easyLvlCrown = new PictureBox();

            this.Controls.Add(easyLvlCrown);
            
            easyLvlCrown.BackgroundImageLayout = ImageLayout.Stretch;
            easyLvlCrown.Size = new Size(25, 25);
            easyLvlCrown.Top = btnEasyLvl.Top + btnEasyLvl.Height + 5;
            easyLvlCrown.Left = btnEasyLvl.Left;
            easyLvlCrown.BackColor = Color.Transparent;

            easyLvlStopwatch = new PictureBox();

            this.Controls.Add(easyLvlStopwatch);
            
            easyLvlStopwatch.BackgroundImageLayout = ImageLayout.Stretch;
            easyLvlStopwatch.Size = new Size(25, 25);
            easyLvlStopwatch.Top = btnEasyLvl.Top + btnEasyLvl.Height + 35;
            easyLvlStopwatch.Left = btnEasyLvl.Left;
            easyLvlStopwatch.BackColor = Color.Transparent;


            easyLvlLabelStats = new Label();
            this.Controls.Add(easyLvlLabelStats);
            easyLvlLabelStats.Font = new Font("Century Gothic", 7, FontStyle.Regular);
            easyLvlLabelStats.Top = easyLvlCrown.Top;
            easyLvlLabelStats.Left = easyLvlCrown.Left + 30;
            easyLvlLabelStats.BackColor = Color.Transparent;
            easyLvlLabelStats.ForeColor = Color.White;

            easyLvlLabelTime = new Label();
            this.Controls.Add(easyLvlLabelTime);

            easyLvlLabelTime.Font = new Font("Century Gothic", 7, FontStyle.Regular);
            easyLvlLabelTime.Top = easyLvlStopwatch.Top;
            easyLvlLabelTime.Left = easyLvlCrown.Left + 30;
            easyLvlLabelTime.BackColor = Color.Transparent;
            easyLvlLabelTime.ForeColor = Color.White;
            easyLvlLabelTime.AutoSize = true;

            /* For lowmedium level */

            lowmediumLvlCrown = new PictureBox();

            this.Controls.Add(lowmediumLvlCrown);
            
            lowmediumLvlCrown.BackgroundImageLayout = ImageLayout.Stretch;
            lowmediumLvlCrown.Size = new Size(25, 25);
            lowmediumLvlCrown.Top = btnLowmediumLvl.Top + btnLowmediumLvl.Height + 5;
            lowmediumLvlCrown.Left = btnLowmediumLvl.Left;
            lowmediumLvlCrown.BackColor = Color.Transparent;

            lowmediumLvlStopwatch = new PictureBox();

            this.Controls.Add(lowmediumLvlStopwatch);
            
            lowmediumLvlStopwatch.BackgroundImageLayout = ImageLayout.Stretch;
            lowmediumLvlStopwatch.Size = new Size(25, 25);
            lowmediumLvlStopwatch.Top = btnLowmediumLvl.Top + btnLowmediumLvl.Height + 35;
            lowmediumLvlStopwatch.Left = btnLowmediumLvl.Left;
            lowmediumLvlStopwatch.BackColor = Color.Transparent;


            lowmediumLvlLabelStats = new Label();
            this.Controls.Add(lowmediumLvlLabelStats);
            lowmediumLvlLabelStats.Font = new Font("Century Gothic", 7, FontStyle.Regular);
            lowmediumLvlLabelStats.Top = lowmediumLvlCrown.Top;
            lowmediumLvlLabelStats.Left = lowmediumLvlCrown.Left + 30;
            lowmediumLvlLabelStats.BackColor = Color.Transparent;
            lowmediumLvlLabelStats.ForeColor = Color.White;

            lowmediumLvlLabelTime = new Label();
            this.Controls.Add(lowmediumLvlLabelTime);

            lowmediumLvlLabelTime.Font = new Font("Century Gothic", 7, FontStyle.Regular);
            lowmediumLvlLabelTime.Top = lowmediumLvlStopwatch.Top;
            lowmediumLvlLabelTime.Left = lowmediumLvlCrown.Left + 30;
            lowmediumLvlLabelTime.BackColor = Color.Transparent;
            lowmediumLvlLabelTime.ForeColor = Color.White;
            lowmediumLvlLabelTime.AutoSize = true;

            /* For highmedium level */

            highmediumLvlCrown = new PictureBox();

            this.Controls.Add(highmediumLvlCrown);
            
            highmediumLvlCrown.BackgroundImageLayout = ImageLayout.Stretch;
            highmediumLvlCrown.Size = new Size(25, 25);
            highmediumLvlCrown.Top = btnHighmediumLvl.Top + btnHighmediumLvl.Height + 5;
            highmediumLvlCrown.Left = btnHighmediumLvl.Left;
            highmediumLvlCrown.BackColor = Color.Transparent;

            highmediumLvlStopwatch = new PictureBox();

            this.Controls.Add(highmediumLvlStopwatch);
            
            highmediumLvlStopwatch.BackgroundImageLayout = ImageLayout.Stretch;
            highmediumLvlStopwatch.Size = new Size(25, 25);
            highmediumLvlStopwatch.Top = btnHighmediumLvl.Top + btnHighmediumLvl.Height + 35;
            highmediumLvlStopwatch.Left = btnHighmediumLvl.Left;
            highmediumLvlStopwatch.BackColor = Color.Transparent;


            highmediumLvlLabelStats = new Label();
            this.Controls.Add(highmediumLvlLabelStats);
            highmediumLvlLabelStats.Font = new Font("Century Gothic", 7, FontStyle.Regular);
            highmediumLvlLabelStats.Top = highmediumLvlCrown.Top;
            highmediumLvlLabelStats.Left = highmediumLvlCrown.Left + 30;
            highmediumLvlLabelStats.BackColor = Color.Transparent;
            highmediumLvlLabelStats.ForeColor = Color.White;

            highmediumLvlLabelTime = new Label();
            this.Controls.Add(highmediumLvlLabelTime);

            highmediumLvlLabelTime.Font = new Font("Century Gothic", 7, FontStyle.Regular);
            highmediumLvlLabelTime.Top = highmediumLvlStopwatch.Top;
            highmediumLvlLabelTime.Left = highmediumLvlCrown.Left + 30;
            highmediumLvlLabelTime.BackColor = Color.Transparent;
            highmediumLvlLabelTime.ForeColor = Color.White;
            highmediumLvlLabelTime.AutoSize = true;

            /* For hard level */

            hardLvlCrown = new PictureBox();

            this.Controls.Add(hardLvlCrown);
           
            hardLvlCrown.BackgroundImageLayout = ImageLayout.Stretch;
            hardLvlCrown.Size = new Size(25, 25);
            hardLvlCrown.Top = btnHardLvl.Top + btnHardLvl.Height + 5;
            hardLvlCrown.Left = btnHardLvl.Left;
            hardLvlCrown.BackColor = Color.Transparent;

            hardLvlStopwatch = new PictureBox();

            this.Controls.Add(hardLvlStopwatch);
            
            hardLvlStopwatch.BackgroundImageLayout = ImageLayout.Stretch;
            hardLvlStopwatch.Size = new Size(25, 25);
            hardLvlStopwatch.Top = btnHardLvl.Top + btnHardLvl.Height + 35;
            hardLvlStopwatch.Left = btnHardLvl.Left;
            hardLvlStopwatch.BackColor = Color.Transparent;


            hardLvlLabelStats = new Label();
            this.Controls.Add(hardLvlLabelStats);
            hardLvlLabelStats.Font = new Font("Century Gothic", 7, FontStyle.Regular);
            hardLvlLabelStats.Top = hardLvlCrown.Top;
            hardLvlLabelStats.Left = hardLvlCrown.Left + 30;
            hardLvlLabelStats.BackColor = Color.Transparent;
            hardLvlLabelStats.ForeColor = Color.White;

            hardLvlLabelTime = new Label();
            this.Controls.Add(hardLvlLabelTime);

            hardLvlLabelTime.Font = new Font("Century Gothic", 7, FontStyle.Regular);
            hardLvlLabelTime.Top = hardLvlStopwatch.Top;
            hardLvlLabelTime.Left = hardLvlCrown.Left + 30;
            hardLvlLabelTime.BackColor = Color.Transparent;
            hardLvlLabelTime.ForeColor = Color.White;
            hardLvlLabelTime.AutoSize = true;


            try
            {
                easyLvlCrown.BackgroundImage = Image.FromFile(@"images\crown.png");
                lowmediumLvlCrown.BackgroundImage = Image.FromFile(@"images\crown.png");
                highmediumLvlCrown.BackgroundImage = Image.FromFile(@"images\crown.png");
                hardLvlCrown.BackgroundImage = Image.FromFile(@"images\crown.png");
            }
            catch { }
            try
            {
                easyLvlStopwatch.BackgroundImage = Image.FromFile(@"images\stopwatch.png");
                lowmediumLvlStopwatch.BackgroundImage = Image.FromFile(@"images\stopwatch.png");
                highmediumLvlStopwatch.BackgroundImage = Image.FromFile(@"images\stopwatch.png");
                hardLvlStopwatch.BackgroundImage = Image.FromFile(@"images\stopwatch.png");
            }
            catch { }
            _Update();
        }

        /* Функція зчитування даних про рекорди відповідного рівня. Приймає
         * масив arr - куди записати інформацію
         * index - звідки зчитувати цю інформацію
         */
        protected void readInfo(long[] arr, int index)
        {
            for (int i = 0; i < 4; i++)
            {
                try
                {
                    arr[i] = Convert.ToInt32(info[index].Split(' ')[i]);
                }
                catch
                {
                    arr[i] = 0;
                }
            }
        }

        /* Функція, що видаляє всі елементи, які не є необХідними
         * у даний момент гри (всі комірки, секундомір, елементи
         * для відображення к-сті мін, що залишились), та обнуляє
         * змінні */
        protected void Clearing()
        {
            nodeCounter = 0;
            mineCounter = 0;
            firstClick = false;
            if (field!=null)
                foreach (Field f in field)
                    f.Dispose();
            this.Size = new Size(720, 650);
            if (lStopwatch!=null)
                lStopwatch.Dispose();
            if (timer != null)
                timer.Dispose();
            if (lMines != null)
                lMines.Dispose();
            if (minesLogo != null)
                minesLogo.Dispose();
        }

        /* Функція для оновлення інформації про рекорди всіХ рівнів, занесення цієї інформації
         * до відповідних масивів та виведення її у головному меню
         */
        protected void _Update()
        {
            text = easyLvlInfo[0].ToString();
            if (easyLvlInfo[1] != 0)
                text += "(" + Math.Round(Convert.ToDouble(easyLvlInfo[0]) / Convert.ToDouble(easyLvlInfo[1]) * 100).ToString() + "%)";
            easyLvlLabelStats.Text = text;
            if (easyLvlInfo[2] == 0)
                text = "none";
            else
                text = (easyLvlInfo[2] / 60000).ToString() + ":" + (((easyLvlInfo[2]) - (easyLvlInfo[2] / 60000) * 60000) / 1000).ToString() + ":" + (easyLvlInfo[2] % 1000).ToString() + "("+ (easyLvlInfo[3] / 60000).ToString() + ":" + (((easyLvlInfo[3]) - (easyLvlInfo[3] / 60000) * 60000) / 1000).ToString() + ":" + (easyLvlInfo[3] % 1000).ToString()+")";
            easyLvlLabelTime.Text = text;
            text = lowmediumLvlInfo[0].ToString();
            if (lowmediumLvlInfo[1] != 0)
                text += "(" + Math.Round(Convert.ToDouble(lowmediumLvlInfo[0]) / Convert.ToDouble(lowmediumLvlInfo[1]) * 100).ToString() + "%)";
            lowmediumLvlLabelStats.Text = text;
            if (lowmediumLvlInfo[2] == 0)
                text = "none";
            else
                text = (lowmediumLvlInfo[2] / 60000).ToString() + ":" + (((lowmediumLvlInfo[2]) - (lowmediumLvlInfo[2] / 60000) * 60000) / 1000).ToString() + ":" + (lowmediumLvlInfo[2] % 1000).ToString() + "(" + (lowmediumLvlInfo[3] / 60000).ToString() + ":" + (((lowmediumLvlInfo[3]) - (lowmediumLvlInfo[3] / 60000) * 60000) / 1000).ToString() + ":" + (lowmediumLvlInfo[3] % 1000).ToString() + ")";
            lowmediumLvlLabelTime.Text = text;
            text = highmediumLvlInfo[0].ToString();
            if (highmediumLvlInfo[1] != 0)
                text += "(" + Math.Round(Convert.ToDouble(highmediumLvlInfo[0]) / Convert.ToDouble(highmediumLvlInfo[1]) * 100).ToString() + "%)";
            highmediumLvlLabelStats.Text = text;
            if (highmediumLvlInfo[2] == 0)
                text = "none";
            else
                text = (highmediumLvlInfo[2] / 60000).ToString() + ":" + (((highmediumLvlInfo[2]) - (highmediumLvlInfo[2] / 60000) * 60000) / 1000).ToString() + ":" + (highmediumLvlInfo[2] % 1000).ToString() + "(" + (highmediumLvlInfo[3] / 60000).ToString() + ":" + (((highmediumLvlInfo[3]) - (highmediumLvlInfo[3] / 60000) * 60000) / 1000).ToString() + ":" + (highmediumLvlInfo[3] % 1000).ToString() + ")";
            highmediumLvlLabelTime.Text = text;
            text = hardLvlInfo[0].ToString();
            if (hardLvlInfo[1] != 0)
                text += "(" + Math.Round(Convert.ToDouble(hardLvlInfo[0]) / Convert.ToDouble(hardLvlInfo[1]) * 100).ToString() + "%)";
            hardLvlLabelStats.Text = text;
            if (hardLvlInfo[2] == 0)
                text = "none";
            else
                text = (hardLvlInfo[2] / 60000).ToString() + ":" + (((hardLvlInfo[2]) - (hardLvlInfo[2] / 60000) * 60000) / 1000).ToString() + ":" + (hardLvlInfo[2] % 1000).ToString() + "(" + (hardLvlInfo[3] / 60000).ToString() + ":" + (((hardLvlInfo[3]) - (hardLvlInfo[3] / 60000) * 60000) / 1000).ToString() + ":" + (hardLvlInfo[3] % 1000).ToString() + ")";
            hardLvlLabelTime.Text = text;
        }

        /* Функція, яка виконується при завершенні рівня. Неважливо, перемогою чи поразкою
         * Всі комірки становляться неативними. Відоражаються позиції мін (при поразці).
         * Також при перемозі формується нова статистика для рекордів рівня
         */
        public void Finish(bool passed, int level)
        {
            nodeCounter = 0;
            mineCounter = 0;
            firstClick = false;
            foreach (Field f in field)
            {
                if (passed)
                {
                    if (f.Value == 'm')
                    {
                        f.BackColor = Color.Gold;
                        try
                        {
                            f.BackgroundImage = Image.FromFile(@"images\flag.png");
                        }
                        catch
                        {
                            f.Text = "f";
                        }
                        f.BackgroundImageLayout = ImageLayout.Stretch;
                    }
                    else
                    {
                        f.BackColor = Color.Salmon;
                        if (f.Value != '0')
                            f.Text = f.Value.ToString();
                    }
                }
                else
                {
                    if (f.Value == 'm' && !f.Flagged)
                    {
                        f.Text = "";
                        try
                        {
                            f.BackgroundImage = Image.FromFile(@"images/mine.png");
                        }
                        catch
                        {
                            f.Text = "M";
                        }
                        f.BackgroundImageLayout = ImageLayout.Stretch;
                        f.BackColor = Color.Maroon;
                    }
                }
                f.Enabled = false;
            }
            switch (level)
            {
                case 1:
                    if (passed)
                    {
                        long time = stopWatch.Millisecond + stopWatch.Second * 1000 + stopWatch.Minute * 60000;
                        if (time < easyLvlInfo[2] || easyLvlInfo[2]==0)
                            easyLvlInfo[2] = time;
                        easyLvlInfo[3] = Convert.ToInt32(Math.Round(Convert.ToDouble(easyLvlInfo[3]*easyLvlInfo[0] + time))/(++easyLvlInfo[0]));
                        easyLvlInfo[1]++;
                    }
                    else
                    {
                        easyLvlInfo[1]++;
                    }
                    break;
                case 2:
                    if (passed)
                    {
                        long time = stopWatch.Millisecond + stopWatch.Second * 1000 + stopWatch.Minute * 60000;
                        if (time < lowmediumLvlInfo[2] || lowmediumLvlInfo[2] == 0)
                            lowmediumLvlInfo[2] = time;
                        lowmediumLvlInfo[3] = Convert.ToInt32(Math.Round(Convert.ToDouble(lowmediumLvlInfo[3] * lowmediumLvlInfo[0] + time)) / (++lowmediumLvlInfo[0]));
                        lowmediumLvlInfo[1]++;
                    }
                    else
                    {
                        lowmediumLvlInfo[1]++;
                    }
                    break;
                case 3:
                    if (passed)
                    {
                        long time = stopWatch.Millisecond + stopWatch.Second * 1000 + stopWatch.Minute * 60000;
                        if (time < highmediumLvlInfo[2] || highmediumLvlInfo[2] == 0)
                            highmediumLvlInfo[2] = time;
                        highmediumLvlInfo[3] = Convert.ToInt32(Math.Round(Convert.ToDouble(highmediumLvlInfo[3] * highmediumLvlInfo[0] + time)) / (++highmediumLvlInfo[0]));
                        highmediumLvlInfo[1]++;
                    }
                    else
                    {
                        highmediumLvlInfo[1]++;
                    }
                    break;
                case 4:
                    if (passed)
                    {
                        long time = stopWatch.Millisecond + stopWatch.Second * 1000 + stopWatch.Minute * 60000;
                        if (time < hardLvlInfo[2] || hardLvlInfo[2] == 0)
                            hardLvlInfo[2] = time;
                        hardLvlInfo[3] = Convert.ToInt32(Math.Round(Convert.ToDouble(hardLvlInfo[3] * hardLvlInfo[0] + time)) / (++hardLvlInfo[0]));
                        hardLvlInfo[1]++;
                    }
                    else
                    {
                        hardLvlInfo[1]++;
                    }
                    break;
            }
        }

        // Подія при натисканні на кнопку "Main menu"
        private void MainMenuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clearing();
            showAll();
            _Update();

        }
        /* Функція, яка виконується при натисканні копнки якого з рівнів.
         * Тут генерується ігрове поле, заводиться секундомір, стовурюються
         * елементи графічного інтерфейсу */
        public void Game(int n, int m, int k) //n,m - size, k - mines
        {
            Clearing();
            hideAll();
            lStopwatch = new Label();
            this.Controls.Add(lStopwatch);
            lStopwatch.Text = "00:00:000";
            lStopwatch.Font = new Font("Century Gothic", 20,FontStyle.Regular);
            lStopwatch.Top = 80;
            lStopwatch.BackColor = Color.Transparent;
            lStopwatch.ForeColor = Color.White;
            lStopwatch.AutoSize = true;
            int size = 50, width = size*m, height = size*n;
            field = new Field[n, m];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    field[i, j] = new Field();
                    this.Controls.Add(field[i, j]);
                    field[i, j].X = i;
                    field[i, j].Y = j;
                    field[i, j].BackgroundImageLayout = ImageLayout.Stretch;
                    field[i, j].Size = new Size(size, size);
                    field[i, j].Left = ((width+300) - size * m - 5 * (m - 1)) / 2 + ((size + 5) * j) - 5;
                    field[i, j].Top = 200 + (size + 5) * i;
                    field[i, j].Value = '0';
                    field[i, j].MouseDown += new MouseEventHandler(field_MouseDown);
                    field[i, j].BackColor = Color.FromName("Maroon");
                    field[i, j].Checked = false;
                    field[i, j].Flagged = false;
                    field[i,j].Font = new Font("Century Gothic", 10, FontStyle.Bold);
                }
            }
            lMines = new Label();
            this.Controls.Add(lMines);
            lMines.Text = k.ToString();
            lMines.Font = new Font("Century Gothic", 20, FontStyle.Regular);
            lMines.Top = 80;
            lMines.BackColor = Color.Transparent;
            lMines.ForeColor = Color.White;
            lMines.AutoSize = true;
            lMines.Left = field[0, m - 1].Left + field[0, m - 1].Width - lMines.Width;

            minesLogo = new PictureBox();
            this.Controls.Add(minesLogo);
            minesLogo.Size = new Size(lStopwatch.Height, lStopwatch.Height);
            try
            {
                minesLogo.BackgroundImage = Image.FromFile(@"images\mine.png");
            }
            catch
            {
                
            }
            
            minesLogo.BackgroundImageLayout = ImageLayout.Stretch;
            minesLogo.Left = lMines.Left - minesLogo.Width-10;
            minesLogo.Top = lMines.Top;
            minesLogo.BackColor = Color.Transparent;

            lStopwatch.Left = field[0, 0].Left-15;
            this.Size = new Size(width+300, height+400);

        }

        // Фунція, яка скриває всі елменти головного меню
        protected void hideAll()
        {

            btnEasyLvl.Visible = false;
            btnLowmediumLvl.Visible = false;
            btnHighmediumLvl.Visible = false;
            btnHardLvl.Visible = false;
            easyLvlCrown.Visible = false;
            easyLvlStopwatch.Visible = false;
            easyLvlLabelStats.Visible = false;
            easyLvlLabelTime.Visible = false;
            lowmediumLvlCrown.Visible = false;
            lowmediumLvlStopwatch.Visible = false;
            lowmediumLvlLabelStats.Visible = false;
            lowmediumLvlLabelTime.Visible = false;
            highmediumLvlCrown.Visible = false;
            highmediumLvlStopwatch.Visible = false;
            highmediumLvlLabelStats.Visible = false;
            highmediumLvlLabelTime.Visible = false;
            hardLvlCrown.Visible = false;
            hardLvlStopwatch.Visible = false;
            hardLvlLabelStats.Visible = false;
            hardLvlLabelTime.Visible = false;
        }

        // Функція, яка відображає всі елементи головного меню
        protected void showAll()
        {

            btnEasyLvl.Visible = true;
            btnLowmediumLvl.Visible = true;
            btnHighmediumLvl.Visible = true;
            btnHardLvl.Visible = true;
            easyLvlCrown.Visible = true;
            easyLvlStopwatch.Visible = true;
            easyLvlLabelStats.Visible = true;
            easyLvlLabelTime.Visible = true;
            lowmediumLvlCrown.Visible = true;
            lowmediumLvlStopwatch.Visible = true;
            lowmediumLvlLabelStats.Visible = true;
            lowmediumLvlLabelTime.Visible = true;
            highmediumLvlCrown.Visible = true;
            highmediumLvlStopwatch.Visible = true;
            highmediumLvlLabelStats.Visible = true;
            highmediumLvlLabelTime.Visible = true;
            hardLvlCrown.Visible = true;
            hardLvlStopwatch.Visible = true;
            hardLvlLabelStats.Visible = true;
            hardLvlLabelTime.Visible = true;

        }

        // Подія, що відбувається при натисканні на кнопку легкого рівня
        protected void btnEasyLvl_Click(object sender, EventArgs e) //n,m - size, k - mimes
        {
            fieldHeight = 10;
            fieldWidth = 10;
            minesQuantity = 9;
            levelDifficulty = 1;
            Game(10, 10, 9);
        }

        // Подія, що відбувається при натисканні на кнопку середнього рівня
        protected void btnLowmediumLvl_Click(object sender, EventArgs e) //n,m - size, k - mimes
        {
            fieldHeight = 16;
            fieldWidth = 16;
            minesQuantity = 40;
            levelDifficulty = 2;
            Game(16, 16, 40);
        }

        // Подія, що відбувається при натисканні на кнопку складного рівня
        protected void btnHighmediumLvl_Click(object sender, EventArgs e) //n,m - size, k - mimes
        {
            fieldHeight = 16;
            fieldWidth = 30;
            minesQuantity = 99;
            levelDifficulty = 3;
            Game(16, 30, 99);
        }

        // Подія, що відбувається при натисканні на кнопку надскладного рівня
        protected void btnHardLvl_Click(object sender, EventArgs e) //n,m - size, k - mimes
        {
            fieldHeight = 20;
            fieldWidth = 35;
            minesQuantity = 175;
            levelDifficulty = 4;
            Game(20, 35, 175);
        }

        /* Функція для відображення часу секундоміра. Порівнюється різниця в часі
         * між тим,коли секундомір був заведений та поточним часом
         */
        private void tickTimer(object sender, EventArgs e)
        {
            if (timer.Enabled)
            {
                long tick = DateTime.Now.Ticks - date.Ticks;
                stopWatch = new DateTime();
                if (lStopwatch.Text != "59:59:999")
                {
                    stopWatch = stopWatch.AddTicks(tick);
                    lStopwatch.Text = String.Format("{0:mm:ss:fff}", stopWatch);
                }
                else
                    timer.Enabled = false;
                
            }
            
        }

        // Функція, яка виконується при натисканні на комірку лівої кнопкої миші
        public void openField(Field node)
        {
            /* Якщо ще не було зроблено кліку, то міни розставляються таким чином, аби після першого
             * кліку відкрилась якась частина ігрового поля */
            if (!firstClick)
            {
                firstClick = true;
                date = DateTime.Now;
                timer = new Timer();
                timer.Interval = 10;
                timer.Tick += new EventHandler(tickTimer);
                timer.Enabled = true;



                for (int z = 0; z < minesQuantity; z++)
                {
                    Random rnd = new Random();
                    int x = rnd.Next(0, fieldHeight);
                    int y = rnd.Next(0, fieldWidth);
                    if (field[x, y].Value == 'm' || (Math.Abs(x-node.X)<=1 && Math.Abs(y-node.Y)<=1))
                        while (field[x, y].Value == 'm' || (Math.Abs(x - node.X) <= 1 && Math.Abs(y - node.Y) <= 1))
                        {
                            x = rnd.Next(0, fieldHeight);
                            y = rnd.Next(0, fieldWidth);
                        }
                    field[x, y].Value = 'm';
                    if (x - 1 >= 0 && y - 1 >= 0)
                        if (field[x - 1, y - 1].Value != 'm')
                            field[x - 1, y - 1].Value = Convert.ToChar(Convert.ToInt32(field[x - 1, y - 1].Value) + 1);

                    if (x - 1 >= 0 && y >= 0)
                        if (field[x - 1, y].Value != 'm')
                            field[x - 1, y].Value = Convert.ToChar(Convert.ToInt32(field[x - 1, y].Value) + 1);

                    if (x - 1 >= 0 && y + 1 <= fieldWidth - 1)
                        if (field[x - 1, y + 1].Value != 'm')
                            field[x - 1, y + 1].Value = Convert.ToChar(Convert.ToInt32(field[x - 1, y + 1].Value) + 1);

                    if (x >= 0 && y - 1 >= 0)
                        if (field[x, y - 1].Value != 'm')
                            field[x, y - 1].Value = Convert.ToChar(Convert.ToInt32(field[x, y - 1].Value) + 1);

                    if (x >= 0 && y + 1 <= fieldWidth - 1)
                        if (field[x, y + 1].Value != 'm')
                            field[x, y + 1].Value = Convert.ToChar(Convert.ToInt32(field[x, y + 1].Value) + 1);

                    if (x + 1 <= fieldHeight - 1 && y - 1 >= 0)
                        if (field[x + 1, y - 1].Value != 'm')
                            field[x + 1, y - 1].Value = Convert.ToChar(Convert.ToInt32(field[x + 1, y - 1].Value) + 1);

                    if (x + 1 <= fieldHeight - 1 && y >= 0)
                        if (field[x + 1, y].Value != 'm')
                            field[x + 1, y].Value = Convert.ToChar(Convert.ToInt32(field[x + 1, y].Value) + 1);

                    if (x + 1 <= fieldHeight - 1 && y + 1 <= fieldWidth - 1)
                        if (field[x + 1, y + 1].Value != 'm')
                            field[x + 1, y + 1].Value = Convert.ToChar(Convert.ToInt32(field[x + 1, y + 1].Value) + 1);

                    //for (int i = 0; i < n; i++)
                    //{
                    //    for (int j = 0; j < m; j++)
                    //    {
                    //        field[i, j].Text = Convert.ToString(field[i, j].Value);
                    //    }
                    //}


                }
            }
            // Якщо в комірці міна і вона не помічена, гравець програє
            if (node.Value == 'm')
            {
                if (!node.Flagged)
                {
                    timer.Enabled = false;
                    MessageBox.Show("You lose!");
                    Finish(false,levelDifficulty);
                }
            // Якщо міна, але помічена, мітка знімається
                else
                {
                    node.Flagged = false;
                    node.Text = "";
                    node.BackColor = Color.FromName("Maroon");
                    node.BackgroundImage = null;
                    mineCounter--;
                }
            }
           
            // Якщо комірка порожня, то автоматично відкриваються комірки навколо неї
            // Це робиться за допомогою визову функції openField від навколишніх комірок
            else if (node.Value == '0')
            {
                if (!node.Checked)
                {
                    node.Text = "";
                    node.Flagged = false;
                    node.BackgroundImage = null;
                    node.BackColor = Color.FromName("Salmon");
                    node.Checked = true;
                    nodeCounter++;
                    if (node.X - 1 >= 0 && node.Y - 1 >= 0 && !field[node.X - 1, node.Y - 1].Checked)
                        openField(field[node.X - 1, node.Y - 1]);
                    if (node.X - 1 >= 0 && !field[node.X - 1, node.Y].Checked)
                        openField(field[node.X - 1, node.Y]);
                    if (node.X - 1 >= 0 && node.Y + 1 <= fieldWidth - 1 && !field[node.X - 1, node.Y + 1].Checked)
                        openField(field[node.X - 1, node.Y + 1]);
                    if (node.Y - 1 >= 0 && !field[node.X, node.Y - 1].Checked)
                        openField(field[node.X, node.Y - 1]);
                    if (node.Y + 1 <= fieldWidth - 1 && !field[node.X, node.Y + 1].Checked)
                        openField(field[node.X, node.Y + 1]);
                    if (node.X + 1 <= fieldHeight - 1 && node.Y - 1 >= 0 && !field[node.X + 1, node.Y - 1].Checked)
                        openField(field[node.X + 1, node.Y - 1]);
                    if (node.X + 1 <= fieldHeight - 1 && !field[node.X + 1, node.Y].Checked)
                        openField(field[node.X + 1, node.Y]);
                    if (node.X + 1 <= fieldHeight - 1 && node.Y + 1 <= fieldWidth - 1 && !field[node.X + 1, node.Y + 1].Checked)
                        openField(field[node.X + 1, node.Y + 1]);
                }
                

            }
            // Інакше комірка відкривається. Якщо всі можливі комірки відкриті, гра рівень завершується з перемогою
            else
            {
                if (!node.Checked)
                {
                    node.Text = Convert.ToString(node.Value);
                    node.BackgroundImage = null;
                    node.BackColor = Color.FromName("Salmon");
                    node.Checked = true;
                    node.Flagged = false;
                    nodeCounter++;
                    if (nodeCounter == fieldHeight * fieldWidth - minesQuantity)
                    {
                        timer.Enabled = false;
                        MessageBox.Show("You win!");
                        Finish(true,levelDifficulty);
                    }
                }
                else
                {
                    if (countFlags(node) == Convert.ToInt32(node.Value.ToString()))
                    {
                        if (node.X - 1 >= 0 && node.Y - 1 >= 0 && !field[node.X - 1, node.Y - 1].Checked && !field[node.X - 1, node.Y - 1].Flagged)
                            openField(field[node.X - 1, node.Y - 1]);
                        if (node.X - 1 >= 0 && !field[node.X - 1, node.Y].Checked && !field[node.X - 1, node.Y].Flagged)
                            openField(field[node.X - 1, node.Y]);
                        if (node.X - 1 >= 0 && node.Y + 1 <= fieldWidth - 1 && !field[node.X - 1, node.Y + 1].Checked && !field[node.X - 1, node.Y + 1].Flagged)
                            openField(field[node.X - 1, node.Y + 1]);
                        if (node.Y - 1 >= 0 && !field[node.X, node.Y - 1].Checked && !field[node.X, node.Y - 1].Flagged)
                            openField(field[node.X, node.Y - 1]);
                        if (node.Y + 1 <= fieldWidth - 1 && !field[node.X, node.Y + 1].Checked && !field[node.X, node.Y + 1].Flagged)
                            openField(field[node.X, node.Y + 1]);
                        if (node.X + 1 <= fieldHeight - 1 && node.Y - 1 >= 0 && !field[node.X + 1, node.Y - 1].Checked && !field[node.X + 1, node.Y - 1].Flagged)
                            openField(field[node.X + 1, node.Y - 1]);
                        if (node.X + 1 <= fieldHeight - 1 && !field[node.X + 1, node.Y].Checked && !field[node.X + 1, node.Y].Flagged)
                            openField(field[node.X + 1, node.Y]);
                        if (node.X + 1 <= fieldHeight - 1 && node.Y + 1 <= fieldWidth - 1 && !field[node.X + 1, node.Y + 1].Checked && !field[node.X + 1, node.Y + 1].Flagged)
                            openField(field[node.X + 1, node.Y + 1]);
                    }
                }
               
            }
        }

        // Функція для підрахнку поставлених флагів навколо комірки
        public int countFlags(Field node)
        {
            int flagCounter = 0;
            if (node.X - 1 >= 0 && node.Y - 1 >= 0 && field[node.X - 1, node.Y - 1].Flagged)
                flagCounter++;
            if (node.X - 1 >= 0 && field[node.X - 1, node.Y].Flagged)
                flagCounter++;
            if (node.X - 1 >= 0 && node.Y + 1 <= fieldWidth - 1 && field[node.X - 1, node.Y + 1].Flagged)
                flagCounter++;
            if (node.Y - 1 >= 0 && field[node.X, node.Y - 1].Flagged)
                flagCounter++;
            if (node.Y + 1 <= fieldWidth - 1 && field[node.X, node.Y + 1].Flagged)
                flagCounter++;
            if (node.X + 1 <= fieldHeight - 1 && node.Y - 1 >= 0 && field[node.X + 1, node.Y - 1].Flagged)
                flagCounter++;
            if (node.X + 1 <= fieldHeight - 1 && field[node.X + 1, node.Y].Flagged)
                flagCounter++;
            if (node.X + 1 <= fieldHeight - 1 && node.Y + 1 <= fieldWidth - 1 && field[node.X + 1, node.Y + 1].Flagged)
                flagCounter++;
            return flagCounter;
        }

        // Функція для підрахнку закритих комірок навколо комірки
        public int countClosed(Field node)
        {
            int closedCounter = 0;
            if (node.X - 1 >= 0 && node.Y - 1 >= 0 && !field[node.X - 1, node.Y - 1].Checked)
                closedCounter++;
            if (node.X - 1 >= 0 && !field[node.X - 1, node.Y].Checked)
                closedCounter++;
            if (node.X - 1 >= 0 && node.Y + 1 <= fieldWidth - 1 && !field[node.X - 1, node.Y + 1].Checked)
                closedCounter++;
            if (node.Y - 1 >= 0 && !field[node.X, node.Y - 1].Checked)
                closedCounter++;
            if (node.Y + 1 <= fieldWidth - 1 && !field[node.X, node.Y + 1].Checked)
                closedCounter++;
            if (node.X + 1 <= fieldHeight - 1 && node.Y - 1 >= 0 && !field[node.X + 1, node.Y - 1].Checked)
                closedCounter++;
            if (node.X + 1 <= fieldHeight - 1 && node.Y >= 0 && !field[node.X + 1, node.Y].Checked)
                closedCounter++;
            if (node.X + 1 <= fieldHeight - 1 && node.Y + 1 <= fieldWidth - 1 && !field[node.X + 1, node.Y + 1].Checked)
                closedCounter++;
            return closedCounter;
        }

        // Подія при натисканні на кормірку. В залежності від того, яка кнопка була натиснута, виконуються різні дії
        protected void field_MouseDown(object sender, MouseEventArgs e) 
        {
            Field node = (Field)sender;

            if (e.Button == MouseButtons.Left)
                openField(node);
            if (e.Button == MouseButtons.Right && firstClick)
            {
                if (!node.Checked)
                {
                    if (!node.Flagged)
                    {
                        try
                        {
                            node.BackgroundImage = Image.FromFile(@"images\flag.png");
                        }
                        catch
                        {
                            node.Text = "f";
                        }
                        node.Flagged = true;
                        lMines.Text = (Convert.ToInt32(lMines.Text) - 1).ToString();
                        node.BackColor = Color.FromName("Gold");
                        if (node.Value == 'm')
                            mineCounter++;
                        if (mineCounter == minesQuantity)
                        {
                            timer.Enabled = false;
                            MessageBox.Show("You win!");
                            Finish(true, levelDifficulty);
                        }
                    }
                    else
                    {
                        node.BackgroundImage = null;
                        node.Text = "";
                        node.Flagged = false;
                        lMines.Text = (Convert.ToInt32(lMines.Text) + 1).ToString();
                        node.BackColor = Color.FromName("Maroon");
                        if (node.Value == 'm')
                            mineCounter--;
                    }
                    
                }
                else
                {
                    

                    if (countClosed(node) == Convert.ToUInt32(node.Value.ToString()))
                    {
                        if (node.X - 1 >= 0 && node.Y - 1 >= 0 && !field[node.X - 1, node.Y - 1].Checked && !field[node.X - 1, node.Y - 1].Flagged)
                        {
                            try
                            {
                                field[node.X - 1, node.Y - 1].BackgroundImage = Image.FromFile(@"images\flag.png");
                            }
                            catch
                            {
                                field[node.X - 1, node.Y - 1].Text = "f";
                            }
                            field[node.X - 1, node.Y - 1].Flagged = true;
                            lMines.Text = (Convert.ToInt32(lMines.Text) - 1).ToString();
                            field[node.X - 1, node.Y - 1].BackColor = Color.FromName("Gold");
                            mineCounter++;

                        }
                        if (node.X - 1 >= 0 && node.Y >= 0 && !field[node.X - 1, node.Y].Checked && !field[node.X - 1, node.Y].Flagged)
                        {
                            try
                            {
                                field[node.X - 1, node.Y].BackgroundImage = Image.FromFile(@"images\flag.png");
                            }
                            catch
                            {
                                field[node.X - 1, node.Y].Text = "f";
                            }
                            field[node.X - 1, node.Y].Flagged = true;
                            lMines.Text = (Convert.ToInt32(lMines.Text) - 1).ToString();
                            field[node.X - 1, node.Y].BackColor = Color.FromName("Gold");
                            mineCounter++;

                        }
                        if (node.X - 1 >= 0 && node.Y + 1 <= fieldWidth - 1 && !field[node.X - 1, node.Y + 1].Checked && !field[node.X - 1, node.Y + 1].Flagged)
                        {
                            try
                            {
                                field[node.X - 1, node.Y + 1].BackgroundImage = Image.FromFile(@"images\flag.png");
                            }
                            catch
                            {
                                field[node.X - 1, node.Y + 1].Text = "f";
                            }
                            field[node.X - 1, node.Y + 1].Flagged = true;
                            lMines.Text = (Convert.ToInt32(lMines.Text) - 1).ToString();
                            field[node.X - 1, node.Y + 1].BackColor = Color.FromName("Gold");
                            mineCounter++;
                        }
                        if (node.X >= 0 && node.Y - 1 >= 0 && !field[node.X, node.Y - 1].Checked && !field[node.X, node.Y - 1].Flagged)
                        {
                            try
                            {
                                field[node.X, node.Y - 1].BackgroundImage = Image.FromFile(@"images\flag.png");
                            }
                            catch
                            {
                                field[node.X, node.Y - 1].Text = "f";
                            }
                            field[node.X , node.Y - 1].Flagged = true;
                            lMines.Text = (Convert.ToInt32(lMines.Text) - 1).ToString();
                            field[node.X , node.Y - 1].BackColor = Color.FromName("Gold");
                            mineCounter++;
                        }
                        if (node.X >= 0 && node.Y + 1 <= fieldWidth - 1 && !field[node.X, node.Y + 1].Checked && !field[node.X, node.Y + 1].Flagged)
                        {
                            try
                            {
                                field[node.X, node.Y + 1].BackgroundImage = Image.FromFile(@"images\flag.png");
                            }
                            catch
                            {
                                field[node.X, node.Y + 1].Text = "f";
                            }
                            field[node.X, node.Y + 1].Flagged = true;
                            lMines.Text = (Convert.ToInt32(lMines.Text) - 1).ToString();
                            field[node.X , node.Y + 1].BackColor = Color.FromName("Gold");
                            mineCounter++;

                        }
                        if (node.X + 1 <= fieldHeight - 1 && node.Y - 1 >= 0 && !field[node.X + 1, node.Y - 1].Checked && !field[node.X + 1, node.Y - 1].Flagged)
                        {
                            try
                            {
                                field[node.X + 1, node.Y - 1].BackgroundImage = Image.FromFile(@"images\flag.png");
                            }
                            catch
                            {
                                field[node.X + 1, node.Y - 1].Text = "f";
                            }
                            field[node.X + 1, node.Y - 1].Flagged = true;
                            lMines.Text = (Convert.ToInt32(lMines.Text) - 1).ToString();
                            field[node.X + 1, node.Y - 1].BackColor = Color.FromName("Gold");
                            mineCounter++;

                        }
                        if (node.X + 1 <= fieldHeight - 1 && node.Y >= 0 && !field[node.X + 1, node.Y].Checked && !field[node.X + 1, node.Y].Flagged)
                        {
                            try
                            {
                                field[node.X + 1, node.Y].BackgroundImage = Image.FromFile(@"images\flag.png");
                            }
                            catch
                            {
                                field[node.X + 1, node.Y].Text = "f";
                            }
                            field[node.X + 1, node.Y].Flagged = true;
                            lMines.Text = (Convert.ToInt32(lMines.Text) - 1).ToString();
                            field[node.X + 1, node.Y].BackColor = Color.FromName("Gold");
                            mineCounter++;

                        }
                        if (node.X + 1 <= fieldHeight - 1 && node.Y + 1 <= fieldWidth - 1 && !field[node.X + 1, node.Y + 1].Checked && !field[node.X + 1, node.Y + 1].Flagged)
                        {
                            try
                            {
                                field[node.X + 1, node.Y + 1].BackgroundImage = Image.FromFile(@"images\flag.png");
                            }
                            catch
                            {
                                field[node.X + 1, node.Y + 1].Text = "f";
                            }
                            field[node.X + 1, node.Y + 1].Flagged = true;
                            lMines.Text = (Convert.ToInt32(lMines.Text) - 1).ToString();
                            field[node.X + 1, node.Y + 1].BackColor = Color.FromName("Gold");
                            mineCounter++;
                        }
                        if (mineCounter == minesQuantity)
                        {
                            timer.Enabled = false;
                            MessageBox.Show("You win!");
                            Finish(true, levelDifficulty);
                        }
                    }
                }
            }
        }
        // Подія при натискання Help. Відкривається HTML файл з правилами гри.
        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(@"saves\rules.html");
            }
            catch
            {
                MessageBox.Show("Sorry. File is corrupted or deleted!");
            }

        }
        // Подія при натисканні кнопки Exit. Всі необхідні дані записуються у файл і програма закривається
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            text = easyLvlInfo[0].ToString() + ' ' + easyLvlInfo[1].ToString() + ' ' + easyLvlInfo[2].ToString() + ' ' + easyLvlInfo[3].ToString() + "\r\n" + lowmediumLvlInfo[0].ToString() + ' ' + lowmediumLvlInfo[1].ToString() + ' ' + lowmediumLvlInfo[2].ToString() + ' ' + lowmediumLvlInfo[3].ToString() + "\r\n" + highmediumLvlInfo[0].ToString() + ' ' + highmediumLvlInfo[1].ToString() + ' ' + highmediumLvlInfo[2].ToString() + ' ' + highmediumLvlInfo[3].ToString() + "\r\n" + hardLvlInfo[0].ToString() + ' ' + hardLvlInfo[1].ToString() + ' ' + hardLvlInfo[2].ToString() + ' ' + hardLvlInfo[3].ToString();
            if (Directory.Exists(@"saves"))
                File.WriteAllText(@"saves\times.txt",text);
            else
            {
                Directory.CreateDirectory(@"saves");
                File.WriteAllText(@"saves\times.txt", text);
            }
            this.Close();
        }
    }
}
