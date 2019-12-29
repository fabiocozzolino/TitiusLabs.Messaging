namespace TitiusLabs.Messaging
{
    public abstract class MessageBase : IMessage
    {
        public int Retry { get; set; } = 1;

        protected MessageBase() : this(1)
        {

        }

        protected MessageBase(int retry)
        {
            Retry = retry;
        }
    }
}
