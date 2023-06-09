﻿using Parser.Message;

using Newtonsoft.Json;

using Networking.Context.Interface;

namespace Networking.Context.Request
{
public class ContextDiscoverRequest : ContextRequest
{
    public ContextDiscoverRequest() : base(Message.Type.DISCOVER, Guid.NewGuid())
    {
    }

    public override byte[] ToStream() => Utility.ENCODING_DEFAULT.GetBytes(JsonConvert.SerializeObject(this));
}
}
