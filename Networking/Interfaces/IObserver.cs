using Networking.Context;

namespace Networking.Interfaces
{
public interface IObserver
{
    void Update(ISubscriber subject, IContext context);
}
}
