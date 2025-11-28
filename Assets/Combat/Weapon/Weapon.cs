using System.Collections.Generic;
using UnityEngine;

// basically, the weapon 'holder' on our Player/Enemy/whatever... play round with inheritances
public class Weapon : MonoBehaviour, IWeapon, IHitboxSource
{
    // parent
    public PlayerCombatManager parent;

    // transform of owner
    public Transform parentTransform;

    // data
    public WeaponDataSO weaponData;
    public AttackSO currentAttack;

    // combo
    [SerializeField]
    private int currentComboIndex = -1;

    // model

    // hitboxes
    public List<Hitbox> activeHitboxes;

    #region MonoBehaviour
    private void Awake()
    {
        currentComboIndex = -1;
    }
    #endregion

    #region Gizmos
    public Color inactiveColor = Color.red;
    public Color collisionOpenColor = Color.green;

    private void OnDrawGizmos()
    {
        Gizmos.matrix = Matrix4x4.TRS(parentTransform.position, parentTransform.rotation, new Vector3(1,1,1));
        if (activeHitboxes.Count <= 0)
        {
            Gizmos.color = inactiveColor;
            Vector3 origin = parentTransform.position + parentTransform.forward * 1.5f;
            Vector3 extents = weaponData.basicAttackList[0].hitbox.GizmoXYZ();
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
        // check against attack progress
        float attackProgress = parent.GetAttackTimerProgress();

        // queue actions based on timing intervals defined in AttackSO
            // hitboxes
            // visual effects
            // yeah...

        
        // hardcoded progress value of ~10% completed to queue one hitbox ONLY
        if (attackProgress >= 0.88 && attackProgress <= 0.92 && activeHitboxes.Count < 1) {
            Attack();
        } 

        // tick all hitboxes & store any that have closed after doing so
        List<int> inactive = new List<int>();
        for (int i = 0; i < activeHitboxes.Count; i++)
        {
            activeHitboxes[i].Tick(delta);
            if (activeHitboxes[i].state == HitboxState.Closed)
                inactive.Add(i);
        }
        // late loop to remove all closed hitboxes, REMOVE BY GUID?
        foreach (var index in inactive)
        {
            activeHitboxes.RemoveAt(index);
        }
        // clear list
        inactive.Clear();
    }

    public void Attack()
    {
        Vector3 spawnPos = parentTransform.position + (parentTransform.forward * 0.5f);
        Quaternion spawnRot = parentTransform.rotation;
        Hitbox hbox = CreateHitbox(spawnPos, spawnRot);
        activeHitboxes.Add(hbox);
        hbox.Execute();
    }

    public AttackSO AttemptAttack()
    {   
        int attNumber = ResolveAttack();
        if (attNumber < 0)
            return null;

        currentAttack = weaponData.basicAttackList[attNumber];
        return currentAttack;
    }
    #endregion

    #region HitboxSource Interface
    public void CollisionedWith(Collider col)
    {
        IDamageable damageable = col.GetComponent<IDamageable>();
        // combatmanager.GetDamageModifiers();
        Debug.Log("doing damage: " + weaponData.basicAttackList[currentComboIndex].damage);
        damageable?.TakeDamage(weaponData.basicAttackList[currentComboIndex].damage);
    }

    public void CollisionedWith(IDamageable damageMe)
    {
        Debug.Log("doing damage: " + weaponData.basicAttackList[currentComboIndex].damage);
        damageMe?.TakeDamage(weaponData.basicAttackList[currentComboIndex].damage);
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
        currentComboIndex = -1;
    }
    #endregion

    #region Helpers
    // create hitbox for current attack from parent's position & rotation
    private Hitbox CreateHitbox(Vector3 spawnPos, Quaternion spawnRotation)
    {
        return new Hitbox(currentAttack.hitboxDuration,
                          spawnPos,
                          currentAttack.hitbox,
                          spawnRotation,
                          this, 
                          currentAttack.hitCount);
    }

    // determine which attack to choose based on current movement context, combo counter, etc.
    private int ResolveAttack()
    {   
        // combo chain not started
        if (currentComboIndex < 0)
            return ++currentComboIndex;

        // do we fall within window to continue a combo
        if (CanMoveToNextAttack())
        {
            return ++currentComboIndex;
        }

        // no attack to progress to, reset our combo and say no unsuccessful attack queue
        ResetWeaponComboCycle();
        return currentComboIndex;

    }

    private bool CanMoveToNextAttack()
    {
        float progress = parent.GetAttackTimerProgress();
        // do we fall within window to continue a combo, remember countdown timer progresses 1 --> 0
        return  (progress <= currentAttack.comboWindowStart) 
                &&
                (currentAttack.comboWindowEnd <= progress);
    }

    public void ResetWeaponComboCycle()
    {
        currentComboIndex = -1;
    }
    #endregion
}




// basically, how should logic flow in this circumstance
    // MovementManager receives input to attempt an attack, pipes it to CombatManager if state isn't AttackState
    // CombatManager calls Weapon.AttemptAttack( ... args ...)
    // Weapon determines where in the attack cycle it is based on some info from MovementManager
    // Weapon spits out which attackSO it should use next
    // CombatManager provides info from attackSO to MovementManager to override AttackState timer and pass StateTransition predicate
    // AttackState calls Enter, queue animation etc.
    // Weapon ticks forward, queueing hitboxes at designated times after windup, etc.