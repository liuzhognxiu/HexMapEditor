using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Hero;
using Assets.Scripts.Monster;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Assets.Scripts.Unit
{
#if UNITY_EDITOR
    [CustomEditor(typeof(HexUnit), true)]
#endif
    public class PlayerHero : HexUnit
    {
        public float flyHight = 5;

        public bool isMoveOver = false;

        public TextMesh text;

        void Start()
        {
            unitBase = new HeroBase
            {
                unit = this,
            };
            speed = unitBase.speed;
            visionRange = isFly ? 5 : 3;

            PathfindOverBack = OverPath;
            if (text)
            {
                text.text = Location.Index.ToString();
            }
        }

        private void OverPath()
        {
            Debug.Log("英雄怒吼！！！！！！！！！");
            CorrectCell();
            isMoveOver = true;
        }

        void CorrectCell()
        {
            Location.Buff = null;
            Location.DisableHighlight();
            Location.TerrainTypeIndex = 0;
            transform.localPosition = Bezier.GetPoint(Location.transform.position, Location.transform.position, Location.transform.position, 1);
        }

        private IEnumerator travel;
        public override void Travel(List<HexCell> path)
        {
            pathToTravel = path;
            StopAllCoroutines();
            travel = TravelPath();
            StartCoroutine(travel);
        }

        readonly WaitForSeconds m_Seconds = new WaitForSeconds(0.2f);
        public IEnumerator Attack(HeroBase monster)
        {
            yield return m_Seconds;
            monster.hp -= (unitBase.attack > monster.defend) ? unitBase.attack - monster.defend : 0;
            Debug.Log("英雄所剩血量：" + unitBase.hp);
            Debug.Log("怪物所剩血量：" + monster.hp);

            if (monster.hp > 0)
            {
                Debug.Log("暂停本次寻路");
                StopCoroutine(travel);
                Grid.UnEnableHight();
                Grid.showhexCells.Clear();
                attackHexUnits.Clear();
                CorrectCell();
            }
        }


        /// <summary>
        /// 根据起飞的高度，判断当前飞行是否需要上升
        /// </summary>
        /// <param name="position"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private Vector3 GetFlyVector3(Vector3 position, float y)
        {
            if (this.Location.IsUnderwater)
            {
                return new Vector3(position.x, 5, position.z);
            }
            if (y < position.y)
            {
                return position;
            }
            return new Vector3(position.x, y, position.z);
        }


        public override IEnumerator TravelPath()
        {
            float flyPositionY = pathToTravel[0].Position.y + flyHight;
            Vector3 a, b, c = pathToTravel[0].Position;
            yield return LookAt(pathToTravel[1].Position);

            if (!m_CurrentTravelLocation)
            {
                m_CurrentTravelLocation = pathToTravel[0];
                RefreshCell(m_CurrentTravelLocation);
            }

            int currentColumn = m_CurrentTravelLocation.ColumnIndex;

            float t = Time.deltaTime * kTravelSpeed;
            for (int i = 1; i < pathToTravel.Count; i++)
            {
                m_CurrentTravelLocation = pathToTravel[i];
                a = c;
                b = pathToTravel[i - 1].Position;

                //每次寻路之前检测下一个目标点是否有怪物，如果有的话，先攻击，攻击未杀死怪物暂停寻路
                if (pathToTravel[i].Unit != null && pathToTravel[i].Unit.isMonster)
                {
                    if (attackHexUnits.Contains(pathToTravel[i].Unit))
                    {
                        attackHexUnits.Remove(pathToTravel[i].Unit);
                    }
                    yield return Attack(pathToTravel[i].Unit.unitBase);
                }

                pathToTravel[i - 1].Unit = null;
                AddBuff(pathToTravel[i]);
                int nextColumn = m_CurrentTravelLocation.ColumnIndex;
                if (currentColumn != nextColumn)
                {
                    if (nextColumn < currentColumn - 1)
                    {
                        a.x -= HexMetrics.innerDiameter * HexMetrics.wrapSize;
                        b.x -= HexMetrics.innerDiameter * HexMetrics.wrapSize;
                    }
                    else if (nextColumn > currentColumn + 1)
                    {
                        a.x += HexMetrics.innerDiameter * HexMetrics.wrapSize;
                        b.x += HexMetrics.innerDiameter * HexMetrics.wrapSize;
                    }
                    Grid.MakeChildOfColumn(transform, nextColumn);
                    currentColumn = nextColumn;
                }

                c = (b + m_CurrentTravelLocation.Position) * 0.5f;
                for (; t < 1f; t += Time.deltaTime * kTravelSpeed)
                {
                    a = GetFlyVector3(a, flyPositionY);
                    b = GetFlyVector3(b, flyPositionY);
                    c = GetFlyVector3(c, flyPositionY);
                    transform.localPosition = Bezier.GetPoint(a, b, c, t);
                    Vector3 d = Bezier.GetDerivative(a, b, c, t);
                    d.y = 0f;
                    transform.localRotation = Quaternion.LookRotation(d);
                    yield return null;
                }
                t -= 1f;

                RefreshCell(pathToTravel[i - 1]);
            }

            // if (!pathToTravel.Last().Unit.isMonster && attackHexUnits != null && attackHexUnits.Count > 0)
            // {
            //     yield return Attack(attackHexUnits.Last().unitBase);
            // }
            if (attackHexUnits != null && attackHexUnits.Count > 0)
            {
                yield return Attack(attackHexUnits.Last().unitBase);
            }

            m_CurrentTravelLocation = null;

            a = c;
            b = m_Location.Position;
            c = b;
            Grid.IncreaseVisibility(m_Location, visionRange);
            for (; t < 1f; t += Time.deltaTime * kTravelSpeed)
            {
                a = GetFlyVector3(a, flyPositionY);
                b = GetFlyVector3(b, flyPositionY);
                c = GetFlyVector3(c, flyPositionY);
                transform.localPosition = Bezier.GetPoint(a, b, c, t);
                Vector3 d = Bezier.GetDerivative(a, b, c, t);
                d.y = 0f;
                transform.localRotation = Quaternion.LookRotation(d);
                yield return null;
            }

            //如果cell是在水平面一下，飞翔的英雄保持再水平面以上的飞行高度
            if (Location.IsUnderwater && isFly)
            {
                transform.localPosition = new Vector3(m_Location.Position.x, HexMetrics.kWaterPositionY + flyHight, m_Location.Position.z);
            }
            else
            {
                transform.localPosition = m_Location.Position + new Vector3(0, flyHight, 0);
            }
            Orientation = transform.localRotation.eulerAngles.y;
            attackHexUnits?.Clear();
            ListPool<HexCell>.Add(pathToTravel);
            PathfindOverBack.Invoke();
            pathToTravel = null;
        }


        void AddBuff(HexCell cell)
        {
            cell.Buff?.OnEnter();
            m_Location = cell;
        }
    }
}
