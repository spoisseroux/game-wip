using UnityEngine;
using System.Collections.Generic;

/*
    A single unit of commands that execute in tandem within an Attack
*/
[System.Serializable]
public class CombatPhase
{
    [SerializeField] List<CombatEvent> combatEvents;
    public float duration;

    public void Begin(CombatOrchestrator unit) 
    {

    }

    public void Update(CombatOrchestrator unit, float delta)
    {
        
    }

    public void End(CombatOrchestrator unit)
    {
        
    }
}