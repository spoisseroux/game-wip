using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "__DefaultStats", menuName = "Stats/DefaultData/SO")]
public class StatDefaultAssetSO : ScriptableObject
{
    public List<StatDefault> defaults;

    [System.Serializable]
    public struct StatDefault
    {
        public StatID ID;
        public float baseValue;
    }
}