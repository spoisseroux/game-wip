using System.Collections.Generic;

public abstract class BaseState : IState
{
    protected readonly PlayerMovementManager motor;
    //protected readonly Animator animator;

    protected BaseState(PlayerMovementManager m /*, Animator a */)
    {
        this.motor = m;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }
    public virtual void Interrupt(BaseState newState) { }

    public override string ToString()
    {
        return GetType().ToString();
    }
}
