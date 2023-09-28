using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace StarCellar.Api.Data
{
    public sealed class User : IdentityUser<Guid>
    {
        public string FullName { get; set; }
        public int Age { get; set; }
        public string Role { get; set; }
        public string Address { get; set; }
        public HashSet<Wine> Wines { get; set; }

        public User() { }

        public User(UserCreateDTO dto)
        {
            FullName = dto.FullName;
            Email = dto.Email;
            UserName = dto.Username;
            Age = dto.Age;
            Role = dto.Role;
            Address = dto.Address;
        }
    }

    public record UserDTO
    {
        public Guid Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Email { get; set; }

        public int Age { get; set; }

        [Required]
        public string Role { get; set; }

        public string Address { get; set; }

        public UserDTO() { }

        public UserDTO(User user)
        {
            Id = user.Id;
            FullName = user.FullName;
            Email = user.Email;
            UserName = user.UserName;
            Age = user.Age;
            Role = user.Role;
            Address = user.Address;
        }
    }

    public record UserCreateDTO
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

    public record UserLoginDTO
    {
        [Required]
        public string Login { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
