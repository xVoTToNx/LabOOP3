using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameSpace
{
    class TelePort : Cell
    {
        static protected Random random = new Random();
        protected int X;
        protected int Y;

        public TelePort(Types p1, int p2, int p3) : base(p1)
        {
            X = p2;
            Y = p3;
        }

        public override void UseMe(Directions key, Entity creature)
        {
            if(type == Types.TPR)
            {
                teleportRandom(X - 5, X + 5, Y - 5, Y + 5, creature as Hero);
            }
            else if(type == Types.TPG)
            {
                teleportRandom(0, Field.Width, 0, Field.Height, creature as Hero);
            }
        }

        private void teleportRandom(int startX, int endX, int startY, int endY, Hero hero)
        {
            if (startX < 0)
                startX = 0;
            if (startY < 0)
                startY = 0;
            if (endX >= Field.Width)
                endX = Field.Width - 1;
            if (endY >= Field.Height)
                endY = Field.Height - 1;

            int randX = random.Next(startX, endX);
            int randY = random.Next(startY, endY);

            while (Field.cell[randX, randY].type != Types.blank &&
                Field.cell[randX, randY].type != Types.food)
            {
                randX = random.Next(startX, endX);
                randY = random.Next(startY, endY);
            }
            Field.cell[hero.X, hero.Y] = new Blank(Types.blank, Field.cell[hero.X, hero.Y].isCrossroad,
                Field.cell[hero.X, hero.Y].dirs);



            Cell.ShowMe(hero.X, hero.Y, Types.blank);
            hero.X = randX;
            hero.Y = randY;
            Cell.ShowMe(hero.X, hero.Y, hero.type);

            Field.cell[X, Y] = new Blank(Types.blank, Field.cell[X, Y].isCrossroad, Field.cell[X, Y].dirs);
            Cell.ShowMe(X, Y, Types.blank);
        }
    }

    class TelePortConnected : TelePort
    {
        List<TelePortConnected> teleports;


        public TelePortConnected(Types p1, ref List<TelePortConnected> teleports, int p2, int p3) : base(p1,p2,p3)
        {
            this.teleports = teleports;
        }

        public override void UseMe(Directions key, Entity creature)
        {
            Hero hero = creature as Hero;
            int number = random.Next(0, teleports.Count);
            while (X == teleports[number].X &&
            Y == teleports[number].Y)
            {
                number = random.Next(0, teleports.Count);
            }
            Field.cell[hero.X, hero.Y] = new Blank(Types.blank, Field.cell[hero.X, hero.Y].isCrossroad,
                Field.cell[hero.X, hero.Y].dirs);
            Cell.ShowMe(hero.X, hero.Y, Types.blank);
            hero.X = teleports[number].X;
            hero.Y = teleports[number].Y;
            Cell.ShowMe(hero.X, hero.Y, Types.TPActivated);
        }
    }
}
