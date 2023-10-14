using SmartControlPanel.Mvvm.ViewModels;

namespace SmartControlPanel.Mvvm.Views;

public partial class SettingsPage : ContentPage
{
	public SettingsPage(SettingsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}