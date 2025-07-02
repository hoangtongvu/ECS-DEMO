// ========================================================
//  UVUnifyAtlas.cs
//  Unity Texture Atlas and Mesh Combiner Tool
// ========================================================
//
//  #region Fields
//  #region Editor Window Setup
//  #region Selection Handling
//  #region OnGUI - Editor UI Drawing
//  #region Object and Folder Loading
//  #region Main Atlas Generation
//  #region Transparent and Opaque Group Processing
//  #region Mesh Combining and Prefab Creation
//  #region Material Transparency Settings
//  #region Utilities
//
// ========================================================


using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.Rendering;
using UnityEngine.Profiling;
using System;
namespace UVUnifyAtlasTool
{

    public class UVUnifyAtlas : EditorWindow
{
    public const string VERSION = "v1.0";
    #region Fields
    private bool verboseLogging = false; // Toggle to reduce spam
    private List<GameObject> selectedObjects = new List<GameObject>();
    private int atlasResolution = 4096;
    private int atlasPadding = 4;
    private Vector2 scrollPos;
    private Texture2D previewTexture;
    private string saveFolderPath = "Assets/UVUnifyAtlas/_Output";
    private Dictionary<Color32, Texture2D> fallbackColorTextureCache = new Dictionary<Color32, Texture2D>();
    private Dictionary<string, Dictionary<GameObject, Texture2D>> objectTextureMap = new Dictionary<string, Dictionary<GameObject, Texture2D>>();
    private Dictionary<string, Rect[]> uvRectsByType = new Dictionary<string, Rect[]>();
    Dictionary<string, Dictionary<int, int>> textureLookupByType = new Dictionary<string, Dictionary<int, int>>();
    private bool hasAlbedo, hasNormal, hasMetallic, hasAO, hasHeight, hasEmission;
    private enum AtlasCompression
    {
        Uncompressed,
        DXT5,
        ASTC
    }
    private enum LODQualityMode
    {
        Balanced,
        PreserveShape,
        FastSimplify
    }
    private LODQualityMode lodQualityMode = LODQualityMode.Balanced;
    private AtlasCompression selectedCompression = AtlasCompression.Uncompressed;

    private bool generatePackedMaskMap = false;
    private bool generateCombinedMesh = false;
    private bool generatePrefab = false;
    private bool showSelectedObjects = true;
    private string objectSearchFilter = "";
    private const string SIGNATURE = "WS-UV1";
    private bool generateSummaryReport = false;
    private bool organizeOutputFolders = true;
    private bool placePrefabInScene = false;
    private bool deleteOriginalsAfterAtlasing = false;
    private Vector3 lastCombinedMeshOffset = Vector3.zero;
    private bool useAlphaClipping = false;
    private float alphaCutoffValue = 0.5f; // Default value
    private bool clearAfterGeneration = false;
    private bool showCompletionDialog = true;
    private bool fbxWarningShownThisSession = false;
    private bool isGenerating = false;
    private Texture2D cachedLogo; // ✅ caches the logo after first load
    private float alphaTransparencyThreshold = 0.01f; // 1% of pixels allowed to be < 255 alpha
    private HashSet<string> forcedOpaqueObjects = new HashSet<string>();
    private Dictionary<int, int> textureInstanceIdToIndex = new Dictionary<int, int>();
    private HashSet<string> fallbackColorUsedObjects = new HashSet<string>();
    private string selectedTagToInclude = "Untagged";
    private string excludedTag = "ExcludeFromAtlas";
    private bool showTagFilters = false;
    private bool loggedThisFrame = false;
    private int lastLoggedVertCount = -1;
    private int lastLoggedUV2Verts = -1; // ✅ for UV2 estimation spam prevention
    private bool useEmojis = true;
    private bool showLODHelpboxAfterGeneration = false;
    private bool manualPackedMaskGenerated = false;

    private bool useObjectNameForPrefab = false;
    private bool includeSubfolders = true;
    private bool atlasOnlyMode = false;
    private string groupSuffix = "";
    private int minimumFinalAtlasSize = 2048;
    private readonly int[] minimumAtlasSizeOptions = new int[] { 512, 1024, 2048, 4096 };
    private readonly int[] atlasResolutionOptions = new int[] { 512, 1024, 2048, 4096, 8192 };
    private static UVUnifyAtlas activeWindow;

    private GameObject[] cachedSelection = new GameObject[0];
    private string filePrefix = "";
    private string fileSuffix = "";
    private bool generateUV2 = false;
    private bool autoMarkStaticForUV2 = true;
    private List<int> lodVertexCounts = new List<int>();
    private List<int> lodTriangleCounts = new List<int>();
    private List<float> lodErrorRates = new List<float>();
    private List<long> lodMemorySizes = new List<long>();

    private bool autoSetTransparency = true;
    private bool forceTransparentMaterial = false;
    private bool hasTransparency = false; // set during scan
    private bool showBatchOptions = true;
    private bool markCopiedPrefabsStatic = false;
    private float uvExpandMargin = 1f; // (margin in pixels, default = 1)
    private bool generateLODs = false;
    private int numberOfLODs = 3;

    private DefaultAsset folderAsset = null;
    private string tagFilter = "";
    private int layerMask = 0;
    private bool filterStaticOnly = false;
    private bool filterRenderersOnly = true;
    private bool duplicatePrefabsBeforeProcessing = true;
    private HashSet<string> transparentObjectsDetected = new HashSet<string>();
    // HDRP Packed Mask Composer UI fields
    private Texture2D hdrpMetallicTex = null;
    private Texture2D hdrpAOTexture = null;
    private Texture2D hdrpHeightTexture = null;
    private Texture2D hdrpEmissionTexture = null;
    private bool showHDRPPackedMaskComposer = true;

    #endregion

    private enum ShaderPipeline { Auto, URP, HDRP, Standard }

    private ShaderPipeline selectedPipeline = ShaderPipeline.Auto;

    readonly string[] uvSourceOptions = new[]
    {
    "_BaseColorMap", // HDRP
    "_BaseMap",      // URP
    "_MainTex"       // Standard
};

    private string uvSourceProp = "_BaseMap"; // ✅ This is what was missing

    private int selectedUVSourceIndex = 0;


    #region Editor Window Setup
    [MenuItem("Tools/UVUnify Atlas")]
    public static void ShowWindow()
    {
        var window = GetWindow<UVUnifyAtlas>("UVUnify Atlas");
        window.minSize = new Vector2(600, 800);

        if (activeWindow != null)
            Selection.selectionChanged -= activeWindow.OnSelectionChanged;

        activeWindow = window;
        Selection.selectionChanged += window.OnSelectionChanged;

        // ✅ FIX: Set default folder if unassigned
        if (string.IsNullOrEmpty(window.saveFolderPath) || window.saveFolderPath == "Assets")
        {
            string defaultOutput = "Assets/UVUnifyAtlas/_Output";
            if (!AssetDatabase.IsValidFolder(defaultOutput))
            {
                Directory.CreateDirectory(defaultOutput);
                AssetDatabase.Refresh();
            }
            window.saveFolderPath = defaultOutput;
        }

        activeWindow.EnsureTagExists("ExcludeFromAtlas");
        InitializePipelineSettings();
        window.Show();
    }


    #endregion

    private static void InitializePipelineSettings()
    {
        // Default to Auto, which will be interpreted later
        activeWindow.selectedPipeline = ShaderPipeline.Auto;
        // Handle default to Standard if no pipeline is active
        if (GraphicsSettings.currentRenderPipeline == null)
        {
            activeWindow.selectedPipeline = ShaderPipeline.Standard;
            activeWindow.uvSourceProp = "_MainTex";
        }

        activeWindow.uvSourceProp = GraphicsSettings.currentRenderPipeline == null ? "_MainTex" : "_BaseMap";
        activeWindow.selectedUVSourceIndex = System.Array.IndexOf(activeWindow.uvSourceOptions, activeWindow.uvSourceProp);
    }

    #region Selection Handling

    private void OnSelectionChanged()
    {
        cachedSelection = Selection.gameObjects != null ? (GameObject[])Selection.gameObjects.Clone() : new GameObject[0];
        Repaint();
#if UNITY_2020_3_OR_NEWER
        EditorApplication.QueuePlayerLoopUpdate();
#endif
    }


