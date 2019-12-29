using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace TitiusLabs.Messaging.Test
{
    public class MessageBusSubscribeTests
    {

        [Test]
        public void Subscribe_DifferentMessage_CalledOnce()
        {
            var firstTask = new TaskCompletionSource<bool>();
            var secondTask = new TaskCompletionSource<bool>();
            var firstSubscriberCount = 0;
            var secondSubscriberCount = 0;
            MessageBus.Current.Subscribe((SimpleMessage sm) =>
            {
                firstSubscriberCount++;
                firstTask.SetResult(true);
            });
            MessageBus.Current.Subscribe((AnotherSimpleMessage sm) =>
            {
                secondSubscriberCount++;
                secondTask.SetResult(true);
            });

            MessageBus.Current.Publish(new SimpleMessage());
            MessageBus.Current.Publish(new AnotherSimpleMessage());

            var firstResult = firstTask.Task.Result;
            var secondResult = secondTask.Task.Result;

            MessageBus.Current.UnsubscribeAll<SimpleMessage>();
            MessageBus.Current.UnsubscribeAll<AnotherSimpleMessage>();

            Assert.IsTrue(firstResult, "First subscriber not invoked");
            Assert.IsTrue(secondResult, "Second subscriber not invoked");
            Assert.AreEqual(1, firstSubscriberCount, "First subscriber not invoked as expected");
            Assert.AreEqual(1, secondSubscriberCount, "Second subscriber not invoked as expected");
        }

        [Test]
        public void Subscribe_SameMessageWithDifferentFilter_CalledOnce()
        {
            var firstTask = new TaskCompletionSource<bool>();
            var secondTask = new TaskCompletionSource<bool>();
            var firstSubscriberCount = 0;
            var secondSubscriberCount = 0;
            MessageBus.Current.Subscribe((SimpleMessage sm) =>
            {
                firstSubscriberCount++;
                firstTask.SetResult(true);
            });
            MessageBus.Current.Subscribe((SimpleMessage sm) =>
            {
                secondSubscriberCount++;
                secondTask.SetResult(true);
            }, (sm) => sm.Active);

            MessageBus.Current.Publish(new SimpleMessage() { Active = true });
            MessageBus.Current.Publish(new SimpleMessage() { Active = false }); 

            var firstResult = firstTask.Task.Result;
            var secondResult = secondTask.Task.Result;

            MessageBus.Current.UnsubscribeAll<SimpleMessage>();

            Assert.IsTrue(firstResult, "Subscriber one not invoked");
            Assert.IsTrue(secondResult, "Subscriber two not invoked");
            Assert.AreEqual(2, firstSubscriberCount, "Subscriber one not called as expected");
            Assert.AreEqual(1, secondSubscriberCount, "Subscriber two not called as expected");
        }
    }
}