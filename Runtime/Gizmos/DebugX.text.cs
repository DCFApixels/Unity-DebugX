using DCFApixels.DebugXCore;
using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DCFApixels
{
    using IN = System.Runtime.CompilerServices.MethodImplAttribute;

    public static partial class DebugX
    {
        public readonly partial struct DrawHandler
        {
            #region Text
            [IN(LINE)] public DrawHandler Text(Vector3 position, object text) => Gizmo(new TextGizmo(position, text, DebugXTextSettings.Default));
            [IN(LINE)] public DrawHandler Text(Vector3 position, object text, DebugXTextSettings settings) => Gizmo(new TextGizmo(position, text, settings));

            private readonly struct TextGizmo : IGizmo<TextGizmo>
            {
                public readonly Vector3 Position;
                public readonly string Text;
                public readonly DebugXTextSettings Settings;
                [IN(LINE)]
                public TextGizmo(Vector3 position, object text, DebugXTextSettings settings)
                {
                    Position = position;
                    Text = text.ToString();
                    Settings = settings;
                }

                public IGizmoRenderer<TextGizmo> RegisterNewRenderer() { return new Renderer(); }

                #region Renderer
                private class Renderer : IGizmoRenderer_PostRender<TextGizmo>
                {
                    private static GUIStyle _labelStyle;
                    private static GUIContent _labelDummy;
                    private static Texture2D _whiteTexture;
                    public int ExecuteOrder => default(UnlitMat).GetExecuteOrder();
                    public bool IsStaticRender => false;
                    public void Prepare(Camera camera, GizmosList<TextGizmo> list) { }
                    public void Render(Camera camera, GizmosList<TextGizmo> list, CommandBuffer cb) { }
                    public void PostRender(Camera camera, GizmosList<TextGizmo> list)
                    {
                        if (camera == null) { return; }
                        if (Event.current.type != EventType.Repaint) { return; }
                        Color dfColor = GUI.color;
                        InitStatic();
                        bool isSceneView = false;
                        var backgroundMaterial = DebugXAssets.Materials.TextBackground;

#if UNITY_EDITOR
                        //TODO костыльный вариант, нужно поискать более надежный способ определить рендер SceneView
                        isSceneView = camera.name == "SceneCamera";
#endif

                        if (isSceneView)
                        {
#if UNITY_EDITOR
                            Handles.BeginGUI();
#endif
                        }
                        else
                        {
                            GL.PushMatrix();
                            GL.LoadPixelMatrix(0, Screen.width, Screen.height, 0);
                        }
                        foreach (ref readonly var item in list)
                        {
                            _labelDummy.text = item.Value.Text;
                            GUIStyle style = _labelStyle;

                            var zoom = GetCameraZoom(camera, item.Value.Position);

                            style.fontSize = Mathf.FloorToInt(Mathf.Lerp(item.Value.Settings.FontSize, item.Value.Settings.FontSize / zoom, item.Value.Settings.WorldSpaceScaleFactor));

                            style.alignment = item.Value.Settings.TextAnchor;
                            if (!(WorldToGUIPointWithDepth(camera, item.Value.Position).z < 0f))
                            {
                                Rect rect = WorldPointToSizedRect(camera, item.Value.Position, _labelDummy, _labelStyle);
                                Color backgroundColor = item.Value.Settings.BackgroundColor * GlobalColor;
                                Graphics.DrawTexture(rect, _whiteTexture, new Rect(0, 0, 1, 1), 0, 0, 0, 0, backgroundColor, backgroundMaterial, -1);
                                GUI.color = item.Color * GlobalColor;
                                style.Draw(rect, _labelDummy, false, false, false, false);
                            }
                        }
                        GUI.color = dfColor;
                        backgroundMaterial.SetColor(ColorPropertyID, Color.white);

                        if (isSceneView)
                        {
#if UNITY_EDITOR
                            Handles.EndGUI();
#endif
                        }
                        else
                        {
                            GL.PopMatrix();
                        }
                    }

                    #region Init
                    private void InitStatic()
                    {
                        if (_labelStyle == null || _labelDummy == null || _whiteTexture == null)
                        {
                            GUIStyleState GenerateGUIStyleState()
                            {
                                var result = new GUIStyleState();
                                result.textColor = Color.white;
                                result.background = null;
                                return result;
                            }
                            GUISkin skin = (GUISkin)typeof(GUI).GetField("s_Skin", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).GetValue(null);  //GUI.s_Skin
                            //GUISkin skin = GUI.skin;
                            _labelStyle = new GUIStyle(skin.label)
                            {
                                richText = false,
                                padding = new RectOffset(1, 1, 0, 0),
                                margin = new RectOffset(0, 0, 0, 0),
                                normal = GenerateGUIStyleState(),
                                active = GenerateGUIStyleState(),
                                hover = GenerateGUIStyleState(),
                                focused = GenerateGUIStyleState(),
                            };

                            _labelDummy = new GUIContent();

                            _whiteTexture = new Texture2D(2, 2);
                            Color32[] color = new Color32[]
                            {
                                Color.white,
                                Color.white,
                                Color.white,
                                Color.white,
                            };
                            _whiteTexture.SetPixels32(color);
                            _whiteTexture.Apply();
                        }
                    }
                    #endregion

                    #region Utils
                    public static Vector3 WorldToGUIPointWithDepth(Camera camera, Vector3 world)
                    {
#if UNITY_EDITOR
                        world = Handles.matrix.MultiplyPoint(world);
#endif
                        Vector3 vector = camera.WorldToScreenPoint(world);
                        vector.y = camera.pixelHeight - vector.y;
                        Vector2 vector2 = (Vector2)(vector);
#if UNITY_EDITOR
                        vector2 = EditorGUIUtility.PixelsToPoints(vector);
#endif
                        return new Vector3(vector2.x, vector2.y, vector.z);
                    }
                    public static Rect WorldPointToSizedRect(Camera camera, Vector3 position, GUIContent content, GUIStyle style)
                    {
                        Vector2 vector = (Vector2)WorldToGUIPointWithDepth(camera, position);
                        Vector2 vector2 = style.CalcSize(content);
                        Rect rect = new Rect(vector.x, vector.y, vector2.x, vector2.y);
                        switch (style.alignment)
                        {
                            case TextAnchor.UpperCenter:
                                rect.x -= rect.width * 0.5f;
                                break;
                            case TextAnchor.UpperRight:
                                rect.x -= rect.width;
                                break;
                            case TextAnchor.MiddleLeft:
                                rect.y -= rect.height * 0.5f;
                                break;
                            case TextAnchor.MiddleCenter:
                                rect.x -= rect.width * 0.5f;
                                rect.y -= rect.height * 0.5f;
                                break;
                            case TextAnchor.MiddleRight:
                                rect.x -= rect.width;
                                rect.y -= rect.height * 0.5f;
                                break;
                            case TextAnchor.LowerLeft:
                                rect.y -= rect.height;
                                break;
                            case TextAnchor.LowerCenter:
                                rect.x -= rect.width * 0.5f;
                                rect.y -= rect.height;
                                break;
                            case TextAnchor.LowerRight:
                                rect.x -= rect.width;
                                rect.y -= rect.height;
                                break;
                        }
                        return style.padding.Add(rect);
                    }
                    private static float GetCameraZoom(Camera camera, Vector3 position)
                    {
#if UNITY_EDITOR
                        position = Handles.matrix.MultiplyPoint(position);
#endif
                        Transform transform = camera.transform;
                        Vector3 position2 = transform.position;
                        float z = Vector3.Dot(position - position2, transform.TransformDirection(new Vector3(0f, 0f, 1f)));
                        Vector3 vector = camera.WorldToScreenPoint(position2 + transform.TransformDirection(new Vector3(0f, 0f, z)));
                        Vector3 vector2 = camera.WorldToScreenPoint(position2 + transform.TransformDirection(new Vector3(1f, 0f, z)));
                        float magnitude = (vector - vector2).magnitude;
                        //return 80f / Mathf.Max(magnitude, 0.0001f) * EditorGUIUtility.pixelsPerPoint;
                        return 80f / Mathf.Max(magnitude, 0.0001f);


                        //const float DEFAULT_ZOOM = 1f;
                        //
                        //if (camera != null)
                        //{
                        //    return camera.orthographicSize;
                        //}
                        //return DEFAULT_ZOOM;

                        //var currentDrawingSceneView = SceneView.currentDrawingSceneView;
                        //
                        //if (currentDrawingSceneView == null)
                        //{
                        //    return DEFAULT_ZOOM;
                        //}
                        //
                        //var localCamera = currentDrawingSceneView.camera;
                        //
                        //if (localCamera != null)
                        //{
                        //    return localCamera.orthographicSize;
                        //}
                        //
                        //return DEFAULT_ZOOM;
                    }
                    #endregion
                }
                #endregion
            }

            #endregion
        }
    }
}