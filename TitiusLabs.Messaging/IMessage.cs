namespace TitiusLabs.Messaging
{
    public interface IMessage
    {
    }

    public interface IRetryMessage : IMessage
    {
        int Retry { get; set; }
    }
}
