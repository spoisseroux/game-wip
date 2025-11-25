using UnityEngine;

public interface IWeapon
{
    public void Tick(float delta); // tick weapon
    public void Attack(); // actually spawn and execute hitboxes
    public AttackSO AttemptAttack(); // determine next attack
}