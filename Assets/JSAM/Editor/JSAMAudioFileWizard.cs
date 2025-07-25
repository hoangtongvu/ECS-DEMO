﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Presets;

namespace JSAM.JSAMEditor
{
    public enum AudioFileType
    {
        Music,
        Sound
    }

    public class JSAMAudioFileWizardEditor : JSAMBaseEditorWindow<JSAMAudioFileWizardEditor>
    {
        Vector2 scroll;

        Preset selectedPreset = null;

        List<Preset> soundPresets = new List<Preset>();
        List<Preset> musicPresets = new List<Preset>();
        List<string> soundPresetNames;
        List<string> musicPresetNames;

        static string SELECTED_SOUND_PRESET_KEY = nameof(JSAMAudioFileWizardEditor) + nameof(SelectedSoundPresetIndex);
        int SelectedSoundPresetIndex
        {
            get => EditorPrefs.GetInt(SELECTED_SOUND_PRESET_KEY, 0);
            set => EditorPrefs.SetInt(SELECTED_SOUND_PRESET_KEY, value);
        }
        static string SELECTED_MUSIC_PRESET_KEY = nameof(JSAMAudioFileWizardEditor) + nameof(SelectedMusicPresetIndex);
        int SelectedMusicPresetIndex
        {
            get => EditorPrefs.GetInt(SELECTED_MUSIC_PRESET_KEY, 0);
            set => EditorPrefs.SetInt(SELECTED_MUSIC_PRESET_KEY, value);
        }

        public static List<AudioClip> files = new List<AudioClip>();
        public static string outputFolder = "";

        public static AudioFileType FileType
        {
            get => (AudioFileType)EditorPrefs.GetInt(FILETYPE_KEY, 0);
            set => EditorPrefs.SetInt(FILETYPE_KEY, (int)value);
        }
        static string FILETYPE_KEY = nameof(JSAMAudioFileWizardEditor) + nameof(FileType);

        Dictionary<Preset, PropertyModification> presetToProp = new Dictionary<Preset, PropertyModification>();

        // Add menu named "My Window" to the Window menu
        [MenuItem("Window/JSAM/Audio File Wizard")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            Window.Show();
            window.SetWindowTitle();
        }

        protected void OnEnable()
        {
            LoadPresets();
        }

        protected override void SetWindowTitle()
        {
            window.titleContent = new GUIContent("Audio File Wizard");
        }

