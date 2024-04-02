using ImGuiNET;
using MathLib;
using System.Numerics;
using Veldrid;
using WinterEngine.MaterialSystem;
using WinterEngine.RenderSystem;
using WinterEngine.Resource;

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

public class IQMModelResource : ModelResource, IResource
{
	public const string Magic = "INTERQUAKEMODEL";
	public const int Version = 2;

    char[] m_FullText;

    public IReadOnlyList<Vertex> Vertices => m_Vertices;
    public IReadOnlyList<ushort> Indices => m_Indices;
    List<Vertex> m_Vertices = new List<Vertex>();
    List<ushort> m_Indices = new List<ushort>();

    #region IQM Data
    public IQMHeader Header;

    List<IQMVertexArray> m_VertexArrays = new List<IQMVertexArray>();
    List<IQMPose> m_Poses = new List<IQMPose>();
    List<IQMJoint> m_Joints = new List<IQMJoint>();
    List<IQMAnim> m_Anims = new List<IQMAnim>();
    List<ushort[]> m_Frames = new List<ushort[]>();
    #endregion

    string GetString(int start)
    {
        string str = "";
        int i = start;
        while (true)
        {
            char c = m_FullText[i];

            if (c == '\0')
                break;

            str += c;
            i++;
        }
        return str;
    }

    public void DisplayData()
    {
        if (ImGui.CollapsingHeader("Vertices"))
        {
            foreach (var item in m_Vertices)
            {
                ImGui.SeparatorText(item.GetType().Name);
                foreach (var field in item.GetType().GetFields())
                {
                    ImGui.Text($"{field.Name}: {field.GetValue(item)}");
                }
                ImGui.Separator();
            }
        }

        if (ImGui.CollapsingHeader("Vertex Arrays"))
        {
            foreach (var item in m_VertexArrays)
            {
                ImGui.SeparatorText(item.GetType().Name);
                foreach (var field in item.GetType().GetFields())
                {
                    ImGui.Text($"{field.Name}: {field.GetValue(item)}");
                }
                ImGui.Separator();
            }
        }

        if (ImGui.CollapsingHeader("Anims"))
        {
            foreach (var item in m_Anims)
            {
                ImGui.SeparatorText(item.GetType().Name);
                foreach (var field in item.GetType().GetFields())
                {
                    ImGui.Text($"{field.Name}: {field.GetValue(item)}");
                }
                ImGui.Separator();
            }
        }

        if (ImGui.CollapsingHeader("Poses"))
        {
            foreach (var item in m_Poses)
            {
                ImGui.SeparatorText(item.GetType().Name);
                foreach (var field in item.GetType().GetFields())
                {
                    ImGui.Text($"{field.Name}: {field.GetValue(item)}");
                }
                ImGui.Separator();
            }
        }

        if (ImGui.CollapsingHeader("Joints"))
        {
            foreach (var item in m_Joints)
            {
                ImGui.SeparatorText(item.GetType().Name);
                foreach (var field in item.GetType().GetFields())
                {
                    ImGui.Text($"{field.Name}: {field.GetValue(item)}");
                }
                ImGui.Separator();
            }
        }

        if (ImGui.CollapsingHeader("Frames"))
        {
            foreach (var item in m_Frames)
            {
                ImGui.TextWrapped($"Channels: item.ToString()");
                ImGui.Separator();
            }
        }
    }

    public void LoadData(Stream stream)
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

        m_FullText = mdlReader.ReadChars((int)Header.num_text);
        #endregion

        #region Read Vertices and Vertex Arrays
        mdlReader.BaseStream.Position = Header.ofs_vtxarrays;

        for (int i = 0; i < Header.num_vtxarrays; i++)
        {
            IQMVertexArray vtxArray = new IQMVertexArray();

            vtxArray.Type = (IQMVtxArrType)mdlReader.ReadUInt32();
            vtxArray.Flags = mdlReader.ReadUInt32();
            vtxArray.Format = (IQMVtxArrFormat)mdlReader.ReadUInt32();
            vtxArray.Size = mdlReader.ReadUInt32();
            vtxArray.Offset = mdlReader.ReadUInt32();

            m_VertexArrays.Add(vtxArray);
        }

