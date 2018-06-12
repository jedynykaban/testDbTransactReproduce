using DbTransactProblem.Implementation;

namespace DbTransactProblem.Interfaces
{
    public interface IBlobReader
    {
        byte[] ToByteArray(Blob blob);
    }

    public interface IBlobWriter
    {
        Blob FromByteArray(byte[] source);
    }

    /// <inheritdoc cref="IBlobReader" />
    /// <inheritdoc cref="IBlobWriter" />
    public interface IBlobReaderWriter : IBlobReader, IBlobWriter
    {
    }
}
