﻿#define CONSOLE

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Media;
using Newtonsoft.Json;
using System.Threading;
using System.Windows.Forms;

using WinFormView;

namespace GameSpace
{
    public delegate void View(int p1, int p2, Types p3);
    public delegate void GetKey(ref int p1);

    public enum Types
    {
        blank,
        heroNormal,
        heroEnerg,
        ghostNormal,
        ghostEnerg,
        wall,
        food,
        energizer,
        landMineDanger,
        landMineNormal,
        landMineItem,
        TP1,
        TP2,
        TPR,
        TPG,
        TPActivated
    }

    public enum Directions
    {
        up,
        left,
        down,
        right
    }

    public enum Positions
    {
        left,
        right
    }

    public enum MenuTypes
    {
        mainMenu,
        optionMenu,
        otherMenu
    }

    public enum Coordinates
    {
        x,
        y
    }

    public class FourMoves
    {
        int[][] keys = new int[4][] {
            new int [2] {0, -1},
            new int [2] {-1, 0},
            new int [2] {0, 1},
            new int [2] {1, 0} };

        public int[] this[Directions key]
        {
            get
            {
                return keys[(int)key];
            }
        }

        public int Length = 4;

    }



    public class Game
    {
        protected GetKey getKey;
        protected int heroTime;
        protected int ghostTime;

        public bool isGameRunning = true;
        public int playerKey;

        public  SoundPlayer player = new SoundPlayer();
        public  bool music = true;

        public static  Player currentPlayer;
        public static Skin currentSkin;

        protected  Hero hero;
        protected  Energizer[] energizers;
        protected  Ghost[] ghosts;

         void AI_n_Timers()
        {
            hero.energizerTimer(ghosts);
            for (int i = 0; i < ghosts.Length; i++)
            {
                if (isGameRunning)
                {
                    ghosts[i].GhostsTimer();
                    ghosts[i].ThinkGhost(hero);
                }
            }
        }

        public virtual void GameOver()
        { }

        public virtual void YouWon()
        { }

        protected virtual void deleteBooster()
        { }

        public void readKey(int playerKey)
        {

            if (playerKey == (int)ConsoleKey.W || playerKey == (int)ConsoleKey.UpArrow)
                hero.MoveHero(Directions.up, ghosts, energizers);

            else if (playerKey == (int)ConsoleKey.A || playerKey == (int)ConsoleKey.LeftArrow)
                hero.MoveHero(Directions.left, ghosts, energizers);

            else if (playerKey == (int)ConsoleKey.S || playerKey == (int)ConsoleKey.DownArrow)
                hero.MoveHero(Directions.down, ghosts, energizers);

            else if (playerKey == (int)ConsoleKey.D || playerKey == (int)ConsoleKey.RightArrow)
                hero.MoveHero(Directions.right, ghosts, energizers);

            else if (playerKey == (int)ConsoleKey.E)
                hero.PlaceLandMine();

            else if (playerKey == (int)ConsoleKey.M)
            {
                music = !music;
                if (music)
                    player.PlayLooping();
                else
                    player.Stop();

            }
            else if (playerKey == (int)ConsoleKey.Spacebar)
                if (hero.booster && !hero.Mod)
                {
                    hero.booster = false;
                    hero.StopWatch.Start();
                    hero.ChangeMod(true);
                    for (int j = 0; j < ghosts.Length; j++)
                    {
                        ghosts[j].ChangeMode(true);
                    }
                    this.deleteBooster();
                    hero.BoosterWatch.Start();
                }
        }

        public void action()
        {
            int deltaGhost = currentPlayer.DeltaGhost;
            Stopwatch clock = new Stopwatch();
            TimeSpan ts;
            clock.Start();
            playerKey = 43;
            do
            {
                while (isGameRunning)
                {
                    ts = clock.Elapsed;
                    if(getKey != null)
                        getKey(ref playerKey);
                    if (ts.Milliseconds % (int)(ghostTime) == 0)
                    {
                        AI_n_Timers();
                        Thread.Sleep(5);
                    }
                    else if (ts.Milliseconds % (int)(heroTime * Hero.speedModifier) == 0 && playerKey != 43)
                    {
                        this.readKey((int)playerKey);
                        if (playerKey == (int)ConsoleKey.Home)
                            YouWon();
                        playerKey = 43;
                    }
                    if (ts.Minutes > 2)
                        clock.Restart();
                }
            } while (isGameRunning);
        }
    }
}
