using System.Collections.Concurrent;
using System.Collections.ObjectModel;

using System.Net;
using Networking;
using Networking.Manager;

using Networking.Context;
using Networking.Context.Discover;
using Networking.Context.Interface;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Cryssage.Views;
using Cryssage.Models;
using Cryssage.Events;
using Cryssage.Utility;

namespace Cryssage
{
public class Context : IContextHandler
{
    const string ContextDirectory = "\\Cryssage\\";
    const string ContextFileName = "Context.json";

    class ContextHost
    {
        public string Name { get; set; } = Environment.MachineName;
        public string DefaultDownloadDirectory {
            get; set;
        } = EnvironmentEx.GetKnownFolder(EnvironmentEx.KnownFolder.Downloads);
    }

    readonly ManagerNetwork managerNetwork;

    UserModelView viewUser = new();
    ContextHost contextHost = new();
    public UserModel UserSelected { get; set; }

    public EventsGUI EventsGUI { get; } = new();

    readonly ConcurrentDictionary<string, bool> clientIpToOnlineStates = new();
    readonly Thread threadBroadcast;
    int threadBroadcastRunning = 1;

    public Context(UserModelView uv)
    {
        // load context (users and context host) and pass our context file info
        managerNetwork = new(this, UserAllLoad());
        // set context handler name for discovery protocol
        Name = contextHost.Name;
        // set remote items and set local view
        uv.Items = viewUser.Items;
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

        UserAllSave();
    }

    void ThreadBroadcast()
    {
        while (threadBroadcastRunning == 1)
        {
            // sleep in 100ms steps and check if the thread is still running between sleeps
            for (int i = 0; i < Networking.Utility.DELAY_BROADCAST_PROCESS_START; i += 100)
            {
                Thread.Sleep(100);
                if (threadBroadcastRunning == 0)
                {
                    return;
                }
            }

            Broadcast();

            // sleep in 100ms steps and check if the thread is still running between sleeps
            for (int i = 0; i < Networking.Utility.DELAY_BROADCAST_PROCESS_STEP; i += 100)
            {
                Thread.Sleep(100);
                if (threadBroadcastRunning == 0)
                {
                    return;
                }
            }

            foreach (var (key, _) in clientIpToOnlineStates)
            {
                var user = viewUser.Items.Where(user => user.Ip == key);
                if (user.Any())
                {
                    user.First().Online = clientIpToOnlineStates[key];
                }

                clientIpToOnlineStates[key] = false;
            }
        }
    }

    public void Send(string ip, IContext context) => managerNetwork.Send(ip, context);

    public void Broadcast() => managerNetwork.Broadcast();

    public void SetName(string name) => Name = contextHost.Name = name;

    public void SetDDD(string ddd) => contextHost.DefaultDownloadDirectory = ddd;

    public string GetDDD() => contextHost.DefaultDownloadDirectory;

    public void UserAllSave()
    {
        // create and check file path
        var folderPathDocuments = EnvironmentEx.GetKnownFolder(EnvironmentEx.KnownFolder.Documents);
        var filePathContextDirectory = folderPathDocuments + ContextDirectory;
        if (!File.Exists(filePathContextDirectory))
        {
            Directory.CreateDirectory(filePathContextDirectory);
        }

        // serialize context and write it
        JsonSerializerSettings settings = new() { TypeNameHandling = TypeNameHandling.Auto };
        var contextJSONAsBytes =
            JsonConvert.SerializeObject(new { contextHost, viewUser }, Formatting.Indented, settings);

        File.WriteAllBytes(filePathContextDirectory + ContextFileName,
                           Networking.Utility.ENCODING_DEFAULT.GetBytes(contextJSONAsBytes));
    }

    public List<ContextFileInfo> UserAllLoad()
    {
        // create and check file path
        var folderPathDocuments = EnvironmentEx.GetKnownFolder(EnvironmentEx.KnownFolder.Documents);
        var filePathContextFile = folderPathDocuments + ContextDirectory + ContextFileName;
        if (!File.Exists(filePathContextFile))
        {
            return new();
        }

        // get json to object
        var contextJSONAsBytes = File.ReadAllBytes(filePathContextFile);
        var contextJSONAsString = Networking.Utility.ENCODING_DEFAULT.GetString(contextJSONAsBytes);
        JsonSerializerSettings settings =
            new() { TypeNameHandling = TypeNameHandling.Auto, Converters = { new JsonConverterEx() } };
        var contextJSONAsJObject = JsonConvert.DeserializeObject<JObject>(contextJSONAsString, settings);

        // initialize objects and reset essential data
        contextHost =
            contextJSONAsJObject.HasValues
                ? contextJSONAsJObject[nameof(contextHost)].ToObject<ContextHost>(JsonSerializer.Create(settings))
                : new();
        viewUser = contextJSONAsJObject.HasValues
                       ? contextJSONAsJObject[nameof(viewUser)].ToObject<UserModelView>(JsonSerializer.Create(settings))
                       : new();

        // return all the users file messages that are mine
        return viewUser.Items.SelectMany(user => user.MessageView.Items)
            .Where(message => message.Type == MessageType.FILE && message.Mine)
            .Select(message =>
                    {
                        var messageFile = (MessageFileModel)message;
                        return new ContextFileInfo(messageFile.FilePath, messageFile.Size, messageFile.Timestamp,
                                                   messageFile.Guid);
                    })
            .ToList();
    }

    public void Clear() => viewUser.Items.Clear();

    public override void OnDiscover(ContextDiscover context)
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

    public override void OnSendProgress(ContextProgress context)
    {
        Console.WriteLine($"OnSendProgress({context.Percentage}, {context.Done})");
        EventsGUI.OnProgressSend(context);
    }

    public override void OnReceiveText(ContextText context)
    {
        Console.WriteLine($"OnReceiveText({context.Text}, {context.DateTime})");

        var message =
            new MessageTextModel(GetUserByIP(context.IP).Name, context.DateTime, false, context.Text, context.GUID);
        GetUserByIP(context.IP).MessageView.Items.Add(message);
        GetUserByIP(context.IP).FireOnPropertyChangedMessageView();
    }

    public override void OnReceiveFileInfo(ContextFileInfo context)
    {
        Console.WriteLine($"OnReceiveFileInfo({context.Path}, {context.Size}, {context.DateTime})");

        var message = new MessageFileModel(GetUserByIP(context.IP).Name, context.DateTime, false, "dotnet_bot.png",
                                           context.Path, context.Size, context.GUID);
        GetUserByIP(context.IP).MessageView.Items.Add(message);
        GetUserByIP(context.IP).FireOnPropertyChangedMessageView();
    }

    public override void OnReceiveProgress(ContextProgress context)
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
