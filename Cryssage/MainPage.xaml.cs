using System.Runtime.InteropServices;

using Cryssage.Views;
using Cryssage.Models;
using Cryssage.Resources;

using Networking.Context;
using Networking.Context.File;

namespace Cryssage
{
public partial class MainPage : ContentPage
{
    [LibraryImport("User32.dll")]
    private static partial short GetAsyncKeyState(int vKey);

    public Context Context;

    public MainPage(UserModelView uv)
    {
        InitializeComponent();

        Context = new(uv);
        collectionViewUsers.ItemsSource = uv.Items;

        InitializeEventsGUI(uv);
    }

    public void Destructor() => Context.Destructor();

    void InitializeEventsGUI(UserModelView uv)
    {
        Context.EventsGUI.OnProgressSend += contextProgress =>
        {
            var user = uv.Items.First(user => user.Ip == contextProgress.IP);
            var messages = user.MessageView.Items.Where(messageFile => messageFile.Guid == contextProgress.GUID);
            if (!messages.Any())
            {
                return;
            }

            var messageFile = messages.First();
            if (messageFile.Type != MessageType.FILE)
            {
                return;
            }

            var messageFileModel = (MessageFileModel)messageFile;
            messageFileModel.Progress = contextProgress.Percentage;
            messageFileModel.ProgressStart = contextProgress.Done ? false : contextProgress.Percentage > 0;
        };

        Context.EventsGUI.OnProgressReceive += contextProgress =>
        {
            var user = uv.Items.First(user => user.Ip == contextProgress.IP);
            var messages = user.MessageView.Items.Where(messageFile => messageFile.Guid == contextProgress.GUID);
            if (!messages.Any())
            {
                return;
            }

            var messageFile = messages.First();
            if (messageFile.Type != MessageType.FILE)
            {
                return;
            }

            var messageFileModel = (MessageFileModel)messageFile;
            messageFileModel.Progress = contextProgress.Percentage;
            messageFileModel.ProgressStart = contextProgress.Done ? false : contextProgress.Percentage > 0;
        };
    }

    void OnSelectionChangedCollectionViewUsers(object sender, SelectionChangedEventArgs e)
    {
        Context.UserSelected = (UserModel)collectionViewUsers.SelectedItem;

        UpdateChatBackgroundMessage();

        collectionViewMessages.ItemsSource = Context.GetUserSelectedItemsMessage();

        var files = Context.GetUserSelectedItemsFile();
        collectionViewFiles.ItemsSource = files;
        collectionViewFiles.IsVisible = files.Count > 0;
    }

    void OnClickButtonSendRecord(object sender, EventArgs e)
    {
        if (!Context.IsAnyUserSelected())
        {
            return;
        }

        EditorSend();
        EditorReset();
    }

    void OnTextChangedEditor(object sender, TextChangedEventArgs e)
    {
        buttonSendRecordIcon.Glyph = editor.Text.Length > 0 ? FontIcons.Airplane : FontIcons.Microphone;

        const int VK_LSHIFT = 0xA0;
        var VK_LSHIFT_STATE = GetAsyncKeyState(VK_LSHIFT);
        const int VK_RSHIFT = 0xA1;
        var VK_RSHIFT_STATE = GetAsyncKeyState(VK_RSHIFT);
        if (VK_LSHIFT_STATE < 0 || VK_RSHIFT_STATE < 0)
        {
            return;
        }

        const int VK_RETURN = 0x0D;
        var VK_RETURN_STATE = GetAsyncKeyState(VK_RETURN);
        if (VK_RETURN_STATE < 0)
        {
            OnClickButtonSendRecord(sender, e);
        }
    }

    async void OnClickedImageButtonAttach(object sender, EventArgs e)
    {
        if (!Context.IsAnyUserSelected())
        {
            return;
        }

        var files = await Picker.PickAnyFiles();
        foreach (var file in files)
        {
            var fileSize = (uint) new FileInfo(file.FullPath).Length;
            var message = new MessageFileModel(Environment.MachineName, DateTime.UtcNow, MessageState.SEEN, true,
                                               "dotnet_bot.png", file.FullPath, fileSize, Guid.NewGuid());

            AddUserSelectedFile(message);
        }
    }

