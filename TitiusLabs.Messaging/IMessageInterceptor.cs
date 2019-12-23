namespace TitiusLabs.Messaging
{
    public interface IMessageInterceptor
    {
        IMessage Execute(IMessage message);
    }
}
