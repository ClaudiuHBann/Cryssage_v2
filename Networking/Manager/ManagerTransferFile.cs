using System.Collections.Concurrent;

namespace Networking.Manager
{
public class ManagerTransferFile
{
    public ConcurrentDictionary<Guid, string> Files { get; } = new();
}
}
