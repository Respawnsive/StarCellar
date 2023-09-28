namespace StarCellar.Api.Utils
{
    internal static class Constants
    {
        internal const string DirectoryPath = "Uploads";

        internal static class Roles
        {
            internal const string Admin = nameof(Admin);
            internal const string User = nameof(User);
        }

        internal static class Policies
        {
            internal const string Admin = nameof(Admin);
            internal const string User = nameof(User);
            internal const string Any = nameof(Any);
        }
    }
}
