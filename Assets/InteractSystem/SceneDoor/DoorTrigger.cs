using UnityEngine;

public class DoorTrigger : MonoBehaviour, IInteractable
{
    [Header("Spawn To")]
    [SerializeField] SceneField sceneToLoad;
    [SerializeField] int spawnID;
    /*
        notes the ID of the SpawnableLocation in the scene, counts up from 0
    */

    [Header("Spawn From")]
    [SerializeField] SpawnableLocation spawnFrom;

    #region IInteractible
    public bool CanInteract()
    {
        return true;
    }

    public void FreePlayer()
    {
        return;
    }

    public void Interact(PlayerMovementManager player)
    {
        SceneTransitionManager.Transition(sceneToLoad, spawnID);
    }

    public bool IsTrigger()
    {
        return false;
    }

    #endregion
}