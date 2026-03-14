using UnityEngine;

public class Dummy : MonoBehaviour, IHittable
{
    public int health = 100;

    public void Hit(HitboxRecord hitboxRecord)
    {
        return;
    }
}
