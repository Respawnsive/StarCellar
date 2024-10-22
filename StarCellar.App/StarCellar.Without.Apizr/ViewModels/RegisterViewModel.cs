using System.Text;
using CommunityToolkit.Maui.Core;
using MiniValidation;
using StarCellar.Without.Apizr.Services.Apis.User;
using StarCellar.Without.Apizr.Services.Apis.User.Dtos;
using StarCellar.Without.Apizr.Services.Navigation;
using StarCellar.Without.Apizr.Views;

namespace StarCellar.Without.Apizr.ViewModels
{
    public partial class RegisterViewModel : BaseViewModel
    {
        [ObservableProperty] private string _fullName;
        [ObservableProperty] private string _email;
        [ObservableProperty] private string _username;
        [ObservableProperty] private string _password;
        [ObservableProperty] private string _confirmPassword;

        private readonly IConnectivity _connectivity;
        private readonly IUserApi _userApi;
        private readonly ISecureStorage _secureStorage;

        /// <inheritdoc />
        public RegisterViewModel(INavigationService navigationService, 
            IConnectivity connectivity, 
            IUserApi userApi, 
            ISecureStorage secureStorage) : base(navigationService)
        {
            _connectivity = connectivity;
            _userApi = userApi;
            _secureStorage = secureStorage;
        }

        [RelayCommand]
        public async Task SaveAsync()
        {
            if (IsBusy)
                return;

            // Connectivity
            if (_connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await NavigationService.DisplayAlert("No connectivity!",
                    $"Please check internet and try again.", "OK");
                return;
            }

            // Validation
            var signUpRequest = new SignUpRequest
            {
                FullName = FullName,
                Email = Email,
                Username = Username,
                Password = Password,
                ConfirmPassword = ConfirmPassword,
                Role = "User"
            };
            if (!MiniValidator.TryValidate(signUpRequest, out var errors))
            {
                var sb = new StringBuilder();
                foreach (var entry in errors)
                {
                    sb.Append($"{entry.Key}:\n");
                    foreach (var error in entry.Value)
                        sb.Append($"  - {error}\n");
                }

                await NavigationService.DisplayAlert("Input error!", sb.ToString(), "OK");
                return;
            }

            // Requesting
            try
            {
                IsBusy = true;

                var user = await _userApi.SignUpAsync(signUpRequest);
                if (user.Id == default)
                {
                    await NavigationService.ShowToast("Unable to signup, please try again later.", ToastDuration.Long);
                    return;
                }

                var signInRequest = new SignInRequest()
                {
                    Login = Email,
                    Password = Password
                };

                var tokens = await _userApi.SignInAsync(signInRequest);
                if (string.IsNullOrEmpty(tokens.AccessToken) || string.IsNullOrWhiteSpace(tokens.RefreshToken))
                {
                    await NavigationService.ShowToast("Unable to signin, please try again later.", ToastDuration.Long);
                    return;
                }

                await _secureStorage.SetAsync(nameof(Tokens.AccessToken), tokens.AccessToken);
                await _secureStorage.SetAsync(nameof(Tokens.RefreshToken), tokens.RefreshToken);

                await NavigationService.GoToAsync($"//{nameof(CellarPage)}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to signin: {ex.Message}");
                await NavigationService.DisplayAlert("Error!", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
