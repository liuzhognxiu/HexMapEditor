using Assets.Scripts.Monster;
using UnityEngine;

namespace Assets.Scripts.Manager
{
    public class GameManger : MonoSingleton<GameManger>
    {

        public bool IsStartGame;
        void Start()
        {
            HexGameUI.Instrance.currRoundText.text =
                $"第{RoundManager.Instance.CurrentRoundNumber.ToString()}回合";
        }

        void FixedUpdate()
        {
            if (IsStartGame)
            {
                if (HexGameUI.Instrance.selectedUnit.isMoveOver)
                {
                    if (RoundManager.Instance.currentIsOver && RoundManager.Instance.currentMonsterMoveOver)
                    {
                        RoundManager.Instance.MonsterDoMove();
                    }
                }

                if (RoundManager.Instance.monsterHexUnits.Count == 0 && HexGameUI.Instrance.selectedUnit.isMoveOver)
                {
                    RoundManager.Instance.currentIsOver = true;
                    RoundManager.Instance.CurrentRoundNumber++;
                    HexGameUI.Instrance.currRoundText.text =
                        $"第{RoundManager.Instance.CurrentRoundNumber.ToString()}回合";
                    HexGameUI.Instrance.selectedUnit.isMoveOver = false;

                    for (int i = 0; i < HexGameUI.Instrance.grid.units.Count; i++)
                    {
                        if (HexGameUI.Instrance.grid.units[i].isMonster)
                        {
                            RoundManager.Instance.monsterHexUnits.Enqueue((MonsterBase)HexGameUI.Instrance.grid.units[i]);
                        }
                    }
                }
            }
        }
    }
}
