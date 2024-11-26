using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using StarCellar.Services.Apis;

namespace StarCellar.With.Apizr.Models
{
    public record User(Guid Id, string UserName, string FullName, string Email, int Age, string Role, string Address);

    public class UserMapper : Profile
    {
        public UserMapper()
        {
            CreateMap<UserDTO, User>();

            CreateMap<User, UserDTO>();
        }
    }
}
