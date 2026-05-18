using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    // various system components
    public PlayerMovementManager movementManager; 
    public CombatOrchestrator combatOrchestrator;
    //public GlyphHolder glpyhHolder;
    public PlayerSaveModule saveModule;

    #region Monobehavior
    private void Awake()
    {
        movementManager = GetComponent<PlayerMovementManager>();
        combatOrchestrator = GetComponent<CombatOrchestrator>();
        //glyphHolder = GetComponent<GlyphHolder>();

        // initialize save
        saveModule = new PlayerSaveModule(this);
        saveModule.Initialize();
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
        //glpyhHolder.LoadRunes(data.runeIDs);
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
        
        // weapon

        // rune holder
        //saveOverwrite.glyphIDs = glyphHolder.SetSavedRunes();
        
        
        return saveOverwrite;
    }
}
