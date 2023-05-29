using Networking.Context.Interface;

namespace Networking.Protocol
{
    public abstract class IProtocol
{
    public IContextHandler ContextHandler { get; set; }

    public IProtocol(IContextHandler contextHandler)
    {
        ContextHandler = contextHandler;
    }

    public virtual IContext GetNextContext(IContext context) => IContext.CreateEOS();
    public abstract IContext Exchange(IContext context);
}
}
