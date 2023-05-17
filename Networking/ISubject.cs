using Networking.Context;

namespace Networking
{
public class ISubscriber
{
    List<IObserver> Observers { get; set; } = new();

    public void Subscribe(IObserver observer) => Observers.Add(observer);
    public void Unsubscribe(IObserver observer) => Observers.Remove(observer);

    protected void Notify(IContext context) => Observers.ForEach(observer => observer.Update(this, context));
}
}
