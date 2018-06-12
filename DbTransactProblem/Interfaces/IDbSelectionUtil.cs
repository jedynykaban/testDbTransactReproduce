using System.Collections.Generic;

namespace DbTransactProblem.Interfaces
{
    public interface IDbSelectionUtil
    {
        TOut SelectFirstOrDefault<TOut>();
        IEnumerable<TOut> SelectAll<TOut>();
    }
}
