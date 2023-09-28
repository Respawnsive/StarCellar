using System.ComponentModel.DataAnnotations;

namespace StarCellar.Api.Data
{
    public record UserRefreshToken
    {
        [Key]
        public Guid Id { get; init; }
        public Guid UserId { get; init; }
    }
}
