using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HitboxState
{
    Closed,
    Open,
    Colliding // iffy
}

public interface IHitboxSource
{
    void CollisionedWith(Collider col);
}

// Monobehavior for Gizmos
public class Hitbox : MonoBehaviour
{
    private IHitboxSource source;
    private HitboxState state = HitboxState.Closed; // default closed

    // positioning information
    public Box box;

    // timing
    public float activeTime; // this in frames or engine ticks??? same with timer!! has to be clarified at some point
    public CountdownTimer active;

    // de-register from Hitbox list event
    public event Action unload = delegate { };

    // good for just registering data to a given weapon's attacks
    public void Initialize(float time, Box b)
    {
        // position info 
        box = b;
        activeTime = time;

        // create the timer
        active = new CountdownTimer(activeTime);
        active.OnStart = StartCheckingCollision;
        active.OnStop = StopCheckingCollision;
    }

    public void Execute(IHitboxSource s)
    {
        // set source weapon/player
        source = s;

        // start --> open collider and begin countdown
        active.Start();
    }

    public void Tick(float deltaTime, Vector3 sourcePos)
    {
        active.Tick(deltaTime);
        if (state != HitboxState.Open) { return; }

        // check for overlaps
        Collider[] cols = Physics.OverlapBox(sourcePos + box.pos, new Vector3(box.length / 2, box.width / 2, box.height / 2));
        for (int i = 0; i < cols.Length; i++)
        {
            source?.CollisionedWith(cols[i]);
        }
    }

    public void StartCheckingCollision()
    {
        state = HitboxState.Open;
    }

    public void StopCheckingCollision()
    {
        // cleanup function
        source = null;
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
        switch (state)
        {
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
