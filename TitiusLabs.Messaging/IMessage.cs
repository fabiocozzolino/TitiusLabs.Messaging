namespace TitiusLabs.Messaging
{
    public interface IMessage
    {
        int Retry { get; set; }
    }
}
