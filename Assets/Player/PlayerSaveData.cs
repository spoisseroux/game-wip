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
    //public List<RuneDataSO> runes;
    public WeaponDataSO weapon;
    public int health;
    // stats, deathcount, etc..... anything you want....

    // create default values on initialization! replace magic #s soon
    public PlayerData()
    {
        //scene = 0;
        position = new SerializeableVector3(SceneTransitionManager.startPosition);
        rotation = new SerializeableQuaternion(SceneTransitionManager.startRotation);
        //runes = new List<RuneDataSO>();
        weapon = null;
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
        Debug.Log(saveData.health);

        // check for data 
        if (SaveGameManager.HasData(guid))
        {
            // load routine
            saveData = SaveGameManager.GetObjectData(guid) as PlayerData;
            player.LoadData(saveData);
        }
        else
        {
            SaveGameManager.AddObject(guid, saveData);
            player.LoadData(saveData);
        }

        Debug.Log("Player save data: " + saveData + saveData.health);
        SaveGameManager.OnSave += SaveData;
    }

    private void OnDestroy()
    {
        SaveData();
        SaveGameManager.OnSave -= SaveData;
    }
    #endregion

    #region Saveable Object
    public override void SaveData()
    {
        Debug.Log("PlayerSaveData::SaveData() --> writing Player data to SaveGameManager");
        //saveData = player.GatherSaveData();
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