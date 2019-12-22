using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace TitiusLabs.Messaging.Test
{
    public class MessageBusTests
    {
        public class SimpleMessaging : IMessage
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
            MessageBus.Current.Subscribe((SimpleMessaging sm) =>
            {
                tcs.SetResult(true);
            });

            MessageBus.Current.Post(new SimpleMessaging());

            var result = tcs.Task.Result;

            MessageBus.Current.UnSubscribeAll<SimpleMessaging>();

            Assert.IsTrue(result);
        }

        [Test]
        public void Post_MultipleSubscriber_Called()
        {
            var tcs1 = new TaskCompletionSource<bool>();
            var tcs2 = new TaskCompletionSource<bool>();

            MessageBus.Current.Subscribe((SimpleMessaging sm) =>
            {
                tcs1.SetResult(true);
            });
            MessageBus.Current.Subscribe((SimpleMessaging sm) =>
            {
                tcs2.SetResult(true);
            });

            MessageBus.Current.Post(new SimpleMessaging());

            var result1 = tcs1.Task.Result;
            var result2 = tcs2.Task.Result;

            MessageBus.Current.UnSubscribeAll<SimpleMessaging>();

            Assert.IsTrue(result1 && result2);
        }
    }
}