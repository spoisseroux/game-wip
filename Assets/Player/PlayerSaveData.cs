using UnityEngine;
using System.Collections.Generic;
using UnityEditor.SearchService;

// data class
[System.Serializable]
public class PlayerData : ISaveData
{
    // saveable fields
    //public int scene;
    public SerializeableVector3 position;
    public SerializeableQuaternion rotation;
    public List<int> runeIDs;
    public WeaponDataSO weapon;
    public int health;
    // stats, deathcount, etc..... anything you want....

    // create default values on initialization! replace magic #s soon
    public PlayerData()
    {
        //scene = 0;
        position = new SerializeableVector3(Vector3.zero);
        rotation = new SerializeableQuaternion(Quaternion.identity);
        runeIDs = new List<int>();
        //weapon = null;
        health = 100;
    }
}

[System.Serializable]
public class PlayerSaveData : SaveableObject
{
    [SerializeField] PlayerManager player;
    [SerializeField] private PlayerData saveData;

    // could hack around by dragging in default weapon, etc. then plugging into saveData

    #region MonoBehaviour
    private void Awake()
    {
        // assign a GUID in the Awake of every SaveableObject inheritor?
        if (string.IsNullOrEmpty(guid))
        {
            AssignID();
        }

        player = GetComponent<PlayerManager>();
    }

    private void Start()
    {
        saveData = new PlayerData();

        // check for data 
        if (SaveGameManager.HasData(guid))
        {
            // load routine
            saveData = SaveGameManager.GetObjectData(guid) as PlayerData;
        }
        else
        {
            SaveGameManager.AddObject(guid, saveData);
        }

        if (!SaveGameManager.GetSaveDebugMode())
            LoadData(saveData);
        
        SaveGameManager.OnSave += SaveData;
    }

    private void OnDestroy()
    {
        // SaveData();
        SaveGameManager.OnSave -= SaveData;
    }
    #endregion

    #region Saveable Object
    public override void SaveData()
    {
        saveData = player.GatherSaveData();
        SaveGameManager.SaveDataAtGUID(this.guid, saveData);
    }

    public override void LoadData(ISaveData data)
    {
        // propogate data out to the systems that need it
        PlayerData pd = data as PlayerData;
        player.LoadData(pd);
    }
    #endregion
}