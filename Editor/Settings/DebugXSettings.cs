#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
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
            window._isHasDisableDebugXInBuildSymbols = null;
            CompilationPipeline.compilationFinished -= CompilationPipeline_compilationFinished;
            CompilationPipeline.compilationFinished += CompilationPipeline_compilationFinished;
        }

        private static void CompilationPipeline_compilationFinished(object obj)
        {
            _isCompilation = false;
        }

        private static bool _isCompilation;
        private bool? _isHasDisableDebugXInBuildSymbols = false;
        private const string DEFINE_NAME = nameof(DebugX.DISABLE_DEBUGX_INBUILD);

        private void OnGUI()
        {
            float tmpValue;

            DebugX.GlobalTimeScale = EditorGUILayout.FloatField("TimeScale", DebugX.GlobalTimeScale);
            EditorGUI.BeginChangeCheck();
            tmpValue = EditorGUILayout.Slider(DebugX.GlobalTimeScale, 0, 2);
            if (EditorGUI.EndChangeCheck())
            {
                DebugX.GlobalTimeScale = tmpValue;
            }

            DebugX.GlobalDotSize = EditorGUILayout.FloatField("DotSize", DebugX.GlobalDotSize);
            EditorGUI.BeginChangeCheck();
            tmpValue = EditorGUILayout.Slider(DebugX.GlobalDotSize, 0, 2);
            if (EditorGUI.EndChangeCheck())
            {
                DebugX.GlobalDotSize = tmpValue;
            }

            DebugX.GlobalColor = EditorGUILayout.ColorField("Color", DebugX.GlobalColor);
            Color color = DebugX.GlobalColor;
            color.a = EditorGUILayout.Slider(DebugX.GlobalColor.a, 0, 1);
            DebugX.GlobalColor = color;


            if (_isCompilation == false)
            {
                if (_isHasDisableDebugXInBuildSymbols == null)
                {
                    BuildTargetGroup group = EditorUserBuildSettings.selectedBuildTargetGroup;
#if UNITY_6000_0_OR_NEWER
                    string symbolsString = PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.FromBuildTargetGroup(group));
#else
                    string symbolsString = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
#endif
                    _isHasDisableDebugXInBuildSymbols = symbolsString.Contains(DEFINE_NAME);
                }

                EditorGUI.BeginChangeCheck();
                _isHasDisableDebugXInBuildSymbols = !EditorGUILayout.ToggleLeft("Show Gizmos in Build", !_isHasDisableDebugXInBuildSymbols.Value);

                if (EditorGUI.EndChangeCheck())
                {
                    BuildTargetGroup group = EditorUserBuildSettings.selectedBuildTargetGroup;
#if UNITY_6000_0_OR_NEWER
                    string symbolsString = PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.FromBuildTargetGroup(group));
#else
                string symbolsString = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
#endif
                    if (symbolsString.Contains(DEFINE_NAME) == false)
                    {
                        symbolsString = symbolsString + ", " + DEFINE_NAME;
                    }
                    else
                    {
                        symbolsString = symbolsString.Replace(DEFINE_NAME, "");

                    }

#if UNITY_6000_0_OR_NEWER
                    PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.FromBuildTargetGroup(group), symbolsString);
#else
                PlayerSettings.SetScriptingDefineSymbolsForGroup(group, symbolsString);
#endif
                    _isCompilation = true;
                }
            }
            else
            {
                _isHasDisableDebugXInBuildSymbols = null;
                GUI.enabled = false;
                EditorGUILayout.ToggleLeft("Show Gizmos in Build (Locked for compilation)", false);
                GUI.enabled = true;
            }




            if (GUILayout.Button("Reset"))
            {
                DebugX.ResetGlobals();
            }

            if (GUILayout.Button("Clear All Gizmos"))
            {
                DebugX.ClearAllGizmos();
            }
        }
    }
}
#endif