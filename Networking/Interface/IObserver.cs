using Networking.Context;

namespace Networking.Interface
{
public interface IObserver
{
    void Update(ISubscriber subject, IContext context);
}
}
