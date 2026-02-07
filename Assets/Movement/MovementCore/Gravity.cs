using Unity.VisualScripting;
using UnityEngine;

// MAYBE MAKE ABSTRACT AND HAVE GRAVITY APPLY DIFFERENTLY
[CreateAssetMenu(fileName="GravitySO", menuName="Data/GravitySO")]
public class Gravity : ScriptableObject
{
    // configs
    [SerializeField] public float startingYVelocity; // resting fall speed
    [SerializeField] public float risingGravityForce; // force of gravity applied while unit is still rising upwards
    [SerializeField] public float fallingGravityForce; // force of gravity applied while unity is falling downwards

    // given current downward velocity of an airborne unit, returns next value given one tick of gravity
    public Vector3 ApplyAirborneGravity(float curr)
    {
        float updated = 0.0f;
        // rising
        if (curr > 0f)
            updated = risingGravityForce * Time.deltaTime;
        // falling
        else
            updated = fallingGravityForce * Time.deltaTime;

        return new Vector3(0.0f, updated, 0.0f);
    }

    // applies gravity to a grounded unit, primarily for sticking them to the ground
    public Vector3 ApplyGroundedGravity()
    {
        return new Vector3(0.0f, startingYVelocity, 0.0f);
    }
}