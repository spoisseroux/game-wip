using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

// Unity Lifecycle Stuff
// 1. Awake
// 2. OnEnable
// 3. OnSceneLoaded/Unloaded
// 4. Start

// ADJUST
public class SaveGameManager : MonoBehaviour
{
    // singleton instance
    private static SaveGameManager instance;

    // all save data
    private static Dictionary<string, ISaveData> save;
    /*
        maps any given GUID --> ISaveData instance
    */

    // configs
    private static bool newGame = true;

    // actions
    public static Action OnSave; // NEED TO FIGURE OUT WHERE && HOW TO HOOKUP SAVEABLE OBJECTS TO THIS!!!

    #region MonoBehaviour
    private void Awake()
    {
        // singleton call
        if (instance != null)
        {
            Debug.Log("More than one save game managers!");
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject); // singleton instance takes care of multiple spawns

        
        //save = new();
        //newGame = true;
        
        // access persistent file path

    }

    private void Start()
    {
        // i sense necessary but we shall see
    }

    // CALLED BEFORE START
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // gather all SaveableObjects
        // load their data
        LoadGame();
    }

    // CALLED BEFORE START
    public void OnSceneUnloaded(Scene scene)
    {
        SaveGame();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }
    #endregion

    #region Saving
    public static void SaveGame()
    {
        if (save == null)
        {
            Debug.Log("SaveGameManager::SaveGame() --> no save data found, need a new save file");
            return;
        }

        // ask all items to save their info
        OnSave?.Invoke();

        // write to json
        var json = JsonConvert.SerializeObject(save); // need second settings argument
        if (Debug.isDebugBuild)
			{
				Debug.Log("Saving file " + Application.persistentDataPath + "/Save.json");
			}
			
			using (var file = new StreamWriter(Application.persistentDataPath + "/Save.json"))
			{
				file.Write(json);
			}
    }
    #endregion

    #region Adding Objects
    public static void AddObject(string id, ISaveData data)
    {
        // add to outer dict
        save.Add(id, data);
    }
    #endregion

    #region Removing Objects
    public static void RemoveItemFromScene(string id)
    {
        save.Remove(id);
    }
    #endregion

    #region Retrieving Objects
    public static ISaveData GetObjectData(string id)
    {
        return save[id];
    }

    public static bool HasData(string id)
		{
			return save.ContainsKey(id);
		}
    #endregion

    #region Loading Save Data
    public void LoadGame()
    {
        // read from file... tbi
        save = null;
        if (save == null)
        {
            // hmmm
        }
    }
    #endregion

    #region New Game
    public void NewGame()
    {
        
    }
    #endregion
}
