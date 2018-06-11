using System;
using DbTransactProblem.Interfaces;
using Starcounter;

namespace DbTransactProblem.Implementation
{
    public class ScDbDataManipulationUtil : IDbDataManipulationUtil
    {
        public void Transact(Action action) => Db.Transact(action);
    }
}
