using System.Collections.Generic;
using Assets.Scripts.Monster;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Hero
{
    public class SelectTestMonster : MonoBehaviour
    {
        public List<Monster> prefabs;
        void Start()
        {
            if (prefabs != null)
            {

                for (int i = 0; i < prefabs.Count; i++)
                {
                    int j = i;
                    prefabs[i].gameObject.AddComponent<Button>().onClick.AddListener(delegate ()
                    {
                        HexUnit.unitPrefab = prefabs[j].prefab;
                    });
                }
            }
        }
    }
}
