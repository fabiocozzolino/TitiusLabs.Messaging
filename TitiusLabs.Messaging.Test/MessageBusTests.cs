using System.Threading;
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
            bool done = false;
            MessageBus.Current.Subscribe((SimpleMessaging sm) =>
            {
                done = true;
            });

            MessageBus.Current.Post(new SimpleMessaging());

            while(!done)
            {
                Thread.Sleep(1000);
            }

            Assert.Pass();
        }
    }
}