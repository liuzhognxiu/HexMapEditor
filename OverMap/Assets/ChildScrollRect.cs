using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    public sealed class ChildScrollRect : ScrollRect
    {
        /// <summary>
        /// 手势移动方向
        /// </summary>
        public enum MoveDirection
        {
            Left = 0,
            Up = 1,
            Right = 2,
            Down = 3,
            None = 4
        }

        /// <summary>
        /// 允许的拖拽方向
        /// </summary>
        public enum DragDirection
        {
            Horizontal,
            Vertical,
            All,
        }

        // ======= 重写Inspector显示以下属性 ======
        [SerializeField]
        private ScrollRect m_Parent = null;
        [SerializeField]
        private DragDirection m_DragDir = DragDirection.Horizontal;
        // ======= 重写Inspector显示以上属性 ======

        private bool m_IsDistaching = false;

        protected override void OnEnable()
        {
            m_IsDistaching = false;
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (DispatchEvent(eventData))
                {
                    m_IsDistaching = true;
                    m_Parent.OnBeginDrag(eventData);
                }
                else
                {
                    m_IsDistaching = false;
                    base.OnBeginDrag(eventData);
                }
            }
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (m_IsDistaching)
            {
                m_Parent.OnDrag(eventData);
            }
            else
            {
                base.OnDrag(eventData);
            }
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            if (m_IsDistaching)
            {
                m_Parent.OnEndDrag(eventData);
                m_IsDistaching = false;
            }
            else
            {
                base.OnEndDrag(eventData);
            }
        }

        /// <summary>
        /// 是否应该转发本次UI事件至父级
        /// </summary>
        private bool DispatchEvent(PointerEventData eventData)
        {
            var delta = eventData.delta;
            var moveDir = DetermineMoveDirection(delta.x, delta.y, 0.6f);
            switch (m_DragDir)
            {
                case DragDirection.Vertical:
                    return moveDir == MoveDirection.Left || moveDir == MoveDirection.Right;
                case DragDirection.Horizontal:
                    return moveDir == MoveDirection.Up || moveDir == MoveDirection.Down;
                case DragDirection.All:
                    return false;
            }
            return false;
        }

        /// <summary>
        /// 计算手势移动方向
        /// </summary>
        private MoveDirection DetermineMoveDirection(float x, float y, float deadZone)
        {
            MoveDirection moveDir = MoveDirection.None;
            if (new Vector2(x, y).sqrMagnitude > deadZone * deadZone)
            {
                if (Mathf.Abs(x) > Mathf.Abs(y))
                {
                    moveDir = x > 0 ? MoveDirection.Right : MoveDirection.Left;
                }
                else
                {
                    moveDir = y > 0 ? MoveDirection.Up : MoveDirection.Down;
                }
            }
            return moveDir;
        }
    }
}
