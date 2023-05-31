using CommunityToolkit.Maui.Storage;

namespace Cryssage
{
public class Picker
{
    public static async Task<IEnumerable<FileResult>> PickAnyFiles(string action = "send")
    {
        FilePickerFileType fileTypes = new(
            new Dictionary<DevicePlatform, IEnumerable<string>> { { DevicePlatform.iOS, Array.Empty<string>() },
                                                                  { DevicePlatform.WinUI, Array.Empty<string>() },
                                                                  { DevicePlatform.Tizen, Array.Empty<string>() },
                                                                  { DevicePlatform.tvOS, Array.Empty<string>() },
                                                                  { DevicePlatform.MacCatalyst, Array.Empty<string>() },
                                                                  { DevicePlatform.macOS, Array.Empty<string>() },
                                                                  { DevicePlatform.watchOS, Array.Empty<string>() },
                                                                  { DevicePlatform.Unknown, Array.Empty<string>() },
                                                                  { DevicePlatform.Android, Array.Empty<string>() } });

        PickOptions options = new() { PickerTitle = $"Please select a file to {action}", FileTypes = fileTypes };

        return await FilePicker.Default.PickMultipleAsync(options);
    }

    public static async Task<string> PickFolder()
    {
        var folder = await FolderPicker.Default.PickAsync(new());
        return folder?.Folder?.Path;
    }
}
}
