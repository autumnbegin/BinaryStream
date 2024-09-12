using System.Runtime.CompilerServices;

namespace ProtocolStream
{
    public partial struct SpanWriter
    {
        public void WriteAligned<T>(T value) where T : unmanaged
        {
            this.WriteAligned(value, Unsafe.SizeOf<T>() << 3);
        }

        public void WriteAligned<T>(T value, int bitCount) where T : unmanaged
        {
            var p = _bytePointer;
            var q = _bitPointer;

            _bitPointer += bitCount;
            _bytePointer += _bitPointer >> 3;
            _bitPointer &= 7;
            this.ValidatePointer();
            if (bitCount <= 0)
            {
                throw new ArgumentException(null, nameof(bitCount));
            }
            var realBytLen = Unsafe.SizeOf<T>();
            var realBitLen = realBytLen << 3;
            var bytIndex = 0;
            var bitIndex = 0;

            unsafe
            {
                var srcPtr = (byte*)Unsafe.AsPointer(ref value);
                var tarPtr = _buffer[p..];

                tarPtr[0] &= (byte)((1 << q) - 1);

                for (; bitIndex < bitCount; bitIndex += 8, bytIndex++)
                {
                    if (bitIndex - q + 16 < bitCount)
                    {
                        tarPtr[bytIndex + 1] = 0;
                        if (bitIndex < realBitLen)
                        {
                            tarPtr[bytIndex] |= (byte)(srcPtr[bytIndex] << q);
                            if (bitIndex + q <= realBitLen)
                            {
                                tarPtr[bytIndex + 1] |= (byte)(srcPtr[bytIndex] >> 8 - q);
                            }
                        }
                    }
                    else if (bitIndex - q + 8 < bitCount)
                    {
                        var maskOffset = bitCount + q - bitIndex - 8;
                        var dataOffset = Math.Min(q, maskOffset);

                        tarPtr[bytIndex + 1] &= (byte)~((1 << maskOffset) - 1);
                        if (bitIndex < realBitLen)
                        {
                            tarPtr[bytIndex] |= (byte)(srcPtr[bytIndex] << q);
                            if (bitIndex + q <= realBitLen)
                            {
                                tarPtr[bytIndex + 1] |= (byte)(srcPtr[bytIndex] >> 8 - q
                                    & (1 << dataOffset) - 1);
                            }
                        }
                    }
                    else
                    {
                        var bitOffset = bitCount - bitIndex;
                        tarPtr[bytIndex] |= (byte)((srcPtr[bytIndex] & (1 << bitOffset) - 1) << q);
                    }
                }
            }
        }
    }
}
