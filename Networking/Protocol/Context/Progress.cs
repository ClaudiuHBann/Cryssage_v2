using Networking.Protocol.Context.Operation;

namespace Networking.Protocol.Context
{
public class IProgress
{
    public delegate void OnProgressDelegate(IContextOperation contextOperation);
    public OnProgressDelegate OnProgress { get; set; } = new((_) =>
                                                                 {});
}
}
