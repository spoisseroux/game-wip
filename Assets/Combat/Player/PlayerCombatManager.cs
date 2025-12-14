using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatManager : MonoBehaviour, IDamageable
{
    // player/movement manager link?

    // health component here
    [SerializeField]
    private int health = 100;
    
    // death event
    public delegate void PlayerDiedEvent();
    public event PlayerDiedEvent Died;

    // active hurtbox here [can be swapped upon dash/etc.]
    private Hurtbox activeHurtbox = null;
    [SerializeField]
    private List<Hurtbox> playerHurtboxes = new List<Hurtbox>();

    [SerializeField] Weapon equippedWeapon;

    // timer
    private CountdownTimer timer;

    #region Monobehavior
    void Start()
    {
        
    }

    void Update()
    {
        if (equippedWeapon != null && timer != null) {
            timer.Tick(Time.deltaTime);
            equippedWeapon.Tick(Time.deltaTime);
        }

    }
    #endregion

    #region Attack
    public AttackSO AttemptAttack()
    {
        AttackSO attack = equippedWeapon.AttemptAttack();
        return attack;
    }

    public void BeginAttack(float duration)
    {
        timer = new CountdownTimer(duration);
        timer.Start();
    }

    public float GetAttackTimerProgress()
    {
        if (timer != null)
            return timer.progress;

        return 10f; // out of bounds value for flagging
    }

    public void ResetWeaponCycle()
    {
        equippedWeapon.ResetWeaponComboCycle();
        timer = null;
    }
    #endregion

    #region Damage Interface
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Died?.Invoke();
        }
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

    #region Health
    public int GetHealth()
    {
        return health;
    }

    public void SetHealth(int h)
    {
        health = h;
    }
    #endregion

}