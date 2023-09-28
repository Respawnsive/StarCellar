using StarCellar.Without.Apizr.Views;

namespace StarCellar.Without.Apizr;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(WineDetailsPage), typeof(WineDetailsPage));
        Routing.RegisterRoute(nameof(WineEditPage), typeof(WineEditPage));
    }
}