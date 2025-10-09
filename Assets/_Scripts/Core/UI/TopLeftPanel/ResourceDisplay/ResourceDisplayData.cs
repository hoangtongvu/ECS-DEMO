using Core.GameResource;
using System;

namespace Core.UI.TopLeftPanel.ResourceDisplay
{
    [Serializable]
    public class ResourceDisplayData
    {
        public ResourceType ResourceType;
        public uint ResourceQuantity;
    }
}