using System;

// interface
public interface IStat
{
    float finalValue { get; }

    public void Increase(float add);
    public void Set(float val);
    public void Reset(float defaultBaseValue);

    event Action<float> OnValueChanged;
}
