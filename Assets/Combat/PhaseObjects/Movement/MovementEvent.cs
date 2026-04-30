using UnityEngine;
/*  
    The context object which a movement event executes upon
*/
public class MovementContext
{
    public Mover mover;
    public Vector3 startDirection;
    public Vector3 updatingDirection;
    public float phaseTime; // count up from 0.0f in phase, then if 

    // attack added for weighted walking multiplier, other things??
    public float walkSpeedModifier;
    public float jumpModifier;

    public MovementContext(Mover moverIn) { mover = moverIn; }
    public MovementContext(Mover moverIn, Vector3 startDir, Vector3 updatingDir) {
        mover = moverIn;
        startDirection = startDir;
        updatingDirection = updatingDir;
    }
}

/*
    A single unit of movement execution
*/
[System.Serializable]
public abstract class MovementEvent : ScriptableObject
{
    [SerializeField] public float duration;
    
    /*
        Take in any additional context or information needed at the start
        If this event is instant, i.e. duration <= 0.0f, nothing happens in Update()
        Does not mean nothing happens in End(), but that's an issue to be sorted out because of how End() is called in Phase
    */
    public abstract void Begin(MovementContext context);

    /*
        Logical update for the event
    */
    public abstract void Tick(MovementContext context, float delta);

    /*
        Clean up, mostly
    */
    public abstract void End(MovementContext context);

    public override string ToString()
    {
        return GetType().ToString();
    }
}

// examples
// play animation
// dash
// walk
// rotate
// leap
// sidestep/strafe move

/*
    These should appear as serializable objects in an AttackSO...
    But how can we populate it?
    Making them a ScriptableObject? 
    But then they're not constructable and easily edited to use the same object with different values
    ughhhh

    Maybe we just make definitions programmatically?
*/