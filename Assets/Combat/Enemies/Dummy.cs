using UnityEngine;

public class Dummy : MonoBehaviour, IDamageable
{
    public int health = 100;

    public void TakeDamage(int damage)
    {
        health -= damage;
    }

    public void TakeDamage(int damage, IHitboxSource source)
    {
        TakeDamage(damage);
    }
}
