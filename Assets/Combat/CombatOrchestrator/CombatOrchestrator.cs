using UnityEngine;
using System.Collections.Generic;

public class CombatOrchestrator : MonoBehaviour, IHittable, IHitboxSource
{
    // health component
    [SerializeField] float health;

    // weapon object.... what actually is this ??
    WeaponHolder equippedWeapon;

    // active hitboxes
    List<Hitbox> activeHitboxes;

    #region MonoBehaviour
    private void Awake() {}
    private void Start() {}

    private void Update()
    {
        // tick active hitboxes
        List<int> inactive = TickActiveHitboxes();

        // remove inactive
        CleanDeadHitboxes(inactive);
    }
    #endregion

    #region Weapon Helpers
    public WeaponDataSO GetEquippedWeapon()
    {
        return equippedWeapon.weaponData;
    }

    public void SetEquippedWeapon(WeaponDataSO weapon)
    {
        equippedWeapon.LoadWeaponFromSave(weapon);
    }
    #endregion

    #region Hittable Interface
    public void Hit(HitboxRecord hitboxRecord)
    {
        return;
    }
    #endregion

    #region Hitbox Ownership
    public void SpawnHitbox(Hitbox hitbox)
    {
        
    }

    // create hitbox for current attack from parent's position & rotation
    private Hitbox CreateHitbox(Vector3 spawnPos, Quaternion spawnRotation)
    {
        /*
        return new Hitbox(currentAttack.hitboxDuration,
                          spawnPos + yDisplace,
                          currentAttack.hitbox,
                          spawnRotation,
                          this,
                          currentAttack.hitCount);
        */
        return null;
    }
    
    private List<int> TickActiveHitboxes()
    {
        List<int> inactive = new List<int>();
        for (int i = 0; i < activeHitboxes.Count; i++)
        {
            activeHitboxes[i].Tick(Time.deltaTime);
            if (!activeHitboxes[i].Active)
                inactive.Add(i);
        }

        return inactive;
    }

    private void CleanDeadHitboxes(List<int> inactiveIndices)
    {
        for (int index = inactiveIndices.Count - 1; index >= 0; index--)
        {
            int removePosition = inactiveIndices[index];
            activeHitboxes.RemoveAt(removePosition);
        }
    } 
    #endregion

    #region HitboxSource Interface
    public void OnHitConfirmed(HitboxRecord hitMe)
    {
        // uhhh tbd. get IHittable and do hitMe.hittable?.Hit()
        // EVERY IHittable implements it in their own way, SaveGem, CombatOrchestrator, Wall idkkkkk
    }
    #endregion

    #region Attack Request
    public AttackSO AttemptAttack()
    {
        // uhhhh fill this is with whatever way to check this you want
        if (true) 
            return null;
        return equippedWeapon.AttemptAttack();
    }
    #endregion

    #region Health
    public void SetHealth(float healthIn)
    {
        health = healthIn;
    }

    public float GetHealth()
    {
        return health;
    }
    #endregion
}