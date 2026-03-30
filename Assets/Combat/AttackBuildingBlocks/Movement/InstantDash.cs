using UnityEngine;

[CreateAssetMenu(fileName = "NewDash", menuName = "MovementEvent/Dash", order = 1)]
public class InstantDash : MovementEvent
{
    [Header("Movespeed")]
    [SerializeField] AnimationCurve curve;
    [SerializeField] float speed;

    public override void Begin(MovementContext context)
    {
        context.startDirection = context.mover.transform.forward;
        context.mover.SetNewRotation(context.startDirection, null);
    }

    public override void Tick(MovementContext context, float delta)
    {
        float t = context.phaseTime / duration;
        context.mover.SetVelocity(curve.Evaluate(t) * speed * context.startDirection, null);
    }

    public override void End(MovementContext context)
    {
        context.mover.SetVelocity(Vector3.zero, null);
    }
}
