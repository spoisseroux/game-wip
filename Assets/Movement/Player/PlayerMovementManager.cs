using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementManager : MonoBehaviour
{
    // parent... is this even gunna be necessary?
    PlayerManager player;

    [Header("Script Components")]
    // components
    [SerializeField] PlayerMotor motor;
    [SerializeField] public CharacterController characterController;
    [SerializeField] InputReader input;
    [SerializeField] PlayerCombatManager combat;
    [SerializeField] AnimationController animationController;

    // fsm & states
    StateMachine fsm;
    public IdleState idleState;
    public WalkState walkState;
    public JumpingState jumpingState;
    public RisingState risingState;
    public FallingState fallingState;
    public LandingState landingState;
    public DashingState dashState;
    public WallJumpState walljumpState;
    public InteractState interactState;
    public AttackState attackState;
    public ChantState chantState;

    // polling input vals
    [HideInInspector] public float horizontalMovement; // A + D keys
    [HideInInspector] public float verticalMovement; // W + S keys
    [HideInInspector] public float moveAmount; // magnitude value of req'd movement

    // ActionRequests class
    protected class ActionRequests
    {
        Dictionary<ActionRequest, bool> req;

        public ActionRequests()
        {
            req = new Dictionary<ActionRequest, bool>
            {
                {ActionRequest.Jump, false},
                {ActionRequest.Dash, false},
                {ActionRequest.WallJump, false},
                {ActionRequest.Attack, false},
                {ActionRequest.Interact, false}
            };
        }

        public void SetRequest(ActionRequest a, bool val)
        {
            req[a] = val;
        }

        public bool Check(ActionRequest a)
        {
            bool isRequesting = req[a];
            SetRequest(a, false); // reset our input
            return isRequesting;
        }
    }
    ActionRequests inputRequests;

    [Header("Movement Settings")]
    public Vector3 moveDirection;

    // probably a good idea to separate this out into a separate script, maybe even a monobehavior 
    // then it could store the InteractTrigger it is involved with and reset itself
    // other scripts read it for state changes and logic flows
    [Header("Interaction Check")]
    [SerializeField] float interactSphereRadius = 0.2f;
    [SerializeField] Vector3 interactCheckTranslationOffset = new Vector3(0f, 1f, 0f);
    [SerializeField] float interactDistance = 5f;
    [SerializeField] IInteractable currentInteraction;

    [Header("Jump")]
    // TODO: grace periods, coyote time (after leaving platform) && jump buffer (input buffering sorta deal)
    public bool bonusJumpTaken = false;

    [Header("Wall Jump")]
    [SerializeField] LayerMask wallLayer;
    [SerializeField] float detectionRange = 10f;
    [SerializeField] float wallJumpSphereRaycastRadius = 0.4f;

    #region Monobehavior
    private void Awake()
    {
        // unity components
        player = GetComponent<PlayerManager>();
        combat = GetComponent<PlayerCombatManager>();
        characterController = GetComponent<CharacterController>();
        animationController = GetComponent<AnimationController>();

        // state machine
        fsm = new StateMachine();

        // states, this for rotation handling or other direct transform manipulation
        idleState = new IdleState(motor, animationController);
        walkState = new WalkState(motor, animationController, this);
        jumpingState = new JumpingState(motor, animationController, this);
        risingState = new RisingState(motor, animationController, this);
        fallingState = new FallingState(motor, animationController, this);
        landingState = new LandingState(motor, animationController, this);
        dashState = new DashingState(motor, animationController, this);
        walljumpState = new WallJumpState(motor, animationController, this);
        interactState = new InteractState(motor, animationController, this);
        attackState = new AttackState(motor, combat, animationController);
        chantState = new ChantState(motor, animationController);

        // idle state transitions
        At(idleState, walkState, new FuncPredicate(() => CheckIfMoving()));
        At(idleState, jumpingState, new FuncPredicate(() => inputRequests.Check(ActionRequest.Jump)));
        At(idleState, interactState, new FuncPredicate(() => currentInteraction != null && currentInteraction.IsTrigger()
                                        && motor.Grounded));
        At(idleState, attackState, new FuncPredicate(() => inputRequests.Check(ActionRequest.Attack) && RequestAttack()));
        At(idleState, chantState, new FuncPredicate(() => false)); // how to fire an event to pipe into here?

        // walk state transitions
        At(walkState, jumpingState, new FuncPredicate(() =>
                                        inputRequests.Check(ActionRequest.Jump)));
        At(walkState, dashState, new FuncPredicate(() => inputRequests.Check(ActionRequest.Dash)));
        At(walkState, interactState, new FuncPredicate(() => currentInteraction != null && currentInteraction.IsTrigger()
                                        && motor.Grounded));
        At(walkState, attackState, new FuncPredicate(() => inputRequests.Check(ActionRequest.Attack) && RequestAttack()));
        At(walkState, chantState, new FuncPredicate(() => false)); // how to fire an event to pipe into here?
        At(walkState, idleState, new FuncPredicate(() => !CheckIfMoving()));

        // jumping state transitions
        At(jumpingState, risingState, new FuncPredicate(() => jumpingState.GetProgress() <= 0));
        At(jumpingState, dashState, new FuncPredicate(() => inputRequests.Check(ActionRequest.Dash)));
        At(jumpingState, attackState, new FuncPredicate(() => inputRequests.Check(ActionRequest.Attack) && RequestAttack()));
        At(jumpingState, walljumpState, new FuncPredicate(() =>
                                        inputRequests.Check(ActionRequest.WallJump)
                                        && !motor.Grounded));

        // rising state transitions
        At(risingState, jumpingState, new FuncPredicate(() =>
                                        inputRequests.Check(ActionRequest.Jump)
                                        && !bonusJumpTaken));
        At(risingState, fallingState, new FuncPredicate(() => !motor.Grounded && motor.currVelocity.y <= 0.0f));
        At(risingState, dashState, new FuncPredicate(() => inputRequests.Check(ActionRequest.Dash)));
        At(risingState, attackState, new FuncPredicate(() => inputRequests.Check(ActionRequest.Attack) && RequestAttack()));
        At(risingState, walljumpState, new FuncPredicate(() => inputRequests.Check(ActionRequest.WallJump)));

        // falling state transitions
        At(fallingState, jumpingState, new FuncPredicate(() =>
                                        inputRequests.Check(ActionRequest.Jump)
                                        && !bonusJumpTaken));
        At(fallingState, landingState, new FuncPredicate(() => motor.Grounded));
        At(fallingState, dashState, new FuncPredicate(() => inputRequests.Check(ActionRequest.Dash)));
        At(fallingState, attackState, new FuncPredicate(() => inputRequests.Check(ActionRequest.Attack) && RequestAttack()));
        At(fallingState, walljumpState, new FuncPredicate(() => inputRequests.Check(ActionRequest.WallJump)));

        // landing state transitions
        At(landingState, idleState, new FuncPredicate(() => landingState.GetProgress() <= 0 && !CheckIfMoving()));
        At(landingState, walkState, new FuncPredicate(() => landingState.GetProgress() <= 0 && CheckIfMoving()));
        At(landingState, jumpingState, new FuncPredicate(() => inputRequests.Check(ActionRequest.Jump)));
        At(landingState, dashState, new FuncPredicate(() => inputRequests.Check(ActionRequest.Dash)));
        At(landingState, attackState, new FuncPredicate(() => inputRequests.Check(ActionRequest.Attack) && RequestAttack()));

        // dashing state transitions
        At(dashState, idleState, new FuncPredicate(() => dashState.GetProgress() <= 0 && motor.Grounded && !CheckIfMoving()));
        At(dashState, walkState, new FuncPredicate(() => dashState.GetProgress() <= 0 && motor.Grounded && CheckIfMoving()));
        At(dashState, risingState, new FuncPredicate(() => dashState.GetProgress() <= 0 
                                                           && !motor.Grounded 
                                                           && motor.currVelocity.y > 0.0f));
        At(dashState, fallingState, new FuncPredicate(() => dashState.GetProgress() <= 0 
                                                           && !motor.Grounded 
                                                           && motor.currVelocity.y <= 0.0f));

        // wall jump state transitions
        At(walljumpState, landingState, new FuncPredicate(() =>
                                        motor.Grounded));
        At(walljumpState, risingState, new FuncPredicate(() =>
                                        !motor.Grounded && motor.currVelocity.y > 0.0f &&
                                        walljumpState.IsFinished()));
        At(walljumpState, fallingState, new FuncPredicate(() =>
                                        !motor.Grounded && motor.currVelocity.y <= 0.0f &&
                                        walljumpState.IsFinished()));

        // interaction state transitions
        At(interactState, idleState, new FuncPredicate(() => currentInteraction == null && !CheckIfMoving())); // need to figure out how to do this!!!
        At(interactState, walkState, new FuncPredicate(() => currentInteraction == null && CheckIfMoving())); // need to figure out how to do this!!!

        // attack state transitions
        At(attackState, idleState, new FuncPredicate(() => motor.Grounded && attackState.GetProgress() <= 0 && !CheckIfMoving()));
        At(attackState, walkState, new FuncPredicate(() => motor.Grounded && attackState.GetProgress() <= 0 && CheckIfMoving()));
        At(attackState, risingState, new FuncPredicate(() => !motor.Grounded 
                                                             && attackState.GetProgress() <= 0
                                                             && motor.currVelocity.y > 0.0f));
        At(attackState, fallingState, new FuncPredicate(() => !motor.Grounded 
                                                             && attackState.GetProgress() <= 0 
                                                             && motor.currVelocity.y <= 0.0f));

        // chant state transitions
        At(chantState, idleState, new FuncPredicate(() => chantState.GetProgress() <= 0 && !CheckIfMoving()));
        At(chantState, walkState, new FuncPredicate(() => chantState.GetProgress() <= 0 && CheckIfMoving()));

        // set initial state
        fsm.SetState(idleState); // need to make a more robust way or "setting" or "forcing" this
        // Would be helpful for cutscenes, scene changes, etc.

        // action requests holder
        input.EnablePlayerActions();
        inputRequests = new ActionRequests();
    }

    private void Update()
    {        
        // read input
        SetMovementInputValues();
        // state machine
        fsm.Update();
    }

    private void OnEnable()
    {
        input.PollInputRequest += OnInputRequest;
    }

    private void OnDisable()
    {
        input.PollInputRequest -= OnInputRequest;
    }
    #endregion

    #region Gizmos
    private void OnDrawGizmosSelected()
    {
        
    }
    #endregion

    #region Input Handling
    private void SetMovementInputValues()
    {
        horizontalMovement = input.horizontalInput;
        verticalMovement = input.verticalInput;
        // clamp for animations (???)
    }

    private void OnInputRequest(ActionRequest action, bool performed)
    {
        inputRequests.SetRequest(action, performed);        
        // hard-coded meep, architecture fixes needed later probly
        if (action == ActionRequest.Interact)
        {
            RequestInteract();
        }
        // hard-coded meep
        if (action == ActionRequest.Attack && fsm.GetCurrentState() == attackState)
        {
            inputRequests.Check(ActionRequest.Attack);
            RequestAttack();
        }
    } 
    #endregion

    #region Movement Logic
    public Vector3 GetTargetRotation()
    {
        //targetRotationDirection = Vector3.zero;
        Vector3 targetRotationDirection = PlayerCamera.instance.playerCam.transform.forward * verticalMovement;
        targetRotationDirection = targetRotationDirection + PlayerCamera.instance.playerCam.transform.right * horizontalMovement;
        targetRotationDirection.Normalize();
        targetRotationDirection.y = 0;

        // if unchanged based on input
        if (targetRotationDirection.sqrMagnitude <= 0.01f)
        {
            targetRotationDirection = transform.forward;
        }

        return targetRotationDirection;
    }

    public bool CheckIfMoving()
    {
        moveAmount = input.moveAmount;
        return moveAmount > 0;
    }
    
    private Vector3 SetMovementDirection()
    {
        Vector3 mD = PlayerCamera.instance.transform.forward * verticalMovement;
        mD = mD + PlayerCamera.instance.transform.right * horizontalMovement;
        mD = NormalizeAndCutY(mD);
        return mD;
    }

    public Vector3 GetMovementDirection()
    {
        return SetMovementDirection();
    }

    private Vector3 NormalizeAndCutY(Vector3 input)
    {
        input.Normalize();
        input.y = 0;
        return input;
    }
    #endregion

    #region Interaction Check 
    // probably makes sense to return an IInteractable 
    private IInteractable GetClosestInteract()
    {
        RaycastHit hit;
        // basic sphere cast for now
        bool cast = Physics.SphereCast(player.transform.position + interactCheckTranslationOffset, // THIS IS WRONG IN SPACE!! FORWARD * OFFSET
                                       interactSphereRadius,
                                       transform.forward, // could be camera forward too!
                                       out hit,
                                       interactDistance);
        if (cast) {
            IInteractable interact = hit.collider.GetComponent<IInteractable>();
            Debug.Log(interact);
            if (interact != null && interact.CanInteract())
                return interact;
        }
        return null;
    }
    #endregion

    #region Actions
    // DASH
    #region Dash
    public Vector3 GetDashDirection()
    {
        Vector3 dashDir;
        // if no input, dash forward
        if (input.moveAmount <= 0.0)
            dashDir = gameObject.transform.forward;
        else
            dashDir = PlayerCamera.instance.transform.forward * verticalMovement
                    + PlayerCamera.instance.transform.right * horizontalMovement;

        return NormalizeAndCutY(dashDir);
    }
    #endregion

    // WALL BOUND
    #region Wall Jump
    private Vector3 FindSeekingDirection()
    {
        Vector3 dir;

        // create seeking direction
        if (input.moveAmount <= 0.0f)
        {
            dir = gameObject.transform.forward;
        }
        else
        {
            dir = PlayerCamera.instance.transform.forward * verticalMovement
                + PlayerCamera.instance.transform.right * horizontalMovement;
        }

        dir = NormalizeAndCutY(dir);
        return dir;
    }

    public Vector3 GetSeekingDirection()
    {
        return FindSeekingDirection();
    }

    public Vector3 GetBounceDirection(Vector3 seekDir, RaycastHit hit)
    {
        // reflect traveling direction over hit normal, then normalize and remove Y value
        return NormalizeAndCutY(Vector3.Reflect(seekDir, hit.normal)); 
    }
    #endregion

    // INTERACT
    #region Interact
    public void RequestInteract() {
        // check and interact
        if (inputRequests.Check(ActionRequest.Interact)) {
            currentInteraction = GetClosestInteract();
            // null exit
            if (currentInteraction == null)
                return;

            // interact
            currentInteraction?.Interact(this);
            // clear from object if we don't want to initiate a State change
            if (!currentInteraction.IsTrigger()) {
                currentInteraction = null;
            }
        }
        return;
    }

    public void ResetInteract()
    {
        Debug.Log("yeah i got a call...");
        currentInteraction = null;
        Debug.Log(currentInteraction);
    }
    #endregion

    // ATTACK
    #region Attack
    
    public bool RequestAttack()
    {
        AttackSO queuedAttack = combat.AttemptAttack();
        if (queuedAttack != null)
        {
            attackState.SetAttackInternals(queuedAttack);
            return true;
        }
        
        return false;
    }
    #endregion
    #endregion

    #region FSM
    void At(IState from, IState to, IPredicate condition) => fsm.AddTransition(from, to, condition);
    void Any(IState to, IPredicate condition) => fsm.AddAnyTransition(to, condition);

    public void SetState(string state)
    {
        switch (state)
        {
            case "interact":
                fsm.SetState(interactState);
                break;
            case "neutral":
                fsm.SetState(walkState);
                break;
        }
    }
    #endregion
}
