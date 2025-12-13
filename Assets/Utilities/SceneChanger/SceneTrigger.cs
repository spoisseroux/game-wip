using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTrigger : MonoBehaviour
{
    // [SerializeField] DATA FOR WHERE IN WHICH SCENE TO TRANSITION!!!
    // [SerializeField] DOOR ID

    [SerializeField] int scene;
    [SerializeField] private Transform destinationInTargetScene;

    #region MonoBehaviour
    // THIS WILL TRIGGER WHEN PLAYER ENTERS, EVEN WITHOUT A CORRESPONDING RIGIDBODY COLLIDER
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // change
            SceneTransitionManager.Transition(scene, destinationInTargetScene);
        }
    }
    #endregion
}
