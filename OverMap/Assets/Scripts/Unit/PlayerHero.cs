﻿using System.Collections;
using System.Collections.Generic;
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


        void Start()
        {
            unitBase = new HeroBase
            {
                unit = this,
            };
            speed = unitBase.speed;
            visionRange = isFly ? 5 : 3;

            PathfindOverBack = OverPath;
        }

        private void OverPath()
        {
            for (int i = 0; i < attackHexUnits.Count; i++)
            {
                if (attackHexUnits[i] != null && attackHexUnits[i].unitBase != null)
                {
                    Attack(attackHexUnits[i].unitBase);
                }
            }
            // if (attackHexUnit != null && attackHexUnit.unitBase != null)
            // {
            //     Attack(attackHexUnit.unitBase);
            // }
            Debug.Log("英雄怒吼！！！！！！！！！");
        }

        public override void Travel(List<HexCell> path)
        {
            m_Location.Unit = null;
            m_Location = path[path.Count - 1];
            m_Location.Unit = this;
            _pathToTravel = path;
            StopAllCoroutines();
            StartCoroutine(TravelPath());
        }

        public override void Attack(HeroBase monster)
        {
            while (monster.hp > 0)
            {
                unitBase.hp -= (monster.attack > unitBase.defend) ? monster.attack - unitBase.defend : 0;
                monster.hp -= (unitBase.attack > monster.defend) ? unitBase.attack - monster.defend : 0;
                Debug.Log("英雄所剩血量：" + unitBase.hp);
                Debug.Log("怪物所剩血量：" + monster.hp);

            }
            if (monster.hp <= 0)
            {
                Grid.RemoveUnit(monster.unit);
            }
        }


        /// <summary>
        /// 根据起飞的高度，判断当前风行是否需要上升
        /// </summary>
        /// <param name="position"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private Vector3 getFlyVector3(Vector3 position, float y)
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
            float flyPositionY = _pathToTravel[0].Position.y + flyHight;
            Vector3 a, b, c = _pathToTravel[0].Position;
            yield return LookAt(_pathToTravel[1].Position);

            if (!m_CurrentTravelLocation)
            {
                m_CurrentTravelLocation = _pathToTravel[0];
                //走过的路程点
                RefreshCell(m_CurrentTravelLocation);
            }
            // Grid.DecreaseVisibility(m_CurrentTravelLocation, visionRange);
            int currentColumn = m_CurrentTravelLocation.ColumnIndex;

            float t = Time.deltaTime * kTravelSpeed;
            for (int i = 1; i < _pathToTravel.Count; i++)
            {
                m_CurrentTravelLocation = _pathToTravel[i];
                a = c;
                b = _pathToTravel[i - 1].Position;
                AddBuff(_pathToTravel[i]);
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
                // Grid.IncreaseVisibility(_pathToTravel[i], visionRange);

                for (; t < 1f; t += Time.deltaTime * kTravelSpeed)
                {
                    a = getFlyVector3(a, flyPositionY);
                    b = getFlyVector3(b, flyPositionY);
                    c = getFlyVector3(c, flyPositionY);
                    transform.localPosition = Bezier.GetPoint(a, b, c, t);
                    Vector3 d = Bezier.GetDerivative(a, b, c, t);
                    d.y = 0f;
                    transform.localRotation = Quaternion.LookRotation(d);
                    yield return null;
                }
                // Grid.DecreaseVisibility(_pathToTravel[i], visionRange);
                t -= 1f;
                RefreshCell(_pathToTravel[i]);
            }


            m_CurrentTravelLocation = null;

            a = c;
            b = m_Location.Position;
            c = b;
            Grid.IncreaseVisibility(m_Location, visionRange);
            for (; t < 1f; t += Time.deltaTime * kTravelSpeed)
            {
                a = getFlyVector3(a, flyPositionY);
                b = getFlyVector3(b, flyPositionY);
                c = getFlyVector3(c, flyPositionY);
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
            Grid.UnEnableHight();

            ListPool<HexCell>.Add(_pathToTravel);
            PathfindOverBack.Invoke();
            _pathToTravel = null;
        }

        void RefreshCell(HexCell cell)
        {
            cell.DisableHighlight();
            cell.Buff = HexMapGenerator.Instrance.getBuffBase(cell);
            cell.TerrainTypeIndex = HexMapGenerator.Instrance.GetTerrain(cell);
        }

        void AddBuff(HexCell cell)
        {
            cell.Buff?.OnEnter();
        }
    }
}