using System.Diagnostics;

using Parser.Message.Header;
using Parser.Message.Packet;

namespace Parser.Message
{
public class MessageConverter
{
    public static byte[] MessageToBytes(Message message)
    {
        var bytesPacketMetadata = PacketMetadataToBytes(message.PacketMetadata);
        var bytesPacketDatas = PacketDatasToBytes(message.PacketDatas);

        byte[] bytes = new byte[bytesPacketMetadata.Length + bytesPacketDatas.Length];
        bytesPacketMetadata.CopyTo(bytes, 0);
        bytesPacketDatas.CopyTo(bytes, bytesPacketMetadata.Length);

        return bytes;
    }

    public static Message BytesToMessage(byte[] bytes)
    {
        var packetMetadataAsBytesRange = 0..(int)HeaderMetadata.SIZE;
        var packetMetadataAsBytes = bytes[packetMetadataAsBytesRange];
        var packetMetadata = BytesToPacketMetadata(packetMetadataAsBytes);

        var packetDatasAsBytes = bytes[(int)HeaderMetadata.SIZE..];
        var packetDatas = BytesToPacketDatas(packetDatasAsBytes);

        return new(packetMetadata, packetDatas);
    }

    public static byte[] PacketMetadataToBytes(PacketMetadata packetMetadata)
    {
        return HeaderMetadataToBytes(packetMetadata.Header);
    }

    public static byte[] PacketDataToBytes(PacketData packetData)
    {
        byte[] packetDataAsBytes = new byte[packetData.Size];

        HeaderDataToBytes(packetData.Header).CopyTo(packetDataAsBytes, 0);
        packetData.Content.CopyTo(packetDataAsBytes, HeaderData.SIZE);

        return packetDataAsBytes;
    }

    public static PacketData BytesToPacketData(byte[] bytes)
    {
        Debug.Assert(bytes.Length <= PacketData.SIZE_MAX);

        var headerDataAsBytesRange = 0..(int)HeaderData.SIZE;
        var headerData = BytesToHeaderData(bytes[headerDataAsBytesRange]);

        var content = bytes[(int)HeaderData.SIZE..];

        return new PacketData(headerData, content);
    }

    public static List<PacketData> BytesToPacketDatas(byte[] bytes)
    {
        List<PacketData> packetDatas = new();

        var packetDatasCount = bytes.Length / PacketData.SIZE_MAX;
        for (long i = 0; i < packetDatasCount; i++)
        {
            var packetDataAsBytesRange = (int)(i * PacketData.SIZE_MAX)..(int)((i + 1) * PacketData.SIZE_MAX);
            var packetData = BytesToPacketData(bytes[packetDataAsBytesRange]);
            packetDatas.Add(packetData);
        }

        var packetDatasLastSize = bytes.Length % PacketData.SIZE_MAX;
        if (packetDatasLastSize > 0)
        {
            var packetData = BytesToPacketData(bytes[(int)(packetDatasCount * PacketData.SIZE_MAX)..]);
            packetDatas.Add(packetData);
        }

        return packetDatas;
    }

    public static PacketMetadata BytesToPacketMetadata(byte[] bytes)
    {
        return new PacketMetadata(BytesToHeaderMetadata(bytes));
    }

    public static byte[] PacketDatasToBytes(List<PacketData> packetDatas)
    {
        uint bytesSize = 0;
        packetDatas.ForEach(packetData => bytesSize += packetData.Size);
        byte[] bytes = new byte[bytesSize];

        for (int i = 0; i < packetDatas.Count; i++)
        {
            PacketDataToBytes(packetDatas[i]).CopyTo(bytes, i * PacketData.SIZE_MAX);
        }

        return bytes;
    }

    public static byte[] HeaderMetadataToBytes(HeaderMetadata headerMetadata)
    {
        byte[] headerAsBytes = new byte[HeaderMetadata.SIZE];

        headerMetadata.GUID.ToByteArray().CopyTo(headerAsBytes, 0);
        headerAsBytes[Utility.GUID_SIZE] = (byte)headerMetadata.Type;
        BitConverter.GetBytes(headerMetadata.Size).CopyTo(headerAsBytes, Utility.GUID_SIZE + sizeof(Message.Type));

        return headerAsBytes;
    }

    public static HeaderMetadata BytesToHeaderMetadata(byte[] bytes)
    {
        Debug.Assert(bytes.Length == HeaderMetadata.SIZE);

        var guidAsBytesRange = 0..(int)Utility.GUID_SIZE;
        var guid = new Guid(bytes[guidAsBytesRange]);

        Message.Type type = (Message.Type)bytes[Utility.GUID_SIZE];
        Debug.Assert(Message.Type.UNKNOWN < type && type < Message.Type.COUNT);

        var sizeAsBytesRange = (int)(Utility.GUID_SIZE + sizeof(Message.Type))..(int)HeaderMetadata.SIZE;
        var sizeAsBytes = bytes[sizeAsBytesRange];
        var size = BitConverter.ToUInt32(sizeAsBytes);

        return new HeaderMetadata(guid, type, size);
    }

    public static byte[] HeaderDataToBytes(HeaderData headerData)
    {
        byte[] headerAsBytes = new byte[HeaderData.SIZE];

        headerData.GUID.ToByteArray().CopyTo(headerAsBytes, 0);
        BitConverter.GetBytes(headerData.Index).CopyTo(headerAsBytes, Utility.GUID_SIZE);

        return headerAsBytes;
    }

    public static HeaderData BytesToHeaderData(byte[] bytes)
    {
        Debug.Assert(bytes.Length == HeaderData.SIZE);

        var guidAsBytesRange = 0..(int)Utility.GUID_SIZE;
        var guid = new Guid(bytes[guidAsBytesRange]);

        var indexAsBytesRange = (int)Utility.GUID_SIZE..(int)HeaderData.SIZE;
        var indexAsBytes = bytes[indexAsBytesRange];
        var index = BitConverter.ToUInt32(indexAsBytes);

        return new HeaderData(guid, index);
    }
}
}
