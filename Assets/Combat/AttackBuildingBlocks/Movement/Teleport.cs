using UnityEngine;

[CreateAssetMenu(fileName = "NewTeleport", menuName = "MovementEvent/Teleport", order = 1)]
public class Teleport : MovementEvent
{
    [SerializeField] public float distance;

    public override void Begin(MovementContext context) { }

    public override void Tick(MovementContext context, float delta) { }

    public override void End(MovementContext context) { }

    public void SetDirection(Vector3 direction) { }
}