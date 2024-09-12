using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using ReactiveUI;

namespace God2Iso.Helpers;

public static class DialogHelpers
{
    static Window? AppMainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;

    public static async Task<string?> OpenFileDialog()
    {
        var mainWindow = AppMainWindow;
        if(mainWindow != null)
        {
            var options = new FilePickerOpenOptions()
            {
                AllowMultiple = false
            };
            var result = await mainWindow.StorageProvider.OpenFilePickerAsync(options);
            if (result != null && result.Count > 0)
                return result[0].Path.LocalPath;
        }

        return null;
    }

    public static async Task<string?> OpenFolderDialog()
    {
        var mainWindow = AppMainWindow;
        if(mainWindow != null)
        {
            var options = new FolderPickerOpenOptions()
            {
                AllowMultiple = false
            };
            var result = await mainWindow.StorageProvider.OpenFolderPickerAsync(options);
            return result[0].Path.LocalPath.ToString();
        }

        return null;
    }
}