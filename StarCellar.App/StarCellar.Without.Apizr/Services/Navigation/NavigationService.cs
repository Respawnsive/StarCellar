using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace StarCellar.Without.Apizr.Services.Navigation;

public class NavigationService : INavigationService
{
    /// <inheritdoc />
    public Task GoToAsync(ShellNavigationState state) => Shell.Current.GoToAsync(state);

    /// <inheritdoc />
    public Task GoToAsync(ShellNavigationState state, bool animate) => Shell.Current.GoToAsync(state, animate);

    /// <inheritdoc />
    public Task GoToAsync(ShellNavigationState state, IDictionary<string, object> parameters) =>
        Shell.Current.GoToAsync(state, parameters);

    /// <inheritdoc />
    public Task GoToAsync(ShellNavigationState state, bool animate, IDictionary<string, object> parameters) =>
        Shell.Current.GoToAsync(state, animate, parameters);

    /// <inheritdoc />
    public Task<string> DisplayActionSheet(string title, string cancel, string destruction, params string[] buttons) =>
        Shell.Current.DisplayActionSheet(title, cancel, destruction, buttons);

    /// <inheritdoc />
    public Task<string> DisplayActionSheet(string title, string cancel, string destruction, FlowDirection flowDirection,
        params string[] buttons) =>
        Shell.Current.DisplayActionSheet(title, cancel, destruction, flowDirection, buttons);

    /// <inheritdoc />
    public Task DisplayAlert(string title, string message, string cancel) =>
        Shell.Current.DisplayAlert(title, message, cancel);

    /// <inheritdoc />
    public Task<bool> DisplayAlert(string title, string message, string accept, string cancel) =>
        Shell.Current.DisplayAlert(title, message, accept, cancel);

    /// <inheritdoc />
    public Task DisplayAlert(string title, string message, string cancel, FlowDirection flowDirection) =>
        Shell.Current.DisplayAlert(title, message, cancel, flowDirection);

    /// <inheritdoc />
    public Task<bool> DisplayAlert(string title, string message, string accept, string cancel,
        FlowDirection flowDirection) => Shell.Current.DisplayAlert(title, message, accept, cancel, flowDirection);

    /// <inheritdoc />
    public Task<string> DisplayPromptAsync(string title, string message, string accept = "OK", string cancel = "Cancel",
        string placeholder = null, int maxLength = -1, Keyboard keyboard = null, string initialValue = "") =>
        Shell.Current.DisplayPromptAsync(title, message, accept, cancel, placeholder, maxLength, keyboard,
            initialValue);

    /// <inheritdoc />
    public Task ShowToast(string message, ToastDuration duration = ToastDuration.Short, double textSize = AlertDefaults.FontSize,
        CancellationToken token = default) => Toast.Make(message, duration, textSize).Show(token);
}