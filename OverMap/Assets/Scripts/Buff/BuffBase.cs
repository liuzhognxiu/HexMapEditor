using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Buff
{

    public enum Bufftype
    {
        None = 0,
        Attack = 1,
        Defend = 2,
        RecoverBlood = 3,
    }

    public interface IBuff
    {
        int GetID();
        void OnEnter();
        void OnUpdate();
        void OnExit();
    }

    public class BuffBase : IBuff
    {
        public delegate void EnterDel(HexUnit unit);

        public delegate void UpdateDel();

        public delegate void ExitDel();

        public delegate void ReturnDel();

        public object[] parms; //参数列表
        private bool canUpdate;
        private int id;
        private bool canDestroy;

        public Bufftype bufftype = Bufftype.None;

        public BuffBase()
        {

        }

        public void SetDel()
        {

        }

        public void SwitchUpdate(bool canUpdate)
        {
            this.canUpdate = canUpdate;
        }

        public int GetID()
        {
            return id;
        }

        public virtual void OnEnter()
        {

        }

        public void OnUpdate()
        {

        }

        public void OnExit()
        {

        }
    }
}
