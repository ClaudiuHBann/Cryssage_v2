using System.Runtime.InteropServices;
using System.Collections.ObjectModel;

using Cryssage.Views;
using Cryssage.Models;
using Cryssage.Resources;

using Networking;
using Networking.Manager;
using Networking.Context;
using Networking.Context.File;

using Parser.Message;

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

        var message = new MessageTextModel(GetUserByIP(context.IP).Name, context.DateTime, MessageState.SEEN, false,
                                           context.Text, context.GUID);
        GetUserByIP(context.IP).MessageView.Items.Add(message);
    }

    public void OnReceiveFileInfo(ContextFileInfo context)
    {
        Console.WriteLine($"OnReceiveFileInfo({context.Name}, {context.Size}, {context.DateTime})");

        var message = new MessageFileModel(GetUserByIP(context.IP).Name, context.DateTime, MessageState.SEEN, false,
                                           "dotnet_bot.png", context.Name, context.Size, context.GUID);
        GetUserByIP(context.IP).MessageView.Items.Add(message);
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

        var userNew = new UserModel("127.0.0.1", "dotnet_bot.png", "Pulea", DateTime.MinValue, "");
        viewUser.Items.Add(userNew);
    }

    void OnSelectionChangedCollectionViewUsers(object sender, SelectionChangedEventArgs e)
    {
        UpdateChatBackgroundMessage();

        collectionViewMessages.ItemsSource = GetUserSelectedItemsMessage();

        var files = GetUserSelectedItemsFile();
        collectionViewFiles.ItemsSource = files;
        collectionViewFiles.IsVisible = files.Count > 0;
    }

    void OnClickButtonSendRecord(object sender, EventArgs e)
    {
        if (!IsAnyUserSelected())
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

    async void OnClickedImageButtonAttach(object sender, EventArgs e)
    {
        if (!IsAnyUserSelected())
        {
            return;
        }

        var files = await Picker.PickAnyFiles();
        foreach (var file in files)
        {
            var fileSize = (uint) new FileInfo(file.FullPath).Length;
            var contextFileInfo = new ContextFileInfo(file.FileName, fileSize);

            var message = new MessageFileModel(Environment.MachineName, DateTime.UtcNow, MessageState.SEEN, true,
                                               "dotnet_bot.png", file.FileName, fileSize, contextFileInfo.GUID);

            AddUserSelectedFile(message);
        }
    }

    async void OnClickedImageButtonDownload(object sender, EventArgs e)
    {
        var folderPath = await Picker.PickFolder();

        var messageModel = (MessageModel)((ImageButton)sender).BindingContext;
        managerNetwork.Send(GetUserSelected().Ip, new ContextRequest(Message.Type.FILE, messageModel.Guid));
    }

    void OnClickedButtonFileRemove(object sender, EventArgs e)
    {
        var messageFileModel = (MessageFileModel)((Button)sender).BindingContext;
        GetUserSelectedItemsFile().Remove(messageFileModel);
    }

    void EditorSend()
    {
        if (IsAnyUserSelected())
        {
            if (editor.Text != null && editor.Text.Trim(' ').Trim('\n').Trim('\r').Length > 0)
            {
                var contextText = new ContextText(editorSendFromReturn ? editor.Text[..^ 1] : editor.Text);
                var messageText = new MessageTextModel(Environment.MachineName, DateTime.UtcNow, MessageState.SEEN,
                                                       true, contextText.Text, contextText.GUID);

                AddUserSelectedMessage(messageText);
                managerNetwork.Send(GetUserSelected().Ip, contextText);
            }

            foreach (var file in GetUserSelectedItemsFile())
            {
                var messageFile = new MessageFileModel(Environment.MachineName, DateTime.UtcNow, MessageState.SEEN,
                                                       true, "dotnet_bot.png", file.Name, file.Size);

                AddUserSelectedMessage(messageFile);
                managerNetwork.Send(GetUserSelected().Ip, new ContextFileInfo(file.Name, file.Size, DateTime.UtcNow));
            }
            GetUserSelectedItemsFile().Clear();
            collectionViewFiles.IsVisible = false;
        }

        UpdateChatBackgroundMessage();
    }

    void EditorReset()
    {
        editor.Text = "";
        buttonSendRecordIcon.Glyph = FontIcons.Microphone;
        editorSendFromReturn = false;
    }

    int GetUserSelectedIndex() => viewUser.Items.IndexOf((UserModel)collectionViewUsers.SelectedItem);

    ObservableCollection<MessageModel> GetUserSelectedItemsMessage()
    {
        var index = GetUserSelectedIndex();
        return index == -1 ? null : viewUser.Items[index].MessageView.Items;
    }

    ObservableCollection<MessageFileModel> GetUserSelectedItemsFile()
    {
        var index = GetUserSelectedIndex();
        return index == -1 ? null : viewUser.Items[index].FileView.Items;
    }

    UserModel GetUserSelected()
    {
        var index = GetUserSelectedIndex();
        return index == -1 ? null : viewUser.Items[index];
    }

    UserModel GetUserByIP(string ip) => viewUser.Items.Where(user => user.Ip == ip).First();

    void AddUserSelectedMessage(MessageModel messageModel) => GetUserSelectedItemsMessage()?.Add(messageModel);

    void AddUserSelectedFile(MessageFileModel messageFileModel)
    {
        var files = GetUserSelectedItemsFile();
        if (files == null)
        {
            return;
        }

        files.Add(messageFileModel);
        collectionViewFiles.IsVisible = true;
    }

    void UpdateChatBackgroundMessage()
    {
        if (GetUserSelectedItemsMessage().Count > 0)
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
