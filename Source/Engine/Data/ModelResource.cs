using MathLib;
using SharpGLTF.Schema2;
using Veldrid;
using WinterEngine.MaterialSystem;
using WinterEngine.RenderSystem;
using WinterEngine.Resource;
using gMaterialSystem = WinterEngine.MaterialSystem.MaterialSystem;

namespace WinterEngine.Data;

public class GLBModelResource : IResource
{
    public struct MeshPrimitive
    {
        public MeshHandle Handle;
        public MaterialResource Material;
        // used by the render system to hold the shader parameter resources
        public ResourceSet ShaderResSet;
    }

    ModelRoot m_ModelRoot;

    public IReadOnlyList<MeshPrimitive> Primitives => m_Primitives;
    List<MeshPrimitive> m_Primitives = new List<MeshPrimitive>();

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

                for (int j = 0; j < verts.Count; j++)
                {
                    Vertices.Add(new Vertex(verts[j], norms[j], RgbaFloat.White, uvs[j]));
                }

                foreach (var index in primitive.GetIndices())
                    Indices.Add((ushort)index);

                meshPrimitive.Handle = new MeshHandle(Vertices.ToArray(), Indices.ToArray());
                m_Primitives.Add(meshPrimitive);
            }
        }
    }
}
