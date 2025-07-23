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
        private static float _globalGreaterPassAlphaCache;
        public static float GlobalGreaterPassAlpha
        {
            get { return _globalGreaterPassAlphaCache; }
            set
            {
                if (_globalGreaterPassAlphaCache == value) { return; }
                _globalGreaterPassAlphaCache = value;
                Shader.SetGlobalFloat(nameID: GlobalGreaterPassAlphaPropertyID, _globalGreaterPassAlphaCache);

#if UNITY_EDITOR
                EditorPrefs.SetFloat(GLOBAL_GREATER_PASS_ALPHA_PREF_NAME, _globalGreaterPassAlphaCache);
#endif
            }
        }
        public static void ResetGlobals()
        {
#if UNITY_EDITOR
            EditorPrefs.DeleteKey(GLOBAL_TIME_SCALE_PREF_NAME);
            EditorPrefs.DeleteKey(GLOBAL_DOT_SIZE_PREF_NAME);
            EditorPrefs.DeleteKey(GLOBAL_COLOR_PREF_NAME);
            EditorPrefs.DeleteKey(GLOBAL_GREATER_PASS_ALPHA_PREF_NAME);
#endif
            _timeScaleCache = default;
            _dotSizeCache = default;
            _globalColorCache = default;
            _globalGreaterPassAlphaCache = default;
            InitGlobals();
        }
        private static void InitGlobals()
        {
#if UNITY_EDITOR
            GlobalTimeScale = EditorPrefs.GetFloat(GLOBAL_TIME_SCALE_PREF_NAME, 1f);
            GlobalDotSize = EditorPrefs.GetFloat(GLOBAL_DOT_SIZE_PREF_NAME, 1f);
            var colorCode = EditorPrefs.GetInt(GLOBAL_COLOR_PREF_NAME, -1);
            GlobalColor = (Color)(*(Color32*)&colorCode);
            GlobalGreaterPassAlpha = EditorPrefs.GetFloat(GLOBAL_GREATER_PASS_ALPHA_PREF_NAME, 0.1f);
#else
            GlobalTimeScale = 1;
            GlobalDotSize = 1;
            GlobalColor = Color.white;
            GlobalGreaterPassAlpha = 0.1f;
#endif
        }
    }
}