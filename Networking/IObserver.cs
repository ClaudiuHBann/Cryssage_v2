using Networking.Context;

namespace Networking
{
public interface IObserver
{
    void Update(ISubscriber subject, IContext context);
}
}
