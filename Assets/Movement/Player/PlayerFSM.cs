using UnityEngine;
using System;

// All states have a protected PlayerMovementManager component set in their BaseState predecessor. Use this to call movement logic
public class NeutralState : BasePlayerState
{
    string walkAnim = "Run_Full";
    string idleAnim = "Idle";

    // maybe split this into Idle && Walk states?
    public NeutralState(PlayerMovementManager f, AnimationController a) : base(f, a) { }

    public override void Enter() { return; }
    public override void Exit() { return; }
    public override void Update()
    {
        motor.HandleRotation();
        if (motor.CheckIfMoving())
        {
            animator.Play(animBase + walkAnim);
        }
        else
        {
            animator.Play(animBase + idleAnim);
        }
        motor.Walk();
    }

    public override void Interrupt(BasePlayerState newState) { return; }
}

public class JumpingState : BasePlayerState
{
    // need to re-tool for proper animations!!! not just transition to neutral immediately
    // timer
    CountdownTimer cooldownTimer;
    float cooldown = 0.5f;

    // animations
    string jumpBase = "Jump_";
    string jumpStart = "Start";

    // float 
    float animatorAdjustment = 7.5f;

    public JumpingState(PlayerMovementManager f, AnimationController a) : base(f, a)
    {
        cooldownTimer = new CountdownTimer(cooldown);
    }

    public override void Enter()
    {
        motor.ApplyJumpingVelocity();
        cooldownTimer.Start();
        animator.SetAnimatorSpeed(animatorAdjustment);
        animator.Play(animBase + jumpBase + jumpStart);
        
    }

    public override void Exit()
    {
        cooldownTimer.Pause();
        cooldownTimer.Reset(cooldown);
        animator.SetDefaultAnimatorSpeed();
    }

    public override void Update()
    {
        motor.Walk();
        motor.HandleRotation();
        cooldownTimer.Tick(Time.deltaTime);
    }

    public override void Interrupt(BasePlayerState newState)
    {
        // pause timers
        cooldownTimer.Pause();
    }

    public float GetProgress()
    {
        return cooldownTimer.progress;
    }
}

public class RisingState : BasePlayerState
{
    public RisingState(PlayerMovementManager m, AnimationController a) : base(m, a) {}

    public override void Enter()
    {
        
    }

    public override void Update()
    {
        motor.Walk();
        motor.HandleRotation();
    }

    public override void Exit()
    {
        
    }

    public override void Interrupt(BasePlayerState newState)
    {
        
    }
}

public class FallingState : BasePlayerState
{
    string jumpBase = "Jump_";
    string jumpFalling = "Falling";

    public FallingState(PlayerMovementManager m, AnimationController a) : base(m, a) {}

    public override void Enter()
    {
        animator.CrossFade(animBase + jumpBase + jumpFalling, 0.3f);
    }

    public override void Update()
    {
        motor.Walk();
        motor.HandleRotation();
    }

    public override void Exit()
    {
        
    }

    public override void Interrupt(BasePlayerState newState)
    {
        
    }
}

public class LandingState : BasePlayerState
{
    string jumpBase = "Jump_";
    string jumpLand = "Landing";

    // timer
    CountdownTimer cooldownTimer;
    float cooldown = 0.3f; // change to anim length!

    // adjustment
    float animatorAdjustment = 7.5f;

    public LandingState(PlayerMovementManager m, AnimationController a) : base(m, a)
    {
        cooldownTimer = new CountdownTimer(cooldown);
    }

    public override void Enter()
    {
        cooldownTimer.Start();
        animator.SetAnimatorSpeed(animatorAdjustment);
        animator.Play(animBase + jumpBase + jumpLand);
    }

    public override void Update()
    {
        cooldownTimer.Tick(Time.deltaTime);
        motor.Walk();
    }

    public override void Exit()
    {
        cooldownTimer.Pause();
        cooldownTimer.Reset(cooldown);
        animator.SetDefaultAnimatorSpeed();
    }

    public override void Interrupt(BasePlayerState newState)
    {
        
    }

    public float GetProgress()
    {
        return cooldownTimer.progress;
    }
}

public class DashingState : BasePlayerState
{
    // timers
    CountdownTimer cooldownTimer;
    float dashCDTime = 1.0f;
    StopwatchTimer activeTimer;
    float activeTime = 0.5f;

    // animation
    string dashAnim = "Dash_Forward";
    string idleAnim = "Idle";

    public DashingState(PlayerMovementManager f, AnimationController a) : base(f, a)
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
        animator.Play(animBase + dashAnim);
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
        else
        {
            motor.Walk();
            animator.CrossFade(animBase + idleAnim, 0.1f);
        }
    }

    public override void Interrupt(BasePlayerState newState)
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

public class WallJumpState : BasePlayerState
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


    public WallJumpState(PlayerMovementManager f, AnimationController a) : base(f, a)
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

    public override void Interrupt(BasePlayerState newState)
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

public class InteractState : BasePlayerState
{
    // animation
    string interactUse = "";
    string interactGeneral = "Interact_Generic";

    public InteractState(PlayerMovementManager m, AnimationController a) : base(m, a)
    {
        
    }

    public override void Enter()
    {
        animator.Play(animBase + interactGeneral);
    }

    public override void Exit()
    {
        
    }

    public override void Update()
    {
        
    }

    public override void Interrupt(BasePlayerState newState)
    {
        throw new NotImplementedException();
    }
}

public class AttackState : BasePlayerState
{   
    // combat link
    PlayerCombatManager combat;

    // timer
    CountdownTimer timer;
    private float duration = 0f;

    // animations
    private string attackAnim;

    // anim adjust
    float animatorAdjustment = 2f;

    public AttackState(PlayerMovementManager m, PlayerCombatManager c, AnimationController a) : base(m, a)
    {
        combat = c;
        attackAnim = "";
    }
    
    public override void Enter()
    {
        timer.Start();
        combat.BeginAttack(duration);
        animator.SetAnimatorSpeed(animatorAdjustment);
        // start anim
        animator.Play(attackAnim);
    }

    public override void Exit()
    {
        timer = null;
        combat.ResetWeaponCycle();
        attackAnim = "";
        animator.SetDefaultAnimatorSpeed();
    }

    public override void Update()
    {
        timer.Tick(Time.deltaTime);
        motor.Walk();
    }

    public override void Interrupt(BasePlayerState newState)
    {
        throw new NotImplementedException();
    }

    public void SetAttackInternals(AttackSO attackData)
    {
        bool active = timer != null;

        // timer
        duration = attackData.attackDuration;
        timer = new CountdownTimer(duration);

        // anim 
        attackAnim = animBase + attackData.animName;
        
        // restart check
        if (active)
            Restart();
    }

    public void Restart()
    {
        Enter();
    }

    public float GetProgress()
    {
        return timer.progress;
    }
}

// chant state???
public class ChantState : BasePlayerState
{
    // timing
    CountdownTimer timer;
    float duration = 1.5f;

    // animation?
    string chantAnim = "";

    public ChantState(PlayerMovementManager m, AnimationController a) : base(m, a)
    {
        timer = new CountdownTimer(duration);
    }

    public override void Enter()
    {
        timer.Start();
        // start anim
        // animator.Play(animBase + chantAnim);
    }

    public override void Exit()
    {
        timer = null;
        animator.SetDefaultAnimatorSpeed();
    }

    public override void Update()
    {
        timer.Tick(Time.deltaTime);
        motor.Walk();
    }

    public override void Interrupt(BasePlayerState newState)
    {
        throw new NotImplementedException();
    }

    public float GetProgress()
    {
        return timer.progress;
    }
}