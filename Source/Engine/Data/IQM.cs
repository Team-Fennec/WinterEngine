using System.Numerics;
using System.Runtime.InteropServices;
using System.Xml;

namespace WinterEngine.Data;

// IQM format

public enum IQMVtxArrType : uint
{
    Position,       // float, 3
    TexCoord,       // float, 2
    Normal,         // float, 3
    Tangent,        // float, 4
    BlendIndexes,   // ubyte, 4
    BlendWeights,   // ubyte, 4
    Color,          // ubyte, 4
    Custom = 0x10
}

public enum IQMVtxArrFormat : uint
{
    Byte,
    UByte,
    Short,
    UShort,
    Int,
    UInt,
    Half,
    Float,
    Double
}

public enum IQMAnimFlags : uint
{
    Loop = 1<<0
}

public class IQMModel
{
	public const string Magic = "INTERQUAKEMODEL";
	public const int Version = 2;
	
	public IQMHeader Header;

    public List<string> Text = new List<string>(); // first string always will be the empty string
    public List<string> Comment = new List<string>();
    public List<ushort> Frames = new List<ushort>(); // one big unsigned short array where each group of framechannels components is one frame.

    List<IQMMesh> m_Meshes = new List<IQMMesh>();
    List<IQMTriangle> m_Triangles = new List<IQMTriangle>();
    List<IQMVertexArray> m_VertexArrays = new List<IQMVertexArray>();
    List<IQMPose> m_Poses = new List<IQMPose>();
    List<IQMJoint> m_Joints = new List<IQMJoint>();
    List<IQMBounds> m_Bounds = new List<IQMBounds>();
    List<IQMAnim> m_Anims = new List<IQMAnim>();

    public IQMModel(Stream stream)
    {
        BinaryReader mdlReader = new BinaryReader(stream);
        mdlReader.BaseStream.Position = 0;
        // read the header
        Header = new IQMHeader();
        Header.ident = mdlReader.ReadChars(16);
        Header.version = mdlReader.ReadInt32();

        if (CConvert.CharToString(Header.ident) != Magic)
        {
            throw new InvalidDataException("Invalid IDENT/MAGIC, Not IQM!");
        }

        if (Header.version > Version)
        {
            throw new InvalidDataException("Invalid version! Higher than 2 is not supported.");
        }

        #region Read Header
        Header.fileSize = mdlReader.ReadUInt32();
        Header.flags = mdlReader.ReadUInt32();
        Header.num_text = mdlReader.ReadUInt32();
        Header.ofs_text = mdlReader.ReadUInt32();
        Header.num_meshes = mdlReader.ReadUInt32();
        Header.ofs_meshes = mdlReader.ReadUInt32();
        Header.num_vtxarrays = mdlReader.ReadUInt32();
        Header.num_vtx = mdlReader.ReadUInt32();
        Header.ofs_vtxarrays = mdlReader.ReadUInt32();
        Header.num_triangles = mdlReader.ReadUInt32();
        Header.ofs_triangles = mdlReader.ReadUInt32();
        Header.ofs_adjacency = mdlReader.ReadUInt32();
        Header.num_joints = mdlReader.ReadUInt32();
        Header.ofs_joints = mdlReader.ReadUInt32();
        Header.num_poses = mdlReader.ReadUInt32();
        Header.ofs_poses = mdlReader.ReadUInt32();
        Header.num_anims = mdlReader.ReadUInt32();
        Header.ofs_anims = mdlReader.ReadUInt32();
        Header.num_frames = mdlReader.ReadUInt32();
        Header.num_framechannels = mdlReader.ReadUInt32();
        Header.ofs_frames = mdlReader.ReadUInt32();
        Header.ofs_bounds = mdlReader.ReadUInt32();
        Header.num_comment = mdlReader.ReadUInt32();
        Header.ofs_comment = mdlReader.ReadUInt32();
        Header.num_extensions = mdlReader.ReadUInt32();
        Header.ofs_extensions = mdlReader.ReadUInt32();
        #endregion

        #region Read Text Strings
        mdlReader.BaseStream.Position = Header.ofs_text;

        int stringCount = 0;
        string currentStr = "";
        while (stringCount < Header.num_text)
        {
            char c = mdlReader.ReadChar();
            
            if (c == '\0')
            {
                Text.Add(currentStr);
                currentStr = "";
            }
            else
            {
                currentStr += c;
            }

            stringCount++;
        }
        #endregion

        #region Read Meshes
        mdlReader.BaseStream.Position = Header.ofs_meshes;

        for (int i = 0; i < Header.num_meshes; i++)
        {
            IQMMesh mesh = new IQMMesh();
            mesh.Name = Text[(int)mdlReader.ReadUInt32()];
            mesh.Material = Text[(int)mdlReader.ReadUInt32()];

            mesh.first_vertex = mdlReader.ReadUInt32();
            mesh.num_vertexes = mdlReader.ReadUInt32();

            mesh.first_triangle = mdlReader.ReadUInt32();
            mesh.num_triangles = mdlReader.ReadUInt32();

            m_Meshes.Add(mesh);
        }
        #endregion

        #region Read Verticies and Vertex Arrays
        mdlReader.BaseStream.Position = Header.ofs_vtxarrays;

        for (int i = 0; i < Header.num_vtxarrays; i++)
        {

        }
        #endregion
    }
}

