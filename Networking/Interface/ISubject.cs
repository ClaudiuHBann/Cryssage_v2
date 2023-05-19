using Networking.Context;

namespace Networking.Interface
{
public class ISubscriber
{
    public enum Type_ : byte
    {
        UNKNOWN,
        TCP,
        UDP_BROADCAST
    }

    public Type_ Type { get; set; } = Type_.UNKNOWN;

    protected ISubscriber(Type_ type)
    {
        Type = type;
    }

    List<IObserver> Observers { get; set; } = new();

    public void Subscribe(IObserver observer) => Observers.Add(observer);
    public void Unsubscribe(IObserver observer) => Observers.Remove(observer);

    protected void Notify(IContext context) => Observers.ForEach(observer => observer.Update(this, context));
}
}
