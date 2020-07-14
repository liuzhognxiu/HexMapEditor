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
            base.OnEnter();
            Debug.Log("获得攻击buff");
            HexGameUI.Instrance.selectedUnit.unitBase.attack += 1;
        }
    }
}
