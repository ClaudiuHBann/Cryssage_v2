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

namespace Cryssage
{
public partial class MainPage : ContentPage
{
    UserView viewUser;

    readonly TCPServerRaw server = new(6969);
    readonly TCPClient client = new();

    readonly EventsNetworking eventsNetworking = new();
    readonly EventsUI eventsUI = new();

    public MainPage(UserView uv)
    {
        InitializeUI(uv);
        InitializeEvents();
        InitializeNetworking();
    }

    void InitializeEvents()
    {
        InitializeEventsNetworking();
        InitializeEventsUI();
    }

    void InitializeEventsNetworking()
    {
        eventsNetworking.OnMessageReceiveError += (error) =>
        {
#pragma warning disable CA1416 // Validate platform compatibility
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"OnMessageReceiveError({error})");
            Console.ForegroundColor = ConsoleColor.White;
#pragma warning restore CA1416 // Validate platform compatibility
        };

        eventsNetworking.OnMessageSendError += (error) =>
        {
#pragma warning disable CA1416 // Validate platform compatibility
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"OnMessageSendError({error})");
            Console.ForegroundColor = ConsoleColor.White;
#pragma warning restore CA1416 // Validate platform compatibility
        };

        eventsNetworking.OnMessageSend += (bytesTransferred) => Console.WriteLine($"OnMessageSend({bytesTransferred})");

        eventsNetworking.OnMessageReceive += (messageDisassembled) =>
        {
            Console.WriteLine(
                $"\nId: {messageDisassembled.GUID}, Type: {messageDisassembled.Type}, Data: {messageDisassembled.Stream.Length}");

            if (messageDisassembled.Stream != null)
            {
                var messageAsString = Encoding.Unicode.GetString(messageDisassembled.Stream);
                var userMessage = new MessageTextModel(messageDisassembled.GUID.ToString(), MessageType.TEXT, "You",
                                                       DateTime.Now, MessageState.SEEN, false, messageAsString);

                eventsUI.OnMessageAdd(userMessage);
            }

            Console.WriteLine();
        };
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

        for (int i = 0; i < 3; i++)
        {
            var userYou = new UserModel("Id", "dotnet_bot.png", "You", DateTime.MinValue, "");
            viewUser.Items.Add(userYou);
        }

        collectionViewUsers.ItemsSource = viewUser.Items;
    }

    void InitializeNetworking()
    {
        server.Start((error, client) =>
                     {
                         if (error != SocketError.Success)
                         {
                             return;
                         }

                         void CallbackReceive(SocketError error, MessageDisassembled messageDisassembled)
                         {
                             if (messageDisassembled != null && messageDisassembled.Stream != null)
                             {
                                 client.Send(messageDisassembled.Stream, messageDisassembled.Type);
                             }

                             client.Receive(CallbackReceive);
                         }

                         client.Receive(CallbackReceive);
                     });

        client.Connect("127.0.0.1", 6969,
                       (error, connected) =>
                       {
                           void CallbackReceive(SocketError error, MessageDisassembled messageDisassembled)
                           {
                               if (error != SocketError.Success)
                               {
                                   eventsNetworking.OnMessageReceiveError(error);
                               }
                               else
                               {
                                   if (messageDisassembled != null)
                                   {
                                       eventsNetworking.OnMessageReceive(messageDisassembled);
                                   }
                               }

                               client.Receive(CallbackReceive);
                           }

                           client.Receive(CallbackReceive);
                       });
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

        client.Send(Encoding.Unicode.GetBytes(message.Text), Message.Type.TEXT,
                    (error, bytesTransferred) =>
                    {
                        if (error != SocketError.Success)
                        {
                            eventsNetworking.OnMessageSendError(error);
                        }

                        if (bytesTransferred > 0)
                        {
                            eventsNetworking.OnMessageSend(bytesTransferred);
                        }
                    });
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
