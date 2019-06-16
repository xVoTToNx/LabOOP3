using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameSpace
{
    class LandMine : Cell
    {
        Energizer[] energizers;
        public LandMine(Types p1, Energizer[] energizers) : base(p1)
        {
            this.energizers = energizers;
        }

        public override void UseMe(Directions key, Entity creature)
        {

            if (type == Types.landMineNormal)
            {
                Ghost ghost = creature as Ghost;
                ghost.death(energizers);
            }
            else
            {
                game.GameOver();
            }
      
        }
    }
}
