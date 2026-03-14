using UnityEngine;
using System;

/*
    Some general notes:

    how do we drag info between states potentially? like jump --> rising for example is we wanted to lerp a value and maintain cooldown
    could trigger a jump coroutine cooldown in manager but eh we'll see

    need to actually come back and edit that MovementSettings variable 
    maybe make it a scriptableobject and have a specific instance for each generic Actor like usual workflow

    then reference through motor.movementValues.walkSpeed; or something

    uhhhh more soon
*/

// All states have a protected PlayerMotor component set in their BaseState predecessor. Use this to call movement logic
public class IdleState : BasePlayerState
{
    string idle = "Idle";

    Coroutine idleAnimRoutine; // silly idle anim re-queue routine
    float refresh; // regen randomly, val between 20-30seconds
    string sillyIdle;

    public IdleState(PlayerMotor m, AnimationController a) : base(m, a) { }
    
    public override void Enter() { animator.Play(animBase + idle); }
    public override void Exit() { return; }

    public override void Update()
    {
        // count timer, if done queue silly anim
    }
}

public class WalkState : BasePlayerState
{
    string walkAnim = "Run_Full";

    AnimationCurve accelCurve;
    float speedMultiplierStart = 0.5f;
    float accelerationDuration = 0.5f;
    float timeElapsed = 0.0f;


    // maybe split this into Idle && Walk states?
    public WalkState(PlayerMotor m, AnimationController a, PlayerMovementManager p) : base(m, a, p) { }

    public override void Enter() 
    {
        // animation
        animator.Play(animBase + walkAnim);
        // init animation curve
        accelCurve = AnimationCurve.EaseInOut(0.0f, speedMultiplierStart, accelerationDuration, 1.0f);
    }

    public override void Exit()
    {
        // reset on exit
        timeElapsed = 0.0f;
    }

    public override void Update()
    {
        if (manager.CheckIfMoving())
        {
            // tick animation curve
            timeElapsed += Time.deltaTime;

            // calc anim curve
            float curveValue = accelCurve.Evaluate(timeElapsed);
            float finalAdjustedSpeed = motor.moveSettings.walkSpeed * curveValue;

            // rotation
            motor.AddRotation(manager.GetTargetRotation(), null);

            // walk logic
            Vector3 walkDir = manager.GetMovementDirection();
            motor.AddVelocity(finalAdjustedSpeed * walkDir, null);
        }
    }
}

// HOW DO WE CARRY THIS TIMER ACROSS JUMP STATES
public class JumpingState : BasePlayerState
{
    // need to re-tool for proper animations!!! not just transition to neutral immediately
    // timer
    CountdownTimer cooldownTimer;
    float cooldown = 0.5f;

    // stats
    float moveSpeedMultiplier = 0.75f; // MAKE THIS A MULTIPLIER OF WALKSPEED WHEN PARENT MOVEMENTSETTINGS OBJECT IS COMPLETE

    // gravity
    float risingGravityForce = -40f; // how can i factor this out... and make it something we only grab from gravity object?

    // animations
    string jumpBase = "Jump_";
    string jumpStart = "Start";
    float animatorAdjustment = 7.5f;

    public JumpingState(PlayerMotor m, AnimationController a, PlayerMovementManager p) : base(m, a, p)
    {
        cooldownTimer = new CountdownTimer(cooldown);
    }

    public override void Enter()
    {   
        // bonus jump check
        if (!motor.Grounded)
            manager.bonusJumpTaken = true;
        
        // get jump velocity
        Vector3 jump = new Vector3(0.0f, Mathf.Sqrt(motor.moveSettings.jumpHeight * -2 * risingGravityForce), 0.0f);

        cooldownTimer.Start();
        animator.SetAnimatorSpeed(animatorAdjustment);
        animator.Play(animBase + jumpBase + jumpStart);

        // apply movement
        motor.AddVelocity(motor.moveSettings.walkSpeed * moveSpeedMultiplier * manager.GetMovementDirection(), null);
        motor.SetVerticalVelocity(jump, null);
        
    }

    public override void Exit()
    {
        cooldownTimer.Pause();
        cooldownTimer.Reset(cooldown);
        animator.SetDefaultAnimatorSpeed();
    }

