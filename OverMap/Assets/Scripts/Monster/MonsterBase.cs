using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Hero;
using UnityEngine;

namespace Assets.Scripts.Monster
{
    public class MonsterBase : HexUnit
    {
        /// <summary>
        /// 每次行动的行动力
        /// </summary>
        public void Start()
        {
            isCanSelect = false;
            isMonster = true;
            speed = 4;
            unitBase = new HeroBase
            {
                attack = 1,
                speed = 0,
                defend = 3,
                unit = this,
            };
        }

        readonly WaitForSeconds m_Seconds = new WaitForSeconds(0.2f);
        public IEnumerator Attack(HeroBase heroBase)
        {
            yield return m_Seconds;
            heroBase.hp -= (unitBase.attack > heroBase.defend) ? unitBase.attack - heroBase.defend : 0;
            Debug.Log("英雄所剩血量：" + heroBase.hp);
            RoundManager.Instance.currentMonsterMoveOver = true;
        }


        public virtual void FindHeroCell()
        {
            if (this.unitBase.hp > 0)
            {
                if (!Location.GetIsNeighbor(HexGameUI.Instrance.selectedUnit.Location))
                {
                    Grid.FindPath(this.Location, GetToCell(HexGameUI.Instrance.selectedUnit.Location), this);
                    this.Travel(Grid.GetPath());
                    if (HexGameUI.Instrance.selectedUnit.Location.coordinates.DistanceTo(GetToCell(HexGameUI.Instrance.selectedUnit.Location).coordinates) < speed)
                    {
                        this.PathfindOverBack = () => { StartCoroutine(Attack(HexGameUI.Instrance.selectedUnit.unitBase)); };
                    }
                }
                else
                {
                    StartCoroutine(Attack(HexGameUI.Instrance.selectedUnit.unitBase));
                }
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

        HexDirection MonsterGetHexDirection(HexCell cell, HexCell toCell)
        {
            if (toCell.coordinates.X == cell.coordinates.X)
            {
                if (toCell.coordinates.Y < cell.coordinates.Y)
                {
                    return HexDirection.SW;
                }
                else
                {
                    return HexDirection.NE;
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
