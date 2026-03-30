using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayAnim", menuName = "MovementEvent/PlayAnimation", order = 1)]
public class PlayAnimation : MovementEvent
{
    [SerializeField] string animName;

    public override void Begin(MovementContext context) { }

    public override void Tick(MovementContext context, float delta) { }

    public override void End(MovementContext context) { }
}