    private void SelectObjectsWithTag(string tag)
    {
        selectedObjects.Clear();

#if UNITY_2023_1_OR_NEWER
        foreach (GameObject obj in GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
#else
        foreach (GameObject obj in GameObject.FindObjectsOfType<GameObject>())
#endif
        {
            //  Must match selected tag
            if (!obj.CompareTag(tag)) continue;

            //  Skip if it's also excluded
            if (excludedTag != "Untagged" &&
    UnityEditorInternal.InternalEditorUtility.tags.Contains(excludedTag) &&
    obj.CompareTag(excludedTag))
                continue;

            var renderer = obj.GetComponent<MeshRenderer>();
            var filter = obj.GetComponent<MeshFilter>();
            if (renderer && filter && filter.sharedMesh != null)
            {
                selectedObjects.Add(obj);
            }
        }

        RefreshSelectedObjects();
        Debug.Log($"✅ Loaded {selectedObjects.Count} objects with tag '{tag}' (excluding tag '{excludedTag}').");
    }


    #endregion



    #region OnGUI - Editor UI Drawing
    private void OnGUI()
    {

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        // Ensure pipeline is resolved before drawing UI toggles
        ResolvePipelineIfNeeded();


        if (previewTexture != null && !loggedThisFrame)
        {
            Debug.Log("Preview texture exists");
            loggedThisFrame = true;
        }
        try
        {
            //UVUnify Atlas Logo
            // 🖼️ UVUnify Atlas Logo (centered)
            Texture2D logo = LoadLogoTexture(); // ✅ uses safe search


            if (logo != null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label(logo, GUILayout.Width(128), GUILayout.Height(128));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }

            //  Tool name + version (centered under logo)
            GUIStyle nameStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 14
            };
            GUILayout.Label("UVUnify Atlas", nameStyle);

            GUIStyle versionStyle = new GUIStyle(EditorStyles.miniLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Italic
            };
            GUILayout.Label(VERSION, versionStyle);


            // Spacer to separate from next section
            GUILayout.Space(10);





            GUILayout.Label("Texture Atlas Settings", EditorStyles.boldLabel);


            if (GUILayout.Button(new GUIContent("Load Selected GameObjects", "Loads all MeshRenderer objects from the currently selected GameObjects into the tool.")))
            {
                selectedObjects.Clear();

                hasAlbedo = hasNormal = hasMetallic = hasAO = hasHeight = hasEmission = false;

                foreach (GameObject root in Selection.gameObjects)
                {
                    MeshRenderer[] renderers = root.GetComponentsInChildren<MeshRenderer>(true);
                    foreach (MeshRenderer renderer in renderers)
                    {
                        if (renderer == null) continue;

                        MeshFilter filter = renderer.GetComponent<MeshFilter>();
                        if (filter == null || filter.sharedMesh == null) continue;

                        GameObject obj = renderer.gameObject;

                        //  Skip object if it matches the excluded tag (and that tag is not 'Untagged')
                        if (excludedTag != "Untagged" &&
    UnityEditorInternal.InternalEditorUtility.tags.Contains(excludedTag) &&
    obj.CompareTag(excludedTag))

                        {
                            if (verboseLogging)
                                Debug.Log($"⛔ Skipped '{obj.name}' due to excluded tag '{excludedTag}'");
                            continue;
                        }

                        //  Only add and analyze new, allowed objects
                        if (!selectedObjects.Contains(obj))
                        {
                            selectedObjects.Add(obj);

                            var mat = renderer.sharedMaterial;
                            if (mat == null) continue;

                            Texture baseMapTex = null;
                            if (mat.HasProperty("_BaseMap"))
                                baseMapTex = mat.GetTexture("_BaseMap");

                            if (baseMapTex != null)
                            {
                                hasAlbedo = true;

                                string path = AssetDatabase.GetAssetPath(baseMapTex);
                                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

                                if (importer != null)
                                {
                                    if (importer.DoesSourceTextureHaveAlpha())
                                    {
                                        if (verboseLogging)
                                            Debug.Log($"✅ Detected alpha channel in: {path}");
                                    }

                                    if (!importer.isReadable)
                                    {
                                        importer.isReadable = true;
                                        importer.SaveAndReimport();
                                    }
                                }

                                if (baseMapTex is Texture2D baseMapTex2D)
                                {
                                    Color32[] pixels = baseMapTex2D.GetPixels32();
                                    int total = pixels.Length;
                                    int transparent = pixels.Count(p => p.a < 255);
                                    float ratio = (float)transparent / total;

                                    if (ratio > alphaTransparencyThreshold)
                                    {
                                        if (verboseLogging)
                                            Debug.Log($"✅ Transparency detected in '{baseMapTex2D.name}' (alpha pixels: {transparent}/{total} = {ratio:P1})");

                                        transparentObjectsDetected.Add(obj.name);
                                        hasTransparency = true;
                                    }
                                }

                                if (IsMaterialTransparent(mat))
                                {
                                    if (verboseLogging)
                                        Debug.Log($"✅ Material '{mat.name}' is set to Transparent — flagging transparency.");
                                    hasTransparency = true;
                                    transparentObjectsDetected.Add(obj.name);
                                }
                            }

                            // Also scan other maps
                            if ((mat.HasProperty("_BaseMap") && mat.GetTexture("_BaseMap") != null) ||
                                (mat.HasProperty("_MainTex") && mat.GetTexture("_MainTex") != null)) hasAlbedo = true;
                            if (mat.HasProperty("_BumpMap") && mat.GetTexture("_BumpMap") != null) hasNormal = true;
                            if (mat.HasProperty("_MetallicGlossMap") && mat.GetTexture("_MetallicGlossMap") != null) hasMetallic = true;
                            if (mat.HasProperty("_OcclusionMap") && mat.GetTexture("_OcclusionMap") != null) hasAO = true;
                            if (mat.HasProperty("_ParallaxMap") && mat.GetTexture("_ParallaxMap") != null) hasHeight = true;
                            if (mat.HasProperty("_EmissionMap") && mat.GetTexture("_EmissionMap") != null) hasEmission = true;
                        }
                    }
                }

                //  After loading completes
                hasTransparency = selectedObjects.Any(obj => transparentObjectsDetected.Contains(obj.name));
                cachedSelection = new GameObject[0];
                OnSelectionChanged(); //  force refresh
            }


            if (GUILayout.Button(new GUIContent("Add Selected GameObjects", "Adds the currently selected GameObjects into the list without clearing existing ones.")))
            {
                int addedCount = 0;

                foreach (GameObject root in Selection.gameObjects)
                {
                    foreach (MeshRenderer renderer in root.GetComponentsInChildren<MeshRenderer>())
                    {
                        MeshFilter filter = renderer.GetComponent<MeshFilter>();
                        if (filter == null) continue;

                        GameObject obj = renderer.gameObject;

                        //  Skip object if it matches the excluded tag (and that tag is not 'Untagged')
                        if (excludedTag != "Untagged" &&
    UnityEditorInternal.InternalEditorUtility.tags.Contains(excludedTag) &&
    obj.CompareTag(excludedTag))

                        {
                            if (verboseLogging)
                                Debug.Log($"⛔ Skipped '{obj.name}' due to excluded tag '{excludedTag}'");
                            continue;
                        }

                        if (!selectedObjects.Contains(obj))
                        {
                            selectedObjects.Add(obj);
                            addedCount++;

                            //  Optional: refresh detected maps immediately
                            var mat = renderer.sharedMaterial;
                            if (mat == null) continue;

                            if ((mat.HasProperty("_BaseMap") && mat.GetTexture("_BaseMap") != null) ||
                                (mat.HasProperty("_MainTex") && mat.GetTexture("_MainTex") != null)) hasAlbedo = true;

                            if (mat.HasProperty("_BumpMap") && mat.GetTexture("_BumpMap") != null) hasNormal = true;
                            if (mat.HasProperty("_MetallicGlossMap") && mat.GetTexture("_MetallicGlossMap") != null) hasMetallic = true;
                            if (mat.HasProperty("_OcclusionMap") && mat.GetTexture("_OcclusionMap") != null) hasAO = true;
                            if (mat.HasProperty("_ParallaxMap") && mat.GetTexture("_ParallaxMap") != null) hasHeight = true;
                            if (mat.HasProperty("_EmissionMap") && mat.GetTexture("_EmissionMap") != null) hasEmission = true;

                            if (IsObjectTransparent(obj))
                            {
                                hasTransparency = true;
                                transparentObjectsDetected.Add(obj.name);
                            }
                        }
                    }
                }

                if (addedCount > 0)
                {
                    if (verboseLogging)
                        Debug.Log($"➕ Added {addedCount} new GameObject(s) to the selection.");
                    RefreshSelectedObjects();
                    Repaint();
                }
                else
                {
                    if (verboseLogging)
                        Debug.Log("➕ No new objects were added (all selected objects were already in the list or excluded).");
                }
            }



            GUI.enabled = true; //Restore GUI after this button




            if (GUILayout.Button(new GUIContent("Clear Selected Objects", "Removes all previously loaded GameObjects from the list.")))
            {
                selectedObjects.Clear();
                hasTransparency = false;
                hasAlbedo = hasNormal = hasMetallic = hasAO = hasHeight = hasEmission = false;
                // Clear HDRP Packed Mask fields
                hdrpMetallicTex = null;
                hdrpAOTexture = null;
                hdrpHeightTexture = null;
                hdrpEmissionTexture = null;
            }

            if (GUILayout.Button(new GUIContent("Refresh Selected Objects", "Revalidates selected objects and re-checks transparency, maps, etc.")))
            {
                RefreshSelectedObjects();
            }

            //Remove Last Added Object
            if (selectedObjects.Count > 0)
            {
                if (GUILayout.Button(new GUIContent("Remove Last Added Object", "Removes the most recently added GameObject from the list.")))
                {
                    GameObject removedObj = selectedObjects[selectedObjects.Count - 1];
                    selectedObjects.RemoveAt(selectedObjects.Count - 1);

                    if (removedObj != null)
                        if (verboseLogging)
                            Debug.Log($"🗑️ Removed last added object: {removedObj.name}");

                        else
                            if (verboseLogging)
                            Debug.Log($"🗑️ Removed a null object from the list.");


                    RefreshSelectedObjects(); //  Recalculate transparency after removing object
                    Repaint(); //  Refresh the list display immediately
                }
            }
            else
            {
                GUI.enabled = false;
                GUILayout.Button(new GUIContent("Remove Last Added Object", "No objects to remove."));
                GUI.enabled = true;
            }


            GUILayout.Space(10);
            GUILayout.Label("Tool Settings", EditorStyles.boldLabel);

            if (GUILayout.Button(new GUIContent("🔄 Reset All Settings to Defaults", "Restores all settings to their original default values.")))
            {
                bool confirm = EditorUtility.DisplayDialog("Reset Settings",
                    "Are you sure you want to reset all settings to their default values?",
                    "Yes, Reset", "Cancel");

                if (confirm)
                {
                    ResetAllSettingsToDefaults();
                    Debug.Log("🧼 All settings reset to defaults.");
                }
            }

            showTagFilters = EditorGUILayout.Foldout(showTagFilters, "Tag Filters", true);

            if (showTagFilters)
            {
                EditorGUILayout.BeginVertical("box");

                selectedTagToInclude = EditorGUILayout.TagField(
                    new GUIContent("Include Objects With Tag", "Load all objects in the scene with this tag."),
                    selectedTagToInclude);

                if (GUILayout.Button("Load All with Tag"))
                {
                    SelectObjectsWithTag(selectedTagToInclude);
                }

                excludedTag = EditorGUILayout.TagField(
                    new GUIContent("Exclude Objects With Tag",
                        "Objects with this tag will be skipped during loading.\n\nSelecting 'Untagged' disables exclusion."),
                    excludedTag);


                EditorGUILayout.EndVertical();
            }




            showBatchOptions = EditorGUILayout.Foldout(showBatchOptions, "Batch Selection Options", true);
            if (showBatchOptions)
            {
                EditorGUILayout.BeginVertical("box");

                // Folder selection
                EditorGUILayout.BeginHorizontal();
                folderAsset = (DefaultAsset)EditorGUILayout.ObjectField(
                    new GUIContent("Select Folder", "Choose a folder from the Project window to load all GameObjects or Prefabs from."),
                    folderAsset,
                    typeof(DefaultAsset),
                    false);
                EditorGUILayout.EndHorizontal();


                // Folder selection row (Include Subfolders + Load Button + Help ?)
                EditorGUILayout.BeginHorizontal();

                // Fixed-width box for the toggle
                EditorGUILayout.BeginVertical(GUILayout.Width(150));
                includeSubfolders = EditorGUILayout.ToggleLeft(
                    new GUIContent("Include Subfolders", "If enabled, loads all GameObjects recursively from nested folders."),
                    includeSubfolders
                );
                EditorGUILayout.EndVertical();

                // Stretch Load button
                if (GUILayout.Button(new GUIContent("Load From Folder", "Loads all GameObjects from the selected folder.\nIf 'Duplicate Prefabs Before Processing' is enabled, prefab assets will be copied and instantiated into the scene."), GUILayout.ExpandWidth(true)))
                {
                    LoadObjectsFromFolder(folderAsset);
                }

                // Fixed width Help button
                if (GUILayout.Button("?", GUILayout.Width(20)))
                {
                    EditorUtility.DisplayDialog(
                        "Load From Folder Info",
                        "This button loads GameObjects from the selected Project folder.\n\nIf 'Duplicate Prefabs Before Processing' is enabled, prefab assets in that folder will be safely duplicated before being instantiated.\n\nIf the toggle is OFF, prefab assets will be skipped entirely.",
                        "OK");
                }

                EditorGUILayout.EndHorizontal();



                GUILayout.Space(4);

                // Tag filter
                EditorGUILayout.BeginHorizontal();
                tagFilter = EditorGUILayout.TagField(new GUIContent("Select by Tag", "Choose a tag to find GameObjects in the current scene."), tagFilter);
                if (GUILayout.Button(new GUIContent("Load Objects With Tag", "Adds all scene GameObjects with the selected tag."), GUILayout.Width(180)))
                {
                    if (!string.IsNullOrEmpty(tagFilter))
                        LoadObjectsWithTag(tagFilter);
                    else
                        EditorUtility.DisplayDialog("Missing Tag", "Please select a tag before loading.", "OK");
                }
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(4);

                // Layer filter
                EditorGUILayout.BeginHorizontal();
                layerMask = EditorGUILayout.LayerField(new GUIContent("Select by Layer", "Choose a layer to find scene objects using it."), layerMask);
                if (GUILayout.Button(new GUIContent("Load Objects With Layer", "Adds all GameObjects from the scene using the selected layer."), GUILayout.Width(180)))
                {
                    LoadObjectsWithLayer(layerMask);
                }
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(4);

                // Duplicate prefabs toggle
                EditorGUILayout.BeginHorizontal();
                useObjectNameForPrefab = EditorGUILayout.ToggleLeft(
                    new GUIContent("Use Object Name for Prefab", "Uses the original object's name instead of 'CombinedMeshObject' when naming the generated prefab."),
                    useObjectNameForPrefab);
                if (GUILayout.Button("?", GUILayout.Width(20)))
                {
                    EditorUtility.DisplayDialog(
                        "Use Object Name for Prefab",
                        "If enabled, the generated prefab name will be based on the object's name instead of the default 'CombinedMeshObject'.\n\nThis helps identify prefabs more easily in complex scenes.",
                        "OK");
                }
                EditorGUILayout.EndHorizontal();



                EditorGUILayout.BeginHorizontal();
                GUI.enabled = duplicatePrefabsBeforeProcessing;
                markCopiedPrefabsStatic = EditorGUILayout.ToggleLeft(
                    new GUIContent("Mark Duplicated Prefabs as Static", "Applies the Static flag to copied prefab instances so they can be selected when 'Only Include Static Objects' is enabled."),
                    markCopiedPrefabsStatic);
                GUI.enabled = true;

                if (GUILayout.Button("?", GUILayout.Width(20)))
                {
                    EditorUtility.DisplayDialog(
                        "Mark Duplicated Prefabs as Static",
                        "This option only affects duplicated prefab instances created via 'Load From Folder'.\n\nIf 'Duplicate Prefabs Before Processing' is OFF, this toggle has no effect.",
                        "OK");
                }
                EditorGUILayout.EndHorizontal();




                GUILayout.Space(4);

                // Filtering toggles
                EditorGUILayout.BeginHorizontal();
                filterStaticOnly = EditorGUILayout.ToggleLeft(new GUIContent("Only Include Static Objects", "Only add objects marked as 'Static' in the Unity inspector."), filterStaticOnly);
                filterRenderersOnly = EditorGUILayout.ToggleLeft(new GUIContent("Exclude Non-Renderable", "Skips any objects that don’t have a MeshFilter + MeshRenderer."), filterRenderersOnly);
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(4);
                if (GUILayout.Button("Select All Static Meshes in Scene"))
                {
                    LoadAllStaticMeshesInScene();
                }

                EditorGUILayout.EndVertical();

            }

            bool anyMissingTextures = selectedObjects.Any(obj =>
            {
                if (obj == null) return false;
                var renderer = obj.GetComponent<MeshRenderer>();
                var mat = renderer ? renderer.sharedMaterial : null;
                if (mat == null) return true;

                return !(mat.HasProperty("_BaseMap") && mat.GetTexture("_BaseMap") != null) &&
                       !(mat.HasProperty("_BaseColorMap") && mat.GetTexture("_BaseColorMap") != null) &&
                       !(mat.HasProperty("_MainTex") && mat.GetTexture("_MainTex") != null) &&
                       !(mat.HasProperty("_BaseTex") && mat.GetTexture("_BaseTex") != null);
            });


            if (anyMissingTextures)
            {
                EditorGUILayout.HelpBox(
                    "Some objects are missing albedo textures, but they will now use fallback colors from their material's _BaseColor or _Color.",
                                MessageType.Warning
                );

                if (GUILayout.Button("Remove All Objects Missing Textures"))
                {
                    int removedCount = selectedObjects.RemoveAll(obj =>
                    {
                        if (obj == null) return true;
                        var renderer = obj.GetComponent<MeshRenderer>();
                        var mat = renderer ? renderer.sharedMaterial : null;
                        if (mat == null) return true;

                        bool hasAlbedo =
                            (mat.HasProperty("_BaseMap") && mat.GetTexture("_BaseMap") != null) ||
                            (mat.HasProperty("_BaseColorMap") && mat.GetTexture("_BaseColorMap") != null);

                        return !hasAlbedo;
                    });

                    if (verboseLogging)
                        Debug.Log($"🧹 Removed {removedCount} object(s) missing albedo textures.");

                    RefreshSelectedObjects(); //  Recalculate transparency and maps
                    Repaint(); //  Update UI
                }
            }

            //Filter out destroyed/null objects before processing
            selectedObjects = selectedObjects.Where(obj => obj != null).ToList();

            int totalObjects = selectedObjects.Count;
            int transparentCount = selectedObjects.Count(obj => transparentObjectsDetected.Contains(obj.name));
            int missingTextureCount = selectedObjects.Count(obj =>
            {
                var renderer = obj.GetComponent<MeshRenderer>();
                var mat = renderer ? renderer.sharedMaterial : null;
                if (mat == null) return true;

                return !(mat.HasProperty("_BaseMap") && mat.GetTexture("_BaseMap") != null) &&
                       !(mat.HasProperty("_BaseColorMap") && mat.GetTexture("_BaseColorMap") != null);
            });
            int readyCount = totalObjects - transparentCount - missingTextureCount;

            EditorGUILayout.HelpBox(
                $" Object Scan Summary:\n" +
                $"- Total Objects: {totalObjects}\n" +
                $"- Transparent: {transparentCount}\n" +
                $"- Missing Albedo: {missingTextureCount}\n" +
                $"- Ready for Atlasing: {readyCount}",
                MessageType.Info
            );





            showSelectedObjects = EditorGUILayout.Foldout(showSelectedObjects, "Selected Objects");
            if (showSelectedObjects)
            {
                objectSearchFilter = EditorGUILayout.TextField("Search", objectSearchFilter);

                GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
                boxStyle.padding = new RectOffset(10, 10, 5, 5);

                EditorGUILayout.BeginVertical(boxStyle);
                selectedObjects.RemoveAll(obj => obj == null); // 🔥 remove deleted GameObjects

                int visibleCount = 0;
                foreach (GameObject obj in selectedObjects)
                {
                    if (obj == null || !obj.name.ToLower().Contains(objectSearchFilter.ToLower()))
                        continue;

                    var renderer = obj.GetComponent<MeshRenderer>();
                    var mat = renderer ? renderer.sharedMaterial : null;

                    bool hasAlbedoTex = false;
                    if (mat != null)
                    {
                        if ((mat.HasProperty("_BaseMap") && mat.GetTexture("_BaseMap") != null) ||
                            (mat.HasProperty("_BaseColorMap") && mat.GetTexture("_BaseColorMap") != null) ||
                            (mat.HasProperty("_MainTex") && mat.GetTexture("_MainTex") != null) ||
                            (mat.HasProperty("_BaseTex") && mat.GetTexture("_BaseTex") != null))
                        {
                            hasAlbedoTex = true;
                        }
                    }


                    string label = "- " + obj.name;

                    if (fallbackColorUsedObjects.Contains(obj.name))
                        label += " (Used Fallback Color)";
                    else if (!hasAlbedoTex)
                        label += " (⚠ Missing Texture)";
                    else if (transparentObjectsDetected.Contains(obj.name))
                        label += " (Transparent)";


                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(label);

                    // Show toggle only if transparency was detected
                    if (transparentObjectsDetected.Contains(obj.name))
                    {
                        bool forcedOpaque = forcedOpaqueObjects.Contains(obj.name);
                        bool newValue = EditorGUILayout.ToggleLeft("Force Opaque", forcedOpaque, GUILayout.Width(100));

                        if (newValue)
                            forcedOpaqueObjects.Add(obj.name);
                        else
                            forcedOpaqueObjects.Remove(obj.name);
                    }
                    EditorGUILayout.EndHorizontal();

                    visibleCount++;
                }





                if (visibleCount == 0)
                    EditorGUILayout.LabelField("No matching objects.");

                EditorGUILayout.EndVertical();
            }


            if (selectedObjects.Count > 0 && hasTransparency)
            {
                bool showTransparencyHelp = EditorPrefs.GetBool("TAT_ShowTransparencyTips", false);
                showTransparencyHelp = EditorGUILayout.Foldout(showTransparencyHelp, "⚠ Transparency Detected — Tips", true);
                EditorPrefs.SetBool("TAT_ShowTransparencyTips", showTransparencyHelp);

                if (showTransparencyHelp)
                {
                    EditorGUILayout.HelpBox(
                        "Some materials use transparency (BaseMap alpha or shader setting).\n" +
                        "Auto-mode will assign Transparent mode if enabled.\n\n" +
                        " To avoid white outlines:\n" +
                        "- Use 4+ pixels of Atlas Padding.\n" +
                        "- Use 1–2 pixels of UV Expand Margin.\n\n" +
                        " Unity doesn't untoggle 'Preserve Specular Lighting' by default.\n" +
                        "Manually uncheck it if bright halos appear.",
                        MessageType.Warning
                    );
                }

                GUI.enabled = hasTransparency;
                autoSetTransparency = EditorGUILayout.ToggleLeft(
                    new GUIContent("Auto-Set Material to Transparent", "Automatically sets Surface Type = Transparent when alpha is detected."),
                    autoSetTransparency);
                GUI.enabled = true;

                if (hasTransparency)
                {
                    useAlphaClipping = EditorGUILayout.ToggleLeft(
                        new GUIContent("Use Alpha Clipping (Cutout)", "Use alpha clipping instead of full transparency for objects with binary alpha."),
                        useAlphaClipping);
                }

                EditorGUI.BeginDisabledGroup(!useAlphaClipping);
                alphaCutoffValue = EditorGUILayout.Slider(
                    new GUIContent($"Alpha Cutoff ({alphaCutoffValue:F2})", "Adjusts threshold for alpha clipping (lower = more transparent pixels kept)"),
                    alphaCutoffValue, 0.01f, 1f);
                EditorGUI.EndDisabledGroup();





            }

            if (selectedObjects.Count > 0 && hasTransparency &&
                selectedObjects.Any(obj => !transparentObjectsDetected.Contains(obj.name)))
            {
                EditorGUILayout.HelpBox(
                    "Note: Transparent and opaque objects are processed separately.\n" +
                    "They will not be combined into the same mesh or material to avoid rendering issues.",
                    MessageType.Info
                );
            }



            GUILayout.Space(10);

            //Vertex Count Display
            int totalVertexCount = 0;
            foreach (var obj in selectedObjects)
            {
                var meshFilter = obj.GetComponent<MeshFilter>();
                if (meshFilter && meshFilter.sharedMesh != null)
                    totalVertexCount += meshFilter.sharedMesh.vertexCount;
            }

            if (totalVertexCount > 65535 && generateCombinedMesh)
            {
                EditorGUILayout.HelpBox(
                    $"⚠️ The combined mesh will exceed Unity's 65,535 vertex limit.\n" +
                    $"The tool will automatically switch to a 32-bit index buffer.\n\n" +
                    $" 32-bit buffers use more memory and may impact performance on lower-end hardware.",
                    MessageType.Warning
                );
            }



            // Color feedback based on count
            bool showPerformanceSummary = EditorPrefs.GetBool("TAT_ShowPerformanceSummary", false);
            showPerformanceSummary = EditorGUILayout.Foldout(showPerformanceSummary, "Performance Summary", true);
            EditorPrefs.SetBool("TAT_ShowPerformanceSummary", showPerformanceSummary);

            if (showPerformanceSummary)
            {
                int objectCount = selectedObjects.Count;
                int avgVerts = objectCount > 0 ? totalVertexCount / objectCount : 0;

                // Extra info for average vertex count
                string avgVertsLine = $" Avg. Vertices per Object: {avgVerts:N0}";
                if (avgVerts > 50000)
                    avgVertsLine += "  ⚠️ High";

                EditorGUILayout.HelpBox(
                    $" Object Count: {objectCount}\n" +
                    $"{avgVertsLine}\n" +
                    $" Combine Meshes Enabled: {generateCombinedMesh}\n" +
                    $" LODs Enabled: {generateLODs}\n" +
                    $" URP Lightmapping UV2: {(generateUV2 ? "Yes" : "No")}\n\n" +
                    $" Unity mesh limit: ~65,535 verts per mesh (some platforms lower).\n" +
                    $" Tip: Keep total under 150k for best responsiveness.",
                    MessageType.Info
                );

                if (generateLODs && lodVertexCounts.Count > 0 && showLODHelpboxAfterGeneration)
                {
                    GUILayout.Space(4);
                    GUILayout.Label("🔻 LOD Vertex Breakdown", EditorStyles.boldLabel);

                    int lod0Verts = lodVertexCounts[0];

                    for (int i = 0; i < lodVertexCounts.Count; i++)
                    {
                        int vCount = lodVertexCounts[i];
                        float percent = lod0Verts > 0 ? (vCount / (float)lod0Verts) * 100f : 0f;

                        Color original = GUI.color;

                        if (i == 0)
                            GUI.color = Color.red;
                        else if (percent > 85f)
                            GUI.color = Color.red;
                        else if (percent > 50f)
                            GUI.color = Color.yellow;
                        else
                            GUI.color = Color.green;

                        EditorGUILayout.LabelField($"LOD{i} Vertices:", $"{vCount:N0} ({percent:F1}%) of LOD0");

                        GUI.color = original;
                    }

                    if (lodVertexCounts.Count >= 3)
                    {
                        float lod1Pct = lodVertexCounts[1] / (float)lodVertexCounts[0] * 100f;
                        float lod2Pct = lodVertexCounts[2] / (float)lodVertexCounts[0] * 100f;

                        if (lod1Pct > 90f && lod2Pct > 85f)
                        {
                            EditorGUILayout.HelpBox(
                                $" LODs are close in complexity to LOD0:\n" +
                                $"- LOD1: {lod1Pct:F1}%\n" +
                                $"- LOD2: {lod2Pct:F1}%\n" +
                                $"Simplification may help performance, but this is fine if quality is preserved.",
                                MessageType.Info
                            );
                        }
                    }
                }
            }




            if (lodVertexCounts.Count == 0) // Only show if LODs haven’t been generated yet
            {
                Color originalColorVC = GUI.color;
                if (totalVertexCount < 50000)
                    GUI.color = Color.green;
                else if (totalVertexCount < 150000)
                    GUI.color = Color.yellow;
                else
                    GUI.color = Color.red;

                EditorGUILayout.BeginHorizontal();

                GUIContent vertexLabel = new GUIContent(
                    $"Total Vertex Count: {totalVertexCount:N0}",
                    "Unity recommends keeping mesh vertex counts below 65,535 per mesh.\nHigh counts may impact performance during atlasing, combining, or LOD generation."
                );
                EditorGUILayout.LabelField(vertexLabel);

                EditorGUILayout.EndHorizontal();
                GUI.color = originalColorVC;
            }



            GUILayout.Label("Detected Maps to Generate:", EditorStyles.boldLabel);
            if (selectedObjects.Count > 0)
            {
                EditorGUILayout.LabelField("Albedo: " + (hasAlbedo ? "Yes" : "No"));
                EditorGUILayout.LabelField("Normal: " + (hasNormal ? "Yes" : "No"));
                EditorGUILayout.LabelField("Metallic: " + (hasMetallic ? "Yes" : "No"));
                EditorGUILayout.LabelField("Ambient Occlusion: " + (hasAO ? "Yes" : "No"));
                EditorGUILayout.LabelField("Height: " + (hasHeight ? "Yes" : "No"));
                EditorGUILayout.LabelField("Emission: " + (hasEmission ? "Yes" : "No"));
                EditorGUILayout.LabelField("Transparency: " + (hasTransparency ? "Yes" : "No"));
            }
            else
            {
                EditorGUILayout.LabelField("No objects selected.");
            }

            GUILayout.Space(10);
            GUILayout.Label("Generation Options", EditorStyles.boldLabel);

            //Scan selected objects for feature availability
            bool canGeneratePackedMask = selectedObjects.Any(obj =>
            {
                var renderer = obj.GetComponentInChildren<MeshRenderer>();
                if (renderer == null) return false;

                var mat = renderer.sharedMaterial;
                if (mat == null) return false;

                return (mat.HasProperty("_MetallicGlossMap") && mat.GetTexture("_MetallicGlossMap") != null) ||
                       (mat.HasProperty("_OcclusionMap") && mat.GetTexture("_OcclusionMap") != null) ||
                       (mat.HasProperty("_ParallaxMap") && mat.GetTexture("_ParallaxMap") != null) ||
                       (mat.HasProperty("_EmissionMap") && mat.GetTexture("_EmissionMap") != null) ||
                       (mat.HasProperty("_MaskMap") && mat.GetTexture("_MaskMap") != null);
            });


            clearAfterGeneration = EditorGUILayout.ToggleLeft(
                new GUIContent("Clear After Generation", "If enabled, the tool will automatically clear the selected objects list after generation."),
                clearAfterGeneration
            );

            atlasOnlyMode = EditorGUILayout.ToggleLeft(
                new GUIContent("Atlas-Only Mode", "Only generate texture atlases and material — no mesh combining, prefab creation, or scene object updates."),
                atlasOnlyMode
            );

            if (clearAfterGeneration)
            {
                showCompletionDialog = EditorGUILayout.ToggleLeft(
                    new GUIContent("Show Confirmation Dialog", "Displays a message after clearing the selected objects."),
                    showCompletionDialog
                );
            }


            bool hasValidMeshFilters = selectedObjects.Any(obj => obj && obj.GetComponentInChildren<MeshFilter>() != null);
            bool hasAtLeastOneObject = selectedObjects.Count >= 1;


            // ==============================
            // 🛡️ Draw Safe Dynamic Toggles
            // ==============================

            // Packed Mask Map toggle
            EditorGUILayout.BeginHorizontal();

            bool packedMaskSupported = selectedPipeline == ShaderPipeline.HDRP || selectedPipeline == ShaderPipeline.URP;

            if (!packedMaskSupported)
            {
                GUI.enabled = false;
                EditorGUILayout.ToggleLeft(
                    new GUIContent("Packed Mask Map", "❌ Not supported in Standard Shader. Only R (Metallic) and A (Smoothness) are used. AO must be separate."),
                    false);
                GUI.enabled = true;
                generatePackedMaskMap = false;
            }
            else if (canGeneratePackedMask)
            {
                generatePackedMaskMap = EditorGUILayout.ToggleLeft(
                    new GUIContent("Packed Mask Map", "Combines Metallic (R), AO (G), Height (B), and Emission (A) into one RGBA texture."),
                    generatePackedMaskMap
                );
            }
            else
            {
                GUI.enabled = false;
                EditorGUILayout.ToggleLeft(
                    new GUIContent("Packed Mask Map", "No valid maps found. Requires at least one of: Metallic, AO, Height, or Emission."),
                    false);
                GUI.enabled = true;
                generatePackedMaskMap = false;
            }

            if (GUILayout.Button("?", GUILayout.Width(20)))
            {
                EditorUtility.DisplayDialog("Packed Mask Map",
                    "Creates a single RGBA texture:\n\n" +
                    "• R: Metallic\n" +
                    "• G: Ambient Occlusion\n" +
                    "• B: Height\n" +
                    "• A: Smoothness or Emission\n\n" +
                    " In Standard Shader:\n" +
                    "• Only R (Metallic) and A (Smoothness) are used.\n" +
                    "• AO is applied as a separate texture.\n" +
                    "• Height/Emission are ignored.",
                    "OK");
            }

            EditorGUILayout.EndHorizontal();
            // ─────────────────────────────
            // 🎛️ HDRP Packed Mask Composer Foldout
            // ─────────────────────────────
            if (selectedPipeline == ShaderPipeline.HDRP)
            {
                showHDRPPackedMaskComposer = EditorGUILayout.Foldout(showHDRPPackedMaskComposer, "🎛️ HDRP Packed Mask Composer", true);
                if (showHDRPPackedMaskComposer)
                {
                    EditorGUILayout.BeginVertical("box");

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("Manual Packed Mask Composer", EditorStyles.boldLabel);
                    if (GUILayout.Button("?", GUILayout.Width(20)))
                    {
                        EditorUtility.DisplayDialog("HDRP Packed Mask Composer",
                            "Creates an HDRP-compatible _MaskMap with the following packing:\n\n" +
                            "• R = Metallic\n" +
                            "• G = Ambient Occlusion\n" +
                            "• B = Height / Detail Mask\n" +
                            "• A = Smoothness (from Metallic alpha or fallback)\n\n" +
                            "You may leave any input empty — sensible defaults will be used.",
                            "OK");
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.HelpBox(
                        "Combine separate texture maps into an HDRP-compatible _MaskMap.\n" +
                        "Channel packing:\nR = Metallic, G = AO, B = DetailMask/Height, A = Smoothness (from Metallic alpha)",
                        MessageType.Info
                    );

                    hdrpMetallicTex = (Texture2D)EditorGUILayout.ObjectField(
                        new GUIContent("Metallic (R)", "Grayscale texture to use for the R channel"),
                        hdrpMetallicTex, typeof(Texture2D), false);

                    hdrpAOTexture = (Texture2D)EditorGUILayout.ObjectField(
                        new GUIContent("Ambient Occlusion (G)", "Grayscale AO texture for the G channel"),
                        hdrpAOTexture, typeof(Texture2D), false);

                    hdrpHeightTexture = (Texture2D)EditorGUILayout.ObjectField(
                        new GUIContent("Height / Detail Mask (B)", "Height or Detail Mask texture for B channel"),
                        hdrpHeightTexture, typeof(Texture2D), false);

                    hdrpEmissionTexture = (Texture2D)EditorGUILayout.ObjectField(
                        new GUIContent("Smoothness Source (A)", "Alpha channel from this texture will be used for Smoothness"),
                        hdrpEmissionTexture, typeof(Texture2D), false);

                    bool anyInputSet = hdrpMetallicTex || hdrpAOTexture || hdrpHeightTexture || hdrpEmissionTexture;

                    if (!anyInputSet)
                    {
                        EditorGUILayout.HelpBox("Assign at least one texture input before generating a Packed Mask.", MessageType.Warning);
                        GUI.enabled = false;
                    }

                    if (GUILayout.Button(new GUIContent("Generate HDRP Packed Mask", "Creates an RGBA packed texture using the selected inputs")))
                    {
                        Texture2D packed = GeneratePackedMaskMap(hdrpMetallicTex, hdrpAOTexture, hdrpHeightTexture, hdrpEmissionTexture);
                        if (packed != null)
                        {
                            manualPackedMaskGenerated = true;
                            string folder = Path.Combine(saveFolderPath, "PackedMasks");
                            Directory.CreateDirectory(folder);
                            string path = GetUniqueFilePath(folder, "HDRP_PackedMask", ".png");
                            File.WriteAllBytes(path, packed.EncodeToPNG());
                            AssetDatabase.ImportAsset(path);

                            string relPath = path.Substring(path.IndexOf("Assets"));
                            TextureImporter importer = AssetImporter.GetAtPath(relPath) as TextureImporter;
                            if (importer != null)
                            {
                                importer.textureType = TextureImporterType.Default;
                                importer.alphaSource = TextureImporterAlphaSource.FromInput;
                                importer.alphaIsTransparency = false;
                                importer.textureCompression = TextureImporterCompression.CompressedHQ;
                                importer.sRGBTexture = false;
                                importer.isReadable = false;
                                importer.SaveAndReimport();
                            }

                            Debug.Log(" HDRP Packed Mask saved to: " + path);
                        }
                    }

                    GUI.enabled = true;
                    EditorGUILayout.EndVertical();
                }
            }





            // Combine Meshes toggle
            EditorGUILayout.BeginHorizontal();

            if (selectedObjects.Count >= 1)
            {
                generateCombinedMesh = EditorGUILayout.ToggleLeft(
                    new GUIContent("Combine Meshes into One", "Merges one or more selected meshes into a single mesh asset. Required for LODs."),
                    generateCombinedMesh
                );
            }
            else
            {
                GUI.enabled = false;
                EditorGUILayout.ToggleLeft(
                    new GUIContent("Combine Meshes into One", "Requires at least one GameObject to be selected."),
                    false);
                GUI.enabled = true;
                generateCombinedMesh = false;
            }


            if (GUILayout.Button("?", GUILayout.Width(20)))
            {
                EditorUtility.DisplayDialog(
                    "Combine Meshes",
                    "Combines selected meshes into one to reduce draw calls.\n\nRequires more than one object selected.",
                    "OK"
                );
            }

            EditorGUILayout.EndHorizontal();


            // Generate Prefab toggle (depends on Combine Meshes)
            EditorGUILayout.BeginHorizontal();

            if (generateCombinedMesh)
            {
                generatePrefab = EditorGUILayout.ToggleLeft(
                    new GUIContent("Generate Prefab", "Saves the combined mesh as a prefab (requires Combine Meshes)."),
                    generatePrefab
                );
            }
            else
            {
                GUI.enabled = false;
                EditorGUILayout.ToggleLeft(
                    new GUIContent("Generate Prefab", "Requires 'Combine Meshes into One' to be enabled."),
                    false);
                GUI.enabled = true;
                generatePrefab = false; //  internal safety
            }

            if (GUILayout.Button("?", GUILayout.Width(20)))
            {
                EditorUtility.DisplayDialog(
                    "Generate Prefab",
                    "Saves the combined object as a prefab asset in your project.\n\nRequires 'Combine Meshes into One' to be enabled.",
                    "OK"
                );
            }
            EditorGUILayout.EndHorizontal();



            // Place Prefab in Scene toggle (depends on Prefab generation)
            EditorGUILayout.BeginHorizontal();

            if (generateCombinedMesh && generatePrefab)
            {
                placePrefabInScene = EditorGUILayout.ToggleLeft(
                    new GUIContent("Place Prefab in Scene", "Automatically places the generated prefab into the scene after saving."),
                    placePrefabInScene
                );
            }
            else
            {
                GUI.enabled = false;
                EditorGUILayout.ToggleLeft(
                    new GUIContent("Place Prefab in Scene", "Requires both 'Combine Meshes' and 'Generate Prefab' to be enabled."),
                    false);
                GUI.enabled = true;
                placePrefabInScene = false; //  internal safety
            }

            if (GUILayout.Button("?", GUILayout.Width(20)))
            {
                EditorUtility.DisplayDialog(
                    "Place Prefab in Scene",
                    "Automatically places the generated prefab instance into the scene.\n\nRequires both 'Combine Meshes' and 'Generate Prefab' to be enabled.",
                    "OK"
                );
            }
            EditorGUILayout.EndHorizontal();



            // Delete Original Objects After Atlasing
            EditorGUILayout.BeginHorizontal();

            bool canDeleteOriginals = !atlasOnlyMode && selectedObjects.Count > 0 && !generateLODs;

            if (canDeleteOriginals)
            {
                deleteOriginalsAfterAtlasing = EditorGUILayout.ToggleLeft(
                    new GUIContent(
                        "Delete Original Objects After Atlasing",
                        "Deletes only scene instances after atlased copies are created.\n\nAssets in the Project folder are never modified."
                    ),
                    deleteOriginalsAfterAtlasing
                );
            }
            else
            {
                GUI.enabled = false;
                EditorGUILayout.ToggleLeft(
                    new GUIContent(
                        "Delete Original Objects After Atlasing",
                        "This option is only available when atlased scene copies are created directly. It's not allowed in Atlas-Only mode or with LODs."
                    ),
                    false);
                GUI.enabled = true;
                deleteOriginalsAfterAtlasing = false;
            }

            if (GUILayout.Button("?", GUILayout.Width(20)))
            {
                EditorUtility.DisplayDialog(
                    "Delete Original Objects After Atlasing",
                    "If enabled, the tool will automatically delete the original GameObjects in the scene after creating their Atlased copies.\n\n" +
                    "This helps clean up your scene after optimization.\n\n" +
                    "⚠️ Important:\n" +
                    "- Only scene instances are deleted.\n" +
                    "- Assets such as meshes, materials, and textures in the Project folder are never modified.\n" +
                    "- Deletion may not always be undoable if asset generation occurs immediately after.\n" +
                    "- Please backup your scene before enabling this option.\n\n" +
                    "Proceed with caution!",
                    "OK"
                );
            }

            EditorGUILayout.EndHorizontal();

            // Check if all objects are transparent
            bool allTransparent = selectedObjects.All(obj => transparentObjectsDetected.Contains(obj.name));

            //  Only show UV2 toggle section if pipeline is resolved to URP
            if (selectedPipeline == ShaderPipeline.URP)
            {
                bool canGenerateUV2 =
                    selectedObjects.Count >= 1 &&
                    generateCombinedMesh &&
                    hasValidMeshFilters &&
                    !hasTransparency;

                //  Disable UV2 if not valid
                if (!canGenerateUV2 && generateUV2)
                    generateUV2 = false;

                bool allObjectsHaveUV2 = selectedObjects.All(obj =>
                {
                    var mf = obj.GetComponent<MeshFilter>();
                    return mf != null && mf.sharedMesh != null &&
                           mf.sharedMesh.uv2 != null &&
                           mf.sharedMesh.uv2.Length == mf.sharedMesh.vertexCount;
                });

                //  Draw toggle for UV2 generation
                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginDisabledGroup(!canGenerateUV2);
                generateUV2 = EditorGUILayout.ToggleLeft(
                    new GUIContent("Generate UV2 for Lightmapping",
                    "Creates UV2 (TexCoord1) on combined mesh for baked lighting in URP.\n" +
                    "Only supported with opaque meshes and Combine Meshes enabled."),
                    generateUV2
                );
                EditorGUI.EndDisabledGroup();

                if (GUILayout.Button("?", GUILayout.Width(20)))
                {
                    EditorUtility.DisplayDialog(
                        "Generate UV2 for Lightmapping",
                        "UV2 is required for static lightmapping in URP.\n\n" +
                        "✔ When enabled:\n" +
                        "- Creates a secondary UV channel (TexCoord1)\n" +
                        "- Marks mesh as 'Contribute GI' (if Static)\n\n" +
                        "⚠ Only use with opaque, combined, static geometry.",
                        "OK"
                    );
                }
                EditorGUILayout.EndHorizontal();

                // Additional helpful context only when enabled
                if (generateUV2)
                {
                    bool allStatic = selectedObjects.All(obj =>
                        GameObjectUtility.AreStaticEditorFlagsSet(obj, StaticEditorFlags.ContributeGI));

                    if (!allStatic)
                    {
                        EditorGUILayout.HelpBox(
                            "⚠️ Some objects are not marked as Static.\n" +
                            "Lightmapping requires 'Contribute GI' to be enabled.",
                            MessageType.Warning
                        );
                    }

                    if (allObjectsHaveUV2)
                    {
                        EditorGUILayout.HelpBox(
                            "ℹ️ All selected meshes already contain UV2.\n" +
                            "These will be overwritten on the combined mesh.",
                            MessageType.Info
                        );
                    }
                }
            }



            // Generate Summary Report toggle + ?
            EditorGUILayout.BeginHorizontal();

            bool canGenerateReport = selectedObjects.Count > 0 && hasValidMeshFilters;

            if (canGenerateReport)
            {
                generateSummaryReport = EditorGUILayout.ToggleLeft(
                    new GUIContent("Generate Summary Report", "Outputs a readable .txt summary file listing the detected maps, settings, and save location."),
                    generateSummaryReport
                );
            }
            else
            {
                GUI.enabled = false;
                EditorGUILayout.ToggleLeft(
                    new GUIContent("Generate Summary Report", "Must have at least one valid object loaded."),
                    false);
                GUI.enabled = true;
                generateSummaryReport = false; //  safely reset internally
            }

            if (GUILayout.Button("?", GUILayout.Width(20)))
            {
                EditorUtility.DisplayDialog(
                    "Generate Summary Report",
                    "Outputs a readable .txt summary file listing:\n• Which maps were detected\n• Resolution and padding\n• Shader settings\n• Final save folder\n\nSaved as 'AtlasSummary.txt' in the output folder.",
                    "OK"
                );
            }

            EditorGUILayout.EndHorizontal();



            // Organize Output Folders toggle + ?
            EditorGUILayout.BeginHorizontal();
            organizeOutputFolders = EditorGUILayout.ToggleLeft(
                new GUIContent("Organize Output Folders", "Saves outputs into structured subfolders like Atlases/, Meshes/, Prefabs/, Materials/, and Reports/"),
                organizeOutputFolders
            );
            if (GUILayout.Button("?", GUILayout.Width(20)))
            {
                EditorUtility.DisplayDialog("Organize Output Folders", "If enabled, the tool will automatically group outputs into structured subfolders such as:\n• Atlases\n• Meshes\n• Materials\n• Prefabs\n• Reports", "OK");
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.Label("Atlas Generation Settings", EditorStyles.boldLabel);
            verboseLogging = EditorGUILayout.ToggleLeft(
        new GUIContent("Verbose Logging", "If enabled, shows detailed log output in the Console."),
        verboseLogging
    );

            useEmojis = EditorGUILayout.ToggleLeft(
                new GUIContent("Use Emojis in Logs", "If enabled, messages will include emoji icons for visual clarity."),
                useEmojis
            );


            EditorGUILayout.Space(4);

            // Atlas Resolution
            EditorGUILayout.BeginHorizontal();
            int atlasResolutionIndex = System.Array.IndexOf(atlasResolutionOptions, atlasResolution);
            atlasResolutionIndex = EditorGUILayout.Popup(
                new GUIContent("Atlas Resolution", "Sets the width and height (in pixels) of the generated texture atlases. Higher values preserve more texture detail but use more memory."),
                atlasResolutionIndex,
                atlasResolutionOptions.Select(x => x.ToString()).ToArray()
            );
            atlasResolution = atlasResolutionOptions[Mathf.Clamp(atlasResolutionIndex, 0, atlasResolutionOptions.Length - 1)];
            if (GUILayout.Button("?", GUILayout.Width(20)))
            {
                EditorUtility.DisplayDialog("Atlas Resolution", "Sets the width and height (in pixels) of the generated texture atlases.\n\nHigher values preserve more texture detail but use more memory and GPU bandwidth.", "OK");
            }
            EditorGUILayout.EndHorizontal();

            // Minimum Final Atlas Size
            EditorGUILayout.BeginHorizontal();
            int selectedIndex = System.Array.IndexOf(minimumAtlasSizeOptions, minimumFinalAtlasSize);
            selectedIndex = EditorGUILayout.Popup(
                new GUIContent("Minimum Final Atlas Size", "Smallest size allowed for generated texture atlases. Prevents shrinking too small and losing quality."),
                selectedIndex,
                minimumAtlasSizeOptions.Select(x => x.ToString()).ToArray()
            );
            minimumFinalAtlasSize = minimumAtlasSizeOptions[Mathf.Clamp(selectedIndex, 0, minimumAtlasSizeOptions.Length - 1)];
            if (GUILayout.Button("?", GUILayout.Width(20)))
            {
                EditorUtility.DisplayDialog(
                    "Minimum Final Atlas Size",
                    "Sets the smallest allowed size for the generated texture atlas.\n\nFor example, 2048 means atlas will not shrink below 2048x2048 even if technically smaller would fit.\n\nPrevents blurry textures caused by over-shrinking.",
                    "OK");
            }
            EditorGUILayout.EndHorizontal();

            // Generate LODs toggle + Number of LOD Levels
            GUILayout.Space(10);
            GUILayout.Label("LOD Generation Settings", EditorStyles.boldLabel);

            // 🛡️ Safety check: only allow LODs if ONE object selected 
            if (selectedObjects.Count != 1)
            {
                generateLODs = false;
                GUI.enabled = false;
                EditorGUILayout.ToggleLeft(
                    new GUIContent("Generate LODs", "LOD generation requires exactly one object to be selected.\n\nThis can be either an original mesh or one combined by this tool."),
                    false
                );
                GUI.enabled = true;

                EditorGUILayout.HelpBox(
                    "LOD generation is only supported when exactly one object is selected.\n\nThe mesh can be original or combined.\nLOD quality depends on mesh topology and optimization.",
                    MessageType.Info
                );
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                generateLODs = EditorGUILayout.ToggleLeft(
                    new GUIContent("Generate LODs",
                        "Generates simplified mesh levels (LOD1–LOD3).\n\nSupported for any single mesh object — either an original or a combined mesh.\nLOD quality depends on mesh complexity and topology."),
                    generateLODs
                );

                if (GUILayout.Button("?", GUILayout.Width(20)))
                {
                    EditorUtility.DisplayDialog(
                        "LOD Generation Info",
                        "LOD Generation creates simplified versions (LOD1, LOD2, etc.) of a mesh to improve performance at a distance.\n\nThis feature works with any single selected object that has a MeshFilter, whether it's a raw model or a combined mesh.\n\n⚠ Results may vary based on the mesh's complexity, geometry, and UVs.",
                        "OK"
                    );
                }
                EditorGUILayout.EndHorizontal();




                if (generateLODs)
                {
                    numberOfLODs = EditorGUILayout.IntSlider(
                        new GUIContent("LOD Levels (Incl. LOD0)", "Total number of LOD levels. Includes the original mesh (LOD0)."),
                        numberOfLODs,
                        2,
                        4
                    );

                    if (numberOfLODs == 4)
                    {
                        EditorGUILayout.HelpBox(
                            "⚠️ 4 LOD levels selected. Usually 2–3 LODs are sufficient.\n" +
                            "Use 4 only for very large objects or open-world assets.",
                            MessageType.Warning
                        );
                    }
                }
            }

            lodQualityMode = (LODQualityMode)EditorGUILayout.EnumPopup(
    new GUIContent("LOD Quality Mode", "Controls how aggressively mesh simplification collapses edges:\n• Balanced: A mix of vertex, UV, and normal cost\n• Preserve Shape: Strong bias toward visual fidelity\n• Fast Simplify: Prioritizes speed and decimation rate"),
    lodQualityMode
);



            GUILayout.Space(10);


            // Atlas Compression Setting
            EditorGUILayout.BeginHorizontal();
            selectedCompression = (AtlasCompression)EditorGUILayout.EnumPopup(
                new GUIContent("Atlas Compression", "Choose texture compression for the generated atlases.\n\nUncompressed = Highest quality\nDXT5 = Smaller files for PC\nASTC = Best for Mobile VR."),
                selectedCompression
            );
            if (GUILayout.Button("?", GUILayout.Width(20)))
            {
                EditorUtility.DisplayDialog(
                    "Atlas Compression",
                    "Select the compression type for texture atlases:\n\n" +
                    "• Uncompressed (Best quality)\n" +
                    "• DXT5 (Smaller size, PC friendly)\n" +
                    "• ASTC (Advanced compression, mobile/VR friendly)\n\n" +
                    "Compression can reduce texture size drastically at small quality cost.",
                    "OK"
                );
            }
            EditorGUILayout.EndHorizontal();


            // Atlas Padding
            EditorGUILayout.BeginHorizontal();
            atlasPadding = EditorGUILayout.IntField(
                new GUIContent("Atlas Padding", "Adds spacing (in pixels) between packed textures to prevent bleeding artifacts during mipmapping.\r\n\r\nFor transparent textures (e.g., leaves, plants), recommend using 4–8 pixels of padding to minimize white or gray outlines caused by UV bleed.\r\n"),
                atlasPadding
            );
            if (GUILayout.Button("?", GUILayout.Width(20)))
            {
                EditorUtility.DisplayDialog("Atlas Padding", "Adds spacing (in pixels) between packed textures to prevent bleeding artifacts caused by mipmapping or bilinear filtering.", "OK");
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(4);
            // UV Expand Margin
            EditorGUILayout.BeginHorizontal();
            uvExpandMargin = EditorGUILayout.Slider(
                new GUIContent("UV Expand Margin (px)", "How much to shrink UV islands inward to prevent texture bleeding.\nRecommended: 1 pixel."),
                uvExpandMargin,
                0f,
                4f
            );
            if (GUILayout.Button("?", GUILayout.Width(20)))
            {
                EditorUtility.DisplayDialog(
                    "UV Expand Margin",
                    "Shrinks UV islands slightly inward to prevent color bleeding between textures.\r\n\r\nFor transparent objects (like leaves or decals), recommend setting this to 1–2 pixels to avoid halo artifacts at edges.\r\n.",
                    "OK"
                );
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(4);

            // Shader Pipeline
            EditorGUILayout.BeginHorizontal();

            // Draw the Shader Pipeline dropdown
            selectedPipeline = (ShaderPipeline)EditorGUILayout.EnumPopup(
                new GUIContent("Shader Pipeline", "Choose which shader type to generate the material for. Auto will detect the current render pipeline."),
                selectedPipeline

            );
            var fallbackProps = new[] { "_BaseMap", "_MainTex", "_BaseColorMap" };





            //  Add "Auto-detected" label if Auto is selected
            if (selectedPipeline == ShaderPipeline.Auto)
            {
                GUILayout.Label("(Auto-detected)", EditorStyles.miniLabel, GUILayout.Width(100));
            }

            // Help button
            if (GUILayout.Button("?", GUILayout.Width(20)))
            {
                EditorUtility.DisplayDialog("Shader Pipeline", "Choose the shader compatibility:\n\n• Auto: Automatically detect the current render pipeline\n• URP / HDRP / Standard: Force a specific material output", "OK");
            }

            EditorGUILayout.EndHorizontal();


            // UV Source Map
            EditorGUILayout.BeginHorizontal();
            selectedUVSourceIndex = System.Array.IndexOf(uvSourceOptions, uvSourceProp);
            selectedUVSourceIndex = EditorGUILayout.Popup(
                new GUIContent("UV Source Map", "Which texture's UV channel to use when baking the new mesh UVs."),
                selectedUVSourceIndex,
                uvSourceOptions
            );
            if (selectedUVSourceIndex >= 0 && selectedUVSourceIndex < uvSourceOptions.Length)
            {
                uvSourceProp = uvSourceOptions[selectedUVSourceIndex];
            }
            else
            {
                if (verboseLogging)
                    Debug.LogWarning("⚠️ Invalid UV source index — falling back to default (_BaseMap).");

                uvSourceProp = "_BaseMap"; // or another safe default
            }
            if (GUILayout.Button("?", GUILayout.Width(20)))
            {
                EditorUtility.DisplayDialog("UV Source Map", "Select which texture property to use for UV remapping during atlas generation.\n\nThis is helpful when different textures (like AO) use different UV sets.", "OK");
            }
            EditorGUILayout.EndHorizontal();

            //Warning if Minimum Atlas Size > Atlas Resolution
            if (minimumFinalAtlasSize > atlasResolution)
            {
                EditorGUILayout.HelpBox(
                    "⚠️ Warning: Minimum Final Atlas Size is larger than Atlas Resolution. This may cause unexpected behavior or prevent shrinking.",
                    MessageType.Warning
                );
            }
            GUILayout.Space(10);





            GUILayout.Space(10);
            GUILayout.Label("File Naming Options", EditorStyles.boldLabel);

            filePrefix = SanitizeString(EditorGUILayout.TextField(
          new GUIContent("File Prefix", "Optional text to add BEFORE generated file names.\n\nExample:\nPrefix 'Env_' ➔ Output 'Env_AlbedoAtlas_Transparent.png'"),
          filePrefix
      ));

            fileSuffix = SanitizeString(EditorGUILayout.TextField(
                new GUIContent("File Suffix", "Optional text to add AFTER the core name but BEFORE the extension.\n\nExample:\nSuffix '_LOD0' ➔ Output 'AlbedoAtlas_Transparent_LOD0.png'"),
                fileSuffix
            ));


            //Sanitize user inputs immediately after editing
            filePrefix = SanitizeString(filePrefix);  //  call SanitizeString
            fileSuffix = SanitizeString(fileSuffix);  //  call SanitizeString

            GUILayout.Space(5);
            GUILayout.Label("Example Output Name:", EditorStyles.boldLabel);

            // Simulate what a filename would look like
            string exampleBaseName = "AlbedoAtlasExample"; // Neutral, always safe
            string exampleGroupSuffix = ""; // No group suffix in example
            string exampleExtension = ".png";


            string previewFilename = filePrefix + exampleBaseName + exampleGroupSuffix + fileSuffix + exampleExtension;

            // Draw the preview inside a nice help box
            EditorGUILayout.HelpBox(previewFilename, MessageType.Info);



            GUILayout.Label("Save Folder (Defaults to _Output)", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            saveFolderPath = EditorGUILayout.TextField(
        new GUIContent("Save Folder", "Must be inside the project's Assets folder. This is where atlases, meshes, and prefabs will be saved."),
        saveFolderPath
    );

            if (GUILayout.Button(new GUIContent("Select Folder", "Choose a target folder inside 'Assets' to save output files."), GUILayout.Width(100)))

            {
                string path = EditorUtility.OpenFolderPanel("Select Save Folder", "Assets", "");
                if (!string.IsNullOrEmpty(path))
                {
                    if (path.StartsWith(Application.dataPath))
                    {
                        saveFolderPath = "Assets" + path.Substring(Application.dataPath.Length);
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Invalid Folder", "Folder must be inside the Assets directory.", "OK");
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Label("Actions", EditorStyles.boldLabel);
            // 🛡️ Live check if any valid objects are loaded
            bool canGenerate = selectedObjects.Any(obj => obj && obj.GetComponentInChildren<MeshFilter>());
            GUI.enabled = selectedObjects.Count > 0 && hasValidMeshFilters;

            Color originalColor = GUI.backgroundColor;
            Color normalColor = new Color(0.5f, 0.9f, 0.8f);
            Color hoverColor = new Color(0.6f, 1f, 0.9f);
            Color dangerColor = new Color(1f, 0.5f, 0.5f);

            string generateLabel = atlasOnlyMode ? "Generate Atlases Only" : "Generate Atlases + Mesh Copies";

            List<string> extras = new List<string>();
            if (generatePackedMaskMap) extras.Add("Packed Mask");
            if (generateCombinedMesh) extras.Add("Combine Meshes");
            if (generatePrefab) extras.Add("Prefab");
            if (placePrefabInScene) extras.Add("Place in Scene");
            if (generateLODs) extras.Add("LODs");
            if (generateSummaryReport) extras.Add("Summary Report");
            if (generateUV2) extras.Add("UV2");
            if (extras.Count > 0) generateLabel += " (" + string.Join(" + ", extras) + ")";


            GUIStyle generateButtonStyle = new GUIStyle(GUI.skin.button)
            {
                fontStyle = FontStyle.Bold,
                fontSize = 14,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true,
                stretchHeight = true,
                padding = new RectOffset(10, 10, 10, 10) // Optional: adds breathing room
            };



            //Stable dynamic generate button
            GUI.enabled = !isGenerating;

            if (GUILayout.Button(
                new GUIContent(generateLabel,
                "Processes selected objects:\n- Generates atlases and shared material\n- Optionally combines meshes\n- Creates prefabs\n- Builds LODs (if enabled)"),
                generateButtonStyle,
                GUILayout.Height(70),
                GUILayout.ExpandWidth(true)))
            {
                isGenerating = true;
                atlasOnlyMode = false;

                // Force repaint so the HelpBox shows
                Repaint();

                EditorApplication.delayCall += () =>
                {
                    try
                    {
                        ProcessTransparentAndOpaqueGroups();
                    }
                    finally
                    {
                        isGenerating = false;
                        Repaint(); // Clear the message after completion
                    }
                };

            }

            GUI.enabled = true;

            if (isGenerating)
            {
                EditorGUILayout.HelpBox("⏳ Generating atlases... Please wait.", MessageType.Info);
                Repaint(); // Force UI to refresh while processing
            }

            if (!fbxWarningShownThisSession && selectedObjects.Count == 1 && generateLODs)
            {
                var meshFilter = selectedObjects[0].GetComponent<MeshFilter>();
                if (meshFilter != null && meshFilter.sharedMesh != null)
                {
                    string meshPath = AssetDatabase.GetAssetPath(meshFilter.sharedMesh);
                    if (!string.IsNullOrEmpty(meshPath) && meshPath.ToLowerInvariant().EndsWith(".fbx"))
                    {
                        fbxWarningShownThisSession = true;

                        EditorUtility.DisplayDialog(
                            "FBX Transform Warning",
                            "LOD generation may result in incorrect scale or sideways orientation if the FBX was exported from Blender without applying transforms.\n\nFix in Blender:\n1. Select the object\n2. Press Ctrl+A → Apply All Transforms\n3. Re-export the FBX.\n\n💡 Tip: Use combined mesh instead for best results.",
                            "OK"
                        );
                    }
                }
            }



            // GUI.enabled = true;
            // GUI.backgroundColor = originalColor;
            // //Generate Button ends here



            GUILayout.Space(10);
            ShowPerformanceWarnings();  //  call here ONCE

            GUILayout.Label("Atlas Preview", EditorStyles.boldLabel);  // only appears once
            try
            {
                if (previewTexture != null && previewTexture.width > 0 && previewTexture.height > 0)
                {
                    Rect previewRect = GUILayoutUtility.GetRect(256, 256, GUILayout.ExpandWidth(false));
                    EditorGUI.DrawPreviewTexture(previewRect, previewTexture);
                }
                else
                {
                    GUILayout.Label("No preview available.", EditorStyles.miniLabel);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"⚠️ Failed to draw preview texture: {ex.Message}");
                previewTexture = null;
            }



        }
        finally
        {
            EditorGUILayout.EndScrollView(); //  Always executes even on exception
            GUILayout.FlexibleSpace(); // Adds breathing room to bottom

        }
        GUILayout.Space(10);
        bool showHelp = EditorPrefs.GetBool("TAT_ShowQuickstartHelp", false);
        showHelp = EditorGUILayout.Foldout(showHelp, "Quickstart Help", true);
        EditorPrefs.SetBool("TAT_ShowQuickstartHelp", showHelp);

        if (showHelp)
        {
            EditorGUILayout.HelpBox(
     "👋 This tool helps you:\n" +
     "• Generate texture atlases from selected GameObjects\n" +
     "• Assign a shared material based on shader pipeline (Standard, URP, HDRP)\n" +
     "• Optionally combine meshes into one\n" +
     "• Optionally generate LODs (Level of Detail)\n" +
     "• Optionally generate Packed Mask Maps (URP/HDRP only)\n" +
     "• Optionally generate UV2s for lightmapping (URP only)\n" +
     "• Optionally save as prefab and place in scene\n\n" +
     "💡 Tips:\n" +
     "• Combine objects first, then generate LODs from the result.\n" +
     "• LODs only work when one object is selected (original or combined).\n" +
     "• If materials or maps were not detected, click 'Refresh Selected Objects'.\n" +
     "• If output is unexpectedly blank, try running once — the tool will auto-detect your pipeline and succeed on the next run.\n\n" +
     "🚫 Limitations:\n" +
     "• Transparent and opaque objects are processed separately\n" +
     "• This tool is Editor-only (not for runtime use)\n" +
     "• SkinnedMeshRenderers and animations are not supported\n" +
     "• LOD generation supports only a single mesh at a time",
     MessageType.Info
 );

        }

    }

    private void ShowPerformanceWarnings()
    {
        int ram = UVUnifySystemSpecs.GetSystemRAMMB();
        int vram = UVUnifySystemSpecs.GetGPUVRAMMB();
        int cores = UVUnifySystemSpecs.GetCPUCores();

        //  Total vertex count for estimations
        int totalVerts = selectedObjects.Sum(obj =>
        {
            var mf = obj.GetComponent<MeshFilter>();
            return (mf && mf.sharedMesh != null) ? mf.sharedMesh.vertexCount : 0;
        });

        //  LOD Warnings + Estimate
        if (generateLODs)
        {
            if (cores < 6 || ram < 8000)
            {
                EditorGUILayout.HelpBox(
                    $"⚠️ Your system has only {cores} CPU core(s) and {ram / 1024f:F1} GB RAM.\nLOD generation may take a long time or freeze Unity.",
                    MessageType.Warning
                );
            }

            string lodEstimate = EstimateLODTime(totalVerts, cores);
            EditorGUILayout.LabelField($"⏳ LOD Mesh Simplification: {lodEstimate}", EditorStyles.miniLabel);

            string roughTotalEstimate = EstimateTotalPipelineTime(totalVerts, cores);
            EditorGUILayout.LabelField($"🧵 Estimated Total Generation Time: {roughTotalEstimate}", EditorStyles.miniLabel);
        }

        //  Atlas memory warning
        if (atlasResolution >= 8192 && vram < 3000)
        {
            EditorGUILayout.HelpBox(
                $"⚠️ Your GPU has only {vram} MB VRAM. High-res atlases (8192x8192) may cause memory issues or crash the Editor.",
                MessageType.Warning
            );
        }


        // ✅ Optional System Specs Section
        GUILayout.Space(10);
        bool showSpecs = EditorPrefs.GetBool("TAT_ShowSystemSpecs", false);
        showSpecs = EditorGUILayout.Foldout(showSpecs, " System Specs Overview", true);
        EditorPrefs.SetBool("TAT_ShowSystemSpecs", showSpecs);

        if (showSpecs)
        {
            EditorGUILayout.LabelField($"CPU: {UVUnifySystemSpecs.GetCPUName()} ({UVUnifySystemSpecs.GetCPUCores()} cores)");
            EditorGUILayout.LabelField($"GPU: {UVUnifySystemSpecs.GetGPUName()} ({UVUnifySystemSpecs.GetGPUVRAMMB()} MB VRAM)");
            EditorGUILayout.LabelField($"RAM: {UVUnifySystemSpecs.GetSystemRAMMB()} MB");
            EditorGUILayout.LabelField($"Unity Version: {Application.unityVersion}");
            EditorGUILayout.LabelField($"Compute Shaders: {(UVUnifySystemSpecs.SupportsComputeShaders() ? "Yes" : "No")}");
            EditorGUILayout.LabelField($"Async GPU Readback: {(UVUnifySystemSpecs.SupportsAsyncReadback() ? "Yes" : "No")}");
        }
    }



    private string EstimateLODTime(int totalVerts, int cores)
    {
        float secPer10k = Mathf.Lerp(12f, 5f, Mathf.InverseLerp(2f, 16f, cores));
        float estimatedBase = (totalVerts / 10000f) * secPer10k;
        estimatedBase = Mathf.Clamp(estimatedBase, 3f, 300f);

        //  ADAPTIVE CORRECTION
        float lastActual = EditorPrefs.GetFloat("UVUnify_LastLODActual", -1);
        float lastEstimate = EditorPrefs.GetFloat("UVUnify_LastLODEstimate", -1);
        float correctionFactor = 1f;

        if (lastActual > 0 && lastEstimate > 0)
        {
            correctionFactor = Mathf.Clamp(lastActual / lastEstimate, 0.2f, 2f);
        }

        float corrected = estimatedBase * correctionFactor;
        float min = Mathf.Clamp(corrected * 0.8f, 3f, 600f);
        float max = Mathf.Clamp(corrected * 1.25f, min + 1f, 900f);

        if (lastLoggedVertCount != totalVerts)
        {
            Debug.Log($"[LOD ESTIMATE] totalVerts = {totalVerts}, Estimate: ~{min:F0}–{max:F0} sec (adjusted)");
            lastLoggedVertCount = totalVerts;
        }

        return $"~{min:F0}–{max:F0} sec";
    }

    private string EstimateTotalPipelineTime(int totalVerts, int cores)
    {
        float lodSeconds = (totalVerts / 10000f) * Mathf.Lerp(12f, 5f, Mathf.InverseLerp(2f, 16f, cores));
        lodSeconds = Mathf.Clamp(lodSeconds, 3f, 300f);

        float totalEstimate = lodSeconds * 2.5f; // assume atlas/mesh/material overhead

        if (totalEstimate < 20f)
            return "~20–30 sec";
        if (totalEstimate < 60f)
            return "~30–60 sec";
        if (totalEstimate < 120f)
            return "~1–2 min";

        return $"> {Mathf.RoundToInt(totalEstimate)} sec";
    }

    private string EstimateCombinedUV2AndLODTime(int totalVerts, int cores, int ram)
    {
        float ramFactor = Mathf.Clamp01((ram / 1000f - 4f) / 12f); // Normalize 4GB–16GB
        float lodFactor = Mathf.InverseLerp(2f, 16f, cores);       // Normalize 2–16 cores

        // Base times
        float uv2Base = (totalVerts / 10000f) * Mathf.Lerp(20f, 6f, ramFactor);
        float lodBase = (totalVerts / 10000f) * Mathf.Lerp(12f, 5f, lodFactor);

        // Add 20–50% margin for overhead (Unity stalls, texture imports, etc)
        float minTime = Mathf.Clamp((uv2Base + lodBase) * 1.8f, 8f, 300f);
        float maxTime = Mathf.Clamp((uv2Base + lodBase) * 2.8f, 10f, 400f);

        return $"~{minTime:F0}–{maxTime:F0} sec";
    }




    private string EstimateUV2Time(int totalVerts, int ramMB)
    {
        float ramFactor = Mathf.Clamp01((ramMB - 4000f) / 12000f); // Normalize 4GB → 16GB
        float secPer10k = Mathf.Lerp(20f, 6f, ramFactor);
        float estimatedBase = (totalVerts / 10000f) * secPer10k;
        estimatedBase = Mathf.Clamp(estimatedBase, 3f, 180f);

        //  Adaptive correction (optional: store these UV2-specific if needed)
        float lastActual = EditorPrefs.GetFloat("UVUnify_LastUV2Actual", -1);
        float lastEstimate = EditorPrefs.GetFloat("UVUnify_LastUV2Estimate", -1);
        float correctionFactor = 1f;

        if (lastActual > 0 && lastEstimate > 0)
        {
            correctionFactor = Mathf.Clamp(lastActual / lastEstimate, 0.2f, 2f);
        }

        float corrected = estimatedBase * correctionFactor;
        float min = Mathf.Clamp(corrected * 0.8f, 3f, 600f);
        float max = Mathf.Clamp(corrected * 1.25f, min + 1f, 900f);

        if (lastLoggedUV2Verts != totalVerts)
        {
            Debug.Log($"[UV2 ESTIMATE] totalVerts = {totalVerts}, Estimate: ~{min:F0}–{max:F0} sec (adjusted)");
            lastLoggedUV2Verts = totalVerts;
        }

        return $"~{min:F0}–{max:F0} sec";
    }


    private string EstimateTotalUV2PipelineTime(int totalVerts, int ramMB)
    {
        float ramFactor = Mathf.Clamp01((ramMB - 4000f) / 12000f);
        float baseSeconds = (totalVerts / 10000f) * Mathf.Lerp(20f, 6f, ramFactor);
        baseSeconds = Mathf.Clamp(baseSeconds, 3f, 120f);

        float totalEstimate = baseSeconds * 2.2f;

        if (totalEstimate < 20f)
            return "~20–30 sec";
        if (totalEstimate < 60f)
            return "~30–60 sec";
        if (totalEstimate < 120f)
            return "~1–2 min";

        return $"> {Mathf.RoundToInt(totalEstimate)} sec";
    }

    private string EstimateCombineTime(int totalVerts)
    {
        // Estimate: ~1–2 seconds per 100k vertices (very fast)
        float seconds = Mathf.Clamp(totalVerts / 100000f * 1.5f, 0.2f, 10f);
        return $"⏱ Mesh Combining Estimate: ~{seconds:F1} sec";
    }


    #endregion


    private Texture2D LoadLogoTexture()
    {
        if (cachedLogo != null)
            return cachedLogo;

        string[] guids = AssetDatabase.FindAssets("uvunify_logo t:Texture2D");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (Path.GetFileName(path).ToLower() == "uvunify_logo.png")
            {
                cachedLogo = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                return cachedLogo;
            }
        }

        Debug.LogWarning("⚠️ uvunify_logo.png not found in project.");
        return null;
    }



    private Texture2D GeneratePackedMaskMap(Texture2D metallicTex, Texture2D aoTex, Texture2D heightTex, Texture2D emissionTex)
    {
        // Pick ANY available texture to define width/height
        Texture2D referenceTex = metallicTex ?? aoTex ?? heightTex ?? emissionTex;

        if (referenceTex == null)
        {
            if (verboseLogging)
                Debug.LogWarning("⚠️ No available textures found to create a Packed Mask Map. Skipping.");

            return null;
        }

        int width = referenceTex.width;
        int height = referenceTex.height;

        Texture2D packedMask = new Texture2D(width, height, TextureFormat.RGBA32, true, true);

        Color[] packedPixels = new Color[width * height];
        Color[] metallicPixels = GetSafePixels(metallicTex, "Metallic", width, height);
        Color[] aoPixels = GetSafePixels(aoTex, "AO", width, height);
        Color[] heightPixels = GetSafePixels(heightTex, "Height", width, height);
        Color[] emissionPixels = GetSafePixels(emissionTex, "Emission", width, height);


        bool isHDRP = GraphicsSettings.currentRenderPipeline != null
    && GraphicsSettings.currentRenderPipeline.GetType().Name.Contains("HDRenderPipelineAsset");

        for (int i = 0; i < packedPixels.Length; i++)
        {
            float r = metallicTex ? metallicPixels[i].r : 0f;   // R: Metallic or 0
            float g = aoTex ? aoPixels[i].r : 1f;               // G: AO or 1 (full ambient)

            float b;
            if (isHDRP)
            {
                b = 1f; // HDRP expects white for Detail Mask
            }
            else
            {
                b = heightTex ? heightPixels[i].r : 0.5f; // Standard/URP store Height
            }

            float smoothness = 1f;
            if (metallicTex)
            {
                smoothness = metallicPixels[i].a;
            }

            packedPixels[i] = new Color(r, g, b, smoothness);
        }


        packedMask.SetPixels(packedPixels);
        packedMask.Apply(true);

        if (verboseLogging)
            Debug.Log("✅ Packed Mask Map generated successfully (partial maps allowed).");


        return packedMask;
    }
    private Color[] GetSafePixels(Texture2D tex, string label, int width, int height)
    {
        if (tex == null)
            return new Color[width * height];

        string path = AssetDatabase.GetAssetPath(tex);
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;

        if (importer != null && !importer.isReadable)
        {
            importer.isReadable = true;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);

            if (verboseLogging)
                Debug.Log($"🔁 Made {label} texture readable: {tex.name}");
        }

        try
        {
            return tex.GetPixels();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ Failed to read pixels from {label} texture '{tex.name}': {ex.Message}");
            return new Color[width * height];
        }

    }
    // ─── Utilities ──────────────────────────────────────────────
    private void NormalizeNormalMap(Texture2D tex)
    {
        Color[] pixels = tex.GetPixels();

        for (int i = 0; i < pixels.Length; i++)
        {
            float x = pixels[i].r * 2f - 1f;
            float y = pixels[i].g * 2f - 1f;
            float z = Mathf.Sqrt(Mathf.Clamp01(1f - x * x - y * y));
            Vector3 normal = new Vector3(x, y, z).normalized;

            pixels[i].r = normal.x * 0.5f + 0.5f;
            pixels[i].g = normal.y * 0.5f + 0.5f;
            pixels[i].b = normal.z * 0.5f + 0.5f;
            pixels[i].a = 1f;
        }

        tex.SetPixels(pixels);
    }







    #region Main Atlas Generation
    private (Dictionary<string, Dictionary<GameObject, Texture2D>> objectTextureMap, Dictionary<string, Rect[]> uvRectsByType) GenerateAllTextureAtlases()

    {



        Mesh combinedMesh = null;
        Texture2D albedoAtlas = null;
        Material atlasMat = null;
        Shader shader = null;
        string shaderPath = "";
        bool anyAtlasGenerated = false;
        ResolvePipelineIfNeeded();

        objectTextureMap.Clear();
        uvRectsByType.Clear();
        fallbackColorUsedObjects.Clear();

        List<GameObject> opaqueObjects = new List<GameObject>();
        List<GameObject> transparentObjects = new List<GameObject>();


        foreach (GameObject obj in selectedObjects)
        {
            if (IsObjectTransparent(obj))
            {
                transparentObjects.Add(obj);
            }
            else
            {
                opaqueObjects.Add(obj);
            }
        }


        Dictionary<string, List<Texture2D>> textureTypeDict = new Dictionary<string, List<Texture2D>>();
        int skippedUVObjects = 0; //  Count objects skipped due to missing or invalid UVs


        //Supports both Standard and URP/HDRP by checking both _MainTex and _BaseMap
        Dictionary<string, string> mapPropNamePairs = new Dictionary<string, string>();

        if (selectedPipeline == ShaderPipeline.HDRP)
        {
            if (shader != null && !shader.name.Contains("HDRP/Lit"))
            {
                Debug.LogWarning($"⚠️ You are using HDRP, but the selected shader is '{shader.name}'.\n" +
                                 "For proper lighting, transparency, and GI, you should use 'HDRP/Lit'.");
            }

            mapPropNamePairs.Add("_BaseColorMap", "Albedo");
            mapPropNamePairs.Add("_NormalMap", "Normal");
        }
        else if (selectedPipeline == ShaderPipeline.Standard)
        {
            mapPropNamePairs.Add("_MainTex", "Albedo");
            mapPropNamePairs.Add("_BumpMap", "Normal");
        }
        else // URP or fallback
        {
            mapPropNamePairs.Add("_BaseMap", "Albedo");
            mapPropNamePairs.Add("_BumpMap", "Normal");
        }



        // Shared maps
        mapPropNamePairs.Add("_MetallicGlossMap", "Metallic");
        mapPropNamePairs.Add("_OcclusionMap", "AO");
        mapPropNamePairs.Add("_ParallaxMap", "Height");
        mapPropNamePairs.Add("_EmissionMap", "Emission");
        if (selectedPipeline == ShaderPipeline.HDRP)
            mapPropNamePairs.Add("_MaskMap", "Mask");





        //  Detect shader name and assign correct pipeline + UV source
        if (selectedPipeline == ShaderPipeline.Auto)
        {
            string detected = DetectPipelineShaderName().ToLowerInvariant();

            if (detected.Contains("hdrp"))
            {
                selectedPipeline = ShaderPipeline.HDRP;
                uvSourceProp = "_BaseColorMap";
            }
            else if (detected.Contains("universal"))
            {
                selectedPipeline = ShaderPipeline.URP;
                uvSourceProp = "_BaseMap";
            }
            else
            {
                selectedPipeline = ShaderPipeline.URP; // Fallback to URP
                uvSourceProp = "_BaseMap";
            }

            //  Update dropdown index to match detected UV source
            selectedUVSourceIndex = System.Array.IndexOf(uvSourceOptions, uvSourceProp);

            if (verboseLogging)
                Debug.Log($"🔍 Auto-detected pipeline: {selectedPipeline}, using UV source: {uvSourceProp}");
        }

        //  Disable Packed Mask Map for Standard — not fully supported
        if (selectedPipeline == ShaderPipeline.Standard)
        {
            generatePackedMaskMap = false;
            if (verboseLogging)
                Debug.Log("⚠️ Disabling Packed Mask Map generation for Standard shader (unsupported format).");
        }



        //  Assign shader name based on selected pipeline
        string shaderName = selectedPipeline switch
        {
            ShaderPipeline.URP => "Universal Render Pipeline/Lit",
            ShaderPipeline.HDRP => "HDRP/Lit",
            ShaderPipeline.Standard => "Standard",
            _ => "Universal Render Pipeline/Lit"
        };


        //Ensure UV source is valid
        var validProps = new[] { "_BaseMap", "_BaseColorMap" };
        if (!validProps.Contains(uvSourceProp))
        {
            uvSourceProp = selectedPipeline switch
            {
                ShaderPipeline.HDRP => "_BaseColorMap",
                ShaderPipeline.Standard => "_MainTex",
                _ => "_BaseMap"
            };
        }





        // Update dropdown index
        selectedUVSourceIndex = System.Array.IndexOf(uvSourceOptions, uvSourceProp);


        // Try loading shader by intended name
        shader = Shader.Find(shaderName);

        //  Warn about unsupported maps in Standard pipeline
        if (selectedPipeline == ShaderPipeline.Standard)
        {
            bool detectedAO = selectedObjects.Any(obj =>
            {
                var mat = obj.GetComponent<MeshRenderer>()?.sharedMaterial;
                return mat != null && mat.HasProperty("_OcclusionMap") && mat.GetTexture("_OcclusionMap") != null;
            });

            bool detectedHeight = selectedObjects.Any(obj =>
            {
                var mat = obj.GetComponent<MeshRenderer>()?.sharedMaterial;
                return mat != null && mat.HasProperty("_ParallaxMap") && mat.GetTexture("_ParallaxMap") != null;
            });

            bool detectedEmission = selectedObjects.Any(obj =>
            {
                var mat = obj.GetComponent<MeshRenderer>()?.sharedMaterial;
                return mat != null && mat.HasProperty("_EmissionMap") && mat.GetTexture("_EmissionMap") != null;
            });

            List<string> unsupported = new();
            if (detectedAO) unsupported.Add("Ambient Occlusion");
            if (detectedHeight) unsupported.Add("Height (Parallax)");
            if (detectedEmission) unsupported.Add("Emission");

            if (unsupported.Count > 0 && verboseLogging)
            {
                Debug.LogWarning(
                    $"⚠️ WARNING: The Standard shader does NOT support the following maps from packed masks:\n" +
                    $"- {string.Join("\n- ", unsupported)}\n\n" +
                    $"These will be ignored or stripped during material setup.\n" +
                    $"For full support (AO/Height/Emission), use URP or HDRP."
                );
            }
        }



        if (shader == null)
        {
            Debug.LogWarning($"⚠️ Shader '{shaderName}' not found. Attempting fallback to Standard...");

            // First fallback: try Standard
            shader = Shader.Find("Standard");
            if (shader != null)
            {
                selectedPipeline = ShaderPipeline.Standard;
                uvSourceProp = "_MainTex";
                selectedUVSourceIndex = Array.IndexOf(uvSourceOptions, uvSourceProp);
                Debug.Log("✅ Fallback to Standard shader succeeded.");
            }
            else
            {
                // Second fallback: try URP
                shader = Shader.Find("Universal Render Pipeline/Lit");
                if (shader != null)
                {
                    selectedPipeline = ShaderPipeline.URP;
                    uvSourceProp = "_BaseMap";
                    selectedUVSourceIndex = Array.IndexOf(uvSourceOptions, uvSourceProp);
                    Debug.Log("✅ Fallback to URP shader succeeded.");
                }
                else
                {
                    Debug.LogError("❌ No supported fallback shader found. Aborting atlas generation.");
                    EditorUtility.DisplayDialog(
                        "Shader Missing",
                        "Neither the Standard shader nor URP shader could be found.\nPlease check your render pipeline packages.",
                        "OK"
                    );
                    if (generateLODs && selectedObjects.Count == 1 && !generateCombinedMesh)
                    {
                        Debug.LogWarning("⚠️ A new material was generated but not used. This happens when LODs are created from an unprocessed object with original UVs.");
                    }

                    return (objectTextureMap, uvRectsByType);
                }
            }
        }


        bool isTransparentGroup = groupSuffix.Contains("Transparent");
        bool isCutout = useAlphaClipping;

        shaderPath = GetShaderPathForGroup();
        EnsureDefaultShaderExists(shaderPath); //  auto-create if missing
        shader = Shader.Find(shaderPath);

        if (shader == null)
        {
            EditorUtility.DisplayDialog(
                "Missing Shader",
                $"The shader '{shaderPath}' could not be found.\n\n" +
                "A default stub shader was generated, but it may not render correctly.\n\n" +
                "To customize rendering, edit the stub shader at:\nAssets/UVUnify/Shaders/",
                "OK");

            Debug.LogWarning($"⚠️ Shader not found: {shaderPath}. Stub was generated, but fallback to URP Lit used.");
            shader = Shader.Find("Universal Render Pipeline/Lit");
        }

        atlasMat = new Material(shader);

        //  Professional material setup
        MaterialHelper.SetupMaterial(atlasMat, selectedPipeline, isTransparentGroup, isCutout);
        Undo.RegisterCreatedObjectUndo(atlasMat, "Create Atlased Material");





        //Which shader is actually used
        Debug.Log($"Shader assigned: {shader?.name} | Pipeline: {selectedPipeline}");

        if (verboseLogging)
            Debug.Log($" Shader assigned and material created: {shader.name}");


        // Assign the albedo atlas to the correct property based on pipeline or actual shader name
        string actualShaderName = atlasMat.shader.name;





        AssignBaseAlbedoTexture(atlasMat, albedoAtlas);






        foreach (var kvp in mapPropNamePairs)
        {
            string prop = kvp.Key;
            string mapName = kvp.Value;



            // Snap-shot loop variables for any delayed lambdas
            string propCopy = prop;

            if (selectedPipeline == ShaderPipeline.HDRP && prop == "_BaseMap") continue;
            if (selectedPipeline == ShaderPipeline.URP && prop == "_BaseColorMap") continue;

            List<Texture2D> textures = new List<Texture2D>();
            Dictionary<GameObject, Texture2D> objMap = new Dictionary<GameObject, Texture2D>();
            textureInstanceIdToIndex.Clear(); // reuse global dictionary for deduplication






            foreach (GameObject obj in selectedObjects)
            {
                var renderer = obj.GetComponent<MeshRenderer>();
                if (!renderer) continue;

                var mat = renderer.sharedMaterial;
                if (mat == null || !mat.HasProperty(prop)) continue;

                Texture2D tex = mat.GetTexture(prop) as Texture2D;

                bool isAlbedoProp = (prop == "_BaseMap" || prop == "_BaseColorMap" || prop == "_MainTex");


                if (tex == null && isAlbedoProp && (mat.HasProperty("_BaseColor") || mat.HasProperty("_Color")))
                {
                    Color fallback = mat.HasProperty("_BaseColor") ? mat.GetColor("_BaseColor") : mat.GetColor("_Color");

                    // Avoid full black unless explicitly intended
                    if (fallback == Color.black)
                        fallback = new Color(0.04f, 0.04f, 0.04f); // Physically plausible dark

                    tex = CreateFallbackTextureFromColor(obj, fallback);

                    fallbackColorUsedObjects.Add(obj.name);


                    if (verboseLogging)
                        Debug.Log($"🎨 Created fallback color texture for '{obj.name}' using color {fallback}");
                }


                // Skip objects with no texture for this map (e.g., no _BaseMap or _BaseColorMap)
                if (tex == null)
                {
                    if (verboseLogging)
                        Debug.LogWarning($"⛔ Skipping '{obj.name}' for {mapName} — no texture assigned.");
                    continue;
                }



                // Skip fallback normal map generation if material never used one
                if ((prop == "_BumpMap" || prop == "_NormalMap") && tex == null)
                {
                    if (mat.HasProperty(prop) && mat.GetTexture(prop) != null)
                    {
                        tex = CreateFlatNormalFallback(obj);
                    }
                    else
                    {
                        if (verboseLogging)
                            Debug.Log($"⛔ Skipping {prop} for '{obj.name}' — material never had a normal map.");
                        continue; // Skip this object for this property
                    }
                }



                if (tex == null)
                {
                    if (verboseLogging)
                        Debug.LogWarning($"❌ Missing {prop} on {obj.name} (even after fallback)");
                    continue;
                }

                int instanceId = tex.GetInstanceID();
                Texture2D dedupedTex;

                if (!textureInstanceIdToIndex.ContainsKey(instanceId))
                {
                    string originalPath = AssetDatabase.GetAssetPath(tex);
                    string folder = EnsureStagingFolderExists();
                    string copyPath = Path.Combine(folder, tex.name + "_" + mapName + "_Copy.png");
                    copyPath = AssetDatabase.GenerateUniqueAssetPath(copyPath);

                    if (string.IsNullOrEmpty(originalPath))
                    {
                        Debug.LogWarning($"⛔ Skipping copy — original texture path is empty (likely a fallback color): {tex.name}");
                        continue;
                    }

                    File.Copy(originalPath, copyPath, true);
                    AssetDatabase.ImportAsset(copyPath, ImportAssetOptions.ForceSynchronousImport);

                    var importer = AssetImporter.GetAtPath(copyPath) as TextureImporter;
                    if (importer != null)
                    {
                        importer.textureType = TextureImporterType.Default;
                        importer.sRGBTexture = false;
                        importer.alphaSource = TextureImporterAlphaSource.None;
                        importer.alphaIsTransparency = false;
                        importer.textureCompression = TextureImporterCompression.Uncompressed;
                        importer.isReadable = true;
                        importer.SaveAndReimport();

                        if (verboseLogging)
                            if (verboseLogging)
                                Debug.Log($" Safe duplicate reimported: {Path.GetFileName(copyPath)}");

                    }

                    dedupedTex = AssetDatabase.LoadAssetAtPath<Texture2D>(copyPath);
                    textureInstanceIdToIndex[instanceId] = textures.Count;
                    textures.Add(dedupedTex);
                }
                else
                {
                    int index = textureInstanceIdToIndex[instanceId];
                    dedupedTex = textures[index];
                }

                objMap[obj] = dedupedTex;


            }

            // Don't build atlas if nothing valid
            textures = textures.Where(t => t != null).ToList();
            if (textures.Count == 0)
            {
                if (verboseLogging)
                    Debug.LogWarning($"⚠️ No valid textures found for {mapName}. Skipping {mapName} atlas generation.");
                continue;
            }

            // Now your atlas and UVs can be safely declared
            Texture2D atlas = null;
            Rect[] uvs = new Rect[textures.Count];


            // Manual Normal Map Tiling (Replaces PackTextures)
            Debug.Log($" groupSuffix: {groupSuffix} | mapName: {mapName}");

            if (groupSuffix.Contains("Transparent") && mapName == "Albedo")
            {
                Debug.Log(" [AtlasGen] Using transparent atlas helper for Albedo map");
                atlas = GenerateTransparentAtlas(textures, atlasResolution, atlasPadding, out uvs);
            }
            else
            {
                int columns = Mathf.CeilToInt(Mathf.Sqrt(textures.Count));
                int rows = Mathf.CeilToInt((float)textures.Count / columns);
                int tileSize = atlasResolution / Mathf.Max(columns, rows);

                atlas = new Texture2D(atlasResolution, atlasResolution, TextureFormat.RGBA32, false, true);
                Color fillColor = Color.clear;
                if (prop == "_BumpMap" || prop == "_NormalMap")
                    fillColor = new Color(0.5f, 0.5f, 1f, 1f);
                else
                    fillColor = new Color(0f, 0f, 0f, 0f);

                Color[] fill = Enumerable.Repeat(fillColor, atlasResolution * atlasResolution).ToArray();
                atlas.SetPixels(fill);

                uvs = new Rect[textures.Count];
                for (int i = 0; i < textures.Count; i++)
                {
                    Texture2D tex = textures[i];
                    int col = i % columns;
                    int row = i / columns;

                    int x = col * tileSize;
                    int y = atlasResolution - (row + 1) * tileSize;

                    Texture2D resized = new Texture2D(tileSize, tileSize, TextureFormat.RGBA32, false, true);
                    RenderTexture rt = RenderTexture.GetTemporary(tileSize, tileSize, 0, RenderTextureFormat.ARGB32);
                    for (int yy = 0; yy < tileSize; yy++)
                    {
                        for (int xx = 0; xx < tileSize; xx++)
                        {
                            float u = (float)xx / tileSize;
                            float v = (float)yy / tileSize;
                            Color c = tex.GetPixelBilinear(u, v);
                            resized.SetPixel(xx, yy, c);
                        }
                    }
                    resized.Apply();

                    RenderTexture.active = null;
                    RenderTexture.ReleaseTemporary(rt);

                    if (prop == "_BumpMap" || prop == "_NormalMap")
                    {
                        // StripAlpha(resized);
                        // NormalizeNormalMap(resized);
                    }

                    atlas.SetPixels(x, y, tileSize, tileSize, resized.GetPixels());
                    uvs[i] = new Rect((float)x / atlasResolution, (float)y / atlasResolution, (float)tileSize / atlasResolution, (float)tileSize / atlasResolution);
                    DestroyImmediate(resized);
                }

                DilateAtlasTileBorders(atlas, uvs, Mathf.Clamp(atlasPadding, 1, 8));
                atlas.Apply();
                if (verboseLogging)
                    Debug.Log($" [TransparentAtlas] Packed {textures.Count} textures. Alpha at (0,0): {atlas.GetPixel(0, 0).a}");


            }

            anyAtlasGenerated = true;

            if (verboseLogging)
                Debug.Log($" Final atlas resolution selected: {atlas.width}×{atlas.height}");






            //  Organized folder for atlases
            string atlasFolder = organizeOutputFolders ? Path.Combine(saveFolderPath, "Atlases" + groupSuffix) : saveFolderPath;
            Directory.CreateDirectory(atlasFolder);

            string filePath = GetUniqueFilePath(atlasFolder, filePrefix + mapName + "Atlas" + groupSuffix + fileSuffix, ".png");

            //  Strip alpha BEFORE saving the normal map
            if ((prop == "_BumpMap" || prop == "_NormalMap") ||
      (prop == "_BaseMap" && !groupSuffix.Contains("Transparent")))
            {
                //  Strip alpha for Normal maps and Opaque Albedo maps
                StripAlpha(atlas);

                string assetPath = filePath.Substring(filePath.IndexOf("Assets"));
                File.WriteAllBytes(filePath, atlas.EncodeToPNG());
                AssetDatabase.ImportAsset(filePath, ImportAssetOptions.ForceSynchronousImport);

                if (prop == "_BumpMap" || prop == "_NormalMap")
                {
                    ForceMarkAsNormalMap(assetPath);
                    AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceSynchronousImport);
                }
            }
            else
            {
                // Transparent Albedo or other maps (preserve alpha)
                File.WriteAllBytes(filePath, atlas.EncodeToPNG());
                AssetDatabase.ImportAsset(filePath, ImportAssetOptions.ForceSynchronousImport);
            }
            // Only apply transparent import settings to Transparent Albedo maps
            if (mapName == "Albedo" && groupSuffix.Contains("Transparent"))
            {
                var importer = AssetImporter.GetAtPath(filePath) as TextureImporter;
                if (importer != null)
                {
                    importer.textureType = TextureImporterType.Default;
                    importer.alphaSource = TextureImporterAlphaSource.FromInput;
                    importer.alphaIsTransparency = true;
                    importer.sRGBTexture = true;

                    importer.textureCompression = TextureImporterCompression.Uncompressed;
                    importer.mipmapEnabled = true; // Enable mipmaps
                    importer.wrapMode = TextureWrapMode.Clamp; // Prevent bleeding from tiling

                    importer.filterMode = FilterMode.Bilinear; // Optional: softer mip fade
                    importer.anisoLevel = 2; // Optional: slightly better quality

                    importer.isReadable = false;
                    importer.SaveAndReimport();

                    Debug.Log(" [Importer] Applied transparency-safe settings to Albedo atlas (Transparent group)");
                }
            }




            //  Force correct import settings for normal maps
            if (mapName == "Normal")
            {
                string assetPath = filePath.Substring(filePath.IndexOf("Assets"));
                ForceMarkAsNormalMap(assetPath);
            }


            //  Normalize URP normal maps on import
            if ((prop == "_BumpMap" || prop == "_NormalMap"))
            {
                string assetPath = filePath.Substring(filePath.IndexOf("Assets"));
                TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                if (importer != null)
                {
                    importer.textureType = TextureImporterType.NormalMap;
                    importer.sRGBTexture = false;
                    importer.alphaSource = TextureImporterAlphaSource.None;
                    importer.alphaIsTransparency = false;
                    importer.textureCompression = TextureImporterCompression.Uncompressed;
                    importer.isReadable = false;
                    importer.SaveAndReimport();
                }
            }







            AssetDatabase.ImportAsset(filePath);

            // Force Max Size for PC/Standalone and Android
            string relativePath = filePath.Substring(filePath.IndexOf("Assets"));
            TextureImporter atlasImporter = AssetImporter.GetAtPath(relativePath) as TextureImporter;
            if (atlasImporter != null)
            {
                atlasImporter.isReadable = true;
                atlasImporter.alphaSource = TextureImporterAlphaSource.FromInput;

                bool isColorMap = mapName == "Albedo";
                atlasImporter.alphaIsTransparency = isColorMap && (hasTransparency || forceTransparentMaterial);

                //  Ensure correct color space for albedo/emission maps
                if (mapName.StartsWith("Albedo") || mapName == "Emission")
                {
                    atlasImporter.sRGBTexture = true;
                }
                else
                {
                    atlasImporter.sRGBTexture = false;
                }





                // If this is the Normal Map Atlas, force texture type = Normal Map
                if (mapName == "Normal")
                {
                    atlasImporter.textureType = TextureImporterType.NormalMap;
                    atlasImporter.convertToNormalmap = false; // ✅ Already a normal map
                    atlasImporter.normalmapFilter = TextureImporterNormalFilter.Standard;
                    atlasImporter.textureCompression = TextureImporterCompression.Uncompressed;

                    atlasImporter.SaveAndReimport(); //  First apply the import settings

                    atlasImporter.isReadable = false; //  Now disable Read/Write
                    atlasImporter.SaveAndReimport();  //  Apply again
                }
                else
                {
                    atlasImporter.SaveAndReimport(); //  Apply texture settings

                    atlasImporter.isReadable = false; //  Disable Read/Write
                    atlasImporter.SaveAndReimport();  //  Apply again
                }





                TextureImporterPlatformSettings platformSettings = new TextureImporterPlatformSettings();
                platformSettings.name = "Standalone"; // Default to PC
                platformSettings.overridden = true;
                platformSettings.maxTextureSize = atlasResolution;

                switch (selectedCompression)
                {
                    case AtlasCompression.Uncompressed:
                        atlasImporter.textureCompression = TextureImporterCompression.Uncompressed;
                        platformSettings.format = TextureImporterFormat.RGBA32; // RGBA32 = uncompressed
                        break;

                    case AtlasCompression.DXT5:
                        atlasImporter.textureCompression = TextureImporterCompression.Compressed;
                        platformSettings.format = TextureImporterFormat.DXT5;
                        break;

                    case AtlasCompression.ASTC:
                        atlasImporter.textureCompression = TextureImporterCompression.Compressed;
                        platformSettings.format = TextureImporterFormat.ASTC_6x6;
                        break;
                }

                atlasImporter.SetPlatformTextureSettings(platformSettings);

                // (Optional) Also apply for Android separately if needed
                TextureImporterPlatformSettings androidSettings = new TextureImporterPlatformSettings();
                androidSettings.name = "Android";
                androidSettings.overridden = true;
                androidSettings.maxTextureSize = atlasResolution;

                if (selectedCompression == AtlasCompression.ASTC)
                    androidSettings.format = TextureImporterFormat.ASTC_6x6;
                else if (selectedCompression == AtlasCompression.DXT5)
                    androidSettings.format = TextureImporterFormat.ETC2_RGBA8; // Best fallback if DXT5 not available
                else
                    androidSettings.format = TextureImporterFormat.RGBA32;

                atlasImporter.SetPlatformTextureSettings(androidSettings);

                atlasImporter.SaveAndReimport();



                //  Disable Read/Write after initial import to save memory
                atlasImporter.isReadable = false;
                atlasImporter.SaveAndReimport();

            }





            AssetDatabase.Refresh();
            if (!anyAtlasGenerated)
            {
                if (verboseLogging)
                    Debug.LogWarning("⚠️ No atlases were generated. Skipping mesh combination, prefab creation, and summary report.");


                if (!atlasOnlyMode && generateSummaryReport)
                {
                    string report = $"📝 Texture Atlas Tool Summary Report\n" +
                                    $"Generated on: {System.DateTime.Now}\n\n" +
                                    $"⚠️ WARNING: No texture atlases could be generated.\n" +
                                    $"Possible causes:\n" +
                                    $"- Missing source textures.\n" +
                                    $"- Unsupported texture types.\n" +
                                    $"- Objects missing valid UVs.\n\n" +
                                    $"Selected Objects: {selectedObjects.Count}\n" +
                                    $"Transparent Objects Detected: {transparentObjectsDetected.Count}\n";

                    string reportFolder = organizeOutputFolders ? Path.Combine(saveFolderPath, "Reports") : saveFolderPath;
                    Directory.CreateDirectory(reportFolder);

                    string summaryPath = GetUniqueFilePath(reportFolder, "AtlasSummary" + groupSuffix, ".txt");
                    File.WriteAllText(summaryPath, report);
                    AssetDatabase.Refresh();

                    if (verboseLogging)
                        Debug.Log($"📄 Warning Summary Report saved to: {summaryPath}");

                }

                return (objectTextureMap, uvRectsByType);
            }


            var finalAtlas = AssetDatabase.LoadAssetAtPath<Texture2D>(filePath.Substring(filePath.IndexOf("Assets")));
            if (finalAtlas != null)
            {
                if ((prop == "_BumpMap" || prop == "_NormalMap"))
                {
                    AssignNormalMap(atlasMat, finalAtlas, prop);
                }
                else if (prop == "_EmissionMap")
                {
                    if (selectedPipeline == ShaderPipeline.Standard)
                        AssignEmissionForStandard(atlasMat, finalAtlas);
                    else
                        AssignEmissionMap(atlasMat, finalAtlas);
                }
                else if (selectedPipeline == ShaderPipeline.Standard)
                {
                    // ✅ Explicit safe Standard shader assignments
                    if (prop == "_MetallicGlossMap" && atlasMat.HasProperty("_MetallicGlossMap"))
                    {
                        atlasMat.SetTexture("_MetallicGlossMap", finalAtlas);
                        atlasMat.EnableKeyword("_METALLICGLOSSMAP");
                        if (atlasMat.HasProperty("_Glossiness")) atlasMat.SetFloat("_Glossiness", 0f);
                        if (atlasMat.HasProperty("_Smoothness")) atlasMat.SetFloat("_Smoothness", 0f);
                        if (verboseLogging) Debug.Log("✅ Assigned _MetallicGlossMap (Standard)");

                        // 🧼 Clean alpha to prevent specular issues
                        StripAlphaFromMetallicGloss(atlasMat);
                    }
                    else if (prop == "_OcclusionMap" && atlasMat.HasProperty("_OcclusionMap"))
                    {
                        atlasMat.SetTexture("_OcclusionMap", finalAtlas);
                        atlasMat.SetFloat("_OcclusionStrength", 1f);
                        if (verboseLogging) Debug.Log("✅ Assigned _OcclusionMap (Standard)");
                    }
                    else if (prop == "_ParallaxMap" && atlasMat.HasProperty("_ParallaxMap"))
                    {
                        atlasMat.SetTexture("_ParallaxMap", finalAtlas);
                        atlasMat.SetFloat("_Parallax", 0.02f);
                        atlasMat.EnableKeyword("_PARALLAXMAP");
                        if (verboseLogging) Debug.Log("✅ Assigned _ParallaxMap (Standard)");
                    }
                    else if (atlasMat.HasProperty(prop))
                    {
                        atlasMat.SetTexture(prop, finalAtlas);
                        if (verboseLogging) Debug.Log($"✅ Assigned {prop} atlas (fallback)");
                    }
                }
                else if (atlasMat.HasProperty(prop))
                {
                    atlasMat.SetTexture(prop, finalAtlas);
                    if (verboseLogging) Debug.Log($"✅ Assigned {prop} atlas");
                }
            }
            // ✅ Final polish for Standard shader
            if (atlasMat.HasProperty("_MetallicGlossMap"))
            {
                Texture2D glossTex = atlasMat.GetTexture("_MetallicGlossMap") as Texture2D;
                if (glossTex != null)
                {
                    string path = AssetDatabase.GetAssetPath(glossTex);
                    var importer = AssetImporter.GetAtPath(path) as TextureImporter;

                    if (importer != null && !importer.isReadable)
                    {
                        importer.isReadable = true;
                        importer.textureCompression = TextureImporterCompression.Uncompressed;
                        importer.SaveAndReimport();
                        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
                        glossTex = AssetDatabase.LoadAssetAtPath<Texture2D>(path); // reload it
                    }

                    if (glossTex != null && glossTex.isReadable && glossTex.format == TextureFormat.RGBA32)
                    {
                        var stripped = new Texture2D(glossTex.width, glossTex.height, TextureFormat.RGB24, false);
                        var pixels = glossTex.GetPixels();
                        for (int i = 0; i < pixels.Length; i++) pixels[i].a = 1f;
                        stripped.SetPixels(pixels);
                        stripped.Apply();

                        atlasMat.SetTexture("_MetallicGlossMap", stripped);
                        if (verboseLogging) Debug.Log("✅ Stripped alpha from _MetallicGlossMap");
                    }
                    else if (!glossTex.isReadable)
                    {
                        Debug.LogWarning("⚠️ _MetallicGlossMap texture is not readable, skipping alpha strip.");
                    }
                }
            }




            // Store atlas texture array
            textureTypeDict[prop] = textures;

            // Store under canonical property name (for lookup later)
            objectTextureMap[prop] = objMap;
            uvRectsByType[prop] = uvs;
            textureLookupByType[prop] = new Dictionary<int, int>(textureInstanceIdToIndex);

            //Ensure the active UV source (e.g., "_MainTex") has a valid mapping
            bool isActiveUVProp = (prop == uvSourceProp);
            if (isActiveUVProp)
            {
                objectTextureMap[uvSourceProp] = objMap;
                uvRectsByType[uvSourceProp] = uvs;
                textureLookupByType[uvSourceProp] = new Dictionary<int, int>(textureInstanceIdToIndex);

                if (verboseLogging)
                    Debug.Log($"✅ UV mapping stored for active UV source: {uvSourceProp}");
            }

            //Set preview texture if this is a primary albedo map
            bool isAlbedoMap = mapName == "Albedo";
            if (isAlbedoMap)
            {
                previewTexture = finalAtlas;
                albedoAtlas = atlas;

                if (verboseLogging)
                    Debug.Log($"🎨 Set preview/albedo atlas: {previewTexture?.name}, size: {previewTexture?.width}x{previewTexture?.height}");
            }




            {
                Texture2D baseTex = finalAtlas;
                // Do not update global hasTransparency after atlas creation
                if (baseTex != null && autoSetTransparency)
                {
                    string path = AssetDatabase.GetAssetPath(baseTex);
                    TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                    if (importer != null)
                    {
                        bool hasAlpha = importer.DoesSourceTextureHaveAlpha();
                        if (verboseLogging)
                            Debug.Log($" {baseTex.name} alpha channel present: {hasAlpha}");

                    }
                }

            }
        }

        string materialFolder = organizeOutputFolders ? Path.Combine(saveFolderPath, "Materials") : saveFolderPath;
        Directory.CreateDirectory(materialFolder);
        string matPath = GetUniqueFilePath(materialFolder, "CombinedAtlas_Material" + groupSuffix, ".mat");



        AssetDatabase.CreateAsset(atlasMat, matPath);
        Undo.RegisterCreatedObjectUndo(atlasMat, "Create Atlased Material");

        AssetDatabase.SetLabels(atlasMat, new[] { "UVUnifyAtlas", "GeneratedMaterial" });



        AssetDatabase.SaveAssets();
        EditorGUIUtility.PingObject(atlasMat);
        Selection.activeObject = atlasMat;


        //  Reload and apply transparency settings based on group type
        atlasMat = AssetDatabase.LoadAssetAtPath<Material>(matPath);

        // 🛡️ Safety: Re-assign correct shader after reload (just in case)
        if (atlasMat != null)
        {
            string expectedShader = selectedPipeline switch
            {
                ShaderPipeline.URP => "Universal Render Pipeline/Lit",
                ShaderPipeline.HDRP => "HDRP/Lit",
                ShaderPipeline.Standard => "Standard",
                _ => "Universal Render Pipeline/Lit"
            };

            Shader verifiedShader = Shader.Find(expectedShader);
            if (verifiedShader != null)
            {
                atlasMat.shader = verifiedShader;
                if (verboseLogging)
                    Debug.Log($" Shader reassigned after reload: {expectedShader}");
            }
            else
            {
                Debug.LogWarning($"⚠️ Shader '{expectedShader}' not found on reassignment. Material may render incorrectly.");
            }
        }






        // === Packed RGB Mask Map Generation ===
        bool hasMetallic = textureTypeDict.ContainsKey("_MetallicGlossMap");
        bool hasAO = textureTypeDict.ContainsKey("_OcclusionMap");
        bool hasHeight = textureTypeDict.ContainsKey("_ParallaxMap");
        bool hasEmission = textureTypeDict.ContainsKey("_EmissionMap");
        //  Smart packed mask support — enable only when meaningful
        bool supportsPackedMask =
            selectedPipeline == ShaderPipeline.HDRP || selectedPipeline == ShaderPipeline.URP;

        bool shouldGeneratePackedMask =
            supportsPackedMask &&
            (hasMetallic || hasAO || hasHeight || hasEmission);

        if (!shouldGeneratePackedMask)
        {
            generatePackedMaskMap = false;

            if (verboseLogging)
                Debug.Log(" Skipping Packed Mask generation: not needed in this pipeline or no valid maps found.");
        }


        if (generatePackedMaskMap && (hasMetallic || hasAO || hasHeight || hasEmission))
        {
            // proceed with generating packed mask atlas

            string packedFolder = organizeOutputFolders ? Path.Combine(saveFolderPath, "Atlases" + groupSuffix) : saveFolderPath;
            Directory.CreateDirectory(packedFolder); // Safety

            //  Hard-load atlases from disk
            Texture2D metallicAtlas = null;
            Texture2D aoAtlas = null;
            Texture2D heightAtlas = null;
            Texture2D emissionAtlas = null;

            string metallicPath = Path.Combine(packedFolder, filePrefix + "MetallicAtlas" + groupSuffix + fileSuffix + ".png");
            string aoPath = Path.Combine(packedFolder, filePrefix + "AOAtlas" + groupSuffix + fileSuffix + ".png");
            string heightPath = Path.Combine(packedFolder, filePrefix + "HeightAtlas" + groupSuffix + fileSuffix + ".png");
            string emissionPath = Path.Combine(packedFolder, filePrefix + "EmissionAtlas" + groupSuffix + fileSuffix + ".png");

            if (File.Exists(metallicPath))
            {
                string metallicRelPath = metallicPath.Substring(metallicPath.IndexOf("Assets"));
                metallicAtlas = AssetDatabase.LoadAssetAtPath<Texture2D>(metallicRelPath);

                var metallicImporter = AssetImporter.GetAtPath(metallicRelPath) as TextureImporter;
                if (metallicImporter != null)
                {
                    metallicImporter.isReadable = true;
                    metallicImporter.textureCompression = TextureImporterCompression.Uncompressed;
                    metallicImporter.sRGBTexture = false;
                    metallicImporter.SaveAndReimport();
                }
            }


            if (File.Exists(aoPath))
                aoAtlas = AssetDatabase.LoadAssetAtPath<Texture2D>(aoPath.Substring(aoPath.IndexOf("Assets")));

            if (File.Exists(heightPath))
                heightAtlas = AssetDatabase.LoadAssetAtPath<Texture2D>(heightPath.Substring(heightPath.IndexOf("Assets")));

            if (File.Exists(emissionPath))
                emissionAtlas = AssetDatabase.LoadAssetAtPath<Texture2D>(emissionPath.Substring(emissionPath.IndexOf("Assets")));

            if (metallicAtlas == null && aoAtlas == null && heightAtlas == null)
            {
                if (verboseLogging)
                    Debug.LogWarning("⚠️ No valid input atlases found for Packed Mask Map generation. Skipping.");

            }
            else
            {
                Texture2D packedMaskAtlas = GeneratePackedMaskMap(metallicAtlas, aoAtlas, heightAtlas, emissionAtlas);

                if (packedMaskAtlas != null)
                {
                    string packedMaskPath = GetUniqueFilePath(packedFolder, "PackedMaskAtlas" + groupSuffix, ".png");
                    File.WriteAllBytes(packedMaskPath, packedMaskAtlas.EncodeToPNG());
                    AssetDatabase.ImportAsset(packedMaskPath);

                    string relativePackedPath = packedMaskPath.Substring(packedMaskPath.IndexOf("Assets"));
                    TextureImporter packedImporter = AssetImporter.GetAtPath(relativePackedPath) as TextureImporter;

                    if (packedImporter != null)
                    {
                        packedImporter.isReadable = true;
                        packedImporter.textureCompression = TextureImporterCompression.CompressedHQ;
                        packedImporter.sRGBTexture = false;
                        packedImporter.SaveAndReimport(); // First apply main settings

                        packedImporter.isReadable = false; // ✅ Disable Read/Write after import
                        packedImporter.SaveAndReimport();  //  Apply again
                    }

                    EditorApplication.delayCall += () =>
                    {
                        if (atlasMat == null) return;

                        string relativePath = packedMaskPath.Substring(packedMaskPath.IndexOf("Assets"));
                        Texture2D refreshedPackedMaskAtlas = AssetDatabase.LoadAssetAtPath<Texture2D>(relativePath);

                        if (refreshedPackedMaskAtlas == null)
                        {
                            Debug.LogError("❌ Could not reload PackedMaskAtlas after import.");
                            return;
                        }

                        //Apply per-pipeline mask assignments
                        ApplyPackedMaskToMaterial(atlasMat, refreshedPackedMaskAtlas, emissionAtlas);

                    };



                    if (verboseLogging)
                        Debug.Log(" Packed Mask Atlas created and saved successfully.");


                }
            }
        }



        foreach (GameObject obj in selectedObjects)
        {
            if (obj == null)
            {
                Debug.LogWarning("⚠️ Skipped null object during mesh remap.");
                continue;
            }

            var mf = obj.GetComponentInChildren<MeshFilter>();
            var renderer = obj.GetComponentInChildren<MeshRenderer>();
            if (!mf || !renderer)
            {
                Debug.LogWarning($"⚠️ Skipped '{obj.name}' — missing MeshFilter or MeshRenderer.");
                continue;
            }

            var mesh = Instantiate(mf.sharedMesh);
            EnsureMeshLightingIntegrity(mesh);


            //Skip if no texture mapping was assigned for this object
            if (!objectTextureMap.ContainsKey(uvSourceProp) || !objectTextureMap[uvSourceProp].ContainsKey(obj))
            {
                if (verboseLogging)
                    Debug.LogWarning($" Skipping '{obj.name}' during UV remap — no texture assigned for {uvSourceProp}.");
                continue;
            }

            Vector2[] originalUVs = mesh.uv;

            if (originalUVs == null || originalUVs.Length != mesh.vertexCount)
            {
                if (verboseLogging)
                    Debug.LogWarning($"⚠️ Skipping '{obj.name}' — mesh has no valid UVs for remapping.");
                skippedUVObjects++;
                continue;
            }




            Vector2[] newUVs = new Vector2[originalUVs.Length];


            // Fallback to _BaseMap if UV source is missing
            string uvKey = uvSourceProp;

            string[] fallbackUVProps = { uvSourceProp, "_BaseMap", "_MainTex", "_BaseColorMap", "_MaskMap" };

            bool foundUV = false;
            foreach (string fallback in fallbackUVProps)
            {
                if (objectTextureMap.ContainsKey(fallback) && objectTextureMap[fallback].ContainsKey(obj))
                {
                    uvKey = fallback;
                    foundUV = true;
                    break;
                }
            }

            if (!foundUV)
            {
                Debug.LogWarning($"❌ UV source not found for {obj.name}, skipping UV remap.");
                continue;
            }



            Texture2D tex = objectTextureMap[uvKey][obj];

            if (!textureLookupByType.ContainsKey(uvKey) ||
                !textureLookupByType[uvKey].ContainsKey(tex.GetInstanceID()) ||
                !uvRectsByType.ContainsKey(uvKey) ||
                textureLookupByType[uvKey][tex.GetInstanceID()] >= uvRectsByType[uvKey].Length)
            {
                if (verboseLogging)
                    Debug.Log($"[UVUnify] Skipped remapping for '{obj.name}' (no texture mapping found).");
                continue;
            }

            int texIndex = textureLookupByType[uvKey][tex.GetInstanceID()];
            Rect rect = uvRectsByType[uvKey][texIndex];




            //Apply UV remap
            float shrinkX = uvExpandMargin / (float)atlasResolution;
            float shrinkY = uvExpandMargin / (float)atlasResolution;

            for (int i = 0; i < originalUVs.Length; i++)
            {
                Vector2 uv = originalUVs[i];
                uv.x = Mathf.Lerp(rect.xMin + shrinkX, rect.xMax - shrinkX, uv.x);
                uv.y = Mathf.Lerp(rect.yMin + shrinkY, rect.yMax - shrinkY, uv.y);
                newUVs[i] = uv;
            }

            mesh.uv = newUVs;



            //Organized folder for per-object meshes
            string meshFolder = organizeOutputFolders ? Path.Combine(saveFolderPath, "Meshes" + groupSuffix)
 : saveFolderPath;
            Directory.CreateDirectory(meshFolder);

            string safeName = obj.name.Replace("/", "_").Replace("\\", "_").Replace(":", "_");
            string meshPath = GetUniqueFilePath(meshFolder, safeName + "_AtlasMesh" + groupSuffix, ".asset");



            AssetDatabase.CreateAsset(mesh, meshPath);

            //Tag the mesh asset for organization
            AssetDatabase.SetLabels(mesh, new[] { "UVUnifyAtlas", "Generated" });

            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Mesh>(meshPath));

            if (verboseLogging)
                Debug.Log($" Mesh saved to: {meshPath}");

            AssetDatabase.SaveAssets();



            //Only create atlased duplicates if mesh combining, LODs, and atlas-only mode are all disabled
            if (!generateCombinedMesh && !generateLODs && !atlasOnlyMode)
            {
                GameObject newObj = Instantiate(obj, obj.transform.position, obj.transform.rotation, obj.transform.parent);
                newObj.name = obj.name + "_Atlased";

                var newMF = newObj.GetComponent<MeshFilter>();
                var newRenderer = newObj.GetComponent<MeshRenderer>();

                if (newMF != null) newMF.sharedMesh = mesh;
                if (newRenderer != null) newRenderer.sharedMaterial = atlasMat;

                Undo.RegisterCreatedObjectUndo(newObj, "Create Atlased Mesh Instance");

                if (verboseLogging)
                    Debug.Log($"Created new GameObject '{newObj.name}' with atlased mesh and material.");
            }
        }

        Debug.Log("All available texture atlases created and applied.");

        if (atlasOnlyMode)
        {
            if (verboseLogging)
                Debug.Log(" Atlas-only mode complete. Skipping mesh combining, prefab creation, and scene placement.");

            return (objectTextureMap, uvRectsByType);
        }

        GameObject combinedObj = null;

        if (generateCombinedMesh)
        {
            (Mesh mesh, GameObject tempObj) = CreateCombinedMeshAndPrefab(atlasMat);
            combinedMesh = mesh;
            combinedObj = tempObj;
            Selection.activeGameObject = combinedObj;
        }


        if (deleteOriginalsAfterAtlasing)
        {
            bool confirmDelete = EditorUtility.DisplayDialog(
                "Confirm Deletion",
                $"This will delete {selectedObjects.Count} original object(s) from your scene.\n\n" +
                "This cannot be undone unless you've saved your scene or use version control.\n\nProceed?",
                "Yes, Delete",
                "Cancel");

            if (confirmDelete)
            {
                foreach (var obj in selectedObjects)
                {
                    if (obj != null)
                        Undo.DestroyObjectImmediate(obj);
                }

                if (verboseLogging)
                    Debug.Log($"Deleted {selectedObjects.Count} object(s) after atlasing.");
            }
            else
            {
                if (verboseLogging)
                    Debug.Log("Deletion cancelled by user.");
            }
        }








        string lodReport = "";

        // Add LOD info if enabled
        if (generateLODs && numberOfLODs >= 2)
        {
            lodReport += "LOD Generation: Enabled\n";
            lodReport += "LOD Levels: " + numberOfLODs + " (includes LOD0)\n";

            if (selectedObjects.Count == 1)
                lodReport += "LOD Source: Single selected object\n";
            else
                lodReport += "LOD Source: Combined mesh from UVUnify\n";

            System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
            bool userCanceled = false;

            for (int i = 1; i < numberOfLODs; i++)
            {
                if (EditorUtility.DisplayCancelableProgressBar(
                    "Building LOD Report",
                    $"Processing LOD{i} of {numberOfLODs - 1}...",
                    i / (float)(numberOfLODs - 1)))
                {
                    Debug.LogWarning("❌ LOD report generation canceled by user.");
                    lodReport += "\nLOD Report was canceled by user.\n";
                    userCanceled = true;
                    break;
                }

                float reductionFactor = Mathf.Pow(0.5f, i);
                lodReport += $", LOD{i} ~ {Mathf.RoundToInt(reductionFactor * 100f)}%";
            }

            if (userCanceled)
                lodReport += "⚠️ LOD generation was canceled before completion.\n";

            EditorUtility.ClearProgressBar();
            stopwatch.Stop();

            int totalVerts = lodVertexCounts.Count > 0 ? lodVertexCounts[0] : 0;
            float actualSeconds = (float)stopwatch.Elapsed.TotalSeconds;
            float estimatedSeconds = (totalVerts / 10000f) * Mathf.Lerp(12f, 5f, Mathf.InverseLerp(2f, 16f, UVUnifySystemSpecs.GetCPUCores()));

            EditorPrefs.SetFloat("UVUnify_LastLODActual", actualSeconds);
            EditorPrefs.SetFloat("UVUnify_LastLODEstimate", estimatedSeconds);

            if (verboseLogging)
                Debug.Log($" Adaptive LOD Stats: actual = {actualSeconds:F1}s, estimate = {estimatedSeconds:F1}s");

            string prefabDir = organizeOutputFolders ? Path.Combine(saveFolderPath, "Prefabs") : saveFolderPath;
            Directory.CreateDirectory(prefabDir);

            string lodPrefabPath = Path.Combine(prefabDir, selectedObjects[0].name + "_LODGroup.prefab");
            lodReport += $"LOD Prefab Path: {lodPrefabPath}\n";
            lodReport += $"LOD Transparency Support: {(hasTransparency ? "Yes (Transparent Shader Applied)" : "No (Opaque Only)")}\n";
            lodReport += $"LOD Alpha Clipping: {(useAlphaClipping ? "Enabled" : "Disabled")}\n";
        }

        // Append LOD breakdown
        if (generateLODs && lodVertexCounts.Count > 0)
        {
            lodReport += "\nLOD Breakdown:\n";
            int lod0Verts = lodVertexCounts[0];
            int lod0Tris = lodTriangleCounts.Count > 0 ? lodTriangleCounts[0] : 0;

            for (int i = 0; i < lodVertexCounts.Count; i++)
            {
                int vert = lodVertexCounts[i];
                int tri = lodTriangleCounts.Count > i ? lodTriangleCounts[i] : 0;
                float vertPct = lod0Verts > 0 ? (vert / (float)lod0Verts) * 100f : 0f;
                float triPct = lod0Tris > 0 ? (tri / (float)lod0Tris) * 100f : 0f;
                float err = lodErrorRates.Count > i ? lodErrorRates[i] * 100f : 0f;
                float mem = lodMemorySizes.Count > i ? lodMemorySizes[i] / (1024f * 1024f) : 0f;

                lodReport += $"- LOD{i}: {vert:N0} verts ({vertPct:F1}%), {tri:N0} tris ({triPct:F1}%), Error {err:F1}% | {mem:F2} MB\n";
            }

            if (lodVertexCounts.Count >= 3)
            {
                float lod1Pct = lodVertexCounts[1] / (float)lodVertexCounts[0] * 100f;
                float lod2Pct = lodVertexCounts[2] / (float)lodVertexCounts[0] * 100f;

                if (lod1Pct > 90f && lod2Pct > 85f)
                {
                    lodReport += $"\n📌 All LODs are close to LOD0 in complexity:\n" +
                                 $"- LOD1: {lod1Pct:F1}% of LOD0\n" +
                                 $"- LOD2: {lod2Pct:F1}% of LOD0\n" +
                                 $"Simplification may help performance, but this is fine if quality is preserved.\n";
                }
            }
        }






        //Add UV2 reporting (even if LODs are off)
        if (generateUV2)
        {
            bool hasValidMeshFilters = selectedObjects.Any(obj => obj && obj.GetComponentInChildren<MeshFilter>() != null);
            bool validUV2Conditions = generateCombinedMesh && selectedObjects.Count > 1 && hasValidMeshFilters;
            bool hasValidUV2 = combinedMesh != null && combinedMesh.uv2 != null && combinedMesh.uv2.Length == combinedMesh.vertexCount;

            lodReport += "\nUV2 Generation:\n";
            lodReport += $"- Enabled: {generateUV2}\n";
            lodReport += $"- Conditions Met: {validUV2Conditions}\n";
            lodReport += $"- UV2 Present on Combined Mesh: {hasValidUV2}\n";
        }

        // 📝 Write Summary Report
        if (!atlasOnlyMode && generateSummaryReport)
        {
            string report = "";
            report += "===============================\n";
            report += "📝 UVUnify Texture Atlas Report\n";
            report += "===============================\n";
            report += $"Generated On: {System.DateTime.Now}\n\n";

            report += $"Tool Version: {VERSION}\n";
            // 📁 Output Info
            report += $"Output Folder: {saveFolderPath}\n";
            report += $"Material Asset: CombinedAtlas_Material{groupSuffix}.mat\n";
            report += $"Group Type: {(hasTransparency ? "Transparent" : "Opaque")}\n";
            report += $"Group Suffix: {groupSuffix}\n";
            report += $"Processed Objects: {selectedObjects.Count}\n";
            report += $"Transparent Objects Detected: {transparentObjectsDetected.Count}\n\n";

            // 🎨 Material Info
            report += "== Material Shader & Transparency ==\n";
            report += $"Shader Pipeline: {selectedPipeline}\n";
            report += $"Shader Used: {atlasMat?.shader?.name ?? "Unknown"}\n";
            report += $"Material Transparency: {(hasTransparency ? "Transparent" : "Opaque")}\n";
            report += $"Auto-Set Transparency: {(autoSetTransparency ? "Yes" : "No")}\n";
            report += $"Alpha Clipping: {(useAlphaClipping ? "Enabled" : "Disabled")}\n";
            report += $"Render Queue: {(hasTransparency ? "3000 (Transparent)" : "2000 (Opaque)")}\n";
            report += $"Surface Type (_Surface): {(hasTransparency ? "1" : "0")}\n\n";

            // 📦 Mesh Info
            report += "== Meshes & Atlas Output ==\n";
            report += combinedMesh != null
                ? $"Combined Mesh Vertex Count: {combinedMesh.vertexCount:N0}\n"
                : "Combined Mesh Vertex Count: (Not Generated)\n";
            report += $"Estimated Draw Call Reduction: {selectedObjects.Count} ➝ {(generateCombinedMesh ? 1 : selectedObjects.Count)}\n";
            report += $"Atlas Resolution: {atlasResolution}px\n";
            report += $"Atlas Padding: {atlasPadding}px\n";
            report += $"UV Expand Margin: {uvExpandMargin}px\n";
            report += $"UV Source Map: {uvSourceProp}\n\n";

            // 🧬 Map Detection
            report += "== Maps Detected ==\n";
            report += $"Albedo: {hasAlbedo}\n";
            report += $"Normal: {hasNormal}\n";
            report += $"Metallic: {hasMetallic}\n";
            report += $"AO: {hasAO}\n";
            report += $"Height: {hasHeight}\n";
            report += $"Emission: {hasEmission}\n";
            report += $"Packed Mask Map: {(generatePackedMaskMap ? "Enabled" : "Disabled")}\n\n";

            // ⚙️ Feature Toggles
            report += "== Features Enabled ==\n";
            report += $"Combine Meshes: {(generateCombinedMesh ? "Yes" : "No")}\n";
            report += $"Generate Prefab: {(generatePrefab ? "Yes" : "No")}\n";
            report += $"LOD Generation: {(generateLODs ? $"Yes ({numberOfLODs} levels)" : "No")}\n";

            // 🌞 UV2 Info (URP baked lighting)
            if (generateUV2)
            {
                bool hasValidMeshFilters = selectedObjects.Any(obj => obj && obj.GetComponentInChildren<MeshFilter>() != null);
                bool validUV2Conditions = generateCombinedMesh && selectedObjects.Count > 1 && hasValidMeshFilters;
                bool hasValidUV2 = combinedMesh != null && combinedMesh.uv2 != null && combinedMesh.uv2.Length == combinedMesh.vertexCount;

                report += "\n== UV2 (Lightmapping) ==\n";
                report += $"- UV2 Generation Enabled: Yes\n";
                report += $"- Conditions Met: {(validUV2Conditions ? "Yes" : "No")}\n";
                report += $"- UV2 Present on Combined Mesh: {(hasValidUV2 ? "Yes" : "No")}\n";
            }

            // 🧠 LOD Report (already built elsewhere)
            if (!string.IsNullOrEmpty(lodReport))
            {
                report += "\n== LOD Summary ==\n";
                report += lodReport;
            }

            // 🚫 Skipped Maps
            report += "\n== Skipped Maps ==\n";
            if (!hasAlbedo) report += "- Albedo\n";
            if (!hasNormal) report += "- Normal\n";
            if (!hasMetallic) report += "- Metallic\n";
            if (!hasAO) report += "- Ambient Occlusion\n";
            if (!hasHeight) report += "- Height\n";
            if (!hasEmission) report += "- Emission\n";


            // 🎛️ Manual HDRP Packed Mask Composer
            if (selectedPipeline == ShaderPipeline.HDRP)
            {
                report += "\n== HDRP Packed Mask Composer ==\n";
                report += $"- Manual Mask Generation Used: {(manualPackedMaskGenerated ? "Yes" : "No")}\n";
                report += $"- Metallic (R): {(hdrpMetallicTex ? hdrpMetallicTex.name : "None")}\n";
                report += $"- AO (G): {(hdrpAOTexture ? hdrpAOTexture.name : "None")}\n";
                report += $"- Height (B): {(hdrpHeightTexture ? hdrpHeightTexture.name : "None")}\n";
                report += $"- Smoothness (A): {(hdrpEmissionTexture ? hdrpEmissionTexture.name : "None")}\n";
            }


            // 💾 Save It
            string reportFolder = organizeOutputFolders ? Path.Combine(saveFolderPath, "Reports") : saveFolderPath;
            Directory.CreateDirectory(reportFolder);

            string summaryPath = GetUniqueFilePath(reportFolder, "AtlasSummary" + groupSuffix, ".txt");
            File.WriteAllText(summaryPath, report);
            AssetDatabase.Refresh();

            if (verboseLogging)
                Debug.Log($"📄 Summary Report saved to: {summaryPath}");
        }




        if (verboseLogging && skippedUVObjects > 0)
            Debug.LogWarning(Emoji("⚠️") + $"{skippedUVObjects} object(s) were skipped due to missing or invalid UVs.");


        //Correct final return
        return (objectTextureMap, uvRectsByType);


    }


    private Texture2D GenerateTransparentAtlas(List<Texture2D> textures, int atlasResolution, int padding, out Rect[] uvs)
    {
        // Reimport all textures to ensure alpha is preserved and readable
        foreach (Texture2D tex in textures)
        {
            string path = AssetDatabase.GetAssetPath(tex);
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Default;
                importer.alphaSource = TextureImporterAlphaSource.FromInput;
                importer.alphaIsTransparency = true;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.isReadable = true;
                importer.mipmapEnabled = false; // Prevents mipmap bleeding
                importer.wrapMode = TextureWrapMode.Clamp;
                importer.filterMode = FilterMode.Point; // Prevents linear bleeding
                importer.sRGBTexture = true;

                EditorUtility.SetDirty(importer);
                importer.SaveAndReimport();

                Debug.Log($"🔁 Reimported {tex.name} with alpha support + filtering fixes");
            }
        }

        // Create transparent-filled background
        Texture2D atlas = new Texture2D(atlasResolution, atlasResolution, TextureFormat.RGBA32, mipChain: false, linear: false);
        Color clear = new Color(0f, 0f, 0f, 0f);
        atlas.SetPixels(Enumerable.Repeat(clear, atlasResolution * atlasResolution).ToArray());

        // Use padding to prevent color bleeding
        uvs = atlas.PackTextures(textures.ToArray(), padding, atlasResolution);

        // Set filtering and wrapping to prevent texture artifacts
        atlas.wrapMode = TextureWrapMode.Clamp;
        atlas.filterMode = FilterMode.Point;

        // Apply changes
        atlas.Apply(updateMipmaps: false, makeNoLongerReadable: false);

        Debug.Log($"[TransparentAtlas] Packed {textures.Count} textures. Alpha at (0,0): {atlas.GetPixel(0, 0).a}");
        return atlas;
    }



    #endregion

    #region Transparent and Opaque Group Processing
    private void RunAtlasPipelineForGroup(List<GameObject> group, bool isTransparent)
    {
        // ✅ Skip atlas/material generation if we're only doing LODs from a single object
        if (generateLODs &&
            !generateCombinedMesh &&
            !generatePackedMaskMap &&
            !generatePrefab &&
            !generateSummaryReport &&
            !generateUV2 &&
            group.Count == 1)
        {
            if (verboseLogging)
                Debug.Log("🛑 Skipping atlas/material generation — LOD-only generation detected.");
            return;
        }

        if (group == null || group.Count == 0) return;

        var originalSelection = selectedObjects;
        selectedObjects = group;

        hasTransparency = isTransparent;
        forceTransparentMaterial = isTransparent;

        if (verboseLogging)
            Debug.Log($"Running atlas group: {(isTransparent ? "TRANSPARENT" : "OPAQUE")}, Objects: {group.Count}");


        if (isTransparent && verboseLogging)
        {
            Debug.Log($"️ '{group[0].name}' group is being treated as Transparent because of texture alpha.");
        }



        groupSuffix = isTransparent ? "_Transparent" : "_Opaque";

        // 🧩 Force-correct UV source based on pipeline (before generation)
        var validProps = new[] { "_BaseMap", "_MainTex", "_BaseColorMap" };
        if (!validProps.Contains(uvSourceProp))
        {
            uvSourceProp = selectedPipeline switch
            {
                ShaderPipeline.URP => "_BaseMap",
                ShaderPipeline.HDRP => "_BaseColorMap",
                _ => "_BaseMap"
            };

        }



        var result = GenerateAllTextureAtlases();
        objectTextureMap = result.objectTextureMap;
        uvRectsByType = result.uvRectsByType;

        selectedObjects = originalSelection;
        groupSuffix = ""; // reset after use
    }




    private void ProcessTransparentAndOpaqueGroups()
    {

        // ✅ Start full generation timer
        System.Diagnostics.Stopwatch fullTimer = System.Diagnostics.Stopwatch.StartNew();

        showLODHelpboxAfterGeneration = false; // reset for new run

        if (generateLODs && selectedObjects.Count != 1)
        {
            EditorUtility.DisplayDialog(
                "LOD Generation Error",
                "⚠️ LOD Generation is only supported when:\n• A combined mesh was just generated by this tool\n• OR exactly one object with a valid MeshFilter is selected\n\nMultiple uncombined objects are not supported for LOD creation.",
                "OK"
            );
            Debug.LogWarning("❌ LOD Generation canceled: Multiple uncombined objects selected.");
        }

        // Validate save folder path before starting
        if (string.IsNullOrEmpty(saveFolderPath))
        {
            EditorUtility.DisplayDialog(
                "Save Folder Missing or Invalid",
                "Please select a valid folder inside the Assets/ directory before generating atlases.\n\n" +
                "This ensures that all atlases, meshes, and prefabs are saved correctly.",
                "OK"
            );
            return;
        }

        List<GameObject> opaqueObjects = new List<GameObject>();
        List<GameObject> transparentObjects = new List<GameObject>();

        foreach (GameObject obj in selectedObjects)
        {
            if (obj == null)
            {
                Debug.LogWarning("⚠️ Null object detected in selection list. Skipping.");
                continue;
            }

            var renderer = obj.GetComponent<MeshRenderer>();
            var mat = renderer != null ? renderer.sharedMaterial : null;
            Texture tex = null;
            if (mat != null)
            {
                if (mat.HasProperty("_BaseMap"))
                    tex = mat.GetTexture("_BaseMap");
                else if (mat.HasProperty("_BaseColorMap"))
                    tex = mat.GetTexture("_BaseColorMap");
            }

            if (IsObjectTransparent(obj))
                transparentObjects.Add(obj);
            else
                opaqueObjects.Add(obj);
        }

        if (opaqueObjects.Count > 0)
        {
            if (verboseLogging)
                Debug.Log("Processing opaque objects...");

            RunAtlasPipelineForGroup(opaqueObjects, false);
        }
        if (generateLODs && opaqueObjects.Count == 1)
        {
            var obj = opaqueObjects[0];
            var mf = obj.GetComponent<MeshFilter>();
            var mr = obj.GetComponent<MeshRenderer>();

            if (mf != null && mr != null)
            {
                var mesh = mf.sharedMesh;
                var mat = mr.sharedMaterial;

                if (mesh != null && mat != null)
                {
                    if (verboseLogging)
                        Debug.Log("✅ Generating LODs for opaque group object.");
                    GenerateLODsForCombinedMesh(mesh, mat, obj);
                }
            }
        }

        if (generateLODs && transparentObjects.Count == 1)
        {
            var obj = transparentObjects[0];
            var mf = obj.GetComponent<MeshFilter>();
            var mr = obj.GetComponent<MeshRenderer>();

            if (mf != null && mr != null)
            {
                var mesh = mf.sharedMesh;
                var mat = mr.sharedMaterial;

                if (mesh != null && mat != null)
                {
                    if (verboseLogging)
                        Debug.Log("✅ Generating LODs for transparent group object.");
                    GenerateLODsForCombinedMesh(mesh, mat, obj);
                }
            }
        }




        if (transparentObjects.Count > 0)
        {
            if (verboseLogging)
                Debug.Log("Processing transparent objects...");

            RunAtlasPipelineForGroup(transparentObjects, true);
        }

        // ✅ Clear selection after generating
        if (clearAfterGeneration)
        {
            selectedObjects.Clear();
            transparentObjectsDetected.Clear();
            RefreshSelectedObjects();
            if (verboseLogging)
                Debug.Log("Cleared selected objects after generation.");
        }

        // ✅ Ensure Unity finishes its final import operations
        AssetDatabase.Refresh();

        // ✅ Stop and log total generation time
        fullTimer.Stop();
        Debug.Log($"🌍 Total generation time: {fullTimer.Elapsed.TotalSeconds:F1} seconds");
        // ✅ User-facing popup
        EditorUtility.DisplayDialog(
       "UVUnify Generation Complete",
       $"All assets processed successfully.\n\nTotal Time: {fullTimer.Elapsed.TotalSeconds:F1} seconds.",
       "OK"
   );
    }




    #endregion

    #region Mesh Combining and Prefab Creation
    private (Mesh, GameObject) CreateCombinedMeshAndPrefab(Material atlasMat)

    {
        // ✅ Clean declaration block — do not reuse existing vars
        List<MeshFilter> filters = new List<MeshFilter>();
        Bounds groupBounds = new Bounds(Vector3.zero, Vector3.zero);
        bool boundsInitialized = false;

        foreach (GameObject obj in selectedObjects)
        {
            var mf = obj.GetComponent<MeshFilter>();
            if (mf != null)
            {
                filters.Add(mf);

                if (!boundsInitialized)
                {
                    groupBounds = new Bounds(mf.transform.position, Vector3.zero);
                    boundsInitialized = true;
                }
                else
                {
                    groupBounds.Encapsulate(mf.transform.position);
                }
            }
        }

        Vector3 groupCenter = groupBounds.center;



        if (filters.Count == 0)
        {
            Debug.LogWarning("⚠️ No MeshFilters found to combine.");
            return (null, null);
        }



        //Step 1: Build combine list
        List<CombineInstance> combineList = new List<CombineInstance>();

        foreach (MeshFilter mf in filters)
        {
            if (mf == null || mf.sharedMesh == null)
            {
                Debug.LogWarning("⚠️ Skipped MeshFilter — missing mesh.");
                continue;
            }

            Mesh meshCopy = UnityEngine.Object.Instantiate(mf.sharedMesh);

            // Step 2: Remap UVs based on atlas Rect
            Vector2[] uvs = meshCopy.uv;
            if (uvs != null && uvs.Length > 0)
            {
                GameObject obj = mf.gameObject;
                string[] fallbackUVProps = { uvSourceProp, "_BaseMap", "_MainTex", "_BaseColorMap", "_MaskMap" };
                string uvKey = null;

                foreach (string fallback in fallbackUVProps)
                {
                    if (objectTextureMap.ContainsKey(fallback) && objectTextureMap[fallback].ContainsKey(obj))
                    {
                        uvKey = fallback;
                        break;
                    }
                }

                if (string.IsNullOrEmpty(uvKey))
                {
                    Debug.LogWarning($"❌ UV source not found for {obj.name}, skipping UV remap.");
                    continue;
                }


                //SAFER UV RECT RESOLUTION
                Rect rect = default;
                bool found = false;

                if (objectTextureMap.ContainsKey(uvKey) && uvRectsByType.ContainsKey(uvKey))
                {
                    Texture2D tex = objectTextureMap[uvKey][obj];
                    List<Texture2D> atlasTextures = objectTextureMap[uvKey].Values.Distinct().ToList();
                    int textureIndex = atlasTextures.IndexOf(tex);

                    if (textureIndex >= 0 && textureIndex < uvRectsByType[uvKey].Length)
                    {
                        rect = uvRectsByType[uvKey][textureIndex];
                        found = true;
                    }
                }

                if (!found)
                {
                    Debug.LogWarning($"⚠️ Failed to find UV rect for {obj.name}. Skipping.");
                    continue;
                }

                for (int j = 0; j < uvs.Length; j++)
                {
                    uvs[j].x = rect.x + uvs[j].x * rect.width;
                    uvs[j].y = rect.y + uvs[j].y * rect.height;
                }
                meshCopy.uv = uvs;
            }


            //Apply position offset to mesh vertices manually
            Vector3[] vertices = meshCopy.vertices;
            for (int i = 0; i < vertices.Length; i++)
            {
                // Convert to world space
                vertices[i] = mf.transform.TransformPoint(vertices[i]);

                // Offset back to centered group space
                vertices[i] -= groupCenter;
            }
            meshCopy.vertices = vertices;
            meshCopy.RecalculateBounds();


            CombineInstance ci = new CombineInstance
            {
                mesh = meshCopy,
                transform = Matrix4x4.identity // ✅ No world-space transform
            };
            combineList.Add(ci);
        }

        if (combineList.Count == 0)
        {
            Debug.LogError("❌ No valid meshes to combine. Aborting.");
            return (null, null);
        }

        // Combine into a single mesh
        int totalVertices = combineList.Sum(ci => ci.mesh != null ? ci.mesh.vertexCount : 0);

        // ✅ Create the mesh object first
        Mesh combinedMesh = new Mesh();
        combinedMesh.name = "CombinedMesh";

        // ✅ Now it's safe to assign index format
        if (totalVertices > 65535)
        {
            combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            Debug.LogWarning($"⚠️ Combined mesh has {totalVertices:N0} vertices — exceeding Unity's 16-bit index limit. Switched to 32-bit IndexFormat automatically.");
        }
        else
        {
            combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt16;
        }

        combinedMesh.CombineMeshes(combineList.ToArray(), true, true, false);
        if (generateUV2 && combinedMesh != null && combinedMesh.uv != null && combinedMesh.uv.Length == combinedMesh.vertexCount)
        {
            combinedMesh.uv2 = combinedMesh.uv;
            if (verboseLogging)
                Debug.Log("✅ UV2 assigned from combined UVs for lightmapping.");
        }

        EnsureMeshLightingIntegrity(combinedMesh);



        // ✅ Position for final GameObject
        Vector3 averageWorldPosition = groupCenter;

        if (generateUV2)
        {
            UnwrapParam unwrapParams;
            UnwrapParam.SetDefaults(out unwrapParams);
            unwrapParams.hardAngle = 60;
            unwrapParams.packMargin = 4; // 4 pixels is a Unity default

            Unwrapping.GenerateSecondaryUVSet(combinedMesh, unwrapParams);
            if (verboseLogging)
                Debug.Log("UV2 channel generated for lightmapping.");
        }




        if (combinedMesh.vertexCount == 0)
        {
            Debug.LogError("❌ Combined mesh has 0 vertices! Aborting prefab creation.");
            return (null, null);
        }
        else
        {
            Debug.Log($"✅ Combined mesh generated: {combinedMesh.vertexCount} vertices.");
        }



        //  Handle 65k vertex warning
        if (combinedMesh.vertexCount > 65535)
        {
            Debug.LogWarning($"⚠️ Combined mesh exceeds 65k vertices ({combinedMesh.vertexCount}). Consider splitting objects.");
            EditorUtility.DisplayDialog("Vertex Limit Warning",
                $"The combined mesh has {combinedMesh.vertexCount:N0} vertices.\n\nUnity's default limit is 65,535 per mesh.",
                "OK");
        }

        //  Save Combined Mesh Asset
        string combinedMeshFolder = organizeOutputFolders ? Path.Combine(saveFolderPath, "Meshes") : saveFolderPath;
        Directory.CreateDirectory(combinedMeshFolder);

        string meshBaseName = useObjectNameForPrefab && selectedObjects.Count > 0
            ? selectedObjects[0].name
            : "CombinedMeshObject";

        string safeMeshName = SanitizeString(meshBaseName);

        string combinedMeshPath = GetUniqueFilePath(combinedMeshFolder, safeMeshName + "_Mesh" + groupSuffix, ".asset");
        AssetDatabase.CreateAsset(combinedMesh, combinedMeshPath);
        AssetDatabase.SaveAssets();

        //  Center the mesh around its own bounds center
        combinedMesh.RecalculateBounds();
        GameObject combinedObject = new GameObject("CombinedMeshObject");
        Undo.RegisterCreatedObjectUndo(combinedObject, "Create Combined Mesh Object");


        combinedObject.transform.position = averageWorldPosition;






        MeshFilter mfNew = combinedObject.AddComponent<MeshFilter>();
        MeshRenderer mrNew = combinedObject.AddComponent<MeshRenderer>();



        mfNew.sharedMesh = combinedMesh;
        if (generateUV2)
            Debug.Log($"UV2 length on final assigned mesh: {combinedMesh.uv2?.Length}");
        // ✅ Enforce Point+Clamp to prevent texture edge bleeding
        Texture albedoTex = null;
        if (atlasMat.HasProperty("_BaseMap"))
            albedoTex = atlasMat.GetTexture("_BaseMap");
        else if (atlasMat.HasProperty("_MainTex"))
            albedoTex = atlasMat.GetTexture("_MainTex");

        if (albedoTex is Texture2D tex2D)
        {
            tex2D.filterMode = FilterMode.Point;
            tex2D.wrapMode = TextureWrapMode.Clamp;
            Debug.Log($"✅ Applied filterMode=Point and wrapMode=Clamp to {tex2D.name}");
        }

        mrNew.sharedMaterial = atlasMat;

        if (verboseLogging)
            Debug.Log("✅ Combined mesh created and UVs correctly remapped.");



        //  Prefab Saving Logic
        if (generatePrefab)
        {
            string prefabDir = organizeOutputFolders ? Path.Combine(saveFolderPath, "Prefabs") : saveFolderPath;
            Directory.CreateDirectory(prefabDir);

            string prefabBaseName = SanitizeString(meshBaseName);
            int counter = 1;
            string prefabPath;

            do
            {
                prefabPath = Path.Combine(prefabDir, $"{prefabBaseName}{groupSuffix}_{counter}.prefab");
                counter++;
            } while (File.Exists(prefabPath));

            prefabPath = AssetDatabase.GenerateUniqueAssetPath(prefabPath);

            PrefabUtility.SaveAsPrefabAsset(combinedObject, prefabPath);

            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath));

            if (verboseLogging)
                Debug.Log($"📦 Prefab saved to: {prefabPath}");



            if (placePrefabInScene)
            {
                GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefabAsset);

                Undo.RegisterCreatedObjectUndo(instance, "Place Prefab in Scene");


                // ✅ Place prefab where combined mesh was zero-centered
                instance.transform.position = averageWorldPosition;



                if (combinedObject != null)
                {
                    UnityEngine.Object.DestroyImmediate(combinedObject);
                }
            }
        }

        // ✅ Only mark as Static if UV2 was generated AND the combined object still exists
        if (generateUV2 && autoMarkStaticForUV2 && combinedObject != null)
        {
            GameObjectUtility.SetStaticEditorFlags(combinedObject, StaticEditorFlags.ContributeGI);
            if (verboseLogging)
                Debug.Log("Marked combined object as Lightmap Static.");
        }

        return (combinedMesh, combinedObject);

    }


