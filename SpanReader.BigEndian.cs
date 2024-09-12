using System.Runtime.CompilerServices;

namespace ProtocolStream
{
    public partial struct SpanReader
    {
        public ushort ReadUShortWithBigEndian()
        {
            return ReadUnalignedWithBigEndian<ushort>(2);
        }

        public short ReadShortWithBigEndian()
        {
            return ReadUnalignedWithBigEndian<short>(2);
        }

        public uint ReadUIntWithBigEndian()
        {
            return ReadUnalignedWithBigEndian<uint>(4);
        }

        public int ReadIntWithBigEndian()
        {
            return ReadUnalignedWithBigEndian<int>(4);
        }

        public ulong ReadULongWithBigEndian()
        {
            return ReadUnalignedWithBigEndian<ulong>(8);
        }

        public long ReadLongWithBigEndian()
        {
            return ReadUnalignedWithBigEndian<long>(8);
        }

        public UInt128 ReadUInt128WithBigEndian()
        {
            return ReadUnalignedWithBigEndian<UInt128>(16);
        }

        public Int128 ReadInt128WithBigEndian()
        {
            return ReadUnalignedWithBigEndian<Int128>(16);
        }

        public Half ReadHalfWithBigEndian()
        {
            return ReadUnalignedWithBigEndian<Half>(2);
        }

        public float ReadFloatWithBigEndian()
        {
            return ReadUnalignedWithBigEndian<float>(4);
        }

        public double ReadDoubleWithBigEndian()
        {
            return ReadUnalignedWithBigEndian<double>(8);
        }

        private T ReadUnalignedWithBigEndian<T>(int size) where T : unmanaged
        {
            var value = default(T);
            var bytePointer = _bytePointer;

            _bytePointer += size;
            ValidatePointer();
            unsafe
            {
                var src = _buffer.Slice(bytePointer, size);
                var dst = (byte*)Unsafe.AsPointer(ref value);

                for (int i = 0, j = size - 1; i < size; i++, j--)
                {
                    dst[i] = src[j];
                }
            }
            return value;
        }
    }
}
