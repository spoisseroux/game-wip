using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newWeaponData", menuName = "Data/Weapon Data", order = 0)]
public class WeaponDataSO : ScriptableObject
{
    // model component
    public GameObject model;
    // attacks
    public List<AttackSO> basicAttackList;
}