using System.ComponentModel.DataAnnotations;

namespace StarCellar.Without.Apizr.Services.Apis.User.Dtos
{
    public record SignUpRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password), MinLength(8)]
        public string Password { get; set; }

        [Required]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }

        [Required]
        public string FullName { get; set; }

        public string Username { get; set; }
        public int Age { get; set; }

        [Required]
        public string Role { get; set; }

        public string Address { get; set; }
    }
}