    // ========================================================
    //  LOD Generation - Internal Collapse System
    // ========================================================

    private void GenerateLODsForCombinedMesh(Mesh originalMesh, Material sharedMaterial, GameObject sourceObject)
    {
        if (originalMesh == null || sharedMaterial == null || sourceObject == null || numberOfLODs < 2)
        {
            Debug.LogWarning("⚠️ Cannot generate LODs: Invalid input.");
            return;
        }

        System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
        int originalTriangleCount = originalMesh.triangles.Length / 3;

        lodTriangleCounts.Clear();
        lodErrorRates.Clear();
        lodMemorySizes.Clear();

        GameObject selectedObject = sourceObject;
        MeshFilter filter = selectedObject.GetComponent<MeshFilter>() ?? selectedObject.GetComponentInChildren<MeshFilter>();
        if (filter == null)
        {
            Debug.LogError("❌ No MeshFilter found.");
            return;
        }

        string lodMeshFolder = organizeOutputFolders ? Path.Combine(saveFolderPath, "Meshes") : saveFolderPath;
        Directory.CreateDirectory(lodMeshFolder);

        GameObject lodParent = new GameObject(selectedObject.name + "_LODGroup");
        lodParent.transform.position = sourceObject.transform.position + lastCombinedMeshOffset;
        lodParent.transform.rotation = sourceObject.transform.rotation;
        lodParent.transform.localScale = sourceObject.transform.localScale;

        List<LOD> lods = new List<LOD>();

        // LOD0
        Mesh lod0Mesh = UnityEngine.Object.Instantiate(originalMesh);
        EnsureMeshLightingIntegrity(lod0Mesh);

        if (selectedPipeline == ShaderPipeline.Standard)
            StripAlphaFromMetallicGloss(sharedMaterial);

        if (selectedPipeline == ShaderPipeline.Standard)
        {
            lod0Mesh.RecalculateNormals();
            lod0Mesh.RecalculateTangents();
            lod0Mesh.RecalculateBounds();

            if (verboseLogging)
                Debug.Log("🔧 Recalculated LOD0 normals/tangents for Standard pipeline.");
        }

        string lod0MeshPath = GetUniqueFilePath(lodMeshFolder, selectedObject.name + "_LOD0", ".asset");
        AssetDatabase.CreateAsset(lod0Mesh, lod0MeshPath);
        AssetDatabase.SaveAssets();

        GameObject lod0 = new GameObject(selectedObject.name + "_LOD0");
        lod0.transform.parent = lodParent.transform;
        lod0.transform.localPosition = Vector3.zero;
        lod0.transform.localRotation = Quaternion.identity;
        lod0.transform.localScale = Vector3.one;

        MeshFilter mf0 = lod0.AddComponent<MeshFilter>();
        MeshRenderer mr0 = lod0.AddComponent<MeshRenderer>();
        mf0.sharedMesh = lod0Mesh;
        mr0.sharedMaterial = sharedMaterial;

        lods.Add(new LOD(0.6f, new[] { mr0 }));
        lodVertexCounts.Clear();
        lodVertexCounts.Add(lod0Mesh.vertexCount);
        lodTriangleCounts.Add(lod0Mesh.triangles.Length / 3);
        lodErrorRates.Add(0f);
        lodMemorySizes.Add(Profiler.GetRuntimeMemorySizeLong(lod0Mesh));

        for (int i = 1; i < numberOfLODs; i++)
        {
            float progress = i / (float)(numberOfLODs - 1);

            if (EditorUtility.DisplayCancelableProgressBar(
                "Generating LOD Meshes",
                $"Simplifying LOD{i} of {numberOfLODs - 1}...",
                progress))
            {
                Debug.LogWarning("❌ LOD mesh generation canceled by user.");
                EditorUtility.ClearProgressBar();
                return;
            }

            float reductionFactor = Mathf.Pow(0.5f, i);
            int targetTriangles = Mathf.FloorToInt(originalMesh.triangles.Length / 3 * reductionFactor);

            Vector3 centerOffset;
            Mesh lodMesh = CreateSimplifiedMesh(originalMesh, targetTriangles, out centerOffset);

            if (lodMesh == null || lodMesh.vertexCount == 0)
            {
                Debug.LogWarning($"⚠️ Skipped LOD{i}: Simplified mesh invalid.");
                continue;
            }

            lodVertexCounts.Add(lodMesh.vertexCount);
            lodTriangleCounts.Add(lodMesh.triangles.Length / 3);
            float err = 1f - (lodMesh.triangles.Length / 3f) / originalTriangleCount;
            lodErrorRates.Add(err);
            lodMemorySizes.Add(Profiler.GetRuntimeMemorySizeLong(lodMesh));

            string lodMeshPath = GetUniqueFilePath(lodMeshFolder, selectedObject.name + $"_LOD{i}", ".asset");
            AssetDatabase.CreateAsset(lodMesh, lodMeshPath);
            AssetDatabase.SaveAssets();

            GameObject lodChild = new GameObject(selectedObject.name + $"_LOD{i}");
            lodChild.transform.parent = lodParent.transform;
            lodChild.transform.localPosition = Vector3.zero;
            lodChild.transform.localRotation = Quaternion.identity;
            lodChild.transform.localScale = Vector3.one;

            FixLODScale(lodChild, selectedObject);

            MeshFilter mf = lodChild.AddComponent<MeshFilter>();
            MeshRenderer mr = lodChild.AddComponent<MeshRenderer>();
            mf.sharedMesh = lodMesh;
            mr.sharedMaterial = sharedMaterial;

            float transitionHeight = 0.6f / (i + 1);
            lods.Add(new LOD(transitionHeight, new[] { mr }));
        }

        EditorUtility.ClearProgressBar();
        stopwatch.Stop();

        if (stopwatch.Elapsed.TotalSeconds > 5)
        {
            Debug.Log($" LOD mesh generation completed in {stopwatch.Elapsed.TotalSeconds:F1} seconds.");
        }

        LODGroup lodGroup = lodParent.AddComponent<LODGroup>();
        lodGroup.SetLODs(lods.ToArray());
        lodGroup.RecalculateBounds();
        lodGroup.fadeMode = LODFadeMode.CrossFade;
        lodGroup.animateCrossFading = true;

        string lodPrefabFolder = organizeOutputFolders ? Path.Combine(saveFolderPath, "Prefabs") : saveFolderPath;
        Directory.CreateDirectory(lodPrefabFolder);
        string lodPrefabPath = GetUniqueFilePath(lodPrefabFolder, lodParent.name, ".prefab");
        PrefabUtility.SaveAsPrefabAsset(lodParent, lodPrefabPath);

        GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(lodPrefabPath);
        if (prefabAsset != null)
        {
            Selection.activeObject = prefabAsset;
            EditorGUIUtility.PingObject(prefabAsset);

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefabAsset);
            instance.transform.position = selectedObject.transform.position;
            instance.transform.rotation = selectedObject.transform.rotation;
            instance.transform.localScale = selectedObject.transform.localScale;
        }

