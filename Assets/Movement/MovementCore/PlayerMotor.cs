using System.Collections.Generic;
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
    [SerializeField] CharacterController cc;
    [SerializeField] Transform owner;

    // requested movement
    private Vector3 accumulatedMovement;
    private Vector3 targetRotationDirection;

    // current vals
    public Vector3 currVelocity => accumulatedMovement;

    #region Physics Check Transforms
    // ground check
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private LayerMask groundMask; // hmmmm maybe too much terrain to manually do this. maybe leave untouched?
    public bool Grounded { get => Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask); }
    
    // wall jump check
    [SerializeField] private Transform wallCheck;
    [SerializeField] private float wallCheckRadius;
    public bool WallTouch { get => Physics.CheckSphere(wallCheck.position, wallCheckRadius); }

    // ceiling check
    [SerializeField] private Transform ceilingCheck;
    public bool CeilingTouch { get => Physics.CheckSphere(ceilingCheck.position, groundCheckRadius); }
    #endregion

    // fix l8r :3
    #region Constant Base Values
    public float rotationSpeed;
    #endregion

    // gravity component

    // buff system --> check for movement buffs
    // weapon system --> check for weapon movement effects
    // apply after receiving SetVelocity logic from other components to move the player

    private Dictionary<MovementAxis, object> movementAxisOwners = new();
    // maybe change object type into a MoveCommand type? 
    // then we can assign priority and compare for authority
    // maybe move the ticking and curves into this component for reference?

    #region MonoBehaviour
    private void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    private void Update()
    {
        // check logic stuff? which authority values run out? which routines are ending?
    } 

    private void LateUpdate()
    {
        ResolveVerticalMovement(); // --> check Vertical first, then Gravity

        // add rotation, PMM returns transform.forward if 0.0f, 0.0f, 0.0f
        Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);
        Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
        owner.rotation = targetRotation;

        // move
        if (accumulatedMovement != Vector3.zero)
        {
            cc.Move(accumulatedMovement * Time.deltaTime);
        }
    }
    #endregion

    /*
        For procedures that are specifically for affecting the CharacterController component
    */
    #region CharacterController API
    // the controlling actions calls SetVelocity(magnitude * directionNormal)
    public override void SetVelocity(Vector3 dir, object source)
    {
        // check authority, bail out if not the owned object

        // add movement
        accumulatedMovement += dir * Time.deltaTime;
    }

    public override void AddVelocity(Vector3 dir, object source)
    {
        
    }
    #endregion

    /*
        For altering the Transform component directly
    */
    #region Transform API
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

    public override void SetNewRotation(Vector3 targetDir, object source) { }
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

    #region Vertical Helpers
    private void ResolveVerticalMovement()
    {
        // if vertical is not owned, then we apply gravity
        if (movementAxisOwners[MovementAxis.Vertical] == null)
        {
            // essentially, given the current y velocity value, return the next value of y based on gravity
            // gravity stores the previous frame's calculated y value internally
            // it wants to affect y differently based on where player is in jump height 
            
            // accumulatedMovement.y = gravity.Tick(accumulatedMovement.y);
        }
    }

    #endregion
}