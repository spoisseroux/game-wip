using UnityEngine;

[CreateAssetMenu(fileName = "StatusEffectSO", menuName = "StatusEffect/StatusEffectData")]
public class StatusEffectSO : ScriptableObject
{
    // duration
    public float duration;
    // values
    public float additiveValue;
    public float multValue;
}
