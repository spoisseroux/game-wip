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
    [SerializeField] List<MovementEvent> movementEvents;

    public float duration; // how do we actually measure this?

    public MovementPhase(List<MovementEvent> events)
    {
        movementEvents = events;
    }

    public void Begin(Mover unit)
    {
        for (int i = 0; i < movementEvents.Count; i++)
        {
            movementEvents[i].Start(unit);
        }
    }

    public void Update(Mover unit, float delta) 
    {   
        for (int i = 0; i < movementEvents.Count; i++)
        {
            // how to use delta
            movementEvents[i].Update(unit);
        }
    }

    public void End(Mover unit) 
    {
        for (int i = 0; i < movementEvents.Count; i++)
        {
            movementEvents[i].End(unit);
        }
    }
}