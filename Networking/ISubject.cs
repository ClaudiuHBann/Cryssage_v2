using Networking.Protocol.Context.Operation;

namespace Networking
{
    public class ISubscriber
    {
        protected List<IObserver> Observers { get; set; } = new();

        public virtual void Subscribe(IObserver observer)
        {
            Observers.Add(observer);
        }

        public virtual void Unsubscribe(IObserver observer)
        {
            Observers.Remove(observer);
        }

        protected virtual void Notify(IContextOperation contextOperation)
        {
            foreach (var observer in Observers)
            {
                observer.Update(this, contextOperation);
            }
        }
    }
}
