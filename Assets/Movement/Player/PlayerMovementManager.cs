using System;
using System.Collections.Generic;
using UnityEngine;

// TODO: eventually abstract this out into generic entity logic, moving grounded/airborne/actionflags into a generic movement manager
public class PlayerMovementManager : MonoBehaviour
{
    // parent
    PlayerManager player;

    // components
    [SerializeField] public CharacterController characterController;
    [SerializeField] InputReader input;
    [SerializeField] PlayerCombatManager combat;
    [SerializeField] AnimationController animationController;

    // fsm & states
    [SerializeField] StateMachine fsm;
    public NeutralState neutralState;
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
    [HideInInspector] public float horizontalMovement;
    [HideInInspector] public float verticalMovement;
    [HideInInspector] public float moveAmount;

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

    // Status fx??
    public MovementStatusHandler moveBuffHandler { get; private set;}

    [Header("Movement Settings")]
    public Vector3 yVel;
    public Vector3 moveDirection;
    public Vector3 targetRotationDirection; // camera & rotation
    [SerializeField] float walkSpeed = 15f;
    [SerializeField] float rotationSpeed;


    [Header("Gravity")]
    [SerializeField] private float minYVel = -20f; // maximum speed player can fall at
    [SerializeField] private float startingYVel = -5f;
    [SerializeField] public float risingGravityForce = -40f;
    [SerializeField] public float fallingGravityForce = -25f;


    [Header("Physics Checks")]
    // Grounded Check, want the widest portion of sphere shown in DrawGizmosSelected to reach the edge of either foot
    [SerializeField] float groundCheckSphereRadius = 0.4f;
    [SerializeField] Vector3 groundCheckTranslationAdjustment = new Vector3(0f, 0f, 0f);
    [SerializeField] LayerMask groundLayer;
    public bool isGrounded = true;

    // probably a good idea to separate this out into a separate script, maybe even a monobehavior 
    // then it could store the InteractTrigger it is involved with and reset itself
    // other scripts read it for state changes and logic flows
    [Header("Interaction Check")]
    [SerializeField] float interactSphereRadius = 0.2f;
    [SerializeField] Vector3 interactCheckTranslationOffset = new Vector3(0f, 1f, 0f);
    [SerializeField] float interactDistance = 2f;
    [SerializeField] IInteractable currentInteraction;

    [Header("Dash")]
    [SerializeField] public float dashSpeed = 30f;

    [Header("Jump")]
    // TODO: grace periods, coyote time (after leaving platform) && jump buffer (input buffering sorta deal)
    public bool bonusJumpTaken = false;
    [SerializeField] float jumpHeight = 4f;

    [Header("WallJump")]
    [SerializeField] LayerMask wallLayer;
    [SerializeField] float seekingSpeed;
    [SerializeField] float bouncingSpeed;
    [SerializeField] float wallJumpY; // play around with making this a formula to calc based on current yVel.y, or Min(5, yVel.y + 15);
    // detection
    [SerializeField] float detectionRange = 10f;
    [SerializeField] float wallJumpSphereRaycastRadius = 0.4f;
    [SerializeField] Vector3 castPosOffset = new Vector3(0.0f, -0.4f, 0.0f);

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

        // states
        neutralState = new NeutralState(this, animationController);
        jumpingState = new JumpingState(this, animationController);
        risingState = new RisingState(this, animationController);
        fallingState = new FallingState(this, animationController);
        landingState = new LandingState(this, animationController);
        dashState = new DashingState(this, animationController);
        walljumpState = new WallJumpState(this, animationController);
        interactState = new InteractState(this, animationController);
        attackState = new AttackState(this, combat, animationController);
        chantState = new ChantState(this, animationController);

        // neutral state transitions
        At(neutralState, jumpingState, new FuncPredicate(() =>
                                        inputRequests.Check(ActionRequest.Jump)));
        At(neutralState, dashState, new FuncPredicate(() => inputRequests.Check(ActionRequest.Dash)));
        At(neutralState, interactState, new FuncPredicate(() => currentInteraction != null && currentInteraction.IsTrigger()
                                        && isGrounded));
        At(neutralState, attackState, new FuncPredicate(() => inputRequests.Check(ActionRequest.Attack) && RequestAttack()));
        At(neutralState, chantState, new FuncPredicate(() => false)); // how to fire an event to pipe into here?

        // jumping state transitions
        At(jumpingState, risingState, new FuncPredicate(() => jumpingState.GetProgress() <= 0));
        At(jumpingState, dashState, new FuncPredicate(() => inputRequests.Check(ActionRequest.Dash)));
        At(jumpingState, attackState, new FuncPredicate(() => inputRequests.Check(ActionRequest.Attack) && RequestAttack()));
        At(jumpingState, walljumpState, new FuncPredicate(() =>
                                        inputRequests.Check(ActionRequest.WallJump)
                                        && !isGrounded));

