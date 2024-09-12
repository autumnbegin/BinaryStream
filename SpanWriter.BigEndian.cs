using System.Runtime.CompilerServices;

namespace ProtocolStream
{
    public partial struct SpanWriter
    {
        public void WriteWithBigEndian(ushort value)
        {
            this.WriteUnalignedWithBigEndian(ref value, 2);
        }

        public void WriteWithBigEndian(short value)
        {
            this.WriteUnalignedWithBigEndian(ref value, 2);
        }

        public void WriteWithBigEndian(uint value)
        {
            this.WriteUnalignedWithBigEndian(ref value, 4);
        }

        public void WriteWithBigEndian(int value)
        {
            this.WriteUnalignedWithBigEndian(ref value, 4);
        }

        public void WriteWithBigEndian(ulong value)
        {
            this.WriteUnalignedWithBigEndian(ref value, 8);
        }

        public void WriteWithBigEndian(long value)
        {
            this.WriteUnalignedWithBigEndian(ref value, 8);
        }

        public void WriteWithBigEndian(UInt128 value)
        {
            this.WriteUnalignedWithBigEndian(ref value, 16);
        }

        public void WriteWithBigEndian(Int128 value)
        {
            this.WriteUnalignedWithBigEndian(ref value, 16);
        }

        public void WriteWithBigEndian(Half value)
        {
            this.WriteUnalignedWithBigEndian(ref value, 2);
        }

        public void WriteWithBigEndian(float value)
        {
            this.WriteUnalignedWithBigEndian(ref value, 4);
        }

        public void WriteWithBigEndian(double value)
        {
            this.WriteUnalignedWithBigEndian(ref value, 8);
        }

        private void WriteUnalignedWithBigEndian<T>(ref T value, int size)
        {
            var p = _bytePointer;

            _bytePointer += size;
            this.ValidatePointer();
            unsafe
            {
                var src = (byte*)Unsafe.AsPointer(ref value);
                var dst = _buffer.Slice(p, size);

                for (int i = 0, j = size - 1; i < size; i++, j--)
                {
                    dst[i] = src[j];
                }
            }
        }
    }
}
