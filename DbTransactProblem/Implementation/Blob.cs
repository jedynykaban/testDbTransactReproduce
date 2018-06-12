using Starcounter;

namespace DbTransactProblem.Implementation
{
    [Database]
    public class Blob : IEntity
    {
        // following: https://docs.starcounter.io/guides/database/data-types#binary , Binary can store up to 1MB
        internal const int BufSize = 0xFFFFF;

        public Binary Data { get; set; }
        public Blob Next { get; set; }
        public long Length { get; set; }

        /// <inheritdoc />
        public void OnDelete()
        {
            var next = Next;
            next?.Delete();
        }
    }
}