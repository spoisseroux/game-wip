using System;
using System.Collections;
using UnityEngine;

public class Altar : SaveableObject, IInteractable
{
    [Header("Player References")]
    public PlayerMovementManager player;

    // state internals
    bool interacting = false;
    private class AltarSaveData : ISaveData
    {
        public bool interactedBefore { get; set; }
    }
    AltarSaveData saveData;

    // anim , needs some work!
    string animName = "Interact_Generic";

    // rune
    [SerializeField] private RuneDataSO storedRune; // don't think we need to 'save' serialized data like runes...

    [Header("Debug")]
    public Material before;
    public Material after;

    #region MonoBehaviour
    private void Awake() {
        interacting = false;
        saveData.interactedBefore = false;

        // assign a GUID?
    }

    private void Start() {
        GetComponent<Renderer>().material = before;

        // check if instantiated
        if (guid != null && guid != String.Empty)
        {
            // check if save data exists
            if (SaveGameManager.HasData(guid))
            {
                data = SaveGameManager.GetObjectData(guid) as AltarSaveData;
            }
            else
            {
                data = new AltarSaveData();
                SaveGameManager.AddObject(guid, data);
            }
        }
    }
    #endregion

    #region Interaction Interface
    public void Interact(PlayerMovementManager p) {
        // only bestow rune once
        if (saveData.interactedBefore)
            return;

        // no simultaneous interactions
        if (interacting)
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
        saveData.interactedBefore = true;
        FreePlayer();
    }

    #endregion
}