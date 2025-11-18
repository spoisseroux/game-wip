using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatManager : MonoBehaviour, IDamageable
{
    // player context
    PlayerManager player;

    // movement manager link?

    // health component here

    // active hurtbox here [can be swapped upon dash/etc.]
    private Hurtbox activeHurtbox = null;
    [SerializeField]
    private List<Hurtbox> playerHurtboxes = new List<Hurtbox>();

    [SerializeField] Weapon equippedWeapon; 
    // weapon having its own Update()
    // this can either be data that passes how to construct attacks to this object
    // or this can be it's own API for generating attacks and such --> this probably better for stacking effects and more complicated behavior

    // Timer object for tracking attacks, check equipped Weapon for it's attackspeed in Attempt()

    #region Monobehavior
    void Start()
    {
        player = GetComponent<PlayerManager>();
    }

    void Update()
    {
        equippedWeapon.Tick(Time.deltaTime);
    }
    #endregion

    #region Attack

    public void AttemptAttack()
    {
        equippedWeapon.Attack();
    }

    #endregion

    #region Damage Interface
    public void TakeDamage(int damage)
    {
        // blehhhhh i'm invincible for now!!!
    }
    #endregion
}