using System.ComponentModel.DataAnnotations;

namespace StarCellar.With.Apizr.Services.Apis.User.Dtos
{
    public record SignInRequest
    {
        [Required]
        [EmailAddress]
        public string Login { get; set; }

        [Required]
        [DataType(DataType.Password), MinLength(8)]
        public string Password { get; set; }

        public TimeSpan? AccessTokenValidity { get; set; }
    }
}
