using UnityEngine;
using System.Collections.Generic;

public enum RuneType
{
    Attack,
    Dash,
    Jump,
    WallJump
}

public interface IRune {
    // gunna have to figure out how we separate 'rendering' and cutscene/control pausing 
    // FROM actual backend data information about what the rune activated

    public abstract void Chant();
    /*
        // create the cutscene
        // play sounds and sfx
        // play shaders and vfx
        // ... more?
    */

    public abstract void Activate(); 
    /*
        // alert any listeners for this rune's activation
        // buff owner
        // .... more?
    */
}


// TODO:
    // need to decide how to 'register' an owner, and push updates on activation to them
    // need to learn how to specifically affect concerned units (mobs, objects, doors, traps)
[CreateAssetMenu(fileName = "AttackRune", menuName = "Runes/RuneData/AttackRune")]
public class AttackRune : RuneDataSO, IRune
{
    public delegate void OnRuneActivation(); 
    public event OnRuneActivation OnAttackRuneActivation;

    public void Chant() {
        return;
    }

    public void Activate() {
        activationCount++;
        OnAttackRuneActivation?.Invoke();
        return;
    }
}

[CreateAssetMenu(fileName = "DashRune", menuName = "Runes/RuneData/DashRune")]
public class DashRune : RuneDataSO, IRune
{
    public delegate void OnRuneActivation(); 
    public event OnRuneActivation OnDashRuneActivation;

    public void Chant() {
        return;
    }

    public void Activate() {
        activationCount++;
        OnDashRuneActivation?.Invoke();
        return;
    }
}

[CreateAssetMenu(fileName = "JumpRune", menuName = "Runes/RuneData/JumpRune")]
public class JumpRune : RuneDataSO, IRune
{
    public delegate void OnRuneActivation(); 
    public event OnRuneActivation OnJumpRuneActivation;

    public void Chant() {
        return;
    }

    public void Activate() {
        // implement activation count
        activationCount++;
        OnJumpRuneActivation?.Invoke();
        return;
    }
}

[CreateAssetMenu(fileName = "WallJumpRune", menuName = "Runes/RuneData/WallJumpRune")]
public class WallJumpRune : RuneDataSO, IRune
{
    public delegate void OnRuneActivation(); 
    public event OnRuneActivation OnWallJumpRuneActivation;

    public void Chant() {
        return;
    }

    public void Activate() {
        activationCount++;
        OnWallJumpRuneActivation?.Invoke();
        return;
    }
}