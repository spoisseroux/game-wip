using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HitboxState
{
    Closed,
    Open,
    Colliding // iffy
}

// could turn this into Box / Sphere, Shape component with overlap
// Monobehavior for Gizmos
public class Hitbox : MonoBehaviour
{
    public LayerMask mask;
    private HitboxState state = HitboxState.Closed; // default closed

    public Vector3 pos = Vector3.one;
    public float length = 0.5f, width = 0.5f, heigh = 0.5f;
    public float activeTime = 1f; // this in frames or engine ticks??? same with timer!! has to be clarified at some point

    public CountdownTimer active;

    public void Initialize()
    {
        // create the timer
        active = new CountdownTimer(activeTime);
        active.OnStart = StartCheckingCollision;
        active.OnStop = StopCheckingCollision;

        // start --> open collider and begin countdown
        active.Start();
    }

    public void Tick(float deltaTime)
    {
        active.Tick(deltaTime);
    }

    public void StartCheckingCollision()
    {
        state = HitboxState.Open; 
    }

    public void StopCheckingCollision()
    {
        state = HitboxState.Closed;
    }

    #region Gizmos
    public Color inactiveColor = Color.green;
    public Color collisionOpenColor = Color.red;
    public Color collidingColor = Color.blue;

    private void OnDrawGizmos()
    {
        CheckGizmoColor();
        Gizmos.color = Color.red;
        //Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
        //Gizmos.DrawCube(Vector3.zero, new Vector3(boxSize.x * 2, boxSize.y * 2, boxSize.z * 2)); // Because size is halfExtents
    }

    private void CheckGizmoColor()
    {
        switch(state) {
        case HitboxState.Closed:
            Gizmos.color = inactiveColor;
            break;
        case HitboxState.Open:
            Gizmos.color = collisionOpenColor;
            break;
        case HitboxState.Colliding:
            Gizmos.color = collidingColor;
            break;
        }
    }
    #endregion
}
