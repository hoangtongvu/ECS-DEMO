﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Audio;

namespace JSAM
{
    public class JSAMSettings : ScriptableObject
    {
        [Tooltip("Specify your custom audio assembly here")]
        [SerializeField] string rootAudioAssembly;
        public string RootAudioAssembly => rootAudioAssembly;

        [Tooltip("Ensures that the AudioManager you think you're referring to actually exists in this scene")]
        [SerializeField] bool establishSingletonDominance = true;
        public bool EstablishSingletonDominance => establishSingletonDominance;
        
        [Tooltip("If true, enables 3D spatialized audio for all sound effects, does not effect music")]
        [SerializeField] bool spatialSound = true;
        public bool Spatialize => spatialSound;

        /// <summary>
        /// Number of Sound Channels to be created on start
        /// </summary>
        [Tooltip("Number of Sound Channels to be created on start")]
        [SerializeField] int startingSoundChannels = 16;
        public int StartingSoundChannels => startingSoundChannels;

        [Tooltip("Number of Music Channels to be created on start")]
        [SerializeField] int startingMusicChannels = 3;
        public int StartingMusicChannels => startingMusicChannels;

        [Tooltip("If the maxDistance property of an Audio File Object is left at 0, then this value will be used as a substitute.")]
        [SerializeField] float defaultSoundMaxDistance = 7;
        public float DefaultSoundMaxDistance => defaultSoundMaxDistance;

        [Tooltip("Affects how AudioClip lists are displayed in FileObject inspectors. " +
            "Toggle this option if you're experiencing issues manipulating Audio Clips in the inspector")]
        [SerializeField] bool useBuiltInAudioListRenderer = 
#if UNITY_2020_3_OR_NEWER
            true;
#else
            false;
#endif
        public bool UseBuiltInAudioListRenderer => useBuiltInAudioListRenderer;

        /// <summary>
        /// If true, AudioManager no longer prints info to the console. Does not affect AudioManager errors/warnings
        /// </summary>
        [Tooltip("If true, AudioManager no longer prints info to the console. Does not affect AudioManager errors/warnings")]
        [SerializeField] bool disableConsoleLogs = false;
        public bool DisableConsoleLogs => disableConsoleLogs;

        /// <summary>
        /// If true, keeps AudioManager alive through scene loads. You're recommended to disable this if your AudioManager is instanced
        /// </summary>
        [Tooltip("If true, keeps AudioManager alive through scene loads. You're recommended to disable this if your AudioManager is instanced")]
        [SerializeField] bool dontDestroyOnLoad = true;
        public new bool DontDestroyOnLoad => dontDestroyOnLoad;

        /// <summary>
        /// If true, adds more Audio Sources automatically if you exceed the starting count, you are recommended to keep this enabled
        /// </summary>
        [Tooltip("If true, adds more Audio Sources automatically if you exceed the starting count, you are recommended to keep this enabled")]
        [SerializeField] bool dynamicSourceAllocation = true;
        public bool DynamicSourceAllocation => dynamicSourceAllocation;

        [Tooltip("The AudioManager will instantiate this prefab during runtime to play sounds from. If null, will use default AudioSource settings.")]
        [SerializeField] GameObject soundChannelPrefabOverride;
        public GameObject SoundChannelPrefab => soundChannelPrefabOverride;
        [Tooltip("The AudioManager will instantiate this prefab during runtime to play music from. If null, will use default AudioSource settings.")]
        [SerializeField] GameObject musicChannelPrefabOverride;
        public GameObject MusicChannelPrefab => musicChannelPrefabOverride;

        /// <summary>
        /// If true, stops all sounds when you change the active scene
        /// </summary>
        [Tooltip("If true, stops all sounds when you change the active scene")]
        [SerializeField] bool stopSoundsOnSceneChanged = false;
        public bool StopSoundsOnSceneChanged => stopSoundsOnSceneChanged;

        /// <summary>
        /// If true, stops all music when you change the active scene
        /// </summary>
        [Tooltip("If true, stops all sounds when you change the active scene")]
        [SerializeField] bool stopMusicOnSceneChanged = false;
        public bool StopMusicOnSceneChanged => stopMusicOnSceneChanged;

        [Tooltip("Use if spatialized sounds are spatializing late when playing in-editor, known to happen with the Oculus SDK")]
        [SerializeField] bool spatializeLateUpdate = false;
        public bool SpatializeOnLateUpdate => spatializeLateUpdate;

        /// <summary>
        /// Specifies how your audio channels will follow their targets in 3D space during runtime. 
        /// Only applies if you have Spatial Sound enabled
        /// </summary>
        public enum SpatializeUpdateMode
        {
            /// <summary>
            /// Audio channels track their targets in world space every Update.
            /// </summary>
            Default,
            /// <summary>
            /// Audio channels track their targets in FixedUpdate. 
            /// Good for targets that move during FixedUpdate
            /// </summary>
            FixedUpdate,
            /// <summary>
            /// Audio channels track their targets in LateUpdate. 
            /// Good for targets that move during LateUpdate
            /// </summary>
            LateUpdate,
            /// <summary>
            /// Audio channels are parented in the hierarchy to their targets. 
            /// Less performance overhead, but will clutter your object hierarchies during runtime
            /// </summary>
            Parented
        }

