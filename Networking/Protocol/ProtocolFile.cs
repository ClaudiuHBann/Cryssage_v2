﻿using Networking.Protocol.Context.Operation;
using Parser.Message;

namespace Networking.Protocol
{
public class ProtocolFile : IProtocol
{
    public ContextOperationFile ContextOperation { get; set; }

    public ProtocolFile(ContextOperationFile contextOperation)
    {
        ContextOperation = contextOperation;
    }

    public Message Exchange(Message message)
    {
        throw new NotImplementedException();
    }
}
}