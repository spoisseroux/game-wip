public interface IWeapon
{
    public AttackSO AttemptAttack(AttackSO attack, CombatPhase currentPhase); // determine next attack
}