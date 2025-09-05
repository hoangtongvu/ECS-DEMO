using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor.Misc
{
    public class ChildrenGOBatchAdderWindow : EditorWindow
    {
        private readonly List<GameObject> children = new();
        private readonly List<GameObject> targetParents = new();

        private Vector2 scrollPos;

        [MenuItem("Tools/Batch/Add Children To GameObjects and Prefabs")]
        public static void ShowWindow()
        {
            GetWindow<ChildrenGOBatchAdderWindow>("Children Batch Adder");
        }

        private void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            GUILayout.Label("Children to Add", EditorStyles.boldLabel);
            DrawObjectList(children);

            GUILayout.Space(10);

            GUILayout.Label("Target Parents", EditorStyles.boldLabel);
            DrawObjectList(targetParents);

            GUILayout.Space(20);

            if (GUILayout.Button("Process Parents", GUILayout.Height(30)))
            {
                ProcessParents();
            }

            EditorGUILayout.EndScrollView();
        }

        private static void DrawObjectList(List<GameObject> list)
        {
            int removeIndex = -1;

            for (int i = 0; i < list.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                list[i] = (GameObject)EditorGUILayout.ObjectField(list[i], typeof(GameObject), true);
                if (GUILayout.Button("X", GUILayout.Width(20)))
                    removeIndex = i;
                EditorGUILayout.EndHorizontal();
            }

            if (removeIndex >= 0)
                list.RemoveAt(removeIndex);

            if (GUILayout.Button("+ Add"))
                list.Add(null);

            if (GUILayout.Button("x Clear all"))
                list.Clear();
        }

        private void ProcessParents()
        {
            if (children.Count == 0 || targetParents.Count == 0)
            {
                Debug.LogWarning("Please assign at least one child prefab and one target.");
                return;
            }

            foreach (var target in targetParents)
            {
                if (target == null) continue;

                string path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(target);

                if (!string.IsNullOrEmpty(path) && AssetDatabase.LoadAssetAtPath<GameObject>(path) == target)
                {
                    // --- Prefab asset ---
                    GameObject prefabRoot = PrefabUtility.LoadPrefabContents(path);

                    foreach (var child in children)
                    {
                        if (child == null) continue;
                        if (prefabRoot.transform.Find(child.name) != null) continue;

                        GameObject instance = CloneChild(child);
                        instance.name = child.name;
                        instance.transform.SetParent(prefabRoot.transform, false);
                    }

                    PrefabUtility.SaveAsPrefabAsset(prefabRoot, path);
                    PrefabUtility.UnloadPrefabContents(prefabRoot);

                    Debug.Log($"Modified prefab asset: {path}");
                }
                else
                {
                    // --- Live scene object ---
                    foreach (var child in children)
                    {
                        if (child == null) continue;
                        if (target.transform.Find(child.name) != null) continue;

                        GameObject instance = CloneChild(child);
                        instance.name = child.name;
                        instance.transform.SetParent(target.transform, false);
                    }

                    Debug.Log($"Modified scene object: {target.name}");
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static GameObject CloneChild(GameObject child)
        {
            // If it's a prefab asset
            string childPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(child);
            if (!string.IsNullOrEmpty(childPath) && AssetDatabase.LoadAssetAtPath<GameObject>(childPath) == child)
            {
                return (GameObject)PrefabUtility.InstantiatePrefab(child);
            }

            // If it's a live scene object → just duplicate
            return Object.Instantiate(child);
        }

    }

}