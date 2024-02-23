using System;
using System.Numerics;
using System.IO;

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

public struct Md3File {
    public int ident; // note: magic number "IDP3"
    public int version; // note: latest known is 15
    public char[] name; // note: size is MAX_QPATH, which is 64
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

    public Md3Frame[] frames;
    public Md3Tag[] tags;
    public Md3Surface[] surfaces;
}

public struct Md3Frame {
    public Vector3 minBounds; // first corner of the bounding box
    public Vector3 maxBounds; // second corner of the bounding box
    public Vector3 localOrigin; // usually (0,0,0)
    public float radius; // radius of bounding sphere
    public char[] name; // U8 * 16
}

public struct Md3Tag {
    public char[] name; // name of tag object, MAX_QPATH (64) size
    public Vector3 origin; // coords of tag object
    public Vector3[] axis; // size of 3, orientation of tag object (???)
}

public struct Md3Surface {
    // Surface Start : offset relative to the md3 object
    public int ident; // magic number. "IDP3"
    public char[] name; // MAX_QPATH , name of surface object
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

    public Md3Shader[] shaders;
    public Md3Triangle[] triangles;
    public Md3TexCoord[] texCoords;
    public Md3Vertex[] vertices;
}

// we will ignore these for now to try and get a model displaying period
public struct Md3Shader {
    public char[] name; // MAX_QPATH, pathname in files/pk3
    public int shaderIndex; // no idea how this is allocated
}

public struct Md3Triangle {
    public int[] indexes; // 3, list of offset values into the list of vertex objects
}

public struct Md3TexCoord {
    public float[] st; // UV
}

public struct Md3Vertex {
    public short x;
    public short y;
    public short z;
    public short normal;
}
