using UnityEngine;

public class Dummy : MonoBehaviour, IHittable
{
    public int health = 100;

    public void Hit(HitboxRecord hitboxRecord)
    {
        return;
    }

    public GameObject GetGameObject()
    {
        return this.gameObject;
    }
}