        for (int v = 0; v < Header.num_vtx; v++)
        {
            Vertex vertex = new Vertex();

            foreach (IQMVertexArray vertexArray in m_VertexArrays)
            {
                mdlReader.BaseStream.Position = vertexArray.Offset;
                switch (vertexArray.Type)
                {
                    case IQMVtxArrType.Position:
                        mdlReader.BaseStream.Position += (sizeof(float) * vertexArray.Size * v);
                        vertex.Position = new Vector3(
                            mdlReader.ReadSingle(),
                            mdlReader.ReadSingle(),
                            mdlReader.ReadSingle()
                        );
                        break;
                    case IQMVtxArrType.TexCoord:
                        mdlReader.BaseStream.Position += (sizeof(float) * vertexArray.Size * v);
                        vertex.UV = new Vector2(
                            mdlReader.ReadSingle(),
                            mdlReader.ReadSingle()
                        );
                        break;
                    case IQMVtxArrType.Normal:
                        mdlReader.BaseStream.Position += (sizeof(float) * vertexArray.Size * v);
                        vertex.Normal = new Vector3(
                            mdlReader.ReadSingle(),
                            mdlReader.ReadSingle(),
                            mdlReader.ReadSingle()
                        );
                        break;
                    case IQMVtxArrType.Tangent:
                        // NONE
                        break;
                    case IQMVtxArrType.BlendIndexes:
                        mdlReader.BaseStream.Position += (sizeof(byte) * vertexArray.Size * v);
                        vertex.Joint = new Vector4(
                            mdlReader.ReadByte(),
                            mdlReader.ReadByte(),
                            mdlReader.ReadByte(),
                            mdlReader.ReadByte()
                        );
                        break;
                    case IQMVtxArrType.BlendWeights:
                        mdlReader.BaseStream.Position += (sizeof(byte) * vertexArray.Size * v);
                        vertex.Weight = new Vector4(
                            mdlReader.ReadByte() / 255.0f,
                            mdlReader.ReadByte() / 255.0f,
                            mdlReader.ReadByte() / 255.0f,
                            mdlReader.ReadByte() / 255.0f
                        );
                        break;
                    case IQMVtxArrType.Color:
                        mdlReader.BaseStream.Position += (sizeof(byte) * vertexArray.Size * v);
                        vertex.Color = new RgbaFloat(
                            mdlReader.ReadByte() / 255.0f,
                            mdlReader.ReadByte() / 255.0f,
                            mdlReader.ReadByte() / 255.0f,
                            mdlReader.ReadByte() / 255.0f
                        );
                        break;
                    case IQMVtxArrType.Custom:
                        break;
                }
            }

            m_Vertices.Add(vertex);
        }
        #endregion

        #region Read Triangles
        mdlReader.BaseStream.Position = Header.ofs_triangles;

        for (var i = 0; i < Header.num_triangles; i++)
        {
            // DANGER: IQM uses u32 for vertex indices!
            m_Indices.Add((ushort)mdlReader.ReadUInt32());
            m_Indices.Add((ushort)mdlReader.ReadUInt32());
            m_Indices.Add((ushort)mdlReader.ReadUInt32());
        }
        #endregion

        #region Read Joints
        mdlReader.BaseStream.Position = Header.ofs_joints;

        for (int i = 0; i < Header.num_joints; i++)
        {
            IQMJoint joint = new IQMJoint();

            joint.Name = GetString((int)mdlReader.ReadUInt32());
            joint.Parent = mdlReader.ReadInt32();
            joint.Translate = new Vector3(
                mdlReader.ReadSingle(),
                mdlReader.ReadSingle(),
                mdlReader.ReadSingle()
            );
            joint.Rotate = new Quaternion(
                mdlReader.ReadSingle(),
                mdlReader.ReadSingle(),
                mdlReader.ReadSingle(),
                mdlReader.ReadSingle()
            );
            joint.Scale = new Vector3(
                mdlReader.ReadSingle(),
                mdlReader.ReadSingle(),
                mdlReader.ReadSingle()
            );

            m_Joints.Add(joint);
        }
        #endregion

        #region Read Animations
        mdlReader.BaseStream.Position = Header.ofs_anims;

        for (int i = 0; i < Header.num_anims; i++)
        {
            IQMAnim anim = new IQMAnim();

            anim.Name = GetString((int)mdlReader.ReadUInt32());
            anim.FirstFrame = mdlReader.ReadUInt32();
            anim.NumFrames = mdlReader.ReadUInt32();
            anim.Framerate = mdlReader.ReadSingle();
            anim.Flags = mdlReader.ReadUInt32();

            m_Anims.Add(anim);
        }
        #endregion

        #region Read Frames
        mdlReader.BaseStream.Position = Header.ofs_frames;

        for (int i = 0; i < Header.num_frames; i++)
        {
            ushort[] Channels = new ushort[Header.num_framechannels];
            for (int chn = 0; chn < Header.num_framechannels; chn++)
            {
                Channels[chn] = mdlReader.ReadUInt16();
            }
            m_Frames.Add(Channels);
        }
        #endregion

        #region Read Poses
        mdlReader.BaseStream.Position = Header.ofs_poses;

        for (int i = 0; i < Header.num_poses; i++)
        {
            IQMPose pose = new IQMPose();

            pose.Parent = mdlReader.ReadInt32();
            pose.ChannelMask = mdlReader.ReadUInt32();
            for (int off = 0; off < pose.ChannelOffset.Length; off++)
            {
                pose.ChannelOffset[off] = mdlReader.ReadSingle();
            }
            for (int off = 0; off < pose.ChannelScale.Length; off++)
            {
                pose.ChannelScale[off] = mdlReader.ReadSingle();
            }

            m_Poses.Add(pose);
        }
        #endregion

        #region Read Meshes
        mdlReader.BaseStream.Position = Header.ofs_meshes;

