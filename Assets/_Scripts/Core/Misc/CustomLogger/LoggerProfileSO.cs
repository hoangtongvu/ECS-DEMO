using UnityEngine;

namespace Core.Misc.CustomLogger
{

    [CreateAssetMenu(fileName = "LoggerProfile", menuName = "SO/LoggerProfile")]
    public class LoggerProfileSO : ScriptableObject
    {
        [SerializeField] private bool logEnabled;

        [SerializeField] private string prefix;
        [SerializeField] private Color prefixColor = Color.black;
        [SerializeField] private string hexColor;

        private void OnValidate()
        {
            this.hexColor = "#" + ColorUtility.ToHtmlStringRGBA(this.prefixColor);
        }

        public void Log(string message)
        {
            if (!this.logEnabled) return;
            Debug.Log($"<color={this.hexColor}>{this.prefix}</color>: {message}");
        }

        public void Log(string message, Object sender)
        {
            if (!this.logEnabled) return;
            Debug.Log($"<color={this.hexColor}>{this.prefix}</color>: {message}", sender);
        }

    }
}