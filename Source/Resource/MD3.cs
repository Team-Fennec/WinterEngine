using System;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;

namespace WinterEngine.Resource;

// note: for vec3, read as triplet of f32 in sequence (read 4 octets, make float, ...)
/*
    The following constants have these values for this code:
    MAX_QPATH: 64
    MD3_MAX_SHADERS: 256
    MD3_MAX_VERTS: 4096
    MD3_MAX_TRIANGLES: 8192
    MD3_MAX_FRAMES: 1024
    MD3_MAX_TAGS: 16
    MD3_MAX_SURFACES: 32
    MD3_VERSION: 15
*/

// fuck my ass dude why are null terminated Cstrings so fucking painful in C#
public static class CConvert {
    public static string CharToString(char[] arr) {
        string s = new string(arr);
        return s.Substring(0, Math.Max(0, s.IndexOf('\0')));
    }
}

public class Md3Model {
    const int MagicIdent = 860898377; // IDP3
    const int Version = 15; // should not exceed this value
    const int MaxQPath = 64;

    public Md3FileHeader Header;
    public List<Md3Frame> Frames;
    public List<Md3Tag> Tags;
    public List<Md3Surface> Surfaces;

    public Md3Model(string fileName) {
        Frames = new List<Md3Frame>();
        Tags = new List<Md3Tag>();
        Surfaces = new List<Md3Surface>();
        StreamReader fStream = ResourceManager.OpenResource(Path.Combine("models", $"{fileName}.md3"));
        // we need to binary this one
        MemoryStream mdlMem = new MemoryStream();
        fStream.BaseStream.CopyTo(mdlMem);
        fStream.Close();

        using (BinaryReader mdlReader = new BinaryReader(mdlMem)) {
            mdlReader.BaseStream.Position = 0;
            // read the header
            Header = new Md3FileHeader();
            Header.ident = mdlReader.ReadInt32();
            Header.version = mdlReader.ReadInt32();
            
            if (Header.ident != MagicIdent) {
                throw new Exception("Invalid file ident, not an MD3 file.");
            }

            if (Header.version > Version) {
                throw new Exception($"Invalid format version, max supported is 15 got {Header.version}.");
            }

            // read the rest of the header now
            Header.name = CConvert.CharToString(mdlReader.ReadChars(MaxQPath));
            Header.flags = mdlReader.ReadInt32(); // apparently this is just unused lol

            Header.numFrames = mdlReader.ReadInt32();
            Header.numTags = mdlReader.ReadInt32();
            Header.numSurfaces = mdlReader.ReadInt32();
            Header.numSkins = mdlReader.ReadInt32();
            
            Header.ofsFrames = mdlReader.ReadInt32();
            Header.ofsTags = mdlReader.ReadInt32();
            Header.ofsSurfaces = mdlReader.ReadInt32();
            Header.ofsEOF = mdlReader.ReadInt32();

            // now that we have a header it's time to read the other components of the file
            mdlReader.BaseStream.Position = Header.ofsFrames;

            int frameCount = 0;
            while (frameCount < Header.numFrames) {
                Md3Frame frame = new Md3Frame();

                frame.minBounds = new Vector3(
                    mdlReader.ReadSingle(),
                    mdlReader.ReadSingle(),
                    mdlReader.ReadSingle()
                );
                frame.maxBounds = new Vector3(
                    mdlReader.ReadSingle(),
                    mdlReader.ReadSingle(),
                    mdlReader.ReadSingle()
                );
                frame.localOrigin = new Vector3(
                    mdlReader.ReadSingle(),
                    mdlReader.ReadSingle(),
                    mdlReader.ReadSingle()
                );
                frame.radius = mdlReader.ReadSingle();
                frame.name = CConvert.CharToString(mdlReader.ReadChars(16));

                Frames.Add(frame);
                frameCount++;
            }

            mdlReader.BaseStream.Position = Header.ofsTags;

            int tagCount = 0;
            while (tagCount < Header.numTags) {
                Md3Tag tag = new Md3Tag();
                
                tag.name = CConvert.CharToString(mdlReader.ReadChars(MaxQPath));
                tag.origin = new Vector3(
                    mdlReader.ReadSingle(),
                    mdlReader.ReadSingle(),
                    mdlReader.ReadSingle()
                );
                tag.axis[0] = new Vector3(
                    mdlReader.ReadSingle(),
                    mdlReader.ReadSingle(),
                    mdlReader.ReadSingle()
                );
                tag.axis[1] = new Vector3(
                    mdlReader.ReadSingle(),
                    mdlReader.ReadSingle(),
                    mdlReader.ReadSingle()
                );
                tag.axis[2] = new Vector3(
                    mdlReader.ReadSingle(),
                    mdlReader.ReadSingle(),
                    mdlReader.ReadSingle()
                );

                Tags.Add(tag);
                tagCount++;
            }

            mdlReader.BaseStream.Position = Header.ofsSurfaces;

            int surfCount = 0;
            while (surfCount < Header.numSurfaces) {
                Md3Surface surf = new Md3Surface();
                
                surf.ident = mdlReader.ReadInt32();
                if (surf.ident != MagicIdent) {
                    throw new Exception("Found non MD3 surface data: Incorrect Ident");
                }

                surf.name = CConvert.CharToString(mdlReader.ReadChars(MaxQPath));
                surf.flags = mdlReader.ReadInt32(); // unused???

                surf.numFrames = mdlReader.ReadInt32();
                surf.numShaders = mdlReader.ReadInt32();
                surf.numVerts = mdlReader.ReadInt32();
                surf.numTriangles = mdlReader.ReadInt32();

                surf.ofsTriangles = mdlReader.ReadInt32();
                surf.ofsShaders = mdlReader.ReadInt32();
                surf.ofsST = mdlReader.ReadInt32();
                surf.ofsXYZNormal = mdlReader.ReadInt32();
                surf.ofsEnd = mdlReader.ReadInt32();

                // read even further down the rabbit hole
                mdlReader.BaseStream.Position = Header.ofsSurfaces + surf.ofsTriangles;
                {
                    int count = 0;
                    while (count < surf.numTriangles) {
                        Md3Triangle trig = new Md3Triangle();
                        trig.indexes[0] = mdlReader.ReadInt32();
                        trig.indexes[1] = mdlReader.ReadInt32();
                        trig.indexes[2] = mdlReader.ReadInt32();
                        surf.triangles.Add(trig);
                        count++;
                    }
                }

                mdlReader.BaseStream.Position = Header.ofsSurfaces + surf.ofsShaders;
                {
                    int count = 0;
                    while (count < surf.numShaders) {
                        Md3Shader shd = new Md3Shader();

                        shd.name = CConvert.CharToString(mdlReader.ReadChars(MaxQPath));
                        shd.shaderIndex = mdlReader.ReadInt32();

                        surf.shaders.Add(shd);
                        count++;
                    }
                }

                mdlReader.BaseStream.Position = Header.ofsSurfaces + surf.ofsST;
                {
                    int count = 0;
                    while (count < surf.numVerts) {
                        surf.texCoords.Add(new Vector2(
                            mdlReader.ReadSingle(),
                            mdlReader.ReadSingle()
                        ));
                        count++;
                    }
                }

                mdlReader.BaseStream.Position = Header.ofsSurfaces + surf.ofsXYZNormal;
                {
                    int count = 0;
                    while (count < (surf.numVerts * surf.numFrames)) {
                        Md3Vertex vtx = new Md3Vertex();

                        vtx.x = mdlReader.ReadInt16();
                        vtx.y = mdlReader.ReadInt16();
                        vtx.z = mdlReader.ReadInt16();
                        vtx.normal = mdlReader.ReadInt16();

                        surf.vertices.Add(vtx);
                        count++;
                    }
                }

                Surfaces.Add(surf);
                surfCount++;
            }
        }
    }
}

