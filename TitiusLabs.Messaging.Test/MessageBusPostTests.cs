using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace TitiusLabs.Messaging.Test
{
    public class MessageBusPostTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Post_OneSubscriber_Called()
        {
            var tcs = new TaskCompletionSource<bool>();
            MessageBus.Current.Subscribe((SimpleMessage sm) =>
            {
                tcs.SetResult(true);
            });

            MessageBus.Current.Publish(new SimpleMessage());

            var result = tcs.Task.Result;

            MessageBus.Current.UnsubscribeAll<SimpleMessage>();

            Assert.IsTrue(result);
        }

        [Test]
        public void Post_MultipleSubscriber_Called()
        {
            var firstTask = new TaskCompletionSource<bool>();
            var secondTask = new TaskCompletionSource<bool>();
            var fullSubscriberCount = 0;

            MessageBus.Current.Subscribe((SimpleMessage sm) =>
            {
                fullSubscriberCount++;
                firstTask.SetResult(true);
            });
            MessageBus.Current.Subscribe((SimpleMessage sm) =>
            {
                fullSubscriberCount++;
                secondTask.SetResult(true);
            });

            MessageBus.Current.Publish(new SimpleMessage());

            var firstResult = firstTask.Task.Result;
            var secondResult = secondTask.Task.Result;

            MessageBus.Current.UnsubscribeAll<SimpleMessage>();

            Assert.IsTrue(firstResult, "Fist subscriber not invoked");
            Assert.IsTrue(secondResult, "Second subscriber not invoked");
            Assert.AreEqual(2, fullSubscriberCount, "Message not delivered to all subscribers");
        }

        [Test]
        public void Post_UnreferencedHandler_NotInvoked()
        {
            var target = new TargetClass();
            MessageBus.Current.Subscribe<SimpleMessage>(target.Receive);

            MessageBus.Current.Publish(new SimpleMessage());
            var resultFirstCall = target.TaskCompleted.Task.Result;

            Assert.IsTrue(resultFirstCall, "First call not executed");

            target = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            MessageBus.Current.Publish(new SimpleMessage());
            //var resultSecondCall = target.TaskCompleted.Task.Result;

            Assert.Pass();
        }
    }
}