using StarCellar.Without.Apizr.ViewModels;

namespace StarCellar.Without.Apizr.Views;

public partial class ProfilePage : ContentPage
{
	public ProfilePage(ProfileViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
}