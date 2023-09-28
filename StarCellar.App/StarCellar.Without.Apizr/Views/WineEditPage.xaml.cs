using StarCellar.Without.Apizr.ViewModels;

namespace StarCellar.Without.Apizr.Views;

public partial class WineEditPage : ContentPage
{
	public WineEditPage(WineEditViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}