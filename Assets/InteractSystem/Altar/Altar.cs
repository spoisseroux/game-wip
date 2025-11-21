using UnityEngine;
using UnityEngine.InputSystem;

public class Altar : MonoBehaviour, IInteractable
{
    [Header("Player References")]
    public PlayerMovementManager player;

    // state internals
    bool interacting = false;
    bool interactedPreviously = false;

    // rune
    [SerializeField] private RuneType storedRune;

    [Header("Debug")]
    public Material before;
    public Material after;

    #region MonoBehaviour
    private void Awake() {
        
    }

    private void Start() {
        GetComponent<Renderer>().material = before;
    }
    #endregion

    #region Interaction Interface
    public void Interact(PlayerMovementManager p) {
        // only bestow rune once
        if (interactedPreviously)
            return;

        // resolve player
        player = p;

        // flag to busy
        interacting = true;

        // material change
        GetComponent<Renderer>().material = after;

        // send rune to player
        player.GetComponent<RuneHolder>().BestowRune(storedRune);

        // turn off option to interact
        interactedPreviously = true;

        FreePlayer();
    }

    public void FreePlayer() {
        player.ResetInteract();
        interacting = false;
    }

    public bool IsTrigger() {
        return true;
    }
    #endregion
}