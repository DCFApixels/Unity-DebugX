using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Build;

namespace DCFApixels.DebugXCore.Internal
{
    internal static class DefinesUtility
    {
        public static (string name, bool flag)[] LoadDefines(Type defineConstsType)
        {
            const BindingFlags REFL_FLAGS = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            var fields = defineConstsType.GetFields(REFL_FLAGS);
            return fields.Where(o => o.FieldType == typeof(bool)).Select(o => (o.Name, (bool)o.GetValue(null))).ToArray();

        }

        public static void ApplyDefines((string name, bool flag)[] defines)
        {
            BuildTargetGroup group = EditorUserBuildSettings.selectedBuildTargetGroup;
#if UNITY_6000_0_OR_NEWER
            string symbolsString = PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.FromBuildTargetGroup(group));
#else
            string symbolsString = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
#endif

            for (int i = 0; i < defines.Length; i++)
            {
                symbolsString = symbolsString.Replace(defines[i].name, "");
            }
            symbolsString += ";" + string.Join(';', defines.Where(o => o.flag).Select(o => o.name));
#if UNITY_6000_0_OR_NEWER
            PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.FromBuildTargetGroup(group), symbolsString);
#else
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, symbolsString);
#endif
        }
    }
}