        [Tooltip("Default - Audio Channels track their targets in World Space every update.\n\n" +
            "FixedUpdate - Audio channels track their targets in FixedUpdate. Good for targets that move during FixedUpdate.\n\n" +
            "LateUpdate - Same as FixedUpdate but in LateUpdate instead.\n\n" +
            "Parented - Audio channels are parented in the hierarchy to their targets. " +
            "Slightly less performance overhead, but will clutter your object hierarchies during runtime.")]
        [SerializeField] SpatializeUpdateMode spatializationMode;
        public SpatializeUpdateMode SpatializationMode => spatializationMode;

        [Tooltip("Changes the pitch of sounds according to Time.timeScale. When Time.timeScale is set to 0, pauses all sounds instead")]
        [SerializeField] bool timeScaledSounds = true;
        public bool TimeScaledSounds => timeScaledSounds;

        [SerializeField] AudioMixer mixer;
        public AudioMixer Mixer => mixer;

        [SerializeField] AudioMixerGroup masterGroup;
        public AudioMixerGroup MasterGroup => masterGroup;

        [SerializeField] AudioMixerGroup musicGroup;
        public AudioMixerGroup MusicGroup => musicGroup;

        [SerializeField] AudioMixerGroup soundGroup;
        public AudioMixerGroup SoundGroup => soundGroup;

        [SerializeField] AudioMixerGroup voiceGroup;
        public AudioMixerGroup VoiceGroup => voiceGroup;

        [Tooltip("If true, will save volume settings into PlayerPrefs and automatically loads previous volume settings on play. ")]
        [SerializeField] bool saveVolumeToPlayerPrefs = true;
        public bool SaveVolumeToPlayerPrefs => saveVolumeToPlayerPrefs;

        [SerializeField] string masterVolumeKey = "JSAM_MASTER_VOL";
        [SerializeField] string masterMutedKey = "JSAM_MASTER_MUTE";
        public string MasterVolumeKey => masterVolumeKey;
        public string MasterMutedKey => masterMutedKey;
        [SerializeField] string musicVolumeKey = "JSAM_MUSIC_VOL";
        [SerializeField] string musicMutedKey = "JSAM_MUSIC_MUTE";
        public string MusicVolumeKey => musicVolumeKey;
        public string MusicMutedKey => musicMutedKey;
        [SerializeField] string soundVolumeKey = "JSAM_SOUND_VOL";
        [SerializeField] string soundMutedKey = "JSAM_SOUND_MUTE";
        public string SoundVolumeKey => soundVolumeKey;
        public string SoundMutedKey => soundMutedKey;
        [SerializeField] string voiceVolumeKey = "JSAM_VOICE_VOL";
        [SerializeField] string voiceMutedKey = "JSAM_VOICE_MUTE";
        public string VoiceVolumeKey => voiceVolumeKey;
        public string VoiceMutedKey => voiceMutedKey;

        [Tooltip("The font size used when rendering \"quick reference guides\" in JSAM editor windows")]
        [SerializeField] int quickReferenceFontSize = 10;
        public int QuickReferenceFontSize
        {
            get
            {
                return quickReferenceFontSize;
            }
        }

        static JSAMSettings settings;
        public static JSAMSettings Settings
        {
            get
            {
                if (settings == null)
                {
                    var asset = Resources.Load(nameof(JSAMSettings));
                    settings = asset as JSAMSettings;
#if UNITY_EDITOR
                    if (settings == null) TryCreateNewSettingsAsset();
#endif
                }
                return settings;
            }
        }

#if UNITY_EDITOR
        static readonly string SETTINGS_PATH = "Assets/Settings/Resources/" + nameof(JSAMSettings) + ".asset";

        public static void TryCreateNewSettingsAsset()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating) return;

            if (!EditorUtility.DisplayDialog(
                "JSAM First Time Setup",
                "In order to function, JSAM needs a place to store settings. By default, a " +
                "Settings asset will be created at " + SETTINGS_PATH + ", but you may move it " +
                "elsewhere, so long as it's in a Resources folder.\n" +
                "Moving it out of the Resources folder will prompt this message to appear again erroneously!",
                "Ok Create It.", "Not Yet!")) return;

            var asset = CreateInstance<JSAMSettings>();
            if (!AssetDatabase.IsValidFolder("Assets/Settings")) AssetDatabase.CreateFolder("Assets", "Settings");
            if (!AssetDatabase.IsValidFolder("Assets/Settings/Resources")) AssetDatabase.CreateFolder("Assets/Settings", "Resources");
            AssetDatabase.CreateAsset(asset, SETTINGS_PATH);
            asset.Reset();

            settings = asset;
            EditorUtility.DisplayDialog("JSAM Settings", "Settings asset created successfully!", "Cool.");
        }

        static SerializedObject serializedObject;
        public static SerializedObject SerializedObject
        {
            get
            {
                if (serializedObject == null)
                {
                    serializedObject = new SerializedObject(Settings);
                    return serializedObject;
                }
                return serializedObject;
            }
        }

        public void ResetEditor()
        {
            SerializedObject.FindProperty(nameof(disableConsoleLogs)).boolValue = false;
            SerializedObject.FindProperty(nameof(quickReferenceFontSize)).intValue = 10;
            SerializedObject.FindProperty(nameof(useBuiltInAudioListRenderer)).boolValue = 
#if UNITY_2020_3_OR_NEWER
                true;
#else
                false;
#endif
            SerializedObject.ApplyModifiedProperties();
        }

        public void Reset()
        {
        }
#endif
    }
}