using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Unit
{
    [CustomEditor(typeof(HexUnit), true)]
    public class TestHero : HexUnit
    {
        public float flyHight = 10;
        void Start()
        {
            visionRange = isFly ? 5 : 3;
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


        private Vector3 getFlyVector3(Vector3 position, float y)
        {
            if (y <= position.y)
            {
                return position;
            }
            return new Vector3(position.x, y, position.z);
        }

        //这里实现可以飞行的英雄的寻路
        //飞行英雄根据起点位置坐标飞行高度，飞往对应的cell，到达指定的cell上空下落到高于地面的位置 
        //永远在水平面以上
        //TODO 需要调整飞跃高山或者飞翔高山，高山飞翔平原降落的动画，需要修改飞往高山提前格子起飞
        public override IEnumerator TravelPath()
        {
            float flyPositionY = _pathToTravel[0].Position.y + flyHight;
            Vector3 a, b, c = _pathToTravel[0].Position;
            yield return LookAt(_pathToTravel[1].Position);

            if (!m_CurrentTravelLocation)
            {
                m_CurrentTravelLocation = _pathToTravel[0];
            }
            Grid.DecreaseVisibility(m_CurrentTravelLocation, visionRange);
            int currentColumn = m_CurrentTravelLocation.ColumnIndex;

            float t = Time.deltaTime * kTravelSpeed;
            for (int i = 1; i < _pathToTravel.Count; i++)
            {
                m_CurrentTravelLocation = _pathToTravel[i];
                a = c;
                b = _pathToTravel[i - 1].Position;

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
                Grid.IncreaseVisibility(_pathToTravel[i], visionRange);

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
                Grid.DecreaseVisibility(_pathToTravel[i], visionRange);
                t -= 1f;
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

            transform.localPosition = m_Location.Position + new Vector3(0, flyHight, 0);
            Orientation = transform.localRotation.eulerAngles.y;
            ListPool<HexCell>.Add(_pathToTravel);
            _pathToTravel = null;
        }
    }
}
