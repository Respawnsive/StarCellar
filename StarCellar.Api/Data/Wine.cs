using System.ComponentModel.DataAnnotations;

namespace StarCellar.Api.Data
{
    public record Wine
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public int Stock { get; set; }
        public int Score { get; set; }
        public Guid OwnerId { get; set; }
        public User Owner { get; set; }

        public Wine() { }
        public Wine(Guid id, string name, string description, string imageUrl, int stock, int score, Guid ownerId, User owner) =>
            (Id, Name, Description, ImageUrl, Stock, Score, OwnerId, Owner) = (id, name, description, imageUrl, stock, score, ownerId, owner);
    }

    public record WineDTO(Guid Id, string Name, string Description, string ImageUrl, int Stock, int Score,
        Guid OwnerId);
}