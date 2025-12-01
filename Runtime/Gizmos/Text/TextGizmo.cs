#if DISABLE_DEBUG
#undef DEBUG
#endif
#if UNITY_EDITOR
using UnityEditor;
#endif
using DCFApixels.DebugXCore;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

namespace DCFApixels
{
    using IN = MethodImplAttribute;
    internal readonly struct TextGizmo : IGizmo<TextGizmo>
    {
        private const MethodImplOptions LINE = MethodImplOptions.AggressiveInlining;
        public readonly Vector3 Position;
        public readonly string Text;
        public readonly DebugXTextSettings Settings;
        [IN(LINE)]
        public TextGizmo(Vector3 position, object text, DebugXTextSettings settings)
        {
            Position = position;
            if(text == null)
            {
                Text = "";
            }
            else
            {
                Text = text.ToString();
            }
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
                if (camera == null || Event.current == null) { return; }
                if (Event.current.type != EventType.Repaint) { return; }
                Color dfColor = GUI.color;
                InitStatic();
                bool isSceneView = false;
                var backgroundMaterial = DebugXAssets.Materials.TextBackground;

#if UNITY_EDITOR
                // TODO: Might replace this with `camera != null && SceneView.GetAllSceneCameras().Any(x => x == camera)`;
                // TODO: Not sure if older Unity-versions have GetAllSceneCameras or not.
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
                    float fontSize = Mathf.Lerp(item.Value.Settings.FontSize, item.Value.Settings.FontSize / zoom, item.Value.Settings.WorldSpaceScaleFactor);
                    style.fontSize = Mathf.Max(1, Mathf.FloorToInt(fontSize));

                    style.alignment = item.Value.Settings.TextAnchor;
                    if (!(WorldToGUIPointWithDepth(camera, item.Value.Position).z < 0f))
                    {
                        Rect rect = WorldPointToSizedRect(camera, item.Value.Position, _labelDummy, _labelStyle);
                        if (item.Value.Settings.IsHasBackground)
                        {
                            Color backgroundColor = item.Value.Settings.BackgroundColor * DebugX.GlobalColor;
                            if (fontSize < 1)
                            {
                                backgroundColor.a *= fontSize;
                            }
                            Graphics.DrawTexture(rect, _whiteTexture, new Rect(0, 0, 1, 1), 0, 0, 0, 0, backgroundColor, backgroundMaterial, -1);
                        }
                        Color color = item.Color * DebugX.GlobalColor;
                        if (fontSize < 1)
                        {
                            color.a *= fontSize;
                        }
                        GUI.color = color;
                        style.Draw(rect, _labelDummy, false, false, false, false);
                    }
                }
                GUI.color = dfColor;
                backgroundMaterial.SetColor(DebugX.ColorPropertyID, Color.white);

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

                    // If calling GUI.skin directly - Unity will throw exceptions saying that GUI.skin can be called only from OnSceneGUI context.
                    GUISkin skin = (GUISkin)typeof(GUI).GetField("s_Skin", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).GetValue(null);  //GUI.s_Skin

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
                Transform cameraTransform = camera.transform;
                Vector3 cameraPos = cameraTransform.position;
                float z = Vector3.Dot(position - cameraPos, cameraTransform.TransformDirection(new Vector3(0f, 0f, 1f)));
                Vector3 pos1 = camera.WorldToScreenPoint(cameraPos + cameraTransform.TransformDirection(new Vector3(0f, 0f, z)));
                Vector3 pos2 = camera.WorldToScreenPoint(cameraPos + cameraTransform.TransformDirection(new Vector3(1f, 0f, z)));
                float magnitude = (pos1 - pos2).magnitude;
#if UNITY_EDITOR
                return 80f / Mathf.Max(magnitude, 0.0001f) * EditorGUIUtility.pixelsPerPoint;
#else
                return 80f / Mathf.Max(magnitude, 0.0001f);
#endif
            }
            #endregion
        }
        #endregion
    }
}