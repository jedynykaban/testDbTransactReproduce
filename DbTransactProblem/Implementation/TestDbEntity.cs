using System;
using Starcounter;

namespace DbTransactProblem.Implementation
{
    [Database]
    public class TestDbEntity
    {
        public TestDbEntity()
        {
            LastUpdateDate = CreationDate = DateTime.UtcNow;
        }

        public string Value { get; set; }

        public DateTime CreationDate { get; set; }
        public DateTime LastUpdateDate { get; set; }

        public Blob Content { get; set; }
    }
}
