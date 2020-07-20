using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Hero
{
    public class HeroBase : Base
    {
        public int attack = 101;

        public int defend = 3;

        public int speed = 24;

        public int hp = 100;

        public HexUnit unit;
    }
}
