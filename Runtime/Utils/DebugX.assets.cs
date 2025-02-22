using DCFApixels.DebugXCore.Internal;
using UnityEngine;
using UnityEngine.Rendering;

namespace DCFApixels
{
    public static partial class DebugX
    {
        public static T LoadMeshesList<T>(T list, string path)
        {
            return MeshesListLoader.Load(list, path);
        }
        public static MeshesList Meshes;
        public readonly struct MeshesList
        {
            public readonly Mesh Arrow;
            public readonly Mesh Cube;
            public readonly Mesh Quad;
            public readonly Mesh Circle; // Circle_1
            public readonly Mesh Sphere; // Sphere_0

            public readonly Mesh CapsuleBody;
            public readonly Mesh CapsuleHead;
            public readonly Mesh FlatCapsuleBody;
            public readonly Mesh FlatCapsuleHead;

            public readonly Mesh Dot;
            public readonly Mesh DotQuad;
            public readonly Mesh DotCross;
            public readonly Mesh DotDiamond;

            public readonly Mesh WireLine;
            public readonly Mesh WireCube;
            public readonly Mesh WireArc;
            public readonly Mesh WireCircle;
            public readonly Mesh WireSphere;
        }


        private static Material CreateMaterial(string name)
        {
            Material result = new Material(Shader.Find(name));
            result.SetColor(ColorPropertyID, Color.white);
            result.renderQueue = (int)RenderQueue.Transparent;
            result.enableInstancing = true;
            return result;
        }
        public static class Materials
        {
            private static Material _lit;
            private static Material _unlit;
            private static Material _wire;
            private static Material _billboard;
            private static Material _dot;

            private static void Init()
            {
                if (_lit != null) { return; }
                _lit = CreateMaterial("DCFApixels/DebugX/Handles Lit");
                _unlit = CreateMaterial("DCFApixels/DebugX/Handles");
                _wire = CreateMaterial("DCFApixels/DebugX/Handles Wire");
                _billboard = CreateMaterial("DCFApixels/DebugX/Handles Buillboard");
                _dot = CreateMaterial("DCFApixels/DebugX/Handles Dot");
            }

            public static Material Lit
            {
                get
                {
                    Init();
                    return _lit;
                }
            }
            public static Material Unlit
            {
                get
                {
                    Init();
                    return _unlit;
                }
            }
            public static Material Wire
            {
                get
                {
                    Init();
                    return _wire;
                }
            }
            public static Material Billboard
            {
                get
                {
                    Init();
                    return _billboard;
                }
            }
            public static Material Dot
            {
                get
                {
                    Init();
                    return _dot;
                }
            }
        }
    }
}