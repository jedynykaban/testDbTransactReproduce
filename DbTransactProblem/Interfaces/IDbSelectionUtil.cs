namespace DbTransactProblem.Interfaces
{
    public interface IDbSelectionUtil
    {
        TOut SelectFirstOrDefault<TOut>();
    }
}
