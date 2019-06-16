using System;

namespace GameSpace
{
    public class Food : Cell
    {
        public Food(Types p1) : base(p1)
        {
        }

        public override void UseMe(Directions key, Entity creature)
        {
            Hero hero = creature as Hero;
            Hero.score++;
            Hero.food++;
            if (Field.MaxFood == Hero.food)
                game.YouWon();
            Field.PrintScore();
            Field.StepHero(key, hero);
        }
    }

    public class LandMineItem : Cell
    {
        public LandMineItem(Types p1) : base(p1)
        {
        }

        public override void UseMe(Directions key, Entity creature)
        {
            Hero hero = creature as Hero;
            Hero.mines++;
            Field.PrintMines();
            Field.StepHero(key, hero);
        }
    }


}
