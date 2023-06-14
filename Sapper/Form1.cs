using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

namespace Sapper
{

    public partial class Form1 : Form
    {

        int w;
        int h;
        int bombPercent = 20;
        int distance = 40; // between button
        int openedCells = 0;
        int bombs = 0;
        

        Image Flag = Image.FromFile(@"C:\Users\38096\source\repos\draft\Sapper\Flag.PNG");
        Image Bomb = Image.FromFile(@"C:\Users\38096\source\repos\draft\Sapper\Bomb.PNG");
        ButtonPlus[,] buttonsArray;
        

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, PaintEventArgs e)
        {
            buttonsArray = new ButtonPlus[w, h];
            GenerateField();
        }


        class ButtonPlus : Button
        {
            public bool isBomb;
            public bool active;
            public bool enqued = false;
            public int xCoordinate;
            public int yCoordinate;
        }


        // decorating buttons
        void button_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, (sender as Button).ClientRectangle,
            SystemColors.ControlLightLight, 5, ButtonBorderStyle.Outset,
            SystemColors.ControlLightLight, 5, ButtonBorderStyle.Outset,
            SystemColors.ControlLight, 5, ButtonBorderStyle.Outset,
            SystemColors.ControlLight, 5, ButtonBorderStyle.Outset);
        }

        private void button_Paint2(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, (sender as Button).ClientRectangle,
            SystemColors.ControlDark, 5, ButtonBorderStyle.Outset,
            SystemColors.ControlDark, 5, ButtonBorderStyle.Outset,
            SystemColors.ControlDark, 5, ButtonBorderStyle.Outset,
            SystemColors.ControlDark, 5, ButtonBorderStyle.Outset);
        }


        void GenerateField()
        {
            buttonsArray = new ButtonPlus[w, h];
            Random bombSummon = new Random();
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    ButtonPlus button = new ButtonPlus();
                    button.Location = new Point(x * distance, y * distance + 30);
                    button.Size = new Size(distance, distance);
                    button.Paint += button_Paint;
                    button.BackColor = Color.White;
                    button.Font = new Font("Segoe UI Black", 18, FontStyle.Bold);
                    button.active = true;

                    if (bombSummon.Next(0, 101) < bombPercent) // making bomb
                    {
                        button.isBomb = true;
                        bombs++;
                    }

                    button.xCoordinate = x;
                    button.yCoordinate = y;
                    Controls.Add(button);
                    buttonsArray[x, y] = button;

                    button.Click += ButtonClick;
                    button.MouseUp += RightButtonClick;
                }
            }
        }


        void Win()
        {
            int allCells = w * h;
            int emptyCells = allCells - bombs;
            if (openedCells == emptyCells)
            {
                MessageBox.Show("YOU WON");
                Application.Restart();
            }
        }


        // marking(removing) flag
        private void RightButtonClick(object sender, MouseEventArgs e)
        {
            ButtonPlus button = sender as ButtonPlus;
            if (e.Button == MouseButtons.Right && button.BackColor == Color.White && button.Text == "")
            {
                if (button.active == true)
                {
                    button.BackgroundImage = Flag;
                    button.active = false;
                }
                else if (button.active == false)
                {
                    button.BackgroundImage = null;
                    button.active = true;
                }
            }
        }


        //selecting button
        void ButtonClick(object sender, EventArgs e)
        {
            ButtonPlus button = sender as ButtonPlus;
            if (button.active)
            {
                if (button.isBomb)
                {
                    var recordedBombs = (from ButtonPlus b in buttonsArray
                                         where b.isBomb
                                         select b).ToList();
                    Explosion(recordedBombs);
                }
                else
                {
                    EmptyField(button);
                    Win();
                }
            }
        }


        //yes Ricko, kaboom
        private void Explosion(List<ButtonPlus> recordedBombs)
        {
            foreach (Button button in recordedBombs)
            {
                button.BackgroundImage = Bomb;
            }
            MessageBox.Show("You lost");
            Application.Restart();
        }


        //giving digits to buttons
        void EmptyField(ButtonPlus button)
        {
            for (int j = 0; j < h; j++)
            {
                for (int i = 0; i < w; i++)
                {
                    if (buttonsArray[i, j] == button)
                    {
                        RevealFields(button.xCoordinate, button.yCoordinate, button);
                    }
                }
            }
        }


        void RevealFields(int xCoord, int yCoord, ButtonPlus button)
        {
            Queue<ButtonPlus> queue = new Queue<ButtonPlus>();
            queue.Enqueue(button);
            button.enqued = true;
            while (queue.Count > 0)
            {
                ButtonPlus currentButton = queue.Dequeue();
                OpenCell(currentButton.xCoordinate, currentButton.yCoordinate, currentButton);
                openedCells++;
                if (BombCounter(currentButton.xCoordinate, currentButton.yCoordinate) == 0)
                {
                    for (int y = currentButton.yCoordinate - 1; y <= currentButton.yCoordinate + 1; y++)
                    {
                        for (int x = currentButton.xCoordinate - 1; x <= currentButton.xCoordinate + 1; x++)
                        {
                            if(x == currentButton.xCoordinate && y == currentButton.yCoordinate)
                            {
                                continue;
                            }
                            if (x >= 0 && x < w && y < h && y >= 0)
                            {
                                if (buttonsArray[x, y].enqued == false)
                                {
                                    queue.Enqueue(buttonsArray[x, y]);
                                    buttonsArray[x, y].enqued = true;
                                }
                            }
                        }
                    }
                }
            }
        }


        void OpenCell(int x, int y, ButtonPlus button)
        {
            int bombsAround = BombCounter(x, y);
            if (bombsAround == 0)
            {
                button.BackgroundImage = null;
                button.BackColor = Color.Gray;
                button.Paint += button_Paint2;
            }
            else
            {
                switch (bombsAround)
                {
                    case 1:
                        button.ForeColor = Color.Blue;
                        break;
                    case 2:
                        button.ForeColor = Color.Green;
                        break;
                    case 3:
                        button.ForeColor = Color.Red;
                        break;
                    case 4:
                        button.ForeColor = Color.DarkBlue;
                        break;
                    case 5:
                        button.ForeColor = Color.Brown;
                        break;
                }
                button.BackColor = Color.LightGray;
                if (button.BackgroundImage == null)
                {
                    button.Text = bombsAround.ToString();
                }
                else
                {
                    button.BackgroundImage = null;
                    button.Text = bombsAround.ToString();
                }
            }
        }


        //counting nearby bombs
        int BombCounter(int x, int y)
        {
            int counter = 0;
            for (int i = x - 1; i <= x + 1; i++)
            {
                for (int j = y - 1; j <= y + 1; j++)
                {
                    if (i >= 0 && i < w && j >= 0 && j < h)
                    {
                        if (buttonsArray[i, j].isBomb)
                        {
                            counter++;
                        }
                    }
                }
            }
            return counter;
        }


        //selecting field size
        private void x10ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            w = 10;
            h = 10;
            GenerateField();
        }

        private void x5ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            w = 5;
            h = 5;
            GenerateField();
        }
    }
}