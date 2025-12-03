using System.Collections;
using UnityEngine;

public class Altar : MonoBehaviour, IInteractable
{
    [Header("Player References")]
    public PlayerMovementManager player;

    // state internals
    bool interacting = false;
    bool interactedPreviously = false;

    // anim , needs some work!
    string animName = "Interact_Generic";

    // rune
    [SerializeField] private RuneDataSO storedRune;

    [Header("Debug")]
    public Material before;
    public Material after;

    #region MonoBehaviour
    private void Awake() {
        interacting = false;
        interactedPreviously = false;
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

        // start coroutine
        StartCoroutine(ExecuteInteraction());
    }

    public void FreePlayer() {
        player.ResetInteract();
        interacting = false;
    }

    public bool IsTrigger() {
        return true;
    }
    #endregion

    #region Coroutine
    public IEnumerator ExecuteInteraction()
    {
        // flag to busy
        interacting = true;
        yield return new WaitForSeconds(1.0f);
        // visuals & data
        GetComponent<Renderer>().material = after;
        player.GetComponent<RuneHolder>().BestowRune(storedRune);
        yield return new WaitForSeconds(2.0f);
        // free player
        interactedPreviously = true;
        FreePlayer();
    }

    #endregion
}