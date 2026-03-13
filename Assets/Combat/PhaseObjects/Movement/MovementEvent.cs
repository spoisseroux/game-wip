using UnityEngine;

[System.Serializable]
public abstract class MovementEvent
{
    [SerializeField] protected float duration;
    [SerializeField] protected float instant;

    public abstract void Start(Mover caster);
    public abstract void Update(Mover caster);
    public abstract void End(Mover caster);
}

// examples
// play animation
// dash
// walk
// rotate
// leap
// sidestep/strafe move

/*
    These should appear as serializable objects in an AttackSO...
    But how can we populate it?
    Making them a ScriptableObject? 
    But then they're not constructable and easily edited to use the same object with different values
    ughhhh

    Maybe we just make definitions programmatically?
*/
public class PlayAnimation : MovementEvent
{
    [SerializeField] string animName;

    public override void Start(Mover caster) { }

    public override void Update(Mover caster) { }

    public override void End(Mover caster) { }
}

public class Dash : MovementEvent
{
    [SerializeField] AnimationCurve curve;
    [SerializeField] float startSpeed;
    [SerializeField] float endSpeed;
    [SerializeField] float rotationSpeed;

    public override void Start(Mover caster) { }

    public override void Update(Mover caster) { }

    public override void End(Mover caster) { }

    public void SetDirection(Vector3 direction) { }
}