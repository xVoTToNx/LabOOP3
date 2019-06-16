using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;

namespace WinFormView
{
    public partial class Menu : Form
    {
        SoundPlayer player = new SoundPlayer();
        public Menu()
        {
            InitializeComponent();
        }

        private void Menu_Load(object sender, EventArgs e)
        {
            player.SoundLocation = "music\\nyan.wav";
            player.PlayLooping();
            Settings settings = new Settings(this, "Choose Player", "players");

            this.Hide();
            settings.ShowDialog();
        }

        private void Play_Click(object sender, EventArgs e)
        {
            PacNyan gameForm = new PacNyan(this, player);

            player.Stop();
       
            this.Hide();
            gameForm.Show();
        }

        private void ChangeMap_Click(object sender, EventArgs e)
        {
            Settings settings = new Settings(this, "Choose Map", "map");

            this.Hide();
            settings.Show();
        }

        private void Exit_Click(object sender, EventArgs e)
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

        private void ChangePlayer_Click(object sender, EventArgs e)
        {
            Settings settings = new Settings(this, "Choose Player", "players");

            this.Hide();
            settings.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Settings settings = new Settings(this, "High Scores", "scores");

            this.Hide();
            settings.Show();
        }
    }
}
