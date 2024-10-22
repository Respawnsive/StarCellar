using StarCellar.Without.Apizr.ViewModels;

namespace StarCellar.Without.Apizr.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}