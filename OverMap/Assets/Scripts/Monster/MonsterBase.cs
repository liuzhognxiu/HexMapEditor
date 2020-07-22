using Assets.Scripts.Hero;
using UnityEngine;

namespace Assets.Scripts.Monster
{
    public class MonsterBase : HexUnit
    {
        /// <summary>
        /// 每次行动的行动力
        /// </summary>
        public int strength = 3;
        public void Start()
        {
            isCanSelect = false;
            isMonster = true;
            strength = 3;
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
                if (!Location.GetIsNeighbor(HexGameUI.Instrance.selectedUnit.Location))
                {
                    Grid.FindPath(this.Location, GetToCell(HexGameUI.Instrance.selectedUnit.Location), this);
                    this.Travel(Grid.GetPath());
                }
                else
                {
                    Debug.Log("攻击英雄");
                    RoundManager.Instance.currentMonsterMoveOver = true;
                }
            }
        }

        //todo 需要添加判断是否在英雄周围，如果是在周围，直接返回周围的cell
        HexCell GetToCell(HexCell cell)
        {
            HexCell toCell = new HexCell { Index = -1 };
            int distance = cell.coordinates.DistanceTo(Location.coordinates);
            strength = strength < distance ? strength : distance - 1;
            for (int i = 0; i < strength; i++)
            {
                toCell = toCell.Index == -1 ? Location.GetNeighbor(MonsterGetHexDirection(cell, Location)) : toCell.GetNeighbor(MonsterGetHexDirection(cell, toCell));
            }
            return toCell;
        }

        HexCell GetToCell(HexCell cell, HexDirection direction)
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

        HexDirection MonsterGetHexDirection(HexCell cell, HexCell toCell)
        {
            if (toCell.coordinates.X == cell.coordinates.X)
            {
                if (toCell.coordinates.Y < cell.coordinates.Y)
                {
                    return HexDirection.NE;
                }
                else
                {
                    return HexDirection.SW;
                }
            }
            if (toCell.coordinates.X > cell.coordinates.X)
            {
                if (toCell.coordinates.Z == cell.coordinates.Z)
                {
                    return HexDirection.W;
                }
                else if (toCell.coordinates.Z > cell.coordinates.Z)
                {
                    return HexDirection.SW;
                }
                else
                {
                    return HexDirection.NW;
                }
            }
            if (toCell.coordinates.X < cell.coordinates.X)
            {
                if (toCell.coordinates.Z == cell.coordinates.Z)
                {
                    return HexDirection.E;
                }
                else if (Location.coordinates.Z > cell.coordinates.Z)
                {
                    return HexDirection.SE;
                }
                else
                {
                    return HexDirection.NE;
                }
            }
            return HexDirection.NE;
        }

        /// <summary>
        /// 判断怪物在玩家的什么方向上
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        HexDirection GetHexDirection(HexCell cell)
        {
            if (cell.coordinates.X == Location.coordinates.X)
            {
                if (cell.coordinates.Y < Location.coordinates.Y)
                {
                    return HexDirection.NE;
                }
                else
                {
                    return HexDirection.SW;
                }
            }
            if (cell.coordinates.X > Location.coordinates.X)
            {
                if (cell.coordinates.Z == Location.coordinates.Z)
                {
                    return HexDirection.W;
                }
                else if (cell.coordinates.Z > Location.coordinates.Z)
                {
                    return HexDirection.SW;
                }
                else
                {
                    return HexDirection.NW;
                }
            }
            if (cell.coordinates.X < Location.coordinates.X)
            {
                if (cell.coordinates.Z == Location.coordinates.Z)
                {
                    return HexDirection.E;
                }
                else if (cell.coordinates.Z > Location.coordinates.Z)
                {
                    return HexDirection.SE;
                }
                else
                {
                    return HexDirection.NE;
                }
            }
            return HexDirection.NE;
        }
    }
}
