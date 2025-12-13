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
    // id
    public int databaseID;

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