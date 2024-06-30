using Components.GameResource;
using System.Linq;
using Unity.Entities;

namespace Utilities.Helpers
{
    public static class ResourceWalletHelper
    {
        public static void TryAddResourceChanged(
            DynamicBuffer<ResourceWalletChangedElement> buffer
            , ResourceWalletChangedElement element)
        {
            if (buffer.Contains(element)) return;
            buffer.Add(element);
        }
    }
}