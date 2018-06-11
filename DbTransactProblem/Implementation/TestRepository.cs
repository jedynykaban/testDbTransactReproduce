using System;
using DbTransactProblem.Interfaces;
using Starcounter;

namespace DbTransactProblem.Implementation
{
    public class TestRepository : ITestRepository
    {
        private readonly IDbSelectionUtil _dbSelectionUtil;
        private readonly IDbDataManipulationUtil _dbDataManipulationUtil;

        public TestRepository(IDbSelectionUtil dbSelectionUtil, IDbDataManipulationUtil dbDataManipulationUtil)
        {
            _dbSelectionUtil = dbSelectionUtil;
            _dbDataManipulationUtil = dbDataManipulationUtil;
        }

        public TestViewModel GetFirst()
        {
            var testDbEntity = _dbSelectionUtil.SelectFirstOrDefault<TestDbEntity>();
            return new TestViewModel { value = testDbEntity?.Value ?? "" };
        }

        public TestViewModel AddOrUpdate(TestViewModel testVmEntity)
        {
            if (testVmEntity == null)
                throw new ArgumentNullException(nameof(testVmEntity));

            var testDbEntity = _dbSelectionUtil.SelectFirstOrDefault<TestDbEntity>();

            _dbDataManipulationUtil.Transact(() =>
            {
                if (testDbEntity == null)
                    testDbEntity = new TestDbEntity();

                testDbEntity.Value = testVmEntity.value;
                throw new Exception("Called on purpose, check if data changes has been rolled back!");
            });

            return testVmEntity;
        }

        public TestViewModel AddOrUpdateDbTransactExplicitlyCalled(TestViewModel testVmEntity)
        {
            if (testVmEntity == null)
                throw new ArgumentNullException(nameof(testVmEntity));

            var testDbEntity = _dbSelectionUtil.SelectFirstOrDefault<TestDbEntity>();

            Db.Transact(() =>
            {
                if (testDbEntity == null)
                    testDbEntity = new TestDbEntity();

                testDbEntity.Value = testVmEntity.value;
                throw new Exception("Called on purpose, data changes has been rolled back in this code configuration");
            });

            return testVmEntity;
        }
    }
}
