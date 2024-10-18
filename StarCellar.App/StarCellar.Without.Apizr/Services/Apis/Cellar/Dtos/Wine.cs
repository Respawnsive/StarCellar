using AutoMapper;

namespace StarCellar.Without.Apizr.Services.Apis.Cellar.Dtos
{
    public partial class Wine : ObservableObject
    {
        [ObservableProperty] public Guid _id;
        [ObservableProperty] public string _name;
        [ObservableProperty] public string _description;
        [ObservableProperty] public string _imageUrl;
        [ObservableProperty] public int _stock;
        [ObservableProperty] public int _score;
        [ObservableProperty] public Guid _ownerId;
        [ObservableProperty] public int _viewCount;
    }

    public record WineDTO(Guid Id, string Name, string Description, string ImageUrl, int Stock, int Score,
        Guid OwnerId);

    public class WineMapper : Profile
    {
        public WineMapper()
        {
            CreateMap<WineDTO, Wine>()
                .ForMember(dest => dest.ViewCount, opt => opt.Ignore());

            CreateMap<Wine, WineDTO>();
        }
    }
}
