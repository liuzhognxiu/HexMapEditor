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
            HexDirection hexDirection = GetHexDirection(cell);
            if (cell.GetNeighbor(hexDirection) != null && cell.GetNeighbor(hexDirection).Unit == null)
            {
                return cell.GetNeighbor(hexDirection);
            }

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

        /// <summary>
        /// 判断怪物在玩家的什么方向上
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        HexDirection GetHexDirection(HexCell cell)
        {
            if (cell.coordinates.X == HexGameUI.Instrance.selectedUnit.Location.coordinates.X)
            {
                if (cell.coordinates.Y > HexGameUI.Instrance.selectedUnit.Location.coordinates.Y)
                {
                    return HexDirection.NE;
                }
                else
                {
                    return HexDirection.SW;
                }
            }
            if (cell.coordinates.X < HexGameUI.Instrance.selectedUnit.Location.coordinates.X)
            {
                if (cell.coordinates.Z == HexGameUI.Instrance.selectedUnit.Location.coordinates.Z)
                {
                    return HexDirection.W;
                }
                else if (cell.coordinates.Z > HexGameUI.Instrance.selectedUnit.Location.coordinates.Z)
                {
                    return HexDirection.NW;
                }
                else
                {
                    return HexDirection.SW;
                }
            }
            if (cell.coordinates.X > HexGameUI.Instrance.selectedUnit.Location.coordinates.X)
            {
                if (cell.coordinates.Z == HexGameUI.Instrance.selectedUnit.Location.coordinates.Z)
                {
                    return HexDirection.E;
                }
                else if (cell.coordinates.Z > HexGameUI.Instrance.selectedUnit.Location.coordinates.Z)
                {
                    return HexDirection.NE;
                }
                else
                {
                    return HexDirection.SE;
                }
            }
            return HexDirection.NE;
        }
    }
}
