public abstract class BasePlayerState : IState
{
    protected readonly PlayerMotor motor;
    protected readonly AnimationController animator;
    protected readonly string animBase = "Character_";

    protected BasePlayerState(PlayerMotor m , AnimationController a)
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