using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTrigger : MonoBehaviour
{
    [Header("Spawn To")]
    [SerializeField] SceneField sceneToLoad;
    [SerializeField] int spawnTo; 
    /*
        notes the ID of the SpawnableLocation in the scene, counts up from 0
    */

    [Header("Spawn From")]
    [SerializeField] SpawnableLocation spawnFrom;

    #region MonoBehaviour
    // THIS WILL TRIGGER WHEN PLAYER ENTERS, EVEN WITHOUT A CORRESPONDING RIGIDBODY COLLIDER
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneTransitionManager.Transition(sceneToLoad, spawnTo);
        }
    }
    #endregion

    #region Scene Transition

    #endregion

}