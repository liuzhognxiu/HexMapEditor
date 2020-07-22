using Assets.Scripts.Manager;
using UnityEngine;

public class NewMapMenu : MonoBehaviour {

	public HexGrid hexGrid;

	public HexMapGenerator mapGenerator;

    public GameObject createButton;

	bool m_GenerateMaps = true;

	bool m_Wrapping = false;

	public void ToggleMapGeneration (bool toggle) {
		m_GenerateMaps = toggle;
	}

	public void ToggleWrapping (bool toggle) {
		m_Wrapping = toggle;
	}

	public void Open () {
		gameObject.SetActive(true);
		HexMapCamera.Locked = true;
	}

	public void Close () {
		gameObject.SetActive(false);
		HexMapCamera.Locked = false;
        createButton.gameObject.SetActive(false);
        GameManger.Instance.IsStartGame = true;
    }

	public void CreateSmallMap () {
		CreateMap(20, 15);  
	}

	public void CreateMediumMap () {
		CreateMap(40, 30);
	}

	public void CreateLargeMap () {
		CreateMap(80, 60);
	}

	void CreateMap (int x, int z) {
		if (m_GenerateMaps) {
			mapGenerator.GenerateMap(x, z, m_Wrapping);
		}
		else {
			hexGrid.CreateMap(x, z, m_Wrapping);
		}
		HexMapCamera.ValidatePosition();
		Close();
	}
}