using UnityEngine;

/*
    Some general notes:

    how do we drag info between states potentially? like jump --> rising for example is we wanted to lerp a value and maintain cooldown
    could trigger a jump coroutine cooldown in manager but eh we'll see

    need to actually come back and edit that MovementSettings variable 
    maybe make it a scriptableobject and then inherit, or just create specific instance for each Actor like usual workflow

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

    float walkSpeed = 15f; // have to init somewhere!!

    float speedMultiplierStart = 0.5f;
    float accelerationDuration = 0.75f;
    float timeElapsed = 0.0f;
    AnimationCurve accelCurve;


    // maybe split this into Idle && Walk states?
    public WalkState(PlayerMotor m, AnimationController a, PlayerMovementManager p) : base(m, a, p) { }

    public override void Enter() 
    {
        // animation
        animator.Play(animBase + walkAnim);
        // init animation curve
        accelCurve = AnimationCurve.EaseInOut(0.0f, speedMultiplierStart, accelerationDuration, 1.0f);
        return; 
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
            float finalAdjustedSpeed = walkSpeed * curveValue;

            // rotation
            motor.AddRotation(manager.HandleRotation(), null);

            // walk logic
            Vector3 walkDir = manager.GetWalkMovementDirection();
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
    float jumpHeight = 4f;
    float moveSpeed = 12f; // have to init somewhere!!

    // gravity
    float risingGravityForce = -40f;

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
        
        // add jump velocity
        motor.AddVelocity(new Vector3(0.0f, Mathf.Sqrt(jumpHeight * -2 * risingGravityForce), 0.0f), null);
        /*
        // applying rising force at jump starts
        yVel.y = Mathf.Sqrt(jumpHeight * -2 * risingGravityForce);
        */

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
        motor.AddRotation(manager.HandleRotation(), null);
        motor.AddVelocity(moveSpeed * manager.GetWalkMovementDirection(), null);
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
    float moveSpeed = 12f;

    public RisingState(PlayerMotor m, AnimationController a, PlayerMovementManager p) : base(m, a, p) {}

    public override void Enter()
    {
        
    }

    public override void Update()
    {
        motor.AddVelocity(moveSpeed * manager.GetWalkMovementDirection(), null);
        motor.AddRotation(manager.HandleRotation(), null);
    }

    public override void Exit()
    {
        
    }
}

public class FallingState : BasePlayerState
{
    string jumpBase = "Jump_";
    string jumpFalling = "Falling";

    float moveSpeed = 12f;

    public FallingState(PlayerMotor m, AnimationController a, PlayerMovementManager p) : base(m, a, p) {}

    public override void Enter()
    {
        animator.CrossFade(animBase + jumpBase + jumpFalling, 0.3f);
    }

    public override void Update()
    {
        motor.AddVelocity(moveSpeed * manager.GetWalkMovementDirection(), null);
        motor.AddRotation(manager.HandleRotation(), null);
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
    float moveSpeed = 7.5f;

    // timer
    CountdownTimer cooldownTimer;
    float cooldown = 0.3f; // change to anim length!

    // anim adjustment
    float animatorAdjustment = 7.5f;

    public LandingState(PlayerMotor m, AnimationController a) : base(m, a)
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
        motor.AddVelocity(moveSpeed * manager.GetWalkMovementDirection(), null);
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
    float dashSpeed = 30f;
    float moveSpeed = 15f;

    // timers
    CountdownTimer cooldownTimer;
    float dashCDTime = 1.0f;
    StopwatchTimer activeTimer;
    float activeTime = 0.5f;

    // constant dash dir
    Vector3 dashDirection;

    // anim curve for speed up
    float elapsed = 0.0f;
    float initialDashSpeedMultiplier = 0.5f;
    float dashAccelDuration = 0.1f; 
    AnimationCurve accelCurve;

    // anim curve for slow down
    float elapsedDownCurve = 0.0f;
    float decelDuration = 0.4f;
    AnimationCurve decelCurve;

    // animation
    string dashAnim = "Dash_Forward";
    string idleAnim = "Idle";

    public DashingState(PlayerMotor m, AnimationController a) : base(m, a)
    {
        // length of state
        cooldownTimer = new CountdownTimer(dashCDTime);

        // active modifier of speed
        activeTimer = new StopwatchTimer(activeTime);

        // anim curve
        accelCurve = AnimationCurve.Linear(0.0f, initialDashSpeedMultiplier, dashAccelDuration, 1.0f);
        decelCurve = AnimationCurve.EaseInOut(0.0f, dashSpeed, decelDuration, moveSpeed);
    }

    public override void Enter()
    {
        dashDirection = manager.GetDashDirection();
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
            elapsed += Time.deltaTime;
            float curveVal = accelCurve.Evaluate(elapsed);
            float dashMult = curveVal * dashSpeed;
            motor.AddVelocity(dashMult * dashDirection, null);
        }
        else
        {
            elapsedDownCurve += Time.deltaTime;
            float speed = decelCurve.Evaluate(elapsedDownCurve);
            
            motor.AddVelocity(speed * manager.GetWalkMovementDirection(), null);
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

    // timers
    private CountdownTimer seekTimer;
    private float seekLength = 0.3f;
    private CountdownTimer bounceTimer;
    private float bounceLength = 0.2f;


    public WallJumpState(PlayerMotor m, AnimationController a) : base(m, a)
    {
        // timers
        seekTimer = new CountdownTimer(seekLength);
        bounceTimer = new CountdownTimer(bounceLength);
        // phase
        phase = WallJumpPhase.Seeking;
    }

    public override void Enter()
    {
        //motor.SetSeekingDirection();
        phase = WallJumpPhase.Seeking;
        //motor.WallJumpBoost();
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

    public override void Update()
    {
        /*
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
        */
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

    public AttackState(PlayerMotor m, PlayerCombatManager c, AnimationController a) : base(m, a)
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
        // motor.Walk();
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