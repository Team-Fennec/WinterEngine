/*
    WinterEngine
    
    SnowPack file definitions
    Based on Ungine PaK Format
    
    WinterEngine (C) 2023-2024 Team Fennec
    (C) 2024 K. 'ashi/eden' J.
*/
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace WinterEngine.Content;

/// <summary>
/// Packed and Compressed game data
/// </summary>
public class SnowPack {
    /// <summary>The 4 bytes used to identify a pack file</summary>
    public const int MagicIdent = 4935763;
    /// <summary>The version of the format</summary>
    public const int Version = 2;
    
    /// <summary>Name of the pack (filename)</summary>
    public string Name;
    /// <summary>The path to the pack file</summary>
    public string FilePath;
    /// <summary>The read header of the pack file</summary>
    public SnowPackHeaderV1 Header;
    /// <summary>The root ExtDecl</summary>
    public SnowPackExtDecl RootExt;
    
#region Construction
    public SnowPack(string filePath) {
        FilePath = filePath;
        if (!File.Exists(FilePath)) {
            throw new FileNotFoundException($"Unable to locate pack file at {FilePath}");
        }
        
        Name = Path.GetFileNameWithoutExtension(FilePath);
        
        // begin reading out with binary reader
        using (var stream = File.Open(FilePath, FileMode.Open))
        {
            using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
            {
                Header = new SnowPackHeaderV1();
                Header.header = new SnowPackFileHeader();
                
                Header.header.magic = reader.ReadUInt32();
                Header.header.version = reader.ReadByte();
                
                if (Header.header.magic != MagicIdent) {
                    throw new Exception("Invalid ident, Not a SnowPack file.");
                }
                if (Header.header.version != Version) {
                    throw new Exception("Invalid version, v1 and v3+ aren't supported!");
                }
                
                Header.originSize = reader.ReadUInt16();
                Header.origin = new string(reader.ReadChars(Header.originSize));
                // todo: use this crc for checking
                Header.headerCrc = reader.ReadUInt32();
                
                // read the root ext
                RootExt = new SnowPackExtDecl();
                RootExt.extensionSize = reader.ReadByte();
                RootExt.extension = new string(reader.ReadChars(RootExt.extensionSize));
                RootExt.directoryCount = reader.ReadUInt16();
                
                RootExt.directories = new SnowPackDirDecl[RootExt.directoryCount];
                for (int i = 0; i < RootExt.directoryCount; i++) {
                    RootExt.directories[i] = new SnowPackDirDecl();
        
                    RootExt.directories[i].pathSize = reader.ReadUInt16();
                    RootExt.directories[i].path = new string(reader.ReadChars(RootExt.directories[i].pathSize));
                    RootExt.directories[i].entryCount = reader.ReadUInt16();
                    
                    RootExt.directories[i].entries = new SnowPackEntryDecl[RootExt.directories[i].entryCount];
                    for (int e = 0; e < RootExt.directories[i].entries.Length; e++) {
                        RootExt.directories[i].entries[e] = new SnowPackEntryDecl();
                    
                        RootExt.directories[i].entries[e].nameSize = reader.ReadByte();
                        RootExt.directories[i].entries[e].name = new string(reader.ReadChars(RootExt.directories[i].entries[e].nameSize));
                        RootExt.directories[i].entries[e].compressionType = (CompressionType)reader.ReadByte();
                        RootExt.directories[i].entries[e].size = reader.ReadUInt32();
                        RootExt.directories[i].entries[e].crc = reader.ReadUInt32();
                        RootExt.directories[i].entries[e].sha = reader.ReadUInt32();
                        
                        RootExt.directories[i].entries[e].data = reader.ReadBytes((int)RootExt.directories[i].entries[e].size);
                        // todo: does this factor in extensions correctly?
                        availableFiles.Add($"{RootExt.directories[i].path}/{RootExt.directories[i].entries[e].name}");
                    }
                }
            }
        }
    }
#endregion
    
#region File Listing
    private List<string> availableFiles = new List<string>();
    
    public bool FileExists(string path) {
        return availableFiles.Contains(path);
    }
#endregion
    
#region Reading Files
    public StreamReader OpenEntry(string path) {
        if (!FileExists(path)) {
            throw new FileNotFoundException($"{path} does not exist within package {Name} ({path})");
        }
        
        string endingPath = $"/{(Path.GetFileName(path))}";
        string rawPath = path.TrimEnd(endingPath.ToCharArray(0, endingPath.Length));
        foreach (SnowPackDirDecl dirDecl in RootExt.directories) {
            if (rawPath == dirDecl.path) {
                foreach (SnowPackEntryDecl entryDecl in dirDecl.entries) {
                    if (entryDecl.name == Path.GetFileName(path)) {
                        // todo(pack): load and decompress the data please uwu
                    }
                }
            }
        }
        
        throw new Exception("Literally how the fuck did you get here");
    }
#endregion
}

/// <summary>The compression type of an entry</summary>
public enum CompressionType : byte {
    None,
    LZMA,
    LZMA2,
    GZIP
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SnowPackFileHeader {
    public uint magic;
    public byte version;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SnowPackHeaderV1 {
    public SnowPackFileHeader header;
    public ushort originSize;
    public string origin;
    public uint headerCrc;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SnowPackExtDecl {
    public byte extensionSize;
    public string extension;
    public ushort directoryCount;
    public SnowPackDirDecl[] directories;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SnowPackDirDecl {
    public ushort pathSize;
    public string path;
    public ushort entryCount;
    public SnowPackEntryDecl[] entries;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SnowPackEntryDecl {
    public byte nameSize;
    public string name;
    public CompressionType compressionType;
    public uint size;
    public uint crc;
    public uint sha;
    public byte[] data;
}