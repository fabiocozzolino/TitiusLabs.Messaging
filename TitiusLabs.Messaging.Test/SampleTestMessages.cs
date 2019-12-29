using System;
using System.Threading.Tasks;

namespace TitiusLabs.Messaging.Test
{
    public class SimpleMessage : MessageBase
    {
        public bool Active { get; set; }
    }

    public class AnotherSimpleMessage : MessageBase
    {

    }

    public class TargetClass
    {
        public TaskCompletionSource<bool> TaskCompleted = new TaskCompletionSource<bool>();
        public void Receive(SimpleMessage message)
        {
            TaskCompleted.SetResult(true);
        }
    }

}
