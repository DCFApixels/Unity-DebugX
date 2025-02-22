using UnityEngine;
using static DCFApixels.DebugX;

namespace DCFApixels.DebugXCore
{
    public interface IStaticData
    {
        string GetName();
    }
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
        public string GetName() => "Lit";
        public int GetExecuteOrder() => 0;
        public Material GetMaterial() => Materials.Lit;
    }
    public readonly struct UnlitMat : IStaticMaterial
    {
        public string GetName() => "Unlit";
        public int GetExecuteOrder() => 100_000;
        public Material GetMaterial() => Materials.Unlit;
    }
    public readonly struct WireMat : IStaticMaterial
    {
        public string GetName() => "Wire";
        public int GetExecuteOrder() => 1_000_000;
        public Material GetMaterial() => Materials.Wire;
    }
    public readonly struct BillboardMat : IStaticMaterial
    {
        public string GetName() => "Billboard";
        public int GetExecuteOrder() => 200_000;
        public Material GetMaterial() => Materials.Billboard;
    }
    public readonly struct DotMat : IStaticMaterial
    {
        public string GetName() => "Dot";
        public int GetExecuteOrder() => 300_000;
        public Material GetMaterial() => Materials.Dot;
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public readonly struct SphereMesh : IStaticMesh
    {
        public string GetName() => "Sphere";
        public Mesh GetMesh() => Meshes.Sphere;
    }
    public readonly struct CubeMesh : IStaticMesh
    {
        public string GetName() => "Cube";
        public Mesh GetMesh() => Meshes.Cube;
    }
    public readonly struct QuadMesh : IStaticMesh
    {
        public string GetName() => "Quad";
        public Mesh GetMesh() => Meshes.Quad;
    }
    public readonly struct CircleMesh : IStaticMesh
    {
        public string GetName() => "Circle";
        public Mesh GetMesh() => Meshes.Circle;
    }
    public readonly struct CapsuleBodyMesh : IStaticMesh
    {
        public string GetName() => "CapsuleBody";
        public Mesh GetMesh() => Meshes.CapsuleBody;
    }
    public readonly struct CapsuleHeadMesh : IStaticMesh
    {
        public string GetName() => "CapsuleHead";
        public Mesh GetMesh() => Meshes.CapsuleHead;
    }
    public readonly struct FlatCapsuleBodyMesh : IStaticMesh
    {
        public string GetName() => "FlatCapsuleBody";
        public Mesh GetMesh() => Meshes.FlatCapsuleBody;
    }
    public readonly struct FlatCapsuleHeadMesh : IStaticMesh
    {
        public string GetName() => "FlatCapsuleHead";
        public Mesh GetMesh() => Meshes.FlatCapsuleHead;
    }
    public readonly struct ArrowMesh : IStaticMesh
    {
        public string GetName() => "Arrow";
        public Mesh GetMesh() => Meshes.Arrow;
    }
    public readonly struct DotMesh : IStaticMesh
    {
        public string GetName() => "Dot";
        public Mesh GetMesh() => Meshes.Dot;
    }
    public readonly struct DotQuadMesh : IStaticMesh
    {
        public string GetName() => "DotQuad";
        public Mesh GetMesh() => Meshes.DotQuad;
    }
    public readonly struct DotDiamondMesh : IStaticMesh
    {
        public string GetName() => "DotDiamond";
        public Mesh GetMesh() => Meshes.DotDiamond;
    }
    public readonly struct DotCrossMesh : IStaticMesh
    {
        public string GetName() => "DotCross";
        public Mesh GetMesh() => Meshes.DotCross;
    }
    public readonly struct WireLineMesh : IStaticMesh
    {
        public string GetName() => "WireLine";
        public Mesh GetMesh() => Meshes.WireLine;
    }
    public readonly struct WireCubeMesh : IStaticMesh
    {
        public string GetName() => "WireCube";
        public Mesh GetMesh() => Meshes.WireCube;
    }
    public readonly struct WireArcMesh : IStaticMesh
    {
        public string GetName() => "WireArc";
        public Mesh GetMesh() => Meshes.WireArc;
    }
    public readonly struct WireCircleMesh : IStaticMesh
    {
        public string GetName() => "WireCircle";
        public Mesh GetMesh() => Meshes.WireCircle;
    }
    public readonly struct WireSphereMesh : IStaticMesh
    {
        public string GetName() => "WireSphere";
        public Mesh GetMesh() => Meshes.WireSphere;
    }
}