        UnityEngine.Object.DestroyImmediate(lodParent);
        showLODHelpboxAfterGeneration = true;
        Debug.Log("<color=lime>✅ LOD prefab saved and instantiated.</color>");
    }


    private Mesh CreateSimplifiedMesh(Mesh originalMesh, int targetTriangleCount, out Vector3 centerOffset)

    {
        if (originalMesh == null || targetTriangleCount < 10)
        {
            Debug.LogWarning("⚠️ Invalid mesh or triangle target.");
            centerOffset = Vector3.zero;
            return null;
        }


        Vector3[] originalVertices = originalMesh.vertices;
        Vector3[] originalNormals = originalMesh.normals;
        Vector2[] originalUVs = originalMesh.uv;
        int[] originalTriangles = originalMesh.triangles;

        Dictionary<int, VertexRecord> verts = new();
        Dictionary<int, TriangleRecord> tris = new();
        SortedSet<EdgeRecord> edgeQueue = new();                  // ✅ priority queue
        Dictionary<(int, int), EdgeRecord> edgeMap = new();       // ✅ direct lookup
        Dictionary<int, HashSet<EdgeRecord>> vertexEdges = new(); // ✅ per-vertex edge tracking


        for (int v = 0; v < originalVertices.Length; v++)
            verts[v] = new VertexRecord(v, originalVertices[v],
                (originalUVs.Length > v ? originalUVs[v] : Vector2.zero),
                (originalNormals.Length > v ? originalNormals[v] : Vector3.up));

        for (int t = 0; t < originalTriangles.Length; t += 3)
        {
            int id = t / 3;
            var tri = new TriangleRecord(id, originalTriangles[t], originalTriangles[t + 1], originalTriangles[t + 2]);
            tris[id] = tri;

            verts[tri.v0].adjacentTriangles.Add(id);
            verts[tri.v1].adjacentTriangles.Add(id);
            verts[tri.v2].adjacentTriangles.Add(id);

            AddEdge(edgeQueue, edgeMap, tri.v0, tri.v1, verts, vertexEdges);
            AddEdge(edgeQueue, edgeMap, tri.v1, tri.v2, verts, vertexEdges);
            AddEdge(edgeQueue, edgeMap, tri.v2, tri.v0, verts, vertexEdges);

        }

        int currentTris = tris.Count;
        int loopGuard = 0;

        while (currentTris > targetTriangleCount && edgeQueue.Count > 0 && loopGuard++ < 10000)
        {
            EdgeRecord bestEdge = edgeQueue.Min;
            if (!CollapseEdge(bestEdge, verts, tris, edgeQueue, edgeMap, vertexEdges))
                break;

            currentTris = tris.Values.Count(t => t.isValid);
        }


        Dictionary<int, int> indexRemap = new();
        List<Vector3> finalVerts = new();
        List<Vector2> finalUVs = new();
        int counter = 0;

        foreach (var v in verts.Values)
        {
            indexRemap[v.originalIndex] = counter++;
            finalVerts.Add(v.position);
            finalUVs.Add(v.uv);
        }

        List<int> finalTris = new();
        foreach (var tri in tris.Values)
        {
            if (!tri.isValid) continue;
            finalTris.Add(indexRemap[tri.v0]);
            finalTris.Add(indexRemap[tri.v1]);
            finalTris.Add(indexRemap[tri.v2]);
        }

        Mesh simplified = new Mesh();
        simplified.SetVertices(finalVerts);
        simplified.SetUVs(0, finalUVs);
        simplified.SetTriangles(finalTris, 0);
        simplified.RecalculateNormals();
        simplified.RecalculateBounds();

        centerOffset = simplified.bounds.center;



        return simplified;
    }
    private void FixLODScale(GameObject lodObject, GameObject reference)
    {
        if (lodObject == null || reference == null) return;

        Transform lodTransform = lodObject.transform;
        Transform refTransform = reference.transform;

        Vector3 refScale = refTransform.lossyScale;
        Vector3 lodParentScale = lodTransform.parent != null ? lodTransform.parent.lossyScale : Vector3.one;

        lodTransform.localScale = new Vector3(
            refScale.x / lodParentScale.x,
            refScale.y / lodParentScale.y,
            refScale.z / lodParentScale.z
        );
    }

