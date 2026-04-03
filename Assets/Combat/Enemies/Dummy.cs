using UnityEngine;

public class Dummy : MonoBehaviour, IHittable
{
    public float health = 100;

    public void Hit(HitboxRecord hitboxRecord)
    {
        Debug.Log("Ow my health! I lost: " + hitboxRecord.context.damage.baseDamage + " health!");
        health -= hitboxRecord.context.damage.baseDamage;
        return;
    }

    public GameObject GetGameObject()
    {
        return this.gameObject;
    }
}
