using System;
using UnityEngine;
using UnityEngine.Playables;

public class LeverSaveData : ISaveData
{
    public bool switched;

    public LeverSaveData()
    {
        switched = false;
    }
}

public class Lever : SaveableObject, IHittable
{
    // components
    [SerializeField] PlayableDirector director;

    // save data
    LeverSaveData saveData;

    // list of gameobjects we want to affect upon hit
    [SerializeField] Platform[] platforms;

    #region MonoBehaviour
    void Awake()
    {
        if (string.IsNullOrEmpty(guid))
        {
            AssignID();
        }

        // set up stuff
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // check save
        saveData = new LeverSaveData();
        if (guid != null && guid != String.Empty)
        {
            if (SaveGameManager.HasData(guid))
            {
                saveData = SaveGameManager.GetObjectData(guid) as LeverSaveData;
            }

            else
                SaveGameManager.AddObject(guid, saveData);
        }

        if (saveData.switched)
        {
            // move lever down
            director.time = director.duration;
            director.Evaluate();
            director.Stop();

            // move platforms
            foreach (Platform p in platforms)
            {
                p.ToggledLever();
            }
        }

        SaveGameManager.OnSave += SaveData;
    }

    void OnDestroy()
    {
        SaveGameManager.OnSave -= SaveData;
    }
    #endregion

    #region IHittable Interface
    public void Hit()
    {
        // director
        director.Play();

        // move platforms
        foreach (Platform p in platforms)
        {
            p.ToggledLever();
        }
    }
    #endregion

    #region Saveable Object
    public override void SaveData()
    {
        Debug.Log("Lever::SaveData() --> saving lever to json");
        SaveGameManager.SaveDataAtGUID(guid, saveData);
    }

    public override void LoadData(ISaveData data)
    {
        saveData = data as LeverSaveData;
    }
    #endregion
}
