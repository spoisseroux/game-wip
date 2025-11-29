public abstract class BasePlayerState : IState
{
    protected readonly PlayerMovementManager motor;
    protected readonly AnimationController animator; // AnimationController???

    protected BasePlayerState(PlayerMovementManager m , AnimationController a)
    {
        this.motor = m;
        this.animator = a;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }
    public virtual void Interrupt(BasePlayerState newState) { }

    public override string ToString()
    {
        return GetType().ToString();
    }
}