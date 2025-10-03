using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// require components
public class PlayerManager : MonoBehaviour
{
    // various system components
    public PlayerMovementManager movementManager; 
    public PlayerCombatManager combatManager;

    #region Monobehavior
    private void Awake()
    {
        movementManager = GetComponent<PlayerMovementManager>();
        combatManager = GetComponent<PlayerCombatManager>();

        // give camera our info
        // PlayerCamera.instance.player = this;
    }

    // for camera, singleton call
    private void LateUpdate()
    {
        PlayerCamera.instance.HandleAllCameraActions();
    }
    #endregion
}
