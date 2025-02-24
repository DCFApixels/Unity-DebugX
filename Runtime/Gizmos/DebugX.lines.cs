//#undef DEBUG
using DCFApixels.DebugXCore;
using UnityEngine;
using UnityEngine.Rendering;

namespace DCFApixels
{
    using IN = System.Runtime.CompilerServices.MethodImplAttribute;
    public static unsafe partial class DebugX
    {
        public readonly partial struct DrawHandler
        {
            //TODO часть функционала рейкастс перенести сюда, типа рисование линий примитивами
            #region LineFade
            [IN(LINE)]
            public DrawHandler LineFade(Vector3 start, Vector3 end)
            {
                const int StepsCount = 6;
                const float Step = 1f / StepsCount;
                const float ColorStep = 1f / (StepsCount + 1);
                Color color = Color;

                Vector3 startPoint = start;
                for (int i = 1; i <= StepsCount; i++)
                {
                    Vector3 endPoint = Vector3.Lerp(start, end, i * Step);
                    color.a = 1f - Color.a * i * ColorStep;
                    Setup(color).Line(startPoint, endPoint);
                    startPoint = endPoint;
                }
                return this;
            }
            #endregion

            #region LineArrow
            [IN(LINE)]
            public DrawHandler LineArrow(Vector3 start, Vector3 end)
            {
                const float MinDistance = 2.5f;
                Vector3 direction = end - start;

                float distance = DebugXUtility.FastSqrt(direction.sqrMagnitude);
                Quaternion rotation = direction == default ? Quaternion.identity : Quaternion.LookRotation(direction);

                var arrowSize = 0.5f;
                if (distance < MinDistance)
                {
                    float x = distance / MinDistance;
                    arrowSize *= x;
                }

                Line(start, end);
                Mesh<ArrowMesh, LitMat>(end, rotation, new Vector3(arrowSize, arrowSize, arrowSize));
                return this;
            }
            #endregion

            #region Line custom
            [IN(LINE)] public DrawHandler Line(Vector3 start, Vector3 end, DebugXLine endType) => Line(start, end, DebugXLine.Default, endType);
            [IN(LINE)]
            public DrawHandler Line(Vector3 start, Vector3 end, DebugXLine startType, DebugXLine endType)
            {
                if (endType == DebugXLine.Default)
                {
                    (end, start, startType, endType) = (start, end, endType, startType);
                }
                if (startType == DebugXLine.Default)
                {
                    switch (endType)
                    {
                        case DebugXLine.Default:
                            Line(start, end);
                            break;
                        case DebugXLine.Arrow:
                            LineArrow(start, end);
                            break;
                        case DebugXLine.Fade:
                            LineFade(start, end);
                            break;
                            //case DebugXLine.Dot:
                            //    Line(start, end);
                            //    Dot(end);
                            //    break;
                            //case DebugXLine.DotCross:
                            //    Line(start, end);
                            //    DotCross(end);
                            //    break;
                            //case DebugXLine.DotQuad:
                            //    Line(start, end);
                            //    DotQuad(end);
                            //    break;
                            //case DebugXLine.DotDiamond:
                            //    Line(start, end);
                            //    DotDiamond(end);
                            //    break;
                    }
                }
                else
                {
                    Vector3 center = (start + end) * 0.5f;
                    Line(center, start, startType);
                    Line(center, end, endType);
                }
                return this;
            }
            #endregion


            #region Ray
            [IN(LINE)] public DrawHandler Ray(Vector3 origin, Quaternion rotation) => Ray(origin, rotation * Vector3.forward);
            [IN(LINE)] public DrawHandler Ray(Ray ray) => Ray(ray.origin, ray.direction);
            [IN(LINE)] public DrawHandler Ray(Vector3 origin, Vector3 direction) => Line(origin, origin + direction);
            #endregion

            #region RayFade
            [IN(LINE)] public DrawHandler RayFade(Vector3 origin, Quaternion rotation) => Ray(origin, rotation * Vector3.forward);
            [IN(LINE)] public DrawHandler RayFade(Ray ray) => RayFade(ray.origin, ray.direction);
            [IN(LINE)] public DrawHandler RayFade(Vector3 origin, Vector3 direction) => LineFade(origin, origin + direction);
            #endregion

            #region RayArrow
            [IN(LINE)] public DrawHandler RayArrow(Vector3 origin, Quaternion rotation) => RayArrow(origin, rotation * Vector3.forward);
            [IN(LINE)] public DrawHandler RayArrow(Ray ray) => RayArrow(ray.origin, ray.direction);
            [IN(LINE)] public DrawHandler RayArrow(Vector3 origin, Vector3 direction) => LineArrow(origin, origin + direction);
            #endregion

            #region Ray custom
            [IN(LINE)]
            public DrawHandler Ray(Vector3 start, Vector3 direction, DebugXLine endType) => Line(start, start + direction, DebugXLine.Default, endType);
            public DrawHandler Ray(Vector3 start, Vector3 direction, DebugXLine startType, DebugXLine endType) => Line(start, start + direction, startType, endType);
            #endregion


            #region WidthLine
            [IN(LINE)] public DrawHandler WidthLine(Vector3 start, Vector3 end, float width) => Gizmo(new WidthLineGizmo(start, end, width));
            private readonly struct WidthLineGizmo : IGizmo<WidthLineGizmo>
            {
                public readonly Vector3 Start;
                public readonly Vector3 End;
                public readonly float HalfWidth;
                [IN(LINE)]
                public WidthLineGizmo(Vector3 start, Vector3 end, float width)
                {
                    Start = start;
                    End = end;
                    HalfWidth = width * 0.5f;
                }
                public IGizmoRenderer<WidthLineGizmo> RegisterNewRenderer() { return new Renderer(); }

                #region Renderer
                private class Renderer : IGizmoRenderer<WidthLineGizmo>
                {
                    public int ExecuteOrder => default(UnlitMat).GetExecuteOrder();
                    public bool IsStaticRender => false;
                    public void Prepare(Camera camera, GizmosList<WidthLineGizmo> list) { }
                    public void Render(Camera camera, GizmosList<WidthLineGizmo> list, CommandBuffer cb)
                    {
                        default(UnlitMat).GetMaterial().SetPass(0);
                        GL.Begin(GL.QUADS);
                        Vector3 cameraPosition = camera.transform.position;
                        foreach (ref readonly var item in list)
                        {
                            GL.Color(item.Color);
                            item.Value.Draw(cameraPosition);
                        }
                        GL.End();
                    }
                }
                [IN(LINE)]
                public void Draw(Vector3 cameraPosition)
                {
                    Vector3 lineDirection = (End - Start).normalized;

                    Vector3 cameraDirection = (cameraPosition - Start).normalized;
                    Vector3 perpendicular = Vector3.Cross(lineDirection, cameraDirection).normalized * HalfWidth;
                    GL.Vertex(Start - perpendicular);
                    GL.Vertex(Start + perpendicular);
                    cameraDirection = (cameraPosition - End).normalized;
                    perpendicular = Vector3.Cross(lineDirection, cameraDirection).normalized * HalfWidth;
                    GL.Vertex(End + perpendicular);
                    GL.Vertex(End - perpendicular);
                }
                #endregion
            }
            #endregion

            #region WidthOutLine
            [IN(LINE)] public DrawHandler WidthOutLine(Vector3 start, Vector3 end, float width) => Gizmo(new WidthOutLineGizmo(start, end, width));
            private readonly struct WidthOutLineGizmo : IGizmo<WidthOutLineGizmo>
            {
                public readonly Vector3 Start;
                public readonly Vector3 End;
                public readonly float HalfWidth;
                [IN(LINE)]
                public WidthOutLineGizmo(Vector3 start, Vector3 end, float width)
                {
                    Start = start;
                    End = end;
                    HalfWidth = width * 0.5f;
                }
                public IGizmoRenderer<WidthOutLineGizmo> RegisterNewRenderer() { return new Renderer(); }

                #region Renderer
                private class Renderer : IGizmoRenderer<WidthOutLineGizmo>
                {
                    public int ExecuteOrder => default(GeometryUnlitMat).GetExecuteOrder();
                    public bool IsStaticRender => false;
                    public void Prepare(Camera camera, GizmosList<WidthOutLineGizmo> list) { }
                    public void Render(Camera camera, GizmosList<WidthOutLineGizmo> list, CommandBuffer cb)
                    {
                        default(GeometryUnlitMat).GetMaterial().SetPass(0);
                        GL.Begin(GL.LINES);
                        var cameraPosition = camera.transform.position;
                        foreach (ref readonly var item in list)
                        {
                            GL.Color(item.Color);
                            item.Value.Draw(cameraPosition);
                        }
                        GL.End();
                    }
                }
                [IN(LINE)]
                public void Draw(Vector3 cameraPosition)
                {
                    Vector3 lineDirection = (End - Start).normalized;

                    Vector3 cameraDirection = (cameraPosition - Start).normalized;
                    Vector3 perpendicular = Vector3.Cross(lineDirection, cameraDirection).normalized * HalfWidth;
                    Vector3 Start1 = Start - perpendicular;
                    Vector3 Start2 = Start + perpendicular;

                    cameraDirection = (cameraPosition - End).normalized;
                    perpendicular = Vector3.Cross(lineDirection, cameraDirection).normalized * HalfWidth;
                    GL.Vertex(Start1);
                    GL.Vertex(End - perpendicular);
                    GL.Vertex(Start2);
                    GL.Vertex(End + perpendicular);
                }
                #endregion
            }
            #endregion

            #region ZigzagLine
            public DrawHandler ZigzagLine(Vector3 start, Vector3 end) => Gizmo(new ZigzagLineGizmo(start, end, 0.3f));
            public DrawHandler ZigzagLine(Vector3 start, Vector3 end, float height) => Gizmo(new ZigzagLineGizmo(start, end, height));
            private readonly struct ZigzagLineGizmo : IGizmo<ZigzagLineGizmo>
            {
                public readonly Vector3 Start;
                public readonly Vector3 End;
                public readonly float Width;
                [IN(LINE)]
                public ZigzagLineGizmo(Vector3 start, Vector3 end, float width)
                {
                    Start = start;
                    End = end;
                    Width = width;
                }
                public IGizmoRenderer<ZigzagLineGizmo> RegisterNewRenderer()
                {
                    return new Renderer();
                }

                #region Renderer
                private class Renderer : IGizmoRenderer<ZigzagLineGizmo>
                {
                    public bool IsStaticRender => false;
                    public int ExecuteOrder => default(GeometryUnlitMat).GetExecuteOrder();
                    public void Prepare(Camera camera, GizmosList<ZigzagLineGizmo> list) { }
                    public void Render(Camera camera, GizmosList<ZigzagLineGizmo> list, CommandBuffer cb)
                    {
                        GL.PushMatrix();
                        default(GeometryUnlitMat).GetMaterial().SetPass(0);
                        GL.Begin(GL.LINES);
                        var cameraPosition = camera.transform.position;
                        foreach (ref readonly var item in list)
                        {
                            GL.Color(item.Color);
                            item.Value.Draw(cameraPosition);
                        }
                        GL.End();
                        GL.PopMatrix();
                    }
                }
                [IN(LINE)]
                public void Draw(Vector3 cameraPosition)
                {
                    var waveFrequency = 0.75f / Width;
                    var waveAmplitude = Width / 2f;


                    Vector3 lineDirection = (End - Start).normalized;
                    float lineLength = (End - Start).magnitude;

                    int waveCount = Mathf.FloorToInt(lineLength * waveFrequency) | 1;

                    Vector3 perpendicularStart = Vector3.Cross(lineDirection, (cameraPosition - Start).normalized).normalized;
                    Vector3 perpendicularEnd = Vector3.Cross(lineDirection, (cameraPosition - End).normalized).normalized;


                    Vector3 prevPoint = Start;
                    float t = 0f;
                    float addT = 1f / waveCount;
                    for (int i = 0; i < waveCount - 1; i++)
                    {
                        t += addT;

                        Vector3 pointOnLine = Vector3.Lerp(Start, End, t);
                        Vector3 perpendicular = Vector3.Lerp(perpendicularStart, perpendicularEnd, t);

                        int bitFloat = ((i & 1) << 31) | 0x3F800000/* -1f */;
                        float offset = (*(float*)&bitFloat) * waveAmplitude;

                        Vector3 nextPoint = pointOnLine + perpendicular * offset;

                        GL.Vertex(prevPoint);
                        GL.Vertex(nextPoint);

                        prevPoint = nextPoint;
                    }

                    GL.Vertex(prevPoint);
                    GL.Vertex(End);
                }
                #endregion

            }
            #endregion
        }
    }
}