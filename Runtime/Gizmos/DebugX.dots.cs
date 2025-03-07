using DCFApixels.DebugXCore;
using UnityEngine;

namespace DCFApixels
{
    using static DebugXConsts;
    using IN = System.Runtime.CompilerServices.MethodImplAttribute;
    public unsafe static partial class DebugX
    {
        public readonly partial struct DrawHandler
        {
            #region DotCross
            [IN(LINE)] public DrawHandler DotCross(Vector3 position) => Mesh<DotCrossMesh, DotMat>(position, Quaternion.identity, new Vector3(0.06f, 0.06f, 1f));
            #endregion

            #region Dot
            [IN(LINE)] public DrawHandler Dot(Vector3 position) => Mesh<DotMesh, DotMat>(position, Quaternion.identity, new Vector3(DOT_SIZE, DOT_SIZE, 1f));
            #endregion

            #region WireDot
            [IN(LINE)] public DrawHandler WireDot(Vector3 position) => Mesh<WireCircleMesh, DotMat>(position, Quaternion.identity, new Vector3(DOT_SIZE * 0.5f, DOT_SIZE * 0.5f, 1f));
            #endregion

            #region DotQuad
            [IN(LINE)] public DrawHandler DotQuad(Vector3 position) => Mesh<DotQuadMesh, DotMat>(position, Quaternion.identity, new Vector3(DOT_SIZE, DOT_SIZE, 1f));
            #endregion

            #region WireDotQuad
            [IN(LINE)] public DrawHandler WireDotQuad(Vector3 position) => Mesh<WireCubeMesh, DotMat>(position, Quaternion.identity, new Vector3(DOT_SIZE, DOT_SIZE, 0f));
            #endregion

            #region DotDiamond
            [IN(LINE)] public DrawHandler DotDiamond(Vector3 position) => Mesh<DotDiamondMesh, DotMat>(position, Quaternion.identity, new Vector3(DOT_SIZE * 1.16f, DOT_SIZE * 1.16f, 1f));
            #endregion

            #region WireDotDiamond
            [IN(LINE)] public DrawHandler WireDotDiamond(Vector3 position) => Mesh<WireDotDiamondMesh, DotMat>(position, Quaternion.identity, new Vector3(DOT_SIZE * 1.16f, DOT_SIZE * 1.16f, 1f));
            #endregion
        }
    }
}