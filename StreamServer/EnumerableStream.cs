using System;
using System.Collections.Generic;
using System.IO;

namespace StreamServer
{
    public abstract class EnumerableStream<T> : Stream
    {
        private const int DefaultItemBufferSize = 5000;

        private readonly IEnumerator<T> enumerator;
        private readonly int itemsBufferSize;

        private int bufferStreamCapacity;
        private Stream bufferStream;
        private StreamWriter writer;
        private bool writeStarted;
        private bool writeFinished;

        protected EnumerableStream(IEnumerable<T> enumerable) : this(enumerable, DefaultItemBufferSize)
        {
        }

        protected EnumerableStream(IEnumerable<T> enumerable, int itemsBufferSize)
        {
            enumerator = enumerable?.GetEnumerator() ?? throw new ArgumentException();
            this.itemsBufferSize = itemsBufferSize;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int readResult = bufferStream?.Read(buffer, offset, count) ?? default(int);

            if (readResult == default(int) && !writeFinished)
            {
                FillBuffer();
                readResult = bufferStream.Read(buffer, offset, count);
            }

            return readResult;
        }

        protected override void Dispose(bool disposing)
        {
            DisposeBuffer();
            enumerator?.Dispose();
            base.Dispose(disposing);
        }

        private void FillBuffer()
        {
            DisposeBuffer();
            InitializeBuffer();

            for (int i = 0; i < itemsBufferSize; i++)
            {
                WriteItem();
            }

            writer.Flush();
            bufferStream.Position = default(int);
        }

        private void DisposeBuffer()
        {
            bufferStreamCapacity = bufferStream?.CanRead == true ? (int) bufferStream.Length : default(int);

            writer?.Dispose();
            bufferStream?.Dispose();
        }

        private void InitializeBuffer()
        {
            bufferStream = new MemoryStream(bufferStreamCapacity);

            writer = new StreamWriter(bufferStream);
        }

        protected void WriteItem()
        {
            if (!writeStarted)
            {
                writer.Write(FirstRow);
            }

            if (enumerator.MoveNext())
            {
                if (writeStarted)
                {
                    writer.Write(DelimiterToken);
                }

                writer.Write(SerializeItem(enumerator.Current));
            }
            else if (!writeFinished)
            {
                writer.Write(LastRow);
                writeFinished = true;
            }

            writeStarted = true;
        }

        protected abstract string SerializeItem(T item);
        protected abstract string DelimiterToken { get; }
        protected abstract string FirstRow { get; }
        protected abstract string LastRow { get; }

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }
    }
}