using System;
using System.Linq;
using DbTransactProblem.Interfaces;
using Starcounter;

namespace DbTransactProblem.Implementation
{
    public class TestRepository : ITestRepository
    {
        private readonly IDbSelectionUtil _dbSelectionUtil;
        private readonly IDbDataManipulationUtil _dbDataManipulationUtil;
        private readonly IBlobReaderWriter _blobReaderWriter;

        public TestRepository(IDbSelectionUtil dbSelectionUtil, IDbDataManipulationUtil dbDataManipulationUtil, IBlobReaderWriter blobReaderWriter)
        {
            _dbSelectionUtil = dbSelectionUtil;
            _dbDataManipulationUtil = dbDataManipulationUtil;
            _blobReaderWriter = blobReaderWriter;
        }

        public TestViewModel GetFirst()
        {
            var testDbEntity = _dbSelectionUtil.SelectFirstOrDefault<TestDbEntity>();
            var typedJson = new TestViewModel();
            if (testDbEntity == null)
                return typedJson;
            
            var bytes = _blobReaderWriter.ToByteArray(testDbEntity.Content);
            typedJson.PopulateFromJson(bytes, bytes.Length);
            return typedJson;
        }

        public TestViewModel AddOrUpdate(TestViewModel testVmEntity)
        {
            if (testVmEntity == null)
                throw new ArgumentNullException(nameof(testVmEntity));

            var testDbEntity = _dbSelectionUtil.SelectFirstOrDefault<TestDbEntity>();

            _dbDataManipulationUtil.Transact(() =>
            {
                var newEntity = false;
                if (testDbEntity == null)
                {
                    testDbEntity = new TestDbEntity();
                    newEntity = true;
                }

                testDbEntity.Value = testVmEntity.value;
                testDbEntity.Content?.Delete();
                testDbEntity.Content = _blobReaderWriter.FromByteArray(testVmEntity.ToJsonUtf8());

                if (!newEntity)
                    //throw new Exception("Called on purpose, check if data changes has been rolled back!");
                {
                    var dbEntity = _dbSelectionUtil.SelectAll<TestDbEntity>().Single(te => te.GetObjectNo() == 123456789);
                    if (dbEntity == null)
                        throw new Exception();
                }
            });

            return testVmEntity;
        }

        public TestViewModel AddOrUpdateDbTransactExplicitlyCalled(TestViewModel testVmEntity)
        {
            if (testVmEntity == null)
                throw new ArgumentNullException(nameof(testVmEntity));

            var testDbEntity = _dbSelectionUtil.SelectFirstOrDefault<TestDbEntity>();

            var newEntity = false;
            Db.Transact(() =>
            {
                if (testDbEntity == null)
                { 
                    testDbEntity = new TestDbEntity();
                    newEntity = true;
                }

                testDbEntity.Value = testVmEntity.value;
                testDbEntity.Content?.Delete();
                testDbEntity.Content = _blobReaderWriter.FromByteArray(testVmEntity.ToJsonUtf8());

                if (!newEntity)
                    throw new Exception("Called on purpose, data changes has been rolled back in this code configuration");
            });

            return testVmEntity;
        }
    }
}
