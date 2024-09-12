using System.Runtime.CompilerServices;

namespace ProtocolStream
{
    public partial struct SpanReader
    {
        public T ReadAligned<T>() where T : unmanaged
        {
            return ReadAligned<T>(Unsafe.SizeOf<T>() << 3);
        }

        public T ReadAligned<T>(int bitCount) where T : unmanaged
        {
            var bytePointer = _bytePointer;
            var bitPointer = _bitPointer;

            _bitPointer += bitCount;
            _bytePointer += _bitPointer >> 3;
            _bitPointer &= 7;
            ValidatePointer();
            if (bitCount <= 0)
            {
                throw new ArgumentException(null, nameof(bitCount));
            }
            var realBytLen = Unsafe.SizeOf<T>();
            var realBitLen = realBytLen << 3;
            var bytIndex = 0;
            var bitIndex = 0;
            var value = default(T);

            unsafe
            {
                var srcPtr = _buffer.Slice(bytePointer);
                var tarPtr = (byte*)Unsafe.AsPointer(ref value);

                for (; bitIndex < bitCount; bitIndex += 8, bytIndex++)
                {
                    if (bitIndex - bitPointer + 16 < bitCount)
                    {
                        if (bitIndex < realBitLen)
                        {
                            tarPtr[bytIndex] |= (byte)(srcPtr[bytIndex] >> bitPointer);
                            if (bitIndex + bitPointer <= realBitLen)
                            {
                                tarPtr[bytIndex] |= (byte)(srcPtr[bytIndex + 1] << 8 - bitPointer);
                            }
                        }
                    }
                    else if (bitIndex - bitPointer + 8 < bitCount)
                    {
                        var maskOffset = bitCount + bitPointer - bitIndex - 8;
                        var dataOffset = Math.Min(bitPointer, maskOffset);

                        if (bitIndex < realBitLen)
                        {
                            tarPtr[bytIndex] |= (byte)(srcPtr[bytIndex] >> bitPointer);
                            if (bitIndex + bitPointer <= realBitLen)
                            {
                                tarPtr[bytIndex] |= (byte)((srcPtr[bytIndex + 1] & (1 << dataOffset) - 1)
                                    << 8 - bitPointer);
                            }
                        }
                    }
                    else
                    {
                        var bitOffset = bitCount - bitIndex;
                        tarPtr[bytIndex] |= (byte)(srcPtr[bytIndex] >> bitPointer & (1 << bitOffset) - 1);
                    }
                }
            }
            return value;
        }
    }
}
