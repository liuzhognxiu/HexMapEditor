using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;

public class Const  {
    
    public enum  PassEventBuff
    {
        [Header("开始")]
        OnStart = 0,
        [Header("角色生成")]
        OnCreate = 1,
        [Header("角色入场")]
        OnCreate1 = 2,
        [Header("角色攻击")]
        OnCreate2 = 3,
        [Header("结束")]
        OnEnd = 4,
    }
    
    public enum BuffEventType
    {
        [Header("时间间隔触发")]
        OnTimeDuration = 0,
        [Header("次数触发")]
        OnCountDuration = 1,
        [Header("随机数")]
        OnRound = 2,
        [Header("是否是主公")]
        OnSelfIsleader = 3,
    }

    /// <summary>
    /// buff类型
    /// </summary>
    public enum  BuffType
    {
        [Header("数值类型buffer")]
        SetValue = 0,
        [Header("状态类型Buffer")]
        SetState = 1,
        [Header("击退")]
        BeatBack = 2,
        [Header("击倒")]
        KnockDown = 3,
    }

    /// <summary>
    /// 目标类型
    /// </summary>
    public enum SelectTargetType
    {
        [Header("自己")]
        self = 0,
        [Header("当前目标")]
        Target = 1,
        [Header("敌方全体")]
        TheEnemyOfAll = 2,
        [Header("击倒")]
        TheEnemyOfFront = 3,
    }

    /// <summary>
    /// 数值buff的数值类型
    /// </summary>
    public enum BufferSetValue
    {
        [Header("攻击")]
        Atk = 1,
        [Header("防御")]
        Def = 2,
    }
}
