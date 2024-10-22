using StarCellar.Without.Apizr.Views;

namespace StarCellar.Without.Apizr;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(WineDetailsPage), typeof(WineDetailsPage));
        Routing.RegisterRoute(nameof(WineEditPage), typeof(WineEditPage));
        Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        SecureStorage.Default.RemoveAll();
        await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
    }
}