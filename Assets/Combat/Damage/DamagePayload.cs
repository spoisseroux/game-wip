/*
    Parent class for all damage information to be used in applying a damage amount
*/
[System.Serializable]
public class DamagePayload
{
    public float damageValue;
    public DamageType type;

    // have to redo this when considering status effects and allied/neutral attacks but start here and expand igs

    // etc?

    public DamagePayload(float damageIn, DamageType typeIn = DamageType.Normal)
    {
        damageValue = damageIn;
        type = typeIn;
    }
}

[System.Serializable]
public enum DamageType
{
    Normal,
    Ambient,
    Treble,
    Bass
}