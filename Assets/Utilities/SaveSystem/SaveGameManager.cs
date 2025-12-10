using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
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
    [SerializeField]
    private static SerializeableDictionary<string, ISaveData> save;
    /*
        maps any given GUID --> ISaveData instance

        NOTE: JSON formats do not serialize/deserialize dictionaries by default, so we are using the ISerializationCallbackReceiver 
        interface to take care of this problem.

        If the scope of datatypes and complexity of objects to save grows larger, then maybe JSON .NET for Unity package would be 
        a good idea to add support for...
    */

    // filehandler
    FileSaveHandler fileHandler;

    // configs
    public bool debugMode = true;

    // actions
    public static Action OnSave; // NEED TO FIGURE OUT WHERE && HOW TO HOOKUP SAVEABLE OBJECTS TO THIS!!!
    public static Action OnLoad; // YEAH FIGURE THIS OUT TOO

    #region MonoBehaviour
    private void Awake()
    {
        // singleton call
        if (instance != null)
        {
            Debug.Log("More than one save game managers!");
            Destroy(this.gameObject);
        }
        //instance = this;
        DontDestroyOnLoad(this.gameObject); // singleton instance takes care of multiple spawns, need fix this

        
        save = new();
        //newGame = true;
        
        // set up file handler
        this.fileHandler = new FileSaveHandler(debugMode);
    }

    private void Start()
    {
        // LoadGame();
    }

    // CALLED BEFORE START
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        LoadGame();
    }

    // CALLED BEFORE START
    public void OnSceneUnloaded(Scene scene)
    {
        Debug.Log("SaveGameManager::OnSceneUnloaded() --> called");
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
    public void SaveGame()
    {
        Debug.Log("SaveGameManager::SaveGame() --> called");
        if (save == null)
        {
            Debug.Log("SaveGameManager::SaveGame() --> no save data found, need a new save file");
            return;
        }

        Debug.Log("SaveGameManager::SaveGame() --> Invoking OnSave Action method");
        // ask all items to save their info
        OnSave?.Invoke();

        // save to file
        Debug.Log("SaveGameManager::SaveGame() --> Attempting to save to file...");
        foreach (KeyValuePair<string, ISaveData> pair in save)
        {
            Debug.Log(pair.Key.ToString() + "\n");
            ISaveData val = pair.Value;
            Debug.Log(val);
        }
        fileHandler.Save(1, save);
    }

    public static void SaveDataAtGUID(string guid, ISaveData data)
    {
        Debug.Log(data);
        save[guid] = data;
    }
    #endregion

    #region Adding Objects
    public static void AddObject(string id, ISaveData data)
    {
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
        // read from file
        save = fileHandler.Load(1);

        // debug mode and null data
        if (save == null && debugMode)
        {
            Debug.Log("SaveGameManager::LoadData() --> no saved data from file, creating new file");
            NewGame(); // hmmm maybe not?
        }

        // just null data, need new game
        if (save == null)
        {
            NewGame();
        }
    }
    #endregion

    #region New Game
    public void NewGame()
    {
        // new game
        save = new SerializeableDictionary<string, ISaveData>();
        Debug.Log("Created new save dictionary!");
    }
    #endregion
}
