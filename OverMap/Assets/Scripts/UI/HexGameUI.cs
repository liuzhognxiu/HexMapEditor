using UnityEngine;
using UnityEngine.EventSystems;

public class HexGameUI : MonoBehaviour {

	public HexGrid grid;

	HexCell m_CurrentCell;

	HexUnit m_SelectedUnit;

	public void SetEditMode (bool toggle) {
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

	void Update () {
		if (!EventSystem.current.IsPointerOverGameObject()) {
			if (Input.GetMouseButtonDown(0)) {
				DoSelection();
			}
			else if (m_SelectedUnit && m_SelectedUnit.isCanSelect) {
				if (Input.GetMouseButtonDown(1)) {
					DoMove();
				}
				else {
					DoPathfinding();
				}
			}
		}
	}

	void DoSelection () {
		grid.ClearPath();
		UpdateCurrentCell();
		if (m_CurrentCell) {
			m_SelectedUnit = m_CurrentCell.Unit;
		}
	}

	void DoPathfinding () {
		if (UpdateCurrentCell()) {
			if (m_CurrentCell && m_SelectedUnit.IsValidDestination(m_CurrentCell)) {
				grid.FindPath(m_SelectedUnit.Location, m_CurrentCell, m_SelectedUnit);
			}
			else {
				grid.ClearPath();
			}
		}
	}

	void DoMove () {
		if (grid.HasPath) {
			m_SelectedUnit.Travel(grid.GetPath());
			grid.ClearPath();
		}
	}

	bool UpdateCurrentCell () {
		HexCell cell =
			grid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
		if (cell != m_CurrentCell) {
			m_CurrentCell = cell;
			return true;
		}
		return false;
	}
}