    public override void Update()
    {
        motor.AddRotation(manager.GetTargetRotation(), null);
        motor.AddVelocity(motor.moveSettings.walkSpeed * moveSpeedMultiplier * manager.GetMovementDirection(), null);
        cooldownTimer.Tick(Time.deltaTime);
    }

    public float GetProgress()
    {
        return cooldownTimer.progress;
    }
}

public class RisingState : BasePlayerState
{
    // stats
    float moveSpeedMultiplier = 0.75f;

    public RisingState(PlayerMotor m, AnimationController a, PlayerMovementManager p) : base(m, a, p) {}

    public override void Enter() { }

    public override void Update()
    {
        motor.AddVelocity(motor.moveSettings.walkSpeed * moveSpeedMultiplier * manager.GetMovementDirection(), null);
        motor.AddRotation(manager.GetTargetRotation(), null);
    }

    public override void Exit() { }
}

public class FallingState : BasePlayerState
{
    string jumpBase = "Jump_";
    string jumpFalling = "Falling";

    float moveSpeedMultiplier = 0.75f; // again, multiplier of parent movementsettingsSO walkspeed var!

    public FallingState(PlayerMotor m, AnimationController a, PlayerMovementManager p) : base(m, a, p) {}

    public override void Enter()
    {
        animator.CrossFade(animBase + jumpBase + jumpFalling, 0.3f);
    }

    public override void Update()
    {
        motor.AddVelocity(motor.moveSettings.walkSpeed * moveSpeedMultiplier * manager.GetMovementDirection(), null);
        motor.AddRotation(manager.GetTargetRotation(), null);
    }

    public override void Exit()
    {
        
    }
}

public class LandingState : BasePlayerState
{
    string jumpBase = "Jump_";
    string jumpLand = "Landing";

    // stats
    float moveSpeedMultiplier = 0.5f;

    // timer
    CountdownTimer cooldownTimer;
    float cooldown = 0.3f; // change to anim length!

    // anim adjustment
    float animatorAdjustment = 7.5f;

    public LandingState(PlayerMotor m, AnimationController a, PlayerMovementManager p) : base(m, a, p)
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
        motor.AddVelocity(motor.moveSettings.walkSpeed * moveSpeedMultiplier * manager.GetMovementDirection(), null);
    }

    public override void Exit()
    {
        cooldownTimer.Pause();
        cooldownTimer.Reset(cooldown);
        animator.SetDefaultAnimatorSpeed();
    }

    public float GetProgress()
    {
        return cooldownTimer.progress;
    }
}

public class DashingState : BasePlayerState
{
    // stats 
    float dashSpeedMultplier = 2f; // make a multiplier of walkspeed??

    // timers
    CountdownTimer cooldownTimer;
    float dashCDTime = 1.0f;
    StopwatchTimer activeTimer;
    float activeTime = 0.5f;

    // constant dash dir
    Vector3 dashDirection;

    // anim curve for speed up
    AnimationCurve accelCurve;
    float elapsed = 0.0f;
    float initialDashSpeedMultiplier = 0.5f;
    float dashAccelDuration = 0.1f; 

    // anim curve for slow down
    AnimationCurve decelCurve;
    float elapsedDownCurve = 0.0f;
    float decelDuration = 0.4f;

    // animation
    string dashAnim = "Dash_Forward";
    string idleAnim = "Idle";

    public DashingState(PlayerMotor m, AnimationController a, PlayerMovementManager p) : base(m, a, p)
    {
        // length of state
        cooldownTimer = new CountdownTimer(dashCDTime);

        // active modifier of speed
        activeTimer = new StopwatchTimer(activeTime);

        // anim curve
        accelCurve = AnimationCurve.Linear(0.0f, initialDashSpeedMultiplier, dashAccelDuration, 1.0f);
        decelCurve = AnimationCurve.EaseInOut(0.0f, motor.moveSettings.walkSpeed * dashSpeedMultplier, decelDuration, motor.moveSettings.walkSpeed);
    }

    public override void Enter()
    {
        Debug.Log("enter dash state");
        dashDirection = manager.GetDashDirection();
        cooldownTimer.Start();
        activeTimer.Start();
        animator.Play(animBase + dashAnim);
    }

