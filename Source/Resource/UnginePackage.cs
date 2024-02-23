using System;
using System.Runtime.InteropServices;

public enum CompressionType : byte {
    None,
    LZMA,
    LZMA2,
    GZIP
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct UpkfFileHeader {
    public uint magic;
    public byte version;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct UpkfHeaderV1 {
    public UpkfFileHeader header;
    public ushort originSize;
    public char[] origin;
    public uint headerCrc;
}
