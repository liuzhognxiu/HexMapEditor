﻿using UnityEngine;
using UnityEngine.EventSystems;

public class HexGameUI : MonoBehaviour
{

    public static HexGameUI Instrance;

    public HexGrid grid;

    HexCell m_CurrentCell;

    public HexUnit selectedUnit;

    void Awake()
    {
        Instrance = this;
    }

    public void SetEditMode(bool toggle)
    {
        enabled = !toggle;
        grid.ShowUI(!toggle);
        grid.ClearPath();
        // if (toggle) {
        // 	Shader.EnableKeyword("HEX_MAP_EDIT_MODE");
        // }
        // else {
        // 	Shader.DisableKeyword("HEX_MAP_EDIT_MODE");
        // }
    }

    void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(0))
            {
                DoSelection();
            }
            else if (selectedUnit && selectedUnit.isCanSelect)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    DoMove();
                    // DoAttack();
                }
                else
                {
                    DoPathfinding();
                }

                if (Input.GetKeyDown(KeyCode.Q))
                {
                    SelectAttackTarget();
                }
            }

        }
    }


    void SelectAttackTarget()
    {
        if (m_CurrentCell.Unit != null && selectedUnit.Location.GetIsNeighbor(m_CurrentCell))
        {
            selectedUnit.attackHexUnit = m_CurrentCell.Unit;
        }
        else
        {
            selectedUnit.attackHexUnit = null;

        }
    }

    void DoSelection()
    {
        grid.ClearPath();
        UpdateCurrentCell();
        if (m_CurrentCell)
        {
            selectedUnit = m_CurrentCell.Unit;
        }
    }

    void DoPathfinding()
    {
        if (UpdateCurrentCell())
        {
            if (m_CurrentCell && selectedUnit.IsValidDestination(m_CurrentCell))
            {
                grid.FindPath(selectedUnit.Location, m_CurrentCell, selectedUnit);
              
            }
            else
            {
                grid.ClearPath();
            }
        }
    }

    void DoAttack()
    {

        if (m_CurrentCell && m_CurrentCell.Unit && m_CurrentCell.Unit.isMonster)
        {
            selectedUnit.Attack(m_CurrentCell.Unit.unitBase);
        }

    }

    void DoMove()
    {
        if (grid.HasPath)
        {
            selectedUnit.Travel(grid.GetPath());
            grid.ClearPath();
        }
    }

    bool UpdateCurrentCell()
    {
        HexCell cell =
            grid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
        if (cell != m_CurrentCell)
        {
            m_CurrentCell = cell;
            return true;
        }
        return false;
    }
}