using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Media;
using Newtonsoft.Json;
using System.Threading;

using GameSpace;

namespace WinFormView
{
    public partial class PacNyan : Form
    {
        static public int rowCount;
        static public int columnCount;
        public static Graphics graphics;
        SoundPlayer playerMenu;
        GameWinForm game;
        public Label scoreLabel = new Label();
        public Label minesLabel = new Label();
        Menu menu;
        Thread threadMain;
        bool nya = true;

        public PacNyan(Menu m, SoundPlayer pl)
        {
            menu = m;
            playerMenu = pl;
            InitializeComponent();
            this.Controls.Add(scoreLabel);
            this.Controls.Add(minesLabel);
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            game = new GameWinForm(this, menu, playerMenu);
            game.Start();
            game.gameCreation();
            MessageBox.Show("Press \"OK\" to start!");
            threadMain = new Thread(game.action);
            threadMain.Start();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            graphics = this.CreateGraphics();
            for(int i = 0; i < Field.Height; i++)
                for(int j = 0; j < Field.Width; j++)
                    Cell.ShowMe(j, i, Field.cell[j, i].type);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            int key = (int)e.KeyCode;

            game.playerKey = key;
        }

        private void PacNyan_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (game.isGameRunning == true)
            {
                game.player.SoundLocation = "music\\nggyu.wav";
                if (game.music)
                    game.player.PlayLooping();
                game.isGameRunning = false;
                MessageBox.Show("Your score: " + Hero.score);
                game.updateScore();

                game.player.Stop();

                playerMenu.PlayLooping();
                menu.Show();
            }
        }
    }


    public class GameWinForm : Game
    {
        PacNyan form;
        Menu menu;
        public static string map;
        public SoundPlayer playerMenu;
        const int normalGhostSpeed = 200;
        const int normalHeroSpeed = 150;

        public GameWinForm(PacNyan formm, Menu m, SoundPlayer pl)
        {
            form = formm;
            playerMenu = pl;
            menu = m;
        }

        public void Start()
        {
            form.scoreLabel.Text = "Your score: 0";
            form.scoreLabel.Dock = DockStyle.Bottom;

            form.minesLabel.Text = "Your mines: 0";
            form.minesLabel.Dock = DockStyle.Bottom;

            Cell.game = this;
            Field.game = this;
            ghostTime = normalGhostSpeed;
            heroTime = normalHeroSpeed;

            getKey = null;

            form.MaximizeBox = false;
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;

            Data.dictionary = new Dictionary<Types, string>{
                {Types.blank, "sprites\\blank.png"},
                {Types.heroNormal, "sprites\\heroNormal.png"},
                {Types.heroEnerg, "sprites\\heroEnerg.png"},
                {Types.ghostNormal,"sprites\\ghostNormal.png" },
                {Types.ghostEnerg, "sprites\\ghostEnerg.png"},
                {Types.wall, "sprites\\wall.png"},
                {Types.food, "sprites\\food.png"},
                {Types.energizer, "sprites\\energizer.png"},
                {Types.landMineNormal, "sprites\\landMineNormal.png"},
                {Types.landMineDanger, "sprites\\landMineDanger.png"},
                {Types.landMineItem, "sprites\\landMineItem.png"},
                {Types.TP1, "sprites\\tp1.png"},
                {Types.TPActivated, "sprites\\TPActivated.png"},
                {Types.TPR, "sprites\\tpR.png"},
                {Types.TPG, "sprites\\tpG.png"},
                {Types.TP2, "sprites\\tp2.png"}
            };
        }

        public void updateScore()
        {
            string[] scores = File.ReadAllLines("scores\\score.txt");
            for (int i = 0; i < scores.Length; i++)
            {
                if (string.Equals(currentPlayer.Name, scores[i].Split('-')[0]))
                {
                    string score = scores[i].Split('-')[1];
                    int scoreI = Int32.Parse(score);
                    if (Hero.score > scoreI)
                    {
                        scores[i] = currentPlayer.Name + "-" + Hero.score.ToString();
                        File.WriteAllLines("scores\\score.txt", scores);
                    }
                    return;
                }
            }
            string[] newScores = new string[scores.Length + 1];
            for (int ii = 0; ii < scores.Length; ii++)
            {
                newScores[ii] = scores[ii];
            }
            newScores[newScores.Length - 1] = currentPlayer.Name + "-" + Hero.score.ToString();
            File.WriteAllLines("scores\\score.txt", newScores);
        }

        public override void GameOver()
        {
            player.SoundLocation = "music\\nggyu.wav";
            if (music)
                player.PlayLooping();
            isGameRunning = false;
            MessageBox.Show("Your score: " + Hero.score);
            updateScore();

            player.Stop();

            playerMenu.PlayLooping();

            menu.Show();
            form.Close();
        }

        public void gameCreation()
        {

            isGameRunning = true;
            player.SoundLocation = currentSkin.Music;
            if (music)
                player.PlayLooping();
            string map = currentPlayer.Map;
            string[] lines = File.ReadAllLines(currentPlayer.Map, Encoding.UTF8);
            int energizersNumber = 0;
            int ghostsNumber = 0;
            int heroes = 0;

            foreach (string str in lines)
                foreach (char ch in str)
                {
                    if (ch == ' ')
                        Field.MaxFood++;
                    else if (ch == 'E')
                        energizersNumber++;
                    else if (ch == 'G')
                        ghostsNumber++;
                    if (ch == 'H')
                        heroes++;
                }
            energizers = new Energizer[energizersNumber];
            ghosts = new Ghost[ghostsNumber];

            Field.Width = lines[0].Length;
            Field.Height = lines.Length;
            Field.CreateField();
            PacNyan.columnCount = Field.Width;
            PacNyan.rowCount = Field.Height;

            form.Size = new Size(PacNyan.columnCount * 25 + 15, PacNyan.rowCount * 25 + 75);

            makingField(lines, currentSkin, energizersNumber, ghostsNumber, form);
        }

        void makingField(string[] lines, Skin symbols, int energizers_number, int ghosts_number,
            PacNyan form)
        {
            Cell.ShowMe = Data.WinFormRender;

            List<TelePortConnected> listTP1 = new List<TelePortConnected>();
            List<TelePortConnected> listTP2 = new List<TelePortConnected>();

            Field.PrintScore = delegate
            {
                if (Hero.score % 25 == 0)
                    form.scoreLabel.Text = "Your score: " + Hero.score;
            };

            Field.PrintMines = delegate
            {
                form.minesLabel.Text = "Your mines: " + Hero.mines;
            };

            for (int i = 0; i < Field.Height; i++)
            {
                if (lines[i].Length == Field.Width)
                {
                    for (int j = 0; j < Field.Width; j++)
                    {
                        if (lines[i][j] == '@')
                        {
                            Field.cell[j, i] = new Wall(Types.wall);
                        }
                        else if (lines[i][j] == ' ')
                        {

                            Field.cell[j, i] = new Food(Types.food);
                        }
                        else if (lines[i][j] == 'E')
                        {
                            Field.cell[j, i] = new Energizer(Types.energizer, j, i, ghosts);
                            energizers_number--;
                        }
                        else if (lines[i][j] == 'G')
                        {
                            Field.cell[j, i] = new Ghost(Types.ghostNormal, j, i);
                            ghosts_number--;
                            ghosts[ghosts_number] = (Ghost)Field.cell[j, i];
                        }
                        else if (lines[i][j] == 'H')
                        {
                            Field.cell[j, i] = new Hero(Types.heroNormal, j, i, energizers);
                            hero = (Hero)Field.cell[j, i];
                        }
                        else if (lines[i][j] == 'b')
                        {
                            Field.cell[j, i] = new Blank(Types.blank);
                        }
                        else if (lines[i][j] == 'L')
                        {
                            Field.cell[j, i] = new LandMine(Types.landMineDanger, energizers);
                        }
                        else if (lines[i][j] == 'M')
                        {
                            Field.cell[j, i] = new LandMineItem(Types.landMineItem);
                        }
                        else if (lines[i][j] == '1')
                        {
                            Field.cell[j, i] = new TelePortConnected(Types.TP1, ref listTP1, j, i);
                            listTP1.Add(Field.cell[j, i] as TelePortConnected);
                        }
                        else if (lines[i][j] == '2')
                        {
                            Field.cell[j, i] = new TelePortConnected(Types.TP2, ref listTP2, j, i);
                            listTP2.Add(Field.cell[j, i] as TelePortConnected);
                        }
                        else if (lines[i][j] == 'R')
                        {
                            Field.cell[j, i] = new TelePort(Types.TPR, j, i);
                        }
                        else if (lines[i][j] == 'O')
                        {
                            Field.cell[j, i] = new TelePort(Types.TPG, j, i);
                        }
                        else
                        {
                            Field.cell[j, i] = new Wall(Types.wall);
                        }
                        if (lines[i][j] != '@')
                        {
                            List<Directions> roads = new List<Directions>();
                            if (lines[i - 1][j] != '@')
                                roads.Add(Directions.up);
                            if (lines[i][j - 1] != '@')
                                roads.Add(Directions.left);
                            if (lines[i + 1][j] != '@')
                                roads.Add(Directions.down);
                            if (lines[i][j + 1] != '@')
                                roads.Add(Directions.right);
                            if (roads.Count >= 3)
                            {
                                if (lines[i][j] == 'G')
                                {
                                    Ghost ghost = (Ghost)Field.cell[j, i];


                                    ghost.undercell.isCrossroad = true;
                                    ghost.undercell.dirs = roads;
                                }
                                else
                                {
                                    Field.cell[j, i].isCrossroad = true;
                                    Field.cell[j, i].dirs = roads;
                                }
                            }
                        }
                    }
                }
            }
            player.SoundLocation = "music\\megalovania.wav";
            if (music)
                player.PlayLooping();
        }

    }

    public static class Data
    {
        public static Dictionary<Types, string> dictionary;
        static Image img;
        public static void WinFormRender(int X, int Y, Types type)
        {
            img = new Bitmap(dictionary[type]);
            if (PacNyan.graphics != null)
                PacNyan.graphics.DrawImage(img, X * 25, Y * 25, 25, 25);
        }
    }
}
