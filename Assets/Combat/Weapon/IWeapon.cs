using UnityEngine;

public interface IWeapon
{
    public void Tick(float delta);
    public void Attack();
    public bool AttemptAttack();
}