public struct IQMHeader
{
    public char[] ident;
    public int version;
    public uint fileSize;
    public uint flags;
    public uint num_text, ofs_text;
    public uint num_meshes, ofs_meshes;
    public uint num_vtxarrays, num_vtx, ofs_vtxarrays;
    public uint num_triangles, ofs_triangles, ofs_adjacency;
    public uint num_joints, ofs_joints;
    public uint num_poses, ofs_poses;
    public uint num_anims, ofs_anims;
    public uint num_frames, num_framechannels, ofs_frames, ofs_bounds;
    public uint num_comment, ofs_comment;
    public uint num_extensions, ofs_extensions; // this is a linked list apparently
}

public struct IQMMesh
{
    public string Name;
    public string Material;
    public uint first_vertex, num_vertexes;
    public uint first_triangle, num_triangles;
}

public struct IQMVertexArray
{
    public IQMVtxArrType Type;
    public uint Flags;
    public IQMVtxArrFormat Format;
    public uint Size;
    public uint Offset; // tightly packed components array
}

public struct IQMTriangle
{
    public uint[] vertex;

    public IQMTriangle()
    {
        vertex = new uint[3];
    }
}

public struct IQMAdjacency
{
    // each value is the index of the adjacent triangle for edge 0, 1, and 2, where ~0 (= -1) indicates no adjacent triangle
    // indexes are relative to the iqmheader.ofs_triangles array and span all meshes, where 0 is the first triangle, 1 is the second, 2 is the third, etc. 
    public uint[] triangle;

    public IQMAdjacency()
    {
        triangle = new uint[3];
    }
}

public struct IQMJoint
{
    public string Name;
    public int Parent; // < 0 means root bone
    public Vector3 Translate;
    public Quaternion Rotate;
    public Vector3 Scale;

    // rotation is in relative/parent local space
    // scale is pre-scaling
    // output = (input*scale)*rotation + translation
}

public struct IQMPose
{
    public int Parent; // < 0 means root bone
    public uint ChannelMask; // 10 channels are present for this joint pose
    public float[] ChannelOffset, ChannelScale;
    // channels 0..2 are translation and channels 3..6 are rotation
    
    // rotation is in relative/parent local space
    // channels 7..9 are scale
    // output = (input*scale)*rotation + translation

    public IQMPose()
    {
        ChannelOffset = new float[10];
        ChannelScale = new float[10];
    }
}

public struct IQMAnim
{
    public string Name;
    public uint FirstFrame, NumFrames;
    public float Framerate;
    public uint Flags;
}

public struct IQMBounds
{
    public Vector3 BBMins, BBMaxs;
    public float XYRadius, Radius; // circular radius in X-Y as well as spherical
}

public struct IQMExtension
{
    public string Name;
    public uint NumData, OfsData;
    public uint OfsExtensions;
}

public struct IQMVertex
{
    public Vector3 Position;
    public Vector2 TexCoord;
    public Vector3 Normal;
    public Vector4 Tangent;
    public byte[] BlendIndices, BlendWeights, Color;

    public IQMVertex()
    {
        BlendIndices = new byte[4];
        BlendWeights = new byte[4];
        Color = new byte[4];
    }
}
