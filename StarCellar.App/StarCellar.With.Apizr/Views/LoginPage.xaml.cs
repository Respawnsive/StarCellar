using StarCellar.With.Apizr.ViewModels;

namespace StarCellar.With.Apizr.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}