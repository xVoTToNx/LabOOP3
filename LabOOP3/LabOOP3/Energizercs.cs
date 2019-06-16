using System;

namespace GameSpace
{
    public class Energizer : Cell
    {
        public bool isAlive = true;
        public int X;
        public int Y;
        Ghost[] ghosts;

        public Energizer(Types p1, int p4, int p5, Ghost[] ghosts) : base(p1)
        {
            X = p4;
            this.ghosts = ghosts;
            Y = p5;
        }

        public override void UseMe(Directions key, Entity creature)
        {
            Hero hero = creature as Hero;
            isAlive = false;
            Hero.score += 10;
            Field.PrintScore();
            Field.StepHero(key, hero);
            if (hero.StopWatch.IsRunning)
            {
                hero.StopWatch.Restart();
                hero.ChangeMod(true);
            }
            else
            {
                hero.StopWatch.Start();
                hero.ChangeMod(true);
                for (int j = 0; j < ghosts.Length; j++)
                {
                    ghosts[j].ChangeMode(true);
                }
            }
        }
    }
}
