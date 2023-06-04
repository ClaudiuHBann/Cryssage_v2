using System.Collections.Concurrent;
using System.Collections.ObjectModel;

using System.Net;
using Networking;
using Networking.Manager;

using Networking.Context;
using Networking.Context.Discover;
using Networking.Context.Interface;

using Cryssage.Views;
using Cryssage.Models;
using Cryssage.Events;

namespace Cryssage
{
public class Context : IContextHandler
{
    readonly ManagerNetwork managerNetwork;

    readonly UserModelView viewUser;
    public UserModel UserSelected { get; set; }

    public EventsGUI EventsGUI { get; } = new();

    readonly ConcurrentDictionary<string, bool> clientIpToOnlineStates = new();
    readonly Thread threadBroadcast;
    int threadBroadcastRunning = 1;

    public Context(UserModelView uv)
    {
        managerNetwork = new(this, new());
        viewUser = uv;

        threadBroadcast = new Thread(ThreadBroadcast);
        threadBroadcast.Start();

#if DEBUG
        var userNew = new UserModel(IPAddress.Loopback.ToString(), "dotnet_bot.png", "Pulea");
        viewUser.Items.Add(userNew);
#endif
    }

    public void Destructor()
    {
        Interlocked.Decrement(ref threadBroadcastRunning);
        threadBroadcast.Join();
    }

    void ThreadBroadcast()
    {
        while (threadBroadcastRunning == 1)
        {
            // sleep in 100ms steps and check if the thread is still running between sleeps
            for (int i = 0; i < Utility.DELAY_BROADCAST_PROCESS_START; i += 100)
            {
                Thread.Sleep(100);
                if (threadBroadcastRunning == 0)
                {
                    return;
                }
            }

            Broadcast();

            // sleep in 100ms steps and check if the thread is still running between sleeps
            for (int i = 0; i < Utility.DELAY_BROADCAST_PROCESS_STEP; i += 100)
            {
                Thread.Sleep(100);
                if (threadBroadcastRunning == 0)
                {
                    return;
                }
            }

            foreach (var (key, _) in clientIpToOnlineStates)
            {
                viewUser.Items.First(user => user.Ip == key).Online = clientIpToOnlineStates[key];
                clientIpToOnlineStates[key] = false;
            }
        }
    }

    public void Send(string ip, IContext context) => managerNetwork.Send(ip, context);

    public void Broadcast() => managerNetwork.Broadcast();

    public void Clear()
    {
    }

    public void OnDiscover(ContextDiscover context)
    {
        Console.WriteLine($"OnDiscover({context.Name})");

        if (viewUser.Items.Any(user => user.Ip == context.IP))
        {
            var user = viewUser.Items.First(user => user.Ip == context.IP);
            user.Name = context.Name;
            user.Online = true;

            clientIpToOnlineStates[context.IP] = true;
        }
        else
        {
            var userNew = new UserModel(context.IP, "dotnet_bot.png", context.Name) { Online = true };
            viewUser.Items.Add(userNew);

            clientIpToOnlineStates[context.IP] = true;
        }
    }

    public void OnSendProgress(ContextProgress context)
    {
        Console.WriteLine($"OnSendProgress({context.Percentage}, {context.Done})");
        EventsGUI.OnProgressSend(context);
    }

    public void OnReceiveText(ContextText context)
    {
        Console.WriteLine($"OnReceiveText({context.Text}, {context.DateTime})");

        var message = new MessageTextModel(GetUserByIP(context.IP).Name, context.DateTime, MessageState.SEEN, false,
                                           context.Text, context.GUID);
        GetUserByIP(context.IP).MessageView.Items.Add(message);
    }

    public void OnReceiveFileInfo(ContextFileInfo context)
    {
        Console.WriteLine($"OnReceiveFileInfo({context.Path}, {context.Size}, {context.DateTime})");

        var message = new MessageFileModel(GetUserByIP(context.IP).Name, context.DateTime, MessageState.SEEN, false,
                                           "dotnet_bot.png", context.Path, context.Size, context.GUID);
        GetUserByIP(context.IP).MessageView.Items.Add(message);
    }

    public void OnReceiveProgress(ContextProgress context)
    {
        Console.WriteLine($"OnReceiveProgress({context.Percentage}, {context.Done})");
        EventsGUI.OnProgressReceive(context);
    }

    public int GetUserSelectedIndex() => viewUser.Items.IndexOf(UserSelected);

    public ObservableCollection<MessageModel> GetUserSelectedItemsMessage()
    {
        var index = GetUserSelectedIndex();
        return index == -1 ? null : viewUser.Items[index].MessageView.Items;
    }

    public ObservableCollection<MessageFileModel> GetUserSelectedItemsFile()
    {
        var index = GetUserSelectedIndex();
        return index == -1 ? null : viewUser.Items[index].FileView.Items;
    }

    public UserModel GetUserSelected()
    {
        var index = GetUserSelectedIndex();
        return index == -1 ? null : viewUser.Items[index];
    }

    public UserModel GetUserByIP(string ip) => viewUser.Items.First(user => user.Ip == ip);

    public void AddUserSelectedMessage(MessageModel messageModel)
    {
        GetUserSelectedItemsMessage()?.Add(messageModel);
        viewUser.Items[GetUserSelectedIndex()].FireOnPropertyChangedMessageView();
    }

    public bool IsAnyUserSelected() => GetUserSelectedIndex() != -1;
}
}
