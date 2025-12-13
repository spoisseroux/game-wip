using UnityEngine.AI;
using UnityEngine;
/*
public class IdleState : BaseEnemyState
{
    float duration = 1f;
    CountdownTimer timer;

    public IdleState(EnemyMotor m, AnimationController a, float dur) : base(m, a)
    {
        duration = dur;
        if (duration > 0)
            timer = new CountdownTimer(duration);
    }

    public override void Enter()
    {
        if (timer != null)
            timer.Start();
    }

    public override void Update()
    {
        if (timer != null)
            timer.Tick(Time.deltaTime);
    }

    public override void Exit()
    {
        if (timer != null)
            timer.Reset(duration);
    }

    public override void Interrupt(BaseEnemyState newState)
    {
        
    }

    public float GetProgress()
    {
        return timer.progress;
    }
}

public class RoamState : BaseEnemyState
{
    readonly Vector3 startPos;
    readonly NavMeshAgent agent;
    readonly float wanderRadius;

    public RoamState(EnemyMotor m, AnimationController a, NavMeshAgent n, float wander) : base(m, a)
    {
        this.agent = n;
        this.startPos = motor.transform.position;
        this.wanderRadius = wander;
    }

    public override void Enter()
    {
        FindNewDestination();
    }

    public override void Update()
    {
        
        if (ReachedDestination())
        {
            // sample a new destination within our wander radius and provide to the agent.... CUSTOMIZE THIS LATER!!!
            var randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection += startPos;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, 1);
            var finalPosition = hit.position;
            
            agent.SetDestination(finalPosition);
        }
        
    }

    public override void Interrupt(BaseEnemyState newState)
    {
        
    }

    public bool ReachedDestination()
    {
        // basically, we've reached our destination if: 
        // 1) there is not a new path being computed
        // 2) it's not within stopping distance of previous path's destination
        // 3) agent does not have a current path or its velocity magnitude says it's not moving
        return !agent.pathPending &&
               agent.remainingDistance <= agent.stoppingDistance &&
               (!agent.hasPath || agent.velocity.sqrMagnitude == 0);
    }

    private void FindNewDestination()
    {
    // sample a new destination within our wander radius and provide to the agent.... CUSTOMIZE THIS LATER!!!
        var randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += startPos;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, 1);
        var finalPosition = hit.position;
        
        agent.SetDestination(finalPosition);
    }
}

public class AlertState : BaseEnemyState
{
    private string animName = "";

    public AlertState(EnemyMotor m, AnimationController a) : base(m, a)
    {
        
    }

    public override void Enter()
    {
        
    }

    public override void Update()
    {
        
    }

    public override void Exit()
    {
        
    }

    public override void Interrupt(BaseEnemyState newState)
    {
        
    }
}

public class ChaseState : BaseEnemyState
{
    readonly Transform player;
    readonly NavMeshAgent agent;

    public ChaseState(EnemyMotor m, AnimationController a, NavMeshAgent n, Transform p) : base(m, a)
    {
        agent = n;
        player = p;
    }

    public override void Enter()
    {
        
    }

    public override void Update()
    {
        
    }

    public override void Exit()
    {
        
    }

    public override void Interrupt(BaseEnemyState newState)
    {
        
    }
}

public class EnemyAttackState : BaseEnemyState
{
    // maybe a combat component, an anim, and blah blah fed in?
    CountdownTimer timer;
    float duration = 1f;

    public EnemyAttackState(EnemyMotor m, AnimationController a) : base(m, a)
    {
        timer = new CountdownTimer(duration);
    }

    public override void Enter()
    {
        timer.Start();
    }

    public override void Update()
    {
        timer.Tick(Time.deltaTime);
    }

    public override void Exit()
    {
        timer.Reset(duration);
    }

    public override void Interrupt(BaseEnemyState newState)
    {
        
    }

    public float GetProgress()
    {
        return timer.progress;
    }
}
*/