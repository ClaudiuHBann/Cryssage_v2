using Networking.Protocol.Context.Operation;

namespace Networking
{
    public interface IObserver
    {
        void Update(ISubscriber subject, IContextOperation contextOperation);
    }
}
