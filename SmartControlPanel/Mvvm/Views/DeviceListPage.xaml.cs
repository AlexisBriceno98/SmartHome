using SmartControlPanel.Mvvm.ViewModels;

namespace SmartControlPanel.Mvvm.Views;

public partial class DeviceListPage : ContentPage
{
	public DeviceListPage(DeviceListViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}