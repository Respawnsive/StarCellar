using StarCellar.With.Apizr.ViewModels;

namespace StarCellar.With.Apizr.Views;

public partial class RegisterPage : ContentPage
{
	public RegisterPage(RegisterViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
}