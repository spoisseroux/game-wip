using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.SceneManagement;

// performs all the backend work of transitioning scenes 
//      - storing and setting spawn location
//      - fading ui canvas
//      - disabling controls
public class SceneTransitionManager : MonoBehaviour
{
    // player
    private GameObject player;

    // override configs
    private const string INTERACT_STRING = "interact";
    private const string NEUTRAL_STRING = "neutral";

    // constant for NO_SPAWN_ID error
    private const int NO_SPAWN_ID = -404;

    // singleton
    public static SceneTransitionManager instance;

    // static members for maintaining data btw scenes
    public Transform playerSpawnData; // where we are sending the player AFTER the scene transitions
    public int spawnID; // ID of spawning location after transitioning scene
    
    #region MonoBehaviour
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.Log("Destroying");
            Destroy(this.gameObject);
            return;
        }

        Debug.Log("Instantiating");
        
        instance = this;
        spawnID = NO_SPAWN_ID;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    #endregion

    #region Static Functions 
    public static void Transition(SceneField scene, int spawnLocID)
    {
        instance.spawnID = spawnLocID;
        instance.StartCoroutine(instance.FadeThenChangeScene(scene));
    }

    public static Transform ReadSpawnTransform()
    {
        return instance.playerSpawnData;
    }
    #endregion

    #region Helpers
    private IEnumerator FadeThenChangeScene(SceneField scene)
    {
        // deactivate player and force animation
        PlayerMovementManager pmm = player.GetComponent<PlayerMovementManager>();
        InputReader.DeactivateControls();
        pmm.SetState(NEUTRAL_STRING);
        pmm.enabled = false;

        // fade out
        FadeScreen.instance.FadeOut();

        // wait a little
        yield return new WaitForSeconds(1.0f);

        // load scene
        SceneManager.LoadScene(scene);

        // wait a little
        yield return new WaitForSeconds(1.0f);

        // fade in
        FadeScreen.instance.FadeIn();

        // reactivate player
        InputReader.ActivateControls();
        pmm.enabled = true;
        // pmm.SetState(NEUTRAL_STRING);
        
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // find correct spawn location
        SetSpawnTransform(spawnID);
        if (playerSpawnData == null)
            return;

        // set player
        Debug.Log("Loading player at:" + playerSpawnData.position);
        player.transform.position = playerSpawnData.position;
        player.transform.rotation = playerSpawnData.rotation;

        // reset 
        // ResetSpawnTransform();
    }
    
    #endregion

    #region Door -> Transform
    private void SetSpawnTransform(int door)
    {
        if (instance.spawnID == NO_SPAWN_ID)
            return;
        
        // find doors
        SpawnDoor[] doors = FindObjectsByType<SpawnDoor>(FindObjectsSortMode.None);
        for (int i = 0; i < 100; i++)
        {
            if (doors[i].GetSpawnID() == door)
            {
                playerSpawnData = doors[i].GetSpawnTransform();
                return;
            }
        }

        playerSpawnData = null;
    }
    #endregion
}
