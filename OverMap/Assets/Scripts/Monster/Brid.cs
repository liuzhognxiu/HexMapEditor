using UnityEngine;

namespace Assets.Scripts.Monster
{
    public class Brid : MonsterBase
    {
        public TextMesh text;
        public int index;

        void Start()
        {
            isFly = true;
            base.Start();

            if (text)
            {
                text.text = Location.Index.ToString();
            }

            index = Location.Index;
        }

        void Update()
        {
            if (this.unitBase.hp <= 0)
            {
                this.Die();
            }
        }

    }
}
