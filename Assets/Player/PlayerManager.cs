using System.Collections.Generic;
using UnityEngine;

// require components
public class PlayerManager : MonoBehaviour
{
    // various system components
    public PlayerMovementManager movementManager; 
    public PlayerCombatManager combatManager;

    // save data for player
    private class PlayerSaveData : ISaveData
    {
        private List<RuneDataSO> runes;
        private WeaponDataSO weapon;
        private int health;
    }

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

        // give camera our info
        // PlayerCamera.instance.player = this;

        // first game start load
        this.transform.position = SceneTransitionManager.startPosition;
        this.transform.rotation = SceneTransitionManager.startRotation;
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
}
