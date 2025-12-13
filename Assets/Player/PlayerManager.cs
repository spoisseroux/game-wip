using System;
using System.Collections.Generic;
using UnityEditor.Overlays;
using UnityEngine;

// require components
public class PlayerManager : MonoBehaviour
{
    // various system components
    public PlayerMovementManager movementManager; 
    public PlayerCombatManager combatManager;
    public RuneHolder runeHolder;

    #region Monobehavior
    private void Awake()
    {
        /*
            UNCOMMENT FOR WHEN ACTUALLY SWITCHING SCENES!!! OTHERWISE YOU SPAWN INTO OBLIVION LOL
            UNCOMMENT FOR WHEN ACTUALLY SWITCHING SCENES!!! OTHERWISE YOU SPAWN INTO OBLIVION LOL
            UNCOMMENT FOR WHEN ACTUALLY SWITCHING SCENES!!! OTHERWISE YOU SPAWN INTO OBLIVION LOL
        */
        // DontDestroyOnLoad(gameObject);
        movementManager = GetComponent<PlayerMovementManager>();
        combatManager = GetComponent<PlayerCombatManager>();
        runeHolder = GetComponent<RuneHolder>();
    }

    private void Start()
    {
        if (SceneTransitionManager.playerSpawnData != null)
        {
            // if so, read scene transition data
            this.transform.position = SceneTransitionManager.playerSpawnData.position;
            this.transform.rotation = SceneTransitionManager.playerSpawnData.rotation;

            // reset
            SceneTransitionManager.ResetSpawnTransform();
        }
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
        combatManager.SetHealth(data.health); // hacky for now
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
        saveOverwrite.health = combatManager.GetHealth();
        // leave til fixed saveOverwrite.weapon = ...

        // rune holder
        saveOverwrite.runeIDs = runeHolder.SetSavedRunes();
        
        
        return saveOverwrite;
    }
}