        for (int i = 0; i < Header.num_meshes; i++)
        {
            IQMMesh mesh = new IQMMesh();
            mesh.Name = GetString((int)mdlReader.ReadUInt32());
            mesh.Material = GetString((int)mdlReader.ReadUInt32());

            mesh.first_vertex = mdlReader.ReadUInt32();
            mesh.num_vertexes = mdlReader.ReadUInt32();

            mesh.first_triangle = mdlReader.ReadUInt32();
            mesh.num_triangles = mdlReader.ReadUInt32();

            MeshPrimitive meshPrimitive = new MeshPrimitive();
            meshPrimitive.Name = mesh.Name;
            meshPrimitive.Material = MaterialSystem.MaterialSystem.Load(mesh.Material);
            meshPrimitive.Handle = new MeshHandle(
                m_Vertices.GetRange((int)mesh.first_vertex, (int)mesh.num_vertexes).ToArray(),
                m_Indices.GetRange((int)mesh.first_triangle*3, (int)mesh.num_triangles*3).ToArray()
            );

            m_Primitives.Add(meshPrimitive);
        }
        #endregion

        stream.Close();
    }
    
    struct PoseTrans
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;
    }

    public int GetAnimLength(string animName)
    {
        foreach (IQMAnim anim in m_Anims)
        {
            if (anim.Name == animName)
            {
                return (int)anim.NumFrames;
            }
        }
        throw new ArgumentException($"No animation with name {animName} found!");
    }

    public float GetAnimFramerate(string animName)
    {
        foreach (IQMAnim anim in m_Anims)
        {
            if (anim.Name == animName)
            {
                return anim.Framerate;
            }
        }
        throw new ArgumentException($"No animation with name {animName} found!");
    }

    public List<Matrix4x4> GetAnimFrameMatrix(string animName, int frame)
    {
        int animOff = -1;
        foreach (IQMAnim anim in m_Anims)
        {
            if (anim.Name == animName)
            {
                animOff = (int)anim.FirstFrame;
                break;
            }
        }
        if (frame > GetAnimLength(animName))
            throw new ArgumentException($"Frame number is outside animation length! (length: {GetAnimLength(animName)})");
        if (animOff == -1)
            throw new ArgumentException($"No animation with name {animName} found!");

        // go through every joint and add it's matrix to the list for that frame
        List<Matrix4x4> jointMats = new List<Matrix4x4>();

        for (int i = 0; i < m_Joints.Count; i++)
        {
            PoseTrans transform = GetJointPoseTrans(i, animOff + frame);

            Matrix4x4 rotation = Matrix4x4.CreateFromQuaternion(transform.Rotation);
            Matrix4x4 scale = Matrix4x4.CreateScale(transform.Scale);
            Matrix4x4 jointMat = rotation * scale;
            jointMat.Translation = transform.Position;

            jointMats.Add(jointMat);
        }

        return jointMats;
    }

    PoseTrans GetJointPoseTrans(int joint, int frame)
    {
        IQMPose pose = m_Poses[joint];

        PoseTrans transform = new PoseTrans()
        {
            Position = new Vector3(pose.ChannelOffset[0], pose.ChannelOffset[1], pose.ChannelOffset[2]),
            Rotation = new Quaternion(pose.ChannelOffset[3], pose.ChannelOffset[4], pose.ChannelOffset[5], pose.ChannelOffset[6]),
            Scale = new Vector3(pose.ChannelOffset[7], pose.ChannelOffset[8], pose.ChannelOffset[9])
        };

        if ((pose.ChannelMask & 0x01) == 1)
            transform.Position.X += m_Frames[frame][0] * pose.ChannelScale[0];
        if ((pose.ChannelMask & 0x02) == 1)
            transform.Position.Y += m_Frames[frame][1] * pose.ChannelScale[1];
        if ((pose.ChannelMask & 0x04) == 1)
            transform.Position.Z += m_Frames[frame][2] * pose.ChannelScale[2];

        if ((pose.ChannelMask & 0x08) == 1)
            transform.Rotation.X += m_Frames[frame][3] * pose.ChannelScale[3];
        if ((pose.ChannelMask & 0x10) == 1)
            transform.Rotation.Y += m_Frames[frame][4] * pose.ChannelScale[4];
        if ((pose.ChannelMask & 0x20) == 1)
            transform.Rotation.Z += m_Frames[frame][5] * pose.ChannelScale[5];
        if ((pose.ChannelMask & 0x40) == 1)
            transform.Rotation.W += m_Frames[frame][6] * pose.ChannelScale[6];

        if ((pose.ChannelMask & 0x80) == 1)
            transform.Scale.X += m_Frames[frame][7] * pose.ChannelScale[7];
        if ((pose.ChannelMask & 0x100) == 1)
            transform.Scale.Y += m_Frames[frame][8] * pose.ChannelScale[8];
        if ((pose.ChannelMask & 0x200) == 1)
            transform.Scale.Z += m_Frames[frame][9] * pose.ChannelScale[9];

        // apply parent transformation
        if (pose.Parent > -1)
        {
            PoseTrans parentTransform = GetJointPoseTrans(pose.Parent, frame);
            transform.Position += parentTransform.Position;
            transform.Rotation += parentTransform.Rotation;
            transform.Scale += parentTransform.Scale;
        }

        return transform;
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

// todo: will we even end up using this?
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
