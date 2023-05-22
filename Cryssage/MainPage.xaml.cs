using System.Runtime.InteropServices;
using System.Collections.ObjectModel;
using System.Text;

using System.Net.Sockets;
using Networking.TCP.Server;
using Networking.TCP.Client;

using Parser.Message;

using Cryssage.Helpers;
using Cryssage.Models;
using Cryssage.Views;
using Cryssage.Events;

using Networking;
using Networking.Context;
using Networking.Context.File;
using Networking.Manager;

namespace Cryssage
{
public partial class MainPage : ContentPage, IContextHandler
{
    UserView viewUser;

    readonly ManagerNetwork managerNetwork;
    readonly EventsUI eventsUI = new();

    public void OnDiscover(ContextDiscover context)
    {
        Console.WriteLine($"OnDiscover({context.Name})");

        if (viewUser.Items.Any(user => user.Ip == context.Ip))
        {
            viewUser.Items.First(user => user.Ip == context.Ip).Name = context.Name;
        }
        else
        {
            var userNew = new UserModel(context.Ip, "dotnet_bot.png", context.Name, DateTime.MinValue, "");
            viewUser.Items.Add(userNew);
        }
    }

    public void OnSendProgress(ContextProgress context)
    {
        Console.WriteLine($"OnSendProgress({context.Percentage}, {context.Done})");
    }

    public void OnReceiveText(ContextText context)
    {
        Console.WriteLine($"OnReceiveText({context.Text}, {context.Timestamp})");

        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(context.Timestamp).ToLocalTime();
        var message =
            new MessageTextModel("", MessageType.TEXT, "Enemy", dateTime, MessageState.SEEN, false, context.Text);
        AddUserMessage(message);
    }

    public void OnReceiveFileInfo(ContextFileInfo context)
    {
        Console.WriteLine($"OnReceiveFileInfo({context.Name}, {context.Size}, {context.Timestamp})");
    }

    public void OnReceiveProgress(ContextProgress context)
    {
        Console.WriteLine($"OnReceiveProgress({context.Percentage}, {context.Done})");
    }

    public MainPage(UserView uv)
    {
        managerNetwork = new(this);

        InitializeUI(uv);
        InitializeEventsUI();
    }

    void InitializeEventsUI()
    {
        eventsUI.OnMessageAdd += (message) =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
                                               {
                                                   AddUserMessage(message);

                                                   UpdateSelectedUserLastMessage(((MessageTextModel)message).Text,
                                                                                 DateTime.Now);
                                                   UpdateLabelMessages();
                                               });
        };
    }

    void InitializeUI(UserView uv)
    {
        InitializeComponent();

        viewUser = uv;
        collectionViewUsers.ItemsSource = viewUser.Items;
    }

    void OnSelectionChangedCollectionViewUsers(object sender, SelectionChangedEventArgs e)
    {
        UpdateLabelMessages();

        collectionViewMessages.ItemsSource = GetUserSelectedItems();
    }

    void OnClickButtonSendRecord(object sender, EventArgs e)
    {
        if (!IsAnyUserSelected() || editor.Text.Trim('\r').Length == 0)
        {
            return;
        }

        EditorSend();
        EditorReset();
    }

    [LibraryImport("User32.dll")]
    private static partial short GetAsyncKeyState(int vKey);

    bool editorSendFromReturn = false;

    void OnTextChangedEditor(object sender, TextChangedEventArgs e)
    {
        buttonSendRecordIcon.Glyph = editor.Text.Length > 0 ? HelperFontIcons.Airplane : HelperFontIcons.Microphone;

        const int VK_RETURN = 0x0D;
        var VK_RETURN_STATE = GetAsyncKeyState(VK_RETURN);
        if ((VK_RETURN_STATE & 1) != 1)
        {
            return;
        }

        const int VK_LSHIFT = 0xA0;
        var VK_LSHIFT_STATE = GetAsyncKeyState(VK_LSHIFT);
        const int VK_RSHIFT = 0xA1;
        var VK_RSHIFT_STATE = GetAsyncKeyState(VK_RSHIFT);
        if ((VK_LSHIFT_STATE & 1) == 1 || (VK_RSHIFT_STATE & 1) == 1)
        {
            return;
        }

        editorSendFromReturn = true;
        OnClickButtonSendRecord(sender, e);
    }

    void EditorSend()
    {
        var message = new MessageTextModel("", MessageType.TEXT, "You", DateTime.Now, MessageState.SEEN, true,
                                           editorSendFromReturn ? editor.Text[..^ 1] : editor.Text);
        AddUserMessage(message);

        managerNetwork.Send(GetUserSelected().Ip,
                            new ContextText(message.Text, (uint)((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds()));
        UpdateLabelMessages();
    }

    void EditorReset()
    {
        editor.Text = "";
        buttonSendRecordIcon.Glyph = HelperFontIcons.Microphone;
        editorSendFromReturn = false;
    }

    int GetUserSelectedIndex()
    {
        return viewUser.Items.IndexOf((UserModel)collectionViewUsers.SelectedItem);
    }

    ObservableCollection<MessageModel> GetUserSelectedItems()
    {
        var index = GetUserSelectedIndex();
        return index == -1 ? null : viewUser.Items[index].MessageView.Items;
    }

    UserModel GetUserSelected()
    {
        var index = GetUserSelectedIndex();
        return index == -1 ? null : viewUser.Items[index];
    }

    void UpdateSelectedUserLastMessage(string message, DateTime dateTime)
    {
        var index = GetUserSelectedIndex();
        if (index == -1)
        {
            return;
        }

        viewUser.Items[index].Message = message;
        viewUser.Items[index].Time = dateTime;
    }

    void AddUserMessage(MessageModel messageModel)
    {
        var userSelectedItems = GetUserSelectedItems();
        userSelectedItems?.Add(messageModel);
    }

    void UpdateLabelMessages()
    {
        if (GetUserSelectedItems().Count > 0)
        {
            labelMessages.Text = "";
        }
        else
        {
            labelMessages.Text = string.Format(HelperStrings.MessageUserSelectedMessagesNone, GetUserSelected().Name);
        }
    }

    bool IsAnyUserSelected()
    {
        return GetUserSelectedIndex() != -1;
    }
}
}
