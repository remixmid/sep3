using System;
using Microsoft.AspNetCore.SignalR;

namespace API.Hubs;

public class CoreHub : Hub {

    [HubMethodName("MessageSaved")]
    public async Task MessageSaved(Object payload)
        => throw new NotImplementedException();


    [HubMethodName("MessagePersisted")]
    public async Task MessagePersisted(Object payload)
        => throw new NotImplementedException();
}
