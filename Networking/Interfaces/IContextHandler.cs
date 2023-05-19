using Networking.Context;
using Networking.Context.File;

namespace Networking.Interfaces
{
    public interface IContextHandler
    {
        // discover
        public void OnDiscover(ContextDiscover context);

        // send
        public void OnSendProgress(ContextProgress context);

        // receive
        public void OnReceiveText(ContextText context);
        public void OnReceiveFileInfo(ContextFileInfo context);
        public void OnReceiveProgress(ContextProgress context);
    }
}
