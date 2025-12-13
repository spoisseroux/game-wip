using UnityEngine;
using System.Collections.Generic;
using System;

[System.Serializable]
public class RuneDoorSaveData : ISaveData
{
    public bool opened { get; set; }

    public RuneDoorSaveData()
    {
        opened = false;
    }
}

public class RuneDoor : SaveableObject, IChantReactor
{
    public Material before;
    public Material after;

    // code for chanting door open
    [SerializeField]
    public List<RuneType> code;

    // save data
    RuneDoorSaveData saveData;

    // state internal
    bool interacting = false;

    #region MonoBehaviour
    private void Awake()
    {
        // assign a GUID in the Awake of every SaveableObject inheritor?
        if (string.IsNullOrEmpty(guid))
        {
            AssignID();
        }

        interacting = false;
    }

    private void Start()
    {
        GetComponent<Renderer>().material = before;

        // check save
        saveData = new RuneDoorSaveData();
        if (guid != null && guid != String.Empty)
        {
            if (SaveGameManager.HasData(guid))
                saveData = SaveGameManager.GetObjectData(guid) as RuneDoorSaveData;

            else 
                SaveGameManager.AddObject(guid, saveData);
        }

        if (saveData.opened)
        {
            GetComponent<Renderer>().material = after;
        }

        // link to event
        SaveGameManager.OnSave += SaveData;
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
    #endregion

    #region Saveable Object 
    public override void SaveData()
    {
        Debug.Log("RuneDoor::SaveData() --> saving RuneDoor data to json");
        SaveGameManager.SaveDataAtGUID(guid, saveData);
    }

    public override void LoadData(ISaveData data)
    {
        saveData = data as RuneDoorSaveData;
    }
    #endregion

    public void OpenRoutine()
    {
        // make noises
        // make shader fx
        GetComponent<Renderer>().material = after;
        // open doors
    }
}