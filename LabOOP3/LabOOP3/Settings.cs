using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

using GameSpace;

namespace WinFormView
{
    public partial class Settings : Form
    {
        Menu form;
        string filePath;
        string text;

        public Settings(Menu f, string str, string path)
        {
            form = f;
            text = str;
            filePath = path;
            InitializeComponent();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = listBox1.SelectedIndex;
            switch(filePath)
            {
                case "map":
                    GameWinForm.currentPlayer.Map = Directory.GetFiles(filePath)[index];
                    GameWinForm.currentPlayer.Save();
                    break;
                case "players":
                    GameWinForm.currentPlayer = JsonConvert.DeserializeObject<Player>
                (File.ReadAllText(Directory.GetFiles(filePath)[index]));
                    GameWinForm.currentPlayer.Save();
                    break;
            }
            this.Close();
            form.Show();
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            if (filePath != "scores")
            {
                string[] files = Directory.GetFiles(filePath);
                for (int i = 0; i < files.Length; i++)
                {
                    this.listBox1.Items.Insert(i, files[i].Split('\\')[1].Split('.')[0]);
                }
            }
            else
            {
                string[] files = File.ReadAllLines(filePath + "\\score.txt");
                for (int i = 0; i < files.Length; i++)
                {
                    this.listBox1.Items.Insert(i, files[i]);
                }
            }
            this.label1.Text = text;
        }

        private void Settings_FormClosed(object sender, FormClosedEventArgs e)
        {
            if(Game.currentPlayer.Map != null)
                form.Show();
            else
            {
                if (Application.MessageLoop)
                {
                    Application.Exit();
                }
                else
                {
                    Environment.Exit(1);
                }
            }
        }
    }
}
