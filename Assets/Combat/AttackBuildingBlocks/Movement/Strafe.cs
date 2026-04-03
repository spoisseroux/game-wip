using UnityEngine;

[CreateAssetMenu(fileName = "NewStrafe", menuName = "MovementEvent/Strafe", order = 1)]
public class Strafe : MovementEvent
{
    [SerializeField] public float speedModifier;

    public override void Begin(MovementContext context) { }

    public override void Tick(MovementContext context, float delta)
    {
        context.mover.AddVelocity(context.mover.movementSettings.walkSpeed 
                                    * speedModifier 
                                    * context.updatingDirection, null);
    }

    public override void End(MovementContext context) { }
}
