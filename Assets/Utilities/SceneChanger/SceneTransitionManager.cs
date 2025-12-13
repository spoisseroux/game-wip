using UnityEngine;
using UnityEngine.SceneManagement;

// performs all the backend work of transitioning scenes 
//      - storing and setting spawn location
public class SceneTransitionManager : MonoBehaviour
{
    // maybe in the future we could do something like
    // dict[INT doorID] --> KEY scene INT, spawn Transform

    public static Transform playerSpawnData; // where we are sending the player AFTER the scene transitions

    public static Vector3 startPosition = new Vector3(-20f, 0f, 10f);
    public static Quaternion startRotation = Quaternion.Euler(0f, 45f, 0f);
    
    #region MonoBehaviour
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        ResetSpawnTransform();
    }
    #endregion

    #region Helpers
    public static void Transition(int scene, Transform data)
    {
        playerSpawnData = data;
        SceneManager.LoadScene(scene);
    }

    public static void ResetSpawnTransform()
    {
        playerSpawnData = null;
    }
    #endregion
}
