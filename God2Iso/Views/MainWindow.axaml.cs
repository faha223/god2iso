using Avalonia.ReactiveUI;
using ReactiveUI;
using Avalonia.Controls;
using God2Iso.ViewModels;

namespace God2Iso.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }
}