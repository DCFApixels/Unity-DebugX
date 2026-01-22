#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace DCFApixels.DebugXCore.Internal
{
    internal class DebugXSettings : EditorWindow
    {
        [MenuItem("Tools/DebugX/Settings")]
        private static void Open()
        {
            DebugXSettings window = (DebugXSettings)EditorWindow.GetWindow(typeof(DebugXSettings));
            window.Show();
            //window._isHasDisableDebugXInBuildSymbols = null;
            CompilationPipeline.compilationFinished -= CompilationPipeline_compilationFinished;
            CompilationPipeline.compilationFinished += CompilationPipeline_compilationFinished;
        }

        private static void CompilationPipeline_compilationFinished(object obj)
        {
            //_isCompilation = false;
            _defines = null;
        }

        //private static bool _isCompilation;
        //private bool? _isHasDisableDebugXInBuildSymbols = false;
        //private const string DEFINE_NAME = nameof(DebugXDefines.DEBUGX_DISABLE_INBUILD);
        private static (string name, bool flag)[] _defines = null;
        private static Vector2 _pos;

        private void OnGUI()
        {
            _pos = GUILayout.BeginScrollView(_pos, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            float tmpFloat;
            int tmpInt;

            DebugX.GlobalTimeScale = EditorGUILayout.FloatField("TimeScale", DebugX.GlobalTimeScale);
            EditorGUI.BeginChangeCheck();
            tmpFloat = EditorGUILayout.Slider(DebugX.GlobalTimeScale, 0, 2);
            if (EditorGUI.EndChangeCheck())
            {
                DebugX.GlobalTimeScale = tmpFloat;
            }

            DebugX.GlobalDotSize = EditorGUILayout.FloatField("DotSize", DebugX.GlobalDotSize);
            EditorGUI.BeginChangeCheck();
            tmpFloat = EditorGUILayout.Slider(DebugX.GlobalDotSize, 0, 2);
            if (EditorGUI.EndChangeCheck())
            {
                DebugX.GlobalDotSize = tmpFloat;
            }

            DebugX.GlobalColor = EditorGUILayout.ColorField("Color", DebugX.GlobalColor);
            Color color = DebugX.GlobalColor;
            color.a = EditorGUILayout.Slider(DebugX.GlobalColor.a, 0, 1);
            DebugX.GlobalColor = color;


            DebugX.GlobalGreaterPassAlpha = EditorGUILayout.FloatField("GreaterPassAlpha", DebugX.GlobalGreaterPassAlpha);
            EditorGUI.BeginChangeCheck();
            tmpFloat = EditorGUILayout.Slider(DebugX.GlobalGreaterPassAlpha, 0, 1);
            if (EditorGUI.EndChangeCheck())
            {
                DebugX.GlobalGreaterPassAlpha = tmpFloat;
            }

            DebugX.AvailablePoolMemory = EditorGUILayout.IntField("AvailablePoolMemory (kb)", DebugX.AvailablePoolMemory);
            EditorGUI.BeginChangeCheck();


            if (GUILayout.Button("Reset"))
            {
                DebugX.ResetGlobals();
            }
            if (GUILayout.Button("Clear All Gizmos"))
            {
                DebugX.ClearAllGizmos();
            }

            GUILayout.Space(4);

            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Information", EditorStyles.helpBox);
            DrawReadonlyLeftToggle("Is SRP", DebugXConsts.IsSRP);
            DrawReadonlyLeftToggle("Support GPU Instancing", DebugXConsts.IsSupportsComputeShaders);
            DrawReadonlyLeftToggle("Support OnGizmosDraw methods", DebugXConsts.IsSRP);
            GUILayout.EndVertical();

            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Scripting Define Symbols", EditorStyles.helpBox);
            if (_defines == null)
            {
                _defines = DefinesUtility.LoadDefines(typeof(DebugXDefines));
            }
            for (int i = 0; i < _defines.Length; i++)
            {
                ref var define = ref _defines[i];
                define.flag = EditorGUILayout.ToggleLeft(define.name, define.flag);
            }
            if (GUILayout.Button("Apply Defines"))
            {
                DefinesUtility.ApplyDefines(_defines);
            }
            GUILayout.EndVertical();

            GUILayout.EndScrollView();
        }

        private void DrawReadonlyLeftToggle(string text, bool value)
        {
            GUILayout.BeginHorizontal();
            bool GUI_enabled_default = GUI.enabled;
            GUI.enabled = false;
            EditorGUILayout.Toggle(value, GUILayout.Width(14), GUILayout.ExpandWidth(false));
            GUI.enabled = GUI_enabled_default;
            EditorGUILayout.LabelField(text, GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();
        }
    }
}
#endif