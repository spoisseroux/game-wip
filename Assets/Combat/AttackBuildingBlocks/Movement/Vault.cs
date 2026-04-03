using UnityEngine;

[CreateAssetMenu(fileName = "NewVault", menuName = "MovementEvent/Vault", order = 1)]
public class Vault : MovementEvent
{
    [SerializeField] public float height;

    public override void Begin(MovementContext context)
    {
        Vector3 addVel = new Vector3(0.0f, Mathf.Sqrt(height * -2 * -40.0f /* NEED TO REFACTOR TO GRAB CONTEXT GRAVITY*/), 0.0f);
        context.mover.AddVelocity(addVel, null);
    }

    public override void Tick(MovementContext context, float delta) { }

    public override void End(MovementContext context) { }
}
