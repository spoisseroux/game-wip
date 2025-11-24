using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// basically, the weapon 'holder' on our Player/Enemy/whatever
public class Weapon : MonoBehaviour, IWeapon, IHitboxSource
{
    // parent
    PlayerCombatManager parent;

    // offset pos
    public Transform parentTransform;

    // data
    public WeaponDataSO weaponData;

    // model

    // hitboxes
    public List<Hitbox> activeHitboxes;

    // combo
    [SerializeField]
    private int currentCombo = 0;

    #region MonoBehaviour
    private void Awake()
    {
        currentCombo = 0;
    }

    private void Start()
    {

    }

    private void Update()
    {

    }
    #endregion

    #region Gizmos
    public Color inactiveColor = Color.green;
    public Color collisionOpenColor = Color.red;

    private void OnDrawGizmos()
    {
        // Gizmos.matrix = Matrix4x4.TRS(parentTransform.position, parentTransform.rotation, new Vector3(1,1,1));
        if (activeHitboxes.Count <= 0)
        {
            Gizmos.color = inactiveColor;
            Vector3 origin = parentTransform.position + parentTransform.forward * 1.5f;
            Vector3 extents = weaponData.basicAttackList[currentCombo].hitbox.GizmoXYZ();
            Gizmos.DrawWireCube(origin, extents);
        }
        else
        {
            foreach (var hitbox in activeHitboxes)
            {
                if (hitbox != null)
                {
                    CheckGizmoColor(hitbox);
                    Vector3 origin = parentTransform.position + parentTransform.forward * 1.5f;
                    Vector3 extents = hitbox.box.GizmoXYZ();
                    Gizmos.DrawWireCube(origin, extents);
                }
            }
        }
    }

    private void CheckGizmoColor(Hitbox h)
    {
        switch (h.state)
        {
            case HitboxState.Closed:
                Gizmos.color = inactiveColor;
                break;
            case HitboxState.Open:
                Gizmos.color = collisionOpenColor;
                break;
        }
    }
    #endregion

    #region Weapon Interface
    public void Tick(float delta)
    {
        // tick all hitboxes & store any that have closed after doing so
        List<int> remove = new List<int>();
        for (int i = 0; i < activeHitboxes.Count; i++)
        {
            activeHitboxes[i].Tick(delta);
            if (activeHitboxes[i].state == HitboxState.Closed)
                remove.Add(i);
        }
        // late loop to remove all closed hitboxes
        foreach (var index in remove)
        {
            activeHitboxes.RemoveAt(index);
        }
        // clear list
        remove.Clear();
    }

    public void Attack()
    {
        Vector3 spawnPos = parentTransform.position + (parentTransform.forward * 0.5f);
        Quaternion spawnRot = parentTransform.rotation;
        Hitbox hbox = CreateHitbox(spawnPos, spawnRot);
        activeHitboxes.Add(hbox);
        hbox.Execute();
    }

    public bool AttemptAttack()
    {
        // replace if with some of the logic checks below 
            // basically, how should logic flow in this circumstance
                // MovementManager receives input to attempt an attack, pipes it to CombatManager if state isn't AttackState
                // CombatManager calls Weapon.AttemptAttack( ... args ...)
                // Weapon determines where in the attack cycle it is based on some info from MovementManager
                // Weapon spits out which attackSO it should use next
                // CombatManager provides info from attackSO to MovementManager to override AttackState timer and pass StateTransition predicate
                // AttackState calls Enter, queue animation etc.
                // after windup period, AttackState calls CombatManager Weapon.Attack() to actually create hitboxes and increment combo counter
        
        ResolveAttack();


        return true;
    }
    #endregion

    #region HitboxSource Interface
    public void CollisionedWith(Collider col)
    {
        IDamageable damageable = col.GetComponent<IDamageable>();
        damageable?.TakeDamage(weaponData.basicAttackList[currentCombo].damage);
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
        // remove model
        // remove weapondataSO
        weaponData = null;
        // reset combo
        currentCombo = 0;
    }
    #endregion

    #region Helpers
    // create hitbox for current attack from parent's position & rotation
    private Hitbox CreateHitbox(Vector3 spawnPos, Quaternion spawnRotation)
    {
        AttackSO attack = weaponData.basicAttackList[currentCombo];
        return new Hitbox(attack.hitboxDuration,
                          spawnPos,
                          attack.hitbox,
                          spawnRotation,
                          this);
    }

    // determine which attack to choose based on current movement context, combo counter, etc.
    private int ResolveAttack()
    {
        return 0;
    }

    private bool CheckAttackCycle()
    {
        return true;
    }

    public void ResetWeaponComboCycle()
    {
        currentCombo = 0;
    }
    #endregion
}