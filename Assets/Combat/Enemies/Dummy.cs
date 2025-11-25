using UnityEngine;

public class Dummy : MonoBehaviour, IDamageable
{
    public int health = 100;

    public void TakeDamage(int damage)
    {
        Debug.Log("hurting");
        health -= damage;
    }
}
