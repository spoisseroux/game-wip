using UnityEngine;

[CreateAssetMenu(fileName = "newWeaponData", menuName = "Data/Weapon Data", order = 0)]
public class WeaponDataSO : ScriptableObject
{
    // time component
    public float length;
    // hitbox sizing component
    public Box hitboxData;
    // damage component
    public int damage;
    // movement component
    public bool controlPlayer;
    public float velocityMultiplier;
    // animation component
    public string animClipName;
    // model component
    public GameObject model;
}