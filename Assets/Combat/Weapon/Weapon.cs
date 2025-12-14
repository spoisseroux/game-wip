using System.Collections.Generic;
using UnityEngine;

// basically, the weapon 'holder' on our Player/Enemy/whatever... play round with inheritances
/*
    SOME TODOS:
        - factor this out into a more generalizable weapon system
        - better source resolution instead of (as ____) casting
        - move the dependencies on parent PCM class away, 
            - combo window timing, hitbox deployment timing, stuff like parent.GetAttackProgress() should not exist....
        - add more general methods for weapons/attacks
        - ways for dynamically anchoring weapon models, gameobjects etc...

*/
public class Weapon : MonoBehaviour, IWeapon, IHitboxSource
{
    // parent
    public PlayerCombatManager parent;
    

    // transform of owner
    public Transform parentTransform;

    // positioning quirks
    public Vector3 yDisplace;

    // data
    public WeaponDataSO weaponData; // meant to be exchanged at runtime but need more services set up for this....
    public WeaponDataSO defaultWeapon; // so we make a default weapon that's separate from the save system for this
    public AttackSO currentAttack;

    // combo
    [SerializeField]
    private int currentComboIndex;

    // model

    // hitboxes
    public List<Hitbox> activeHitboxes;

    #region MonoBehaviour
    private void Awake()
    {
        currentComboIndex = -1;
    }

    private void Start()
    {
        weaponData = defaultWeapon;
    }
    #endregion

    #region Gizmos
    public Color inactiveColor = Color.red;
    public Color collisionOpenColor = Color.green;

    private void OnDrawGizmos()
    {
        foreach (var hitbox in activeHitboxes)
        {
            if (hitbox != null)
            {
                CheckGizmoColor(hitbox);
                HitboxGizmo gd = hitbox.GetGizmoData();
                Gizmos.matrix = Matrix4x4.TRS(gd.position, gd.rotation, Vector3.one);
                Gizmos.DrawWireCube(Vector3.zero, gd.extents);
            }
        }
        Gizmos.matrix = Matrix4x4.identity;
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
        Vector3 spawnPos = parentTransform.position + parentTransform.forward;
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
        return;
    }

    public void CollisionedWith(IDamageable damageMe)
    {
        // source resolution
        if (damageMe != parent as IDamageable)
            damageMe?.TakeDamage(weaponData.basicAttackList[currentComboIndex].damage);
    }

    public void CollisionedWith(IHittable hitMe)
    {
        hitMe?.Hit();
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

    #region Helpers
    // create hitbox for current attack from parent's position & rotation
    private Hitbox CreateHitbox(Vector3 spawnPos, Quaternion spawnRotation)
    {
        return new Hitbox(currentAttack.hitboxDuration,
                          spawnPos + yDisplace,
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
        // do we fall within window to continue a combo 
        // remember countdown timer progress starts at 1 and decreases to 0!!!
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