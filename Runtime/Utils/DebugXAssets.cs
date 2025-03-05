using UnityEngine;

namespace DCFApixels.DebugXCore
{
    public static partial class DebugXAssets
    {
        static DebugXAssets()
        {
            Meshes = DebugXUtility.LoadStaticData(new MeshesList(), $"DCFApixels.DebugX/MeshesList");
            Materials = DebugXUtility.LoadStaticData(new MaterialsList(), $"DCFApixels.DebugX/MaterialsList");
        }
        public static MeshesList Meshes;
        public readonly struct MeshesList
        {
            public readonly Mesh Arrow;
            public readonly Mesh Cube;
            public readonly Mesh Quad;
            public readonly Mesh Circle; // Circle_1
            public readonly Mesh Sphere; // Sphere_0
            public readonly Mesh Cylinder;
            public readonly Mesh Cone;
            public readonly Mesh Triangle;

            public readonly Mesh CapsuleBody;
            public readonly Mesh CapsuleHead;
            public readonly Mesh FlatCapsuleBody;
            public readonly Mesh FlatCapsuleHead;

            public readonly Mesh Dot;
            public readonly Mesh DotQuad;
            public readonly Mesh DotCross;
            public readonly Mesh DotDiamond;
            public readonly Mesh WireDotDiamond;

            public readonly Mesh WireLine;
            public readonly Mesh WireCube;
            public readonly Mesh WireArc;
            public readonly Mesh WireCircle;
            public readonly Mesh WireSphere;
        }

        public static MaterialsList Materials;
        public readonly struct MaterialsList
        {
            public readonly Material Lit;
            //public readonly Material Lit_InstancedProcedural;
            public readonly Material Unlit;
            //public readonly Material Unlit_InstancedProcedural;
            public readonly Material UnlitOverwrite;
            //public readonly Material UnlitOverwrite_InstancedProcedural;
            public readonly Material Billboard;
            //public readonly Material Billboard_InstancedProcedural;
            public readonly Material Dot;
            //public readonly Material Dot_InstancedProcedural;
            public readonly Material Wire;
            //public readonly Material Wire_InstancedProcedural;

            internal readonly Material TextBackground;
        }
    }
}