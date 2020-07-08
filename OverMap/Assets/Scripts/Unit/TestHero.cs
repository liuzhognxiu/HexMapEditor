using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Unit
{
    [CustomEditor(typeof(HexUnit), true)]
    public class TestHero : HexUnit
    {
        void Start()
        {
            visionRange = isFly ? 5 : 3;
        }
    }
}
