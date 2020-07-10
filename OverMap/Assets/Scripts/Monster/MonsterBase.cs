using UnityEngine;

namespace Assets.Scripts.Monster
{
    public class MonsterBase : HexUnit
    {

        public GameObject monster;

        public void Start()
        {
            isCanSelect = false;
            // ValidateLocation();
        }
    }
}
