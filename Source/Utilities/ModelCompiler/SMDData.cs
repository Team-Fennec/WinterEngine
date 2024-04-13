using System;
using System.IO;

namespace ModelCompiler;

public struct SmdBone
{

}

public struct SmdFrame
{
	public int BoneID;
	public Vector3 Position;
	public Vector3 Rotation;
}

public struct SmdTriangle
{
	public string Material;
	public int ParentBone;
	public Vector3 Position;
	public Vector3 Normal;
	public Vector2 UV;
	// todo: support weightmap links
}

public struct SmdMesh
{

}

public class SMDFile
{
	public List<SmdTriangle> Triangles;
	public List<SmdFrame> Frames;

	public SMDFile(string filename)
	{

	}
}
