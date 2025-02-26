using UnityEngine;
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
        public Material GetMaterial() => DebugXAssets.Materials.Lit;
    }
    public readonly struct UnlitMat : IStaticMaterial
    {
        public int GetExecuteOrder() => 100_000;
        public Material GetMaterial() => DebugXAssets.Materials.Unlit;
    }
    public readonly struct BillboardMat : IStaticMaterial
    {
        public int GetExecuteOrder() => 200_000;
        public Material GetMaterial() => DebugXAssets.Materials.Billboard;
    }
    public readonly struct DotMat : IStaticMaterial
    {
        public int GetExecuteOrder() => 300_000;
        public Material GetMaterial() => DebugXAssets.Materials.Dot;
    }
    public readonly struct GeometryUnlitMat : IStaticMaterial
    {
        public int GetExecuteOrder() => 1_000_000;
        public Material GetMaterial() => DebugXAssets.Materials.Unlit;
    }
    public readonly struct WireMat : IStaticMaterial
    {
        public int GetExecuteOrder() => 1_000_000;
        public Material GetMaterial() => DebugXAssets.Materials.Wire;
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public readonly struct SphereMesh : IStaticMesh
    {
        public Mesh GetMesh() => DebugXAssets.Meshes.Sphere;
    }
    public readonly struct CubeMesh : IStaticMesh
    {
        public Mesh GetMesh() => DebugXAssets.Meshes.Cube;
    }
    public readonly struct QuadMesh : IStaticMesh
    {
        public Mesh GetMesh() => DebugXAssets.Meshes.Quad;
    }
    public readonly struct CircleMesh : IStaticMesh
    {
        public Mesh GetMesh() => DebugXAssets.Meshes.Circle;
    }
    public readonly struct CylinderMesh : IStaticMesh
    {
        public Mesh GetMesh() => DebugXAssets.Meshes.Cylinder;
    }
    public readonly struct ConeMesh : IStaticMesh
    {
        public Mesh GetMesh() => DebugXAssets.Meshes.Cone;
    }
    public readonly struct TriangleMesh : IStaticMesh
    {
        public Mesh GetMesh() => DebugXAssets.Meshes.Triangle;
    }
    public readonly struct CapsuleBodyMesh : IStaticMesh
    {
        public Mesh GetMesh() => DebugXAssets.Meshes.CapsuleBody;
    }
    public readonly struct CapsuleHeadMesh : IStaticMesh
    {
        public Mesh GetMesh() => DebugXAssets.Meshes.CapsuleHead;
    }
    public readonly struct FlatCapsuleBodyMesh : IStaticMesh
    {
        public Mesh GetMesh() => DebugXAssets.Meshes.FlatCapsuleBody;
    }
    public readonly struct FlatCapsuleHeadMesh : IStaticMesh
    {
        public Mesh GetMesh() => DebugXAssets.Meshes.FlatCapsuleHead;
    }
    public readonly struct ArrowMesh : IStaticMesh
    {
        public Mesh GetMesh() => DebugXAssets.Meshes.Arrow;
    }
    public readonly struct DotMesh : IStaticMesh
    {
        public Mesh GetMesh() => DebugXAssets.Meshes.Dot;
    }
    public readonly struct DotQuadMesh : IStaticMesh
    {
        public Mesh GetMesh() => DebugXAssets.Meshes.DotQuad;
    }
    public readonly struct DotDiamondMesh : IStaticMesh
    {
        public Mesh GetMesh() => DebugXAssets.Meshes.DotDiamond;
    }
    public readonly struct WireDotDiamondMesh : IStaticMesh
    {
        public Mesh GetMesh() => DebugXAssets.Meshes.WireDotDiamond;
    }
    public readonly struct DotCrossMesh : IStaticMesh
    {
        public Mesh GetMesh() => DebugXAssets.Meshes.DotCross;
    }
    public readonly struct WireLineMesh : IStaticMesh
    {
        public Mesh GetMesh() => DebugXAssets.Meshes.WireLine;
    }
    public readonly struct WireCubeMesh : IStaticMesh
    {
        public Mesh GetMesh() => DebugXAssets.Meshes.WireCube;
    }
    public readonly struct WireArcMesh : IStaticMesh
    {
        public Mesh GetMesh() => DebugXAssets.Meshes.WireArc;
    }
    public readonly struct WireCircleMesh : IStaticMesh
    {
        public Mesh GetMesh() => DebugXAssets.Meshes.WireCircle;
    }
    public readonly struct WireSphereMesh : IStaticMesh
    {
        public Mesh GetMesh() => DebugXAssets.Meshes.WireSphere;
    }
}