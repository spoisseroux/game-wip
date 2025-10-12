using UnityEngine;

public class TestWeapon : MonoBehaviour, IWeapon
{
    public int damage = 1;

    #region Weapon Interface
    public void BasicAttack()
    {
        throw new System.NotImplementedException();
    }

    public void AerialAttack()
    {
        throw new System.NotImplementedException();
    }
    #endregion
}