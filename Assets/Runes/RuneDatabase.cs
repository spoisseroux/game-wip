using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "RuneDatabase", menuName = "Runes/RuneDatabase")]
public class RuneDatabase : ScriptableObject
{
    public List<RuneDataSO> runes;
    public Dictionary<RuneType, RuneDataSO> map;

    public RuneDataSO GetRune(RuneType r)
    {
        if (map == null)
            map = runes.ToDictionary(r => r.runeValue, r => r); // uhhhhh
        return map[r];
    }
}
