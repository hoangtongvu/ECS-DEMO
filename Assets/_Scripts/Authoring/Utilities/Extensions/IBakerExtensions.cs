using Unity.Entities;

namespace Authoring.Utilities.Extensions
{
    public static class IBakerExtensions
    {
        public static void AddAndDisableComponent<T>(this IBaker baker, in Entity entity)
            where T : struct, IEnableableComponent
        {
            baker.AddComponent<T>(entity);
            baker.SetComponentEnabled<T>(entity, false);
        }

    }

}
