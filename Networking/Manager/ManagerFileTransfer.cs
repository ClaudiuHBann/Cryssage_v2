using System.Collections.Concurrent;
using Networking.Context.File;

namespace Networking.Manager
{
public class ManagerFileTransfer
{
    // file guid and it's local file path with index
    readonly ConcurrentDictionary<Guid, (string filePath, uint fileSize, uint fileIndex)> fileGUIDToLocalFilePaths =
        new();

    public ManagerFileTransfer()
    {
    }

    public void Add(ContextFileRequest contextRequestFile) => fileGUIDToLocalFilePaths[contextRequestFile.GUID] =
        new(contextRequestFile.Path, contextRequestFile.Size, contextRequestFile.Index);

    void Remove(Guid fileGUID)
    {
        if (fileGUIDToLocalFilePaths.ContainsKey(fileGUID))
        {
            fileGUIDToLocalFilePaths.Remove(fileGUID, out _);
        }
    }

    public bool Write(Guid fileGUID, byte[] stream)
    {
        if (!fileGUIDToLocalFilePaths.ContainsKey(fileGUID) ||
            !File.Exists(fileGUIDToLocalFilePaths[fileGUID].filePath))
        {
            return false;
        }

        // open file stream and write stream
        using var streamFile = new FileStream(fileGUIDToLocalFilePaths[fileGUID].filePath, FileMode.Append);
        streamFile.Write(stream, 0, stream.Length);

        // update file index
        fileGUIDToLocalFilePaths[fileGUID] =
            new(fileGUIDToLocalFilePaths[fileGUID].filePath, fileGUIDToLocalFilePaths[fileGUID].fileSize,
                fileGUIDToLocalFilePaths[fileGUID].fileIndex + (uint)stream.Length);
        if (fileGUIDToLocalFilePaths[fileGUID].fileIndex == fileGUIDToLocalFilePaths[fileGUID].fileSize)
        {
            Remove(fileGUID);
        }

        return true;
    }

    public byte[]? Read(Guid fileGUID)
    {
        if (!fileGUIDToLocalFilePaths.ContainsKey(fileGUID) ||
            !File.Exists(fileGUIDToLocalFilePaths[fileGUID].filePath))
        {
            return null;
        }

        // open file stream and create buffer
        using var streamFile = new FileStream(fileGUIDToLocalFilePaths[fileGUID].filePath, FileMode.Open);
        var stream = new byte[Utility.FILE_CHUNK_SIZE];

        // read from file stream to the buffer and resize the buffer
        var bytesReadCount = streamFile.Read(stream, 0, stream.Length);
        Array.Resize(ref stream, bytesReadCount);

        // update the file index
        fileGUIDToLocalFilePaths[fileGUID] =
            new(fileGUIDToLocalFilePaths[fileGUID].filePath, fileGUIDToLocalFilePaths[fileGUID].fileSize,
                fileGUIDToLocalFilePaths[fileGUID].fileIndex + (uint)bytesReadCount);

        // file finished reading, remove from cache
        if (bytesReadCount == 0)
        {
            Remove(fileGUID);
        }
        return stream;
    }
}
}
