using System.Collections.Generic;
using UnityEngine;

public enum RuneType
{
    Attack,
    Dash,
    Jump,
    WallJump
}

[CreateAssetMenu(fileName = "__Rune", menuName = "Runes/RuneData/Runes")]
public class RuneDataSO : ScriptableObject {
    // sfx file
    public AudioClip soundFX;
    // highlight shader
    public Material shader;
    // sprite
    public Sprite UIimage;
    // affect duration
    public float activeLength;
    // EnumVal
    public RuneType runeValue;
    // activation count for pushing updates to mobs, traps, etc.
    public int activationCount;
    // list of effects
    public List<IStatusEffect> effects;
}

// concrete runes
/*
[CreateAssetMenu(fileName = "AttackRune", menuName = "Runes/RuneData/AttackRune")]
public class AttackRune : RuneDataSO
{
    
}

[CreateAssetMenu(fileName = "DashRune", menuName = "Runes/RuneData/DashRune")]
public class DashRune : RuneDataSO
{
    
}

[CreateAssetMenu(fileName = "JumpRune", menuName = "Runes/RuneData/JumpRune")]
public class JumpRune : RuneDataSO
{
    
}

[CreateAssetMenu(fileName = "WallJumpRune", menuName = "Runes/RuneData/WallJumpRune")]
public class WallJumpRune : RuneDataSO
{
    
}
*/