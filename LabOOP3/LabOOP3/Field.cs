﻿using System;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameSpace
{
    public class Field
    {
        public delegate void printingScore();
        static public printingScore PrintScore;
        static public printingScore PrintMines;
        static public Label Scorelabel;
        static public Label MinesLabel;
        static public Game game;
        static public int Width;
        static public int Height;
        static public int MaxFood;
        static public Cell[,] cell;

        static public void CreateField()
        {
            cell = new Cell[Width, Height];
        }

        static public void ReWrite()
        {
            for (int i = 0; i < Height; i++)
                for (int j = 0; j < Width; j++)
                    Cell.ShowMe(j, i, Field.cell[j,i].type);
        }

        static public void StepHero(Directions key, Entity entity)
        {
            if (game.isGameRunning)
            {
                if (cell[entity.X, entity.Y].type != Types.TP1 &&
                    cell[entity.X, entity.Y].type != Types.TP2)
                {
                    cell[entity.X, entity.Y] = new Blank(Types.blank,
                    cell[entity.X, entity.Y].isCrossroad, cell[entity.X, entity.Y].dirs);
                }
                Cell.ShowMe(entity.X, entity.Y, cell[entity.X, entity.Y].type);
                entity.X += Entity.Moves[key][0];
                entity.Y += Entity.Moves[key][1];

                cell[entity.X, entity.Y] = entity;
                Cell.ShowMe(entity.X, entity.Y, entity.type);
            }
        }
        static public void StepGhost(Directions key, Entity entity)
        { 
            if (game.isGameRunning)
            {
                Ghost ghost = (Ghost)entity;
                cell[ghost.X, ghost.Y] = ghost.undercell;

                 Cell.ShowMe(entity.X, entity.Y, cell[entity.X, entity.Y].type);
                entity.X += Entity.Moves[key][0];
                entity.Y += Entity.Moves[key][1];

                ghost.undercell = cell[ghost.X, ghost.Y];
                cell[ghost.X, ghost.Y] = ghost;

                cell[ghost.X, ghost.Y].isCrossroad = ghost.undercell.isCrossroad;
                cell[ghost.X, ghost.Y].dirs = ghost.undercell.dirs;
                Cell.ShowMe(entity.X, entity.Y, entity.type);
            }
        }
    }
}
