using StarCellar.With.Apizr.ViewModels;

namespace StarCellar.With.Apizr.Views;

public partial class CellarPage : ContentPage
{
	public CellarPage(CellarViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}

