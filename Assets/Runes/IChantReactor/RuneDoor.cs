using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Playables;

[System.Serializable]
public class RuneDoorSaveData : ISaveData
{
    public bool opened { get; set; }

    public RuneDoorSaveData() { opened = false; }
    public RuneDoorSaveData(bool open) { opened = open; }
}

public class RuneDoorSaveModule : SaveModule<RuneDoorSaveData>
{
    protected readonly RuneDoor door;
    public RuneDoorSaveModule(RuneDoor doorIn) { door = doorIn; }

    protected override void ApplyTypedData(RuneDoorSaveData data) => door.ApplySaveData(data);
    protected override RuneDoorSaveData CollectTypedData() => door.CollectSaveData();
}

public class RuneDoor : MonoBehaviour, IChantReactor
{
    // code for chanting door open
    [SerializeField]
    public List<RuneType> code;

    // save data
    RuneDoorSaveModule saveData;
    bool open = false;

    // state internal
    bool interacting = false;

    #region MonoBehaviour
    private void Awake()
    {
        saveData = new RuneDoorSaveModule(this);
        saveData.Initialize();
    }

    private void Start()
    {
        if (open)
        {
            // open door manually, maybe needs to be diff here
            OpenRoutine();
        }
    }

    private void OnDestroy()
    {
        // remove from save event
        saveData.DetachFromSaveManager();
    }
    #endregion

    #region IChantReactor
    public void React(List<RuneType> runes)
    {
        Debug.Log("Woah someone wanted something from me the humble RoonDoar...");
        if (IsValidChant(runes))
        {
            OpenRoutine();
        }
    }
    #endregion

    #region Helpers
    public bool IsValidChant(List<RuneType> runes)
    {
        if (code.Count != runes.Count) 
            return false;

        for (int i = 0; i < code.Count; i++)
        {
            if (runes[i] != code[i])
                return false;
        }

        return true;
    }

    public void OpenRoutine()
    {
        // open door
        PlayableDirector dir = GetComponent<PlayableDirector>();
        dir.time = dir.duration;
        dir.Evaluate();
        dir.Stop();
    }
    #endregion

    #region Save & Load 
    public void ApplySaveData(RuneDoorSaveData data)
    {
        open = data.opened;
    }

    public RuneDoorSaveData CollectSaveData()
    {
        return new RuneDoorSaveData(open);
    }
    #endregion
}