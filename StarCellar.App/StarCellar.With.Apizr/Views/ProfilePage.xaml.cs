using StarCellar.With.Apizr.ViewModels;

namespace StarCellar.With.Apizr.Views;

public partial class ProfilePage : ContentPage
{
	public ProfilePage(ProfileViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
}