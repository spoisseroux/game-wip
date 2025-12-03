using UnityEditor.ShaderGraph.Internal;

public class MovementStatusHandler
{
    private float addBonus;
    private float multBonus;

    public MovementStatusHandler()
    {
        addBonus = 0;
        multBonus = 1;
    }

    public void ChangeAdditiveBonus(float input)
    {
        addBonus += input;
    }

    public void ChangeMultiplicativeBonus(float input)
    {
        multBonus += input;
    }

    public float ApplyBonuses(float movementValue)
    {
        return (multBonus * movementValue) + addBonus;
    }
}