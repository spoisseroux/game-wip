using UnityEngine;
using System;

// All states have a protected PlayerMovementManager component set in their BaseState predecessor. Use this to call movement logic
public class NeutralState : BaseState
{
    public NeutralState(PlayerMovementManager f) : base(f) { }

    public override void Enter() { return; }
    public override void Exit() { return; }
    public override void Update()
    {
        motor.HandleRotation();
        motor.Walk();
    }

    public override void Interrupt(BaseState newState) { return; }
}

public class JumpingState : BaseState
{
    // internals
    CountdownTimer cooldownTimer;
    float cooldown = 0.5f;

    public JumpingState(PlayerMovementManager f) : base(f)
    {
        cooldownTimer = new CountdownTimer(cooldown);
    }

    public override void Enter()
    {
        motor.ApplyJumpingVelocity();
        cooldownTimer.Start();
    }

    public override void Exit()
    {
        cooldownTimer.Pause();
        cooldownTimer.Reset(cooldown);
    }

    public override void Update()
    {
        motor.Walk();
        motor.HandleRotation();
        cooldownTimer.Tick(Time.deltaTime);
    }

    public override void Interrupt(BaseState newState)
    {
        // pause timers
        cooldownTimer.Pause();
    }

    public float GetProgress()
    {
        return cooldownTimer.progress;
    }
}

public class DashingState : BaseState
{
    // internals
    CountdownTimer cooldownTimer;
    float dashCDTime = 1.0f;
    StopwatchTimer activeTimer;
    float activeTime = 0.5f;

    public DashingState(PlayerMovementManager f) : base(f)
    {
        // length of state
        cooldownTimer = new CountdownTimer(dashCDTime);

        // active modifier of speed
        activeTimer = new StopwatchTimer(activeTime);
    }

    public override void Enter()
    {
        motor.SetDashDirection();
        cooldownTimer.Start();
        activeTimer.Start();
    }

    public override void Exit()
    {
        cooldownTimer.Reset(dashCDTime);
        activeTimer.Reset();
    }

    public override void Update()
    {
        activeTimer.Tick(Time.deltaTime);
        cooldownTimer.Tick(Time.deltaTime);
        // move if active
        if (!activeTimer.lapComplete)
        {
            motor.Dash();
        }
    }

    public override void Interrupt(BaseState newState)
    {
        // pause timers
        activeTimer.Pause();
        cooldownTimer.Pause();
    }

    public float GetProgress()
    {
        return cooldownTimer.progress;
    }
}

public class WallJumpState : BaseState
{
    // phase enum
    public enum WallJumpPhase
    {
        Seeking,
        Bouncing
    }
    WallJumpPhase phase;

    // timers
    private CountdownTimer seekTimer;
    private float seekLength = 0.3f;
    private CountdownTimer bounceTimer;
    private float bounceLength = 0.2f;


    public WallJumpState(PlayerMovementManager f) : base(f)
    {
        // timers
        seekTimer = new CountdownTimer(seekLength);
        bounceTimer = new CountdownTimer(bounceLength);
        // phase
        phase = WallJumpPhase.Seeking;
    }

    public override void Enter()
    {
        motor.SetSeekingDirection();
        phase = WallJumpPhase.Seeking;
        motor.WallJumpBoost();
        seekTimer.Start();
    }

    public override void Exit()
    {
        // reset timers
        seekTimer.Pause();
        seekTimer.Reset(seekLength);
        bounceTimer.Pause();
        bounceTimer.Reset(bounceLength);

        // edit internals
        phase = WallJumpPhase.Seeking;
    }

    public override void Interrupt(BaseState newState)
    {
        throw new System.NotImplementedException();
    }

    public override void Update()
    {
        // seeking phase update
        if (phase == WallJumpPhase.Seeking)
        {
            // tick timer
            seekTimer.Tick(Time.deltaTime);
            // check for wall contact
            Tuple<bool, RaycastHit> hitCheck = motor.WallContactCheck();
            Debug.Log(hitCheck.Item1.ToString() + " " + hitCheck.Item2.ToString());

            // if yes, transition to bouncing
            if (hitCheck.Item1)
            {
                Debug.Log("transition to bounce");
                phase = WallJumpPhase.Bouncing;
                motor.WallJumpBoost();

                seekTimer.Pause();
                seekTimer.Reset(seekLength);

                bounceTimer.Start();
                motor.SetBounceDirection(hitCheck.Item2);
            }
            // if no, move
            else
            {
                Debug.Log("moving");
                seekTimer.Tick(Time.deltaTime);
                motor.SeekWall();
            }
        }

        // bounce phase update
        else
        {
            Debug.Log("bouncing NOW!");
            bounceTimer.Tick(Time.deltaTime);
            motor.BounceOffWall();
        }

    }

    public bool IsFinished()
    {
        bool inProgress = false;
        switch (phase)
        {
            case WallJumpPhase.Seeking:
                inProgress = seekTimer.progress <= 0;
                break;
            case WallJumpPhase.Bouncing:
                inProgress = bounceTimer.progress <= 0;
                break;
        }
        return inProgress;
    }
}

public class InteractState : BaseState
{
    // maybe we make a hook here to the component we want to receive input?

    public InteractState(PlayerMovementManager m) : base(m)
    {
        
    }

    public override void Enter()
    {
        
    }

    public override void Exit()
    {
        
    }

    public override void Update()
    {
        
    }

    public override void Interrupt(BaseState newState)
    {
        throw new NotImplementedException();
    }
}

public class AttackState : BaseState
{   
    PlayerCombatManager combat;
    AttackSO currentAttack;
    CountdownTimer timer;
    private float duration = 0f;

    public AttackState(PlayerMovementManager m, PlayerCombatManager c) : base(m)
    {
        combat = c;
    }
    
    public override void Enter()
    {
        timer.Start();
    }

    public override void Exit()
    {
        
    }

    public override void Update()
    {
        // update attack

        // when timer at active point, queue hitboxes

        motor.Walk();
    }

    public override void Interrupt(BaseState newState)
    {
        throw new NotImplementedException();
    }

    public void SetAttack(AttackSO attack)
    {
        currentAttack = attack;
        // timer
        duration = attack.attackDuration;
        timer = new CountdownTimer(duration);
    }
}
