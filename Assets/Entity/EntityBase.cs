using UnityEngine;

public class EntityBase : MonoBehaviour, IHittable
{
    // components
    [SerializeField] protected Animator animator;

    #region IDamageable Interface
    public void Hit(HitboxRecord hitboxRecord)
    {
        return;
    }
    #endregion
}
