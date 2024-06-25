using UnityEngine;

namespace Core.Misc.CustomLogger
{
    public sealed class MainLoggersCtrl
    {
        [SerializeField] private LoggerProfileSO systemLogger;
        [SerializeField] private LoggerProfileSO gameplayLogger;


        private static MainLoggersCtrl instance;
        private static MainLoggersCtrl Instance => instance ??= new();


        public static LoggerProfileSO SystemLogger => Instance.systemLogger;
        public static LoggerProfileSO GameplayLogger => Instance.gameplayLogger;



#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void ClearOnLoad()
        {
            DestroyInstance();
        }
#endif

        public static void DestroyInstance() => instance = null;



        private MainLoggersCtrl() => this.LoadAllLoggers();

        private void LoadAllLoggers()
        {
            LoadLoggerProfile(out this.systemLogger, "SystemLoggerProfile");
            LoadLoggerProfile(out this.gameplayLogger, "GameplayLoggerProfile");
        }

        private static void LoadLoggerProfile(out LoggerProfileSO logger, string fileName)
        {
            logger = Resources.Load<LoggerProfileSO>("LoggerProfiles/" + fileName);
            if (logger != null) return;
            Debug.LogError($"Can't load LoggerProfile with name {fileName}");
        }


    }
}