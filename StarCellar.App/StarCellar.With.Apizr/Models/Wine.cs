using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using StarCellar.Services.Apis;

namespace StarCellar.With.Apizr.Models
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
