using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// authority flags, essentially does any specific action control this axis of movement currently
public enum MovementAxis
{
    None = 0,
    Horizontal = 1,
    Vertical = 2, // vertical separate for complete control
    Gravity = 3, // gravity separate for wonky aerial movement
    Rotation = 4
}

public abstract class Mover : MonoBehaviour
{
    // buff system
    // [SerializeableField] BuffSystem buffStore;
    public MovementSettings movementSettings;

    protected Dictionary<MovementAxis, object> movementAxisOwners = new Dictionary<MovementAxis, object> {
        {MovementAxis.None, null},
        {MovementAxis.Horizontal, null},
        {MovementAxis.Vertical, null},
        {MovementAxis.Gravity, null},
        {MovementAxis.Rotation, null}
    };

    #region Horizontal Movement
    public abstract void SetVelocity(Vector3 dir, object source);
    public abstract void AddVelocity(Vector3 dir, object source);
    #endregion

    #region Rotation
    public abstract void AddRotation(Vector3 target, object source);
    public abstract void SetNewRotation(Vector3 targetDir, object source);
    #endregion

    #region Authority Set & Release
    public abstract void TryClaimAxis(MovementAxis axis, object affector);
    public abstract void ForceClaimAxis(MovementAxis axis, object affector);
    public abstract void ReleaseAxis(MovementAxis axis);
    #endregion

    #region Vertical Movement
    public abstract void SetVerticalVelocity(Vector3 newYVel, object affector);
    public abstract void AddVerticalVelocity(Vector3 addYVel, object affector);
    #endregion
}