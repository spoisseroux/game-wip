using UnityEngine;
using System.Collections.Generic;

/*
    A single unit of commands that execute in tandem within an Attack
*/
[System.Serializable]
public class CombatPhase
{
    [SerializeReference] public List<CombatEvent> combatEvents;
    public float duration;

    public void Begin(CombatContext ctx) 
    {

    }

    public void Update(CombatContext ctx, float delta)
    {
        
    }

    public void End(CombatContext ctx)
    {
        
    }
}