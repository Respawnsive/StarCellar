using StarCellar.Without.Apizr.ViewModels;

namespace StarCellar.Without.Apizr.Views;

public partial class CellarPage : ContentPage
{
	public CellarPage(CellarViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}

