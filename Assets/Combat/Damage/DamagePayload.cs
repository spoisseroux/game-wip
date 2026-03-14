/*
    Parent class for all damage information to be used in applying a damage amount
*/
[System.Serializable]
public class DamagePayload
{
    float baseDamage;
    DamageType type;

    // status effect

    // etc?
}

[System.Serializable]
public enum DamageType
{
    Normal
}