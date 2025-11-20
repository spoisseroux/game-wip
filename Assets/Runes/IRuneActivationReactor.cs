/*
    General interface for any objects that will be affected by a single rune's activation, whether from afar or nearby

    Need to figure out the data structuring for pushing updates to damage/health of mobs, damage of traps, erratic-ness of terrain, etc.,
    that's based on a rune's number of activations in a given run
*/
#region Interface
public interface IRuneActivationReactor
{
    public virtual void React() { return; }
}
#endregion

#region Specific Reactors
// Maybe we make Monobehaviour?
// On event receipt, we then read the AttackRuneSO and update base values based on the rune SO's activationcount
public class AttackRuneReactor : IRuneActivationReactor
{
    public AttackRuneReactor(AttackRune rune) {
        rune.OnAttackRuneActivation += React;
    }

    public void React() {
        
    }

    public void Unsubscribe(AttackRune rune) {
        rune.OnAttackRuneActivation -= React;
    }
}

public class DashRuneReactor : IRuneActivationReactor
{
    public DashRuneReactor(DashRune rune) {
        rune.OnDashRuneActivation += React;
    }

    public void React() {
        
    }

    public void Unsubscribe(DashRune rune) {
        rune.OnDashRuneActivation -= React;
    }
}

public class JumpRuneReactor : IRuneActivationReactor
{
    public JumpRuneReactor(JumpRune rune) {
        rune.OnJumpRuneActivation += React;
    }
    
    public void React() {
        
    }

    public void Unsubscribe(JumpRune rune) {
        rune.OnJumpRuneActivation -= React;
    }
}

public class WallJumpRuneReactor : IRuneActivationReactor
{
    public WallJumpRuneReactor(WallJumpRune rune) {
        rune.OnWallJumpRuneActivation += React;
    }

    public void React() {
        
    }

    public void Unsubscribe(WallJumpRune rune) {
        rune.OnWallJumpRuneActivation -= React;
    }
}
#endregion