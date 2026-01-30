using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerMotor : Mover
{
    [SerializeField] CharacterController cc;

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
        // check which authority values run out?
    } 

    private void LateUpdate()
    {
        ResolveVerticalMovement(); // --> check Vertical first, then Gravity

        // add rotation

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

    public override void AddRotation(Quaternion target, object source)
    {
        
    }

    public override void SetNewRotation(Vector3 targetDir, object source) { }
    #endregion

    /*
        For altering the Transform component directly
    */
    #region Transform API
    #endregion

    #region Authority Set & Release
    public override void TryClaimAxis(MovementAxis axis, object affector)
    {
        // check for object affecting
    }

    public override void ForceClaimAxis(MovementAxis axis, object affector)
    {
        
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