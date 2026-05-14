using System.Collections.Generic;
using UnityEngine;

// basically, the weapon 'holder' on our Player/Enemy/whatever... play round with inheritances
/*
    REFACTOR IDEAS:
        - does a weapon really need to understand time? can we make attacks and other things tick instead?
        - instead, i'd like to convert this into a WeaponHolder object that's only responsibilities are: 
                store weapon data
                receive commands to attack
                resolve which attack to queue based on attack/player/movement/etc
        - another idea is an attack orchestrator, which does similar for attack objects?


    IDEALLY:
        - this holds WeaponDataSO data as a WeaponHolder object
        - the parent CombatOrchestrator object receives an input to attack
        - it pings this down to the WeaponHolder, essentially to ask "hey, what attack do I run?"
        - WeaponHolder resolves this giving key context from CombatOrchestrator
        - returns the resolved AttackSO timeline up to the CombatOrchestrator


    BENEFITS:
        - WeaponHolder retains a singular focus, hold data and serve data related to it
        - CombatOrchestator can resolve context and command requests first without dragging the WeaponHolder into it
            - Oh, can I even attack?
            - Should it be aerial?
            - Am I at a point in combo where I can chain to the next?
        - WeaponHolder maintains some combo state, stance, equipped item 

                

*/
public class WeaponHolder : MonoBehaviour, IWeapon
{
    // parent
    public CombatOrchestrator parent;

    // weapon data
    public WeaponDataSO weaponData; // meant to be exchanged at runtime but need more services set up for this....
    public WeaponDataSO defaultWeapon; // so we make a default weapon that's separate from the save system for this

    // model and placement --> attach script to root player object, drag transform of hand to here, set prefab.transform.parent as this
    [SerializeField] Transform anchorLocation;
    [SerializeField] GameObject weaponPrefab;

    // hitboxes
    public List<Hitbox> activeHitboxes;

    #region MonoBehaviour
    private void Awake()
    {
        
    }

    private void Start()
    {
        weaponData = defaultWeapon;
    }
    #endregion

    #region Weapon Interface & Helpers
    /*
        Upon receiving a request to queue an Attack 
        Compare the current Attack and the phase its currently in
        Return the next attack to queue based on results
    */
    public AttackSO AttemptAttack(AttackSO attack, CombatPhase currentPhase)
    {   
        AttackSO resolved = null;

        // if null, start at root attack
        if (attack == null)
        {
            resolved = weaponData.basicAttackList[0];
            Debug.Log("WeaponHolder()::Resolved to --> Attack: " + resolved + "Combo Index: " + 0);
        }

        // if mid-attack, check all combat events in this phase for combo window
        else
        {
            for (int i = 0; i < currentPhase.combatEvents.Count; i++)
            {
                // assume that combo-ending attacks DO NOT have a ListenForComboInput object 
                // so +1'ing the index shouldn't go out of range of weapon's attack list
                if (currentPhase.combatEvents[i].ToString() == "ListenForComboInput")
                {
                    resolved = weaponData.basicAttackList[attack.comboIndex + 1];
                    Debug.Log("WeaponHolder()::Resolved to --> Attack: " + resolved + "Combo Index: " + (attack.comboIndex + 1));
                    break;
                }
            }
        }

        // null if no new attack, AttackSO object if yes successful attack queue
        return resolved;
    }
    #endregion

    #region Swapping
    public void SwapWeapon(WeaponDataSO context)
    {
        RemoveWeapon();
        EquipWeapon(context);
    }

    private void EquipWeapon(WeaponDataSO context)
    {
        // load weapondataSO
        weaponData = context;
        // load model
        // play sfx or anims
    }

    private void RemoveWeapon()
    {
        
    }

    public void LoadWeaponFromSave(WeaponDataSO weapon)
    {
        if (defaultWeapon != null)
        {
            weaponData = defaultWeapon;
            return;
        }
        weaponData = weapon;
    }
    #endregion

}