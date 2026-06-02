using System.Collections.Generic;
using UnityEngine;

#region Save Module
[System.Serializable]
public class StatBookSaveData : ISaveData
{
    public SerializeableDictionary<StatID, StatData> stats;

    // load
    public StatBookSaveData(Dictionary<StatID, Stat> entries)
    {
        foreach (var key in entries.Keys)
            stats.Add(key, entries[key].statData);
    }
    // new
    public StatBookSaveData() { stats = new SerializeableDictionary<StatID, StatData>(); }
}

public class StatBookSaveModule : SaveModule<StatBookSaveData>
{
    private readonly StatBook statBook;

    public StatBookSaveModule(StatBook book) { statBook = book; }

    protected override void ApplyTypedData(StatBookSaveData data) => statBook.ApplySaveData(data);
    protected override StatBookSaveData CollectTypedData() => statBook.CollectSaveData();
}
#endregion

public class StatBook : MonoBehaviour, IStatBook
{
    // data store
    private Dictionary<StatID, Stat> stats; // need UI to verify...

    // default store, one SO per Unit
    private StatDefaultAssetSO defaultData;

    // save data
    private StatBookSaveModule save;

    #region MonoBehaviour
    private void Awake()
    {
        stats = new Dictionary<StatID, Stat>();
        save = new StatBookSaveModule(this);
        save.Initialize();
    }

    private void OnDestroy()
    {
        // save.DetachFromSaveManager();
    }
    #endregion

    #region IStatBook Interface 
    public IStat Get(StatID id) => stats[id];

    public bool TryGet(StatID id, out IStat stat)
    {
        var found = stats.TryGetValue(id, out var s);
        stat = s;
        return found;
    }
    #endregion

    #region Save/Load
    public void ApplySaveData(StatBookSaveData data)
    {
        foreach (var (id, statData) in data.stats)
        {
            if (stats.TryGetValue(id, out var stat))
            {
                // more complicated stat initialization can go here, but for now just baseValue is fine
                stat.Set(statData.baseValue);
            }
        }
    }

    public StatBookSaveData CollectSaveData()
    {
        return new StatBookSaveData(stats);
    }
    #endregion

    #region Helpers
    private void ResetToDefault()
    {
        // Loop through each defaultSO StatDefault objects, apply their data to Dictionary
        foreach (var def in defaultData.defaults)
            stats[def.ID].Set(def.baseValue);
    }
    #endregion
}