using System.Runtime.InteropServices;
using System.Collections.ObjectModel;

using Cryssage.Views;
using Cryssage.Models;
using Cryssage.Resources;

using Networking;
using Networking.Manager;
using Networking.Context;
using Networking.Context.File;

namespace Cryssage
{
public partial class MainPage : ContentPage, IContextHandler
{
    [LibraryImport("User32.dll")]
    private static partial short GetAsyncKeyState(int vKey);

    UserView viewUser;
    readonly ManagerNetwork managerNetwork;

    bool editorSendFromReturn = false;

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

        var message = new MessageTextModel("", MessageType.TEXT, "Enemy", context.DateTime, MessageState.SEEN, false,
                                           context.Text);
        GetUserByIP(context.IP).MessageView.Items.Add(message);
    }

    public void OnReceiveFileInfo(ContextFileInfo context)
    {
        Console.WriteLine($"OnReceiveFileInfo({context.Name}, {context.Size}, {context.DateTime})");
    }

    public void OnReceiveProgress(ContextProgress context)
    {
        Console.WriteLine($"OnReceiveProgress({context.Percentage}, {context.Done})");
    }

    public MainPage(UserView uv)
    {
        managerNetwork = new(this);

        InitializeUI(uv);
    }

    void InitializeUI(UserView uv)
    {
        InitializeComponent();

        viewUser = uv;
        collectionViewUsers.ItemsSource = viewUser.Items;
    }

    void OnSelectionChangedCollectionViewUsers(object sender, SelectionChangedEventArgs e)
    {
        UpdateChatBackgroundMessage();

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

    void OnTextChangedEditor(object sender, TextChangedEventArgs e)
    {
        buttonSendRecordIcon.Glyph = editor.Text.Length > 0 ? FontIcons.Airplane : FontIcons.Microphone;

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
        AddUserSelectedMessage(message);

        managerNetwork.Send(GetUserSelected().Ip, new ContextText(message.Text, DateTime.UtcNow));
        UpdateChatBackgroundMessage();
    }

    void EditorReset()
    {
        editor.Text = "";
        buttonSendRecordIcon.Glyph = FontIcons.Microphone;
        editorSendFromReturn = false;
    }

    int GetUserSelectedIndex() => viewUser.Items.IndexOf((UserModel)collectionViewUsers.SelectedItem);

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

    UserModel GetUserByIP(string ip) => viewUser.Items.Where(user => user.Ip == ip).First();

    void AddUserSelectedMessage(MessageModel messageModel) => GetUserSelectedItems()?.Add(messageModel);

    void UpdateChatBackgroundMessage()
    {
        if (GetUserSelectedItems().Count > 0)
        {
            labelMessages.Text = "";
        }
        else
        {
            labelMessages.Text = string.Format(Strings.MessageUserSelectedMessagesNone, GetUserSelected().Name);
        }
    }

    bool IsAnyUserSelected() => GetUserSelectedIndex() != -1;
}
}
