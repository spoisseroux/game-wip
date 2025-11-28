using System;
using UnityEngine;

[CreateAssetMenu(fileName = "newAttackDataSO", menuName = "Data/AttackDataSO")]
public class AttackSO : ScriptableObject
{
    // duration for the whole attack --> [windup + active + finishing anim]
    public float attackDuration;
    // duration of hitbox
    public float hitboxDuration;
    // damage
    public int damage;
    // hitbox
    public Box hitbox; // could make a list!!!!
    // movespeed modifier
    public float movespeedModifier;
    // anim
    public string animClipName;
    // combo window, check against attackstate timer?
    public float comboWindowStart;
    public float comboWindowEnd;
    // hit number
    public int hitCount;
}
