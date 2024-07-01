using Core.MyEntity;
using UnityEngine;

namespace Core.Tool
{
    [CreateAssetMenu(fileName = "ToolProfile", menuName = "SO/MyEntity/ToolProfile")]
    public class ToolProfileSO : EntityProfileSO
    {
        [Header("Tool Profile")]
        public ToolType ToolType;
    }
}