using UnityEngine;

[CreateAssetMenu(fileName = "NewPlaySFX", menuName = "CombatEvent/PlaySFX", order = 1)]
public class PlaySFX : CombatEvent
{
    [SerializeField] AudioClip sfxClip;

    public override void Execute(CombatContext context) { }

    public override void Tick(CombatContext context, float delta) { }

    public override void CleanUp(CombatContext context) { }
}
