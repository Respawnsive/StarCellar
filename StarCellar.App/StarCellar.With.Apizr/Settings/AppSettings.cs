namespace StarCellar.With.Apizr.Settings
{
    public class AppSettings
    {
        public string BaseAddress { get; set; }

        public string AuthToken
        {
            get => Preferences.Default.Get("AuthToken", string.Empty);
            set => Preferences.Default.Set("AuthToken", value);
        }
    }
}
