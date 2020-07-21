using Assets.Scripts.Hero;
using UnityEngine;

namespace Assets.Scripts.Monster
{
    public class MonsterBase : HexUnit
    {
        /// <summary>
        /// 每次行动的行动力
        /// </summary>
        public int strength = 6;
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
                Grid.FindPath(this.Location, GetToCell(HexGameUI.Instrance.selectedUnit.Location), this);
                this.Travel(Grid.GetPath());
            }
        }

        HexCell GetToCell(HexCell cell)
        {
            HexCell toCell = new HexCell();
            for (int i = 0; i < (int)HexDirection.NW; i++)
            {
                if (cell.GetNeighbor((HexDirection)i) != null && cell.GetNeighbor((HexDirection)i).Unit == null)
                {
                    toCell = cell.GetNeighbor((HexDirection)i);
                }
            }

            if (toCell == null)
            {
                toCell = GetToCell(HexGameUI.Instrance.selectedUnit.Location.GetNeighbor(HexDirection.NE));
            }
            return toCell;
        }
    }
}
