using StarCellar.With.Apizr.ViewModels;

namespace StarCellar.With.Apizr.Views;

public partial class WineDetailsPage : ContentPage
{
	public WineDetailsPage(WineDetailsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}