using System.Collections.Generic;
using ZBase.Foundation.PubSub;

namespace Utilities.Extensions
{
    public static class SubscriptionExtensions
    {
        public static void AddTo(this ISubscription self, List<ISubscription> list)
            => list?.Add(self);

    }
}