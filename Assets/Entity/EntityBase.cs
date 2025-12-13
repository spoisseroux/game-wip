using UnityEngine;

public class EntityBase : MonoBehaviour, IDamageable
{
    // components
    [SerializeField] protected Animator animator;

    #region IDamageable Interface
    public void TakeDamage(int amount)
    {
        return;
    }

    public void TakeDamage(int amount, IHitboxSource source)
    {
        return;
    }
    #endregion
}
