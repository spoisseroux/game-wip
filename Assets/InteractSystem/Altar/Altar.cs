using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
/*
    CURRENTLY:
        Have to determine the best way to "apply" data to the objects in concrete terms
        Yes, the PlayerManager has a way to load it, and the SaveModule has an abstract ApplyData() function
        But how do we propagate it down into the individual objects properly, with low code duplication?

        It feels a bit awkward right now, whereas before at least accessing data was simple and consistent
*/

// for saving
[System.Serializable]
public class AltarSaveData : ISaveData
{
    public bool interactedBefore { get; set; }

    public AltarSaveData()
    {
        interactedBefore = false;
    }

    public AltarSaveData(bool interacted)
    {
        interactedBefore = interacted;
    } 
}

public class AltarSaveModule : SaveModule<AltarSaveData>
{
    private readonly Altar altar;

    public AltarSaveModule(Altar altarIn)
    {
        altar = altarIn;
    }
    
    protected override void ApplyTypedData(AltarSaveData data) => altar.ApplySaveData(data);
    protected override AltarSaveData CollectTypedData() => altar.CollectSaveData();
}

public class Altar : MonoBehaviour, IInteractable
{
    // player we're interacting with
    [Header("Player References")]
    public PlayerMovementManager player;

    // state internals
    bool interacting = false;

    // save module
    AltarSaveModule save;
    bool canInteract = true;

    // rune
    [SerializeField] private RuneDataSO storedRune; // don't think we need to 'save' serialized data like runes...

    // materials
    [Header("Debug")]
    public Material before;
    public Material after;

    #region MonoBehaviour
    private void Awake() 
    {
        // load in save
        save = new AltarSaveModule(this);
        save.Initialize(); // abstract base call, takes care of GUID assignment/resolution
    }

    private void Start() {
        // value read and set from save module
        if (!canInteract)
        {
            PlayableDirector dir = GetComponent<PlayableDirector>();
            dir.time = dir.duration;
            dir.Evaluate();
            dir.Stop();
        }
    }

    private void OnDestroy()
    {
        save.DetachFromSaveManager();
    }
    #endregion

    #region Interaction Interface
    public bool CanInteract()
    {
        // if we have interacted before, want to return false!
        return false; // read save data
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
        // play director
        GetComponent<PlayableDirector>()?.Play();

        // flag to busy
        interacting = true;
        yield return new WaitForSeconds(1.0f);

        // visuals, data transfer, writing internal save data
        player.GetComponent<RuneHolder>().BestowRune(storedRune);
        canInteract = false;
        yield return new WaitForSeconds(2.0f);

        // free player
        FreePlayer();
    }
    #endregion

    #region Load Save
    public void ApplySaveData(AltarSaveData data)
    {
        canInteract = data.interactedBefore;
    }

    public AltarSaveData CollectSaveData()
    {
        return new AltarSaveData(canInteract);
    }
    #endregion
}