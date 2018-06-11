using System;

namespace DbTransactProblem.Interfaces
{
    public interface IDbDataManipulationUtil
    {
        void Transact(Action action);
    }
}
