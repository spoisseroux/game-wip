using System;
using System.Collections.Generic;
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
        //this.transform.position = data.position;

        // movement manager saved objects

        // combatManager.Equip(data.weaponData);
        // combatManager.SetHealth(data.health);
        combatManager.TakeDamage(-1 * data.health); // hacky for now

        // rune holder
        // runeHolder.LoadRunes(List<RuneDataSO> runes);
    }

    public PlayerData GatherSaveData()
    {
        PlayerData hey = new PlayerData();
        hey.health = 14690;
        return null;
    }
}
