using System;
using System.Collections;
using UnityEngine;

// for saving
[System.Serializable]
public class AltarSaveData : ISaveData
{
    public bool interactedBefore { get; set; }

    public AltarSaveData()
    {
        interactedBefore = false;
    }
}

public class Altar : SaveableObject, IInteractable
{
    // player we're interacting with
    [Header("Player References")]
    public PlayerMovementManager player;

    // state internals
    bool interacting = false;

    // data
    AltarSaveData saveData = new AltarSaveData();

    // rune
    [SerializeField] private RuneDataSO storedRune; // don't think we need to 'save' serialized data like runes...

    // materials
    [Header("Debug")]
    public Material before;
    public Material after;

    #region MonoBehaviour
    private void Awake() {
        // assign a GUID in the Awake of every SaveableObject inheritor?
        if (string.IsNullOrEmpty(guid))
        {
            AssignID();
        }

        interacting = false;
    }

    private void Start() {
        GetComponent<Renderer>().material = before;

        saveData = new AltarSaveData();
        // check if instantiated
        if (guid != null && guid != String.Empty)
        {
            // check if save data exists
            if (SaveGameManager.HasData(guid))
            {
                saveData = SaveGameManager.GetObjectData(guid) as AltarSaveData;
            }
            else
            {
                SaveGameManager.AddObject(guid, saveData);
            }
        }
        Debug.Log("Altar save data: " + saveData + saveData.interactedBefore);

        if (saveData.interactedBefore)
        {
            GetComponent<Renderer>().material = after;
        }

        SaveGameManager.OnSave += SaveData;
    }

    private void OnDestroy()
    {
        SaveGameManager.OnSave -= SaveData;
    }
    #endregion

    #region Interaction Interface
    public bool CanInteract()
    {
        // if we have interacted before, want to return false!
        return !saveData.interactedBefore;
    }

    public void Interact(PlayerMovementManager p) {
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

    #region Saveable Object
    public override void SaveData()
    {
        Debug.Log("Altar::SaveData() --> writing Altar data to SaveGameManager");
        SaveGameManager.SaveDataAtGUID(this.guid, saveData);
    }

    public override void LoadData(ISaveData data)
    {
        Debug.Log("Altar::LoadData() --> Loading save data.... ");
        saveData = data as AltarSaveData; 
    }
    #endregion
}