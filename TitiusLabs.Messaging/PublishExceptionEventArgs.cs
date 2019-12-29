using System;
using System.Reflection;

namespace TitiusLabs.Messaging
{
    public class PublishExceptionEventArgs : EventArgs
    {
        public Exception Exception { get; set; }
        public IMessage Message { get; set; }
        public MethodInfo SubscriberType { get; set; }
    }
}