using System;
using System.Collections.Generic;
using System.Text;

namespace TitiusLabs.Messaging
{
    internal static class Extensions
    {
        public static bool CanRetry(this IMessage message)
        {
            var retryCount = message is IRetryMessage 
                ? --(message as IRetryMessage).Retry 
                : 0;

            return retryCount > 0;
        }
    }
}
