using System;
using UnityEngine;

public abstract class SaveableObject : MonoBehaviour
{
    protected ISaveData data; // hmm....
    public string guid;

    /*
        This function is called within the Awake() of every SaveableObject existing in the game world
    */
    [ContextMenu("Generate guid for id")]
    protected void AssignID()
    {
        guid = Guid.NewGuid().ToString();
        Debug.Log("New Call to Assign a GUID");
    }

    public abstract void SaveData();
    public abstract void LoadData(ISaveData data);
}

public abstract class SaveModule
{
    public ISaveData data;
    public string ID;

    [ContextMenu("Generate guid for id")]
    protected void AssignID()
    {
        ID = Guid.NewGuid().ToString();
        Debug.Log("New Call to Assign a GUID");
    }

    // called once 
    public void Initialize()
    {
        if (SaveGameManager.HasData(ID))
        {
            var existing = SaveGameManager.GetObjectData(ID);
            if (!SaveGameManager.GetSaveDebugMode())
            {
                LoadData(existing);
            }

            // subscribe to event
            SaveGameManager.OnSave += SaveData;
        }
    }

    public void SaveData()
    {
        SaveGameManager.SaveDataAtGUID(ID, CollectData());
    }

    public void DetachFromSaveManager()
    {
        SaveGameManager.OnSave -= SaveData;
    }

    public abstract ISaveData CollectData();
    public abstract void LoadData(ISaveData data);
}


// Generic template for save data
public abstract class SaveModule<T> : SaveModule where T : ISaveData, new()
{
    public override ISaveData CollectData() => CollectTypedData();
    public override void LoadData(ISaveData data)
    {
        if (data is T typedData)
            ApplyTypedData(typedData);
        else
            Debug.Log("uhhhh error in templated save data");
    }

    // concrete instances implement this
    protected abstract T CollectTypedData();
    protected abstract void ApplyTypedData(T data);
}