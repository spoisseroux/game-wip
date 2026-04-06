using UnityEngine;
using System.Collections.Generic;

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
    public float health;
    // stats, deathcount, etc..... anything you want....

    // create default values on initialization! replace magic #s soon
    public PlayerData()
    {
        //scene = 0;
        position = new SerializeableVector3(Vector3.zero);
        rotation = new SerializeableQuaternion(Quaternion.identity);
        runeIDs = new List<int>();
        //weapon = null;
        health = 100f;
    }
}

public class PlayerSaveModule : SaveModule<PlayerData>
{
    private readonly PlayerManager playerManager;

    public PlayerSaveModule(PlayerManager manager)
    {
        playerManager = manager;
    }

    protected override void ApplyTypedData(PlayerData dataIn) => playerManager.LoadData(dataIn);
    protected override PlayerData CollectTypedData() => playerManager.GatherSaveData();
}