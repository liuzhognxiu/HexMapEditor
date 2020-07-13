using Assets.Scripts.Hero;
using UnityEngine;

namespace Assets.Scripts.Monster
{
    public class MonsterBase : HexUnit
    {

        public GameObject monster;

        public void Start()
        {
            isCanSelect = false;
            isMonster = true;
            unitBase = new HeroBase
            {
                attack = 1,
                speed = 0,
                def = 3,
                unit  = this,
            };
        }
    }
}
