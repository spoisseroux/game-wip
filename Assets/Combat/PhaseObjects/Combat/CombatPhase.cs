using UnityEngine;
using System.Collections.Generic;

/*
    A single unit of commands that execute in tandem within an Attack
*/
[System.Serializable]
public class CombatPhase
{
    [SerializeReference] public List<CombatEvent> combatEvents;
    public float duration { get { return CalculateDuration(); } }

    public void Begin(CombatContext ctx) 
    {
        ctx.phaseTime = 0.0f;
        foreach (var evnt in combatEvents)
            evnt.Execute(ctx);
    }

    public void Tick(CombatContext ctx, float delta)
    {
        for (int i = 0; i < combatEvents.Count; i++)
        {
            // check if event is done, CLEAN UP HERE or LATER?
            if (ctx.phaseTime > combatEvents[i].duration)
                continue;
            
            // update 
            combatEvents[i].Tick(ctx, delta);
        }
    }

    public void End(CombatContext ctx)
    {
        foreach (var evnt in combatEvents)
            evnt.CleanUp(ctx);
    }

    public float CalculateDuration()
    {
        float longestDuration = float.MinValue;
        for (int i = 0; i < combatEvents.Count; i++)
        {
            if (longestDuration <= combatEvents[i].duration)
                longestDuration = combatEvents[i].duration;
        }

        return longestDuration;
    }
}