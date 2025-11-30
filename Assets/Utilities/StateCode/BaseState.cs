using System.Collections.Generic;

// a general framework outline for creating states on top of the StateMachine.cs code
// applies to any unit that could feasibly use one
public abstract class BaseState : IState
{
    protected BaseState() { }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }
    // public virtual void Interrupt(BaseState newState) { }

    public override string ToString()
    {
        return GetType().ToString();
    }
}