    private void AddEdge(
     SortedSet<EdgeRecord> edgeQueue,
     Dictionary<(int, int), EdgeRecord> edgeMap,
     int a, int b,
     Dictionary<int, VertexRecord> verts,
     Dictionary<int, HashSet<EdgeRecord>> vertexEdges)
    {
        if (a == b || !verts.ContainsKey(a) || !verts.ContainsKey(b)) return;

        int low = Mathf.Min(a, b);
        int high = Mathf.Max(a, b);
        var key = (low, high);

        Vector3 posA = verts[low].position;
        Vector3 posB = verts[high].position;
        Vector2 uvA = verts[low].uv;
        Vector2 uvB = verts[high].uv;
        Vector3 normalA = verts[low].normal;
        Vector3 normalB = verts[high].normal;

        float positionCost = (posA - posB).sqrMagnitude;
        float uvCost = (uvA - uvB).sqrMagnitude;
        float normalCost = Vector3.Angle(normalA, normalB) / 180f;

        float cost = lodQualityMode switch
        {
            LODQualityMode.Balanced => positionCost + uvCost + normalCost * 0.5f,
            LODQualityMode.PreserveShape => positionCost * 0.2f + uvCost * 0.5f + normalCost * 2f,
            LODQualityMode.FastSimplify => positionCost + uvCost * 0.1f,
            _ => positionCost
        };

        if (edgeMap.TryGetValue(key, out EdgeRecord existing))
        {
            edgeQueue.Remove(existing); // Reinsert with new cost
            existing.cost = cost;
            existing.collapsePosition = (posA + posB) * 0.5f;
            edgeQueue.Add(existing);
        }
        else
        {
            EdgeRecord newEdge = new EdgeRecord(low, high, cost, (posA + posB) * 0.5f);
            edgeMap[key] = newEdge;
            edgeQueue.Add(newEdge);

            if (!vertexEdges.TryGetValue(low, out var setA)) vertexEdges[low] = setA = new();
            if (!vertexEdges.TryGetValue(high, out var setB)) vertexEdges[high] = setB = new();

            setA.Add(newEdge);
            setB.Add(newEdge);
        }
    }


