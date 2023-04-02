using Cryssage.Models;

namespace Cryssage.Generators
{
    public class GeneratorModels
    {
        public static UserModel CreateUserModel(string id, string avatar, string name, DateTime time, string message) =>
        new()
        {
            Id = id,
            Avatar = avatar,
            Name = name,
            Time = time,
            Message = message,
        };

        public static MessageFileModel CreateMessageFileModel(string id, MessageType type, string sender, DateTime timestamp, MessageState state, bool mine, string icon, string name, int size) =>
            new()
            {
                Id = id,
                Type = type,
                Sender = sender,
                Timestamp = timestamp,
                State = state,
                Mine = mine,
                Icon = icon,
                Name = name,
                Size = size
            };

        public static MessageTextModel CreateMessageTextModel(string id, MessageType type, string sender, DateTime timestamp, MessageState state, bool mine, string text) =>
            new()
            {
                Id = id,
                Type = type,
                Sender = sender,
                Timestamp = timestamp,
                State = state,
                Mine = mine,
                Text = text
            };

        public static UserModel CreateUserYou() => GeneratorModels.CreateUserModel("Id", "dotnet_bot.png", "You", DateTime.MinValue, "");
    }
}
