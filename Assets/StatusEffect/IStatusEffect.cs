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
        
    }

    public void UpdateStatus()
    {
        
    }

    public void ExitStatus()
    {
        
    }
}