    private bool CollapseEdge(
      EdgeRecord edge,
      Dictionary<int, VertexRecord> verts,
      Dictionary<int, TriangleRecord> tris,
      SortedSet<EdgeRecord> edgeQueue,
      Dictionary<(int, int), EdgeRecord> edgeMap,
      Dictionary<int, HashSet<EdgeRecord>> vertexEdges)
    {
        if (!verts.ContainsKey(edge.vA) || !verts.ContainsKey(edge.vB))
            return false;

        VertexRecord a = verts[edge.vA];
        VertexRecord b = verts[edge.vB];

        HashSet<int> sharedTris = new(a.adjacentTriangles);
        sharedTris.IntersectWith(b.adjacentTriangles);
        foreach (int triId in sharedTris)
            if (tris.TryGetValue(triId, out var tri))
                tri.isValid = false;

        HashSet<int> remainingTris = new(b.adjacentTriangles);
        remainingTris.ExceptWith(sharedTris);

        foreach (int triId in remainingTris)
        {
            if (!tris.TryGetValue(triId, out var tri) || !tri.isValid) continue;
            if (tri.v0 == b.originalIndex) tri.v0 = a.originalIndex;
            if (tri.v1 == b.originalIndex) tri.v1 = a.originalIndex;
            if (tri.v2 == b.originalIndex) tri.v2 = a.originalIndex;
            a.adjacentTriangles.Add(triId);
        }

        b.adjacentTriangles.Clear();
        verts.Remove(b.originalIndex);

        a.position = edge.collapsePosition;
        a.uv = (a.uv + b.uv) * 0.5f;

        if (vertexEdges.TryGetValue(b.originalIndex, out var incident))
        {
            foreach (var e in incident.ToList()) // ✅ Snapshot
            {
                edgeQueue.Remove(e);
                edgeMap.Remove((e.vA, e.vB));

                if (vertexEdges.TryGetValue(e.vA, out var s1))
                    s1.Remove(e);

                if (vertexEdges.TryGetValue(e.vB, out var s2))
                    s2.Remove(e);
            }
            vertexEdges.Remove(b.originalIndex);
        }



        foreach (var triId in a.adjacentTriangles)
        {
            if (!tris.TryGetValue(triId, out var tri) || !tri.isValid) continue;
            AddEdge(edgeQueue, edgeMap, tri.v0, tri.v1, verts, vertexEdges);
            AddEdge(edgeQueue, edgeMap, tri.v1, tri.v2, verts, vertexEdges);
            AddEdge(edgeQueue, edgeMap, tri.v2, tri.v0, verts, vertexEdges);
        }

        return true;
    }





