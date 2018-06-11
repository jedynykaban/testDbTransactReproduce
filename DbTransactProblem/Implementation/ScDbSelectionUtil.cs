using System.Linq;
using DbTransactProblem.Interfaces;
using Starcounter;

namespace DbTransactProblem.Implementation
{
    public class ScDbSelectionUtil : IDbSelectionUtil
    {
        public static string SelectAllQueryStatement<TOut>() => $"SELECT x FROM {typeof(TOut).FullName} x";

        public TOut SelectFirstOrDefault<TOut>()
            => Db.SQL<TOut>(SelectAllQueryStatement<TOut>()).FirstOrDefault();
    }
}
