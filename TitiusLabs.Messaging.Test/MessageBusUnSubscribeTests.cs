using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace TitiusLabs.Messaging.Test
{
    public class MessageBusUnSubscribeTests
    {
        [Test]
        public void UnSubscribeByToken_OneSubscriber_NotInvokedAfter()
        {
            var tcs = new TaskCompletionSource<bool>();
            var count = 0;
            bool result = false;

            var token = MessageBus.Current.Subscribe((SimpleMessage sm) =>
            {
                count++;
                tcs.SetResult(true);
            }); 

            MessageBus.Current.Publish(new SimpleMessage());
            result = tcs.Task.Result;

            MessageBus.Current.Unsubscribe(token);

            Assert.IsTrue(result);
            Assert.AreEqual(count, 1);
        }

        [Test]
        public void UnSubscribeByDisposing_OneSubscriber_NotInvokedAfter()
        {
            var tcs = new TaskCompletionSource<bool>();
            var count = 0;
            bool result = false;

            using (var token = MessageBus.Current.Subscribe((SimpleMessage sm) =>
            {
                count++;
                tcs.SetResult(true);
            }))
            {

                MessageBus.Current.Publish(new SimpleMessage());
                result = tcs.Task.Result;
            }

            Assert.IsTrue(result);
            Assert.AreEqual(count, 1);
        }
    }
}