    private class VertexRecord
    {
        public int originalIndex;
        public Vector3 position;
        public Vector2 uv;
        public Vector3 normal;
        public HashSet<int> adjacentTriangles = new();

        public VertexRecord(int index, Vector3 pos, Vector2 uvCoord, Vector3 norm)
        {
            originalIndex = index;
            position = pos;
            uv = uvCoord;
            normal = norm;
        }
    }

    private class TriangleRecord
    {
        public int index;
        public int v0, v1, v2;
        public bool isValid = true;

        public TriangleRecord(int idx, int a, int b, int c)
        {
            index = idx;
            v0 = a;
            v1 = b;
            v2 = c;
        }

        public int[] GetIndices() => new int[] { v0, v1, v2 };
    }





    #endregion

    #region Material Transparency Settings
    private void ApplyTransparencySettings(Material mat, ShaderPipeline pipeline)
    {
        if (mat == null) return;

        if (verboseLogging)
            Debug.Log($" ApplyTransparencySettings() called on {mat.name} | Pipeline = {pipeline}");

        switch (pipeline)
        {
            case ShaderPipeline.URP:
                ApplyURPTransparency(mat);
                break;

            case ShaderPipeline.HDRP:
                ApplyHDRPTransparency(mat);
                break;

            case ShaderPipeline.Standard:
                ApplyStandardTransparency(mat);
                break;
        }

        // Final cleanup (shared across all pipelines)
        if (mat.HasProperty("_BlendModePreserveSpecular"))
            mat.SetFloat("_BlendModePreserveSpecular", 0f);

        EditorUtility.SetDirty(mat);
        AssetDatabase.SaveAssets();
        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(mat));

        if (verboseLogging)
            Debug.Log($"✔️ Transparency settings finalized for {mat.name}");
    }
    private void ApplyURPTransparency(Material mat)
    {
        if (mat.HasProperty("_Surface")) mat.SetFloat("_Surface", 1);
        if (mat.HasProperty("_ZWrite")) mat.SetFloat("_ZWrite", useAlphaClipping ? 1f : 0f);
        if (mat.HasProperty("_AlphaClip")) mat.SetFloat("_AlphaClip", useAlphaClipping ? 1f : 0f);
        if (mat.HasProperty("_Cutoff")) mat.SetFloat("_Cutoff", alphaCutoffValue);

        mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        mat.DisableKeyword("_SURFACE_TYPE_OPAQUE");

        mat.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);

        if (useAlphaClipping)
        {
            mat.EnableKeyword("_ALPHATEST_ON");
            mat.DisableKeyword("_ALPHABLEND_ON");
            mat.renderQueue = (int)RenderQueue.AlphaTest;
        }
        else
        {
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.renderQueue = (int)RenderQueue.Transparent;
        }

        if (mat.HasProperty("_SpecularHighlights"))
        {
            mat.SetFloat("_SpecularHighlights", 0f);
            mat.DisableKeyword("_SPECULARHIGHLIGHTS_ON");

            if (verboseLogging)
                Debug.Log($"⛔ Disabled Preserve Specular Lighting on: {mat.name}");
        }
    }

    private void ApplyHDRPTransparency(Material mat)
    {
        if (mat.HasProperty("_SurfaceType")) mat.SetFloat("_SurfaceType", 1);
        if (mat.HasProperty("_ZWrite")) mat.SetFloat("_ZWrite", useAlphaClipping ? 1f : 0f);

        if (useAlphaClipping)
        {
            if (mat.HasProperty("_AlphaCutoffEnable")) mat.SetFloat("_AlphaCutoffEnable", 1f);
            if (mat.HasProperty("_Cutoff")) mat.SetFloat("_Cutoff", alphaCutoffValue);
            mat.EnableKeyword("_ALPHATEST_ON");
            mat.renderQueue = (int)RenderQueue.AlphaTest;
        }
        else
        {
            if (mat.HasProperty("_AlphaCutoffEnable")) mat.SetFloat("_AlphaCutoffEnable", 0f);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.renderQueue = (int)RenderQueue.Transparent;
        }

        if (verboseLogging)
            Debug.Log($"✔️ HDRP transparency applied to {mat.name}");
    }

    private void ApplyStandardTransparency(Material mat)
    {
        if (!mat.HasProperty("_Mode")) return;

        bool cutout = useAlphaClipping;
        bool transparent = hasTransparency && !cutout;

        mat.SetFloat("_Mode", transparent ? 3 : (cutout ? 1 : 0)); // 0 = Opaque, 1 = Cutout, 3 = Transparent

        UnityEngine.Rendering.BlendMode src = transparent ? UnityEngine.Rendering.BlendMode.SrcAlpha : UnityEngine.Rendering.BlendMode.One;
        UnityEngine.Rendering.BlendMode dst = transparent ? UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha : UnityEngine.Rendering.BlendMode.Zero;
        mat.SetInt("_SrcBlend", (int)src);
        mat.SetInt("_DstBlend", (int)dst);
        mat.SetInt("_ZWrite", cutout ? 1 : (transparent ? 0 : 1));

        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");

        if (cutout)
        {
            mat.EnableKeyword("_ALPHATEST_ON");
            mat.renderQueue = 2450;
        }
        else if (transparent)
        {
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.renderQueue = 3000;
        }
        else
        {
            mat.renderQueue = 2000;
        }

        if (mat.HasProperty("_Cutoff"))
            mat.SetFloat("_Cutoff", alphaCutoffValue);
    }





    #endregion

    private string DetectPipelineShaderName()
    {
#if UNITY_2023_1_OR_NEWER
        var rpAsset = UnityEngine.Rendering.GraphicsSettings.defaultRenderPipeline;
#else
        var rpAsset = GraphicsSettings.currentRenderPipeline;
#endif

        if (rpAsset == null)
        {
            if (verboseLogging)
                Debug.Log(" Render pipeline: Unknown or missing. Defaulting to URP.");

            return "Universal Render Pipeline/Lit";
        }

        string typeName = rpAsset.GetType().ToString();
        if (verboseLogging)
            Debug.Log($" Detected Render Pipeline: {typeName}");

        if (typeName.Contains("UniversalRenderPipelineAsset"))
        {
            return "Universal Render Pipeline/Lit";
        }

        if (typeName.Contains("HDRenderPipelineAsset"))
        {
            return "HDRP/Lit";
        }

        Debug.LogWarning("⚠️ Unknown pipeline type, falling back to URP.");
        return "Universal Render Pipeline/Lit";
    }

    private string GetShaderPathForGroup()
    {
        string pipelinePrefix = selectedPipeline switch
        {
            ShaderPipeline.URP => "URP",
            ShaderPipeline.HDRP => "HDRP",
            ShaderPipeline.Standard => "Standard",
            _ => "URP"
        };

        string type = hasTransparency
            ? (useAlphaClipping ? "TransparentCutout" : "Transparent")
            : "OpaqueAtlas";

        return $"UVUnify/{pipelinePrefix}/{type}";
    }

    private void EnsureDefaultShaderExists(string shaderPath)
    {
        if (Shader.Find(shaderPath) != null)
            return;

        string safeName = shaderPath.Replace("UVUnify/", "").Replace("/", "_") + ".shader";
        string folder = "Assets/UVUnify/Shaders";
        string fullPath = Path.Combine(folder, safeName);

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        if (File.Exists(fullPath))
            return;

        string renderTag = "Opaque";
        string renderQueue = "Geometry";
        string blendCode = "";
        string alphaClipCode = "";
        string alphaProps = "";
        string alphaVars = "";

        if (shaderPath.Contains("TransparentCutout"))
        {
            renderTag = "TransparentCutout";
            renderQueue = "AlphaTest";
            alphaProps = "_Cutoff (\"Alpha Cutoff\", Range(0,1)) = 0.5";
            alphaVars = "float _Cutoff;";
            alphaClipCode = "clip(col.a - _Cutoff);";
        }
        else if (shaderPath.Contains("Transparent"))
        {
            renderTag = "Transparent";
            renderQueue = "Transparent";
            blendCode = "Blend SrcAlpha OneMinusSrcAlpha\nZWrite Off";
        }

        string shaderTemplate = $@"Shader ""{shaderPath}"" {{
    Properties {{
        _BaseMap (""Base Map"", 2D) = ""white"" {{ }}
        {alphaProps}
    }}
    SubShader {{
        Tags {{ ""RenderType""=""{renderTag}"" ""Queue""=""{renderQueue}"" }}
        LOD 100
        Pass {{
            {blendCode}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include ""UnityCG.cginc""

            sampler2D _BaseMap;
            {alphaVars}

            struct appdata {{
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            }};

            struct v2f {{
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            }};

            v2f vert(appdata v)
            {{
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }}

            fixed4 frag(v2f i) : SV_Target
            {{
                fixed4 col = tex2D(_BaseMap, i.uv);
                {alphaClipCode}
                return col;
            }}
            ENDCG
        }}
    }}
}}";

        File.WriteAllText(fullPath, shaderTemplate.Replace("\n", "\r\n"));
        AssetDatabase.ImportAsset(fullPath);
        Debug.Log($"🧩 Stub shader created: {shaderPath}");
    }





    #region Object and Folder Loading
    private void LoadObjectsFromFolder(DefaultAsset folder)
    {
        if (folder == null) return;

        selectedObjects.Clear(); //  Clear existing before loading new


        string path = AssetDatabase.GetAssetPath(folder);
        string[] guids = includeSubfolders
    ? AssetDatabase.FindAssets("t:GameObject", new[] { path }) // Recursive search (Unity's default)
    : Directory.GetFiles(path, "*.prefab", SearchOption.TopDirectoryOnly)
        .Where(p => p.EndsWith(".prefab"))
        .Select(p => AssetDatabase.AssetPathToGUID(p.Replace("\\", "/")))
        .ToArray();

        int skippedPrefabs = 0, copiedPrefabs = 0, addedObjects = 0;

        string prefabCopyFolder = Path.Combine("Assets", "AtlasTool_PrefabCopies");
        if (duplicatePrefabsBeforeProcessing && !AssetDatabase.IsValidFolder(prefabCopyFolder))
        {
            AssetDatabase.CreateFolder("Assets", "AtlasTool_PrefabCopies");
        }

        foreach (string guid in guids)
        {
            string objPath = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(objPath);
            if (prefab == null) continue;

            PrefabAssetType type = PrefabUtility.GetPrefabAssetType(prefab);

            if (type != PrefabAssetType.NotAPrefab)
            {
                if (!duplicatePrefabsBeforeProcessing)
                {
                    Debug.LogWarning($"⚠️ Skipped prefab asset: {prefab.name} (Enable 'Duplicate Prefabs Before Processing' to include it safely)");
                    skippedPrefabs++;
                    continue;
                }

                // Create unique path for copy
                string copyPath = Path.Combine(prefabCopyFolder, prefab.name + "_Copy.prefab");
                copyPath = AssetDatabase.GenerateUniqueAssetPath(copyPath);

                if (AssetDatabase.CopyAsset(objPath, copyPath))
                {
                    GameObject copiedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(copyPath);
                    GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(copiedPrefab);

                    // 📍 Position at scene view center for better visibility
                    SceneView sceneView = SceneView.lastActiveSceneView;
                    Vector3 spawnPos = sceneView != null ? sceneView.pivot : Vector3.zero;
                    instance.transform.position = spawnPos;
                    if (markCopiedPrefabsStatic)
                        GameObjectUtility.SetStaticEditorFlags(instance, StaticEditorFlags.BatchingStatic);

                    Undo.RegisterCreatedObjectUndo(instance, "Add copied prefab to scene");
                    TryAddObject(instance);
                    copiedPrefabs++;
                }
                else
                {
                    Debug.LogWarning($"❌ Failed to copy prefab: {prefab.name}");
                }
            }
            else
            {
                TryAddObject(prefab);
                addedObjects++;
            }
        }


        if (verboseLogging)
            Debug.Log($" Folder load summary: {addedObjects} scene objects added, {copiedPrefabs} prefabs duplicated, {skippedPrefabs} prefab assets skipped.");

        if (addedObjects == 0 && copiedPrefabs == 0)
        {
            EditorUtility.DisplayDialog("No Valid Objects Found",
                "No prefabs or GameObjects were loaded from the selected folder.\n\nCheck your filters or enable subfolder search.",
                "OK");
        }

        if (verboseLogging)
            Debug.Log($" Folder load summary: {addedObjects} scene objects added, {copiedPrefabs} prefabs duplicated, {skippedPrefabs} prefab assets skipped.");


    }

    private void LoadObjectsWithTag(string tag)
    {
        selectedObjects.Clear(); //  Clear existing before loading new
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag(tag))
        {
            TryAddObject(obj);
        }
    }

    private void LoadObjectsWithLayer(int layer)
    {
        selectedObjects.Clear(); //  Clear existing before loading new
#if UNITY_2023_1_OR_NEWER
        foreach (GameObject obj in GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
#else
        foreach (GameObject obj in GameObject.FindObjectsOfType<GameObject>())
#endif


        {
            if (obj.layer == layer)
            {
                TryAddObject(obj);
            }
        }
    }

    private void TryAddObject(GameObject obj)
    {
        if (obj == null || selectedObjects.Contains(obj)) return;

        // ⛔ Exclude objects by tag
        if (!string.IsNullOrEmpty(excludedTag) && obj.CompareTag(excludedTag))
        {
            if (verboseLogging)
                Debug.LogWarning($"⛔ Skipped '{obj.name}' due to excluded tag '{excludedTag}'.");
            return;
        }


        // Check for Static filter
        if (filterStaticOnly && !GameObjectUtility.AreStaticEditorFlagsSet(obj, StaticEditorFlags.BatchingStatic))
        {
            Debug.LogWarning($"⛔ Skipped '{obj.name}' — not marked as Static.");
            return;
        }

        // Check for MeshRenderer + MeshFilter
        if (filterRenderersOnly && (obj.GetComponent<MeshRenderer>() == null || obj.GetComponent<MeshFilter>() == null))
        {
            Debug.LogWarning($"⛔ Skipped '{obj.name}' — missing MeshRenderer or MeshFilter.");
            return;
        }

        selectedObjects.Add(obj);
        if (verboseLogging)
            Debug.Log($" Added: {obj.name}");

    }
    #endregion

    #region Utilities
    private bool IsMaterialTransparent(Material mat)
    {
        if (mat == null) return false;

        //Surface flags
        if (mat.HasProperty("_Surface") && mat.GetFloat("_Surface") == 1f)
        {
            if (verboseLogging) Debug.Log($"🔍 '{mat.name}' is Transparent via _Surface");
            return true;
        }

        if (mat.HasProperty("_SurfaceType") && mat.GetFloat("_SurfaceType") == 1f)
        {
            if (verboseLogging) Debug.Log($"🔍 '{mat.name}' is Transparent via _SurfaceType");
            return true;
        }

        //Alpha Clipping
        if (mat.HasProperty("_AlphaClip") && mat.GetFloat("_AlphaClip") > 0f)
        {
            if (verboseLogging) Debug.Log($"🔍 '{mat.name}' uses Alpha Clipping (_AlphaClip > 0)");
            return true;
        }

        //Shader name fallback
        string shaderName = mat.shader != null ? mat.shader.name.ToLowerInvariant() : "";
        if (shaderName.Contains("transparent") && !shaderName.Contains("opaque"))
        {
            if (verboseLogging) Debug.Log($"🔍 '{mat.name}' marked transparent via shader name: {shaderName}");
            return true;
        }

        //Render Queue fallback (AlphaTest/Transparent starts at 2450)
        if (mat.renderQueue >= 2450)
        {
            if (verboseLogging) Debug.Log($"🔍 '{mat.name}' marked transparent via Render Queue = {mat.renderQueue}");
            return true;
        }

        return false;
    }





    private bool IsObjectTransparent(GameObject obj)
    {
        if (obj == null) return false;

        if (forcedOpaqueObjects.Contains(obj.name))
            return false;

        if (obj.name.Contains("_Opaque") && !obj.name.Contains("Transparent"))
        {
            if (verboseLogging)
                Debug.Log($"️ '{obj.name}' assumed opaque due to naming convention.");
            return false;
        }


        var renderer = obj.GetComponent<MeshRenderer>();
        if (renderer == null || renderer.sharedMaterial == null) return false;

        Material mat = renderer.sharedMaterial;

        //1. Check surface/alpha/queue/shader name
        if (IsMaterialTransparent(mat))
            return true;

        //2. Fallback texture alpha check
        Texture tex = null;
        if (mat.HasProperty("_BaseMap"))
            tex = mat.GetTexture("_BaseMap");
        else if (mat.HasProperty("_BaseColorMap"))
            tex = mat.GetTexture("_BaseColorMap");
        else if (mat.HasProperty("_MainTex"))
            tex = mat.GetTexture("_MainTex");
        else if (mat.HasProperty("_BaseTex"))
            tex = mat.GetTexture("_BaseTex"); // new fallback for Unlit/Cutout

        if (tex is Texture2D tex2D)
        {
            try
            {
                Color32[] pixels = tex2D.GetPixels32();
                int total = pixels.Length;
                int transparent = pixels.Count(p => p.a < 255);
                float ratio = (float)transparent / total;

                if (ratio > alphaTransparencyThreshold)
                {
                    if (verboseLogging)
                        Debug.Log($"🌫️ Pixel-based alpha detected in '{obj.name}' ({transparent}/{total} transparent pixels)");
                    return true;
                }
            }
            catch
            {
                if (verboseLogging)
                    Debug.LogWarning($"⚠️ Could not read pixels from texture '{tex.name}' on '{obj.name}'");
                return false;
            }
        }

        return false;
    }




    private string Emoji(string symbol)
    {
        return useEmojis ? symbol + " " : "";
    }

    private string SanitizeString(string input)
    {
        if (string.IsNullOrEmpty(input)) return "";

        return input.Trim()
            .Replace("/", "_")
            .Replace("\\", "_")
            .Replace(":", "_")
            .Replace("*", "_")
            .Replace("?", "_")
            .Replace("\"", "_")
            .Replace("<", "_")
            .Replace(">", "_")
            .Replace("|", "_");
    }



    private string GetUniqueFilePath(string basePath, string baseName, string extension)
    {
        string path = Path.Combine(basePath, baseName + extension);
        int counter = 1;

        while (File.Exists(path))
        {
            path = Path.Combine(basePath, $"{baseName}_{counter}{extension}");
            counter++;
        }

        return path;
    }


    private void LoadAllStaticMeshesInScene()
    {
        selectedObjects.Clear();
        int added = 0;

#if UNITY_2023_1_OR_NEWER
        foreach (GameObject obj in GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
#else
        foreach (GameObject obj in GameObject.FindObjectsOfType<GameObject>())
#endif


        {
            if (!GameObjectUtility.AreStaticEditorFlagsSet(obj, StaticEditorFlags.BatchingStatic)) continue;

            var meshRenderer = obj.GetComponent<MeshRenderer>();
            var meshFilter = obj.GetComponent<MeshFilter>();
            if (meshRenderer && meshFilter)
            {
                selectedObjects.Add(obj);
                added++;
            }
        }

        if (verboseLogging)
            Debug.Log($"✅ Added {added} static mesh objects from scene.");
    }

    private void RefreshSelectedObjects()
    {
        if (selectedObjects == null) return;

        //  First: Remove any destroyed (null) objects
        selectedObjects.RemoveAll(obj => obj == null);

        hasTransparency = false;
        hasAlbedo = hasNormal = hasMetallic = hasAO = hasHeight = hasEmission = false;
        transparentObjectsDetected.Clear();

        foreach (GameObject obj in selectedObjects)
        {
            if (obj == null) continue;
            var renderer = obj.GetComponent<MeshRenderer>();
            if (renderer == null) continue;

            var mat = renderer.sharedMaterial;
            if (mat == null) continue;

            // Albedo: texture or color fallback
            if ((mat.HasProperty("_BaseMap") && mat.GetTexture("_BaseMap") != null) ||
                (mat.HasProperty("_MainTex") && mat.GetTexture("_MainTex") != null))
            {
                hasAlbedo = true;
            }
            else if (mat.HasProperty("_BaseColor") || mat.HasProperty("_Color"))
            {
                hasAlbedo = true; // ✅ Count color-only materials too
            }

            else if (mat.HasProperty("_BaseColor"))
            {
                // Still counts as valid albedo via color only
                hasAlbedo = true;
            }

            // Normal, Metallic, AO, etc.
            if (mat.HasProperty("_BumpMap") && mat.GetTexture("_BumpMap") != null) hasNormal = true;
            if (mat.HasProperty("_MetallicGlossMap") && mat.GetTexture("_MetallicGlossMap") != null) hasMetallic = true;
            if (mat.HasProperty("_OcclusionMap") && mat.GetTexture("_OcclusionMap") != null) hasAO = true;
            if (mat.HasProperty("_ParallaxMap") && mat.GetTexture("_ParallaxMap") != null) hasHeight = true;
            if (mat.HasProperty("_EmissionMap") && mat.GetTexture("_EmissionMap") != null) hasEmission = true;

            // Transparency check
            if (IsObjectTransparent(obj))
            {
                hasTransparency = true;
                transparentObjectsDetected.Add(obj.name);
            }
        }

        if (verboseLogging)
            Debug.Log("🔄 Refreshed selected objects. (Destroyed objects removed.)");
    }



    private void RemapMeshUVs(Mesh mesh, Rect atlasRect)
    {
        Vector2[] uvs = mesh.uv;
        if (uvs == null || uvs.Length == 0) return;

        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i].x = Mathf.Lerp(atlasRect.xMin, atlasRect.xMax, uvs[i].x);
            uvs[i].y = Mathf.Lerp(atlasRect.yMin, atlasRect.yMax, uvs[i].y);
        }

        mesh.uv = uvs;
    }

    public static void ClearLogoCache()
    {
        if (activeWindow != null)
        {
            activeWindow.cachedLogo = null;
            activeWindow.Repaint(); // refresh the UI
            if (activeWindow.verboseLogging)
                Debug.Log("🔁 UVUnify logo cache cleared due to asset change.");
        }
    }

    private void StripAlpha(Texture2D tex)
    {
        Color[] pixels = tex.GetPixels();
        for (int i = 0; i < pixels.Length; i++)
            pixels[i].a = 1f;
        tex.SetPixels(pixels);
        tex.Apply();
    }

    private string EnsureStagingFolderExists()
    {
        string folder = Path.Combine(saveFolderPath, "TextureCopies");
        if (!AssetDatabase.IsValidFolder(folder))
            Directory.CreateDirectory(folder);
        return folder;
    }



    private Texture2D CreateFlatAlbedoFallback(GameObject obj, Color color)
    {
        Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        tex.SetPixels(Enumerable.Repeat(color, 4).ToArray());
        tex.Apply();

        string folder = EnsureStagingFolderExists();
        string path = Path.Combine(folder, obj.name + "_FlatAlbedo.png");
        File.WriteAllBytes(path, tex.EncodeToPNG());
        AssetDatabase.ImportAsset(path);

        return AssetDatabase.LoadAssetAtPath<Texture2D>(path.Substring(path.IndexOf("Assets")));
    }

    private Texture2D CreateFlatNormalFallback(GameObject obj)
    {
        Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false, true);
        tex.SetPixels(Enumerable.Repeat(new Color(0.5f, 0.5f, 1f, 1f), 4).ToArray());
        tex.Apply();

        string folder = EnsureStagingFolderExists();
        string path = Path.Combine(folder, obj.name + "_FlatNormal.png");
        File.WriteAllBytes(path, tex.EncodeToPNG());
        AssetDatabase.ImportAsset(path);

        string assetPath = path.Substring(path.IndexOf("Assets"));
        var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.NormalMap;
            importer.sRGBTexture = false;
            importer.SaveAndReimport();
        }

        return AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
    }

    private void ForceMarkAsNormalMap(string assetPath)
    {
        var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.NormalMap;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.sRGBTexture = false;
            importer.alphaSource = TextureImporterAlphaSource.None;
            importer.alphaIsTransparency = false;
            importer.isReadable = false; // force off to match Unity expectations
            importer.SaveAndReimport();

            if (verboseLogging)
                Debug.Log($"Force-fixed normal map import settings: {assetPath}");
        }
    }

    private Texture2D CreateFallbackTextureFromColor(GameObject obj, Color color)
    {
        Color32 colorKey = (Color32)color;

        if (fallbackColorTextureCache.TryGetValue(colorKey, out Texture2D cached))
        {
            if (verboseLogging)
                if (verboseLogging)
                    Debug.Log($"Reused cached fallback color texture for color: {color}");
            return cached;
        }

        Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false, false);
        tex.SetPixels(Enumerable.Repeat((Color)colorKey, 4).ToArray());
        tex.Apply();

        string folder = EnsureStagingFolderExists();
        string safeColorHex = ColorUtility.ToHtmlStringRGBA(colorKey);
        string path = Path.Combine(folder, $"FlatColor_{safeColorHex}.png");
        File.WriteAllBytes(path, tex.EncodeToPNG());
        AssetDatabase.ImportAsset(path);

        string assetPath = path.Substring(path.IndexOf("Assets"));
        var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Default;
            importer.sRGBTexture = true;
            importer.alphaIsTransparency = false;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.isReadable = true;
            importer.SaveAndReimport();
        }

        Texture2D imported = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
        fallbackColorTextureCache[colorKey] = imported;

        return imported;
    }


    private void ResetAllSettingsToDefaults()
    {
        verboseLogging = false;
        atlasResolution = 4096;
        atlasPadding = 4;
        saveFolderPath = "Assets";
        selectedCompression = AtlasCompression.Uncompressed;

        generatePackedMaskMap = false;
        generateCombinedMesh = false;
        generatePrefab = false;
        showSelectedObjects = true;
        objectSearchFilter = "";
        generateSummaryReport = false;
        organizeOutputFolders = true;
        placePrefabInScene = false;
        deleteOriginalsAfterAtlasing = false;
        lastCombinedMeshOffset = Vector3.zero;
        useAlphaClipping = false;
        alphaCutoffValue = 0.5f;
        clearAfterGeneration = false;
        showCompletionDialog = true;
        autoSetTransparency = true;
        forceTransparentMaterial = false;
        hasTransparency = false;
        showBatchOptions = true;
        markCopiedPrefabsStatic = false;
        uvExpandMargin = 1f;
        generateLODs = false;
        numberOfLODs = 3;
        filePrefix = "";
        fileSuffix = "";
        generateUV2 = false;
        autoMarkStaticForUV2 = true;
        selectedTagToInclude = "Untagged";
        excludedTag = "ExcludeFromAtlas";
        showTagFilters = false;
        useObjectNameForPrefab = false;
        includeSubfolders = true;
        atlasOnlyMode = false;
        groupSuffix = "";
        minimumFinalAtlasSize = 2048;
        selectedPipeline = ShaderPipeline.Auto;
        uvSourceProp = "_BaseMap";
        selectedUVSourceIndex = 0;
        lodQualityMode = LODQualityMode.Balanced;
        layerMask = 0;
        filterStaticOnly = false;
        filterRenderersOnly = true;
        duplicatePrefabsBeforeProcessing = true;

        // Clear selections and caches
        selectedObjects.Clear();
        transparentObjectsDetected.Clear();
        forcedOpaqueObjects.Clear();
        fallbackColorUsedObjects.Clear();
        fallbackColorTextureCache.Clear();
        //Clear HDRP Packed Mask Composer fields
        hdrpMetallicTex = null;
        hdrpAOTexture = null;
        hdrpHeightTexture = null;
        hdrpEmissionTexture = null;

        RefreshSelectedObjects();
        Repaint();
    }


    private void DilateAtlasTileBorders(Texture2D atlas, Rect[] uvs, int paddingPixels)
    {
        if (atlas == null || uvs == null || uvs.Length == 0 || paddingPixels <= 0) return;

        int width = atlas.width;
        int height = atlas.height;
        Color[] pixels = atlas.GetPixels();

        foreach (Rect uv in uvs)
        {
            int x = Mathf.RoundToInt(uv.x * width);
            int y = Mathf.RoundToInt(uv.y * height);
            int w = Mathf.RoundToInt(uv.width * width);
            int h = Mathf.RoundToInt(uv.height * height);

            for (int d = 1; d <= paddingPixels; d++)
            {
                for (int i = -d; i < w + d; i++)
                {
                    int tx = Mathf.Clamp(x + i, 0, width - 1);

                    // Top
                    int topY = Mathf.Clamp(y + h + d - 1, 0, height - 1);
                    int fromTop = Mathf.Clamp(y + h - 1, 0, height - 1);
                    pixels[topY * width + tx] = pixels[fromTop * width + tx];

                    // Bottom
                    int botY = Mathf.Clamp(y - d, 0, height - 1);
                    int fromBot = Mathf.Clamp(y, 0, height - 1);
                    pixels[botY * width + tx] = pixels[fromBot * width + tx];
                }

                for (int j = -d; j < h + d; j++)
                {
                    int ty = Mathf.Clamp(y + j, 0, height - 1);

                    // Right
                    int rightX = Mathf.Clamp(x + w + d - 1, 0, width - 1);
                    int fromRight = Mathf.Clamp(x + w - 1, 0, width - 1);
                    pixels[ty * width + rightX] = pixels[ty * width + fromRight];

                    // Left
                    int leftX = Mathf.Clamp(x - d, 0, width - 1);
                    int fromLeft = Mathf.Clamp(x, 0, width - 1);
                    pixels[ty * width + leftX] = pixels[ty * width + fromLeft];
                }
            }
        }

        atlas.SetPixels(pixels);
        atlas.Apply();
        if (verboseLogging)
            Debug.Log(" Edge dilation applied to all atlas tiles to prevent color bleeding.");
    }


    private void EnsureTagExists(string tagName)
    {
#if UNITY_EDITOR
        var tags = UnityEditorInternal.InternalEditorUtility.tags;
        if (!tags.Contains(tagName))
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty tagsProp = tagManager.FindProperty("tags");

            for (int i = 0; i < tagsProp.arraySize; i++)
            {
                if (tagsProp.GetArrayElementAtIndex(i).stringValue == tagName)
                    return; // Tag already exists
            }

            tagsProp.InsertArrayElementAtIndex(tagsProp.arraySize);
            tagsProp.GetArrayElementAtIndex(tagsProp.arraySize - 1).stringValue = tagName;
            tagManager.ApplyModifiedProperties();

            Debug.Log($" Created missing tag: {tagName}");
        }
