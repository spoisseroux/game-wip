using UnityEngine;

[CreateAssetMenu(fileName = "NewComboInput", menuName = "CombatEvent/ComboInput", order = 1)]
public class ListenForComboInput : CombatEvent
{
    public override void Execute(CombatContext context) { Debug.Log("Combo input window started"); }

    public override void Tick(CombatContext context, float delta) { }

    public override void CleanUp(CombatContext context) { Debug.Log("Combo input window ended"); }
}
