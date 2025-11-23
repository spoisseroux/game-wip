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

    #region MonoBehaviour
    private void Awake()
    {
        
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
            Vector3 extents = weaponData.hitboxData.GizmoXYZ();
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
        foreach (var hitbox in activeHitboxes)
        {
            hitbox.Tick(delta);
        }
    }

    public void Attack()
    {
        // spawn a new hitbox, push it to list, execute, and hook event up
        Debug.Log(parentTransform.position + parentTransform.forward * 0.5f);
        Hitbox hbox = new Hitbox(weaponData.length, 
                                 parentTransform.position + parentTransform.forward * 0.5f,
                                 weaponData.hitboxData, 
                                 parentTransform.rotation, 
                                 this);
        activeHitboxes.Add(hbox);
        hbox.Execute();
        hbox.unload += RemoveHitbox;
    }
    #endregion

    #region HitboxSource Interface
    public void CollisionedWith(Collider col)
    {
        IDamageable damageable = col.GetComponent<IDamageable>();
        damageable?.TakeDamage(weaponData.damage);
    }
    #endregion

    #region Swapping
    public void SwapWeapon(WeaponDataSO context)
    {

    }

    public void EquipWeapon(WeaponDataSO context)
    {

    }

    public void RemoveWeapon()
    {

    }
    #endregion

    #region Helpers
    private void RemoveHitbox(Hitbox h)
    {
        Debug.Log("Removing hitbox");
        activeHitboxes.Remove(h);
        h.unload -= RemoveHitbox;
    }
    #endregion
}