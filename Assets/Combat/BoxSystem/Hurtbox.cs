using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HurtboxState
{
    Active,
    Inactive
}

public class Hurtbox
{
    private HurtboxState state = HurtboxState.Active;

    public Vector3 pos;
    public float length, width, height;
}
