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




    // offset location for spawning hitboxes ?? usually ends up being a magnitude applied to this.transform.forward anyways....
    // public Vector3 offset;
}