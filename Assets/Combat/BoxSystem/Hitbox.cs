using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
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
[Serializable]
public class Hitbox
{
    public IHitboxSource source { get; private set; } // weapon or player? have to pick an object to host this hitbox eventually. it can be tied back to player in any case
    public HitboxState state { get; private set; }

    // positioning information
    public Box box;
    public Vector3 position;
    public Quaternion orientation;

    // timing
    public float activeTime; // this in frames or engine ticks??? same with timer!! has to be clarified at some point
    public CountdownTimer active;

    // de-register from Hitbox list event
    public event Action<Hitbox> unload = delegate { };

    public Hitbox(float time, Vector3 p, Box b, Quaternion q, IHitboxSource s)
    {
        // parent
        source = s;

        // position info 
        position = p;
        box = b;
        orientation = q;

        // timing
        activeTime = time;

        // state
        state = HitboxState.Closed;

        // create the timer
        active = new CountdownTimer(activeTime);
        active.OnStart = StartCheckingCollision;
        active.OnStop = StopCheckingCollision;
    }

    public void Execute()
    {
        // start --> open collider and begin countdown
        active.Start();
    }

    public void Tick(float deltaTime)
    {
        active.Tick(deltaTime);
        if (state != HitboxState.Open) { return; }

        Debug.Log("ticking active");

        // check for overlaps, function calls for half-extents, eventually add in orientation of Player
        Collider[] cols = Physics.OverlapBox(position + box.pos, new Vector3(box.length / 2, box.width / 2, box.height / 2));
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
        Debug.Log("hitbox ending");
        // cleanup function
        unload?.Invoke(this);
        source = null;
        state = HitboxState.Closed;
    }
}