        private void OnGUI()
        {
            if (Event.current.type == EventType.MouseEnterWindow)
            {
                window.Repaint();
            }

            GUIContent blontent;

            Rect overlay = EditorGUILayout.BeginVertical(GUI.skin.box, new GUILayoutOption[] { GUILayout.MinHeight(70), GUILayout.MaxHeight(200) });
            scroll = EditorGUILayout.BeginScrollView(scroll);

            for (int i = files.Count - 1; i > -1; i--)
            {
                if (files[i] == null)
                {
                    files.RemoveAt(i);
                    continue;
                }
                Rect rect = EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                var element = files[i];
                blontent = new GUIContent(element.name);

                EditorGUILayout.LabelField(i.ToString(), new GUILayoutOption[] { GUILayout.MaxWidth(25) });

                EditorGUILayout.LabelField(blontent, new GUILayoutOption[] { GUILayout.MaxWidth(Window.position.width / 3) });

                using (new EditorGUI.DisabledScope(true))
                {
                    files[i] = EditorGUILayout.ObjectField(GUIContent.none, files[i], typeof(AudioClip), false) as AudioClip;
                }

                JSAMEditorHelper.BeginColourChange(Color.red);
                blontent = new GUIContent("X", "Remove this AudioClip from the list");
                if (GUILayout.Button(blontent, new GUILayoutOption[] { GUILayout.MaxWidth(25) }))
                {
                    files.Remove(files[i]);
                    GUIUtility.ExitGUI();
                }
                JSAMEditorHelper.EndColourChange();
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

            HandleDragAndDrop(overlay);

            blontent = new GUIContent("Clear Files", "Remove all files from the above list. This process can be undone with Edit -> Undo");
            if (GUILayout.Button(blontent, new GUILayoutOption[] { GUILayout.ExpandWidth(false) }))
            {
                files.Clear();
            }

            blontent = new GUIContent("Audio File Type", "The type of Audio File you want to generate from these Audio Clips");
            EditorGUI.BeginChangeCheck();
            FileType = (AudioFileType)EditorGUILayout.EnumPopup(blontent, FileType);
            if (EditorGUI.EndChangeCheck())
            {
                if (FileType == AudioFileType.Music)
                {
                    if (musicPresets.Count > 0) selectedPreset = musicPresets[SelectedMusicPresetIndex];
                }
                else
                {
                    if (soundPresets.Count > 0) selectedPreset = soundPresets[SelectedSoundPresetIndex];
                }
            }

            EditorGUILayout.BeginHorizontal();
            blontent = new GUIContent("Preset to Apply", "Audio File objects created through the Audio File Wizard will be created using this preset as a template.");
            // Music
            if (FileType == AudioFileType.Music)
            {
                if (musicPresets.Count == 0)
                {
                    using (new EditorGUI.DisabledScope(true))
                        SelectedMusicPresetIndex = EditorGUILayout.Popup(blontent, SelectedMusicPresetIndex, new string[] { "<None>" });
                }
                else
                {
                    EditorGUI.BeginChangeCheck();
                    SelectedMusicPresetIndex = EditorGUILayout.Popup(blontent, SelectedMusicPresetIndex, musicPresetNames.ToArray());
                    if (EditorGUI.EndChangeCheck())
                    {
                        selectedPreset = musicPresets[SelectedMusicPresetIndex];
                    }
                }
            }
            // Sound
            else
            {
                if (soundPresets.Count == 0)
                {
                    using (new EditorGUI.DisabledScope(true))
                        SelectedSoundPresetIndex = EditorGUILayout.Popup(blontent, SelectedSoundPresetIndex, new string[] { "<None>" });
                }
                else
                {
                    EditorGUI.BeginChangeCheck();
                    SelectedSoundPresetIndex = EditorGUILayout.Popup(blontent, SelectedSoundPresetIndex, soundPresetNames.ToArray());
                    if (EditorGUI.EndChangeCheck())
                    {
                        selectedPreset = soundPresets[SelectedSoundPresetIndex];
                    }
                }
            }

            blontent = new GUIContent("Refresh Presets", "Reload your assets from your configured preset folder. " +
                "/nPress this button if you recently created a new Audio File preset and don't see it in the drop-down menu. " +
                "/nYou can change your preset folder location in Project Settings -> Audio - JSAM");
            if (GUILayout.Button(blontent, new GUILayoutOption[] { GUILayout.ExpandWidth(false) }))
            {
                LoadPresets();
                GUIUtility.ExitGUI();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            if (selectedPreset != null)
            {
                blontent = new GUIContent(presetToProp[selectedPreset].value);
            }
            else
            {
                blontent = new GUIContent("No preset selected");
            }

            var skin = GUI.skin.box.ApplyTextAnchor(TextAnchor.UpperLeft).SetTextColor(GUI.skin.label.normal.textColor);
            EditorGUILayout.LabelField(new GUIContent("Preset Description"), blontent, skin, new GUILayoutOption[] { GUILayout.ExpandWidth(true) });
            EditorGUILayout.EndHorizontal();

            EditorGUI.BeginChangeCheck();
            outputFolder = JSAMEditorHelper.RenderSmartFolderProperty(new GUIContent("Output Folder"), outputFolder);
            if (EditorGUI.EndChangeCheck() && outputFolder.Length > 0)
            {
                // Check if user created a new folder in the file-dialog
                if (Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), outputFolder)))
                {
                    if (!AssetDatabase.IsValidFolder(outputFolder))
                    {
                        AssetDatabase.ImportAsset(outputFolder);
                    }
                }
            }

            blontent = new GUIContent("Generate Audio File Objects",
                "Create audio file objects with the provided Audio Clips according to the selected preset. " +
                "Audio File objects will be saved to the output folder.");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();

            bool cantGenerate = files.Count == 0 || selectedPreset == null || !AssetDatabase.IsValidFolder(outputFolder);

            using (new EditorGUI.DisabledScope(cantGenerate))
            {
                if (GUILayout.Button(blontent, new GUILayoutOption[] { GUILayout.ExpandWidth(false) }))
                {
                    if (FileType == AudioFileType.Music)
                    {
                        GenerateAudioFileObjects<MusicFileObject>(selectedPreset);
                    }
                    else
                    {
                        GenerateAudioFileObjects<SoundFileObject>(selectedPreset);
                    }
                }
            }
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
        }

