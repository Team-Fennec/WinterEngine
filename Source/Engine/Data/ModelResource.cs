using MathLib;
using SharpGLTF.Schema2;
using SharpGLTF.Transforms;
using SharpGLTF.Runtime;
using System.Numerics;
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
    }

    public IReadOnlyList<MeshPrimitive> Primitives => m_Primitives;
    protected List<MeshPrimitive> m_Primitives = new List<MeshPrimitive>();
}

public class GLBModelResource : ModelResource, IResource
{
    ModelRoot m_ModelRoot;
    SceneTemplate m_SceneTemplate;
    SceneInstance m_SceneInstance;

    public void LoadData(Stream stream)
    {
        // el em fucking ay oh
        m_ModelRoot = ModelRoot.ReadGLB(stream);
        stream.Close();

        m_SceneTemplate = SceneTemplate.Create(m_ModelRoot.DefaultScene);
        m_SceneInstance = m_SceneTemplate.CreateInstance();

        foreach (Mesh meshInf in m_ModelRoot.LogicalMeshes)
        {
            foreach (var primitive in meshInf.Primitives)
            {
                MeshPrimitive meshPrimitive = new MeshPrimitive();

                // it says not to, we do it anyways because how else do we load materials
                // we have a special format for materials and disregard any other info.
                meshPrimitive.Material = gMaterialSystem.Load(primitive.Material.Name);

                List<Vertex> Vertices = new List<Vertex>();
                List<uint> Indices = new List<uint>();

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
                    Indices.Add(index);

                meshPrimitive.Handle = new MeshHandle(Vertices.ToArray(), Indices.ToArray());

                m_Primitives.Add(meshPrimitive);
            }
        }
    }

    public List<Matrix4x4> GetAnimJointMatrices(string animName, float time)
    {
        List<Matrix4x4> jointMats = new List<Matrix4x4>();

        Animation? anim = null;
        foreach(Animation animation in m_ModelRoot.LogicalAnimations)
        {
            if (animation.Name == animName)
            {
                anim = animation;
                break;
            }
        }

        if (anim == null)
        {
            throw new Exception($"No animation by name {animName} found!");
        }

        foreach (Node joint in m_ModelRoot.LogicalNodes)
        {
            if (joint.IsSkinJoint)
            {
                jointMats.Add(GetJointTransform(joint, anim, time));
            }
        }

        return jointMats;
    }

    Matrix4x4 GetJointTransform(Node joint, Animation anim, float time)
    {
        AffineTransform transform = joint.GetLocalTransform(anim, time);
        Matrix4x4 jointMat = transform.Matrix;

        if (joint.VisualParent != null)
        {
            jointMat *= GetJointTransform(joint.VisualParent, anim, time);
        }

        return jointMat;
    }
}