        // rising state transitions
        At(risingState, jumpingState, new FuncPredicate(() =>
                                        inputRequests.Check(ActionRequest.Jump)
                                        && !bonusJumpTaken));
        At(risingState, fallingState, new FuncPredicate(() => !isGrounded && GetVerticalMovementComponent().y <= 0.0f));
        At(risingState, dashState, new FuncPredicate(() => inputRequests.Check(ActionRequest.Dash)));
        At(risingState, attackState, new FuncPredicate(() => inputRequests.Check(ActionRequest.Attack) && RequestAttack()));
        At(risingState, walljumpState, new FuncPredicate(() => inputRequests.Check(ActionRequest.WallJump)));

        // falling state transitions
        At(fallingState, jumpingState, new FuncPredicate(() =>
                                        inputRequests.Check(ActionRequest.Jump)
                                        && !bonusJumpTaken));
        At(fallingState, landingState, new FuncPredicate(() => isGrounded));
        At(fallingState, dashState, new FuncPredicate(() => inputRequests.Check(ActionRequest.Dash)));
        At(fallingState, attackState, new FuncPredicate(() => inputRequests.Check(ActionRequest.Attack) && RequestAttack()));
        At(fallingState, walljumpState, new FuncPredicate(() => inputRequests.Check(ActionRequest.WallJump)));

        // landing state transitions
        At(landingState, jumpingState, new FuncPredicate(() => inputRequests.Check(ActionRequest.Jump)));
        At(landingState, neutralState, new FuncPredicate(() => landingState.GetProgress() <= 0));
        At(landingState, dashState, new FuncPredicate(() => inputRequests.Check(ActionRequest.Dash)));
        At(landingState, attackState, new FuncPredicate(() => inputRequests.Check(ActionRequest.Attack) && RequestAttack()));

        // dashing state transitions
        At(dashState, neutralState, new FuncPredicate(() => dashState.GetProgress() <= 0 && isGrounded));
        At(dashState, risingState, new FuncPredicate(() => dashState.GetProgress() <= 0 
                                                           && !isGrounded 
                                                           && GetVerticalMovementComponent().y > 0.0f));
        At(dashState, fallingState, new FuncPredicate(() => dashState.GetProgress() <= 0 
                                                           && !isGrounded 
                                                           && GetVerticalMovementComponent().y <= 0.0f));

        // wall jump state transitions
        At(walljumpState, landingState, new FuncPredicate(() =>
                                        isGrounded));
        At(walljumpState, risingState, new FuncPredicate(() =>
                                        !isGrounded && GetVerticalMovementComponent().y > 0.0f &&
                                        walljumpState.IsFinished()));
        At(walljumpState, fallingState, new FuncPredicate(() =>
                                        !isGrounded && GetVerticalMovementComponent().y <= 0.0f &&
                                        walljumpState.IsFinished()));

        // interaction state transitions
        At(interactState, neutralState, new FuncPredicate(() => currentInteraction == null)); // need to figure out how to do this!!!

        // attack state transitions
        At(attackState, neutralState, new FuncPredicate(() => isGrounded && attackState.GetProgress() <= 0));
        At(attackState, risingState, new FuncPredicate(() => !isGrounded 
                                                             && attackState.GetProgress() <= 0
                                                             && GetVerticalMovementComponent().y > 0.0f));
        At(attackState, fallingState, new FuncPredicate(() => !isGrounded 
                                                             && attackState.GetProgress() <= 0 
                                                             && GetVerticalMovementComponent().y <= 0.0f));

        // chant state transitions
        At(chantState, neutralState, new FuncPredicate(() => chantState.GetProgress() <= 0));

        // set initial state
        fsm.SetState(neutralState);

