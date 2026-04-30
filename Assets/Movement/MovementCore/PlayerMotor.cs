using System;
using UnityEngine;

/*
    Ok, the ~object~, let's make it... it could end up just being a BasePlayerState object

    Every one of these Commands has:
        - a priority value 
            --> defines what Actions it can interrupt
            --> what Actions it can be interrupted by
            --> what Actions it executes simultaneously with, etc.

        - List<MovementPhase>
            --> executed in owning object, i.e. an attack or dash etc.
            --> full list added 

        - a routine flag (HMMMM, CAN THIS GO IN ACTIONS AND THE COMMAND JUST ORCHESTRATES ACCORDINGLY???)
            --> does this just trigger in a single moment? only check priority and execute accordingly
            --> is there a temporal element to this? try claim authority, then execute accordingly

    Realistically, the corresponding state should be responsible for the timing, storage and orchestrating of these Commands

    Every MovementAction has:
        - a routine flag 
            --> does this just trigger in a single moment? only check priority and execute accordingly
            --> is there a temporal element to this? try claim authority, then execute accordingly
        - a duration
            --> maybe just == 0 if trigger only, read and react?
*/



public class PlayerMotor : Mover
{
    [Header("Components")]
    [SerializeField] CharacterController cc;
    [SerializeField] Transform owner;

    // requested movement
    [Header("Stored Vectors")]
    private Vector3 accumulatedHorizontalMovement;
    private Vector3 targetRotationDirection;
    [SerializeField] private Vector3 yVel;

    // current vals
    public Vector3 currVelocity => accumulatedHorizontalMovement + yVel;

    public bool bonusJumpTaken = false;

