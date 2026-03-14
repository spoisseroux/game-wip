using UnityEngine;
/*  
    The context object which a movement event executes upon
*/
public struct MovementContext
{
    public Mover mover;
    public Vector3 direction;
    public float phaseTime;

    // attack added for weighted walking multiplier, other things??
    public AttackSO attack;
}

/*
    A single unit of movement execution
*/
[System.Serializable]
public abstract class MovementEvent : ScriptableObject
{
    [SerializeField] protected float duration;
    
    public abstract void Start(MovementContext context);
    public abstract void Update(MovementContext context);
    public abstract void End(MovementContext context);
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
[CreateAssetMenu(fileName = "NewPlayAnim", menuName = "MovementEvent/PlayAnimation", order = 1)]
public class PlayAnimation : MovementEvent
{
    [SerializeField] string animName;

    public override void Start(MovementContext context) { }

    public override void Update(MovementContext context) { }

    public override void End(MovementContext context) { }
}

[CreateAssetMenu(fileName = "NewDash", menuName = "MovementEvent/Dash", order = 1)]
public class Dash : MovementEvent
{
    [SerializeField] AnimationCurve curve;
    [SerializeField] float speed;

    public override void Start(MovementContext context)
    {
        context.direction = context.mover.transform.forward;
    }

    public override void Update(MovementContext context)
    {
        float t = context.phaseTime / duration;
        context.mover.SetVelocity(curve.Evaluate(t) * speed * context.direction, null);
    }

    public override void End(MovementContext context)
    {
        context.mover.SetVelocity(Vector3.zero, null);
    }
}

[CreateAssetMenu(fileName = "NewTeleport", menuName = "MovementEvent/Teleport", order = 1)]
public class Teleport : MovementEvent
{
    [SerializeField] public float distance;

    public override void Start(MovementContext context) { }

    public override void Update(MovementContext context) { }

    public override void End(MovementContext context) { }

    public void SetDirection(Vector3 direction) { }
}

[CreateAssetMenu(fileName = "NewRotate", menuName = "MovementEvent/Rotate", order = 1)]
public class Rotate : MovementEvent
{
    [SerializeField] public float revolutions; // ???
    [SerializeField] public float rotationSpeed;

    public override void Start(MovementContext context) { }

    public override void Update(MovementContext context) { }

    public override void End(MovementContext context) { }
}