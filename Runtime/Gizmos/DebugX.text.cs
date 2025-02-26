using DCFApixels.DebugXCore;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Rendering;

namespace DCFApixels
{
    using IN = System.Runtime.CompilerServices.MethodImplAttribute;

    public static partial class DebugX
    {
        public readonly partial struct DrawHandler
        {
            #region Text
            [IN(LINE)] public DrawHandler TextWorldScale(Vector3 position, object text) => Gizmo(new TextGizmo(position, text, DebugXTextSettings.Default, true));
            [IN(LINE)] public DrawHandler TextWorldScale(Vector3 position, object text, DebugXTextSettings settings) => Gizmo(new TextGizmo(position, text, settings, true));
            [IN(LINE)] public DrawHandler Text(Vector3 position, object text) => Gizmo(new TextGizmo(position, text, DebugXTextSettings.Default, false));
            [IN(LINE)] public DrawHandler Text(Vector3 position, object text, DebugXTextSettings settings) => Gizmo(new TextGizmo(position, text, settings, false));

            private readonly struct TextGizmo : IGizmo<TextGizmo>
            {
                public readonly Vector3 Position;
                public readonly string Text;
                public readonly DebugXTextSettings Settings;
                public readonly bool IsWorldSpaceScale;
                [IN(LINE)]
                public TextGizmo(Vector3 position, object text, DebugXTextSettings settings, bool isWorldSpaceScale)
                {
                    Position = position;
                    Text = text.ToString();
                    Settings = settings;
                    IsWorldSpaceScale = isWorldSpaceScale;
                }

                public IGizmoRenderer<TextGizmo> RegisterNewRenderer() { return new Renderer(); }

                #region Renderer
                private class Renderer : IGizmoRenderer_UnityGizmos<TextGizmo>
                {
                    private static Color32[] _backgroundTexturePixels;
                    private static Texture2D _backgroundTexture;

                    private static GUIStyle _labelStyle; 
                    private static GUIStyle _labelStyleWithBackground;
                    private static GUIContent _labelDummy;
                    public int ExecuteOrder => default(UnlitMat).GetExecuteOrder();
                    public bool IsStaticRender => false;
                    public void Prepare(Camera camera, GizmosList<TextGizmo> list) { }
                    public void Render(Camera camera, GizmosList<TextGizmo> list, CommandBuffer cb) { }
                    public void Render_UnityGizmos(Camera camera, GizmosList<TextGizmo> list)
                    {
#if UNITY_EDITOR
                        InitStatic();
                        var zoom = GetCameraZoom(camera);

                        Handles.BeginGUI();
                        foreach (ref readonly var item in list)
                        {
                            GUI.contentColor = item.Color * GlobalColor;
                            GUI.backgroundColor = item.Value.Settings.BackgroundColor * GlobalColor;

                            _labelDummy.text = item.Value.Text;
                            GUIStyle style = item.Value.Settings.IsHasBackground ? _labelStyleWithBackground : _labelStyle;

                            style.fontSize = item.Value.IsWorldSpaceScale
                                ? Mathf.FloorToInt(item.Value.Settings.FontSize / zoom)
                                : item.Value.Settings.FontSize;

                            style.alignment = item.Value.Settings.TextAnchor;

                            _labelStyle.normal = new GUIStyleState
                            {
                                textColor = item.Color * GlobalColor,
                            };


                            if (!(HandleUtility.WorldToGUIPointWithDepth(item.Value.Position).z < 0f))
                            {
                                GUI.Label(HandleUtility.WorldPointToSizedRect(item.Value.Position, _labelDummy, _labelStyle), _labelDummy, style);
                            }
                        }
                        Handles.EndGUI();
#endif
                    }
                    private void InitStatic()
                    {
                        const int BACKGROUND_TEXTURE_WIDTH = 2;
                        const int BACKGROUND_TEXTURE_HEIGHT = 2;
                        const int BACKGROUND_TEXTURE_PIXELS_COUNT = BACKGROUND_TEXTURE_WIDTH * BACKGROUND_TEXTURE_HEIGHT;

                        if (_labelStyle == null || _labelDummy == null || _labelStyleWithBackground == null)
                        {
                            _labelStyle = new GUIStyle(GUI.skin.label)
                            {
                                richText = false,
                                padding = new RectOffset(0, 0, 0, 0),
                                margin = new RectOffset(0, 0, 0, 0)
                            };
                            _labelDummy = new GUIContent();

                            var backgroundTexturePixels = new Color32[BACKGROUND_TEXTURE_PIXELS_COUNT];
                            for (int i = 0; i < backgroundTexturePixels.Length; i++)
                            {
                                backgroundTexturePixels[i] = new Color32(255, 255, 255, 255);
                            }
                            _backgroundTexturePixels = backgroundTexturePixels;

                            var backgroundTexture = new Texture2D(BACKGROUND_TEXTURE_WIDTH, BACKGROUND_TEXTURE_HEIGHT);
                            backgroundTexture.SetPixels32(backgroundTexturePixels);
                            backgroundTexture.Apply();

                            _backgroundTexture = backgroundTexture;

                            _labelStyleWithBackground = new GUIStyle(_labelStyle);
                            _labelStyleWithBackground.normal.background = backgroundTexture;
                            _labelStyleWithBackground.active.background = backgroundTexture;
                            _labelStyleWithBackground.focused.background = backgroundTexture;
                            _labelStyleWithBackground.hover.background = backgroundTexture;
                        }
                    }
                    private static float GetCameraZoom(Camera camera)
                    {
                        const float DEFAULT_ZOOM = 1f;

                        if (camera != null)
                        {
                            return camera.orthographicSize;
                        }
                        return DEFAULT_ZOOM;

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
                }
                #endregion
            }

#endregion
        }
    }
}