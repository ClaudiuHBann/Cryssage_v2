using Networking.Protocol.Context.Operation;

namespace Networking.Protocol
{
public interface IObserver
{
    void Update(ISubscriber subject, IContextOperation contextOperation);
}
}
