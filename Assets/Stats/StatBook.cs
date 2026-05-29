using System.Collections.Generic;
using UnityEngine;

#region Save Module
// save module
[System.Serializable]
public class StatBookSaveData : ISaveData
{
    public List<StatEntry> stats;

    [System.Serializable]
    public struct StatEntry
    {
        public StatID id;
        public StatData values;
    }

    // load
    public StatBookSaveData(List<StatEntry> entries) { stats = entries; }
    // new
    public StatBookSaveData() { stats = new List<StatEntry>(); }
}

public class StatBookSaveModule : SaveModule<StatBookSaveData>
{
    private readonly StatBook statBook;

    public StatBookSaveModule(StatBook book) { statBook = book; }

    protected override void ApplyTypedData(StatBookSaveData data) => statBook.ApplySaveData(data);
    protected override StatBookSaveData CollectTypedData() => statBook.CollectSaveData();
}
#endregion

// interface for stat books
public interface IStatBook
{
    IStat Get(StatID id);
    bool TryGet(StatID id, out IStat stat);
}

public class StatBook : MonoBehaviour, IStatBook
{
    // data store
    private Dictionary<StatID, Stat> stats;

    // default store
    private StatDefaultAssetSO defaultData;

    // save data
    private StatBookSaveModule save;

    #region MonoBehaviour
    private void Awake()
    {
        stats = new Dictionary<StatID, Stat>();
        save = new StatBookSaveModule(this);
        save.Initialize();

        InitializeDefault();
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
        foreach (StatBookSaveData.StatEntry entry in data.stats)
        {
            if (stats.TryGetValue(entry.id, out var stat))
            {
                stat.Set(entry.values.baseValue);
            }
        }
    }

    public StatBookSaveData CollectSaveData()
    {
        return new StatBookSaveData(DictToSaveList(stats));
    }
    #endregion

    #region Helpers
    private void InitializeDefault()
    {
        foreach (var def in defaultData.defaults)
            stats[def.ID] = new Stat(def.baseValue);
    }

    private List<StatBookSaveData.StatEntry> DictToSaveList(Dictionary<StatID, Stat> statsIn)
    {
        List<StatBookSaveData.StatEntry> outList = new List<StatBookSaveData.StatEntry>();
        foreach ( var (id, stat) in statsIn)
            outList.Add(new StatBookSaveData.StatEntry {id = id, values = stat.statData});

        return outList;
    }
    #endregion
}