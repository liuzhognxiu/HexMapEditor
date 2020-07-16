﻿using System.Linq;
using UnityEngine;
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
            if (selectedUnit && selectedUnit.isCanSelect)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    AddSearchFrontier();
                }
                if (Input.GetMouseButtonDown(0))
                {
                    DeleteSearchFrontier();
                }
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    SelectAttackTarget();
                    DoMove();
                }
            }

        }
    }

    void DeleteSearchFrontier()
    {
        HexCell cell =
            grid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
        if (cell == grid.showhexCells.Last())
        {
            grid.showhexCells.Last().DisableHighlight();
            grid.showhexCells.Remove(grid.showhexCells.Last());
            grid.ShowPath(selectedUnit.speed, m_CurrentCell, selectedUnit.Location, true);
        }
    }

    void AddSearchFrontier()
    {

        if (UpdateCurrentCell() && m_CurrentCell)
        {
            if (grid.showhexCells.Count != 0)
            {
                if (
                    selectedUnit.IsValidDestination(grid.showhexCells.Last()) &&
                    grid.showhexCells.Last().GetIsNeighbor(m_CurrentCell) &&
                    grid.showhexCells.Last().Buff.bufftype == m_CurrentCell.Buff.bufftype
                    )
                {
                    if (!grid.showhexCells.Contains(m_CurrentCell))
                    {
                        grid.showhexCells.Add(m_CurrentCell);
                    }
                }
            }
            else if (selectedUnit.IsValidDestination(m_CurrentCell) && selectedUnit.Location.GetIsNeighbor(m_CurrentCell))
            {
                grid.showhexCells.Add(selectedUnit.Location);
                if (!grid.showhexCells.Contains(m_CurrentCell))
                {
                    grid.showhexCells.Add(m_CurrentCell);
                }
            }
            grid.ShowPath(selectedUnit.speed, m_CurrentCell, selectedUnit.Location, true);

        }

    }

    void SelectAttackTarget()
    {
        // if (m_CurrentCell.Unit != null && selectedUnit.Location.GetIsNeighbor(m_CurrentCell))
        // {
        //     selectedUnit.attackHexUnit = m_CurrentCell.Unit;
        // }
        // else
        // {
        //     selectedUnit.attackHexUnit = null;
        // }

        if (m_CurrentCell.Unit != null && selectedUnit.Location.GetIsNeighbor(m_CurrentCell))
        {
            if (!selectedUnit.attackHexUnits.Contains(m_CurrentCell.Unit))
            {
                selectedUnit.attackHexUnits.Add(m_CurrentCell.Unit);
            }
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
        selectedUnit.Travel(grid.showhexCells);
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