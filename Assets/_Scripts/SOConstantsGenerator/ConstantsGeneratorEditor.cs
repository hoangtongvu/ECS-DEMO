using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using static SOConstantsGenerator.ConstantsGeneratorHelper;

namespace SOConstantsGenerator;

[CustomEditor(typeof(ScriptableObject), true)]
public class ConstantsGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var targetType = target.GetType();
        var generateAttr = targetType.GetCustomAttribute<GenerateConstantsForAttribute>();
        if (generateAttr == null)
            return;

        if (GUILayout.Button("Generate Constants"))
        {
            GenerateConstants(target, generateAttr);
        }
    }

    private void GenerateConstants(Object so, GenerateConstantsForAttribute generateConstantsForAttribute)
    {
        var className = generateConstantsForAttribute.ConstHolderClassName;
        var classNamespace = generateConstantsForAttribute.ConstHolderClassNamespace;
        var type = so.GetType();

        // Look for the target assembly field
        var outputFolderField = type.GetField("OutputFolder", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        string folder = null;

        if (outputFolderField != null)
        {
            var folderAsset = outputFolderField.GetValue(so) as DefaultAsset;

            if (folderAsset != null)
                folder = AssetDatabase.GetAssetPath(folderAsset);
        }

        // fallback to script directory if no asmdef is assigned
        if (string.IsNullOrEmpty(folder))
            folder = Path.GetDirectoryName(GetScriptPath(type));

        var outputPath = Path.Combine(folder, className + ".cs");

        GenerateFile(outputPath, so, className, classNamespace);

        AssetDatabase.Refresh();
        Debug.Log("Generated constants: " + outputPath);
    }

    private string GetScriptPath(System.Type type)
    {
        var script = MonoScript.FromScriptableObject((ScriptableObject)target);
        return AssetDatabase.GetAssetPath(script);
    }
}