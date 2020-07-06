using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectTestHero : MonoBehaviour
{
    public List<Hero> prefabs;
    void Start()
    {
        if (prefabs != null)
        {

            for (int i = 0; i < prefabs.Count; i++)
            {
                int j = i;
                prefabs[i].gameObject.AddComponent<Button>().onClick.AddListener(delegate ()
                    {
                        HexUnit.unitPrefab = prefabs[j].Prefab;
                    });
            }
        }
    }
}
