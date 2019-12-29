using System;
using System.Reflection;
using System.Threading.Tasks;

namespace TitiusLabs.Messaging
{
    internal abstract class MessageSubscription
    {
        public static MessageSubscription<TMessage> Create<TMessage>(Action<TMessage> subscriber, Func<TMessage, bool> filter, TaskScheduler scheduler) where TMessage : IMessage
        {
            return new MessageSubscription<TMessage>
            {
                Subscriber = new WeakReference<Action<TMessage>>(subscriber),
                Scheduler = scheduler,
                Filter = new WeakReference<Func<TMessage, bool>>(filter)
            };
        }

        public ISubscriptionToken SubscriptionToken { get; set; }

        public TaskScheduler Scheduler { get; set; }

        public abstract void Dispatch(IMessage message);
        internal abstract MethodInfo GetSubscriberType();
    }

    internal class MessageSubscription<TMessage> : MessageSubscription where TMessage : IMessage
    {
        public WeakReference<Action<TMessage>> Subscriber { get; set; }
        public WeakReference<Func<TMessage, bool>> Filter { get; set; }

        public override void Dispatch(IMessage message)
        {
            Filter.TryGetTarget(out var filter);

            var typedMessage = (TMessage)message;
            if ((filter?.Invoke(typedMessage) ?? true) && Subscriber.TryGetTarget(out var subscriber))
            {
                subscriber.Invoke(typedMessage);
            }
        }

        internal override MethodInfo GetSubscriberType()
        {
            return Subscriber.TryGetTarget(out var target) ? target.Method : null;
        }
    }
}
