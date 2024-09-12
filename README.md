# ProtocolStream

## Introduction

ProtocolStream is a simple yet powerful stream-based protocol encoding and decoding library designed to streamline the serialization and deserialization of binary data. The library provides an intuitive API for effortless handling of various fundamental data types and custom structures within a stream, supporting both Big Endian mode and bitwise operations.

## Quick Start

```csharp
var buffer=new byte[1024];
var writer=new SpanWriter(buffer);

writer.Write(new int(1));
writer.Write(new byte(1));
writer.write(new ushort(1));
writer.Write("12345");
...
var reader=new SpanReader(buffer);

Console.WriteLine(reader.ReadInt());
Console.WriteLine(reader.ReadByte());
Console.WriteLine(reader.ReadUShort());
Console.WriteLine(reader.ReadString(5));
...

```

## Advanced Usage

### Support for Big Endian Mode

```csharp
var buffer=new byte[1024];
var writer=new SpanWriter(buffer);

writer.WriteWithBigEndian(new int(1),);
writer.Write(new byte(1));
writer.WriteWithBigEndian(new ushort(1));
writer.Write("12345");
...
var reader=new SpanReader(buffer);

Console.WriteLine(reader.ReadIntWithBigEndian());
Console.WriteLine(reader.ReadByte());
Console.WriteLine(reader.ReadUShortWithBigEndian());
Console.WriteLine(reader.ReadString(5));
...

```

### Bitwise Operations Mode

```csharp
var buffer=new byte[1024];
var writer=new SpanWriter(buffer);

writer.Seek(0,1); // Position to the 1st bit of the 0th byte
writer.WriteAligned(new int(1));
writer.WriteAligned(new byte(1));
writer.WriteAligned(new ushort(1));
writer.Seek(8,0); // Position to the 0th bit of the 8th byte
writer.Write("12345");

...
var reader=new SpanReader(buffer);

reader.Seek(0,1);
Console.WriteLine(reader.ReadInt());
Console.WriteLine(reader.ReadByte());
Console.WriteLine(reader.ReadUShort());
reader.Seek(8,0);
Console.WriteLine(reader.ReadString(5));
...

```

`Reader Operations`

Similarly, the reader supports the Seek(bytePosition, bitPosition) method for bitwise-level positioning and uses regular read methods (omitted here due to truncation) to retrieve data from the specified positions.