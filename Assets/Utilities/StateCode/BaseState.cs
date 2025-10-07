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
    // public virtual void FixedUpdate() { }
    public virtual void Exit() { }
    public abstract void Interrupt(BaseState newState);

    // seems like it scales VERY poorly, we'll see
    public List<BaseState> validTransitions;
    public bool CheckTransitions(BaseState state)
    {
        return validTransitions.Contains(state);
    }

    public override string ToString()
    {
        return GetType().ToString();
    }
}
