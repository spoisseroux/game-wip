using System;

// registry of all stats, could i rename these to audio stuff? :3
public enum StatID { Health, Agility, Power, Magic, Defense }

// data object
public struct StatData
{  
    public float baseValue;
    public float flatBonus;
    public float multBonus;

    public float final { get => (baseValue * multBonus) + flatBonus; }

    public static StatData DefaultValue(float baseVal) => new StatData{
        baseValue = baseVal, 
        flatBonus = 0, 
        multBonus = 1
    };
}

public class Stat : IStat
{
    private StatData data;
    public event Action<float> OnValueChanged;
    public float finalValue { get => data.final; }

    public Stat(float inValue)
    {
        data = StatData.DefaultValue(inValue);
    }

    public StatData statData { get => data; }

    #region Interface
    public void Increase(float add)
    {
        data.baseValue += add;
        OnValueChanged?.Invoke(data.baseValue);
    }

    public void Set(float val)
    {
        data.baseValue = val;
        OnValueChanged?.Invoke(data.baseValue);
    }

    public void Reset(float defaultBaseValue)
    {
        data.baseValue = defaultBaseValue;
        // what do we do for storing a default? 
    }
    #endregion
}