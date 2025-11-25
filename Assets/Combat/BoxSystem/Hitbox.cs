using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public IHitboxSource source { get; private set; }
    public HitboxState state { get; private set; }

    // positioning information
    public Box box;
    public Vector3 position;
    public Quaternion orientation;

    // timing
    public float activeTime; // this in frames or engine ticks??? same with timer!! has to be clarified at some point
    public CountdownTimer active;

    // dictionary for ensuring hits don't balloon to infinity
    private Dictionary<Collider, int> hitColliders;
    private int hitCount;

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

        // stored hits
        hitColliders = new Dictionary<Collider, int>();

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

        // check for overlaps with damageable objects
        Collider[] cols = Physics.OverlapBox(position, new Vector3(box.length / 2, box.width / 2, box.height / 2), orientation);
        for (int i = 0; i < cols.Length; i++)
        {
            // check against previous hits
            if (hitColliders.ContainsKey(cols[i]))
            {
                if (hitColliders[cols[i]] < hitCount) {
                    source?.CollisionedWith(cols[i]);
                    hitColliders[cols[i]]++;
                }
            }
            else
            {
                source?.CollisionedWith(cols[i]);
                hitColliders.Add(cols[i], 1);
            }
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
        source = null;
        state = HitboxState.Closed;
    }
}
