using UnityEngine;

public interface IHittable
{
    public abstract void Hit(HitboxRecord hitRecord);
    public abstract GameObject GetGameObject();
}