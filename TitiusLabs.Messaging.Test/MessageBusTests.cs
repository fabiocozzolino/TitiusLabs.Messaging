using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace TitiusLabs.Messaging.Test
{
    public class MessageBusTests
    {
        public class SimpleMessage : IMessage
        {

        }

        public class AnotherSimpleMessage : IMessage
        {

        }

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

            MessageBus.Current.Post(new SimpleMessage());

            var result = tcs.Task.Result;

            MessageBus.Current.UnSubscribeAll<SimpleMessage>();

            Assert.IsTrue(result);
        }

        [Test]
        public void Post_MultipleSubscriber_Called()
        {
            var tcs1 = new TaskCompletionSource<bool>();
            var tcs2 = new TaskCompletionSource<bool>();
            var count = 0;

            MessageBus.Current.Subscribe((SimpleMessage sm) =>
            {
                count++;
                tcs1.SetResult(true);
            });
            MessageBus.Current.Subscribe((SimpleMessage sm) =>
            {
                count++;
                tcs2.SetResult(true);
            });

            MessageBus.Current.Post(new SimpleMessage());

            var result1 = tcs1.Task.Result;
            var result2 = tcs2.Task.Result;

            MessageBus.Current.UnSubscribeAll<SimpleMessage>();

            Assert.IsTrue(result1 && result2 && count == 2);
        }

        [Test]
        public void Subscribe_DifferentMessage_CalledOnce()
        {
            var tcs1 = new TaskCompletionSource<bool>();
            var tcs2 = new TaskCompletionSource<bool>();
            var count1 = 0;
            var count2 = 0;
            MessageBus.Current.Subscribe((SimpleMessage sm) =>
            {
                count1++;
                tcs1.SetResult(true);
            });
            MessageBus.Current.Subscribe((AnotherSimpleMessage sm) =>
            {
                count2++;
                tcs2.SetResult(true);
            });

            MessageBus.Current.Post(new SimpleMessage());
            MessageBus.Current.Post(new AnotherSimpleMessage());

            var result1 = tcs1.Task.Result;
            var result2 = tcs2.Task.Result;

            MessageBus.Current.UnSubscribeAll<SimpleMessage>();
            MessageBus.Current.UnSubscribeAll<AnotherSimpleMessage>();

            Assert.IsTrue(result1 && result2);
            Assert.IsTrue(count1 == 1 && count2 == 1);
        }
    }
}