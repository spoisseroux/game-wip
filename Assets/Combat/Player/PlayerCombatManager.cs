using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatManager : MonoBehaviour, IDamageable
{
    // player context
    PlayerManager player;

    // movement manager link?

    // health component here

    // list of active hitboxes here
    private List<Hitbox> activeHitboxes = new List<Hitbox>();

    // active hurtbox here [can be swapped upon dash/etc.]
    private Hurtbox activeHurtbox = null;
    [SerializeField]
    private List<Hurtbox> playerHurtboxes = new List<Hurtbox>();

    // Weapon equippedWeapon; weapon having its own Update()
    // this can either be data that passes how to construct attacks to this object
    // or this can be it's own API for generating attacks and such --> this probably better for stacking effects and more complicated behavior

    // Timer object for tracking attacks, check equipped Weapon for it's attackspeed in Attempt()
    public float timeSinceAttack = 0.0f;

    // Basic Attack
    [Header("Basic Attack")]
    // compose into Hitbox object at some point, which could eventually be moved under a Weapon's data!!
    [SerializeField] Vector3 hitboxPos; // relative to Player
    [SerializeField] float hitboxLength;
    [SerializeField] float hitboxHeight;
    [SerializeField] float hitboxWidth;
    // timing data
    [SerializeField] float activationTime;
    [SerializeField] float timeActive;
    [SerializeField] float cooldownTime;

    #region Monobehavior
    void Start()
    {
        player = GetComponent<PlayerManager>();
    }

    void Update()
    {
        foreach (Hitbox activeBox in activeHitboxes)
        {
            activeBox.Tick(Time.deltaTime, this.transform.position);
        }
    }
    #endregion

    #region Attack

    public void AttemptBasicAttack()
    {
        
    }

    #endregion

    #region Damage Interface
    public void TakeDamage(int damage)
    {
        // blehhhhh i'm invincible for now!!!
    }
    #endregion
}