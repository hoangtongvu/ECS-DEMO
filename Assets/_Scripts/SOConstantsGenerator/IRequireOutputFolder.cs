using UnityEditor;

namespace SOConstantsGenerator;

public interface IRequireOutputFolder
{
    public DefaultAsset OutputFolder { get; set; }
}