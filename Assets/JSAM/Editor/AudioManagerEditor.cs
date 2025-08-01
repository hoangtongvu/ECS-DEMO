﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace JSAM.JSAMEditor
{
    /// <summary>
    /// Thank god to brownboot67 for his advice
    /// https://forum.unity.com/threads/custom-editor-not-saving-changes.424675/
    /// </summary>
    [CustomEditor(typeof(AudioManager))]
    public class AudioManagerEditor : Editor
    {
        static Color buttonPressedColor = new Color(1, 1, 1);

        AudioManager myScript;

        //static bool showAdvancedSettings;

        static bool showHowTo;

        SerializedProperty preloadedLibraries;

        List<string> excludedProperties = new List<string> { "m_Script" };

        const string SHOW_VOLUME = "JSAM_AUDIOMANAGER_SHOWVOLUME";
        static bool showVolume
        {
            get
            {
                if (!EditorPrefs.HasKey(SHOW_VOLUME)) EditorPrefs.SetBool(SHOW_VOLUME, false);
                return EditorPrefs.GetBool(SHOW_VOLUME);
            }
            set => EditorPrefs.SetBool(SHOW_VOLUME, value);
        }

        const string SHOW_LIBRARIES = "JSAM_AUDIOMANAGER_SHOWLIBRARY";
        static bool showLibraries
        {
            get
            {
                if (!EditorPrefs.HasKey(SHOW_LIBRARIES)) EditorPrefs.SetBool(SHOW_LIBRARIES, false);
                return EditorPrefs.GetBool(SHOW_LIBRARIES);
            }
            set => EditorPrefs.SetBool(SHOW_LIBRARIES, value);
        }

        private void OnEnable()
        {
            myScript = (AudioManager)target;

            myScript.EstablishSingletonDominance();

            preloadedLibraries = serializedObject.FindProperty(nameof(preloadedLibraries));

            Application.logMessageReceived += UnityDebugLog;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= UnityDebugLog;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
                    
            EditorGUILayout.BeginVertical(GUI.skin.box);
            if (AudioManager.Instance == myScript)
            {
                JSAMEditorHelper.BeginColourChange(Color.green);
                EditorGUILayout.LabelField("Looks good! This is the active AudioManager!", EditorStyles.boldLabel.ApplyTextAnchor(TextAnchor.MiddleCenter));
                JSAMEditorHelper.EndColourChange();
            }
            else
            {
                JSAMEditorHelper.BeginColourChange(Color.red);
                EditorGUILayout.LabelField("This is NOT the active AudioManager!", EditorStyles.boldLabel.ApplyTextAnchor(TextAnchor.MiddleCenter));
                JSAMEditorHelper.EndColourChange();
            }
            EditorGUILayout.EndVertical();

            RenderVolumeControls();

            EditorGUILayout.PropertyField(preloadedLibraries);

            if (Application.isPlaying)
            {
                var libraries = AudioManager.InternalInstance.LoadedLibraries;

                EditorGUILayout.LabelField($"Loaded Libraries: {libraries.Count}", EditorStyles.boldLabel);

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                if (libraries.Count == 0)
                {
                    EditorGUILayout.LabelField("No Libraries Loaded");
                }
                else
                {
                    foreach (var item in libraries)
                    {
                        // Usage of the Users variable is still undecided
                        //EditorGUILayout.LabelField($"{item.Value.Library.name} (Users: {item.Value.Users})");
                        if (GUILayout.Button($"{item.Value.Library.name}"))
                        {
                            AudioLibraryEditor.OnOpenAsset(item.Value.Library.GetInstanceID(), 0);
                        }
                    }
                }
                
                EditorGUILayout.EndVertical();
            }

            if (serializedObject.hasModifiedProperties)
            {
                serializedObject.ApplyModifiedProperties();
            }

            #region Quick Reference Guide
            showHowTo = EditorCompatability.SpecialFoldouts(showHowTo, "Quick Reference Guide");
            if (showHowTo)
            {
                JSAMEditorHelper.RenderHelpbox("Overview");
                JSAMEditorHelper.RenderHelpbox("This component is the backbone of the entire JSAM Audio Manager system and ideally should occupy it's own gameobject.");
                JSAMEditorHelper.RenderHelpbox("Remember to mouse over the various menu options in this and other JSAM windows to learn more about them!");
                JSAMEditorHelper.RenderHelpbox("Please ensure that you don't have multiple AudioManagers in one scene.");
                JSAMEditorHelper.RenderHelpbox(
                    "If you have any questions, suggestions or bug reports, feel free to open a new issue " +
                    "on Github repository's Issues page or send me an email directly!"
                    );

                EditorGUILayout.Space();

                JSAMEditorHelper.RenderHelpbox("Tips");
                JSAMEditorHelper.RenderHelpbox(
                    "The Github Repository is usually more up to date with bug fixes " + 
                    "than what's shown on the Unity Asset Store, so give it a look just in case!"
                    );
                JSAMEditorHelper.RenderHelpbox(
                    "Here are some helpful links, more of which can be found under\nWindows -> JSAM -> JSAM Startup"
                    );
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(new GUIContent("Report a Bug", "Click on me to go to the bug report page in a new browser window"), new GUILayoutOption[] { GUILayout.MinWidth(100) }))
                {
                    Application.OpenURL("https://github.com/jackyyang09/Simple-Unity-Audio-Manager/issues");
                }
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(new GUIContent("Github Releases", "Click on me to check out the latest releases in a new browser window"), new GUILayoutOption[] { GUILayout.MinWidth(100) }))
                {
                    Application.OpenURL("https://github.com/jackyyang09/Simple-Unity-Audio-Manager/releases");
                }
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(new GUIContent("Email", "You can find me at jackyyang267@gmail.com"), new GUILayoutOption[] { GUILayout.MinWidth(100) }))
                {
                    Application.OpenURL("mailto:jackyyang267@gmail.com");
                }
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
            EditorCompatability.EndSpecialFoldoutGroup();
            #endregion  
        }

        static void UnityDebugLog(string message, string stackTrace, LogType logType)
        {
            // Code from this steffen-itterheim
            // https://answers.unity.com/questions/482765/detect-compilation-errors-in-editor-script.html
            // if we receive a Debug.LogError we can assume that compilation failed
            if (logType == LogType.Error)
                EditorUtility.ClearProgressBar();
        }

        /// <summary>
        /// Just hides the fancy loading bar lmao
        /// </summary>
        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            EditorUtility.ClearProgressBar();
        }

        [MenuItem("GameObject/Audio/JSAM/Audio Manager", false, 1)]
        public static void AddAudioManager()
        {
            AudioManager existingAudioManager = JSAMCompatibility.FindObjectOfType<AudioManager>();
            if (!existingAudioManager)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("Audio Manager t:GameObject")[0]);
                GameObject newManager = (GameObject)Instantiate(AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)));
                if (Selection.activeTransform != null)
                {
                    newManager.transform.parent = Selection.activeTransform;
                    newManager.transform.localPosition = Vector3.zero;
                }
                newManager.name = newManager.name.Replace("(Clone)", string.Empty);
                EditorGUIUtility.PingObject(newManager);
                Selection.activeGameObject = newManager;
                Undo.RegisterCreatedObjectUndo(newManager, "Added new AudioManager");
            }
            else
            {
                EditorUtility.DisplayDialog("Error!", "AudioManager already exists in this scene!", "OK");
                Selection.activeObject = existingAudioManager.gameObject;
            }
        }

        void RenderVolumeControls()
        {
            showVolume = EditorCompatability.SpecialFoldouts(showVolume, new GUIContent("Volume Controls"));

            float master = 1, music = 1, sound = 1, voice = 1;
            bool masterMuted = false, musicMuted = false, soundMuted = false, voiceMuted = false;

            EditorGUILayout.BeginVertical(GUI.skin.box);
            using (new EditorGUI.DisabledGroupScope(!Application.isPlaying))
            {
                if (showVolume)
                {
                    if (Application.isPlaying)
                    {
                        master = AudioManager.MasterVolume;
                        music = AudioManager.MusicVolume;
                        sound = AudioManager.SoundVolume;
                        voice = AudioManager.VoiceVolume;
                        masterMuted = AudioManager.MasterMuted;
                        musicMuted = AudioManager.MusicMuted;
                        soundMuted = AudioManager.SoundMuted;
                        voiceMuted = AudioManager.VoiceMuted;
                    }
                    else
                    {
                        EditorGUILayout.LabelField("Volume can only be changed during runtime!", GUI.skin.label.ApplyWordWrap().ApplyBoldText());
                    }

                    EditorGUI.BeginChangeCheck();
                    var tuple = RenderVolumeSlider("Master", master, masterMuted);
                    if (EditorGUI.EndChangeCheck())
                    {
                        AudioManager.MasterVolume = tuple.Item1;
                        AudioManager.MasterMuted = tuple.Item2;
                    }

                    EditorGUI.BeginChangeCheck();
                    tuple = RenderVolumeSlider("Music", music, musicMuted);
                    if (EditorGUI.EndChangeCheck())
                    {
                        AudioManager.MusicVolume = tuple.Item1;
                        AudioManager.MusicMuted = tuple.Item2;
                    }

                    EditorGUI.BeginChangeCheck();
                    tuple = RenderVolumeSlider("Sound", sound, soundMuted);
                    if (EditorGUI.EndChangeCheck())
                    {
                        AudioManager.SoundVolume = tuple.Item1;
                        AudioManager.SoundMuted = tuple.Item2;
                    }

                    EditorGUI.BeginChangeCheck();
                    tuple = RenderVolumeSlider("Voice", voice, voiceMuted);
                    if (EditorGUI.EndChangeCheck())
                    {
                        AudioManager.VoiceVolume = tuple.Item1;
                        AudioManager.VoiceMuted = tuple.Item2;
                    }
                }
            }
            EditorGUILayout.EndVertical();
            EditorCompatability.EndSpecialFoldoutGroup();
        }

        (float, bool) RenderVolumeSlider(string channelName, float volume, bool muted)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent(channelName + " Volume"), new GUILayoutOption[] { GUILayout.MaxWidth(90) });
            if (muted) JSAMEditorHelper.BeginColourChange(buttonPressedColor);
            if (JSAMEditorHelper.CondensedButton(" MUTE "))
            {
                muted = !muted;
            }
            if (muted) JSAMEditorHelper.EndColourChange();
            using (new EditorGUI.DisabledGroupScope(muted))
            {
                volume = EditorGUILayout.Slider(volume, 0, 1);
            }
            EditorGUILayout.EndHorizontal();
            return (volume, muted);
        }
    }
}