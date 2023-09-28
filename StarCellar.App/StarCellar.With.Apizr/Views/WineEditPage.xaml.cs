using StarCellar.With.Apizr.ViewModels;

namespace StarCellar.With.Apizr.Views;

public partial class WineEditPage : ContentPage
{
	public WineEditPage(WineEditViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}