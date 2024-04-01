using MathLib;
using SharpGLTF.Schema2;
using SharpGLTF.Transforms;
using System.Numerics;
using System.Reflection;
using System.Security.Policy;
using Veldrid;
using WinterEngine.MaterialSystem;
using WinterEngine.RenderSystem;
using WinterEngine.Resource;
using gMaterialSystem = WinterEngine.MaterialSystem.MaterialSystem;

namespace WinterEngine.Data;

public abstract class ModelResource
{
    public struct MeshPrimitive
    {
        public string Name;
        public MeshHandle Handle;
        public MaterialResource Material;
        // used by the render system to hold the shader parameter resources
        public ResourceSet ShaderResSet;
    }

    public IReadOnlyList<MeshPrimitive> Primitives => m_Primitives;
    protected List<MeshPrimitive> m_Primitives = new List<MeshPrimitive>();
}

public class GLBModelResource : ModelResource, IResource
{
    ModelRoot m_ModelRoot;

    public void LoadData(Stream stream)
    {
        // el em fucking ay oh
        m_ModelRoot = ModelRoot.ReadGLB(stream);
        stream.Close();

        foreach (Mesh meshInf in m_ModelRoot.LogicalMeshes)
        {
            foreach (var primitive in meshInf.Primitives)
            {
                MeshPrimitive meshPrimitive = new MeshPrimitive();

                // it says not to, we do it anyways because how else do we load materials
                // we have a special format for materials and disregard any other info.
                meshPrimitive.Material = gMaterialSystem.Load(primitive.Material.Name);

                List<Vertex> Vertices = new List<Vertex>();
                List<ushort> Indices = new List<ushort>();

                var verts = primitive.GetVertexAccessor("POSITION").AsVector3Array();
                var uvs = primitive.GetVertexAccessor("TEXCOORD_0").AsVector2Array();
                var norms = primitive.GetVertexAccessor("NORMAL").AsVector3Array();
                var weights = primitive.GetVertexAccessor("WEIGHTS_0");
                var joints = primitive.GetVertexAccessor("JOINTS_0");

                if (weights != null && joints != null)
                {
                    var ws = weights.AsVector4Array();
                    var js = joints.AsVector4Array();

                    for (int j = 0; j < verts.Count; j++)
                    {
                        Vertices.Add(new Vertex(verts[j], norms[j], RgbaFloat.White, uvs[j], js[j], ws[j]));
                    }
                }
                else
                {
                    for (int j = 0; j < verts.Count; j++)
                    {
                        Vertices.Add(new Vertex(verts[j], norms[j], RgbaFloat.White, uvs[j]));
                    }
                }

                foreach (var index in primitive.GetIndices())
                    Indices.Add((ushort)index);

                meshPrimitive.Handle = new MeshHandle(Vertices.ToArray(), Indices.ToArray());

                m_Primitives.Add(meshPrimitive);
            }
        }
    }
}
