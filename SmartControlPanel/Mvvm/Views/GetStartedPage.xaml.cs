using SmartControlPanel.Mvvm.ViewModels;

namespace SmartControlPanel.Mvvm.Views;

public partial class GetStartedPage : ContentPage
{
    public GetStartedPage(GetStartedViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}