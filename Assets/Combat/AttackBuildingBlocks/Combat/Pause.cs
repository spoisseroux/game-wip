using UnityEngine;

[CreateAssetMenu(fileName = "NewPause", menuName = "CombatEvent/Pause", order = 1)]
public class Pause : CombatEvent
{
    public override void Execute(CombatContext context) { Debug.Log("Attack Pause Started"); }

    public override void Tick(CombatContext context, float delta) { }

    public override void CleanUp(CombatContext context) { Debug.Log("Attack Pause Ended"); }
}
