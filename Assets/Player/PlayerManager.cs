using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    // various system components
    public PlayerMovementManager movementManager; 
    public CombatOrchestrator combatOrchestrator;
    public RuneHolder runeHolder;

    #region Monobehavior
    private void Awake()
    {
        movementManager = GetComponent<PlayerMovementManager>();
        combatOrchestrator = GetComponent<CombatOrchestrator>();
        runeHolder = GetComponent<RuneHolder>();
    }

    private void Start()
    {
        
    }

    // for camera, singleton call
    private void LateUpdate()
    {
        PlayerCamera.instance.HandleAllCameraActions();
    }
    #endregion

    public void LoadData(PlayerData data)
    {
        Debug.Log("PlayerManager::LoadData() --> loading player data");
        // player manager
        this.transform.position = data.position.UnityVector;
        this.transform.rotation = data.rotation.UnityQuaternion;

        // movement manager saved objects

        // combat saved objects
        combatOrchestrator.SetHealth(data.health); // hacky for now
        // combatManager.SetEquippedWeapon(data.weapon); // leaving alone for a moment

        // rune holder
        runeHolder.LoadRunes(data.runeIDs);
    }

    public PlayerData GatherSaveData()
    {
        PlayerData saveOverwrite = new PlayerData();

        // fill save data down the hierarchy
        // gameobject level
        saveOverwrite.position = new SerializeableVector3(this.transform.position);
        saveOverwrite.rotation = new SerializeableQuaternion(this.transform.rotation);

        // combat manager
        saveOverwrite.health = combatOrchestrator.GetHealth();
        // leave til fixed saveOverwrite.weapon = ...

        // rune holder
        saveOverwrite.runeIDs = runeHolder.SetSavedRunes();
        
        
        return saveOverwrite;
    }
}
