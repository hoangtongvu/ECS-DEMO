using UnityEditor;
using UnityEngine;
using Unity.Mathematics;

namespace Editor.Misc
{
    public class HalfInspectorWindow : EditorWindow
    {
        private ushort rawUShort;
        private float resultFloat;

        [MenuItem("Tools/Half Float Viewer")]
        public static void ShowWindow()
        {
            GetWindow<HalfInspectorWindow>("Half Float Viewer");
        }

        void OnGUI()
        {
            GUILayout.Label("Convert ushort (raw half) to float", EditorStyles.boldLabel);

            rawUShort = (ushort)EditorGUILayout.IntField("Raw ushort:", rawUShort);

            if (GUILayout.Button("Convert to float"))
            {
                resultFloat = ConvertHalfToFloat(rawUShort);
            }

            EditorGUILayout.LabelField("Float result:", resultFloat.ToString("R"));
        }

        /// <summary>
        /// Converts a raw 16-bit half value (ushort) to float using Unity's half representation.
        /// </summary>
        public static float ConvertHalfToFloat(ushort rawValue)
        {
            // This mimics the bit-level reinterpretation Unity.Mathematics does internally
            half h = default;
            h.value = rawValue;
            return h;
        }

        static class UnsafeUtility
        {
            public static ref TTo As<TFrom, TTo>(ref TFrom source)
            {
                return ref System.Runtime.CompilerServices.Unsafe.As<TFrom, TTo>(ref source);
            }
        }

    }

}
