using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "newWeaponData", menuName = "Data/Weapon Data", order = 0)]
public class WeaponDataSO : ScriptableObject
{
    // model component
    [JsonIgnore]
    public GameObject modelPrefab;
    // basic attacks
    public List<AttackSO> basicAttackList;
    // attack range --> for now this is a standard magnitude applied to transform.forward, one for all attacks on the weapon
    public float attackRange;
    // stat scaling
    public StatID scalingStat;
    public float damageScalingFactor;
}