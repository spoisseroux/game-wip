using UnityEngine.AI;

public abstract class BaseEnemyState : IState
{
    protected readonly EnemyMotor motor;
    protected readonly AnimationController animator;
    protected readonly string ownerAnimName;

    protected BaseEnemyState(EnemyMotor m, AnimationController a)
    {
        this.motor = m;
        this.animator = a;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }
    public virtual void Interrupt(BaseEnemyState newState) { }

    public override string ToString()
    {
        return GetType().ToString();
    }
}