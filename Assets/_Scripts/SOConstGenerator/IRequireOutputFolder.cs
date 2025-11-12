using UnityEditor;

namespace SOConstGenerator;

public interface IRequireOutputFolder
{
    public DefaultAsset OutputFolder { get; set; }
}