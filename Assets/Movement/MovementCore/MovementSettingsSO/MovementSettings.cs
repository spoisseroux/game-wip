using UnityEngine;

[CreateAssetMenu(fileName = "MovementSettings", menuName = "Data/MovementSettingsSO")]
public class MovementSettings : ScriptableObject
{
    public float walkSpeed;
    public float jumpHeight;
    public float rotationSpeed;
    public float minimumYVelocity;
}
