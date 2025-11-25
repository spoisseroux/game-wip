using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatManager : MonoBehaviour, IDamageable
{
    // player context
    PlayerManager player;

    // movement manager link?

    // health component here
    int health = 100;

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
        player = GetComponent<PlayerManager>();
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
        if (attack != null) {
            timer = new CountdownTimer(attack.attackDuration);
        }
        return attack;
    }

    public void BeginAttack()
    {
        if (timer != null)
        {
            Debug.Log("Starting timer in CombatManager");
            timer.Start();
        }
    }

    public float GetAttackTimerProgress()
    {
        if (timer != null)
            return timer.progress;

        return 1.1f; // out of bounds value for flagging
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
    }
    #endregion
}