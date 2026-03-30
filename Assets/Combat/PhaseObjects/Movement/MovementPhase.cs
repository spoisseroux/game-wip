using UnityEngine;
using System.Collections.Generic;

/*
    Essentially, a list of these get tossed off to the MovementController or FSM owner of a given Unit
    This controller will hand the data off to the given AttackState and let it populate the routines within 

    These phases should boil down to a bunch of calls like motor.AddVelocity() motor.AddRotation() etc.

    But the MovementEvent actually is... hmm.

    Should it be actions that just exist in a moment and affect a random Motor?

    Should they be little packages of data we pass off to FSM and affect the motor?

    like 
    Start(Motor caster) { 
        motor.AddVelocity(dashspeed * dir); 
    }

    or 

*/


/*
    A single unit of movement commands that execute in tandem
*/
[System.Serializable]
public class MovementPhase
{
    [SerializeReference] public List<MovementEvent> movementEvents;

    public float duration { get { return CalculateDuration(); } }

    public void Begin(MovementContext ctx)
    {
        ctx.phaseTime = 0.0f;
        for (int i = 0; i < movementEvents.Count; i++)
        {
            movementEvents[i].Begin(ctx);
        }
    }

    public void Tick(MovementContext ctx, float delta) 
    {
        for (int i = 0; i < movementEvents.Count; i++)
        {
            // check if individual event is done
            if (ctx.phaseTime >= movementEvents[i].duration)
                continue;
            
            // update 
            movementEvents[i].Tick(ctx, delta);
        }
    }

    public void End(MovementContext ctx) 
    {
        for (int i = 0; i < movementEvents.Count; i++)
        {
            movementEvents[i].End(ctx);
        }
    }

    public float CalculateDuration()
    {
        float longestDuration = float.MinValue;
        for (int i = 0; i < movementEvents.Count; i++)
        {
            if (longestDuration <= movementEvents[i].duration)
                longestDuration = movementEvents[i].duration;
        }

        return longestDuration;
    }
}