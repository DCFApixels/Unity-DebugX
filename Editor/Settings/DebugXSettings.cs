#if UNITY_EDITOR
using UnityEditor;
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
        }

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