    public override void Exit()
    {
        Debug.Log("exit dash State");
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
            elapsed += Time.deltaTime;
            float curveVal = accelCurve.Evaluate(elapsed);
            float dashMult = curveVal * (motor.moveSettings.walkSpeed * dashSpeedMultplier);
            motor.AddVelocity(dashMult * dashDirection, null);
        }
        else
        {
            elapsedDownCurve += Time.deltaTime;
            float speed = decelCurve.Evaluate(elapsedDownCurve);
            
            motor.AddVelocity(speed * manager.GetMovementDirection(), null);
            animator.CrossFade(animBase + idleAnim, 0.1f);
        }
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

    // configs
    float checkDistance = 2.0f;
    float seekSpeed = 10f;
    float bounceSpeed = 10f;
    Vector3 yVelBoost = new Vector3(0.0f, 5.0f, 0.0f);

    // timers
    private CountdownTimer seekTimer;
    private float seekLength = 0.5f;
    private CountdownTimer bounceTimer;
    private float bounceLength = 0.5f;

    // directions
    Vector3 seekDir;
    Vector3 bounceDir;

    // Anim curves for seeking and bouncing! ADD LATER!!!


    public WallJumpState(PlayerMotor m, AnimationController a, PlayerMovementManager p) : base(m, a, p)
    {
        // timers
        seekTimer = new CountdownTimer(seekLength);
        bounceTimer = new CountdownTimer(bounceLength);
        // phase
        phase = WallJumpPhase.Seeking;
    }

    public override void Enter()
    {
        seekDir = manager.GetSeekingDirection();
        phase = WallJumpPhase.Seeking;
        seekTimer.Start();
        motor.AddVelocity(yVelBoost, null);
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

        // fix vectors
        seekDir = Vector3.zero;
        bounceDir = Vector3.zero;

        Debug.Log("exited walljump");
    }

    public override void Update()
    {
        switch (phase)
        {
            case WallJumpPhase.Seeking:
                SeekWall();
                break;
            case WallJumpPhase.Bouncing:
                BounceOffWall();
                break;
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

    private void SeekWall()
    {
        seekTimer.Tick(Time.deltaTime);
        
        Tuple<bool, RaycastHit> hitCheck = motor.WallContact(seekDir, checkDistance);

        // if yes, transition to bouncing
        if (hitCheck.Item1)
        {
            phase = WallJumpPhase.Bouncing;

            seekTimer.Pause();
            seekTimer.Reset(seekLength);

            // set bounce dir and change rotation
            bounceDir = manager.GetBounceDirection(seekDir, hitCheck.Item2);
            motor.SetNewRotation(bounceDir, null);
            motor.AddVelocity(yVelBoost, null);
            bounceTimer.Start();
        }
        // if no, move
        else
        {
            seekTimer.Tick(Time.deltaTime);
            motor.AddVelocity(seekSpeed * seekDir, null);
        }
    }

    private void BounceOffWall()
    {
        bounceTimer.Tick(Time.deltaTime);
        motor.AddVelocity(bounceSpeed * bounceDir, null); // FIX
    }
}

public class InteractState : BasePlayerState
{
    // animation
    string interactUse = "";
    string interactGeneral = "Interact_Generic";

    public InteractState(PlayerMotor m, AnimationController a, PlayerMovementManager p) : base(m, a, p)
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
        // motor.AddRotation(manager.HandleRotation(), null);
    }
}

/*
    THIS SHOULD ONLY NEED MOVEMENT PHASES!!!
    COMBAT LINK IDEALLY SHOULD NOT BE HERE WHEN REFACTOR IS DONE
*/
public class AttackState : BasePlayerState
{

    // timer
    CountdownTimer timer;
    private float duration = 0f;

    // animations
    private string attackAnim;

    // anim adjust
    float animatorAdjustment = 2f;

    public AttackState(PlayerMotor m, AnimationController a) : base(m, a)
    {
        attackAnim = "";
    }
    
    public override void Enter()
    {
        timer.Start();

        animator.SetAnimatorSpeed(animatorAdjustment);
        // start anim
        animator.Play(attackAnim);
    }

    public override void Exit()
    {
        timer = null;
        attackAnim = "";
        animator.SetDefaultAnimatorSpeed();
    }

    public override void Update()
    {
        timer.Tick(Time.deltaTime);
        // motor.Walk();
    }

    public void SetAttackInternals(AttackSO attackData)
    {
        bool active = timer != null;
        
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

    // speed modifier


    public ChantState(PlayerMotor m, AnimationController a) : base(m, a)
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
        // motor.Walk();
    }

    public float GetProgress()
    {
        return timer.progress;
    }
}