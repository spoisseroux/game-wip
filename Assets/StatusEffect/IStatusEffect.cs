using UnityEngine;
using System.Collections;

public interface IStatusEffect
{
    void ApplyStatus(GameObject target);
    void UpdateStatus();
    void ExitStatus();
}

// does this need to be 1-1 between buff and factory?
public class StatusEffectFactory : ScriptableObject {
    // maybe the StatusEffectSO goes here
    StatusEffectSO moveBuff;

    public MovementSpeedEffect CreateMovementBuff()
    {
        return new MovementSpeedEffect(moveBuff);
    }
}

// concrete effects
public class MovementSpeedEffect : IStatusEffect
{
    private StatusEffectSO statusEffect;
    GameObject target;
    CountdownTimer timer;

    public MovementSpeedEffect(StatusEffectSO se) { 
        statusEffect = se; 
        timer = new CountdownTimer(statusEffect.duration);
    }

    public void ApplyStatus(GameObject t)
    {
        target = t;
        PlayerMovementManager motor = target.GetComponent<PlayerMovementManager>();
        motor.ChangeAdditiveBonus(statusEffect.additiveValue);
        motor.ChangeMultBonus(statusEffect.multValue);
        timer.Start();
    }

    public void UpdateStatus()
    {
        timer.Tick(Time.deltaTime);
        if (timer.progress <= 0)
            ExitStatus();
    }

    public void ExitStatus()
    {
        PlayerMovementManager motor = target.GetComponent<PlayerMovementManager>();
        motor.ChangeAdditiveBonus(-1 * statusEffect.additiveValue);
        motor.ChangeMultBonus(-1 * statusEffect.multValue);
        timer = null;
    }
}


