using Parser;
using Parser.Message;
using Parser.Message.Header;
using Parser.Message.Packet;

namespace Test.ParserTests
{
class MessageTests : ITests
{
    public override bool Test()
    {
        Modules.Enqueue(TestMessageManager);
        Modules.Enqueue(TestMessageConverter);

        return TestModules();
    }

    static bool TestMessageConverter()
    {
        var guid = Guid.NewGuid();

        HeaderMetadata headerMetadataStart = new(guid, Message.Type.PING, 69);
        var headerMetadataAsBytes = MessageConverter.HeaderMetadataToBytes(headerMetadataStart);
        var headerMetadataEnd = MessageConverter.BytesToHeaderMetadata(headerMetadataAsBytes);
        if (PrintModuleTest(headerMetadataStart == headerMetadataEnd, "HeaderMetadata"))
        {
            return false;
        }

        HeaderData headerDataStart = new(guid, 69);
        var headerDataAsBytes = MessageConverter.HeaderDataToBytes(headerDataStart);
        var headerDataEnd = MessageConverter.BytesToHeaderData(headerDataAsBytes);
        if (PrintModuleTest(headerDataStart == headerDataEnd, "HeaderData"))
        {
            return false;
        }

        PacketMetadata packetMetadataStart = new(headerMetadataStart);
        var packetMetadataAsBytes = MessageConverter.PacketMetadataToBytes(packetMetadataStart);
        var packetMetadataEnd = MessageConverter.BytesToPacketMetadata(packetMetadataAsBytes);
        if (PrintModuleTest(packetMetadataStart == packetMetadataEnd, "PacketMetadata"))
        {
            return false;
        }

        // clang-format off
        PacketData packetDataStart = new(headerDataStart, "69"u8.ToArray());
        // clang-format on
        var packetDataAsBytes = MessageConverter.PacketDataToBytes(packetDataStart);
        var packetDataEnd = MessageConverter.BytesToPacketData(packetDataAsBytes);
        if (PrintModuleTest(packetDataStart == packetDataEnd, "PacketData"))
        {
            return false;
        }

        byte[] bytes = new byte[1000000];
        for (int i = 0; i < bytes.Length; i++)
        {
            bytes[i] = (byte)i;
        }

        var packetDatasEnd = MessageConverter.BytesToPacketDatas(bytes);
        var packetDatasAsBytes = MessageConverter.PacketDatasToBytes(packetDatasEnd);

        if (PrintModuleTest(bytes != packetDatasAsBytes, "MessageConverter"))
        {
            return false;
        }

        return true;
    }

    static bool TestMessageManager()
    {
        var gu1d = Guid.NewGuid();

        byte[] dataStart = new byte[1000000];
        for (int i = 0; i < dataStart.Length; i++)
        {
            dataStart[i] = (byte)i;
        }

        var message = MessageManager.ToMessage(dataStart, Message.Type.TEXT, gu1d);
        var messageDisassembled = MessageManager.FromMessage(message);

        if (PrintModuleTest(message.PacketMetadata.Header.GUID == messageDisassembled.GUID &&
                                message.PacketMetadata.Header.Type == messageDisassembled.Type &&
                                messageDisassembled.Stream != null &&
                                dataStart.SequenceEqual(messageDisassembled.Stream),
                            "MessageManager"))
        {
            return false;
        }

        return true;
    }
}
}
