using UnityEngine;

public abstract class RuneDataSO : ScriptableObject {
    // sfx file
    // associated state, hmmmmm where...
    // sprite
    public Sprite UIimage;
    // affect duration
    public float activeLength;
    // EnumVal
    public RuneType runeValue;
    // activation count for pushing updates to mobs, traps, etc.
    public int activationCount;
}
