using Avalonia.Threading;
using libGod2Iso;
using God2Iso.Helpers;

namespace God2Iso.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    GamesOnDemandConverter _converter = new()
    {
        OutputDirectory = Environment.CurrentDirectory
    };

    public List<string> Packages => new (_converter.Packages);

    private string? _selectedPackage = null;
    public string? SelectedPackage 
    {
        get => _selectedPackage;
        set
        {
            _selectedPackage = value;
            OnPropertyChanged(nameof(SelectedPackage));
        }
    }

    public string OutputDirectory
    {
        get => _converter.OutputDirectory;
        set
        {
            _converter.OutputDirectory = value;
            OnPropertyChanged(nameof(OutputDirectory));
        }
    }

    private float _packageProgress = 0.0f;
    public float PackageProgress
    {
        get => _packageProgress;
        set 
        {
            _packageProgress = value;
            OnPropertyChanged(nameof(PackageProgress));
        }
    }

    private float _overallProgress = 0.0f;
    public float OverallProgress
    {
        get => _overallProgress;
        set
        {
            _overallProgress = value;
            OnPropertyChanged(nameof(OverallProgress));
        }
    }

    private bool _enableUI = true;
    public bool EnableUI
    {
        get => _enableUI;
        set 
        {
            _enableUI = value;
            OnPropertyChanged(nameof(EnableUI));
        }
    }

    public bool CreateGoodIsoHeader
    {
        get => _converter.CreateGoodIsoHeader;
        set{
            _converter.CreateGoodIsoHeader = value;
            OnPropertyChanged(nameof(CreateGoodIsoHeader));
        }
    }

    public MainWindowViewModel()
    {
        _converter.ProgressChanged += Converter_ProgressChanged;
    }

    public void Converter_ProgressChanged(float packageProgress, float overallProgress)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            PackageProgress = packageProgress;
            OverallProgress = overallProgress;
        });
    }

    public async Task BrowseForPackage()
    {
        var result = await DialogHelpers.OpenFileDialog();
        if(result != null)
        {
            _converter.Packages.Add(result);
            OnPropertyChanged(nameof(Packages));
        }
    }

    public async Task BrowseForOutputDirectory()
    {
        var result = await DialogHelpers.OpenFolderDialog();
        if(result != null)
        {
            OutputDirectory = result;
        }
    }

    public Task ClearPackages()
    {
        _converter.Packages.Clear();
        OnPropertyChanged(nameof(Packages));

        return Task.CompletedTask;
    }

    public Task RemoveSelectedPackage()
    {
        if(SelectedPackage != null)
        {
            _converter.Packages.Remove(SelectedPackage);
            SelectedPackage = null;
            OnPropertyChanged(nameof(Packages));
        }
        return Task.CompletedTask;
    }

    public async Task ProcessPackages()
    {
        EnableUI = false;

        await Task.Run(() =>
        {
            _converter.ConvertPackages();
        }).ConfigureAwait(true);

        EnableUI = true;
    }
}