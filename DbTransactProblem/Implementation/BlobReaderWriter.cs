using System;
using System.IO;
using DbTransactProblem.Interfaces;
using Starcounter;

namespace DbTransactProblem.Implementation
{
    public class BlobReaderWriter : IBlobReaderWriter
    {
        #region IBlobReaderWriter

        public Blob FromByteArray(byte[] source)
            => FromByteArrayStatic(source);

        public byte[] ToByteArray(Blob blob)
            => ToByteArrayStatic(blob);

        #endregion

        internal static Blob FromByteArrayStatic(byte[] source)
        {
            using (var ms = new MemoryStream(source, false))
                return FromStream(ms);
        }

        internal static byte[] ToByteArrayStatic(Blob blob)
        {
            var bytes = new byte[blob.Length];
            var pos = 0;
            for (var b = blob; b != null; b = b.Next)
            {
                var currentBytes = b.Data.ToArray();
                Buffer.BlockCopy(currentBytes, 0, bytes, pos, currentBytes.Length);
                pos += currentBytes.Length;
            }
            return bytes;
        }

        private static Blob FromStream(Stream source)
        {
            long len = 0;
            var buf = new byte[Blob.BufSize];
            var data = ReadBinary(source, buf);

            len += data.Length;
            var result = new Blob
            {
                Data = data
            };
            var current = result;
            for (data = ReadBinary(source, buf); data.Length > 0; data = ReadBinary(source, buf))
            {
                len += data.Length;
                current = current.Next = new Blob {Data = data, Length = data.Length};
            }
            result.Length = len;
            return result;
        }

        private static Binary ReadBinary(Stream source, byte[] buf)
        {
            using (var bs = new BinaryStream())
            {
                int read;
                while (bs.Length < Blob.BufSize && (read =
                           source.Read(buf, 0, Blob.BufSize - (int)bs.Length)) > 0)
                {
                    bs.Write(buf, 0, read);
                }
                return bs.ToBinary();
            }
        }

        public Stream GetStream(Blob blob) => new BlobStream(blob);

        private class BlobStream : Stream
        {
            private readonly Blob _blob;
            private Blob _current;
            private BinaryStream _stream;

            public BlobStream(Blob b)
            {
                _blob = _current = b;
                // ReSharper disable once ImpureMethodCallOnReadonlyValueField
                _stream = _current.Data.GetStream();
            }

            /// <inheritdoc />
            /// <summary>
            /// We can read.
            /// </summary>
            public override bool CanRead => true;

            /// <inheritdoc />
            /// <summary>
            /// Seeking not supported. Should be pretty easy to implement though.
            /// </summary>
            public override bool CanSeek => false;

            /// <inheritdoc />
            /// <summary>
            /// No.
            /// </summary>
            public override bool CanWrite => false;

            /// <inheritdoc />
            /// <summary>
            /// Writing not supported.
            /// </summary>
            public override void Flush() => throw new NotSupportedException();

            public override long Length => _blob.Length;

            /// <inheritdoc />
            /// <summary>
            /// Not supported.
            /// </summary>
            public override long Position
            {
                get => throw new NotSupportedException();
                set => throw new NotSupportedException();
            }

            public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

            public override void SetLength(long value) => throw new NotSupportedException();

            public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

            public override int Read(byte[] buffer, int offset, int count)
            {
                if (buffer == null)
                    throw new ArgumentNullException(nameof(buffer));
                if (offset < 0 || offset > buffer.Length)
                    throw new ArgumentOutOfRangeException(nameof(offset));
                if (count < 0 || count > buffer.Length - offset)
                    throw new ArgumentOutOfRangeException(nameof(count));
                return InternalRead(buffer, offset, count);
            }

            private int InternalRead(byte[] buffer, int offset, int count)
            {
                var read = 0;
                while (_current != null && read < count)
                {
                    read += _stream.Read(buffer, offset + read, count - read);
                    if (read >= count)
                        continue;
                    _current = _current.Next;
                    if (_current != null)
                        _stream = _current.Data.GetStream();
                }
                return read;
            }
        }

    }
}
