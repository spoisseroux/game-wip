public abstract class BasePlayerState : IState
{
    // protected MovementSettings moveSettings; 
    // MOVED THIS TO MOTOR CAUSE SCRIPTABLE OBJECT DRAG/INIT ISSUES IN A PURE C# OBJECT

    protected readonly PlayerMotor motor;
    protected readonly AnimationController animator;
    protected readonly PlayerMovementManager manager; // for camera instance vars
    protected readonly string animBase = "Character_";

    protected BasePlayerState(PlayerMotor m , AnimationController a, PlayerMovementManager p = null)
    {
        this.motor = m;
        this.animator = a;
        this.manager = p;
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