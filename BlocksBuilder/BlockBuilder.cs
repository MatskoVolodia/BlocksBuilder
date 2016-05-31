using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

/// <summary>
/// Hello, guys
/// As you can see it's my simple analog of 
/// Symbian game "Tower Bloxx" (aaaaaaaaaaaalmost analog, you know;)
/// Enjoy!
/// </summary>

namespace BlocksBuilder
{
    public partial class BlocksBuilder : Form
    {
        GameLogic newGame = new GameLogic();
        public event Action NewLevel;
        private List<PictureBox> blocks = new List<PictureBox>();
        private int scores = 0;
        private int highScore = 0;
        AboutBox1 about = new AboutBox1();

        public BlocksBuilder()
        {
            InitializeComponent();
            System.IO.TextReader highFile = new System.IO.StreamReader("high.v");
            this.Icon = new Icon("box_blue.ico");
            highScore = int.Parse(highFile.ReadLine());
            highLabel.Text = "High Score: " + highScore;
            DoubleBuffered = true;
            newGame.GameOver += () =>
            {
                UpdateScores();
                scores = 0;          
                CleanAll();
                newGame.CleanAll();
                newGame.ResetSpeed();
                MessageBox.Show("Game over");
            };

            // So, here we cut those side of block which is out of previous block
            newGame.CutItDude += (x, y) =>
            {
                if (y == true)
                {
                    blocks[blocks.Count - 1].Size =
                        new Size(blocks[blocks.Count - 1].Size.Width - x, blocks[blocks.Count - 1].Size.Height);
                } else
                {
                    blocks[blocks.Count - 1].Size =
                        new Size(blocks[blocks.Count - 1].Size.Width - x, blocks[blocks.Count - 1].Size.Height);
                    blocks[blocks.Count - 1].Location =
                        new Point(blocks[blocks.Count - 1].Location.X + x, blocks[blocks.Count - 1].Location.Y);
                }
            };
            NewLevel += () =>
            {
                // when score reaches (% 16 == 0) all blocks are removed from screen, but speed raises a bit
                newGame.RaiseSpeed();
                newGame.CleanAll();
                CleanAll();
                CreateNewBlock();
            };
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateScores();
            CleanAll();
            newGame.CleanAll();
            newGame.ResetSpeed();
            scores = 0;
            yourLabel.Text = "Your Score: " + scores.ToString();
            CreateNewBlock();
        }

        private void CreateNewBlock()
        {
            blocks.Add(new PictureBox()
            {
                Location = new Point(newGame.LeftBorder, newGame.CurrentBlockLevel),
                Size = new Size(newGame.CurrentBlockWidth, newGame.BlockHeight)        
            });

            blocks[blocks.Count - 1].Image = (Image)new Bitmap(newGame.CurrentBlockWidth, newGame.BlockHeight);
            using (Graphics g = Graphics.FromImage(blocks[blocks.Count - 1].Image))
            {
                g.FillRectangle(new SolidBrush(Color.RoyalBlue), 0, 0, newGame.CurrentBlockWidth, newGame.BlockHeight);
            }
            blocks[blocks.Count - 1].Invalidate();
            Controls.Add(blocks[blocks.Count - 1]);
        }
        
        private void mainTimer_Tick(object sender, EventArgs e)
        {
            if (blocks.Count == 0) return;
            newGame.DoMove();
            // handmade data binding between class data and control ;)
            blocks[blocks.Count - 1].Location = new Point(newGame.CurrentBlockCoordinates, newGame.CurrentBlockLevel);
        }

        private void CleanAll()
        {
            for (int i = 0; i < blocks.Count; i++)
            {
                blocks[i].Visible = false;
            }
            blocks = null;
            blocks = new List<PictureBox>();
        }

        private void BlocksBuilder_KeyDown(object sender, KeyEventArgs e)
        {
            if (blocks.Count == 0) return;
            if (e.KeyCode == Keys.Space)
            {
                bool temp = newGame.GetUpdate(blocks[blocks.Count - 1].Location.X);
                if (!temp) return;   
                CreateNewBlock();
                scores++;
                if (scores % 16 == 0) NewLevel();
                yourLabel.Text = "Your Score: " + scores.ToString();
            }          
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            about = new AboutBox1();
            about.Show();
        }

        private void UpdateScores()
        {           
            if (scores > highScore)
            {
                highScore = scores;
                highLabel.Text = "High Score: " + highScore;
                System.IO.TextWriter writeHigh = new System.IO.StreamWriter("high.v");
                writeHigh.WriteLine(highScore);
                writeHigh.Close();
            }
        }
    }
}