    #region Physics Check Transforms
    [Header("Ground Check")]
    // want the widest portion of sphere shown in DrawGizmosSelected to reach the edge of either foot
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private LayerMask groundMask; // hmmmm maybe too much terrain to manually do this. maybe leave untouched?
    public bool Grounded { get => Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask); }
    
    // wall jump check, boxcast 
    [Header("Wall Jump Check")]
    [SerializeField] private Transform wallCheck; // position, rotation
    [SerializeField] private Vector3 wallJumpBoxMult; // width, height, length
    [SerializeField] private LayerMask layersToIgnore;
    public Tuple<bool, RaycastHit> WallContact(Vector3 direction, float dist) { 
        RaycastHit hit;
        bool foundHit = Physics.BoxCast(wallCheck.position, Vector3.Scale(transform.localScale, wallJumpBoxMult), direction, out hit, transform.rotation, dist, ~layersToIgnore);
        return new Tuple<bool, RaycastHit>(foundHit, hit);
    }

    // ceiling check
    [Header("Ceiling Check")]
    [SerializeField] private Transform ceilingCheck;
    public bool CeilingTouch { get => Physics.CheckSphere(ceilingCheck.position, groundCheckRadius); }
    #endregion

    // fix l8r :3
    /*
    #region Constant Base Values
    [Header("Base Movement Values")]
    public MovementSettings movementSettings;
    #endregion
    */

    // gravity component
    [SerializeField] Gravity gravity;

    // buff system --> check for movement buffs
    // weapon system --> check for weapon movement effects
    // apply after receiving SetVelocity logic from other components to move the player

    // maybe change object type into a MoveCommand type? 
    // then we can assign priority and compare for authority
    // maybe move the ticking and curves into this component for reference?

    #region MonoBehaviour
    private void Awake()
    {
        cc = GetComponent<CharacterController>();
        yVel = Vector3.zero; // fresh start?
    }

    private void Update()
    {
        // check logic stuff? which authority values run out? which routines are ending?
        if (Grounded)
            bonusJumpTaken = false;

        // add gravity
        HandleGravity(); // directly edit yVal

        // add vertical control routines (should be in state? i think.)

        // adjust rotation
        if (targetRotationDirection.sqrMagnitude <= 0.001f)
            targetRotationDirection = transform.forward;
    } 

    private void LateUpdate()
    {
        /*
            Do we move all of this into FixedUpdate and then clear our values in LateUpdate????
        */

        // rotation
        Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);
        Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * movementSettings.rotationSpeed);
        transform.rotation = targetRotation; // have to set the transform directly

        // combine XZ and Y dirs, then move
        cc.Move(Time.deltaTime * currVelocity);

        // clear rotational & horizontal ONLY
        accumulatedHorizontalMovement = Vector3.zero;
        targetRotationDirection = Vector3.zero;
    }
    #endregion

    #region Gizmos
    private void OnDrawGizmos()
    {
        // ground check
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(groundCheck.position,
                            groundCheckRadius);
        
        // wall jump
        RaycastHit hit;
        float hitDist = 2f;
        bool wallCheckHit = Physics.BoxCast(wallCheck.position, Vector3.Scale(transform.localScale, wallJumpBoxMult), transform.forward, out hit, transform.rotation, hitDist);
        if (wallCheckHit)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(wallCheck.position, transform.forward * hit.distance);
            Gizmos.DrawWireCube(wallCheck.position + transform.forward * hit.distance, Vector3.Scale(transform.localScale, wallJumpBoxMult) * 2); // need to re-expand from half-extents
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(wallCheck.transform.position, transform.forward * hitDist);
        }

        // ceiling check
    }
    #endregion

    /*
        For procedures that are specifically for affecting the CharacterController component
    */
    #region Horizontal Movement
    // the controlling actions calls SetVelocity(magnitude * directionNormal)
    // any delta-time is handled in final call to CharacterController.Move(...)
    public override void SetVelocity(Vector3 dir, object source)
    {
        // check authority, bail out if not the owned object

        // add movement
        accumulatedHorizontalMovement = dir;
    }

    public override void AddVelocity(Vector3 dir, object source)
    {
        accumulatedHorizontalMovement += dir;
    }
    #endregion

    /*
        For altering the Transform component directly
    */
    #region Rotation
    /*
        This function in normal routines (walk, rotate to interact, airborne, etc.) is called from each individual state
        But provided a value from the PlayerMovementManager's HandleRotation function 
        It has current camera positioning and facing direction info that is necessary to compute the target rotation
    */
    public override void AddRotation(Vector3 target, object source)
    {
        // check authority???

        // player movement manager call
        targetRotationDirection = target;
    }
     
    /*
        Used to force a unit to a specific rotation immediately or much more quickly
        Usually to be used by an interaction or a cutscene
    */
    public override void SetNewRotation(Vector3 targetDir, object source)
    {
        Quaternion newRotation = Quaternion.LookRotation(targetDir);
        transform.rotation = newRotation;
    }
    #endregion

    #region Authority Set & Release
    public override void TryClaimAxis(MovementAxis axis, object affector)
    {
        // check for object affecting
    }

    public override void ForceClaimAxis(MovementAxis axis, object affector)
    {
        movementAxisOwners[axis] = affector;
    }

    public override void ReleaseAxis(MovementAxis axis)
    {
        movementAxisOwners[axis] = null;
    }
    #endregion

    #region Vertical Movement
    public override void SetVerticalVelocity(Vector3 newYVel, object affector)
    {
        yVel = newYVel;
    }

    public override void AddVerticalVelocity(Vector3 addYVel, object affector)
    {
        yVel += addYVel;
    }

    // directly edits yVel value
    private void HandleGravity()
    {
        // gravity
        if (Grounded)
        {
            bonusJumpTaken = false;
            // not attempting to jump, stick to the ground and reset jump counter
            if (yVel.y <= 0.0f) {
                yVel = gravity.ApplyGroundedGravity();
            }
        }
        else
            yVel += gravity.ApplyAirborneGravity(yVel.y);
        
        // clamp
        yVel.y = Mathf.Max(yVel.y, movementSettings.minimumYVelocity);
    }

    private Vector3 ResolveVerticalMovement()
    {
        Vector3 verticalMovement = yVel;

        /*
            Need to fix the logic on this, but essentially there should be some determination on whether the following occurs
            1. Vertical takes over Gravity completely
            2. Vertical applies and then Gravity applies
            3. Only gravity applies

            Need to make some authority stuff and assign priorities to objects, then maybe it'll be clearer

            For now, we just apply Gravity and let everything else do as it wants


            For Gravity --> essentially, we have some sort of object owning it, and then on ApplyGravity call we augment it???
        */ 
        return Vector3.zero;
    }

    #endregion
}