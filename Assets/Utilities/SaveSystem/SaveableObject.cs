using System;
using UnityEngine;

public abstract class SaveableObject : MonoBehaviour
{
    protected ISaveData data;
    public string guid;

    [ContextMenu("Generate guid for id")]
    protected void AssignID()
    {
        guid = Guid.NewGuid().ToString();
        Debug.Log("New Call to Assign a GUID");
    }

    public abstract void SaveData();
    public abstract void LoadData(ISaveData data);
}
