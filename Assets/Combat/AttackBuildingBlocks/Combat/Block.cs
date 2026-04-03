using UnityEngine;

[CreateAssetMenu(fileName = "NewBlock", menuName = "CombatEvent/Block", order = 1)]
public class Block : CombatEvent
{
    public override void Execute(CombatContext context) { }

    public override void Tick(CombatContext context, float delta) { }

    public override void CleanUp(CombatContext context) { }
}