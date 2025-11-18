using UnityEngine;

public interface IWeapon
{
    void Tick(float delta);
    void Attack();
}