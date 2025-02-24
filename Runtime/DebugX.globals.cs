using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DCFApixels
{
    public static unsafe partial class DebugX
    {
        private static float _timeScaleCache;
        public static float GlobalTimeScale
        {
            get { return _timeScaleCache; }
            set
            {
                value = Mathf.Max(0f, value);
                if (_timeScaleCache == value) { return; }
                _timeScaleCache = value;
#if UNITY_EDITOR
                EditorPrefs.SetFloat(GLOBAL_TIME_SCALE_PREF_NAME, value);
#endif
            }
        }
        private static float _dotSizeCache;
        public static float GlobalDotSize
        {
            get { return _dotSizeCache; }
            set
            {
                if (_dotSizeCache == value) { return; }
                _dotSizeCache = value;
                Shader.SetGlobalFloat(GlobalDotSizePropertyID, _dotSizeCache);
#if UNITY_EDITOR
                EditorPrefs.SetFloat(GLOBAL_DOT_SIZE_PREF_NAME, _dotSizeCache);
#endif
            }
        }
        private static Color _globalColorCache;
        public static Color GlobalColor
        {
            get { return _globalColorCache; }
            set
            {
                if (_globalColorCache == value) { return; }
                _globalColorCache = value;
                Shader.SetGlobalVector(nameID: GlobalColorPropertyID, _globalColorCache);

                Color32 c32 = (Color32)value;
                var record = *(int*)&c32;
#if UNITY_EDITOR
                EditorPrefs.SetInt(GLOBAL_COLOR_PREF_NAME, record);
#endif
            }
        }
        public static void ResetGlobals()
        {
#if UNITY_EDITOR
            EditorPrefs.DeleteKey(GLOBAL_TIME_SCALE_PREF_NAME);
            EditorPrefs.DeleteKey(GLOBAL_DOT_SIZE_PREF_NAME);
            EditorPrefs.DeleteKey(GLOBAL_COLOR_PREF_NAME);
#endif
            _dotSizeCache = default;
            _timeScaleCache = default;
            _globalColorCache = default;
            InitGlobals();
        }
        private static void InitGlobals()
        {
            GlobalTimeScale = 1;
            GlobalDotSize = 1;
            GlobalColor = Color.white;
#if UNITY_EDITOR
            GlobalTimeScale = EditorPrefs.GetFloat(GLOBAL_TIME_SCALE_PREF_NAME, 1f);
            GlobalDotSize = EditorPrefs.GetFloat(GLOBAL_DOT_SIZE_PREF_NAME, 1f);
            var colorCode = EditorPrefs.GetInt(GLOBAL_COLOR_PREF_NAME, -1);
            GlobalColor = (Color)(*(Color32*)&colorCode);
#endif
        }
    }
}