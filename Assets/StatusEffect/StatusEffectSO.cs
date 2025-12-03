using UnityEngine;

public enum StatusEffectType
{
    Movement,
    Combat
}

public enum StatDestination
{
    MoveSpeed,
    JumpHeight,
    WallJumpSpeed,


    Damage
}

public enum StatusEffectMode
{
    Multiplicative,
    Additive
}

[CreateAssetMenu(fileName = "StatusEffectSO", menuName = "StatusEffect/StatusEffectData")]
public class StatusEffectSO : ScriptableObject
{
    // rune alignment
    public RuneType rune;
    // duration
    public float duration;
    // type
    public StatusEffectType type;
    // mode
    public StatusEffectMode mode;
    // value
    public float value;
}
