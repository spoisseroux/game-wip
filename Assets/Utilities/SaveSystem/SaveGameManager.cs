using System;
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
    public bool debugMode; 
    /* 
        if on debug mode, we'll want to forgo loading of player, possibly other objects as well
        we want to provide the option to set data manually and keep it maintained through debug testing
    */

    // actions
    public static Action OnSave; 
    /* 
        Hooked up to every SaveableObject within its Start() function
        Removed from every SaveableObject within its OnDestroy() function
    */

    #region MonoBehaviour
    private void Awake()
    {
        // singleton call
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        Debug.Log("Initialized SaveManager");
        
        save = new();
        this.fileHandler = new FileSaveHandler(debugMode);
    }

    // CALLED BEFORE START
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (debugMode)
            return;
        
        LoadGame();
    }

    // CALLED BEFORE START
    public void OnSceneUnloaded(Scene scene)
    {
        // SaveGame();
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

    #region Debug Reader
    public static bool GetSaveDebugMode()
    {
        return instance.debugMode;
    }
    #endregion

    #region Saving
    public void SaveGame()
    {
        if (debugMode) 
            return;

        if (save == null)
        {
            Debug.Log("SaveGameManager::SaveGame() --> no save data found, need a new save file");
            return;
        }

        Debug.Log("SaveGameManager::SaveGame() --> Attempting to save to file...");
        // ask all saveable objects to record their info, then save to file
        OnSave?.Invoke();
        fileHandler.Save(1, save);
    }

    public static void SaveDataAtGUID(string guid, ISaveData data)
    {
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
        // if we want to avoid loading from file and just do debug testing
        if (debugMode)
            return;

        // read from file
        save = fileHandler.Load(1);
        if (save == null)
        {
            Debug.Log("SaveGameManager::LoadData() --> no saved data from file, creating new file");
            NewGame(); // hmmm maybe not?
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
