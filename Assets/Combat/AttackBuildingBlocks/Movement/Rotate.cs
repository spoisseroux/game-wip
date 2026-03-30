using UnityEngine;

[CreateAssetMenu(fileName = "NewRotate", menuName = "MovementEvent/Rotate", order = 1)]
public class Rotate : MovementEvent
{
    [SerializeField] public float revolutions; // ???
    [SerializeField] public float rotationSpeed;

    public override void Begin(MovementContext context) { }

    public override void Tick(MovementContext context, float delta) { }

    public override void End(MovementContext context) { }
}
