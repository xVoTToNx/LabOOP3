#define CONSOLE

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Media;
using Newtonsoft.Json;
using System.Threading;


#if CONSOLE
using ConsoleView;
#endif

namespace LabOOP1
{
    public delegate void View(int p1, int p2, Types p3);

    public enum Types
    {
        blank,
        heroNormal,
        heroEnerg,
        ghostNormal,
        ghostEnerg,
        wall,
        food,
        energizer
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
        protected const int millisecondsBeforeConfiguring = 1250;
        protected const int normalFPS = 6;
        protected const int increasingDeltaGhost = 200;
        protected const float delayDeltaGhost = 1.06f;

         public bool Kitty = true;

        protected  SoundPlayer player = new SoundPlayer();
        protected  bool music = true;

        protected  Player currentPlayer;
        protected  Skin currentSkin;

        protected  Hero hero;
        protected  Energizer[] energizers;
        protected  Ghost[] ghosts;

         void AI_n_Timers()
        {
            hero.energizerTimer(ghosts);
            for (int i = 0; i < ghosts.Length; i++)
            {
                if (Kitty)
                {
                    ghosts[i].GhostsTimer();
                    ghosts[i].ThinkGhost(hero);
                }
            }
        }

         void readKey(ConsoleKey playerKey)
        {

            if(playerKey == ConsoleKey.W || playerKey == ConsoleKey.UpArrow)
                    hero.MoveHero(Directions.up, ghosts, energizers);

            else if (playerKey == ConsoleKey.A || playerKey == ConsoleKey.LeftArrow)
                hero.MoveHero(Directions.left, ghosts, energizers);

            else if (playerKey == ConsoleKey.S || playerKey == ConsoleKey.DownArrow)
                hero.MoveHero(Directions.down, ghosts, energizers);

            else if (playerKey == ConsoleKey.D || playerKey == ConsoleKey.RightArrow)
                hero.MoveHero(Directions.right, ghosts, energizers);

            else if (playerKey == ConsoleKey.M)
            {
                music = !music;
                player.SoundLocation = currentSkin.Music;
                if (music)
                    player.PlayLooping();
                else
                    player.Stop();
            }
            else if (playerKey == ConsoleKey.Spacebar)
                if (hero.saver && !hero.Mod)
                {
                    hero.saver = false;
                    hero.StopWatch.Start();
                    hero.ChangeMod(true);
                    for (int j = 0; j < ghosts.Length; j++)
                    {
                        ghosts[j].ChangeMode(true);
                    }
                    Console.SetCursorPosition(12, Field.Height + 1);
                    Console.Write("       ");
                    hero.SaverWatch.Start();
                }
            }

        public virtual void GameOver()
        { }

        public virtual void YouWon()
        { }

        protected void action()
        {
            int frames = 0;
            int deltaGhost = currentPlayer.DeltaGhost;
            Stopwatch clock = new Stopwatch();
            clock.Start();
            bool flag = false;
            ConsoleKey playerKey = ConsoleKey.Execute;
            do
            {
                while (!Console.KeyAvailable && Kitty)
                {
                    if ((int)(frames % (deltaGhost * Ghost.speedModifier)) == 0)
                    {
                        DeltaTime.Update();
                        Console.SetCursorPosition(6, Field.Height);
                        Console.ForegroundColor = ConsoleColor.Green;
                        int FPS = DeltaTime.getFPS();
                        Console.Write(FPS + " ");
                        if (currentPlayer.Map == "Test.txt")
                        {
                            if (clock.ElapsedMilliseconds > millisecondsBeforeConfiguring)
                            {
                                flag = true;
                                clock.Stop();
                            }
                            if (flag)
                            {
                                if (FPS > normalFPS)
                                    currentPlayer.DeltaGhost += increasingDeltaGhost;
                                else if (FPS < normalFPS && currentPlayer.DeltaGhost > increasingDeltaGhost * normalFPS)
                                    currentPlayer.DeltaGhost -= increasingDeltaGhost;
                                Console.SetCursorPosition(9, Field.Height);
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.Write(deltaGhost + "      ");
                            }
                        }
                        AI_n_Timers();
                    }
                    else if ((int)(frames % (deltaGhost * Hero.speedModifier)) == 0 && playerKey != ConsoleKey.Execute)
                    {
                        readKey(playerKey);
                        if (playerKey == ConsoleKey.Home)
                            YouWon();
                        playerKey = ConsoleKey.Execute;
                    }
                    else if (frames == 100000)
                        frames = 0;
                    if (deltaGhost != currentPlayer.DeltaGhost && frames % (deltaGhost * delayDeltaGhost) == 0)
                            deltaGhost = currentPlayer.DeltaGhost;
                    frames++;
                }
                playerKey = Console.ReadKey(true).Key;
            } while (Kitty);
        }

        static void Main()
        {
            GameConsole game = new GameConsole();
            game.Start();
        }
    }
}
