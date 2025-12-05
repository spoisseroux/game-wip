using System.Collections.Generic;
/*
    Reserved for things like Doors which specifically react to a sequence of runes, and would generate an interaction as a result
*/
public interface IChantReactor
{
    public abstract void React(List<RuneType> runes);
}
