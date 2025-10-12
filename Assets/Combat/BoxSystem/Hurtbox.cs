public enum HurtboxState
{
    Active,
    Inactive
}

public class Hurtbox
{
    // state
    private HurtboxState state = HurtboxState.Active;

    // positioning
    public Box box;
}
