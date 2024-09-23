using CommunityToolkit.HighPerformance;
using System.Runtime.CompilerServices;
using System.Text;

namespace ProtocolStream
{
    public ref partial struct SpanReader
    {
        private readonly ReadOnlySpan<byte> _buffer;

        private readonly int _length;

        private int _bytePointer = 0;

        private int _bitPointer = 0;

        public SpanReader(byte[] buffer)
        {
            _length = buffer.Length;
            _buffer = buffer.AsSpan();
        }

        public SpanReader(ReadOnlyMemory<byte> buffer)
        {
            _buffer = buffer.Span;
            _length = buffer.Length;
        }

        public SpanReader(ReadOnlySpan<byte> buffer)
        {
            _buffer = buffer;
            _length = buffer.Length;
        }

        public readonly int BytePointer => _bytePointer;

        public readonly int BitPointer => _bitPointer;

        public readonly int Length => _length;

        public void Seek(int bytePosition)
        {
            _bytePointer = bytePosition;
            _bitPointer = 0;
        }

        public void Seek(int bytePosition, int bitPosition)
        {
            _bytePointer = bytePosition + (bitPosition >> 3);
            _bitPointer = bitPosition & 7;
        }

        public void Offset(int byteOffset, bool isResetBitPosition = true)
        {
            _bytePointer += byteOffset;
            if (isResetBitPosition)
            {
                _bitPointer = 0;
            }
        }

        public void Offset(int byteOffset, int bitOffset)
        {
            _bitPointer += bitOffset;
            _bytePointer += byteOffset + (_bitPointer >> 3);
            _bitPointer &= 7;
        }

        public byte ReadByte()
        {
            var p = _bytePointer;
            _bytePointer += 1;
            this.ValidatePointer();
            return Unsafe.ReadUnaligned<byte>(in _buffer[p]);
        }

        public sbyte ReadSByte()
        {
            var p = _bytePointer;
            _bytePointer += 1;
            this.ValidatePointer();
            return Unsafe.ReadUnaligned<sbyte>(in _buffer[p]);
        }

        public char ReadChar()
        {
            var p = _bytePointer;
            _bytePointer += 1;
            this.ValidatePointer();
            return Unsafe.ReadUnaligned<char>(in _buffer[p]);
        }

        public ushort ReadUShort()
        {
            var p = _bytePointer;
            _bytePointer += 2;
            this.ValidatePointer();
            return Unsafe.ReadUnaligned<ushort>(in _buffer[p]);
        }

        public short ReadShort()
        {
            var p = _bytePointer;
            _bytePointer += 2;
            this.ValidatePointer();
            return Unsafe.ReadUnaligned<short>(in _buffer[p]);
        }

        public uint ReadUInt()
        {
            var p = _bytePointer;
            _bytePointer += 4;
            this.ValidatePointer();
            return Unsafe.ReadUnaligned<uint>(in _buffer[p]);
        }

        public int ReadInt()
        {
            var p = _bytePointer;
            _bytePointer += 4;
            this.ValidatePointer();
            return Unsafe.ReadUnaligned<int>(in _buffer[p]);
        }

        public ulong ReadULong()
        {
            var p = _bytePointer;
            _bytePointer += 8;
            this.ValidatePointer();
            return Unsafe.ReadUnaligned<ulong>(in _buffer[p]);
        }

        public long ReadLong()
        {
            var p = _bytePointer;
            _bytePointer += 8;
            this.ValidatePointer();
            return Unsafe.ReadUnaligned<long>(in _buffer[p]);
        }

        public UInt128 ReadUInt128()
        {
            var p = _bytePointer;
            _bytePointer += 16;
            this.ValidatePointer();
            return Unsafe.ReadUnaligned<UInt128>(in _buffer[p]);
        }

        public Int128 ReadInt128()
        {
            var p = _bytePointer;
            _bytePointer += 16;
            this.ValidatePointer();
            return Unsafe.ReadUnaligned<Int128>(in _buffer[p]);
        }

        public Half ReadHalf()
        {
            var p = _bytePointer;
            _bytePointer += 2;
            this.ValidatePointer();
            return Unsafe.ReadUnaligned<Half>(in _buffer[p]);
        }

        public float ReadFloat()
        {
            var p = _bytePointer;
            _bytePointer += 4;
            this.ValidatePointer();
            return Unsafe.ReadUnaligned<float>(in _buffer[p]);
        }

        public double ReadDouble()
        {
            var p = _bytePointer;
            _bytePointer += 8;
            this.ValidatePointer();
            return Unsafe.ReadUnaligned<double>(in _buffer[p]);
        }

        public string ReadString(int length)
        {
            return this.ReadString(length, Encoding.ASCII);
        }

        public string ReadString(int length, Encoding encoding)
        {
            var p = _bytePointer;
            _bytePointer += length;
            this.ValidatePointer();
            return encoding.GetString(_buffer.Slice(p, length))
                .TrimEnd(default(char));
        }

        public byte[] ReadBytes(int length)
        {
            var p = _bytePointer;
            _bytePointer += length;
            this.ValidatePointer();
            return _buffer.Slice(p, length).ToArray();
        }

        public ReadOnlyMemory<byte> ReadMemory(int length)
        {
            return this.ReadBytes(length);
        }

        public ReadOnlySpan<byte> ReadSpan(int length)
        {
            return this.ReadBytes(length);
        }

        public void ReadTo(Span<byte> target)
        {
            var p = _bytePointer;
            var length = target.Length;
            _bytePointer += length;
            this.ValidatePointer();
            Unsafe.CopyBlock(ref target[0], in _buffer[p], (uint)length);
        }

        public void ReadTo(Span<byte> target, int length)
        {
            var p = _bytePointer;
            _bytePointer += length;
            this.ValidatePointer();
            Unsafe.CopyBlock(ref target[0], in _buffer[p], (uint)length);
        }

        public bool ReadBoolean()
        {
            var p = _bytePointer;
            var q = _bitPointer;
            _bitPointer += 1;
            _bytePointer += _bitPointer >> 3;
            _bitPointer &= 7;
            this.ValidatePointer();
            return (_buffer[p] >> q & 1) == 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private readonly void ValidatePointer()
        {
            if (_bytePointer > _length || _bytePointer == _length && _bitPointer > 0)
            {
                throw new IndexOutOfRangeException("Memory out of range");
            }
        }
    }
}
