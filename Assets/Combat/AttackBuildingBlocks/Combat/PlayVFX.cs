using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayVFX", menuName = "CombatEvent/PlayVFX", order = 1)]
public class PlayVFX : CombatEvent
{
    [SerializeField] GameObject vfxPrefab;

    public override void Execute(CombatContext context) { }

    public override void Tick(CombatContext context, float delta) { }

    public override void CleanUp(CombatContext context) { }
}
