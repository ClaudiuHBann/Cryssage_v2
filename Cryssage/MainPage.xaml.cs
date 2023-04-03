using System.Collections.ObjectModel;
using System.Text;

using System.Net.Sockets;
using Networking.TCP.Server;
using Networking.TCP.Client;

using Parser.Message;

using Cryssage.Helpers;
using Cryssage.Models;
using Cryssage.Views;

namespace Cryssage
{
public partial class MainPage : ContentPage
{
    UserView viewUser;
    readonly TCPServerRaw server = new(6969);
    readonly TCPClient client = new();

#nullable enable
    void OnReceive(SocketError error, MessageDisassembled? messageDisassembled)
#nullable disable
    {
        if (messageDisassembled != null)
        {
            Console.WriteLine("Info:");
            Console.WriteLine("id: " + messageDisassembled.GUID.ToString());
            Console.WriteLine("type: " + messageDisassembled.Type.ToString());

            if (messageDisassembled.Stream != null)
            {
                Console.WriteLine("data: " + Encoding.Unicode.GetString(messageDisassembled.Stream));

                var messageAsString = Encoding.Unicode.GetString(messageDisassembled.Stream);

                MainThread.BeginInvokeOnMainThread(() =>
                                                   {
                                                       AddUserMessage(new MessageTextModel(
                                                           messageDisassembled.GUID.ToString(), MessageType.TEXT, "You",
                                                           DateTime.Now, MessageState.SEEN, false, messageAsString));

                                                       SetUserSelected(messageAsString);
                                                       SetUserSelected(DateTime.Now);

                                                       UpdateLabelMessages(false);
                                                   });
            }

            client.Receive(OnReceive);
        }
    }

    public MainPage(UserView uv)
    {
        Initialize(uv);
    }

    void Initialize(UserView uv)
    {
        InitializeComponent();

        viewUser = uv;

        collectionViewUsers.ItemsSource = viewUser.Items;

        // create and add yourself
        var userYou = new UserModel("Id", "dotnet_bot.png", "You", DateTime.MinValue, "");
        viewUser.Items.Add(userYou);

        server.Start((error, client) =>
                     {
                         if (error == SocketError.Success)
                         {
                             void callbackReceive(SocketError error, MessageDisassembled messageDisassembled)
                             {
                                 if (messageDisassembled != null && messageDisassembled.Stream != null)
                                 {
                                     client.Send(messageDisassembled.Stream, messageDisassembled.Type);
                                 }

                                 client.Receive(callbackReceive);
                             }

                             client.Receive(callbackReceive);
                         }
                     });
        client.Connect("127.0.0.1", 6969, (error, connected) => client.Receive(OnReceive));
    }

    void OnSelectionChangedCollectionViewUsers(object sender, SelectionChangedEventArgs e)
    {
        UpdateLabelMessages(false);

        collectionViewMessages.ItemsSource = GetUserSelectedItems();
    }

    void OnClickButtonSendRecord(object sender, EventArgs e)
    {
        if (!IsAnyUserSelected() || editor.Text.Length == 0)
        {
            return;
        }

        AddUserMessage(
            new MessageTextModel("", MessageType.TEXT, "You", DateTime.Now, MessageState.SEEN, true, editor.Text));

        // send message
        client.Send(Encoding.Unicode.GetBytes(editor.Text), Message.Type.TEXT);
        UpdateLabelMessages(true);

        // reset UI
        editor.Text = "";
        buttonSendRecordIcon.Glyph = HelperFontIcons.Microphone;
    }

    void OnTextChangedEditor(object sender, TextChangedEventArgs e)
    {
        buttonSendRecordIcon.Glyph =
            editor.Text.Length > 0 ? Helpers.HelperFontIcons.Airplane : Helpers.HelperFontIcons.Microphone;
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

    void SetUserSelected(string message)
    {
        var index = GetUserSelectedIndex();
        if (index == -1)
        {
            return;
        }

        viewUser.Items[index].Message = message;
    }

    void SetUserSelected(DateTime dateTime)
    {
        var index = GetUserSelectedIndex();
        if (index == -1)
        {
            return;
        }

        viewUser.Items[index].Time = dateTime;
    }

    void AddUserMessage(MessageModel messageModel)
    {
        var userSelectedItems = GetUserSelectedItems();
        userSelectedItems?.Add(messageModel);
    }

    void UpdateLabelMessages(bool anyway = false)
    {
        if (anyway || GetUserSelectedItems().Count > 0)
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
