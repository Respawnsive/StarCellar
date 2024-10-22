using StarCellar.Without.Apizr.ViewModels;

namespace StarCellar.Without.Apizr.Views;

public partial class RegisterPage : ContentPage
{
	public RegisterPage(RegisterViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
}