using Assets.Scripts.Hero;
using UnityEngine;

namespace Assets.Scripts.Monster
{
    public class MonsterBase : HexUnit
    {
        public void Start()
        {
            isCanSelect = false;
            isMonster = true;
            unitBase = new HeroBase
            {
                attack = 1,
                speed = 0,
                defend = 3,
                unit = this,
            };
        }

        public virtual void FindHeroCell()
        {
            if (this.unitBase.hp > 0)
            {
                Grid.FindPath(this.Location, HexGameUI.Instrance.selectedUnit.Location.GetNeighbor(HexDirection.E), this);
                this.Travel(Grid.GetPath());
            }

        }
    }
}