public struct Md3FileHeader {
    public int ident; // note: magic number "IDP3"
    public int version; // note: latest known is 15
    public string name; // note: size is MAX_QPATH, which is 64
    public int flags; // note: ?????
    
    public int numFrames;
    public int numTags;
    public int numSurfaces;
    public int numSkins;

    public int ofsFrames;
    public int ofsTags;
    public int ofsSurfaces;
    public int ofsEOF;
    // note: no offset for skin objects
}

public struct Md3Frame {
    public Vector3 minBounds; // first corner of the bounding box
    public Vector3 maxBounds; // second corner of the bounding box
    public Vector3 localOrigin; // usually (0,0,0)
    public float radius; // radius of bounding sphere
    public string name; // U8 * 16
}

public struct Md3Tag {
    public string name; // name of tag object, MAX_QPATH (64) size
    public Vector3 origin; // coords of tag object
    public Vector3[] axis; // size of 3, orientation of tag object (???)

    public Md3Tag() {
        axis = new Vector3[3];
    }
}

public struct Md3Surface {
    // Surface Start : offset relative to the md3 object
    public int ident; // magic number. "IDP3"
    public string name; // MAX_QPATH , name of surface object
    public int flags; // flags?

    public int numFrames;
    public int numShaders;
    public int numVerts;
    public int numTriangles;

    public int ofsTriangles;
    public int ofsShaders;
    public int ofsST;
    public int ofsXYZNormal;
    public int ofsEnd;

    public List<Md3Shader> shaders;
    public List<Md3Triangle> triangles;
    public List<Vector2> texCoords;
    public List<Md3Vertex> vertices;

    public Md3Surface() {
        shaders = new List<Md3Shader>();
        triangles = new List<Md3Triangle>();
        texCoords = new List<Vector2>();
        vertices = new List<Md3Vertex>();
    }
}

// we will ignore these for now to try and get a model displaying period
public struct Md3Shader {
    public string name; // MAX_QPATH, pathname in files/pk3
    public int shaderIndex; // no idea how this is allocated
}

public struct Md3Triangle {
    public int[] indexes; // 3, list of offset values into the list of vertex objects
    
    public Md3Triangle() {
        indexes = new int[3];
    }
}

public struct Md3TexCoord {
    public Vector2 UV; // UV
}

public struct Md3Vertex {
    public short x;
    public short y;
    public short z;
    public short normal;
}