#endif
    }

    private void ApplyStandardPipelineSettings(Material mat)
    {
        if (mat == null) return;

        mat.shader = Shader.Find("Standard");

        bool cutout = useAlphaClipping;
        bool transparent = hasTransparency && !cutout;

        // 0 = Opaque, 1 = Cutout, 2 = Fade, 3 = Transparent
        mat.SetFloat("_Mode", transparent ? 3 : (cutout ? 1 : 0));

        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", cutout ? 1 : (transparent ? 0 : 1));

        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.EnableKeyword(cutout ? "_ALPHATEST_ON" : "_ALPHABLEND_ON");
        mat.renderQueue = transparent ? 3000 : (cutout ? 2450 : 2000);

        if (mat.HasProperty("_Cutoff"))
            mat.SetFloat("_Cutoff", alphaCutoffValue);

        if (verboseLogging)
            Debug.Log($" Applied Standard shader transparency settings (Transparent: {transparent}, Cutout: {cutout})");
    }

    private void ApplyPackedMaskToMaterial(Material atlasMat, Texture2D packedMaskTex, Texture2D emissionAtlas)

    {
        if (atlasMat == null || packedMaskTex == null)
        {
            Debug.LogWarning(" ApplyPackedMaskToMaterial called with null inputs.");
            return;
        }

        string shaderName = atlasMat.shader?.name ?? "";

        if (selectedPipeline == ShaderPipeline.HDRP || shaderName.Contains("HDRP"))
        {
            if (atlasMat.HasProperty("_MaskMap"))
            {
                atlasMat.SetTexture("_MaskMap", packedMaskTex);
                if (verboseLogging) Debug.Log("Assigned PackedMaskAtlas to _MaskMap (HDRP)");
            }

            if (atlasMat.HasProperty("_EmissiveColorMap"))
            {
                atlasMat.SetTexture("_EmissiveColorMap", packedMaskTex);
                atlasMat.SetColor("_EmissiveColor", Color.white);
                atlasMat.EnableKeyword("_EMISSION");
                if (verboseLogging) Debug.Log("Assigned PackedMaskAtlas to _EmissiveColorMap (HDRP)");
            }
        }
        else if (selectedPipeline == ShaderPipeline.URP || shaderName.Contains("Universal"))
        {
            if (atlasMat.HasProperty("_MetallicGlossMap"))
            {
                atlasMat.SetTexture("_MetallicGlossMap", packedMaskTex);
                atlasMat.EnableKeyword("_METALLICGLOSSMAP");
                if (verboseLogging) Debug.Log("Assigned PackedMaskAtlas to _MetallicGlossMap (URP)");
            }

            if (atlasMat.HasProperty("_OcclusionMap"))
            {
                atlasMat.SetTexture("_OcclusionMap", packedMaskTex);
                atlasMat.SetFloat("_OcclusionStrength", 1f);
                if (verboseLogging) Debug.Log("Assigned PackedMaskAtlas to _OcclusionMap (URP)");
            }

            if (atlasMat.HasProperty("_EmissionMap") && emissionAtlas != null)
            {
                atlasMat.SetTexture("_EmissionMap", emissionAtlas);
                atlasMat.SetColor("_EmissionColor", Color.white);
                atlasMat.EnableKeyword("_EMISSION");
                if (verboseLogging) Debug.Log("Assigned actual emission map to _EmissionMap");
            }
            else
            {
                // Do not assign packed mask as emission
                if (verboseLogging) Debug.Log("Skipped assigning _EmissionMap — no valid emission atlas detected.");
            }

        }
        else if (selectedPipeline == ShaderPipeline.Standard || shaderName.Contains("Standard"))
        {
            // ✔ Only use R+A from packed mask for Metallic+Smoothness
            if (atlasMat.HasProperty("_MetallicGlossMap"))
            {
                atlasMat.SetTexture("_MetallicGlossMap", packedMaskTex);
                atlasMat.EnableKeyword("_METALLICGLOSSMAP");

                if (atlasMat.HasProperty("_Glossiness")) atlasMat.SetFloat("_Glossiness", 0f);
                if (atlasMat.HasProperty("_Smoothness")) atlasMat.SetFloat("_Smoothness", 0f);
            }

            // ❗ Load AO separately (Standard doesn't support G in packed mask)
            string aoPath = Path.Combine(saveFolderPath, "Atlases" + groupSuffix, filePrefix + "AOAtlas" + groupSuffix + fileSuffix + ".png");
            if (File.Exists(aoPath))
            {
                string aoAssetPath = aoPath.Substring(aoPath.IndexOf("Assets"));
                Texture2D aoTex = AssetDatabase.LoadAssetAtPath<Texture2D>(aoAssetPath);
                if (atlasMat.HasProperty("_OcclusionMap") && aoTex != null)
                {
                    atlasMat.SetTexture("_OcclusionMap", aoTex);
                    atlasMat.SetFloat("_OcclusionStrength", 1f);
                    if (verboseLogging)
                        Debug.Log("✅ Standard: Assigned AO map as separate texture (Standard doesn't support G from Packed Mask).");
                }
            }

            // 🔒 Do NOT assign emission from packed mask
            if (atlasMat.HasProperty("_EmissionMap"))
            {
                atlasMat.SetTexture("_EmissionMap", null);
                atlasMat.SetColor("_EmissionColor", Color.black);
                atlasMat.DisableKeyword("_EMISSION");
            }

            // 🔒 Remove height map assignment — unsupported in packed mask
            if (atlasMat.HasProperty("_ParallaxMap"))
            {
                atlasMat.SetTexture("_ParallaxMap", null);
            }
        }
    }


    private void AssignBaseAlbedoTexture(Material mat, Texture2D albedoTex)
    {
        if (mat == null || albedoTex == null) return;

        if (mat.HasProperty("_BaseMap"))
        {
            mat.SetTexture("_BaseMap", albedoTex);
            mat.SetColor("_BaseColor", Color.white);
            if (verboseLogging) Debug.Log(" Assigned _BaseMap and _BaseColor");
        }
        else if (mat.HasProperty("_MainTex"))
        {
            mat.SetTexture("_MainTex", albedoTex);
            mat.SetColor("_Color", Color.white);
            if (verboseLogging) Debug.Log(" Assigned _MainTex and _Color (Standard fallback)");
        }
        else
        {
            Debug.LogWarning("⚠️ No recognized albedo property found on material shader.");
        }
    }


    private void AssignNormalMap(Material mat, Texture2D normalTex, string originalProp)
    {
        if (mat == null || normalTex == null) return;

        string shaderName = mat.shader?.name.ToLowerInvariant() ?? "";

        if ((selectedPipeline == ShaderPipeline.URP || shaderName.Contains("universal")) &&
            originalProp == "_BumpMap" && mat.HasProperty("_BumpMap"))
        {
            mat.SetTexture("_BumpMap", normalTex);
            mat.EnableKeyword("_NORMALMAP");
            if (verboseLogging) Debug.Log("Assigned normal map to _BumpMap (URP)");
        }
        else if ((selectedPipeline == ShaderPipeline.HDRP || shaderName.Contains("hdrp")) &&
                 originalProp == "_NormalMap" && mat.HasProperty("_NormalMap"))
        {
            mat.SetTexture("_NormalMap", normalTex);
            mat.EnableKeyword("_NORMALMAP");
            if (verboseLogging) Debug.Log("Assigned normal map to _NormalMap (HDRP)");
        }
        else if ((selectedPipeline == ShaderPipeline.Standard || shaderName.Contains("standard")) &&
                 mat.HasProperty("_BumpMap"))
        {
            mat.SetTexture("_BumpMap", normalTex);
            mat.EnableKeyword("_NORMALMAP");
            if (verboseLogging) Debug.Log("Assigned normal map to _BumpMap (Standard)");
        }
        else
        {
            Debug.LogWarning($"⚠️ Could not assign normal map '{originalProp}' — shader '{mat.shader.name}' missing expected property.");
        }
    }

    private void AssignEmissionMap(Material mat, Texture2D emissionTex)
    {
        if (mat == null || emissionTex == null) return;

        string shaderName = mat.shader?.name.ToLowerInvariant() ?? "";

        if ((selectedPipeline == ShaderPipeline.HDRP || shaderName.Contains("hdrp")) &&
            mat.HasProperty("_EmissiveColorMap"))
        {
            mat.SetTexture("_EmissiveColorMap", emissionTex);
            mat.SetColor("_EmissiveColor", Color.white);
            mat.EnableKeyword("_EMISSION");
            if (verboseLogging) Debug.Log("Assigned emission map to _EmissiveColorMap (HDRP)");
        }
        else if ((selectedPipeline == ShaderPipeline.URP || shaderName.Contains("universal")) &&
                 mat.HasProperty("_EmissionMap"))
        {
            mat.SetTexture("_EmissionMap", emissionTex);
            mat.SetColor("_EmissionColor", Color.white);
            mat.EnableKeyword("_EMISSION");
            if (verboseLogging) Debug.Log("Assigned emission map to _EmissionMap (URP)");
        }
        else if ((selectedPipeline == ShaderPipeline.Standard || shaderName.Contains("standard")) &&
                 mat.HasProperty("_EmissionMap"))
        {
            mat.SetTexture("_EmissionMap", emissionTex);
            mat.SetColor("_EmissionColor", Color.white);
            mat.EnableKeyword("_EMISSION");
            if (verboseLogging) Debug.Log("Assigned emission map to _EmissionMap (Standard)");
        }
        else
        {
            Debug.LogWarning($"⚠️ Could not assign emission map — shader: {mat.shader.name}");
        }
    }
    private class EdgeRecord : IComparable<EdgeRecord>
    {
        public int vA, vB;
        public float cost;
        public Vector3 collapsePosition;

        public EdgeRecord(int a, int b, float c, Vector3 pos)
        {
            vA = Mathf.Min(a, b);
            vB = Mathf.Max(a, b);
            cost = c;
            collapsePosition = pos;
        }

        public override int GetHashCode() => vA * 73856093 ^ vB * 19349663;
        public override bool Equals(object obj) => obj is EdgeRecord other && vA == other.vA && vB == other.vB;

        public int CompareTo(EdgeRecord other)
        {
            if (cost != other.cost)
                return cost.CompareTo(other.cost);
            if (vA != other.vA)
                return vA.CompareTo(other.vA);
            return vB.CompareTo(other.vB);
        }
    }

    public static class UVUnifySystemSpecs
    {
        public static int GetSystemRAMMB() => SystemInfo.systemMemorySize;
        public static int GetGPUVRAMMB() => SystemInfo.graphicsMemorySize;
        public static int GetCPUCores() => SystemInfo.processorCount;
        public static string GetCPUName() => SystemInfo.processorType;
        public static string GetGPUName() => SystemInfo.graphicsDeviceName;
        public static bool SupportsAsyncReadback() => SystemInfo.supportsAsyncGPUReadback;
        public static bool SupportsComputeShaders() => SystemInfo.supportsComputeShaders;
    }


    // ========================
    // MaterialHelper
    // ========================
    private static class MaterialHelper
    {
        public static void SetupMaterial(Material mat, ShaderPipeline pipeline, bool isTransparent, bool isCutout = false)
        {
            switch (pipeline)
            {
                case ShaderPipeline.Standard:
                    SetupStandardMaterial(mat, isTransparent, isCutout);
                    break;
                case ShaderPipeline.URP:
                    SetupURPMaterial(mat, isTransparent, isCutout);
                    break;
                case ShaderPipeline.HDRP:
                    SetupHDRPMaterial(mat, isTransparent, isCutout);
                    break;
            }
        }

        private static void SetupStandardMaterial(Material mat, bool isTransparent, bool isCutout)
        {
            mat.shader = Shader.Find("Standard");

            mat.SetFloat("_Mode", isTransparent ? 3 : isCutout ? 1 : 0);
            mat.SetOverrideTag("RenderType", isTransparent ? "Transparent" : isCutout ? "TransparentCutout" : "Opaque");

            UnityEngine.Rendering.BlendMode src = isTransparent ? UnityEngine.Rendering.BlendMode.SrcAlpha : UnityEngine.Rendering.BlendMode.One;
            UnityEngine.Rendering.BlendMode dst = isTransparent ? UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha : UnityEngine.Rendering.BlendMode.Zero;
            mat.SetInt("_SrcBlend", (int)src);
            mat.SetInt("_DstBlend", (int)dst);
            mat.SetInt("_ZWrite", isTransparent ? 0 : 1);

            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.DisableKeyword("_ALPHABLEND_ON");

            if (isTransparent)
                mat.EnableKeyword("_ALPHABLEND_ON");
            else if (isCutout)
                mat.EnableKeyword("_ALPHATEST_ON");

            mat.renderQueue = isTransparent ? 3000 : isCutout ? 2450 : 2000;

            //  Clamp shine — fix plasticky look
            if (mat.HasProperty("_Glossiness")) mat.SetFloat("_Glossiness", 0f);
            if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", 0f);
            if (mat.HasProperty("_SpecColor")) mat.SetColor("_SpecColor", Color.black);

            //  Disable forward options that cause glare
            if (mat.HasProperty("_SpecularHighlights")) mat.SetFloat("_SpecularHighlights", 0f);
            if (mat.HasProperty("_GlossyReflections")) mat.SetFloat("_GlossyReflections", 0f);
        }


        private static void SetupURPMaterial(Material mat, bool isTransparent, bool isCutout)
        {
            mat.shader = Shader.Find("Universal Render Pipeline/Lit");

            mat.SetFloat("_Surface", isTransparent ? 1f : 0f);
            mat.SetFloat("_ZWrite", isTransparent ? 0f : 1f);
            mat.SetOverrideTag("RenderType", isTransparent ? "Transparent" : "Opaque");

            mat.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
            if (isTransparent)
                mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");

            mat.SetFloat("_AlphaClip", isCutout ? 1f : 0f);
            if (isCutout)
                mat.EnableKeyword("_ALPHATEST_ON");
            else
                mat.DisableKeyword("_ALPHATEST_ON");

            mat.renderQueue = isCutout ? 2450 : isTransparent ? 3000 : 2000;
        }

        private static void SetupHDRPMaterial(Material mat, bool isTransparent, bool isCutout)
        {
            mat.shader = Shader.Find("HDRP/Lit");

            mat.SetFloat("_SurfaceType", isTransparent ? 1f : 0f);
            mat.SetFloat("_ZWrite", isTransparent ? 0f : 1f);
            mat.SetOverrideTag("RenderType", isTransparent ? "Transparent" : "Opaque");

            if (isCutout)
            {
                mat.SetFloat("_AlphaCutoffEnable", 1f);
                mat.EnableKeyword("_ALPHATEST_ON");
                mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
            }
            else
            {
                mat.SetFloat("_AlphaCutoffEnable", 0f);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.renderQueue = isTransparent ? 3000 : 2000;
            }
        }
    }

    private Texture2D FixAOForStandard(Texture2D aoTex)
    {
        if (aoTex == null || selectedPipeline != ShaderPipeline.Standard)
            return aoTex;

        Texture2D fixedTex = new Texture2D(aoTex.width, aoTex.height, TextureFormat.RGBA32, false);
        Color[] pixels = aoTex.GetPixels();

        for (int i = 0; i < pixels.Length; i++)
        {
            float gray = pixels[i].grayscale;
            pixels[i] = new Color(0f, gray, 0f, 1f); // only green carries AO
        }

        fixedTex.SetPixels(pixels);
        fixedTex.Apply();
        return fixedTex;
    }

    private Texture2D FixHeightForStandard(Texture2D heightTex)
    {
        if (heightTex == null || selectedPipeline != ShaderPipeline.Standard)
            return heightTex;

        Texture2D fixedTex = new Texture2D(heightTex.width, heightTex.height, TextureFormat.RGBA32, false);
        Color[] pixels = heightTex.GetPixels();

        for (int i = 0; i < pixels.Length; i++)
        {
            float gray = pixels[i].grayscale;
            pixels[i] = new Color(0f, gray, 0f, 1f); // only green carries height
        }

        fixedTex.SetPixels(pixels);
        fixedTex.Apply();
        return fixedTex;
    }

    private void AssignEmissionForStandard(Material mat, Texture2D emissionTex)
    {
        if (mat == null || selectedPipeline != ShaderPipeline.Standard || emissionTex == null)
            return;

        mat.SetTexture("_EmissionMap", emissionTex);
        mat.SetColor("_EmissionColor", Color.white); // Can be brightened if needed
        mat.EnableKeyword("_EMISSION");

        if (verboseLogging)
            Debug.Log("✅ Standard: Assigned EmissionMap and enabled _EMISSION.");
    }

    private void EnsureMeshLightingIntegrity(Mesh mesh)
    {
        if (mesh == null) return;

        // Always do this — HDRP also expects tangents
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mesh.RecalculateBounds();

        if (verboseLogging)
            Debug.Log($"✅ Mesh lighting integrity ensured (Normals + Tangents recalculated).");
    }


    private void StripAlphaFromMetallicGloss(Material mat)
    {
        if (selectedPipeline != ShaderPipeline.Standard) return;
        if (mat == null || !mat.HasProperty("_MetallicGlossMap")) return;

        Texture2D glossTex = mat.GetTexture("_MetallicGlossMap") as Texture2D;
        if (glossTex == null || glossTex.format != TextureFormat.RGBA32 || !glossTex.isReadable) return;

        Texture2D stripped = new Texture2D(glossTex.width, glossTex.height, TextureFormat.RGB24, false);
        Color[] pixels = glossTex.GetPixels();
        for (int i = 0; i < pixels.Length; i++)
            pixels[i].a = 1f;

        stripped.SetPixels(pixels);
        stripped.Apply();

        mat.SetTexture("_MetallicGlossMap", stripped);
        mat.EnableKeyword("_METALLICGLOSSMAP");

        if (verboseLogging)
            Debug.Log(" Alpha stripped from MetallicGlossMap for Standard LOD material.");
    }

    private void ResolvePipelineIfNeeded()
    {
        if (selectedPipeline != ShaderPipeline.Auto) return;

        string detected = DetectPipelineShaderName().ToLowerInvariant();
        if (detected.Contains("hdrp")) selectedPipeline = ShaderPipeline.HDRP;
        else if (detected.Contains("universal")) selectedPipeline = ShaderPipeline.URP;
        else selectedPipeline = ShaderPipeline.Standard;

        if (verboseLogging)
            Debug.Log($" Auto-resolved pipeline to: {selectedPipeline}");
    }


    #endregion

}


// Outside the main class

public class UVUnifyLogoWatcher : AssetPostprocessor
{
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string path in importedAssets)
        {
            if (path.ToLower().Contains("uvunify_logo"))
            {
                UVUnifyAtlas.ClearLogoCache();
                break;
            }
        }

        foreach (string path in movedAssets)
        {
            if (path.ToLower().Contains("uvunify_logo"))
            {
                UVUnifyAtlas.ClearLogoCache();
                break;
            }
        }

        foreach (string path in deletedAssets)
        {
            if (path.ToLower().Contains("uvunify_logo"))
            {
                UVUnifyAtlas.ClearLogoCache();
                break;
            }
        }
    }

}



}
