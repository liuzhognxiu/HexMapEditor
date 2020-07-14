using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Buff
{
    class AttackBuff : BuffBase
    {

        public override void OnEnter()
        {
            HexGameUI.Instrance.selectedUnit.unitBase.attack += 1;
        }
    }

    class DefendBuff : BuffBase
    {

        public override void OnEnter()
        {
            HexGameUI.Instrance.selectedUnit.unitBase.defend += 1;
        }
    }

    class HPBuff : BuffBase
    {

        public override void OnEnter()
        {
            HexGameUI.Instrance.selectedUnit.unitBase.hp += 1;
        }
    }

    class EventBuff : BuffBase
    {

        public override void OnEnter()
        {
            Debug.Log("触发了事件！！！！！！！！！！！！");
            // HexGameUI.Instrance.selectedUnit.unitBase.defend += 1;
        }
    }

}
