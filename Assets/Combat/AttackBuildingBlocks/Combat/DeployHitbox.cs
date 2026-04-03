using UnityEngine;

[CreateAssetMenu(fileName = "NewDeployHitbox", menuName = "CombatEvent/DeployHitbox", order = 1)]
public class DeployHitbox : CombatEvent
{
    [SerializeField] private Box boxInfo;
    [SerializeField] private int hitCount;
    [SerializeField] private float hitboxDuration;

    // curve for moving position, rotation, etc.... have to figure this out later
    // also the issue of Hitbox ownership, do we want to just call hitbox.Reposition() from here
    // or do we say, hey, combatorch, here's what my animation curve is saying... pass this over to the hitbox for me?

    public override void Execute(CombatContext context)
    {
        Debug.Log("Deploying hitbox");
        context.orch.RegisterHitbox(boxInfo, hitCount, hitboxDuration);
    }

    // use this to update hitbox position i guess?
    public override void Tick(CombatContext context, float delta) { }

    public override void CleanUp(CombatContext context) { }
}