        void LoadPresets()
        {
            soundPresets = new List<Preset>();
            soundPresetNames = new List<string>();

            musicPresets = new List<Preset>();
            musicPresetNames = new List<string>();

            var presets = JSAMEditorHelper.ImportAssetsOrFoldersAtPath<Preset>(JSAMPaths.Instance.JSAMPresetsPath);
            if (!string.IsNullOrEmpty(JSAMPaths.Instance.PresetsPath))
            {
                presets.AddRange(JSAMEditorHelper.ImportAssetsOrFoldersAtPath<Preset>(JSAMPaths.Instance.PresetsPath));
            }

            for (int i = 0; i < presets.Count; i++)
            {
                var testMusic = CreateInstance<MusicFileObject>();
                var testSound = CreateInstance<SoundFileObject>();

                if (presets[i].CanBeAppliedTo(testMusic))
                {
                    musicPresets.Add(presets[i]);
                    musicPresetNames.Add(presets[i].name);
                }
                else if (presets[i].CanBeAppliedTo(testSound))
                {
                    soundPresets.Add(presets[i]);
                    soundPresetNames.Add(presets[i].name);
                }

                presetToProp[presets[i]] = presets[i].FindProp("presetDescription");
            }

            if (FileType == AudioFileType.Music)
            {
                if (musicPresets.Count > 0) selectedPreset = musicPresets[SelectedMusicPresetIndex];
            }
            else
            {
                if (soundPresets.Count > 0) selectedPreset = soundPresets[SelectedSoundPresetIndex];
            }
        }

        void GenerateAudioFileObjects<T>(Preset preset) where T : BaseAudioFileObject
        {
            EditorUtility.DisplayProgressBar("Generating Audio File Objects", "Starting...", 0);
            int i = 0;
            for (; i < files.Count; i++)
            {
                if (EditorUtility.DisplayCancelableProgressBar("Generating Audio File Objects (" + i + "/" + files.Count + ")",
                    files[i].name, (float)i / (float)files.Count)) break;

                var newObject = CreateInstance<T>();
                preset.ApplyTo(newObject);
                SerializedObject newSO = new SerializedObject(newObject);
                var f = newSO.FindProperty("files");
                f.ClearArray();
                f.AddAndReturnNewArrayElement().objectReferenceValue = files[i];
                newSO.ApplyModifiedProperties();
                string finalPath = outputFolder + "/" + files[i].name + ".asset";
                if (!JSAMEditorHelper.CreateAssetSafe(newObject, finalPath))
                {
                    EditorUtility.DisplayDialog("Generation Interrupted", "Failed to create folders/assets!", "OK");
                    break;
                }
            }
            EditorUtility.ClearProgressBar();
            EditorUtility.FocusProjectWindow();
            EditorUtility.DisplayDialog(
                "Generation Finished",
                "Successfully generated " + i + "/" + files.Count + " files!", "OK");
            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(outputFolder));
        }

        void HandleDragAndDrop(Rect dragRect)
        {
            string label = files.Count == 0 ? "Drag AudioClips here" : "";

            if (JSAMEditorHelper.DragAndDropRegion(dragRect, label, "Release to Add AudioClips"))
            {
                List<AudioClip> duplicates = new List<AudioClip>();

                for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
                {
                    string filePath = AssetDatabase.GetAssetPath(DragAndDrop.objectReferences[i]);
                    var clips = JSAMEditorHelper.ImportAssetsOrFoldersAtPath<AudioClip>(filePath);

                    for (int j = 0; j < clips.Count; j++)
                    {
                        if (files.Contains(clips[j]))
                        {
                            duplicates.Add(clips[j]);
                            continue;
                        }

                        files.Add(clips[j]);
                    }
                }

                if (duplicates.Count > 0)
                {
                    string multiLine = string.Empty;
                    for (int i = 0; i < duplicates.Count; i++)
                    {
                        multiLine = duplicates[i].name + "/n";
                    }
                    EditorUtility.DisplayDialog("Duplicate Audio Clips!",
                        "The following Audio File Objects are already present in the wizard and have been skipped./n" + multiLine,
                        "OK");
                }
            }
        }
    }
}