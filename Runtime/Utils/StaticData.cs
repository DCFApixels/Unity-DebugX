using UnityEngine;
using static DCFApixels.DebugX;

namespace DCFApixels.DebugXCore
{
    public interface IStaticData { }
    public interface IStaticMaterial : IStaticData
    {
        int GetExecuteOrder();
        Material GetMaterial();
    }
    public interface IStaticMesh : IStaticData
    {
        Mesh GetMesh();
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public readonly struct LitMat : IStaticMaterial
    {
        public int GetExecuteOrder() => 0;
        public Material GetMaterial() => Materials.Lit;
    }
    public readonly struct UnlitMat : IStaticMaterial
    {
        public int GetExecuteOrder() => 100_000;
        public Material GetMaterial() => Materials.Unlit;
    }
    public readonly struct WireMat : IStaticMaterial
    {
        public int GetExecuteOrder() => 1_000_000;
        public Material GetMaterial() => Materials.Wire;
    }
    public readonly struct BillboardMat : IStaticMaterial
    {
        public int GetExecuteOrder() => 200_000;
        public Material GetMaterial() => Materials.Billboard;
    }
    public readonly struct DotMat : IStaticMaterial
    {
        public int GetExecuteOrder() => 300_000;
        public Material GetMaterial() => Materials.Dot;
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public readonly struct SphereMesh : IStaticMesh
    {
        public Mesh GetMesh() => Meshes.Sphere;
    }
    public readonly struct CubeMesh : IStaticMesh
    {
        public Mesh GetMesh() => Meshes.Cube;
    }
    public readonly struct QuadMesh : IStaticMesh
    {
        public Mesh GetMesh() => Meshes.Quad;
    }
    public readonly struct CircleMesh : IStaticMesh
    {
        public Mesh GetMesh() => Meshes.Circle;
    }
    public readonly struct CapsuleBodyMesh : IStaticMesh
    {
        public Mesh GetMesh() => Meshes.CapsuleBody;
    }
    public readonly struct CapsuleHeadMesh : IStaticMesh
    {
        public Mesh GetMesh() => Meshes.CapsuleHead;
    }
    public readonly struct FlatCapsuleBodyMesh : IStaticMesh
    {
        public Mesh GetMesh() => Meshes.FlatCapsuleBody;
    }
    public readonly struct FlatCapsuleHeadMesh : IStaticMesh
    {
        public Mesh GetMesh() => Meshes.FlatCapsuleHead;
    }
    public readonly struct ArrowMesh : IStaticMesh
    {
        public Mesh GetMesh() => Meshes.Arrow;
    }
    public readonly struct DotMesh : IStaticMesh
    {
        public Mesh GetMesh() => Meshes.Dot;
    }
    public readonly struct DotQuadMesh : IStaticMesh
    {
        public Mesh GetMesh() => Meshes.DotQuad;
    }
    public readonly struct DotDiamondMesh : IStaticMesh
    {
        public Mesh GetMesh() => Meshes.DotDiamond;
    }
    public readonly struct DotCrossMesh : IStaticMesh
    {
        public Mesh GetMesh() => Meshes.DotCross;
    }
    public readonly struct WireLineMesh : IStaticMesh
    {
        public Mesh GetMesh() => Meshes.WireLine;
    }
    public readonly struct WireCubeMesh : IStaticMesh
    {
        public Mesh GetMesh() => Meshes.WireCube;
    }
    public readonly struct WireArcMesh : IStaticMesh
    {
        public Mesh GetMesh() => Meshes.WireArc;
    }
    public readonly struct WireCircleMesh : IStaticMesh
    {
        public Mesh GetMesh() => Meshes.WireCircle;
    }
    public readonly struct WireSphereMesh : IStaticMesh
    {
        public Mesh GetMesh() => Meshes.WireSphere;
    }
}