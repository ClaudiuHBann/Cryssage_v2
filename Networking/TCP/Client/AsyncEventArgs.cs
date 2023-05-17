using System.Net.Sockets;

namespace Networking.TCP.Client
{
public class AsyncEventArgs
{
    public enum Type_
    {
        UNKNOWN,   // populates context with default values
        CONNECT,   // populates context with socket error and connected
        PROGRESS,  // populates context with socket error and bytes transferred/received in total
        RECEIVE,   // populates context with socket error and stream
        DISCONNECT // populates context with socket error
    }

    public Type_ Type { get; set; } = Type_.UNKNOWN;
    public SocketError Error { get; set; } = SocketError.Success;
    public bool Connected { get; set; } = false;
    public uint BytesTransferredTotal { get; set; } = 0;
    public byte[]? Stream { get; set; } = null;
    public bool Done { get; set; } = false;

    public AsyncEventArgs(SocketError error, bool connected)
    {
        Type = Type_.CONNECT;
        Error = error;
        Connected = connected;
        Done = true;
    }

    public AsyncEventArgs(SocketError error)
    {
        Type = Type_.DISCONNECT;
        Error = error;
        Done = true;
    }

    public AsyncEventArgs(SocketError error, byte[]? stream, bool done = false)
    {
        Type = Type_.RECEIVE;
        Error = error;
        Stream = stream;
        Done = done;
    }

    public AsyncEventArgs(SocketError error, uint bytesTransferredTotal, bool done = false)
    {
        Type = Type_.PROGRESS;
        Error = error;
        BytesTransferredTotal = bytesTransferredTotal;
        Done = done;
    }
}
}
