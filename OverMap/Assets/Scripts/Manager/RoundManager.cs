using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Monster;

public class RoundManager : MonoSingleton<RoundManager>
{
    private int m_CurrentRoundNumber = 1;

    public int CurrentRoundNumber
    {
        get => m_CurrentRoundNumber;
        set
        {
            m_CurrentRoundNumber = value;
            if (CurrentRoundNumber % 5 == 0)
            {
                for (int i = 0; i < (int)Random.Range(1, 4); i++)
                {
                    HexMapGenerator.Instrance.CreateMonster();
                }
            }
        }
    }

    public bool currentIsOver = true;

    public bool currentMonsterMoveOver = true;

    public Queue<MonsterBase> monsterHexUnits = new Queue<MonsterBase>();

    public void MonsterDoMove()
    {
        if (monsterHexUnits.Count > 0)
        {
            MonsterBase monster = monsterHexUnits.Dequeue();
            monster.FindHeroCell();
            currentMonsterMoveOver = false;
        }
    }

}