    async void OnClickedImageButtonDownload(object sender, EventArgs e)
    {
        var messageFile = (MessageFileModel)((ImageButton)sender).BindingContext;

        var pathFolder = await Picker.PickFolder();
        if (pathFolder == null)
        {
            return;
        }
        var pathFile = pathFolder + "\\" + Path.GetFileName(messageFile.FilePath);

        Context.Send(Context.GetUserSelected().Ip,
                     new ContextFileRequest(pathFile, messageFile.Size, messageFile.Guid));
    }

    void OnClickedButtonFileRemove(object sender, EventArgs e)
    {
        var messageFileModel = (MessageFileModel)((Button)sender).BindingContext;
        Context.GetUserSelectedItemsFile().Remove(messageFileModel);
        collectionViewFiles.IsVisible = Context.GetUserSelectedItemsFile().Count > 0;
        buttonSendRecordIcon.Glyph =
            Context.GetUserSelectedItemsFile().Count > 0 ? FontIcons.Airplane : FontIcons.Microphone;
    }

    void EditorSend()
    {
        if (!Context.IsAnyUserSelected())
        {
            return;
        }

        if (editor.Text != null)
        {
            var editorTextTrimmed = editor.Text.Trim(' ').Trim('\n').Trim('\r');
            if (editorTextTrimmed.Length > 0)
            {
                Console.WriteLine(editorTextTrimmed);
                var contextText = new ContextText(editorTextTrimmed);
                var messageText = new MessageTextModel(Environment.MachineName, DateTime.UtcNow, MessageState.SEEN,
                                                       true, contextText.Text, contextText.GUID);

                Context.AddUserSelectedMessage(messageText);
                Context.Send(Context.GetUserSelected().Ip, contextText);
            }
        }

        foreach (var file in Context.GetUserSelectedItemsFile())
        {
            var messageFile = new MessageFileModel(Environment.MachineName, DateTime.UtcNow, MessageState.SEEN, true,
                                                   "dotnet_bot.png", file.FilePath, file.Size);

            Context.AddUserSelectedMessage(messageFile);
            Context.Send(Context.GetUserSelected().Ip,
                         new ContextFileInfo(file.FilePath, file.Size, DateTime.UtcNow, messageFile.Guid));
        }

        UpdateChatBackgroundMessage();
        collectionViewMessages.ScrollTo(Context.GetUserSelectedItemsMessage().Count - 1);
    }

    void EditorReset()
    {
        editor.Text = "";
        buttonSendRecordIcon.Glyph = FontIcons.Microphone;

        Context.GetUserSelectedItemsFile().Clear();
        collectionViewFiles.IsVisible = false;
    }

    void AddUserSelectedFile(MessageFileModel messageFileModel)
    {
        var files = Context.GetUserSelectedItemsFile();
        if (files == null)
        {
            return;
        }

        files.Add(messageFileModel);
        collectionViewFiles.IsVisible = true;
        buttonSendRecordIcon.Glyph = FontIcons.Airplane;
    }

    void UpdateChatBackgroundMessage()
    {
        if (Context.GetUserSelectedItemsMessage().Count > 0)
        {
            labelMessages.Text = "";
        }
        else
        {
            labelMessages.Text = string.Format(Strings.MessageUserSelectedMessagesNone, Context.GetUserSelected().Name);
        }
    }

    void OnClickedMenuFlyoutItemPeersSearch(object sender, EventArgs e) => Context.Broadcast();

    async void OnClickedMenuFlyoutItemPeersClear(object sender, EventArgs e)
    {
        bool clear = await DisplayAlert(Strings.MessagePeersClearTitle, Strings.MessagePeersClearDescription,
                                        Strings.MessageYes, Strings.MessageNo);
        if (clear)
        {
            Context.Clear();
        }
    }

    async void OnClickedMenuFlyoutItemChangeName(object sender, EventArgs e)
    {
        var name = await DisplayPromptAsync(Strings.MessageChangeNameTitle, Strings.MessageChangeNameDescription);
    }

    async void OnClickedMenuFlyoutItemChangeDDD(object sender, EventArgs e)
    {
        var pathFolder = await Picker.PickFolder();
        if (pathFolder == null)
        {
            return;
        }
    }

    async void OnClickedMenuFlyoutItemAbout(object sender, EventArgs e) =>
        await DisplayAlert(Strings.MessageAboutTitle, Strings.MessageAboutDescription, Strings.MessageOK);
}
}
