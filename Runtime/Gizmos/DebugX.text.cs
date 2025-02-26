using DCFApixels.DebugXCore;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Rendering;

namespace DCFApixels {
    using IN = System.Runtime.CompilerServices.MethodImplAttribute;
    
    public static partial class DebugX {
        public readonly partial struct DrawHandler {
            #region Text

            /// <summary>
            /// Draw text at the world position.
            /// Can pass any object where ToString() will be called.
            /// </summary>
            /// <param name="position">World position.</param>
            /// <param name="text">String or any other object.</param>
            [IN(LINE)] public DrawHandler Text(Vector3 position, object text) => Gizmo(new TextGizmo(new TextBuilder(position, text)));

            /// <summary>
            /// Draw text at the world position.
            /// Can pass any object where ToString() will be called.
            /// </summary>
            /// <param name="position">World position.</param>
            /// <param name="text">String or any other object.</param>
            /// <param name="fontSize">Text font size.</param>
            [IN(LINE)] public DrawHandler Text(Vector3 position, object text, int fontSize) => Gizmo(new TextGizmo(new TextBuilder(position, text, fontSize)));
            
            /// <summary>
            /// Draw text at the world position.
            /// Can pass any object where ToString() will be called.
            /// </summary>
            /// <param name="position">World position.</param>
            /// <param name="text">String or any other object.</param>
            /// <param name="fontSize">Text font size.</param>
            /// <param name="textAnchor">Text alignment.</param>
            [IN(LINE)] public DrawHandler Text(Vector3 position, object text, int fontSize, TextAnchor textAnchor) => Gizmo(new TextGizmo(new TextBuilder(position, text, fontSize, textAnchor)));
            
            /// <summary>
            /// Use text builder to pass more details about how text-gizmo should be drawn.
            /// </summary>
            /// <param name="textBuilder">Settings with a builder pattern.</param>
            [IN(LINE)] public DrawHandler Text(TextBuilder textBuilder) => Gizmo(new TextGizmo(textBuilder));
            
            private readonly struct TextGizmo : IGizmo<TextGizmo> {
                private const int BACKGROUND_TEXTURE_WIDTH = 2;
                private const int BACKGROUND_TEXTURE_HEIGHT = 2;
                private const int BACKGROUND_TEXTURE_PIXELS_COUNT = BACKGROUND_TEXTURE_WIDTH * BACKGROUND_TEXTURE_HEIGHT;
                
                // TODO: Normally Texture2D should be destroyed when not needed anymore. Though it will live through entire app lifetime. What about editor?
                private static Texture2D _backgroundTexture;
                private static Color32[] _backgroundTexturePixels;
                public readonly TextBuilder TextBuilderInstance;
                [IN(LINE)]
                public TextGizmo(TextBuilder textBuilder) {
                    TextBuilderInstance = textBuilder;
                    CheckTextureInstance();
                }

                /// <summary>
                /// Should be called once after the app domain is cleared.
                /// </summary>
                private void CheckTextureInstance() {
                    if (_backgroundTexture != null) {
                        return;
                    }
                    
                    _backgroundTexture = new Texture2D(BACKGROUND_TEXTURE_WIDTH, BACKGROUND_TEXTURE_HEIGHT);
                    _backgroundTexturePixels = new Color32[BACKGROUND_TEXTURE_PIXELS_COUNT];
                    _backgroundTexture.SetPixels32(_backgroundTexturePixels);
                    _backgroundTexture.Apply();
                }
                
                public IGizmoRenderer<TextGizmo> RegisterNewRenderer() { return new Renderer(); }

                #region Renderer
                private class Renderer : IGizmoRenderer_UnityGizmos<TextGizmo>
                {
                    private static GUIStyle _labelStyle;
                    private static GUIContent _labelDummy;
                    public int ExecuteOrder => default(UnlitMat).GetExecuteOrder();
                    public bool IsStaticRender => false;
                    public void Prepare(Camera camera, GizmosList<TextGizmo> list) { }
                    public void Render(Camera camera, GizmosList<TextGizmo> list, CommandBuffer cb) { }
                    public void Render_UnityGizmos(Camera camera, GizmosList<TextGizmo> list)
                    {
#if UNITY_EDITOR
                        Color defaultColor = GUI.color;
                        if (_labelStyle == null || _labelDummy == null)
                        {
                            _labelStyle = GUI.skin.label;
                            _labelStyle.richText = false;
                            _labelDummy = new GUIContent();
                        }

                        if (list.Count == 0) {
                            return;
                        }

                        var zoom = GetCameraZoom();
                        
                        Handles.BeginGUI();
                        foreach (ref readonly var item in list)
                        {
                            GUI.color = item.Color * GlobalColor;
                            _labelDummy.text = item.Value.TextBuilderInstance.Text.ToString();

                            _labelStyle.fontSize = item.Value.TextBuilderInstance.UseWorldScale 
                                ? Mathf.FloorToInt(item.Value.TextBuilderInstance.FontSize / zoom) 
                                : item.Value.TextBuilderInstance.FontSize;
                            
                            _labelStyle.alignment = item.Value.TextBuilderInstance.TextAnchor;

                            _labelStyle.normal = new GUIStyleState {
                                textColor = item.Color * GlobalColor,
                            };
                            
                            if (item.Value.TextBuilderInstance.UseBackground) {
                                for (int i = 0; i < BACKGROUND_TEXTURE_PIXELS_COUNT; i++) {
                                    _backgroundTexturePixels[i] = item.Value.TextBuilderInstance.BackgroundColor;
                                }

                                _backgroundTexture.SetPixels32(_backgroundTexturePixels);
                                _backgroundTexture.Apply();

                                _labelStyle.normal.background = _backgroundTexture;
                            }
                            
                            if (!(HandleUtility.WorldToGUIPointWithDepth(item.Value.TextBuilderInstance.Position).z < 0f))
                            {
                                GUI.Label(HandleUtility.WorldPointToSizedRect(item.Value.TextBuilderInstance.Position, _labelDummy, _labelStyle), _labelDummy, _labelStyle);
                            }
                        }
                        Handles.EndGUI();
                        GUI.color = defaultColor;

                        float GetCameraZoom() {
                            const float DEFAULT_ZOOM = 1f;
                        
                            var localCamera = GetCurrentCamera();
                            if (localCamera != null) {
                                return localCamera.orthographicSize;
                            }
                            
                            var currentDrawingSceneView = SceneView.currentDrawingSceneView;

                            if (currentDrawingSceneView == null) {
                                return DEFAULT_ZOOM;
                            }
                            
                            localCamera = currentDrawingSceneView.camera;
                                
                            if (camera != null) {
                                return localCamera.orthographicSize;
                            }

                            return DEFAULT_ZOOM;
                        }
#endif
                    }
                }
                #endregion
            }
            #endregion
            
            #region TextBuilder
            /// <summary>
            /// Set text gizmos instance settings using a builder pattern. 
            /// </summary>
            public struct TextBuilder {
                private const TextAnchor DEFAULT_TEXT_ANCHOR = TextAnchor.MiddleLeft;
                private const int DEFAULT_FONT_SIZE = 16;
                
                /// <summary>
                /// Text world position.
                /// </summary>
                public Vector3 Position { get; set; }
                
                /// <summary>
                /// Text. Uses ToString() of the passed object. 
                /// </summary>
                public object Text { get; set; }

                /// <summary>
                /// Font size. Default is <see cref="DEFAULT_FONT_SIZE" />.
                /// </summary>
                public int FontSize { get; set; }
                
                /// <summary>
                /// Text alignment. Default is <see cref="DEFAULT_TEXT_ANCHOR" />.
                /// </summary>
                public TextAnchor TextAnchor { get; set; }
                
                /// <summary>
                /// Background texture color.
                /// </summary>
                public Color BackgroundColor { get; set; }
                
                /// <summary>
                /// Flag to use background texture and background color when rendering text gizmo instance.
                /// </summary>
                public bool UseBackground { get; set; }
                
                /// <summary>
                /// If set true - camera zooming will affect text scale to keep same size in the world.
                /// </summary>
                public bool UseWorldScale { get; set; }

                public TextBuilder(Vector3 position, object text, int fontSize = DEFAULT_FONT_SIZE, TextAnchor textAnchor = DEFAULT_TEXT_ANCHOR) : this() {
                    Position = position;
                    Text = text;
                    FontSize = fontSize;
                    TextAnchor = textAnchor;
                }

                public TextBuilder SetPosition(Vector3 position) {
                    Position = position;
                    return this;
                }
                    
                public TextBuilder SetText(object text) {
                    Text = text;
                    return this;
                }
                    
                public TextBuilder SetFontSize(int fontSize) {
                    FontSize = fontSize;
                    return this;
                }
                    
                public TextBuilder SetTextAnchor(TextAnchor textAnchor) {
                    TextAnchor = textAnchor;
                    return this;
                }

                public TextBuilder SetBackground(Color backgroundColor) {
                    UseBackground = true;
                    BackgroundColor = backgroundColor;
                    return this;
                }
                    
                public TextBuilder RemoveBackground() {
                    UseBackground = false;
                    return this;
                }
                
                public TextBuilder SetWorldScaling() {
                    UseWorldScale = true;
                    return this;
                }
                    
                public TextBuilder RemoveWorldScaling() {
                    UseWorldScale = false;
                    return this;
                }
            }
            #endregion
        }
    }
}