using System.Collections.Generic;
using ZBase.Foundation.PubSub;

namespace Core.Utilities.Extensions
{
    public static class SubscriptionExtensions
    {
        public static void AddTo(this ISubscription self, List<ISubscription> list)
            => list?.Add(self);

    }
}