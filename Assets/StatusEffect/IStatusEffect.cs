using UnityEngine;

public interface IStatusEffect
{
    void ApplyStatus(GameObject target);
    bool UpdateStatus();
    void ExitStatus();
}

// does this need to be 1-1 between buff and factory?
public class StatusEffectFactory : ScriptableObject {
    // maybe the StatusEffectSO goes here

    public MovementSpeedEffect CreateMovementBuff()
    {
        return null;//new MovementSpeedEffect(statusEffect);
    }
}

// concrete effects
public class MovementSpeedEffect : IStatusEffect
{
    private StatusEffectSO statusEffect;

    public MovementSpeedEffect(StatusEffectSO se) { statusEffect = se; }

    public void ApplyStatus(GameObject target)
    {
        throw new System.NotImplementedException();
    }

    public bool UpdateStatus()
    {
        throw new System.NotImplementedException();
    }

    public void ExitStatus()
    {
        throw new System.NotImplementedException();
    }
}


