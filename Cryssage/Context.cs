using System.Collections.ObjectModel;

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

    public Context(UserModelView uv)
    {
        managerNetwork = new(this, new());
        viewUser = uv;

        var userNew = new UserModel("127.0.0.1", "dotnet_bot.png", "Pulea", DateTime.MinValue, "");
        var userNew1 = new UserModel("127.0.0.1", "dotnet_bot.png", "Pulea", DateTime.MinValue, "");
        viewUser.Items.Add(userNew);
        viewUser.Items.Add(userNew1);
    }

    public void Send(string ip, IContext context) => managerNetwork.Send(ip, context);

    public void OnDiscover(ContextDiscover context)
    {
        Console.WriteLine($"OnDiscover({context.Name})");

        if (viewUser.Items.Any(user => user.Ip == context.IP))
        {
            viewUser.Items.First(user => user.Ip == context.IP).Name = context.Name;
        }
        else
        {
            var userNew = new UserModel(context.IP, "dotnet_bot.png", context.Name, DateTime.MinValue, "");
            viewUser.Items.Add(userNew);
        }
    }

    public void OnSendProgress(ContextProgress context)
    {
        Console.WriteLine($"OnSendProgress({context.Percentage}, {context.Done})");
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

    public UserModel GetUserByIP(string ip) => viewUser.Items.Where(user => user.Ip == ip).First();

    public void AddUserSelectedMessage(MessageModel messageModel) => GetUserSelectedItemsMessage()?.Add(messageModel);

    public bool IsAnyUserSelected() => GetUserSelectedIndex() != -1;
}
}