        // action requests holder
        input.EnablePlayerActions();
        inputRequests = new ActionRequests();
    }

    private void Start()
    {
        // maybe move states and transitions here
        moveBuffHandler = new MovementStatusHandler();
    }

    private void Update()
    {
        // tick status effects
        
        // read input
        SetMovementValues();
        // checks
        HandleGravity();
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
        // ground check
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(GetComponent<PlayerManager>().gameObject.transform.position + groundCheckTranslationAdjustment, groundCheckSphereRadius);

        // walljump raycast, isn't rly working anyways, bad linalg
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position + castPosOffset,
                            wallJumpSphereRaycastRadius);
    }
    #endregion

    #region Input Handling
    private void SetMovementValues()
    {
        horizontalMovement = input.horizontalInput;
        verticalMovement = input.verticalInput;
        // clamp for animations (???)
    }

    private Vector3 SetWalkMovementDir()
    {
        Vector3 mD = PlayerCamera.instance.transform.forward * verticalMovement;
        mD = mD + PlayerCamera.instance.transform.right * horizontalMovement;
        mD = NormalizeAndCutY(mD);
        return mD;
    }

    private Vector3 NormalizeAndCutY(Vector3 input)
    {
        input.Normalize();
        input.y = 0;
        return input;
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
    public void HandleRotation()
    {
        targetRotationDirection = Vector3.zero;
        targetRotationDirection = PlayerCamera.instance.playerCam.transform.forward * verticalMovement;
        targetRotationDirection = targetRotationDirection + PlayerCamera.instance.playerCam.transform.right * horizontalMovement;
        targetRotationDirection.Normalize();
        targetRotationDirection.y = 0;

        // if unchanged based on input
        if (targetRotationDirection == Vector3.zero)
        {
            targetRotationDirection = transform.forward;
        }

        Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);
        Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = targetRotation;
    }

    public void HandleGravity()
    {
        GroundedCheck();
        if (isGrounded)
        {
            // not attempting to jump, stick to the ground and reset jump counter
            if (yVel.y <= 0)
            {
                yVel.y = startingYVel;
                bonusJumpTaken = false;
            }
        }
        else
        {
            // rising gravity
            if (yVel.y > 0)
            {
                yVel.y += risingGravityForce * Time.deltaTime;
            }
            // falling gravity
            else
            {
                yVel.y += fallingGravityForce * Time.deltaTime;
            }
        }
        // clamp to -20f
        yVel.y = Mathf.Max(yVel.y, minYVel);
        // apply
        characterController.Move(yVel * Time.deltaTime);
    }

    public void SetNewRotation(Vector3 targetDir)
    {
        targetDir.Normalize();
        targetDir.y = 0;

        Quaternion newRotation = Quaternion.LookRotation(targetDir);
        transform.rotation = newRotation;
    }

    public bool CheckIfMoving()
    {
        moveAmount = input.moveAmount;
        return moveAmount > 0;
    }

    public void Walk()
    {
        float finalSpeed = moveBuffHandler.ApplyBonuses(walkSpeed);
        moveDirection = SetWalkMovementDir();
        characterController.Move(finalSpeed * Time.deltaTime * moveDirection);
    }

    public void Dash()
    {
        characterController.Move(dashSpeed * Time.deltaTime * moveDirection);
    }

    public void SeekWall()
    {
        characterController.Move(seekingSpeed * Time.deltaTime * moveDirection);
    }

    public void BounceOffWall()
    {
        characterController.Move(bouncingSpeed * Time.deltaTime * moveDirection);
    }
    #endregion

    #region Physics Checks 
    public void GroundedCheck()
    {
        isGrounded = Physics.CheckSphere(player.transform.position + groundCheckTranslationAdjustment, groundCheckSphereRadius, groundLayer);
    }

    public Tuple<bool, RaycastHit> WallContactCheck()
    {
        RaycastHit hitData;
        bool hitVal = Physics.SphereCast(transform.position + castPosOffset, // THIS IS WRONG IN SPACE!! FORWARD * OFFSET
                            wallJumpSphereRaycastRadius,
                            moveDirection,
                            out hitData,
                            detectionRange,
                            wallLayer);
        return new Tuple<bool, RaycastHit>(hitVal, hitData);
    }

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
        Debug.Log(cast);
        if (cast) {
            Debug.Log("cast hit!");
            return hit.collider.GetComponent<IInteractable>();
        }
        return null;
    }
    
    // check vertical velocity
    public Vector3 GetVerticalMovementComponent()
    {
        return yVel;
    }
    #endregion

    #region Actions
    // DASH
    #region Dash
    public void SetDashDirection()
    {
        // if no input, dash forward
        if (input.moveAmount <= 0.0)
            moveDirection = gameObject.transform.forward;
        else
            moveDirection = PlayerCamera.instance.transform.forward * verticalMovement
                          + PlayerCamera.instance.transform.right * horizontalMovement;

        moveDirection = NormalizeAndCutY(moveDirection);
    }
    #endregion

    // JUMP
    #region Jump
    public void ApplyJumpingVelocity()
    {
        if (!isGrounded)
        {
            bonusJumpTaken = true;
        }

        // applying rising force at jump starts
        yVel.y = Mathf.Sqrt(jumpHeight * -2 * risingGravityForce);
    }
    #endregion

    // WALL BOUND
    #region Wall Jump

    public void SetSeekingDirection()
    {
        moveDirection = FindSeekingDirection();
    }

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
            dir = PlayerCamera.instance.transform.forward * verticalMovement;
            dir += PlayerCamera.instance.transform.right * horizontalMovement;
        }

        dir = NormalizeAndCutY(dir);
        return dir;
    }

    public void SetBounceDirection(RaycastHit hit)
    {
        Vector3 b = Vector3.Reflect(moveDirection, hit.normal);
        b = NormalizeAndCutY(b);
        moveDirection = b;
        // edit rotation too
        SetNewRotation(moveDirection);
    }

    public void WallJumpBoost()
    {
        yVel.y += wallJumpY;
    }
    #endregion

    // INTERACT
    #region Interact
    public void RequestInteract() {
        // check and interact
        if (inputRequests.Check(ActionRequest.Interact)) {
            Debug.Log("checking for interacts");
            currentInteraction = GetClosestInteract();
            // null exit
            if (currentInteraction == null)
                return;

            // interact
            Debug.Log("found one!");
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
    #endregion

    #region StatusEffects
    public void ChangeAdditiveBonus(float input)
    {
        moveBuffHandler.ChangeAdditiveBonus(input);
    }

    public void ChangeMultBonus(float input)
    {
        moveBuffHandler.ChangeMultiplicativeBonus(input);
    }

    #endregion

}
