using System.Runtime.InteropServices;

namespace Cryssage.Utility
{
public class EnvironmentEx
{
    [DllImport("shell32", CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = false)]
    private static extern string SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)] Guid rfid, uint dwFlags = 0,
                                                      nint hToken = 0);

    public enum KnownFolder : byte
    {
        Downloads,
        Documents
    }

    static readonly Dictionary<KnownFolder, Guid> _guids =
        new() { [KnownFolder.Downloads] = new("374DE290-123F-4565-9164-39C4925E467B"),
                [KnownFolder.Documents] = new("FDD39AD0-238F-46AF-ADB4-6C85480369C7") };

    public static string GetKnownFolder(KnownFolder knownFolder) => SHGetKnownFolderPath(_guids[knownFolder]);
}
}
