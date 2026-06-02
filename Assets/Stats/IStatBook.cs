// interface for stat books
public interface IStatBook
{
    IStat Get(StatID id);
    bool TryGet(StatID id, out IStat stat);
}