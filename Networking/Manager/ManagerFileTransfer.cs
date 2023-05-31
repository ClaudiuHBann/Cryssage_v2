using System.Collections.Concurrent;

using Networking.Context;
using Networking.Context.File;

namespace Networking.Manager
{
public class ManagerFileTransfer
{
    class FileInfo
    {
        public string Path { get; set; }
        public uint Size { get; set; }
        public uint Index { get; set; }
        public bool Request { get; set; }

        public FileInfo(string path, uint size, uint index, bool request)
        {
            Path = path;
            Size = size;
            Index = index;
            Request = request;
        }
    }

    // file guid and our file path (to read and write depends) with index and size
    readonly ConcurrentDictionary<Guid, FileInfo> fileGUIDToLocalFilePaths = new();

    public ManagerFileTransfer(List<ContextFileInfo> contextFileInfos) => contextFileInfos.ForEach(Add);

    public void Add(ContextFileInfo contextFileInfo)
    {
        fileGUIDToLocalFilePaths[contextFileInfo.GUID] =
            new(contextFileInfo.Path, contextFileInfo.Size, contextFileInfo.Size, false);
    }

    public void Add(ContextFileRequest contextRequestFile)
    {
        if (fileGUIDToLocalFilePaths.ContainsKey(contextRequestFile.GUID))
        {
            // send time or more when adding we refresh the existent fileInfo
            if (fileGUIDToLocalFilePaths[contextRequestFile.GUID].Request)
            {
                fileGUIDToLocalFilePaths[contextRequestFile.GUID] =
                    new(contextRequestFile.Path, contextRequestFile.Size, contextRequestFile.Index, true);
            }
            else
            {
                fileGUIDToLocalFilePaths[contextRequestFile.GUID].Index = contextRequestFile.Index;
            }
        }
        else
        {
            // first time when adding it's a request
            fileGUIDToLocalFilePaths[contextRequestFile.GUID] =
                new(contextRequestFile.Path, contextRequestFile.Size, contextRequestFile.Index, true);
        }
    }

    public bool Write(Guid fileGUID, byte[] stream)
    {
        // if the index is same as the size then we need to send EOS
        if (!fileGUIDToLocalFilePaths.ContainsKey(fileGUID) ||
            fileGUIDToLocalFilePaths[fileGUID].Index == fileGUIDToLocalFilePaths[fileGUID].Size)
        {
            return false;
        }

        // open file stream and write stream
        using var streamFile = new FileStream(fileGUIDToLocalFilePaths[fileGUID].Path, FileMode.Append);
        streamFile.Write(stream, 0, stream.Length);

        // update file index
        fileGUIDToLocalFilePaths[fileGUID].Index += (uint)stream.Length;

        return true;
    }

    public byte[]? Read(Guid fileGUID)
    {
        if (!fileGUIDToLocalFilePaths.ContainsKey(fileGUID) || !File.Exists(fileGUIDToLocalFilePaths[fileGUID].Path) ||
            fileGUIDToLocalFilePaths[fileGUID].Index == fileGUIDToLocalFilePaths[fileGUID].Size)
        {
            return null;
        }

        // open file stream and create buffer
        using var streamFile = new FileStream(fileGUIDToLocalFilePaths[fileGUID].Path, FileMode.Open, FileAccess.Read);
        var stream = new byte[Utility.FILE_CHUNK_SIZE];

        // read from file stream to the buffer and resize the buffer
        streamFile.Seek(fileGUIDToLocalFilePaths[fileGUID].Index, SeekOrigin.Begin);
        var bytesReadCount = streamFile.Read(stream, 0, stream.Length);
        Array.Resize(ref stream, bytesReadCount);

        // update the file index
        fileGUIDToLocalFilePaths[fileGUID].Index += (uint)bytesReadCount;

        return stream;
    }
}
}
