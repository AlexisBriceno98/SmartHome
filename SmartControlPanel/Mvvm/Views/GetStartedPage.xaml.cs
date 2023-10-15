using SmartControlPanel.Mvvm.ViewModels;

namespace SmartControlPanel.Mvvm.Views;

public partial class GetStartedPage : ContentPage
{
    private GetStartedViewModel _viewModel;
    public GetStartedPage(GetStartedViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }

    private void DeviceSwitch_Toggled(object sender, ToggledEventArgs e)
    {
        _viewModel.ToggleFanState(e);
    }
}